using System;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;

namespace Easychart.Finance
{
	/// <summary>
	/// Intraday information of certain exchange.
	/// Include open/close time, time zone etc.
	/// </summary>
	public class ExchangeIntraday:ICloneable
	{
		static public double YahooTimeZone = -5; //-4

		private bool nativeCycle;
		[DefaultValue(false),XmlAttribute,Description("Use exchange data cycle in X-axis, used in two periods exchanges.")]
		public bool NativeCycle
		{
			get
			{
				return nativeCycle;
			}
			set
			{
				nativeCycle = true;
			}
		}
		
		private bool showFirstXLabel;
		[DefaultValue(false),XmlAttribute,Description("Show first label in the X-axis")]
		public bool ShowFirstXLabel
		{
			get
			{
				return showFirstXLabel;
			}
			set
			{
				showFirstXLabel = value;
			}
		}

		private double timeZone;
		[DefaultValue(0),XmlAttribute,Description("Time zone of the exchange")]
		public double TimeZone
		{
			get
			{
				return timeZone;
			}
			set
			{
				timeZone = value;
			}
		}

		//static public int YahooAdjust = 5;
		private int yahooDelay = 30;
		[DefaultValue(30),Browsable(false),XmlIgnore]
		public int YahooDelay 
		{
			get
			{
				return yahooDelay;
			}
			set
			{
				yahooDelay = value;
			}
		}

		private ArrayList alRemoveDay;

		public virtual void Add(TimePeriod tp)
		{
			alTimePeriods.Add(tp);
		}

		ArrayList alTimePeriods = new ArrayList();
		[TypeConverter(typeof(TimePeriodsConverter)),Description("Time periods, separate by comma.")]
		public TimePeriod[] TimePeriods
		{
			get
			{
				return (TimePeriod[])alTimePeriods.ToArray(typeof(TimePeriod));
			}
			set
			{
				alTimePeriods.Clear();
				alTimePeriods.AddRange(value);
			}
		}

		public virtual TimePeriod this[int Index] 
		{
			get
			{
				return (TimePeriod)alTimePeriods[Index];
			}
		}

		[Browsable(false),XmlIgnore]
		public DateTime ExchangeTime
		{
			get
			{
				return DateTime.UtcNow.AddHours(TimeZone);
			}
		}

		[Browsable(false),XmlIgnore]
		public DateTime ExchangeDate
		{
			get
			{
				return ExchangeTime.Date;
			}
		}

		public void AddRemoveDays(int Start,int End)
		{
			if (alRemoveDay==null)
				alRemoveDay = new ArrayList();
			else alRemoveDay.Clear();
			for(int k=Start+1; k<End; k++)
				alRemoveDay.Add(k);
		}

		public bool InRemoveDays(double D)
		{
			return (alRemoveDay!=null) && (alRemoveDay.IndexOf((int)D)>=0);
		}

		public int RawDaysBetween(double d1,double d2)
		{
			int i=0;
			int i1 = (int)d1+1;
			int i2 = (int)d2;
			if (alRemoveDay!=null)
			{
				for(int k=i1; k<i2; k++)
					if (alRemoveDay.IndexOf(k)<0)
						i++;
				return i;
			}
			return (i2-i1);
		}

		public double OneDayTime(double D)
		{
			double Frac = D-(int)D;
			double Current = 0;

			foreach(TimePeriod tp in alTimePeriods)
			{
				if (Frac>tp.Time2) 
					Current += tp.Time2 - tp.Time1;
				else 
				{
					if (Frac>tp.Time1)
						Current +=Frac - tp.Time1;
					break;
				}
			}
			return Current;
		}

		public double GetOpenTimePerDay() 
		{
			double Sum = 0;
			foreach(TimePeriod tp in alTimePeriods) 
				Sum +=tp.Time2-tp.Time1;
			return Sum;
		}

		public double[] GetMinuteDate(DateTime t1,DateTime t2)
		{
			ArrayList al = new ArrayList();
			double D1 = t1.ToOADate();
			double D2 = t2.ToOADate();
			while (D1<=D2)
			{
				foreach(TimePeriod tp in alTimePeriods) 
				{
					int i=0; 
					double d =tp.Time1;
					while (d<tp.Time2)
					{
						d =tp.Time1+(double)i/24/60;
						al.Add(D1+d);
						i++;
					};
				}
				D1 +=1;
			}
			return (double[])al.ToArray(typeof(double));
		}

		[Browsable(false),XmlIgnore]
		public int Count
		{
			get
			{
				return alTimePeriods.Count;
			}
		}

		public bool IsEstimateOpen(DateTime D)
		{
			return (D.Hour>=DateTime.FromOADate(this[0].Time1).Hour) &&
				(D.Hour<=DateTime.FromOADate(this[Count-1].Time2).AddMinutes(YahooDelay).Hour);
		}

		public bool IsBeforeMarketStart(DateTime D)
		{
			DateTime DD = DateTime.FromOADate(this[0].Time1);
			return D.Hour<DD.Hour || (D.Hour==DD.Hour && D.Minute<DD.Minute);
		}

