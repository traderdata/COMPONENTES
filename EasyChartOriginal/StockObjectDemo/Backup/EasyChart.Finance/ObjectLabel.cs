using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Easychart.Finance
{
	public enum StickAlignment {LeftTop,LeftCenter,LeftBottom,CenterTop,CenterCenter,CenterBottom,RightTop,RightCenter,RightBottom};
	/// <summary>
	/// Summary description for ObjectLabel.
	/// </summary>
	public class ObjectLabel
	{
		/// <summary>
		/// Border color of this label
		/// </summary>
		public Color BorderColor;
		/// <summary>
		/// Background color of this label
		/// </summary>
		public Color BackColor;
		/// <summary>
		/// Text brush of the label text
		/// </summary>
		public Brush TextBrush = Brushes.Black;
		/// <summary>
		/// Stick height of this label
		/// </summary>
		public int StickHeight = 6;

		public StickAlignment StickAlignment;

		public StringFormat format = new StringFormat(StringFormat.GenericDefault);
		public Font TextFont = new Font("Verdana",8);
		public string Text;
		public int Left;
		public int Top;
		public int RoundWidth = 4;
		public int ShadowWidth = 2;

		public ObjectLabel()
		{
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

		public void Draw(Graphics g)
		{
			SizeF sf = g.MeasureString(Text,TextFont,1000,format);
			RectangleF Rect = new RectangleF(Left,Top,sf.Width+4,sf.Height+4);
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
			PointF P = new PointF(Left,Top);

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

			if (BackColor!=Color.Empty)
				g.FillPolygon(new SolidBrush(BackColor),ps);

			if (BorderColor!=Color.Empty)
				g.DrawLines(new Pen(BorderColor),ps);

			TextRenderingHint trh = g.TextRenderingHint;
			g.TextRenderingHint = TextRenderingHint.AntiAlias;
			g.DrawString(Text,TextFont,TextBrush,Rect,format);
			g.TextRenderingHint  = trh;
		}
	}
}