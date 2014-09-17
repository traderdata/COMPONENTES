using System;
using System.Drawing;
using System.ComponentModel;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectPoint.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct ObjectPoint
	{
		double dataIndex;
		double price;

		public double DataIndex
		{
			get
			{
				return dataIndex;
			}
			set 
			{
				dataIndex = value;
			}
		}

		public double Price
		{
			get 
			{
				return price;
			}
			set 
			{
				price = value;
			}
		}

		public override string ToString()
		{
			return "{"+dataIndex+","+price+"}";
		}

		public PointF ToPointF()
		{
			return new PointF((float)dataIndex,(float)price);
		}
	}
}
