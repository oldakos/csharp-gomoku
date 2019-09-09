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
        Font coordFont; //font to label axes

        const int ss = 20; //STONE SIZE constant for drawing purposes
        const int ls = 24; //LINE SPACING constant for drawing purposes
        const bool clickMove = true; //enable moves by clicking the board (otherwise text input?)

        public GUI(Gamestate gs) {
            InitializeComponent();
            GS = gs;
            Engine = new MyGreatEngine(this);
        }

        public GUI() {

        }

        private void Form1_Load(object sender, EventArgs e) {
            //hide the "terminal gamestate" label
            lblTerminal.Text = "TERMINAL GAMESTATE";
            lblTerminal.Hide();

            lblEval.Text = "Current pos heuristic value:\n0";
            lblEngineMove.Text = "N/A";
            //set up the panel for the board
            pnlBoard.Location = new Point(32, 32);
            pnlBoard.Width = (Gamestate.sizeX + 1) * ls;
            pnlBoard.Height = (Gamestate.sizeY + 1) * ls;
            pnlBoard.Paint += pnlBoard_Paint;
            pnlBoard.Click += pnlBoard_Click;
            //prepare brushes and pens
            bBlack = new SolidBrush(Color.Black);
            bWhite = new SolidBrush(Color.White);
            bRed = new SolidBrush(Color.Red);
            pBlack = new Pen(bBlack, 1);
            coordFont = new Font("Arial", 10);
            //add the board's Panel Control to form
            this.Controls.Add(pnlBoard);
            //prevent any 'unitialized' nonsense with TopEngineMove
            TopEngineMove = new Square(-1, -1);
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

            //clear previous grid (without this Undo wouldn't be shown properly)

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

            //coordinate labels
            startX += -ls - 6;
            startY += -ls - 6;
            for (int i = 1; i <= Gamestate.sizeX; i++) {
                g.DrawString(i.ToString(), coordFont, bBlack, startX + i * ls, y - 6);
            }
            for (int j = 1; j <= Gamestate.sizeY; j++) {
                g.DrawString(j.ToString(), coordFont, bBlack, x - 6, startY + j * ls);
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

        private void btnReset_Click(object sender, EventArgs e) {
            ResetGame();
            pnlBoard.Refresh();
            MessageBox.Show("Game and engine have been reset");
        }

        private void btnStartThink_Click(object sender, EventArgs e) {
            Engine.StartThink();
        }

        private void btnStopThink_Click(object sender, EventArgs e) {
            Engine.StopThink();
        }

        private void GUI_FormClosing(object sender, FormClosingEventArgs e) {
            Engine.StopThink();
        }

        private void btnEngineMove_Click(object sender, EventArgs e) {
            if (TryMakeMove(TopEngineMove)) pnlBoard.Refresh();
        }

        private void btnHelp_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "Hi! Play moves on the board by clicking it.\n" +
                "The functions of the controls top to bottom are as follows:\n" +
                "The first label shows the heuristic value of the current position, higher number means better for black. (Only active when the engine is running)\n"+
                "The \"Make engine move\" button is the same as clicking on the \"Recommended\" coordinates yourself.\n" +
                "The engine's recommendation's score is the higher the better the *player to move* is. (Different from current eval)\n" +
                "Scores like +-123456 mean that the engine is certain a player must win, the recommended move then might not make sense.\n" +
                "The \"Start think\" and \"End think\" buttons start/end the background work of the engine.\n" +
                "Finally, the \"Undo\" and \"Reset\" buttons do exactly what their label says.\n" +
                "A special label pops up whenever a terminal position is reached. (Someone wins or the board is full)"
                );
        }

        private void pnlBoard_Click(object sender, EventArgs e) {
            // get click coords within the panel
            Point p = pnlBoard.PointToClient(Cursor.Position);

            // calculate board coords
            var move = new Square(p.X / ls, p.Y / ls);

            // try to play that move
            if (TryMakeMove(move)) {
                pnlBoard.Refresh();
            }
        }

        private void btnUndo_Click(object sender, EventArgs e) {
            if (TryUndoMove()) {
                pnlBoard.Refresh();
            }
        }
    }

    /// <summary>
    /// We have this because the DoubleBuffered property of a "normal" Panel is not accessible, and redrawing without double buffer causes flickering.
    /// </summary>
    public class DoubleBufferPanel : Panel {
        public DoubleBufferPanel() {
            this.DoubleBuffered = true;
        }
    }
}
