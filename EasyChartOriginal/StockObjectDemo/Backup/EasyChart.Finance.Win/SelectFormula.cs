using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Show a dialog to let end user select formulas from the formula plugins
	/// </summary>
	[ToolboxItem(false)]
	public class SelectFormula : System.Windows.Forms.Form	
	{
		private System.Windows.Forms.TreeView tvFormula;
		public System.Windows.Forms.ImageList ilFormula;
		private System.Windows.Forms.Splitter sp1;
		private System.Windows.Forms.Panel pnClient;
		private System.Windows.Forms.TextBox tbDesc;
		private System.Windows.Forms.Label lFullName;
		private System.Windows.Forms.GroupBox gbParam;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ListBox lbLines;
		private System.Windows.Forms.Button btnEdit;

		private bool SelectLine;
		private string[] FilterPrefixes;

		private string Result;

		const int OneDelta = 20;
		static private int Delta;

		public SelectFormula()
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SelectFormula));
			this.tvFormula = new System.Windows.Forms.TreeView();
			this.ilFormula = new System.Windows.Forms.ImageList(this.components);
			this.sp1 = new System.Windows.Forms.Splitter();
			this.pnClient = new System.Windows.Forms.Panel();
			this.btnEdit = new System.Windows.Forms.Button();
			this.lbLines = new System.Windows.Forms.ListBox();
			this.tbDesc = new System.Windows.Forms.TextBox();
			this.lFullName = new System.Windows.Forms.Label();
			this.gbParam = new System.Windows.Forms.GroupBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pnClient.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvFormula
			// 
			this.tvFormula.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tvFormula.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvFormula.FullRowSelect = true;
			this.tvFormula.HideSelection = false;
			this.tvFormula.ImageIndex = -1;
			this.tvFormula.Location = new System.Drawing.Point(0, 0);
			this.tvFormula.Name = "tvFormula";
			this.tvFormula.SelectedImageIndex = -1;
			this.tvFormula.Size = new System.Drawing.Size(392, 504);
			this.tvFormula.TabIndex = 3;
			this.tvFormula.DoubleClick += new System.EventHandler(this.tvFormula_DoubleClick);
			this.tvFormula.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFormula_AfterSelect);
			// 
			// ilFormula
			// 
			this.ilFormula.ImageSize = new System.Drawing.Size(16, 16);
			this.ilFormula.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilFormula.ImageStream")));
			this.ilFormula.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// sp1
			// 
			this.sp1.Location = new System.Drawing.Point(392, 0);
			this.sp1.Name = "sp1";
			this.sp1.Size = new System.Drawing.Size(3, 504);
			this.sp1.TabIndex = 4;
			this.sp1.TabStop = false;
			// 
			// pnClient
			// 
			this.pnClient.Controls.Add(this.btnEdit);
			this.pnClient.Controls.Add(this.lbLines);
			this.pnClient.Controls.Add(this.tbDesc);
			this.pnClient.Controls.Add(this.lFullName);
			this.pnClient.Controls.Add(this.gbParam);
			this.pnClient.Controls.Add(this.btnOK);
			this.pnClient.Controls.Add(this.btnCancel);
			this.pnClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnClient.Location = new System.Drawing.Point(392, 0);
			this.pnClient.Name = "pnClient";
			this.pnClient.Size = new System.Drawing.Size(338, 504);
			this.pnClient.TabIndex = 5;
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnEdit.Location = new System.Drawing.Point(68, 467);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.TabIndex = 8;
			this.btnEdit.Text = "&Edit";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// lbLines
			// 
			this.lbLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbLines.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbLines.Location = new System.Drawing.Point(8, 312);
			this.lbLines.Name = "lbLines";
			this.lbLines.Size = new System.Drawing.Size(325, 93);
			this.lbLines.TabIndex = 7;
			this.lbLines.Visible = false;
			this.lbLines.DoubleClick += new System.EventHandler(this.lbLines_DoubleClick);
			// 
			// tbDesc
			// 
			this.tbDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbDesc.BackColor = System.Drawing.Color.Beige;
			this.tbDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbDesc.Location = new System.Drawing.Point(8, 32);
			this.tbDesc.Multiline = true;
			this.tbDesc.Name = "tbDesc";
			this.tbDesc.ReadOnly = true;
			this.tbDesc.Size = new System.Drawing.Size(325, 96);
			this.tbDesc.TabIndex = 6;
			this.tbDesc.Text = "";
			// 
			// lFullName
			// 
			this.lFullName.ForeColor = System.Drawing.Color.Blue;
			this.lFullName.Location = new System.Drawing.Point(8, 8);
			this.lFullName.Name = "lFullName";
			this.lFullName.Size = new System.Drawing.Size(413, 23);
			this.lFullName.TabIndex = 5;
			// 
			// gbParam
			// 
			this.gbParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbParam.Location = new System.Drawing.Point(8, 136);
			this.gbParam.Name = "gbParam";
			this.gbParam.Size = new System.Drawing.Size(325, 168);
			this.gbParam.TabIndex = 2;
			this.gbParam.TabStop = false;
			this.gbParam.Text = "Parameters";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(157, 467);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(245, 467);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "&Cancel";
			// 
			// SelectFormula
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(730, 504);
			this.Controls.Add(this.sp1);
			this.Controls.Add(this.pnClient);
			this.Controls.Add(this.tvFormula);
			this.Font = new System.Drawing.Font("Verdana", 8.5F);
			this.MinimumSize = new System.Drawing.Size(600, 500);
			this.Name = "SelectFormula";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Formula";
			this.Load += new System.EventHandler(this.SelectFormula_Load);
			this.Closed += new System.EventHandler(this.SelectFormula_Closed);
			this.pnClient.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void CreateNode(FormulaBase fb)
		{
			string r = fb.GetType().ToString();
			if (r.StartsWith("FML."))
				r = r.Substring(4);
			int i = r.IndexOf('.');
			if (i<0) 
				r = "Basic."+r;

			TreeNode tn = tvFormula.Nodes[0];
			while ((i=r.IndexOf('.'))>0) 
			{
				string s = r.Substring(0,i);
				r = r.Substring(i+1);
				TreeNode tnNew = null;
				for(int j =0; j<tn.Nodes.Count; j++) 
				{
					if (tn.Nodes[j].Text == s) 
					{
						tnNew = tn.Nodes[j];
						break;
					}
				}
				if (tnNew==null) 
					tn.Nodes.Add(tnNew = new TreeNode(s,0,0));

				tn = tnNew;
			}
			TreeNode tnText = new TreeNode(fb.CombineName,1,1);
			tnText.Tag = fb;
			tn.Nodes.Add(tnText);
		}

		private void RefreshTree() 
		{
			tvFormula.BeginUpdate();
			try
			{
				tvFormula.ImageList = ilFormula;
				tvFormula.Nodes.Clear();
				tvFormula.Nodes.Add("Root");
				tvFormula.Nodes[0].Nodes.Add("Basic");
				//tvFormula.Nodes[0].Nodes.Add("Test");

				FormulaBase[] fbs = FormulaBase.GetAllFormulas();
				foreach(FormulaBase fb in fbs)
				{
					CreateNode(fb);
				}

				tvFormula.Nodes[0].Expand();
				tvFormula.Nodes[0].Nodes[0].Expand();
				gbParam.Controls.Clear();

				if (tvFormula.Nodes[0].Nodes[0].Nodes.Count>0)
					tvFormula.SelectedNode = tvFormula.Nodes[0].Nodes[0].Nodes[0];
			}
			finally
			{
				tvFormula.EndUpdate();
			}
		}

		private void SelectFormula_Load(object sender, System.EventArgs e)
		{
			Location = new Point(Left+Delta,Top+Delta);
			Delta +=OneDelta;
			btnEdit.Enabled = !FormulaSourceEditor.EditorVisible;
			lbLines.Visible = SelectLine;
			RefreshTree();
		}

		static private void CreateSelectButton(GroupBox gbParam,ImageList ilFormula, int Left,int Top,int ImageIndex,TextBox tb, EventHandler Click)
		{
			Button B = new Button();
			B.Width = 20;
			B.Height = 20;
			B.FlatStyle = FlatStyle.Popup;
			B.Left = Left;
			B.Top = Top;
			B.ImageList = ilFormula;
			B.ImageIndex = ImageIndex;
			B.Parent = gbParam;
			B.Click+=Click;
			B.Tag = tb;
		}

		static public void AddParamToGroupBox(GroupBox gbParam, ImageList ilFormula,FormulaBase fb,EventHandler ehLeave)
		{
			gbParam.Controls.Clear();
			if (fb!=null)
			{
				int x = 10;
				int y = 20;
				int h = 30;
				int w = 0;

				for(int i=0; i<fb.Params.Count; i++) 
				{
					Label l = new Label();
					l.AutoSize = true;
					l.Text = fb.Params[i].Name +"=";
					l.Left = x;
					l.Top = y+3;
					l.Parent = gbParam;
					w = Math.Max(w,l.Width);
					y +=h;
				}

				y = 20;
				x += w + 6;
				for(int i=0; i<fb.Params.Count; i++) 
				{
					FormulaParam fp = fb.Params[i];
					TextBox tb = new TextBox();
					tb.Left = x; 
					tb.Top = y;
					tb.Text = fp.Value;
					tb.Parent = gbParam;
					tb.Leave +=ehLeave;

					int k = x+tb.Width+6;
					if (fp.ParamType==FormulaParamType.Double)
					{
						Label l = new Label();
						l.AutoSize = true;
						l.Text = "("+fp.MinValue+"--"+fp.MaxValue+")";
						l.Left = k;
						l.Top = y+3;
						l.Parent = gbParam;
						k +=l.Right;
					}

					switch (fb.Params[i].ParamType) 
					{
						case FormulaParamType.Indicator:
							CreateSelectButton(gbParam,ilFormula,k+6,y,1,tb,new EventHandler(SelectFormula_Click));
							break;
						case FormulaParamType.Symbol:
							CreateSelectButton(gbParam,ilFormula,k+6,y,2,tb,new EventHandler(SelectSymbol_Click));
							break;
					}

					y +=h;
				}
			}
		}

		private void tvFormula_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			FormulaBase fb = (FormulaBase)e.Node.Tag;
			lbLines.Items.Clear();
			if (fb!=null)
			{
				lFullName.Text = fb.LongName;
				tbDesc.Text = fb.Description;
				try
				{
					FormulaPackage fps = fb.Run(CommonDataProvider.Empty);
					for(int i=0; i<fps.Count; i++)
					{
						FormulaData fd = fps[i];
						if (fd.Name==null)
							lbLines.Items.Add("["+i+"]");
						else 
							lbLines.Items.Add(fd.Name);
					}
				}
				catch
				{
				}
			}
			AddParamToGroupBox(gbParam,ilFormula,fb,null);
		}

		static public string GetParam(GroupBox gbParam)
		{
			string s = "";
			foreach(Control c in gbParam.Controls)
				if (c is TextBox)
				{
					if (s!="") s += ",";
					string r = (c as TextBox).Text;
					if (r.IndexOf(',')>=0)
						r = "\""+r+"\"";
					s +=r;
				}
			return "("+s+")";
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			TreeNode t = tvFormula.SelectedNode;
			Result = null;
			if (t!=null)
			{
				FormulaBase fb = (FormulaBase)t.Tag;
				if (fb!=null)
				{
					Result = fb.TypeName;
					if (Result!=null)
						Result +=GetParam(gbParam);
					if (SelectLine)
					{
						string s = (string)lbLines.SelectedItem;
						if (s!=null)
						{
							if (!s.StartsWith("["))
								s = "["+s+"]";
							Result +=s;
						}
					}
				}
			}
		}

		private void tvFormula_DoubleClick(object sender, System.EventArgs e)
		{
			btnOK.PerformClick();
		}

		private void btnEdit_Click(object sender, System.EventArgs e)
		{
			if (tvFormula.SelectedNode!=null)
				if (tvFormula.SelectedNode.Tag!=null)
					ChartWinControl.EditFormula((FormulaBase)tvFormula.SelectedNode.Tag);
		}

		private void lbLines_DoubleClick(object sender, System.EventArgs e)
		{
			btnOK.PerformClick();
		}

		/// <summary>
		/// Show the select formula dialog
		/// </summary>
		/// <param name="Default"></param>
		/// <param name="FilterPrefixes">Filter the formulas by formula prefix array</param>
		/// <param name="SelectLine">Show the list box to let user select the formula lines</param>
		/// <returns></returns>
		public string Select(string Default,string[] FilterPrefixes,bool SelectLine)
		{
			//SelectFormula Current = new SelectFormula();
			if (FilterPrefixes!=null)
				for(int i=0; i<FilterPrefixes.Length; i++)
					FilterPrefixes[i] = FilterPrefixes[i].ToUpper();

			this.SelectLine = SelectLine;
			this.FilterPrefixes = FilterPrefixes;
			if (this.ShowDialog()==DialogResult.OK)
				return this.Result;
			return null;
		}

		static private void SelectFormula_Click(object sender, EventArgs e)
		{
			TextBox tb = (TextBox)((Button)sender).Tag;
			tb.Text = ChartWinControl.DoSelectFormula(tb.Text,null,true);
			tb.Focus();
		}

		static private void SelectSymbol_Click(object sender, EventArgs e)
		{
			TextBox tb = (TextBox)((Button)sender).Tag;
			tb.Text = ChartWinControl.DoSelectSymbol(tb.Text);
			tb.Focus();
		}

		private void SelectFormula_Closed(object sender, System.EventArgs e)
		{
			Delta-=OneDelta;
		}
	}
}