using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	public enum CycleStyle{Equal,FibonacciCycle,Sqr,Symmetry};

	/// <summary>
	/// Summary description for CycleObject.
	/// </summary>
	public class CycleObject: LineGroupTextObject
	{
		double CurrentCycle;
		ArrayList alStart = new ArrayList();
		ArrayList alEnd = new ArrayList();
		bool showCycleText = true;
		CycleStyle cycleStyle = CycleStyle.Equal;
		int maxLines = 20;

		[DefaultValue(20),XmlAttribute]
		public int MaxLines
		{
			get
			{
				return maxLines;
			}
			set
			{
				maxLines = value;
			}
		}

		[ReadOnly(true),DefaultValue(CycleStyle.Equal),XmlAttribute]
		public CycleStyle CycleStyle
		{
			get
			{
				return cycleStyle;
			}
			set
			{
				cycleStyle = value;
			}
		}

		[DefaultValue(true),XmlAttribute]
		public bool ShowCycleText
		{
			get
			{
				return showCycleText;
			}
			set
			{
				showCycleText = value;
			}
		}


		public CycleObject()
		{
			SmoothingMode = ObjectSmoothingMode.Default;
			ObjectFont.TextFont = new Font(ObjectFont.TextFont,FontStyle.Italic);
		}

		public void Equal()
		{
			CycleStyle = CycleStyle.Equal;
		}

		public void FibonacciCycle()
		{
			CycleStyle = CycleStyle.FibonacciCycle;
		}

		public void Sqr()
		{
			CycleStyle = CycleStyle.Sqr;
		}

		public void Symmetry()
		{
			CycleStyle = CycleStyle.Symmetry;
		}

		public override int InitNum
		{
			get
			{
				return 1+((cycleStyle==CycleStyle.Equal | cycleStyle==CycleStyle.Symmetry)?1:0);
			}
		}

		public override int ControlPointNum
		{
			get
			{
				return this.InitNum;
			}
		}

		public override void CalcPoint()
		{
			int A = 1;
			int B = 1;

			int Bars = 1;

			PointF p0 = ToPointF(ControlPoints[0]);
			double ColumnWidth =Manager.Canvas.Chart.ColumnWidth;
			if (ControlPointNum>1)
			{
				PointF p1 = ToPointF(ControlPoints[1]);
				Bars =(int)Math.Round((p1.X-p0.X)/ColumnWidth);
				if (Bars==0) Bars = 1;
			}

			CurrentCycle = 0;
			Rectangle R = Area.Canvas.Rect;
			float X = 1f;
			alStart.Clear();
			alEnd.Clear();
			for(int i=0; X>0 && X<R.Right && i<maxLines; i++)
			{
				X = p0.X+(float)(ColumnWidth*CurrentCycle);
				alStart.Add(new PointF(X,R.Top));
				alEnd.Add(new PointF(X,R.Bottom));
				switch (cycleStyle)
				{
					case CycleStyle.Equal:
						CurrentCycle += Bars;
						break;
					case CycleStyle.FibonacciCycle:
						if ((i % 2) == 0)
						{
							CurrentCycle = B;
							A = A+B;
						} 
						else 
						{
							CurrentCycle = A;
							B = B+A;
						}
						break;
					case CycleStyle.Sqr:
						CurrentCycle =(i+1)*(i+1);
						break;
					case CycleStyle.Symmetry:
						if (i==0)
							CurrentCycle = 0;
						else if (i==1)
							CurrentCycle = Bars;
						else if (i==2)
							CurrentCycle = -Bars;
						else X = 0;
						break;
				}
			}
			pfStart = (PointF[])alStart.ToArray(typeof(PointF));
			pfEnd = (PointF[])alEnd.ToArray(typeof(PointF));
		}

		private string GetStr(int i)
		{
			double ColumnWidth =Manager.Canvas.Chart.ColumnWidth;
			return ((int)(Math.Round(pfStart[i].X-pfStart[0].X) / ColumnWidth)).ToString();
		}

		private RectangleF GetRect(string s,int i)
		{
			SizeF sf = ObjectFont.Measure(CurrentGraphics,s);
			Rectangle R = Area.Canvas.Rect;
			return new RectangleF(pfStart[i].X,R.Top,sf.Width,R.Height);
		}

		private RectangleF GetRect(int i)
		{
			return GetRect(GetStr(i),i);
		}

		public override Region GetRegion()
		{
			Region R = base.GetRegion ();
			if (showCycleText)
				for(int i=0; i<pfStart.Length; i++)
					R.Union(GetRect(i));
			return R;
		}

		public override void Draw(Graphics g)
		{
			base.Draw (g);
			if (showCycleText)
			{
				for(int i=0; i<pfStart.Length; i++)
				{
					string s = GetStr(i);
					ObjectFont.DrawString(s,g,GetRect(s,i));
				}
			}
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
				new ObjectInit("Equal cycle line",typeof(CycleObject),"Equal","Cycle","CycleE",500),
				new ObjectInit("Fibonacci cycle line",typeof(CycleObject),"FibonacciCycle","Cycle","CycleF"),
				new ObjectInit("Sqr cycle line",typeof(CycleObject),"Sqr","Cycle","CycleSqr"),
				new ObjectInit("Symmetry line",typeof(CycleObject),"Symmetry","Cycle","CycleSym"),
				};
		}
	}
}