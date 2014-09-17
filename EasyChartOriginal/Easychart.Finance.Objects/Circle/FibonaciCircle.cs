using System;
using System.Drawing;

namespace TestEpg.Objects
{
	/// <summary>
	/// Summary description for FibonaciCircle.
	/// </summary>
	public class FibonaciCircle : ObjectBase
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

		public FibonaciCircle()
		{
			split = new float[]{
				(float)(3/2-Math.Sqrt(5)/2),
				(float)(Math.Sqrt(5)/2-1/2),
				(float)(1),
				(float)(5/2-Math.Sqrt(5f)/2),
				(float)(Math.Sqrt(5)/2+1/2),
				(float)(2),
				(float)(7/2-Math.Sqrt(5f)/2),
				(float)(Math.Sqrt(5)/2+3/2),
				(float)(Math.Sqrt(5)+2),
				(float)(11/2+Math.Sqrt(5)/2),
				(float)(15/2+Math.Sqrt(5)*3/2),
			};
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw (g);
			PointF p1 = ControlPoints[0];
			PointF p2 = ControlPoints[1];

			float r1 = p2.X-p1.X;
			float r2 = p1.Y-p2.Y;

			Pen pen = LinePen.GetPen();
			foreach(float f in split)
			{
				g.DrawEllipse(pen, p1.X-r1*f , p2.Y-r2*f , r1*2*f , r2*2*f);
			}

			g.DrawLine(pen,p1.X,p1.Y,p1.X,p2.Y);
			g.DrawLine(pen,p1.X,p2.Y,p2.X,p2.Y);
		}
	}
}
