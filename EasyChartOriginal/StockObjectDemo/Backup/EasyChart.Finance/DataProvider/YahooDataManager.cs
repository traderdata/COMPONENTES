using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Caching;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// Data Manager for yahoo finance
	/// </summary>
	public class YahooDataManager:YahooCSVDataManager
	{
		/// <summary>
		/// Create YahooDataManager
		/// </summary>
		public YahooDataManager():base()
		{
		}

		public override void SetStrings(CommonDataProvider cdp, string Code)
		{
			cdp.SetStringData("Code",Code);
		}

		/// <summary>
		/// Create IDataProvider by Code and Count
		/// </summary>
		/// <param name="Code">Stock Symbol</param>
		/// <param name="Count">How many bars to get</param>
		/// <returns>IDataProvider</returns>
		public override IDataProvider GetData(string Code,int Count)
		{
			DateTime d1;
			if (Count>365*73) 
			{
				Count = 365*73;
				d1 = new DateTime(1930,1,1);
			} 
			else 
				d1 = DateTime.Now.AddDays(-Count).Date;

			
			DateTime d2 = DateTime.Now;
			if (EndTime!=DateTime.MinValue)
				d2 = EndTime.AddDays(1);
			if (StartTime!=DateTime.MinValue)
				d1 = StartTime;

			string URL = "http://table.finance.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g=d&ignore=.csv";

			string s = string.Format(URL,Code,d1.Month-1,d1.Day,d1.Year,d2.Month-1,d2.Day,d2.Year);
			//WebClient wc = new WebClient();
			try
			{
				byte[] bs = FormulaHelper.DownloadData(s);// wc.DownloadData(s);
				CommonDataProvider cdp = LoadYahooCSV(bs);
				SetStrings(cdp,Code);
				return cdp;
			}
			catch (WebException)
			{
				throw new Exception("Symbol "+Code+" not found!");
			}
			catch (Exception e)
			{
				throw new Exception("Invalid data format!"+Code+";"+e.Message);
			}
		}

		public static string DownloadRealtimeFromYahoo(string Code,string Param) 
		{
			string Prefix = "";
			string Suffix = Code.ToLower();
			Encoding E = Encoding.ASCII;

			string URL = "http://"+Prefix+"finance.yahoo.com/d/quotes.csv?s="+Code+"&f="+Param;
			string r = string.Format(URL,Code);
			//WebClient wc = new WebClient();
			byte[] bsReal = FormulaHelper.DownloadData(r);// wc.DownloadData(r);
			return E.GetString(bsReal);
		}

		public static string TryDownloadRealtimeFromYahoo(string Code,string Param)
		{
			for(int i=0; i<5; i++)
				try
				{
					return DownloadRealtimeFromYahoo(Code,Param);
				} 
				catch
				{
				}
			return null;
		}

		public static string[] GetStockName(string Code) 
		{
			string[] ss = {};
			string s = DownloadRealtimeFromYahoo(Code,"snx");
			if (s!=null) 
			{
				ss = s.Split(',');
				for(int i=0; i<3; i++)
				{
					ss[i] = ss[i].Trim();
					if (ss[i].Length>2)
					{
						ss[i] = ss[i].Substring(1,ss[i].Length-2);
						ss[i] = ss[i].Trim();
					}
				}
			}
			return ss;
		}
	}
}