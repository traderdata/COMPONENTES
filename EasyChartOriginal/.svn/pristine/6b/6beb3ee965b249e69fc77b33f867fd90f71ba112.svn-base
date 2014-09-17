using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections;

namespace Easychart.Finance
{
	/// <summary>
	/// Define the base data cycle enum
	/// </summary>
	public enum DataCycleBase {TICK,SECOND,MINUTE,HOUR,DAY,WEEK,MONTH,QUARTER,HALFYEAR,YEAR};

	/// <summary>
	/// Summary description for DataCycle.
	/// </summary>
	[TypeConverter(typeof(DataCycleConverter))]
	public class DataCycle
	{
		public int WeekAdjust;

		/// <summary>
		/// The base data cycle enum
		/// </summary>
		private DataCycleBase cycleBase = DataCycleBase.DAY;
		[DefaultValue(DataCycleBase.DAY), XmlAttribute,RefreshProperties(RefreshProperties.Repaint)]
		public DataCycleBase CycleBase
		{
			get
			{
				return cycleBase;
			}
			set
			{
				cycleBase = value;
			}
		}

		private int repeat;
		/// <summary>
		/// The repeat count of the base data cycle
		/// </summary>
		[DefaultValue(1), XmlAttribute,RefreshProperties(RefreshProperties.Repaint)]
		public int Repeat
		{
			get
			{
				return repeat;
			}
			set
			{
				repeat = value;
			}
		}
		
		/// <summary>
		/// Constructor of DataCycle
		/// </summary>
		public DataCycle()
		{
		}

		/// <summary>
		/// Constructor of DataCycle
		/// </summary>
		/// <param name="CycleBase">The DataCycleBase enum</param>
		/// <param name="Repeat">Repeat count of DataCycleBase</param>
		public DataCycle(DataCycleBase CycleBase,int Repeat) 
		{
			this.cycleBase = CycleBase;
			this.repeat = Repeat;
		}

		/// <summary>
		/// Get the sequence of date
		/// </summary>
		/// <param name="D">OADate</param>
		/// <returns>
		/// Date in same DataCycle return same result
		/// different DataCycle return different result.
		/// For example :  DataCycle = 1 month
		/// 2004-1-3 and 2004-1-10 will return same result
		/// 2004-1-3 and 2004-2-1 will return different result
		/// </returns>
		public int GetSequence(double D)
		{
			int Num = 0;
			if (CycleBase==DataCycleBase.DAY) 
			{
				Num = (int)D;
			}
			else if (CycleBase==DataCycleBase.WEEK) 
			{
				Num = (int)(D+WeekAdjust)/7;
			} 
			else if (CycleBase==DataCycleBase.YEAR) 
			{
				Num =DateTime.FromOADate(D).Year;
			}
			else if (CycleBase>DataCycleBase.WEEK)
			{
				DateTime Date = DateTime.FromOADate(D);
				Num = Date.Year*12+Date.Month-1;
				switch (CycleBase) 
				{
					case DataCycleBase.MONTH:
						break;
					case DataCycleBase.QUARTER:
						Num = Num/3;
						break;
					case DataCycleBase.HALFYEAR:
						Num = Num/6;
						break;
				} 
			}
			else 
			{
				if (CycleBase==DataCycleBase.HOUR)
					Num =(int)(D*24/Repeat+0.001);
				else if (CycleBase==DataCycleBase.MINUTE)
					Num =(int)(((int)D % 100 + D- (int)D)*24*60/Repeat+0.001);
				else Num = (int)(((int)D % 100 + D- (int)D)*24*3600/Repeat+0.001);
				return Num;
			}
			return Num /=Repeat;
			//return Num;
		}

		/// <summary>
		/// true if d1 and d2 in same data cycle
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public bool SameSequence(double d1,double d2)
		{
			if (cycleBase == DataCycleBase.TICK)
				return d1==d2;
			else return GetSequence(d1)==GetSequence(d2);
		}

		public bool SameSequence(DateTime d1,DateTime d2)
		{
			return SameSequence(d1.ToOADate(),d2.ToOADate());
		}

		/// <summary>
		/// Tick
		/// </summary>
		/// <returns></returns>
		public static DataCycle Tick
		{
			get
			{
				return new DataCycle(DataCycleBase.TICK,1);
			}
		}

