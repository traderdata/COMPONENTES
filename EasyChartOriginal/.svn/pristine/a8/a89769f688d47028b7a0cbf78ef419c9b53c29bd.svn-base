using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for FillPolygonObject.
	/// </summary>
	public class FillPolygonObject : PolygonObject
	{
		private BrushMapper brush = new BrushMapper();
		public BrushMapper Brush
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

		public bool ShouldSerializeBrush()
		{
			return BrushMapper.NotDefault(Brush);
		}

		public FillPolygonObject()
		{
		}

		public void Empty()
		{
			Brush.Alpha = 0;
		}

		public void Fill()
		{
			Brush.BrushStyle = BrushMapperStyle.Solid;
			Brush.Color = Color.FromArgb(160,160,64);
			Brush.Alpha = 40;
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
			if (Brush.BrushStyle==BrushMapperStyle.Empty ||
				(Brush.BrushStyle==BrushMapperStyle.Solid && Brush.Color == Color.Empty))
				return;
			try
			{
				g.FillPolygon(Brush.GetBrush(GetMaxRect()),AllPoints);
			}
			catch
			{
			}
		}
	}
}
