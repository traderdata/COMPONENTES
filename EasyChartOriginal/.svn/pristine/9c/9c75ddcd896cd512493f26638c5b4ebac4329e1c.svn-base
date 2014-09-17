using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Drawing.Printing;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using Easychart.Finance.DataClient;

namespace Easychart.Finance.Win
{
	public enum MouseZoomMode {None,Rect};
	public enum ShowLineMode {Default,Show,HidePrice,HideAll};
	public enum MouseWheelMode {None,Scroll,Zoom};
	public enum ZoomCenterPosition {Center,Right,Left};
	public enum DoubleClickStyle {None,Auto};

	/// <summary>
	/// Occurs when cursor pos changed
	/// </summary>
	public delegate void CursorPosChanged(object sender, FormulaChart Chart, int Pos, IDataProvider idp);
	/// <summary>
	/// Occurs before apply skin
	/// </summary>
	public delegate void ApplySkinHandler(object sender, FormulaSkin fs);
	/// <summary>
	/// Occurs after mouse move
	/// </summary>
	public delegate void AfterMouseMove(object sender,System.Windows.Forms.MouseEventArgs e,FormulaHitInfo HitInfo);

	/// <summary>
	/// Occurs after bind the data provider
	/// </summary>
	public delegate void AfterBindData(object sender, BindDataEventArgs e);

	/// <summary>
	/// Lauch when the charting system want the formula selector
	/// </summary>
	public delegate string OnSelectFormula(string Default,string[] FilterPrefixes,bool SelectLine);

	/// <summary>
	/// Lauch when the charting system want select a string
	/// </summary>
	public delegate string OnSelectString(string Default);

	/// <summary>
	/// Easy Stock Chart Windows Control
	/// </summary>
	/// <example>
	/// ChartWinControl cwc = new ChartWinControl();
	/// cwc.DataManager = new RandomDataManager();
	/// cwc.Symbol = "MSFT";
	/// </example>
	public class ChartWinControl : System.Windows.Forms.UserControl , IObjectCanvas
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private int LastX;
		private int LastY;
		private Graphics ControlGraphics;
		private int Page;
		private int LastCursorPos;
		private IDataProvider LastProvider;
		private bool showTopLine = true;
		private bool needRedraw;
		private bool needRebind;
		private bool needRefresh;
		private bool needResetCursorPos;
		private double minColumnWidth =0.01;
		private double maxColumnWidth = 500;
		private bool designing;
		private bool needDrawCursor;
		private ChartDragMode chartDragMode = ChartDragMode.Axis;
		private bool showIndicatorValues = true;
		private bool showOverlayValues = true;
		private bool adjustData = true;

		private ArrayList favoriteFormulas;
		private ChartDragInfo DragInfo;
		private FormulaHitInfo MouseDownInfo;
		private StockRenderType stockRenderType = StockRenderType.Default;
		private StickRenderType stickRenderType = StickRenderType.Default;
		//private bool enableMouseWheel = true;
		private Bitmap MemBmp;

		private Bitmap CrossMemBmp;
		private Graphics CrossMemBmpG;

		private Bitmap MouseZoomBmp;
		private Graphics MouseZoomBmpG;
		private RectangleF MouseZoomRect;
		public System.Windows.Forms.ContextMenu cmRight;
		private System.Windows.Forms.MenuItem miFavorite;
		private System.Windows.Forms.MenuItem miEdit;
		private System.Windows.Forms.MenuItem miCopy;

		private System.Windows.Forms.ContextMenu cmMain;
		private System.Windows.Forms.MenuItem miChart;
		private System.Windows.Forms.MenuItem miCycle;
		private System.Windows.Forms.MenuItem miIndicator;
		private System.Windows.Forms.MenuItem miAxisType;
		private System.Windows.Forms.MenuItem miNormal;
		private System.Windows.Forms.MenuItem miLog;
		private System.Windows.Forms.MenuItem miSkin;
		private System.Windows.Forms.MenuItem miView;
		private System.Windows.Forms.MenuItem miCrossCursor;
		private System.Windows.Forms.MenuItem miStatistic;
		private System.Windows.Forms.MenuItem miCalculator;
		private System.Windows.Forms.MenuItem miSp1;
		private System.Windows.Forms.MenuItem miChartEdit;
		private System.Windows.Forms.MenuItem miChartCopy;
		private System.Windows.Forms.MenuItem miFormulaManager;
		private System.Windows.Forms.MenuItem miSp2;
		private System.Windows.Forms.MenuItem miSp4;
		private System.Windows.Forms.MenuItem miAddFormula;
		private System.Windows.Forms.MenuItem miCloseFormula;
		private System.Windows.Forms.MenuItem miStatisticWindow;
		private System.Windows.Forms.MenuItem miAdjust;
		private EventHandler ehFavoriteIndicator;

		private System.Windows.Forms.PrintDialog printDialog;
		private System.Drawing.Printing.PrintDocument printDocument;
		private System.Windows.Forms.PrintPreviewDialog previewDialog;
		private System.Windows.Forms.PageSetupDialog setupDialog;
		private string areaPercent = "3;1;1;1;1";
		private string symbol = "MSFT";
		private DataManagerBase historyDataManager;
		private DataManagerBase intraDataManager;
		private DataManagerBase CurrentDataManager;

		private bool EnableYScale;
		private bool EnableXScale;
		private bool EnableResize;

		//private Rectangle LastBindRect = Rectangle.Empty;
		private bool needDrawLastBarOnly = false;
		public FormulaUserSkin UserSkin = new FormulaUserSkin();
		public Easychart.Finance.Win.StatisticControl StatisticWindow;
		
		static public OnSelectFormula OnSelectFormula = new OnSelectFormula(DefaultSelectFormula);
		static public OnSelectString OnSelectMethod = new OnSelectString(DefaultSelectMethod);
		static public OnSelectString OnSelectSymbol = null;

		/// <summary>
		/// If you specify this, will add these text in the price area
		/// Sample format:
		/// {CODE} Prev Close:{LC}O:{OPEN}H:{HIGH}L:{LOW}C:{CLOSE}V:{VOLUME}Chg:{CHANGE}
		/// </summary>
		[Category("Stock Chart")]
		[Description("If you specify this, will add these text in the price area,Sample format:Prev Close:{LC}O:{OPEN}H:{HIGH}L:{LOW}C:{CLOSE}V:{VOLUME}Chg:{Change} {D:yyyy-MM-dd}")]
		public string PriceLabelFormat
		{
			get
			{
				return chart.PriceLabelFormat;
			}
			set
			{
				chart.PriceLabelFormat = value;
			}
		}

#if (!vs2005)
		private Cursor defaultCursor = null;
		[DefaultValue(null), Category("Stock Chart"),Description("Default cursor")]
		public Cursor DefaultCursor 
		{
			get
			{
				return defaultCursor;
			}
			set
			{
				defaultCursor = value;
			}
		}
#endif

		private bool memoryCrossCursor;
		/// <summary>
		/// Draw cross cursor in memory , this will prevent flicker
		/// </summary>
		[Category("Stock Chart")]
		[DefaultValue(false),Description("Draw cross cursor in memory , this will prevent flicker")]
		public bool MemoryCrossCursor
		{
			get
			{
				return memoryCrossCursor;
			}
			set
			{
				memoryCrossCursor = value;
			}
		}

		private bool resetYAfterXChanged = true;
		/// <summary>
		/// Reset Y-axis to auto scale when X-axis changed.
		/// </summary>
		[Category("Stock Chart")]
		[DefaultValue(true),Description("Reset Y-axis to auto scale when X-axis changed.")]
		public bool ResetYAfterXChanged
		{
			get
			{
				return resetYAfterXChanged;
			}
			set
			{
				resetYAfterXChanged = value;
			}
		}

		private bool fixedTime;
		/// <summary>
		/// Fixed time frame between StartTime and EndTime, used for intraday chart
		/// </summary>
		[DefaultValue(false),Category("Stock Chart"),Description("Fixed time frame between StartTime and EndTime, used for intraday chart")]
		public bool FixedTime
		{
			get
			{
				return fixedTime;
			}
			set
			{
				fixedTime = value;
				NeedRebind();
			}
		}


		private ShowLineMode showHorizontalGrid;
		/// <summary>
		/// How to show the horizontal grid line
		/// </summary>
		[DefaultValue(ShowLineMode.Default),Category("Stock Chart"),Description("How to show the horizontal grid line")]
		public ShowLineMode ShowHorizontalGrid
		{
			get
			{
				return showHorizontalGrid;
			}
			set
			{
				showHorizontalGrid = value;
				NeedRedraw();
			}
		}

		private ShowLineMode showVerticalGrid;
		/// <summary>
		/// How to show the vertical grid line
		/// </summary>
		[DefaultValue(ShowLineMode.Default),Category("Stock Chart"),Description("How to show the vertical grid line")]
		public ShowLineMode ShowVerticalGrid
		{
			get
			{
				return showVerticalGrid;
			}
			set
			{
				showVerticalGrid = value;
				NeedRedraw();
			}
		}

		private ExchangeIntraday intradayInfo = ExchangeIntraday.US;
		/// <summary>
		/// Intraday exchange informations
		/// </summary>
		[Browsable(false),Description("Intraday exchange informations")]
		public ExchangeIntraday IntradayInfo
		{
			get
			{
				return intradayInfo;
			}
			set
			{
				intradayInfo = value;
				NeedRebind();
			}
		}

		private MouseAction selectFormulaMouseMode = MouseAction.MouseDown;
	
		/// <summary>
		/// How to select a formula through mouse
		/// </summary>
		[DefaultValue(MouseAction.MouseDown),Category("Stock Chart")]
		[Description("How to select a formula through mouse")]
		public MouseAction SelectFormulaMouseMode
		{
			get
			{
				return selectFormulaMouseMode;
			}
			set
			{
				selectFormulaMouseMode = value;
			}
		}

		private MouseZoomMode mouseZoomMode = MouseZoomMode.Rect;
		/// <summary>
		/// Use mouse to zoom the chart
		/// </summary>
		[Description("Use mouse to zoom the chart")]
		[DefaultValue(MouseZoomMode.Rect),Category("Stock Chart Mouse Zoom")]
		public MouseZoomMode MouseZoomMode
		{
			get
			{
				return mouseZoomMode;
			}
			set
			{
				mouseZoomMode = value;
			}
		}

		private ZoomCenterPosition zoomPosition = ZoomCenterPosition.Center;
		/// <summary>
		/// Position of the zoom center
		/// </summary>
		[DefaultValue(ZoomCenterPosition.Center),Category("Stock Chart Mouse Zoom"), Description("Position of the zoom center")]
		public ZoomCenterPosition ZoomPosition
		{
			get
			{
				return zoomPosition;
			}
			set
			{
				zoomPosition = value;
			}
		}

		private ValueTextMode valueTextMode = ValueTextMode.Default;
		/// <summary>
		/// How to draw the value text
		/// </summary>
		[DefaultValue(ValueTextMode.Default),Category("Stock Chart"), Description("How to draw the value text")]
		public ValueTextMode ValueTextMode
		{
			get
			{
				return valueTextMode;
			}
			set
			{
				valueTextMode = value;
				NeedRebind();
			}
		}

		static private Color defaultZoomBackColor = Color.FromArgb(32,Color.Green);
		private Color mouseZoomBackColor = defaultZoomBackColor;
		/// <summary>
		/// The background color of the mouse zoom
		/// </summary>
		[Description("The background color of the mouse zoom")]
		[DefaultValue(typeof(Color),"32, 0, 128, 0"),Category("Stock Chart Mouse Zoom")]
		public Color MouseZoomBackColor
		{
			get
			{
				return mouseZoomBackColor;
			}
			set
			{
				mouseZoomBackColor = value;
			}
		}

		private Rectangle fixedZoomRect = Rectangle.Empty;
		[Description("Rectangle for fixed zoom box. Empty box will draw the zoom box by hand. Center fixed zoom box (-150,-150,300,300), TopLeft fixed zoom box (0,0,300,300)")]
		[DefaultValue(typeof(Rectangle),"0, 0, 0, 0"), Category("Stock Chart Mouse Zoom")]
		public Rectangle FixedZoomRect
		{
			get
			{
				return fixedZoomRect;
			}
			set
			{
				fixedZoomRect = value;
			}
		}

		/// <summary>
		/// How to show the cross cursor according mouse action, mouse move or mouse down
		/// </summary>
		private MouseAction crossCursorMouseMode = MouseAction.MouseMove;
		[DefaultValue(MouseAction.MouseMove ),Category("Stock Chart")]
		[Description("How to show the cross cursor according mouse action, mouse move or mouse down")]
		public MouseAction CrossCursorMouseMode
		{
			get
			{
				return crossCursorMouseMode;
			}
			set
			{
				crossCursorMouseMode = value;
			}
		}


