using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace csharp_gomoku {

    /// <summary>
    /// IEngine implementation, requires a 'GUI' class instance to send its recommended move to.
    /// </summary>
    public partial class MyGreatEngine : IEngine {

        private int moveCount; //to check for draw and determine whose move it is
        private ulong currentHash;
        private byte[,] board;
        private byte[,] considered; //the considered moves will always be within 2 "chess king moves" from any placed stone
        //considered squares take up a Byte rather than Bool because we want to be able to decrement; they fit a Byte because they're at most 5*5

        private ACAutomaton aca;
        private Zobrist zob;        
        private TransposTable tt; 

        private GUI gui;

        private Random rng; //for move generation shuffling

        //indexer to allow for padding to be transparent if desired
        public byte this[int x, int y] {
            get { return board[x + 4, y + 4]; }
            private set { board[x + 4, y + 4] = value; }
        }

        public MyGreatEngine(GUI g) {
            aca = new ACAutomaton();
            zob = new Zobrist();
            tt = new TransposTable(24);
            gui = g;
            Reset();
            rng = new Random();
        }

        /// <summary>
        /// Iterates over moves which are valid and "considered" on the current board.
        /// </summary>
        private IEnumerable<Square> GenerateMoves() {
            for (int i = 2; i < Gamestate.sizeX + 2; i++) {
                for (int j = 2; j < Gamestate.sizeY + 2; j++) {
                    if ((considered[i, j] > 0) && (this[i - 2, j - 2] == 0)) yield return new Square(i - 2, j - 2);
                }
            }
        }

        /// <summary>
        /// Apply the 'Fisher-Yates shuffle' algo on the 'GenerateMoves' enumeration.
        /// </summary>
        /// <param name="bestMove">Returns this parameter first, regardless of the generated moves.</param>
        private IEnumerable<Square> GenerateShuffledMoves(Square bestMove) {
            //first return the param if it's legal
            int x = bestMove.x;
            int y = bestMove.y;
            if (this[x, y] == 0) yield return new Square(x, y);

            var tempList = GenerateMoves().ToList();
            int j;
            for (int i = 0; i < tempList.Count; i++) {
                j = rng.Next(i, tempList.Count);
                yield return tempList[j];
                tempList[j] = tempList[i];
            }
        }

        /// <summary>
        /// Send the latest "best move" to the GUI.
        /// </summary>
        private void UpdateGui(int depth, int score) {
            MoveReport mr = new MoveReport(bestMove, depth, score, currentHeuristicScore);
            if (!stopSearch) gui.DisplayEngineMove(mr); //calling this after search is stopped leads to a deadlock situation because the GUI thread has called Join on this thread
        }

        #region Interface implementation

        Square bestMove;
        Thread t;
        bool stopSearch; //for telling the worker thread to stop and return
        bool enableNewSearches;
        int currentHeuristicScore;

        public void Reset() {

            StopThink();

            board = new byte[Gamestate.sizeX + 8, Gamestate.sizeY + 8]; //the 8 extra spaces are padding for victory checking (+-4)
            considered = new byte[Gamestate.sizeX + 4, Gamestate.sizeY + 4]; // the 4 extra are padding for consideration marking (+-2)
            moveCount = 0;
            aca.Reset(true); //black goes first
            currentHash = 0;
        }

        //before every move update, the search must be paused (returned to root node) because otherwise the engine's internal board and/or search algo would crash and burn

        public void DoMove(Square sq) {
            PauseThink();
            incrementBoard(sq, moveCount % 2 == 0); //mark the move from outside
            ResumeThink();
        }

        public void UndoMove(Square sq) {
            PauseThink();
            decrementBoard(sq);
            ResumeThink();
        }        

        public void StartThink() {
            enableNewSearches = true;
            ResumeThink();
        }

        /// <summary>
        /// Stop search and prevent its resuming after move updates, until StartThink is called.
        /// </summary>
        public void StopThink() {
            PauseThink();
            enableNewSearches = false;
        }

        #endregion

        /// <summary>
        /// As opposed to StopThink, this only pauses the search for the internal board to get "synchronized".
        /// </summary>
        private void PauseThink() {
            stopSearch = true;
            if (null != t) t.Join();
        }

        /// <summary>
        /// Only start thinking if "pause" is on.
        /// </summary>
        private void ResumeThink() {
            if (stopSearch && enableNewSearches) { //only start if previous search has stopped and new search is allowed
                currentHeuristicScore = evaluate();

                stopSearch = false;
                t = new Thread(
                  () => IterativeSearch()
                  );
                t.Start();
            }
        }

        public void IterativeSearch() {
            int depth = 1;
            int score;
            while (!stopSearch) {
                score = DepthLimitedSearch(depth);
                UpdateGui(depth, score);
                depth++;
            }
            return;
        }

        /// <summary>
        /// Assigns to "bestMove" the result of a search of fixed max depth and returns its score.
        /// </summary>
        /// <param name="depth">Depth must be greater than 0.</param>
        public int DepthLimitedSearch(int depth) {
            //it is like a negamax step but we work with the 'bestMove' variable and disregard terminal state and omit TT writing

            int color;
            if (moveCount % 2 == 0) color = 1;
            else color = -1;

            int value = -123456789;
            int alpha = -123456789;
            int beta = 123456789;

            //TT lookup
            TTEntry ttEntry;
            if (tt.TryGetValue(currentHash, out ttEntry)) {
                if (ttEntry.Depth >= depth) {
                    bestMove = ttEntry.BestMove; //as opposed to "inside the negamax", we only rewrite bestMove with a "deeper" recorded value!
                    switch (ttEntry.Flag) {
                        case TTFlag.Exact:
                            return ttEntry.Score;
                        case TTFlag.Upper:
                            beta = ttEntry.Score;
                            break;
                        case TTFlag.Lower:
                            alpha = ttEntry.Score;
                            break;
                        default:
                            break;
                    }
                }
            }

            int candidateValue;
            foreach (Square child in GenerateShuffledMoves(bestMove)) {

                incrementBoard(child, color == 1);
                candidateValue = -negamax(child, depth - 1, -color, -beta, -alpha);
                decrementBoard(child);

                if (stopSearch) break; // do not overwrite bestMove with a result that came from an interrupted branch

                if (candidateValue > value) {
                    value = candidateValue;
                    if (value > alpha) alpha = value;
                    bestMove = child;
                }

            }

            //first move of the game goes in the middle, there are no 'considered moves' in such case anyway.
            if (moveCount == 0) {
                bestMove = new Square(Gamestate.sizeX / 2, Gamestate.sizeY / 2);
                return 0;
            }

            return value;
        }


        /// <summary>
        /// Write a move into the internal board. Expand the area of considered moves.
        /// The coordinate paramteres must be ACTUAL legal board coords (padding handled inside).
        /// </summary>
        /// <param name="black">True if the stone to be placed is black.</param>
        private void incrementBoard(Square move, bool black) {
            byte color; //derive board value from the boolean parameter 'black'
            int boardX = move.x + 4; //to accomodate board padding
            int boardY = move.y + 4;
            int consX = move.x + 2; //to accomodate consideration padding
            int consY = move.y + 2;

            //update the stone
            if (black) color = 2;
            else color = 1;
            board[boardX, boardY] = color;


            //update considered moves - enable/increment the 5x5 area around the new move
            for (int i = -2; i <= 2; i++) {
                for (int j = -2; j <= 2; j++) {
                    considered[consX + i, consY + j]++;
                }
            }

            moveCount++;

            //update zobhash
            currentHash = currentHash ^ zob.Table[move.x, move.y, color - 1];
        }

        /// <summary>
        /// Check if there's a 5-in-a-row on the current board containing the given square for the given player.
        /// </summary>
        private bool checkWin(Square move, bool black) {
            byte color;
            if (black) color = 2;
            else color = 1;

            int boardX = move.x + 4; //to accomodate board padding
            int boardY = move.y + 4;

            //copypasta & padding usage to optimize compared to how Gamestate checks for win           
            //horizontal
            int counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX + i, boardY] == color) counter++;
                else counter = 0;
                if (counter == 5) return true;
            }
            //vertical
            counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX, boardY + i] == color) counter++;
                else counter = 0;
                if (counter == 5) return true;
            }
            //asc diagonal
            counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX + i, boardY + i] == color) counter++;
                else counter = 0;
                if (counter == 5) return true;
            }
            //desc diagonal
            counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX + i, boardY - i] == color) counter++;
                else counter = 0;
                if (counter == 5) return true;
            }

            return false;
        }

        /// <summary>
        /// Clear a move from the internal board, remove its neighboring area from considered moves.
        /// </summary>
        private void decrementBoard(Square move) {
            int consX = move.x + 2;
            int consY = move.y + 2;

            //update hash
            currentHash = currentHash ^ zob.Table[move.x, move.y, this[move.x, move.y] - 1];

            //remove the stone
            board[move.x + 4, move.y + 4] = 0; //+4 padding

            //update considered moves - disable/decrement the 5x5 area around the removed move
            for (int i = -2; i <= 2; i++) {
                for (int j = -2; j <= 2; j++) {
                    considered[consX + i, consY + j]--;
                }
            }

            moveCount--;
        }

        /// <summary>
        /// Recursively calculates the value of the current position, up to the given depth.
        /// </summary>
        /// <param name="move">The last move. Win is checked in its neighborhood.</param>
        /// <param name="depth">The remaining available depth.</param>
        /// <param name="color">The color of the player to move. 1 ~ black, -1 ~ white.</param>
        /// <param name="alpha">The lower boundary on "interesting" scores.</param>
        /// <param name="beta">The upper boundary on "interesting" scores.</param>
        private int negamax(Square move, int depth, int color, int alpha, int beta) {

            Square localBestMove = move;
            int origAlpha = alpha;

            //TT lookup
            TTEntry ttEntry;
            if (tt.TryGetValue(currentHash, out ttEntry)) {
                localBestMove = ttEntry.BestMove; //for ordering purposes, just take the best move regardless of depth. For the rest, depth does matter.
                if (ttEntry.Depth >= depth) {
                    switch (ttEntry.Flag) {
                        case TTFlag.Exact:
                            return ttEntry.Score;
                        case TTFlag.Upper:
                            if (ttEntry.Score < beta) beta = ttEntry.Score;
                            break;
                        case TTFlag.Lower:
                            if (ttEntry.Score > alpha) alpha = ttEntry.Score;
                            break;
                        default:
                            break;
                    }
                    if (alpha >= beta) {
                        return ttEntry.Score;
                    }
                }
            }

            //if state is terminal, return:
            if (checkWin(move, color == -1)) {
                return -12345678; //someone won, so the score [calculated always for black] is +inf * (previous color). We return (color to move)*(score for black) --> always -inf
            }
            if (moveCount >= Gamestate.maxMoves) {
                return 0; //draw, return 0
            }
            if (depth <= 0) {
                int result = color * evaluate(); //not a win, not a draw, but reached maximum depth (0 remaining depth), return evaluation
                return result;
            }

            //otherwise keep searching children
            int value = -12345678;
            int candidateValue;
            foreach (Square child in GenerateShuffledMoves(localBestMove)) {
                if (stopSearch) return value; //do not visit any more children if search is stopped

                incrementBoard(child, color == 1);
                candidateValue = -negamax(child, depth - 1, -color, -beta, -alpha);
                decrementBoard(child);

                if (candidateValue > value) {
                    value = candidateValue;
                    localBestMove = child;
                }
                if (value > alpha) alpha = value;
                if (alpha >= beta) break;
            }

            if (stopSearch) return value;//do not update TT if search is stopped

            //TT update
            TTFlag flag;
            if (value <= origAlpha) flag = TTFlag.Upper;
            else if (value >= beta) flag = TTFlag.Lower;
            else flag = TTFlag.Exact;
            ttEntry = new TTEntry(currentHash, localBestMove, depth, value, flag);
            tt.Write(ttEntry);

            return value;
        }
    }
}
