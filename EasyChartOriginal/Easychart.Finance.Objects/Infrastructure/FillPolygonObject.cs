using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for FillPolygonObject.
	/// </summary>
	public class FillPolygonObject : PolygonObject
	{
		private ObjectBrush brush = new ObjectBrush(Color.Empty);
		public ObjectBrush Brush
		{
			get
			{
				return brush;
			}
			set
			{
				brush = value;
			}
		}


		public FillPolygonObject()
		{
		}

		public override bool Closed
		{
			get
			{
				return true;
			}
		}

		public override void Draw(Graphics g)
		{
			base.Draw (g);
			if (Brush.BrushStyle==BrushStyle.Empty ||
				(Brush.BrushStyle==BrushStyle.Solid && Brush.Color == Color.Empty))
				return;
			g.FillPolygon(Brush.GetBrush(GetMaxRect()),AllPoints);
		}
	}
}
