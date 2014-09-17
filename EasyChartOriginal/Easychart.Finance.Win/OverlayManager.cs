using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Manage overlay indicators of the chart
	/// </summary>
	[ToolboxItem(false)]
	public class OverlayManager : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox ddlAddOverlay;
		private System.Windows.Forms.TextBox tbP1;
		private System.Windows.Forms.TextBox tbP2;
		private System.Windows.Forms.TextBox tbP3;
		private System.Windows.Forms.ListBox lbOverlayList;
		private System.Windows.Forms.GroupBox gbParameter;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox tbP4;
		private System.Windows.Forms.GroupBox gbCurrent;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnSource;
		private bool DisableChangeEvent;

		/// <summary>
		/// Create instance of OverlayManager
		/// </summary>
		public OverlayManager()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ddlAddOverlay.Items.Clear();
			ddlAddOverlay.Items.Add("Select indicators to add");
			foreach(string s in new string[]{"HL","MA","EMA","BB",
				"AreaBB","SAR","ZIGLABEL","ZIG","ZIGW","ZIGSR","SR","EXTEND.SRAxisY","COMPARE","COMPARE2",
				"Fibonnaci","LinRegr"})
				ddlAddOverlay.Items.Add(FormulaBase.GetFormulaByName(s));

			ddlAddOverlay.SelectedIndex = 0;
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
			this.ddlAddOverlay = new System.Windows.Forms.ComboBox();
			this.lbOverlayList = new System.Windows.Forms.ListBox();
			this.tbP1 = new System.Windows.Forms.TextBox();
			this.tbP2 = new System.Windows.Forms.TextBox();
			this.tbP3 = new System.Windows.Forms.TextBox();
			this.gbParameter = new System.Windows.Forms.GroupBox();
			this.tbP4 = new System.Windows.Forms.TextBox();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.gbCurrent = new System.Windows.Forms.GroupBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSource = new System.Windows.Forms.Button();
			this.gbParameter.SuspendLayout();
			this.gbCurrent.SuspendLayout();
			this.SuspendLayout();
			// 
			// ddlAddOverlay
			// 
			this.ddlAddOverlay.DisplayMember = "CombineName";
			this.ddlAddOverlay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAddOverlay.Items.AddRange(new object[] {
															   "HL=Horizontal Line",
															   "MA=Simple Moving Average",
															   "EMA=Exponential Moving Average"});
			this.ddlAddOverlay.Location = new System.Drawing.Point(16, 16);
			this.ddlAddOverlay.MaxDropDownItems = 12;
			this.ddlAddOverlay.Name = "ddlAddOverlay";
			this.ddlAddOverlay.Size = new System.Drawing.Size(259, 21);
			this.ddlAddOverlay.TabIndex = 0;
			this.ddlAddOverlay.SelectedIndexChanged += new System.EventHandler(this.ddlAddOverlay_SelectedIndexChanged);
			// 
			// lbOverlayList
			// 
			this.lbOverlayList.DisplayMember = "Description";
			this.lbOverlayList.Location = new System.Drawing.Point(8, 24);
			this.lbOverlayList.Name = "lbOverlayList";
			this.lbOverlayList.Size = new System.Drawing.Size(256, 251);
			this.lbOverlayList.TabIndex = 1;
			this.lbOverlayList.ValueMember = "Name";
			this.lbOverlayList.SelectedValueChanged += new System.EventHandler(this.lbOverlayList_SelectedValueChanged);
			// 
			// tbP1
			// 
			this.tbP1.Location = new System.Drawing.Point(16, 24);
			this.tbP1.Name = "tbP1";
			this.tbP1.TabIndex = 2;
			this.tbP1.Text = "";
			this.tbP1.Leave += new System.EventHandler(this.tbP4_Leave);
			// 
			// tbP2
			// 
			this.tbP2.Location = new System.Drawing.Point(16, 64);
			this.tbP2.Name = "tbP2";
			this.tbP2.TabIndex = 3;
			this.tbP2.Text = "";
			this.tbP2.Leave += new System.EventHandler(this.tbP4_Leave);
			// 
			// tbP3
			// 
			this.tbP3.Location = new System.Drawing.Point(16, 104);
			this.tbP3.Name = "tbP3";
			this.tbP3.TabIndex = 4;
			this.tbP3.Text = "";
			this.tbP3.Leave += new System.EventHandler(this.tbP4_Leave);
			// 
			// gbParameter
			// 
			this.gbParameter.Controls.Add(this.tbP4);
			this.gbParameter.Controls.Add(this.tbP3);
			this.gbParameter.Controls.Add(this.tbP2);
			this.gbParameter.Controls.Add(this.tbP1);
			this.gbParameter.Location = new System.Drawing.Point(273, 18);
			this.gbParameter.Name = "gbParameter";
			this.gbParameter.Size = new System.Drawing.Size(143, 223);
			this.gbParameter.TabIndex = 5;
			this.gbParameter.TabStop = false;
			this.gbParameter.Text = "Parameters";
			// 
			// tbP4
			// 
			this.tbP4.Location = new System.Drawing.Point(16, 144);
			this.tbP4.Name = "tbP4";
			this.tbP4.TabIndex = 5;
			this.tbP4.Text = "";
			this.tbP4.Leave += new System.EventHandler(this.tbP4_Leave);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(274, 250);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(62, 23);
			this.btnDelete.TabIndex = 6;
			this.btnDelete.Text = "Delete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(16, 344);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "&OK";
			// 
			// gbCurrent
			// 
			this.gbCurrent.Controls.Add(this.btnSource);
			this.gbCurrent.Controls.Add(this.lbOverlayList);
			this.gbCurrent.Controls.Add(this.gbParameter);
			this.gbCurrent.Controls.Add(this.btnDelete);
			this.gbCurrent.Location = new System.Drawing.Point(16, 48);
			this.gbCurrent.Name = "gbCurrent";
			this.gbCurrent.Size = new System.Drawing.Size(424, 288);
			this.gbCurrent.TabIndex = 8;
			this.gbCurrent.TabStop = false;
			this.gbCurrent.Text = "Current Overlays";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(104, 344);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "&Cancel";
			// 
			// btnSource
			// 
			this.btnSource.Location = new System.Drawing.Point(344, 251);
			this.btnSource.Name = "btnSource";
			this.btnSource.Size = new System.Drawing.Size(64, 23);
			this.btnSource.TabIndex = 7;
			this.btnSource.Text = "Source";
			this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
			// 
			// OverlayManager
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(450, 375);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.gbCurrent);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.ddlAddOverlay);
			this.Font = new System.Drawing.Font("Verdana", 8.5F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "OverlayManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Overlay Manager";
			this.gbParameter.ResumeLayout(false);
			this.gbCurrent.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void AddFormulas(string Name)
		{
			lbOverlayList.Items.Add(Name);
		}

		private void AddFormulas()
		{
			FormulaBase fb = (FormulaBase)ddlAddOverlay.SelectedItem;
			if (fb!=null)
				AddFormulas(fb.FullName.Substring(4));
		}

		private void SelectOverlay(int Index)
		{
			if (Index<0) Index = 0;
			if (Index>=lbOverlayList.Items.Count) Index = lbOverlayList.Items.Count-1;
			if (Index>=0 && Index<lbOverlayList.Items.Count)
				lbOverlayList.SelectedIndex = Index;
		}

		private void SelectFirstOverlay()
		{
			SelectOverlay(0);
		}

		private void SelectLastOverlay()
		{
			SelectOverlay(lbOverlayList.Items.Count-1);
		}

		private void DeleteFormulas()
		{
			DisableChangeEvent = true;
			int i = lbOverlayList.SelectedIndex;
			if (i>=0)
			{
				try 
				{
					lbOverlayList.Items.RemoveAt(i);
				}
				finally
				{
					DisableChangeEvent = false;
				}
				SelectOverlay(i);
			}
		}

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			DeleteFormulas();
		}

		private void lbOverlayList_SelectedValueChanged(object sender, System.EventArgs e)
		{
			if (!DisableChangeEvent) 
			{
				string FormulaName;
				SelectParameter.ParamToTextBox(
					(string)lbOverlayList.SelectedItem,
					new TextBox[]{tbP1,tbP2,tbP3,tbP4},
					out FormulaName);
			}
		}

		private void ddlAddOverlay_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (ddlAddOverlay.SelectedIndex>0) 
			{
				AddFormulas();
				ddlAddOverlay.SelectedIndex = 0;
				SelectLastOverlay();
			}
		}

		private void tbP4_Leave(object sender, System.EventArgs e)
		{
			string s = SelectParameter.TextBoxToParam((string)lbOverlayList.SelectedItem,
				new TextBox[]{tbP1,tbP2,tbP3,tbP4});
			if (s!="")
			{
				DisableChangeEvent = true;
				try 
				{
					lbOverlayList.Items[lbOverlayList.SelectedIndex] = s;
				} 
				finally
				{
					DisableChangeEvent  = false;
				}
			}
		}

		/// <summary>
		/// Get or set current overlay formula string
		/// </summary>
		public string CurrentOverlay
		{
			get
			{
				string s = "";
				foreach(string r in lbOverlayList.Items)
				{
					if (s!="") s +=";";
					s += r;
				}
				return s;
			}
			set
			{
				lbOverlayList.Items.Clear();
				if (value!="" && value!=null)
				{
					foreach(string s in value.Split(';'))
						AddFormulas(s);
					SelectFirstOverlay();
				}
			}
		}

		/// <summary>
		/// Show overlay manager form
		/// </summary>
		/// <param name="DefaultOverlay">Default overlay formulas</param>
		/// <returns></returns>
		public DialogResult ShowForm(string DefaultOverlay)
		{
			this.CurrentOverlay = DefaultOverlay;
			return ShowDialog();
		}

		private void btnSource_Click(object sender, System.EventArgs e)
		{
			int i = lbOverlayList.SelectedIndex;
			if (i>=0)
			{
				FormulaBase fb = FormulaBase.GetFormulaByName((string)lbOverlayList.Items[i]);
				ChartWinControl.EditFormula(fb);
			}
		}
	}
}