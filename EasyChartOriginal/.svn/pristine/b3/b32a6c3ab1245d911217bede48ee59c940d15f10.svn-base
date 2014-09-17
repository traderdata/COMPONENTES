using System;
using System.Drawing;

namespace Easychart.Finance
{

	/// <summary>
	/// Stock Formula Canvas, Used by formula system natively.
	/// </summary>
	public class FormulaCanvas 
	{
		public int Start;
		public int Count;
		public int Stop;
		public double ColumnWidth;
		public double ColumnPercent;
		public Rectangle Rect;
		public Rectangle ClipRect;
		public Rectangle FrameRect;
		public double[] DATE;
		public float LabelHeight;

		public Graphics CurrentGraph;
		public FormulaAxisX AxisX;

		public float GetX(int i) 
		{
			return (float)(Rect.X+(float)((i-Stop)*ColumnWidth+ColumnWidth/2));
		}

		public int Left
		{
			get
			{
				return (int)(Rect.Left + ColumnWidth);
			}
		}

		public int Right 
		{
			get
			{
				  return (int)(Rect.Left  + ColumnWidth*(Count-1));
			}
		}
	}
}
