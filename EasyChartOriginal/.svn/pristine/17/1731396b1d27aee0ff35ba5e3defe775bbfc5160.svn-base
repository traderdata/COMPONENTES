using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Globalization;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// Will take an inner data manager, you can add streaming data to it.
	/// </summary>
	public class MemoryDataManager : DataManagerBase
	{
		static private Hashtable htStreaming = new Hashtable();
		static private Hashtable htHistorical = new Hashtable();
		private DataManagerBase InnerDataManager;

		/// <summary>
		/// Add new streaming data packet
		/// </summary>
		/// <param name="dp">streaming data packet</param>
		public void AddNewPacket(DataPacket dp)
		{
			string Symbol = dp.Symbol;
			//if code end with =X remove it
			if (Symbol.EndsWith("=X"))
				Symbol= Symbol.Substring(0,Symbol.Length-2);
			CommonDataProvider cdp = htStreaming[Symbol] as CommonDataProvider;
			if (cdp==null)
			{
				cdp = CommonDataProvider.Empty;
				htStreaming[Symbol] = cdp;
			}
			cdp.Merge(dp);
		}

		/// <summary>
		/// Remove symbol from memory
		/// </summary>
		/// <param name="Symbol">Symbol which you want to remove from the memory</param>
		public void RemoveSymbol(string Symbol)
		{
			htHistorical.Remove(Symbol);
			htStreaming.Remove(Symbol);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="InnerDataManager">Inner data manager</param>
		public MemoryDataManager(DataManagerBase InnerDataManager)
		{
			this.InnerDataManager = InnerDataManager;
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			CommonDataProvider cdp1;
			cdp1 = (CommonDataProvider)htHistorical[Code];
			if (cdp1==null) 
			{
				if (InnerDataManager!=null)
				{
					InnerDataManager.StartTime = this.StartTime;
					InnerDataManager.EndTime = this.EndTime;
					cdp1 = InnerDataManager[Code,Count] as CommonDataProvider;
					cdp1.DataManager = this;
				}
				else 
					cdp1 = CommonDataProvider.Empty;
				htHistorical[Code] = cdp1;
			}

			CommonDataProvider cdp2 = (CommonDataProvider)htStreaming[Code];
			if (cdp2!=null) 
			{
				cdp1.Merge(cdp2);
				cdp2.ClearData();
			}
			cdp1.SetStringData("Code",Code);
			return cdp1;
		}
	}
}
