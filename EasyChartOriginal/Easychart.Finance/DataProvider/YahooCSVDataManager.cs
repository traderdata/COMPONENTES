using System;
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
	/// Data Manager for yahoo finance format
	/// </summary>
	public class YahooCSVDataManager:CacheDataManagerBase
	{
		private string Root;
		private string Ext;

		public YahooCSVDataManager()
		{
			CacheTimeSpan = TimeSpan.FromHours(12);//.FromDays(1);
		}

		public YahooCSVDataManager(string Root,string Ext)
		{
			if (!Root.EndsWith("\\")) Root +="\\";
			this.Root = Root;
			if (!Ext.StartsWith(".")) Ext = "."+Ext;
			this.Ext = Ext;
		}

		public CommonDataProvider LoadYahooCSV(StreamReader sr)
		{
			string s = sr.ReadToEnd().Trim();
			string[] ss = s.Split('\n');
			ArrayList al = new ArrayList();
			for(int i=1; i<ss.Length; i++) 
			{
				ss[i] = ss[i].Trim();
				if (!ss[i].StartsWith("<!--"))
					al.Add(ss[i]);
			}

			int N = al.Count;
			double[] CLOSE = new double[N];
			double[] OPEN = new double[N];
			double[] HIGH = new double[N];
			double[] LOW = new double[N];
			double[] VOLUME = new double[N];
			double[] DATE = new double[N];
			double[] ADJCLOSE = new double[N];

			DateTimeFormatInfo dtfi = DateTimeFormatInfo.InvariantInfo;
			NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
			for (int i=0; i<N; i++) 
			{
				string[] sss = ((string)al[i]).Split(',');
				if (sss.Length<7)
				{
					string[] rrr = new string[7];
					for(int k=0; k<sss.Length; k++)
						rrr[k] = sss[k];
					if (sss.Length==6)
						rrr[6] = sss[4];
					//Format: 3-Mar-1904,13
					if (sss.Length==2) 
						for(int k=2; k<rrr.Length; k++)
							rrr[k] = sss[1];

					sss = rrr;
				}
				int j = N-i-1;
				DATE[j] = DateTime.ParseExact(sss[0],"yyyy-MM-dd",dtfi).ToOADate();  //%d-MMM-yy
				OPEN[j] = double.Parse(sss[1],nfi);
				HIGH[j] = double.Parse(sss[2],nfi);
				LOW[j] = double.Parse(sss[3],nfi);
				CLOSE[j] = double.Parse(sss[4],nfi);
				VOLUME[j] = double.Parse(sss[5],nfi);
				ADJCLOSE[j] = double.Parse(sss[6],nfi);
			}

			CommonDataProvider cdp = new CommonDataProvider(this);
			cdp.LoadBinary(new double[][]{OPEN,HIGH,LOW,CLOSE,VOLUME,DATE,ADJCLOSE});
			return cdp;
		}

		public CommonDataProvider LoadYahooCSV(Stream stream)
		{
			using (StreamReader sr = new StreamReader(stream))
				return LoadYahooCSV(sr);
		}

		public CommonDataProvider LoadYahooCSV(byte[] data)
		{
			return LoadYahooCSV(new MemoryStream(data));
		}

		public CommonDataProvider LoadYahooCSV(string FileName)
		{
			return LoadYahooCSV(File.OpenRead(FileName));
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			string Filename = Root+Code+Ext;
			if (File.Exists(Filename))
			{
				using (StreamReader sr = new StreamReader(Filename))
				{
					CommonDataProvider cdp = LoadYahooCSV(sr);
					cdp.SetStringData("Code",Code);
					return cdp;
				}
			}
			return CommonDataProvider.Empty;
		}

		//http://ichart.finance.yahoo.com/table.csv?s=S&d=11&e=25&f=2004&g=d&a=0&b=2&c=1970
		public override void SaveData(string Symbol,IDataProvider idp,Stream OutStream,DateTime Start,DateTime End,bool Intraday)
		{
			int Count = idp.Count;
			double[] Date =(double[])idp["DATE"];
			double[] Open =(double[])idp["OPEN"];
			double[] High = (double[])idp["HIGH"];
			double[] Low = (double[])idp["LOW"];
			double[] Close = (double[])idp["CLOSE"];
			double[] Volume = (double[])idp["VOLUME"];
			double[] AdjClose = (double[])idp["ADJCLOSE"];
			using (StreamWriter sw = new StreamWriter(OutStream))
			{
				sw.WriteLine("Date,Open,High,Low,Close,Volume,Adj. Close*");
				double d1 = Start.ToOADate();
				double d2 = End.ToOADate();
				for(int i=Count-1; i>=0; i--)
					if (Date[i]>=d1 && Date[i]<=d2)
						sw.WriteLine(
							DateTime.FromOADate(Date[i]).ToString("yyyy-MM-dd",DateTimeFormatInfo.InvariantInfo)+","+ //dd-MMM-yy
							Open[i].ToString("f2")+","+
							High[i].ToString("f2")+","+
							Low[i].ToString("f2")+","+
							Close[i].ToString("f2")+","+
							Volume[i].ToString("f0")+","+
							AdjClose[i].ToString("f2"));
			}
		}
	}
}
