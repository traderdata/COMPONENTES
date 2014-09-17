using System;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// Summary description for StreamingData.
	/// </summary>
	public class StreamingData
	{
		public string Symbol;
		public DateTime QuoteTime;
		public double Price;
		public double Volume;
		public DateTime Time;

		public StreamingData(string Symbol,DateTime QuoteTime,double Price,double Volume)
		{
			this.Symbol = Symbol;
			this.QuoteTime = QuoteTime;
			this.Price = Price;
			this.Volume = Volume;
			this.Time = DateTime.Now;
		}
	}
}
