using System;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for BaseTextObject.
	/// </summary>
	public class LineGroupTextObject : LineGroupObject
	{

		string textFormat = "{0:f2} {1:p1}";
		/// <summary>
		/// Text format for text on the line
		/// </summary>
		[DefaultValue("{0:f2} {1:p1}")]
		public string TextFormat
		{
			get
			{
				return textFormat;
			}
			set
			{
				textFormat = value; 
			}
		}

		/// <summary>
		/// Help the user input TextFormat in property grid
		/// </summary>
		[XmlIgnore]
		public string[] FormatLines
		{
			get
			{
				string[] ss = textFormat.Split('\r');
				for(int i=0; i<ss.Length; i++)
					ss[i] = ss[i].Trim();
				return ss;
			}
			set
			{
				textFormat = string.Join("\r\n",value);
			}
		}


		private FontMapper objectFont;
		public FontMapper ObjectFont
		{
			get
			{
				return objectFont;
			}
			set
			{
				objectFont = value;
			}
		}

		private BrushMapper textBackBrush = new BrushMapper(BrushMapperStyle.Empty);
		public BrushMapper TextBackBrush
		{
			get
			{
				return textBackBrush;
			}
			set
			{
				textBackBrush = value;
			}
		}

		public bool ShouldSerializeTextBackBrush()
		{
			return textBackBrush.BrushStyle!=BrushMapperStyle.Empty;
		}

		private PenMapper textFramePen = new PenMapper(Color.Black,1,0);
		public PenMapper TextFramePen
		{
			get
			{
				return textFramePen;
			}
			set
			{
				textFramePen = value;
			}
		}

		public bool ShouldSerializeTextFramePen()
		{
			return textFramePen.Alpha!=0;
		}

		public LineGroupTextObject()
		{
			objectFont = new FontMapper();
		}

		public virtual RectangleF GetTextRect()
		{
			return RectangleF.Empty;
		}

		public virtual RectangleF GetTextRect(int LineNumber,bool Horizon)
		{
			if (CurrentGraphics!=null) 
			{
				SizeF sf = ObjectFont.Measure(CurrentGraphics,GetStr());
				RectangleF R = RectangleF.Empty;
				if (Horizon)
					R =new RectangleF(
						pfStart[LineNumber].X,pfStart[LineNumber].Y-sf.Height,
						pfEnd[LineNumber].X-pfStart[LineNumber].X,sf.Height*2);
				else 
					R =new RectangleF(
						pfStart[LineNumber].X-sf.Width,pfStart[LineNumber].Y,
						sf.Width*2,pfEnd[LineNumber].Y-pfStart[LineNumber].Y);
				if (R.Width<0) 
				{
					R.X +=R.Width;
					R.Width = -R.Width;;
				}
				if (R.Height<0) 
				{
					R.Y +=R.Height;
					R.Height = -R.Height;
				}
				R.Width = Math.Max(R.Width,sf.Width);
				R.Height = Math.Max(R.Height,sf.Height);

				return R;
			}
			return RectangleF.Empty;
		}

		public virtual string GetStr()
		{
			return null;
		}

		public override Region GetRegion()
		{
			Region  R = base.GetRegion ();
			if (CurrentGraphics!=null) 
			{
				RectangleF Rect = GetTextRect();
				Rect.Inflate(1,1);
				R.Union(Rect);
			}
			return R;
		}

		public override void Draw(Graphics g)
		{
			base.Draw (g);

			string s = GetStr();
			if (s!=null) 
			{
				s = s.Trim();
				RectangleF TextRect = GetTextRect();
				RectangleF R = ObjectFont.GetTextBackRect(s,g,TextRect);
				R.Height--;
				if (ObjectFont.LineAlignment==StringAlignment.Near)
					R.Y++;

				if (textBackBrush!=null)
				{
					if (textBackBrush.BrushStyle != BrushMapperStyle.Empty) 
					{
						Brush B = textBackBrush.GetBrush(R);
						g.FillRectangle(B,R);
					}
				}
				if (textFramePen!=null)
				{
					if (textFramePen.Alpha>0)
					{
						Pen P = textFramePen.GetPen();
						g.DrawRectangles(P,new RectangleF[]{R});
					}
				}
				ObjectFont.DrawString(s,g,GetTextRect());
			}
		}
	}
}