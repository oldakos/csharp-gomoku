using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace csharp_gomoku {
           
    public partial class GUI {

        Gamestate GS;
        IEngine Engine;
        Square TopEngineMove;

        delegate void SafeCallDelegate(MoveReport mr);
        //Necessary to use all this because "just" changing a label from another thread raises exceptions

        public void ResetGame() {
            GS.Reset();
            Engine.Reset();
        }

        /// <summary>
        /// Makes a move on the given square and lets the engine know.
        /// </summary>
        /// <returns>True if successfully commited in the Gamestate.</returns>
        public bool TryMakeMove(Square s) {
            if (GS.TryMakeMove(s)) {
                Engine.DoMove(s);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Undo last move and let the engine know.
        /// </summary>
        /// <returns>True if successfuly undone in the Gamestate.</returns>
        public bool TryUndoMove() {
            Square undone = GS.LastMove;
            if (GS.TryUndoMove()) {
                Engine.UndoMove(undone);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Updates the "lblEngineMove" label to show the given move. Safe to call from other threads.
        /// </summary>
        /// <param name="mr"></param>
        public void DisplayEngineMove(MoveReport mr) {
            if (lblEngineMove.InvokeRequired) {
                var d = new SafeCallDelegate(DisplayEngineMove);
                lblEngineMove.Invoke(d, new object[] { mr });
            }
            else {
                TopEngineMove = mr.move;
                lblEngineMove.Text = mr.ToString();
                lblEval.Text = "Current pos heuristic value:\n" + mr.currentHeuristicScore.ToString();
            }
        }
    }

    public interface IEngine {

        /// <summary>
        /// Prepare the engine for a new game.
        /// </summary>
        void Reset();

        /// <summary>
        /// Let the engine know about a stone being added to the board.
        /// </summary>
        void DoMove(Square sq);

        /// <summary>
        /// Let the engine know about a stone being removed from the board.
        /// </summary>
        void UndoMove(Square sq);

        /// <summary>
        /// Request the engine to start indefinite search.
        /// </summary>
        void StartThink();

        /// <summary>
        /// Stop the current search.
        /// </summary>
        void StopThink();
    }

    public struct MoveReport {

        public Square move;
        public int depth;
        public int score;
        public int currentHeuristicScore;

        public MoveReport(Square move, int depth, int score, int heur) {
            this.move = move;
            this.depth = depth;
            this.score = score;
            this.currentHeuristicScore = heur;
        }

        public override string ToString() {            
            return "Move: " + move.ToString() + "\nDepth: " + depth.ToString() + "\nScore: " + score.ToString();
        }
    }
}

