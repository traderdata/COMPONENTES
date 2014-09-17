using System;
using System.Drawing;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using System.Web;
using EasyTools;
using Easychart.Finance;

namespace WebDemos
{
	public delegate bool UpdateService (DateTime d,string Exchange);
	/// <summary>
	/// Summary description for Config.
	/// </summary>
	public class Config
	{
		private Config()
		{
		}

		//#if(!LITE)
		//		static public UpdateService AutoUpdateSource
		//		{
		//			get
		//			{
		//				string s = Read("AutoUpdateSource");
		//				if (string.Compare(s,"DataService")==0)
		//					return new UpdateService(DataService.UpdateQuote);
		//				else return new UpdateService(InternetDataToDB.UpdateQuote);
		//			}
		//		}
		//#endif

		static public bool ConvertQueryToCookie = ReadBool("ConvertQueryToCookie",true);
		static public int ParameterDefWidth = ReadInt("ParameterDefWidth",60);
		static public int AutoYahooToDB = ReadInt("AutoYahooToDB",1);
		static public bool EnableAutoService = ReadBool("EnableAutoService",false);
		static public bool EnableAutoUpdate = ReadBool("EnableAutoUpdate",false);
		static public bool EnableStreaming = ReadBool("EnableStreaming",false);
		static public bool EnableYahooStreaming = ReadBool("EnableYahooStreaming",false);
		static public bool EnableChangeToAdmin = ReadBool("EnableChangeToAdmin",false);
		static public string StreamingDataClient = Read("StreamingDataClient");
		
		static public string CSVExt = Read("CSVExt");
		static public string CSVDataPath = HttpRuntime.AppDomainAppPath+Read("CSVDataPath");

		static public string DefaultExchange
		{
			get
			{
				return Read("DefaultExchange",null);
			}
		}
		
		static public string CustomChartDataManager
		{
			get
			{
				return  Read("CustomChartDataManager","DB");
			}
		}

		static public string GalleryDataManager 
		{
			get
			{
				return Read("GalleryDataManager","DB");
			}
		}

		static public string WaterMarkText
		{
			get
			{
				return Read("WaterMarkText","");
			}
		}

		static public Font WaterMarkFont
		{
			get
			{
				try
				{
					TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
					string s = Read("WaterMarkFont");
					if (s!=null && s!="" && tc!=null)
						return (Font)tc.ConvertFromString(s);
				}
				catch
				{
				}
				return new Font("Verdana",40,GraphicsUnit.Pixel);
			}
		}

		static public Color WaterMarkColor
		{
			get
			{
				try
				{
					string s = Read("WaterMarkColor");
					if (s!=null && s!="")
						return ColorTranslator.FromHtml(s);
				} 
				catch
				{
				}
				return Color.FromArgb(64,Color.Gray);
			}
		}

		static public string DefaultDataManager 
		{
			get 
			{
				return Read("DefaultDataManager","DB");
			}
		}

		static public string IntraDataManager 
		{
			get
			{
				return Read("IntraDataManager","Intra");
			}
		}

		static public string WebChartDataManager 
		{
			get
			{
				return Read("WebChartDataManager","Yahoo");
			}
		}

		static public string YAxisFormat = Read("YAxisFormat");
		static public string SymbolCase = Read("SymbolCase");
		static public string ImageFormat = Read("ImageFormat","Gif");
		static public int GifColors = ReadInt("GifColors",255);
		static public Color TransparentColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(Read("TransparentColor"));
		static public string CompanyName = Read("CompanyName","Easy Chart Inc");
		static public string URL = Read("URL","http://finance.easychart.net");
		static public string IntradaySymbols = Read("IntradaySymbols","^DJI;^IXIC;^SPX;^SSEC(China);^FCHI(France);^CDAXX(Germany)"); 
		static public bool SaveInServerDataManager = Read("SaveInServerDataManager")=="1";

		static public string IntradayIndicators = Read("IntradayIndicators","VOLMA;SlowSTO;MACD");
		static public bool IntradayShowCycle = ReadBool("IntradayShowCycle",true);
		static public string IntradayGallerySize = Read("IntradayGallerySize","220*160");
		static public bool IntradayPopup = ReadBool("IntradayPopup",false);
		static public string IntradaySkin = Read("IntradaySkin");
		static public string SymbolParameterName = Read("SymbolParameterName","Symbol");
		static public string IntradayDefaultSize= Read("IntradayDefaultSize","800*600");
		static public int  IntradayCacheTime = ReadInt("IntradayCacheTime",0);
		static public bool ShowXAxisInLastArea = ReadBool("ShowXAxisInLastArea",false);
		static public bool ShowMainAreaLineX= ReadBool("ShowMainAreaLineX",true);
		static public bool ShowMainAreaLineY= ReadBool("ShowMainAreaLineY",true);

		static public string WebProxy = Read("WebProxy");
		static public string LayoutForCustomChart = Read("LayoutForCustomChart","Default;Price");
		static public string LayoutForIntraday = Read("LayoutForIntraday","Intra");
		static public string LayoutForSmall = Read("LayoutForSmall","Small");
		static public bool PrescanCheckVolume = ReadBool("Prescan.CheckVolume",false);
		static public int PrescanAvgVolumeDays = ReadInt("Prescan.AvgVolumeDays",20);
		static public int PrescanAvgVolume = ReadInt("Prescan.AvgVolume",100000);
		static public int PrescanLastDay = ReadInt("Prescan.LastDay",0);
		static public bool PrescanLoadToMemory = ReadBool("Prescan.LoadToMemory",false);

