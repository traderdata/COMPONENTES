using System;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectVLine.
	/// </summary>
	[Serializable]
	public class ObjectVLine : BaseObject
	{
		bool showDate = true;
		string dateFormat = "yyyy-MM-dd dddd";
		Font font = new Font("verdana",10);
		Color foreColor = Color.Black;

		public ObjectVLine() :base()
		{
		}

		public override int ControlPointNum
		{
			get 
			{
				return 1;
			}
		}

		public bool ShowDate
		{
			get
			{
				return showDate;
			}
			set
			{
				showDate = value;
			}
		}

		[XmlIgnore()] 
		public Color ForeColor 
		{
			get
			{
				return foreColor;
			}
			set
			{
				foreColor = value;
			}
		}

		[XmlElement("Color")]
		[Browsable(false)]
		public string XmlForeColor 
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(ForeColor);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				ForeColor = (Color)tc.ConvertFromString(value);
			}
		}

		[XmlIgnore()] 
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
			}
		}

		[XmlElement("Font")]
		[Browsable(false)]
		public string XmlFont
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				return tc.ConvertToString(Font);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				Font = (Font)tc.ConvertFromString(value);
			}
		}

		public string DateFormat
		{
			get
			{
				return dateFormat;
			}
			set 
			{
				dateFormat = value;
			}
		}

		public override bool InObject(int X, int Y)
		{
			return Math.Abs(X-ToPointF(ControlPoints[0]).X)<2;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			float X = ToPointF(ControlPoints[0]).X;
			float Y = ToPointF(ControlPoints[0]).Y;

			if (Manager.Canvas!=null && Manager.Canvas.BackChart!=null && Manager.Canvas.BackChart.DataProvider!=null)
			{
				Rectangle R = Manager.Canvas.BackChart.Rect;
				g.DrawLine(LinePen.GetPen(), X,R.Top ,X, R.Bottom);
				if (ShowDate)
				{
					IDataProvider idp = Manager.Canvas.BackChart.DataProvider;
					double d = idp["DATE"][(int)((X-Manager.Canvas.BackChart.Start) / Manager.Canvas.BackChart.ColumnWidth)];
					g.DrawString(DateTime.FromOADate(d).ToString(DateFormat),Font, 
						new SolidBrush(Color.FromArgb(LinePen.AlphaBlend,ForeColor)),X,Y);
				}
			}
		}
	}
}
