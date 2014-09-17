using System;
using System.Collections;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using EasyTools;
using Easychart.Finance.DataProvider;
using Easychart.Finance;

namespace WebDemos
{
	/// <summary>
	/// Load data from a sql database
	/// </summary>
	public class DBDataManager:DataManagerBase
	{
		public bool AutoYahooToDB;

		public DBDataManager()
		{
		}

		public override IDataProvider GetData(string Code,int Count)
		{
			Code = Code.Trim();
			Hashtable htList = GetSymbolListHashtable();
			string s = (string)htList[Code];
			CommonDataProvider cdpn = new CommonDataProvider(this);
			

			byte[] bs = new byte[]{};
			string[] ss = null;

			if (s!=null)
			{
				ss = s.Split(',');
				if (ss[3]!="") 
				{
					string r = (string)htList[ss[3]];
					if (r!=null)
						ss = r.Split(',');
				}
				if (Count>0)
				{
					// Load data from file
					bs = LoadHisDataFromFile(Code,Count*DataPacket.PacketByteSize);
					if (DownloadRealTimeQuote)
						try 
						{
							DataPacket dp = DataPacket.DownloadFromYahoo(Code);
						
							if (dp!=null && !dp.IsZeroValue)
								bs = CommonDataProvider.MergeOneQuote(bs,dp);
						}
						catch 
						{
						}

					cdpn.LoadByteBinary(bs);

					// Update data from yahoo
					if (AutoYahooToDB) 
					{
						string FileName = GetHisDataFile(Code);
						DateTime d = new DateTime(1900,1,1);
						try 
						{
							d = File.GetLastWriteTime(FileName);// GetLastAccessTime(FileName);
						} 
						catch 
						{
						}
						DateTime d1 = DateTime.Now.Date;
						DateTime d2 = d.Date;
						TimeSpan ts= d1-d2;
						if (ts.TotalDays>0)
						{
							YahooDataManager ydm = new YahooDataManager();
							CommonDataProvider cdpDelta = (CommonDataProvider)ydm[Code,ts.Days+5];
							cdpDelta.Adjusted = false;
							if (cdpDelta.Count>0)
							{
								double[] dd1 = cdpDelta["DATE"];
								double[] dd2 = cdpn["DATE"];
								if (cdpn.Count==0 || (int)dd2[dd2.Length-1]<(int)dd1[dd1.Length-1])
								{
									cdpn.Merge(cdpDelta);
									Impersonate.ChangeToAdmin();
									Utils.UpdateRealtime(Code,cdpn);
								}
							}
							cdpn.SaveBinary(FileName);
						}
					}
				}
			}
			else 
			{
				try 
				{
					// Download data from yahoo
					if (AutoYahooToDB)
					{
						YahooDataManager ydm = new YahooDataManager();
						cdpn = (CommonDataProvider)ydm[Code];
						if (cdpn.Count>0) 
						{
							UpdateForNewSymbol(Code,cdpn,true);
						} 
						else throw new Exception("Invalid Quote : "+Code);
					} 
					else cdpn.LoadByteBinary(bs);
				}
				catch (Exception e)
				{
					Tools.Log(e.Message);
					cdpn.LoadByteBinary(bs);
				}
			}
			//cdpn = TrimToEndTime(cdpn);

			cdpn.SetStringData("Code",Code);
			if (ss!=null && ss.Length>2) 
			{
				cdpn.SetStringData("Code",ss[0]);
				cdpn.SetStringData("Name",ss[1]);
				cdpn.SetStringData("Exchange",ss[2]);
			}
			return cdpn;
		}

		private string GetWhere(string Exchange, string ConditionId, string Key)
		{
			string Where = " where 1=1";
			if (ConditionId!=null && ConditionId.StartsWith("_")) 
			{
				string s = ConditionId.Substring(1);
				if (s!="")
					Where = " where a.QuoteCode in ('"+ string.Join("','",s.Split(','))+"')";
			}
			else //if (Tools.ToIntDef(ConditionId,int.MinValue)!=int.MinValue)
				if (ConditionId!=null && ConditionId!="")
					Where = ",ScanedQuote b where a.QuoteCode=b.QuoteCode and b.ConditionId in ("+ConditionId+")";

			if (Key!=null) 
				Where +=" and (a.QuoteCode like '%"+Key+"%' or a.QuoteName like '%"+Key+"%')";
			if (Exchange!=null && Exchange!="")
				Where +=" and a."+Utils.GetExchangeCond(Exchange); 
			return Where;
		}

