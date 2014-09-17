using System;
using System.Xml.Serialization;
using System.Drawing;
using System.ComponentModel;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	public enum SingleLineType {Vertical,Horizontal};
	/// <summary>
	/// Summary description for SingleLine.
	/// </summary>
	public class SingleLineObject : LineGroupTextObject
	{
		string dataFormat = "f2";
		[XmlAttribute]
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
		
		SingleLineType lineType = SingleLineType.Horizontal;
		[RefreshProperties(RefreshProperties.All),DefaultValue(SingleLineType.Horizontal),XmlAttribute]
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
					ObjectFont.LineAlignment = StringAlignment.Near;
				}
				else 
				{
					dataFormat = "yyyy-MM-dd";
					ObjectFont.Alignment = StringAlignment.Far;
				}
			}
		}

		bool openStart = true;
		[DefaultValue(true),XmlAttribute]
		public bool OpenStart
		{
			get
			{
				return openStart;
			}
			set
			{
				openStart = value;
			}
		}

		bool openEnd = true;
		[DefaultValue(true),XmlAttribute]
		public bool OpenEnd
		{
			get
			{
				return openEnd;
			}
			set
			{
				openEnd = value;
			}
		}

		public SingleLineObject()
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
			if (InMove) 
				SetSnapLine(0,OpenStart,OpenEnd);

			pfStart = new PointF[1];
			pfEnd = new PointF[1];

			pfStart[0] = ToPointF(ControlPoints[0]);
			pfEnd[0] = pfStart[0];
			if (lineType==SingleLineType.Horizontal)
				pfEnd[0].X++;
			else pfEnd[0].Y++;
			
			PointF pfOldStart = pfStart[0];

			if (openStart)
				ExpandLine(ref pfEnd[0],ref pfStart[0]);
			if (openEnd)
				ExpandLine(ref pfOldStart,ref pfEnd[0]); //pfStart[0]
			//ExpandLine2(ref pfStart[0],ref pfEnd[0]);
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
				FormulaChart BackChart = Manager.Canvas.Chart;
				int i = BackChart.DateToIndex(ControlPoints[0].X);
				DateTime D = BackChart.IndexToDate(i);
				return D.ToString(DataFormat);
			}
		}

		public override RectangleF GetTextRect()
		{
			return base.GetTextRect(0,lineType==SingleLineType.Horizontal);
		}

		public override ObjectInit[] RegObject()
		{
			return 
				new ObjectInit[]{
									   new ObjectInit("Vertical line",typeof(SingleLineObject),"Vertical","Line","VLine"),
									   new ObjectInit("Horizontal line",typeof(SingleLineObject),"Horizontal","Line","HLine"),
								   };
		}
	}
}