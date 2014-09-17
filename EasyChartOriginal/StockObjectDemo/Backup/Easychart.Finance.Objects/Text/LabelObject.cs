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
	public class LabelObject:BaseObject
	{
		private BrushMapper backBrush = new BrushMapper();
		private int stickHeight;
		private int roundWidth;
		private int shadowWidth;
		private StickAlignment stickAlignment;
		private string NewText;
		private RectangleF Rect;
		
		private FontMapper labelFont = new FontMapper();

		private string text = "Label";

		public LabelObject()
		{
		}

		public void InitLabel()
		{
			stickHeight = 6;
			shadowWidth = 2;
			roundWidth = 2;
			backBrush.Color = Color.Yellow;
		}

		public void InitPriceLabel()
		{
			InitLabel();
			LabelFont.Alignment = StringAlignment.Near;
			text = "Date = {D}\r\nOpen = {O}\r\nHigh = {H}\r\nLow = {L}\r\nClose = {0}";
		}

		public void InitPriceDateLabel()
		{
			InitLabel();
			text = "Label\r\n{0}\r\n{D}";
		}

		public void InitTransparentText()
		{
			stickHeight = 0;
			shadowWidth = 0;
			roundWidth = 0;
			backBrush.BrushStyle = BrushMapperStyle.Empty;
			LinePen.Alpha = 0;
			LabelFont.TextBrush.Alpha = 100;
			LabelFont.TextFont = new Font(LabelFont.TextFont.FontFamily,40);
		}

		/// <summary>
		/// Background color of this label
		/// </summary>
		public BrushMapper BackBrush 
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
		/// Stick height of this label
		/// </summary>
		[DefaultValue(0)]
		[XmlAttribute]
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

		[XmlAttribute]
		[DefaultValue(StickAlignment.LeftTop)]
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
		/// Text Font
		/// </summary>
		public FontMapper LabelFont
		{
			get
			{
				return labelFont;
			}
			set
			{
				labelFont = value;
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

		[XmlIgnore]
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

		[XmlAttribute]
		[DefaultValue(0)]
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

		[XmlAttribute]
		[DefaultValue(0)]
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

		public PointF[] ToPoints()
		{
			ArrayList al = new ArrayList();
			if (CurrentGraphics!=null)
			{
				NewText = ReplaceTag(ControlPoints[0],Text);
				SizeF sf = LabelFont.Measure(CurrentGraphics,NewText);
				PointF P = ToPointF(ControlPoints[0]);
				Rect = new RectangleF(P.X,P.Y,sf.Width+4,sf.Height+4);
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
			}
			return (PointF[])al.ToArray(typeof(PointF));
		}

		public override RectangleF GetMaxRect()
		{
			return base.GetMaxRect (ToPoints());
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			PointF[] ps = ToPoints();
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

			LabelFont.DrawString(NewText,g,Rect);
			g.TextRenderingHint  = trh;
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
						new ObjectInit("Label",typeof(LabelObject),"InitLabel","Text","TextL",800),
						new ObjectInit("Price Label",typeof(LabelObject),"InitPriceLabel","Text","TextLP"),
						new ObjectInit("Price Date Label",typeof(LabelObject),"InitPriceDateLabel","Text","TextLPD"),
						new ObjectInit("Transparent Text",typeof(LabelObject),"InitTransparentText","Text","Text"),
				};
		}
	}
}