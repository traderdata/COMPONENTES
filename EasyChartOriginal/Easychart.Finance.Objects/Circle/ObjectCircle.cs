using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectCircle.
	/// </summary>
	public class ObjectCircle : BaseObject
	{
		public ObjectCircle():base()
		{
		}

		public override bool InObject(int X, int Y)
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			double r= Dist(p1,p2);
			return Math.Abs(Dist(new PointF(X,Y),p1)-r)<=this.LinePen.Width+1;
		}

		public override Region GetRegion()
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			float r= (float)Dist(p1,p2);
			
			int w = LinePen.Width+6;
			RectangleF R = new RectangleF(p1.X-r-w, p1.Y-r-w, p1.X+r+2*w, p1.Y+r+2*w);
			if (R.X<0) R.X = 0;
			if (R.Y<0) R.Y = 0;
			return new Region(R);
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			double r= Dist(p1,p2);

			g.DrawEllipse(LinePen.GetPen(),(int)(p1.X-r),(int)(p1.Y-r),(int)(r*2),(int)(r*2));
		}
	}
}
