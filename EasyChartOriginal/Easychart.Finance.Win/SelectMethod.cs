using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Select method dialog is used to help users build formula script.
	/// It will list all method supported by the formula script language
	/// </summary>
	[ToolboxItem(false)]
	public class SelectMethod : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		//private static SelectMethod Current;
		private System.Windows.Forms.TreeView tvMethod;
		private System.Windows.Forms.Splitter spLeft;
		private System.Windows.Forms.Panel pnRight;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lName;
		private System.Windows.Forms.Label lDescription;
		private System.Windows.Forms.Label lParam;
		private System.Windows.Forms.ImageList ilMethod;
		private string Result;

		/// <summary>
		/// Create the select method dialog instance
		/// </summary>
		public SelectMethod()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SelectMethod));
			this.tvMethod = new System.Windows.Forms.TreeView();
			this.ilMethod = new System.Windows.Forms.ImageList(this.components);
			this.spLeft = new System.Windows.Forms.Splitter();
			this.pnRight = new System.Windows.Forms.Panel();
			this.lParam = new System.Windows.Forms.Label();
			this.lDescription = new System.Windows.Forms.Label();
			this.lName = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.pnRight.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvMethod
			// 
			this.tvMethod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tvMethod.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvMethod.FullRowSelect = true;
			this.tvMethod.HideSelection = false;
			this.tvMethod.ImageList = this.ilMethod;
			this.tvMethod.Location = new System.Drawing.Point(0, 0);
			this.tvMethod.Name = "tvMethod";
			this.tvMethod.Size = new System.Drawing.Size(360, 429);
			this.tvMethod.TabIndex = 4;
			this.tvMethod.DoubleClick += new System.EventHandler(this.tvMethod_DoubleClick);
			this.tvMethod.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvMethod_AfterSelect);
			// 
			// ilMethod
			// 
			this.ilMethod.ImageSize = new System.Drawing.Size(16, 16);
			this.ilMethod.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMethod.ImageStream")));
			this.ilMethod.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// spLeft
			// 
			this.spLeft.Location = new System.Drawing.Point(360, 0);
			this.spLeft.Name = "spLeft";
			this.spLeft.Size = new System.Drawing.Size(3, 429);
			this.spLeft.TabIndex = 5;
			this.spLeft.TabStop = false;
			// 
			// pnRight
			// 
			this.pnRight.Controls.Add(this.lParam);
			this.pnRight.Controls.Add(this.lDescription);
			this.pnRight.Controls.Add(this.lName);
			this.pnRight.Controls.Add(this.btnCancel);
			this.pnRight.Controls.Add(this.btnOK);
			this.pnRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnRight.Location = new System.Drawing.Point(363, 0);
			this.pnRight.Name = "pnRight";
			this.pnRight.Size = new System.Drawing.Size(245, 429);
			this.pnRight.TabIndex = 6;
			// 
			// lParam
			// 
			this.lParam.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(64)), ((System.Byte)(0)));
			this.lParam.Location = new System.Drawing.Point(16, 208);
			this.lParam.Name = "lParam";
			this.lParam.Size = new System.Drawing.Size(216, 120);
			this.lParam.TabIndex = 4;
			this.lParam.Text = "Param";
			// 
			// lDescription
			// 
			this.lDescription.ForeColor = System.Drawing.Color.Blue;
			this.lDescription.Location = new System.Drawing.Point(16, 56);
			this.lDescription.Name = "lDescription";
			this.lDescription.Size = new System.Drawing.Size(216, 136);
			this.lDescription.TabIndex = 3;
			this.lDescription.Text = "Description";
			// 
			// lName
			// 
			this.lName.AutoSize = true;
			this.lName.Location = new System.Drawing.Point(16, 24);
			this.lName.Name = "lName";
			this.lName.Size = new System.Drawing.Size(38, 17);
			this.lName.TabIndex = 2;
			this.lName.Text = "Name";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(152, 384);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(64, 384);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// SelectMethod
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(608, 429);
			this.Controls.Add(this.pnRight);
			this.Controls.Add(this.spLeft);
			this.Controls.Add(this.tvMethod);
			this.Font = new System.Drawing.Font("Verdana", 8.5F);
			this.MinimumSize = new System.Drawing.Size(500, 400);
			this.Name = "SelectMethod";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Method";
			this.Load += new System.EventHandler(this.SelectMethod_Load);
			this.pnRight.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void RefreshTree()
		{
			tvMethod.BeginUpdate();
			try
			{
				tvMethod.Nodes.Clear();
				tvMethod.Nodes.Add("Root");
				MemberInfo[] mis = FormulaBase.GetAllMembers();
				for(int i =0; i<mis.Length; i++) 
				{
					object[] os = mis[i].GetCustomAttributes(false);
					TreeNode tn = tvMethod.Nodes[0];
					foreach(object o in os)
					{
						if (o is CategoryAttribute) 
						{
							TreeNode tnCategory= null;
							string Category = (o as CategoryAttribute).Category;
							for(int j =0; j<tn.Nodes.Count; j++) 
							{
								if (tn.Nodes[j].Text == Category) 
								{
									tnCategory = tn.Nodes[j];
									break;
								}
							}
							if (tnCategory==null) 
								tn.Nodes.Add(tnCategory = new TreeNode(Category,0,0));

							TreeNode tnText = new TreeNode(mis[i].Name+GetParam(mis[i]),1,1);
							tnText.Tag = mis[i];
							tnCategory.Nodes.Add(tnText);
						}
					}
				}
				tvMethod.Nodes[0].Expand();
			}
			finally
			{
				tvMethod.EndUpdate();
			}
		}

		private void SelectMethod_Load(object sender, System.EventArgs e)
		{
			RefreshTree();
		}

		private string ReplaceType(Type t)
		{
			if (t==typeof(FormulaData))
				return "Data Array";
			else if (t==typeof(double) || t==typeof(int))
				return "Number";
			else if (t==typeof(string))
				return "string";
			else if (t==typeof(bool))
				return "TRUE or FALSE";
			return "";
		}

		private string GetParam(MemberInfo mi)
		{
			string P = "";
			if (mi is MethodInfo)
			{
				MethodInfo mii = mi as MethodInfo;
				ParameterInfo[] pis = mii.GetParameters();
				for(int j=0; j<pis.Length; j++)
				{
					if (P!="")
						P +=",";
					P +=pis[j].Name;
				}
				P = "("+P+")";
			}
			return P;
		}

		private string GetParamDesc(MemberInfo mi)
		{
			string P = "";
			if (mi is MethodInfo)
			{
				MethodInfo mii = mi as MethodInfo;
				ParameterInfo[] pis = mii.GetParameters();
				for(int j=0; j<pis.Length; j++)
					P += pis[j].Name+"\t:"+ReplaceType(pis[j].ParameterType)+"\r\n";
			}
			return P;
		}

		private void tvMethod_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode t = tvMethod.SelectedNode;
			if (t!=null && t.Tag!=null)
			{
				MemberInfo mi = (MemberInfo)t.Tag;
				object[] os = mi.GetCustomAttributes(typeof(DescriptionAttribute),false);
				lName.Text = mi.Name;
				lDescription.Text = (os[0] as DescriptionAttribute).Description;
				lParam.Text = GetParamDesc(mi);
			} 
			else
			{
				lName.Text = "";
				lDescription.Text = "";
				lParam.Text = "";
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Result = null;
			TreeNode t = tvMethod.SelectedNode;
			if (t!=null && t.Tag!=null)
			{
				MemberInfo mi = (MemberInfo)t.Tag;
				Result = mi.Name+GetParam(mi);
			};
		}

		private void tvMethod_DoubleClick(object sender, System.EventArgs e)
		{
			TreeNode t = tvMethod.SelectedNode;
			if (t!=null && t.Tag!=null)
				btnOK.PerformClick();
		}

		/// <summary>
		/// Show select method form
		/// </summary>
		/// <param name="Default"></param>
		/// <returns>Selected method</returns>
		public string Select(string Default)
		{
			if (this.ShowDialog()==DialogResult.OK)
				return this.Result;
			return null;
		}
	}
}