		private bool nativeContextMenu = true;
		/// <summary>
		/// Use native context menu
		/// </summary>
		[DefaultValue(true),Category("Stock Chart"),Description("Use native context menu")]
		public bool NativeContextMenu
		{
			get
			{
				return nativeContextMenu;
			}
			set
			{
				nativeContextMenu = value;
			}
		}

		private DateTime startTime;
		/// <summary>
		/// Start time of the chart
		/// </summary>
		[DefaultValue(typeof(DateTime), "1-1-1"),Category("Stock Chart Time"),Description("Start time of the chart")]
		public DateTime StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				startTime = value;
				if (fixedTime || (CurrentDataManager!=null && CurrentDataManager.VirtualFetch))
					NeedRebind();
				else NeedRedraw();
				if (memoryCrossCursor)
					needDrawCursor = true;
			}
		}

		private DateTime endTime;
		/// <summary>
		/// End time of the chart, if leave this field empty, will use StockBars instead.
		/// </summary>
		[DefaultValue(typeof(DateTime), "1-1-1"),Category("Stock Chart Time"),Description("End time of the chart, if leave this field empty, will use StockBars instead")]
		public DateTime EndTime
		{
			get
			{
				return endTime;
			}
			set
			{
				endTime = value;
				if (fixedTime || (CurrentDataManager!=null && CurrentDataManager.VirtualFetch))
					NeedRebind();
				else NeedRedraw();
				if (memoryCrossCursor)
					needDrawCursor = true;
			}
		}

		private int stockBars = 150;
		/// <summary>
		/// How many bars will be displayed in the chart control, this will take effect when EndTime is empty.
		/// </summary>
		[DefaultValue(150),Category("Stock Chart Time"),Description("This will take effect when EndTime is empty.")]
		public int StockBars
		{
			get
			{
				return stockBars;
			}
			set
			{
				if (value<1) value = 1;
				stockBars = value;
				NeedRedraw();
				EndTime = DateTime.MinValue;
			}
		}

		private double afterBars = 0.1;
		/// <summary>
		/// How many right space after the last bar of the chart. If the value between 0 and 1, it means the percentage of the bar.
		/// </summary>
		[DefaultValue(0.1),Category("Stock Chart Time"),Description("How many right space after the last bar of the chart. If the value between 0 and 1, it means the percentage of the bar.")]
		public double AfterBars
		{
			get
			{
				return afterBars;
			}
			set
			{
				afterBars = value;
			}
		}

		private double maxPrice;
		/// <summary>
		/// Max value of Y-axis in main stock area
		/// </summary>
		[Browsable(false)]
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
		/// <summary>
		/// Min value of Y-axis in main stock area
		/// </summary>
		[Browsable(false)]
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

		/// <summary>
		/// Historical data manager
		/// </summary>
		[Browsable(false) , DefaultValue(null)]
		public DataManagerBase HistoryDataManager
		{
			get
			{
				return historyDataManager;
			}
			set
			{
				if (value!=historyDataManager)
				{
					historyDataManager = value;
					NeedRebind();
				}
			}
		}

		/// <summary>
		/// Intraday data manager
		/// </summary>
		[Browsable(false) , DefaultValue(null)]
		public DataManagerBase IntraDataManager
		{
			get
			{
				return intraDataManager;
			}
			set
			{
				if (value!=intraDataManager)
				{
					intraDataManager = value;
					NeedRebind();
				}
			}
		}

		/// <summary>
		/// Set data manager , this will set both the Historical data manager and Intraday data manager
		/// </summary>
		/// <example>
		/// ChartWinControl cwc = new ChartWinControl();
		/// cwc.DataManager = new RandomDataManager();
		/// cwc.Symbol = "MSFT";
		/// </example>
		[Browsable(false)]
		public DataManagerBase DataManager
		{
			set
			{
				HistoryDataManager = value;
				IntraDataManager = value;
			}
		}

		private DataCycle currentDataCycle = DataCycle.Day;
		/// <summary>
		/// Get or set current data cycle
		/// </summary>
		[DefaultValue(typeof(DataCycle),"DAY1"),Description("Data Cycle"),Category("Stock Chart")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataCycle CurrentDataCycle
		{
			get
			{
				return currentDataCycle;
			}
			set
			{
				if (currentDataCycle != value)
				{
					if (BeforeCycleChange!=null)
						BeforeCycleChange(this,new EventArgs());

					if (value.CycleBase>=DataCycleBase.DAY && chart.FixedTime) 
					{
						fixedTime = false;
						chart.FixedTime = false;
					}

					NeedRebind();
					currentDataCycle = value;
					if (AfterCycleChange!=null)
						AfterCycleChange(this,new EventArgs());
				}
			}
		}

		/// <summary>
		/// Stock symbol
		/// </summary>
		/// <example>
		/// ChartWinControl cwc = new ChartWinControl();
		/// cwc.DataManager = new RandomDataManager();
		/// cwc.Symbol = "MSFT";
		/// </example>
		[DefaultValue("MSFT"),Description("Stock Symbol"),Category("Stock Chart")]
		public string Symbol
		{
			get
			{
				return symbol;
			}
			set
			{
				if (value!=symbol)
				{
					symbol = value;
					NeedRebind();
				}
			}
		}
		
		/// <summary>
		/// Height percentage of each area,separated by semi colon
		/// </summary>
		[Description("Height percentage of each area,separated by semi colon."),Category("Stock Chart")]
		[DefaultValue("3;1;1;1;1")]
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

		/// <summary>
		/// Adjust the data automatically when there is price split
		/// </summary>
		[Description("Adjust the data automatically when there is price split"),Category("Stock Chart")]
		[DefaultValue(true)]
		public bool AdjustData
		{
			get
			{
				return adjustData;
			}
			set
			{
				adjustData = value;
				NeedRebind();
			}
		}

		/// <summary>
		/// Stock chart render type
		/// </summary>
		[Description("Stock chart render type"),Category("Stock Chart")]
		[DefaultValue(StockRenderType.Default)]
		public StockRenderType StockRenderType
		{
			get
			{
				return stockRenderType;
			}
			set
			{
				stockRenderType = value;
				NeedRebind();
			}
		}

		/// <summary>
		/// Stick render type is used for Volumn Stick,Color Stick and Stick Line
		/// </summary>
		[Description("Stick render type is used for Volumn Stick,Color Stick and Stick Line")]
		[Category("Stock Chart")]
		[DefaultValue(StickRenderType.Default)]
		public StickRenderType StickRenderType
		{
			get
			{
				return stickRenderType;
			}
			set
			{
				stickRenderType = value;
				NeedRebind();
			}
		}

		private MouseWheelMode mouseWheelMode = MouseWheelMode.Scroll;
		/// <summary>
		/// Use mouse wheel to scroll or zoom the chart
		/// </summary>
		[DefaultValue(MouseWheelMode.Scroll),Category("Stock Chart"),Description("Use mouse wheel to scroll or zoom the chart")]
		public MouseWheelMode MouseWheelMode
		{
			get
			{
				return mouseWheelMode;
			}
			set
			{
				mouseWheelMode = value;
			}
		}

		private LatestValueType latestValueType = LatestValueType.All;
		/// <summary>
		/// How to show the latest value labels in the axis-Y
		/// </summary>
		[DefaultValue(LatestValueType.All),Category("Stock Chart"),Description("How to show the latest value in the axis-Y")]
		public LatestValueType LatestValueType
		{
			get
			{
				return latestValueType;
			}
			set
			{
				latestValueType = value;
				NeedRedraw();
			}
		}

		/// <summary>
		/// Show indicator values in the statistic window
		/// </summary>
		[DefaultValue(true),Category("Stock Chart"),Description("Show indicator values in the statistic window")]
		public bool ShowIndicatorValues
		{
			get
			{
				return showIndicatorValues;
			}
			set
			{
				showIndicatorValues = value;
			}
		}

		/// <summary>
		/// Show overlay values in the statistic window
		/// </summary>
		[DefaultValue(true),Category("Stock Chart"),Description("Show overlay values in the statistic window")]
		public bool ShowOverlayValues
		{
			get
			{
				return showOverlayValues;
			}
			set
			{
				showOverlayValues = value;
			}
		}
		
		#region IObjectCanvas
		private FormulaChart chart;
		/// <summary>
		/// Return Underlay formula chart
		/// </summary>
		[Browsable(false)]
		public FormulaChart Chart
		{
			get
			{
				return chart;
			}
		}

		/// <summary>
		/// return current control, used by stock object
		/// </summary>
		[Browsable(false)]
		public Control DesignerControl 
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// return if in the designing mode, used by stock object
		/// </summary>
		[Browsable(false)]
		public bool Designing
		{
			get 
			{
				return designing;
			}
			set 
			{
				designing = value;
			}
		}
		#endregion

		/// <summary>
		/// Behaver when drag the chart
		/// </summary>
		[Description("Behaver when drag the chart"),Category("Stock Chart")]
		[DefaultValue(ChartDragMode.Axis)]
		public ChartDragMode ChartDragMode
		{
			get
			{
				return chartDragMode;
			}
			set
			{
				chartDragMode = value;
			}
		}
		
		private bool showCrossCursor = true;
		/// <summary>
		/// Show cross cursor on the chart control
		/// </summary>
		[Description("Show cross cursor on the chart"),Category("Stock Chart")]
		[DefaultValue(true)]
		public bool ShowCrossCursor
		{
			get 
			{
				return showCrossCursor;
			}
			set 
			{
				showCrossCursor = value;
				miCrossCursor.Checked = value;
				chart.ShowHLine = value;
				chart.ShowVLine = value;
				if (!value) Invalidate();
			}
		}

		/// <summary>
		/// First bar index of the chart control.
		/// </summary>
		[Browsable(false), Category("Stock Chart Time"),DefaultValue(0)]
		public int StartBar
		{
			get
			{
				return chart.Start;
			}
			set
			{
				if (value<0) value=0;
				chart.Start = value;
				if (!chart.FixedTime)
				{
					chart.StartTime = DateTime.MinValue;
					chart.EndTime = DateTime.MaxValue;
				}
				NeedRedraw();
			}
		}

		private bool showStatistic = true;
		/// <summary>
		/// Show statistic windows on the chart
		/// </summary>
		[DefaultValue(true),Description("Show statistic windows on the chart"),Category("Stock Chart")]
		public bool ShowStatistic
		{
			get 
			{
				return showStatistic;
			}
			set 
			{
				bool b = showStatistic!=value && ShowStatisticChanged!=null;
				showStatistic = value;
				if (b)
					ShowStatisticChanged(this,new EventArgs());
			}
		}

		/// <summary>
		/// Occurs when cursor pos changed
		/// </summary>
		[Category("Stock Chart"),Description("Occurs when cursor pos changed")]
		public event CursorPosChanged CursorPosChanged;
		/// <summary>
		/// Occurs before data cycle changed
		/// </summary>
		[Category("Stock Chart"),Description("Occurs before data cycle changed")]
		public event EventHandler BeforeCycleChange;
		/// <summary>
		/// Occurs after data cycle changed
		/// </summary>
		[Category("Stock Chart"),Description("Occurs after data cycle changed")]
		public event EventHandler AfterCycleChange;
		/// <summary>
		/// Occurs before apply skin
		/// </summary>
		[Category("Stock Chart"),Description("Occurs before apply skin")]
		public event ApplySkinHandler BeforeApplySkin;

		/// <summary>
		/// Occurs when the value of ShowStatistic Changed
		/// </summary>
		[Category("Stock Chart"),Description("Occurs when the value of ShowStatistic Changed")]
		public event EventHandler ShowStatisticChanged;

		/// <summary>
		/// Occurs after apply skin
		/// </summary>
		[Category("Stock Chart"),Description("Occurs after apply skin")]
		public event EventHandler AfterApplySkin;

		/// <summary>
		/// Occurs after mouse move
		/// </summary>
		[Category("Stock Chart"),Description("Occurs after mouse move")]
		public event AfterMouseMove AfterMouseMove;

		/// <summary>
		/// Occurs after data bind
		/// </summary>
		[Category("Stock Chart"),Description("Occurs after data bind")]
		public event AfterBindData AfterBindData;

		/// <summary>
		/// Occurs when paint the chart on native canvas
		/// </summary>
		[Category("Stock Chart"),Description("Occurs when paint the chart on native canvas")]
		public event NativePaintHandler NativePaint;
		/// <summary>
		/// Occurs when paint extra informations on the chart, it was used by "Easy Stock Object"
		/// </summary>
		[Category("Stock Chart"),Description("Occurs when paint extra informations on the chart, it was used by Easy Stock Object")]
		public event NativePaintHandler ExtraPaint;
		/// <summary>
		/// Occurs when stock data or data cycle changed
		/// </summary>
		[Category("Stock Chart"),Description("Occurs when stock data or data cycle changed")]
		public event EventHandler DataChanged;
		/// <summary>
		/// Occurs when stock view changed
		/// </summary>
		[Category("Stock Chart"),Description("Occurs when stock view changed")]
		public event ViewChangedHandler ViewChanged;

		/// <summary>
		/// Create instance of the ChartWinControl
		/// </summary>
		public ChartWinControl()
		{
			InitializeComponent();

			Page = 10;
			LastCursorPos = -1;
			favoriteFormulas = new ArrayList();
			EnableYScale = true;
			EnableXScale = true;
			EnableResize = true;

			chart = new FormulaChart();
			chart.BitmapCache = true;
			chart.LatestValueType = this.LatestValueType;
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ChartWinControl_MouseWheel);
			this.AfterApplySkin +=new EventHandler(ChartWinControl_AfterApplySkin);
			chart.NativePaint +=new NativePaintHandler(Chart_NativePaint);
			chart.ExtraPaint +=new NativePaintHandler(Chart_ExtraPaint);
			chart.ViewChanged +=new ViewChangedHandler(Chart_ViewChanged);

			chart.AddArea("MAIN",3);
			//DefaultFormulas = "MAIN#AreaBB#MA(50)#MA(200);VOLMA;RSI(14)#RSI(28);MACD;SlowSTO";
			FavoriteFormulas = "VOLMA;RSI;CCI;OBV;ATR;FastSTO;SlowSTO;ROC;TRIX;WR;AD;CMF;PPO;StochRSI;ULT;BBWidth;PVO";

			CursorPosChanged += new CursorPosChanged(ShowStatisticWindow);
			ehFavoriteIndicator = new EventHandler(mmFavorite_SelectedIndexChanged);
			CreateCycleMenu();
			StatisticWindow.OnHide+=new EventHandler(StatisticWindow_OnHide);
		}

		static public string DoSelectFormula(string Default,string[] FilterPrefixes,bool SelectLine)
		{
			if (OnSelectFormula!=null)
				return OnSelectFormula(Default,FilterPrefixes,SelectLine);
			return Default;
		}

		static private string DefaultSelectFormula(string Default,string[] FilterPrefixes,bool SelectLine)
		{
			return new SelectFormula().Select(Default,FilterPrefixes,SelectLine);
		}

		static private string DefaultSelectMethod(string Default)
		{
			return new SelectMethod().Select(Default);
		}

		static public string DoSelectSymbol(string Default)
		{
			if (OnSelectSymbol!=null)
				return OnSelectSymbol(Default);
			return Default;
		}


		/// <summary>
		/// Minimum stock bar column width
		/// </summary>
		[Category("Stock Chart")]
		[DefaultValue(0.01)]
		public double MinColumnWidth
		{
			get
			{
				return minColumnWidth;
			}
			set
			{
				minColumnWidth = value;
			}
		}

		/// <summary>
		/// Maximum stock bar column width
		/// </summary>
		[Category("Stock Chart")]
		[DefaultValue(500.0)]
		public double MaxColumnWidth
		{
			get
			{
				return maxColumnWidth;
			}
			set
			{
				maxColumnWidth = value;
			}
		}

		private double columnWidth = 4.0;
		/// <summary>
		/// Stock bar column width, not used currently
		/// </summary>
		[Browsable(false), DefaultValue(4.0), Category("Stock Chart Time")]
		public double ColumnWidth
		{
			get
			{
				return columnWidth;
			}
			set
			{
				columnWidth = value;
			}
		}

		/// <summary>
		/// Redraw current area
		/// </summary>
		/// <param name="fa">The formula area need to be redraw</param>
		public void NeedRedraw(FormulaArea fa)
		{
			needRedraw = true;
			if (fa!=null)
			{
				Rectangle R = fa.Rect;
				R.Inflate(1,1);
				Invalidate(R);
			}
			else Invalidate();
//			if (!needRebind)
//				needDrawLastBarOnly = false;
		}

		/// <summary>
		/// Redraw the chart control
		/// </summary>
		public void NeedRedraw()
		{
			NeedRedraw(null);
		}

		private bool needAutoScaleY = true;


		/// <summary>
		/// Rebind formula areas
		/// </summary>
		/// <param name="AutoScaleY">Set axis-y to auto scale mode if true</param>
		/// <param name="DrawLastBarOnly">Redraw last bar only</param>
		private void NeedRebind(bool AutoScaleY,bool DrawLastBarOnly)
		{
			needAutoScaleY = AutoScaleY;
			needDrawLastBarOnly = DrawLastBarOnly;
			needRedraw = true;
			needRebind = true;
			Invalidate();
		}

		/// <summary>
		/// Rebind formula areas
		/// </summary>
		/// <param name="AutoScaleY">Set axis-y to auto scale mode if true </param>
		public void NeedRebind(bool AutoScaleY)
		{
			NeedRebind(AutoScaleY,false);
		}

		/// <summary>
		/// Rebind formula areas
		/// </summary>
		public void NeedRebind()
		{
			NeedRebind(true);
		}

		/// <summary>
		/// Refresh the formula when formula plugins changed
		/// </summary>
		public void NeedRefresh()
		{
			needRedraw = true;
			needRebind = true;
			needRefresh = true;
			Invalidate();
		}

		/// <summary>
		/// Draw a line on top of the chart control
		/// </summary>
		[DefaultValue(true), Category("Stock Chart")]
		[Description("Draw a line on top of the chart control")]
		public bool ShowTopLine
		{
			get 
			{
				return showTopLine;
			}
			set 
			{
				showTopLine = value;
			}
		}

		/// <summary>
		/// Setup skin MenuItem
		/// </summary>
		/// <returns></returns>
		public MenuItem GetSkinMenu() 
		{
			string[] ss = FormulaSkin.GetBuildInSkins();
			miSkin.MenuItems.Clear();
			foreach(string s in ss)
				miSkin.MenuItems.Add(s,new EventHandler(mmSkin_SelectedIndexChanged));
			//miSkin.MenuItems[miSkin.MenuItems.Count-1].PerformClick();
			return miSkin;
		}

		/// <summary>
		/// Apply new skins to the chart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mmSkin_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			foreach(MenuItem mi in miSkin.MenuItems)
				mi.Checked = false;
			MenuItem miCurrent = ((MenuItem)sender);
			miCurrent.Checked = true;
			string s = miCurrent.Text;
			Skin = s;
		}

		/// <summary>
		/// Get the default chart menu, this will include skin sub menu,cycle sub menu, area sub menu, axis type sub menu
		/// </summary>
		/// <returns></returns>
		public MenuItem GetChartMenu()
		{
			return GetChartMenu(true,true,true,true);
		}

		public MenuItem GetChartMenu(bool Skin,bool Cycle,bool Area, bool Axis)
		{
			miChart.MenuItems.Clear();
			//Setup skin MenuItem
			if (Skin)
				miChart.MenuItems.Add(GetSkinMenu());
			
			//Setup data cycle MenuItem
			if (Cycle)
				miChart.MenuItems.Add(miCycle);

			//Set indicator areas , 9 areas maximum
			if (Area)
				miChart.MenuItems.Add(GetAreaMenu(9));

			//Set Axis type menu.
			if (Axis)
				miChart.MenuItems.Add(GetAxisMenu());
			return miChart;
		}

		/// <summary>
		/// Get edit menu, include show copy menu currently
		/// </summary>
		/// <returns></returns>
		public MenuItem GetEditMenu() 
		{
			return miChartEdit;
		}

		/// <summary>
		/// Get view menu, include cross cursor,statistic windows menu item
		/// </summary>
		/// <returns></returns>
		public MenuItem GetViewMenu() 
		{
			return miView;
		}

		private void miCycle_Popup(object sender, System.EventArgs e)
		{
			foreach(MenuItem mi in miCycle.MenuItems)
				mi.Checked = mi.Text == currentDataCycle.ToString();
		}

		/// <summary>
		/// Apply new data cycle to the chart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miCycle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			EndTime = DateTime.MinValue;
			CurrentDataCycle =DataCycle.Parse((sender as MenuItem).Text);
		}

		/// <summary>
		/// This will create and return the formula area menus
		/// </summary>
		/// <param name="Count"></param>
		/// <returns></returns>
		public MenuItem GetAreaMenu(int Count) 
		{
			miIndicator.MenuItems.Clear();
			for(int i=0; i<Count; i++) 
			{
				miIndicator.MenuItems.Add(
					new MenuItem( "&"+(i+1)+" area"+((i>0)?"s":""),
					new EventHandler(miIndicator_SelectedIndexChanged),
					(Shortcut)(Shortcut.Alt1+i))
					);
			}
			return miIndicator;
		}

		/// <summary>
		/// Use or don't use DefaultFormulas and OverlayFormulas properties of the chart control for default areas
		/// </summary>
		/// <param name="b">True if you want to use DefaultFormulas and OverlayFormulas for default areas</param>
		public void UseDefaultFormulas(bool b)
		{
			//needSetOverlayFormulas = b;
			needSetDefaultFormulas = b;
		}

		private void SetAreaCount(int Count)
		{
			if (chart.Areas.Count!=Count)
			{
				while (chart.Areas.Count>Count)
					chart.Areas.RemoveAt(Count);

				int LastCount = chart.Areas.Count;
				ExpandDefaultFormulas(Count);
				double H =1;
				if (chart.Areas.Count>0) 
					H = chart[0].HeightPercent/3;
				for(int i=LastCount; i<chart.Areas.Count; i++)
					chart[i].HeightPercent = H;
				areaPercent = chart.GetAreaPercent();
				NeedRebind();
			}
			UseDefaultFormulas(false);
		}

		private void miIndicator_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			AreaCount = (sender as MenuItem).Index+1;
		}

		/// <summary>
		/// Return the axis menu
		/// </summary>
		/// <returns></returns>
		public MenuItem GetAxisMenu() 
		{
			return miAxisType;
		}

		private ScaleType scaleType = ScaleType.Default;
		/// <summary>
		/// Get or set the scale type of main stock area
		/// </summary>
		[DefaultValue(ScaleType.Default), Category("Stock Chart"),Description("Get or set the scale type of main stock area")]
		public ScaleType ScaleType
		{
			get
			{
				return scaleType;
			}
			set
			{
				scaleType = value;
				NeedRedraw();
			}
		}

		/// <summary>
		/// Switch Normal/Log in AxisY
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miNormalAxis_Click(object sender, System.EventArgs e)
		{
			foreach(MenuItem m in miAxisType.MenuItems)
				m.Checked = false;
			MenuItem mi = (sender as MenuItem);
			mi.Checked = true;
			ScaleType = (ScaleType)mi.MergeOrder;
		}

		/// <summary>
		/// This function prevent flicker 
		/// </summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			if (chart==null || chart.DataProvider==null)
				base.OnPaintBackground (pevent);
		}

		bool showCursorLabel = true;
		/// <summary>
		/// Show cursor label on the chart control
		/// </summary>
		[DefaultValue(true), Category("Stock Chart")]
		public bool ShowCursorLabel
		{
			get 
			{
				return showCursorLabel;
			}
			set 
			{
				showCursorLabel = value;
				NeedRebind();
			}
		}

		private string[] Split(string s,char separator)
		{
			ArrayList al = new ArrayList();
			int j = 0;
			int k = 0;
			for(int i=0; i<s.Length; i++)
			{
				if (s[i]=='(') j++;
				else if (s[i]==')') j--;
				if (j==0 && s[i]==separator)
				{
					al.Add(s.Substring(k,i-k));
					k = i+1;
				}
			}
			return (string[])al.ToArray(typeof(string));
		}

		private void AreasToDefaultFormulas()
		{
			defaultFormulas = string.Join(";",chart.AreaToStrings());// string.Join(";",chart.AreaToStrings(1,chart.Areas.Count-1));
		}

		private void DefaultFormulasToAreas()
		{
			if (defaultFormulas!=null)
			{
				chart.Areas.Clear();
				chart.StringsToArea(defaultFormulas.Split(';'));

//				while (chart.Areas.Count>1)
//					chart.Areas.RemoveAt(1);
//				if (defaultFormulas!="")
//					chart.StringsToArea(defaultFormulas.Split(';'));
			}
		}

		private bool needSetDefaultFormulas;
		private string defaultFormulas;
		/// <summary>
		/// Default formulas shown on the chart control,separate by semi colon, these formulas was defined in the plugins
		/// </summary>
		[Category("Stock Chart"),DefaultValue("VOLMA;RSI(14)#RSI(28);MACD;SlowSTO")]
		public string DefaultFormulas
		{
			get 
			{
				return defaultFormulas;
			}
			set
			{
				defaultFormulas = value;
				needSetDefaultFormulas = true;
				NeedRebind();
			}
		}

		/// <summary>
		/// Save chart default formulas and overlays formulas to properties
		/// </summary>
		public void SaveChartProperties()
		{
			//AreasToOverlayFormulas();
			AreasToDefaultFormulas();
			areaPercent = chart.GetAreaPercent();
		}

		private void ExpandDefaultFormulas(int Count) 
		{
			double H = chart.Areas[0].HeightPercent/3;
			if (H<1)
				H = 1;
			while (Count>chart.Areas.Count)
				chart.AddArea("VOLMA",H);
		}

		private void SetDefaultFormulas(int Index,string s) 
		{
			ExpandDefaultFormulas(Index);
			chart[Index].Formulas.Clear();
			chart[Index].StringToFormula(s,'#');
		}

		/// <summary>
		/// Favorite formulas listed on the right menu of the chart control
		/// </summary>
		[Category("Stock Chart")]
		public string FavoriteFormulas
		{
			set 
			{
				if (value!=null && value!="") 
				{
					favoriteFormulas.Clear();
					favoriteFormulas.AddRange(value.Split(';'));
					BuildFavoriteMenu();
				}
			}
			get 
			{
				return string.Join(";",(string[])favoriteFormulas.ToArray(typeof(string)));
			}
		}

		const string defaultFavoriteCycles = "MINUTE1;MINUTE5;MINUTE15;MINUTE30;HOUR1;DAY1;WEEK1;MONTH1;YEAR1";
		private string favoriteCycles = defaultFavoriteCycles;
		/// <summary>
		/// Favorite data cycles,listed on the cycle menu.
		/// </summary>
		[DefaultValue(defaultFavoriteCycles), Category("Stock Chart")]
		public string FavoriteCycles
		{
			get 
			{
				return favoriteCycles;
			}
			set
			{
				favoriteCycles = value;
				CreateCycleMenu();
			}
		}
		
		/// <summary>
		/// Bind formulas to area
		/// </summary>
		/// <param name="AreaIndex">Area index</param>
		/// <param name="FormulaName">Formula name, defined in the formula plugins</param>
		public void SetAreaByName(int AreaIndex,string FormulaName)
		{
			if (AreaIndex>=0 && AreaIndex<chart.Areas.Count) 
				SetAreaByName(chart.Areas[AreaIndex],FormulaName);
		}

		/// <summary>
		/// Bind formulas to area
		/// </summary>
		/// <param name="fa">Formula area</param>
		/// <param name="FormulaName">Formula name, defined in the formula plugins</param>
		public void SetAreaByName(FormulaArea fa ,string FormulaName)
		{
			if (fa!=null) 
			{
				int Index = chart.Areas.IndexOf(fa);
				if (Index>=0)
				{
					string s = fa.FormulaToString('#');
					try 
					{
						SetDefaultFormulas(Index,FormulaName);
						//fa.Bind();
					} 
					catch 
					{
						SetDefaultFormulas(Index,s);
						//fa.Bind();
						throw;
					}
					SaveChartProperties();
					NeedRebind();
					//NeedRedraw();
				}
			}
		}

		private void RecreateFormula(FormulaArea fa) 
		{
			string[] ss = fa.FormulaToStrings();
			fa.Formulas.Clear();
			fa.StringsToFormula(ss);
		}

		private void RecreateFormula() 
		{
			if (chart!=null)
				foreach(FormulaArea fa in chart.Areas)
					RecreateFormula(fa);
			SaveChartProperties();
		}

		/// <summary>
		/// Bind formula to selected area
		/// </summary>
		/// <param name="FormulaName"></param>
		public void SetAreaByName(string FormulaName)
		{
			FormulaArea fa = chart.SelectedArea;
			if (fa==null)
				fa = chart.Areas[chart.Areas.Count-1];
			SetAreaByName(fa,FormulaName);
		}

		//private int areaCount = 4;
		/// <summary>
		/// Area count on the chart control
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AreaCount
		{
			get 
			{
				 return chart.Areas.Count;
			}
			set 
			{
				SetAreaCount(value);
			}
		}

		/// <summary>
		/// Let chart control handle the key event
		/// </summary>
		/// <param name="sender">Sender of this event</param>
		/// <param name="e">Key event</param>
		public void HandleKeyEvent(object sender,KeyEventArgs e)
		{
			if (ContainsFocus)
				this.OnKeyDown(e);
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
			this.cmRight = new System.Windows.Forms.ContextMenu();
			this.miAddFormula = new System.Windows.Forms.MenuItem();
			this.miCloseFormula = new System.Windows.Forms.MenuItem();
			this.miFormulaManager = new System.Windows.Forms.MenuItem();
			this.miEdit = new System.Windows.Forms.MenuItem();
			this.miFavorite = new System.Windows.Forms.MenuItem();
			this.miSp4 = new System.Windows.Forms.MenuItem();
			this.miStatisticWindow = new System.Windows.Forms.MenuItem();
			this.miAdjust = new System.Windows.Forms.MenuItem();
			this.miSp2 = new System.Windows.Forms.MenuItem();
			this.miCopy = new System.Windows.Forms.MenuItem();
			this.cmMain = new System.Windows.Forms.ContextMenu();
			this.miChart = new System.Windows.Forms.MenuItem();
			this.miSkin = new System.Windows.Forms.MenuItem();
			this.miCycle = new System.Windows.Forms.MenuItem();
			this.miIndicator = new System.Windows.Forms.MenuItem();
			this.miAxisType = new System.Windows.Forms.MenuItem();
			this.miNormal = new System.Windows.Forms.MenuItem();
			this.miLog = new System.Windows.Forms.MenuItem();
			this.miView = new System.Windows.Forms.MenuItem();
			this.miCrossCursor = new System.Windows.Forms.MenuItem();
			this.miStatistic = new System.Windows.Forms.MenuItem();
			this.miSp1 = new System.Windows.Forms.MenuItem();
			this.miCalculator = new System.Windows.Forms.MenuItem();
			this.miChartEdit = new System.Windows.Forms.MenuItem();
			this.miChartCopy = new System.Windows.Forms.MenuItem();
			this.StatisticWindow = new Easychart.Finance.Win.StatisticControl();
			this.SuspendLayout();
			// 
			// cmRight
			// 
			this.cmRight.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.miAddFormula,
																					this.miCloseFormula,
																					this.miFormulaManager,
																					this.miEdit,
																					this.miFavorite,
																					this.miSp4,
																					this.miStatisticWindow,
																					this.miAdjust,
																					this.miSp2,
																					this.miCopy});
			this.cmRight.Popup += new System.EventHandler(this.cmRight_Popup);
			// 
			// miAddFormula
			// 
			this.miAddFormula.Index = 0;
			this.miAddFormula.Text = "&Add New Formula Area";
			this.miAddFormula.Click += new System.EventHandler(this.miAddFormula_Click);
			// 
			// miCloseFormula
			// 
			this.miCloseFormula.Index = 1;
			this.miCloseFormula.MergeOrder = 10;
			this.miCloseFormula.Text = "Close Selected Formula";
			this.miCloseFormula.Click += new System.EventHandler(this.miCloseFormula_Click);
			// 
			// miFormulaManager
			// 
			this.miFormulaManager.Index = 2;
			this.miFormulaManager.MergeOrder = 20;
			this.miFormulaManager.Text = "E&dit Formula";
			this.miFormulaManager.Click += new System.EventHandler(this.miFormulaEditor_Click);
			// 
			// miEdit
			// 
			this.miEdit.Index = 3;
			this.miEdit.MergeOrder = 30;
			this.miEdit.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.miEdit.Text = "&Edit Formula Source Code";
			this.miEdit.Click += new System.EventHandler(this.miEdit_Click);
			// 
			// miFavorite
			// 
			this.miFavorite.Index = 4;
			this.miFavorite.MergeOrder = 40;
			this.miFavorite.Text = "&Favorite Indicators";
			// 
			// miSp4
			// 
			this.miSp4.Index = 5;
			this.miSp4.MergeOrder = 50;
			this.miSp4.Text = "-";
			// 
			// miStatisticWindow
			// 
			this.miStatisticWindow.Index = 6;
			this.miStatisticWindow.MergeOrder = 80;
			this.miStatisticWindow.Text = "Show &statistic window";
			this.miStatisticWindow.Click += new System.EventHandler(this.miStatisticWindow_Click);
			// 
			// miAdjust
			// 
			this.miAdjust.Index = 7;
			this.miAdjust.MergeOrder = 90;
			this.miAdjust.Text = "&Adjust Data";
			this.miAdjust.Click += new System.EventHandler(this.miAdjust_Click);
			// 
			// miSp2
			// 
			this.miSp2.Index = 8;
			this.miSp2.MergeOrder = 100;
			this.miSp2.Text = "-";
			// 
			// miCopy
			// 
			this.miCopy.Index = 9;
			this.miCopy.MergeOrder = 110;
			this.miCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.miCopy.Text = "&Copy";
			this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
			// 
			// cmMain
			// 
			this.cmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miChart,
																				   this.miView,
																				   this.miChartEdit});
			// 
			// miChart
			// 
			this.miChart.Index = 0;
			this.miChart.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.miSkin,
																					this.miCycle,
																					this.miIndicator,
																					this.miAxisType});
			this.miChart.Text = "&Chart";
			// 
			// miSkin
			// 
			this.miSkin.Index = 0;
			this.miSkin.Text = "&Skin";
			// 
			// miCycle
			// 
			this.miCycle.Index = 1;
			this.miCycle.Text = "&Cycle";
			this.miCycle.Click += new System.EventHandler(this.miCycle_Popup);
			// 
			// miIndicator
			// 
			this.miIndicator.Index = 2;
			this.miIndicator.Text = "&Indicator Areas";
			// 
			// miAxisType
			// 
			this.miAxisType.Index = 3;
			this.miAxisType.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.miNormal,
																					   this.miLog});
			this.miAxisType.Text = "&Axis Type";
			// 
			// miNormal
			// 
			this.miNormal.Index = 0;
			this.miNormal.RadioCheck = true;
			this.miNormal.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.miNormal.Text = "Normal";
			this.miNormal.Click += new System.EventHandler(this.miNormalAxis_Click);
			// 
			// miLog
			// 
			this.miLog.Index = 1;
			this.miLog.MergeOrder = 1;
			this.miLog.RadioCheck = true;
			this.miLog.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.miLog.Text = "Logarithm";
			this.miLog.Click += new System.EventHandler(this.miNormalAxis_Click);
			// 
			// miView
			// 
			this.miView.Index = 1;
			this.miView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miCrossCursor,
																				   this.miStatistic,
																				   this.miSp1,
																				   this.miCalculator});
			this.miView.Text = "&View";
			this.miView.Popup += new System.EventHandler(this.miView_Popup);
			// 
			// miCrossCursor
			// 
			this.miCrossCursor.Checked = true;
			this.miCrossCursor.Index = 0;
			this.miCrossCursor.Text = "Cross Cursor";
			this.miCrossCursor.Click += new System.EventHandler(this.miCrossCursor_Click);
			// 
			// miStatistic
			// 
			this.miStatistic.Index = 1;
			this.miStatistic.Text = "Statistic Window";
			this.miStatistic.Click += new System.EventHandler(this.miStatistic_Click);
			// 
			// miSp1
			// 
			this.miSp1.Index = 2;
			this.miSp1.Text = "-";
			// 
			// miCalculator
			// 
			this.miCalculator.Index = 3;
			this.miCalculator.Shortcut = System.Windows.Forms.Shortcut.CtrlJ;
			this.miCalculator.Text = "&Calculator";
			this.miCalculator.Click += new System.EventHandler(this.miCalculator_Click);
			// 
			// miChartEdit
			// 
			this.miChartEdit.Index = 2;
			this.miChartEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.miChartCopy});
			this.miChartEdit.Text = "&Edit";
			// 
			// miChartCopy
			// 
			this.miChartCopy.Index = 0;
			this.miChartCopy.Text = "&Copy";
			this.miChartCopy.Click += new System.EventHandler(this.miChartCopy_Click);
			// 
			// StatisticWindow
			// 
			this.StatisticWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.StatisticWindow.BackColor = System.Drawing.SystemColors.Info;
			this.StatisticWindow.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.StatisticWindow.ForeColor = System.Drawing.Color.Black;
			this.StatisticWindow.Location = new System.Drawing.Point(-1000, 56);
			this.StatisticWindow.Name = "StatisticWindow";
			this.StatisticWindow.Size = new System.Drawing.Size(192, 296);
			this.StatisticWindow.TabIndex = 0;
			this.StatisticWindow.Visible = false;
			// 
			// ChartWinControl
			// 
			this.CausesValidation = false;
			this.Controls.Add(this.StatisticWindow);
			this.Name = "ChartWinControl";
			this.Size = new System.Drawing.Size(584, 448);
			this.VisibleChanged += new System.EventHandler(this.ChartWinControl_VisibleChanged);
			this.SizeChanged += new System.EventHandler(this.ChartWinControl_SizeChanged);
			this.Enter += new System.EventHandler(this.ChartWinControl_Enter);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChartWinControl_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ChartWinControl_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChartWinControl_KeyDown);
			this.DoubleClick += new System.EventHandler(this.ChartWinControl_DoubleClick);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChartWinControl_MouseMove);
			this.MouseLeave += new System.EventHandler(this.ChartWinControl_MouseLeave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartWinControl_MouseDown);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Merge context menu
		/// </summary>
		/// <param name="cm">Context menu to be merged</param>
		public void MergeRightMenu(ContextMenu cm) 
		{
			cmRight.MergeMenu(cm);
		}

		private void NextFormula(int Delta)
		{
			int i = chart.Areas.IndexOf(chart.SelectedArea);
			if (i<0)
				i = chart.Areas.Count-1;

			if (i>=0)
			{
				FormulaArea fa = chart[i];
				if (!fa.IsMain())
				{
					string s = fa.FormulaToString('#');
					int k = s.IndexOf('(');
					if (k>=0)
						s = s.Substring(0,k);
					int j=-1;
					for(int t=0; t<favoriteFormulas.Count; t++)
						if (string.Compare(s,(string)favoriteFormulas[t],true)==0)
							j = t;
					if (j<0) j = 0;

					j = (j+Delta+favoriteFormulas.Count) % favoriteFormulas.Count;
					SetAreaByName(i,(string)favoriteFormulas[j]);
					NeedRebind();
				}
			}
		}

		private void CreateCycleMenu()
		{
			if (favoriteCycles!=null)
			{
				miCycle.MenuItems.Clear();
				foreach(string s in favoriteCycles.Split(';'))
					miCycle.MenuItems.Add(s,new EventHandler(miCycle_SelectedIndexChanged));
			}
		}

		private void NextCycle(int Delta)
		{
			if (favoriteCycles!=null && favoriteCycles!="") 
			{
				ArrayList dataCycles = new ArrayList();
				dataCycles.AddRange(favoriteCycles.Split(';'));

				int i = dataCycles.IndexOf(CurrentDataCycle.ToString());
				if (i<0) i = 0;
				i = (i+Delta+dataCycles.Count) % dataCycles.Count;
				if (!fixedTime)
					EndTime = DateTime.MinValue;
				CurrentDataCycle = DataCycle.Parse(dataCycles[i].ToString());
			}
		}

		private void NextArea(int Delta) 
		{
			int Start = chart.Areas.IndexOf(chart.SelectedArea);
			Start = (Start+Delta+chart.Areas.Count) % chart.Areas.Count;
			if (Start<chart.Areas.Count)
				chart.SelectedArea = chart[Start];

//			if (Start<0)
//				Start = chart.Areas.Count-1;
//
//			for(int i=Start; ; )
//			{
//				i = (i+Delta+chart.Areas.Count) % chart.Areas.Count;
//				if (i==Start) return;
//				if (!chart[i].IsMain()) 
//				{
//					chart.SelectedArea = chart[i];
//					break;
//				}
//			}
			NeedRedraw();
		}

		private void DrawCursor(Graphics g)
		{
			chart.DrawCursor(g,LastHitInfo.X,LastHitInfo.Y,false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="DefaultY">
		///		-1 don't show Y-cross cursor
		///		NaN show Y-cross cursor at price
		/// </param>
		private void DrawCursorByPos(Graphics g,float DefaultY)
		{
			float X;
			float Y;
			chart.GetXYFromPos(out X,out Y);

			LastHitInfo = chart.GetHitInfo(X,Y);
			if (!float.IsNaN(DefaultY))
				LastHitInfo.Y = DefaultY;

			if (LastHitInfo.HitType==FormulaHitType.htArea && g!=null)
				DrawCursor(g);
			else LastHitInfo.X=-1;
		}

		private void DrawCursorByPos(Graphics g)
		{
			DrawCursorByPos(g,float.NaN);
		}

		/// <summary>
		/// Draw Cross Cursor based on a date time
		/// </summary>
		/// <param name="t"></param>
		public void DrawCursorByTime(DateTime t)
		{
			int i = chart.DateToIndex(t.ToOADate());
			if (i!=chart.CursorPos) 
			{
				chart.CursorPos = i;
				DrawCursorByPos(ControlGraphics,-1);
				ChangeCursorPos();
			}
		}

		/// <summary>
		/// Get current time at cursor
		/// </summary>
		/// <returns></returns>
		public DateTime GetCurrentTime()
		{
			return chart.IndexToDate();
		}

		private void ChartWinControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (chart!=null)
			{
				int OldStart = chart.Start;
				int OldPos = chart.CursorPos;
				int Delta = e.Control?Page:1;

				switch (e.KeyCode) 
				{
					case Keys.Subtract:
					case Keys.Down:
						if (e.Control) 
						{
							NextArea(1);
							e.Handled = true;
						} 
						else 
							ScaleChart(-0.05);
						break;
					case Keys.Add:
					case Keys.Up:
						if (e.Control) 
						{
							NextArea(-1);
							e.Handled = true;
						} 
						else 
							ScaleChart(0.05);
						break;
					case Keys.Left:
						chart.AdjustCursorByPos(
							ControlGraphics,chart.CursorPos-Delta,chart.Start+(e.Shift?Delta:0));
						e.Handled = true;
						break;
					case Keys.Right:
						chart.AdjustCursorByPos(ControlGraphics,chart.CursorPos+Delta,chart.Start-(e.Shift?Delta:0));
						e.Handled = true;
						break;
					case Keys.Home:
						chart.AdjustCursorByPos(ControlGraphics,0,chart.Start);
						e.Handled = true;
						break;
					case Keys.End:
						chart.AdjustCursorByPos(ControlGraphics,int.MaxValue,chart.Start);
						e.Handled = true;
						break;
					case Keys.Multiply:
						NextFormula(1);
						e.Handled = true;
						break;
					case Keys.Divide:
						NextFormula(-1);
						e.Handled = true;
						break;
					case Keys.F7:
					case Keys.F8:
						NextCycle(1-(e.KeyCode==Keys.F8?0:2));
						//OldStart = 0;
						e.Handled = true;
						break;
					case Keys.Escape:
						
						CancelMouseZoom();
						Invalidate();
						return;
					default :
						return;
				}

				if (e.Handled)
					ChangeCursorPos();
				if (OldPos!=chart.CursorPos && OldStart!=chart.Start)
					NeedRedraw();

				if (needRedraw)
				{
					if (OldStart!=chart.Start)
						MoveChartXBars(3,1,OldStart-chart.Start);
					needDrawCursor = true;
				} 
				else 
					DrawCursorByPos(ControlGraphics);
			}
		}

		/// <summary>
		/// Reset the chart to show latest bars
		/// </summary>
		/// <param name="columnWidth"></param>
		public void Reset(int columnWidth)
		{
			EndTime = DateTime.MinValue;
			this.StockBars = this.Width/columnWidth;
			//ColumnWidth = columnWidth;
			AutoScaleAxisY();
		}

		public void ShowAllBars()
		{
			if (chart!=null && chart.DataProvider!=null)
			{
				double[] Date = chart.DataProvider["DATE"];
				if (Date.Length>0)
				{
					StartTime = DateTime.FromOADate(Date[0]);
					EndTime = DateTime.FromOADate(Date[Date.Length-1]);
				}
			}
		}

		/// <summary>
		/// Show more or less stock bars according ScaleFactor
		/// </summary>
		/// <param name="ScaleFactor"></param>
		public void ScaleChart(double ScaleFactor)
		{
			if (chart!=null && !chart.FixedTime)
			{
				int TotalBar = chart.GetTotalBars();
				int i = (int)(TotalBar * ScaleFactor + 0.5);
				if (i==0)
					if (ScaleFactor>0) i=1; else i=-1;
				
				int j = -i;
				if (zoomPosition==ZoomCenterPosition.Left)
					i = 0;

				if (zoomPosition==ZoomCenterPosition.Right)
					j = 0;
				MoveChartXBars(3,0,i);
				MoveChartXBars(3,2,j);
			}
		}

		/// <summary>
		/// Resize the chart when the form resized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChartWinControl_SizeChanged(object sender, System.EventArgs e)
		{
			ControlGraphics = this.CreateGraphics();
			if (chart!=null) 
			{
				Rectangle R = this.ClientRectangle;
				/*
				R.X +=30;
				R.Width -=60;
				R.Y +=20;
				R.Height -=40;
				*/
				chart.Rect = R;
			}
			else ControlGraphics.Clear(BackColor);
			NeedRedraw();
		}

		private void CancelMouseZoom()
		{
			if (MouseZoomBmp!=null) 
			{
				DragInfo = null;
				MouseZoomBmp=null;
				MouseZoomRect = Rectangle.Empty;
			}
		}

		/// <summary>
		/// Handle the mouse down event, select area of current mouse pos
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChartWinControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			MouseDownInfo = chart.GetHitInfo(e.X,e.Y);
			if (MouseDownInfo.Area !=null)
			{
				//if (MouseDownInfo.Area!= chart[0])
					chart.SelectedArea = MouseDownInfo.Area;

				if (selectFormulaMouseMode==MouseAction.MouseDown && !Designing)
					chart.SetSelectedData(MouseDownInfo);

				if (e.Button!=MouseButtons.Right)
				{
					if (CrossCursorMouseMode==MouseAction.MouseDown && !Designing) 
						MouseCrossCursor(ControlGraphics,MouseDownInfo);
				}
				NeedRedraw();
			}
			if (e.Button==MouseButtons.Right && !Designing)
			{
				if (MouseZoomBmp!=null)
				{
					CancelMouseZoom();
				}
				else if (this.Visible) 
				{
					ContextMenu cm = cmRight;
					if (!nativeContextMenu)
						cm = ContextMenu;
					if (cm!=null)
						cm.Show(this,new Point(e.X,e.Y));
				}
			}
			else 
			{
				if (!Designing)
				{
					//if (ChartDragMode!=ChartDragMode.Chart && MouseDownInfo.HitType!=FormulaHitType.htArea)
					DragInfo = new ChartDragInfo(chart,MouseDownInfo);
					if ((MouseDownInfo.HitType==FormulaHitType.htArea) && (mouseZoomMode!=MouseZoomMode.None) && !FixedTime) 
					{
						LastX = e.X;
						LastY = e.Y;
						MemBmp = GetBitmap(false,true);
						MouseZoomBmp  = new Bitmap(MemBmp.Width,MemBmp.Height,PixelFormat.Format32bppPArgb);
						MouseZoomBmpG = Graphics.FromImage(MouseZoomBmp);
						HideCursor();
					}

//					if (!((ChartDragMode==ChartDragMode.Chart) ||
//						ChartDragMode==ChartDragMode.Axis && MouseDownInfo.HitType!=FormulaHitType.htArea))
//						MouseZoomBmp = GetBitmap(false,true);
				}
			}
		}

		/// <summary>
		/// True is the chart is under a mouse drag
		/// </summary>
		[Browsable(false)]
		public bool IsDragging
		{
			get
			{
				return DragInfo!=null;
			}
		}

		//TODO:CursorPos should be a property,fire the ChangeCursorPos in the setter
		private void ChangeCursorPos()
		{
			if (chart!=null && chart.DataProvider!=null)
			{
				if (LastCursorPos!=chart.CursorPos || LastProvider!=chart.DataProvider)
				{
					IDataProvider idp = chart.DataProvider;
					if (chart.CursorPos>=0 && chart.CursorPos<idp.Count)
					{
						LastProvider = chart.DataProvider;
						LastCursorPos = chart.CursorPos;

						if (CursorPosChanged!=null)
							CursorPosChanged(this,chart,LastCursorPos,idp);
					}
				}
			}
		}

		/// <summary>
		/// Show statistic at current pos
		/// </summary>
		private void ShowStatisticWindow(object Sender, FormulaChart Chart,int Pos,IDataProvider idp)
		{
			if (ShowStatistic)
			{
				if (StatisticWindow.Left<0) 
					StatisticWindow.Left = 10;

				string Format = "f2";
				if (Chart.MainArea!=null && Chart.MainArea.IsMain())
					Format = Chart.MainArea.AxisY.Format;

				double D = idp["DATE"][Chart.CursorPos];
				double O = idp["OPEN"][Chart.CursorPos];
				double H = idp["HIGH"][Chart.CursorPos];
				double L = idp["LOW"][Chart.CursorPos];
				double C = idp["CLOSE"][Chart.CursorPos];
				double V = idp["VOLUME"][Chart.CursorPos];
							
				string Last = "";
				string Change = "";
				string ChangeP = "";
				if (Chart.CursorPos>0) 
				{
					double LL = idp["CLOSE"][Chart.CursorPos-1];
					if (LL!=double.NaN) 
					{
						Last = FormulaHelper.FormatDouble(LL,Format);
						Change = (C-LL).ToString("f2");
						ChangeP = ((C-LL)/LL).ToString("p2");
					}
				}
					
				StringBuilder sb = new StringBuilder();
				sb.Append("Symbol="+idp.GetStringData("Code")+";");
				sb.Append("Date="+
					DateTime.FromOADate(D).ToString(Chart.XCursorFormat,DateTimeFormatInfo.InvariantInfo)+";");
				sb.Append("Current="+FormulaHelper.FormatDouble(C,Format)+";");
				sb.Append("Last="+Last+";");
				sb.Append("Open="+FormulaHelper.FormatDouble(O,Format)+";");
				sb.Append("High="+FormulaHelper.FormatDouble(H,Format)+";");
				sb.Append("Low="+FormulaHelper.FormatDouble(L,Format)+";");
				sb.Append("Close="+FormulaHelper.FormatDouble(C,Format)+";");
				sb.Append("Volume="+V.ToString()+";");
				sb.Append("Change="+Change+";");
				sb.Append("Percent="+ChangeP+";");

				if (ShowIndicatorValues)
					try
					{
						for(int i=showOverlayValues?0:1; i<Chart.Areas.Count; i++)
						{
							string s;
							int j=0;
							int k=0;
							foreach(FormulaData fd in Chart[i].FormulaDataArray)
							{
								if (k==0)
									s = Chart[i].Formulas[j].DisplayName;
								else s = "";
								if (fd.ValueTextMode!=ValueTextMode.None)// !fd.TextInvisible)
								{
									string r = (s+fd.Name).Trim();
									if (r!="")
										sb.Append(r+"="+FormulaHelper.FormatDouble(fd[Chart.CursorPos], Chart[i].AxisYs[fd.AxisYIndex].Format)+";");
								}
								k++;
								if (k==Chart[i].Packages[j].Count) 
								{
									j++;
									k = 0;
								}
							}
						}
					}
					catch
					{
					}
				
				StatisticWindow.RefreshData(sb.ToString());

				LastProvider = Chart.DataProvider;
				LastCursorPos = Chart.CursorPos;
			}
		}

		private void MoveChartXBars(int TotalPart,int CurrentPart,DateTime StartTime,DateTime EndTime,int MoveBars)
		{
			if (chart!=null && chart.DataProvider!=null)
			{
				double[] dd = chart.DataProvider["DATE"];
				DateTime d1 = this.StartTime;
				DateTime d2 = this.EndTime;
				bool NeedRestore = false;
				if (CurrentPart>0)
				{
					int i1 = chart.DateToIndex(EndTime,-1)+MoveBars;
					if (i1<0) 
					{
						i1=0;
						NeedRestore = true;
					}
					if (chart.CursorPos>i1) 
					{
						chart.CursorPos = i1;
						ChangeCursorPos();
					}

					//if (CurrentPart!=1 && i1<chart.CursorPos) i1 = chart.CursorPos;
					this.EndTime = chart.IndexToDate(i1);
				}
				if (CurrentPart<TotalPart-1)
				{
					int i2 = chart.DateToIndex(StartTime)+MoveBars;
					if (i2>=dd.Length-1) 
					{
						i2 = dd.Length-2;
						NeedRestore = true;
					}
					if (i2<0) 
					{
						i2=0;
						//NeedRestore = true;
					}
					//if (CurrentPart!=1 && i2>chart.CursorPos) i2 = chart.CursorPos;
					if (chart.CursorPos<i2) 
					{
						chart.CursorPos = i2;
						ChangeCursorPos();
					}
					
					this.StartTime = chart.IndexToDate(i2);
				}
				if (NeedRestore && CurrentPart==1)
				{
					this.StartTime = d1;
					this.EndTime = d2;
				}

				if (resetYAfterXChanged)
				{
					FormulaArea fa = chart.MainArea;
					if (fa!=null)
					{
						minPrice = 0;
						fa.AxisY.AutoScale = true;
					}
				}

				if (CurrentDataManager!=null && CurrentDataManager.VirtualFetch)
					NeedRebind();
			}
		}

		/// <summary>
		/// Move chart by bars
		/// </summary>
		/// <param name="MoveBars">How many bars to move</param>
		public void MoveChartXBars(int MoveBars)
		{
			MoveChartXBars(3,1,MoveBars);
		}

		private void MoveChartXBars(int TotalPart,int CurrentPart,int MoveBars)
		{
			MoveChartXBars(TotalPart,CurrentPart,chart.StartTime,chart.EndTime,MoveBars);
		}

		private void MoveChartX(FormulaArea fa,int TotalPart,int CurrentPart,float DeltaX)
		{
			if (DeltaX!=0)
			{
				if (EndTime==DateTime.MinValue) 
				{
					EndTime = chart.EndTime;
					StartTime = chart.StartTime;
				}
				int i1=chart.DateToIndex(chart.EndTime,-1);
				int i2=chart.DateToIndex(chart.StartTime);
				double TotalBars = i1-i2+1;
				int MoveBars = (int)(TotalBars*DeltaX/fa.Rect.Width);
				MoveChartXBars(TotalPart,CurrentPart,DragInfo.StartTime,DragInfo.EndTime,MoveBars);
			}
		}

		private void MoveChartY(FormulaArea fa,int Part,int CurrentPart,float DeltaY)
		{
			if (DeltaY!=0)
			{
				FormulaAxisY fay = DragInfo.HitInfo.AxisY;
				if (fay==null)
					fay = fa.AxisY;
				fay.AutoScale = false;
				double MoveY = (DragInfo.AreaMaxY - DragInfo.AreaMinY)/fa.Rect.Height*DeltaY;
				if (CurrentPart>0)
					fay.MinY = DragInfo.AreaMinY- MoveY;
				if (CurrentPart<Part-1)
					fay.MaxY = DragInfo.AreaMaxY- MoveY;

				//fay.ChangedByUser = true;
				minPrice = 0;
				maxPrice = 0;
				NeedRedraw(fa);
			}
		}

		private void InvalidateHitInfo(FormulaHitInfo fhi)
		{
			Region R = new Region(new RectangleF(fhi.X-1,0,2,chart.Rect.Height));
			R.Union(new RectangleF(0,fhi.Y-1,chart.Rect.Width,2));
			foreach(FormulaArea fa in chart.Areas)
			{
				RectangleF R1 = fa.AxisX.LastCursorRect;
				R.Union(new RectangleF(fhi.X-R1.Width,R1.Y,R1.Width*2,R1.Height));

				R.Union(new RectangleF(fa.Rect.X,fa.Rect.Y,fa.Rect.Width,20));
				foreach(FormulaAxisY fay in fa.AxisYs) 
				{
					R1 = fay.LastCursorRect;
					if (!R1.IsEmpty)
						R.Union(new RectangleF(R1.X,fhi.Y,R1.Width,R1.Height));
				}
			}
			//R.Union(new RectangleF(fhi.X-32,fhi.Y-32,64,64));
			//return R;
			Invalidate(R);
		}

		private void HideCursor()
		{
			chart.HideCursor(ControlGraphics);
		}

		FormulaHitInfo LastHitInfo;
		private void MouseCrossCursor(Graphics g,FormulaHitInfo HitInfo)
		{
			if (Designing) return;

			if (HitInfo.HitType==FormulaHitType.htArea)
			{
				chart.CursorPos = HitInfo.CursorPos;
				if (memoryCrossCursor)
				{
					//rgnInvalidate = new Region();
					//rgnInvalidate.MakeEmpty();
					InvalidateHitInfo(LastHitInfo);
					InvalidateHitInfo(HitInfo);
					//Invalidate(rgnInvalidate);
				}
				else
					chart.DrawCursor(g,HitInfo.X,HitInfo.Y,false);

				ChangeCursorPos();
				LastHitInfo = HitInfo;
			} 
			else 
			{
				HideCursor();
				LastHitInfo.X = 0;
			}
		}

		private RectangleF GetRect(float x1,float y1,float x2,float y2)
		{
			return new RectangleF(Math.Min(x1,x2),Math.Min(y1,y2),Math.Abs(x2-x1),Math.Abs(y2-y1));
		}

		/// <summary>
		/// Move cursor , show statistic
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChartWinControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if (Designing) return;
				int Part = 3 - (ChartDragMode==ChartDragMode.Chart?1:0);
				if (LastX!=e.X || LastY!=e.Y)
				{
					FormulaHitInfo HitInfo = chart.GetHitInfo(e.X,e.Y);
					if (DragInfo!=null)
					{
						float DeltaY = DragInfo.HitInfo.Y - HitInfo.Y;
						float DeltaX = DragInfo.HitInfo.X - HitInfo.X;

						FormulaArea fa1 = DragInfo.HitInfo.Area;
						if (MouseZoomBmp!=null)
						{
							RectangleF R1;
							RectangleF R2;
							if (FixedZoomRect!=Rectangle.Empty) 
							{
								R1 = FixedZoomRect;
								R1.Offset(e.X,e.Y);
								R2 = FixedZoomRect;
								R2.Offset(LastX,LastY);
							}
							else 
							{
								R1 = GetRect(DragInfo.HitInfo.X,DragInfo.HitInfo.Y,e.X,e.Y);
								R2 = GetRect(DragInfo.HitInfo.X,DragInfo.HitInfo.Y,LastX,LastY);
							}

							MouseZoomRect = RectangleF.Union(R1,R2);
							MouseZoomBmpG.DrawImage(MemBmp,MouseZoomRect,MouseZoomRect,GraphicsUnit.Pixel);

							Rectangle RR = Rectangle.Truncate(R1); //
							RR.Width--;
							RR.Height--;
							MouseZoomBmpG.DrawRectangle(Pens.Black,RR);
							if (mouseZoomBackColor.A!=0)
								MouseZoomBmpG.FillRectangle(new SolidBrush(mouseZoomBackColor),RR);

							ControlGraphics.DrawImage(MouseZoomBmp,MouseZoomRect,MouseZoomRect,GraphicsUnit.Pixel);
						}
						else if (EnableResize && DragInfo.HitInfo.HitType == FormulaHitType.htSize)
						{
							#region Handler resize
							for(int i=0; i<chart.Areas.Count; i++)
								chart.Areas[i].HeightPercent = DragInfo.AreaHeight[i];

							int j = chart.Areas.IndexOf(fa1);
							if (j<chart.Areas.Count-1)
							{
								FormulaArea fa2 = chart.Areas[j+1];
								double H1 = DragInfo.AreaHeight[j]-DeltaY;
								double H2 = DragInfo.AreaHeight[j+1]+DeltaY;
								if (H1<40) 
								{
									H2 -= 40-H1;
									H1 = 40;
								}
								if (H2<40) 
								{
									H1 -=40-H2;
									H2 = 40;
								}
								fa1.HeightPercent = H1;
								fa2.HeightPercent = H2;
								NeedRedraw(fa1);
								NeedRedraw(fa2);
							}
							areaPercent = chart.GetAreaPercent();
							#endregion
						} 
						else  if (ChartDragMode!=ChartDragMode.None)
						{
							if (EnableXScale && !chart.FixedTime && DragInfo.HitInfo.HitType == FormulaHitType.htAxisX)
							{
								MoveChartX(fa1,Part,DragInfo.HitInfo.XPart(Part),DeltaX);
							}
							else if (EnableYScale && DragInfo.HitInfo.HitType == FormulaHitType.htAxisY)
							{
								MoveChartY(fa1,Part,DragInfo.HitInfo.YPart(Part),DeltaY);
							} 
							else if (ChartDragMode==ChartDragMode.Chart && DragInfo.HitInfo.HitType == FormulaHitType.htArea)
							{
								if (!chart.FixedTime)
									MoveChartX(fa1,3,1,DeltaX);
								MoveChartY(fa1,3,1,DeltaY);
							}
						} 
					}
					else 
					{
						if (CrossCursorMouseMode==MouseAction.MouseMove)
							MouseCrossCursor(ControlGraphics,HitInfo);
						if (selectFormulaMouseMode==MouseAction.MouseMove) 
						{
							FormulaData sd = chart.GetSelectedData();
							if (!object.Equals(sd,HitInfo.Data)) 
							{
								chart.SetSelectedData(HitInfo);
								NeedRedraw();
							}
						}
						#region set cursor shape
						if (HitInfo.Area!=null)
						{
							switch (HitInfo.HitType)
							{
								case FormulaHitType.htAxisX:
									if (EnableXScale && !chart.FixedTime && ChartDragMode!=ChartDragMode.None)
									{
										int XP = HitInfo.XPart(Part);
										if (XP==0 || XP==Part-1)
											Cursor = Cursors.SizeWE;
										else 
											Cursor = Cursors.Hand;
									} 
									else Cursor = DefaultCursor;
									break;
								case FormulaHitType.htAxisY:
									if (EnableYScale && ChartDragMode!=ChartDragMode.None)
									{
										int YP = HitInfo.YPart(Part);
										if (YP==0 || YP==Part-1)
											Cursor = Cursors.SizeNS;
										else 
											Cursor = Cursors.Hand;
									} 
									else Cursor = DefaultCursor;
									break;
								case FormulaHitType.htSize:
									if (EnableResize)
										Cursor = Cursors.HSplit;
									break;
								case FormulaHitType.htArea:
									if (selectFormulaMouseMode!=MouseAction.None)
										if (HitInfo.Formula!=null)
											Cursor = Cursors.Hand;
										else Cursor = DefaultCursor;
									else Cursor = DefaultCursor;
									break;
								default:
									Cursor = DefaultCursor;
									break;
							}
						}
						#endregion
					}
					LastX = e.X;
					LastY = e.Y;
					if (AfterMouseMove!=null)
						AfterMouseMove(this,e,HitInfo);
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Handle mouse leave event, hide the cursor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChartWinControl_MouseLeave(object sender, System.EventArgs e)
		{
			if (chart!=null && CrossCursorMouseMode==MouseAction.MouseMove)
				HideCursor();
		}

		private void SetCursorPos()
		{
			chart.CursorPos = LastHitInfo.CursorPos;
		}

		/// <summary>
		/// Handle mouse whell event, move the chart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChartWinControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (mouseWheelMode!=MouseWheelMode.None && chart!=null && !FixedTime) 
			{
				int Page = Math.Sign(e.Delta)*(((Form.ModifierKeys & Keys.Alt)!=0)?1:12);

				if (mouseWheelMode==MouseWheelMode.Scroll)
					MoveChartXBars(3,1,Page);
				else ScaleChart(Page/100.0);

				needResetCursorPos = true;
				needDrawCursor = false;
				LastHitInfo =  chart.GetHitInfo(e.X,e.Y);
			}
		}

		public void RenderChart(Graphics g)
		{
			Graphics OldG = g;
			if (memoryCrossCursor)
			{
				Rectangle R = chart.Rect;
				if (CrossMemBmp==null || CrossMemBmp.Width!=R.Width || CrossMemBmp.Height!=R.Height)
				{
					CrossMemBmp = new Bitmap(R.Width,R.Height,PixelFormat.Format32bppPArgb);
					CrossMemBmpG = Graphics.FromImage(CrossMemBmp);
				}

				Region rgn = null;
				if (needDrawLastBarOnly && needRebind)
				{
					rgn = new Region();
					rgn.MakeEmpty();
				}

				g = CrossMemBmpG;
				g.SetClip(OldG);
				chart.Render(g,rgn);
				OldG.SetClip(g);

				if (LastHitInfo.X!=0) 
				{
					if (needRebind)
						SetCursorPos();
					if (needDrawCursor)
					{
						DrawCursorByPos(g);
						needDrawCursor =false;
					}
					else DrawCursor(g);
				}
			} 
			else 
			{
				chart.Render(g);
				if (needDrawCursor) 
				{
					chart.DrawCursor(g);
					needDrawCursor =false;
				} 
				else if (crossCursorMouseMode ==MouseAction.MouseDown)
				{
					if (LastHitInfo.X!=0) 
					{
						LastHitInfo = chart.GetHitInfo(LastHitInfo.X,LastHitInfo.Y);	
						MouseCrossCursor(ControlGraphics,LastHitInfo);
					}
				}
			}
			if (MouseZoomBmp!=null)
				g.DrawImage(MouseZoomBmp,MouseZoomRect,MouseZoomRect,GraphicsUnit.Pixel);

			if (memoryCrossCursor)
				OldG.DrawImage(CrossMemBmp,0,0);
		}

		private void RebindLastBar()
		{
			needDrawLastBarOnly = true;
			BindData();
			RenderChart(ControlGraphics);
		}

		public bool BindData()
		{
			bool BindOK = true;
			if (HistoryDataManager!=null && IntraDataManager!=null)
			{
				CommonDataProvider cdp = null;
				//DataManagerBase OldDataManager = CurrentDataManager;

				if (currentDataCycle.CycleBase>=DataCycleBase.DAY)
					CurrentDataManager = HistoryDataManager;
				else 
				{
					CurrentDataManager = IntraDataManager;
					CurrentDataManager.IsFix = this.fixedTime;
					//CurrentDataManager.StartTime = this.startTime;
					//CurrentDataManager.EndTime = this.endTime;
					chart.FixedTime = this.fixedTime;
				}

				if (CurrentDataManager.VirtualFetch)
				{
					CurrentDataManager.StartTime = this.startTime;
					CurrentDataManager.EndTime = this.endTime;
				}

				BindOK = false;
				try
				{
					cdp = (CommonDataProvider)CurrentDataManager[symbol];
					if (AfterBindData!=null)
						AfterBindData(this,new BindDataEventArgs(cdp,CurrentDataManager));
					BindOK = cdp!=null;
				}
				catch
				{
					//	MessageBox.Show(ex.Message);
				}

				if (cdp!=null) 
					chart.SetDataProviderNoBind(cdp);
				else endTime = DateTime.MinValue;

				if (fixedTime && cdp!=null)
					cdp.IntradayInfo = IntradayInfo;

				if (chart.DataProvider!=null)
					if (chart.DataProvider is CommonDataProvider)
					{
						if (HasData())
						{
							if (chart.DataProvider.DataCycle.ToString()!=currentDataCycle.ToString())
							{
								ColumnWidth = 5;
								StartBar = 0;
								chart.DataProvider.DataCycle = currentDataCycle;
							}
							if (DataChanged!=null)
								DataChanged(this,new EventArgs());
						}
						(chart.DataProvider as CommonDataProvider).Adjusted = adjustData;
					}
				chart.SetAreaPercent(areaPercent);	
				//if (needAutoScaleY)  
				//	AutoAdjustYAxis();
				//else needAutoScaleY = true;
				chart.Bind();
				ApplySkin();
			}
			return BindOK;
		}

		/// <summary>
		/// Draw the formula chart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChartWinControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (chart!=null)
			{
				StatisticWindow.Visible = showStatistic && StatisticWindow.HasData();
				if (needSetDefaultFormulas)
				{
					DefaultFormulasToAreas();
					needSetDefaultFormulas = false;
				}

				if (needRefresh) 
				{
					RecreateFormula();
				}

				bool BindOK = true;
				if (needRebind)
					BindOK  = BindData();

				if (needRedraw && BindOK)
				{
					if (chart.FixedTime && CurrentDataManager!=null)
					{
						chart.StartTime = startTime;
						chart.EndTime = endTime;
					} 
					else 
					{
						if (chart.DataProvider!=null)
						{
							double[] dd = chart.DataProvider["DATE"];
							if (endTime==DateTime.MinValue)
							{
								if (dd.Length>0)
								{
									chart.EndTime = DateTime.FromOADate(dd[dd.Length-1]);
									chart.StartTime = DateTime.FromOADate(dd[Math.Max(0,dd.Length-stockBars)]);
									EndTime = chart.EndTime;
									StartTime = chart.StartTime;
								}
							}
							else
							{
								int i1 = chart.DateToIndex(endTime);
								int i2; 
								if (afterBars<1)
									i2 = (int)((dd.Length-1)*(1+afterBars));
								else i2 = (int)(dd.Length-1+afterBars);

								if (i1>i2) 
									endTime = chart.IndexToDate(i2);

								chart.StartTime = startTime;
								chart.EndTime = endTime;
							}
						}
					}
					FormulaArea fa = chart.MainArea;
					if (fa!=null)
					{
						if (minPrice>0)
						{
							fa.AxisY.MinY = minPrice;
							fa.AxisY.MaxY = maxPrice;
							fa.AxisY.AutoScale = false;
						}
						if (stockRenderType!=StockRenderType.Default)
							fa.StockRenderType  = stockRenderType;
						if (HasData() && scaleType!=ScaleType.Default) 
							fa.AxisY.Scale = scaleType;
					}

					chart.ValueTextMode = valueTextMode;

					if (stickRenderType!=StickRenderType.Default)
						chart.StickRenderType = stickRenderType;

					chart.LatestValueType = latestValueType;
					chart.ShowCursorLabel= showCursorLabel;
					//chart.HideCursor();

					if (showVerticalGrid!=ShowLineMode.Default)
					{
						if (showVerticalGrid==ShowLineMode.HidePrice)
							fa.AxisY.MajorTick.ShowLine = false;
						else 
							foreach(FormulaArea faa in chart.Areas)
								faa.AxisY.MajorTick.ShowLine = showVerticalGrid==ShowLineMode.Show;
					}

					if (showHorizontalGrid!=ShowLineMode.Default)
					{
						if (showHorizontalGrid==ShowLineMode.HidePrice)
							fa.AxisX.MajorTick.ShowLine = false;
						else 
							foreach(FormulaArea faa in chart.Areas)
								faa.AxisX.MajorTick.ShowLine = showHorizontalGrid==ShowLineMode.Show;
					}

					chart.NeedRedraw = true;
					chart.ExtraNeedRedraw = true;
				}
				
				if (needResetCursorPos)
				{
					SetCursorPos();
					ChangeCursorPos();
					needResetCursorPos = false;
				}

				RenderChart(e.Graphics);

				needRedraw = false;
				needRebind = false;
				needRefresh = false;
			}
		}

		private void SetupPrinting()
		{
			if (printDialog==null)
			{
				this.printDialog = new System.Windows.Forms.PrintDialog();
				this.printDocument = new System.Drawing.Printing.PrintDocument();
				this.previewDialog = new System.Windows.Forms.PrintPreviewDialog();
				this.setupDialog = new System.Windows.Forms.PageSetupDialog();

				printDocument.DefaultPageSettings.Landscape = true;
				printDocument.DefaultPageSettings.Margins = new Margins(30,30,30,30);
				this.printDialog.Document = this.printDocument;

				this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
				this.previewDialog.Document = this.printDocument;
				this.setupDialog.Document = this.printDocument;
			}
		}

		/// <summary>
		/// Print the chart
		/// </summary>
		public void Print()
		{
			SetupPrinting();
			if (printDialog.ShowDialog()==DialogResult.OK)
			{
				printDocument.Print();
			}
		}

		/// <summary>
		/// Print preview of the chart
		/// </summary>
		public void PrintPreview()
		{
			SetupPrinting();
			previewDialog.ShowDialog();
		}

		/// <summary>
		/// Printer setup
		/// </summary>
		public void PrintSetup()
		{
			SetupPrinting();
			setupDialog.ShowDialog();
		}

		/// <summary>
		/// Get bitmap of the chart control
		/// </summary>
		/// <param name="ShowStatistic">show statistic window on the bitmap</param>
		/// <param name="ShowExtra">show extra informations to the chart, like "Stock Object"</param>
		/// <returns></returns>
		public Bitmap GetBitmap(bool ShowStatistic,bool ShowExtra)
		{
			chart.ExtraNeedRedraw = ShowExtra;
			Bitmap bmp = chart.GetMemBitmap();
			Bitmap B = new Bitmap(bmp);
			Graphics g = Graphics.FromImage(B);
			g.DrawImage(bmp,0,0);
			g.DrawLine(Pens.Black,0,0,B.Width,0);
			if (ShowStatistic)
				StatisticWindow.PaintTo(g);
			return B;
		}

		private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			Bitmap B = GetBitmap(true,true);
			e.Graphics.DrawImage(B,e.MarginBounds,new Rectangle(0,0,B.Width,B.Height),GraphicsUnit.Pixel);
		}

		private bool HasData()
		{
			return chart!=null && chart.DataProvider!=null;
		}

		/// <summary>
		/// Apply skin
		/// </summary>
		/// <param name="fs">Formula Skin</param>
		public void ApplySkin(FormulaSkin fs)
		{
			if (fs==null) return;
			if (BeforeApplySkin!=null)
				BeforeApplySkin(this,fs);

			chart.SetSkin(fs);
			chart[0].Back.TopPen.Width = ShowTopLine?2:0;
			chart[0].AxisY.Back.TopPen.Width = ShowTopLine?2:0;
			
			if (AfterApplySkin!=null)
				AfterApplySkin(this,new EventArgs());
			foreach(FormulaArea fa in chart.Areas)
				fa.RemoveAutoMultiplyForStockYAxis();
			//NeedRebind(needAutoScaleY,needDrawLastBarOnly);
		}

		private void ApplySkin()
		{
			FormulaSkin fs = FormulaSkin.GetSkinByName(skin);
			ApplySkin(fs);
		}

		private string skin = "RedWhite";
		/// <summary>
		/// Get or set the formula skin
		/// </summary>
		[TypeConverter(typeof(SkinConverter)),Category("Stock Chart")]
		[DefaultValue("RedWhite")]
		public string Skin 
		{
			get 
			{
				return skin;
			}
			set 
			{
				skin = value;
				NeedRebind();
			}
		}

		/// <summary>
		/// Get the default caption of the chart control, like MSFT-WEEK1
		/// </summary>
		[Browsable(false)]
		public string Caption
		{
			get
			{
				IDataProvider idp = chart.DataProvider;
				string s = idp.GetStringData("Code");
				string Name = idp.GetStringData("Name");
				if (Name!=null)
					s += "("+Name+")";

				string Exchange = idp.GetStringData("Exchange");
				if (Exchange!=null)
					s +="@"+Exchange;
				return s+" - "+CurrentDataCycle;
			}
		}

		private void ChartWinControl_Enter(object sender, System.EventArgs e)
		{
			Invalidate();
		}

		/// <summary>
		/// Set axis-y scale automatically
		/// </summary>
		/// <param name="fay"></param>
		public void AutoAdjustYAxis(FormulaAxisY fay)
		{
			MinPrice = 0;
			fay.AutoScale = true;
			NeedRedraw();
		}

		/// <summary>
		/// Set all axis-y in the chart scale automatically
		/// </summary>
		public void AutoAdjustYAxis()
		{
			foreach(FormulaArea fa in chart.Areas)
				foreach(FormulaAxisY fay in fa.AxisYs)
					AutoAdjustYAxis(fay);
		}

		/// <summary>
		/// double click to popup the formula select form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChartWinControl_DoubleClick(object sender, System.EventArgs e)
		{
			if (MouseDownInfo.HitType==FormulaHitType.htAxisY)
				AutoAdjustYAxis(MouseDownInfo.AxisY);
		}

		/// <summary>
		/// Let the control scale Y-axis automatically
		/// </summary>
		public void AutoScaleAxisY()
		{
			foreach(FormulaArea fa in chart.Areas)
				foreach(FormulaAxisY fay in fa.AxisYs)
					fay.AutoScale = true;
			NeedRebind();
			//NeedRedraw();
		}

		/// <summary>
		/// Copy the data of selected area to clip board
		/// </summary>
		/// <param name="Separatar"></param>
		public void CopyAreaToClipboard(string Separatar) 
		{
			Clipboard.SetDataObject(chart.GetAreaTextData(Separatar,true));
		}

		/// <summary>
		/// Copy the data in chart control to clip board
		/// </summary>
		/// <param name="Separatar"></param>
		public void CopyToClipboard(string Separatar) 
		{
			Clipboard.SetDataObject(chart.GetChartTextData(Separatar,true));
		}

		/// <summary>
		/// Open the formula source editor
		/// </summary>
		/// <param name="Filename">Formula file name</param>
		/// <param name="Formula">Default formula</param>
		public static void OpenFormulaSourceEditor(string Filename,string Formula)
		{
			FormulaSourceEditor.Open(Filename,Formula);
		}

		/// <summary>
		/// Open the formula source editor
		/// </summary>
		public static void OpenFormulaSourceEditor()
		{
			OpenFormulaSourceEditor("","");
		}

		private void BuildFavoriteMenu() 
		{
			cmRight.MenuItems.Remove(miFavorite);
			miFavorite = new MenuItem("&Favorite Indicators");
			cmRight.MenuItems.Add(0,miFavorite);
			
			foreach(string s in favoriteFormulas)
			{
				miFavorite.MenuItems.Add(s,ehFavoriteIndicator);
			}
		}

		private void mmFavorite_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FormulaArea fa = chart.SelectedArea;
			if (fa!=null && !fa.IsMain())
				SetAreaByName(fa,(sender as MenuItem).Text);
		}

		private void miEdit_Click(object sender, System.EventArgs e)
		{
			EditFormula();
		}

		private void miCopy_Click(object sender, System.EventArgs e)
		{
			CopyAreaToClipboard(",");
		}

		private void miChartCopy_Click(object sender, System.EventArgs e)
		{
			CopyToClipboard(",");
		}

		/// <summary>
		/// Switch checkbox of show cursor at menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miCrossCursor_Click(object sender, System.EventArgs e)
		{
			ShowCrossCursor = !ShowCrossCursor;
		}

		/// <summary>
		/// Show/Hide statistic form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miStatistic_Click(object sender, System.EventArgs e)
		{
			ShowStatistic = !ShowStatistic;
		}

		/// <summary>
		/// Run calculator of system
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miCalculator_Click(object sender, System.EventArgs e)
		{
			Process.Start("Calc.exe");
		}

		/// <summary>
		/// Set the check box of statistic menu item when Menu View popup
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miView_Popup(object sender, System.EventArgs e)
		{
			miStatistic.Checked = ShowStatistic;
		}

		private void Chart_NativePaint(object sender, NativePaintArgs e)
		{
			if (NativePaint!=null)
				NativePaint(this,e);
		}

		private void Chart_ExtraPaint(object sender, NativePaintArgs e)
		{
		
			if (ExtraPaint!=null)
				ExtraPaint(this,e);
		}

		private void ChartWinControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (DragInfo!=null)
			{
				DragInfo = null;
				Invalidate();
			}
			if (MouseZoomBmp!=null)
			{
				if (e.Button==MouseButtons.Left && !MouseZoomRect.IsEmpty && MouseZoomRect.Width>100)
				{
					FormulaHitInfo fhi1 = chart.GetHitInfo(MouseZoomRect.X,MouseZoomRect.Y);
					FormulaHitInfo fhi2 = chart.GetHitInfo(MouseZoomRect.Right,MouseZoomRect.Bottom);
					int i1 = fhi1.CursorPos;
					int i2 = fhi2.CursorPos;
					if (chart.DataProvider!=null)
					{
						double[] dd = chart.DataProvider["DATE"];
						if (i1<dd.Length)
						{
							StartTime = chart.IndexToDate(fhi1.CursorPos);
							EndTime = chart.IndexToDate(fhi2.CursorPos);
						}
					}
				}
				MouseZoomBmp=null;
				MouseZoomRect = Rectangle.Empty;
			}
		}

		/// <summary>
		/// Lauch the overlay formula editor
		/// </summary>
		public void EditOverlay()
		{
//			OverlayManager OverlayForm = new OverlayManager();
//			string overlay = string.Join(";",chart[0].FormulaToStrings(1,chart[0].Formulas.Count-1));
//			if (OverlayForm.ShowForm(overlay)==DialogResult.OK)
//			{
//				OverlayFormulas = OverlayForm.CurrentOverlay;
//				NeedRebind();
//			}
		}

		private void miOverlay_Click(object sender, System.EventArgs e)
		{
			EditOverlay();
		}

		private void Chart_ViewChanged(object sender, ViewChangedArgs e)
		{
			if (ViewChanged!=null)
				ViewChanged(this,e);
		}

		private void miStatisticWindow_Click(object sender, System.EventArgs e)
		{
			ShowStatistic = !ShowStatistic;
		}

		private void cmRight_Popup(object sender, System.EventArgs e)
		{
			miStatisticWindow.Checked = ShowStatistic;
			miAdjust.Checked = AdjustData;

			FormulaArea fa = chart.SelectedArea;
			if (fa!=null) 
			{
				miFavorite.Enabled = !fa.IsMain();
				if (fa.Formulas.Count>0)
				{
					foreach(MenuItem mi in miFavorite.MenuItems)
						mi.Checked =string.Compare(mi.Text,fa.Formulas[0].Name,true)==0;
				}
			}
		}

		private void miAdjust_Click(object sender, System.EventArgs e)
		{
			AdjustData = !AdjustData;
		}

		/// <summary>
		/// Edit the formula source code, this will lauch the formula source editor
		/// </summary>
		/// <param name="fb">Formula need to edit, it will be the default formula in the formula editor</param>
		/// <returns></returns>
		public static bool EditFormula(FormulaBase fb)
		{
			string s = PluginManager.GetFormulaFile(fb);
			if (s!=null) 
			{
				string p = Path.GetDirectoryName(s);
				s = Path.GetFileNameWithoutExtension(s);
				s = s.Replace('_','.');
				s = p+"\\"+s;
				if (File.Exists(s)) 
				{
					OpenFormulaSourceEditor(s,fb.GetType().ToString());
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Edit the formula source code, this will lauch the formula source editor
		/// </summary>
		public void EditFormula()
		{
			FormulaArea fa = chart.SelectedArea;
			if (fa!=null)
			{
				if (fa.Formulas.Count>0)  
				{
					FormulaBase fb = null;
					if (fa.SelectedFormula!=null)
						fb = fa.SelectedFormula;
					else fb = fa.Formulas[0];
					if (EditFormula(fb))
						return;
				}
			}
			MessageBox.Show("Can't edit the source code of this formula");
		}

		/// <summary>
		/// Will lauch a dialog to let end user edit the formula
		/// </summary>
		/// <param name="fa">Formula area</param>
		public void EditFormula(FormulaArea fa)
		{
			if (fa!=null)
			{
				FormulaManager FormulaManagerForm = new FormulaManager();
				if (FormulaManagerForm.ShowForm(fa,fa.SelectedFormula)==DialogResult.OK)
					SetAreaByName(fa,FormulaManagerForm.CurrentFormulas);
			} 
			else EditOverlay();
		}

		private void miFormulaEditor_Click(object sender, System.EventArgs e)
		{
			EditFormula(chart.SelectedArea);
		}

		/// <summary>
		/// Insert new formula area
		/// </summary>
		public void InsertNewFormula()
		{
			InsertNewFormula("");
		}

		public void InsertNewFormula(string Default)
		{
			FormulaArea fa = new FormulaArea(chart);
			FormulaManager FormulaManagerForm = new FormulaManager();
			FormulaManagerForm.CurrentFormulas = Default;
			if (FormulaManagerForm.ShowForm()==DialogResult.OK)
			{
				fa.HeightPercent = chart[0].HeightPercent/3;
				chart.Areas.Add(fa);
				SetAreaByName(fa,FormulaManagerForm.CurrentFormulas);
				NeedRebind();
			}
		}

		private void miAddFormula_Click(object sender, System.EventArgs e)
		{
			InsertNewFormula();
		}

		/// <summary>
		/// Close a formula area
		/// </summary>
		/// <param name="fa"></param>
		public void CloseArea(FormulaArea fa)
		{
			int i = chart.Areas.IndexOf(fa);
			if (i>=0 && chart.Areas.Count>1)
			{
				chart.Areas.RemoveAt(i);
				areaPercent = chart.GetAreaPercent();
				SaveChartProperties();
				NeedRebind();
			}
		}

		private void miCloseFormula_Click(object sender, System.EventArgs e)
		{
			CloseArea(chart.SelectedArea);
		}

		/// <summary>
		/// Move formula area to a new index
		/// </summary>
		/// <param name="fa"></param>
		/// <param name="NewIndex"></param>
		public void MoveArea(FormulaArea fa, int NewIndex)
		{
			int i = chart.Areas.IndexOf(fa);
			if (i>=0)
			{
				if (NewIndex>i)
					NewIndex--;
				if (NewIndex>=0)
				{
					chart.Areas.RemoveAt(i);
					if (NewIndex>chart.Areas.Count)
						NewIndex = chart.Areas.Count;
					chart.Areas.Insert(NewIndex,fa);
					areaPercent = chart.GetAreaPercent();
					SaveChartProperties();
					NeedRebind();
				}
			}
		}

		/// <summary>
		/// Move formula area down
		/// </summary>
		/// <param name="fa"></param>
		public void MoveAreaDown(FormulaArea fa)
		{
			int i = chart.Areas.IndexOf(fa);
			MoveArea(fa,i+2);
		}

		/// <summary>
		/// Move Formula area up
		/// </summary>
		/// <param name="fa"></param>
		public void MoveAreaUp(FormulaArea fa)
		{
			int i = chart.Areas.IndexOf(fa);
			MoveArea(fa,i-1);
		}

		private void ChartWinControl_AfterApplySkin(object sender, EventArgs e)
		{
			chart.ExtendYAxis(TwoYAxisType.AreaSame);
		}

		private void StatisticWindow_OnHide(object sender, EventArgs e)
		{
			ShowStatistic = StatisticWindow.Visible;
		}

		private void ChartWinControl_VisibleChanged(object sender, System.EventArgs e)
		{
			if (Visible)
				ControlGraphics = this.CreateGraphics();
		}
	}

	public class BindDataEventArgs:EventArgs
	{
		public CommonDataProvider cdp;
		public DataManagerBase dmb;
		public BindDataEventArgs(CommonDataProvider cdp,DataManagerBase dmb)
		{
			this.cdp = cdp;
			this.dmb = dmb;
		}
	}
}