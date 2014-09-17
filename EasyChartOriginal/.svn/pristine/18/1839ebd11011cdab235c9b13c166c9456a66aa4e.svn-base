using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ImgControl.
	/// </summary>
	public class ImgControl : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListBox lbImgName;
		private System.Windows.Forms.Button btnDel;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.OpenFileDialog odIcon;
		private IWindowsFormsEditorService edSvc;
		public string ImgName;

		public ImgControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		private void LoadFileList()
		{
			string s = FormulaHelper.ImageRoot;
			string[] ss = Directory.GetFiles(s);
			for(int i=0; i<ss.Length; i++)
				ss[i] = Path.GetFileName(ss[i]);
			lbImgName.Items.Clear();
			lbImgName.Items.AddRange(ss);
		}

		public ImgControl(string Value,IWindowsFormsEditorService edSvc) : this()
		{
			ImgName = Value;
			this.edSvc = edSvc;
			LoadFileList();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbImgName = new System.Windows.Forms.ListBox();
			this.btnDel = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.odIcon = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// lbImgName
			// 
			this.lbImgName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbImgName.Location = new System.Drawing.Point(0, 0);
			this.lbImgName.Name = "lbImgName";
			this.lbImgName.Size = new System.Drawing.Size(168, 184);
			this.lbImgName.TabIndex = 0;
			this.lbImgName.DoubleClick += new System.EventHandler(this.lbImgName_DoubleClick);
			this.lbImgName.SelectedIndexChanged += new System.EventHandler(this.lbImgName_SelectedIndexChanged);
			// 
			// btnDel
			// 
			this.btnDel.Location = new System.Drawing.Point(11, 190);
			this.btnDel.Name = "btnDel";
			this.btnDel.Size = new System.Drawing.Size(56, 20);
			this.btnDel.TabIndex = 1;
			this.btnDel.Text = "&Delete";
			this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(88, 190);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(56, 20);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "&Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// ImgControl
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnDel);
			this.Controls.Add(this.lbImgName);
			this.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.Name = "ImgControl";
			this.Size = new System.Drawing.Size(168, 216);
			this.ResumeLayout(false);

		}
		#endregion

		private void lbImgName_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ImgName = lbImgName.SelectedItem.ToString();
		}

		private void lbImgName_DoubleClick(object sender, System.EventArgs e)
		{
			edSvc.CloseDropDown();
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			if (odIcon.ShowDialog()==DialogResult.OK)
			{
				string s = FormulaHelper.ImageRoot;
				string Filename = odIcon.FileName;
				string r = Path.GetFileName(Filename);
				try
				{
					File.Copy(Filename, s+r,true);
					ImgName = r;
					LoadFileList();
				} 
				catch
				{
				}
			}
		}

		private void btnDel_Click(object sender, System.EventArgs e)
		{
			if (lbImgName.SelectedItem!=null)
			{
				string s = FormulaHelper.ImageRoot;
				try
				{
					File.Delete(s+lbImgName.SelectedItem);
					LoadFileList();
				} 
				catch
				{
				}
			}
		}
	}
}
