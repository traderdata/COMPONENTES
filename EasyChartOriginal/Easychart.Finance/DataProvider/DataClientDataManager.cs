using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Globalization;
using Easychart.Finance.DataClient;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// Summary description for DataClientDataManager.
	/// </summary>
	public class DataClientDataManager : DataManagerBase
	{
		DataClientBase DataClient;
		bool Intraday;
		public DataClientDataManager(DataClientBase DataClient, bool Intraday)
		{
			this.DataClient = DataClient;
			this.Intraday = Intraday;
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			if (DataClient.Logined) 
			{
				CommonDataProvider cdp;
				if (Intraday)
					cdp = DataClient.GetIntradayData(Code,1,StartTime,EndTime);
				else cdp = DataClient.GetHistoricalData(Code);
				cdp.SetStringData("Code",Code);
				return cdp;
			}
			return base.GetData (Code, Count);
		}
	}
}
