using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using Easychart.Finance.DataClient;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// DataFeed client login dialog 
	/// </summary>
	[ToolboxItem(false)]
	public class DataFeedLogin : System.Windows.Forms.Form
	{
		private System.Windows.Forms.LinkLabel lbReg;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.TextBox tbUserName;
		private System.Windows.Forms.Label lPassword;
		private System.Windows.Forms.Label lUserName;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lMsg;

		private DataClientBase DataClient;
		private System.Windows.Forms.LinkLabel lbHomePage;
		private System.Windows.Forms.LinkLabel lbLogin;
	
		/// <summary>
		/// Get current login Username
		/// </summary>
		public string Username
		{
			get
			{
				return tbUserName.Text;
			}
		}

		/// <summary>
		/// Get current login password
		/// </summary>
		public string Password
		{
			get
			{
				return tbPassword.Text;
			}
		}

		static private DataFeedLogin current;
		/// <summary>
		/// Current data feed login dialog
		/// </summary>
		static public DataFeedLogin Current
		{
			get
			{
				if (current==null)
					current = new DataFeedLogin();
				return current;
			}
		}

		/// <summary>
		/// Show the login dialog
		/// </summary>
		/// <param name="DataClient">Data client</param>
		/// <returns></returns>
		static public DialogResult Login(DataClientBase DataClient)
		{
			Current.DataClient = DataClient;
			return Current.ShowDialog();
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		
		private DataFeedLogin()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			this.lbReg = new System.Windows.Forms.LinkLabel();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.tbUserName = new System.Windows.Forms.TextBox();
			this.lPassword = new System.Windows.Forms.Label();
			this.lUserName = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.lMsg = new System.Windows.Forms.Label();
			this.lbHomePage = new System.Windows.Forms.LinkLabel();
			this.lbLogin = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// lbReg
			// 
			this.lbReg.AutoSize = true;
			this.lbReg.Location = new System.Drawing.Point(176, 8);
			this.lbReg.Name = "lbReg";
			this.lbReg.Size = new System.Drawing.Size(184, 17);
			this.lbReg.TabIndex = 21;
			this.lbReg.TabStop = true;
			this.lbReg.Text = "Not a member? Register Here!";
			this.lbReg.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbReg_LinkClicked);
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(288, 40);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.PasswordChar = '*';
			this.tbPassword.Size = new System.Drawing.Size(128, 21);
			this.tbPassword.TabIndex = 18;
			this.tbPassword.Text = ((string)(configurationAppSettings.GetValue("tbPassword.Text", typeof(string))));
			// 
			// tbUserName
			// 
			this.tbUserName.Location = new System.Drawing.Point(96, 40);
			this.tbUserName.Name = "tbUserName";
			this.tbUserName.Size = new System.Drawing.Size(112, 21);
			this.tbUserName.TabIndex = 17;
			this.tbUserName.Text = ((string)(configurationAppSettings.GetValue("tbUserName.Text", typeof(string))));
			// 
			// lPassword
			// 
			this.lPassword.Location = new System.Drawing.Point(220, 40);
			this.lPassword.Name = "lPassword";
			this.lPassword.Size = new System.Drawing.Size(65, 15);
			this.lPassword.TabIndex = 20;
			this.lPassword.Text = "Password:";
			// 
			// lUserName
			// 
			this.lUserName.Location = new System.Drawing.Point(16, 40);
			this.lUserName.Name = "lUserName";
			this.lUserName.Size = new System.Drawing.Size(74, 15);
			this.lUserName.TabIndex = 19;
			this.lUserName.Text = "User Name:";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(342, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 24;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(246, 72);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(83, 23);
			this.btnOK.TabIndex = 23;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lMsg
			// 
			this.lMsg.AutoSize = true;
			this.lMsg.Location = new System.Drawing.Point(16, 72);
			this.lMsg.Name = "lMsg";
			this.lMsg.Size = new System.Drawing.Size(54, 17);
			this.lMsg.TabIndex = 25;
			this.lMsg.Text = "Message";
			// 
			// lbHomePage
			// 
			this.lbHomePage.AutoSize = true;
			this.lbHomePage.Location = new System.Drawing.Point(16, 8);
			this.lbHomePage.Name = "lbHomePage";
			this.lbHomePage.Size = new System.Drawing.Size(67, 17);
			this.lbHomePage.TabIndex = 26;
			this.lbHomePage.TabStop = true;
			this.lbHomePage.Text = "HomePage";
			this.lbHomePage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbReg_LinkClicked);
			// 
			// lbLogin
			// 
			this.lbLogin.AutoSize = true;
			this.lbLogin.Location = new System.Drawing.Point(96, 8);
			this.lbLogin.Name = "lbLogin";
			this.lbLogin.Size = new System.Drawing.Size(68, 17);
			this.lbLogin.TabIndex = 27;
			this.lbLogin.TabStop = true;
			this.lbLogin.Text = "Login Page";
			this.lbLogin.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbReg_LinkClicked);
			// 
			// DataFeedLogin
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(434, 111);
			this.Controls.Add(this.lbLogin);
			this.Controls.Add(this.lbHomePage);
			this.Controls.Add(this.lMsg);
			this.Controls.Add(this.lbReg);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.tbUserName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lPassword);
			this.Controls.Add(this.lUserName);
			this.Font = new System.Drawing.Font("Verdana", 8.5F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataFeedLogin";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Login for";
			this.Activated += new System.EventHandler(this.DataFeedLogin_Activated);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			lMsg.Text = "Connecting...";
			Application.DoEvents();
			if (DataClient.Login(Username,Password)) 
			{
				lMsg.Text ="Login successfully!";
				DialogResult = DialogResult.OK;
			}
			else lMsg.Text = "Login failed!";
			Application.DoEvents();
		}

		private void lbReg_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(e.Link.LinkData.ToString());
		}

		private void DataFeedLogin_Activated(object sender, System.EventArgs e)
		{
			lMsg.Text = "";
			if (DataClient!=null) 
			{
				Text = DataClient.DataFeedName;
				lbReg.Links[0].LinkData = DataClient.RegURL;
				lbReg.Visible = DataClient.RegURL!="";

				lbHomePage.Links[0].LinkData = DataClient.HomePage;
				lbHomePage.Visible = DataClient.HomePage!="";

				lbLogin.Links[0].LinkData = DataClient.LoginURL;
				lbLogin.Visible = DataClient.LoginURL!="";
			}
		}
	}
}
