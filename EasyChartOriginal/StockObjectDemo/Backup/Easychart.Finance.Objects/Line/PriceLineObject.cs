using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for PriceLine.
	/// </summary>
	public class PriceLineObject : LineGroupTextObject
	{
		public PriceLineObject()
		{
			SmoothingMode = ObjectSmoothingMode.Default;
			ObjectFont.TextFont = new Font(ObjectFont.TextFont,FontStyle.Italic);
			TextFormat = "{0:f2} \r\n {1:p2}";
		}

		public override void CalcPoint()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			if (Area!=null && Area.Canvas!=null)
			{
				Rectangle R = Area.Canvas.Rect;
				pfStart = new PointF[3];
				pfEnd = new PointF[3];
				pfStart[0] = pfs[0];
				pfEnd[0] = new PointF(pfs[1].X,pfs[0].Y);
				pfStart[1] = new PointF(pfs[0].X,pfs[1].Y);
				pfEnd[1] = pfs[1];
				pfStart[2] = new PointF((pfs[0].X+pfs[1].X)/2,pfs[0].Y);
				pfEnd[2] = new PointF((pfs[0].X+pfs[1].X)/2,pfs[1].Y);
			}
		}

		public override string GetStr()
		{
			double d = ControlPoints[1].Y-ControlPoints[0].Y;
			return string.Format(TextFormat, d,d/ControlPoints[0].Y); 
			//return d.ToString("f2")+"\r\n"+(d/ControlPoints[0].Y).ToString("p2");
		}

		public override RectangleF GetTextRect()
		{
			return base.GetTextRect(2,false);
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
								   new ObjectInit("Price Line",typeof(PriceLineObject),null,"LineGroup","PriceLine"),
				};
		}
	}
}
