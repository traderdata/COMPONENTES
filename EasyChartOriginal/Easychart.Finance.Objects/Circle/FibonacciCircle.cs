using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for FibonaciCircle.
	/// </summary>
	public class FibonacciCircle : BaseObject
	{
		private float[] split;

		public float[] Split
		{
			get
			{
				return split;
			}
			set 
			{
				split = value;
			}
		}

		public FibonacciCircle()
		{
			split = new float[]{
				(float)(3.0/2-Math.Sqrt(5)/2),
				(float)(Math.Sqrt(5)/2-1.0/2),
				(float)(1.0f),
				(float)(5.0/2-Math.Sqrt(5f)/2),
				(float)(Math.Sqrt(5)/2+1.0/2),
				(float)(2.0f),
				(float)(7.0/2-Math.Sqrt(5f)/2),
				(float)(Math.Sqrt(5)/2+3.0/2),
				(float)(Math.Sqrt(5)+2),
				(float)(11.0/2+Math.Sqrt(5)/2),
				(float)(15.0/2+Math.Sqrt(5)*3/2),
			};
		}

		public override RectangleF GetMaxRect()
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			float f = split[split.Length-1];
			float r1 = Math.Abs((p2.X-p1.X)*f);
			float r2 = Math.Abs((p1.Y-p2.Y)*f);

			float x1 = p1.X;
			float y1 = p2.Y;
			
			int w = LinePen.Width+6;
			return new RectangleF(x1-r1-w,y1-r2-w, r1*2+w*2,r2*2+w*2);
		}

		public override bool InObject(int X, int Y)
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);

			float r1 = p2.X-p1.X;
			float r2 = p1.Y-p2.Y;

			foreach(float f in split) 
			{
				bool b = PointInEllipse(X,Y,p1.X,p2.Y,r1*f,r2*f,LinePen.Width+1);
				if (b) return b;
			}
			return false;
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw (g);
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);

			float r1 = p2.X-p1.X;
			float r2 = p1.Y-p2.Y;

			Pen pen = LinePen.GetPen();
			foreach(float f in split)
				g.DrawEllipse(pen, p1.X-r1*f , p2.Y-r2*f , r1*2*f , r2*2*f);

			g.DrawLine(pen,p1.X,p1.Y,p1.X,p2.Y);
			g.DrawLine(pen,p1.X,p2.Y,p2.X,p2.Y);
		}
	}
}
