using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TETRA_Coverage_Monitor
{
    public partial class LoginForm : Form
    {
        SDS_Remote_WS WS = new SDS_Remote_WS();

        private bool maxwindow = false;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void Login_Btn_Click(object sender, EventArgs e)
        {

            if (Password_Txt.Text != string.Empty || UserName_Txt.Text != string.Empty)
            {
                Code.Singleton.user_obj = WS.Users_Select(UserName_Txt.Text, Password_Txt.Text);

                if (Code.Singleton.user_obj != null)
                {
                    this.Hide();
                    View home = new View();
                    home.ShowDialog();
                }
                else
                {
                  lbl_Error_loginform.Text = "No Account avilable with this username and password ";
                }
            }
            else
            {
                lbl_Error_loginform.Text = "Please enter value in all field.";
            }
          }


        private void LoginForm_Load(object sender, EventArgs e)
        {
        }

        #region form buttons
        private void btn_maximize_login_form_Click(object sender, EventArgs e)
        {
            if(maxwindow)
            {
                this.WindowState = FormWindowState.Normal;
                maxwindow = false;
            }
            else
            {
                this.MinimumSize = this.Size;
                this.WindowState = FormWindowState.Maximized;
                this.MaximumSize = this.Size;
                maxwindow = true;
            }

        }

        private void btn_close_login_form_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_minimize_login_form_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region form moveable
        private bool mouseDown;
        private Point lastLocation;

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location =  new Point( (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void LoginForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void pnl_controls_loginform_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void pnl_controls_loginform_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                LoginForm form = this;
                form.Location = new Point((form.Location.X - lastLocation.X) + e.X, (form.Location.Y - lastLocation.Y) + e.Y);
                form.Update();
            }
        }

        private void pnl_controls_loginform_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;

        }
        #endregion
    }
}
