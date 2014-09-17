using System;
using System.Data;
using System.Globalization;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos
{
	/// <summary>
	/// Summary description for IntraDataManager.
	/// </summary>
	public class IntraDataManager:IntraDataManagerBase
	{
		public IntraDataManager()
		{
		}

		public override void SetStrings(CommonDataProvider cdp, string Code)
		{
			cdp.SetStringData("Code",Code);
			switch (Code)
			{
				case "^DJI":
					cdp.SetStringData("Name","Dow Jones");
					break;
				case "^IXIC":
					cdp.SetStringData("Name","Nasdaq");
					break;
				case "^SPX":
				case "^GSPC":
					cdp.SetStringData("Name","S & P 500");
					break;
			}
		}

		private double ToAutoDouble(object o)
		{
			if (o is Single)
				return ToSingle(o);
			else return ToDouble(o);
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			try
			{
				CommonDataProvider cdp = new CommonDataProvider(this);
				cdp.DateMergeType = MergeCycleType.OPEN;
				DbParam[] dps = new DbParam[]{
												 new DbParam("@Symbol",DbType.String,Code),
												 new DbParam("@Q1",DbType.DateTime,StartTime.AddDays(-5)),
												 new DbParam("@Q2",DbType.DateTime,EndTime),
				};
				BaseDb bd = DB.Open(false);
				try
				{
					DataTable dt = bd.GetDataTable("select Price,Volume,QuoteTime from Intraday where Symbol=? and QuoteTime>=? and QuoteTime<=? order by QuoteTime",dps);
					double LastV = -1;
					foreach(DataRow dr in dt.Rows)
					{
						double NowV = (double)dr["Volume"];
						if (LastV>=0) 
						{
							if (NowV>LastV)
								dr["Volume"] = NowV - LastV;
							else dr["Volume"] = (double)0;
						}
						LastV = NowV;
					}

					dps[2].Value = StartTime;
					dps[1].Value = StartTime.AddDays(-30);
					DataRow drr = bd.GetFirstRow("select Price from Intraday where Symbol=? and QuoteTime>=? and QuoteTime<? order by QuoteTime desc",dps);

					if (dt.Rows.Count>0)
						cdp.SetStringData("LastTradeTime",((DateTime)dt.Rows[dt.Rows.Count-1]["QuoteTime"]).ToString("yyyy-MM-dd HH:mm:ss",DateTimeFormatInfo.InvariantInfo));
					if (drr!=null)
						cdp.SetStringData("LastPrice",drr[0].ToString());
					SetStrings(cdp,Code);

					if (dt.Rows.Count==0)
						dt.Rows.Add(new object[]{double.NaN,0,StartTime});
					double[] CLOSE =  new double[dt.Rows.Count];
					double[] VOLUME =  new double[dt.Rows.Count];
					double[] DATE = new double[dt.Rows.Count];

					for(int i=0; i<dt.Rows.Count; i++)
					{
						CLOSE[i] = ToAutoDouble(dt.Rows[i]["Price"]);
						VOLUME[i] = ToAutoDouble(dt.Rows[i]["Volume"]);
						DATE[i] = ((DateTime)dt.Rows[i]["QuoteTime"]).ToOADate();
					}
					cdp.LoadBinary("CLOSE",CLOSE);
					cdp.LoadBinary("DATE",DATE);
					cdp.LoadBinary("VOLUME",VOLUME);
				}
				finally
				{
					bd.Close();
				}
				return cdp;
			}
			catch (Exception e)
			{
				Tools.Log("IntraDataManager:"+e.Message);
			}
			return base.GetData (Code, Count);
		}
	}
}