using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for Ellipse.
	/// </summary>
	public class ObjectEllipse : BaseObject
	{
		public ObjectEllipse():base()
		{
		}

		public override bool InObject(int X, int Y)
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);

			float r1 = p2.X-p1.X;
			float r2 = p1.Y-p2.Y;
			return PointInEllipse(X,Y,p2.X,p1.Y,r1,r2,LinePen.Width+1);
		}

		public override RectangleF GetMaxRect()
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			float r1 = p2.X-p1.X;
			float r2 = p1.Y-p2.Y;
			
			float x1 = p1.X;
			float y1 = p2.Y;
			if (r1<0) 
			{
				x1 +=r1*2;
				r1 = - r1;
			}
			if (r2<0)
			{
				y1 += r2*2;
				r2 = - r2;
			}
			
			int w = LinePen.Width+6;
			return new RectangleF(x1-w,y1-w, r1*2+w*2,r2*2+w*2);
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw(g);
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			float r1 = p2.X-p1.X;
			float r2 = p1.Y-p2.Y;

			g.DrawEllipse(LinePen.GetPen(),p1.X,p2.Y,r1*2,r2*2);
		}
	}
}
