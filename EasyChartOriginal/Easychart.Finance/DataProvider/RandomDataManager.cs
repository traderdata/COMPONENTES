using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Globalization;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// Random DataManager for test purpose
	/// </summary>
	public class RandomDataManager:CacheDataManagerBase
	{
		//bool Intraday;
		int MaxCount;
		public RandomDataManager(ExchangeIntraday ei,int MaxCount) 
		{
			this.IntradayInfo = ei;
			this.MaxCount = MaxCount;
		}

		public RandomDataManager(ExchangeIntraday ei) :this(ei,1000)
		{
		}

		public RandomDataManager(bool Intraday,int MaxCount) : this(null,MaxCount)
		{
			if (Intraday)
				this.IntradayInfo = ExchangeIntraday.US;
		}

		public RandomDataManager(bool Intraday):this(Intraday,1000)
		{
		}

		public RandomDataManager():this(false)
		{
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			CommonDataProvider cdp =new CommonDataProvider(this);
			if (Code==null)
				Code = "MSFT";
			Random Rnd = new Random(Code.GetHashCode());
			/// ds[0] : OPEN
			/// ds[1] : HIGH
			/// ds[2] : LOW
			/// ds[3] : CLOSE
			/// ds[4] : VOLUME
			/// ds[5] : DATE
			double[][] ds = new double[6][];
			double[] dd = null;
			if (IntradayInfo!=null)
			{
				//ExchangeIntraday ei = ExchangeIntraday.US;
				dd = IntradayInfo.GetMinuteDate( DateTime.Today.AddDays(-7),DateTime.Today);
				MaxCount = dd.Length;
			}

			for(int i=0; i<ds.Length; i++)
				ds[i] = new double[MaxCount];

			for(int i=0; i<MaxCount; i++)
			{
				if (i==0)
				{
					ds[0][i] = 20;
					ds[3][i] = 21;
					ds[4][i] = 100000;
				} 
				else 
				{
					ds[0][i] = Math.Round(ds[0][i-1] + Rnd.NextDouble()-0.48,2);
					ds[3][i] = Math.Round(ds[0][i-1] + Rnd.NextDouble()-0.48,2);
					ds[4][i] = Math.Round(Math.Abs(ds[4][i-1] + Rnd.Next(100000)-50000),2);
				}
				ds[1][i] = Math.Round(Math.Max(ds[0][i],ds[3][i]) + Rnd.NextDouble(),2);
				ds[2][i] = Math.Round(Math.Min(ds[0][i],ds[3][i]) - Rnd.NextDouble(),2);
				if (IntradayInfo!=null)
					ds[5][i] = dd[i];
				else ds[5][i] = DateTime.Today.AddDays(i-MaxCount).ToOADate();
			}
			cdp.LoadBinary(ds);
			cdp.SetStringData("Code",Code);
//			if (IntradayInfo!=null)
//				cdp.SetStringData("Code",Code+"@Random Intraday");
//			else cdp.SetStringData("Code",Code+"@Random");
			return cdp;
		}
	}
}
