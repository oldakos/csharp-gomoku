using System;
using System.Collections.Generic;
using csharp_gomoku;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests_csharp_gomoku {
    [TestClass]
    public class EngineTests {
        [TestMethod]
        public void IncrementAndDecrementResultInSameState() {

            MyGreatEngine engine1 = new MyGreatEngine(new GUI());
            PrivateObject obj1 = new PrivateObject(engine1);

            byte[,] expectedBoard = (byte[,])obj1.GetFieldOrProperty("board");
            byte[,] expectedCons = (byte[,])obj1.GetFieldOrProperty("considered");
            var expectedHash = obj1.GetFieldOrProperty("currentHash");

            MyGreatEngine engine2 = new MyGreatEngine(new GUI());
            PrivateObject obj2 = new PrivateObject(engine2);

            obj2.Invoke("incrementBoard", new object[] { new Square(6, 7), true });
            obj2.Invoke("decrementBoard", new object[] { new Square(6, 7) });

            byte[,] resultBoard = (byte[,])obj2.GetFieldOrProperty("board");
            byte[,] resultCons = (byte[,])obj2.GetFieldOrProperty("considered");
            var resultHash = obj2.GetFieldOrProperty("currentHash");

            for (int i = 0; i < 12; i++) {
                for (int j = 0; j < 12; j++) {
                    Assert.AreEqual(expectedBoard[i, j], resultBoard[i, j]);
                    Assert.AreEqual(expectedCons[i, j], resultCons[i, j]);
                }
            }
            Assert.AreEqual(expectedHash, resultHash);
        }

        [TestMethod]
        public void MoveGenerationTest() {
            MyGreatEngine engine = new MyGreatEngine(new GUI());
            PrivateObject obj = new PrivateObject(engine);
            obj.Invoke("incrementBoard", new object[] { new Square(4, 5), true });

            var expected = new List<Square> {
                new Square(2, 3),
                new Square(3, 3),
                new Square(4, 3),
                new Square(5, 3),
                new Square(6, 3),
                new Square(2, 4),
                new Square(3, 4),
                new Square(4, 4),
                new Square(5, 4),
                new Square(6, 4),
                new Square(2, 6),
                new Square(3, 6),
                new Square(4, 6),
                new Square(5, 6),
                new Square(6, 6),
                new Square(2, 7),
                new Square(3, 7),
                new Square(4, 7),
                new Square(5, 7),
                new Square(6, 7),
                new Square(2, 5),
                new Square(3, 5),
                new Square(5, 5),
                new Square(6, 5)
            };

            var actual1 = new List<Square>();

            var gen = (IEnumerable<Square>)obj.Invoke("GenerateMoves", new object[] { });
            foreach (Square item in gen) {
                actual1.Add(item);
            }

            CollectionAssert.AreEquivalent(expected, actual1);

            //--------- now the shuffled one which should in addition always return its argument first

            expected.Add(new Square(15, 15));

            var actual2 = new List<Square>();

            var genShuffle = (IEnumerable<Square>)obj.Invoke("GenerateShuffledMoves", new object[] { new Square(15, 15) });
            foreach (Square item in genShuffle) {
                actual2.Add(item);
            }


            CollectionAssert.AreEquivalent(expected, actual2);
        }
    }

    [TestClass]
    public class TransposTableTests {
        [TestMethod]
        public void FirstRecordOfAHashGetsReturned() {

            ulong hash = 0x12_34_56_78_9A_BC_DE_F1;

            TransposTable tt = new TransposTable(8);
            TTEntry orig = new TTEntry(hash, new Square(4, 5), 6, 7, TTFlag.Exact);
            tt.Write(orig);
            TTEntry returned;
            bool retVal = tt.TryGetValue(hash, out returned);

            Assert.AreEqual(true, retVal);
            Assert.AreEqual(orig.Hash, returned.Hash);
            Assert.AreEqual(orig.BestMove, returned.BestMove);
            Assert.AreEqual(orig.Depth, returned.Depth);
            Assert.AreEqual(orig.Score, returned.Score);
            Assert.AreEqual(orig.Flag, returned.Flag);
        }

        [TestMethod]
        public void IndexCollision_TryGetValue_ReturnsFalse() {
            TransposTable tt = new TransposTable(8);
            TTEntry orig = new TTEntry(0x12_34_56_78_9A_BC_DE_F1, new Square(4, 5), 6, 7, TTFlag.Exact);
            tt.Write(orig);
            TTEntry returned;
            bool retVal = tt.TryGetValue(0xFF_FF_FF_FF_9A_BC_DE_F1, out returned);

            Assert.AreEqual(false, retVal);
        }

        [TestMethod]
        public void IndexCollision_HigherDepth_Remains() {

            ulong hash1 = 0x12_34_56_78_9A_BC_DE_F1;
            ulong hash2 = 0xFF_FF_FF_FF_9A_BC_DE_F1;

            TransposTable tt = new TransposTable(8);
            TTEntry orig = new TTEntry(hash1, new Square(4, 5), 6, 7, TTFlag.Exact);
            tt.Write(orig);
            TTEntry higherDepth = new TTEntry(hash2, new Square(4, 5), 60, 7, TTFlag.Exact);
            tt.Write(higherDepth); //should overwrite

            TTEntry returned;
            bool retVal = tt.TryGetValue(hash2, out returned);

            Assert.AreEqual(true, retVal);

            tt.Write(orig); //should not overwrite
            retVal = tt.TryGetValue(hash2, out returned);

            Assert.AreEqual(true, retVal);
        }
    }

    [TestClass]
    public class AhoCorasickTests {

        [TestMethod]
        public void AC_TestOnSomeString() {
            ACAutomaton aca = new ACAutomaton();

            aca.FeedSquare(0);
            aca.FeedSquare(2);
            aca.FeedSquare(2);
            aca.FeedSquare(2);
            aca.FeedSquare(2);
            aca.FeedSquare(0);
            aca.FeedSquare(2);
            aca.FeedSquare(2);

            PrivateObject obj = new PrivateObject(aca);
            int actual = (int)obj.GetFieldOrProperty("LineValue");
            int expected = 4 * (ACAutomaton.valueOf4);
            //expecting 02222, 22220, 22202 and 22022 (in this order.)

            //note: this test depends on how the heuristic is currently set up
            //it is supposed to test if the actual aho-corasick works and finds overlaying substrings
            Assert.AreEqual(expected, actual);
        }        
    }
}
