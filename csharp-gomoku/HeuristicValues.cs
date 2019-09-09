using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_gomoku {

    public partial class ACAutomaton {

        public const int valueOf4 = 3;
        public const int valueOfOpen3 = 5;

        /// <summary>
        /// Adds all the linear heuristic patterns (with values) to the AC tree.
        /// </summary>
        private void FillTree() {
            //Higher value means better for black ('black' is board record '2').

            //4s for black (can win)           
            AddWord(new byte[] { 0, 2, 2, 2, 2 }, valueOf4);
            AddWord(new byte[] { 2, 0, 2, 2, 2 }, valueOf4);
            AddWord(new byte[] { 2, 2, 0, 2, 2 }, valueOf4);
            AddWord(new byte[] { 2, 2, 2, 0, 2 }, valueOf4);
            AddWord(new byte[] { 2, 2, 2, 2, 0 }, valueOf4);

            //4s for white           
            AddWord(new byte[] { 0, 1, 1, 1, 1 }, -valueOf4);
            AddWord(new byte[] { 1, 0, 1, 1, 1 }, -valueOf4);
            AddWord(new byte[] { 1, 1, 0, 1, 1 }, -valueOf4);
            AddWord(new byte[] { 1, 1, 1, 0, 1 }, -valueOf4);
            AddWord(new byte[] { 1, 1, 1, 1, 0 }, -valueOf4);

            //open 3s for black (can become a pair of 4s with one move)
            AddWord(new byte[] { 0, 2, 2, 2, 0, 0 }, valueOfOpen3);
            AddWord(new byte[] { 0, 0, 2, 2, 2, 0 }, valueOfOpen3);
            AddWord(new byte[] { 0, 2, 2, 0, 2, 0 }, valueOfOpen3);
            AddWord(new byte[] { 0, 2, 0, 2, 2, 0 }, valueOfOpen3);

            //open 3s for white
            AddWord(new byte[] { 0, 1, 1, 1, 0, 0 }, -valueOfOpen3);
            AddWord(new byte[] { 0, 0, 1, 1, 1, 0 }, -valueOfOpen3);
            AddWord(new byte[] { 0, 1, 1, 0, 1, 0 }, -valueOfOpen3);
            AddWord(new byte[] { 0, 1, 0, 1, 1, 0 }, -valueOfOpen3);

            //CAREFUL NOT TO USE THE SAME VALUES AS THE 'THREAT CONSTANTS'
            //3s for black (can become a 4)
            AddWord(new byte[] { 0, 2, 2, 2, 0 }, 2);
            AddWord(new byte[] { 2, 2, 2, 0, 0 }, 2);
            AddWord(new byte[] { 0, 0, 2, 2, 2 }, 2);
            AddWord(new byte[] { 2, 2, 0, 2, 0 }, 2);
            AddWord(new byte[] { 0, 2, 0, 2, 2 }, 2);
            AddWord(new byte[] { 0, 2, 2, 0, 2 }, 2);
            AddWord(new byte[] { 2, 0, 2, 2, 0 }, 2);

            //3s for white
            AddWord(new byte[] { 0, 1, 1, 1, 0 }, -2);
            AddWord(new byte[] { 1, 1, 1, 0, 0 }, -2);
            AddWord(new byte[] { 0, 0, 1, 1, 1 }, -2);
            AddWord(new byte[] { 1, 1, 0, 1, 0 }, -2);
            AddWord(new byte[] { 0, 1, 0, 1, 1 }, -2);
            AddWord(new byte[] { 0, 1, 1, 0, 1 }, -2);
            AddWord(new byte[] { 1, 0, 1, 1, 0 }, -2);

            //open 2s for black (can become an open 3)
            AddWord(new byte[] { 0, 2, 2, 0, 0, 0 }, 10);
            AddWord(new byte[] { 0, 2, 0, 2, 0, 0 }, 10);
            AddWord(new byte[] { 0, 2, 0, 0, 2, 0 }, 10);
            AddWord(new byte[] { 0, 0, 2, 2, 0, 0 }, 10);
            AddWord(new byte[] { 0, 0, 2, 0, 2, 0 }, 10);
            AddWord(new byte[] { 0, 0, 0, 2, 2, 0 }, 10);

            //open 2s for white (can become an open 3)
            AddWord(new byte[] { 0, 1, 1, 0, 0, 0 }, -10);
            AddWord(new byte[] { 0, 1, 0, 1, 0, 0 }, -10);
            AddWord(new byte[] { 0, 1, 0, 0, 1, 0 }, -10);
            AddWord(new byte[] { 0, 0, 1, 1, 0, 0 }, -10);
            AddWord(new byte[] { 0, 0, 1, 0, 1, 0 }, -10);
            AddWord(new byte[] { 0, 0, 0, 1, 1, 0 }, -10);


            //AddWord(new byte[] { }, 69);
            //AddWord(new byte[] { }, 69);
            //AddWord(new byte[] { }, 69);
            //AddWord(new byte[] { }, 69);            
        }
    }
}
