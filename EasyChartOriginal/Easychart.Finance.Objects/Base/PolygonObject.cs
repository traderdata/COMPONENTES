using System;
using System.Collections;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for PolygonObject.
	/// </summary>
	public class PolygonObject:BaseObject
	{
		protected PointF[] AllPoints;
		public PolygonObject()
		{
		}

		public override RectangleF GetMaxRect()
		{
			AllPoints = CalcPoint();
			return base.GetMaxRect (AllPoints);
		}

		public virtual PointF[] CalcPoint()
		{
			return new PointF[]{};
		}

		[Browsable(false),XmlIgnore]
		public virtual bool Closed
		{
			get
			{
				return false;
			}
		}

		public override bool InObject(int X, int Y)
		{
			return InLineSegment(X,Y,AllPoints,LinePen.Width,Closed);
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw (g);
			AllPoints = CalcPoint();
			if (AllPoints.Length>0)
			{
				try
				{
					if (Closed)
						g.DrawPolygon(LinePen.GetPen(),AllPoints);
					else 
						g.DrawLines(LinePen.GetPen(),AllPoints);
				} 
				catch
				{
				}
			}
		}
	}
}
