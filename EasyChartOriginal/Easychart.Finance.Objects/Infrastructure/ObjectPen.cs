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
	[Editor(typeof(Easychart.Finance.Objects.ObjectPenEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class ObjectPen
	{
		private Color color = Color.Black;
		private byte alpha = 255;
		private int width = 1;
		private DashStyle dashStyle;
		private ArrowCap startCap;// = new ArrowCap();
		private ArrowCap endCap;// = new ArrowCap();
		private DashCap dashCap;
		private static ObjectPen defaultPen = new ObjectPen();

		public ObjectPen()
		{
		}

		public static bool NotDefault(ObjectPen op)
		{
			return defaultPen.Alpha!=op.Alpha || defaultPen.Color!=op.Color || defaultPen.DashCap!=op.DashCap ||
				defaultPen.DashStyle!=op.DashStyle || defaultPen.EndCap!=op.EndCap || defaultPen.StartCap!=op.StartCap ||
				defaultPen.Width!=op.Width;
		}

		[XmlAttribute]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(DashCap.Flat)]
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
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(DashStyle.Solid)]
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

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(255),XmlAttribute]
		public byte Alpha
		{
			get 
			{
				return alpha;
			}
			set 
			{
				alpha = value;
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
				color = value; //Color.FromArgb(color.A,value);
			}
		}

		[XmlAttribute(AttributeName="Color")]
		[Browsable(false)]
		[DefaultValue("Black")]
		public string XmlColor
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(null,ObjectHelper.enUS,color);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				color = (Color)tc.ConvertFromString(null,ObjectHelper.enUS,value);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(1),XmlAttribute]
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
			Pen p = new Pen(Color.FromArgb(alpha,color),width);
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