using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for WeekTrendsObject.
	/// </summary>
	public class WeekTrendsObject : InvisibleObject
	{
		public WeekTrendsObject()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private int longTermTrend;
		[Description("Long Term Trend")]
		public int LongTermTrend
		{
			get
			{
				return longTermTrend;
			}
			set
			{
				longTermTrend = value;
			}
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]
					{
						//new ObjectInit( "Week Trends",typeof(WeekTrendsObject),"","Invisible","Default")
					};
		}

	}
}
