using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

namespace Easychart.Finance.Objects
{
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

		public ObjectInit ObjectType;
		private System.Windows.Forms.Panel pnColor;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ImageList ilTools;
		private System.ComponentModel.IContainer components;
		public event EventHandler ToolsChanged;
		public ObjectPen DefaultPen
		{
			get
			{
				ObjectPen op = new ObjectPen();
				op.Width = int.Parse(ddlPenWidth.Text);
				op.Color = pnColor.BackColor;
				op.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),ddlStyle.Text);
				return op;
			}
		}

		public ObjectToolPanel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
			this.pnDefault = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.pnColor = new System.Windows.Forms.Panel();
			this.ddlStyle = new System.Windows.Forms.ComboBox();
			this.lStyle = new System.Windows.Forms.Label();
			this.lPenWidth = new System.Windows.Forms.Label();
			this.ddlPenWidth = new System.Windows.Forms.ComboBox();
			this.cdPen = new System.Windows.Forms.ColorDialog();
			this.ilTools = new System.Windows.Forms.ImageList(this.components);
			this.pnDefault.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbToolPanel
			// 
			this.tbToolPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbToolPanel.ButtonSize = new System.Drawing.Size(28, 28);
			this.tbToolPanel.Divider = false;
			this.tbToolPanel.DropDownArrows = true;
			this.tbToolPanel.Location = new System.Drawing.Point(0, 0);
			this.tbToolPanel.Name = "tbToolPanel";
			this.tbToolPanel.ShowToolTips = true;
			this.tbToolPanel.Size = new System.Drawing.Size(96, 33);
			this.tbToolPanel.TabIndex = 0;
			this.tbToolPanel.Click += new System.EventHandler(this.tbToolPanel_Click);
			this.tbToolPanel.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbToolPanel_ButtonClick);
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
			this.lPenWidth.Location = new System.Drawing.Point(7, 48);
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
			// ilTools
			// 
			this.ilTools.ImageSize = new System.Drawing.Size(16, 16);
			this.ilTools.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ObjectToolPanel
			// 
			this.Controls.Add(this.pnDefault);
			this.Controls.Add(this.tbToolPanel);
			this.Name = "ObjectToolPanel";
			this.Size = new System.Drawing.Size(96, 440);
			this.Load += new System.EventHandler(this.ObjectToolPanel_Load);
			this.pnDefault.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void AddObject(ObjectInit ObjectType)
		{
			ToolBarButton tbb = new ToolBarButton();
			tbb.Tag = ObjectType;
			if (ObjectType!=null)
				tbb.ToolTipText = ObjectType.Name;
			else tbb.ToolTipText = "Select";
			tbToolPanel.Buttons.Add(tbb);
		}

		private void AddSeparator()
		{
			ToolBarButton tbb = new ToolBarButton();
			tbb.Style = ToolBarButtonStyle.Separator;
			tbToolPanel.Buttons.Add(tbb);
		}

		private void ObjectToolPanel_Load(object sender, System.EventArgs e)
		{
			ddlStyle.Items.AddRange(Enum.GetNames(typeof(DashStyle)));
			ddlStyle.SelectedIndex = 0;

			AddObject(null);
			AddObject(new ObjectInit("Segment",typeof(ObjectLine)));
			AddObject(new ObjectInit("Line",typeof(ObjectLine),"InitUnlimit1"));
			AddObject(new ObjectInit("Line",typeof(ObjectLine),"InitUnlimit2"));
			AddObject(new ObjectInit("Arrow Line",typeof(ObjectLine),"InitArrowCap"));
			AddObject(new ObjectInit("Verticle Line",typeof(ObjectVLine)));
			AddObject(new ObjectInit("3 Segments Line",typeof(ObjectLine),"InitLine3"));
			AddObject(new ObjectInit("5 Segments Line",typeof(ObjectLine),"InitLine5"));
			AddObject(new ObjectInit("8 Segments Line",typeof(ObjectLine),"InitLine8"));
			AddObject(new ObjectInit("4 Segments Line",typeof(ObjectLine),"InitLine4"));
			
			AddSeparator();
			AddObject(new ObjectInit("Fibonacci Line",typeof(FibonacciLine),"InitFibonacci"));
			AddObject(new ObjectInit("Percentage Line",typeof(FibonacciLine),"InitPercent"));
			AddObject(new ObjectInit("Fibonacci Line A",typeof(FibonacciLine),"InitFibonacciA"));
			AddObject(new ObjectInit("Percentage Line A",typeof(FibonacciLine),"InitPercentA"));
			
			AddSeparator();
			AddObject(new ObjectInit("Equal cycle line",typeof(CycleObject),"Equal"));
			AddObject(new ObjectInit("Fabonacci cycle line",typeof(CycleObject),"FabonacciCycle"));
			AddObject(new ObjectInit("Sqr cycle line",typeof(CycleObject),"Sqr"));
			AddObject(new ObjectInit("Symmetry line",typeof(CycleObject),"Symmetry"));
						
			AddSeparator();
			AddObject(new ObjectInit("Circle",typeof(ObjectCircle)));
			AddObject(new ObjectInit("Ellipse",typeof(ObjectEllipse)));
			AddObject(new ObjectInit("Multi Arc",typeof(MultiArc)));
			AddObject(new ObjectInit("Fibonacci Circle",typeof(FibonacciCircle)));
			AddSeparator();
			AddObject(new ObjectInit(typeof(ObjectLabel)));
			AddSeparator();
			AddObject(new ObjectInit("Triangle",typeof(TriangleObject),"Triangle"));
			AddObject(new ObjectInit("ParralleloGram",typeof(TriangleObject),"ParralleloGram"));
			AddSeparator();
			AddObject(new ObjectInit("LinearRegression",typeof(LinearRegression)));
			
		}

		private void tbToolPanel_Click(object sender, System.EventArgs e)
		{
		
		}

		private void tbToolPanel_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			ObjectType = (ObjectInit)e.Button.Tag;
			if (ToolsChanged!=null)
				ToolsChanged(this,new EventArgs());
		}

		private void pnColor_Click(object sender, System.EventArgs e)
		{
			if (cdPen.ShowDialog()==DialogResult.OK)
			{
				pnColor.BackColor = cdPen.Color;
			}
		}

	}
}