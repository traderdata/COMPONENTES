///
/// Easy Stock Chart Core Class Lib
/// Copyright : Easychart Inc
///	Email : support@easychart.net
///	
//#define TRIAL
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Web;
using System.Web.Caching;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance
{
	public enum ChartDragMode {None,Axis,Chart};
	public enum LatestValueType {None,StockOnly,All,Custom};
	public enum TwoYAxisType {AreaSame, AreaDifferent};
	public enum ValueTextMode {None,TextOnly,ValueOnly,Both,Default};

	/// <summary>
	/// Mouse Action
	/// </summary>
	public enum MouseAction
	{
		/// <summary>
		/// No action
		/// </summary>
		None,
		/// <summary>
		/// Do action when mouse move
		/// </summary>
		MouseMove,
		/// <summary>
		/// Do action when mouse down
		/// </summary>
		MouseDown
	};

	/// <summary>
	/// Stock Formula chart , collection of stock Formula areas
	/// </summary>
	public class FormulaChart
	{
		private static Random Rnd = new Random();
//#if(TRIAL)
        private string TrialVersion = "Terminal Traderdata\nhttp://www.traderdata.com.br";
//#endif
		private Bitmap MemBmp;
		private Graphics MemG;
		private Bitmap ExtraMemBmp;
		//private Bitmap backgroungImage;
		//private Bitmap ExtraMemG;

		private IDataProvider dataProvider;
		internal int LabelWidth;
		internal int LabelWidth2;
		private double columnWidth = 7;
		private Graphics CurrentGraph;

		private RectangleF LastHLine = Rectangle.Empty;
		private RectangleF LastVLine = Rectangle.Empty;

		/// <summary>
		/// How to draw latest data in axis Y 
		/// </summary>
		public LatestValueType LatestValueType=LatestValueType.None;

		/// <summary>
		/// How to draw the value text
		/// </summary>
		public ValueTextMode ValueTextMode = ValueTextMode.Default;

		public bool NeedRedraw = true;
		public bool ExtraNeedRedraw;
		public bool ShowHLine = true;
		public bool ShowVLine = true;
		public bool ShowCursorLabel;
		public int RenderCount;
		public Pen CursorPen;
		public bool ShowXAxisInLastArea;
		public bool FixedTime;
		public bool ExpandDate = true;
		public FormulaUserSkin UserSkin = new FormulaUserSkin();

		public event NativePaintHandler NativePaint;
		public event NativePaintHandler ExtraPaint;
		public event ViewChangedHandler ViewChanged;

		/// <summary>
		/// return current Y-axis width
		/// </summary>
		public int CurrentYAxisWidth
		{
			get
			{
				return LabelWidth;
			}
		}

		/// <summary>
		/// Width of one stock data
		/// </summary>
		public double ColumnWidth 
		{
			get
			{
				return columnWidth;
			}
			set
			{
				columnWidth = value;
			}
		}

		public TwoYAxisType TwoYAxisType = TwoYAxisType.AreaSame;

		/// <summary>
		/// Margin of the stock view
		/// </summary>
		public int MarginWidth = 6;

		/// <summary>
		/// The start index of the data
		/// </summary>
		public int Start;
		/// <summary>
		/// When StockRenderType is Candle or Bar, this property specify the stock entity percentage of the column width
		/// </summary>
		public float ColumnPercent = 0.6f;
		/// <summary>
		/// The start date time of the stock chart
		/// </summary>
		public DateTime StartTime = DateTime.MinValue;
		/// <summary>
		/// The end date tiem of the stock chart
		/// </summary>
		public DateTime EndTime = DateTime.MaxValue;
		/// <summary>
		/// The stock formula areas
		/// </summary>
		public AreaCollection Areas;
		/// <summary>
		/// If using the bitmap cache
		/// </summary>
		public bool BitmapCache;
		
		/// <summary>
		/// Current cursor pos
		/// </summary>
		public int CursorPos;

		/// <summary>
		/// Display value labels in each areas
		/// </summary>
		public bool ShowValueLabel = true;

		/// <summary>
		/// Collection of XFormat
		/// </summary>
		public XFormatCollection AllXFormats;

		public Layout PriceLabelLayout;
		private string priceLabelFormat;
		/// <summary>
		/// If you specify this, will add these text in the price area
		/// Sample format:
		/// Prev Close:{CODE} {LC}O:{OPEN}H:{HIGH}L:{LOW}C:{CLOSE}V:{VOLUME}Chg:{Change}
		/// </summary>
		public string PriceLabelFormat
		{
			get
			{
				return priceLabelFormat;
			}
			set
			{
				priceLabelFormat = value;
				PriceLabelLayout = Layout.ParseString("ColorText="+value,Rectangle.Empty);
			}
		}
		
		/// <summary>
		/// Volumn stick and color stick render type
		/// </summary>
		public StickRenderType StickRenderType;

		//public StatisticWindow Statistic;

		/// <summary>
		/// The stock data provider of this chart
		/// </summary>
		public IDataProvider DataProvider
		{
			get 
			{
				return dataProvider;
			}
			set 
			{
				dataProvider = value;
				Bind();
			}
		}

		public void SetDataProviderNoBind(IDataProvider idp)
		{
			if (dataProvider!=null)
				idp.DataCycle = dataProvider.DataCycle;
			dataProvider = idp;
		}

		public void AdjustStartEndTime(int InViewBarsCount)
		{
			if (DataProvider!=null)
			{
				double[] fdDate = DataProvider["DATE"];
				if (fdDate!=null)
				{
					EndTime = DateTime.FromOADate(fdDate[fdDate.Length-1-Start]);
					if (columnWidth<0.00001)
						columnWidth = 0.00001;
					int Stop = Start + InViewBarsCount;
					if (Stop>=fdDate.Length)
					{
						Stop = fdDate.Length-1;
						Start = 0;
						EndTime = DateTime.FromOADate(fdDate[0]+InViewBarsCount);//DateTime.FromOADate(fdDate[fdDate.Length-1-Start]);
					}
					StartTime = DateTime.FromOADate(fdDate[fdDate.Length-1-Stop]);
					//ApplyXFormat();
				}
			}
		}

		public void AdjustStartEndTime()
		{
			AdjustStartEndTime((int)((Rect.Width-LabelWidth-LabelWidth2)/columnWidth));
		}

		/// <summary>
		/// The drawing rectangle of the stock chart
		/// </summary>
		public Rectangle Rect;

		public void ApplyXFormat(double Days100)
		{
			if (AllXFormats!=null)
			{
				foreach(FormulaXFormat fxf in AllXFormats)
				{
					if (Days100<fxf.Days100Pixel)
					{
						int R = fxf.Interval.Repeat;
						if (fxf.CycleDivide>0)
							R = (int)(Days100/fxf.CycleDivide);
						if (R==0) R = 1;
						DataCycle = new DataCycle(fxf.Interval.CycleBase,R);
						XCursorFormat = fxf.XCursorFormat;
						//AxisLabelAlign = fxf.AxisLabelAlign;
						AxisXFormat = fxf.XFormat;
						
						if (fxf.Visible!=null)
							fxf.SetVisible(this);
						if (fxf.ShowMajorLine!=null)
							fxf.SetMajorLine(this);
						if (fxf.ShowMinorLine!=null)
							fxf.SetMinorLine(this);
						break;
					}
				}
			}
		}

		/// <summary>
		/// The data cycle of this data stock chart
		/// </summary>
		public DataCycle DataCycle 
		{
			set 
			{
				foreach(FormulaArea fa in Areas)
					fa.AxisX.DataCycle = value;
			}
		}

		public AxisLabelAlign AxisLabelAlign
		{
			set
			{
				foreach(FormulaArea fa in Areas)
					fa.AxisX.AxisLabelAlign = value;
			}
		}

		/// <summary>
		/// Set CursorFormat of AxisX
		/// </summary>
		public string XCursorFormat
		{
			set
			{
				foreach(FormulaArea fa in Areas)
					fa.AxisX.CursorFormat = value;
			}
			get
			{
				if (Areas.Count>0)
					return Areas[0].AxisX.CursorFormat;
				return "dd-MMM-yyyy dddd";
			}
		}

		/// <summary>
		/// X-Axis label format
		/// </summary>
		public string AxisXFormat
		{
			set 
			{
				foreach(FormulaArea fa in Areas)
					fa.AxisX.Format = value;
			}
		}

		/// <summary>
		/// Y-Axis label format
		/// </summary>
		public string AxisYFormat
		{
			set 
			{
				foreach(FormulaArea fa in Areas)
					fa.AxisY.Format = value;
			}
		}

		public void SetAxisXFormat(int Index,string value)
		{
			foreach(FormulaArea fa in Areas)
				if (Index>=0 && Index<fa.AxisXs.Count)
					fa.AxisXs[Index].Format = value;
		}

		public void SetAxisXVisible(int Index,bool value)
		{
			foreach(FormulaArea fa in Areas)
				if (Index>=0 && Index<fa.AxisXs.Count)
					fa.AxisXs[Index].Visible = value;
		}

		public void SetAxisXDataCycle(int Index,DataCycle dc) 
		{
			foreach(FormulaArea fa in Areas)
				if (Index>=0 && Index<fa.AxisXs.Count)
					fa.AxisXs[Index].DataCycle = dc;
		}

		public void SetAxisXShowMajorLine(int Index,bool value) 
		{
			foreach(FormulaArea fa in Areas)
				if (Index>=0 && Index<fa.AxisXs.Count)
					fa.AxisXs[Index].MajorTick.ShowLine = value;
		}

		public void SetAxisXShowMinorLine(int Index,bool value) 
		{
			foreach(FormulaArea fa in Areas)
				if (Index>=0 && Index<fa.AxisXs.Count)
					fa.AxisXs[Index].MinorTick.ShowLine = value;
		}

		/// <summary>
		/// Get the stock formula area by name
		/// </summary>
		public FormulaArea this[string Name]
		{
			get 
			{
				return Areas[Name];
			}
		}

		/// <summary>
		/// Get the stock formula area by index
		/// </summary>
		public FormulaArea this[int index]
		{
			get 
			{
				return Areas[index];
			}
		}

		/// <summary>
		/// Create the stock formula chart, add the main stock formula area
		/// </summary>
		public FormulaChart()
		{
			//ExtraPaint +=new NativePaintHandler(FormulaChart_ExtraPaint);
			Areas = new AreaCollection();
			
			EndTime = DateTime.Now.AddDays(1);
			StartTime = EndTime.AddMonths(-6);
		}

		public FormulaChart(Rectangle Rect):this()
		{
			ColumnWidth = 7;
			Start = 0;
			this.Rect = Rect;
		}

		/// <summary>
		/// Set the predefined stock chart skin
		/// </summary>
		/// <param name="fs"></param>
		public void SetSkin(FormulaSkin fs) 
		{
			if (fs!=null)
				fs.Bind(this);
		}

		/// <summary>
		/// Set predefined stock chart skin
		/// </summary>
		/// <param name="Skin"></param>
		public void SetSkin(string Skin) 
		{
			FormulaSkin fs = FormulaSkin.GetSkinByName(Skin);
			if (fs!=null)
				SetSkin(fs);
		}

		/// <summary>
		/// Bind the stock Formula to data provider
		/// </summary>
		public void Bind() 
		{
			CursorPos = -1;

			foreach(FormulaArea fa in Areas)
				fa.Bind();
		}

		/// <summary>
		/// Add a new stock Formula area
		/// </summary>
		/// <param name="fb"></param>
		public void AddArea(FormulaBase fb)
		{
			FormulaArea fa = new FormulaArea(this);
			fa.HeightPercent = 1;
			fa.AddFormula(fb);
			Areas.Add(fa);
		}

		/// <summary>
		/// Add a new stock formula area
		/// </summary>
		/// <param name="Name">Formula Name, such as MA(10)</param>
		/// <param name="Quote">Stock Symbol , such as MSFT</param>
		/// <param name="Percent">Percentage of this area</param>
		public void AddArea(string Name,string Quote,double Percent)
		{
			FormulaArea fa = new FormulaArea(this);
			fa.HeightPercent = Percent;
			fa.StringToFormula(Name,Quote,'#');
			Areas.Add(fa);

//			string[] ss = Name.Split('#');
//			if (ss.Length>0)
//			{
//				FormulaArea fa = new FormulaArea(this,ss[0].TrimEnd('!'),Quote,Percent);
//				for(int i=1; i<ss.Length; i++)
//					fa.AddFormula(ss[i].TrimEnd('!'));
//				Areas.Add(fa);
//			}
		}

		/// <summary>
		/// Add a new stock formula area
		/// </summary>
		/// <param name="Name">Formula Name, such as MA(10)</param>
		/// <param name="Percent">Percentage of this area</param>
		public void AddArea(string Name,double Percent)
		{
			AddArea(Name,"",Percent);
		}

		/// <summary>
		/// Add a new stock formula area by stock Formula name
		/// </summary>
		/// <param name="Name">Formula Name, such as MA(10)</param>
		public void AddArea(string Name)
		{
			AddArea(Name,1);
		}

		/// <summary>
		/// Insert stock formula at Index
		/// </summary>
		/// <param name="Index"></param>
		/// <param name="fb"></param>
		public void InsertArea(int Index,FormulaBase fb)
		{
			FormulaArea fa = new FormulaArea(this);
			fa.HeightPercent = 1;
			fa.AddFormula(fb);
			Areas.Insert(Index,fa);
		}

		/// <summary>
		/// Insert stock formula at Index
		/// </summary>
		/// <param name="Index"></param>
		/// <param name="Name">Formula Name such as MA(10)</param>
		/// <param name="Percent">Percentage of this area</param>
		public void InsertArea(int Index,string Name,double Percent) 
		{
			InsertArea(Index,Name,"",Percent);
		}

		/// <summary>
		/// Insert stock formula at Index
		/// </summary>
		/// <param name="Index"></param>
		/// <param name="Name">Formula Name such as MA(10)</param>
		/// <param name="Quote">Stock Symbol , such as MSFT</param>
		/// <param name="Percent">Percentage of this area</param>
		public void InsertArea(int Index,string Name,string Quote,double Percent) 
		{
			Areas.Insert(Index,new FormulaArea(this,Name,Quote,Percent));
		}

		/// <summary>
		/// Insert stock formula at Index
		/// </summary>
		/// <param name="Index"></param>
		/// <param name="Name">Formula Name such as MA(10)</param>
		public void InsertArea(int Index,string Name) 
		{
			InsertArea(Index,Name,1);
		}

		public void RemoveArea(string Name) 
		{
			Areas.Remove(Name);
		}

		/// <summary>
		/// Create memory bitmap & graphics
		/// </summary>
		/// <param name="R"></param>
		private void SetMemGraphics(Rectangle R)
		{
			if (MemBmp==null)
			{
				MemBmp = new Bitmap(R.Width,R.Height,PixelFormat.Format32bppPArgb);
				MemG = Graphics.FromImage(MemBmp);
			}
			else
			{
				if (MemBmp.Width!=R.Width || MemBmp.Height!=R.Height)
				{
					MemBmp = new Bitmap(R.Width,R.Height,PixelFormat.Format32bppPArgb);
					MemG = Graphics.FromImage(MemBmp);
					GC.Collect();
				}
			}
		}

		public int GetTotalBars()
		{
			//double[] dd = DataProvider["DATE"];
			//int i1=FindIndex(dd,EndTime.ToOADate(),-1);
			//int i2=FindIndex(dd,StartTime.ToOADate());
			int i1=DateToIndex(EndTime,-1);
			int i2=DateToIndex(StartTime);
			return i1-i2;
		}

		/// <summary>
		/// Recalculate the column width by StartTime and EndTime
		/// </summary>
		private void SetView()
		{
			double[] dd = DataProvider["DATE"];
			double W = (double)(Rect.Width-LabelWidth-LabelWidth2-MarginWidth);
			if (StartTime==DateTime.MinValue || EndTime==DateTime.MaxValue)
			{
				int i = dd.Length-1-Start;
				if (i<0) i=0;
				if (i>=dd.Length) i = dd.Length-1;
				if (i>=0)
				{
					EndTime = DateTime.FromOADate(dd[i]);
					i = i-(int)(W/ColumnWidth);
				
					if (i<0) i=0;
					if (i>=dd.Length) i = dd.Length-1;
					StartTime = DateTime.FromOADate(dd[i]);
				}
			}

			int i1 = DateToIndex(EndTime,-1);
			int i2 = DateToIndex(StartTime);
			if (i2<0) i2 = 0;
			if (i1<i2) i1 = i2;
			if (i1>=0)
			{
				StartTime = IndexToDate(i2);
				EndTime = IndexToDate(i1);
				columnWidth = W/(i1-i2+1); //+1
				Start = dd.Length-i1-1;
				if (i1>=dd.Length) i1 = dd.Length-1;
				W = columnWidth*(i1-i2+1);
				ApplyXFormat((IndexToDoubleDate(i1)-IndexToDoubleDate(i2))/W*100);
				if (ViewChanged!=null)
					ViewChanged(this,new ViewChangedArgs(i2,i1,0,dd.Length-1));
			}
		}
		
		/// <summary>
		/// Recalculate the stock label width
		/// </summary>
		/// <param name="g"></param>
		private void AdjustLabelWidth(Graphics g)
		{
			LabelWidth = int.MinValue;
			foreach(FormulaArea fa in Areas)
				if (fa.Visible)
					LabelWidth = Math.Max(LabelWidth,fa.CalcLabelWidth(g));
			LabelWidth +=4;
			foreach(FormulaArea fa in Areas)
				if (fa.Visible)
					fa.AxisY.Width = LabelWidth;

			for(int i=1; i<2; i++) 
			{
				LabelWidth2 = 0;
				int M = 0;
				foreach(FormulaArea fa in Areas)
					if (fa.Visible && fa.AxisYs.Count>i)
						M = Math.Max(M,fa.AxisYs[i].CalcLabelWidth(fa.Canvas,fa));

				foreach(FormulaArea fa in Areas)
					if (fa.Visible && fa.AxisYs.Count>i)
						fa.AxisYs[i].Width = M;
				LabelWidth2 = M;
			}
			//AdjustStartEndTime();
		}

		private void InternalRender(Graphics g)
		{
			try 
			{
				if (DataProvider==null) return;
				//FormulaBase.ClearCache();
				double Sum = 0;
				foreach(FormulaArea fa in Areas)
					if (fa.Visible)
						Sum +=fa.HeightPercent;

				double r = 0;
				for(int i=0; i<Areas.Count; i++)
				{
					FormulaArea fa = Areas[i];
					if (fa.Visible) 
					{
						fa.Rect = new Rectangle(
							Rect.X , Rect.Y+(int)(Rect.Height*r/Sum),
							Rect.Width,(int)(Rect.Height*fa.HeightPercent/Sum)+1);
						if (i<Areas.Count-1)
							fa.Rect.Height++;
						else fa.Rect.Height = Rect.Bottom-fa.Rect.Top;
						//else fa.Rect.Height--;
						r +=fa.HeightPercent;
					}
				}

				AdjustLabelWidth(g);
				SetView();

				for(int i=0; i<Areas.Count; i++)
				{
					FormulaArea fa = Areas[i];
					if (fa.Visible)
						try 
						{
							if (ShowXAxisInLastArea && i<Areas.Count-1)
								foreach(FormulaAxisX fax in fa.AxisXs)
									fax.Visible = false;
							fa.Render(g);
						} 
						catch (Exception ex) 
						{
							StringFormat sf = new StringFormat();
							sf.Alignment = StringAlignment.Center;
							sf.LineAlignment = StringAlignment.Center;
							g.DrawString(ex.ToString(),new Font("verdana",10),Brushes.Red,fa.Rect,sf);
						}
				}

				if (NativePaint!=null)
					NativePaint(this,new NativePaintArgs(g, Rect,MemBmp));
			}
			catch (Exception e)
			{
				g.DrawString(e.Message,new Font("verdana",10),Brushes.Red,1,30);
//				//throw;
			}

//#if (TRIAL)
			StringFormat DrawFormat = new StringFormat();
			DrawFormat.Alignment = StringAlignment.Center;
			DrawFormat.LineAlignment = StringAlignment.Center;
			g.DrawString(TrialVersion,new Font("Verdana",40,GraphicsUnit.Pixel),
				new SolidBrush(Color.FromArgb(128,Color.Gray)),Rect,DrawFormat);
//#endif
		}

		/// <summary>
		/// If formulas has second YAxisIndex, add second y-axis to all areas 
		/// </summary>
		public void ExtendYAxis(TwoYAxisType  tt)
		{
			TwoYAxisType = tt;
			AxisPos ap = AxisPos.Left;
			bool b = false;
			foreach(FormulaArea fa in Areas)
			{
				for(int i=0; i<fa.Formulas.Count; i++)
				{
					int YIndex = fa.Formulas[i].AxisYIndex;
					if (YIndex==1) 
					{
						if (fa.AxisY.AxisPos==AxisPos.Left)
							ap = AxisPos.Right;
						b = true;
					}
				}
			}

			if (b)
			{
				foreach(FormulaArea fa in Areas)
				{
					for(int i=0; i<fa.Formulas.Count; i++)
					{
						int YIndex = fa.Formulas[i].AxisYIndex;
						if (YIndex==1) 
							fa.AddNewAxisY(ap,true,i);
					}
					if (fa.AxisXs.Count<2) 
					{
						if (tt==TwoYAxisType.AreaSame)
							fa.AddNewAxisY(ap,true,-1);
						else fa.RemoveUnusedAxisY();
					}
				}
			} 
			else 
				foreach(FormulaArea fa in Areas)
					fa.RemoveUnusedAxisY();
		}

		public Bitmap GetMemBitmap(Rectangle R)
		{
			R.Offset(-R.X,-R.Y);
			R.Inflate(1,1);
			if (NeedRedraw)
			{
				SetMemGraphics(R);
				InternalRender(MemG);
				DrawValueText(MemG);
			}

			if (ExtraNeedRedraw && ExtraPaint!=null)
			{
				//MemG.SetClip(CurrentGraph,CombineMode.Complement);
				NativePaintArgs npa = new NativePaintArgs(CurrentGraph,R,MemBmp); //MemG
				ExtraPaint(this,npa);
				if (npa.NewBitmap!=null)
					ExtraMemBmp = npa.NewBitmap;
				ExtraNeedRedraw = false;
			};

			if (ExtraMemBmp!=null)
				return ExtraMemBmp;
			else return MemBmp;
		}

		public Bitmap GetMemBitmap()
		{
			return GetMemBitmap(Rect);
		}

		/// <summary>
		/// Render stock chart to graphics
		/// </summary>
		/// <param name="g"></param>
		/// <param name="R"></param>
		public void Render(Graphics g,Region R)
		{
			CurrentGraph = g;
			if (BitmapCache)
			{
				Bitmap bmp = GetMemBitmap();
				if (R!=null)
				{
					foreach(FormulaArea fa in Areas) 
					{
						R.Union(fa.GetLastBarRect());
						foreach(FormulaAxisY fay in fa.AxisYs)
							R.Union(fay.Rect);
						foreach(FormulaAxisX fax in fa.AxisXs)
							R.Union(fax.Rect);
					}
					g.SetClip(R,CombineMode.Replace);
				}

				g.DrawImage(bmp,Rect.X,Rect.Y);
				NeedRedraw = false;
			}
			else
			{
				InternalRender(g);
				DrawValueText(g);
			}
			RenderCount++;
		}

		/// <summary>
		/// Render stock chart to graphics
		/// </summary>
		/// <param name="g"></param>
		public void Render(Graphics g)
		{
			Render(g,null);
		}

		/// <summary>
		/// Create a bitmap of this stock chart
		/// </summary>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <param name="R"></param>
		/// <returns></returns>
		public Bitmap GetBitmap(int Width,int Height,Rectangle R)
		{
			Bitmap b = new Bitmap(Width,Height,PixelFormat.Format32bppArgb);
			Rect = R;
			Graphics g = Graphics.FromImage(b);
			Render(g);
			return b;
		}

		/// <summary>
		/// Create a bitmap of this stock chart
		/// </summary>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <returns></returns>
		public Bitmap GetBitmap(int Width,int Height)
		{
			return GetBitmap(Width,Height,new Rectangle(0,0,Width,Height-1));
		}

		/// <summary>
		/// Save the formula chart image to a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <param name="R"></param>
		/// <param name="ifm"></param>
		public void SaveToStream(Stream stream , int Width, int Height, Rectangle R,ImageFormat ifm,float X,float Y) 
		{
			Bitmap b = GetBitmap(Width,Height,R);
			Rect = new Rectangle(0,0,Width,Height);
			if (X>0 && Y>0) 
				DrawCursor(Graphics.FromImage(b),X,Y);
			MemoryStream ms = new MemoryStream();
			b.Save(ms,ifm);
			ms.WriteTo(stream);
		}

		/// <summary>
		/// Save the formula chart image to a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <param name="R"></param>
		/// <param name="ifm"></param>
		public void SaveToStream(Stream stream , int Width, int Height, Rectangle R,ImageFormat ifm) 
		{
			SaveToStream(stream,Width,Height,R,ifm,0,0);
		}

		/// <summary>
		/// Save the formula chart image to a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		public void SaveToStream(Stream stream , int Width, int Height) 
		{
			SaveToStream(stream,Width,Height,new Rectangle(0,0,Width,Height),ImageFormat.Png);
		}

		/// <summary>
		/// Save the formula chart image to a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public void SaveToStream(Stream stream , int Width, int Height, float X,float Y) 
		{
			SaveToStream(stream,Width,Height,new Rectangle(0,0,Width,Height),ImageFormat.Png,X,Y);
		}

		/// <summary>
		/// Save formula chart to asp.net cache.
		/// <code>
		///	protected System.Web.UI.WebControls.ImageButton ibChart;
		/// ibChart.ImageUrl = "../ImageFromCache.aspx?CacheId="+
		///			fc.SaveToImageStream(800,700,ImageFormat.Png,0,0);
		/// </code>
		/// </summary>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <param name="ifm"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <returns></returns>
		public int SaveToImageStream(int Width, int Height,ImageFormat ifm,float X,float Y)
		{
				MemoryStream ms = new MemoryStream();
				SaveToStream(ms,Width,Height,new Rectangle(0,0,Width,Height),ifm,X,Y);
				return SaveToImageStream(ms);
		}

		public int SaveToImageStream(Stream s) 
		{
			if (HttpRuntime.Cache!=null) 
			{
				int RandomId = Rnd.Next(int.MaxValue);
				HttpRuntime.Cache.Add(RandomId.ToString(),
					s,
					null,
					DateTime.MaxValue, 
					new TimeSpan(0,2,0),
					CacheItemPriority.High,
					null);
				return RandomId;
			}
			return -1;
		}

		/// <summary>
		/// Show the chart Image to web page
		/// </summary>
		/// <param name="Width">Image Width</param>
		/// <param name="Height">Image Height</param>
		/// <param name="Rect">The Chart Rectangle</param>
		/// <param name="ifm">Chart Format</param>
		/// <returns></returns>
		public string SaveToWeb(int Width,int Height,Rectangle Rect,ImageFormat ifm)
		{
			Bitmap B = GetBitmap(Width,Height,Rect);
			MemoryStream ms = new MemoryStream();
			B.Save(ms,ifm); 
			return "~/ImageFromCache.aspx?CacheId="+SaveToImageStream(ms);
		}

		public string SaveToWeb(int Width,int Height,ImageFormat ifm)
		{
			return SaveToWeb(Width,Height,new Rectangle(0,0,Width,Height),ifm);
		}

		public string SaveToWeb(int Width,int Height)
		{
			return SaveToWeb(Width,Height,ImageFormat.Png);
		}

		private static void DrawString(Graphics g,string Text,Font F,Brush bText,ref float CurrentX,float Y,int Mode)
		{
			if (Mode==1)
				g.DrawString(Text,F,bText,CurrentX,Y);
			CurrentX +=g.MeasureString(Text,F).Width;
		}

		private static Brush GetBrush(double D1,double D2,Color[] C)
		{
			if (D1==0)
				return new SolidBrush(C[1]);
			else return new SolidBrush(C[D1.CompareTo(D2)+1]);
		}

		/// <summary>
		/// Get or set the selected area of the chart
		/// </summary>
		public FormulaArea SelectedArea 
		{
			get 
			{
				foreach(FormulaArea fa in Areas)
					if (fa.Selected)
						return fa;
				return null;
			}
			set 
			{
				foreach(FormulaArea fa in Areas)
					fa.Selected = fa==value;
			}
		}

		/// <summary>
		/// Set selected data, the data will be drawn a selected sign
		/// </summary>
		/// <param name="HitInfo"></param>
		public void SetSelectedData(FormulaHitInfo HitInfo)
		{
			foreach(FormulaArea fa in Areas)
				if (fa==HitInfo.Area) 
				{
					fa.SelectedData  = null;
					fa.SelectedFormula = null;
					foreach(FormulaData fd in fa.FormulaDataArray)
						if (object.Equals(fd,HitInfo.Data)) 
						{
							fa.SelectedData  = fd;
							fa.SelectedFormula = HitInfo.Formula;
						}
				} 
				else 
				{
					fa.SelectedData = null;
					fa.SelectedFormula = null;
				}
		}

		/// <summary>
		/// Get selected formula
		/// </summary>
		/// <returns></returns>
		public FormulaData GetSelectedData()
		{
			foreach(FormulaArea fa in Areas)
				if (!object.Equals(fa.SelectedData,null))
					return fa.SelectedData;
			return null;
		}

		/// <summary>
		/// Get the Hit info of (X,Y)
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="GetDetail">if true, return the formula data in (X,Y), will be a little slow</param>
		/// <returns>Formula Hit Info</returns>
		public FormulaHitInfo GetHitInfo(float X,float Y)
		{
			bool GetDetail = true;
			FormulaHitInfo HitInfo = new FormulaHitInfo();
			if (DataProvider!=null)
			{
				X -=Rect.X;
				Y -=Rect.Y;
				HitInfo.X = X;
				HitInfo.Y = Y;
				HitInfo.HitType = FormulaHitType.htNoWhere;
				int XI = (int)X;
				int YI = (int)Y;
				int Count = 0;
				foreach(FormulaArea fa in Areas)
					if (fa.Visible)
					{
						if (Count==0)
						{
							Rectangle R = fa.Canvas.Rect;
							int i = (int)(X - R.X);
							HitInfo.CursorPos = fa.Canvas.Stop + (int)(i / ColumnWidth);
							Count++;
						}

						if (Math.Abs(fa.Rect.Bottom-YI)<3) 
						{
							HitInfo.Area = fa;
							HitInfo.HitType = FormulaHitType.htSize;
							break;
						}
						else if (fa.Rect.Contains(XI,YI))
						{
							HitInfo.Area = fa;
							HitInfo.HitType = FormulaHitType.htArea;
							foreach(FormulaAxisY fay in fa.AxisYs)
								if (fay.Visible && fay.FrameRect.Contains(XI,YI))
								{
									HitInfo.HitType = FormulaHitType.htAxisY;
									HitInfo.AxisY = fay;
									return HitInfo;
								}
							foreach(FormulaAxisX fax in fa.AxisXs)
								if (fax.Visible &&  fax.Rect.Contains(XI,YI-1))
								{
									HitInfo.HitType = FormulaHitType.htAxisX;
									HitInfo.AxisX = fax;
									return HitInfo;
								}

							if (GetDetail)
							{
								double MinDistance = double.MaxValue;
								foreach(FormulaData fd in fa.FormulaDataArray)
								{
									//FormulaData fd = fa.GetTransform(fdo);
									double d = fd.GetDistance(X,Y);
									if (MinDistance>d) 
									{
										MinDistance = d;
										HitInfo.Data = fd;
									}
								}

								if (MinDistance>4)
									HitInfo.Data = null;

								if (!object.Equals(HitInfo.Data,null))
								{
									int k1 = 0;
									for(int j=0; j<fa.Formulas.Count; j++)
									{
										int k2 = k1+fa.Packages[j].Count;
										for(int i=k1; i<k2; i++)
											if (i<fa.FormulaDataArray.Count)
												if (object.Equals(fa.FormulaDataArray[i],HitInfo.Data)) 
												{
													HitInfo.Formula = fa.Formulas[j];
													HitInfo.FormulaResult = fa.Packages[j];
												}
										k1 = k2;
									}
								}
							}
						}
					}
			}
			return HitInfo;
		}

		/// <summary>
		/// Get the price area of the chart
		/// </summary>
		public FormulaArea MainArea
		{
			get 
			{
				foreach(FormulaArea fa in Areas)
					if (fa.IsMain())
						return fa;
				if (Areas.Count>0)
					return Areas[0];
				return null;
			}
		}

//		public static int FindIndex(double[] dd, double d)
//		{
//			return FindIndex(dd,d,0);
//		}
//
//		public static int FindIndex(double[] dd, double d,int Dir)
//		{
//			int i = 0;
//			int j = dd.Length-1;
//			while (i<j) 
//			{
//				int k=(i+j-Dir)/2;
//				if (dd[k]<d)
//					i = k+Dir+1;
//				else if (dd[k]>d)
//					j = k+Dir;
//				else return k;
//			}
//			return i;
//		}

		/// <summary>
		/// Convert double date to index
		/// </summary>
		/// <param name="d"></param>
		/// <param name="Dir"></param>
		/// <returns></returns>
		public int DateToIndex(double d,int Dir)
		{
			return FormulaBase.DateToBar(DataProvider,d,Dir,ExpandDate);
		}

		public int DateToIndex(DateTime D,int Dir)
		{
			return DateToIndex(D.ToOADate(),Dir);
		}

		public int DateToIndex(DateTime D)
		{
			return DateToIndex(D.ToOADate(),0);
		}

		public int DateToIndex(double d)
		{
			return DateToIndex(d,0);
		}

		public double IndexToDoubleDate(int i)
		{
			double[] dd = DataProvider["DATE"];
			if (i<0) i = 0;
			if (!ExpandDate)
				i = dd.Length-1;
			if (i<dd.Length)
				return dd[i];
			else 
			{
				if (dd.Length>0)
				{
					double d = dd[dd.Length-1];

					DataCycleBase dcb = DataProvider.DataCycle.CycleBase;
					int j = (i-dd.Length+1);
					try
					{
						switch (dcb)
						{
							case DataCycleBase.WEEK:
								return d+j*7;
							case DataCycleBase.MONTH:
							case DataCycleBase.QUARTER:
							case DataCycleBase.YEAR:
								DateTime d1 = DateTime.FromOADate(d);
								if (dcb==DataCycleBase.MONTH)
									d1 = d1.AddMonths(j);
								else if (dcb==DataCycleBase.QUARTER)
									d1 = d1.AddMonths(j*3);
								else if (dcb==DataCycleBase.YEAR)
									d1 = d1.AddYears(j);
								return d1.ToOADate();
							default :
								int OldWeek = (int)d % 7;
								d += j/5*7+j%5;
								int NowWeek = (int)d % 7;
				
								if (NowWeek<OldWeek)
									d +=2;
								return d;
						}
					} 
					catch
					{
						return DateTime.MaxValue.ToOADate();
					}
				}
				return 0;
			}
		}

		public DateTime IndexToDate(int i)
		{
			try
			{
				return DateTime.FromOADate(IndexToDoubleDate(i));
			}
			catch
			{
				return DateTime.MaxValue;
			}
		}

		public DateTime IndexToDate()
		{
			return IndexToDate(CursorPos);
		}

		public PointF GetPointAt(FormulaArea fa, int DateIndex,double Price)
		{
			if (fa!=null && fa.Canvas!=null && fa.AxisY!=null)
			{
				float x = (float)((DateIndex - fa.Canvas.Stop+0.5)* ColumnWidth);
				float y = fa.AxisY.CalcY(Price);
				foreach(FormulaAxisY fay in fa.AxisYs)
				{
					if (fay.AxisPos==AxisPos.Left)
						x +=fay.Width;
				}
				return new PointF(x,y);
			}
			return PointF.Empty;
		}

		public PointF GetPointAt(string AreaName, int DateIndex,double Price)
		{
			return GetPointAt(this[AreaName],DateIndex,Price);
		}

		/// <summary>
		/// Get point of certain Date and Price
		/// </summary>
		/// <param name="DateIndex"></param>
		/// <param name="Price"></param>
		/// <returns></returns>
		public PointF GetPointAt(int DateIndex,double Price)
		{
			return GetPointAt(MainArea,DateIndex,Price);
		}

		public PointF GetPointAt(DateTime DateIndex,string DataType)
		{
			return GetPointAt(DateIndex,DataType,0);
		}

		public PointF GetPointAt(FormulaArea fa,double d,string DataType,double Price)
		{
			if (DataProvider!=null) 
			{
				double[] dd = DataProvider["DATE"];
				double[] PriceArray = null;
				if (DataType!=null)
					PriceArray = DataProvider[DataType];

				if (dd!=null && dd.Length>0) 
				{
					//int i = FindIndex(dd,d);
					int i = DateToIndex(d);
					if (PriceArray!=null)
					{
						if (i<PriceArray.Length)
							Price = PriceArray[i];
						else Price = PriceArray[PriceArray.Length-1];

					}return GetPointAt(fa,i,Price);
				}
			}
			return PointF.Empty;
		}

		public PointF GetPointAt(string AreaName,double d,string DataType,double Price)
		{
			return GetPointAt(this[AreaName],d,DataType,Price);
		}

		public PointF GetPointAt(double d,string DataType,double Price)
		{
			return GetPointAt(MainArea,d,DataType,Price);
		}

		public PointF GetPointAt(FormulaArea fa,DateTime DateIndex,string DataType,double Price)
		{
			double d = DateIndex.ToOADate();
			return GetPointAt(fa,d,DataType,Price);
		}

		public PointF GetPointAt(DateTime DateIndex,string DataType,double Price)
		{
			return GetPointAt(MainArea,DateIndex,DataType,Price);
		}
        
		public PointF GetPointAt(FormulaArea fa,DateTime DateIndex,double Price)
		{
			return GetPointAt(fa,DateIndex,null,Price);
		}

		/// <summary>
		/// Get point of certain Date and Price
		/// </summary>
		/// <param name="DateIndex"></param>
		/// <param name="Price"></param>
		/// <returns></returns>
		public PointF GetPointAt(DateTime DateIndex,double Price)
		{
			return GetPointAt(MainArea,DateIndex,null,Price);
		}

		public double GetPriceAt(DateTime DateIndex,string DataType)
		{
			double d = DateIndex.ToOADate();
			if (DataProvider!=null) 
			{
				double[] dd = DataProvider["DATE"];
				double[] PriceArray = DataProvider[DataType];

				if (dd!=null && dd.Length>0) 
				{
					//int i = FindIndex(dd,d);
					int i = DateToIndex(d);
					if (PriceArray!=null)
						return PriceArray[i];
				}
			}
			return double.NaN;
		}

		/// <summary>
		/// Set area by pos
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public void SelectAreaByPos(float X,float Y) 
		{
			FormulaHitInfo HitInfo = GetHitInfo(X,Y);
			if (HitInfo.Area!=null)
				SelectedArea = HitInfo.Area;
		}

		/// <summary>
		/// Set cursor pos by X and Y
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public void SetCursorPos(float X,float Y)
		{
			FormulaHitInfo HitInfo = GetHitInfo(X,Y);
			CursorPos = HitInfo.CursorPos;
		}

		/// <summary>
		/// Draw Value Text at current cursor pos
		/// </summary>
		/// <param name="g"></param>
		public void DrawValueText(Graphics g)
		{
			if (DataProvider!=null)
				foreach(FormulaArea fa in Areas)
					if (fa.Visible)
						fa.DrawValueText(g);
		}

		/// <summary>
		/// Draw Value Text at position of (X,Y), and adjust the cursor pos by X and Y
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public void DrawValueText(float X,float Y) 
		{
			SetCursorPos(X,Y);
			DrawValueText(CurrentGraph);
		}

		/// <summary>
		/// Restore chart from memory bitmap
		/// </summary>
		/// <param name="g"></param>
		/// <param name="R"></param>
		public void RestoreMemBmp(Graphics g,RectangleF R)
		{
			if (!R.IsEmpty && BitmapCache) 
			{
				//R.Inflate(50,50);
				RectangleF Dest = R;
				Dest.Offset(Rect.X,Rect.Y);
				if (ExtraMemBmp!=null)
					g.DrawImage(ExtraMemBmp,Dest,R,GraphicsUnit.Pixel);
				else g.DrawImage(MemBmp,Dest,R,GraphicsUnit.Pixel);
			}
		}

		/// <summary>
		/// Hide the cursor of the chart
		/// </summary>
		/// <param name="g"></param>
		public void HideCursor(Graphics g) 
		{
			if (BitmapCache && MemBmp!=null) 
			{
				RestoreMemBmp(g,LastHLine);
				RestoreMemBmp(g,LastVLine);
			}

			foreach(FormulaArea fa in Areas)
			{
				foreach(FormulaAxisY fay in fa.AxisYs) 
				{
					RestoreMemBmp(g,fay.LastCursorRect);
					fay.LastCursorRect = RectangleF.Empty;
				}
				RestoreMemBmp(g,fa.AxisX.LastCursorRect);
				fa.AxisX.LastCursorRect = RectangleF.Empty;
			}
		}

		/// <summary>
		/// Draw cursor to g at X and Y
		/// </summary>
		/// <param name="g"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public void DrawCursor(Graphics g,float X,float Y,bool ChangeCursorPosByXY)
		{
			if (DataProvider==null) return; // || !ShowCursorLabel
			CurrentGraph = g;

			FormulaHitInfo HitInfo = GetHitInfo(X,Y);
			int ExtraWidth = 0;
			if (MemBmp!=null)
			{
				if (ShowHLine) 
					RestoreMemBmp(g,LastHLine);
				if (ShowVLine)
					RestoreMemBmp(g,LastVLine);
			}

			if (ShowHLine && !double.IsNaN(Y)) 
			{
				if (Y!=Math.Floor(Y)) ExtraWidth = 1;
				LastHLine.X = Rect.X;
				if (MainArea!=null)
					LastHLine.X = MainArea.Canvas.ClipRect.X;
				LastHLine.Y = Y-Rect.Y-ExtraWidth;
				LastHLine.Width = Rect.Width;
				LastHLine.Height =CursorPen.Width+ExtraWidth;

				g.DrawLine(CursorPen,LastHLine.X,Y,Rect.Right,Y);
			}

			if (ShowVLine && !double.IsNaN(X))
			{
				g.DrawLine(CursorPen,X,Rect.Y,X,Rect.Bottom);
				if (X!=Math.Floor(X)) ExtraWidth = 1;
				LastVLine.X = X-Rect.X-ExtraWidth;
				LastVLine.Y = 0;
				LastVLine.Width = CursorPen.Width+ExtraWidth;
				LastVLine.Height = Rect.Height;
			}
			//DrawValueText(X,Y);
			if (ChangeCursorPosByXY)
				SetCursorPos(X,Y);
			DrawValueText(g);

			if (ShowCursorLabel)
			{
				foreach(FormulaArea fa in Areas)
					if (fa.Visible /*&& fa!=HitInfo.Area*/) 
					{
						foreach(FormulaAxisY fay in fa.AxisYs) 
							if (fay.LastCursorRect!=RectangleF.Empty && fay.LineBinded)
							{
								RestoreMemBmp(g,fay.LastCursorRect);
								fay.LastCursorRect = RectangleF.Empty;
							}
					}

				if (!double.IsNaN(Y) && HitInfo.Area!=null) 
					foreach(FormulaAxisY fay in HitInfo.Area.AxisYs) 
						if (fay.LineBinded)
							fay.DrawCursor(g,this,HitInfo.Area,Y);

				if (!double.IsNaN(X))
					for(int i=Areas.Count-1; i>=0; i--)
					{
						if (Areas[i].AxisX.Visible)
						{
							Areas[i].AxisX.DrawCursor(g,this,Areas[i],X);
							break;
						}
					}
			}
		}

		public void DrawCursor(Graphics g,float X,float Y)
		{
			DrawCursor(g,X,Y,true);
		}

		/// <summary>
		/// Draw cursor according current cursor pos
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fa"></param>
		/// <param name="fd"></param>
		public void DrawCursor(Graphics g,FormulaArea fa,FormulaData fd) 
		{
			float X = Rect.X+fa.Canvas.GetX(CursorPos);
			float Y = Rect.Y+fa.AxisY.CalcY(fd[CursorPos]);
			DrawCursor(g,X,Y);
		}

		public void GetXYFromPos(out float X,out float Y)
		{
			X = float.NaN;
			Y = float.NaN;
			foreach(FormulaArea fa in Areas)
				if (fa.IsMain()) 
				{
					FormulaData fd = DataProvider["CLOSE"];
					if (CursorPos>=0 && CursorPos<fd.Length)
					{
						if (FixedTime)
						{
							FormulaData fdDate = DataProvider["DATE"];
							X = fa.Canvas.AxisX.GetX(fdDate[CursorPos],fa.Canvas.Left,fa.Canvas.Right);
						} 
						else 
							X = Rect.X+fa.Canvas.GetX(CursorPos);
						Y = Rect.Y+fa.AxisY.CalcY(fd[CursorPos]);
					}
				}
		}

		/// <summary>
		/// Draw cursor at current pos
		/// </summary>
		/// <param name="g"></param>
		public void DrawCursor(Graphics g)
		{
			float X;
			float Y;
			GetXYFromPos(out X,out Y);
			if (!double.IsNaN(X) && !double.IsNaN(Y))
				DrawCursor(g,X,Y,false);
		}

		public ObjectPoint GetValueFromPos(float X,float Y,ref FormulaArea fa)
		{
			ObjectPoint op = new ObjectPoint();
			FormulaHitInfo hi = GetHitInfo(X,Y);
			if (fa==null)
				fa = hi.Area;
			if (fa!=null) 
			{
				op.Y = fa.AxisY.GetValueFromY(Y-Rect.Y)*fa.AxisY.MultiplyFactor;
				int i = hi.CursorPos;
				double[] fdDate = DataProvider["DATE"];
				if (fdDate!=null && fdDate.Length>0) 
				{
					if (i<0)
						op.X = fdDate[0]+i;
					else if (i>=fdDate.Length)
						op.X = IndexToDoubleDate(i);
						//op.X = fdDate[fdDate.Length-1]+(i-fdDate.Length+1);
					else op.X = fdDate[i];
				}
			}
			return op;
		}

		public ObjectPoint GetValueFromPos(float X,float Y)
		{
			FormulaArea fa = null;
			return GetValueFromPos(X,Y,ref fa);
		}

		/// <summary>
		/// Adjust current cursor pos by NewPos and NewStart
		/// </summary>
		/// <param name="g"></param>
		/// <param name="NewPos"></param>
		/// <param name="NewStart"></param>
		public void AdjustCursorByPos(Graphics g,int NewPos,int NewStart,bool NeedDrawCursor) 
		{
			FormulaArea fa = MainArea;
			if (fa!=null) 
			{
				double[] fdDate = DataProvider["DATE"];
				FormulaData fd = DataProvider["CLOSE"];
				if (fd.Length<=0) return;
				Start = NewStart;
				//if (Start<0) Start = 0;
				if (Start>fd.Length-1) Start = fd.Length-1;

				if (NewPos<0) NewPos = 0;
				if (NewPos>fd.Length-1) NewPos = fd.Length-1;
				if (NewPos!=this.CursorPos) 
				{
					if (NewPos<fa.Canvas.Stop) 
						Start += fa.Canvas.Stop - NewPos;
					if (NewPos>fd.Length-1-Start)
						Start = fd.Length-1-NewPos;
					this.CursorPos = NewPos;
				}
				if (NeedDrawCursor)
					DrawCursor(g,fa,fd);
			}
		}

		public void AdjustCursorByPos(Graphics g,int NewPos,int NewStart) 
		{
			AdjustCursorByPos(g,NewPos,NewStart,false);
		}

		/// <summary>
		/// Draw lastest data of given data provider
		/// </summary>
		/// <param name="g"></param>
		/// <param name="idp"></param>
		/// <param name="F"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="Colors">Color[0]:Down color , Color[1]:Equal color, Color[2]: Up color , Color[3]:Other color </param>
		/// <param name="Fields">POHLCVG P:Prev Close,O:Open,H:High,L:Low,C:CLose,V:Volume,G:Change</param>
		public static void DrawLastestData(Graphics g,IDataProvider idp,Font F,float X,float Y,float Width,bool AlignRight,Color[] Colors,string Format,string Fields) 
		{
			int Count = idp.Count;
			double Last = 0;
			if (Count>1)
				Last = idp["CLOSE"][Count-2];
			double O = idp["OPEN"][Count-1];
			double H = idp["HIGH"][Count-1];
			double L = idp["LOW"][Count-1];
			double C = idp["CLOSE"][Count-1];
			double V = idp["VOLUME"][Count-1];
			
			for(int i=AlignRight?0:1; i<2; i++) 
			{
				float CurrentX = X;
				Brush bText = new SolidBrush(Colors[3]);
				Font nf = new Font(F,FontStyle.Bold);
				if (Last!=0 && Fields.IndexOf("P")>=0) 
				{
					DrawString(g,"Prev Close:",nf,bText,ref CurrentX,Y,i);
					DrawString(g,Last.ToString(Format,NumberFormatInfo.InvariantInfo),F,GetBrush(Last,Last,Colors),ref CurrentX,Y,i);
				}

				if (Fields.IndexOf("O")>=0)
				{
					DrawString(g,"O:",nf,bText,ref CurrentX,Y,i);
					DrawString(g,O.ToString(Format,NumberFormatInfo.InvariantInfo),F,GetBrush(Last,O,Colors),ref CurrentX,Y,i);
				}

				if (Fields.IndexOf("H")>=0)
				{
					DrawString(g,"H:",nf,bText,ref CurrentX,Y,i);
					DrawString(g,H.ToString(Format,NumberFormatInfo.InvariantInfo),F,GetBrush(Last,H,Colors),ref CurrentX,Y,i);
				}

				if (Fields.IndexOf("L")>=0)
				{
					DrawString(g,"L:",nf,bText,ref CurrentX,Y,i);
					DrawString(g,L.ToString(Format,NumberFormatInfo.InvariantInfo),F,GetBrush(Last,L,Colors),ref CurrentX,Y,i);
				}

				if (Fields.IndexOf("C")>=0)
				{
					DrawString(g,"C:",nf,bText,ref CurrentX,Y,i);
					DrawString(g,C.ToString(Format,NumberFormatInfo.InvariantInfo),F,GetBrush(Last,C,Colors),ref CurrentX,Y,i);
				}

				if (Fields.IndexOf("V")>=0)
				{
					DrawString(g,"V:",nf,bText,ref CurrentX,Y,i);
					DrawString(g,V.ToString(NumberFormatInfo.InvariantInfo),F,GetBrush(Last,C,Colors),ref CurrentX,Y,i);
				}

				if (Last!=0 && Fields.IndexOf("P")>=0) 
				{
					DrawString(g,"Chg:",nf,bText,ref CurrentX,Y,i);
					double Change = C-Last;
					DrawString(g,Change.ToString("+0.##;-0.##;0",NumberFormatInfo.InvariantInfo)+
						"("+(Change/Last).ToString("p2",NumberFormatInfo.InvariantInfo)+")",F,GetBrush(Last,C,Colors),ref CurrentX,Y,i);
				}
				X = Width-CurrentX;
			}
		}

		private string GetTextData(FormulaPackage FormulaDataArray,string Separator,bool ShowHeader)
		{
			StringBuilder sb = new StringBuilder();
			if (DataProvider!=null)
			{
				FormulaData fdDate = DataProvider["DATE"]; 
				fdDate.Name = "Date";
				ArrayList al = new ArrayList();
				al.Add(fdDate);
				al.AddRange(FormulaDataArray);
				if (al.Count>0) 
				{
					for(int i=0-(ShowHeader?1:0); i<fdDate.Length; i++) 
					{
						for(int j=0; j<al.Count; j++)
						{
							FormulaData fd = (FormulaData)al[j];
							if (ShowHeader && i<0) 
								sb.Append(fd.Name);
							if (i>=0) 
							{
								if (j==0)
									sb.Append(DateTime.FromOADate(fd[i]).ToString("dd-MMM-yy",DateTimeFormatInfo.InvariantInfo));
								else sb.Append(fd[i].ToString("f2",NumberFormatInfo.InvariantInfo));
							}
							if (j<FormulaDataArray.Count-1)
								sb.Append(Separator);
						}
						sb.Append("\r\n");
					}
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Get the formula data of selected area
		/// </summary>
		/// <param name="fa"></param>
		/// <param name="Separator"></param>
		/// <param name="ShowHeader"></param>
		/// <returns></returns>
		public string GetAreaTextData(FormulaArea fa,string Separator,bool ShowHeader) 
		{
			return GetTextData(fa.FormulaDataArray,Separator,ShowHeader);
		}

		/// <summary>
		/// Get the formula data of selected area
		/// </summary>
		/// <param name="Separator"></param>
		/// <param name="ShowHeader"></param>
		/// <returns></returns>
		public string GetAreaTextData(string Separator,bool ShowHeader) 
		{
			if (Areas.Count>0) 
			{
				FormulaArea fa = SelectedArea;
				if (fa==null)
					fa = Areas[Areas.Count-1];
				return GetAreaTextData(fa,Separator,ShowHeader);
			}
			return "";
		}

		/// <summary>
		/// Get the formula data
		/// </summary>
		/// <param name="Separator"></param>
		/// <param name="ShowHeader"></param>
		/// <returns></returns>
		public string GetChartTextData(string Separator,bool ShowHeader)
		{
			FormulaPackage fp = new FormulaPackage();
			foreach(FormulaArea fa in Areas)
				fp.AddRange(fa.FormulaDataArray);
			return GetTextData(fp,Separator,ShowHeader);
		}

		public void AddAreas(string Indicators)
		{
			if (Indicators!=null && Indicators!="")
			{
				string[] ss = Indicators.Split(';');
				foreach(string s in ss) 
				{
					int i = s.IndexOf('(');
					int j = s.IndexOf(')');
					string r = s;
					int Percent = 1;
					if (j>i)
					{
						r = s.Substring(0,i);
						try
						{
							Percent = int.Parse(s.Substring(i+1,j-i-1));
						} 
						catch
						{
						}
					}
					AddArea(r,Percent);
				}
			}
		}

		public void AddOverlays(string Overlays)
		{
			if (Overlays!=null && Overlays!="" && Areas.Count>0)
			{
				string[] ss = Overlays.Split(';');
				foreach(string s in ss)
					this[0].AddFormula(s);
			}
		}

		/// <summary>
		/// Create chart by indicators,overlays, data provider, and skin
		/// </summary>
		/// <param name="Indicators"></param>
		/// <param name="Overlays"></param>
		/// <param name="DataProvider"></param>
		/// <param name="Skin"></param>
		/// <returns></returns>
		static public FormulaChart CreateChart(string Indicators,string Overlays,IDataProvider DataProvider,string Skin)
		{
			FormulaChart Chart = new FormulaChart();
			Chart.AddAreas(Indicators);
			Chart.AddOverlays(Overlays);
			if (DataProvider!=null)
				Chart.DataProvider = DataProvider;
			if (Skin!=null && Skin!="")
				Chart.SetSkin(Skin);
			return Chart;
		}

		static public FormulaChart CreateChart(IDataProvider idp,string Skin)
		{
			return CreateChart("Main(3);VOLMA;SlowSTO;MACD","MA(14);MA(28)",idp,Skin);
		}

		static public FormulaChart CreateChart(IDataProvider idp)
		{
			return CreateChart(idp,"RedWhite");
		}

		/// <summary>
		/// Get area height percent string
		/// </summary>
		/// <returns></returns>
		public string GetAreaPercent()
		{
			StringBuilder sb = new StringBuilder();
			foreach(FormulaArea fa in Areas)
			{
				if (sb.Length!=0)  sb.Append(";");
				sb.Append(fa.HeightPercent.ToString());
			}
			return sb.ToString();
		}

		/// <summary>
		/// Convert area formula to string array
		/// </summary>
		/// <returns></returns>
		public string[] AreaToStrings()
		{
			return AreaToStrings(0,Areas.Count);
		}

		/// <summary>
		/// Convert area formula to string array
		/// </summary>
		/// <param name="Start"></param>
		/// <param name="Count"></param>
		/// <returns></returns>
		public string[] AreaToStrings(int Start,int Count)
		{
			string[] ss = new string[Count];
			for(int i=0; i<ss.Length; i++)
				ss[i] = string.Join("#",Areas[Start+i].FormulaToStrings());
			return ss;
		}

		/// <summary>
		/// Convert formula name string array to areas
		/// </summary>
		/// <param name="ss">Formula name string array</param>
		public void StringsToArea(string[] ss)
		{
			for(int i=0; i<ss.Length; i++)
				AddArea(ss[i]);
		}

		private double ToDoubleDef(string s,double Def)
		{
			try
			{
				return double.Parse(s);
			}
			catch
			{
				return Def;
			}
		}

		/// <summary>
		/// Set area percent
		/// </summary>
		/// <param name="areaPercent">Percent separated by semi-colon</param>
		public void SetAreaPercent(string areaPercent)
		{
			if (areaPercent!=null)
			{
				string[] ss = areaPercent.Split(';');
				double[] dd = new double[Areas.Count];
				for(int i=0; i<dd.Length; i++)
					dd[i] = 1;
				double Sum = 0;
				for(int i=0; i<dd.Length; i++)
				{
					if (i<ss.Length)
						dd[i] = Math.Abs(ToDoubleDef(ss[i],1));
					Sum +=dd[i];
				}

				// Verify if there are areas too small, if have, reset all areas
				bool b = true;
				for(int i=0; i<dd.Length; i++)
					if (dd[i]/Sum<0.05)
						b = false;
				if (!b) 
				{
					for(int i=0; i<dd.Length; i++)
						dd[i] = 1;
					if (dd.Length>0)
						dd[0] = 3;
				}
				for(int i=0; i<dd.Length; i++)
					Areas[i].HeightPercent = dd[i];
			}
		}

		/// <summary>
		/// Show price as 32nd format
		/// </summary>
		/// <param name="IsBond">If use bond label style</param>
		/// <param name="Format">Y4 means 32nd format, show 4 digitals</param>
		public void SetBondFormat(bool IsBond, string Format)
		{
			if (MainArea!=null && MainArea.IsMain())
			{
				MainArea.AxisY.Format = Format;
				MainArea.AxisY.IsBond = IsBond;
			}
		}
	}
}