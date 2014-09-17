using System;
using System.Web;
using EasyTools;
using Easychart.Finance.DataProvider;
using WebDemos.net.easychart.data;

namespace WebDemos
{
	/// <summary>
	/// Load data from data base.
	/// if no symbol found ,load the data from easy chart data service.
	/// </summary>
	public class ServiceDataManager:DataManagerBase
	{
		private DBDataManager ddm;
		public ServiceDataManager()
		{
			ddm = new DBDataManager();
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			ddm.DownloadRealTimeQuote = this.DownloadRealTimeQuote;
			CommonDataProvider cdp = (CommonDataProvider)ddm[Code,Count];
			if (!cdp.HasData)
			{
				EasyStockChartDataFeed df = new EasyStockChartDataFeed();
				try
				{
					byte[] bs = df.BinaryHistory(Code,true);
					if (bs!=null)
					{
						cdp.LoadByteBinary(bs);
						DBDataManager.UpdateForNewSymbol(Code,cdp,Config.SaveInServerDataManager);
					}
				}
				catch
				{
				}
			}
			return cdp;
		}
	}
}
