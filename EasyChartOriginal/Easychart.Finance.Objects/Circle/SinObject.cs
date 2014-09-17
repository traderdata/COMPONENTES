using System;
using System.Collections;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Sin Object
	/// </summary>
	public class SinObject:PolygonObject
	{
		ArrayList alPoint = new ArrayList();
		public SinObject()
		{
		}

		public override RectangleF GetMaxRect()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			Rectangle R = Area.Canvas.Rect;
			pfs[0].X = R.Left;
			pfs[1].X = R.Right;
			return base.GetMaxRect(pfs);
		}

		public override PointF[] CalcPoint()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			alPoint.Clear();
			float W = pfs[1].X-pfs[0].X;
			float H = (pfs[1].Y-pfs[0].Y)/2;
			Rectangle R = Area.Canvas.Rect;
			if (W!=0)
			for(int i=R.Left; i<R.Right; i++)
				alPoint.Add(new PointF(i,(float)(pfs[0].Y+H+H*Math.Cos((i-pfs[1].X)/W*Math.PI))));
			return (PointF[])alPoint.ToArray(typeof(PointF));
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
						   new ObjectInit("Sin Object",typeof(SinObject),null,"Circle","Sin"),
				};
		}
	}
}
