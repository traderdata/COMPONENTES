using System;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	public enum BrushStyle {Solid,Hatch,Linear,Empty};

	/// <summary>
	/// Summary description for ComponentBrush.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Editor(typeof(Easychart.Finance.Objects.ObjectBrushEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class ObjectBrush
	{
		private Color color = Color.Black;
		private Color color2;
		private byte alpha = 255;
		private byte alpha2 = 0;
		private int angle;
		private HatchStyle hatchStyle;
		private BrushStyle brushStyle;
		private static ObjectBrush defaultBrush = new ObjectBrush();

		public ObjectBrush()
		{
		}

		public static bool NotDefault(ObjectBrush ob)
		{
			return ob.Color!=defaultBrush.Color || ob.Color2!=defaultBrush.Color2 ||
				ob.Alpha!=defaultBrush.Alpha || ob.Alpha2!=defaultBrush.Alpha2 || ob.Angle!=defaultBrush.Angle ||
				ob.HatchStyle!=defaultBrush.HatchStyle || ob.BrushStyle!=defaultBrush.BrushStyle;
		}

//		public ObjectBrush(BrushStyle brushStyle):this()
//		{
//			this.brushStyle = brushStyle;
//		}
//
//		public ObjectBrush(Color color):this(BrushStyle.Solid)
//		{
//			this.color = color;
//		}
//
//		public ObjectBrush(Color color,byte alpha):this(color)
//		{
//			this.alpha = alpha;
//		}

		/// <summary>
		/// Alpha blend for color
		/// </summary>
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

		/// <summary>
		/// The first color for this brush
		/// </summary>
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

		/// <summary>
		/// Xml converter for Color
		/// </summary>
		[XmlAttribute(AttributeName="Color")]
		[Browsable(false)]
		[DefaultValue("Black")]
		public string XmlColor
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(null,ObjectHelper.enUS,Color);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				color = (Color)tc.ConvertFromString(null,ObjectHelper.enUS,value);
			}
		}

		/// <summary>
		/// Alpha blend for color2
		/// </summary>
		[XmlIgnore]
		[RefreshProperties(RefreshProperties.All),XmlAttribute]
		public byte Alpha2
		{
			get 
			{
				return alpha2;
			}
			set 
			{
				alpha2 = value;
			}
		}

		/// <summary>
		/// The second color for this brush
		/// </summary>
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

		/// <summary>
		/// Xml converter for Color2
		/// </summary>
		[XmlAttribute(AttributeName="Color2")]
		[Browsable(false)]
		[DefaultValue("")]
		public string XmlColor2
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(null,ObjectHelper.enUS,Color2);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				color2 = (Color)tc.ConvertFromString(null,ObjectHelper.enUS,value);
			}
		}

		/// <summary>
		/// Hatch style type
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		[XmlAttribute]
		[DefaultValue(HatchStyle.Horizontal)]
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

		/// <summary>
		/// Solid,Hatch,Linear,Empty
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		[XmlAttribute]
		[DefaultValue(BrushStyle.Solid)]
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

		/// <summary>
		/// Angle for the brushes
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		[XmlAttribute]
		[DefaultValue(0)]
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

		/// <summary>
		/// Get GDI+ brushes
		/// </summary>
		/// <param name="R"></param>
		/// <returns></returns>
		public Brush GetBrush(RectangleF R)
		{
			switch (BrushStyle)
			{
				case BrushStyle.Hatch:
					return new HatchBrush(hatchStyle,Color.FromArgb(alpha,color),Color.FromArgb(alpha2,color2));
				case BrushStyle.Linear:
					LinearGradientBrush lgr = new LinearGradientBrush(R,Color.FromArgb(alpha,color),
						Color.FromArgb(alpha2,color2),angle,false); 
					return lgr;
				case BrushStyle.Empty:
					return new SolidBrush(Color.Empty);
				default:
					return new SolidBrush(Color.FromArgb(alpha,color));
			}
		}

		/// <summary>
		/// Get GDI+ brushes
		/// </summary>
		/// <returns></returns>
		public Brush GetBrush()
		{
			return GetBrush(new RectangleF(0,0,640,480));
		}

		/// <summary>
		/// Get object brush name
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return BrushStyle.ToString();
		}

	}
}
