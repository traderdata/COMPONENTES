using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for ObjectPoint.
	/// </summary>
	//[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct ObjectPoint
	{
		private double x;
		private double y;

		[XmlAttribute]
		public double X
		{
			get
			{
				return x;
			}
			set 
			{
				x = value;
			}
		}

		[XmlAttribute]
		public double Y
		{
			get 
			{
				return y;
			}
			set 
			{
				y = value;
			}
		}

		public override string ToString()
		{
			return "{"+DateTime.FromOADate(x).ToString("yyyy-MM-dd",DateTimeFormatInfo.InvariantInfo)+","+y.ToString("f3")+"}";
		}
		
		public ObjectPoint(double x,double y)
		{
			this.x = x;
			this.y = y;
		}

		public static ObjectPoint Empty
		{
			get
			{
				ObjectPoint op = new ObjectPoint();
				op.X= double.NaN;
				op.Y= double.NaN;
				return op;
			}
		}
	}
}