using System;
using Easychart.Finance.Win;
using System.Windows.Forms;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for WebCanvas.
	/// </summary>
	public class WebCanvas : IObjectCanvas
	{
		FormulaChart chart;

		public FormulaChart Chart
		{
			get
			{
				return chart;
			}
		}

		public Control DesignerControl
		{
			get
			{
				return null;
			}
		}

		public bool Designing
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		
		public WebCanvas(FormulaChart fc)
		{
			this.chart = fc;
		}
	}
}
