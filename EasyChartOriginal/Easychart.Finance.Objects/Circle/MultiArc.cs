using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for MultiArc.
	/// </summary>
	public class MultiArc : BaseObject
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

		public MultiArc()
		{
			split = new float[]{1/3f,3/8f,1/2f,5/8f,2/3f};
		}

		public override RectangleF GetMaxRect()
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			float r= (float)Dist(p1,p2);
			
			int w = LinePen.Width+6;
			RectangleF R = new RectangleF(p1.X-r-w, p1.Y-r-w, p1.X+r+2*w, p1.Y+r+2*w);
			if (R.X<0) R.X = 0;
			if (R.Y<0) R.Y = 0;
			return R;
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw (g);
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			float r= (float)Dist(p1,p2);

			Pen pen = LinePen.GetPen();
			float StartAngle = 0 ;
			if (p2.Y<p1.Y)
				StartAngle = 180;

			if (r>0)
			foreach(float f in split)
				g.DrawArc(pen,new RectangleF(p1.X-r*f,p1.Y-r*f,r*2*f,r*2*f),StartAngle,180);

			g.DrawLine(pen,p1,p2);
		}
	}
}
