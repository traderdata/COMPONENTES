using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

namespace WebDemos.Admin
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class Login : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox tbUserName;
		protected System.Web.UI.WebControls.TextBox tbPassword;
		protected System.Web.UI.WebControls.Label lMsg;
		protected System.Web.UI.WebControls.Button btnLogin;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnLogin_Click(object sender, System.EventArgs e)
		{
			if (FormsAuthentication.Authenticate(tbUserName.Text,tbPassword.Text))
			{
				FormsAuthentication.SetAuthCookie(tbUserName.Text,false);
				FormsAuthentication.RedirectFromLoginPage(tbUserName.Text,false);
			} 
			else 
			{
				lMsg.Text ="<b>Something went wrong...</b> please re-enter your credentials...";
			}
		}
	}
}