		public override DataTable GetStockList(string Exchange, string ConditionId, string Key, string Sort, int StartRecords, int MaxRecords)
		{
			if (Sort=="" || Sort==null)
				Sort = "a.QuoteCode";
			if (Sort!="")
				Sort =" Order by "+Sort;
			string c1 = "c.LastTime Date,c.LastA Last,c.OpenA,c.HighA,c.LowA,c.CloseA,c.VolumeA,(c.CloseA-c.LastA)/c.LastA as ChangeA,";
			string c2 = "left join Realtime c on a.QuoteCode=c.QuoteCode ";
			
			if (ConditionId!=null && ConditionId.StartsWith("NoRealtime"))
			{
				ConditionId = ConditionId.Substring(10);
				c1 = "";
				c2 = "";
			}
			return DB.GetDataTable( "select a.QuoteCode,a.QuoteName,"+c1+"a.Exchange"
				+" from StockData a "+c2
				+GetWhere(Exchange,ConditionId,Key)+Sort, null,StartRecords,MaxRecords);
		}

		public override DataTable GetSymbolList(string Exchange, string ConditionId, string Key, string Sort, int StartRecords, int MaxRecords)
		{
			string Sql = "select distinct a.QuoteCode from StockData a "+GetWhere(Exchange,ConditionId,Key)+" order by a.QuoteCode";
			DataTable dt = DB.GetDataTable(Sql,null,StartRecords,MaxRecords);
			return dt;
		}

		public override int SymbolCount(string Exchange, string ConditionId, string Key)
		{
			DataRow drCount = 
				DB.GetFirstRow("select count(*) from StockData a"+GetWhere(Exchange,ConditionId,Key));
			return Tools.ToIntDef(drCount[0].ToString(),0);
		}

		public override DataTable Exchanges
		{
			get
			{
				DataTable dt = DB.GetDataTable("select distinct Exchange as Value from StockData where not (QuoteCode like '^%')");
				dt.Columns.Add("Text");
				for(int i=0; i<dt.Rows.Count; i++)
					dt.Rows[i][1] = dt.Rows[i][0];

				DataRow dr = dt.NewRow();
				dr[0] = "^";
				dr[1] = "INDEX"; 
				dt.Rows.InsertAt(dr,0);

				dr = dt.NewRow();
				dr[0] = "";
				dr[1] = "ALL";
				dt.Rows.InsertAt(dr,0);
				return dt;
			}
		}

		public static string DataRoot
		{
			get
			{
				return HttpRuntime.AppDomainAppPath+Config.HistoricalDataPath;
			}
		}

		public static string GetHisDataFile(string Code) 
		{
			string s = Code.ToUpper();
			if (s.StartsWith("PRN"))
				Code = "P"+Code;
			else if (s.StartsWith("AUX"))
				Code = "A"+Code;
			return DataRoot+Code+".dat";
		}

		public static byte[] LoadHisDataFromFile(string Code,int Count) 
		{
			string FileName = GetHisDataFile(Code);
			if (File.Exists(FileName)) 
			{
				FileStream fs = File.OpenRead(FileName);
				if (Count>fs.Length) 
					Count = (int)fs.Length;
				else fs.Seek(-Count,SeekOrigin.End);
				byte[] bs = new byte[Count];
				fs.Read(bs,0,bs.Length);
				fs.Close();
				return bs;
			}
			else return new byte[0];
		}

		public static byte[] LoadHisDataFromFile(string Code) 
		{
			return LoadHisDataFromFile(Code,DataPacket.MaxValue*DataPacket.PacketByteSize);
		}

		public static void SaveBinary(string Code,byte[] bs) 
		{
			string FileName = GetHisDataFile(Code);
			FileStream fs = File.OpenWrite(FileName);
			fs.Write(bs,0,bs.Length);
			fs.Close();
		}

		public static void RefreshSymbolList()
		{
			HttpRuntime.Cache.Remove("QuoteList");
		}

		public static Hashtable GetSymbolListHashtable()
		{
			Hashtable htList = (Hashtable)HttpRuntime.Cache["QuoteList"];
			if (htList==null) 
			{
				htList = new Hashtable(new CaseInsensitiveHashCodeProvider(),new CaseInsensitiveComparer());
				DataTable dt = DB.GetDataTable("select QuoteCode,QuoteName,Exchange,AliasCode from StockData");
				foreach(DataRow dr in dt.Rows)
					htList[dr[0].ToString()] =dr[0]+","+dr[1]+","+dr[2]+","+dr[3];
				HttpRuntime.Cache["QuoteList"] = htList;
			}
			return htList;
		}

