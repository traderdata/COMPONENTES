using System;
using System.Drawing;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for CycleCounterObject.
	/// </summary>
	public class CycleCounterObject: LineGroupTextObject
	{
		public CycleCounterObject()
		{
			ObjectFont.TextFont = new Font(ObjectFont.TextFont,FontStyle.Bold | FontStyle.Italic);
			ObjectFont.Alignment = StringAlignment.Center;
			SmoothingMode = ObjectSmoothingMode.Default;
		}

		public override void CalcPoint()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			pfStart = new PointF[3];
			pfEnd = new PointF[3];
			Rectangle R = Area.Canvas.Rect;
			pfStart[0] = new PointF(pfs[0].X,R.Top);
			pfEnd[0] = new PointF(pfs[0].X,R.Bottom);
			pfStart[1] = new PointF(pfs[1].X,R.Top);
			pfEnd[1] = new PointF(pfs[1].X,R.Bottom);
			pfStart[2] = new PointF(pfs[0].X,pfs[1].Y);
			pfEnd[2] = new PointF(pfs[1].X,pfs[1].Y);
		}

		public override string GetStr()
		{
			//IDataProvider idp = Manager.Canvas.BackChart.DataProvider;
			//double[] dd = idp["DATE"];
			FormulaChart BackChart = Manager.Canvas.Chart;
			//int Bar1 = FormulaChart.FindIndex(dd,ControlPoints[0].X);
			//int Bar2 = FormulaChart.FindIndex(dd,ControlPoints[1].X);
			int Bar1 = BackChart.DateToIndex(ControlPoints[0].X);
			int Bar2 = BackChart.DateToIndex(ControlPoints[1].X);
			return (Bar2-Bar1)+"(T)";
		}

		public override RectangleF GetTextRect()
		{
			return base.GetTextRect(2,true);
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
					new ObjectInit("CycleCounterObject",typeof(CycleCounterObject),null,"Cycle","CycleC")
			   };
		}
	}
}
