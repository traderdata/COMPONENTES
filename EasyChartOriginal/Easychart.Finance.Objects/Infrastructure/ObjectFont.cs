using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectFont.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Editor(typeof(Easychart.Finance.Objects.ObjectFontEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class ObjectFont
	{
		private ObjectBrush textBrush = new ObjectBrush();
		private StringAlignment alignment = StringAlignment.Center;
		private StringAlignment lineAlignment = StringAlignment.Center;
		private Font textFont = new Font("Verdana",10);

		public bool ShouldSerializeTextBrush()
		{
			return ObjectBrush.NotDefault(TextBrush);
		}

		/// <summary>
		/// Text brush of the label text
		/// </summary>
		public ObjectBrush TextBrush
		{
			get
			{
				return textBrush;
			}
			set
			{
				textBrush = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(StringAlignment.Center),XmlAttribute]
		public  StringAlignment Alignment
		{
			get
			{
				return alignment;
			}
			set
			{
				alignment = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(StringAlignment.Center),XmlAttribute]
		public StringAlignment LineAlignment
		{
			get
			{
				return lineAlignment;
			}
			set
			{
				lineAlignment = value;
			}
		}

		/// <summary>
		/// Text Font
		/// </summary>
		[XmlIgnore()] 
		public Font TextFont
		{
			get
			{
				return textFont;
			}
			set
			{
				textFont = value;
			}
		}

		[XmlElement("TextFont")]
		[Browsable(false)]
		[DefaultValue("Verdana, 10pt")]
		public string XmlTextFont
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				return tc.ConvertToString(null,ObjectHelper.enUS,TextFont);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				TextFont = (Font)tc.ConvertFromString(null,ObjectHelper.enUS,value);
			}
		}

		public ObjectFont()
		{
		}

		public SizeF Measure(Graphics g,string s)
		{
			return Measure(g,s,1000);
		}

		public SizeF Measure(Graphics g,string s,int w)
		{
			if (g!=null) 
			{
				StringFormat sf = new StringFormat();
				sf.Alignment = alignment;
				sf.LineAlignment = lineAlignment;
				return g.MeasureString(s,textFont,w,sf);
			}
			return SizeF.Empty;
		}

		public void DrawString(string s,Graphics g,RectangleF Rect)
		{
			StringFormat sf = new StringFormat();
			sf.Alignment = alignment;
			sf.LineAlignment = lineAlignment;
			g.DrawString(s,textFont,textBrush.GetBrush(),Rect,sf);
		}

		public override string ToString()
		{
			TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
			return tc.ConvertToString(TextFont);
		}
	}
}
