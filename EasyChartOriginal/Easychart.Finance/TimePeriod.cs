using System;
using System.Collections;
using System.ComponentModel;

namespace Easychart.Finance
{
	/// <summary>
	/// This struct describe a time period
	/// </summary>
	[TypeConverter(typeof(TimePeriodConverter))]//ExpandableObjectConverter))]
	public struct TimePeriod
	{
		/// <summary>
		/// Start time , Ole Date
		/// </summary>
		public double Time1;
		/// <summary>
		/// End time ,  Ole Date
		/// </summary>
		public double Time2;
		/// <summary>
		/// Create the TimePeriod by two time
		/// </summary>
		/// <param name="t1">Start Time</param>
		/// <param name="t2">End time</param>
		public TimePeriod(double t1,double t2) 
		{
			Time1 = t1-(int)t1;
			Time2 = t2-(int)t2;
		}

		public TimePeriod(DateTime t1,DateTime t2) 
			:this(t1.ToOADate(),t2.ToOADate())
		{
		}

		public override string ToString()
		{
			return DateTime.FromOADate(Time1).ToString("HH:mm")+" - "+DateTime.FromOADate(Time2).ToString("HH:mm");
		}
	}

	public class TimePeriodConverter:TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType==typeof(string))
				return true;
			return base.CanConvertFrom (context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType==typeof(string))
				return true;
			return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string)
			{
				string s = value as string;
				int i=s.IndexOf('-');
				if (i>=0)
				{
					string s1 = s.Substring(0,i)+":00";
					string s2 = s.Substring(i+1)+":00";
					return new TimePeriod(DateTime.Parse(s1),DateTime.Parse(s2));
				}
				return new TimePeriod(0,0);
			}
			return base.ConvertFrom (context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType==typeof(string) && value is TimePeriod)
				return ((TimePeriod)value).ToString();
			return base.ConvertTo (context, culture, value, destinationType);
		}
	}

	public class TimePeriodsConverter:ArrayConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType==typeof(string))
				return true;
			return base.CanConvertFrom (context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType==typeof(string))
				return true;
			return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string)
			{
				string s = value as string;
				string[] ss = s.Split(',');
				if (ss.Length>0)
				{
					TimePeriod[] tps = new TimePeriod[ss.Length];
					TypeConverter tc = TypeDescriptor.GetConverter(typeof(TimePeriod));
					for(int i=0; i<ss.Length; i++)
						tps[i] = (TimePeriod)tc.ConvertFromString(ss[i]);
					return tps;
				} 
			}
			return base.ConvertFrom (context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType==typeof(string) && value is TimePeriod[]) 
			{
				string s = "";
				foreach(TimePeriod tp in (value as TimePeriod[]))
				{
					if (s!="")
						s +=",";
					s +=tp.ToString();
				}
				return s;
			}
			return base.ConvertTo (context, culture, value, destinationType);
		}


	}
}