namespace TETRA_Coverage_Monitor
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.Password_Txt = new System.Windows.Forms.TextBox();
            this.passwordpic_btn = new System.Windows.Forms.Button();
            this.btn_close_login_form = new System.Windows.Forms.Button();
            this.UserName_Txt = new System.Windows.Forms.TextBox();
            this.btn_login = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lbl_Error_loginform = new System.Windows.Forms.Label();
            this.btn_minimize_login_form = new System.Windows.Forms.Button();
            this.pnl_controls_loginform = new System.Windows.Forms.Panel();
            this.pnl_controls_loginform.SuspendLayout();
            this.SuspendLayout();
            // 
            // Password_Txt
            // 
            this.Password_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Password_Txt.Location = new System.Drawing.Point(142, 101);
            this.Password_Txt.Name = "Password_Txt";
            this.Password_Txt.PasswordChar = '*';
            this.Password_Txt.Size = new System.Drawing.Size(232, 20);
            this.Password_Txt.TabIndex = 5;
            this.Password_Txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // passwordpic_btn
            // 
            this.passwordpic_btn.BackColor = System.Drawing.Color.Transparent;
            this.passwordpic_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("passwordpic_btn.BackgroundImage")));
            this.passwordpic_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.passwordpic_btn.Enabled = false;
            this.passwordpic_btn.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.passwordpic_btn.FlatAppearance.BorderSize = 0;
            this.passwordpic_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.passwordpic_btn.ForeColor = System.Drawing.Color.Transparent;
            this.passwordpic_btn.Location = new System.Drawing.Point(93, 94);
            this.passwordpic_btn.Name = "passwordpic_btn";
            this.passwordpic_btn.Size = new System.Drawing.Size(43, 37);
            this.passwordpic_btn.TabIndex = 8;
            this.passwordpic_btn.UseVisualStyleBackColor = false;
            // 
            // btn_close_login_form
            // 
            this.btn_close_login_form.BackColor = System.Drawing.Color.Transparent;
            this.btn_close_login_form.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_close_login_form.BackgroundImage")));
            this.btn_close_login_form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_close_login_form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_close_login_form.FlatAppearance.BorderSize = 0;
            this.btn_close_login_form.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close_login_form.ForeColor = System.Drawing.Color.Transparent;
            this.btn_close_login_form.Location = new System.Drawing.Point(450, 6);
            this.btn_close_login_form.Name = "btn_close_login_form";
            this.btn_close_login_form.Size = new System.Drawing.Size(13, 12);
            this.btn_close_login_form.TabIndex = 0;
            this.btn_close_login_form.UseVisualStyleBackColor = false;
            this.btn_close_login_form.Click += new System.EventHandler(this.btn_close_login_form_Click);
            // 
            // UserName_Txt
            // 
            this.UserName_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.UserName_Txt.Location = new System.Drawing.Point(142, 64);
            this.UserName_Txt.Name = "UserName_Txt";
            this.UserName_Txt.Size = new System.Drawing.Size(232, 20);
            this.UserName_Txt.TabIndex = 4;
            this.UserName_Txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_login
            // 
            this.btn_login.BackColor = System.Drawing.Color.DodgerBlue;
            this.btn_login.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_login.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(238)))));
            this.btn_login.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_login.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_login.ForeColor = System.Drawing.Color.White;
            this.btn_login.Image = ((System.Drawing.Image)(resources.GetObject("btn_login.Image")));
            this.btn_login.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_login.Location = new System.Drawing.Point(270, 136);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(104, 27);
            this.btn_login.TabIndex = 6;
            this.btn_login.Text = "Login";
            this.btn_login.UseVisualStyleBackColor = false;
            this.btn_login.Click += new System.EventHandler(this.Login_Btn_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Enabled = false;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.Transparent;
            this.button1.Location = new System.Drawing.Point(3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(19, 20);
            this.button1.TabIndex = 13;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Enabled = false;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.Transparent;
            this.button2.Location = new System.Drawing.Point(93, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(43, 37);
            this.button2.TabIndex = 11;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // lbl_Error_loginform
            // 
            this.lbl_Error_loginform.AutoSize = true;
            this.lbl_Error_loginform.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(238)))));
            this.lbl_Error_loginform.ForeColor = System.Drawing.Color.DarkRed;
            this.lbl_Error_loginform.Location = new System.Drawing.Point(139, 174);
            this.lbl_Error_loginform.Name = "lbl_Error_loginform";
            this.lbl_Error_loginform.Size = new System.Drawing.Size(0, 13);
            this.lbl_Error_loginform.TabIndex = 13;
            // 
            // btn_minimize_login_form
            // 
            this.btn_minimize_login_form.BackColor = System.Drawing.Color.Transparent;
            this.btn_minimize_login_form.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_minimize_login_form.BackgroundImage")));
            this.btn_minimize_login_form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_minimize_login_form.FlatAppearance.BorderSize = 0;
            this.btn_minimize_login_form.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_minimize_login_form.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_minimize_login_form.ForeColor = System.Drawing.Color.Black;
            this.btn_minimize_login_form.Location = new System.Drawing.Point(424, 4);
            this.btn_minimize_login_form.Name = "btn_minimize_login_form";
            this.btn_minimize_login_form.Size = new System.Drawing.Size(16, 15);
            this.btn_minimize_login_form.TabIndex = 14;
            this.btn_minimize_login_form.UseVisualStyleBackColor = false;
            this.btn_minimize_login_form.Click += new System.EventHandler(this.btn_minimize_login_form_Click);
            // 
            // pnl_controls_loginform
            // 
            this.pnl_controls_loginform.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(238)))));
            this.pnl_controls_loginform.Controls.Add(this.button1);
            this.pnl_controls_loginform.Controls.Add(this.btn_close_login_form);
            this.pnl_controls_loginform.Controls.Add(this.btn_minimize_login_form);
            this.pnl_controls_loginform.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_controls_loginform.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnl_controls_loginform.Location = new System.Drawing.Point(0, 0);
            this.pnl_controls_loginform.Name = "pnl_controls_loginform";
            this.pnl_controls_loginform.Size = new System.Drawing.Size(472, 23);
            this.pnl_controls_loginform.TabIndex = 9;
            this.pnl_controls_loginform.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnl_controls_loginform_MouseDown);
            this.pnl_controls_loginform.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnl_controls_loginform_MouseMove);
            this.pnl_controls_loginform.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnl_controls_loginform_MouseUp);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btn_login;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(46)))), ((int)(((byte)(62)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btn_close_login_form;
            this.ClientSize = new System.Drawing.Size(472, 196);
            this.Controls.Add(this.lbl_Error_loginform);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pnl_controls_loginform);
            this.Controls.Add(this.UserName_Txt);
            this.Controls.Add(this.Password_Txt);
            this.Controls.Add(this.btn_login);
            this.Controls.Add(this.passwordpic_btn);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(472, 196);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login Page";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
            this.pnl_controls_loginform.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox Password_Txt;
        private System.Windows.Forms.Button passwordpic_btn;
        private System.Windows.Forms.Button btn_close_login_form;
        private System.Windows.Forms.TextBox UserName_Txt;
        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbl_Error_loginform;
        private System.Windows.Forms.Button btn_minimize_login_form;
        private System.Windows.Forms.Panel pnl_controls_loginform;
    }
}

