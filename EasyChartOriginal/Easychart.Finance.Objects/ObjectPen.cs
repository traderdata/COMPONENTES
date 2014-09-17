using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Easychart.Finance.Objects
{
	
	/// <summary>
	/// Summary description for ComponentPen.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class ObjectPen
	{
		private Color color = Color.Black;
		private byte alphaBlend = 255;
		private int width = 1;
		private DashStyle dashStyle;
		private ArrowCap startCap;// = new ArrowCap();
		private ArrowCap endCap;// = new ArrowCap();
		private DashCap dashCap;

		public ObjectPen()
		{
		}

		[XmlAttribute]
		public byte AlphaBlend
		{
			get 
			{
				return alphaBlend;
			}
			set 
			{
				alphaBlend = value;
			}
		}

		[XmlAttribute]
		public DashCap DashCap
		{
			get 
			{
				return dashCap;
			}
			set 
			{
				dashCap = value;
			}
		}

		[XmlAttribute]
		public DashStyle DashStyle
		{
			get 
			{
				return dashStyle;
			}
			set 
			{
				dashStyle = value;
			}
		}

		public ArrowCap StartCap
		{
			get 
			{
				return startCap;
			}
			set 
			{
				startCap = value;
			}
		}

		public ArrowCap EndCap
		{
			get 
			{
				return endCap;
			}
			set 
			{
				endCap = value;
			}
		}

		[XmlIgnore()] 
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		[XmlAttribute(AttributeName="Color")]
		[Browsable(false)]
		public string XmlColor
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(Color);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				Color = (Color)tc.ConvertFromString(value);
			}
		}

		[XmlAttribute]
		public int Width
		{
			get
			{
				return width;
			}
			set 
			{
				width = value;
			}
		}

		public Pen GetPen()
		{
			Pen p = new Pen(Color.FromArgb(AlphaBlend,color),width);
			p.DashStyle = DashStyle;
			p.DashCap = DashCap;

			if (startCap!=null && startCap.Width!=0 && startCap.Height!=0)
				p.CustomStartCap = new AdjustableArrowCap(startCap.Width,startCap.Height, startCap.Filled);
			if (endCap!=null && endCap.Width!=0 && endCap.Height!=0)
				p.CustomEndCap = new AdjustableArrowCap(endCap.Width,endCap.Height, endCap.Filled);
			return p;
		}

		public override string ToString()
		{
			return color.Name+","+width;
		}
	}
}