using System;
using System.ComponentModel;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Net;
using System.Text;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.DataClient
{
	public delegate void StreamingDataChanged(object sender,DataPacket dp);
	public delegate void OnSymbolList(object sender ,string[] Symbols);

	public enum WorkingMode {EodUpdate,RealtimeStream};

	/// <summary>
	/// base client class for data feed.
	/// </summary>
	public class DataClientBase
	{
		protected ArrayList alStreamingSymbols = new ArrayList();
		protected string Ticket;
		protected bool Canceled;

		public virtual event DataProgress OnProgress;
		public virtual event StreamingDataChanged OnStreamingData;
		public virtual event EventHandler OnStreamingStopped;
		private Thread StreamingThread;
		[Browsable(false)]
		public Exception LastError;
		
		private Hashtable htExtraData = new Hashtable();
		/// <summary>
		/// Extra information hash table
		/// </summary>
		[Browsable(false)]
		public Hashtable ExtraData
		{
			get
			{
				return htExtraData;
			}
		}

		public void ClearLastError()
		{
			LastError = null;
		}

		public WorkingMode WorkingMode;

		public DataClientBase()
		{
			//Remove warning
			OnProgress -=null;
			OnStreamingData -=null;
			OnStreamingStopped -=null;
		}

		/// <summary>
		/// Load extra information of this data feed
		/// </summary>
		public virtual void LoadExtraData()
		{
		}

		/// <summary>
		/// Save extra information to disk
		/// </summary>
		public virtual void SaveExtraData()
		{
		}

		/// <summary>
		/// true if the data feed is FREE
		/// </summary>
		[Category("Features")]
		public virtual bool IsFree
		{
			get
			{
				return false;
			}
		}

		public bool UtcStreamingTime = true;

		/// <summary>
		/// true if support grab end of day data in single request, default value is false
		/// </summary>
		[Category("Features")]
		public virtual bool SupportEod
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Maximum symbols supported by the streaming data request, default value is 1
		/// <seealso cref="StartStreaming"/>
		/// </summary>
		[Category("Features")]
		public virtual int MaxSymbolsForStreaming
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Maximum symbols supported by end of day data, default value is 1
		/// </summary>
		[Category("Features")]
		public virtual int MaxSymbolsForData
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// true if the data feed support intraday data, default value is false
		/// </summary>
		[Category("Features")]
		public virtual bool SupportIntraday
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// true if the data feed is in local disk
		/// </summary>
		[Category("Features")]
		public virtual bool IsLocal
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// true if the data feed support streaming data, default value is false
		/// </summary>
		[Category("Features")]
		public virtual bool SupportStreaming
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// true if the data feed support symbol list feature, default value is false
		/// </summary>
		[Category("Features")]
		public virtual bool SupportSymbolList
		{
			get
			{
				return false;
			}
		}

		[Category("Features")]
		public virtual bool SupportIndustry
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// true if logined
		/// </summary>
		[Browsable(false)]
		public bool Logined
		{
			get
			{
				return !NeedLogin || (Ticket!=null && Ticket!="");
			}
		}

		private string proxy;
		/// <summary>
		/// Proxy server and port , 10.1.1.222:80
		/// </summary>
		[Browsable(false)]
		public string Proxy
		{
			get
			{
				return proxy;
			}
			set
			{
				proxy = value;
			}
		}

		/// <summary>
		/// true if the data feed need login
		/// </summary>
		[Category("Features")]
		public virtual bool NeedLogin
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// return the data feed name
		/// </summary>
		[Category("Properties")]
		public virtual string DataFeedName
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Description of the data feed
		/// </summary>
		[Category("Properties")]
		public virtual string Description
		{
			get
			{
				return DataFeedName;
			}
		}

		/// <summary>
		/// return the data feed registration URL
		/// </summary>
		[Category("Properties")]
		public virtual string RegURL
		{
			get
			{
				return HomePage;
			}
		}

		/// <summary>
		/// return the data feed login URL
		/// </summary>
		[Category("Properties")]
		public virtual string LoginURL
		{
			get
			{
				return HomePage;
			}
		}

		/// <summary>
		/// return the data feed home page
		/// </summary>
		[Category("Properties")]
		public virtual string HomePage
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Recreate the activeX control if need
		/// </summary>
		public virtual void ResetActiveX()
		{
		}

		/// <summary>
		/// Login the data feed
		/// </summary>
		/// <param name="Username">Username</param>
		/// <param name="Password">Password</param>
		/// <returns>true if login successfully</returns>
		public virtual bool Login(string Username, string Password)
		{
			return false;
		}

		/// <summary>
		/// Get CommonDataProvider for symbol, override one of the GetData and GetDatas is ok.
		/// </summary>
		/// <param name="symbol">stock symbol</param>
		/// <param name="dataCycle">data cycle</param>
		/// <param name="startTime">start time</param>
		/// <param name="endTime">end time</param>
		/// <returns>CommonDataProvider</returns>
		public virtual CommonDataProvider GetData(string symbol,DataCycle dataCycle,DateTime startTime,DateTime endTime)
		{
			return GetDatas(new string[]{symbol},dataCycle,startTime,endTime)[0];
		}

		/// <summary>
		/// Get CommonDataProvider for symbols, override one of the GetData and GetDatas is ok.
		/// </summary>
		/// <param name="symbols">stock symbols</param>
		/// <param name="dataCycle">data cycle</param>
		/// <param name="startTime">start time</param>
		/// <param name="endTime">end time</param>
		/// <returns>Array of CommonDataProvider</returns>
		public virtual CommonDataProvider[] GetDatas(string[] symbols,DataCycle dataCycle,DateTime startTime,DateTime endTime)
		{
			if (symbols.Length>0)
				return new CommonDataProvider[]{GetData(symbols[0],dataCycle,startTime,endTime)};
			return null;
		}

		/// <summary>
		/// Get all historical data for special symbol
		/// </summary>
		/// <param name="Symbol"></param>
		/// <returns>CommonDataProvider contains the historical data</returns>
		public CommonDataProvider GetHistoricalData(string Symbol)
		{
			return GetHistoricalData(Symbol,DateTime.Today.AddYears(-10),DateTime.Today);
		}

		/// <summary>
		/// Get intraday data
		/// </summary>
		/// <param name="Symbol">Stock Symbol</param>
		/// <param name="Interval">Interval minutes</param>
		/// <param name="StartTime">Start Time</param>
		/// <param name="EndTime">End Time</param>
		/// <returns></returns>
		public CommonDataProvider GetIntradayData(string Symbol,int Interval,DateTime StartTime,DateTime EndTime)
		{
			return GetData(Symbol,new DataCycle(DataCycleBase.MINUTE,Interval),StartTime,EndTime);
		}

		public CommonDataProvider GetHistoricalData(string Symbol,DateTime StartTime,DateTime EndTime)
		{
			return GetData(Symbol,DataCycle.Day,StartTime,EndTime);
		}

		public CommonDataProvider GetCachedHistoricalData(string Symbol)
		{
			string CacheRoot = Environment.CurrentDirectory+"\\Cache\\";
			if (!Directory.Exists(CacheRoot))
				Directory.CreateDirectory(CacheRoot);
			string Cache = CacheRoot+Symbol+".dat";
			CommonDataProvider cdp = null;
			if (Symbol!="") 
			{
				if (File.Exists(Cache) && File.GetLastWriteTime(Cache).Date==DateTime.Now.Date)
				{
					cdp = new CommonDataProvider(null);
					cdp.LoadBinary(Cache);
					cdp.SetStringData("Code",Symbol.ToUpper());
				}
				else 
				{
					cdp = GetHistoricalData(Symbol);
					if (cdp!=null && cdp.Count>0)
						cdp.SaveBinary(Cache);
				}
			}
			return cdp;
		}

		public virtual string[] LookupSymbols(string Key,string Exchanges,string StockType,string Market)
		{
			return null;
		}

		public virtual string[] GetIndustry()
		{
			return null;
		}

		public virtual string[] GetExchanges()
		{
			return null;
		}

		public virtual string[] GetStockType()
		{
			return null;
		}

		public virtual string[] GetMarket()
		{
			return null;
		}

		public virtual DataPacket[] GetEodData(string Exchanges,string[] symbols,DateTime time)
		{
			return null;
		}

		public override string ToString()
		{
			return DataFeedName;
		}

		static public void LoadDataFeeds()
		{
		}

		/// <summary>
		/// Get all data feed classes in current domain
		/// </summary>
		/// <returns>data feed class array</returns>
		static public DataClientBase[] GetAllDataFeeds()
		{
			Assembly[] ass = ass = AppDomain.CurrentDomain.GetAssemblies();
			ArrayList al = new ArrayList();
			foreach(Assembly a in ass)
			{
				if (a.FullName.ToUpper().IndexOf("EASYCHART")>=0)
				{
					try
					{
						Type[] ts = a.GetTypes();
						foreach(Type t in ts)
						{
							if (t.BaseType==typeof(DataClientBase))
								al.Add(Activator.CreateInstance(t));
						}
					} 
					catch 
					{
					}
				}
			}
			if (al.Count==0)
				al.Add(new EasyChartDataClient());
			return (DataClientBase[])al.ToArray(typeof(DataClientBase));
		}

		/// <summary>
		/// Load data feed DLLs from a folder
		/// </summary>
		/// <param name="FilePath">Folder include data feed DLLs</param>
		static public void LoadDataFeed(string FilePath)
		{
			if (Directory.Exists(FilePath))
				foreach(string s in Directory.GetFiles(FilePath,"*.dll"))
				{
					string r = Path.GetFileNameWithoutExtension(s).ToUpper();
					if (r.IndexOf("EASYCHART")>=0)
						Assembly.LoadFrom(s);
				}
		}

		/// <summary>
		/// Load data feed DLLs from default folder, the default folder is AssemblyPath\DataFeed\
		/// </summary>
		static public void LoadDataFeed()
		{
			LoadDataFeed(FormulaHelper.DataFeedRoot);
		}

		protected string DownloadString(string URL)
		{
			byte[] bs = DownloadBinary(null,URL);
			if (bs!=null)
				return Encoding.UTF8.GetString(bs);
			return null;
		}

		protected byte[] DownloadBinary(string Symbol,string URL)
		{
			if (URL.Length>2 && URL[1]==':') 
			{
				using (FileStream fs = File.OpenRead(URL))
				{
					byte[] bs= new byte[fs.Length];
					fs.Read(bs,0,bs.Length);
					return bs;
				}
			}

			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(URL);
			try
			{
				hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705; .NET CLR 1.1.4322)";
				hwr.Accept = "*/*";
				hwr.KeepAlive = false;
				hwr.Referer = "http://subscribe.easychart.net";

				if (Proxy!=null && Proxy!="")
					hwr.Proxy = new WebProxy(Proxy);
				else if (FormulaHelper.DownloadProxy!=null)
					hwr.Proxy =FormulaHelper.DownloadProxy;


				if (Logined && Ticket!=null)
				{
					hwr.CookieContainer = new CookieContainer();
					hwr.CookieContainer.Add(new Cookie("dc",Ticket,"/","subscribe.easychart.net"));
				}

				HttpWebResponse hws= (HttpWebResponse)hwr.GetResponse();
				BinaryReader br  = new BinaryReader(hws.GetResponseStream());
				

				int Sum = 0;
				Canceled = false;
				ArrayList al = new ArrayList();
				while (true)
				{
					byte[] bs = br.ReadBytes(1000);
					if (hws.ContentLength>0)
					{
						if (OnProgress!=null)
						{
							Sum +=bs.Length;
							OnProgress(this, new DataProgressArgs(Symbol,Sum,(int)hws.ContentLength));
						}
					}
					if (Canceled)
						break;
					if (bs.Length>0)
						al.AddRange(bs);
					else break;
				}
				byte[] bss = (byte[])al.ToArray(typeof(byte));
				return  bss;
			}
			finally
			{
				hwr.Abort();
			}
		}
		
		/// <summary>
		/// true if handle streaming data in thread
		/// </summary>
		public virtual bool ThreadStreaming
		{
			get
			{
				return true;
			}
		}

		private string TrimStreamingSymbol(string s)
		{
			int j = 0;
			for(int i=0; i<s.Length; i++) 
			{
				if (s[i]==',')
					j++;
				if (j>=MaxSymbolsForStreaming)
					return s.Substring(0,i);
			}
			return s;
		}

		/// <summary>
		/// Start streaming
		/// </summary>
		/// <param name="Symbols">Symbols to streaming, separated by comma or semi colon</param>
		public void StartStreaming(string Symbols)
		{
			SetStreamingSymbol(Symbols);
			if (ThreadStreaming)
			{
				if (StreamingThread==null) 
				{
					//StopStreaming();
					StreamingThread = new Thread(new ThreadStart(DownloadStreaming));
					StreamingThread.Start();
				}
			} else
				DownloadStreaming();
		}

		public bool HasStreamingSymbol()
		{
			return alStreamingSymbols.Count>0;
		}

		public string GetStreamingSymbol(string separator)
		{
			return string.Join(separator,(string[])alStreamingSymbols.ToArray(typeof(string)));
		}

		/// <summary>
		/// Stop streaming
		/// </summary>
		public virtual void StopStreaming()
		{
			if (ThreadStreaming)
			{
				if (StreamingThread!=null)
				{
					StreamingThread.Abort();
					StreamingThread.Join();
					StreamingThread = null;
				}
			}
		}

		public virtual void DownloadStreaming()
		{
		}
			
		public virtual void StopDownload()
		{
		}

		public virtual void SteamingSymbolChanged()
		{
			if (!ThreadStreaming)
				DownloadStreaming();
		}

		/// <summary>
		/// Set the download list
		/// </summary>
		/// <param name="Symbol"></param>
		public void SetStreamingSymbol(string Symbol)
		{
			alStreamingSymbols.Clear();
			AddStreamingSymbol(Symbol);
		}

		/// <summary>
		/// Add new symbol to the download list
		/// </summary>
		/// <param name="Symbol"></param>
		public void AddStreamingSymbol(string Symbol)
		{
			string[] ss = Symbol.Split(',',';');
			foreach(string s in ss)
			{
				string r = s.ToUpper();
				if (alStreamingSymbols.IndexOf(r)<0)
					alStreamingSymbols.Add(r);
			}
			SteamingSymbolChanged();
		}

		/// <summary>
		/// Remove symbol from the download list
		/// </summary>
		/// <param name="Symbol"></param>
		public void RemoveStreamingSymbol(string Symbol)
		{
			alStreamingSymbols.Remove(Symbol);
			SteamingSymbolChanged();
		}
	}
}