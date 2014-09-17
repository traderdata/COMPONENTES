using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for CircleObject.
	/// </summary>
	public class CircleObject: FillPolygonObject
	{
		public CircleObject()
		{
		}

		public override System.Drawing.PointF[] CalcPoint()
		{
			GraphicsPath gp = new GraphicsPath();
			PointF[] pfs = ToPoints(ControlPoints);
			float R = (float)Dist(pfs[0],pfs[1]);
			try
			{
				gp.AddArc(pfs[0].X-R,pfs[0].Y-R,R*2,R*2,0f,360f);
				gp.Flatten();
				PointF[] pfs1 = gp.PathPoints;
				return pfs1;
			} 
			catch
			{
			}
			return base.CalcPoint();
		}
	}
}
