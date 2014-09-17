using System;
using System.Drawing.Text;
using System.Drawing;
using System.Data;
using System.Collections;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Net;
using System.Globalization;
using System.Text;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
		private Utils()
		{
		}

		/// <summary>
		/// ^DJI(US) will return ^DJI
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		static public string GetPart1(string s)
		{
			int i = s.IndexOf('(');
			if (i<0) i = s.Length;
			return s.Substring(0,i);
		}

		static public string GetPart2(string Symbol)
		{
			int i = Symbol.IndexOf('(');
			int j = Symbol.LastIndexOf(')');
			if (j>i)
				return Symbol.Substring(i+1,j-i-1);
			return "";
		}

		/// <summary>
		/// ^DJI(US) will return US
		/// </summary>
		/// <param name="Symbol"></param>
		/// <returns></returns>
		static public string GetExchange(string Symbol)
		{
			string[] ss = Config.IntradaySymbols.Split(';');
			foreach(string s in ss)
				if (s.StartsWith(Symbol))
					return GetPart2(s);
			return "";
		}

		public static string GetName(string s)
		{
			int i=s.IndexOf('{');
			if (i>=0)
				s = s.Substring(0,i);
			i=s.IndexOf('=');
			if (i>=0)
				return s.Substring(0,i);
			else return s;
		}

		public static string GetValue(string s) 
		{
			int i=s.IndexOf('{');
			if (i>=0)
				s = s.Substring(0,i);
			i=s.IndexOf('=');
			if (i>=0)
				return s.Substring(i+1);
			else return s;
		}

		public static string GetParam(string s,string Def) 
		{
			int i=s.IndexOf('{');
			int j=s.LastIndexOf('}');
			if (i>=0 && j>i)
				return s.Substring(i+1,j-i-1);
			else return Def;
		}

		public static void AddSkinToDropList(DropDownList ddlSkin)
		{
			string[] ss = FormulaSkin.GetBuildInSkins();
			foreach(string s in ss) 
			{
				ListItem li = new ListItem(s);
				if (string.Compare(s,Config.DefaultSkin)==0)
					li.Selected = true;
				ddlSkin.Items.Add(li);
			}
		}

		public static int ObjPlusDef(object o,int Def)
		{
			if (o==null) 
				return Def;
			else 
			{
				return (int)o+1;
			}
		}

		public static int ObjDef(object o,int Def)
		{
			if (o==null)
				return Def;
			else return (int)o;
		}

		public static void PreRange(string Range,out DateTime StartDate,out DateTime EndDate,out string Cycle)
		{
			EndDate = DateTime.Now.AddDays(1);
			StartDate = EndDate;
			Cycle = "DAY1";
			if (Range.Length<2)
				Range = "3m";
			Range = Range.ToLower();
			if (Range=="max") Range = "120y";
			int Num = Tools.ToIntDef(Range.Substring(0,Range.Length-1),1);
			if (Range.EndsWith("m")) 
				StartDate = EndDate.AddMonths(-Num);
			else if (Range.EndsWith("y"))
			{
				StartDate = EndDate.AddYears(-Num);
				if (Num==1)
					Cycle = "DAY1";
				else if (Num<5)
					Cycle = "WEEK1";
				else if (Num<10)
					Cycle = "WEEK2";
				else if (Num<120)
					Cycle = "MONTH1";
				else Cycle = "MONTH6";
			}
			else if (Range.EndsWith("d"))
			{
				StartDate = EndDate.AddDays(-Num);

			}
		}

		public static void PreRange(string Range,out string Start,out string End,out string Cycle)
		{
			DateTime StartDate;
			DateTime EndDate;
			PreRange(Range,out StartDate,out EndDate,out Cycle);
			Start = StartDate.ToString("yyyyMMdd");
			End = EndDate.ToString("yyyyMMdd");
		}

		public static void BulkInsert(ArrayList al) 
		{
			BaseDb bd = DB.Open(false);
			try 
			{
				DbParam[] dps = new DbParam[]{
												 new DbParam("@ConditionId",DbType.Int32,0),
												 new DbParam("@QuoteCode",DbType.String,""),
											 };
				foreach(object oo in al) 
				{
					string[] ss = oo.ToString().Split(',');
					dps[0].Value = ss[0];
					dps[1].Value = ss[1];
					try
					{
						bd.DoCommand("insert into ScanedQuote (ConditionId,QuoteCode) values (?,?)",dps);
					} 
					catch (Exception e)
					{
						Tools.Log("Bulk Insert:"+ss[0]+","+ss[1]+","+e);
					}
				}
			}
			finally 
			{
				bd.Close();
			}
		}

		public static void UpdateRealtime(string QouteCode,IDataProvider idp)
		{
			int i=idp.Count;
			if (i!=0) 
			{
				DbParam[] dps = new DbParam[]{
												 new DbParam("@LastA",DbType.Double,DBNull.Value),
												 new DbParam("@LastTime",DbType.DateTime,DateTime.FromOADate(idp["DATE"][i-1])),
												 new DbParam("@OpenA",DbType.Double, idp["OPEN"][i-1]),
												 new DbParam("@HighA",DbType.Double,idp["HIGH"][i-1]),
												 new DbParam("@LowA",DbType.Double,idp["LOW"][i-1]),
												 new DbParam("@CloseA",DbType.Double,idp["CLOSE"][i-1]),
												 new DbParam("@VolumeA",DbType.Double,idp["VOLUME"][i-1]),
												 new DbParam("@QuoteCode",DbType.String,QouteCode),
											 };
				if (i>1)
					dps[0].Value = idp["CLOSE"][i-2];

				//int k = 0;
				for(int j=0; j<dps.Length; j++)
					if (dps[j].Type==DbType.Double)
						if (dps[j].Value is Double && double.IsNaN((double)dps[j].Value))
							dps[j].Value = DBNull.Value;


				//for(int j=2; j<6; j++)
				//	if (dps[j].Value is Double && double.IsNaN((double)dps[j].Value))
				//		k++;

				//if (k!=4)
				//{
				//	Tools.Log("0="+dps[0].Value+";2="+dps[2].Value+";3="+dps[3].Value+";4="+dps[4].Value+";5="+dps[5].Value);
					if (DB.DoCommand("update Realtime set LastA=?,LastTime=?,OpenA=?,HighA=?,LowA=?,CloseA=?,VolumeA=? where QuoteCode=?",dps)==0)
					{
						DB.DoCommand("insert into Realtime (LastA,LastTime,OpenA,HighA,LowA,CloseA,VolumeA,QuoteCode) values (?,?,?,?,?,?,?,?)",dps);
					}
				//}
			}
		}

		public static string GetExchangeCond(string Exchange)
		{
			if (Exchange=="^")
				Exchange = "QuoteCode like '^%'";
			else if (Exchange=="" || Exchange==null)
				Exchange = "Exchange !='Economic'";
			else Exchange = "Exchange like '"+Exchange+"%'";
			return Exchange;
		}

		public static DateTime ToDateDef(string s,DateTime Def)
		{
			return Tools.ToDateDef(s,new string[]{"yyyyMMdd","yyyyMMddHHmmss"},Def);
		}

		static public DataManagerBase GetDataManager(string DataManagerName)
		{
			if (string.Compare(DataManagerName,"Yahoo",true)==0)
				return new YahooDataManager();
			else if (DataManagerName.StartsWith("MS("))
				return new MSDataManager(DataManagerName.Substring(3,DataManagerName.Length-4));

			DataManagerName = "WebDemos."+DataManagerName+"DataManager";
			Type t = Type.GetType(DataManagerName);
			if (t==null)
				throw new Exception(DataManagerName+" not found!");
			return (DataManagerBase)Activator.CreateInstance(t);
		}

		static public DataManagerBase GetDefaultDataManager()
		{
			return GetDataManager(Config.DefaultDataManager);
		}

		public static byte[] DownloadData(string URL)
		{
			return FormulaHelper.DownloadData(URL);
//			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(URL);
//			hwr.Timeout = Config.WebTimeout*1000;
//
//			if (Config.WebProxy!=null && Config.WebProxy!="")
//				hwr.Proxy = new WebProxy(Config.WebProxy);
//			hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
//			ArrayList al = new ArrayList();
//			using (HttpWebResponse hws= (HttpWebResponse)hwr.GetResponse())
//			{
//				BinaryReader br = new BinaryReader(hws.GetResponseStream());
//				byte[] bs;
//				do 
//				{
//					bs = br.ReadBytes(100000);
//					al.AddRange(bs);
//				} while (bs.Length>0);
//				return (byte[])al.ToArray(typeof(byte));
//			}
		}



		public static string DownloadString(string URL)
		{
			byte[] bs = DownloadData(URL);
			return Encoding.UTF8.GetString(bs);
		}

		/// <summary>
		/// Verify volume and last date according parameters defined in web.config
		/// </summary>
		/// <param name="idp"></param>
		/// <returns></returns>
		public static bool VerifyVolumeAndDate(IDataProvider idp) 
		{
			bool b = true;
			try
			{
				if (Config.PrescanCheckVolume) 
				{
					double[] V = idp["VOLUME"];
					int AvgVolumeDays = Config.PrescanAvgVolumeDays;
					double Sum = 0;
					int j = 0;
					for(int i=V.Length-1; i>=Math.Max(0,V.Length-1-AvgVolumeDays); i--) 
					{
						Sum +=V[i];
						j++;
					}
					if (j>0)
						if (Sum/j<Config.PrescanAvgVolume)
							b = false;
				}

				if (Config.PrescanLastDay>0)
				{
					double[] D = idp["DATE"];
					if (D.Length>0 && D[D.Length-1]<=DateTime.Today.ToOADate()-Config.PrescanLastDay)
						b = false;
				}
			}
			catch
			{
			}
			return b;
		}

		static public void DownloadYahooHistory(string Symbol,bool Force,bool AddRealtime)
		{
			string FileName = DBDataManager.GetHisDataFile(Symbol);
			if (Force || !File.Exists(FileName))
			{
				DateTime d = DateTime.Now.AddDays(3);
				string URL = "http://table.finance.yahoo.com/table.csv?s={0}&d="+(d.Month-1)+"&e="+d.Day+"&f="+d.Year+"&g=d&a=6&b=30&c="+Config.HistoricalDataYear+"&ignore=.csv";
				string s = string.Format(URL,Symbol);
				byte[] bs = DownloadData(s);
				CommonDataProvider cdpn = new YahooDataManager().LoadYahooCSV(bs);
				if (AddRealtime)
					cdpn.Merge(DataPacket.DownloadFromYahoo(Symbol));
				UpdateRealtime(Symbol,cdpn);
				if (File.Exists(FileName))
					File.Delete(FileName);
				cdpn.SaveBinary(FileName);
			}
		}

		static public void SetYahooCacheRoot(IDataManager idm)
		{
			if (idm is YahooDataManager) 
			{
				YahooDataManager ydm = idm as YahooDataManager;
				ydm.CacheRoot = HttpRuntime.AppDomainAppPath+"Cache\\";
			}
		}

		static public void BindExchange(DropDownList ddlExchange)
		{
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			ddlExchange.DataSource = dmb.Exchanges;
			ddlExchange.DataBind();
			try
			{
				ddlExchange.SelectedValue = Config.DefaultExchange;
			}
			catch
			{
			}
		}

	}
}
