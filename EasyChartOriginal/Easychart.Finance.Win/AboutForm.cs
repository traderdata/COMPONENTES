using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Show the copyright information of easy stock chart windows control
	/// </summary>
	[ToolboxItem(false)]
	public class AboutForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView lvAssembly;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnSystemInfo;
		private System.Windows.Forms.LinkLabel lbSite;
		private System.Windows.Forms.ColumnHeader chName;
		private System.Windows.Forms.ColumnHeader chVersion;
		private System.Windows.Forms.Label lVersion;
		private System.Windows.Forms.Label lCopyright;
		private System.Windows.Forms.Label lName;
		private System.Windows.Forms.Label lComponent;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel lbEMail;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Default about box
		/// </summary>
		public AboutForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.lName = new System.Windows.Forms.Label();
			this.lvAssembly = new System.Windows.Forms.ListView();
			this.chName = new System.Windows.Forms.ColumnHeader();
			this.chVersion = new System.Windows.Forms.ColumnHeader();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnSystemInfo = new System.Windows.Forms.Button();
			this.lbSite = new System.Windows.Forms.LinkLabel();
			this.lVersion = new System.Windows.Forms.Label();
			this.lCopyright = new System.Windows.Forms.Label();
			this.lComponent = new System.Windows.Forms.Label();
			this.lbEMail = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lName
			// 
			this.lName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lName.Location = new System.Drawing.Point(15, 16);
			this.lName.Name = "lName";
			this.lName.Size = new System.Drawing.Size(360, 16);
			this.lName.TabIndex = 0;
			this.lName.Text = "Easy Financial Chart Windows Demo";
			// 
			// lvAssembly
			// 
			this.lvAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvAssembly.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.chName,
																						 this.chVersion});
			this.lvAssembly.GridLines = true;
			this.lvAssembly.Location = new System.Drawing.Point(16, 77);
			this.lvAssembly.Name = "lvAssembly";
			this.lvAssembly.Size = new System.Drawing.Size(456, 123);
			this.lvAssembly.TabIndex = 1;
			this.lvAssembly.View = System.Windows.Forms.View.Details;
			// 
			// chName
			// 
			this.chName.Text = "Name";
			this.chName.Width = 205;
			// 
			// chVersion
			// 
			this.chVersion.Text = "Version";
			this.chVersion.Width = 161;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(368, 217);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(96, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			// 
			// btnSystemInfo
			// 
			this.btnSystemInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSystemInfo.Location = new System.Drawing.Point(368, 249);
			this.btnSystemInfo.Name = "btnSystemInfo";
			this.btnSystemInfo.Size = new System.Drawing.Size(96, 23);
			this.btnSystemInfo.TabIndex = 3;
			this.btnSystemInfo.Text = "System Info";
			this.btnSystemInfo.Click += new System.EventHandler(this.btnSystemInfo_Click);
			// 
			// lbSite
			// 
			this.lbSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lbSite.AutoSize = true;
			this.lbSite.Location = new System.Drawing.Point(80, 216);
			this.lbSite.Name = "lbSite";
			this.lbSite.Size = new System.Drawing.Size(166, 17);
			this.lbSite.TabIndex = 4;
			this.lbSite.TabStop = true;
			this.lbSite.Text = "http://finance.easychart.net";
			this.lbSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbSite_LinkClicked);
			// 
			// lVersion
			// 
			this.lVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lVersion.AutoSize = true;
			this.lVersion.Location = new System.Drawing.Point(15, 37);
			this.lVersion.Name = "lVersion";
			this.lVersion.Size = new System.Drawing.Size(65, 17);
			this.lVersion.TabIndex = 5;
			this.lVersion.Text = "<Version>";
			// 
			// lCopyright
			// 
			this.lCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lCopyright.AutoSize = true;
			this.lCopyright.Location = new System.Drawing.Point(15, 264);
			this.lCopyright.Name = "lCopyright";
			this.lCopyright.Size = new System.Drawing.Size(85, 17);
			this.lCopyright.TabIndex = 6;
			this.lCopyright.Text = "<Copy Right>";
			// 
			// lComponent
			// 
			this.lComponent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lComponent.AutoSize = true;
			this.lComponent.Location = new System.Drawing.Point(15, 58);
			this.lComponent.Name = "lComponent";
			this.lComponent.Size = new System.Drawing.Size(126, 17);
			this.lComponent.TabIndex = 7;
			this.lComponent.Text = "Product components:";
			// 
			// lbEMail
			// 
			this.lbEMail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lbEMail.AutoSize = true;
			this.lbEMail.Location = new System.Drawing.Point(80, 240);
			this.lbEMail.Name = "lbEMail";
			this.lbEMail.Size = new System.Drawing.Size(177, 17);
			this.lbEMail.TabIndex = 8;
			this.lbEMail.TabStop = true;
			this.lbEMail.Text = "mailto:support@easychart.net";
			this.lbEMail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbSite_LinkClicked);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(16, 217);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Home:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 240);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 17);
			this.label2.TabIndex = 10;
			this.label2.Text = "Support:";
			// 
			// AboutForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(490, 304);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lbEMail);
			this.Controls.Add(this.lComponent);
			this.Controls.Add(this.lCopyright);
			this.Controls.Add(this.lVersion);
			this.Controls.Add(this.lbSite);
			this.Controls.Add(this.btnSystemInfo);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lvAssembly);
			this.Controls.Add(this.lName);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			this.Load += new System.EventHandler(this.AboutForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void AboutForm_Load(object sender, System.EventArgs e)
		{
			Assembly a = Assembly.GetExecutingAssembly();
			lCopyright.Text = ((AssemblyCopyrightAttribute)(Attribute.GetCustomAttribute(a,typeof(AssemblyCopyrightAttribute)))).Copyright;
			lVersion.Text = "Version : "+Application.ProductVersion;
			lName.Text = Application.ProductName;
			lbSite.Links[0].LinkData = lbSite.Text;
			lbEMail.Links[0].LinkData = lbEMail.Text;
			foreach(AssemblyName an in a.GetReferencedAssemblies())
			{
				ListViewItem lvi = lvAssembly.Items.Add(an.Name);
				lvi.SubItems.Add(an.Version.ToString());
			}
		}

		private void btnSystemInfo_Click(object sender, System.EventArgs e)
		{
			Process.Start("MSInfo32.exe");
		}

		private void lbSite_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(e.Link.LinkData.ToString());
		}

		/// <summary>
		/// Show the about box
		/// </summary>
		public static void ShowForm() 
		{
			AboutForm af = new AboutForm();
			af.ShowDialog();
		}
	}
}
