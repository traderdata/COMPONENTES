using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for FibonnaciLine.
	/// </summary>
	[Serializable]
	public class FibonacciLine : LineGroupTextObject
	{
		private SizeF sf;
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

		SnapType snap;
		public SnapType Snap
		{
			get
			{
				return snap;
			}
			set
			{
				snap = value;
			}
		}

		public FibonacciLine()
		{
			SmoothingMode = ObjectSmoothingMode.Default;
			ObjectFont.TextFont = new Font(ObjectFont.TextFont,FontStyle.Italic);
		}

		public void InitFibonacci()
		{
			split = new float[]{0f,0.382f,0.5f,0.618f,1f};
		}

		public void InitFibonacciA()
		{
			split = new float[]{0f,0.236f,0.382f,0.5f,0.618f,1f,1.382f,1.5f,1.618f,2f,2.382f,2.618f,4.236f,6.853f,11.088f,17.941f,29.029f};
		}

		public void InitPercent()
		{
			split = new float[]{0f,1f/8,2f/8,1f/3,3f/8,4f/8,5f/8,2f/3,6f/8,7f/8,1f};
		}

		public void InitPercentA()
		{
			split = new float[]{-3f,-2f,-1f,0f,1f,2f,3f};
		}

		public override void CalcPoint()
		{
			SetSnapPrice(snap);
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);

			double A = ControlPoints[0].Y;
			double B = ControlPoints[1].Y;
			
			pfStart = new PointF[split.Length];
			pfEnd = new PointF[split.Length];
			ObjectPoint op = new ObjectPoint(ControlPoints[0].X,0);
			for(int i=0; i<split.Length; i++)
			{
				op.Y = (B-A)* split[i] + A;
				PointF p = ToPointF(op);

				pfStart[i] = new PointF(p1.X,p.Y);
				pfEnd[i] = new PointF(p2.X,p.Y);
			}
		}

		private string GetStr(int i)
		{
			return split[i].ToString("p1");
		}

		private RectangleF GetRect(int i)
		{
			SizeF sf = ObjectFont.Measure(CurrentGraphics,GetStr(i));
			float X = Math.Min(pfStart[i].X,pfEnd[i].X);
			float Y = pfStart[i].Y;
			float W = Math.Max(Math.Abs(pfStart[i].X-pfEnd[i].X),sf.Width);
			float H = sf.Height;
			return new RectangleF(X,Y,W,H);
		}

		public override Region GetRegion()
		{
			Region R = base.GetRegion ();
			if (CurrentGraphics!=null)
			{
				for(int i=0; i<split.Length; i++)
					R.Union(GetRect(i));
			}
			return R;
		}

		public override void Draw(Graphics g)
		{
			base.Draw (g);
			sf = SizeF.Empty;
			for(int i=0; i<split.Length; i++)
				try
				{
					ObjectFont.DrawString(GetStr(i),g,GetRect(i));
				} 
				catch
				{
				}
		}
	}
}