using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Easychart.Finance;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Select parameters for a formula
	/// </summary>
	[ToolboxItem(false)]
	public class SelectParameter : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox tb1;
		private System.Windows.Forms.TextBox tb2;
		private System.Windows.Forms.TextBox tb3;
		private System.Windows.Forms.TextBox tb4;
		private System.Windows.Forms.Button btnOK;

		private TextBox[] tbParams;
		private System.Windows.Forms.Label lParameter;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnCancel;
		private string FormulaName;

		/// <summary>
		/// Create instance of the SelectParameter
		/// </summary>
		public SelectParameter()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			tbParams =new TextBox[] {tb1,tb2,tb3,tb4};
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
			this.tb1 = new System.Windows.Forms.TextBox();
			this.tb2 = new System.Windows.Forms.TextBox();
			this.tb3 = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.lParameter = new System.Windows.Forms.Label();
			this.tb4 = new System.Windows.Forms.TextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tb1
			// 
			this.tb1.Location = new System.Drawing.Point(48, 48);
			this.tb1.Name = "tb1";
			this.tb1.Size = new System.Drawing.Size(176, 22);
			this.tb1.TabIndex = 1;
			this.tb1.Text = "";
			// 
			// tb2
			// 
			this.tb2.Location = new System.Drawing.Point(48, 80);
			this.tb2.Name = "tb2";
			this.tb2.Size = new System.Drawing.Size(176, 22);
			this.tb2.TabIndex = 2;
			this.tb2.Text = "";
			// 
			// tb3
			// 
			this.tb3.Location = new System.Drawing.Point(48, 112);
			this.tb3.Name = "tb3";
			this.tb3.Size = new System.Drawing.Size(176, 22);
			this.tb3.TabIndex = 3;
			this.tb3.Text = "";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(88, 184);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			// 
			// lParameter
			// 
			this.lParameter.AutoSize = true;
			this.lParameter.Location = new System.Drawing.Point(13, 18);
			this.lParameter.Name = "lParameter";
			this.lParameter.Size = new System.Drawing.Size(119, 18);
			this.lParameter.TabIndex = 5;
			this.lParameter.Text = "Adjust parameter:";
			// 
			// tb4
			// 
			this.tb4.Location = new System.Drawing.Point(48, 144);
			this.tb4.Name = "tb4";
			this.tb4.Size = new System.Drawing.Size(176, 22);
			this.tb4.TabIndex = 6;
			this.tb4.Text = "";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(160, 184);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			// 
			// SelectFormula
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(242, 215);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tb4);
			this.Controls.Add(this.lParameter);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tb3);
			this.Controls.Add(this.tb2);
			this.Controls.Add(this.tb1);
			this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectFormula";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SelectFormula";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Extract the formula name and and parameters from a string
		/// </summary>
		/// <param name="NameAndParam">Name and parameter string , such as MA(50)</param>
		/// <param name="tbs">TextBox array, the method will extract parameters to this array</param>
		/// <param name="FormulaName">return the formula name</param>
		public static void ParamToTextBox(string NameAndParam,TextBox[] tbs,out string FormulaName)
		{
			FormulaBase fb = FormulaBase.GetFormulaByName(NameAndParam);
			FormulaName = fb.FormulaName;
			for(int i=0; i<tbs.Length; i++) 
			{
				if (i<fb.Params.Count)
					tbs[i].Text = fb.Params[i].Value;
				else tbs[i].Text = "";
			}
		}

		private void SetFormula(string r)
		{
			Text = r;
			ParamToTextBox(r,tbParams,out FormulaName);
		}

		/// <summary>
		/// Create a formula string with parameters
		/// </summary>
		/// <param name="FormulaName">Formula name</param>
		/// <param name="tbs">Parameter TextBox array </param>
		/// <returns>Formula string with parameters</returns>
		public static string TextBoxToParam(string FormulaName,TextBox[] tbs)
		{
			string s = "";
			int j = FormulaName.IndexOf('(');
			if (j>=0)
				FormulaName = FormulaName.Substring(0,j);

			for(int i=0; i<tbs.Length; i++) 
			{
				if (tbs[i].Text!="")
				{
					if (s!="") s+=",";
					s +=tbs[i].Text;
				}
				else break;
			}
			if (s!="") s = "("+s+")";
			FormulaBase fb = FormulaBase.GetFormulaByName(FormulaName);
			if (fb!=null)
				return FormulaName+s;
			else return "";
		}

		private string GetFormula() 
		{
			return TextBoxToParam(FormulaName,tbParams);
		}

		/// <summary>
		/// Show select parameter dialog
		/// </summary>
		/// <param name="Default">Default parameters</param>
		/// <returns>Selected parameters, separated by comma</returns>
		public static string ShowForm(string Default) 
		{
			try 
			{
				SelectParameter sp = new SelectParameter();
				sp.SetFormula(Default);
				if (sp.ShowDialog()==DialogResult.OK) 
					return sp.GetFormula();
			} 
			catch 
			{
			}
			return "";
		}
	}
}
