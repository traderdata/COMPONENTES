using System;
using System.Drawing;

namespace Easychart.Finance
{
	/// <summary>
	/// This enum defines the different part of the chart
	/// </summary>
	public enum FormulaHitType {htNoWhere,htSize,htAxisX,htAxisY,htArea}//,htData}

	/// <summary>
	/// The information of (X,Y) in the chart
	/// </summary>
	public struct FormulaHitInfo
	{
		public float X;
		public float Y;
		public FormulaArea Area;
		/// <summary>
		/// Selected formula, null if no selected formula
		/// </summary>
		public FormulaBase Formula;
		/// <summary>
		/// Selected formula bind result, null if no selected formula
		/// </summary>
		public FormulaPackage FormulaResult;
		/// <summary>
		/// Selected formula data , null if no selected formula
		/// </summary>
		public FormulaData Data;
		public FormulaHitType HitType;
		public FormulaAxisY AxisY;
		public FormulaAxisX AxisX;
		public int CursorPos;

		public int XPart(int MaxPart)
		{
			if (Area!=null) 
			{
				Rectangle R = Area.Canvas.Rect;
				return ((int)X-R.X)*MaxPart/R.Width;
			}
			return -1;
		}

		public int YPart(int MaxPart)
		{
			if (Area!=null) 
			{
				Rectangle R = Area.Canvas.Rect;
				return ((int)Y-R.Y)*MaxPart/R.Height;
			}
			return -1;
		}
	}
}