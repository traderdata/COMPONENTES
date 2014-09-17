using System;
using System.ComponentModel;
using System.Xml.Serialization;
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
		SpiralType spiralType = SpiralType.Archimedes;
		int sweepAngle = 1800;

		[DefaultValue(SpiralType.Archimedes),XmlAttribute]
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

		[DefaultValue(1800),XmlAttribute]
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

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
				new ObjectInit("Logarithmic Spiral",typeof(SpiralObject),"Logarithmic","Circle","SpiralL",1000),
				new ObjectInit("Archimedes Spiral",typeof(SpiralObject),"Archimedes","Circle","SpiralA"),
				new ObjectInit("Parabolic Spiral",typeof(SpiralObject),"Parabolic","Circle","SpiralP"),
				new ObjectInit("Hyperbolic Spiral",typeof(SpiralObject),"Hyperbolic","Circle","SpiralH"),
				new ObjectInit("Lituus Spiral",typeof(SpiralObject),"Lituus","Circle","SpiralLi"),
				};
		}
	}
}