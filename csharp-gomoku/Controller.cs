using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace csharp_gomoku {

    public interface IEngine {

        /// <summary>
        /// Prepare the engine for a new game.
        /// </summary>
        void Reset();

        /// <summary>
        /// Let the engine know about a stone being added to the board.
        /// </summary>
        void EnemyMove(Square sq);

        /// <summary>
        /// Let the engine know about a stone being removed from the board.
        /// </summary>
        void UndoMove(Square sq);

        /// <summary>
        /// Update the engine with a new position before asking it to play again
        /// </summary>
        /// <param name="gs">The new gamestate</param>
        void ResetPosition(Gamestate gs);

        #region time limited search

        /// <summary>
        /// Request the engine to start indefinite search.
        /// </summary>
        /// <param name="gs">Current gamestate.</param>
        void StartMove();

        /// <summary>
        /// Stop the current search and return the best move.
        /// </summary>
        Square EndMove();

        #endregion

        /// <summary>
        /// Request the engine to search within given depth and play the result.
        /// </summary>
        Square DepthLimitedSearch(int depth);
    }

    public class Controller {

        private enum Gamemode { Engine, Manual };

        Gamemode Mode;
        Gamestate GS;
        IEngine Engine;

        public Controller(Gamestate gs, IEngine ngin) {
            GS = gs;
            Engine = ngin;
        }

        public void ResetGame() {
            GS.Reset();
            Engine.Reset();
        }

        /// <summary>
        /// Makes a move on the given square and lets the engine know.
        /// </summary>
        /// <param name="s">The square of the move.</param>
        /// <returns>True if successfully commited in the Gamestate.</returns>
        public bool TryMakeMove(Square s) {
            if (GS.TryMakeMove(s)) {
                Engine.EnemyMove(s);
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

        public void MakeEngineMove() {
            Engine.StartMove();
            Thread.Sleep(1000);
            GS.TryMakeMove(Engine.EndMove());
        }

    }
}
