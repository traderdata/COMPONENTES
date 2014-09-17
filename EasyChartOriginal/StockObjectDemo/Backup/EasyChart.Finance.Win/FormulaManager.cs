using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// The formula manager is used to build formulas from the formula plugins
	/// </summary>
	[ToolboxItem(false)]
	public class FormulaManager : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox gbCurrent;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnMore;

		private bool DisableChangeEvent;
		private System.Windows.Forms.ListBox lbFormulaList;
		private System.Windows.Forms.ComboBox ddlFavoriteFormula;
		private System.ComponentModel.IContainer components;
		private string[] ListedFormulas = {"MAIN","MainArea","VOLMA","MACD","RSI","CMF","TRIX","CCI","FastSTO","SlowSTO","ATR","OBV","ULT","DPO","WR","PPO","PVO","StochRSI","AD",};
		private System.Windows.Forms.Button btnSource;
		private System.Windows.Forms.ImageList ilIcon;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
		public System.Windows.Forms.ImageList ilFormula;
		private System.Windows.Forms.GroupBox gbParam;
		private System.Windows.Forms.ToolTip tpBtnUp;
		private System.Windows.Forms.ToolTip tpBtnDown;
		private System.Windows.Forms.ToolTip tpDelete;
		private System.Windows.Forms.ToolTip tpSource;
		private System.Windows.Forms.CheckBox cbSecondYAxis;
		private string[] OverlayFormulas = {"MAIN","MainArea","HL","MA","EMA","BB",
												"AreaBB","SAR","ZIGLABEL","ZIG","ZIGICON","ZIGW","ZIGSR","SR","SRAxisY","COMPARE","COMPARE2",
												"Fibonnaci","LinRegr","TradingIcon",};

		/// <summary>
		/// Create instance of formula manager
		/// </summary>
		public FormulaManager()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			tpBtnUp.SetToolTip(btnUp,"Move Up");
			tpBtnDown.SetToolTip(btnDown,"Move Down");
			tpDelete.SetToolTip(btnDelete,"Delete");
			tpSource.SetToolTip(btnSource,"Source Code");
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormulaManager));
			this.ddlFavoriteFormula = new System.Windows.Forms.ComboBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbCurrent = new System.Windows.Forms.GroupBox();
			this.btnDown = new System.Windows.Forms.Button();
			this.ilIcon = new System.Windows.Forms.ImageList(this.components);
			this.btnUp = new System.Windows.Forms.Button();
			this.btnSource = new System.Windows.Forms.Button();
			this.lbFormulaList = new System.Windows.Forms.ListBox();
			this.gbParam = new System.Windows.Forms.GroupBox();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnMore = new System.Windows.Forms.Button();
			this.ilFormula = new System.Windows.Forms.ImageList(this.components);
			this.tpBtnUp = new System.Windows.Forms.ToolTip(this.components);
			this.tpBtnDown = new System.Windows.Forms.ToolTip(this.components);
			this.tpDelete = new System.Windows.Forms.ToolTip(this.components);
			this.tpSource = new System.Windows.Forms.ToolTip(this.components);
			this.cbSecondYAxis = new System.Windows.Forms.CheckBox();
			this.gbCurrent.SuspendLayout();
			this.SuspendLayout();
			// 
			// ddlFavoriteFormula
			// 
			this.ddlFavoriteFormula.DisplayMember = "CombineName";
			this.ddlFavoriteFormula.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlFavoriteFormula.Location = new System.Drawing.Point(16, 16);
			this.ddlFavoriteFormula.MaxDropDownItems = 12;
			this.ddlFavoriteFormula.Name = "ddlFavoriteFormula";
			this.ddlFavoriteFormula.Size = new System.Drawing.Size(259, 21);
			this.ddlFavoriteFormula.TabIndex = 0;
			this.ddlFavoriteFormula.SelectedIndexChanged += new System.EventHandler(this.ddlFavoriteFormula_SelectedIndexChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(104, 400);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "&Cancel";
			// 
			// gbCurrent
			// 
			this.gbCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbCurrent.Controls.Add(this.cbSecondYAxis);
			this.gbCurrent.Controls.Add(this.btnDown);
			this.gbCurrent.Controls.Add(this.btnUp);
			this.gbCurrent.Controls.Add(this.btnSource);
			this.gbCurrent.Controls.Add(this.lbFormulaList);
			this.gbCurrent.Controls.Add(this.gbParam);
			this.gbCurrent.Controls.Add(this.btnDelete);
			this.gbCurrent.Location = new System.Drawing.Point(16, 48);
			this.gbCurrent.Name = "gbCurrent";
			this.gbCurrent.Size = new System.Drawing.Size(592, 344);
			this.gbCurrent.TabIndex = 12;
			this.gbCurrent.TabStop = false;
			this.gbCurrent.Text = "Current Formulas";
			// 
			// btnDown
			// 
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnDown.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnDown.ImageIndex = 1;
			this.btnDown.ImageList = this.ilIcon;
			this.btnDown.Location = new System.Drawing.Point(280, 64);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(24, 24);
			this.btnDown.TabIndex = 4;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// ilIcon
			// 
			this.ilIcon.ImageSize = new System.Drawing.Size(20, 20);
			this.ilIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcon.ImageStream")));
			this.ilIcon.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// btnUp
			// 
			this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnUp.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnUp.ImageIndex = 0;
			this.btnUp.ImageList = this.ilIcon;
			this.btnUp.Location = new System.Drawing.Point(280, 32);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(24, 24);
			this.btnUp.TabIndex = 3;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnSource
			// 
			this.btnSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSource.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSource.ImageIndex = 3;
			this.btnSource.ImageList = this.ilIcon;
			this.btnSource.Location = new System.Drawing.Point(280, 129);
			this.btnSource.Name = "btnSource";
			this.btnSource.Size = new System.Drawing.Size(24, 24);
			this.btnSource.TabIndex = 6;
			this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
			// 
			// lbFormulaList
			// 
			this.lbFormulaList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbFormulaList.DisplayMember = "Description";
			this.lbFormulaList.Location = new System.Drawing.Point(8, 24);
			this.lbFormulaList.Name = "lbFormulaList";
			this.lbFormulaList.Size = new System.Drawing.Size(264, 303);
			this.lbFormulaList.TabIndex = 2;
			this.lbFormulaList.ValueMember = "Name";
			this.lbFormulaList.SelectedIndexChanged += new System.EventHandler(this.lbOverlayList_SelectedIndexChanged);
			// 
			// gbParam
			// 
			this.gbParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbParam.Location = new System.Drawing.Point(312, 18);
			this.gbParam.Name = "gbParam";
			this.gbParam.Size = new System.Drawing.Size(272, 287);
			this.gbParam.TabIndex = 7;
			this.gbParam.TabStop = false;
			this.gbParam.Text = "Parameters";
			// 
			// btnDelete
			// 
			this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnDelete.ImageIndex = 2;
			this.btnDelete.ImageList = this.ilIcon;
			this.btnDelete.Location = new System.Drawing.Point(280, 96);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(24, 24);
			this.btnDelete.TabIndex = 5;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(16, 400);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "&OK";
			// 
			// btnMore
			// 
			this.btnMore.Location = new System.Drawing.Point(288, 15);
			this.btnMore.Name = "btnMore";
			this.btnMore.Size = new System.Drawing.Size(72, 23);
			this.btnMore.TabIndex = 1;
			this.btnMore.Text = "More";
			this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
			// 
			// ilFormula
			// 
			this.ilFormula.ImageSize = new System.Drawing.Size(16, 16);
			this.ilFormula.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilFormula.ImageStream")));
			this.ilFormula.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// cbSecondYAxis
			// 
			this.cbSecondYAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSecondYAxis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cbSecondYAxis.Location = new System.Drawing.Point(312, 313);
			this.cbSecondYAxis.Name = "cbSecondYAxis";
			this.cbSecondYAxis.Size = new System.Drawing.Size(208, 19);
			this.cbSecondYAxis.TabIndex = 8;
			this.cbSecondYAxis.Text = "Show in second Y-axis";
			this.cbSecondYAxis.Click += new System.EventHandler(this.cbSecondYAxis_Click);
			this.cbSecondYAxis.CheckedChanged += new System.EventHandler(this.cbSecondYAxis_CheckedChanged);
			// 
			// FormulaManager
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(616, 438);
			this.Controls.Add(this.btnMore);
			this.Controls.Add(this.ddlFavoriteFormula);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.gbCurrent);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Verdana", 8.5F);
			this.MinimumSize = new System.Drawing.Size(400, 400);
			this.Name = "FormulaManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Formula Manager";
			this.gbCurrent.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetSelectFormula(int Index)
		{
			if (Index<0) Index = 0;
			if (Index>=lbFormulaList.Items.Count) Index = lbFormulaList.Items.Count-1;
			if (Index>=0 && Index<lbFormulaList.Items.Count)
				lbFormulaList.SelectedIndex = Index;
		}

		private void SelectFirstFormula()
		{
			SetSelectFormula(0);
		}

		private void SelectLastFormula()
		{
			SetSelectFormula(lbFormulaList.Items.Count-1);
		}

		/// <summary>
		/// Get or set the current formulas, multi formulas was separated by #
		/// </summary>
		public string CurrentFormulas
		{
			get
			{
				string s = "";
				foreach(string r in lbFormulaList.Items)
				{
					if (s!="") s +="#";
					s += r;
				}
				return s;
			}
			set
			{
				lbFormulaList.Items.Clear();
				if (value!="" && value!=null)
				{
					foreach(string s in value.Split('#'))
						AddFormulas(s);
					SelectFirstFormula();
				}
			}
		}

		/// <summary>
		/// Get or set selected formula
		/// </summary>
		public string SelectedFormula
		{
			get
			{
				if (lbFormulaList.SelectedIndex>0)
					return lbFormulaList.Items[lbFormulaList.SelectedIndex].ToString();
				return "";
			}
			set
			{
				int i = lbFormulaList.Items.IndexOf(value);
				if (i>=0)
					lbFormulaList.SelectedIndex = i;
			}
		}

		private void DeleteFormulas()
		{
			DisableChangeEvent = true;
			int i = lbFormulaList.SelectedIndex;
			if (i>=0)
			{
				try 
				{
					lbFormulaList.Items.RemoveAt(i);
				}
				finally
				{
					DisableChangeEvent = false;
				}
				SetSelectFormula(i);
			}
		}

		private void AddFormulas(string Name)
		{
			lbFormulaList.Items.Add(Name);
			SelectLastFormula();
		}

		private void AddFormulas()
		{
			FormulaBase fb = (FormulaBase)ddlFavoriteFormula.SelectedItem;
			if (fb!=null)
				AddFormulas(fb.CreateName);
		}

		private void btnMore_Click(object sender, System.EventArgs e)
		{
			string s = ChartWinControl.DoSelectFormula(null,null,false);
			if (s!=null)
				AddFormulas(s);
		}

		/// <summary>
		/// Create a formula string with parameters
		/// </summary>
		/// <param name="FormulaName">Formula name</param>
		/// <returns>Formula string with parameters</returns>
		private string TextBoxToParam(string FormulaName)
		{
			//string s = "";
			int j = FormulaName.IndexOf('(');
			if (j>=0)
				FormulaName = FormulaName.Substring(0,j);

//			foreach(Control c in gbParam.Controls) 
//				if (c is TextBox) 
//				{
//					if (s!="") s +=",";
//					s +=((TextBox)c).Text;
//				}
//			if (s!="") s = "("+s+")";
			
			string s = SelectFormula.GetParam(gbParam);

			FormulaBase fb = FormulaBase.GetFormulaByName(FormulaName);
			if (fb!=null)
				return FormulaName+s;
			else return "";
		}

		/// <summary>
		/// Extract the formula name and and parameters from a string
		/// </summary>
		/// <param name="NameAndParam">Name and parameter string , such as MA(50)</param>
		/// <param name="tbs">TextBox array, the method will extract parameters to this array</param>
		/// <param name="FormulaName">return the formula name</param>
		public void ParamToTextBox(string NameAndParam,out string FormulaName)
		{
			cbSecondYAxis.Checked = NameAndParam.EndsWith("!");
			FormulaBase fb = FormulaBase.GetFormulaByName(NameAndParam.TrimEnd('!'));
			FormulaName = fb.FormulaName;
			SelectFormula.AddParamToGroupBox(gbParam,ilFormula,fb,new EventHandler(tbP4_Leave));
		}

		private void lbOverlayList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!DisableChangeEvent) 
			{
				string FormulaName;
				ParamToTextBox(
					(string)lbFormulaList.SelectedItem,
					out FormulaName);
			}
		}

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			DeleteFormulas();
		}

		private void tbP4_Leave(object sender, System.EventArgs e)
		{
			string s = TextBoxToParam((string)lbFormulaList.SelectedItem);
			if (s!="")
			{
				DisableChangeEvent = true;
				try
				{
					lbFormulaList.Items[lbFormulaList.SelectedIndex] = s;
				}
				finally
				{
					DisableChangeEvent  = false;
				}
			}
		}

		private void ddlFavoriteFormula_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (ddlFavoriteFormula.SelectedIndex>0)
			{
				AddFormulas();
				ddlFavoriteFormula.SelectedIndex = 0;
			}
		}

		private void AddFavorite(string[] Formulas)
		{
			ddlFavoriteFormula.Items.Clear();
			ddlFavoriteFormula.Items.Add("Select to add new formula");
			foreach(string s in Formulas) 
			{
				FormulaBase fb = FormulaBase.GetFormulaByName(s);
				foreach(object o in ddlFavoriteFormula.Items)
					if (o.GetType()==fb.GetType())
						goto Next;
				ddlFavoriteFormula.Items.Add(fb);
			Next:;
			}
			ddlFavoriteFormula.SelectedIndex = 0;
		}

		/// <summary>
		/// Show the formula manager
		/// </summary>
		/// <param name="Formulas">The default formulas</param>
		/// <returns>Dialog result</returns>
		public DialogResult ShowForm(FormulaArea fa, FormulaBase SelectedFormula)
		{
			if (fa!=null)
			{
				string[] Formulas = fa.FormulaToStrings();
				if (Formulas!=null) 
				{
					this.CurrentFormulas = string.Join("#",Formulas);
					string s = "";
					if (SelectedFormula!=null)
						s = SelectedFormula.CreateName;

					this.SelectedFormula = s;

					ArrayList al = new ArrayList();
					al.AddRange(Formulas);
					if (fa.IsMain())
						al.AddRange(OverlayFormulas);
					else al.AddRange(ListedFormulas);
					AddFavorite((string[])al.ToArray(typeof(string)));
				}
			}
			return ShowDialog();
		}

		private void cbSecondYAxis_Click(object sender, System.EventArgs e)
		{
		}

		private void cbSecondYAxis_CheckedChanged(object sender, System.EventArgs e)
		{
			DisableChangeEvent = true;
			try
			{
				if (lbFormulaList.SelectedItem!=null)
				{
					string s = lbFormulaList.SelectedItem.ToString();
					lbFormulaList.Items[lbFormulaList.SelectedIndex] = s.TrimEnd('!')+(cbSecondYAxis.Checked?"!":"");
				}
			} 
			finally
			{
				DisableChangeEvent = false;
			}
		}

		private void MoveFormula(int Delta)
		{
			DisableChangeEvent = true;
			try
			{
				int i = lbFormulaList.SelectedIndex;
				int j = i+Delta;
				if (i>=0 && j>=0 && j<lbFormulaList.Items.Count)
				{
					object o = lbFormulaList.Items[i];
					lbFormulaList.Items.RemoveAt(i);
					lbFormulaList.Items.Insert(i+Delta,o);
					lbFormulaList.SelectedIndex = j;
				}
			} 
			finally
			{
				DisableChangeEvent = false;
			}
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			MoveFormula(1);
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			MoveFormula(-1);
		}

		private void btnSource_Click(object sender, System.EventArgs e)
		{
			int i = lbFormulaList.SelectedIndex;
			if (i>=0)
			{
				FormulaBase fb = FormulaBase.GetFormulaByName((string)lbFormulaList.Items[i]);
				ChartWinControl.EditFormula(fb);
			}
		}


		/// <summary>
		/// Show the formula manager with default formulas
		/// </summary>
		/// <returns></returns>
		public DialogResult ShowForm()
		{
			AddFavorite(ListedFormulas);
			return ShowForm(null,null);
		}
	}
}