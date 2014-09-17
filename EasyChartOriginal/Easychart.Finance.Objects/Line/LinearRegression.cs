using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	public enum RegressionType {Channel,AsynChannel,StdChannel,StdErrorChannel,UpDownTrend}

	/// <summary>
	/// Summary description for LinearRegression.
	/// </summary>
	public class LinearRegressionObject : LineGroupObject
	{
		RegressionType regressionType;
		double percentage = 1;
		bool showAuxLine = false;
		bool upLine = true;
		bool downLine = true;
		bool centerLine = true;

		FormulaData fd;
		int Bar1;
		int Bar2;
		double A;
		double B;

		SegmentCollection scLines = new SegmentCollection();

		private bool openStart = false;
		private bool openEnd = false;
		[DefaultValue(false),XmlAttribute]
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

		[DefaultValue(false),XmlAttribute]
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

		public LinearRegressionObject()
		{
		}

		[DefaultValue(false),XmlAttribute]
		public bool ShowAuxLine 
		{
			get
			{
				return showAuxLine;
			}
			set
			{
				showAuxLine = value;
			}
		}

		[DefaultValue(true),XmlAttribute]
		public bool UpLine 
		{
			get
			{
				return upLine;
			}
			set
			{
				upLine = value;
			}
		}

		[DefaultValue(true),XmlAttribute]
		public bool DownLine 
		{
			get
			{
				return downLine;
			}
			set
			{
				downLine = value;
			}
		}

		[DefaultValue(true),XmlAttribute]
		public bool CenterLine 
		{
			get
			{
				return centerLine;
			}
			set
			{
				centerLine = value;
			}
		}

		public void InitSingle()
		{
			regressionType = RegressionType.Channel;
			upLine = false;
			downLine = false;
		}

		public void InitChannel()
		{
			regressionType = RegressionType.Channel;
		}

		public void InitOpenChannel()
		{
			regressionType = RegressionType.Channel;
			openEnd = true;
		}

		public void InitAsynChannel()
		{
			regressionType = RegressionType.AsynChannel;
		}

		public void InitStdChannel()
		{
			regressionType = RegressionType.StdChannel;
		}

		public void InitStdErrorChannel()
		{
			regressionType = RegressionType.StdErrorChannel;
		}

		public void InitUpTrend()
		{
			regressionType = RegressionType.UpDownTrend;
			downLine = false;
		}

		public void InitDownTrend()
		{
			regressionType = RegressionType.UpDownTrend;
			upLine = false;
		}

		public void InitUpDownTrend()
		{
			regressionType = RegressionType.UpDownTrend;
		}

		[DefaultValue(RegressionType.Channel),XmlAttribute]
		public RegressionType RegressionType
		{
			get
			{
				return regressionType;
			}
			set
			{
				regressionType = value;
			}
		}

		[DefaultValue(1),XmlAttribute]
		public double Percentage
		{
			get
			{
				return percentage;
			}
			set
			{
				percentage = value;
			}
		}


		private float Std(bool Error)  
		{
			double[] dd = fd.Data;
			double MA = 0;
			int BarE = Math.Min(Bar2,dd.Length);
			int BarB = Math.Min(Bar1,dd.Length);
			if (BarE==BarB)
				return 0;
			for(int i = BarB; i<BarE; i++)
				MA +=dd[i];
			MA/=BarE-BarB;
			double R = 0;
			for(int i = BarB; i<BarE; i++)
				R +=(dd[i]-MA)*(dd[i]-MA);
			R /=(BarE-BarB)-(Error?0:1);
			R = Math.Sqrt(R);
			return (float)R;
		}


		private void AddLine(ObjectPoint op1,ObjectPoint op2,float Delta)
		{
			op1.Y -=Delta*percentage;
			op2.Y -=Delta*percentage;
			scLines.Add(op1,op2);
		}

		public override void CalcPoint()
		{
			if (Area.FormulaDataArray.Count==0)
				return;
			fd = Area.FormulaDataArray[0];
			//IDataProvider idp = Manager.Canvas.BackChart.DataProvider;
			//double[] dd = idp["DATE"];
			FormulaChart BackChart = Manager.Canvas.Chart;
			int i1 = 0;
			int i2 = 1;
			if (ControlPoints[0].X>ControlPoints[1].X)
			{
				i1 = 1;
				i2 = 0;
			}
			//Bar1 = FormulaChart.FindIndex(dd,ControlPoints[i1].X);
			//Bar2 = FormulaChart.FindIndex(dd,ControlPoints[i2].X);
			Bar1 = BackChart.DateToIndex(ControlPoints[i1].X);
			Bar2 = BackChart.DateToIndex(ControlPoints[i2].X);

			if (regressionType!=RegressionType.UpDownTrend) 
				FormulaBase.CalcLinearRegression(fd,Bar2,Bar2-Bar1,out A,out B);
			else 
			{
				A = ControlPoints[i1].Y;
				B = (ControlPoints[i2].Y-ControlPoints[i1].Y)/(Bar2-Bar1);
			}

			ObjectPoint opStart = ControlPoints[i1];
			ObjectPoint opEnd = ControlPoints[i2];
			opStart.Y = A;
			opEnd.Y = A+B*(Bar2-Bar1);

			scLines.Clear();
			if (centerLine)
				scLines.Add(opStart,opEnd);
			float Delta;
			if (regressionType==RegressionType.Channel || 
				regressionType==RegressionType.AsynChannel || 
				regressionType==RegressionType.UpDownTrend)
			{
				float dtUp = CalcDelta(fd,A,B,Bar1,Bar2,"H",true);
				float dtDown = CalcDelta(fd,A,B,Bar1,Bar2,"L",false);
				if (regressionType==RegressionType.Channel)
					Delta = Math.Max(Math.Abs(dtUp),Math.Abs(dtDown));
				else Delta = -dtUp;
				if (upLine)
					AddLine(opStart,opEnd,Delta);

				if (regressionType!=RegressionType.Channel)
					Delta = dtDown;
				if (downLine)
					AddLine(opStart,opEnd,-Delta);
			} 
			else if (regressionType==RegressionType.StdChannel || 
				regressionType==RegressionType.StdErrorChannel)
			{
				Delta = Std(regressionType==RegressionType.StdErrorChannel);
				AddLine(opStart,opEnd,Delta);
				AddLine(opStart,opEnd,-Delta);
			} 

			pfStart = new PointF[scLines.Count];
			pfEnd = new PointF[scLines.Count];
			for(int j=0; j<scLines.Count; j++)
			{
				ObjectSegment os = scLines[j];
				if (!double.IsNaN(os.op1.Y) && !double.IsNaN(os.op2.Y))
				{
					pfStart[j] = ToPointF(os.op1);
					pfEnd[j] = ToPointF(os.op2);
				} 
				else 
				{
					pfStart[j] = PointF.Empty;
					pfEnd[j] = PointF.Empty;
				}
			}
			OpenStartEnd(openStart,openEnd);
		}

		public override System.Drawing.RectangleF GetMaxRect()
		{
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);

			Rectangle R = Area.Canvas.Rect;
			p1.Y = R.Y;
			p2.Y = R.Bottom;
			if (openStart) p1.X = R.Left;
			if (openEnd) p2.X = R.Right;
			return base.GetMaxRect(new PointF[]{p1,p2});
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw (g);

			Pen P = LinePen.GetPen();
			PointF p1 = ToPointF(ControlPoints[0]);
			PointF p2 = ToPointF(ControlPoints[1]);
			Rectangle R = Area.Canvas.Rect;

			if ((InMove || showAuxLine) && p1!=PointF.Empty && p2!=PointF.Empty)
			{
				g.DrawLine(P,p1.X,R.Y,p1.X,R.Bottom);
				g.DrawLine(P,p2.X,R.Y,p2.X,R.Bottom);
			}
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
				new ObjectInit("Linear Regression",typeof(LinearRegressionObject),"InitSingle","LinearReg","LRSingle",200),
				new ObjectInit("Channel Linear Regression",typeof(LinearRegressionObject),"InitChannel","LinearReg","LRChannel"),
				new ObjectInit("Open Channel Linear Regression",typeof(LinearRegressionObject),"InitOpenChannel","LinearReg","LROChannel"),
				new ObjectInit("Asyn Channel Linear Regression",typeof(LinearRegressionObject),"InitAsynChannel","LinearReg","LRAChannel"),
				new ObjectInit("Std Channel",typeof(LinearRegressionObject),"InitStdChannel","LinearReg","LRSTD"),
				new ObjectInit("Std Error Channel",typeof(LinearRegressionObject),"InitStdErrorChannel","LinearReg","LRError"),
				new ObjectInit("Up Trend Channel",typeof(LinearRegressionObject),"InitUpTrend","LinearReg","LRUp"),
				new ObjectInit("Down Trend Channel",typeof(LinearRegressionObject),"InitDownTrend","LinearReg","LRDown"),
				new ObjectInit("Up/Down Trend Channel",typeof(LinearRegressionObject),"InitUpDownTrend","LinearReg","LRUpDown"),
				};
		}
	}
}