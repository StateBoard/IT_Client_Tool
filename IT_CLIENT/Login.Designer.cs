namespace IT_CLIENT
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.txtseat = new System.Windows.Forms.TextBox();
            this.lbl_sname = new System.Windows.Forms.Label();
            this.lbldate = new System.Windows.Forms.Label();
            this.lblclg = new System.Windows.Forms.Label();
            this.btnlogin = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtsign = new System.Windows.Forms.TextBox();
            this.lblinvipw = new System.Windows.Forms.Label();
            this.rdbcom = new System.Windows.Forms.RadioButton();
            this.rdbart = new System.Windows.Forms.RadioButton();
            this.rdbsci = new System.Windows.Forms.RadioButton();
            this.lbldt = new System.Windows.Forms.Label();
            this.lblclgno = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblstream = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtseat
            // 
            this.txtseat.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtseat.Location = new System.Drawing.Point(161, 58);
            this.txtseat.MaxLength = 7;
            this.txtseat.Name = "txtseat";
            this.txtseat.Size = new System.Drawing.Size(159, 20);
            this.txtseat.TabIndex = 17;
            // 
            // lbl_sname
            // 
            this.lbl_sname.AutoSize = true;
            this.lbl_sname.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_sname.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_sname.Location = new System.Drawing.Point(16, 61);
            this.lbl_sname.Name = "lbl_sname";
            this.lbl_sname.Size = new System.Drawing.Size(113, 17);
            this.lbl_sname.TabIndex = 16;
            this.lbl_sname.Text = "Enter Seat No  -";
            // 
            // lbldate
            // 
            this.lbldate.AutoSize = true;
            this.lbldate.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldate.Location = new System.Drawing.Point(335, 22);
            this.lbldate.Name = "lbldate";
            this.lbldate.Size = new System.Drawing.Size(47, 17);
            this.lbldate.TabIndex = 13;
            this.lbldate.Text = "label8";
            // 
            // lblclg
            // 
            this.lblclg.AutoSize = true;
            this.lblclg.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblclg.Location = new System.Drawing.Point(157, 22);
            this.lblclg.Name = "lblclg";
            this.lblclg.Size = new System.Drawing.Size(47, 17);
            this.lblclg.TabIndex = 12;
            this.lblclg.Text = "label7";
            // 
            // btnlogin
            // 
            this.btnlogin.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnlogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnlogin.ForeColor = System.Drawing.Color.MidnightBlue;
            this.btnlogin.Location = new System.Drawing.Point(123, 213);
            this.btnlogin.Name = "btnlogin";
            this.btnlogin.Size = new System.Drawing.Size(100, 29);
            this.btnlogin.TabIndex = 11;
            this.btnlogin.Text = "LOGIN";
            this.btnlogin.UseVisualStyleBackColor = false;
            this.btnlogin.Click += new System.EventHandler(this.btnlogin_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label6.Location = new System.Drawing.Point(39, 176);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(381, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "(Invigilator Should check all the data filled by student )";
            // 
            // txtsign
            // 
            this.txtsign.Location = new System.Drawing.Point(161, 140);
            this.txtsign.Name = "txtsign";
            this.txtsign.PasswordChar = '*';
            this.txtsign.Size = new System.Drawing.Size(159, 20);
            this.txtsign.TabIndex = 9;
            // 
            // lblinvipw
            // 
            this.lblinvipw.AutoSize = true;
            this.lblinvipw.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblinvipw.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblinvipw.Location = new System.Drawing.Point(16, 140);
            this.lblinvipw.Name = "lblinvipw";
            this.lblinvipw.Size = new System.Drawing.Size(151, 17);
            this.lblinvipw.TabIndex = 8;
            this.lblinvipw.Text = "Invigilator Signature -";
            // 
            // rdbcom
            // 
            this.rdbcom.AutoSize = true;
            this.rdbcom.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbcom.Location = new System.Drawing.Point(304, 96);
            this.rdbcom.Name = "rdbcom";
            this.rdbcom.Size = new System.Drawing.Size(98, 21);
            this.rdbcom.TabIndex = 7;
            this.rdbcom.TabStop = true;
            this.rdbcom.Text = "Commerce";
            this.rdbcom.UseVisualStyleBackColor = true;
            // 
            // rdbart
            // 
            this.rdbart.AutoSize = true;
            this.rdbart.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbart.Location = new System.Drawing.Point(243, 96);
            this.rdbart.Name = "rdbart";
            this.rdbart.Size = new System.Drawing.Size(55, 21);
            this.rdbart.TabIndex = 6;
            this.rdbart.TabStop = true;
            this.rdbart.Text = "Arts";
            this.rdbart.UseVisualStyleBackColor = true;
            // 
            // rdbsci
            // 
            this.rdbsci.AutoSize = true;
            this.rdbsci.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbsci.Location = new System.Drawing.Point(161, 96);
            this.rdbsci.Name = "rdbsci";
            this.rdbsci.Size = new System.Drawing.Size(76, 21);
            this.rdbsci.TabIndex = 5;
            this.rdbsci.TabStop = true;
            this.rdbsci.Text = "Science";
            this.rdbsci.UseVisualStyleBackColor = true;
            // 
            // lbldt
            // 
            this.lbldt.AutoSize = true;
            this.lbldt.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldt.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbldt.Location = new System.Drawing.Point(281, 20);
            this.lbldt.Name = "lbldt";
            this.lbldt.Size = new System.Drawing.Size(48, 17);
            this.lbldt.TabIndex = 1;
            this.lbldt.Text = "Date -";
            // 
            // lblclgno
            // 
            this.lblclgno.AutoSize = true;
            this.lblclgno.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblclgno.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblclgno.Location = new System.Drawing.Point(16, 22);
            this.lblclgno.Name = "lblclgno";
            this.lblclgno.Size = new System.Drawing.Size(133, 17);
            this.lblclgno.TabIndex = 0;
            this.lblclgno.Text = "College Index No -";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::IT_CLIENT.Properties.Resources.Header;
            this.pictureBox1.Location = new System.Drawing.Point(0, -4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(481, 81);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // lblstream
            // 
            this.lblstream.AutoSize = true;
            this.lblstream.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstream.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblstream.Location = new System.Drawing.Point(16, 100);
            this.lblstream.Name = "lblstream";
            this.lblstream.Size = new System.Drawing.Size(63, 17);
            this.lblstream.TabIndex = 4;
            this.lblstream.Text = "Stream -";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Exit);
            this.panel1.Controls.Add(this.txtseat);
            this.panel1.Controls.Add(this.lbl_sname);
            this.panel1.Controls.Add(this.lbldate);
            this.panel1.Controls.Add(this.lblclg);
            this.panel1.Controls.Add(this.btnlogin);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtsign);
            this.panel1.Controls.Add(this.lblinvipw);
            this.panel1.Controls.Add(this.rdbcom);
            this.panel1.Controls.Add(this.rdbart);
            this.panel1.Controls.Add(this.rdbsci);
            this.panel1.Controls.Add(this.lblstream);
            this.panel1.Controls.Add(this.lbldt);
            this.panel1.Controls.Add(this.lblclgno);
            this.panel1.Location = new System.Drawing.Point(9, 137);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(461, 258);
            this.panel1.TabIndex = 19;
            // 
            // Exit
            // 
            this.Exit.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Exit.ForeColor = System.Drawing.Color.MidnightBlue;
            this.Exit.Location = new System.Drawing.Point(229, 213);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(100, 29);
            this.Exit.TabIndex = 18;
            this.Exit.Text = "Exit";
            this.Exit.UseVisualStyleBackColor = false;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.MidnightBlue;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(185, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "Login Screen";
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 407);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtseat;
        private System.Windows.Forms.Label lbl_sname;
        private System.Windows.Forms.Label lbldate;
        private System.Windows.Forms.Label lblclg;
        private System.Windows.Forms.Button btnlogin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtsign;
        private System.Windows.Forms.Label lblinvipw;
        private System.Windows.Forms.RadioButton rdbcom;
        private System.Windows.Forms.RadioButton rdbart;
        private System.Windows.Forms.RadioButton rdbsci;
        private System.Windows.Forms.Label lbldt;
        private System.Windows.Forms.Label lblclgno;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblstream;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button Exit;
    }
}