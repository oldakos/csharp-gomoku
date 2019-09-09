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
            this.btnReset = new System.Windows.Forms.Button();
            this.lblEngineMove = new System.Windows.Forms.Label();
            this.lblStaticEngineMove = new System.Windows.Forms.Label();
            this.btnStartThink = new System.Windows.Forms.Button();
            this.btnStopThink = new System.Windows.Forms.Button();
            this.lblEval = new System.Windows.Forms.Label();
            this.btnEngineMove = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(635, 519);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(98, 35);
            this.btnUndo.TabIndex = 3;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // lblTerminal
            // 
            this.lblTerminal.AutoSize = true;
            this.lblTerminal.Location = new System.Drawing.Point(653, 91);
            this.lblTerminal.Name = "lblTerminal";
            this.lblTerminal.Size = new System.Drawing.Size(57, 13);
            this.lblTerminal.TabIndex = 4;
            this.lblTerminal.Text = "lblTerminal";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(635, 560);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(98, 36);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblEngineMove
            // 
            this.lblEngineMove.AutoSize = true;
            this.lblEngineMove.Location = new System.Drawing.Point(632, 313);
            this.lblEngineMove.Name = "lblEngineMove";
            this.lblEngineMove.Size = new System.Drawing.Size(94, 13);
            this.lblEngineMove.TabIndex = 8;
            this.lblEngineMove.Text = "*enginemovehere*";
            // 
            // lblStaticEngineMove
            // 
            this.lblStaticEngineMove.AutoSize = true;
            this.lblStaticEngineMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblStaticEngineMove.Location = new System.Drawing.Point(632, 288);
            this.lblStaticEngineMove.Name = "lblStaticEngineMove";
            this.lblStaticEngineMove.Size = new System.Drawing.Size(129, 15);
            this.lblStaticEngineMove.TabIndex = 9;
            this.lblStaticEngineMove.Text = "Recommended move:";
            // 
            // btnStartThink
            // 
            this.btnStartThink.Location = new System.Drawing.Point(635, 405);
            this.btnStartThink.Name = "btnStartThink";
            this.btnStartThink.Size = new System.Drawing.Size(75, 23);
            this.btnStartThink.TabIndex = 10;
            this.btnStartThink.Text = "Start Think";
            this.btnStartThink.UseVisualStyleBackColor = true;
            this.btnStartThink.Click += new System.EventHandler(this.btnStartThink_Click);
            // 
            // btnStopThink
            // 
            this.btnStopThink.Location = new System.Drawing.Point(635, 434);
            this.btnStopThink.Name = "btnStopThink";
            this.btnStopThink.Size = new System.Drawing.Size(75, 23);
            this.btnStopThink.TabIndex = 11;
            this.btnStopThink.Text = "Stop Think";
            this.btnStopThink.UseVisualStyleBackColor = true;
            this.btnStopThink.Click += new System.EventHandler(this.btnStopThink_Click);
            // 
            // lblEval
            // 
            this.lblEval.AutoSize = true;
            this.lblEval.Location = new System.Drawing.Point(632, 158);
            this.lblEval.Name = "lblEval";
            this.lblEval.Size = new System.Drawing.Size(38, 13);
            this.lblEval.TabIndex = 12;
            this.lblEval.Text = "lblEval";
            // 
            // btnEngineMove
            // 
            this.btnEngineMove.Location = new System.Drawing.Point(635, 249);
            this.btnEngineMove.Name = "btnEngineMove";
            this.btnEngineMove.Size = new System.Drawing.Size(107, 23);
            this.btnEngineMove.TabIndex = 13;
            this.btnEngineMove.Text = "Make Engine move";
            this.btnEngineMove.UseVisualStyleBackColor = true;
            this.btnEngineMove.Click += new System.EventHandler(this.btnEngineMove_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(704, 12);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 14;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 628);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnEngineMove);
            this.Controls.Add(this.lblEval);
            this.Controls.Add(this.btnStopThink);
            this.Controls.Add(this.btnStartThink);
            this.Controls.Add(this.lblStaticEngineMove);
            this.Controls.Add(this.lblEngineMove);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblTerminal);
            this.Controls.Add(this.btnUndo);
            this.Name = "GUI";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GUI_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Label lblTerminal;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblEngineMove;
        private System.Windows.Forms.Label lblStaticEngineMove;
        private System.Windows.Forms.Button btnStartThink;
        private System.Windows.Forms.Button btnStopThink;
        private System.Windows.Forms.Label lblEval;
        private System.Windows.Forms.Button btnEngineMove;
        private System.Windows.Forms.Button btnHelp;
    }
}

