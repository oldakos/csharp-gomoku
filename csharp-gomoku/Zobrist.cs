using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_gomoku {
    class Zobrist {

        /// <summary>
        /// The 64-bit codes for every combination of stone and square. Setting is private and is only done in the constructor so that it stays unchanged forever
        /// </summary>
        public ulong[,,] Table {
            get;
            private set;
        }

        /// <summary>
        /// Initialize the table with random hashes for each (Square, Color) pair.
        /// </summary>
        public Zobrist() {
            Table = new ulong[Gamestate.sizeX, Gamestate.sizeY, 2];
            Random r = new Random();
            for (int i = 0; i < Gamestate.sizeX; i++) {
                for (int j = 0; j < Gamestate.sizeY; j++) {
                    for (int k = 0; k < 2; k++) {
                        Table[i, j, k] = (ulong)(long)(r.NextDouble() * long.MaxValue);
                    }
                }
            }
        }

        /// <summary>
        /// Get the given Engine's current position's hash.
        /// </summary>
        public ulong Hash(MyGreatEngine ngin) {
            //This is here 'just in case', normally the engine keeps its hash up to date incrementally
            ulong h = 0;
            byte k;
            for (int i = 0; i < Gamestate.sizeX; i++) {
                for (int j = 0; j < Gamestate.sizeY; j++) {
                    k = ngin[i, j];
                    if (k != 0) {
                        h = h ^ Table[i, j, k - 1];
                    }
                }
            }
            return h;
        }
    }
}
