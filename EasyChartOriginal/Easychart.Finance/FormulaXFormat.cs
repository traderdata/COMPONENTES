using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for FormulaXFormat.
	/// </summary>
	public class FormulaXFormat
	{

		private bool[] visible;
		private bool[] showMajorLine;
		private bool[] showMinorLine;

		private double days100Pixel = 0;
		[DefaultValue(0.0),XmlAttribute]
		public double Days100Pixel
		{
			get
			{
				return days100Pixel;
			}
			set
			{
				days100Pixel = value;
			}
		}

		private DataCycle interval;
		[DefaultValue(typeof(DataCycle),"DAY1")]
		public DataCycle Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}

		private string xFormat = "";
		[DefaultValue(""), XmlAttribute]
		public string XFormat
		{
			get
			{
				return xFormat;
			}
			set
			{
				xFormat = value;
			}
		}

		private double cycleDivide;
		[XmlAttribute]
		public double CycleDivide
		{
			get
			{
				return cycleDivide;
			}
			set
			{
				cycleDivide = value;
			}
		}

		private string xCursorFormat = "yyyy-MM-dd dddd";
		[DefaultValue("yyyy-MM-dd dddd"),XmlAttribute]
		public string XCursorFormat
		{
			get
			{
				return xCursorFormat;
			}
			set
			{
				xCursorFormat = value;
			}
		}

		private AxisLabelAlign axisLabelAlign = AxisLabelAlign.TickRight;
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

		public bool[] Visible
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

		public bool[] ShowMajorLine
		{
			get
			{
				return showMajorLine;
			}
			set
			{
				showMajorLine = value;
			}
		}

		public bool[] ShowMinorLine
		{
			get
			{
				return showMinorLine;
			}
			set
			{
				showMinorLine = value;
			}
		}

		public FormulaXFormat()
		{
		}

		public FormulaXFormat(double Days100Pixel,string Interval,string XFormat):this()
		{
			this.days100Pixel = Days100Pixel;
			this.interval = DataCycle.Parse(Interval);
			this.xFormat = XFormat;
		}

		public FormulaXFormat(double Days100Pixel,string Interval,string XFormat,double CycleDivide):
			this(Days100Pixel,Interval,XFormat)
		{
			this.cycleDivide = CycleDivide;
		}

		public FormulaXFormat(double Days100Pixel,string Interval,string XFormat,double CycleDivide,string XCursorFormat):
			this(Days100Pixel,Interval,XFormat,CycleDivide)
		{
			this.xCursorFormat = XCursorFormat;
		}

		public FormulaXFormat(double Days100Pixel,string Interval,string XFormat,double CycleDivide,string XCursorFormat,AxisLabelAlign AxisLabelAlign):
			this(Days100Pixel,Interval,XFormat,CycleDivide,XCursorFormat)
		{
			this.axisLabelAlign = AxisLabelAlign;
		}

		public FormulaXFormat(double Days100Pixel,string Interval,string XFormat,double CycleDivide,bool[] Visible,bool[] ShowMajorLine,bool[] ShowMinorLine):
			this(Days100Pixel,Interval,XFormat,CycleDivide)
		{
			this.visible = Visible;
			this.showMajorLine = ShowMajorLine;
			this.showMinorLine = ShowMinorLine;
		}

		public void SetVisible(FormulaChart fc)
		{
			for(int i=0; i<Visible.Length; i++)
				fc.SetAxisXVisible(i,Visible[i]);
		}

		public void SetMajorLine(FormulaChart fc)
		{
			for(int i=0; i<ShowMajorLine.Length; i++)
				fc.SetAxisXShowMajorLine(i,ShowMajorLine[i]);
		}

		public void SetMinorLine(FormulaChart fc)
		{
			for(int i=0; i<ShowMinorLine.Length; i++)
				fc.SetAxisXShowMinorLine(i,ShowMinorLine[i]);
		}

		public override string ToString()
		{
			return days100Pixel+"-"+interval;
		}

	}

	public class XFormatCollection:CollectionBase
	{
		public virtual void Add(FormulaXFormat fxf)
		{
			this.List.Add(fxf);
		}

		public virtual FormulaXFormat this[int Index] 
		{
			get
			{
				return (FormulaXFormat)this.List[Index];
			}
		}

		public static XFormatCollection Default
		{
			get
			{
				XFormatCollection xfc = new XFormatCollection();
				xfc.Add(new FormulaXFormat(0.001,"MINUTE","{$MMMdd }H:mm",double.NaN,"yyyy-MM-dd HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.005,"MINUTE5","{$MMMdd }H:mm",double.NaN,"yyyy-MM-dd HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.01,"MINUTE10","{$MMMdd }H:mm",double.NaN,"yyyy-MM-dd HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.03,"MINUTE30","{$MMMdd }H:mm",double.NaN,"yyyy-MM-dd HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.06,"HOUR","{$MMMdd }H:mm",double.NaN,"yyyy-MM-dd HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.2,"HOUR2","{$MMMdd }HH:mm",double.NaN,"yyyy-MM-dd HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.8,"HOUR8","{$MMMdd }HH:mm",double.NaN,"yyyy-MM-dd HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(1,"DAY","{MMM}dd"));
				xfc.Add(new FormulaXFormat(9,"DAY","{MMM}dd",0.6));
				xfc.Add(new FormulaXFormat(14,"WEEK","{yyyy}MMMdd"));
				xfc.Add(new FormulaXFormat(40,"MONTH","{yyyy}MMM"));
				xfc.Add(new FormulaXFormat(80,"MONTH2","{yyyy}MMM"));
				xfc.Add(new FormulaXFormat(160,"MONTH4","{yyyy}MMM"));
				xfc.Add(new FormulaXFormat(570,"YEAR1","yyyy"));
				xfc.Add(new FormulaXFormat(1100,"YEAR2","yyyy"));
				xfc.Add(new FormulaXFormat(int.MaxValue,"YEAR","yyyy",500));
				return xfc;
			}
		}

		static public XFormatCollection AsxFormat
		{
			get
			{
				XFormatCollection xfc = new XFormatCollection();
				xfc.Add(new FormulaXFormat(0.001,"MINUTE","{$MMMdd }H:mm",double.NaN,"dd MMM yyyy HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.005,"MINUTE5","{$MMMdd }H:mm",double.NaN,"dd MMM yyyy HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.01,"MINUTE10","{$MMMdd }H:mm",double.NaN,"dd MMM yyyy HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.03,"MINUTE30","{$MMMdd }H:mm",double.NaN,"dd MMM yyyy HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.06,"HOUR","{$MMMdd }H:mm",double.NaN,"dd MMM yyyy HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.2,"HOUR2","{$MMMdd }HH:mm",double.NaN,"dd MMM yyyy HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(0.8,"HOUR8","{$MMMdd }HH:mm",double.NaN,"dd MMM yyyy HH:mm:ss",AxisLabelAlign.TickCenter));
				xfc.Add(new FormulaXFormat(1,"DAY","{MMM}dd",double.NaN,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(9,"DAY","{MMM}dd",0.6,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(14,"WEEK","{yyyy}MMMdd",double.NaN,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(40,"MONTH","{yyyy}MMM",double.NaN,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(80,"MONTH2","{yyyy}MMM",double.NaN,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(160,"MONTH4","{yyyy}MMM",double.NaN,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(570,"YEAR1","yyyy",double.NaN,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(1100,"YEAR2","yyyy",double.NaN,"dd MMM yyyy"));
				xfc.Add(new FormulaXFormat(int.MaxValue,"YEAR","yyyy",500,"dd MMM yyyy"));
				return xfc;
			}
		}

		public static XFormatCollection TwoAxisX
		{
			get
			{
				XFormatCollection xfc = new XFormatCollection();

				xfc.Add(new FormulaXFormat(0.001,"MINUTE","{$MMMdd }H:mm",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(0.005,"MINUTE5","{$MMMdd }H:mm",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(0.01,"MINUTE10","{$MMMdd }H:mm",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(0.03,"MINUTE30","{$MMMdd }H:mm",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(0.06,"HOUR","{$MMMdd }H:mm",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(0.3,"HOUR2","{$MMMdd }h:mm",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(0.8,"HOUR8","{$MMMdd }h:mm",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));

				xfc.Add(new FormulaXFormat(5,"DAY","%d",double.NaN,
					new bool[]{true,true},new bool[]{true,true},new bool[]{true,false}));

				xfc.Add(new FormulaXFormat(10,"WEEK","%d",double.NaN,
					new bool[]{true,true},new bool[]{true,true},new bool[]{true,false}));
				xfc.Add(new FormulaXFormat(40,"WEEK2","%d",double.NaN,
					new bool[]{true,true},new bool[]{true,true},new bool[]{true,false}));
				xfc.Add(new FormulaXFormat(80,"MONTH2","{yyyy}MMM",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(160,"MONTH4","{yyyy}MMM",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(570,"YEAR1","yyyy",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(1100,"YEAR2","yyyy",double.NaN,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				xfc.Add(new FormulaXFormat(int.MaxValue,"YEAR","yyyy",500,
					new bool[]{true,false},new bool[]{true,false},new bool[]{false,false}));
				return xfc;
			}
		}
	}
}