		public static void UpdateForNewSymbol(string Code,CommonDataProvider cdp,bool Save)
		{
			Utils.UpdateRealtime(Code,cdp);

			if (Save)
			{
				Impersonate.ChangeToAdmin();
				cdp.SaveBinary(GetHisDataFile(Code));
			}
			RefreshSymbolList();

			string[] ss = YahooDataManager.GetStockName(Code);
			if (ss.Length==3) 
				try
				{
					DB.DoCommand("insert into StockData (QuoteCode,QuoteName,Exchange) values (?,?,?)",
						new DbParam[]{
										 new DbParam("@Code",DbType.String,Code),
										 new DbParam("@Name",DbType.String,ss[1]),
										 new DbParam("@Exchange",DbType.String,ss[2]),
					});
				}
				catch
				{
				}
			cdp.SetStringData("Code",ss[0]);
			cdp.SetStringData("Name",ss[1]);
			cdp.SetStringData("Exchange",ss[2]);
		}

		public override System.Web.UI.WebControls.DataGridColumn[] StockListColumns
		{
			get
			{
				return new DataGridColumn[]{
					CreateHyperLinkColumn("Chart","QuoteCode","CustomChart.aspx?"+Config.SymbolParameterName+"={0}"),
					CreateBoundColumn("Symbol","QuoteCode","a.QuoteCode",null),
					CreateBoundColumn("Name","QuoteName","QuoteName",null),
					CreateBoundColumn("Date","Date","Date","{0:yyyy-MM-dd}"),
					CreateBoundColumn("Last","Last","Last desc","{0:f2}"),
					CreateBoundColumn("Open","OpenA","OpenA desc","{0:f2}"),
					CreateBoundColumn("High","HighA","HighA desc","{0:f2}"),
					CreateBoundColumn("Low","LowA","LowA desc","{0:f2}"),
					CreateBoundColumn("Close","CloseA","CloseA desc","{0:f2}"),
					CreateBoundColumn("Volume","VolumeA","VolumeA desc","{0:f2}"),
					CreateBoundColumn("Change","ChangeA","ChangeA desc","{0:p2}"),
					CreateBoundColumn("Exchange","Exchange","Exchange",null),
				};
			}
		}

		public override void SaveSymbolList(string[] ss,out int succ,out int failed)
		{
			DbParam[] dps = new DbParam[]{
											 new DbParam("@QuoteCode",DbType.String,""),
											 new DbParam("@QuoteName",DbType.String,""),
											 new DbParam("@Exchange",DbType.String,""),
			};

			succ=0;
			failed=0;
			// false to support mysql
			BaseDb bd = DB.Open(false);
			try
			{
				for(int i=0; i<ss.Length; i++) 
				{
					string[] rr = ss[i].Trim().Split(';');
					if (rr.Length!=3) continue;
					dps[0].Value = rr[0].Trim();
					dps[1].Value = rr[1].Trim();
					dps[2].Value = rr[2].Trim();
					try 
					{
						if (bd.GetFirstRow("select QuoteCode from StockData where QuoteCode=?",new DbParam[]{dps[0]})==null)
						{
							bd.DoCommand("insert into StockData (QuoteCode,QuoteName,Exchange) values (?,?,?)",dps);
							succ++;
						}
					} 
					catch
					{
						failed++;
					}
				}
			} 
			finally 
			{
				bd.Close();
			};
		}

		public override void SaveData(string Symbol,IDataProvider idp, Stream OutStream, DateTime Start, DateTime End, bool Intraday)
		{
			string Filename = GetHisDataFile(Symbol);
			if (File.Exists(Filename))
				File.Delete(Filename);
			Utils.UpdateRealtime(Symbol,idp);
			(idp as CommonDataProvider).SaveBinary(Filename);
		}

		public override CommonDataProvider UpdateEod(string Symbol, CommonDataProvider cdpDelta)
		{
			CommonDataProvider cdp =  base.UpdateEod (Symbol, cdpDelta);
			Utils.UpdateRealtime(Symbol,cdp);
			return cdp;
		}

		public override int DeleteSymbols(string Exchange, string[] Symbols, bool Remain, bool DeleteRealtime, bool DeleteHistorical)
		{
			string s = " where 1=1 ";
			if (Exchange!=null && Exchange!="")
				s +=" and "+Utils.GetExchangeCond(Exchange);
			string Sql = "delete from StockData  "+s;
			if (Symbols!=null)
				Sql +="and QuoteCode "+(Remain?"not":"")+" in ('"+string.Join("','",Symbols)+"')";
			
			int Count = DB.DoCommand(Sql);
			if (DeleteRealtime)
				DB.DoCommand("delete from Realtime where not QuoteCode in (select QuoteCode from StockData)");

			if (DeleteHistorical)
			{
				string[] ss = GetSymbolStrings();
				Hashtable ht = new Hashtable();
				foreach(string r in ss)
					ht[r.ToUpper()] = 1;
				if (Directory.Exists(DataRoot))
					foreach(string r in Directory.GetFiles(DataRoot))
					{
						string t = Path.GetFileNameWithoutExtension(r).ToUpper();
						if (!object.Equals(ht[t],1))
							File.Delete(r);
					}
			}
			return Count;
		}
	}
}