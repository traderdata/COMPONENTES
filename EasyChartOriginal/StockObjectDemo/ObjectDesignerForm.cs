using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Easychart.Finance.Objects;
using Easychart.Finance;
using Easychart.Finance.Win;
using Easychart.Finance.DataProvider;

namespace StockObjectDemo
{
	/// <summary>
	/// Summary description for ObjectDesignerForm.
	/// </summary>
	public class ObjectDesignerForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Splitter spProperty;
		private Easychart.Finance.Objects.ObjectManager Manager;
		private System.Windows.Forms.PropertyGrid pg;
		private Easychart.Finance.Win.ChartWinControl Designer;
		private System.Windows.Forms.MainMenu mmMain;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MenuItem miSp1;
		private System.Windows.Forms.MenuItem miExit;
		private System.Windows.Forms.OpenFileDialog ofd;
		private System.Windows.Forms.SaveFileDialog sfd;
		private System.Windows.Forms.MenuItem miHelp;
		private System.Windows.Forms.MenuItem miWebSite;

		public static ObjectDesignerForm MainForm;
		private System.Windows.Forms.Panel pnRight;
		private Easychart.Finance.Objects.ObjectTree ObjectTree;
		private System.Windows.Forms.Splitter spTree;
		private Easychart.Finance.Objects.ObjectToolPanel ToolPanel;
		private System.Windows.Forms.Panel pnTop;
		private System.Windows.Forms.Button btnSegment;
		private System.Windows.Forms.Button btnFib;
		private System.Windows.Forms.Button btnLabel;
		private System.Windows.Forms.Button btnArc;
		private System.Windows.Forms.MenuItem miFeatures;
		private System.Windows.Forms.MenuItem miSingleLine;
		private System.Windows.Forms.MenuItem miSelect;
		private System.Windows.Forms.MenuItem miRemoveAll;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ObjectDesignerForm()
		{
			PluginManager.Load(FormulaHelper.Root+"Plugins\\");
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
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			MainForm = new ObjectDesignerForm();
			Application.Run(MainForm);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pg = new System.Windows.Forms.PropertyGrid();
			this.spProperty = new System.Windows.Forms.Splitter();
			this.Designer = new Easychart.Finance.Win.ChartWinControl();
			this.mmMain = new System.Windows.Forms.MainMenu();
			this.miFile = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miSp1 = new System.Windows.Forms.MenuItem();
			this.miExit = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.miWebSite = new System.Windows.Forms.MenuItem();
			this.miFeatures = new System.Windows.Forms.MenuItem();
			this.miSingleLine = new System.Windows.Forms.MenuItem();
			this.miSelect = new System.Windows.Forms.MenuItem();
			this.ofd = new System.Windows.Forms.OpenFileDialog();
			this.sfd = new System.Windows.Forms.SaveFileDialog();
			this.pnRight = new System.Windows.Forms.Panel();
			this.spTree = new System.Windows.Forms.Splitter();
			this.ObjectTree = new Easychart.Finance.Objects.ObjectTree();
			this.ToolPanel = new Easychart.Finance.Objects.ObjectToolPanel();
			this.pnTop = new System.Windows.Forms.Panel();
			this.btnArc = new System.Windows.Forms.Button();
			this.btnLabel = new System.Windows.Forms.Button();
			this.btnFib = new System.Windows.Forms.Button();
			this.btnSegment = new System.Windows.Forms.Button();
			this.miRemoveAll = new System.Windows.Forms.MenuItem();
			this.pnRight.SuspendLayout();
			this.pnTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// pg
			// 
			this.pg.CommandsVisibleIfAvailable = true;
			this.pg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pg.LargeButtons = false;
			this.pg.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pg.Location = new System.Drawing.Point(0, 179);
			this.pg.Name = "pg";
			this.pg.Size = new System.Drawing.Size(200, 278);
			this.pg.TabIndex = 3;
			this.pg.Text = "PropertyGrid";
			this.pg.ViewBackColor = System.Drawing.SystemColors.Window;
			this.pg.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// spProperty
			// 
			this.spProperty.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.spProperty.Dock = System.Windows.Forms.DockStyle.Right;
			this.spProperty.Location = new System.Drawing.Point(565, 40);
			this.spProperty.MinExtra = 0;
			this.spProperty.MinSize = 0;
			this.spProperty.Name = "spProperty";
			this.spProperty.Size = new System.Drawing.Size(3, 417);
			this.spProperty.TabIndex = 4;
			this.spProperty.TabStop = false;
			// 
			// Designer
			// 
			this.Designer.CausesValidation = false;
			this.Designer.DefaultFormulas = "MAIN#AreaBB#MA(50)#MA(200);VOLMA;SlowSTO;MACD;RSI(14)#RSI(28)";
			this.Designer.Designing = false;
			this.Designer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Designer.EndTime = new System.DateTime(((long)(0)));
			this.Designer.FavoriteFormulas = "VOLMA;AreaRSI;DEMO.MACD;CCI;OBV;ATR;FastSTO;SlowSTO;ROC;TRIX;WR;AD;CMF;PPO;StochR" +
				"SI;ULT;BBWidth;PVO";
			this.Designer.LatestValueType = Easychart.Finance.LatestValueType.None;
			this.Designer.Location = new System.Drawing.Point(117, 40);
			this.Designer.MaxPrice = 0;
			this.Designer.MinPrice = 0;
			this.Designer.Name = "Designer";
			this.Designer.PriceLabelFormat = null;
			this.Designer.ShowStatistic = false;
			this.Designer.Size = new System.Drawing.Size(448, 417);
			this.Designer.StartTime = new System.DateTime(((long)(0)));
			this.Designer.Symbol = null;
			this.Designer.TabIndex = 5;
			this.Designer.NativePaint += new Easychart.Finance.NativePaintHandler(this.Designer_NativePaint);
			this.Designer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Designer_MouseDown);
			// 
			// mmMain
			// 
			this.mmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miFile,
																				   this.miHelp,
																				   this.miFeatures});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miOpen,
																				   this.miSave,
																				   this.miSp1,
																				   this.miExit});
			this.miFile.Text = "&File";
			// 
			// miOpen
			// 
			this.miOpen.Index = 0;
			this.miOpen.Text = "&Open";
			this.miOpen.Click += new System.EventHandler(this.miOpen_Click);
			// 
			// miSave
			// 
			this.miSave.Index = 1;
			this.miSave.Text = "&Save";
			this.miSave.Click += new System.EventHandler(this.miSave_Click);
			// 
			// miSp1
			// 
			this.miSp1.Index = 2;
			this.miSp1.Text = "-";
			// 
			// miExit
			// 
			this.miExit.Index = 3;
			this.miExit.Text = "&Exit";
			this.miExit.Click += new System.EventHandler(this.miExit_Click);
			// 
			// miHelp
			// 
			this.miHelp.Index = 1;
			this.miHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miWebSite});
			this.miHelp.Text = "&Help";
			// 
			// miWebSite
			// 
			this.miWebSite.Index = 0;
			this.miWebSite.Text = "&Web Site";
			this.miWebSite.Click += new System.EventHandler(this.miWebSite_Click);
			// 
			// miFeatures
			// 
			this.miFeatures.Index = 2;
			this.miFeatures.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.miSingleLine,
																					   this.miSelect,
																					   this.miRemoveAll});
			this.miFeatures.Text = "F&eatures";
			// 
			// miSingleLine
			// 
			this.miSingleLine.Index = 0;
			this.miSingleLine.Text = "Single Line in Vol Area";
			this.miSingleLine.Click += new System.EventHandler(this.miSingleLine_Click);
			// 
			// miSelect
			// 
			this.miSelect.Index = 1;
			this.miSelect.Text = "Select First Object";
			this.miSelect.Click += new System.EventHandler(this.miSelect_Click);
			// 
			// ofd
			// 
			this.ofd.Filter = "Object File(*.xml)|*.xml";
			// 
			// sfd
			// 
			this.sfd.DefaultExt = "xml";
			this.sfd.Filter = "Object File(*.xml)|*.xml";
			// 
			// pnRight
			// 
			this.pnRight.Controls.Add(this.pg);
			this.pnRight.Controls.Add(this.spTree);
			this.pnRight.Controls.Add(this.ObjectTree);
			this.pnRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnRight.Location = new System.Drawing.Point(568, 0);
			this.pnRight.Name = "pnRight";
			this.pnRight.Size = new System.Drawing.Size(200, 457);
			this.pnRight.TabIndex = 6;
			// 
			// spTree
			// 
			this.spTree.Dock = System.Windows.Forms.DockStyle.Top;
			this.spTree.Location = new System.Drawing.Point(0, 176);
			this.spTree.Name = "spTree";
			this.spTree.Size = new System.Drawing.Size(200, 3);
			this.spTree.TabIndex = 5;
			this.spTree.TabStop = false;
			// 
			// ObjectTree
			// 
			this.ObjectTree.Dock = System.Windows.Forms.DockStyle.Top;
			this.ObjectTree.Location = new System.Drawing.Point(0, 0);
			this.ObjectTree.Name = "ObjectTree";
			this.ObjectTree.Size = new System.Drawing.Size(200, 176);
			this.ObjectTree.TabIndex = 4;
			// 
			// ToolPanel
			// 
			this.ToolPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.ToolPanel.Location = new System.Drawing.Point(0, 0);
			this.ToolPanel.Name = "ToolPanel";
			this.ToolPanel.ResetAfterEachDraw = true;
			this.ToolPanel.Size = new System.Drawing.Size(117, 457);
			this.ToolPanel.TabIndex = 7;
			this.ToolPanel.AfterButtonCreated += new Easychart.Finance.Objects.ObjectButtonEventHandler(this.ToolPanel_AfterButtonCreated);
			// 
			// pnTop
			// 
			this.pnTop.Controls.Add(this.btnArc);
			this.pnTop.Controls.Add(this.btnLabel);
			this.pnTop.Controls.Add(this.btnFib);
			this.pnTop.Controls.Add(this.btnSegment);
			this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnTop.Location = new System.Drawing.Point(117, 0);
			this.pnTop.Name = "pnTop";
			this.pnTop.Size = new System.Drawing.Size(451, 40);
			this.pnTop.TabIndex = 8;
			// 
			// btnArc
			// 
			this.btnArc.Location = new System.Drawing.Point(248, 8);
			this.btnArc.Name = "btnArc";
			this.btnArc.TabIndex = 3;
			this.btnArc.Text = "Fill Circle";
			this.btnArc.Click += new System.EventHandler(this.btnArc_Click);
			// 
			// btnLabel
			// 
			this.btnLabel.Location = new System.Drawing.Point(168, 8);
			this.btnLabel.Name = "btnLabel";
			this.btnLabel.TabIndex = 2;
			this.btnLabel.Text = "Label";
			this.btnLabel.Click += new System.EventHandler(this.btnLabel_Click);
			// 
			// btnFib
			// 
			this.btnFib.Location = new System.Drawing.Point(88, 8);
			this.btnFib.Name = "btnFib";
			this.btnFib.TabIndex = 1;
			this.btnFib.Text = "Fibonacci";
			this.btnFib.Click += new System.EventHandler(this.btnFib_Click);
			// 
			// btnSegment
			// 
			this.btnSegment.Location = new System.Drawing.Point(8, 8);
			this.btnSegment.Name = "btnSegment";
			this.btnSegment.TabIndex = 0;
			this.btnSegment.Text = "Segment";
			this.btnSegment.Click += new System.EventHandler(this.btnSegment_Click);
			// 
			// miRemoveAll
			// 
			this.miRemoveAll.Index = 2;
			this.miRemoveAll.Text = "Remove All";
			this.miRemoveAll.Click += new System.EventHandler(this.miRemoveAll_Click);
			// 
			// ObjectDesignerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(768, 457);
			this.Controls.Add(this.Designer);
			this.Controls.Add(this.spProperty);
			this.Controls.Add(this.pnTop);
			this.Controls.Add(this.pnRight);
			this.Controls.Add(this.ToolPanel);
			this.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.KeyPreview = true;
			this.Menu = this.mmMain;
			this.Name = "ObjectDesignerForm";
			this.Text = "Object Designer Form";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.ObjectDesignerForm_Load);
			this.pnRight.ResumeLayout(false);
			this.pnTop.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Load CSV data from file
		/// </summary>
		/// <param name="FileName"></param>
		private void LoadCSVFile(string Symbol)
		{
			DataManagerBase dmb = new YahooCSVDataManager(Environment.CurrentDirectory,"CSV");
			Designer.DataManager = dmb;
			Designer.Symbol = Symbol;
		}

		private void ObjectDesignerForm_Load(object sender, System.EventArgs e)
		{
				LoadCSVFile("MSFT");
				Manager = new ObjectManager(Designer,pg,ToolPanel,ObjectTree);
				Manager.AfterCreateStart +=new ObjectEventHandler(Manager_AfterCreateStart);
				Designer.ScaleType = ScaleType.Log;
				KeyMessageFilter.AddMessageFilter(Designer);
		}

		private void miOpen_Click(object sender, System.EventArgs e)
		{
			if (ofd.ShowDialog()==DialogResult.OK) 
			{
				Manager.ReadXml(ofd.FileName,true);
				Designer.DefaultFormulas = Manager.Indicators;
				Designer.AreaPercent = Manager.AreaPercent;
			}
		}

		private void miExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void miSave_Click(object sender, System.EventArgs e)
		{
			if (sfd.ShowDialog()==DialogResult.OK)
				Manager.WriteXml(sfd.FileName);
		}

		private void miWebSite_Click(object sender, System.EventArgs e)
		{
			Process.Start("http://finance.easychart.net");
		}

		//ObjectManager omDynamic = new ObjectManager();
		//LabelObject lo = new LabelObject();
		private void Designer_NativePaint(object sender, Easychart.Finance.NativePaintArgs e)
		{
//			if (omDynamic.Objects.Count==0)
//			{
//				omDynamic.SetCanvas(Designer);
//				lo.Area = Designer.Chart.MainArea;
//				lo.InitPriceLabel();
//				lo.ControlPoints[0] = new ObjectPoint(new DateTime(2003,9,20).ToOADate(),26.7);
//				omDynamic.AddObject(lo);
//			}
//			lo.Draw(e.Graphics);
		}

		private void btnSegment_Click(object sender, System.EventArgs e)
		{
			Designer.Designing = true;
			Manager.ObjectType = new ObjectInit(typeof(LineObject));
		}

		private void btnFib_Click(object sender, System.EventArgs e)
		{
			Designer.Designing = true;
			Manager.ObjectType = new ObjectInit(typeof(FibonacciLineObject),"InitPercent");
		}

		private void btnLabel_Click(object sender, System.EventArgs e)
		{
			Designer.Designing = true;
			Manager.ObjectType = new ObjectInit("Price Date Label", typeof(LabelObject), "InitPriceDateLabel", "Text", "TextLPD");
		}

		private void btnArc_Click(object sender, System.EventArgs e)
		{
			Designer.Designing = true;
			Manager.ObjectType = new ObjectInit("Fill Ellpise",typeof(CircleObject),"Fill");
		}

		private void Manager_AfterCreateStart(object sender, BaseObject Object)
		{
			if (Object is FillPolygonObject)
			{
				(Object as FillPolygonObject).Brush = new BrushMapper(Color.Red);
				(Object as FillPolygonObject).Brush.Alpha = 10;
			}
			else if (Object is FibonacciLineObject)
			{
				FibonacciLineObject flo = Object as FibonacciLineObject;
				flo.ObjectFont.Alignment = StringAlignment.Near;
				if (flo.ObjectType.InitMethod=="InitPercent")
					flo.Split=new float[]{0f,0.33f,0.5f,0.66f,1f};
			} 
			else if (!Manager.InPlaceTextEdit && Object is LabelObject && Object.ObjectType.InitMethod=="InitLabel") //InitPriceDateLabel
			{
				string s = InputBox.ShowInputBox("Input the label","Label");
				if (s!="") 
				{
					(Object as LabelObject).Text = s;
					Manager.PostMouseUp();
				}
				else Manager.CancelCreate(Object);
			}

//			if (Object is LabelObject && Object.ObjectType.InitMethod=="InitDateLabel")
//			{
//				(Object as LabelObject).Text = s;
//				Manager.PostMouseUp();
//			}
		}

		private void Designer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button==MouseButtons.Right)
			{
				BaseObject bo = Manager.GetObjectAt(e.X,e.Y);
				if (bo!=null)
					Manager.SelectedObject = bo;
				// Popup your context menu according current object.
			}
		}

		private void ToolPanel_AfterButtonCreated(object sender, System.Windows.Forms.ToolBarButton tbb)
		{
//			if (tbb.Tag is ObjectInit)
//			{
//				ObjectInit oi = tbb.Tag as ObjectInit;
//				if (oi.BaseType==typeof(CircleObject) && oi.InitMethod=="Fill")
//					tbb.Visible = false;
//			}
		}

		private void miSingleLine_Click(object sender, System.EventArgs e)
		{
			SingleLineObject sl = new SingleLineObject();
			sl.Init();
			sl.AreaName = "FML.VOLMA";
			sl.LineType = SingleLineType.Vertical;   
			sl.ControlPoints[0] = new ObjectPoint(new DateTime(2003, 9, 19).ToOADate(), 26.7);
			sl.LinePen.Color = Color.Red;   
			Manager.AddObject(sl);
			Designer.NeedRedraw();
		}

		private void miSelect_Click(object sender, System.EventArgs e)
		{
			if (Manager.Objects.Count>0)
				Manager.SelectedObject = Manager.Objects[0];
			Designer.NeedRedraw();
		}

		private void miRemoveAll_Click(object sender, System.EventArgs e)
		{
			Manager.Clear();
		}
	}
}