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
    public partial class GUI : Form {

        DoubleBufferPanel pnlBoard = new DoubleBufferPanel();
        Brush bBlack, bWhite, bRed;
        Pen pBlack;

        Gamestate GS; //draw gamestate from here
        Controller ctrl; //send inputs here

        const int ss = 20; //STONE SIZE constant for drawing purposes
        const int ls = 24; //LINE SPACING constant for drawing purposes
        const bool clickMove = true; //enable moves by clicking the board (otherwise text input?)

        public GUI(Controller controller, Gamestate gs) {
            InitializeComponent();
            this.ctrl = controller;
            GS = gs;
        }

        public GUI() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            //hide the "terminal gamestate" label
            lblTerminal.Text = "TERMINAL GAMESTATE";
            lblTerminal.Hide();
            //set up the panel for the board
            pnlBoard.Location = new Point(32, 32);
            pnlBoard.Width = Gamestate.sizeX * ls;
            pnlBoard.Height = Gamestate.sizeY * ls;
            pnlBoard.Paint += pnlBoard_Paint;
            pnlBoard.Click += pnlBoard_Click;
            //prepare brushes and pens
            bBlack = new SolidBrush(Color.Black);
            bWhite = new SolidBrush(Color.White);
            bRed = new SolidBrush(Color.Red);
            pBlack = new Pen(bBlack, 1);
            //add the board's Panel Control to form
            this.Controls.Add(pnlBoard);
        }

        /// <summary>
        /// Draws a stone on the specified board coords.
        /// </summary>
        /// <param name="black">True for black, false for white</param>
        private void drawStone(int x, int y, bool black, Graphics g) {
            if (x < 0 || x >= Gamestate.sizeX || y < 0 || y >= Gamestate.sizeY) return;

            x = ls / 2 + x * ls;
            y = ls / 2 + y * ls;

            int hs = ss / 2; //halfsize
            if (black) {
                g.FillEllipse(bBlack, x - hs, y - hs, ss, ss);
            }
            else {
                g.FillEllipse(bBlack, x - hs, y - hs, ss, ss);
                g.FillEllipse(bWhite, x - hs + 2, y - hs + 2, ss - 4, ss - 4);
            }
        }

        private void pnlBoard_Paint(object sender, PaintEventArgs e) { //reads from GS

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //clear previous grid (for Undoing)

            g.Clear(pnlBoard.BackColor);

            //draw the grid

            int startX = ls / 2;
            int startY = ls / 2;
            int x, y;
            //horizontal lines
            int lineLength = (Gamestate.sizeX - 1) * ls;
            int endX = startX + lineLength;
            y = startY;
            for (int i = 0; i < Gamestate.sizeY; i++) {
                g.DrawLine(pBlack, startX, y, endX, y);
                y += ls;
            }
            //vertical lines
            lineLength = (Gamestate.sizeY - 1) * ls;
            int endY = startY + lineLength;
            x = startX;
            for (int i = 0; i < Gamestate.sizeX; i++) {
                g.DrawLine(pBlack, x, startY, x, endY);
                x += ls;
            }

            //then, draw the stones

            byte val;
            for (int i = 0; i < Gamestate.sizeX; i++) {
                for (int j = 0; j < Gamestate.sizeY; j++) {
                    val = GS[new Square(i, j)];
                    if (val == 2) drawStone(i, j, true, g);
                    if (val == 1) drawStone(i, j, false, g);
                }
            }

            //highlight the last move

            if (GS.TurnCounter > 0) {
                x = GS.LastMove.x;
                y = GS.LastMove.y;
                x = ls / 2 + x * ls;
                y = ls / 2 + y * ls;

                g.FillEllipse(bRed, x - 2, y - 2, 4, 4);
            }

            //if gamestate is terminal, show the label

            if (GS.GetResult() == Result.Ongoing) lblTerminal.Hide();
            else lblTerminal.Show();
        }

        private void btnEngineBlack_Click(object sender, EventArgs e) {
            MessageBox.Show("This does nothing");
        }

        private void btnEngineWhite_Click(object sender, EventArgs e) {
            MessageBox.Show("This does nothing");
        }

        private void btnPvp_Click(object sender, EventArgs e) {
            ctrl.ResetGame();
            pnlBoard.Refresh();
            MessageBox.Show("Game and engine have been reset");
        }

        private void btnEngineMove_Click(object sender, EventArgs e) {
            ctrl.MakeEngineMove();
            pnlBoard.Refresh();
        }

        private void pnlBoard_Click(object sender, EventArgs e) {
            if (!clickMove) return;
            else {
                // get click coords within the panel
                Point p = pnlBoard.PointToClient(Cursor.Position);

                // calculate board coords
                var move = new Square(p.X / ls, p.Y / ls);

                // try to play the move; if successful, redraw board
                if (ctrl.TryMakeMove(move)) pnlBoard.Refresh();
            }
        }

        private void btnUndo_Click(object sender, EventArgs e) {
            if (ctrl.TryUndoMove()) {
                pnlBoard.Refresh();
            }
        }
    }

    /// <summary>
    /// We have this because the DoubleBuffered property of a normal Panel is not accessible, and redrawing without double buffer causes flickering.
    /// </summary>
    public class DoubleBufferPanel : Panel {
        public DoubleBufferPanel() {
            this.DoubleBuffered = true;
        }
    }
}
