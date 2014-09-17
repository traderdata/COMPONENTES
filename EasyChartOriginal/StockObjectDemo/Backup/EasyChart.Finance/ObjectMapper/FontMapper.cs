using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for ObjectFont.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Editor(typeof(Easychart.Finance.FontMapperEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class FontMapper
	{
		private BrushMapper textBrush = new BrushMapper();
		private StringAlignment alignment = StringAlignment.Center;
		private StringAlignment lineAlignment = StringAlignment.Center;
		private Font textFont;

		public FontMapper():this("Verdana",10)
		{
		}

		public FontMapper(string familyName,float emSize)
		{
			textFont = new Font(familyName,emSize);
		}

		public FontMapper Clone()
		{
			FontMapper fm = new FontMapper(textFont.FontFamily.Name,textFont.Size);
			fm.textBrush = textBrush.Clone();
			fm.alignment = alignment;
			fm.lineAlignment = lineAlignment;
			return fm;
		}


		public bool ShouldSerializeTextBrush()
		{
			return BrushMapper.NotDefault(TextBrush);
		}

		/// <summary>
		/// Text brush of the label text
		/// </summary>
		public BrushMapper TextBrush
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

		[XmlElement("TextFont"), Browsable(false), DefaultValue("Verdana, 10pt")]
		public string XmlTextFont
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				return tc.ConvertToString(null,FormulaHelper.enUS,TextFont);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				TextFont = (Font)tc.ConvertFromString(null,FormulaHelper.enUS,value);
			}
		}

		public SizeF Measure(Graphics g,string s)
		{
			return Measure(g,s,1000);
		}

		public SizeF Measure(Graphics g,string s,int w)
		{
			if (g!=null) 
				return g.MeasureString(s,textFont,w,GetStringFormat());
			return SizeF.Empty;
		}

		private StringFormat GetStringFormat()
		{
			StringFormat sf = new StringFormat();
			sf.Alignment = alignment;
			sf.LineAlignment = lineAlignment;
			return sf;
		}

		public RectangleF GetTextBackRect(string s,Graphics g,RectangleF Rect)
		{
			if (g!=null)
			{
				SizeF sf = g.MeasureString(s,textFont,Rect.Size,GetStringFormat());
				RectangleF R = new RectangleF(Rect.Location,sf);
				if (alignment==StringAlignment.Center)
					R.X += (Rect.Width-R.Width)/2;
				else if (alignment==StringAlignment.Far)
					R.X +=Rect.Width-R.Width;

				if (lineAlignment==StringAlignment.Center)
					R.Y +=(Rect.Height-R.Height)/2;
				else if (lineAlignment==StringAlignment.Far)
					R.Y +=Rect.Height-R.Height;
				return R;
			}
			return RectangleF.Empty;
		}

		public void DrawString(string s,Graphics g,RectangleF Rect)
		{
			g.DrawString(s,textFont,textBrush.GetBrush(Rect),Rect,GetStringFormat());
		}

		public override string ToString()
		{
			TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
			return tc.ConvertToString(TextFont);
		}
	}
}
