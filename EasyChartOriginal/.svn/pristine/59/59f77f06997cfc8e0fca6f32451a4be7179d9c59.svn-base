using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// DataManager interface
	/// </summary>
	public interface IDataManager 
	{
		/// <summary>
		/// Return the DataProvider of certain code
		/// </summary>
		IDataProvider this[string Code] {get;}
		/// <summary>
		/// Return lastest 'Count' days DataProvider of certain code
		/// </summary>
		IDataProvider this[string Code,int Count] {get;}
		DataTable GetStockList(string Exchange,string ConditionId,string Key,string Sort,int StartRecords,int MaxRecords);
		DataTable GetSymbolList(string Exchange,string ConditionId,string Key,string Sort,int StartRecords,int MaxRecords);
		int SymbolCount(string Exchange,string ConditionId,string Key);
		DataTable Exchanges{get;}
		DataGridColumn[] StockListColumns{get;}
	}
}
