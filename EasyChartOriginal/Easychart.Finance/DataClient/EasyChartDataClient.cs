using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Easychart.Finance.DataProvider;
using System.Globalization;

namespace Easychart.Finance.DataClient
{
	/// <summary>
	/// Summary description for EasyChartDataClient.
	/// </summary>
	public class EasyChartDataClient : DataClientBase
	{
		public override event DataProgress OnProgress;
		public event EventHandler OnFinished;
		public override event StreamingDataChanged OnStreamingData;
		public override event EventHandler OnStreamingStopped;

		public override bool SupportEod
		{
			get
			{
				return true;
			}
		}

		public override bool IsFree
		{
			get
			{
				return true;
			}
		}

		public override bool SupportSymbolList
		{
			get
			{
				return true;
			}
		}

		public override bool SupportIntraday
		{
			get
			{
				return false;
			}
		}

		public override bool SupportStreaming
		{
			get
			{
				return true;
			}
		}

		public override bool SupportIndustry
		{
			get
			{
				return true;
			}
		}

		public override string[] GetExchanges()
		{
//							<asp:ListItem Value="AMEX">American Stock Exchange </asp:ListItem>
//							<asp:ListItem Value="NasdaqNM">Nasdaq Stock Exchange NM</asp:ListItem>
//							<asp:ListItem Value="NasdaqSC">Nasdaq Stock Exchange SC</asp:ListItem>
//							<asp:ListItem Value="NYSE">NYSE Stock Exchanges</asp:ListItem>
//							<asp:ListItem Value="OTC BB">OTC Bulletin Board Market</asp:ListItem>
//							<asp:ListItem Value="Shanghai">Shanghai Stock Exchange</asp:ListItem>
//							<asp:ListItem Value="Shenzhen">Shenzhen Stock Exchange</asp:ListItem>
//							<asp:ListItem Value="Toronto">Toronto Stock Exchange (TOR)</asp:ListItem>
//							<asp:ListItem Value="Vancouver">TSX Venture Exchange (CVE)</asp:ListItem>
//							<asp:ListItem Value="^">Global Indices</asp:ListItem>
//
			return new string[]{"AMEX;Nasdaq;Nyse;^=All US & Index","AMEX;Nasdaq;Nyse=All US","AMEX","Nasdaq","Nyse","OTC+BB=OTC BB","Shanghai","Shenzhen","TOR;CDNX=Canada","TOR=Toronto","CDNX=Vancouver","ASX","^=Global Indices"};
		}

		public override string Description
		{
			get
			{
				return "This is a free data feed, provided by http://finance.easychart.net.";
			}
		}

		public EasyChartDataClient()
		{
			OnFinished -=null;
			OnProgress -=null;
		}

		public override bool Login(string Username, string Password)
		{
			//if (Ticket==null || Ticket=="")
			//{
				Ticket = DownloadString(
					"http://subscribe.easychart.net/GetTicket.aspx?UserId="+Username+"&Password="+Password);
			//}
			return Ticket!=null && Ticket!="";
		}

		public override string[] LookupSymbols(string Key,string Exchanges,string StockType,string Market)
		{
			if (Logined)
			{
				string s = DownloadString("http://subscribe.easychart.net/member/datafeed.aspx?f=SymbolList&Exchanges="+Exchanges);
				if (s!=null)
				{
					string[] ss = s.Trim().Split('\r');
					return ss;
				}
			}
			return null;
		}

		public override void StopDownload()
		{
			Canceled = true;
		}

		public override CommonDataProvider GetData(string symbols, DataCycle dataCycle, DateTime startTime, DateTime endTime)
		{
			byte[] bs = DownloadBinary(symbols,"http://subscribe.easychart.net/member/datafeed.aspx?f=BinaryHistory&AddWhenNoSymbol=1&Symbol="+symbols);
			if (bs!=null)
			{
				CommonDataProvider cdp = new CommonDataProvider(null);
				cdp.LoadByteBinary(bs);
				cdp.SetStringData("Code",symbols.ToUpper());
				return cdp;
			}
			return null;
		}

		public override DataPacket[] GetEodData(string Exchanges,string[] symbols,DateTime Time)
		{
			string s = DownloadString("http://subscribe.easychart.net/member/datafeed.aspx?f=EndOfDay&Exchanges="+Exchanges+"&Date="+Time.ToString("yyyy-MM-dd"));
			if (s!=null && s!="")
			{
				string[] ss = s.Trim().Split('\r');
				DataPacket[] dps = new DataPacket[ss.Length];
				for(int i=0; i<dps.Length; i++)
					dps[i] = DataPacket.ParseEODData(ss[i].Trim());
				return dps;
			}
			return base.GetEodData (Exchanges,symbols,Time);
		}

		public override void DownloadStreaming()
		{
			try
			{
				string URL = "http://data.easychart.net/streaming.aspx?Symbols="+GetStreamingSymbol(";");
				HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(URL);
				if (Proxy!=null && Proxy!="")
					hwr.Proxy = new WebProxy(Proxy);
				hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705; .NET CLR 1.1.4322)";
				HttpWebResponse hws= (HttpWebResponse)hwr.GetResponse();
				StreamReader sr = new StreamReader(hws.GetResponseStream(),Encoding.ASCII);
				while (true) 
				{
					string r  = sr.ReadLine();
					if (r==null) break;
					string[] ss = r.Split(';');
					if (OnStreamingData!=null)
					{
						IFormatProvider fp = FormulaHelper.USFormat;
						DataPacket dp = new DataPacket(ss[0],
							DateTime.Parse(ss[1],DateTimeFormatInfo.InvariantInfo),
							float.Parse(ss[2],fp),
							double.Parse(ss[3],fp));
						dp.TimeZone = double.MaxValue;
						OnStreamingData(this,dp);
					}
				}
			}
			catch (Exception e)
			{
				LastError = e;
			}
			finally
			{
				if (OnStreamingStopped!=null)
					OnStreamingStopped(this,new EventArgs());
			}
		}

		public override string DataFeedName
		{
			get
			{
				return "Easy Chart Demo Data Feed";
			}
		}

		public override string RegURL
		{
			get
			{
				return "http://subscribe.easychart.net/reg.aspx";
			}
		}

		public override string HomePage
		{
			get
			{
				return "http://finance.easychart.net";
			}
		}

		public override string LoginURL
		{
			get
			{
				return "http://subscribe.easychart.net/login.aspx";
			}
		}

		public override string[] GetIndustry()
		{
			string s = DownloadString("http://data.easychart.net/Industry.aspx");
			if (s!=null)
			{
				string[] ss = s.Split('\r');
				return ss;
			}
			return base.GetIndustry();
		}
	}
}