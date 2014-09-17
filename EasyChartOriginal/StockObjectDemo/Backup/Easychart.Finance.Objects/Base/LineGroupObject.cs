using System;
using System.Drawing;
using System.Collections;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for LineGroupObject.
	/// </summary>
	public class LineGroupObject:BaseObject
	{
		protected PointF[] pfStart;
		protected PointF[] pfEnd;

		public LineGroupObject()
		{
		}

		public virtual void CalcPoint()
		{
		}

		public void OpenStartEnd(bool openStart,bool openEnd)
		{
			if (pfStart!=null)
				for(int i=0; i<pfStart.Length; i++)
				{
					if (openStart) ExpandLine(ref pfEnd[i],ref pfStart[i]);
					if (openEnd) ExpandLine(ref pfStart[i],ref pfEnd[i]);
				}
		}

		public override System.Drawing.RectangleF GetMaxRect()
		{
			PointF p0 = ToPointF(ControlPoints[0]);
			CalcPoint();
			if (pfStart!=null && pfEnd!=null) 
			{
				ArrayList al = new ArrayList();
				al.AddRange(pfStart);
				al.AddRange(pfEnd);
				return base.GetMaxRect((PointF[])al.ToArray(typeof(PointF)));
			}
			return base.GetMaxRect();
		}

		public override bool InObject(int X, int Y)
		{
			CalcPoint();
			if (pfStart!=null && pfEnd!=null)
				for(int i=0; i<pfStart.Length; i++)
					if (pfStart[i]!=PointF.Empty)
					{
						bool b =InLineSegment(X,Y,pfStart[i],pfEnd[i],LinePen.Width);
						if (b) return b;
					}
			return false;
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw (g);
			CalcPoint();
			if (pfStart!=null && pfEnd!=null)
				for(int i=0; i<pfStart.Length; i++)
					try
					{
						g.DrawLine(LinePen.GetPen(),pfStart[i],pfEnd[i]);
					} 
					catch
					{
					}
		}
	}
}