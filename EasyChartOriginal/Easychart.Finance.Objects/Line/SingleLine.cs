using System;
using System.Drawing;
using System.ComponentModel;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	public enum SingleLineType {Vertical,Horizontal};
	/// <summary>
	/// Summary description for SingleLine.
	/// </summary>
	public class SingleLine : LineGroupTextObject
	{
		string dataFormat = "f2";
		public string DataFormat 
		{
			get
			{
				return dataFormat;
			}
			set
			{
				dataFormat = value;
			}
		}
		
		SingleLineType lineType;
		[RefreshProperties(RefreshProperties.All)]
		public SingleLineType LineType
		{
			get
			{
				return lineType;
			}
			set
			{
				lineType = value;
				if (value== SingleLineType.Horizontal) 
				{
					dataFormat = "f2";
					ObjectFont.Alignment = StringAlignment.Near;
				}
				else 
				{
					dataFormat = "yyyy-MM-dd";
					ObjectFont.Alignment = StringAlignment.Far;
				}
			}
		}

		public SingleLine()
		{
			ObjectFont.TextFont = new Font(ObjectFont.TextFont,FontStyle.Bold | FontStyle.Italic);
			ObjectFont.TextBrush.Color = Color.Red;
			SmoothingMode = ObjectSmoothingMode.Default;
		}

		public void Vertical()
		{
			LineType = SingleLineType.Vertical;
		}

		public void Horizontal()
		{
			LineType = SingleLineType.Horizontal;
		}

		public override int InitNum
		{
			get
			{
				return 1;
			}
		}

		public override int ControlPointNum
		{
			get
			{
				return 1;
			}
		}

		public override void CalcPoint()
		{
			pfStart = new PointF[1];
			pfEnd = new PointF[1];

			pfStart[0] = ToPointF(ControlPoints[0]);
			pfEnd[0] = pfStart[0];
			if (lineType==SingleLineType.Horizontal)
				pfEnd[0].X++;
			else pfEnd[0].Y++;
			ExpandLine2(ref pfStart[0],ref pfEnd[0]);
		}

		public override string GetStr()
		{
			if (lineType==SingleLineType.Horizontal) 
			{
				PointF p = ToPointF(ControlPoints[0]);
				double Value = Area.AxisY.GetValueFromY(p.Y);
				return Value.ToString(DataFormat);
			} 
			else 
			{
				IDataProvider idp = Manager.Canvas.BackChart.DataProvider;
				double[] dd = idp["DATE"];
				double d = dd[FormulaChart.FindIndex(dd,ControlPoints[0].X)];
				return DateTime.FromOADate(d).ToString(DataFormat);
			}
		}

		public override RectangleF GetTextRect()
		{
			return base.GetTextRect(0,lineType==SingleLineType.Horizontal);
		}
	}
}