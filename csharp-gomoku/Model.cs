using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_gomoku {

    /// <summary>
    /// A simple ordered pair of integers.
    /// </summary>
    public struct Square {
        public int x, y;

        public Square(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Square operator +(Square s1, Square s2) {
            return new Square(s1.x + s2.x, s1.y + s2.y);
        }

        public static Square operator *(int i, Square s) {
            return new Square(i * s.x, i * s.y);
        }
    }

    public enum Result { Ongoing, BlackWin, WhiteWin, Draw }

    public enum Player { Black, White }

    /// <summary>
    /// Stores information about current game, including an "undo stack"
    /// </summary>
    public class Gamestate {

        public Gamestate() {
            // initial state -> all squares are 0, turnCounter 0.
            board = new byte[sizeX, sizeY];
            MoveHistory = new Stack<Square>();
        }

        public const int sizeX = 15;
        public const int sizeY = 15;
        public const int maxMoves = sizeX * sizeY;

        private byte[,] board;  // 0~empty, 1~white, 2~black
        private Stack<Square> MoveHistory; // redundancy to allow move undoing

        public Square LastMove {
            get; private set;
        }
        public int TurnCounter {
            get; private set;
        } // the stone colour to move is "2 - turnCounter%2"        
                

        public byte this[Square s] {
            get {
                if (IsInBounds(s)) return board[s.x, s.y];
                else return 6; //for out-of-bounds Get, return something that cannot appear on the board
            }
            private set {
                if (IsInBounds(s)) board[s.x, s.y] = value;
            }
        }            

        /// <summary>
        /// Returns "true" if the given Square is a valid 0-based index of the board.
        /// </summary>
        public bool IsInBounds(Square s) {
            return (s.x >= 0) && (s.x < sizeX) && (s.y >= 0) && (s.y < sizeY);
        }

        public Player GetPlayerToMove() {
            if (TurnCounter % 2 == 0) return Player.Black;
            else return Player.White;
        }

        public bool TryMakeMove(Square s) {
            //if gamestate is not terminal AND target square is in-bounds and empty, make a move there.
            if ((this.GetResult() == Result.Ongoing) && (this[s] == 0)) {
                this[s] = (byte)(2 - TurnCounter % 2);
                TurnCounter++;
                LastMove = s;
                MoveHistory.Push(s);
                return true;
            }
            else return false;
        }

        public bool TryUndoMove() {
            if (MoveHistory.Count != 0) {
                this[LastMove] = 0;
                TurnCounter--;
                MoveHistory.Pop();
                if (MoveHistory.Count != 0) LastMove = MoveHistory.Peek();
                return true;
            }
            else return false;
        }

        public void Reset() {
            board = new byte[sizeX, sizeY];
            MoveHistory = new Stack<Square>();
            TurnCounter = 0;
        }

        /// <summary>
        /// Returns if the game has ended and how. See enum "Result".
        /// </summary>
        public Result GetResult() {
            //check if last move won, in all directions
            if (//horizontal
                CheckWin(new Square(1, 0)) ||
                //vertical
                CheckWin(new Square(0, 1)) ||
                //asc diag
                CheckWin(new Square(1, 1)) ||
                //desc diag
                CheckWin(new Square(1, -1))
                ) return WinResult();

            //else check draw
            if (TurnCounter >= maxMoves) return Result.Draw;

            //no win, no draw
            return Result.Ongoing;
        }

        /// <summary>
        /// Checks for 5 in a row, in the given direction, containing the "lastMove" square.
        /// </summary>
        /// <param name="d">! The directional vector !</param>
        private bool CheckWin(Square d) {
            byte player = (byte)(1 + (TurnCounter % 2));
            int counter = 0;
            for (int i = -4; i <= 4; i++) {
                if (this[i * d + LastMove] == player) counter++;
                else counter = 0;
                if (counter == 5) return true;
            }
            return false;
        }

        /// <summary>
        /// Determines who moved last and returns corresponding Result value.
        /// </summary>
        private Result WinResult() {
            if (GetPlayerToMove() == Player.Black) return Result.WhiteWin;
            else return Result.BlackWin;
        }
    }
}
