using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_gomoku {

    public enum TTFlag { Exact, Upper, Lower }

    public class TTEntry {
        public ulong Hash;
        public Square BestMove;
        public int Depth;
        public int Score;
        public TTFlag Flag;

        public TTEntry(ulong hash, Square best, int depth, int score, TTFlag flag) {
            Hash = hash;
            BestMove = best;
            Score = score;
            Flag = flag;
            Depth = depth;
        }
    }

    /// <summary>
    /// A transposition table that allows access from multiple threads.
    /// </summary>
    class TransposTable {

        private TTEntry[] table;
        private int size; //the size of the index *in bits*

        /// <summary>
        /// Initializes the table itself, with an array of given size.
        /// </summary>
        /// <param name="indexSize">The number of bits to index by.</param>
        public TransposTable(int indexSize) {
            size = indexSize;
            table = new TTEntry[1 << size];
        }

        /// <summary>
        /// Look up a position's hash in the table.
        /// </summary>
        /// <param name="hash">The hash to search for.</param>
        /// <param name="entry">The entry from the table.</param>
        /// <returns>True if the lookup was successful and the out-param contains the correct record.</returns>
        public bool TryGetValue(ulong hash, out TTEntry entry) {

            entry = table[GetIndex(hash)];

            if (null == entry) return false;
            if (entry.Hash == hash) return true;
            return false;
        }

        /// <summary>
        /// Submit an entry to the tablebase. It will decide for itself whether it keeps it or prefers the previous record.
        /// </summary>
        /// <param name="entry">The new entry.</param>
        public void Write(TTEntry entry) {

            //CURRENT STRATEGY: "Replace with higher depth"

            int index = GetIndex(entry.Hash);
            TTEntry oldEntry = table[index];
            if ((null == oldEntry) || (entry.Depth > oldEntry.Depth)) table[index] = entry;

        }

        private int GetIndex(ulong hash) {
            return (int)(hash & (((uint)1 << size) - 1)); //take N lowest bits of hash, N = 'size'; that will be the table index
        }
    }
}
