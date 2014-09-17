using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for DayParameter.
	/// </summary>
	public class DayTrendsObject : InvisibleObject
	{
		public DayTrendsObject()
		{
		}

		private int shortTermTrend;
		[Description("Short Term Trend")]
		public int ShortTermTrend
		{
			get
			{
				return shortTermTrend;
			}
			set
			{
				shortTermTrend = value;
			}
		}

		private int middleTermTrend;
		[Description("Middle Term Trend")]
		public int MiddleTermTrend
		{
			get
			{
				return middleTermTrend;
			}
			set
			{
				middleTermTrend = value;
			}
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]
					{
						//new ObjectInit( "Day Trends",typeof(DayTrendsObject),"","Invisible","Default",1100)
					};
		}
	}
}