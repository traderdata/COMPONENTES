using System;
using System.Collections;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	public enum SpiralType{Archimedes,Logarithmic,Parabolic,Hyperbolic,Lituus};

	/// <summary>
	/// Summary description for LogObject.
	/// </summary>
	public class SpiralObject:PolygonObject
	{
		SpiralType spiralType;
		int sweepAngle = 1800;
		public SpiralType SpiralType
		{
			get
			{
				return spiralType;
			}
			set
			{
				spiralType = value;
			}
		}

		public int SweepAngle
		{
			get
			{
				return sweepAngle;
			}
			set
			{
				sweepAngle = value;
			}
		}

		ArrayList alPoint = new ArrayList();
		public SpiralObject()
		{
		}
		
		public void Archimedes()
		{
			spiralType = SpiralType.Archimedes;
		}

		public void Logarithmic()
		{
			spiralType = SpiralType.Logarithmic;
		}
		
		public void Parabolic()
		{
			spiralType = SpiralType.Parabolic;
		}
		
		public void Hyperbolic()
		{
			spiralType = SpiralType.Hyperbolic;
		}
		
		public void Lituus()
		{
			spiralType = SpiralType.Lituus;
		}

		public override PointF[] CalcPoint()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			alPoint.Clear();
			alPoint.Add(pfs[0]);
			float W = pfs[1].X-pfs[0].X;
			float H = pfs[1].Y-pfs[0].Y;
			double StartR = Dist(pfs[0],pfs[1]);
			double StartAngle = Math.Atan2(H,W);
			StartAngle += Math.PI*2;
			Rectangle Rect = Area.Canvas.Rect;
			double Len = Math.Sqrt(Rect.Width*Rect.Width+Rect.Height*Rect.Height);

			double A = 1;
			switch (spiralType)
			{
				case SpiralType.Archimedes:
					A = StartR/StartAngle;
					break;
				case SpiralType.Logarithmic:
					A = StartR/Math.Exp(StartAngle);
					break;
				case SpiralType.Parabolic:
					A = Math.Sqrt(StartR*StartR/StartAngle);
					break;
				case SpiralType.Hyperbolic:
					A = StartR*StartAngle;
					break;
				case SpiralType.Lituus:
					A = Math.Sqrt(StartR*StartR*StartAngle);
					break;
			}

			for(int i=0; i<sweepAngle; i++) 
			{
				double T = (double)i/180*Math.PI+StartAngle;
				double R = 0;
				switch (spiralType)
				{
					case SpiralType.Archimedes:
						R = A*T;
						break;
					case SpiralType.Logarithmic:
						R = A*Math.Exp(T);
						break;
					case SpiralType.Parabolic:
						R = Math.Sqrt(A*A*T);
						break;
					case SpiralType.Hyperbolic:
						R = A/T;
						break;
					case SpiralType.Lituus:
						R = Math.Sqrt(A*A/T);
						break;
				}
				if (R>Len) break;
				float X = (float)(pfs[0].X+R*Math.Cos(T));
				float Y = (float)(pfs[0].Y+R*Math.Sin(T));
				if (!float.IsInfinity(X) && !float.IsInfinity(Y))
					alPoint.Add(new PointF(X,Y));
			}
			return (PointF[])alPoint.ToArray(typeof(PointF));
		}

		public override ObjectInit[] GetObjectList()
		{
			return new ObjectInit[]{
				new ObjectInit("Logarithmic Spiral",GetType(),"Logarithmic","Cycle","SpiralL"),
				new ObjectInit("Archimedes Spiral",GetType(),"Archimedes","Cycle","SpiralA"),
				new ObjectInit("Parabolic Spiral",GetType(),"Parabolic","Cycle","SpiralP"),
				new ObjectInit("Hyperbolic Spiral",GetType(),"Hyperbolic","Cycle","SpiralH"),
				new ObjectInit("Lituus Spiral",GetType(),"Lituus","Cycle","SpiralLi"),
			};
		}
	}
}