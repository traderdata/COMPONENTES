using System;
using System.Drawing;
using System.Collections;

namespace Easychart.Finance
{
	/// <summary>
	/// Formula Label 
	/// </summary>
	public class FormulaLabel:ICloneable
	{
		/// <summary>
		/// Border color of this label
		/// </summary>
		public Color BorderColor;
		/// <summary>
		/// Background color of this label
		/// </summary>
		public Color BGColor;
		/// <summary>
		/// Text brush of the label text
		/// </summary>
		public Brush TextBrush;
		/// <summary>
		/// Stick height of this label
		/// </summary>
		public int StickHeight = 6;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="BGColor"></param>
		public FormulaLabel(Color BorderColor,Color BGColor):this(BorderColor,BGColor,new SolidBrush(Color.White))
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="BGColor"></param>
		/// <param name="TextBrush"></param>
		public FormulaLabel(Color BorderColor,Color BGColor,Brush TextBrush)
		{
			this.BorderColor = BorderColor;
			this.BGColor = BGColor;
			this.TextBrush = TextBrush;
		}

		/// <summary>
		/// Draw string using current label
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="Text"></param>
		/// <param name="TextFont"></param>
		/// <param name="TextBrush"></param>
		/// <param name="VAlign"></param>
		/// <param name="Align"></param>
		/// <param name="Pos"></param>
		/// <param name="ShowArrow"></param>
		public void DrawString(Graphics g,string Text,Font TextFont,Brush TextBrush,VerticalAlign VAlign,FormulaAlign Align,PointF Pos,bool ShowArrow)
		{
			DrawString(g,Text,TextFont,TextBrush,VAlign,Align,new RectangleF(Pos,Size.Empty),ShowArrow);
		}

		/// <summary>
		/// Draw string using current label
		/// </summary>
		/// <param name="g"></param>
		/// <param name="Text"></param>
		/// <param name="TextFont"></param>
		/// <param name="TextBrush"></param>
		/// <param name="VAlign"></param>
		/// <param name="Align"></param>
		/// <param name="Rect"></param>
		/// <param name="ShowArrow"></param>
		public void DrawString(Graphics g,string Text,Font TextFont,Brush TextBrush,VerticalAlign VAlign,FormulaAlign Align,RectangleF Rect,bool ShowArrow)
		{
			if (double.IsNaN(Rect.Y) || double.IsInfinity(Rect.Y)) return;
			PointF Pos = Rect.Location;
			SizeF sf = Rect.Size;
			SizeF sfText = g.MeasureString(Text,TextFont);
			if (sf.Width==0)
				sf.Width = sfText.Width;
			if (sf.Height<sfText.Height)
				sf.Height = sfText.Height;

			RectangleF r = RectangleF.Empty;
			ArrayList al = new ArrayList();
			int a = 3;
			int b = 0;
			if (ShowArrow)
				b = StickHeight;
			int c = (int)sf.Height;
			if (VAlign==VerticalAlign.Bottom)
			{
				b = -b; 
				c = -c;
			}
			if (VAlign==VerticalAlign.VCenter)
				Pos.Y +=sf.Height/2;

			float w = 0;
			if (Align==FormulaAlign.Center)
				w = sf.Width/2;
			else if (Align==FormulaAlign.Right)
				w = sf.Width+2;

			if (ShowArrow)
			{
				al.Add(Pos);
				al.Add(new PointF(Pos.X-a,Pos.Y-b));
			}
			al.Add(new PointF(Pos.X-w,Pos.Y-b));
			al.Add(new PointF(Pos.X-w,Pos.Y-b-c));
			al.Add(new PointF(Pos.X-w+1+sf.Width,Pos.Y-b-c));
			al.Add(new PointF(Pos.X-w+1+sf.Width,Pos.Y-b));
			if (ShowArrow)
				al.Add(new PointF(Pos.X+a,Pos.Y-b));
			al.Add(al[0]);

			try
			{
				if (BGColor!=Color.Empty)
					g.FillPolygon(new SolidBrush(BGColor),(PointF[])al.ToArray(typeof(PointF)));
				if (BorderColor!=Color.Empty)
					g.DrawLines(new Pen(BorderColor),(PointF[])al.ToArray(typeof(PointF)));
			
				RectangleF rf = new RectangleF(new PointF(Pos.X-w,Pos.Y-b),sf);
				if (c>0)
					rf.Y -=sf.Height;
				g.DrawString(Text,TextFont,TextBrush,rf);
			} 
			catch
			{
			}
		}

		public void SetProperTextColor()
		{
			Color T = Color.Black;
			if (Math.Sqrt(BGColor.R*BGColor.R+BGColor.G*BGColor.G+BGColor.B*BGColor.B)<300)
				T = Color.White;
//			if (BGColor.R<160 || BGColor.G<160 || BGColor.B<160)
//				T = Color.White;
			if (TextBrush is SolidBrush)
				(TextBrush as SolidBrush).Color = T;
		}

		public static FormulaLabel EmptyLabel 
		{
			get 
			{
				return new FormulaLabel(Color.Empty,Color.Empty,null);
			}
		}

		public static FormulaLabel RedLabel 
		{
			get 
			{
				return new FormulaLabel(Color.Black,Color.FromArgb(255,180,180),Brushes.Black);
			}
		}

		public static FormulaLabel GreenLabel 
		{
			get 
			{
				return new FormulaLabel(Color.Black,Color.FromArgb(180,255,180),Brushes.Black);
			}
		}

		public static FormulaLabel WhiteLabel 
		{
			get 
			{
				return new FormulaLabel(Color.DarkGreen,Color.WhiteSmoke,Brushes.Black);
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			return new FormulaLabel(this.BorderColor,this.BGColor,(Brush)this.TextBrush.Clone());
		}

		#endregion
	}
}