using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Easychart.Finance.Objects
{
	public enum Shape {ParralleloGram,Triangle}

	/// <summary>
	/// Summary description for TriangleObject.
	/// </summary>
	public class TriangleObject:FillPolygonObject
	{
		private Shape shape = Shape.Triangle;
		[DefaultValue(Shape.Triangle),XmlAttribute]
		public Shape Shape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
			}
		}

		public TriangleObject()
		{
		}

		public void Triangle()
		{
			shape = Shape.Triangle;
			Empty();
		}

		public void ParralleloGram()
		{
			shape = Shape.ParralleloGram;
			Empty();
		}

		public void TriangleF()
		{
			Triangle();
			Fill();
		}

		public void ParralleloGramF()
		{
			ParralleloGram();
			Fill();
		}

		public override int ControlPointNum
		{
			get
			{
				return 3;
			}
		}

		public override int InitNum
		{
			get
			{
				return 3;
			}
		}

		public override PointF[] CalcPoint()
		{
			if (shape==Shape.ParralleloGram)
			{
				PointF[] pfs = ToPoints(ControlPoints);
				ArrayList al = new ArrayList();
				al.AddRange(pfs);
				PointF P = new PointF(pfs[0].X-pfs[1].X+pfs[2].X,pfs[0].Y-pfs[1].Y+pfs[2].Y);
				al.Add(P);
				return (PointF[])al.ToArray(typeof(PointF));
			} 
			else 
				return ToPoints(ControlPoints);
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
				new ObjectInit("Triangle",typeof(TriangleObject),"Triangle","Shape","Traingle"),
				new ObjectInit("ParralleloGram",typeof(TriangleObject),"ParralleloGram","Shape","ParralleloGram"),
				new ObjectInit("Triangle",typeof(TriangleObject),"TriangleF","Shape","TraingleF"),
				new ObjectInit("ParralleloGram",typeof(TriangleObject),"ParralleloGramF","Shape","ParralleloGramF"),
				};
		}
	}
}