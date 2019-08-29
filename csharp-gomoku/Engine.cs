using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_gomoku {

    /// <summary>
    /// Breaks down if asked to play from a terminal state :)
    /// </summary>
    public partial class MyGreatEngine : IEngine {

        private int moveCount; //to check for draw and determine whose move it is
        private byte[,] board;
        private byte[,] considered; //the considered moves will always be within 3 "chess king moves" from any placed stone
        //considered moves are stored as Byte rather than Bool to be able to decrement, they fit a Byte because they're at most 7*7
        private bool makeFirstMove;

        public MyGreatEngine() {
            Init();
        }

        /// <summary>
        /// Iterates over valid and "considered" moves on the current board.
        /// </summary>
        private IEnumerable<Square> GenerateMoves() {
            for (int i = 3; i < Gamestate.sizeX + 3; i++) {
                for (int j = 3; j < Gamestate.sizeY + 3; j++) {
                    if ((considered[i, j] > 0) && (board[i + 1, j + 1] == 0)) yield return new Square(i - 3, j - 3);
                }
            }
        }

        #region Interface implementation

        public void Init() {
            board = new byte[Gamestate.sizeX + 8, Gamestate.sizeY + 8]; //the 8 extra spaces are padding for victory checking (+-4)
            considered = new byte[Gamestate.sizeX + 6, Gamestate.sizeY + 6]; // the 6 extra are padding for consideration marking (+-3)
            moveCount = 0;
        }

        public void EnemyMove(Square sq) {
            incrementBoard(sq, moveCount % 2 == 0);
        }

        public void UndoMove(Square sq) {
            decrementBoard(sq);
        }

        public void ResetPosition(Gamestate gs) {

            throw new NotImplementedException(); // not yet needed for anything, will just do undo/reset
            
            //Deepcopy the board
            for (int i = 0; i < Gamestate.sizeX; i++) {
                for (int j = 0; j < Gamestate.sizeY; j++) {
                    board[i + 4, j + 4] = gs[new Square(i,j)];
                }
            }


        }

        #region Time limited

        public void StartSearch() {
            throw new NotImplementedException();
        }

        public Square EndSearch() {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Returns "best move" and updates inner board with it.
        /// </summary>
        /// <param name="depth">Depth must be greater than 0.</param>
        public Square DepthLimitedSearch(int depth) {

            if (depth < 1) throw new Exception("The engine cannot suggest a move with search depth 0 or less");           

            int color;
            Square bestMove = new Square(-1, -1); //some move WILL be generated unless the engine is called to play in an already drawn boardstate.
            int value = int.MinValue;

            if (moveCount % 2 == 0) color = 1;
            else color = -1;

            int candidateValue;
            foreach (Square child in GenerateMoves()) {
                candidateValue = -negamax(child, depth - 1, -color);
                if (candidateValue > value) {
                    value = candidateValue;
                    bestMove = child;
                }
            }

            //first move of the game goes in the middle, there are no candidates in such case anyway.
            if (moveCount == 0) {
                bestMove = new Square(Gamestate.sizeX / 2, Gamestate.sizeY / 2);
            }

            //increment! then return
            incrementBoard(bestMove, color == 1);
            return bestMove;
        }

        #endregion

        /// <summary>
        /// Write a move into the internal board, return true if it won the game. Expand the area of considered moves.
        /// The coordinate paramteres must be ACTUAL legal board coords (padding handled inside).
        /// </summary>
        /// <param name="black">True if the stone to be placed is black.</param>
        /// <returns>Returns true if the move is winning.</returns>
        private bool incrementBoard(Square move, bool black) {
            //NOTE: expanding the Considered moves is necessary after "winning move" because we still undo them

            byte color; //to check board against
            bool win = false; //return value
            int boardX = move.x + 4; //to accomodate board padding
            int boardY = move.y + 4;
            int consX = move.x + 3; //to accomodate consideration padding
            int consY = move.y + 3;

            //update the stone
            if (black) color = 2;
            else color = 1;
            board[boardX, boardY] = color;

            //check win - copypasta & padding usage to optimize compared to how Gamestate checks for win           
            //horizontal
            int counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX + i, boardY] == color) counter++;
                else counter = 0;
                if (counter == 5) win = true;
            }
            //vertical
            counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX, boardY + i] == color) counter++;
                else counter = 0;
                if (counter == 5) win = true;
            }
            //asc diagonal
            counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX + i, boardY + i] == color) counter++;
                else counter = 0;
                if (counter == 5) win = true;
            }
            //desc diagonal
            counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (board[boardX + i, boardY - i] == color) counter++;
                else counter = 0;
                if (counter == 5) win = true;
            }

            //update considered moves - enable/increment the 7x7 area around the new move
            for (int i = -3; i <= 3; i++) {
                for (int j = -3; j <= 3; j++) {
                    considered[consX + i, consY + j]++;
                }
            }

            //inc moveCount
            moveCount++;

            return win;
        }

        /// <summary>
        /// Clear a move from the internal board, remove its neighboring area from considered moves.
        /// </summary>
        private void decrementBoard(Square move) {
            int consX = move.x + 3;
            int consY = move.y + 3;

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
        /// Increments the board with the "entry" move, finds value of the resulting position, decrements board back and returns the value.
        /// </summary>
        /// <param name="x">X coord of the entry move</param>
        /// <param name="y">Y coord of the entry move</param>
        /// <param name="depth">The remaining depth.</param>
        /// <param name="color">The color of the player to move "after increment".</param>
        /// <returns>The value</returns>
        private int negamax(Square move, int depth, int color) {
            //note that incrementBoard() is called in the first IF condition

            //if state is terminal, return:
            if (incrementBoard(move, color == -1)) {//increment is done with previous player's stone, arg 'color' is the player to move after increment, hence the Minus
                decrementBoard(move);
                return color * int.MinValue; //previous player won, so the value for player "to move" is minus inf
            }
            if (moveCount >= Gamestate.maxMoves) {
                decrementBoard(move);
                return 0; //draw, return 0
            }
            if (depth <= 0) {
                decrementBoard(move);
                return color * evaluate(); //not a win, not a draw, but reached maximum depth (0 remaining depth), return evaluation
            }

            //otherwise keep searching children
            int value = int.MinValue;
            int candidateValue;
            foreach (Square child in GenerateMoves()) {
                candidateValue = -negamax(child, depth - 1, -color);
                if (candidateValue > value) value = candidateValue;
            }

            decrementBoard(move);
            return value;
        }
    }
}
