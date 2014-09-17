using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// This is a simple input box
	/// </summary>
	[ToolboxItem(false)]
	public class InputBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lCaption;
		private System.Windows.Forms.TextBox tbData;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private InputBox()
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

		/// <summary>
		/// Show the input box
		/// </summary>
		/// <param name="Caption">Input box caption</param>
		/// <param name="Default">Default value in the input box</param>
		/// <returns>The value returned by the input box</returns>
		public static string ShowInputBox(string Caption,string Default) 
		{
			InputBox box = new InputBox();
			box.lCaption.Text = Caption;
			box.tbData.Text = Default;
			if (box.ShowDialog()==DialogResult.OK) 
				return box.tbData.Text;
			else return "";
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lCaption = new System.Windows.Forms.Label();
			this.tbData = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lCaption
			// 
			this.lCaption.Location = new System.Drawing.Point(8, 11);
			this.lCaption.Name = "lCaption";
			this.lCaption.Size = new System.Drawing.Size(104, 16);
			this.lCaption.TabIndex = 0;
			this.lCaption.Text = "Your Text:";
			this.lCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbData
			// 
			this.tbData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbData.Location = new System.Drawing.Point(8, 37);
			this.tbData.Name = "tbData";
			this.tbData.Size = new System.Drawing.Size(400, 21);
			this.tbData.TabIndex = 1;
			this.tbData.Text = "";
			this.tbData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbData_KeyDown);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(333, 65);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			// 
			// InputBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(418, 95);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tbData);
			this.Controls.Add(this.lCaption);
			this.Font = new System.Drawing.Font("Verdana", 8.5F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "InputBox";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputBox_KeyDown);
			this.ResumeLayout(false);

		}
		#endregion

		private void tbData_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode==Keys.Enter)
				btnOK.PerformClick();
		}

		private void InputBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode==Keys.Escape)
				Close();
		}
	}
}