		public DateTime GetCurrentTradingDay()
		{
			DateTime D = DateTime.UtcNow.AddHours(TimeZone);
			if (IsBeforeMarketStart(D)) D = D.AddDays(-1);
			while (D.DayOfWeek==DayOfWeek.Saturday || D.DayOfWeek==DayOfWeek.Sunday)
				D = D.AddDays(-1);
			return D.Date;
		}

		public bool InTimePeriod(double D)
		{
			double Frac = D-(int)D;
			foreach(TimePeriod tp in alTimePeriods)
			{
				if (Frac>=tp.Time1 && Frac<=tp.Time2+1.0/24/60)
					return true;
			}
			return false;
		}

		public static string[] BuildInExchange
		{
			get
			{
				Type t = typeof(ExchangeIntraday);
				PropertyInfo[] pis = t.GetProperties(BindingFlags.Static | BindingFlags.Public);
				string[] ss = new string[pis.Length-1];
				for(int i =1; i<pis.Length; i++) 
					ss[i-1] = pis[i].Name;
				return ss;
			}
		}

		public static ExchangeIntraday GetExchangeIntraday(string Exchange) 
		{
			if (Exchange=="") return US;
			try 
			{
				Type t = typeof(ExchangeIntraday);
				return (ExchangeIntraday)t.InvokeMember(Exchange,BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty,null,null,null);
			} 
			catch 
			{
				return US;
			}
		}

		public static ExchangeIntraday US
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					(new DateTime(1,1,1,9,30,0)).ToOADate(),
					(new DateTime(1,1,1,16,0,0)).ToOADate()));
				ei.timeZone = YahooTimeZone;
				return ei;
			}
		}

		public static ExchangeIntraday Canada
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					(new DateTime(1,1,1,9,30,0)).ToOADate(),
					(new DateTime(1,1,1,16,0,0)).ToOADate()));
				ei.timeZone = YahooTimeZone;
				return ei;
			}
		}

		public static ExchangeIntraday France
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					(new DateTime(1,1,1,9,0,0)).ToOADate(),
					(new DateTime(1,1,1,17,30,0)).ToOADate()));
				ei.timeZone = 6+YahooTimeZone;
				return ei;
			}
		}

		public static ExchangeIntraday Germany
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					(new DateTime(1,1,1,9,0,0)).ToOADate(),
					(new DateTime(1,1,1,17,0,0)).ToOADate()));
				ei.timeZone = 6+YahooTimeZone;
				return ei;
			}
		}

		public static ExchangeIntraday UK
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					(new DateTime(1,1,1,8,0,0)).ToOADate(),
					(new DateTime(1,1,1,16,45,0)).ToOADate()));
				ei.timeZone = 5+YahooTimeZone;
				return ei;
			}
		}

		public static ExchangeIntraday Forex
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					(new DateTime(1,1,1,0,0,0)).ToOADate(),
					(new DateTime(1,1,1,23,59,59)).ToOADate()));
				ei.TimeZone = 0;
				ei.yahooDelay = 0;
				ei.ShowFirstXLabel = true;
				return ei;
			}
		}

		public static ExchangeIntraday Sg
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.nativeCycle = true;
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,9,0,0),
					new DateTime(1,1,1,12,30,0)));
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,14,0,0),
					new DateTime(1,1,1,17,0,0)));
				ei.timeZone = 8;
				ei.showFirstXLabel = true;
				return ei;
			}
		}

		public static ExchangeIntraday China
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.NativeCycle = true;
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,9,30,0),
					new DateTime(1,1,1,11,30,0)));
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,13,0,0),
					new DateTime(1,1,1,15,0,0)));
				ei.timeZone = 8;
				ei.showFirstXLabel = true;
				return ei;
			}
		}

		public static ExchangeIntraday HK
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.NativeCycle = true;
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,10,0,0),
					new DateTime(1,1,1,12,30,0)));
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,14,30,0),
					new DateTime(1,1,1,16,0,0)));
				ei.timeZone = 8;
				ei.showFirstXLabel = true;
				return ei;
			}
		}

		public static ExchangeIntraday Japan
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.NativeCycle = true;

				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,9,0,0),
					new DateTime(1,1,1,11,0,0)));
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,12,30,0),
					new DateTime(1,1,1,15,0,0)));

				ei.timeZone = 9;
				ei.showFirstXLabel = true;
				return ei;
			}
		}

		public static ExchangeIntraday ASX
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,10,0,0),
					new DateTime(1,1,1,16,0,0)));
				ei.timeZone = 9;//11;
				return ei;
			}
		}

		public static ExchangeIntraday ASXE
		{
			get
			{
				ExchangeIntraday ei = new ExchangeIntraday();
				ei.Add(
					new TimePeriod(
					new DateTime(1,1,1,9,0,0),
					new DateTime(1,1,1,17,0,0)));
				ei.timeZone = 10;
				return ei;
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			ExchangeIntraday ei = new ExchangeIntraday();
			ei.nativeCycle = nativeCycle;
			ei.showFirstXLabel = showFirstXLabel;
			ei.timeZone = timeZone;
			ei.yahooDelay = yahooDelay;
			ei.TimePeriods = (TimePeriod[])TimePeriods.Clone();
			return ei;
		}
		#endregion
	}
}