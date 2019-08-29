namespace csharp_gomoku {
    partial class GUI {
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
            this.btnUndo = new System.Windows.Forms.Button();
            this.lblTerminal = new System.Windows.Forms.Label();
            this.btnEngineBlack = new System.Windows.Forms.Button();
            this.btnEngineWhite = new System.Windows.Forms.Button();
            this.btnPvp = new System.Windows.Forms.Button();
            this.lblNewgame = new System.Windows.Forms.Label();
            this.btnEngineMove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(674, 12);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(105, 35);
            this.btnUndo.TabIndex = 3;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // lblTerminal
            // 
            this.lblTerminal.AutoSize = true;
            this.lblTerminal.Location = new System.Drawing.Point(634, 99);
            this.lblTerminal.Name = "lblTerminal";
            this.lblTerminal.Size = new System.Drawing.Size(57, 13);
            this.lblTerminal.TabIndex = 4;
            this.lblTerminal.Text = "lblTerminal";
            // 
            // btnEngineBlack
            // 
            this.btnEngineBlack.Location = new System.Drawing.Point(646, 211);
            this.btnEngineBlack.Name = "btnEngineBlack";
            this.btnEngineBlack.Size = new System.Drawing.Size(98, 23);
            this.btnEngineBlack.TabIndex = 5;
            this.btnEngineBlack.Text = "Engine is Black";
            this.btnEngineBlack.UseVisualStyleBackColor = true;
            this.btnEngineBlack.Click += new System.EventHandler(this.btnEngineBlack_Click);
            // 
            // btnEngineWhite
            // 
            this.btnEngineWhite.Location = new System.Drawing.Point(646, 240);
            this.btnEngineWhite.Name = "btnEngineWhite";
            this.btnEngineWhite.Size = new System.Drawing.Size(98, 23);
            this.btnEngineWhite.TabIndex = 6;
            this.btnEngineWhite.Text = "Engine is White";
            this.btnEngineWhite.UseVisualStyleBackColor = true;
            this.btnEngineWhite.Click += new System.EventHandler(this.btnEngineWhite_Click);
            // 
            // btnPvp
            // 
            this.btnPvp.Location = new System.Drawing.Point(646, 269);
            this.btnPvp.Name = "btnPvp";
            this.btnPvp.Size = new System.Drawing.Size(98, 36);
            this.btnPvp.TabIndex = 7;
            this.btnPvp.Text = "RESET";
            this.btnPvp.UseVisualStyleBackColor = true;
            this.btnPvp.Click += new System.EventHandler(this.btnPvp_Click);
            // 
            // lblNewgame
            // 
            this.lblNewgame.AutoSize = true;
            this.lblNewgame.Location = new System.Drawing.Point(656, 185);
            this.lblNewgame.Name = "lblNewgame";
            this.lblNewgame.Size = new System.Drawing.Size(77, 13);
            this.lblNewgame.TabIndex = 8;
            this.lblNewgame.Text = "RESET GAME";
            // 
            // btnEngineMove
            // 
            this.btnEngineMove.Location = new System.Drawing.Point(646, 391);
            this.btnEngineMove.Name = "btnEngineMove";
            this.btnEngineMove.Size = new System.Drawing.Size(98, 23);
            this.btnEngineMove.TabIndex = 9;
            this.btnEngineMove.Text = "Engine move";
            this.btnEngineMove.UseVisualStyleBackColor = true;
            this.btnEngineMove.Click += new System.EventHandler(this.btnEngineMove_Click);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 628);
            this.Controls.Add(this.btnEngineMove);
            this.Controls.Add(this.lblNewgame);
            this.Controls.Add(this.btnPvp);
            this.Controls.Add(this.btnEngineWhite);
            this.Controls.Add(this.btnEngineBlack);
            this.Controls.Add(this.lblTerminal);
            this.Controls.Add(this.btnUndo);
            this.Name = "GUI";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Label lblTerminal;
        private System.Windows.Forms.Button btnEngineBlack;
        private System.Windows.Forms.Button btnEngineWhite;
        private System.Windows.Forms.Button btnPvp;
        private System.Windows.Forms.Label lblNewgame;
        private System.Windows.Forms.Button btnEngineMove;
    }
}

