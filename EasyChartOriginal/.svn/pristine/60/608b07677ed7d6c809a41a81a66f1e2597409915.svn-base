using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.IO;

namespace Easychart.Finance
{
	public enum AxisLabelAlign{TickRight,TickCenter,TickLeft,Center};
	public enum AxisPos {Left,Right};

	/// <summary>
	/// Define the scale type of the finance chart
	/// </summary>
	public enum ScaleType 
	{
		/// <summary>
		/// Normal line
		/// </summary>
		Normal,
		/// <summary>
		/// Logarithm line
		/// </summary>
		Log,
		/// <summary>
		/// Square Root
		/// </summary>
		SquareRoot,
		/// <summary>
		/// Use default scale type
		/// </summary>
		Default,
	};

	public delegate double TransformFunc(double d);

	/// <summary>
	/// Y axis of a Formula area
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FormulaAxisY
	{
		private float[] LabelPos;
		private double[] LabelValues;

		[Browsable(false), XmlIgnore]
		public double[] CustomLine;

		[Browsable(false), XmlIgnore]
		public bool IsBond;

		[Browsable(false), XmlIgnore]
		public TransformFunc Transform;

		[Browsable(false), XmlIgnore]
		public TransformFunc RevertTransform;

		[Browsable(false), XmlIgnore]
		public RectangleF LastCursorRect;

		[Browsable(false), XmlIgnore]
		internal bool LineBinded;

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

//		[Browsable(false), XmlIgnore]
//		public bool ChangedByUser = false;

		private bool autoScale = true;
		/// <summary>
		/// Scale the max/min value of Y-Axis automatically
		/// </summary>
		[DefaultValue(true),XmlAttribute]
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

		private double ZeroSqrt(double d)
		{
			if (d<0) 
				return 0;
			else return Math.Sqrt(d);
		}

		private double Sqr(double d) 
		{
			return d*d;
		}

		private ScaleType scale = ScaleType.Normal;
		[DefaultValue(ScaleType.Normal), XmlAttribute]
		public ScaleType Scale
		{
			set 
			{
				scale = value;
				if (value == ScaleType.Normal) 
				{
					Transform = null;
					RevertTransform = null;
				}
				else if (value == ScaleType.Log) 
				{
					Transform = new TransformFunc(ZeroLog);
					RevertTransform = new TransformFunc(Math.Exp);
				}
				else if (value == ScaleType.SquareRoot) 
				{
					Transform = new TransformFunc(ZeroSqrt);
					RevertTransform = new TransformFunc(Sqr);
				}
			}
			get 
			{
				return scale;
			}
		}

		[Browsable(false), XmlIgnore]
		public Rectangle Rect;
		[Browsable(false), XmlIgnore]
		public Rectangle FrameRect;

		private bool autoMultiply = true;
		/// <summary>
		/// Calculate multiply factor automatically
		/// </summary>
		[DefaultValue(true),XmlAttribute]
		public bool AutoMultiply 
		{
			get
			{
				return autoMultiply;
			}
			set
			{
				autoMultiply = value;
			}
		}

		private double multiplyFactor = 1;
		/// <summary>
		/// Multiply the stock data by this factor
		/// </summary>
		[DefaultValue(1),XmlAttribute]
		public double MultiplyFactor
		{
			get
			{
				return multiplyFactor;
			}
			set
			{
				multiplyFactor = value;
			}
		}

		/// <summary>
		/// AxisY 's width
		/// </summary>
		private int width = 80;
		[DefaultValue(80),XmlAttribute]
		public int Width 
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

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

		private FormulaBack multiplyBack;
		public FormulaBack MultiplyBack
		{
			get
			{
				return multiplyBack;
			}
			set
			{
				multiplyBack = value;
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
		
		private AxisPos axisPos = AxisPos.Right;
		[DefaultValue(AxisPos.Right),XmlAttribute]
		public AxisPos AxisPos 
		{
			get
			{
				return axisPos;
			}
			set
			{
				axisPos = value;
			}
		}

		private double refValue = 0;
		[DefaultValue(0),XmlIgnore,Browsable(false)]
		public double RefValue
		{
			get
			{
				return refValue;
			}
			set
			{
				refValue = value;
			}
		}

		private bool showAsPercent = false;
		[DefaultValue(false),XmlIgnore,Browsable(false)]
		public bool ShowAsPercent
		{
			get
			{
				return showAsPercent;
			}
			set
			{
				showAsPercent = value;
			}
		}

		private bool autoFormat = true;
		[DefaultValue(true),XmlAttribute]
		public bool AutoFormat
		{
			get
			{
				return autoFormat;
			}
			set
			{
				autoFormat = value;
			}
		}

		private string format = "";
		[DefaultValue(""),XmlAttribute]
		public string Format 
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
				AutoFormat = value=="" || value==null;
			}
		}

		[Browsable(false), XmlIgnore]
		public double MaxY = double.MaxValue;
		[Browsable(false), XmlIgnore]
		public double MinY = double.MinValue;

		/// <summary>
		/// Copy attribute from another FormulaAxisY
		/// </summary>
		/// <param name="fay"></param>
		public void CopyFrom(FormulaAxisY fay) 
		{
			visible = fay.visible;
			autoMultiply = fay.autoMultiply;
			width = fay.width;
			//autoScale = fay.autoScale;
			majorTick = fay.majorTick.Clone();
			minorTick = fay.minorTick.Clone();
			back = (FormulaBack)fay.back.Clone();
			autoFormat = fay.autoFormat;
			format = fay.format;
			multiplyBack = (FormulaBack)fay.multiplyBack.Clone();
			labelFont = (Font)fay.labelFont.Clone();
			labelBrush = fay.labelBrush.Clone();
			axisPos  = fay.axisPos;
		}

		/// <summary>
		/// Constructor of Y-Axis
		/// </summary>
		public FormulaAxisY()
		{
			majorTick = new FormulaTick();
			majorTick.ShowLine = true;

			minorTick = new FormulaTick();
			minorTick.TickWidth = 3;
			minorTick.MinimumPixel = 10;

			Back = new FormulaBack();
			MultiplyBack = new FormulaBack();
			MultiplyBack.BackGround = new BrushMapper(Color.Yellow);//new SolidBrush(Color.Yellow);
		}

		private string MultiplyFactorToString() 
		{
			if (MultiplyFactor>=1000000)
				return "x"+(MultiplyFactor/1000000)+"M";
			else if (MultiplyFactor>=1000)
				return "x"+(MultiplyFactor/1000)+"K";
			else return "x"+MultiplyFactor.ToString();
		}

		private double ZeroLog(double d) 
		{
			if (d<=0)
				//return double.NaN;
				return 0;
			return Math.Log(Math.Abs(d));
		}

		private int ZeroLog10(double d) 
		{
			if (d==0)
				return 0;
			return (int)Math.Log10(Math.Abs(d));
		}

		private void CalcLableLine(FormulaCanvas Canvas)
		{
			double CalcMinY = this.MinY;
			double CalcMaxY = this.MaxY;
			if (ShowAsPercent && !double.IsNaN(RefValue))
			{
				CalcMinY =CalcMinY/RefValue-1;
				CalcMaxY =CalcMaxY/RefValue-1;
			}

			double SpanY = CalcMaxY-CalcMinY;

			double Step = Math.Pow(10,Math.Floor(Math.Log10(SpanY))+1);
			double[] DivStep = {2,2.5,2};

			if (IsBond && Step<1)
				Step = 1;
			for(int k=0; SpanY/Step*Canvas.LabelHeight*3<Canvas.Rect.Height; k++)
				if (IsBond && Step<=1)
					Step /=2;
				else Step /=DivStep[k % DivStep.Length];

			double LabelMinY = Math.Floor(CalcMinY/Step)*Step;
			double LabelMaxY = Math.Ceiling(CalcMaxY/Step)*Step;

			int N = (int)((LabelMaxY-LabelMinY)/Step);

			bool IsCustom =CustomLine!=null && CustomLine.Length>0;
			if (IsCustom)
				N = CustomLine.Length-1;

			LabelPos = new float[N+1];
			LabelValues = new double[N+1];
			for(int i=0; i<=N; i++)
			{
				if (IsCustom)
					LabelValues[i] = CustomLine[i];
				else 
					LabelValues[i] = LabelMinY+Step*i;
				if (ShowAsPercent && !double.IsNaN(RefValue)) 
					LabelPos[i] = CalcY((LabelValues[i]+1)*RefValue);
				else LabelPos[i] = CalcY(LabelValues[i]);
			}

			if (AutoMultiply && !ShowAsPercent)
			{
				MultiplyFactor = 1;
				int[] L = {ZeroLog10(CalcMinY),ZeroLog10(CalcMaxY)};
				if (Math.Sign(L[1])!=Math.Sign(L[0]))
					L[0] = 0;
				foreach(int i in L) 
				{
					if (i>3)
						MultiplyFactor = Math.Max(MultiplyFactor,Math.Pow(10,i-3));
					else if (i<-1)
						MultiplyFactor = Math.Min(MultiplyFactor,Math.Pow(10,i+1));
				}
			}
		}

		private void SetBestFormat(FormulaCanvas Canvas,FormulaArea Area,int Start)
		{
			int k = Start;
			for(int i=0; i<LabelValues.Length; i++)
			{
				double d = (LabelValues[i]/MultiplyFactor);
				k = FormulaHelper.TestBestFormat(d,k);
			}

			bool b = false;
			for(int i = 0; i<Area.FormulaDataArray.Count; i++) 
			{
				FormulaRenderType frt = (Area.FormulaDataArray[i] as FormulaData).RenderType;
				if (frt==FormulaRenderType.STOCK)
				{
					double d = GetLastValue(Canvas,Area,i);
					if (!double.IsNaN(d)) 
					{
						k = FormulaHelper.TestBestFormat(d,k);
						b = true;
					}
				}
			}
				
			if (format!=null && format.Length>1 && !b)
				format = "f"+format.Substring(1);
			else format = "f"+k; 
		}

		/// <summary>
		/// Calc Y-axis width
		/// </summary>
		/// <param name="Canvas"></param>
		/// <returns></returns>
		public int CalcLabelWidth(FormulaCanvas Canvas,FormulaArea Area) 
		{
			Graphics g = Canvas.CurrentGraph;
			CalcLableLine(Canvas);
			int LabelWidth = int.MinValue;
			string s;
			
			int Start = 0;
			if (AutoFormat) 
			{
				if (MaxY<=10 && MinY<1)
					Start = 3;
				else if (MaxY<=100)
					Start = 2;
				else if (MaxY<=1000)
					Start = 1;
				else Start = 0;
				format = "Z"+Start;
			}
			if (format.StartsWith("Z"))
				SetBestFormat(Canvas,Area,Start);

			for(int i=-1; i<LabelValues.Length; i++)
			{
				if (i<0)
					s = MultiplyFactorToString();
				else 
				{
					double d = (LabelValues[i]/MultiplyFactor);
					s = FormulaHelper.FormatDouble(d,format);
				}
				LabelWidth = Math.Max(LabelWidth,(int)g.MeasureString(s,LabelFont).Width);
			}
			return LabelWidth+majorTick.TickWidth;
		}

		private double GetLastValue(FormulaCanvas Canvas,FormulaArea Area,int LineIndex)
		{
			bool IsUp;
			return GetLastValue(Canvas,Area,LineIndex,out IsUp);
		}

		private double GetLastValue(FormulaCanvas Canvas,FormulaArea Area,int LineIndex,out bool IsUp)
		{
			LatestValueType lvt = Area.Parent.LatestValueType;
			FormulaData f = Area.FormulaDataArray[LineIndex];
			double d = double.NaN;
			IsUp = true;
			if (Area.AxisYs[f.AxisYIndex]==this && 
				((lvt==LatestValueType.All && f.ValueTextMode!=ValueTextMode.None/*!f.TextInvisible*/) ||
				((lvt==LatestValueType.All || lvt==LatestValueType.StockOnly) && f.RenderType==FormulaRenderType.STOCK) ||
				(lvt==LatestValueType.Custom && f.LastValueInAxis)	))
			{
				int Start = Math.Max(0,Canvas.Start);
				if (f.Length>Start) 
				{
					int LastIndex = f.Length-1-Start;
					for(int k = f.Length-1-Start; k>=0; k--)
						if (!double.IsNaN(f[k]))
						{
							LastIndex = k;
							break;
						}

					d = f[LastIndex];
					if (LastIndex>0)
						if (f[LastIndex-1]>d)
							IsUp = false;
				}
			}
			return d;
		}

		/// <summary>
		/// Render Y-axis
		/// </summary>
		/// <param name="Canvas"></param>
		/// <param name="Area"></param>
		public void Render(FormulaCanvas Canvas,FormulaArea Area)
		{
			CalcLableLine(Canvas);
			Rectangle R = FrameRect;//Canvas.FrameRect;
			Graphics g = Canvas.CurrentGraph;
			
			int X = R.Left;
			if (AxisPos==AxisPos.Left) 
				X = R.Right;

			int w1 = majorTick.TickWidth;
			if (majorTick.FullTick)
				w1= R.Width;
			if (majorTick.Inside)
				w1= -w1;

			int w2 = MinorTick.TickWidth;
			if (MinorTick.FullTick)
				w2= R.Width;
			if (MinorTick.Inside)
				w2= -w2;

			if (AxisPos==AxisPos.Left) 
			{
				w1 = -w1;
				w2 = -w2;
			}

			Back.Render(g,R);
			
			if (!LineBinded)
				return;
			
			Pen MajorLinePen = majorTick.LinePen.GetPen();
			Pen MajorTickPen = majorTick.TickPen.GetPen();
			Pen MinorLinePen = minorTick.LinePen.GetPen();
			Pen MinorTickPen = minorTick.TickPen.GetPen();

			float LastY = float.MaxValue;
			Brush LB = labelBrush.GetBrush();
			for(int i=0; i<LabelPos.Length; i++) 
			{
				if (LabelPos[i]>=Canvas.Rect.Top && LabelPos[i]<=Canvas.Rect.Bottom)
				{
					int TextX = X;
					double d = (LabelValues[i]/MultiplyFactor);
					string s = FormulaHelper.FormatDouble(d,format);
					SizeF LabelSize = g.MeasureString(s,LabelFont);
					if (AxisPos==AxisPos.Left) 
					{
						TextX -=(int)LabelSize.Width;
						if (w1<0)
							TextX +=w1;
					} 
					else 
					{
						if (w1>0)
							TextX +=w1;
					}
					float TextY = LabelPos[i]-Canvas.LabelHeight/2;
					
					if (majorTick.ShowText && LastY-TextY>LabelSize.Height)
					{
						g.DrawString(
							s,
							labelFont,
							LB,
							TextX,
							TextY);
						LastY = TextY;
					}

					if (majorTick.ShowLine)
						g.DrawLine(MajorLinePen,Canvas.Rect.Left,LabelPos[i],Canvas.Rect.Right,LabelPos[i]);

					if (majorTick.ShowTick)
						g.DrawLine(MajorTickPen,
							X,LabelPos[i],
							X+w1,LabelPos[i]);
				}

				if (minorTick.Visible && !double.IsInfinity(LabelPos[i])) 
					if (i!=LabelPos.Length-1)
					{
						int TickCount = MinorTick.Count;
						if (minorTick.MinimumPixel!=0) 
							TickCount = (int)((LabelPos[i]-LabelPos[i+1])/MinorTick.MinimumPixel);

						if (minorTick.ShowTick)
						for(float d1=LabelPos[i]; d1>LabelPos[i+1]; d1+=(float)(LabelPos[i+1]-LabelPos[i])/TickCount)
							if (d1>=R.Top && d1<=R.Bottom)
								g.DrawLine(MinorTickPen,
									X,d1,
									X+w2,d1);
					}
			}

//			if (customTick.Visible)
//			{
//				for(int i=0; i<CustomPos.Length; i++)
//				{
//					if (customTick.ShowText && LastY-TextY>LabelSize.Height)
//					{
//						g.DrawString(
//							s,
//							labelFont,
//							LB,
//							TextX,
//							TextY);
//						LastY = TextY;
//					}
//
//					if (customTick.ShowLine)
//						g.DrawLine(MajorLinePen,Canvas.Rect.Left,CustomPos[i],Canvas.Rect.Right,CustomPos[i]);
//				}
//			}
			
			// Draw multiply factor
			if (MultiplyFactor!=1) 
			{
				string s = MultiplyFactorToString();
				Rectangle MR = R;
				MR.Y = (int)(MR.Bottom - Canvas.LabelHeight - 2);
				if (Area.AxisX.Visible)
					MR.Y -=(int)(Canvas.LabelHeight / 2 + 1);
				MR.Height = (int)Canvas.LabelHeight;
				MR.Width = (int)g.MeasureString(s,LabelFont).Width+1;

				if (AxisPos==AxisPos.Left) 
					MR.Offset(R.Width-MR.Width-2,0);

				MultiplyBack.Render(g,MR);
				g.DrawString(s,LabelFont,LB,MR);
			}

			//Draw selected frame
			if (Area.Selected)
				if (Area.SelectedPen!=null)
				{
					Rectangle SelectRect = R;
					SelectRect.Inflate(-1,-1);
					g.DrawRectangle(Area.SelectedPen,SelectRect);
				}

			LatestValueType lvt = Area.Parent.LatestValueType;
			if (lvt != LatestValueType.None)
			{
				for(int i = 0; i<Area.FormulaDataArray.Count; i++)
				{
					bool IsUp;
					double d = GetLastValue(Canvas,Area,i,out IsUp);
					if (!double.IsNaN(d))
					{
//					}
					FormulaData f = Area.FormulaDataArray[i];
//					if (Area.AxisYs[f.AxisYIndex]==this && 
//						((lvt==LatestValueType.All && !f.TextInvisible) ||
//						((lvt==LatestValueType.All || lvt==LatestValueType.StockOnly) && f.RenderType==FormulaRenderType.STOCK) ||
//						(lvt==LatestValueType.Custom && f.LastValueInAxis)	))
//					{
						FormulaLabel fl = Area.Labels[2];
						if (lvt!=LatestValueType.StockOnly)
						{
							fl = (FormulaLabel)fl.Clone();
							Pen p = Area.GetCurrentPen(f,i);
							fl.BGColor = Color.FromArgb(255,p.Color);
							if (fl.BGColor==Color.Empty)
								fl.BGColor = Color.White;
							fl.SetProperTextColor();
						}
//						int Start = Math.Max(0,Canvas.Start);
//						if (f.Length>Start) 
//						{
//							int LastIndex = f.Length-1-Start;
//							for(int k = f.Length-1-Start; k>=0; k--)
//								if (!double.IsNaN(f[k]))
//								{
//									LastIndex = k;
//									break;
//								}
//
//							double d = f[LastIndex];
							if (lvt==LatestValueType.StockOnly)
//								if (LastIndex>0)
//									if (f[LastIndex-1]>d)
								if (!IsUp)
										fl = Area.Labels[1];
							string s = " "+FormulaHelper.FormatDouble(d/MultiplyFactor,format);
							FormulaAlign fa = FormulaAlign.Left;
							if (AxisPos == AxisPos.Left) 
								fa = FormulaAlign.Right;
							fl.DrawString(g,s,LabelFont,fl.TextBrush,VerticalAlign.Bottom,fa,new PointF(X,CalcY(d)),false);
//						}
					}
				}
			}
		}

		/// <summary>
		/// Draw price label at Y
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fc"></param>
		/// <param name="Area"></param>
		/// <param name="Y">Y-Position</param>
		/// <param name="d">Price</param>
		public void DrawCursor(Graphics g,FormulaChart fc,FormulaArea Area,float Y,double d) 
		{
			if (!LastCursorRect.IsEmpty)
				fc.RestoreMemBmp(g,LastCursorRect);
			
			FormulaLabel fl = Area.Labels[2];
			string s = FormulaHelper.FormatDouble(d,format);
			SizeF sf = g.MeasureString(s,LabelFont);

			RectangleF R = new RectangleF(Rect.Left,Y-fc.Rect.Y,Rect.Width-1-Back.RightPen.Width,sf.Height);
			LastCursorRect = R;
			LastCursorRect.Inflate(2,1);
			R.Offset(fc.Rect.Location);

			fl.DrawString(g,s,LabelFont,fl.TextBrush,VerticalAlign.Bottom,FormulaAlign.Left,R,false);
		}

		/// <summary>
		/// Draw price label at Y
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fc"></param>
		/// <param name="Area"></param>
		/// <param name="Y">Y-position</param>
		public void DrawCursor(Graphics g,FormulaChart fc,FormulaArea Area,float Y) 
		{
			DrawCursor(g,fc,Area,Y,GetValueFromY(Y-fc.Rect.Y));
		}

		/// <summary>
		/// Calc Y-position of price
		/// </summary>
		/// <param name="d">Price</param>
		/// <param name="Max">Max value in Y-axis</param>
		/// <param name="Min">Min value in Y-axis</param>
		/// <returns></returns>
		public float CalcY(double d,double Max,double Min) 
		{
			return (float)(Rect.Bottom-Rect.Height*(d-Min)/(Max-Min));
		}

		/// <summary>
		/// Calc Y-position of price
		/// </summary>
		/// <param name="d">Price</param>
		/// <returns></returns>
		public float CalcY(double d) 
		{
			if (MinY==MaxY)
				return 0;
			else
			{
				d = d/MultiplyFactor;
				double m1 = MaxY/MultiplyFactor;
				double m2 = MinY/MultiplyFactor;
				if (Transform!=null) 
				{
					d = Transform(d);
					m1 = Transform(m1);
					m2 = Transform(m2);
				}
				return CalcY(d,m1,m2);

//				if (Scale==ScaleType.Normal)
//				{
//					return CalcY(d/MultiplyFactor,MaxY/MultiplyFactor,MinY/MultiplyFactor);
//				} 
//				else 
//				{
//					double LogMinY = ZeroLog(MinY/MultiplyFactor);
//					double LogMaxY = ZeroLog(MaxY/MultiplyFactor);
//					d = ZeroLog(d/MultiplyFactor);
//					return CalcY(d,LogMaxY,LogMinY);
//				}
			}
		}

		/// <summary>
		/// Calc price from value
		/// </summary>
		/// <param name="Y">Y-Position</param>
		/// <param name="Max">Max position of Y-Axis</param>
		/// <param name="Min">Min position of Y-Axis</param>
		/// <returns></returns>
		public double GetValueFromY(float Y,double Max,double Min) 
		{
			return (Rect.Bottom-Y)/Rect.Height*(Max-Min)+Min;
		}

		/// <summary>
		/// Calc price from value
		/// </summary>
		/// <param name="Y">Y-Position</param>
		/// <returns></returns>
		public double GetValueFromY(float Y)
		{
			if (MinY==MaxY)
				return MinY;
			else
			{
				double m1 =MaxY/MultiplyFactor;
				double m2 = MinY/MultiplyFactor;
				if (Transform!=null) 
				{
					m1 = Transform(m1);
					m2 = Transform(m2);
				}
				double R = GetValueFromY(Y,m1,m2);
				if (RevertTransform!=null) 
					R = RevertTransform(R);
				return R;
			}
		}

		public override string ToString()
		{
			return scale.ToString ();
		}

	}

	/// <summary>
	/// Collection of Y-Axis
	/// </summary>
	public class AxisYCollection:CollectionBase
	{
		public virtual int Add(FormulaAxisY fay)
		{
			return List.Add(fay);
		}

		public virtual FormulaAxisY this[int Index] 
		{
			get
			{
				if (Index<List.Count)
					return (FormulaAxisY)this.List[Index];
				else throw new Exception("Formula Y-Axis bind out of range.("+Index+")");
			}
		}

		public void Remove(FormulaAxisY value) 
		{
			List.Remove(value);
		}
	}
}