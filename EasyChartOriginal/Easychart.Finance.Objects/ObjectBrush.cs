using System;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	public enum BrushStyle {Solid,Hatch,Linear};

	/// <summary>
	/// Summary description for ComponentBrush.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ObjectBrush
	{
		private Color color = Color.Black;
		private Color color2;
		private int angle;
		private HatchStyle hatchStyle;
		private BrushStyle brushStyle;
		private Rectangle gradientRect = new Rectangle(0,0,100,100);

		public ObjectBrush()
		{
		}

		public ObjectBrush(Color Color):this()
		{
			this.color = Color;
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

		[XmlElement("Color")]
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

		[XmlIgnore()] 
		public Color Color2
		{
			get
			{
				return color2;
			}
			set
			{
				color2 = value;
			}
		}

		[XmlElement("Color2")]
		[Browsable(false)]
		public string XmlColor2
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(Color2);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				Color2 = (Color)tc.ConvertFromString(value);
			}
		}

		public HatchStyle HatchStyle
		{
			get
			{
				return hatchStyle;
			}
			set
			{
				hatchStyle = value;
			}
		}

		public BrushStyle BrushStyle
		{
			get
			{
				return brushStyle;
			}
			set
			{
				brushStyle = value;
			}
		}

		public int Angle
		{
			get
			{
				return angle;
			}
			set
			{
				angle = value;
			}
		}

		[XmlIgnore()] 
		public Rectangle GradientRect
		{
			get
			{
				return gradientRect;
			}
			set
			{
				gradientRect = value;
			}
		}

		[XmlElement("GradientRect")]
		[Browsable(false)]
		public string XmlGradientRect
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Rectangle));
				return tc.ConvertToString(GradientRect);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Rectangle));
				GradientRect = (Rectangle)tc.ConvertFromString(value);
			}
		}

		public Brush GetBrush()
		{
			switch (BrushStyle)
			{
				case BrushStyle.Hatch:
					return new HatchBrush(hatchStyle,Color,Color2);
				case BrushStyle.Linear:
					return new LinearGradientBrush(gradientRect,Color,Color2,Angle);
				default:
					return new SolidBrush(color);
			}
		}
	}
}
