using System;
using System.Data;
using EasyDb;
using System.Net;
using System.IO;
using System.Text;
using Easychart.Finance.DataProvider;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for DBDataManager.
	/// </summary>
	public class DBDataManager:IDataManager
	{
		public bool DownloadHistoryWhileNeed;
		public bool DownloadRealTimeQuote;

		public DBDataManager()
		{
		}

		public IDataProvider GetData(string Code)
		{
			DataRow dr = DB.GetFirstRow("select * from StockData where QuoteCode=? or AliasCode=?",
				new DbParam[]{
								 new DbParam("@QuoteCode",DbType.String,Code),
								 new DbParam("@AliasCode",DbType.String,Code),
			});

			CSVDataProvider cdpn = new CSVDataProvider(this);
			if (dr!=null)
			{
				byte[] bs = Utils.LoadHisDataFromFile(Code);
				if (DownloadRealTimeQuote)
					try 
					{
						DataPackage dp = DataPackage.DownloadFromYahoo(Code);
						bs = CSVDataProvider.MergeOneQuote(bs,dp);
					}
					catch 
					{
					}

				cdpn.LoadBinary(bs);
				cdpn.SetStringData("Code",Code);
				cdpn.SetStringData("Name",dr["QuoteName"].ToString());
				cdpn.SetStringData("Exchange",dr["Exchange"].ToString());
				return cdpn;
			}
			throw new Exception("No Data Found");
		}

		public IDataProvider this[string Code]
		{
			get 
			{
				return GetData(Code);
			}
		}
	}
}
