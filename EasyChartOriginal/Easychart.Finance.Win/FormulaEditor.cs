using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Easychart.Finance;


namespace Easychart.Finance.Win
{
	/// <summary>
	/// FormulaEditor is used to create/modify the formula script language.
	/// </summary>
	public class FormulaEditor : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TreeView FormulaTree;
		private System.Windows.Forms.TabControl tcFormula;
		private System.Windows.Forms.TabPage tpNamespace;
		private System.Windows.Forms.TabPage tpParameter;
		private System.Windows.Forms.TabPage tpFormulaProgram;
		private System.Windows.Forms.Label lMinValue;
		private System.Windows.Forms.Label lMaxValue;
		private System.Windows.Forms.Label lDefaultValue;
		private System.Windows.Forms.TextBox tbDefaultValue;
		private System.Windows.Forms.TextBox tbMinValue;
		private System.Windows.Forms.TextBox tbMaxValue;
		private System.Windows.Forms.TextBox tbParamName;
		private System.Windows.Forms.Label lParamName;
		private System.Windows.Forms.TextBox tbParamDesc;
		private System.Windows.Forms.Label lParamDesc;
		private System.Windows.Forms.MainMenu mmFumular;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.ContextMenu cmTree;
		private System.Windows.Forms.MenuItem miAddParam;
		private System.Windows.Forms.MenuItem miAddNamespace;
		private System.Windows.Forms.MenuItem miDeleteNode;
		private System.Windows.Forms.MenuItem miAddFormulaProgram;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.OpenFileDialog odFormula;
		private System.Windows.Forms.SaveFileDialog sdFormula;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MenuItem miExit;
		private System.Windows.Forms.MenuItem miCompile;
		private System.Windows.Forms.Splitter spFormula;
		private System.Windows.Forms.Panel pnFormula;
		private System.Windows.Forms.MenuItem miSaveAs;
		private System.Windows.Forms.MenuItem miNew;
		private System.Windows.Forms.ImageList ilFormula;
	
		private System.Windows.Forms.ListView lvErrors;
		private System.Windows.Forms.ColumnHeader HeaderColumn;
		private System.Windows.Forms.ColumnHeader HeaderLine;
		private System.Windows.Forms.ColumnHeader HeaderName;
		private System.Windows.Forms.ColumnHeader HeaderNumber;
		private System.Windows.Forms.ColumnHeader HeaderMessage;
		private System.Windows.Forms.Button btnDebug;
		private System.Windows.Forms.Label lFormulaProgramDesc;
		private System.Windows.Forms.TextBox tbFormulaProgramDesc;
		private System.Windows.Forms.CheckBox cbIsMainView;
		private System.Windows.Forms.TextBox tbFormulaProgramName;
		private System.Windows.Forms.Label lFormulaProgramName;
		private System.Windows.Forms.Label lFormulaProgramCode;
		private System.Windows.Forms.Panel pnProgram;
		private System.Windows.Forms.Splitter spProgram;
		private System.Windows.Forms.Panel pnNamespace;
		private System.Windows.Forms.TextBox tbNamespaceDesc;
		private System.Windows.Forms.Label lNamespaceDescription;
		private System.Windows.Forms.Label lNamespaceName;
		private System.Windows.Forms.TextBox tbNamespaceName;
		private System.Windows.Forms.CheckBox cbNotRealNameSpace;
		private System.Windows.Forms.Label lFullName;
		private System.Windows.Forms.TextBox tbProgramFullName;
		private System.Windows.Forms.Label lParamType;
		private System.Windows.Forms.ComboBox cbParamType;
		private System.Windows.Forms.ListBox lbIntelliSence;
		private System.Windows.Forms.RichTextBox tbFormulaProgramCode;
		private System.Windows.Forms.Panel pnCode;
		private System.Windows.Forms.Button btnCompile;
		private TreeNode LastNode;
		private bool modified;
		private FormulaSpace fs;
		private Label lOverride = new Label();
		private bool DisableChange;
		private static FormulaEditor CurrentEditor;

		bool Modified 
		{
			get 
			{
				if (modified)
				{
					switch (MessageBox.Show("Text modified. Save modifications ?","Confirmation",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question)) 
					{
						case DialogResult.Yes:
						{
							return !Save();
						}
						case DialogResult.No: 
						{
							return false;
						}
						default: // Cancel
							return true;
					}
				} 
				else 
				{
					return false;
				}
			}
			set 
			{
				if (modified!=value) 
				{
					modified=value;
					if (modified) 
					{
						if (!Text.EndsWith("*"))
							Text +="*";
					} 
					else 
					{
						Text = Filename;
					}
				}
			}
		}

		private string filename="";
		/// <summary>
		/// Currently file name
		/// </summary>
		public string Filename 
		{
			get 
			{
				return filename; 
			}
			set 
			{
				filename=value;
				Text=filename;
			}
		}


		public FormulaEditor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			fs = new FormulaSpace("FML");
			fs.Description = "Namespace description";
			LoadToTree(fs);

			lOverride.Parent = pnFormula;
			lOverride.Width = 2000;
			lOverride.Height = 24;
			lOverride.Top = tcFormula.Top;
			lOverride.Left = 0;
			lOverride.BringToFront();
			lOverride.BackColor = Color.WhiteSmoke;
			lOverride.Font = new Font("verdana",11,FontStyle.Bold);
			lOverride.ForeColor = Color.DarkGray;

