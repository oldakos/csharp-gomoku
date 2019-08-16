namespace csharp_gomoku {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tbMoveinput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBoard = new System.Windows.Forms.Panel();
            this.btnTestdrawing = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbMoveinput
            // 
            this.tbMoveinput.Location = new System.Drawing.Point(507, 25);
            this.tbMoveinput.Name = "tbMoveinput";
            this.tbMoveinput.Size = new System.Drawing.Size(100, 20);
            this.tbMoveinput.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(520, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "MOVE INPUT";
            // 
            // pnlBoard
            // 
            this.pnlBoard.Location = new System.Drawing.Point(12, 9);
            this.pnlBoard.Name = "pnlBoard";
            this.pnlBoard.Size = new System.Drawing.Size(440, 440);
            this.pnlBoard.TabIndex = 2;
            this.pnlBoard.Click += new System.EventHandler(this.pnlBoard_Click);
            this.pnlBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBoard_Paint);
            // 
            // btnTestdrawing
            // 
            this.btnTestdrawing.Location = new System.Drawing.Point(519, 153);
            this.btnTestdrawing.Name = "btnTestdrawing";
            this.btnTestdrawing.Size = new System.Drawing.Size(75, 23);
            this.btnTestdrawing.TabIndex = 3;
            this.btnTestdrawing.Text = "button1";
            this.btnTestdrawing.UseVisualStyleBackColor = true;
            this.btnTestdrawing.Click += new System.EventHandler(this.btnTestdrawing_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 458);
            this.Controls.Add(this.btnTestdrawing);
            this.Controls.Add(this.pnlBoard);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMoveinput);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbMoveinput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlBoard;
        private System.Windows.Forms.Button btnTestdrawing;
    }
}

