using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;

namespace Easychart.Finance.Objects
{
	public delegate void ObjectButtonEventHandler(object sender,ToolBarButton tbb);
	/// <summary>
	/// Summary description for ObjectToolPanel.
	/// </summary>
	public class ObjectToolPanel : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ToolBar tbToolPanel;

		private System.Windows.Forms.Panel pnDefault;
		private System.Windows.Forms.ComboBox ddlPenWidth;
		private System.Windows.Forms.Label lPenWidth;
		private System.Windows.Forms.ColorDialog cdPen;
		private System.Windows.Forms.Label lStyle;
		private System.Windows.Forms.ComboBox ddlStyle;

		private System.Windows.Forms.Panel pnColor;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ImageList ilTools;
		private System.ComponentModel.IContainer components;

		private ToolBarButton LastButton;
		private bool resetAfterEachDraw = true;
		private ToolBarButton ArrowButton;
		
		public event EventHandler ToolsChanged;
		public event ObjectButtonEventHandler AfterButtonCreated;

		public ObjectInit ObjectType;

		[Browsable(false)]
		public PenMapper DefaultPen
		{
			get
			{
				PenMapper op = new PenMapper();
				op.Width = FormulaHelper.ToIntDef(ddlPenWidth.Text,1);
				op.Color = pnColor.BackColor;
				op.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),ddlStyle.Text);
				return op;
			}
		}

		[Description("Reset to design mode after each draw"),Category("Stock Object")]
		public  bool ResetAfterEachDraw 
		{
			get
			{
				return resetAfterEachDraw;
			}
			set
			{
				resetAfterEachDraw = value;
			}
		}

		public ObjectToolPanel()
		{
			// This call is required by the Windows.Forms Form Designer.
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tbToolPanel = new System.Windows.Forms.ToolBar();
			this.ilTools = new System.Windows.Forms.ImageList(this.components);
			this.pnDefault = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.pnColor = new System.Windows.Forms.Panel();
			this.ddlStyle = new System.Windows.Forms.ComboBox();
			this.lStyle = new System.Windows.Forms.Label();
			this.lPenWidth = new System.Windows.Forms.Label();
			this.ddlPenWidth = new System.Windows.Forms.ComboBox();
			this.cdPen = new System.Windows.Forms.ColorDialog();
			this.pnDefault.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbToolPanel
			// 
			this.tbToolPanel.AllowDrop = true;
			this.tbToolPanel.ButtonSize = new System.Drawing.Size(24, 24);
			this.tbToolPanel.Divider = false;
			this.tbToolPanel.DropDownArrows = true;
			this.tbToolPanel.ImageList = this.ilTools;
			this.tbToolPanel.Location = new System.Drawing.Point(0, 0);
			this.tbToolPanel.Name = "tbToolPanel";
			this.tbToolPanel.ShowToolTips = true;
			this.tbToolPanel.Size = new System.Drawing.Size(96, 28);
			this.tbToolPanel.TabIndex = 0;
			this.tbToolPanel.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbToolPanel_ButtonClick);
			// 
			// ilTools
			// 
			this.ilTools.ImageSize = new System.Drawing.Size(20, 20);
			this.ilTools.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// pnDefault
			// 
			this.pnDefault.Controls.Add(this.label1);
			this.pnDefault.Controls.Add(this.pnColor);
			this.pnDefault.Controls.Add(this.ddlStyle);
			this.pnDefault.Controls.Add(this.lStyle);
			this.pnDefault.Controls.Add(this.lPenWidth);
			this.pnDefault.Controls.Add(this.ddlPenWidth);
			this.pnDefault.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnDefault.Location = new System.Drawing.Point(0, 264);
			this.pnDefault.Name = "pnDefault";
			this.pnDefault.Size = new System.Drawing.Size(96, 176);
			this.pnDefault.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "Default Pen:";
			// 
			// pnColor
			// 
			this.pnColor.BackColor = System.Drawing.Color.Black;
			this.pnColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnColor.Location = new System.Drawing.Point(40, 144);
			this.pnColor.Name = "pnColor";
			this.pnColor.Size = new System.Drawing.Size(24, 24);
			this.pnColor.TabIndex = 5;
			this.pnColor.Click += new System.EventHandler(this.pnColor_Click);
			// 
			// ddlStyle
			// 
			this.ddlStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlStyle.Location = new System.Drawing.Point(8, 116);
			this.ddlStyle.Name = "ddlStyle";
			this.ddlStyle.Size = new System.Drawing.Size(56, 20);
			this.ddlStyle.TabIndex = 3;
			// 
			// lStyle
			// 
			this.lStyle.AutoSize = true;
			this.lStyle.Location = new System.Drawing.Point(7, 96);
			this.lStyle.Name = "lStyle";
			this.lStyle.Size = new System.Drawing.Size(42, 17);
			this.lStyle.TabIndex = 2;
			this.lStyle.Text = "Style:";
			// 
			// lPenWidth
			// 
			this.lPenWidth.AutoSize = true;
			this.lPenWidth.Location = new System.Drawing.Point(7, 47);
			this.lPenWidth.Name = "lPenWidth";
			this.lPenWidth.Size = new System.Drawing.Size(42, 17);
			this.lPenWidth.TabIndex = 1;
			this.lPenWidth.Text = "Width:";
			// 
			// ddlPenWidth
			// 
			this.ddlPenWidth.Items.AddRange(new object[] {
															 "1",
															 "2",
															 "3",
															 "4",
															 "5",
															 "6",
															 "7",
															 "8",
															 "9",
															 "10"});
			this.ddlPenWidth.Location = new System.Drawing.Point(8, 65);
			this.ddlPenWidth.Name = "ddlPenWidth";
			this.ddlPenWidth.Size = new System.Drawing.Size(56, 20);
			this.ddlPenWidth.TabIndex = 0;
			this.ddlPenWidth.Text = "1";
			// 
			// ObjectToolPanel
			// 
			this.Controls.Add(this.pnDefault);
			this.Controls.Add(this.tbToolPanel);
			this.Name = "ObjectToolPanel";
			this.Size = new System.Drawing.Size(96, 440);
			this.pnDefault.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		static Assembly IconAssembly = Assembly.GetExecutingAssembly();


		private void AddButton(ToolBarButton tbb)
		{
			tbToolPanel.Buttons.Add(tbb);
			if (AfterButtonCreated!=null)
				AfterButtonCreated(this,tbb);
		}

		/// <summary>
		/// Add object to the tool panel
		/// </summary>
		/// <param name="ObjectType"></param>
		private void AddObject(ObjectInit ObjectType)
		{
			ToolBarButton tbb = new ToolBarButton();
			tbb.Tag = ObjectType;
			string IconName;
			if (ObjectType!=null)
			{
				tbb.ToolTipText = ObjectType.Name;
				//IconName=FormulaHelper.GetIconFile(ObjectType.Icon);
				IconName = ObjectType.Icon;
			}
			else 
			{
				ArrowButton = tbb;
				tbb.ToolTipText = "Select";
				//IconName = FormulaHelper.GetIconFile("Arrow");
				IconName = "Arrow";
			}
			
			Stream s = IconAssembly.GetManifestResourceStream("Easychart.Finance.Objects.Icons."+IconName+".gif");
			if (s!=null)
			{
				Image I = Bitmap.FromStream(s);
				ilTools.Images.Add(I);
				tbb.ImageIndex = ilTools.Images.Count-1;
			}

//			if (File.Exists(IconName))
//			{
//				Image I = Bitmap.FromFile(IconName);
//				ilTools.Images.Add(I);
//				tbb.ImageIndex = ilTools.Images.Count-1;
//			}
			AddButton(tbb);
		}

		/// <summary>
		/// Add separator to the tool panel
		/// </summary>
		private void AddSeparator()
		{
			ToolBarButton tbb = new ToolBarButton();
			tbb.Style = ToolBarButtonStyle.Separator;
			AddButton(tbb);
		}

		/// <summary>
		/// Load tool panel button according to the registered objects
		/// </summary>
		public void LoadObjectTool()
		{
			if (tbToolPanel.Buttons.Count==0)
			{
				//SuspendLayout();
				//try
				//{
					ddlStyle.Items.AddRange(Enum.GetNames(typeof(DashStyle)));
					ddlStyle.SelectedIndex = 0;
					AddObject(null);

					ObjectManager.SortCategory();
					foreach(ObjectCategory oc in ObjectManager.alCategory)
					{
						foreach(ObjectInit oi in oc.ObjectList)
							AddObject(oi);
						AddSeparator();
					}
				//}
				//finally
				//{
				//	ResumeLayout();
				//}
			}
		}

		private void SetButton(ToolBarButton tbb)
		{
			if (tbb!=null)
			{
				ObjectType = (ObjectInit)tbb.Tag;
				tbb.Pushed = true;
				if (LastButton!=null && LastButton!=tbb)
					LastButton.Pushed = false;
				LastButton = tbb;
				if (ToolsChanged!=null)
					ToolsChanged(this,new EventArgs());
			}
		}

		private void tbToolPanel_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			SetButton(e.Button);
		}

		private void pnColor_Click(object sender, System.EventArgs e)
		{
			if (cdPen.ShowDialog()==DialogResult.OK)
			{
				pnColor.BackColor = cdPen.Color;
			}
		}

		public void Manager_AfterCreateFinished(object sender, BaseObject Object)
		{
			if (resetAfterEachDraw)
				SetButton(ArrowButton);
		}
	}
}