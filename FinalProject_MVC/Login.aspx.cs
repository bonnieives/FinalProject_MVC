using FinalProject_MVC.DAL;
using System;
using System.Linq;
using System.Web.UI;

namespace FinalProject_MVC
{
    public partial class Login : Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();

            using (FinalProjectContext db = new FinalProjectContext())
            {
                bool isValidUser = db.Users.Any(u => u.Email == username && u.Password == password);

                if (isValidUser)
                {
                    Response.Redirect("~/");
                }
                else
                {
                    lblErrorMessage.Visible = true;
                    lblErrorMessage.Text = "Invalid username or password.";
                }
            }
        }
    }
}
