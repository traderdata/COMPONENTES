using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Easychart.Finance.DataProvider;
using Easychart.Finance.DataClient;

namespace Easychart.Finance.DataClient
{
	/// <summary>
	/// Summary description for YahooDataClient.
	/// </summary>
	public class YahooDataClient : DataClientBase
	{
		public override event DataProgress OnProgress;
		public override event StreamingDataChanged OnStreamingData;
		public override event EventHandler OnStreamingStopped;

		public YahooDataClient()
		{
		}

		public override bool IsFree
		{
			get
			{
				return true;
			}
		}

		public override bool NeedLogin
		{
			get
			{
				return false;
			}
		}

		public override int MaxSymbolsForStreaming
		{
			get
			{
				return 100;
			}
		}

		public override string HomePage
		{
			get
			{
				return "http://finance.yahoo.com";
			}
		}

		public override bool SupportEod
		{
			get
			{
				return false;
			}
		}

		public override bool SupportIntraday
		{
			get
			{
				return false;
			}
		}


		public override string DataFeedName
		{
			get
			{
				return "Yahoo finance";
			}
		}

		public override string Description
		{
			get
			{
				return "Provided by yahoo finance";
			}
		}

		public override string[] GetStockType()
		{
			return new string[]{"S=Stocks","E=ETF","I=Indices","M=Mutual Fund","O=Options"};
		}

		public override string[] GetMarket()
		{
			return new string[]{"US=U.S. & Canada","=World Markets"};
		}

		public override string[] LookupSymbols(string Key, string Exchanges, string StockType, string Market)
		{
			if (StockType==null) StockType = "";
			if (Market==null) Market = "";
			string s = DownloadString("http://finance.yahoo.com/l?s="+Key+"&t="+StockType+"&m="+Market);
			//"<a href=(?<href>[^>]+)><font face=arial size=-1>(?<symbol>[^<]+)</font>",RegexOptions.IgnoreCase);
			MatchCollection mmc = Regex.Matches(s,
				"<TR bgcolor=#ffffff><TD><font face=arial size=-1><a href=(?<href>[^>]+)>(?<symbol>[^<]+)</a></font></TD><TD><font face=arial size=-1>(?<name>[^<]+)</font></TD><TD><font face=arial size=-1>(?<exchange>[^<]+)</font>",RegexOptions.IgnoreCase);
			
			ArrayList al = new ArrayList();
			foreach(Match m in mmc)
				al.Add(m.Groups["symbol"].Value+";"+m.Groups["name"].Value+";"+m.Groups["exchange"].Value);
			return (string[])al.ToArray(typeof(string));
		}

		public override CommonDataProvider GetData(string symbols, Easychart.Finance.DataCycle dataCycle, DateTime startTime, DateTime endTime)
		{
			string URL = "http://table.finance.yahoo.com/table.csv?s={0}&d={4}&e={5}&f={6}&g=d&a={1}&b={2}&c={3}&ignore=.csv";
			string s = string.Format(URL,symbols,startTime.Month-1,startTime.Day,startTime.Year,endTime.Month-1,endTime.Day,endTime.Year);
			byte[] bs = DownloadBinary(symbols,s);
			CommonDataProvider cdp = new YahooCSVDataManager().LoadYahooCSV(bs);
			return cdp;
		}

		public override DataPacket[] GetEodData(string Exchanges, string[] symbols, DateTime time)
		{
			string URL = "http://table.finance.yahoo.com/table.csv?s={0}&d={4}&e={5}&f={6}&g=d&a={1}&b={2}&c={3}&ignore=.csv";
			DataPacket[] dps = new DataPacket[symbols.Length];
			DateTime start = time.AddDays(-1);
			for(int i=0; i<symbols.Length; i++)
			{
				string symbol = symbols[i];
				string s = string.Format(URL,symbol,start.Month-1,start.Day,start.Year,time.Month-1,time.Day,time.Year);
				byte[] bs = DownloadBinary(symbol,s);
				CommonDataProvider cdp = new YahooCSVDataManager().LoadYahooCSV(bs);
				dps[i] = cdp.GetLastPackage();
				if (OnProgress!=null)
					OnProgress(this,new DataProgressArgs(symbol,i,symbols.Length));
			}
			return dps;
		}

		public override void DownloadStreaming()
		{
			try
			{
				while (true)
				{
					DataPacket[] dps = DataPacket.DownloadMultiFromYahoo(GetStreamingSymbol(","));
					foreach(DataPacket dp in dps)
					{
						if (dp!=null && !dp.IsZeroValue)
						{
							if (UtcStreamingTime)
								dp.Date = dp.Date.Date+dp.Date.AddHours(-dp.TimeZone).TimeOfDay;
							if (OnStreamingData!=null)
								OnStreamingData(this,dp);
						}
					}

					Thread.Sleep(5000);
				}
			}
			finally
			{
				if (OnStreamingStopped!=null)
					OnStreamingStopped(this,new EventArgs());
			}
		}
	}
}