		static public string LatestValueType = Read("LatestValueType","StockOnly");
		static public int WebTimeout = ReadInt("WebTimeout",30*60); // 30 minutes by default
		static public string HistoricalDataPath = Read("HistoricalDataPath","Data\\");
		static public int FixDataTestDays = ReadInt("FixData.TestDays",300);
		static public int FixDataDifference = ReadInt("FixData.Difference",25);
		static public int FixDataGapDays = ReadInt("FixData.GapDays",10);
		static public int FixDataNoDataDays = ReadInt("FixData.NoDataDays",7);
		
		static public StickRenderType StickRenderType = (StickRenderType)ReadEnum("StickRenderType",typeof(StickRenderType),StickRenderType.Default);
		static public int DatePickerStartYear = ReadInt("DatePicker.StartYear",1896);
		static public int MaxDataForPull = ReadInt("MaxDataForPull",300);
		static public string NetQuoteUpdatePath = Read("NetQuoteUpdatePath","UpdatePath");

		static public bool YahooQuoteEmptySymbolBox = ReadBool("YahooQuote.EmptySymbolBox",false);
		static public int AdjustHours = ReadInt("AdjustHours",0);

		static public string TickExt=Read("TickExt",".tic");
		static public string DataExt=Read("DataExt",".dat");
		static public string DataPath=Read("DataPath","datafiles");
		static public string ChartPath
		{
			get
			{
				return Read("ChartPath","");
			}
		}

		static public string Path 
		{
			get 
			{
				string s = HttpRuntime.AppDomainAppVirtualPath;
				if (!s.EndsWith("/"))
					s +="/";
				return s;
			}
		}

		static public string PluginsDirectory 
		{
			get
			{
				string s = Read("PluginsDir");
				if (s==null || s=="")
					return null;
				if (!s.EndsWith("\\"))
					s +="\\";
				s = HttpRuntime.AppDomainAppPath+s;
				return s;
			}
		}

		static public int MinOverlay = ReadInt("MinOverlay",5);
		static public int MinIndicator = ReadInt("MinIndicator",5);

		public static string AdminUserName = Read("AdminUserName");
		public static string AdminPassword = Read("AdminPassword");
		public static string PreScanExchange=Read("PreScanExchange","NASDAQ;NYES;AMEX");

		public static string PreScan
		{
			get 
			{
				string s = Read("PreScanFormula");
				if (s==null) return "";
				return s;
			}
			set 
			{
				Write("PreScanFormula",value);
			}
		}

		public static string DefaultSkin = Read("DefaultSkin","RedWhite");
		
		public static int IncludeTodayQuote = ReadInt("IncludeTodayQuote",0);
 
		public static bool RealtimeVisible()
		{
			if (IncludeTodayQuote==3)
				return true;
			int H = DateTime.UtcNow.AddHours(-4).Hour;
			if (IncludeTodayQuote!=2 && IncludeTodayQuote!=4 || H<MarketOpen || H>MarketClose)
				return false;
			else return true;
		}

		public static bool UseRealtime(System.Web.UI.WebControls.CheckBox cb)
		{
			int H = DateTime.UtcNow.AddHours(-4).Hour;
			if (IncludeTodayQuote==1)
				return true;
			else if (Config.IncludeTodayQuote==0)
				return false;
			else if (Config.IncludeTodayQuote==4 && H>=MarketOpen && H<MarketClose)
				return true;
			else return cb.Visible && cb.Checked;
		}

		static public string HistoricalDataYear = Read("HistoricalDataYear","1980");
		static public bool KeepLatestScanResultOnly = ReadBool("KeepLatestScanResultOnly",true);

		 static public string AutoPullFormulaData
		{
			get 
			{
				return Read("AutoPullFormulaData");
			}
			set 
			{
				Write("AutoPullFormulaData",value);
			}
		}

		static public string DefaultChartWidth = ReadInt("DefaultChartWidth",780).ToString();
		static public int MarketOpen = ReadInt("MarketOpen",9);
		static public int MarketClose=ReadInt("MarketClose",16);
		static public string AutoUpdate = Read("AutoUpdate");
		static public string AutoUpdateFormula = Read("AutoUpdateFormula");

		public static int ReadInt(string Key,int Def)
		{
			return Tools.ToIntDef(Read(Key),Def);
		}

		static private object ReadEnum(string Key,Type T,object Def)
		{
			try
			{
				return  Enum.Parse(T,Read(Key,Def.ToString()),false);
			}
			catch
			{
				return Def;
			}
		}

		public static bool ReadBool(string Key,bool Def)
		{
			string s = Read(Key);
			if (s=="1")
				return true;
			else if (s=="0")
				return false;
			else return Def;
		}

		public static string Read(string Key,string Def) 
		{
			string s = ConfigurationSettings.AppSettings[Key];
			if (s==null)
				return Def;
			return s;
		}

		public static string Read(string Key)
		{
			return Read(Key,"");
		}

		public static void Write(string Key,string Value)
		{
			XmlDocument xd = new XmlDocument();
			string s = HttpRuntime.AppDomainAppPath+@"\web.config";
			xd.Load(s);
			XmlNode xns = xd.SelectSingleNode("/configuration/appSettings");
			for(int i=0; i<xns.ChildNodes.Count; i++)
				if (xns.ChildNodes[i] is XmlElement)
				{
					XmlElement xe = xns.ChildNodes[i] as XmlElement;
					if (xe.Name.ToLower() == "add") 
					{
						if (string.Compare(xe.GetAttribute("key").ToString(),Key,true)==0)
							xe.SetAttribute("value",Value);
					}
				}
			Impersonate.ChangeToAdmin();
			xd.Save(s);
		}
	}
}
