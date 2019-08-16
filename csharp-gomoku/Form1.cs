using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csharp_gomoku {
    public partial class Form1 : Form {

        Graphics g;
        Brush bBlack;
        Brush bWhite;
        Pen pBlack;

        bool blackToMove;

        const int ss = 20; //STONE SIZE constant for drawing purposes
        const int ls = 24; //LINE SPACING constant for drawing purposes
        const bool clickMove = true; //enable moves by clicking the board

        public Form1() {
            InitializeComponent();
        }

        private void btnTestdrawing_Click(object sender, EventArgs e) {
            drawStone(2, 5, true); //black
            drawStone(10, 15, false); //white
        }

        private void Form1_Load(object sender, EventArgs e) {
            drawingInit();
            blackToMove = true;
        }

        private void drawingInit() {
            g = pnlBoard.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            bBlack = new SolidBrush(Color.Black);
            bWhite = new SolidBrush(Color.White);
            pBlack = new Pen(bBlack, 1);           
        }

        private void drawBoard() {
            int startX = 2 * ls;
            int startY = 2 * ls;            
            int lineLength = Board.Size * ls;
            int endX = startX + lineLength;
            int endY = startY + lineLength;
            int x, y;
            //horizontal lines
            y = startY;
            for (int i = 0; i < Board.Size+1; i++) {
                g.DrawLine(pBlack, startX, y, endX, y);
                y += ls;
            }
            //vertical lines
            x = startX;
            for (int i = 0; i < Board.Size+1; i++) {
                g.DrawLine(pBlack, x, startY, x, endY);
                x += ls;
            }
        }

        /// <summary>
        /// Draws a stone on the board, on the specified grid coords
        /// </summary>
        /// <param name="black">True for black, false for white</param>
        private void drawStone(int x, int y, bool black) {
            if (x < 1 || x > Board.Size || y < 1 || y > Board.Size) return;

            x = (1 + x) * ls;
            y = (1 + y) * ls;

            int hs = ss / 2; //halfsize
            if (black) {
                g.FillEllipse(bBlack, x-hs, y-hs, ss, ss);
            }
            else {
                g.FillEllipse(bBlack, x-hs, y-hs, ss, ss);
                g.FillEllipse(bWhite, x-hs+2, y-hs+2, ss-4, ss-4);
            }
        }

        private void pnlBoard_Paint(object sender, PaintEventArgs e) {
            drawBoard();
        }

        private void pnlBoard_Click(object sender, EventArgs e) {
            if (!clickMove) return;

        }
    }

    public struct Board {
        public const int Size = 15;
    }

}
