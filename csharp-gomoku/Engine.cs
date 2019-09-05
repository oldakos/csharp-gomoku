using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace csharp_gomoku {

    /// <summary>
    /// Note: Breaks down if asked to play from a terminal state :)
    /// </summary>
    public partial class MyGreatEngine : IEngine {

        private int moveCount; //to check for draw and determine whose move it is
        private ulong currentHash;
        private byte[,] board;
        private byte[,] considered; //the considered moves will always be within 3 "chess king moves" from any placed stone
        //considered squares take up a Byte rather than Bool to be able to decrement; they fit a Byte because they're at most 7*7

        private ACAutomaton aca;
        private Zobrist zob; //        
        private TransposTable tt; //transposition table

        //indexer to allow for padding to be somewhat transparent
        public byte this[int x, int y] {
            get { return board[x + 4, y + 4]; }
            private set { board[x + 4, y + 4] = value; }
        }

        public MyGreatEngine() {
            aca = new ACAutomaton();
            zob = new Zobrist();
            tt = new TransposTable(24);
            Reset();
        }

        /// <summary>
        /// Iterates over valid and "considered" moves on the current board. The parameter is returned first.
        /// </summary>
        private IEnumerable<Square> GenerateMoves(Square bestMove) {
            //first suggest the best move from earlier iterations
            int x = bestMove.x;
            int y = bestMove.y;
            if (board[x + 4, y + 4] == 0) yield return new Square(x, y);

            //then loop over the rest of moves
            for (int i = 3; i < Gamestate.sizeX + 3; i++) {
                for (int j = 3; j < Gamestate.sizeY + 3; j++) {
                    if ((i == 3 + x) && (j == 3 + y)) continue;
                    if ((considered[i, j] > 0) && (board[i + 1, j + 1] == 0)) yield return new Square(i - 3, j - 3);
                }
            }
        }

        #region Interface implementation

        public void Reset() {
            board = new byte[Gamestate.sizeX + 8, Gamestate.sizeY + 8]; //the 8 extra spaces are padding for victory checking (+-4)
            considered = new byte[Gamestate.sizeX + 6, Gamestate.sizeY + 6]; // the 6 extra are padding for consideration marking (+-3)
            moveCount = 0;
            aca.Reset(true);
            currentHash = 0;
        }

        public void EnemyMove(Square sq) {
            
            if ((null != t) && (t.IsAlive)) {   //wrap up the pondering if it is running
                stopSearch = true;
                t.Join();
            }
            incrementBoard(sq, moveCount % 2 == 0); //mark the move from outside

            stopSearch = false; //start the search
            t = new Thread(
                () => IterativeSearch(false)
                );
            t.Start();
        }

        public void UndoMove(Square sq) {
            decrementBoard(sq);
        }

        public void ResetPosition(Gamestate gs) {

            throw new NotImplementedException(); // not yet needed for anything, will just do undo/reset

        }

        #region Time limited

        Square bestMove; //the move to calculate first in iterative deepening
        Thread t;       //we will use a separate thread to do time-limited thinking as well as 'pondering' during enemy move
        bool stopSearch;

        public void StartMove() {            
            //we've actually started the search when we were notified of the enemy's move
        }

        public Square EndMove() {
            stopSearch = true;
            t.Join();   //wrap up the current search
            Square output = bestMove;
            stopSearch = false;
            t = new Thread(
                () => IterativeSearch(true)
                );
            t.Start(); //start an indefinite search from the resulting position (for the enemy) to populate TT
            return output;
        }

        public void IterativeSearch(bool justPonder) {
            int depth = 1;
            while (!stopSearch) {
                bestMove = DepthLimitedSearch(depth);
                depth++;
            }
            //if we're being asked to play, we will increment, but if only pondering, the increment comes from outside
            if (!justPonder) incrementBoard(bestMove, moveCount % 2 == 0);
            return;
        }

        #endregion

        /// <summary>
        /// Returns "best move"
        /// </summary>
        /// <param name="depth">Depth must be greater than 0.</param>
        public Square DepthLimitedSearch(int depth) {

            if (depth < 1) throw new Exception("The engine cannot suggest a move with search depth 0 or less");

            int color;
            int value = -123456789;

            if (moveCount % 2 == 0) color = 1;
            else color = -1;
            int alpha = -123456789;

            int candidateValue;
            foreach (Square child in GenerateMoves(bestMove)) {
                incrementBoard(child, color == 1);
                candidateValue = -negamax(child, depth - 1, -color, -123456789, -alpha);
                decrementBoard(child);
                if (stopSearch) break;
                if (candidateValue > value) {
                    value = candidateValue;
                    if (value > alpha) alpha = value;
                    bestMove = child;
                }

            }

            //first move of the game goes in the middle, there are no 'considered moves' in such case anyway.
            if (moveCount == 0) {
                bestMove = new Square(Gamestate.sizeX / 2, Gamestate.sizeY / 2);
            }

            return bestMove;
        }

        #endregion

        /// <summary>
        /// Write a move into the internal board. Expand the area of considered moves.
        /// The coordinate paramteres must be ACTUAL legal board coords (padding handled inside).
        /// </summary>
        /// <param name="black">True if the stone to be placed is black.</param>
        private void incrementBoard(Square move, bool black) {
            byte color; //derive board value from the boolean parameter 'black'
            int boardX = move.x + 4; //to accomodate board padding
            int boardY = move.y + 4;
            int consX = move.x + 3; //to accomodate consideration padding
            int consY = move.y + 3;

            //update the stone
            if (black) color = 2;
            else color = 1;
            board[boardX, boardY] = color;


            //update considered moves - enable/increment the 7x7 area around the new move
            for (int i = -3; i <= 3; i++) {
                for (int j = -3; j <= 3; j++) {
                    considered[consX + i, consY + j]++;
                }
            }

            //inc moveCount
            moveCount++;

            //update hash
            currentHash = currentHash ^ zob.Table[move.x, move.y, color - 1];
        }

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
            int consX = move.x + 3;
            int consY = move.y + 3;

            //update hash
            currentHash = currentHash ^ zob.Table[move.x, move.y, this[move.x, move.y] - 1];

            //remove the stone
            board[move.x + 4, move.y + 4] = 0; //+4 padding

            //update considered moves - disable/decrement the 7x7 area around the removed move
            for (int i = -3; i <= 3; i++) {
                for (int j = -3; j <= 3; j++) {
                    considered[consX + i, consY + j]--;
                }
            }

            //dec moveCount
            moveCount--;


        }

        /// <summary>
        /// Recursively calculates the value of the current position, up to the given depth.
        /// </summary>
        /// <param name="move">The last move. Win is checked in its neighborhood.z</param>
        /// <param name="depth">The remaining depth.</param>
        /// <param name="color">The color of the player to move. 1 ~ black, -1 ~ white.</param>
        /// <param name="alpha">The lower boundary on "interesting" scores.</param>
        /// <param name="beta">The upper boundary on "interesting" scores.</param>
        private int negamax(Square move, int depth, int color, int alpha, int beta) {

            Square bestMove = move;

            //TT lookup
            TTEntry ttEntry;
            if (tt.TryGetValue(currentHash, out ttEntry)) {
                bestMove = ttEntry.BestMove; //for ordering purposes, just take the best move regardless of depth. For the rest, depth does matter.
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
            int origAlpha = alpha; //used 

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
            foreach (Square child in GenerateMoves(bestMove)) {
                if (stopSearch) return value;

                incrementBoard(child, color == 1);
                candidateValue = -negamax(child, depth - 1, -color, -beta, -alpha);
                decrementBoard(child);

                if (candidateValue > value) value = candidateValue;
                if (value > alpha) alpha = value;
                if (alpha >= beta) break;
            }

            //finally update the TT
            TTFlag flag;
            if (value <= origAlpha) flag = TTFlag.Upper;
            else if (value >= beta) flag = TTFlag.Lower;
            else flag = TTFlag.Exact;
            ttEntry = new TTEntry(currentHash, bestMove, depth, value, flag);
            tt.Write(ttEntry);

            return value;
        }
    }
}