			AddChangeEvent(this);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormulaEditor));
			this.FormulaTree = new System.Windows.Forms.TreeView();
			this.ilFormula = new System.Windows.Forms.ImageList(this.components);
			this.tcFormula = new System.Windows.Forms.TabControl();
			this.tpNamespace = new System.Windows.Forms.TabPage();
			this.pnNamespace = new System.Windows.Forms.Panel();
			this.cbNotRealNameSpace = new System.Windows.Forms.CheckBox();
			this.lNamespaceDescription = new System.Windows.Forms.Label();
			this.tbNamespaceDesc = new System.Windows.Forms.TextBox();
			this.lNamespaceName = new System.Windows.Forms.Label();
			this.tbNamespaceName = new System.Windows.Forms.TextBox();
			this.tpFormulaProgram = new System.Windows.Forms.TabPage();
			this.spProgram = new System.Windows.Forms.Splitter();
			this.pnProgram = new System.Windows.Forms.Panel();
			this.btnCompile = new System.Windows.Forms.Button();
			this.lbIntelliSence = new System.Windows.Forms.ListBox();
			this.lFullName = new System.Windows.Forms.Label();
			this.tbProgramFullName = new System.Windows.Forms.TextBox();
			this.btnDebug = new System.Windows.Forms.Button();
			this.lFormulaProgramDesc = new System.Windows.Forms.Label();
			this.tbFormulaProgramDesc = new System.Windows.Forms.TextBox();
			this.cbIsMainView = new System.Windows.Forms.CheckBox();
			this.tbFormulaProgramName = new System.Windows.Forms.TextBox();
			this.lFormulaProgramName = new System.Windows.Forms.Label();
			this.lFormulaProgramCode = new System.Windows.Forms.Label();
			this.pnCode = new System.Windows.Forms.Panel();
			this.tbFormulaProgramCode = new System.Windows.Forms.RichTextBox();
			this.lvErrors = new System.Windows.Forms.ListView();
			this.HeaderName = new System.Windows.Forms.ColumnHeader();
			this.HeaderLine = new System.Windows.Forms.ColumnHeader();
			this.HeaderColumn = new System.Windows.Forms.ColumnHeader();
			this.HeaderNumber = new System.Windows.Forms.ColumnHeader();
			this.HeaderMessage = new System.Windows.Forms.ColumnHeader();
			this.tpParameter = new System.Windows.Forms.TabPage();
			this.cbParamType = new System.Windows.Forms.ComboBox();
			this.lParamType = new System.Windows.Forms.Label();
			this.tbParamDesc = new System.Windows.Forms.TextBox();
			this.lParamDesc = new System.Windows.Forms.Label();
			this.tbParamName = new System.Windows.Forms.TextBox();
			this.lParamName = new System.Windows.Forms.Label();
			this.tbMaxValue = new System.Windows.Forms.TextBox();
			this.tbMinValue = new System.Windows.Forms.TextBox();
			this.tbDefaultValue = new System.Windows.Forms.TextBox();
			this.lDefaultValue = new System.Windows.Forms.Label();
			this.lMaxValue = new System.Windows.Forms.Label();
			this.lMinValue = new System.Windows.Forms.Label();
			this.spFormula = new System.Windows.Forms.Splitter();
			this.mmFumular = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miNew = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miSaveAs = new System.Windows.Forms.MenuItem();
			this.miCompile = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.miExit = new System.Windows.Forms.MenuItem();
			this.cmTree = new System.Windows.Forms.ContextMenu();
			this.miAddNamespace = new System.Windows.Forms.MenuItem();
			this.miAddFormulaProgram = new System.Windows.Forms.MenuItem();
			this.miAddParam = new System.Windows.Forms.MenuItem();
			this.miDeleteNode = new System.Windows.Forms.MenuItem();
			this.odFormula = new System.Windows.Forms.OpenFileDialog();
			this.sdFormula = new System.Windows.Forms.SaveFileDialog();
			this.pnFormula = new System.Windows.Forms.Panel();
			this.tcFormula.SuspendLayout();
			this.tpNamespace.SuspendLayout();
			this.pnNamespace.SuspendLayout();
			this.tpFormulaProgram.SuspendLayout();
			this.pnProgram.SuspendLayout();
			this.pnCode.SuspendLayout();
			this.tpParameter.SuspendLayout();
			this.pnFormula.SuspendLayout();
			this.SuspendLayout();
			// 
			// FormulaTree
			// 
			this.FormulaTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.FormulaTree.Dock = System.Windows.Forms.DockStyle.Left;
			this.FormulaTree.FullRowSelect = true;
			this.FormulaTree.HideSelection = false;
			this.FormulaTree.ImageList = this.ilFormula;
			this.FormulaTree.Location = new System.Drawing.Point(0, 0);
			this.FormulaTree.Name = "FormulaTree";
			this.FormulaTree.Size = new System.Drawing.Size(200, 553);
			this.FormulaTree.TabIndex = 0;
			this.FormulaTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormulaTree_MouseDown);
			this.FormulaTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.FormulaTree_AfterSelect);
			this.FormulaTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.FormulaTree_BeforeSelect);
			// 
			// ilFormula
			// 
			this.ilFormula.ImageSize = new System.Drawing.Size(16, 16);
			this.ilFormula.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilFormula.ImageStream")));
			this.ilFormula.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tcFormula
			// 
			this.tcFormula.AllowDrop = true;
			this.tcFormula.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tcFormula.Controls.Add(this.tpNamespace);
			this.tcFormula.Controls.Add(this.tpFormulaProgram);
			this.tcFormula.Controls.Add(this.tpParameter);
			this.tcFormula.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcFormula.ItemSize = new System.Drawing.Size(1, 0);
			this.tcFormula.Location = new System.Drawing.Point(0, 0);
			this.tcFormula.Name = "tcFormula";
			this.tcFormula.SelectedIndex = 0;
			this.tcFormula.Size = new System.Drawing.Size(540, 553);
			this.tcFormula.TabIndex = 4;
			this.tcFormula.TabStop = false;
			// 
			// tpNamespace
			// 
			this.tpNamespace.Controls.Add(this.pnNamespace);
			this.tpNamespace.Location = new System.Drawing.Point(4, 25);
			this.tpNamespace.Name = "tpNamespace";
			this.tpNamespace.Size = new System.Drawing.Size(532, 524);
			this.tpNamespace.TabIndex = 0;
			this.tpNamespace.Text = "Namespace";
			// 
			// pnNamespace
			// 
			this.pnNamespace.Controls.Add(this.cbNotRealNameSpace);
			this.pnNamespace.Controls.Add(this.lNamespaceDescription);
			this.pnNamespace.Controls.Add(this.tbNamespaceDesc);
			this.pnNamespace.Controls.Add(this.lNamespaceName);
			this.pnNamespace.Controls.Add(this.tbNamespaceName);
			this.pnNamespace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnNamespace.Location = new System.Drawing.Point(0, 0);
			this.pnNamespace.Name = "pnNamespace";
			this.pnNamespace.Size = new System.Drawing.Size(532, 524);
			this.pnNamespace.TabIndex = 3;
			// 
			// cbNotRealNameSpace
			// 
			this.cbNotRealNameSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbNotRealNameSpace.Location = new System.Drawing.Point(16, 489);
			this.cbNotRealNameSpace.Name = "cbNotRealNameSpace";
			this.cbNotRealNameSpace.Size = new System.Drawing.Size(488, 24);
			this.cbNotRealNameSpace.TabIndex = 7;
			this.cbNotRealNameSpace.Text = "Group Only";
			// 
			// lNamespaceDescription
			// 
			this.lNamespaceDescription.BackColor = System.Drawing.SystemColors.Control;
			this.lNamespaceDescription.Location = new System.Drawing.Point(20, 48);
			this.lNamespaceDescription.Name = "lNamespaceDescription";
			this.lNamespaceDescription.Size = new System.Drawing.Size(80, 16);
			this.lNamespaceDescription.TabIndex = 6;
			this.lNamespaceDescription.Text = "Description";
			// 
			// tbNamespaceDesc
			// 
			this.tbNamespaceDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbNamespaceDesc.BackColor = System.Drawing.SystemColors.Info;
			this.tbNamespaceDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbNamespaceDesc.Location = new System.Drawing.Point(108, 48);
			this.tbNamespaceDesc.Multiline = true;
			this.tbNamespaceDesc.Name = "tbNamespaceDesc";
			this.tbNamespaceDesc.Size = new System.Drawing.Size(404, 425);
			this.tbNamespaceDesc.TabIndex = 5;
			this.tbNamespaceDesc.Text = "";
			// 
			// lNamespaceName
			// 
			this.lNamespaceName.AccessibleName = "";
			this.lNamespaceName.BackColor = System.Drawing.SystemColors.Control;
			this.lNamespaceName.Location = new System.Drawing.Point(21, 16);
			this.lNamespaceName.Name = "lNamespaceName";
			this.lNamespaceName.Size = new System.Drawing.Size(72, 14);
			this.lNamespaceName.TabIndex = 4;
			this.lNamespaceName.Text = "Name";
			// 
			// tbNamespaceName
			// 
			this.tbNamespaceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbNamespaceName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbNamespaceName.Location = new System.Drawing.Point(108, 16);
			this.tbNamespaceName.Name = "tbNamespaceName";
			this.tbNamespaceName.Size = new System.Drawing.Size(404, 21);
			this.tbNamespaceName.TabIndex = 3;
			this.tbNamespaceName.Text = "";
			// 
			// tpFormulaProgram
			// 
			this.tpFormulaProgram.Controls.Add(this.spProgram);
			this.tpFormulaProgram.Controls.Add(this.pnProgram);
			this.tpFormulaProgram.Controls.Add(this.lvErrors);
			this.tpFormulaProgram.Location = new System.Drawing.Point(4, 25);
			this.tpFormulaProgram.Name = "tpFormulaProgram";
			this.tpFormulaProgram.Size = new System.Drawing.Size(532, 524);
			this.tpFormulaProgram.TabIndex = 1;
			this.tpFormulaProgram.Text = "FormulaProgram";
			// 
			// spProgram
			// 
			this.spProgram.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.spProgram.Location = new System.Drawing.Point(0, 449);
			this.spProgram.MinExtra = 300;
			this.spProgram.MinSize = 0;
			this.spProgram.Name = "spProgram";
			this.spProgram.Size = new System.Drawing.Size(532, 3);
			this.spProgram.TabIndex = 14;
			this.spProgram.TabStop = false;
			// 
			// pnProgram
			// 
			this.pnProgram.Controls.Add(this.btnCompile);
			this.pnProgram.Controls.Add(this.lbIntelliSence);
			this.pnProgram.Controls.Add(this.lFullName);
			this.pnProgram.Controls.Add(this.tbProgramFullName);
			this.pnProgram.Controls.Add(this.btnDebug);
			this.pnProgram.Controls.Add(this.lFormulaProgramDesc);
			this.pnProgram.Controls.Add(this.tbFormulaProgramDesc);
			this.pnProgram.Controls.Add(this.cbIsMainView);
			this.pnProgram.Controls.Add(this.tbFormulaProgramName);
			this.pnProgram.Controls.Add(this.lFormulaProgramName);
			this.pnProgram.Controls.Add(this.lFormulaProgramCode);
			this.pnProgram.Controls.Add(this.pnCode);
			this.pnProgram.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnProgram.Location = new System.Drawing.Point(0, 0);
			this.pnProgram.Name = "pnProgram";
			this.pnProgram.Size = new System.Drawing.Size(532, 452);
			this.pnProgram.TabIndex = 13;
			// 
			// btnCompile
			// 
			this.btnCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCompile.Location = new System.Drawing.Point(320, 421);
			this.btnCompile.Name = "btnCompile";
			this.btnCompile.TabIndex = 23;
			this.btnCompile.Text = "&Compile";
			this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
			// 
			// lbIntelliSence
			// 
			this.lbIntelliSence.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbIntelliSence.Location = new System.Drawing.Point(32, 136);
			this.lbIntelliSence.Name = "lbIntelliSence";
			this.lbIntelliSence.Size = new System.Drawing.Size(224, 132);
			this.lbIntelliSence.TabIndex = 22;
			this.lbIntelliSence.TabStop = false;
			this.lbIntelliSence.Visible = false;
			this.lbIntelliSence.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbIntelliSence_KeyDown);
			// 
			// lFullName
			// 
			this.lFullName.Location = new System.Drawing.Point(13, 41);
			this.lFullName.Name = "lFullName";
			this.lFullName.Size = new System.Drawing.Size(72, 23);
			this.lFullName.TabIndex = 21;
			this.lFullName.Text = "Full Name:";
			// 
			// tbProgramFullName
			// 
			this.tbProgramFullName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbProgramFullName.BackColor = System.Drawing.Color.SeaShell;
			this.tbProgramFullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbProgramFullName.Location = new System.Drawing.Point(101, 39);
			this.tbProgramFullName.Name = "tbProgramFullName";
			this.tbProgramFullName.Size = new System.Drawing.Size(419, 21);
			this.tbProgramFullName.TabIndex = 13;
			this.tbProgramFullName.Text = "";
			// 
			// btnDebug
			// 
			this.btnDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDebug.Location = new System.Drawing.Point(232, 421);
			this.btnDebug.Name = "btnDebug";
			this.btnDebug.TabIndex = 17;
			this.btnDebug.Text = "&Debug";
			this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
			// 
			// lFormulaProgramDesc
			// 
			this.lFormulaProgramDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lFormulaProgramDesc.Location = new System.Drawing.Point(8, 297);
			this.lFormulaProgramDesc.Name = "lFormulaProgramDesc";
			this.lFormulaProgramDesc.Size = new System.Drawing.Size(80, 23);
			this.lFormulaProgramDesc.TabIndex = 18;
			this.lFormulaProgramDesc.Text = "Description:";
			// 
			// tbFormulaProgramDesc
			// 
			this.tbFormulaProgramDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbFormulaProgramDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbFormulaProgramDesc.Location = new System.Drawing.Point(100, 300);
			this.tbFormulaProgramDesc.Multiline = true;
			this.tbFormulaProgramDesc.Name = "tbFormulaProgramDesc";
			this.tbFormulaProgramDesc.Size = new System.Drawing.Size(419, 112);
			this.tbFormulaProgramDesc.TabIndex = 15;
			this.tbFormulaProgramDesc.Text = "";
			this.tbFormulaProgramDesc.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// cbIsMainView
			// 
			this.cbIsMainView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbIsMainView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cbIsMainView.Location = new System.Drawing.Point(16, 420);
			this.cbIsMainView.Name = "cbIsMainView";
			this.cbIsMainView.TabIndex = 16;
			this.cbIsMainView.Text = "Main View";
			this.cbIsMainView.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// tbFormulaProgramName
			// 
			this.tbFormulaProgramName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbFormulaProgramName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbFormulaProgramName.Location = new System.Drawing.Point(101, 8);
			this.tbFormulaProgramName.Name = "tbFormulaProgramName";
			this.tbFormulaProgramName.Size = new System.Drawing.Size(419, 21);
			this.tbFormulaProgramName.TabIndex = 12;
			this.tbFormulaProgramName.Text = "";
			this.tbFormulaProgramName.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// lFormulaProgramName
			// 
			this.lFormulaProgramName.Location = new System.Drawing.Point(13, 10);
			this.lFormulaProgramName.Name = "lFormulaProgramName";
			this.lFormulaProgramName.Size = new System.Drawing.Size(72, 23);
			this.lFormulaProgramName.TabIndex = 17;
			this.lFormulaProgramName.Text = "Name:";
			// 
			// lFormulaProgramCode
			// 
			this.lFormulaProgramCode.Location = new System.Drawing.Point(13, 72);
			this.lFormulaProgramCode.Name = "lFormulaProgramCode";
			this.lFormulaProgramCode.Size = new System.Drawing.Size(72, 23);
			this.lFormulaProgramCode.TabIndex = 16;
			this.lFormulaProgramCode.Text = "Code:";
			// 
			// pnCode
			// 
			this.pnCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnCode.Controls.Add(this.tbFormulaProgramCode);
			this.pnCode.Location = new System.Drawing.Point(101, 72);
			this.pnCode.Name = "pnCode";
			this.pnCode.Size = new System.Drawing.Size(419, 217);
			this.pnCode.TabIndex = 14;
			this.pnCode.TabStop = true;
			// 
			// tbFormulaProgramCode
			// 
			this.tbFormulaProgramCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbFormulaProgramCode.BackColor = System.Drawing.SystemColors.Info;
			this.tbFormulaProgramCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbFormulaProgramCode.Location = new System.Drawing.Point(0, 0);
			this.tbFormulaProgramCode.Name = "tbFormulaProgramCode";
			this.tbFormulaProgramCode.Size = new System.Drawing.Size(417, 215);
			this.tbFormulaProgramCode.TabIndex = 14;
			this.tbFormulaProgramCode.Text = "";
			this.tbFormulaProgramCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbFormulaProgramCode_KeyDown);
			this.tbFormulaProgramCode.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// lvErrors
			// 
			this.lvErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lvErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.HeaderName,
																					   this.HeaderLine,
																					   this.HeaderColumn,
																					   this.HeaderNumber,
																					   this.HeaderMessage});
			this.lvErrors.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lvErrors.FullRowSelect = true;
			this.lvErrors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvErrors.Location = new System.Drawing.Point(0, 452);
			this.lvErrors.Name = "lvErrors";
			this.lvErrors.Size = new System.Drawing.Size(532, 72);
			this.lvErrors.TabIndex = 18;
			this.lvErrors.View = System.Windows.Forms.View.Details;
			this.lvErrors.DoubleClick += new System.EventHandler(this.lvErrors_DoubleClick);
			// 
			// HeaderName
			// 
			this.HeaderName.Text = "Name";
			this.HeaderName.Width = 80;
			// 
			// HeaderLine
			// 
			this.HeaderLine.Text = "Line";
			// 
			// HeaderColumn
			// 
			this.HeaderColumn.Text = "Column";
			// 
			// HeaderNumber
			// 
			this.HeaderNumber.Text = "Number";
			this.HeaderNumber.Width = 80;
			// 
			// HeaderMessage
			// 
			this.HeaderMessage.Text = "Message";
			this.HeaderMessage.Width = 200;
			// 
			// tpParameter
			// 
			this.tpParameter.Controls.Add(this.cbParamType);
			this.tpParameter.Controls.Add(this.lParamType);
			this.tpParameter.Controls.Add(this.tbParamDesc);
			this.tpParameter.Controls.Add(this.lParamDesc);
			this.tpParameter.Controls.Add(this.tbParamName);
			this.tpParameter.Controls.Add(this.lParamName);
			this.tpParameter.Controls.Add(this.tbMaxValue);
			this.tpParameter.Controls.Add(this.tbMinValue);
			this.tpParameter.Controls.Add(this.tbDefaultValue);
			this.tpParameter.Controls.Add(this.lDefaultValue);
			this.tpParameter.Controls.Add(this.lMaxValue);
			this.tpParameter.Controls.Add(this.lMinValue);
			this.tpParameter.Location = new System.Drawing.Point(4, 26);
			this.tpParameter.Name = "tpParameter";
			this.tpParameter.Size = new System.Drawing.Size(532, 523);
			this.tpParameter.TabIndex = 2;
			this.tpParameter.Text = "Parameter";
			// 
			// cbParamType
			// 
			this.cbParamType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbParamType.Items.AddRange(new object[] {
															 "double",
															 "string"});
			this.cbParamType.Location = new System.Drawing.Point(96, 136);
			this.cbParamType.Name = "cbParamType";
			this.cbParamType.TabIndex = 10;
			// 
			// lParamType
			// 
			this.lParamType.Location = new System.Drawing.Point(8, 136);
			this.lParamType.Name = "lParamType";
			this.lParamType.Size = new System.Drawing.Size(88, 23);
			this.lParamType.TabIndex = 9;
			this.lParamType.Text = "Type:";
			// 
			// tbParamDesc
			// 
			this.tbParamDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbParamDesc.Location = new System.Drawing.Point(96, 168);
			this.tbParamDesc.Multiline = true;
			this.tbParamDesc.Name = "tbParamDesc";
			this.tbParamDesc.Size = new System.Drawing.Size(312, 104);
			this.tbParamDesc.TabIndex = 5;
			this.tbParamDesc.Text = "";
			this.tbParamDesc.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// lParamDesc
			// 
			this.lParamDesc.Location = new System.Drawing.Point(8, 168);
			this.lParamDesc.Name = "lParamDesc";
			this.lParamDesc.Size = new System.Drawing.Size(88, 23);
			this.lParamDesc.TabIndex = 8;
			this.lParamDesc.Text = "Description:";
			// 
			// tbParamName
			// 
			this.tbParamName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbParamName.Location = new System.Drawing.Point(96, 8);
			this.tbParamName.Name = "tbParamName";
			this.tbParamName.Size = new System.Drawing.Size(136, 21);
			this.tbParamName.TabIndex = 0;
			this.tbParamName.Text = "";
			this.tbParamName.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// lParamName
			// 
			this.lParamName.Location = new System.Drawing.Point(8, 8);
			this.lParamName.Name = "lParamName";
			this.lParamName.Size = new System.Drawing.Size(80, 23);
			this.lParamName.TabIndex = 6;
			this.lParamName.Text = "Name:";
			// 
			// tbMaxValue
			// 
			this.tbMaxValue.BackColor = System.Drawing.SystemColors.Info;
			this.tbMaxValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbMaxValue.Location = new System.Drawing.Point(96, 104);
			this.tbMaxValue.Name = "tbMaxValue";
			this.tbMaxValue.Size = new System.Drawing.Size(136, 21);
			this.tbMaxValue.TabIndex = 3;
			this.tbMaxValue.Text = "";
			this.tbMaxValue.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// tbMinValue
			// 
			this.tbMinValue.BackColor = System.Drawing.SystemColors.Info;
			this.tbMinValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbMinValue.Location = new System.Drawing.Point(96, 72);
			this.tbMinValue.Name = "tbMinValue";
			this.tbMinValue.Size = new System.Drawing.Size(136, 21);
			this.tbMinValue.TabIndex = 2;
			this.tbMinValue.Text = "";
			this.tbMinValue.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// tbDefaultValue
			// 
			this.tbDefaultValue.BackColor = System.Drawing.SystemColors.Info;
			this.tbDefaultValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbDefaultValue.Location = new System.Drawing.Point(96, 40);
			this.tbDefaultValue.Name = "tbDefaultValue";
			this.tbDefaultValue.Size = new System.Drawing.Size(136, 21);
			this.tbDefaultValue.TabIndex = 1;
			this.tbDefaultValue.Text = "";
			this.tbDefaultValue.Leave += new System.EventHandler(this.tbFormulaProgramCode_Leave);
			// 
			// lDefaultValue
			// 
			this.lDefaultValue.Location = new System.Drawing.Point(8, 40);
			this.lDefaultValue.Name = "lDefaultValue";
			this.lDefaultValue.Size = new System.Drawing.Size(80, 23);
			this.lDefaultValue.TabIndex = 2;
			this.lDefaultValue.Text = "Default:";
			// 
			// lMaxValue
			// 
			this.lMaxValue.Location = new System.Drawing.Point(8, 104);
			this.lMaxValue.Name = "lMaxValue";
			this.lMaxValue.Size = new System.Drawing.Size(80, 23);
			this.lMaxValue.TabIndex = 1;
			this.lMaxValue.Text = "Maximum:";
			// 
			// lMinValue
			// 
			this.lMinValue.Location = new System.Drawing.Point(8, 72);
			this.lMinValue.Name = "lMinValue";
			this.lMinValue.Size = new System.Drawing.Size(80, 23);
			this.lMinValue.TabIndex = 0;
			this.lMinValue.Text = "Minimum:";
			// 
			// spFormula
			// 
			this.spFormula.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(224)), ((System.Byte)(224)), ((System.Byte)(224)));
			this.spFormula.Location = new System.Drawing.Point(200, 0);
			this.spFormula.MinExtra = 400;
			this.spFormula.MinSize = 150;
			this.spFormula.Name = "spFormula";
			this.spFormula.Size = new System.Drawing.Size(4, 553);
			this.spFormula.TabIndex = 5;
			this.spFormula.TabStop = false;
			// 
			// mmFumular
			// 
			this.mmFumular.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.miNew,
																					  this.miOpen,
																					  this.miSave,
																					  this.miSaveAs,
																					  this.miCompile,
																					  this.menuItem4,
																					  this.miExit});
			this.menuItem1.Text = "&File";
			// 
			// miNew
			// 
			this.miNew.Index = 0;
			this.miNew.Text = "&New";
			this.miNew.Click += new System.EventHandler(this.miNew_Click);
			// 
			// miOpen
			// 
			this.miOpen.Index = 1;
			this.miOpen.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.miOpen.Text = "&Open";
			this.miOpen.Click += new System.EventHandler(this.miOpen_Click);
			// 
			// miSave
			// 
			this.miSave.Index = 2;
			this.miSave.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.miSave.Text = "&Save";
			this.miSave.Click += new System.EventHandler(this.miSave_Click);
			// 
			// miSaveAs
			// 
			this.miSaveAs.Index = 3;
			this.miSaveAs.Text = "Save &As";
			this.miSaveAs.Click += new System.EventHandler(this.miSaveAs_Click);
			// 
			// miCompile
			// 
			this.miCompile.Index = 4;
			this.miCompile.Shortcut = System.Windows.Forms.Shortcut.F9;
			this.miCompile.Text = "&Compile";
			this.miCompile.Click += new System.EventHandler(this.miCompile_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 5;
			this.menuItem4.Text = "-";
			// 
			// miExit
			// 
			this.miExit.Index = 6;
			this.miExit.Text = "&Exit";
			this.miExit.Click += new System.EventHandler(this.miExit_Click);
			// 
			// cmTree
			// 
			this.cmTree.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miAddNamespace,
																				   this.miAddFormulaProgram,
																				   this.miAddParam,
																				   this.miDeleteNode});
			// 
			// miAddNamespace
			// 
			this.miAddNamespace.Index = 0;
			this.miAddNamespace.Text = "Add Namespace";
			this.miAddNamespace.Click += new System.EventHandler(this.miAddNamespace_Click);
			// 
			// miAddFormulaProgram
			// 
			this.miAddFormulaProgram.Index = 1;
			this.miAddFormulaProgram.Text = "Add Formula FormulaProgram";
			this.miAddFormulaProgram.Click += new System.EventHandler(this.miAddFormulaProgram_Click);
			// 
			// miAddParam
			// 
			this.miAddParam.Index = 2;
			this.miAddParam.Text = "Add Formula Parameter";
			this.miAddParam.Click += new System.EventHandler(this.miAddParam_Click);
			// 
			// miDeleteNode
			// 
			this.miDeleteNode.Index = 3;
			this.miDeleteNode.Text = "Delete Node";
			this.miDeleteNode.Click += new System.EventHandler(this.miDeleteNode_Click);
			// 
			// odFormula
			// 
			this.odFormula.DefaultExt = "fml";
			this.odFormula.Filter = "Formula File(*.fml)|*.fml|All files (*.*)|*.*";
			this.odFormula.RestoreDirectory = true;
			// 
			// sdFormula
			// 
			this.sdFormula.DefaultExt = "fml";
			this.sdFormula.Filter = "Formula File(*.fml)|*.fml|All files (*.*)|*.*";
			this.sdFormula.RestoreDirectory = true;
			// 
			// pnFormula
			// 
			this.pnFormula.Controls.Add(this.tcFormula);
			this.pnFormula.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnFormula.Location = new System.Drawing.Point(204, 0);
			this.pnFormula.Name = "pnFormula";
			this.pnFormula.Size = new System.Drawing.Size(540, 553);
			this.pnFormula.TabIndex = 6;
			// 
			// FormulaEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(744, 553);
			this.Controls.Add(this.pnFormula);
			this.Controls.Add(this.spFormula);
			this.Controls.Add(this.FormulaTree);
			this.Font = new System.Drawing.Font("Verdana", 8.5F);
			this.KeyPreview = true;
			this.Menu = this.mmFumular;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "FormulaEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Formula Editor";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormulaEditor_KeyDown);
			this.Click += new System.EventHandler(this.btnDebug_Click);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormulaEditor_Closing);
			this.Load += new System.EventHandler(this.FormulaEditor_Load);
			this.Activated += new System.EventHandler(this.FormulaEditor_Activated);
			this.tcFormula.ResumeLayout(false);
			this.tpNamespace.ResumeLayout(false);
			this.pnNamespace.ResumeLayout(false);
			this.tpFormulaProgram.ResumeLayout(false);
			this.pnProgram.ResumeLayout(false);
			this.pnCode.ResumeLayout(false);
			this.tpParameter.ResumeLayout(false);
			this.pnFormula.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormulaEditor_Load(object sender, System.EventArgs e)
		{
			string[] ss = Environment.GetCommandLineArgs();
			if (ss.Length==2) 
				LoadEditor(ss[1],null);
			else if (ss.Length==3)
				LoadEditor(ss[1],ss[2]);
		}

		private void LoadEditor(string Filename,string Formula)
		{
			if (File.Exists(Filename)) 
			{
				LoadFromFile(Filename);
				if (Formula!=null) 
				{
					int i = Formula.LastIndexOf('.');
					if (i>=0)
						Formula = Formula.Substring(i+1);
					TreeNode tr = FindNode(FormulaTree.Nodes[0],Formula);
					if (tr!=null)  
					{
						FormulaTree.SelectedNode = tr;
						FormulaTree.SelectedNode.Expand();
					}
				}
			}
			ShowDialog();
		}

		/// <summary>
		/// Open the Formula Editor.
		/// This will open the formula editor, load formula script from "Filename", and make "Formula" default. 
		/// </summary>
		/// <param name="Filename">default formula script file</param>
		/// <param name="Formula">default formula name</param>
		public static void Open(string Filename,string Formula)
		{
			if (CurrentEditor==null)
				CurrentEditor = new FormulaEditor();
			CurrentEditor.LoadEditor(Filename,Formula);
		}

		private bool Save()
		{
			if (Filename=="")
			{
				return SaveAs();
			} 
			else 
			{
				try 
				{
					if (Compile()) 
					{
						SaveDataToNode();
						fs.Write(Filename);
						Modified=false;
						return true;
					}
				} 
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				}
				return false;
			}
		}

		public bool SaveAs()
		{
			if (sdFormula.ShowDialog()==DialogResult.OK) 
			{
				SaveDataToNode();
				Filename=sdFormula.FileName;
				odFormula.FileName=Filename;
				try 
				{
					if (Compile()) 
					{
						fs.Write(Filename);
						Modified=false;
						return true;
					}
				} 
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				}
			} 
			return false;
		}

		private void LoadToTree(TreeNode tn,FormulaParam fp) 
		{
			tn.Text = fp.Name;
			tn.Tag = fp;
		}
		
		private void LoadToTree(TreeNode tn,FormulaProgram p) 
		{
			tn.Text = p.Name;
			tn.Tag = p;
			foreach(FormulaParam fp in p.Params) 
			{
				TreeNode t = new TreeNode();
				t.ImageIndex = 2;
				t.SelectedImageIndex = 2;
				tn.Nodes.Add(t);
				LoadToTree(t,fp);
			}
		}

		private void LoadToTree(TreeNode tn,FormulaSpace fs) 
		{
			tn.Text = fs.Name;
			tn.Tag = fs;
			foreach(FormulaSpace n in fs.Namespaces)
			{
				TreeNode t = new TreeNode();
				t.ImageIndex = 0;
				t.SelectedImageIndex = 0;
				tn.Nodes.Add(t);
				LoadToTree(t,n);
			}
			foreach(FormulaProgram p in fs.Programs) 
			{
				TreeNode t = new TreeNode();
				t.ImageIndex = 1;
				t.SelectedImageIndex = 1;
				tn.Nodes.Add(t);
				LoadToTree(t,p);
			}
		}

		private void LoadToTree(FormulaSpace fs)
		{
			FormulaTree.BeginUpdate();
			try 
			{
				FormulaTree.Nodes.Clear();
				TreeNode t = new TreeNode("FML",0,0);
				FormulaTree.Nodes.Add(t);
				LoadToTree(FormulaTree.Nodes[0],fs);
			} 
			finally 
			{
				FormulaTree.EndUpdate();
			}
			FormulaTree.SelectedNode = FormulaTree.TopNode;
		}
		
		private void AddChangeEvent(Control c)
		{
			if (c is TextBox)
				((TextBox)c).TextChanged +=new System.EventHandler(this.cbIsMainView_CheckedChanged);
			else if (c is CheckBox)
				((CheckBox)c).TextChanged +=new System.EventHandler(this.cbIsMainView_CheckedChanged);
			else if (c is RichTextBox)
				((RichTextBox)c).TextChanged +=new System.EventHandler(this.cbIsMainView_CheckedChanged);

			foreach(Control cc in c.Controls) 
				AddChangeEvent(cc);
		}

		private void LocateFirstProgram()
		{
			TreeNode N = FormulaTree.SelectedNode;
			if (!(N.Tag is FormulaProgram))
			{
				N = FormulaTree.Nodes[0];
				while (!(N.Tag is FormulaProgram) && (N.Nodes.Count>0))
				{
					N = N.Nodes[0];
				}
			}
			FormulaTree.SelectedNode = N;
		}

		private string[] Trim(string[] ss) 
		{
			for(int i=0; i<ss.Length; i++)
				ss[i] = ss[i].Trim();
			return ss;
		}

		private void FormulaTree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			DisableChange = true;
			try 
			{
				if (e.Node.Tag is FormulaSpace)
				{
					FormulaSpace Current = (FormulaSpace)e.Node.Tag;
					tbNamespaceName.Text = Current.Name;
					tbNamespaceDesc.Text = Current.Description;
					cbNotRealNameSpace.Checked = Current.GroupOnly;
					tcFormula.SelectedTab = tpNamespace;
					lOverride.Text = "Namespace properties:"+Current.Name;
				}
				else if (e.Node.Tag is FormulaProgram)
				{
					FormulaProgram Current = (FormulaProgram)e.Node.Tag;
					tbFormulaProgramName.Text = Current.Name;
					tbProgramFullName.Text = Current.FullName;
					if (Current.Code!=null)
						tbFormulaProgramCode.Lines = Trim(Current.Code.Split('\n'));
					else tbFormulaProgramCode.Text="";

					if (Current.Description!=null)
						tbFormulaProgramDesc.Lines = Trim(Current.Description.Split('\n'));
					else tbFormulaProgramDesc.Text ="";
					cbIsMainView.Checked = Current.IsMainView;
					tcFormula.SelectedTab = tpFormulaProgram;
					lOverride.Text = "Formula script code:"+Current.Name;
				}
				else if (e.Node.Tag is FormulaParam) 
				{
					FormulaParam Current = (FormulaParam)e.Node.Tag;
					tbParamName.Text = Current.Name;
					tbParamDesc.Text = Current.Description;
					tbMinValue.Text = Current.MinValue;
					tbMaxValue.Text = Current.MaxValue;
					tbDefaultValue.Text = Current.DefaultValue;
					int i = cbParamType.FindString(Current.ParamType);
					if (i>=0)
						cbParamType.SelectedIndex = i;
					tcFormula.SelectedTab = tpParameter;
					lOverride.Text = "Formula script parameter:"+Current.Name;
				}
				FormulaTree.Focus();
				LastNode = e.Node;
			}
			finally 
			{
				DisableChange = false;
			}
		}

		private void SaveDataToNode(TreeNode tn) 
		{
			if (tn!=null)
			{
				if (tn.Tag is FormulaProgram)
				{
					FormulaProgram Current = tn.Tag as FormulaProgram;
					Current.Name = tbFormulaProgramName.Text;
					Current.FullName = tbProgramFullName.Text;
					Current.Code = tbFormulaProgramCode.Text;
					Current.Description = tbFormulaProgramDesc.Text;
					Current.IsMainView = cbIsMainView.Checked;
					tn.Text = Current.Name;
				} 
				else if (tn.Tag is FormulaSpace)
				{
					FormulaSpace Current = tn.Tag as FormulaSpace;
					Current.Name = tbNamespaceName.Text;
					Current.Description = tbNamespaceDesc.Text;
					Current.GroupOnly = cbNotRealNameSpace.Checked;
					tn.Text = Current.Name;
				} 
				else if (tn.Tag is FormulaParam)
				{
					FormulaParam Current = tn.Tag as FormulaParam;
					Current.Name = tbParamName.Text;
					Current.MinValue = tbMinValue.Text;
					Current.MaxValue = tbMaxValue.Text;
					Current.DefaultValue = tbDefaultValue.Text;
					if (cbParamType.Text=="")
						cbParamType.SelectedIndex = 0;
					Current.ParamType = cbParamType.SelectedItem.ToString();
					Current.Description = tbParamDesc.Text;
					tn.Text = Current.Name;
				}
			}
		}

		private void SaveDataToNode() 
		{
			SaveDataToNode(FormulaTree.SelectedNode);
		}

		private void FormulaTree_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			SaveDataToNode(LastNode);
		}

		private void FormulaTree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) 
			{
				TreeNode tn = FormulaTree.GetNodeAt(e.X,e.Y);
				if (tn==null)
					tn = FormulaTree.SelectedNode;
				else FormulaTree.SelectedNode = tn;

				if (tn.Tag is FormulaSpace) 
				{
					miAddFormulaProgram.Visible = true;
					miAddNamespace.Visible = true;
					miAddParam.Visible = false;
				} 
				else if (tn.Tag is FormulaProgram) 
				{
					miAddFormulaProgram.Visible = false;
					miAddNamespace.Visible = false;
					miAddParam.Visible = true;
				} 
				else if (tn.Tag is FormulaParam)  
				{
					miAddFormulaProgram.Visible = false;
					miAddNamespace.Visible = false;
					miAddParam.Visible = false;
				}
				cmTree.Show(FormulaTree,new Point(e.X,e.Y));
			}
		}

		private void miAddNamespace_Click(object sender, System.EventArgs e)
		{
			TreeNode T = FormulaTree.SelectedNode;
			
			FormulaSpace A = (FormulaSpace)T.Tag;
			FormulaSpace B = new FormulaSpace();
			A.Namespaces.Add(B);

			TreeNode N = new TreeNode("NewSpace");
			B.Name = N.Text;
			T.Nodes.Add(N);
			N.ImageIndex = 0;
			N.SelectedImageIndex = 0;
			N.Tag = B;
			FormulaTree.SelectedNode = N;
		}

		private void miAddFormulaProgram_Click(object sender, System.EventArgs e)
		{
			TreeNode T = FormulaTree.SelectedNode;
			
			FormulaSpace A = (FormulaSpace)T.Tag;

			FormulaProgram B = new FormulaProgram();
			A.Programs. Add(B);

			TreeNode N = new TreeNode("NewCode");
			B.Name = N.Text;
			T.Nodes.Add(N);
			N.ImageIndex = 1;
			N.SelectedImageIndex = 1;
			N.Tag = B;
			FormulaTree.SelectedNode = N;
		}

		private void miAddParam_Click(object sender, System.EventArgs e)
		{
			TreeNode T = FormulaTree.SelectedNode;
			
			FormulaProgram A = (FormulaProgram)T.Tag;
			FormulaParam B = new FormulaParam();
			A.Params.Add(B);

			TreeNode N = new TreeNode("NewParam");
			B.Name = N.Text;
			T.Nodes.Add(N);
			N.ImageIndex = 2;
			N.SelectedImageIndex = 2;
			N.Tag = B;
			FormulaTree.SelectedNode = N;
		}

		private void miDeleteNode_Click(object sender, System.EventArgs e)
		{
			TreeNode T = FormulaTree.SelectedNode;
			TreeNode P = T.Parent;
			if (P!=null) 
			{
				object O1 = P.Tag;
				object O2 = T.Tag;
				if (O1 is FormulaSpace)
				{
					if (O2 is FormulaSpace)
						((FormulaSpace)O1).Namespaces.Remove((FormulaSpace)O2);
					else if (O2 is FormulaProgram)
						((FormulaSpace)O1).Programs.Remove((FormulaProgram)O2);
				}
				else if (O1 is FormulaProgram) 
				{
					if (O2 is FormulaParam)
						((FormulaProgram)O1).Params.Remove((FormulaParam)O2);
				}
				T.Remove();
			}
		}

		private void LoadFromFile(string FileName) 
		{
			if (File.Exists(FileName))
			{
				fs = FormulaSpace.Read(FileName);
				this.Filename=FileName;
				sdFormula.FileName=Filename;
				LoadToTree(fs);
				Modified=false;
				FormulaTree.Nodes[0].Expand();
			}
		}

		private void miOpen_Click(object sender, System.EventArgs e)
		{
			if (!Modified) 
			{
				if (odFormula.ShowDialog()==DialogResult.OK) 
				{
					try 
					{
						LoadFromFile(odFormula.FileName);
					} 
					catch (Exception ex) 
					{
						MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
					}
				}
			}
		}

		private void miSave_Click(object sender, System.EventArgs e)
		{
			Save();
		}

		private void miExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void FormulaEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Modified) 
				e.Cancel=true;
		}

		private void miSaveAs_Click(object sender, System.EventArgs e)
		{
			SaveAs();
		}

		private void miNew_Click(object sender, System.EventArgs e)
		{
			if (!Modified) 
			{
				Filename = "";
				fs = new FormulaSpace("FML");
				LoadToTree(fs);
			}
		}

		private void cbIsMainView_CheckedChanged(object sender, System.EventArgs e)
		{
			if (!DisableChange)
				Modified = true;
		}

		private void AddError(System.CodeDom.Compiler.CompilerErrorCollection ces,FormulaProgram fp) 
		{
			lvErrors.Items.Clear();
			foreach(System.CodeDom.Compiler.CompilerError ce in ces) 
			{
				ListViewItem lvi=null;
				if (fp!=null)
				{
					lvi = lvErrors.Items.Add(fp.Name);
					lvi.Tag = fp;
				}
				else 
				{
					FormulaProgram fpa = fs.GetProgramByLineNum(ce.Line);
					if (fpa!=null)
					{
						lvi = lvErrors.Items.Add(fpa.Name);
						fpa.AdjustErrors(ce);
						lvi.Tag = fpa;
					} 
					else 
					{
						lvi = lvErrors.Items.Add("");
					}
				}
				if (lvi!=null) 
				{
					lvi.SubItems.Add(ce.Line.ToString());
					lvi.SubItems.Add(ce.Column.ToString());
					lvi.SubItems.Add(ce.ErrorNumber.ToString());
					lvi.SubItems.Add(ce.ErrorText);
				}
			}
		}

		private void btnDebug_Click(object sender, System.EventArgs e)
		{
			FormulaProgram fp = (FormulaProgram)FormulaTree.SelectedNode.Tag;
			try 
			{
				fp.Compile();
				lvErrors.Items.Clear();
				lvErrors.Items.Add("OK!");
			}
			catch (FormulaErrorException fe)
			{
				AddError(fe.ces,fp);
			}
		}

		private bool Compile() 
		{
			try 
			{
				long Start = DateTime.Now.Ticks;
				SaveDataToNode();
				fs.SaveCShartSource(Filename.Replace('.','_')+".cs");
				fs.Compile(Filename.Replace('.','_')+".dll","");
				lvErrors.Items.Clear();
				lvErrors.Items.Add("OK! - "+(DateTime.Now.Ticks-Start)/10000+"ms");
				return true;
			}
			catch (FormulaErrorException fe)
			{
				AddError(fe.ces,null);
				return false;
			}
			finally 
			{
				LocateFirstProgram();
			}
		}

		private void miCompile_Click(object sender, System.EventArgs e)
		{
			Compile();
		}

		private TreeNode FindNode(TreeNode tn,string ProgramName) 
		{
			if (tn.Tag is FormulaProgram && (tn.Tag as FormulaProgram).Name == ProgramName)
				return tn;
			else 
			{
				foreach(TreeNode tna in tn.Nodes)
				{
					TreeNode tnb = FindNode(tna,ProgramName);
					if (tnb!=null)
						return tnb;
				}
				return null;
			}
		}

		private TreeNode FindNode(TreeNode tn,FormulaProgram fp) 
		{
			if (tn.Tag == fp)
				return tn;
			else 
			{
				foreach(TreeNode tna in tn.Nodes)
				{
					TreeNode tnb = FindNode(tna,fp);
					if (tnb!=null)
						return tnb;
				}
				return null;
			}
		}

		private void lvErrors_DoubleClick(object sender, System.EventArgs e)
		{
			ListViewItem lvi = lvErrors.FocusedItem;
			if (lvi!=null) 
			{
				FormulaProgram fp = (FormulaProgram)lvi.Tag;
				if (fp!=null) 
				{
					TreeNode tr = FindNode(FormulaTree.Nodes[0],fp);
					if (tr!=null) 
					{
						FormulaTree.SelectedNode = tr;
						int Line = int.Parse(lvi.SubItems[1].Text);
						int Column = int.Parse(lvi.SubItems[2].Text);
						for(int i=0; i<Line-1 && i<tbFormulaProgramCode.Lines.Length; i++) 
							Column +=tbFormulaProgramCode.Lines[i].Length+1;
						if (Column<0) Column = 0;
						tbFormulaProgramCode.SelectionStart =Column;
						tbFormulaProgramCode.Focus();
					}
				}
			}
		}

		private void tbFormulaProgramCode_Leave(object sender, System.EventArgs e)
		{
			SaveDataToNode();
		}

		private void tbFormulaProgramCode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode==Keys.J && e.Control) 
			{
				lbIntelliSence.Items.Clear();
				lbIntelliSence.Items.AddRange(FormulaBase.GetAllFormulas());

				int line, index;
				index = tbFormulaProgramCode.SelectionStart;
				line = tbFormulaProgramCode.GetLineFromCharIndex(index);
				
				Point pt1 = pnCode.Location;
				Point pt2 = tbFormulaProgramCode.GetPositionFromCharIndex(index);
				pt2.Offset(pt1.X+8,pt1.Y+14);
				lbIntelliSence.Location = pt2;
				lbIntelliSence.Visible= true;
				lbIntelliSence.Focus();
			}
		}

		private void lbIntelliSence_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode==Keys.Escape)
				lbIntelliSence.Visible = false;
			else if (e.KeyCode==Keys.Enter) 
			{
				string s = lbIntelliSence.SelectedItem.ToString();
				Clipboard.SetDataObject(s);
				tbFormulaProgramCode.Paste();
				lbIntelliSence.Visible = false;
			}
		}

		private void btnCompile_Click(object sender, System.EventArgs e)
		{
			Compile();
		}

		private void FormulaEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}

		private void FormulaEditor_Activated(object sender, System.EventArgs e)
		{
			tbFormulaProgramCode.Focus();
		}
	}
}