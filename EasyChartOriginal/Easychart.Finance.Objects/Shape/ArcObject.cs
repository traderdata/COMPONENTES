using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for EllipseObject.
	/// </summary>
	public class ArcObject:FillPolygonObject
	{
		public ArcObject()
		{
		}

		public override System.Drawing.PointF[] CalcPoint()
		{
			GraphicsPath gp = new GraphicsPath();
			PointF[] pfs = ToPoints(ControlPoints);

			try
			{
				float W = Math.Abs(pfs[1].X-pfs[0].X)*2;
				float H = Math.Abs(pfs[1].Y-pfs[0].Y)*2;
				float Left;
				if (pfs[0].X<pfs[1].X)
					Left = pfs[0].X;
				else Left = pfs[1].X*2-pfs[0].X;
				float Top;
				if (pfs[0].Y<pfs[1].Y)
					Top = pfs[0].Y*2-pfs[1].Y;
				else Top = pfs[1].Y;
				
				gp.AddArc(Left,Top,W,H,0f,360f);
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
