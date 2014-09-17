using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Threading;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Easychart.Finance.Win;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	public delegate void ObjectEventHandler(object sender,BaseObject Object);

	/// <summary>
	/// Summary description for ObjectManager.
	/// </summary>
	[XmlRoot(IsNullable = false,ElementName="Root")]
	public class ObjectManager
	{
		private BaseObject selectedObject;
		private ObjectDragging DragObject;
		private ObjectPoint StartPoint = ObjectPoint.Empty; 
		private ObjectPoint EndPoint = ObjectPoint.Empty;
		private Bitmap MemBmp;
		private Bitmap DragMemBmp;
		private ObjectCollection objects = new ObjectCollection();
		private bool SavedShowCrossCursor;
		private bool SavedShowStatistic;
		private bool ControlSettingSaved;
		private int ObjectSteps;
		private PropertyGrid propertyGrid;
		private ObjectToolPanel ToolPanel;
		private ObjectTree ObjectTree;
		private Graphics MemBmpG;
		private Graphics DragMemBmpG;
		private float CurrentMouseX;
		private float CurrentMouseY;

		[XmlIgnore]
		public IObjectCanvas Canvas;
		[XmlIgnore]
		public Control Designer;
		[XmlIgnore]
		public Graphics FormGraphics;
		public event ObjectEventHandler AfterSelect;
		public event ObjectEventHandler AfterCreateStart;
		public event ObjectEventHandler AfterCreateFinished;
		[XmlIgnore]
		public ObjectInit ObjectType;
		
		private string oldLabelText;

		private static Hashtable htCategory = new Hashtable();
		private static Hashtable htAssembly = new Hashtable();
		public static ArrayList alCategory = new ArrayList();
		public static ArrayList AllTypes = new ArrayList();
		static XmlSerializer xsReadWrite;

		private bool inPlaceTextEdit = true;
		/// <summary>
		/// Input text in the chart
		/// </summary>
		[XmlAttribute,DefaultValue(true)]
		public bool InPlaceTextEdit
		{
			get
			{
				return inPlaceTextEdit;
			}
			set
			{
				inPlaceTextEdit = value;
			}
		}

		private TextBox EditTextBox;

		private bool changed;
		/// <summary>
		/// true if the objects changed after last load
		/// </summary>
		[XmlIgnore]
		public bool Changed 
		{
			get
			{
				return changed;
			}
			set
			{
				changed = value;
			}
		}

		[XmlElement(typeof(BaseObject))]
		public ObjectCollection Objects
		{
			get
			{
				return objects;
			}
			set
			{
				objects = value;
			}
		}

		private string symbol;
		[XmlAttribute]
		public string Symbol
		{
			get
			{
				return symbol;
			}
			set
			{
				symbol = value;
			}
		}

		private double startTime;
		[XmlAttribute]
		public double StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				startTime = value;
			}
		}

		private double endTime;
		[XmlAttribute]
		public double EndTime
		{
			get
			{
				return endTime;
			}
			set
			{
				endTime = value;
			}
		}

		private double latestTime;
		[XmlAttribute]
		public double LatestTime
		{
			get
			{
				return latestTime;
			}
			set
			{
				latestTime = value;
			}
		}

		private double maxPrice;
		[XmlAttribute]
		public double MaxPrice
		{
			get
			{
				return maxPrice;
			}
			set
			{
				maxPrice = value;
			}
		}

		private double minPrice;
		[XmlAttribute]
		public double MinPrice
		{
			get
			{
				return minPrice;
			}
			set
			{
				minPrice = value;
			}
		}

		private string indicators;
		[XmlAttribute]
		public string Indicators
		{
			get
			{
				return indicators;
			}
			set
			{
				indicators = value;
			}
		}

		private int width;
		[XmlAttribute]
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		private int height;
		[XmlAttribute]
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}
//
//		private string overlays;
//		[XmlAttribute]
//		public string Overlays
//		{
//			get
//			{
//				return overlays;
//			}
//			set
//			{
//				overlays = value;
//			}
//		}

		private string areaPercent;
		[XmlAttribute]
		public string AreaPercent
		{
			get
			{
				return areaPercent;
			}
			set
			{
				areaPercent = value;
			}
		}

		string skin;
		[XmlAttribute]
		public string Skin
		{
			get
			{
				return skin;
			}
			set
			{
				skin = value;
			}
		}

		string currentDataCycle;
		[XmlAttribute]
		public string CurrentDataCycle
		{
			get
			{
				return currentDataCycle;
			}
			set
			{
				currentDataCycle = value;
			}
		}

		private StockRenderType stockRenderType;
		[XmlAttribute]
		public StockRenderType StockRenderType
		{
			get
			{
				return stockRenderType;
			}
			set
			{
				stockRenderType = value;
			}
		}

		private StickRenderType stickRenderType;
		[XmlAttribute]
		public StickRenderType StickRenderType
		{
			get
			{
				return stickRenderType;
			}
			set
			{
				stickRenderType = value;
			}
		}

		public void AddObject(BaseObject bo)
		{
			objects.Add(bo);
			SetObjectManager(bo);
			RebuildTree();
		}

		public ObjectManager():this(null)
		{
		}

		public ObjectManager(IObjectCanvas Canvas):this(Canvas,null,null)
		{
		}

		[DllImport("User32.dll")] 
		public static extern bool PostMessage(IntPtr hWnd, int wMsg, int wParam, IntPtr lParam);
		private const int WM_MOUSEUP = 0x202;

		/// <summary>
		/// Post mouse up message to the chart control
		/// </summary>
		public void PostMouseUp()
		{
			PostMessage((Canvas as ChartWinControl).Handle,WM_MOUSEUP,0,IntPtr.Zero);
		}

		public void Invalidate(Region R)
		{
			if (Designer!=null)
			{
				Designer.Invalidate(R);
				if (Canvas!=null && Canvas.Chart!=null)
					Canvas.Chart.ExtraNeedRedraw = true;
			}
		}

		public void Invalidate()
		{
			Invalidate(null);
		}

		public ObjectManager(IObjectCanvas Canvas,PropertyGrid propertyGrid,ObjectToolPanel ToolPanel)
		{
			LoadSettings(ToolPanel);
			SetHandler(Canvas,propertyGrid,ToolPanel);
			if (ToolPanel!=null)
				ToolPanel.LoadObjectTool();

			EditTextBox = new TextBox();
			EditTextBox.Visible = false;
			if (Canvas!=null && Canvas.DesignerControl!=null)
			{
				EditTextBox.BorderStyle = BorderStyle.FixedSingle;
				EditTextBox.Parent = Canvas.DesignerControl.Parent;
				EditTextBox.BringToFront();
				EditTextBox.Multiline = true;
				EditTextBox.LostFocus+=new EventHandler(EditTextBox_LostFocus);
				EditTextBox.KeyDown +=new KeyEventHandler(EditTextBox_KeyDown);
				EditTextBox.TextChanged +=new EventHandler(EditTextBox_TextChanged);
			}
		}

		public ObjectManager(IObjectCanvas Canvas,PropertyGrid propertyGrid,ObjectToolPanel ToolPanel,ObjectTree ObjectTree)
			:this(Canvas,propertyGrid,ToolPanel)
		{
			this.ObjectTree = ObjectTree;
		}

		public static void BackgroundLoading(object o,bool FromAllTypes)
		{
			if (FromAllTypes)
			{
				AllTypes.Clear();
				AllTypes.Add(typeof(ChannelObject));
				AllTypes.Add(typeof(CrossObject));
				AllTypes.Add(typeof(GridObject));
				AllTypes.Add(typeof(ArcObject));
				AllTypes.Add(typeof(CircleObject));
				AllTypes.Add(typeof(FibonacciCircleObject));
				AllTypes.Add(typeof(MultiArcObject));
				AllTypes.Add(typeof(SinObject));
				AllTypes.Add(typeof(SpiralObject));
				AllTypes.Add(typeof(CycleCounterObject));
				AllTypes.Add(typeof(CycleObject));
				AllTypes.Add(typeof(FanObject));
				AllTypes.Add(typeof(ImageObject));
				AllTypes.Add(typeof(FibonacciLineObject));
				AllTypes.Add(typeof(LinearRegressionObject));
				AllTypes.Add(typeof(LineObject));
				AllTypes.Add(typeof(PriceLineObject));
				AllTypes.Add(typeof(SingleLineObject));
				AllTypes.Add(typeof(RectangleObject));
				AllTypes.Add(typeof(TriangleObject));
				AllTypes.Add(typeof(LabelObject));
				RegAllTypes();
			} 
			else 
			{
				RegAssembly(Assembly.GetExecutingAssembly());
				RegAssembly(Assembly.GetCallingAssembly());
			}
			CreateSerializer();
			if (o is ObjectToolPanel)
				((ObjectToolPanel)o).LoadObjectTool();
		}

		public static void BackgroundLoading(object o)
		{
			BackgroundLoading(o,false);
		}

		public static void LoadSettings(ObjectToolPanel ToolPanel)
		{
			if (htAssembly.Count==0 && AllTypes.Count==0)
			{
				BackgroundLoading(ToolPanel);
				//				RegAssembly(Assembly.GetExecutingAssembly());
				//				RegAssembly(Assembly.GetCallingAssembly());
				//				CreateSerializer();
				//				ToolPanel.LoadObjectTool();
			}
			//ThreadPool.QueueUserWorkItem(new WaitCallback(BackgroundLoading),ToolPanel);
		}

		public void SetCanvas(IObjectCanvas Canvas)
		{
			this.Canvas = Canvas;
			Designer = Canvas.DesignerControl;
			SetObjectManager();
			if (Designer!=null)
			{
				if (Designer.Tag==null)
				{
					Designer.Tag = 1;
					Designer.MouseDown +=new System.Windows.Forms.MouseEventHandler(DesignerControl_MouseDown);
					Designer.MouseMove +=new System.Windows.Forms.MouseEventHandler(DesignerControl_MouseMove);
					Designer.MouseUp +=new System.Windows.Forms.MouseEventHandler(DesignerControl_MouseUp);
					Designer.MouseWheel +=new MouseEventHandler(Designer_MouseWheel);

					if (Designer is ChartWinControl)
						(Designer as ChartWinControl).ExtraPaint +=new NativePaintHandler(ObjectManager_ExtraPaint);
					else Designer.Paint +=new System.Windows.Forms.PaintEventHandler(DesignerControl_Paint);

					Designer.SizeChanged +=new EventHandler(DesignerControl_SizeChanged);
					Designer.KeyDown +=new KeyEventHandler(Designer_KeyDown);
				}
			}
		}

		public void SetPropertyGrid(PropertyGrid propertyGrid)
		{
			this.propertyGrid = propertyGrid;
			propertyGrid.PropertyValueChanged +=new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
		}

		public void SetToolPanel(ObjectToolPanel ToolPanel)
		{
			this.ToolPanel = ToolPanel;
			ToolPanel.ToolsChanged +=new EventHandler(ToolPanel_ToolsChanged);
		}

		private void SetHandler(IObjectCanvas Canvas,PropertyGrid propertyGrid,ObjectToolPanel ToolPanel)
		{
			if (Canvas!=null)
				SetCanvas(Canvas);
			if (propertyGrid!=null)
				SetPropertyGrid(propertyGrid);
			if (ToolPanel!=null)
			{
				SetToolPanel(ToolPanel);
				AfterCreateStart +=new ObjectEventHandler(ObjectManager_AfterCreateStart);
				AfterSelect +=new ObjectEventHandler(ObjectManager_AfterSelect);
				AfterCreateFinished +=new ObjectEventHandler(ToolPanel.Manager_AfterCreateFinished);
				AfterCreateFinished +=new ObjectEventHandler(ObjectManager_AfterCreateFinished);
			}
		}

		public void DrawObject(Graphics g,BaseObject ob,bool Selected)
		{
			Region OldClip = null;
			if (ob.Area!=null) 
			{
				OldClip = g.Clip;
				if (ob.Area.Canvas!=null)
					g.SetClip(ob.Area.Canvas.Rect,CombineMode.Intersect);
			}
			ob.Draw(g);
			if (Selected)
				ob.DrawControlPoint(g);
			if (OldClip!=null)
				g.SetClip(OldClip,CombineMode.Replace);
		}

		public void Draw(Graphics g,BaseObject SelectedObject,BaseObject MovingObject)
		{
			try
			{
				foreach(BaseObject ob in objects)
					if (ob!=MovingObject)
						DrawObject(g,ob,ob==SelectedObject);
			} 
			catch (Exception e)
			{
				g.DrawString(e.ToString(),new Font("Verdana",13),Brushes.Black,10,10);
			}
		}

		public BaseObject GetObjectAt(int X,int Y)
		{
			foreach(BaseObject ob in objects) 
			{
				if (ob.InObject(X,Y)) 
					return ob;
			}
			return null;
		}

		public int GetPointIndex(int X,int Y,out BaseObject CurrentObject)
		{
			foreach(BaseObject ob in objects) 
			{
				int i =ob.GetControlPoint(X,Y);
				if (i>=0) 
				{
					CurrentObject = ob;
					return i;
				}
			}
			CurrentObject = null;
			return -1;
		}

		public int GetPointIndex(int X,int Y)
		{
			BaseObject ob;
			return GetPointIndex(X,Y,out ob);
		}

		public void SetObjectManager(BaseObject bo)
		{
			bo.SetObjectManager(this);
			if (Canvas.Chart!=null) 
			{
				FormulaArea fa = Canvas.Chart[bo.AreaName];
				if (fa==null) 
				{
					fa = Canvas.Chart[0];
					bo.AreaName = fa.Name;
				}
				bo.Area = fa;
			}
		}

		public void SetObjectManager()
		{
			foreach(BaseObject bo in Objects) 
				SetObjectManager(bo);
		}

		[XmlIgnore]
		public BaseObject SelectedObject
		{
			get 
			{
				return selectedObject;
			}
			set 
			{
				selectedObject = value;
				if (value!=null && AfterSelect!=null)
					AfterSelect(this,value);
			}
		}

		static void RegAssembly(Assembly A)
		{
			if (A!=null)
				if (htAssembly[A]==null)
				{
					htAssembly[A] = 1;
					Type[] ts = A.GetTypes();
					foreach(Type t in ts)
						if (t.Name.EndsWith("Object"))
						{
							object o =t.InvokeMember(null,
								BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance
								,null,null,null);
							if (o is BaseObject) 
							{
								ObjectInit[] ois = (o as BaseObject).RegObject();
								if (ois!=null && ois.Length>0)
								{
									if (AllTypes.IndexOf(t)<0) 
									{
										xsReadWrite = null;
										AllTypes.Add(t);
									}
									RegObjects(ois);
								};
							}
						}
				}
		}

		static void RegAllTypes()
		{
			xsReadWrite = null;
			foreach(Type t in AllTypes)
			{
				object o =t.InvokeMember(null,
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance
					,null,null,null);

				if (o is BaseObject) 
				{
					ObjectInit[] ois = (o as BaseObject).RegObject();
					RegObjects(ois);
				}
			}
		}

		public static void RegObject(ObjectInit oi)
		{
			object o = htCategory[oi.Category];
			ObjectCategory oc;
			if (o==null)
			{
				oc = new ObjectCategory(oi.Category,oi.CategoryOrder);
				htCategory[oi.Category] = oc;
				alCategory.Add(oc);
			} 
			else 
			{
				oc = (ObjectCategory)o;
				oc.Order = Math.Min(oi.CategoryOrder,oc.Order);
			}
			oc.ObjectList.Add(oi);
		}

		public static void RegObjects(ObjectInit[] ois)
		{
			if (ois!=null)
				foreach(ObjectInit oi in ois)
					RegObject(oi);
		}

		public static void SortCategory()
		{
			alCategory.Sort(new CompareCategory());
		}

		#region Persistent
		static public void CreateSerializer()
		{
			if (xsReadWrite==null)
				xsReadWrite = new XmlSerializer(typeof(ObjectManager),(Type[])AllTypes.ToArray(typeof(Type)));
		}

		public void WriteXml(TextWriter writer)
		{
			if (Canvas!=null && Canvas.Chart!=null)
			{
				FormulaChart fc = Canvas.Chart;
				IDataProvider idp = fc.DataProvider;
				if (idp!=null)
				{
					symbol = idp.GetStringData("Code");
					currentDataCycle = idp.DataCycle.ToString();
					startTime = fc.StartTime.ToOADate();
					endTime = fc.EndTime.ToOADate();
					areaPercent = fc.GetAreaPercent();
					indicators = string.Join(";",fc.AreaToStrings());
					stickRenderType = fc.StickRenderType;

					width = fc.Rect.Width;
					height = fc.Rect.Height;
					
					if (Canvas.DesignerControl is ChartWinControl)
						skin = ((ChartWinControl)Canvas.DesignerControl).Skin;

					if (fc.MainArea!=null)
					{
						maxPrice = fc.MainArea.AxisY.MaxY;
						minPrice = fc.MainArea.AxisY.MinY;
						stockRenderType = fc.MainArea.StockRenderType;
					}

					double[] dd = idp["DATE"];
					if (dd.Length>0)
						latestTime = dd[dd.Length-1];
				}
			}

			CreateSerializer();
			xsReadWrite.Serialize(writer, this, 
				new XmlSerializerNamespaces(
				new XmlQualifiedName[]{
									//	  new XmlQualifiedName("EasyChart","http://finance.easychart.net")
									  }
				));
			Changed = false;
		}

		public void WriteXml(string FileName)
		{
			TextWriter writer = new StreamWriter(FileName);
			try
			{
				writer.NewLine = "\r\n";
				WriteXml(writer);
			} 
			finally 
			{
				writer.Close();
			}
		}

		public void ReadXml(TextReader reader,bool AssignCanvas)
		{
			CreateSerializer();
			ObjectManager om = (ObjectManager)xsReadWrite.Deserialize(reader);
			objects.Clear();
			foreach(BaseObject bo in om.objects)
				objects.Add(bo);
			this.symbol = om.symbol;
			this.startTime = om.startTime;
			this.endTime = om.endTime;
			this.maxPrice = om.maxPrice;
			this.minPrice = om.minPrice;
			this.latestTime = om.latestTime;
			this.stockRenderType = om.stockRenderType;
			this.indicators = om.indicators;
			this.areaPercent = om.areaPercent;
			this.skin= om.skin;
			this.currentDataCycle = om.currentDataCycle;
			this.width = om.width;
			this.height = om.height;

			if (AssignCanvas)
			{
				SetCanvas(Canvas);
				RebuildTree();
				Invalidate();
			}
		}

		public void ReadXml(string FileName,bool AssignCanvas)
		{
			using (TextReader reader = new StreamReader(FileName))
				ReadXml(reader,AssignCanvas);
		}

		public void Clear()
		{
			changed = true;
			objects.Clear();
			RebuildTree();
			Invalidate();
		}

		public void SaveObject(string Symbol)
		{
			SaveObject(Symbol,Symbol);
		}

		public void SaveObject(string Symbol,string ObjectName,bool Force)
		{
			if (Symbol!=null && Symbol!="")
			{
				string ObjectFileName = FormulaHelper.GetObjectFile(ObjectName);
				if (File.Exists(ObjectFileName) || objects.Count>0 || Force)
					WriteXml(ObjectFileName);
			}
		}

		public void SaveObject(string Symbol,string ObjectName)
		{
			SaveObject(Symbol,ObjectName,false);
		}

		/// <summary>
		/// Load object and bind object properties to the chart
		/// </summary>
		/// <param name="Symbol"></param>
		/// <param name="ObjectName">Object file name</param>
		public void LoadObject(string Symbol,string ObjectName)
		{
			if (Symbol!=null && Symbol!="")
			{
				if (Canvas.DesignerControl is ChartWinControl)
				{
					ChartWinControl cwc = Canvas.DesignerControl as ChartWinControl;
					FormulaArea fa = cwc.Chart.MainArea;

					string ObjectFileName = FormulaHelper.GetObjectFile(ObjectName);
					if (File.Exists(ObjectFileName)) 
					{
						ReadXml(ObjectFileName,true);

						if (startTime>0)
							cwc.StartTime = DateTime.FromOADate(startTime);
						if (endTime>0)
							cwc.EndTime= DateTime.FromOADate(endTime);

						if (fa!=null)
						{
							if (minPrice>0) 
							{
								cwc.MinPrice= minPrice;
								cwc.MaxPrice = maxPrice;
							}
						}
					}
					else 
					{
						if (fa!=null)
						{
							cwc.MinPrice = 0;
							cwc.EndTime = DateTime.MinValue;
							fa.AxisY.AutoScale = true;
						}
						Clear();
					}
				}
			}
		}

		public void LoadObject(string Symbol)
		{
			LoadObject(Symbol,Symbol);
		}

		static public ObjectManager FromChart(FormulaChart fc)
		{
			ObjectManager om = new ObjectManager(new WebCanvas(fc));
			fc.NativePaint +=new NativePaintHandler(om.ObjectManager_DirectPaint);
			return om;
		}

		static public FormulaChart ShowObjectOnChart(FormulaChart fc,TextReader reader,DataManagerBase dmb,
			bool ObjectLayout)
		{
			ObjectManager om = FromChart(fc);
			om.ReadXml(reader,false);
			if (om.startTime!=0.0)
				fc.StartTime = DateTime.FromOADate(om.startTime);
			if (om.endTime!=0.0)
				fc.EndTime = DateTime.FromOADate(om.endTime);
			
			if (ObjectLayout && om.indicators!=null)
			{
				fc.Areas.Clear();
				if (om.indicators!=null)
					fc.StringsToArea(om.indicators.Split(';'));
				fc.SetAreaPercent(om.areaPercent);
				fc.Rect = new Rectangle(0,0,om.width,om.height);
			}
			om.SetCanvas(om.Canvas);
			if (om.skin!=null)
				fc.SetSkin(om.skin);

			fc.StickRenderType = om.stickRenderType;
			if (om.symbol!=null && om.symbol!="") 
			{
				//if (om.latestTime!=0.0)
				//	dmb.EndTime = DateTime.FromOADate(om.latestTime);
				CommonDataProvider cdp = (CommonDataProvider)dmb[om.symbol];
				cdp.DataCycle = DataCycle.Parse(om.currentDataCycle);
				fc.DataProvider = cdp;
			}
			FormulaArea fa = fc.MainArea;
			if (fa!=null)
				fa.StockRenderType = om.stockRenderType;

			if (om.minPrice!=0)
			{
				if (fa!=null)
				{
					fa.AxisY.MinY = om.minPrice;
					fa.AxisY.MaxY = om.maxPrice;
					fa.AxisY.AutoScale = false;
				}
			}
			return fc;
		}

		static public FormulaChart ShowObjectOnChart(TextReader reader,DataManagerBase dmb,bool ObjectLayout)
		{
			FormulaChart fc = FormulaChart.CreateChart(null);
			return ShowObjectOnChart(fc,reader,dmb,ObjectLayout);
		}

		static public FormulaChart ShowObjectOnChart(FormulaChart fc,string Symbol,string FileName,DataManagerBase dmb,bool ObjectLayout)
		{
			if (File.Exists(FileName))
			{
				using (TextReader reader = new StreamReader(FileName))
					return ShowObjectOnChart(fc,reader,dmb,ObjectLayout);
			}  
			else 
				fc.DataProvider = dmb[Symbol];
			return fc;
		}

		static public FormulaChart ShowObjectOnChart(string Symbol,string FileName,DataManagerBase dmb,bool ObjectLayout)
		{
			FormulaChart fc = FormulaChart.CreateChart(null);
			return ShowObjectOnChart(fc,Symbol,FileName,dmb,ObjectLayout);
		}

		#endregion

		#region Event Handler
		private void CreateMemBmp()
		{
			if (MemBmp==null && Canvas!=null) 
			{
				MemBmp = new Bitmap(
					Canvas.Chart.Rect.Width,Canvas.Chart.Rect.Height,PixelFormat.Format32bppPArgb); 
				MemBmpG = Graphics.FromImage(MemBmp);
			}
		}

		private void DrawMemBmp(Bitmap BackImage,Region Clip)
		{
			CreateMemBmp();
			if (Designer!=null && MemBmp!=null)
			{
				//Graphics g = Graphics.FromImage(MemBmp);
				if (Clip!=null)
					MemBmpG.SetClip(Clip,CombineMode.Replace);
				if (BackImage!=null)
					MemBmpG.DrawImage(BackImage,0,0);
				else MemBmpG.Clear(Designer.BackColor);
				BaseObject MovingObject = null;
				if (DragObject!=null)
					MovingObject = DragObject.Object;
				Draw(MemBmpG,SelectedObject,MovingObject);
			}
		}

		private ObjectPoint GetValueFromPos(float X,float Y,ref FormulaArea fa)
		{
			return Canvas.Chart.GetValueFromPos(X,Y,ref fa);
		}

		private void SaveChartControlSetting()
		{
			if (Designer is ChartWinControl && !ControlSettingSaved) 
			{
				SavedShowCrossCursor = (Designer as ChartWinControl).ShowCrossCursor;
				SavedShowStatistic = (Designer as ChartWinControl).ShowStatistic;
				ControlSettingSaved = true;

				ChartWinControl cwc = Designer as ChartWinControl;
				if (cwc.CrossCursorMouseMode!=MouseAction.MouseDown)
					cwc.ShowCrossCursor = false;
				cwc.ShowStatistic = false;
			}
		}

		private void ResizeText()
		{
			if (EditTextBox.Visible)
			{
				Graphics g = Canvas.DesignerControl.CreateGraphics();
				string s = EditTextBox.Text;
				if (s.EndsWith("\n") || s.Trim()=="")
					s +="A";
				SizeF sf = (EditTextBox.Tag as LabelObject).LabelFont.Measure(g,s);
				EditTextBox.Width = (int)sf.Width+28;
				EditTextBox.Height = (int)sf.Height+2;
			}
		}

		private void ShowEditBox(LabelObject lo)
		{
			EditTextBox.SendToBack();
			EditTextBox.Visible = true;
			lo.SetObjectManager(this);
			Point p = Point.Truncate(lo.ToPointF(lo.ControlPoints[0]));
			EditTextBox.Left = Designer.Left+ p.X;
			EditTextBox.Top = Designer.Top+ p.Y;
			EditTextBox.Tag = lo;
			EditTextBox.Text = lo.Text;
			EditTextBox.Font = lo.LabelFont.TextFont;
			EditTextBox.SelectAll();
			EditTextBox.Focus();
			EditTextBox.BringToFront();
			DragObjectFinished();
		}

		/// <summary>
		/// Call this in AfterCreateFinished event handler will cancel the object
		/// </summary>
		/// <param name="Object"></param>
		public void CancelCreate(BaseObject Object)
		{
			Object.InSetup = false;
		}

		private void EditLabelObject(BaseObject bo)
		{
			if (bo is LabelObject)
			{
				LabelObject lo = bo as LabelObject;
				int i = objects.IndexOf(lo);
				if (i>=0)
				{
					oldLabelText = lo.Text;
					objects.Remove(lo);
					ShowEditBox(lo);
				}
			}
		}

		private void DesignerControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Changed = true;
			FormulaArea fa = null;
			StartPoint = GetValueFromPos(e.X,e.Y,ref fa);
			BaseObject bo;
			
			if (ObjectType==null)
			{
				if (e.Button==MouseButtons.Left)
				{
					SelectedObject = null;
					int PointIndex = GetPointIndex(e.X,e.Y,out bo);
					if (bo==null)
						bo = GetObjectAt(e.X,e.Y);

					if (bo!=null)
					{
						SaveChartControlSetting();
						Canvas.Designing = true;
						SelectedObject = bo;
						DragObject = new ObjectDragging(new PointF(e.X,e.Y),PointIndex,bo);
						Invalidate(bo.GetRegion());
						bo.InMove = true;
						if (e.Clicks>1)
							EditLabelObject(bo);
					}
				}
			} 
			else 
			{
				if (e.Button==MouseButtons.Right)
				{
					if (DragObject!=null)
						objects.Remove(DragObject.Object);
					DragObjectFinished();
				}
				else 
				{
					if (ObjectSteps==0 && fa!=null)
					{
						SaveChartControlSetting();
						bo = ObjectType.Invoke();
						bo.AreaName = fa.Name;
						bo.Area = fa;
						bo.InSetup = true;
						bo.InMove = true;
						if (AfterCreateStart!=null)
							AfterCreateStart(this,bo);
						if (bo.InSetup)
						{
							for(int i=0; i<bo.ControlPointNum; i++)
								bo.ControlPoints[i] = StartPoint;
							if (bo is LabelObject && inPlaceTextEdit)
							{
								oldLabelText = "";
								ShowEditBox(bo as LabelObject);
							}
							else 
							{
								AddObject(bo);
								SelectedObject = bo;
								DragObject = new ObjectDragging(new PointF(e.X,e.Y),ObjectSteps+bo.InitNum>1?1:0,bo);
							}
						} else DragObjectFinished();
					} 
				}
			}
			DragMemBmp = null;
		}

		private void InvalidateObject(BaseObject ob)
		{
			Region R = ob.GetRegion();
			if (ob.Area!=null && ob.Area.Canvas!=null)
				R.Intersect(ob.Area.Canvas.Rect);
			Invalidate(R);
		}

		private void DesignerControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				CurrentMouseX = e.X;
				CurrentMouseY = e.Y;
				if (DragObject!=null)
				{
					FormulaArea fa = DragObject.Object.Area;
					float DeltaX = e.X-DragObject.StartPoint.X;
					float DeltaY = e.Y-DragObject.StartPoint.Y;
					InvalidateObject(DragObject.Object);
					BaseObject ob = DragObject.Object;
					if (ob.ControlPointNum==0)
						return;
					Changed = true;
					if (DragObject.ControlPointIndex<0) 
					{
						for(int i=0; i<DragObject.Object.ControlPoints.Length; i++) 
						{
							PointF pf = DragObject.Object.ToPointF(DragObject.ControlPoints[i]);
							DragObject.Object.ControlPoints[i] = GetValueFromPos(pf.X+DeltaX,pf.Y+DeltaY,ref fa);
						}
					} 
					else 
					{
						ob.ControlPoints[DragObject.ControlPointIndex] = GetValueFromPos(e.X,e.Y,ref fa);

						if (ob.InitNum>0 && ob.InitPoints!=null && ob.InSetup)
						{
							if (DragObject.ControlPoints.Length>1)
							{
								PointF pf1 = ob.ToPointF(ob.ControlPoints[1]);
								PointF pf0 = ob.ToPointF(ob.ControlPoints[0]);

								float FactorX = (pf1.X-pf0.X)/ob.InitPoints[1].X;
								float FactorY = (pf1.Y-pf0.Y)/ob.InitPoints[1].Y;
								for(int i=2; i<ob.ControlPoints.Length; i++)
								{
									ob.ControlPoints[i] = GetValueFromPos(
										pf0.X+FactorX*ob.InitPoints[i].X,
										pf0.Y+FactorY*ob.InitPoints[i].Y,ref fa);
								}
							}
						} 
					}
					InvalidateObject(DragObject.Object);
				} 
				else 
				{
					Cursor OldCursor = Designer.Cursor;
					bool b = GetPointIndex(e.X,e.Y)>=0;
					if (b)
						Designer.Cursor = Cursors.SizeAll;
					else 
					{
						BaseObject ob = GetObjectAt(e.X,e.Y);
						if (ob!=null)
							Designer.Cursor = Cursors.Hand;
						else if (ObjectType==null)
							Designer.Cursor = OldCursor;
						else Designer.Cursor = Cursors.Cross;
					}
				}
			}
			catch
			{
			}
		}

		private Bitmap ObjectPaint(Graphics ObjectG,Bitmap BackImage) 
		{
			if (DragObject!=null) 
			{
				if (DragMemBmp==null) 
				{
					DrawMemBmp(BackImage,ObjectG.Clip);
					DragMemBmp = new Bitmap(MemBmp.Width,MemBmp.Height,PixelFormat.Format32bppPArgb);
					DragMemBmpG = Graphics.FromImage(DragMemBmp);
				}

				DragMemBmpG.SetClip(ObjectG.Clip,CombineMode.Replace);
				DragMemBmpG.DrawImage(MemBmp,0,0);

				DrawObject(DragMemBmpG,DragObject.Object,true);
				return DragMemBmp;
			} 
			else
			{
				try
				{
					DrawMemBmp(BackImage,ObjectG.Clip);
				} 
				catch
				{
					DrawMemBmp(BackImage,null);
				}
				return MemBmp;
			}
		}

		private void DesignerControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Bitmap B = ObjectPaint(e.Graphics,null);
			e.Graphics.DrawImage(B,0,0);
		}

		private void RestoreChartControlSetting()
		{
			if (Designer is ChartWinControl && ControlSettingSaved) 
			{
				(Designer as ChartWinControl).ShowCrossCursor = SavedShowCrossCursor;
				(Designer as ChartWinControl).ShowStatistic = SavedShowStatistic;
				ControlSettingSaved = false;
			}
		}

		private void DragObjectFinished()
		{
			RestoreChartControlSetting();
			if (DragObject!=null) 
			{
				DragObject.Object.InSetup = false;
				DragObject.Object.InMove = false;
			}

			if (AfterCreateFinished!=null)
			{
				if (DragObject!=null)
					AfterCreateFinished(this,DragObject.Object);
				else AfterCreateFinished(this,null);
			}

			DragObject = null;
			ObjectType = null;
			Canvas.Designing = false;
			ObjectSteps = 0;
			Changed = true;
			Invalidate();
		}

		private void DesignerControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (ObjectType==null)
				DragObjectFinished();
			else if (DragObject!=null)
			{
				BaseObject ob = DragObject.Object;
				if (ObjectSteps==0)
				{
					if (DragObject.ControlPoints.Length>1)
					{
						PointF pf1 = ob.ToPointF(ob.ControlPoints[1]);
						PointF pf0 = ob.ToPointF(ob.ControlPoints[0]);
						if (Math.Sqrt((pf1.X-pf0.X)*(pf1.X-pf0.X)+(pf1.Y-pf0.Y)*(pf1.Y-pf0.Y))>20)
							ObjectSteps++;
					}
				}

				if (ObjectSteps!=0)
					DragObject.ControlPointIndex = ObjectSteps+1;

				ObjectSteps++;
				if (ObjectSteps==ob.InitNum || ob.InitNum==0)
					DragObjectFinished();
			}
		}

		private void DesignerControl_SizeChanged(object sender, EventArgs e)
		{
			MemBmp = null;
			FormGraphics = Designer.CreateGraphics();
		}

		public void ObjectManager_ExtraPaint(object sender, NativePaintArgs e)
		{
			//if (DragObject!=null || objects.Count>0)
			SetObjectManager();
			e.NewBitmap = ObjectPaint(e.Graphics,e.NativeBitmap);
		}

		public void ObjectManager_DirectPaint(object sender, NativePaintArgs e)
		{
			Draw(e.Graphics,null,null);
		}

		#endregion

		private void RebuildTree()
		{
			if (ObjectTree!=null)
				ObjectTree.RebuildTree(this);
		}

		#region Keyboard
		public void Delete()
		{
			int i = objects.IndexOf(SelectedObject);
			if (i>=0)
			{
				objects.Remove(SelectedObject);
				if (i>=objects.Count)
					i = objects.Count-1;
				if (i>=0)
					SelectedObject = objects[i];
				else SelectedObject = null;
				Invalidate();
				if (AfterSelect!=null)
					AfterSelect(this,SelectedObject);
				RebuildTree();
			}
		}

		public void Copy()
		{
			if (SelectedObject!=null) 
			{
				Type t = SelectedObject.GetType();
				XmlSerializer xs = new XmlSerializer(t);
				MemoryStream ms = new MemoryStream();
				StreamWriter sw = new StreamWriter(ms);
				sw.WriteLine(t.FullName);
				xs.Serialize(sw,SelectedObject);
				ms.Position = 0;
				string s = new StreamReader(ms).ReadToEnd();
				Clipboard.SetDataObject(s);
			}
		}

		public void Paste()
		{
			try
			{
				IDataObject ido = Clipboard.GetDataObject();
				string s = (string)ido.GetData(typeof(string));
				if (s!=null)
				{
					byte[] bs = Encoding.UTF8.GetBytes(s);
					MemoryStream ms = new MemoryStream(bs);
					StreamReader sr = new StreamReader(ms);
					string t = sr.ReadLine();
					Type T = Type.GetType(t);
					if (T!=null)
					{
						XmlSerializer xs = new XmlSerializer(T);
						object o = xs.Deserialize(sr);
						if (o is BaseObject)
						{
							BaseObject bo = o as BaseObject;
							bo.SetObjectManager(this);

							FormulaHitInfo fhi = Canvas.Chart.GetHitInfo(CurrentMouseX,CurrentMouseY);
							FormulaArea fa = fhi.Area;
							if (fhi.HitType == FormulaHitType.htArea && fa!=null)
							{
								PointF pf0 = bo.ToPointF(bo.ControlPoints[0]);
								float DeltaX = CurrentMouseX-pf0.X;
								float DeltaY = CurrentMouseY-pf0.Y;

								for(int i=0; i<bo.ControlPoints.Length; i++) 
								{
									PointF pf= bo.ToPointF(bo.ControlPoints[i]);
									bo.ControlPoints[i] = GetValueFromPos(pf.X+DeltaX,pf.Y+DeltaY,ref fa);
								}
								AddObject(bo);
								Invalidate();
							}
						}
					}
				}
			} 
			catch
			{
			}
		}

		private void Designer_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode) 
			{
				case Keys.Delete:
					Delete();
					e.Handled = true;
					break;
				case Keys.C:
					if (e.Control) 
					{
						Copy();
						e.Handled = true;
					}
					break;
				case Keys.V:
					if (e.Control) 
					{
						Paste();
						e.Handled = true;
					}
					break;
				case Keys.F2:
					EditLabelObject(selectedObject);
					break;
			}
			changed |= e.Handled || e.KeyCode==Keys.Left || e.KeyCode==Keys.Right || e.KeyCode==Keys.Up || e.KeyCode==Keys.Down;
		}
		#endregion

		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			Changed = true;
			Invalidate();
		}

		private void ObjectManager_AfterCreateStart(object sender, BaseObject Object)
		{
			PenMapper op = ToolPanel.DefaultPen;
			Object.LinePen.Color = op.Color;
			Object.LinePen.Width= op.Width;
			Object.LinePen.DashStyle = op.DashStyle;
		}

		private void ObjectManager_AfterSelect(object sender, BaseObject Object)
		{
			if (propertyGrid.SelectedObject != Object)
				propertyGrid.SelectedObject = Object;
		}

		private void ObjectManager_AfterCreateFinished(object sender, BaseObject Object)
		{
			propertyGrid.Refresh();
		}

		private void ToolPanel_ToolsChanged(object sender, EventArgs e)
		{
			ObjectType = ToolPanel.ObjectType;
			Canvas.Designing = ObjectType !=null;
		}

		private void Designer_MouseWheel(object sender, MouseEventArgs e)
		{
			Changed = true;
		}

		private void ConfirmOrCancelEdit(bool Cancel)
		{
			if (EditTextBox.Visible && EditTextBox.Tag is LabelObject)
			{
				if (!Cancel || oldLabelText!="")
				{
					LabelObject lo = EditTextBox.Tag as LabelObject;
					if (Cancel && oldLabelText!="")
						lo.Text = oldLabelText;
					else lo.Text = EditTextBox.Text;
					lo.InSetup = false;
					lo.InMove = false;
					AddObject(lo);
					Invalidate();
				}

				EditTextBox.Tag = null;
				EditTextBox.Visible = false;
			}
		}

		private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode==Keys.Escape)
				ConfirmOrCancelEdit(true);
			if (!e.Control && e.KeyCode==Keys.Enter)
				ConfirmOrCancelEdit(false);
		}

		private void EditTextBox_LostFocus(object sender, EventArgs e)
		{
			ConfirmOrCancelEdit(false);
		}

		private void EditTextBox_TextChanged(object sender, EventArgs e)
		{
			ResizeText();
		}
	}
	public class CompareCategory: IComparer  
	{
		int IComparer.Compare( Object x, Object y )  
		{
			int i = (x as ObjectCategory).Order;
			int j = (y as ObjectCategory).Order;

			if (i>j)
				return 1;
			else if (i<j)
				return -1;
			else return 0;
		}
	}

	public class CompareComponent: IComparer
	{
		int IComparer.Compare( Object x, Object y )  
		{
			int i = (x as ObjectInit).Order;
			int j = (y as ObjectInit).Order;

			if (i>j)
				return 1;
			else if (i<j)
				return -1;
			else return 0;
		}
	}
}