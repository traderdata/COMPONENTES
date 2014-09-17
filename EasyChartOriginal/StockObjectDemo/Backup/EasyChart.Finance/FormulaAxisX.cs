using System;
using System.Xml.Serialization;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;

namespace Easychart.Finance
{
	/// <summary>
	/// X axis of a Formula area
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FormulaAxisX
	{
		private FormulaData fdDate;
		private double Total=-1;
		private double OneDay=-1;

		[Browsable(false), XmlIgnore]
		public ExchangeIntraday IntradayInfo;

		[Browsable(false), XmlIgnore]
		public Rectangle Rect;
		
		[Browsable(false), XmlIgnore]
		public RectangleF LastCursorRect;
		
		private bool visible = true;
		[DefaultValue(true),XmlAttribute]
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}

		private bool autoScale;
		[DefaultValue(false),XmlAttribute]
		public bool AutoScale
		{
			get
			{
				return autoScale;
			}
			set
			{
				autoScale = value;
			}
		}

		private int height = 16;
		[DefaultValue(16),XmlAttribute]
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}
		
		private string format = "yyMMM";
		[DefaultValue("yyMMM"),XmlAttribute]
		public string Format 
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
			}
		}

		[Browsable(false), XmlIgnore]
		public IFormatProvider DateFormatProvider;

		private FormulaTick majorTick;
		public FormulaTick MajorTick
		{
			get
			{
				return majorTick;
			}
			set
			{
				majorTick = value;
			}
		}

		private FormulaTick minorTick;
		public FormulaTick MinorTick
		{
			get
			{
				return minorTick;
			}
			set
			{
				minorTick = value;
			}
		}

		private FormulaBack back;
		public FormulaBack Back
		{
			get
			{
				return back;
			}
			set
			{
				back = value;
			}
		}

		private Font labelFont = new Font("verdana",8);
		[XmlIgnore]
		public Font LabelFont 
		{
			get
			{
				return labelFont;
			}
			set
			{
				labelFont = value;
			}
		}

		private BrushMapper labelBrush = new BrushMapper(Color.Black);
		public BrushMapper LabelBrush 
		{
			get
			{
				return labelBrush;
			}
			set
			{
				labelBrush = value;
			}
		}

		private AxisLabelAlign axisLabelAlign;
		[DefaultValue(AxisLabelAlign.TickRight),XmlAttribute]
		public AxisLabelAlign AxisLabelAlign
		{
			get
			{
				return axisLabelAlign;
			}
			set
			{
				axisLabelAlign = value;
			}
		}

		[XmlIgnore]
		public DateTime StartTime = DateTime.MinValue;
		[XmlIgnore]
		public DateTime EndTime = DateTime.MaxValue;

		[Browsable(false),XmlIgnore]
		public DataCycle DataCycle = DataCycle.Month;
		
		private string cursorFormat = "dd-MMM-yyyy dddd";
		[DefaultValue("dd-MMM-yyyy dddd"),XmlAttribute]
		public string CursorFormat 
		{
			get
			{
				return cursorFormat;
			}
			set
			{
				cursorFormat = value;
			}
		}
		
		/// <summary>
		/// Copy attribute from another FormulaAxisX
		/// </summary>
		/// <param name="fax"></param>
		public void CopyFrom(FormulaAxisX fax) 
		{
			visible = fax.visible;
			DateFormatProvider = fax.DateFormatProvider;
			autoScale = fax.autoScale;
			format = fax.format;
			majorTick = fax.majorTick.Clone();
			minorTick = fax.minorTick.Clone();
			back = (FormulaBack)fax.back.Clone();
			labelFont = (Font)fax.labelFont.Clone();
			labelBrush = fax.labelBrush.Clone();
			axisLabelAlign = fax.axisLabelAlign;
			DataCycle = fax.DataCycle;
			cursorFormat = fax.cursorFormat;
			height = fax.height;
		}

		public FormulaAxisX() 
		{
			Back = new FormulaBack();

			MajorTick = new FormulaTick();
			MajorTick.ShowLine = true;
			MajorTick.Inside = true;

			MinorTick = new FormulaTick();
			MinorTick.Count = 1;
			MinorTick.Inside = true;
			MinorTick.TickWidth = 2;
		}

		[Browsable(false),XmlIgnore]
		public bool IsFixedTime
		{
			get 
			{
				return StartTime > DateTime.MinValue;
			}
		}

		private double FromStartTime(double D)
		{
			if (OneDay==-1)
				OneDay = IntradayInfo.GetOpenTimePerDay();
			double d1 = StartTime.ToOADate();
			if ((int)d1==(int)D)
				return IntradayInfo.OneDayTime(D)-IntradayInfo.OneDayTime(d1);
			else return OneDay-IntradayInfo.OneDayTime(d1)+IntradayInfo.OneDayTime(D)+
					 IntradayInfo.RawDaysBetween(d1,D)*OneDay;
		}

		/// <summary>
		/// Get X position of certain date
		/// </summary>
		/// <param name="D">Ole Date</param>
		/// <param name="x1">Start position</param>
		/// <param name="x2">End position</param>
		/// <returns></returns>
		public float GetX(double D,int x1,int x2)
		{
			if (Total==-1)
				Total =  FromStartTime(EndTime.ToOADate());
			double Current = FromStartTime(D);
			return (float)(x1+(x2-x1)/Total*Current);
		}

		public void Prepare()
		{
			Total = -1;
		}

		/// <summary>
		/// Render the X-Axis
		/// </summary>
		/// <param name="Canvas">FormulaCanvas</param>
		/// <param name="fa">FormulaArea</param>
		public void Render(FormulaCanvas Canvas,FormulaArea fa)
		{
			if (Visible)
			{
				if (fa.AxisY.AxisPos == AxisPos.Left)
					Rect.X--;
				Graphics g = Canvas.CurrentGraph;
				Back.Render(g,Rect);
			}

			if (majorTick.Visible || minorTick.Visible) 
			{
				double[] Date;
				Date = fa.Parent.DataProvider["DATE"];
				fdDate = new FormulaData(Date);
				fdDate.Canvas = Canvas;
				fdDate.AxisY = fa.AxisY;

				PointF[] pfs = fdDate.GetPoints();

				majorTick.DataCycle=DataCycle;
				majorTick.Format=Format;
				majorTick.DateFormatProvider = DateFormatProvider;

				majorTick.DrawXAxisTick(Canvas,Date,fdDate,pfs,this,IntradayInfo);
				minorTick.DrawXAxisTick(Canvas,Date,fdDate,pfs,this,IntradayInfo);
			}
		}

		/// <summary>
		/// Draw date label at X
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fc"></param>
		/// <param name="Area"></param>
		/// <param name="X"></param>
		public void DrawCursor(Graphics g,FormulaChart fc,FormulaArea Area,float X)
		{
			if (!LastCursorRect.IsEmpty)
				fc.RestoreMemBmp(g,LastCursorRect);
			
			FormulaLabel fl = Area.Labels[2];
			int i = fc.CursorPos;
			if ((object)fdDate!=null)
			if (i>=0)
			{
				DateTime D = fc.IndexToDate(i);
				string s = D.ToString(CursorFormat); //,DateTimeFormatInfo.InvariantInfo
				SizeF sf = g.MeasureString(s,LabelFont);

				RectangleF R = new RectangleF(X-fc.Rect.X,Rect.Y,sf.Width,Rect.Height-1);
				if (R.Right>Rect.Right)
					R.Offset(-R.Width-1,0);
				LastCursorRect = R;
				LastCursorRect.Inflate(2,1);
				R.Offset(fc.Rect.Location);

				fl.DrawString(g,s,LabelFont,fl.TextBrush,VerticalAlign.Bottom,FormulaAlign.Left,R,false);
			}
		}

		public override string ToString()
		{
			return "AxisX-"+format;
		}
	}

	/// <summary>
	/// Collection of X-Axis
	/// </summary>
	public class AxisXCollection:CollectionBase
	{
		public virtual int Add(FormulaAxisX fax)
		{
			return List.Add(fax);
		}

		public virtual FormulaAxisX this[int Index] 
		{
			get
			{
				if (Index<List.Count)
					return (FormulaAxisX)this.List[Index];
				else return null;
			}
		}

		public void Remove(FormulaAxisX value) 
		{
			List.Remove(value);
		}
	}
}