		/// <summary>
		/// 1 Day
		/// </summary>
		/// <returns></returns>
		public static DataCycle Day
		{
			get
			{
				return new DataCycle(DataCycleBase.DAY,1);
			}
		}

		/// <summary>
		/// 1 Week
		/// </summary>
		/// <returns></returns>
		public static DataCycle Week
		{
			get
			{
				return new DataCycle(DataCycleBase.WEEK,1);
			}
		}

		/// <summary>
		/// 1 Month
		/// </summary>
		/// <returns></returns>
		public static DataCycle Month
		{
			get
			{
				return new DataCycle(DataCycleBase.MONTH,1);
			}
		}

		/// <summary>
		/// 1 Quarter
		/// </summary>
		/// <returns></returns>
		public static DataCycle Quarter
		{
			get
			{
				return new DataCycle(DataCycleBase.QUARTER,1);
			}
		}

		/// <summary>
		/// 1 Year
		/// </summary>
		/// <returns></returns>
		public static DataCycle Year
		{
			get
			{
				return new DataCycle(DataCycleBase.YEAR,1);
			}
		}
		
		/// <summary>
		/// 1 Minute
		/// </summary>
		/// <returns></returns>
		public static DataCycle Minute
		{
			get
			{
				return new DataCycle(DataCycleBase.MINUTE,1);
			}
		}

		/// <summary>
		/// 1 Hour
		/// </summary>
		public static DataCycle Hour
		{
			get
			{
				return new DataCycle(DataCycleBase.HOUR,1);
			}
		}

		/// <summary>
		/// Add DateTime and DataCycle
		/// </summary>
		/// <param name="d"></param>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static DateTime operator + (DateTime d,DataCycle dc)
		{
			switch (dc.CycleBase)
			{
				case DataCycleBase.DAY:
					return d.AddDays(dc.Repeat);
				case DataCycleBase.HOUR:
					return d.AddHours(dc.Repeat);
				case DataCycleBase.HALFYEAR:
					return d.AddMonths(dc.Repeat*6);
				case DataCycleBase.MINUTE:
					return d.AddMinutes(dc.Repeat);
				case DataCycleBase.MONTH:
					return d.AddMonths(dc.Repeat);
				case DataCycleBase.QUARTER:
					return d.AddMonths(dc.Repeat*3);
				case DataCycleBase.SECOND:
					return d.AddSeconds(dc.Repeat);
				case DataCycleBase.WEEK:
					return d.AddDays(dc.Repeat*7);
				case DataCycleBase.YEAR:
					return d.AddYears(dc.Repeat);
			}
			return d;
		}

		/// <summary>
		/// Negative DataCycle
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static DataCycle operator - (DataCycle dc) 
		{
			return new DataCycle(dc.cycleBase,-dc.repeat);
		}

		/// <summary>
		/// DateTime substract DataCycle
		/// </summary>
		/// <param name="d"></param>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static DateTime operator - (DateTime d,DataCycle dc)
		{
			return d+(-dc);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// DataCycle string like MONTH1,DAY15 etc.
		/// </returns>
		public override string ToString() 
		{
			return CycleBase.ToString()+Repeat;
		}

		public override bool Equals(object obj)
		{
			if (obj is DataCycle)
				return (obj as DataCycle).cycleBase==cycleBase && (obj as DataCycle).repeat ==repeat;
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Parse DataCycle from string
		/// </summary>
		/// <param name="s">DataCycle string like MONTH1,DAY15 etc.</param>
		/// <returns>DataCycle</returns>
		public static DataCycle Parse(string s)
		{
			try 
			{
				for(int i=0; i<s.Length; i++) 
				{
					if (char.IsDigit(s,i))
					{
						return new DataCycle(
							(DataCycleBase)Enum.Parse(typeof(DataCycleBase),s.Substring(0,i),true),
							int.Parse(s.Substring(i)));

					}
				}
				return new DataCycle((DataCycleBase)Enum.Parse(typeof(DataCycleBase),s,true),1);
			}
			catch 
			{
				return DataCycle.Day;
			}
		}
	}

	/// <summary>
	/// Collection of DataCycle
	/// </summary>
	public class DataCycleCollection:CollectionBase
	{
		public DataCycle this[int index] 
		{
			get 
			{
				return (DataCycle)List[index];
			}
			set 
			{
				List[index] = value;
			}
		}

		public int Add(DataCycle value)
		{
			return List.Add(value);
		}
	}
}

