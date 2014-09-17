using System;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Easychart.Finance
{
	/// <summary>
	/// Define how the formula data will be transform before shown
	/// </summary>
	public enum Transform{Normal,FirstDataOfView,PercentView,FullView}

	/// <summary>
	/// Formula data array
	/// </summary>
	public class FormulaData : ICloneable
	{
		Hashtable htDataArray;
		Hashtable htData;
		Hashtable htPoints;

		public FormulaCanvas Canvas;
		public int AxisYIndex = 0;
		public FormulaAxisY AxisY;
		public FormulaAxisX AxisX;
		public string Name;
		public FormulaRenderType RenderType;
		public FormulaType FormulaType = FormulaType.Array;
		public FormulaAlign Align;
		public FormulaDot Dot;
		public Color FormulaUpColor = Color.Empty;
		public Color FormulaDownColor = Color.Empty;

		public DashStyle DashStyle = DashStyle.Solid;
		public int LineWidth = -1;
		public Transform Transform;
		public double PercentView;
		public string Format = "f2";
		public SmoothingMode Smoothing = SmoothingMode.HighQuality;
		//public bool TextInvisible;
		public ValueTextMode ValueTextMode = ValueTextMode.Both;
		public FormulaBase ParentFormula;
		
		public Pen LinePen;
		public Brush AreaBrush;
		public Font TextFont;
		public Font NameFont;
		public int LabelIndex = 0;
		public int AxisMargin;
		public VerticalAlign VAlign = VerticalAlign.Top;
		public double[] Data;
		public bool LastValueInAxis;
		public bool SameColor;
		public double HighPercent = 1;
		public double LowPercent;
		public byte Alpha = 255;  
		public bool Horizontal;

		/// <summary>
		/// Hashtable store the sub data array. e.g O,H,L value of the stock bar.
		/// SubData["O"] return the Open data array
		/// SubData["H"] return the High data array
		/// SubData["L"] return the Low data array
		/// </summary>
		public Hashtable SubData
		{
			get
			{
				return htDataArray;
			}
		}

		private string CurrentPointName;

		/// <summary>
		/// Get or set the double array attached to this data
		/// </summary>
		public double[] this[string Name]
		{
			get 
			{
				if (htDataArray==null) 
					htDataArray = new Hashtable();
				return (double[])htDataArray[Name];
			}
			set 
			{
				if (htDataArray==null)
					htDataArray = new Hashtable();
				htDataArray[Name] = value;
			}
		}

		/// <summary>
		/// Get or set each value of this data array
		/// </summary>
		public double this[int N]
		{
			get 
			{
				return Data[N];
			}
			set 
			{
				Data[N] = value;
			}
		}

		/// <summary>
		/// Get each value of this data array
		/// </summary>
		public double this[double N] 
		{
			get 
			{
				return this[(int)N];
			}
		}

		/// <summary>
		/// Get the extra data count
		/// </summary>
		public int ExtraDataCount
		{
			get
			{
				if (htDataArray==null)
					return 0;
				else return htDataArray.Count;
			}
		}

		public Hashtable OwnerData
		{
			get 
			{
				if (htData==null)
					htData = new Hashtable();
				return htData;
			}
		}

		public double LASTDATA 
		{
			get 
			{
				if (Length>0)
					return this[Length-1];
				else return double.NaN;
			}
		}

		public double LASTVALUE
		{
			get
			{
				for(int i=Length-1; i>=0; i--)
					if (!double.IsNaN(Data[i]))
						return Data[i];
				return double.NaN;
			}
		}

		public double LAST
		{
			get
			{
				return LASTDATA;
			}
		}

		public double FIRST
		{
			get
			{
				if (Length>0)
					return this[0];
				else return double.NaN;
			}
		}

		public double Avg
		{
			get 
			{
				double d = 0;
				for (int i=0; i<Length; i++)
					d +=Data[i];
				return d/Length;
			}
		}

		public double ConstValue
		{
			get 
			{
				if (Data.Length>0)
					return Data[0];
				else return 0;
			}
		}

		#region Constructor 
		public FormulaData()
		{
		}

		public FormulaData(int N):this()
		{
			this.Data = new double[N];
		}

		public FormulaData(double[] Data):this()
		{
			this.Data = (double[])Data.Clone();
		}

		public FormulaData(FormulaData f):this(f.Length)
		{
			f.Data.CopyTo(this.Data,0);
		}

		#endregion

		#region Attribute
		private void SetColor(string s,bool IsDownColor)
		{
			try 
			{
				if (IsDownColor)
					FormulaDownColor = ColorTranslator.FromHtml(s);
				else FormulaUpColor = ColorTranslator.FromHtml(s);
			}
			catch 
			{
			}
		}

		private void SetFont(string s)
		{
			try
			{
				s = s.Trim(')','(',' ');
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				TextFont = (Font)tc.ConvertFromString(null,FormulaHelper.enUS,s);
			} 
			catch (Exception e)
			{
				s = e.Message;
			}
		}

		private void SetWidth(string s) 
		{
			try 
			{
				int i=int.Parse(s);
				if (i>=0 && i<8)
					LineWidth = i;
			} 
			catch 
			{
			}
		}

		private void SetAxisMargin(string s)
		{
			try 
			{
				int i=int.Parse(s);
				AxisMargin = i;
			} 
			catch 
			{
			}
		}

		private void SetBrush(string s) 
		{
			try 
			{
				AreaBrush = new SolidBrush(ColorTranslator.FromHtml(s));
			}
			catch 
			{
			}
		}

		private void SetLabel(string s) 
		{
			try 
			{
				LabelIndex = int.Parse(s);
			}
			catch 
			{
			}
		}

		private void SetVAlign(string s) 
		{
			try 
			{
				VAlign=(VerticalAlign)int.Parse(s);
			}
			catch 
			{
			}
		}

		private void SetStyle(string s)
		{
			try
			{
				DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),s,true);
			}
			catch
			{
			}
		}

		public void SetAlign(string s) 
		{
			try 
			{
				Align = (FormulaAlign) int.Parse(s);
			} 
			catch 
			{
			}
		}

		public void SetPercent(string s,bool High)
		{
			double d;
			try
			{
				d = double.Parse(s);
				if (d<0) d = 0;
				if (d>1) d = 1;
				if (High)
					HighPercent = d;
				else LowPercent = d;
			}
			catch
			{
			}
		}

		public void SetAlpha(string s)
		{
			try
			{
				Alpha = byte.Parse(s);
			}
			catch
			{
			}
		}

		public void SetAttr(string s) 
		{
			try 
			{
				RenderType = (FormulaRenderType)Enum.Parse(typeof(FormulaRenderType),s,true);
				return;
			}
			catch 
			{
			}

			try 
			{
				Dot = (FormulaDot)Enum.Parse(typeof(FormulaDot),s,true);
				return;
			}
			catch 
			{
			}

			try 
			{
				Align = (FormulaAlign)Enum.Parse(typeof(FormulaAlign),s,true);
				return;
			}
			catch 
			{
			}

			try 
			{
				Smoothing = (SmoothingMode)Enum.Parse(typeof(SmoothingMode),s,true);
				return;
			} 
			catch 
			{
			}

			try 
			{
				VAlign = (VerticalAlign)Enum.Parse(typeof(VerticalAlign),s,true);
				return;
			} 
			catch 
			{
			}

			try 
			{
				Transform = (Transform)Enum.Parse(typeof(Transform),s,true);
			}
			catch 
			{
			}

			string su  = s.ToUpper();
			if (su=="SAMECOLOR")
				SameColor = true;
			else if (su.StartsWith("COLOR"))
				SetColor(s.Substring(5),false);
			else if (su.StartsWith("UPCOLOR"))
				SetColor(s.Substring(7),false);
			else if (su.StartsWith("DOWNCOLOR"))
				SetColor(s.Substring(9),true);
			else if (su.StartsWith("WIDTH"))
				SetWidth(s.Substring(5));
			else if (su.StartsWith("LINETHICK"))
				SetWidth(s.Substring(9));
			else if (su.StartsWith("ALIGN"))
				SetAlign(s.Substring(5));
			else if (su.StartsWith("BRUSH"))
				SetBrush(s.Substring(5));
			else if (su.StartsWith("LABEL"))
				SetLabel(s.Substring(5));
			else if (su.StartsWith("VALIGN"))
				SetVAlign(s.Substring(6));
			else if (su.StartsWith("AXISMARGIN"))
				SetAxisMargin(s.Substring(10));
			else if (su.StartsWith("STYLE"))
				SetStyle(s.Substring(5));
			else if (su.StartsWith("FONT"))
				SetFont(s.Substring(4));
			else if (su.StartsWith("HIGH"))
				SetPercent(s.Substring(4),true);
			else if (su.StartsWith("LOW"))
				SetPercent(s.Substring(3),false);
			else if (su.StartsWith("ALPHA"))
				SetAlpha(s.Substring(5));
			else if (su.StartsWith("YLABEL"))
				LastValueInAxis = true;
			else if (su=="VALUETEXT.NONE" || su=="NOVALUELABEL")
				//TextInvisible = true;
				ValueTextMode = ValueTextMode.None;
			else if (su=="VALUETEXT.TEXTONLY")
				ValueTextMode = ValueTextMode.TextOnly;
			else if (su=="VALUETEXT.VALUEONLY")
				ValueTextMode = ValueTextMode.ValueOnly;
			else if (su=="HORIZONTAL")
				Horizontal = true;
		}

		public void SetAttrs(string s) 
		{
			ArrayList al = new ArrayList();
			int k = 0;
			int j = 0;
			for(int i=0; i<s.Length; i++)
			{
				if (s[i]=='(')
					k++;
				else if (s[i]==')')
					k--;
				else if (s[i]==',' && k==0) 
				{
					al.Add(s.Substring(j,i-j));
					j = i+1;
				}
			}
			if (s.Length>j)
				al.Add(s.Substring(j,s.Length-j));

			string[] ss = (string[])al.ToArray(typeof(string));
			foreach(string r in ss)
				SetAttr(r);
		}

		#endregion

		#region helper functions
		/// <summary>
		/// Set formula data to a constant value
		/// </summary>
		/// <param name="d">value</param>
		public void Set(double d) 
		{
			for(int i=0; i<Data.Length; i++)
				Data[i] = d;
		}

		public void FillTo(ref double[] dd,int N) 
		{
			if (dd.Length>=N) return;
			double[] Old = dd;
			double d = double.NaN; //0
			if (FormulaType == FormulaType.Const)
				if (Old.Length>0)
					d = Old[0];

			dd = new double[N];
			for(int i=0; i<Old.Length; i++)
				dd[N-Old.Length+i] = Old[i];
			for(int i=0; i<N-Old.Length; i++) 
				dd[i] = d;
		}

		public void FillTo(int N) 
		{
			FillTo(ref Data,N);
			if (htDataArray!=null)
			{
				string[] ss = new string[htDataArray.Count];
				int i=0;
				foreach(string s in htDataArray.Keys)
					ss[i++] = s;

				foreach(string s in ss)
				{
					double[] dd =(double[])htDataArray[s];
					FillTo(ref dd,N);
					htDataArray[s] = dd;
				}
			}
		}

		public int Length 
		{
			get 
			{
				return Data.Length;
			}
		}

		public bool IsNaN () 
		{
			for(int i=0; i<Length; i++)
				if (!double.IsNaN(Data[i]) && !double.IsInfinity(Data[i]))
					return false;
			return true;
		}

		public double MaxValue(double[] dd,int Start,int Count,double CurrentMax)
		{
			double d = CurrentMax;
			for(int i=dd.Length-Math.Max(0,Start)-1; i>=Math.Max(0,dd.Length-Start-Count); i--) 
				if (!double.IsNaN(dd[i]) && !double.IsInfinity(dd[i]))
					d = Math.Max(d,dd[i]);
			return d;
		}
		
		public double MaxValue(int Start,int Count)
		{
			double d = MaxValue(Data,Start,Count,double.MinValue);
			if (htDataArray!=null)
			{
				foreach(string s in htDataArray.Keys)
					if (!s.StartsWith("COND") && !s.StartsWith("NUMBER"))
						d = MaxValue((double[])htDataArray[s],Start,Count,d);
			}
			return d;
		}
		
		public double MaxValue()
		{
			return MaxValue(0,Length);
		}

		public double MinValue(double[] dd,int Start,int Count,double CurrentMin)
		{
			double d = CurrentMin;
			for(int i=Data.Length-Math.Max(0,Start)-1; i>=Math.Max(0,Data.Length-Start-Count); i--) 
				if (!double.IsNaN(dd[i]))
					d = Math.Min(d,dd[i]);
			return d;
		}

		public double MinValue(int Start,int Count)
		{
			if (RenderType == FormulaRenderType.VOLSTICK)
				return 0;
			else 
			{
				double d = MinValue(Data,Start,Count,double.MaxValue);
				if (htDataArray!=null)
				{
					foreach(string s in htDataArray.Keys)
						if (!s.StartsWith("COND") && !s.StartsWith("NUMBER"))
							d = MinValue((double[])htDataArray[s],Start,Count,d);

				}
				return d;
			}
		}

		public double MinValue()
		{
			return MinValue(0,Length);
		}

		/*
		private int GetInViewStop(double[] dd)
		{
			int Stop = Math.Max(0,dd.Length-Canvas.Start-Canvas.Count);
			if (Canvas.DATE!=null) 
				Stop = 0;
			return Stop;
		}
		*/

		public double[] GetInViewData(double[] dd) 
		{
			//int Stop = GetInViewStop(dd);
			int Start = dd.Length-Canvas.Start-1;
			ArrayList al = new ArrayList();
			for(int i=dd.Length-Canvas.Start-1; i>=Canvas.Stop; i--)
			{
				if (!double.IsNaN(dd[i]))
					al.Add(dd[i]);
			}
			return (double[])al.ToArray(typeof(double));
		}

		public double[] GetInViewData() 
		{
			return GetInViewData(Data);
		}

		public double[] GetInViewData(string Name) 
		{
			return GetInViewData((double[])htDataArray[Name]);
		}

		public PointF[] GetPoints(double[] dd,bool AddEmptyPoint,double[] Filter)
		{
			return GetPoints(dd,AddEmptyPoint,Filter,false);
		}

		public PointF[] GetPoints(double[] dd,bool AddEmptyPoint,double[] Filter,bool ValueOnly)
		{
			if (dd==null) return null;
			ArrayList al = new ArrayList();

			double MinY = AxisY.MinY;
			double MaxY = AxisY.MaxY;
			Rectangle R = Canvas.Rect;

			if (AxisY.Transform!=null) 
			{
				MinY = AxisY.Transform(MinY);
				MaxY = AxisY.Transform(MaxY);
			}

//			if (AxisY.Scale==ScaleType.Log) 
//			{
//				if (MinY>0)
//					MinY = Math.Log(MinY);
//				if (MaxY>0)
//					MaxY = Math.Log(MaxY);
//			}
			
			int x1 = 0;
			int x2 = 0;
			if (Canvas.DATE!=null) 
			{
				x1 = Canvas.Left; 
				x2 = Canvas.Right;
			}
			//int Stop = GetInViewStop(dd);
			int Stop = Canvas.Stop;
			Canvas.AxisX.Prepare();
			for(int i=dd.Length-Math.Max(0,Canvas.Start)-1; i>=Stop; i--)
			{
				if (!double.IsNaN(dd[i]) && 
					(Filter==null || 
					(Filter[i]!=0) && !double.IsNaN(Filter[i])
					)
					)
				{
					float X;
					
					if (Canvas.DATE!=null)
						X = Canvas.AxisX.GetX(Canvas.DATE[i],x1,x2);
					else X = Canvas.GetX(i);

					double d = dd[i];
					if (ValueOnly)
						al.Add(new PointF(X,i));
					else 
					{
						if (MinY!=MaxY)
						{
							if (AxisY.Transform!=null) 
								d = AxisY.Transform(d);
							
//							if (AxisY.Scale==ScaleType.Log)
//								if (d>0)
//									d = Math.Log(d);
							al.Add(new PointF(X,AxisY.CalcY(d,MaxY,MinY)));
						}
						else
						{
							al.Add(new PointF(X,0));
						}
					}
				}
				else
				{ 
					if (AddEmptyPoint)
						al.Add(PointF.Empty);
				}
			}
			PointF[] Points = (PointF[])al.ToArray(typeof(PointF));
			if (htPoints==null)
				htPoints = new Hashtable();
			htPoints[CurrentPointName] = Points;
			return Points;
		}

		private double[] GetSubData(string Name)
		{
			CurrentPointName = Name;
			return (double[])htDataArray[Name];
		}

		private double[] MainData
		{
			get
			{
				CurrentPointName = "MainData@";
				return Data;
			}
		}

		public PointF[] GetPoints(string Name,bool AddEmptyPoint,string Filter) 
		{
			return GetPoints(GetSubData(Name),AddEmptyPoint,(double[])htDataArray[Filter]);
		}

		public PointF[] GetPoints(double[] dd,bool AddEmptyPoint)
		{
			return GetPoints(dd,AddEmptyPoint,null);
		}

		public PointF[] GetPoints(bool AddEmptyPoint,double[] Filter)
		{
			return GetPoints(MainData,AddEmptyPoint,Filter);
		}

		public PointF[] GetPoints(bool AddEmptyPoint)
		{
			return GetPoints(MainData,AddEmptyPoint);
		}

		public PointF[] GetPoints(string Name,bool AddEmptyPoint) 
		{
			return GetPoints(GetSubData(Name),AddEmptyPoint);
		}

		public PointF[] GetPoints(string Name) 
		{
			if (htDataArray==null)
				return null;
			return GetPoints(GetSubData(Name),false);
		}

		public PointF[] GetPoints()
		{
			return GetPoints(MainData,false);
		}

		public void CopyPoint(FormulaData fd)
		{
			this.htPoints = fd.htPoints;
		}

		private int GetMinDistIndex(float X,PointF[] Points)
		{
			int i1 = 0; 
			int i2 = Points.Length-1;
			while (i1<i2)
			{
				int k = (i1+i2)/2;
				if (X<Points[k].X)
					i1 = k+1;
				else if (X>Points[k].X)
					i2 = k;
				else return k;
			}
			return i1;
		}

		public double GetDistance(float X,float Y)
		{
			double M = double.MaxValue;
			if (htPoints!=null)
			foreach(PointF[] Points in htPoints.Values)
			{
				if (Points!=null) 
				{
					int i = GetMinDistIndex(X,Points);
					if (i>=0 && i<Points.Length)
						M = Math.Min(M,Math.Abs(Y-Points[i].Y));
				}
			}
			return M;
		}

		public static void MakeSameLength(FormulaData f1,FormulaData f2) 
		{
			if (f1.Length==f2.Length)
				return;
			else 
			{
				if (f1.Length<f2.Length)
					f1.FillTo(f2.Length);
				else f2.FillTo(f1.Length);
			}
		}

		public static void MakeSameLength(params FormulaData[] fs) 
		{
			if (fs==null || fs.Length==0) return;
			for(int i=1; i<fs.Length; i++)
				for (int j=0; j<i; j++) 
					MakeSameLength(fs[i],fs[j]);
		}

		#endregion

		#region operator + - * /

		/// <summary>
		/// Operator + of FormulaData
		/// </summary>
		/// <param name="f1">The Formula data</param>
		/// <returns>A new instance of Formula f1</returns>
		public static FormulaData operator + (FormulaData f1) 
		{
			return new FormulaData(f1);
		}

		/// <summary>
		/// Operator + of two Formula Data
		/// </summary>
		/// <param name="f1">Formula Data 1</param>
		/// <param name="f2">Formula Data 2</param>
		/// <returns></returns>
		public static FormulaData operator + (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i]+f2.Data[i];
			if (f1.FormulaType==FormulaType.Const && f2.FormulaType==FormulaType.Const)
				f.FormulaType = FormulaType.Const;
			return f;
		}

		public static FormulaData operator - (FormulaData f1) 
		{
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = -f1.Data[i];
			f.FormulaType = f1.FormulaType;
			return f;
		}

		public static FormulaData operator - (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i]-f2.Data[i];
			if (f1.FormulaType==FormulaType.Const && f2.FormulaType==FormulaType.Const)
				f.FormulaType = FormulaType.Const;
			return f;
		}

		public static FormulaData operator * (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i]*f2.Data[i];
			if (f1.FormulaType==FormulaType.Const && f2.FormulaType==FormulaType.Const)
				f.FormulaType = FormulaType.Const;
			return f;
		}

		public static FormulaData operator / (FormulaData f1,FormulaData f2) 
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i]/f2.Data[i];
			if (f1.FormulaType==FormulaType.Const && f2.FormulaType==FormulaType.Const)
				f.FormulaType = FormulaType.Const;
			return f;
		}

		#endregion

		#region operator > < >= <= == != Equals
		public static FormulaData operator > (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);

			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i]>f2.Data[i]?1:0;
			return f;
		}

		public static FormulaData operator < (FormulaData f1,FormulaData f2)
		{
			return f2>f1;
		}

		public static FormulaData operator >= (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i]>=f2.Data[i]?1:0;
			return f;
		}

		public static FormulaData operator <= (FormulaData f1,FormulaData f2)
		{
			return f2>=f1;
		}

		public static FormulaData operator == (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				nf.Data[i] = f1.Data[i]==f2.Data[i]?1:0;

			return nf;
		}

		public static FormulaData operator != (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i]!=f2.Data[i]?1:0;
			return f;
		}

		public override bool Equals(object obj)
		{
			if (obj is FormulaData)
			{
				FormulaData f = (FormulaData)obj;
				for(int i=0; i<f.Length; i++)
					if (f.Data[i] != Data[i])
						return false;
				return true;
			}
			else return base.Equals (obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override string ToString()
		{
			return ToString("f2");
		}

		public string ToString(string format)
		{
			StringBuilder sb = new StringBuilder();
			for(int i=0; i<Length; i++) 
			{
				if (i!=0)
					sb.Append(',');
				sb.Append(Data[i].ToString(format));
			}
			return sb.ToString();
		}

		#endregion 

		#region operator & | ^ % ! ~ ++ --
		public static FormulaData operator & (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = (int)f1.Data[i] & (int)f2.Data[i];
			return f;
		}

		public static FormulaData operator | (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = (int)f1.Data[i] | (int)f2.Data[i];
			return f;
		}

		public static FormulaData operator ^ (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = (int)f1.Data[i] ^ (int)f2.Data[i];
			return f;
		}

		public static FormulaData operator % (FormulaData f1,FormulaData f2)
		{
			MakeSameLength(f1,f2);
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1,f2);
			FormulaData f = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				f.Data[i] = f1.Data[i] % f2.Data[i];
			return f;
		}

		public static FormulaData operator ! (FormulaData f1)
		{
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
			{
				if (f1.Data[i]==0)
					nf.Data[i] = 1;
				else nf.Data[i] = 0;
			}
			return nf;
		}

		public static FormulaData operator ~ (FormulaData f1)
		{
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				nf.Data[i] = ~(int)f1.Data[i];
			return nf;
		}

		public static FormulaData operator ++ (FormulaData f1)
		{
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				nf.Data[i] = f1.Data[i]+1;
			return nf;
		}

		public static FormulaData operator -- (FormulaData f1)
		{
			if (FormulaBase.Testing) return FormulaBase.TestData(0,f1);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				nf.Data[i] = f1.Data[i]-1;
			return nf;
		}


		#endregion

		#region implicit convert
		public static implicit operator FormulaData (double N) 
		{
			FormulaData f = new FormulaData(1);
			f.Data[0] = N;
			f.FormulaType = FormulaType.Const;
			return f;
		}

		public static implicit operator FormulaData (bool b) 
		{
			return b?1:0;
		}

		public static implicit operator FormulaData (double[] dd) 
		{
			return new FormulaData(dd);
		}

		public static implicit operator FormulaData (string s) 
		{
			double d = 0;
			try 
			{
				d = double.Parse(s);
			} 
			catch 
			{
			}
			return d;
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			FormulaData nd = new FormulaData(this);
			nd.Canvas = Canvas;
			nd.AxisYIndex = AxisYIndex;
			nd.AxisY = AxisY;
			nd.AxisX = AxisX;
			nd.Name = Name;
			nd.RenderType = RenderType;
			nd.FormulaType = FormulaType;
			nd.Align = Align;
			nd.Dot = Dot;
			nd.FormulaUpColor = FormulaUpColor;
			nd.FormulaDownColor = FormulaDownColor;

			nd.DashStyle = DashStyle;
			nd.LineWidth = LineWidth;
			nd.Transform = Transform;
			nd.PercentView = PercentView;
			nd.Format = Format;
			nd.Smoothing = Smoothing;
			//nd.TextInvisible = TextInvisible;
			nd.ValueTextMode = ValueTextMode;
			nd.ParentFormula =ParentFormula;
		
			nd.LinePen = LinePen;
			nd.AreaBrush = AreaBrush;
			nd.TextFont = TextFont;
			nd.NameFont = NameFont;
			nd.LabelIndex = LabelIndex;
			nd.AxisMargin = AxisMargin;
			nd.VAlign = VAlign;
			nd.LastValueInAxis = LastValueInAxis;
			nd.SameColor = SameColor;
			nd.htPoints = htPoints;
			if (htDataArray!=null)
			foreach(string Key in SubData.Keys)
				nd[Key] = (double[])((double[])SubData[Key]).Clone();
			return nd;
		}

		#endregion
	}
}