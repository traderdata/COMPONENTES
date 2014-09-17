using System;
using System.ComponentModel;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for FibonnaciLine.
	/// </summary>
	[Serializable]
	public class FibonacciLineObject : LineGroupTextObject
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
		[DefaultValue(SnapType.None)]
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

		private bool openStart = false;
		private bool openEnd = false;
		[DefaultValue(false)]
		public bool OpenStart
		{
			get
			{
				return openStart;
			}
			set
			{
				openStart = value;
			}
		}

		[DefaultValue(false)]
		public bool OpenEnd
		{
			get
			{
				return openEnd;
			}
			set
			{
				openEnd = value;
			}
		}

		public FibonacciLineObject()
		{
			SmoothingMode = ObjectSmoothingMode.Default;
			ObjectFont.TextFont = new Font(ObjectFont.TextFont,FontStyle.Italic);

			TextFormat = "{0:f2} {1:p2}";
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

		private double GetPrice(int LineIndex)
		{
			return (ControlPoints[0].Y-ControlPoints[1].Y)* split[LineIndex] + ControlPoints[1].Y;
		}

		public override void CalcPoint()
		{
			SetSnapPrice(snap);
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			
			pfStart = new PointF[split.Length];
			pfEnd = new PointF[split.Length];
			ObjectPoint op = new ObjectPoint(ControlPoints[0].X,0);
			for(int i=0; i<split.Length; i++)
			{
				op.Y = GetPrice(i);
				PointF p = ToPointF(op);

				pfStart[i] = new PointF(p1.X,p.Y);
				pfEnd[i] = new PointF(p2.X,p.Y);
			}
			OpenStartEnd(openStart,openEnd);
		}

		private string GetStr(int i)
		{
			return string.Format(TextFormat,GetPrice(i),split[i]);
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

			Pen P = LinePen.GetPen();
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			Rectangle R = Area.Canvas.Rect;
			if (InMove && p1!=PointF.Empty && p2!=PointF.Empty)
				g.DrawLine(P,p1.X,p1.Y,p2.X,p2.Y);
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
						new ObjectInit("Fibonacci Line",typeof(FibonacciLineObject),"InitFibonacci","LineGroup","FibLine",300),
						new ObjectInit("Percentage Line",typeof(FibonacciLineObject),"InitPercent","LineGroup","PercentLine"),
						new ObjectInit("Fibonacci Line A",typeof(FibonacciLineObject),"InitFibonacciA","LineGroup","FibLineA"),
						new ObjectInit("Percentage Line A",typeof(FibonacciLineObject),"InitPercentA","LineGroup","PercentLineA"),
				};
		}
	}
}