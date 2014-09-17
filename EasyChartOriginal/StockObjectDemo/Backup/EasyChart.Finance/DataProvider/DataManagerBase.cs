using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// The base implement of the IDataManager interface
	/// </summary>
	public class DataManagerBase :IDataManager
	{
		private bool changed;
		public bool Changed
		{
			get
			{
				return changed;
			}
			set
			{
				changed = value;
			}
		}

		private DateTime startTime;
		public DateTime StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				startTime = value;
				changed = true;
			}
		}
		private DateTime endTime;
		public DateTime EndTime
		{
			get
			{
				return endTime;
			}
			set
			{
				endTime = value;
				changed = true;
			}
		}

		private bool isFix;
		public bool IsFix
		{
			get
			{
				return isFix;
			}
			set
			{
				isFix = value;
				changed = true;
			}
		}

		private ExchangeIntraday intradayInfo;
		public ExchangeIntraday IntradayInfo
		{
			get
			{
				return intradayInfo;
			}
			set
			{
				intradayInfo = value;
				changed = true;
			}
		}

		private int futureBars;
		public int FutureBars
		{
			get
			{
				return futureBars;
			}
			set
			{
				futureBars = value;
				changed = true;
			}
		}

		private bool downloadRealTimeQuote;
		public bool DownloadRealTimeQuote
		{
			get
			{
				return downloadRealTimeQuote;
			}
			set
			{
				downloadRealTimeQuote = value;
				changed = true;
			}
		}

		public DataManagerBase()
		{
		}

		public DataTable RecordRange(DataTable dt, int StartRecords, int MaxRecords)
		{
			for(int i=0; i<StartRecords; i++)
				dt.Rows.RemoveAt(0);
			while (dt.Rows.Count>MaxRecords)
				dt.Rows.RemoveAt(dt.Rows.Count-1);
			return dt;
		}

		private bool virtualFetch;
		/// <summary>
		/// True if the data manager support virtual fetching
		/// </summary>
		public  bool VirtualFetch
		{
			get
			{
				return virtualFetch;
			}
			set
			{
				virtualFetch = value;
			}
		}

		private DateTime virtualStartTime;
		/// <summary>
		/// Start time of virtual fetching
		/// </summary>
		public  DateTime VirtualStartTime
		{
			get
			{
				return virtualStartTime;
			}
			set
			{
				virtualStartTime = value;
			}
		}

		private DateTime virtualEndTime;
		/// <summary>
		/// End time of virtual fetching
		/// </summary>
		public DateTime VirtualEndTime
		{
			get
			{
				return virtualEndTime;
			}
			set
			{
				virtualEndTime = value;
			}
		}

		/// <summary>
		/// Get DataProvider of certain code.
		/// Must be implement by inherited classes.
		/// </summary>
		/// <param name="Code">Stock symbol</param>
		/// <param name="Count">How many days to get</param>
		/// <returns>IDataProvider</returns>
		public virtual IDataProvider GetData(string Code,int Count)
		{
			return null;
		}

		public virtual void SaveData(string Symbol,IDataProvider idp,Stream OutStream,DateTime Start,DateTime End,bool Intraday)
		{	
		}

		public void SaveData(string Symbol,IDataProvider idp,bool Intraday)
		{
			SaveData(Symbol,idp,null,DateTime.MinValue,DateTime.MaxValue,Intraday);
		}

		/// <summary>
		/// Save symbols list to database.
		/// </summary>
		/// <param name="ss">
		/// Symbol;Name;Exchange
		/// ... ...
		/// </param>
		public virtual void SaveSymbolList(string[] ss,out int succ,out int failed)
		{
			succ = 0;
			failed = ss.Length;
		}

		public virtual CommonDataProvider UpdateEod(string Symbol,CommonDataProvider cdpDelta)		
		{
			CommonDataProvider cdp = (CommonDataProvider)this[Symbol];
			cdp.Merge(cdpDelta);
			SaveData(Symbol,cdp,false);
			return cdp;
		}

		public virtual void SetStrings(CommonDataProvider cdp, string Code)
		{
			cdp.SetStringData("Code",Code);
		}

		/// <summary>
		/// Implement the interface
		/// </summary>
		public virtual IDataProvider this[string Code]
		{
			get 
			{
				return this[Code,DataPacket.MaxValue];
			}
		}

		/// <summary>
		/// Implement the interface
		/// </summary>
		public virtual IDataProvider this[string Code,int Count]
		{
			get 
			{
				IDataProvider idp = GetData(Code,Count);
				if (idp is CommonDataProvider)
					(idp as CommonDataProvider).FutureBars=this.futureBars;
				return idp;
			}
		}

		public virtual DataTable GetStockList(string Exchange,string ConditionId,string Key,string Sort,int StartRecords,int MaxRecords)
		{
			return null;
		}

		public DataTable GetStockList(string Exchange,string ConditionId,string Key)
		{
			return GetStockList(Exchange,ConditionId,Key,"",0,int.MaxValue);
		}

		public DataTable GetStockList()
		{
			return GetStockList(null,null,null);
		}

		/// <summary>
		/// Delete symbols from the data manager
		/// </summary>
		/// <param name="Symbols">Stock symbols separated by comma or semi-colon</param>
		/// <returns></returns>
		public int DeleteSymbols(string Symbols)
		{
			return DeleteSymbols(Symbols.Split(',',';'));
		}

		/// <summary>
		/// Delete symbols from the data manager
		/// </summary>
		/// <param name="Symbols"></param>
		/// <returns></returns>
		public int DeleteSymbols(string[] Symbols)
		{
			return DeleteSymbols("",Symbols,false,true,true);
		}

		public virtual int DeleteSymbols(string Exchange,string[] Symbols,bool Remain, bool DeleteRealtime,bool DeleteHistorical)
		{
			return 0;
		}

		public virtual DataTable GetSymbolList(string Exchange,string ConditionId,string Key,string Sort,int StartRecords,int MaxRecords)
		{
			return null;
		}

		public DataTable GetSymbolList(string Exchange,string ConditionId,string Key)
		{
			return GetSymbolList(Exchange,ConditionId,Key,"",0,int.MaxValue);
		}

		public DataTable GetSymbolList()
		{
			return GetSymbolList(null,null,null);
		}

		public string[] GetSymbolStrings(string Exchange,string ConditionId,string Key)
		{
			DataTable dt = GetSymbolList(Exchange,ConditionId,Key);
			string[] ss = new string[dt.Rows.Count];
			for(int i=0; i<ss.Length; i++)
				ss[i] = dt.Rows[i][0].ToString();
			return ss;
		}

		public string[] GetSymbolStrings()
		{
			return GetSymbolStrings(null,null,null);
		}

		public virtual int SymbolCount(string Exchange,string ConditionId,string Key)
		{
			return -1;
		}

		public virtual DataTable Exchanges
		{
			get
			{
				return null;
			}
		}

		public virtual DataGridColumn[] StockListColumns
		{
			get
			{
				return null;
			}
		}

		static public double ToDouble(object o)
		{
			if (o==DBNull.Value)
				return double.NaN;
			else return (double)o;
		}

		static public double ToDecimalDouble(object o)
		{
			if (o==DBNull.Value)
				return double.NaN;
			else return (double)(System.Decimal)o;
		}

		static public Single ToSingle(object o)
		{
			if (o==DBNull.Value)
				return Single.NaN;
			else return (Single)o;
		}

		static public int ToInt(object o)
		{
			if (o==DBNull.Value)
				return 0;
			else return (int)o;
		}

		static public long ToInt64(object o)
		{
			if (o==DBNull.Value)
				return 0;
			else return (long)o;
		}

		static public DateTime ToDate(object o)
		{
			if (o==DBNull.Value)
				return DateTime.Today;
			return (DateTime)o;
		}

		static public DateTime ToDateDef(string Format,string s,DateTime Def)
		{
			try
			{
				return DateTime.ParseExact(s,Format,DateTimeFormatInfo.InvariantInfo);
			}
			catch
			{
				return Def;
			}
		}

		protected BoundColumn CreateBoundColumn(string HeaderText,string DataField,string SortExpression,string DataFormatString)
		{
			BoundColumn bc = new BoundColumn();
			bc.DataField = DataField;
			bc.SortExpression = SortExpression;
			bc.HeaderText = HeaderText;
			bc.DataFormatString = DataFormatString;
			return bc;
		}

		protected HyperLinkColumn CreateHyperLinkColumn(string Text,string DataNavigateUrlField,string DataNavigateUrlFormatString)
		{
			HyperLinkColumn hlc = new HyperLinkColumn();
			hlc.Text = Text;
			hlc.DataNavigateUrlField = DataNavigateUrlField;
			hlc.DataNavigateUrlFormatString = DataNavigateUrlFormatString;
			return hlc;
		}
	}
}
