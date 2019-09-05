using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_gomoku {
    public partial class MyGreatEngine {
        /// <summary>
        /// Evaluates the current board. Higher value means better for black (color '1', board record '2').
        /// Uses Aho-Corasick word-search algo to find linear patterns.
        /// AC algo source: Pruvodce labyrintem algoritmu; Mares M., Valla T.; Praha 2017
        /// </summary>
        private int evaluate() {
            if (moveCount >= Gamestate.maxMoves) return 0;
            aca.Reset(moveCount % 2 == 0);
            //vertical
            for (int i = 0; i < Gamestate.sizeX; i++) {
                aca.NewLine();
                for (int j = 0; j < Gamestate.sizeY; j++) {
                    aca.FeedSquare(this[i, j]);
                }
            }
            //horizontal
            for (int i = 0; i < Gamestate.sizeY; i++) {
                aca.NewLine();
                for (int j = 0; j < Gamestate.sizeX; j++) {
                    aca.FeedSquare(this[j, i]);
                }
            }

            int k, l;
            //asc diag          
            for (int i = 0; i < Gamestate.sizeX; i++) {
                aca.NewLine();
                k = i;
                l = 0;
                while ((k < Gamestate.sizeX) && (l < Gamestate.sizeY)) {
                    aca.FeedSquare(this[k, l]);
                    k++;
                    l++;
                }
            }
            for (int j = 1; j < Gamestate.sizeY; j++) {
                aca.NewLine();
                k = 0;
                l = j;
                while ((k < Gamestate.sizeX) && (l < Gamestate.sizeY)) {
                    aca.FeedSquare(this[k, l]);
                    k++;
                    l++;
                }
            }

            //desc diag          
            for (int i = 0; i < Gamestate.sizeX; i++) {
                aca.NewLine();
                k = i;
                l = Gamestate.sizeY - 1;
                while ((k < Gamestate.sizeX) && (l >= 0)) {
                    aca.FeedSquare(this[k, l]);
                    k++;
                    l--;
                }
            }
            for (int j = 1; j < Gamestate.sizeY; j++) {
                aca.NewLine();
                k = 0;
                l = j;
                while ((k < Gamestate.sizeX) && (l >= 0)) {
                    aca.FeedSquare(this[k, l]);
                    k++;
                    l--;
                }
            }

            return aca.GetTotalValue();
        }
    }

    /// <summary>
    /// The 'Automaton' class for Aho Corasick
    /// </summary>
    public partial class ACAutomaton {

        ACNode Root;
        ACNode CurrentNode;
        ACNode ShortcutProbe;
        private int LineValue;
        private int TotalValue;
        bool blackToMove;
        int black4, black3, white4, white3; //to keep track of how many threatening patterns there are on the board

        public ACAutomaton() {
            BuildAutomaton();
        }

        #region build

        /// <summary>
        /// Generate tree nodes for the given word and assign the value.
        /// </summary>
        private void AddWord(IEnumerable<Byte> word, int value) {
            ACNode node = Root;
            foreach (byte b in word) {
                if (null == node.Forward[b]) node.Forward[b] = new ACNode();
                node = node.Forward[b];
            }
            node.Value = value;
        }

        /// <summary>
        /// Builds the automaton using patterns from HeuristicValues.cs
        /// </summary>
        public void BuildAutomaton() {
            Root = new ACNode();
            FillTree();
            Queue<ACNode> q = new Queue<ACNode>();
            foreach (ACNode son in Root.Forward) {
                if (null == son) continue;
                q.Enqueue(son);
                son.Back = Root;
            }
            ACNode i, z, s;
            while (q.Count != 0) {
                i = q.Dequeue();
                for (byte b = 0; b <= 2; b++) {
                    s = i.Forward[b];
                    if (null == s) continue;

                    z = i.Back;
                    while ((null == z.Forward[b]) && (z != Root)) z = z.Back;
                    if (null != z.Forward[b]) z = z.Forward[b];
                    s.Back = z;
                    if (z.Value != 0) s.Shortcut = z;
                    else s.Shortcut = z.Shortcut;

                    q.Enqueue(s);
                }
            }
            CurrentNode = Root;
        }

        #endregion

        #region search

        /// <summary>
        /// Sets all values to 0, preparing to evaluate a new board, with the indication of whose turn it is.
        /// </summary>
        public void Reset(bool black) {
            blackToMove = black;
            LineValue = 0;
            TotalValue = 0;
            CurrentNode = Root;
            black4 = 0;
            black3 = 0;
            white4 = 0;
            white3 = 0;
        }

        /// <summary>
        /// Adds Linevalue to Totalvalue before setting it to 0, preparing to evaluate the next line.
        /// </summary>
        public void NewLine() {
            TotalValue += LineValue;
            LineValue = 0;
            CurrentNode = Root;
        }

        /// <summary>
        /// Give a square's value to the automaton; called one-by-one for an entire line.
        /// </summary>
        public void FeedSquare(byte b) {
            //update current node based on the read square
            while ((null == CurrentNode.Forward[b]) && (CurrentNode != Root)) CurrentNode = CurrentNode.Back;
            if (null != CurrentNode.Forward[b]) CurrentNode = CurrentNode.Forward[b];

            //update value based on node's value and shortcuts
            ShortcutProbe = CurrentNode;
            while (null != ShortcutProbe) {
                checkScore(ShortcutProbe.Value);
                LineValue += ShortcutProbe.Value;
                ShortcutProbe = ShortcutProbe.Shortcut;
            }
        }

        /// <summary>
        /// Checks whether the score of a pattern belongs to a "dangerous" one (4s, open 3s), and makes note if it does
        /// </summary>
        /// <param name="value">A pattern's score.</param>
        void checkScore(int value) {
            if (value == valueOf4) black4++;
            if (value == -valueOf4) white4++;
            if (value == valueOfOpen3) black3++;
            if (value == -valueOfOpen3) white3++;
        }

        /// <summary>
        /// Check if there is a combination of threatening patterns and return an appropriate large score.
        /// Otherwise just add the last line's value to the total and return it.
        /// </summary>
        public int GetTotalValue() {
            if (blackToMove) {
                if (black4 > 0) return 12345678;    //win in one move
                if ((black3 > 0) && (white4 == 0)) return 12345678; //no need to block white's 4, can extend an open 3 to "double" 4 -> win
                if (white4 > 1) return -12345678; //white has more than one 4 that we need to block -> loss
                //other pattern combinations have corner cases in which they aren't necessarily great.
                if (white3 > 2) TotalValue += -70;  //2 open 3s could actually only be one as a "double positive" ('.xxx..' and '..xxx.')
                                                    //but more than 2 of them can sometimes be trouble (not always, because they can be blocked in one move and/or with a 4)
            }
            else {
                if (white4 > 0) return -12345678;
                if ((white3 > 0) && (black4 == 0)) return -12345678;
                if (black4 > 1) return 12345678;
                if (black3 > 2) TotalValue += 70;
            }

            TotalValue += LineValue;
            return TotalValue;
        }

        #endregion

    }

    /// <summary>
    /// The 'node' class for Aho Corasick
    /// </summary>
    public class ACNode {

        public ACNode Back; //the edge to take if there 'forward' edge is null
        public ACNode Shortcut; //closest valued state reachable by 'back' edges
        public int Value; //the evaluation score of the word ending in this node
        public ACNode[] Forward; //the edge to take after reading a given value in the board

        public ACNode() {
            Forward = new ACNode[3]; //the alphabet will always be {0,1,2}
        }

    }
}
