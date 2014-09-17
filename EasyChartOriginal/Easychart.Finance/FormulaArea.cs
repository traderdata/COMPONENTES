using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Globalization;
using System.Text;
using System.IO;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance
{
	/// <summary>
	/// Draw chart according to compiled Formulas
	/// </summary>
	public class FormulaArea
	{
		//private string name;
		private int MaxLen;
		private string BindingErrors;
		
		private Rectangle LastClientRect;
		private double LastColumnWidth;
		private double LastMaxY;
		private double LastMinY;
		private bool clientRectChanged;

		public bool ClientRectChanged
		{
			get
			{
				return clientRectChanged;
			}
		}

		public FormulaPackageCollection Packages;

		/// <summary>
		/// ArrayList of FormulaData
		/// </summary>
		public FormulaPackage FormulaDataArray;
		/// <summary>
		/// All Formulas will be shown on this Formula area
		/// </summary>
		public FormulaCollection Formulas;
		/// <summary>
		/// Show if this area is visible
		/// </summary>
		public bool Visible;

		/// <summary>
		/// All AxisY will be shown on this Formula area
		/// </summary>
		public AxisYCollection AxisYs;
		public AxisXCollection AxisXs;

		/// <summary>
		/// Shows If this FormulaArea is selected area
		/// </summary>
		public bool Selected;

		/// <summary>
		/// Will draw a selected sign on this data line.
		/// </summary>
		public FormulaData SelectedData;

		/// <summary>
		/// Selected formula
		/// </summary>
		public FormulaBase SelectedFormula;

		/// <summary>
		/// Selected FormulaArea frame pen
		/// </summary>
		public Pen SelectedPen;

		/*
		public bool Shadow=true;
		public int ShadowDistance=1;
		public int ShadowWidth=2;
		public SmoothingMode SmoothingMode;
		*/

		/// <summary>
		/// The back ground of this stock area
		/// </summary>
		public FormulaBack Back;
		/// <summary>
		/// X axis of this stock area
		/// </summary>
		public FormulaAxisX AxisX;
		/// <summary>
		/// Y axis of the stock area
		/// </summary>
		public FormulaAxisY AxisY;
		/// <summary>
		/// Canvas of this stock area
		/// </summary>
		public FormulaCanvas Canvas;
		/// <summary>
		/// The stock chart this area belong to
		/// </summary>
		public FormulaChart Parent;
		/// <summary>
		/// If set this property to true will draw stock VOLUME as line, or as rectangle.
		/// </summary>
		//public bool DrawVolumeAsLine = true;
		/// <summary>
		/// The stock chart render type
		/// </summary>
		public StockRenderType StockRenderType;
		
		/// <summary>
		/// Volumn stick and color stick render type
		/// </summary>
		//public StickRenderType StickRenderType;

		/// <summary>
		/// The stock chart scale type
		/// </summary>
		public ScaleType ScaleType;
		/// <summary>
		/// The color map of the Formula lines
		/// </summary>
		public Color[] Colors;

		public FormulaLabel[] Labels;
		/// <summary>
		/// Default pen of Formula lines
		/// </summary>
		public Pen LinePen;
		/// <summary>
		/// Stock data pens
		/// </summary>
		public Pen[] BarPens;
		/// <summary>
		/// Stock data brushes
		/// </summary>
		public Brush[] BarBrushes;
		/// <summary>
		/// The brush used to draw the Formula name
		/// </summary>
		public Brush NameBrush;
		/// <summary>
		/// The font used to draw the Formula name
		/// </summary>
		public Font NameFont;
		/// <summary>
		/// The font userd to draw the Formula text
		/// </summary>
		public Font TextFont;
		/// <summary>
		/// The height percentage of the area in the stock chart
		/// </summary>
		public double HeightPercent;
		/// <summary>
		/// Top margin of this area
		/// </summary>
		public double TopMargin;
		/// <summary>
		/// Bottom margin of this area
		/// </summary>
		public double BottomMargin;
		/// <summary>
		/// Draw the stock Formula in this rectangle
		/// </summary>
		public Rectangle Rect;
		/// <summary>
		/// Formula name
		/// </summary>
		public string Name
		{
			get 
			{
				if (Formulas.Count>0)
					return Formulas[0].GetType().ToString();
				return "NoName";
			}
			//			set 
			//			{
			//				name = value;
			//			}
		}
		/// <summary>
		/// return the data provider of the stock Formula
		/// </summary>
		public IDataProvider DataProvider
		{
			get 
			{
				return Parent.DataProvider;
			}
		}

		public FormulaData this[int Index]
		{
			get 
			{
				return FormulaDataArray[Index];
			}
		}

		public FormulaData this[string Name]
		{
			get 
			{
				return FormulaDataArray[Name];
			}
		}

		public FormulaAxisY AddNewAxisY(AxisPos ap)
		{
			return AddNewAxisY(ap,true,-1);
		}

		public FormulaAxisY AddNewAxisY(AxisPos ap,bool Visible,int FormulaIndex)
		{
			FormulaAxisY fay;
			if (AxisYs.Count==1)
			{
				fay = new FormulaAxisY();
				fay.CopyFrom(AxisY);
				AxisYs.Add(fay);
			} else fay = AxisYs[1];

			fay.AutoScale = true;
			fay.MajorTick.ShowLine = false;
			fay.MinorTick.ShowLine = false;
			fay.AxisPos = ap;
			fay.Visible = Visible;
			fay.Back.RightPen.Width=1;
			
			
//			if (FormulaIndex<FormulaDataArray.Count && FormulaIndex>=0)
//				this[FormulaIndex].AxisYIndex = AxisYs.Count-1;

			if (FormulaIndex<Formulas.Count && FormulaIndex>=0)
				for(int i=0; i<FormulaDataArray.Count; i++)
					if (this[i].ParentFormula==Formulas[FormulaIndex])
						this[i].AxisYIndex = AxisYs.Count-1;

			return fay;
		}	

		/// <summary>
		/// Create the Formula area under the stock chart
		/// </summary>
		/// <param name="fc"></param>
		public FormulaArea(FormulaChart fc)
		{
			Visible = true;
			AxisYs = new AxisYCollection();
			AxisXs = new AxisXCollection();
			SelectedPen = new Pen(Color.Red,2);
			StockRenderType = StockRenderType.Candle;
			Colors = new Color[]{Color.Blue,Color.Red,Color.Green,Color.Black,Color.Orange,Color.DarkGray,Color.DarkTurquoise};
			Labels = new FormulaLabel[]{FormulaLabel.EmptyLabel,FormulaLabel.RedLabel,FormulaLabel.GreenLabel,FormulaLabel.WhiteLabel};
			LinePen = new Pen(Color.Black);
			BarPens = new Pen[]{new Pen(Color.Black,1),new Pen(Color.Black,1),new Pen(Color.Blue,1)};
			BarBrushes = new Brush[]{null,null,Brushes.Blue};
			NameBrush = new SolidBrush(Color.Black);
			NameFont = new Font("Verdana",8);
			TextFont = new Font("Verdana",8);

			Parent = fc;
			Packages = new FormulaPackageCollection();
			FormulaDataArray = new FormulaPackage();
			Formulas = new FormulaCollection();
			Back = new FormulaBack();
			
			AxisX = new FormulaAxisX();
			AxisX.Format = "MMMyy";
			//AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
			AxisXs.Add(AxisX); 

			AxisY = new FormulaAxisY();
			AxisYs.Add(AxisY);
		}
		
		/// <summary>
		/// Create the Formula area by a Formula name
		/// </summary>
		/// <param name="fc"></param>
		/// <param name="Name"></param>
		public FormulaArea(FormulaChart fc,string Name,string Quote):this(fc)
		{
			AddFormula(Name,Quote);
		}

		/// <summary>
		/// Create the Formula area by a Formula name and Height percentage in the stock chart
		/// </summary>
		/// <param name="fc"></param>
		/// <param name="Name"></param>
		/// <param name="HeightPercent"></param>
		public FormulaArea(FormulaChart fc,string Name,string Quote,double HeightPercent):this(fc,Name,Quote)
		{
			this.HeightPercent = HeightPercent;
		}

		/// <summary>
		/// Get FormulaData
		/// </summary>
		/// <param name="Index">The FormulaData Index</param>
		/// <returns></returns>
		public FormulaData GetFormulaData(int Index) 
		{
			return (FormulaData)FormulaDataArray[Index];
		}

		/// <summary>
		/// Add new Formula lines by Formula name, can use parameter , such as MA(10)
		/// </summary>
		/// <param name="Name">Formula name</param>
		public void AddFormula(string Name) 
		{
			Formulas.Add(Name);
		}

		/// <summary>
		/// Add new Formula lines by Formula name and stock symbol
		/// </summary>
		/// <param name="Name">Formula name, such as MA(10)</param>
		/// <param name="Quote">stock symbol, such as MSFT</param>
		public void AddFormula(string Name,string Quote) 
		{
			Formulas.Add(Name,Quote);
		}

		/// <summary>
		/// Add new Formula lines by Formula
		/// </summary>
		/// <param name="fb">Formulas</param>
		public void AddFormula(FormulaBase fb)
		{
			Formulas.Add(fb);
		}

		/// <summary>
		/// Insert new Formula lines by Formula name, can use parameter , such as MA(10)
		/// </summary>
		/// <param name="Index">Insert at</param>
		/// <param name="Name">Formula Name</param>
		public void InsertFormula(int Index,string Name) 
		{
			Formulas.Insert(Index,Name);
		}

		/// <summary>
		/// Insert new Formula lines by Formula
		/// </summary>
		/// <param name="Index"></param>
		/// <param name="fb"></param>
		public void InsertFormula(int Index,FormulaBase fb)
		{
			Formulas.Insert(Index,fb);
		}

		/// <summary>
		/// Remove Formula
		/// </summary>
		/// <param name="fb"></param>
		public void RemoveFormula(FormulaBase fb) 
		{
			SelectedData = null;
			Formulas.Remove(fb);
		}

		/// <summary>
		/// Remove Formula lines by Formula name
		/// </summary>
		/// <param name="Name"></param>
		public void RemoveFormula(string Name)
		{
			SelectedData = null;
			Formulas.Remove(Name);
		}

		/// <summary>
		/// Bind data provider , create Formula line data of current data provider
		/// </summary>
		public void Bind()
		{
			BindingErrors = null;
			try 
			{
				//Save selected formula formula and data
				string SelectedFormulaName = null;
				int SelectedIndex = 0;
				if (SelectedFormula!=null && !object.Equals(SelectedData,null))
				{
					SelectedFormulaName = SelectedFormula.FullName;
					foreach(FormulaData fd in FormulaDataArray)
						if (fd.ParentFormula==SelectedFormula) 
						{
							if (object.Equals( fd,SelectedData))
								break;
							else SelectedIndex++;
						}
				}


				FormulaDataArray.Clear();
				if (DataProvider!=null)
				{
					Packages.Clear();
					foreach(FormulaBase fb in Formulas) 
					{
						IDataProvider idp = DataProvider;
						if (fb.Quote!=null && fb.Quote!="" && idp.DataManager!=null)
						{
							if (string.Compare(idp.GetStringData("Code"),fb.Quote,true)!=0)
							{
								if (idp.DataManager==null)
									throw new Exception(fb.Quote+" not found!");
								idp = idp.DataManager[fb.Quote];
								idp.BaseDataProvider = DataProvider;
							}
						}
						try 
						{
							FormulaPackage fps = fb.Run(idp);
							foreach(FormulaData fd in fps)
								fd.ParentFormula = fb;
							Packages.Add(fps);
						}
						catch (Exception e1) 
						{
							throw new Exception("Errors in formula \""+fb.DisplayName+"\":"+e1.Message,e1);
						}
					}
				}
				foreach(FormulaPackage fp in Packages)
					FormulaDataArray.AddRange(fp);

				//Restore selected formula and data
				int i = 0;
				if (SelectedFormulaName!=null)
				foreach(FormulaData fd in FormulaDataArray)
					if (fd.ParentFormula.FullName==SelectedFormulaName)
					{
						if (i==SelectedIndex)
						{
							SelectedFormula = fd.ParentFormula;
							SelectedData = fd;
							break;
						} else i++;
					}

			} 
			catch (Exception e2) 
			{
				BindingErrors = e2.Message;
				if (e2.InnerException!=null)
					BindingErrors +="\n"+e2.InnerException;

			}
		}

		private void MovePoint(PointF[] pfs,int Delta) 
		{
			for(int i=0; i<pfs.Length; i++)
			{
				pfs[i].X +=Delta;
				pfs[i].Y +=Delta;
			}
		}

		private void DrawLines(Graphics g,FormulaData f,Pen P,ArrayList al,bool IsUp)
		{
			PointF[] pfsA = (PointF[])al.ToArray(typeof(PointF));
			P.Color = IsUp?f.FormulaUpColor:f.FormulaDownColor;
			g.DrawLines(P,pfsA);
			al.Clear();
		}

		private void DrawPoints(Graphics g,FormulaData f,Pen CurrentPen,PointF[] pfs,double ColumnWidth)
		{
			float w = (float)(ColumnWidth*Parent.ColumnPercent);
			switch (f.Dot) 
			{
				case FormulaDot.NORMAL:
					if (pfs.Length>1 && CurrentPen.Width>0) 
					{
						if (f.Smoothing == SmoothingMode.Invalid)
							g.SmoothingMode = SmoothingMode.HighQuality;
						else g.SmoothingMode =f.Smoothing;
						try
						{
							if (f.FormulaDownColor!=Color.Empty && pfs.Length>0)
							{
								ArrayList al = new ArrayList();
								bool b1 = true;
								al.Add(pfs[0]);
								for(int i=1; i<pfs.Length; i++)
								{
									bool b2 = pfs[i].Y>pfs[i-1].Y;
									if (b1!=b2)
									{
										if (al.Count>1)
										{
											DrawLines(g,f,CurrentPen,al,b1);
											al.Add(pfs[i-1]);
										}
										b1 = b2;
									} 
									al.Add(pfs[i]);
								}
								DrawLines(g,f,CurrentPen,al,b1);
							} 
							else 
							{
								if (f.Horizontal)
								{
									int LastIndex = 0;
									for(int i=1; i<=pfs.Length; i++) 
									{
										if (i==pfs.Length ||  pfs[i].Y!=pfs[LastIndex].Y)
										{
											g.DrawLine(CurrentPen,pfs[LastIndex],pfs[i-1]);
											LastIndex = i;
										}
									}
								}
								else 
									g.DrawLines(CurrentPen,pfs);
							}
						}
						catch
						{
						}
						g.SmoothingMode = SmoothingMode.Default;
					}
					break;
				case FormulaDot.CIRCLEDOT:
					CurrentPen.Width = 1;
					for(int i=0; i<pfs.Length; i++) 
						g.DrawEllipse(CurrentPen,pfs[i].X-w/2,pfs[i].Y-w/2,w,w);
					break;
				case FormulaDot.CROSSDOT:
					w = (float)(ColumnWidth*Parent.ColumnPercent);
					for(int i=0; i<pfs.Length; i++) 
					{
						g.DrawLine(CurrentPen,pfs[i].X-w/2,pfs[i].Y,pfs[i].X+w/2,pfs[i].Y);
						g.DrawLine(CurrentPen,pfs[i].X,pfs[i].Y-w/2,pfs[i].X,pfs[i].Y+w/2);
					}
					break;
				case FormulaDot.POINTDOT:
					w = (float)(ColumnWidth*Parent.ColumnPercent+1);
					Brush b = new SolidBrush(CurrentPen.Color);
					for(int i=0; i<pfs.Length; i++) 
						g.FillEllipse(b,pfs[i].X-w/2,pfs[i].Y-w/2,w,w);
					break;
			}
		}

		private void DrawSelectedSign(Graphics g,FormulaData fd,PointF[] pfs)
		{
			if (object.Equals(fd,SelectedData))
				try
				{
					ArrayList al = new ArrayList(); 
					float w = 5;

					float LastX = float.MaxValue;
					for(int i=0; i<pfs.Length; i++)
						if ((LastX-pfs[i].X)>50) 
						{
							al.Add(new RectangleF(pfs[i].X-w/2,pfs[i].Y-w/2,w,w));
							LastX = pfs[i].X;
						}

					RectangleF[] rfs = (RectangleF[])al.ToArray(typeof(RectangleF));
					g.FillRectangles(Brushes.PaleGoldenrod, rfs);
					g.DrawRectangles(Pens.Black,rfs);
				}
				catch
				{
				}
		}

		private void DrawBackground(Rectangle R)
		{
			Back.Render(Canvas.CurrentGraph,R);
		}

		private void MakeSameLength()
		{
			/*
			MaxLen = int.MinValue;
			foreach(FormulaData f in FormulaDataArray)
			{
				MaxLen = Math.Max(MaxLen,f.Length);
			}
			*/
			MaxLen = DataProvider.Count;
			
			// Make all Formula same length
			foreach(FormulaData f in FormulaDataArray)
				if (f.Length<MaxLen)
					f.FillTo(MaxLen);
			if (Parent.CursorPos==-1)
				Parent.CursorPos = MaxLen-1;
		}

		/// <summary>
		/// Calc the min/max value of Y-Axis
		/// </summary>
		private void CalcMinMax()
		{
			for(int i=0; i<AxisYs.Count; i++)
			{
				FormulaAxisY fay = AxisYs[i];
				if (fay.AutoScale)
				{
					//bool b = false;
					fay.MinY = double.MaxValue;
					fay.MaxY = double.MinValue;
					if (fay.AutoScale || fay.MinY==double.MinValue || fay.MaxY==double.MaxValue)
					{
						foreach(FormulaData f in FormulaDataArray)
						{
							if (f.AxisY==fay && f.RenderType!=FormulaRenderType.VERTLINE 
								&& f.LineWidth!=0 && f.Transform<Transform.PercentView)
							{
								//b = true;
								double A = f.MinValue(Canvas.Start,Canvas.Count);
								double B = f.MaxValue(Canvas.Start,Canvas.Count);
								double H = f.HighPercent;
								double L = f.LowPercent;

								double d1 = A;
								double d2 = B;
								if (H!=1 || L!=0) 
								{
									d1 = (A*H-B*L)/(H-L);
									d2 = (B-d1*(1-H))/H;
								}

								fay.MinY = Math.Min(fay.MinY,d1);
								fay.MaxY = Math.Max(fay.MaxY,d2);
								FormulaBase fb = f.ParentFormula;
								if (fb!=null)
								{
									if (!double.IsNaN(fb.MinY))
										fay.MinY =fb.MinY;
									if (!double.IsNaN(fb.MaxY))
										fay.MaxY = fb.MaxY;
								}
							}
						}

						if (TopMargin!=0 || BottomMargin!=0) 
						{
							double H = Math.Abs(Math.Min(fay.MaxY-fay.MinY,fay.MinY));
							fay.MinY -=H * BottomMargin / 100;
							fay.MaxY+=H * TopMargin / 100;
						}
						
						if (fay.MinY!=fay.MaxY)
						{
							double ValueHeight = (fay.MaxY-fay.MinY)/Canvas.Rect.Height*24;
							foreach(FormulaData f in FormulaDataArray)
							{
								if (f.RenderType == FormulaRenderType.TEXT)
								{
									if (f.VAlign == VerticalAlign.Top)
										fay.MaxY =Math.Max(fay.MaxY,f.MaxValue(Canvas.Start,Canvas.Count)+ValueHeight);
									else if (f.VAlign == VerticalAlign.Bottom)
										fay.MinY =Math.Min(fay.MinY,f.MinValue(Canvas.Start,Canvas.Count)-ValueHeight);
								} 
							}

							double d1 = double.MinValue;
							double d2 = double.MinValue;
							foreach(FormulaData f in FormulaDataArray)
							{
								if (f.RenderType==FormulaRenderType.ICON)
								{
									Image I = (Image)f.OwnerData["ICON"];
									if (I!=null) 
									{
										double d = (fay.MaxY-fay.MinY)/Canvas.Rect.Height*I.Height;

										if (f.VAlign == VerticalAlign.Top)
											d1 = Math.Max(d1,d);
										else if (f.VAlign==VerticalAlign.Bottom)
											d2 = Math.Max(d2,d);;
									}
								}
							}

							if (d1!=double.MinValue)
								fay.MaxY +=d1;
							if (d2!=double.MinValue)
								fay.MinY -=d2;

							if (IsMain(i) & fay.MinY<0)
							{
								double M = 0;
								foreach(FormulaData f in FormulaDataArray)
									if (f.RenderType == FormulaRenderType.STOCK && f.AxisYIndex==i)
										M = f.MinValue(Canvas.Start,Canvas.Count);
								fay.MinY = M;
							}
						}
					} 

					if (double.IsInfinity(fay.MaxY) || double.IsNaN(fay.MaxY) ||
						double.IsInfinity(fay.MinY) || double.IsNaN(fay.MinY) || fay.MaxY<fay.MinY)
					{
						fay.MaxY = 1;
						fay.MinY = 0;
					}

					if (fay.MaxY==fay.MinY)
					{
						fay.MinY -=0.5;
						fay.MaxY +=0.5;
					}
				}
			}
		}

		private void BindYAxis()
		{
			Hashtable ht = new Hashtable();
			for(int i=0; i<FormulaDataArray.Count; i++)
				ht[this[i].AxisYIndex] = 1;

			for(int i=0; i<AxisYs.Count; i++)
				AxisYs[i].LineBinded = ht.ContainsKey(i);
		}

		Random Rnd = new Random();

		public Pen GetCurrentPen(FormulaData fd, int ColorIndex)
		{
			Pen Result;
			//FormulaData fd = (FormulaData)FormulaDataArray[Index];
			if (fd.LinePen == null) 
			{
				Result = (Pen)LinePen.Clone();
				Result.DashStyle = fd.DashStyle;
				if (!fd.FormulaUpColor.IsEmpty)
					Result.Color = fd.FormulaUpColor;
				else 
					Result.Color = Colors[ColorIndex % Colors.Length];
				if (fd.LineWidth>=0)
					Result.Width = fd.LineWidth;
				if (fd.Alpha!=255)
					Result.Color =Color.FromArgb(fd.Alpha,Result.Color);
			} 
			else Result = fd.LinePen;
			return Result;
		}

		public Pen SetAlpha(int Index,Pen p)
		{
			FormulaData fd = (FormulaData)FormulaDataArray[Index];
			if (fd.Alpha==255)
				return p;
			p = (Pen)p.Clone();
			p.Color = Color.FromArgb(fd.Alpha,p.Color);
			return p;
		}

		public Pen[] AdjustAlpha(Pen[] ps,byte Alpha)
		{
			if (Alpha==255)
				return ps;
			Pen[] ps2 = new Pen[ps.Length];
			for(int i=0; i<ps.Length; i++)
			{
				ps2[i] = (Pen)ps[i].Clone();
				ps2[i].Color = Color.FromArgb(Alpha,ps[i].Color);
			}
			return ps2;
		}

		private Brush[] AdjustAlpha(Brush[] bs,byte Alpha)
		{
			if (Alpha==255)
				return bs;

			Brush[] bs2 = new Brush[bs.Length];
			for(int i=0; i<bs.Length; i++)
			{
				bs2[i] = (Brush)bs[i].Clone();
				if (bs2[i] is SolidBrush)
				{
					SolidBrush sb = bs2[i] as SolidBrush;
					sb.Color= Color.FromArgb(Alpha,sb.Color);
				}
			}
			return bs2;
		}

		private Brush AdjustBrush(Brush b,FormulaData fd)
		{
			if (fd.AreaBrush!=null)
				b = fd.AreaBrush;

			if (b is SolidBrush)
				if ((b as SolidBrush).Color.A==0)
					b = null;
			return b;
		}

		private Font GetCurrentFont(int Index) 
		{
			Font Result;
			FormulaData fd = (FormulaData)FormulaDataArray[Index];

			if (fd.TextFont == null)
			{
				Result = TextFont;
			}
			else 
				Result = fd.TextFont;
			return Result;
		}

//		private double GetInViewData(FormulaData f,int DataIndex,out int Index) 
//		{
//			Index = Math.Max(0,f.Length-Canvas.Start-Canvas.Count+DataIndex);
//			if (Index<f.Length) 
//			{
//				double d = f[Index];
//				int Delta = 0;
//				while ((d==0 || double.IsNaN(d)) && Index<f.Length) 
//				{
//					d = f[Index];
//					Index++;
//					Delta++;
//				}
//				Index = DataIndex+Delta;
//				return d;
//			} 
//			return double.NaN;
//		}
//
//		private double GetInViewData(int LineIndex,int DataIndex,out int Index) 
//		{
//			FormulaData f = (FormulaData)FormulaDataArray[LineIndex];
//			return GetInViewData(f,DataIndex,out Index);
//		}
//
//		private double GetInViewData(int LineIndex,int DataIndex) 
//		{
//			if (LineIndex<FormulaDataArray.Count)
//			{
//				int i;
//				return GetInViewData(LineIndex,DataIndex,out i);
//			}
//			return double.NaN;
//		}
//
//		private double GetInViewData(string LineName,int DataIndex) 
//		{
//			for(int i=0; i<FormulaDataArray.Count; i++) 
//			{
//				FormulaData f = (FormulaData )FormulaDataArray[i];
//				if (string.Compare(f.Name,LineName,true)==0)
//					return GetInViewData(i,DataIndex);
//			}
//			return double.NaN;
//		}

		private FormulaData MainFormula
		{
			get
			{
				foreach(FormulaData f in FormulaDataArray)
					if (f.RenderType==FormulaRenderType.STOCK)
						return f;
				if (FormulaDataArray.Count>0)
					return (FormulaData)FormulaDataArray[0];
				return null;
			}
		}

		private void GetTransform(double[] dd,double A,double B)
		{
			for(int i=0; i<dd.Length; i++)
				dd[i] = dd[i]*A+B;
		}

		private void GetTransform(FormulaData f,double A,double B)
		{
			GetTransform(f.Data,A,B);
			if (f.SubData!=null)
				foreach(string Key in f.SubData.Keys)
					GetTransform((double[])f.SubData[Key],A,B);
		}

		public FormulaData GetTransform(FormulaData f)
		{
			if (f.Transform== Transform.Normal)
				return f;
			else 
			{
				FormulaData nf = (FormulaData)f.Clone();
				if (object.Equals(MainFormula,null) || object.Equals(MainFormula,f))
					return nf;

				double A = 0;
				double B = 0;
				if (f.Transform== Transform.FirstDataOfView)
				{
					int i1 = Parent.DateToIndex(Parent.StartTime);
					int i2 = Parent.DateToIndex(Parent.EndTime);
					for(int i=i1; i<i2 && i<nf.Length; i++)
						if (!double.IsNaN(nf.Data[i])) 
						{
							B = nf[i];
							A = MainFormula[i]/B;
							B = 0;
							break;
						}
				} 
				else if (f.Transform == Transform.PercentView)  
				{
					double M = f.MaxValue(Canvas.Start,Canvas.Count);
					A = (f.AxisY.MaxY-f.AxisY.MinY)/M*f.PercentView;
					B = f.AxisY.MinY;
				} 
				else if (f.Transform == Transform.FullView)
				{
					double M1 = f.MinValue(Canvas.Start,Canvas.Count);
					double M2 = f.MaxValue(Canvas.Start,Canvas.Count);
					double N1 = MainFormula.MinValue(Canvas.Start,Canvas.Count);
					double N2 = MainFormula.MaxValue(Canvas.Start,Canvas.Count);

					A = (N2-N1)/(M2-M1);
					B = N1-M1*A;
				}
				GetTransform(nf,A,B);
				return nf;
			}
		}
		
		/// <summary>
		/// Scaling the values of data array of another stocks
		/// </summary>
		private void AdjustMoving()
		{
			bool Recal = false;
			Hashtable ht = new Hashtable();
			for(int j=1; j<FormulaDataArray.Count; j++)
			{
				FormulaData f = (FormulaData)FormulaDataArray[j];
				if (f.Transform== Transform.FirstDataOfView)
				{
					ht[j] = FormulaDataArray[j];
					FormulaDataArray[j] = GetTransform(f);
					Recal = true;
				} 
			}
			if (Recal) CalcMinMax();
			foreach(int i in ht.Keys)
				FormulaDataArray[i] = (FormulaData)ht[i];

//			for(int j=0; j<FormulaDataArray.Count; j++)
//			{
//				FormulaData f = (FormulaData)FormulaDataArray[j];
//				if (f.Transform == Transform.PercentView) 
//				{
//					ht[j] = FormulaDataArray[j];
//					FormulaDataArray[j] = GetTransform(f);
//				}
//			}
		}

//		private void AdjustMoving()
//		{
//			bool Recal = false;
//			for(int j=1; j<FormulaDataArray.Count; j++)
//			{
//				FormulaData f = (FormulaData)FormulaDataArray[j];
//				if (f.Transform== Transform.FirstDataOfView)
//				{
//					int Index;
//					double B = GetInViewData(j,0,out Index);
//					double A = GetInViewData(0,Index)/B;
//					for(int i=0; i<f.Length; i++)
//						f[i] *= A;
//					Recal = true;
//				}
//			}
//
//			if (Recal) CalcMinMax();
//			foreach(FormulaData f in FormulaDataArray) 
//			{
//				if (f.Transform == Transform.PercentView) 
//				{
//					double M = f.MaxValue(Canvas.Start,Canvas.Count);
//					double A = (f.AxisY.MaxY-f.AxisY.MinY)/M*f.PercentView;
//					double B = f.AxisY.MinY;
//					for(int i=0; i<f.Length; i++)
//						f[i] = f[i]*A+B;
//				}
//			}
//		}

		/// <summary>
		/// Draw line label and value at top of the FormulaArea
		/// </summary>
		public void DrawValueText() 
		{
			DrawValueText(Canvas.CurrentGraph);
		}

		/// <summary>
		/// Draw line label and value at top of the FormulaArea
		/// </summary>
		/// <param name="g"></param>
		public void DrawValueText(Graphics g)
		{
			//int ValueIndex = ((FormulaData)FormulaDataArray[0]).Length-1-Parent.Pos;
			if (Canvas==null) return;
			if (!Parent.ShowValueLabel) return;
			int ValueIndex = Parent.CursorPos;
			RectangleF R = new RectangleF(Canvas.ClipRect.Left+Back.LeftPen.Width+2,
				Canvas.FrameRect.Top+Back.TopPen.Width,
				Canvas.ClipRect.Width-Back.LeftPen.Width-Back.RightPen.Width-2,
				(int)Canvas.LabelHeight-Back.TopPen.Width);

			if (g!=Canvas.CurrentGraph)
			{
				R.Offset(Parent.Rect.Location);
				g.FillRectangle(Back.BackGround.GetBrush(),R);
			}

			int X = (int)R.Left;
			int Y = (int)R.Top;

			int j=0;
			int k=0;
			g.SetClip(R);
			try
			{
				ValueTextMode vtmFormula = ValueTextMode.Default;
				for(int i=0; i<FormulaDataArray.Count; i++)
				{
					FormulaData fd = (FormulaData)FormulaDataArray[i];
					Pen CurrentPen = GetCurrentPen(fd,i);
					Brush TextBrush = new SolidBrush(CurrentPen.Color);
					ValueTextMode vtm = Parent.ValueTextMode;

					if (k<=i) 
					{
						FormulaBase fb =Formulas[j];
						vtmFormula = fb.ValueTextMode;

						FormulaPackage fp =Packages[j];
						string r = fb.DisplayName;
						Brush B = TextBrush;
						if (j==0) B = NameBrush;

						if (!fb.TextInvisible)
						{
							if (r!=null && r!="") 
							{
								g.DrawString(r,NameFont,B,X,Y);
								X +=(int)g.MeasureString(r,NameFont).Width+4;
							}

							if (fd.RenderType == FormulaRenderType.STOCK && Parent.PriceLabelLayout!=null) 
							{
								Parent.PriceLabelLayout.SetFont(NameFont);
								if (ValueIndex>=0 && ValueIndex < fd.Length) 
									X = Parent.PriceLabelLayout.Render(g,Rectangle.Empty,Parent, new Point(X,Y),ValueIndex).X +4;
							}
						}
					
						j++;
						k+=fp.Count;
					}
					if (vtm==ValueTextMode.Default)
						vtm = vtmFormula;

					//if (fd.TextInvisible) continue;
					if (vtm==ValueTextMode.Default)
						vtm = fd.ValueTextMode;
					if (vtm==ValueTextMode.None) continue;
					if (ValueIndex>=0 && ValueIndex < fd.Length)
					{
						string s = "";
						if (vtm==ValueTextMode.TextOnly || vtm==ValueTextMode.Both)
							s += fd.Name;

						if (vtm==ValueTextMode.ValueOnly || vtm==ValueTextMode.Both)
						{
							if (s!=null && s!="")
								s +=":";
							double d = fd[ValueIndex]/AxisYs[fd.AxisYIndex].MultiplyFactor;
							string Format = AxisYs[fd.AxisYIndex].Format;
							string lf = Format.ToLower();
							if (lf=="f0" || lf=="f1")
								Format ="f2";
							s += FormulaHelper.FormatDouble(d,Format);
						}
						if (s!="")
						{
							g.DrawString(s,NameFont,TextBrush,X,Y);
							X +=(int)g.MeasureString(s,NameFont).Width+4;
						}
					}
				}
			} 
			finally 
			{
				g.ResetClip();
			}
		}

		private void CalcInViewPoint()
		{
			Rectangle R = Canvas.FrameRect;
			Rectangle R2 = R;

			for(int i=0; i<AxisYs.Count; i++)
			{
				FormulaAxisY fay =AxisYs[i];
				if (fay.Visible)
				{
					if (i==0)
						R2.Width -= fay.Width;
					R.Width -= fay.Width;
					fay.FrameRect = new Rectangle(R.Right,R.Top,fay.Width,R.Height);
					if (fay.AxisPos == AxisPos.Left) 
					{
						fay.FrameRect.X = R.X;
						if (i==0)
							R2.X +=fay.Width;
						R.X +=fay.Width;
					}
				}
			}
			
			int LH = (int)Math.Ceiling(Canvas.LabelHeight);

			if (!Parent.ShowValueLabel) 
				LH /=2;

			R.Y +=LH;
			R.Height -= LH;

			int OldHeight = R.Height;
			for(int i=AxisXs.Count-1; i>=0; i--)
			{
				FormulaAxisX fax = AxisXs[i];
				if (fax.Visible) 
				{
					R.Height -=fax.Height;
					fax.Rect = R;
					fax.Rect.Y = R.Bottom;
					fax.Rect.Height = fax.Height;
					fax.Rect.Width++;
					if (i<AxisXs.Count-1)
						fax.Rect.Height++;
				}
			}
			if (R.Height == OldHeight)
				R.Height -= (int)Math.Ceiling(Canvas.LabelHeight/2);

			R2.Y = R.Y;
			R2.Height = R.Height;

			if (Parent.TwoYAxisType==TwoYAxisType.AreaSame)
				R2 = R;

			Canvas.Rect = R2;
			Canvas.ClipRect = R;

			if (!Parent.FixedTime)
			{
				Canvas.Count = (int)((R2.Width- Parent.MarginWidth) / Canvas.ColumnWidth);
				if ((Canvas.Start+Canvas.Count)>MaxLen)
				{
					if (Canvas.Count>MaxLen) 
					{
						Canvas.Start = 0;
						Canvas.Count = MaxLen;
					} 
					else 
					{
						Canvas.Start = MaxLen-Canvas.Count;	
					}
				}
				Canvas.Stop = Math.Max(0,MaxLen-Canvas.Start-Canvas.Count);
			} 
			else 
			{
				int i1 = Parent.DateToIndex(Parent.StartTime);
				int i2 = Parent.DateToIndex(Parent.EndTime);
				Canvas.Count = i2-i1+1;
				Canvas.Start = MaxLen-i2-1;
				Canvas.Stop = Math.Max(0,MaxLen-Canvas.Start-Canvas.Count);
				Canvas.ColumnWidth = ((double)R.Width- Parent.MarginWidth) / Canvas.Count;
			}
		}

		private void InitCanvas(Graphics g,Rectangle R,int Start,double ColumnWidth,double ColumnPercent)
		{
			if (Canvas==null)
				Canvas = new FormulaCanvas();
			Canvas.CurrentGraph = g;
			Canvas.AxisX = AxisX;

			AxisX.StartTime = Parent.StartTime;
			AxisX.EndTime = Parent.EndTime;
			if (Parent.FixedTime)
			{
				Canvas.DATE = DataProvider["DATE"];
				if (DataProvider is CommonDataProvider)
					AxisX.IntradayInfo = (DataProvider as CommonDataProvider).IntradayInfo;
			} 
			else 
			{
				Canvas.DATE = null;
				AxisX.IntradayInfo = null;
			}

			MakeSameLength();
			Canvas.FrameRect = R;
			Canvas.Start = Start;
			Canvas.ColumnWidth = ColumnWidth;
			Canvas.ColumnPercent = ColumnPercent;
			Canvas.LabelHeight = Canvas.CurrentGraph.MeasureString("0",AxisY.LabelFont).Height;
			CalcInViewPoint();

			//Bind Y-axis to FormulaData
			foreach(FormulaData f in FormulaDataArray)
			{
				f.Canvas = Canvas;
				if (f.AxisYIndex<AxisYs.Count)
					f.AxisY = AxisYs[f.AxisYIndex];
				else f.AxisY = AxisY;
			}

			CalcMinMax();
			AdjustMoving();

			if (AxisY.ShowAsPercent && AxisY.RefValue==0)
				if (!object.Equals(MainFormula,null))
					AxisY.RefValue = MainFormula[Parent.DateToIndex(Parent.StartTime)];
		}

		/// <summary>
		/// Calc Y-axis width of this FormulaArea
		/// </summary>
		/// <param name="g">Graphics to draw this formula</param>
		/// <returns></returns>
		public int CalcLabelWidth(Graphics g) 
		{
			InitCanvas(g,Rect,Parent.Start,Parent.ColumnWidth,Parent.ColumnPercent);
			int AxisMargin = 0;
			foreach(FormulaData f in FormulaDataArray) 
				AxisMargin = Math.Max(f.AxisMargin,AxisMargin);
			return AxisY.CalcLabelWidth(Canvas,this)+AxisMargin;
		}

		public string ReplaceTag(int Bar,FormulaData fd,string s)
		{
			while (true) 
			{
				int i1 = s.IndexOf('{');
				int i2 = s.IndexOf('}');
				if (i2>i1)
				{
					string s1 = s.Substring(i1+1,i2-i1-1);
					int i = s1.IndexOf(':');
					string s3 = "";
					string s2 = s1;
					if (i>0)
					{
						s2 = s1.Substring(0,i);
						s3 = s1.Substring(i+1);
					}

					double[] dd = DataProvider["DATE"];
					if (string.Compare(s2,"D")==0)
					{
						if (s3=="")
							s3 = "yyyy-MM-dd";
						s2 = DateTime.FromOADate(dd[Bar]).ToString(s3);
					} 
					else 
					{
						try
						{
							dd = DataProvider[s2];
						}
						catch
						{
							FormulaData fdd = fd["NUMBER"+s2];
							dd = fdd.Data;
						}

						if (dd!=null && Bar<dd.Length) 
						{
							if (s3=="")
								s3 = "f2";
							s2 = dd[Bar].ToString(s3);
						} 
						else s2 = "NaN";
					} 
					s = s.Substring(0,i1)+s2+s.Substring(i2+1);
				} 
				else break;
			}
			return s;
		}

		/// <summary>
		/// Calc the cross point of two lines
		/// </summary>
		/// <param name="p0">First line start point</param>
		/// <param name="p1">First line end point</param>
		/// <param name="p3">Second line start point</param>
		/// <param name="p2">Second line end point</param>
		/// <returns>Cross point</returns>
		public PointF CalcCenter(PointF p0,PointF p1,PointF p3,PointF p2) 
		{
			float y = (p3.Y*(p2.Y-p1.Y)-p2.Y*(p3.Y-p0.Y))/(p0.Y+p2.Y-p1.Y-p3.Y);
			float a = p3.Y-p0.Y;
			float x = float.NaN;
			if (a!=0)
				x = (p2.X-p0.X)*(y-p3.Y)/a+p2.X;
			else 
			{
				a = p2.Y-p1.Y;
				x = (p2.X-p0.X)*(y-p2.Y)/a+p2.X;
			}

			if (double.IsNaN(x) || double.IsInfinity(x) || double.IsNaN(y) || double.IsInfinity(y))
				return PointF.Empty;
			else return new PointF(x,y);
		}

		private void InsertPoint(ArrayList al,PointF[] pfs,PointF[] pfs2,int j) 
		{
			if (j>0 && j<pfs.Length)
			{
				PointF pf = CalcCenter(pfs[j-1],pfs2[j-1],pfs[j],pfs2[j]);
				if (pf!=PointF.Empty)
					al.Insert(al.Count/2,pf);
			}
		}

		private Pen[] AdjustPenWidth(int w)
		{
			if (BarPens!=null )
			{
				if (BarPens[0].Width>1)
					StockRenderType = StockRenderType.OHLCBars;
				Pen[] ps = new Pen[BarPens.Length];
				for(int i=0; i<BarPens.Length; i++) 
				{
					ps[i] = (Pen)BarPens[i].Clone();
					if (BarPens[i].Width>1)
					{
						if (w<ps[i].Width*1.5)
							ps[i].Width = w/1.5f;
					}
					if (w<2 && BarBrushes[i] is SolidBrush && StockRenderType==StockRenderType.Candle)
					{
						Color C = (BarBrushes[i] as SolidBrush).Color;
						if (C.A!=0)
							ps[i].Color = C;
					}
				}
				return ps;
			}
			return BarPens;
		}

		private Pen[] AdjustStickPenWidth(Pen[] ps,float w)
		{
			if (Parent.StickRenderType==StickRenderType.ThickLine) 
			{
				Pen[] ps2 = new Pen[ps.Length];
				for(int i=0; i<ps.Length; i++)
				{
					ps2[i] = (Pen)ps[i].Clone();
					ps2[i].Width = w;
				}
				return ps2;
			} else return ps;
		}

		private void DrawStick(Graphics g,float x,float y,float h,Pen p,Brush B,double ColumnWidth)
		{
			StickRenderType srt = Parent.StickRenderType;
			float w = (float)(ColumnWidth*Parent.ColumnPercent);
			if (w>2 && p.Width<2 && (srt==StickRenderType.Column || srt==StickRenderType.ThickColumn))
			{
				if (srt==StickRenderType.ThickColumn)
					w = (float)ColumnWidth;
				if (h<0)
				{
					y +=h;
					h = -h;
				}

				RectangleF rf = new RectangleF((float)(x-w/2),y,w,h);

				if (w<2.5)
					rf.Width = 1;
				if (B!=null && rf.Width>1)
					g.FillRectangle(B,rf);
				
				//g.DrawRectangle(p,rf.Left,rf.Top,rf.Width,rf.Height);
				g.DrawRectangle(p,Rectangle.Round(rf));
			} 
			else 
				g.DrawLine(p,x,y,x,y+h);
		}

		private void AdjustAlign(PointF[] pfs,FormulaData fd,float W,float H)
		{
			float x=0;
			float y=0;
			switch (fd.Align)
			{
				case FormulaAlign.Left:
					x = -W;
					break;
				case FormulaAlign.Center:
					x = -(W-1)/2;
					break;
			}

			switch (fd.VAlign)
			{
				case VerticalAlign.Top:
					y = -H;
					break;
				case VerticalAlign.VCenter:
					y = -H/2;
					break;
			}
			RectangleF R = RectangleF.Empty;
			if (Canvas!=null)
				R = Canvas.Rect;
			for(int i=0; i<pfs.Length; i++) 
			{
				pfs[i].X +=x;
				pfs[i].Y +=y;
				
				if (R!=RectangleF.Empty)
				{
					switch (fd.VAlign)
					{
						case VerticalAlign.ScreenBottom:
							pfs[i].Y = R.Bottom-H;
							break;
						case VerticalAlign.ScreenTop:
							pfs[i].Y = R.Top;
							break;
						case VerticalAlign.ScreenCenter:
							pfs[i].Y = (R.Bottom+R.Top)/2;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Render the formula area
		/// </summary>
		/// <param name="g"></param>
		public void Render(Graphics g)
		{
			Render(g,Rect,Parent.Start,Parent.ColumnWidth);
		}

		/// <summary>
		/// Render the formula area
		/// </summary>
		/// <param name="g">Graphics to render</param>
		/// <param name="R">Rectangle</param>
		/// <param name="Start">Zero based start index</param>
		/// <param name="ColumnWidth">Width of each bar</param>
		public void Render(Graphics g,Rectangle R,int Start,double ColumnWidth)
		{
			DrawBackground(R);
			BindYAxis();
			InitCanvas(g,R,Start,ColumnWidth,Parent.ColumnPercent);
			
			foreach(FormulaAxisX fax in AxisXs)
			{
				fax.Render(Canvas,this);
			}

			foreach(FormulaBase fb in Formulas)
			{
				int i = fb.AxisYIndex; 
				if (i<AxisYs.Count) 
				{
					if (fb.CustomLine!=null)
						AxisYs[i].CustomLine = fb.CustomLine;
					else
					{
						AxisYs[i].CustomLine = null;
						if (fb.HideYGridLine)
						{
							AxisYs[i].MajorTick.ShowLine = false;
							AxisYs[i].MajorTick.ShowTick = false;
							AxisYs[i].MajorTick.ShowText = false;

							AxisYs[i].MinorTick.ShowLine = false;
							AxisYs[i].MinorTick.ShowTick = false;
							AxisYs[i].MinorTick.ShowText = false;
						}
					}
				}
			}

			foreach(FormulaAxisY fay in AxisYs) 
			{
				// This 3 lines is needed for draw formula lines
				fay.Rect = fay.FrameRect;
				fay.Rect.Y = Canvas.Rect.Y;
				fay.Rect.Height = Canvas.Rect.Height;
				if (fay.Visible)
					fay.Render(Canvas,this);
			}

			R = Canvas.Rect;
			if (BindingErrors!=null) 
			{
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.DrawString(BindingErrors,new Font("Verdana",10),NameBrush,R,sf);
				return;
			}
			clientRectChanged = R!=LastClientRect || ColumnWidth!=LastColumnWidth || AxisY.MaxY!=LastMaxY || AxisY.MinY!=LastMinY;

			//g.SetClip(new Rectangle(R.Left,R.Top,Canvas.FrameRect.Width,R.Height));
			g.SetClip(Canvas.ClipRect);
			try
			{
				//draw each Formula
				int ColorIndex = -1;
				for(int i =0; i<FormulaDataArray.Count; i++)
				{
					FormulaData fd = GetTransform((FormulaData)FormulaDataArray[i]);
					if (fd.IsNaN()) continue;
					double[] Filter = null;
					if (fd.RenderType==FormulaRenderType.POLY ||
						fd.RenderType==FormulaRenderType.PARTLINE ||
						fd.RenderType==FormulaRenderType.FILLRGN ||
						fd.RenderType==FormulaRenderType.LINE ||
						fd.RenderType==FormulaRenderType.TEXT ||
						fd.RenderType==FormulaRenderType.ICON ||
						fd.RenderType==FormulaRenderType.STICKLINE)
						Filter = fd["COND"];

					PointF[] pfs2;
					PointF[] pfs = fd.GetPoints(fd.RenderType==FormulaRenderType.PARTLINE || 
						fd.RenderType==FormulaRenderType.FILLRGN 
						,Filter);

					FormulaDataArray[i].CopyPoint(fd);
					if (!fd.SameColor || ColorIndex<0)
						ColorIndex++;
					Pen CurrentPen = GetCurrentPen(fd,ColorIndex);
					Font CurrentTextFont = GetCurrentFont(ColorIndex);
					int w = (int)(ColumnWidth*Parent.ColumnPercent+0.4);
					Pen[] Pens;

					switch (fd.RenderType)
					{
						case FormulaRenderType.VOLSTICK:
							RectangleF[] rfs = new RectangleF[1];
							FormulaData fdClose = DataProvider["CLOSE"];

							FormulaData fdRising;
							if (StockRenderType <= StockRenderType.OHLCBars)
								 fdRising= fdClose>FormulaBase.REF(fdClose,1);
							else 
							{
								FormulaData fdOpen = DataProvider["OPEN"];
								fdRising = fdClose>fdOpen;
							}
							fdRising = fdRising*fd.MaxValue(Canvas.Start,Canvas.Count);
							fdRising.AxisY = fd.AxisY;
							fdRising.AxisYIndex = fd.AxisYIndex;
							fdRising.Canvas = this.Canvas;
							pfs2 = fdRising.GetPoints();

							float Z = fd.AxisY.CalcY(0);
							Pens = AdjustPenWidth(w);
							Pens = AdjustStickPenWidth(Pens,w);
							Pens = AdjustAlpha(Pens,fd.Alpha);
							Brush[] Brushes = AdjustAlpha(BarBrushes,fd.Alpha);

							for(int j=0; j<pfs.Length; j++)
							{
								Pen P = CurrentPen;
								Brush B = null;
								if (pfs2!=null) 
								{
									int pi = pfs2[j].Y==Z?2:0;
									P = Pens[pi];
									B = Brushes[pi];
								}
								DrawStick(g,pfs[j].X,pfs[j].Y,R.Bottom-pfs[j].Y,P,B,ColumnWidth);
							}
							break;
						case FormulaRenderType.COLORSTICK:
							float Zero = fd.AxisY.CalcY(0);
							Pens = AdjustPenWidth(w);
							Pens = AdjustStickPenWidth(Pens,w);
							for(int j=0; j<pfs.Length; j++)
							{
								try
								{
									int pi = pfs[j].Y<Zero?0:2;
									Pen P = Pens[pi];
									Brush B = BarBrushes[pi];
									//B = AdjustBrush(B,fd);
									//PointF pfb = new PointF(pfs[j].X,Zero);
									DrawStick(g,pfs[j].X,pfs[j].Y,Zero-pfs[j].Y,P,B,ColumnWidth);
									//g.DrawLine(g,pfs[j],pfb);
								} 
								catch
								{
								}
							}
							break;
						case FormulaRenderType.STICKLINE:
							double[] P2 =(double[])fd["PRICE2"];
							if (P2!=null)
							for(int j=0; j<pfs.Length; j++)
							{
								PointF pfb = new PointF(pfs[j].X,fd.AxisY.CalcY(P2[j]));
								DrawStick(g,pfs[j].X,pfs[j].Y,pfb.Y-pfs[j].Y,CurrentPen,AdjustBrush(null,fd),ColumnWidth*0.9);
								//g.DrawLine(CurrentPen,pfs[j],pfb);
							}
							break;
						case FormulaRenderType.TEXT:
							double[] dd =(double[])fd["NUMBER"];
							string Text = (string)fd.OwnerData["TEXT"];
							string FormatText = Text;
							object o = fd.OwnerData["FORMAT"];
							string FormatString =null;
							if (o!=null)
								FormatString = o.ToString();
							PointF[] pfs3 = fd.GetPoints(dd,false,fd["COND"],true);
							for(int j=0; j<pfs.Length; j++)
							{
								if (dd!=null) 
								{
									if (FormatString==null)
										Text = ReplaceTag((int)pfs3[j].Y,fd,FormatText);
									else Text = dd[(int)pfs3[j].Y].ToString(FormatString);
								}

								PointF pfText = new PointF(pfs[j].X,pfs[j].Y);
								FormulaLabel fl = Labels[fd.LabelIndex % Labels.Length];
								Brush tb = fl.TextBrush;
								if (tb==null) tb = new SolidBrush(CurrentPen.Color);
								fl.DrawString(g,Text,CurrentTextFont,tb,fd.VAlign,fd.Align,pfText,fd.VAlign!=VerticalAlign.VCenter);
							}
							break;
						case FormulaRenderType.ICON:
							Image I = (Image)fd.OwnerData["ICON"];
							if (I!=null)
							{
								AdjustAlign(pfs,fd,I.Width,I.Height);
								for(int j=0; j<pfs.Length; j++)
									g.DrawImage(I,pfs[j]);
							}
							break;
						case FormulaRenderType.VERTLINE:
							for(int j=0; j<pfs.Length; j++)
							{
								if (fd.Data[j]==1)
								{
									g.DrawLine(CurrentPen,pfs[j].X,R.Top,pfs[j].X,R.Bottom);
								}
							}
							break;
						case FormulaRenderType.LINE:
							pfs2 = fd.GetPoints("PRICE2",false,"COND2");
							for(int j=0; j<pfs.Length; j++)
							{
								for(int k=0; k<pfs2.Length; k++) 
								{
									if (pfs[j].X<pfs2[k].X)
									{
										g.SmoothingMode = fd.Smoothing;
										g.DrawLine(CurrentPen,pfs[j].X,pfs[j].Y,pfs2[k].X,pfs2[k].Y);
										while(j<pfs.Length && pfs[j].X<pfs2[k].X) j++;
										break;
									}
								}
							}
							break;
//						case FormulaRenderType.POLY:
//							DrawPoints(g,fd,CurrentPen,pfs2,ColumnWidth);
//							for(int j=0; j<pfs.Length-1; j++)
//								g.DrawLine(CurrentPen,pfs[j].X,pfs[j].Y,pfs[j+1].X,pfs[j+1].Y);
//							break;
						case FormulaRenderType.STOCK:
						case FormulaRenderType.MONOSTOCK:
							if (StockRenderType == StockRenderType.Line)
								DrawPoints(g,fd,CurrentPen,pfs,ColumnWidth);
							else
							{
								PointF[] pfsH = fd.GetPoints("H");
								PointF[] pfsL = fd.GetPoints("L");
								PointF[] pfsO = fd.GetPoints("O");
								
								if (pfsH==null || pfsL==null || pfsO==null)
								{
									DrawPoints(g,fd,CurrentPen,pfs,ColumnWidth);
									break;
								}

								Pens = AdjustPenWidth(w);

								for(int j=0; j<pfs.Length; j++)
								{
									bool UpDown = pfsO[j].Y>pfs[j].Y;
									if (StockRenderType <= StockRenderType.OHLCBars)
									{
										if (j<pfs.Length-1)
											UpDown = pfs[j+1].Y>pfs[j].Y;
										else UpDown = true;
									}

									Pen p = Pens[UpDown?0:2];
									Brush b = BarBrushes[UpDown?0:2];

									b = AdjustBrush(b,fd);

									if (fd.RenderType==FormulaRenderType.MONOSTOCK)
										p = CurrentPen;

									if (w<=1 || StockRenderType <= StockRenderType.OHLCBars) 
										g.DrawLine(p,pfsH[j],pfsL[j]);

									if (StockRenderType == StockRenderType.HLCBars)
									{
										PointF pf = pfs[j];
										pf.X  +=w+1;
										g.DrawLine(p, Point.Round(pfs[j]), Point.Round(pf));
									}
									else if (StockRenderType == StockRenderType.OHLCBars)
									{
										PointF pf = pfs[j];
										pf.X  +=w/2+1;
										g.DrawLine(p, Point.Round(pfs[j]), Point.Round(pf));

										pf = pfsO[j];
										pf.X  -=w/2+1;
										g.DrawLine(p, Point.Round(pfsO[j]), Point.Round(pf));
									}
									else 
									{
										float x = pfs[j].X-w/2;
										float y = Math.Min(pfs[j].Y,pfsO[j].Y);
										float h = Math.Abs(pfsO[j].Y-pfs[j].Y);
										if (h==0) 
										{
											p = BarPens[1];
											if (w>0)
												g.DrawLine(p,x,y,x+w,y);
											else g.DrawLine(p,x,y,x,y+1);
											g.DrawLine(p,pfsH[j],pfsL[j]);
										}
										else if (w>1)
										{
											RectangleF r = new RectangleF(x,y,w,h);
											g.DrawLine(p,pfsH[j],new PointF(pfsH[j].X,r.Top));
											g.DrawLine(p,pfsL[j],new PointF(pfsL[j].X,r.Bottom));
											try
											{
												if (b!=null)
													g.FillRectangle(b,r);
												//g.DrawRectangles(p,new RectangleF[]{r});
												g.DrawRectangle(p, Rectangle.Round(r));
												//g.DrawLines(p, new PointF[]{new PointF(r.Left,r.Top), new PointF(r.Left,r.Bottom), new PointF(r.Right,r.Bottom),new PointF(r.Right,r.Top),new PointF(r.Left,r.Top) }); //g.DrawRectangles(p,new RectangleF[]{r});
											} 
											catch
											{
											}
										}
									}
								}
							}
							break;
						case FormulaRenderType.FILLRGN:
							pfs2 = fd.GetPoints( "PRICE2",true,"COND");
							PointF[] pfsN1 = fd.GetPoints(true);
							PointF[] pfsN2 = fd.GetPoints("PRICE2",true);

							ArrayList al = new ArrayList();
							for(int j=0; j<=pfs.Length; j++) 
								if (j<pfs.Length && pfs[j]!=PointF.Empty) 
								{
									if (al.Count==0)
										InsertPoint(al,pfsN1,pfsN2,j);
									al.Insert(al.Count/2,pfs2[j]);
									al.Insert(al.Count/2,pfs[j]);
								} 
								else 
								{
									if (al.Count>0) 
									{
										InsertPoint(al,pfsN1,pfsN2,j);
										PointF[] pfsFill = (PointF[])al.ToArray(typeof(PointF));
										Brush br = fd.AreaBrush;
										if (br==null) br = new SolidBrush(Color.FromArgb(128,Color.Blue));
										if (pfs.Length>1)
											g.FillPolygon(br,pfsFill);
										al.Clear();
									}
								}
							break;
						case FormulaRenderType.FILLAREA:
							PointF[] pfsN = fd.GetPoints(false);
							if (pfsN.Length>0)
							{
								ArrayList alArea = new ArrayList();
								alArea.AddRange(pfsN);
								alArea.Add(new PointF(pfsN[pfsN.Length-1].X,R.Bottom));
								alArea.Add(new PointF(pfsN[0].X,R.Bottom));
								pfsN = (PointF[])alArea.ToArray(typeof(PointF));
								Brush br = fd.AreaBrush;
								if (br==null) br = new SolidBrush(Color.FromArgb(128,Color.Blue));
								if (pfs.Length>1)
									g.FillPolygon(br,pfsN);
							}
							break;
						case FormulaRenderType.PARTLINE:
							ArrayList alPoint = new ArrayList();
							for(int j=0; j<pfs.Length; j++) 
							{
								if (j == pfs.Length-1 || pfs[j].IsEmpty)
								{
									if (alPoint.Count>0) 
									{
										pfs2 = (PointF[])alPoint.ToArray(typeof(PointF));
										DrawPoints(g,fd,CurrentPen,pfs2,ColumnWidth);
									}
									alPoint.Clear();
								} 
								else
									alPoint.Add(pfs[j]);
							}
							break;
						case FormulaRenderType.AXISY:
							if (pfs.Length>0) 
							{
								double start = (double)fd.OwnerData["START"];
								double end = (double)fd.OwnerData["END"];
								float Y = pfs[0].Y;
								R = fd.AxisY.FrameRect;
								float x1 = (float)(R.X+start);
								float x2 = (float)(R.Right-end);
								if (fd.AxisY.AxisPos==AxisPos.Left)
								{
									x1 = (float)(R.Right-start);
									x2 = (float)(R.X+end);
								}
								g.DrawLine(CurrentPen,x1,Y,x2,Y);
							}
							break;
						case FormulaRenderType.AXISYTEXT:
							if (pfs.Length>0) 
							{
								string T = (string)fd.OwnerData["TEXT"];
								float start = (float)(double)fd.OwnerData["START"];
								float Y = pfs[0].Y;
								float X = 0;
								R = fd.AxisY.FrameRect;

								FormulaAlign fa = fd.Align;
								if (fd.AxisY.AxisPos == AxisPos.Right)
								{
									fa = FormulaAlign.Right;
									X = R.Right-start;
								} 
								else 
								{
									fa = FormulaAlign.Left;
									X = R.X+start;							
								}

								PointF pfText = new PointF(X,Y);
								FormulaLabel fl = Labels[fd.LabelIndex % Labels.Length];
								Brush tb = fl.TextBrush;
								if (tb==null) tb = new SolidBrush(CurrentPen.Color);
								fl.DrawString(g,T,CurrentTextFont,tb,fd.VAlign,fa,pfText,fd.VAlign!=VerticalAlign.VCenter);
							}
							break;
						default:
							DrawPoints(g,fd,CurrentPen,pfs,ColumnWidth);
							break;
					}
					DrawSelectedSign(g,FormulaDataArray[i],pfs);
				}
			} 
			finally 
			{
				LastClientRect = R;
				LastColumnWidth = ColumnWidth;
				LastMinY = AxisY.MinY;
				LastMaxY = AxisY.MaxY;
				g.ResetClip();
			}
			/*
			if (FormulaDataArray.Count>0)
			{
				DrawValueText();
			}
			*/
		}

		public RectangleF GetLastBarRect()
		{
			if (!ClientRectChanged)
				if (FormulaDataArray.Count>0) 
				{
					FormulaData fd = FormulaDataArray[0];
					PointF[] pfs = fd.GetPoints();
					if (pfs.Length>1)
					{
						PointF pf1 = pfs[0];
						PointF pf2 = pfs[1];
						RectangleF rf = new RectangleF(pf2.X,Rect.Top,(pf1.X-pf2.X)*2,Rect.Height);
						return rf;
					}
				}
			return Rect;
		}

		/// <summary>
		/// Get bitmap of current stock area
		/// </summary>
		/// <param name="Width">Bitmap width</param>
		/// <param name="Height">Bitmap height</param>
		/// <param name="Start">Zero based start index</param>
		/// <param name="ColumnWidth">Width of each bar</param>
		/// <returns>Bitmap of this FormulaArea</returns>
		public Bitmap GetBitmap(int Width, int Height,int Start,float ColumnWidth)
		{
			Bitmap b = new Bitmap(Width,Height);
			Render(Graphics.FromImage(b),new Rectangle(0,0,b.Width,b.Height),Start,/*Count,*/ColumnWidth);
			return b;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Return the formula data separate by semicolon</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for(int i =0; i<FormulaDataArray.Count; i++) 
			{
				if (i!=0)
					sb.Append(';');
				sb.Append(FormulaDataArray[i]);
			}
			return sb.ToString();
		}

		/// <summary>
		/// The unique name of this formula
		/// </summary>
		/// <returns></returns>
		public string GetUnique() 
		{
			return Name+"."+DataProvider.GetUnique();
		}

		/// <summary>
		/// determine if this FormulaArea is the area
		/// </summary>
		/// <returns></returns>
		public bool IsMain(int AxisYIndex) 
		{
			for(int i =0; i<FormulaDataArray.Count; i++)
				if ((this[i].RenderType == FormulaRenderType.STOCK || 
					this[i].RenderType==FormulaRenderType.MONOSTOCK ||
					string.Compare(this[i].Name,"Main",true)==0) && 
					(AxisYIndex<0 || this[i].AxisYIndex==AxisYIndex))
					return true;
			return false;
		}

		/// <summary>
		/// determine if this FormulaArea is the area
		/// </summary>
		/// <returns></returns>
		public bool IsMain() 
		{
			return IsMain(-1);
		}

		public void RemoveAutoMultiplyForStockYAxis()
		{
			for(int i =0; i<FormulaDataArray.Count; i++)
				if (this[i].RenderType == FormulaRenderType.STOCK)
					AxisYs[this[i].AxisYIndex].AutoMultiply = false;
		}

		/// <summary>
		/// Convert all formulas to string array
		/// </summary>
		/// <param name="Start">Start index</param>
		/// <param name="Count">Count</param>
		/// <returns></returns>
		public string[] FormulaToStrings(int Start,int Count)
		{
			string[] ss = new string[Count];
			for(int i=0; i<ss.Length; i++) 
			{
				FormulaBase fb = Formulas[i+Start];
				ss[i] = fb.CreateName;
			}
			return ss;
		}

		/// <summary>
		/// Convert all formulas to string array
		/// </summary>
		/// <returns></returns>
		public string[] FormulaToStrings()
		{
			return FormulaToStrings(0,Formulas.Count);
		}

		/// <summary>
		/// Convert all formulas to string
		/// </summary>
		/// <returns></returns>
		public string FormulaToString(char separator)
		{
			return string.Join(separator.ToString(),FormulaToStrings());
		}

		/// <summary>
		/// Convert all formulas to string
		/// </summary>
		/// <param name="separator"></param>
		/// <param name="Start"></param>
		/// <param name="Count"></param>
		/// <returns></returns>
		public string FormulaToString(char separator,int Start,int Count)
		{
			return string.Join(separator.ToString(),FormulaToStrings(Start,Count));
		}

		/// <summary>
		/// Convert string array to formula
		/// </summary>
		/// <param name="ss">formula name array</param>
		/// <param name="Quote">Stock Symbol</param>
		public void StringsToFormula(string[] ss,string Quote)
		{
			for(int i=0; i<ss.Length; i++)
				AddFormula(ss[i],Quote);
		}

		/// <summary>
		/// Convert string array to formula
		/// </summary>
		/// <param name="ss">formula name array</param>
		public void StringsToFormula(string[] ss)
		{
			StringsToFormula(ss,"");
		}

		/// <summary>
		/// Convert string 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="Quote">Stock Symbol</param>
		/// <param name="separator"></param>
		public void StringToFormula(string s,string Quote, char separator)
		{
			StringsToFormula(s.Split(separator),Quote);
		}

		/// <summary>
		/// Convert string 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="separator"></param>
		public void StringToFormula(string s,char separator)
		{
			StringToFormula(s,"",separator);
		}


		/// <summary>
		/// Remove unused Y-Axis 
		/// </summary>
		public void RemoveUnusedAxisY()
		{
			ArrayList al = new ArrayList();

			for(int i=0; i<Formulas.Count; i++)
			{
				int YIndex = Formulas[i].AxisYIndex;
				if (al.IndexOf(YIndex)<0)
					al.Add(YIndex);
			}

			if (al.IndexOf(1)<0 && AxisYs.Count>1)
				AxisYs.RemoveAt(1);

			if (al.IndexOf(0)<0 && AxisYs.Count>1)
			{
				AxisYs.RemoveAt(1);
				for(int i=0; i<FormulaDataArray.Count; i++)
					this[i].AxisYIndex = 0;
				for(int i=0; i<Formulas.Count; i++)
					Formulas[i].AxisYIndex = 0;
			}
		}
	}

	/// <summary>
	/// Collection of stock Formula area
	/// </summary>
	public class AreaCollection:CollectionBase
	{
		public virtual int Add(FormulaArea fa)
		{
			return List.Add(fa);
		}

		public virtual void Insert(int Index,FormulaArea fa)
		{
			List.Insert(Index,fa);
		}

		public virtual FormulaArea this[int Index] 
		{
			get
			{
				return (FormulaArea)this.List[Index];
			}
		}

		public FormulaArea this[string Name] 
		{
			get
			{
				foreach(object o in List)
					if (string.Compare(((FormulaArea)o).Name,Name,true)==0)
						return (FormulaArea)o;
				return null;
			}
		}

		public int IndexOf(FormulaArea fa) 
		{
			return List.IndexOf(fa);
		}

		public void Remove(FormulaArea value) 
		{
			List.Remove(value);
		}
		
		public void Remove(string Name) 
		{
			List.Remove(this[Name]);
		}
	}
}