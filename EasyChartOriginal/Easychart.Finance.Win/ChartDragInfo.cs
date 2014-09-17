using System;
using System.Drawing;
using Easychart.Finance;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Save the informations such as which part is dragging,area's height etc before a mouse drag
	/// </summary>
	public class ChartDragInfo
	{
		/// <summary>
		/// FormulaHitInfo of current drag
		/// </summary>
		public FormulaHitInfo HitInfo;
		/// <summary>
		/// Area height percentage before current drag
		/// </summary>
		public int[] AreaHeight;
		/// <summary>
		/// Minimum Y-axis value before current drag
		/// </summary>
		public double AreaMinY;
		/// <summary>
		/// Maximum Y-axis value before current drag
		/// </summary>
		public double AreaMaxY;
		/// <summary>
		/// StartTime before current drag
		/// </summary>
		public DateTime StartTime;
		/// <summary>
		/// EndTime before current drag
		/// </summary>
		public DateTime EndTime;

		/// <summary>
		/// Constructor of ChartDragInfo
		/// </summary>
		/// <param name="Chart">FormulaChart instance</param>
		/// <param name="HitInfo">The Hit information of the dragging start point</param>
		public ChartDragInfo(FormulaChart Chart,FormulaHitInfo HitInfo)
		{
			
			this.HitInfo = HitInfo;
			AreaHeight = new int[Chart.Areas.Count];
			FormulaAxisY fay = HitInfo.AxisY;
			if (fay==null && HitInfo.Area!=null)
				fay = HitInfo.Area.AxisY;
			if (fay!=null)
			{
				AreaMinY = fay.MinY;
				AreaMaxY = fay.MaxY;
			}

			for(int i=0; i<Chart.Areas.Count; i++)
			{
				FormulaArea fa = Chart.Areas[i];
				AreaHeight[i] = fa.Rect.Height;
			}
			StartTime = Chart.StartTime;
			EndTime = Chart.EndTime;
		}
	}
}
