using System;

namespace Easychart.Finance.DataClient
{
	/// <summary>
	/// Summary description for DataProgressArgs.
	/// </summary>
	public class DataProgressArgs
	{
		public string Symbol;
		public int CurrentValue;
		public int MaxValue;
		public DataProgressArgs(string Symbol,int CurrentValue,int MaxValue)
		{
			this.Symbol = Symbol;
			this.CurrentValue = CurrentValue;
			this.MaxValue = MaxValue;
		}
	}
	public delegate void DataProgress(object sender ,DataProgressArgs e);
}
