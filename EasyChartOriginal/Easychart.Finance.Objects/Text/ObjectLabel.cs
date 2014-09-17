using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Easychart.Finance.Objects
{
	public enum StickAlignment {LeftTop,LeftCenter,LeftBottom,CenterTop,CenterCenter,CenterBottom,RightTop,RightCenter,RightBottom};
	/// <summary>
	/// Summary description for ObjectLabel.
	/// </summary>
	public class ObjectLabel:BaseObject
	{
		private ObjectBrush backBrush = new ObjectBrush(Color.Yellow);
		private ObjectBrush textBrush = new ObjectBrush();
		private int stickHeight = 6;
		private StickAlignment stickAlignment;
		private StringFormat format;
		private Font textFont = new Font("Verdana",12);
		private string text = "Label";
		private int roundWidth = 4;
		private int shadowWidth = 2;
		private bool autoSize = true;

		/// <summary>
		/// Background color of this label
		/// </summary>
		public ObjectBrush BackBrush 
		{
			get
			{
				return backBrush;
			}
			set
			{
				backBrush = value;
			}
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

		/// <summary>
		/// Stick height of this label
		/// </summary>
		public int StickHeight
		{
			get
			{
				return stickHeight;
			}
			set
			{
				stickHeight = value;
			}
		}

		public StickAlignment StickAlignment
		{
			get
			{
				return stickAlignment;
			}
			set
			{
				stickAlignment = value;
			}
		}

		/// <summary>
		/// String Format
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public StringFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
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
		public string XmlTextFont
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				return tc.ConvertToString(TextFont);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				TextFont = (Font)tc.ConvertFromString(value);
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public string[] Lines
		{
			get
			{
				string[] ss = text.Split('\r');
				for(int i=0; i<ss.Length; i++)
					ss[i] = ss[i].Trim();
				return ss;
			}
			set
			{
				text = string.Join("\r\n",value);
			}
		}

		public int RoundWidth
		{
			get
			{
				return roundWidth;
			}
			set
			{
				roundWidth = value;
			}
		}

		public int ShadowWidth
		{
			get
			{
				return shadowWidth;
			}
			set
			{
				shadowWidth = value;
			}
		}

		public bool AutoSize
		{
			get
			{
				return autoSize;
			}
			set
			{
				autoSize = value;
			}
		}

		public ObjectLabel()
		{
			format = new StringFormat(StringFormat.GenericDefault);
			format.Alignment  = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
		}

		public void OffsetPoint(PointF[] pfs,float OffsetX,float OffsetY)
		{
			for(int i=0; i<pfs.Length; i++) 
			{
				pfs[i].X += OffsetX;
				pfs[i].Y += OffsetY;
			}
		}

		public ArrayList OffsetPoint(ArrayList al,float OffsetX,float OffsetY)
		{
			PointF[] pfs = (PointF[])al.ToArray(typeof(PointF));
			OffsetPoint(pfs,OffsetX,OffsetY);
			ArrayList alRst = new ArrayList();
			alRst.AddRange(pfs);
			return alRst;
		}

		public override bool InObject(int X, int Y)
		{
			return GetMaxRect().Contains(X, Y);
		}

		public override int ControlPointNum
		{
			get
			{
				return 1;
			}
		}

		public override int InitNum
		{
			get
			{
				return 1;
			}
		}

		public override RectangleF GetMaxRect()
		{
			PointF P = ToPointF(ControlPoints[0]);
			Graphics g = CurrentGraphics;
			RectangleF Rect = RectangleF.Empty;
			if (g!=null)
			{
				SizeF sf = g.MeasureString(Text,TextFont,1000,format);
				Rect = new RectangleF(P.X-5,P.Y-5,
					sf.Width+4+StickHeight+ShadowWidth+8,
					sf.Height+4+StickHeight+ShadowWidth+5);
			}
			return Rect;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			PointF P = ToPointF(ControlPoints[0]);
			SizeF sf = g.MeasureString(Text,TextFont,1000,format);
			RectangleF Rect = new RectangleF(P.X,P.Y,sf.Width+4,sf.Height+4);
			ArrayList al = new ArrayList();
			al.Add(new PointF(Rect.Left,Rect.Top+RoundWidth));
			al.Add(new PointF(Rect.Left+RoundWidth,Rect.Top));

			al.Add(new PointF(Rect.Right-RoundWidth,Rect.Top));
			al.Add(new PointF(Rect.Right,Rect.Top+RoundWidth));

			al.Add(new PointF(Rect.Right,Rect.Bottom-RoundWidth));
			al.Add(new PointF(Rect.Right-RoundWidth,Rect.Bottom));

			al.Add(new PointF(Rect.Left+RoundWidth,Rect.Bottom));
			al.Add(new PointF(Rect.Left,Rect.Bottom-RoundWidth));
			al.Add(al[0]);

			if (StickHeight>0)
			{
				float OffsetX = 0;
				float OffsetY = 0;
				int Index = 1;
				switch (StickAlignment)
				{
					case StickAlignment.LeftTop:
						OffsetX =StickHeight;
						OffsetY =StickHeight;
						break;
					case StickAlignment.LeftCenter:
						break;
					case StickAlignment.LeftBottom:
						OffsetX =StickHeight;
						OffsetY =-Rect.Height-StickHeight;
						Index = 7;
						break;
					case StickAlignment.CenterTop:
						break;
					case StickAlignment.CenterCenter:
						break;
					case StickAlignment.CenterBottom:
						break;
					case StickAlignment.RightTop:
						OffsetX=-Rect.Width-StickHeight;
						OffsetY=StickHeight;
						Index = 3;
						break;
					case StickAlignment.RightCenter:
						break;
					case StickAlignment.RightBottom:
						OffsetX =-Rect.Width-StickHeight;
						OffsetY =-Rect.Height-StickHeight;
						Index = 5;
						break;
				}
				al = OffsetPoint(al,OffsetX,OffsetY);
				al.Insert(Index,P);
				Rect.Offset(OffsetX,OffsetY);
			}

			PointF[] ps = (PointF[])al.ToArray(typeof(PointF));
			if (ShadowWidth>0)
			{
				PointF[] pss = (PointF[])ps.Clone();
				OffsetPoint(pss,ShadowWidth,ShadowWidth);
				g.FillPolygon(new SolidBrush(Color.FromArgb(64,Color.Black)),pss);
			}

			if (BackBrush.Color!=Color.Empty)
				g.FillPolygon(BackBrush.GetBrush(GetMaxRect()),ps);

			if (LinePen.Color!=Color.Empty)
				g.DrawLines(LinePen.GetPen(),ps);

			TextRenderingHint trh = g.TextRenderingHint;
			g.TextRenderingHint = TextRenderingHint.AntiAlias;
			g.DrawString(Text,TextFont,TextBrush.GetBrush(),Rect,format);
			g.TextRenderingHint  = trh;
		}
	}
}