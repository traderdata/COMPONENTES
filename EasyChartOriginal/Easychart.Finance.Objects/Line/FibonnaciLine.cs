using System;
using System.Drawing;

namespace TestEpg.Objects
{
	/// <summary>
	/// Summary description for FibonnaciLine.
	/// </summary>
	[Serializable]
	public class FibonnaciLine : ObjectBase
	{
		public FibonnaciLine()
		{
		}

		public override bool InObject(int X, int Y)
		{
			PointF p1 = ControlPoints[0];
			PointF p2 = ControlPoints[1];

			PointF pp1 = new PointF(p1.X,p1.Y);
			PointF pp2 = new PointF(p2.X,p1.Y);
			float[] ff = {0f,0.382f,0.5f,0.618f,1f};
			float MinX = Math.Min(pp1.X,pp2.X);
			float MaxX = Math.Max(pp1.X,pp2.X);
			foreach(float f in ff)
			{
				pp1.Y = (p2.Y-p1.Y) * f +p1.Y;
				pp2.Y = (p2.Y-p1.Y) * f +p1.Y;
				if (Math.Abs(Y-pp1.Y)<=this.LinePen.Width+1 && X>MinX && X<MaxX)
					return true;
			}
			return false;
		}

		public override RectangleF GetMaxRect()
		{
			RectangleF R = base.GetMaxRect();
			R.Height +=20;
			return R;
		}

		public override void Draw(Graphics g)
		{
			base.Draw (g);
			PointF p1 = ControlPoints[0];
			PointF p2 = ControlPoints[1];

			PointF pp1 = new PointF(p1.X,p1.Y);
			PointF pp2 = new PointF(p2.X,p1.Y);
			float[] ff = {0f,0.382f,0.5f,0.618f,1f};
			foreach(float f in ff)
			{
				pp1.Y = (p2.Y-p1.Y) * f +p1.Y;
				pp2.Y = (p2.Y-p1.Y) * f +p1.Y;
				g.DrawString(f.ToString("p1"),ObjectCanvas.ObjectFont,ObjectCanvas.ObjectBrush,pp1.X,pp1.Y);
				g.DrawLine(LinePen.GetPen(),pp1,pp2);
			}
		}
	}
}
