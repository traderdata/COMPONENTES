using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Reflection;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	public enum ObjectSmoothingMode {Default,AntiAlias};

	/// <summary>
	/// Summary description for Base.
	/// </summary>
	[Serializable]
	public class BaseObject
	{
		private ObjectPoint[] controlPoints;
		private PenMapper linePen = new PenMapper();
		private ObjectSmoothingMode smoothingMode;
		private FormulaArea area;
		private ObjectInit objectType; 

		public BaseObject()
		{
			smoothingMode = ObjectSmoothingMode.AntiAlias;
			Init();
		}

		[Browsable(false),XmlIgnore]
		public ObjectInit ObjectType
		{
			get
			{
				return objectType;
			}
			set
			{
				objectType = value;
			}
		}

		[Browsable(false)]
		public virtual string Name
		{
			get
			{
				string s = GetType().ToString();
				int i = s.LastIndexOf('.');
				if (i>0)
					s = s.Substring(i+1);
				if (s.EndsWith("Object"))
					s = s.Substring(0,s.Length-6);
				return s;
			}
		}

		public BaseObject(ObjectManager Manager):this()
		{
			SetObjectManager(Manager);
		}

		public void Init()
		{
			ControlPoints = new ObjectPoint[ControlPointNum];
		}

		[TypeConverter(typeof(PointArrayConverter))]
		public virtual ObjectPoint[] ControlPoints
		{
			get 
			{
				return controlPoints;
			}
			set 
			{
				controlPoints = value;
			}
		}

		[NonSerialized]
		[XmlIgnore]
		public bool InSetup;
		[NonSerialized]
		[XmlIgnore]
		public bool InMove;
		[NonSerialized]
		protected ObjectManager Manager;
		[NonSerialized]
		protected Graphics CurrentGraphics;

		private string areaName = "FML.MAIN";
		[Browsable(false)]
		[DefaultValue("FML.MAIN")]
		public string AreaName
		{
			get
			{
				return areaName;
			}
			set
			{
				areaName = value;
			}
		}

		[Browsable(false),XmlIgnore]
		public FormulaArea Area
		{
			get
			{
				return area;
			}
			set
			{
				area = value;
			}
		}

		private double snapPercent = 0.02;
		[Category("Design"),XmlIgnore]
		public virtual double SnapPercent
		{
			get
			{
				return snapPercent;
			}
			set
			{
				snapPercent = value;
			}
		}

		[XmlIgnore]
		public PointF[] InitPoints;
		public void SetObjectManager(ObjectManager Manager)
		{
			this.Manager = Manager;
		}

		/// <summary>
		/// Calculate the Upper channel or Lower channel from line Y=A+BY
		/// between Bar1 and Bar2
		/// </summary>
		/// <param name="fd"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="Bar1"></param>
		/// <param name="Bar2"></param>
		/// <param name="LineName"></param>
		/// <param name="CalcMax"></param>
		/// <returns></returns>
		public float CalcDelta(FormulaData fd,double A,double B,int Bar1,int Bar2,string LineName,bool CalcMax)
		{
			double[] dd = fd[LineName];
			if (dd==null)
				dd=fd.Data;
			double M = double.MinValue;
			if (!CalcMax)
				M = double.MaxValue;
			for(int i =Math.Max(0,Bar1); i<Math.Min(Bar2,dd.Length); i++)
			{
				double Y = A+B*(i-Bar1);
				if (CalcMax)
					M = Math.Max(M,dd[i]-Y);
				else M = Math.Min(M,dd[i]-Y);
			}
			return (float)M;
		}

		/// <summary>
		/// Make control points snap to price
		/// </summary>
		/// <param name="snap"></param>
		public void SetSnapPrice(SnapType snap)
		{
			if (snap==SnapType.Price)
			{
				FormulaChart BackChart = Manager.Canvas.Chart;
				IDataProvider idp = BackChart.DataProvider;
				int i1 = 0; 
				int i2 = 1;
				if (ControlPointNum==1)
					i2 = 0;

				int Bar1 = BackChart.DateToIndex(ControlPoints[i1].X);
				int Bar2 = BackChart.DateToIndex(ControlPoints[i2].X);
				if (Bar1>Bar2)
					Swap(ref Bar1,ref Bar2);

				FormulaData fd = Area.FormulaDataArray[0];
				double[] dd1 = fd["L"];
				if (dd1==null)
					dd1=fd.Data;
				double[] dd2 = fd["H"];
				if (dd2==null)
					dd2=fd.Data;
				if (Bar1>=dd1.Length)
					Bar1 = dd1.Length-1;
				if (Bar2>=dd1.Length)
					Bar2 = dd1.Length-1;

				float A = float.MaxValue;
				float B = float.MinValue;
				for(int i = Bar1; i<=Bar2; i++)
				{
					A = Math.Min(A,(float)dd1[i]);
					B = Math.Max(B,(float)dd2[i]);
				}
				if (ControlPointNum>1)
				{
					if (ControlPoints[0].Y<ControlPoints[1].Y) Swap(ref A,ref B);
					ControlPoints[0].Y = B;
					ControlPoints[1].Y = A;
				} 
				else 
				{
					if (ControlPoints[0].Y<A) 
						ControlPoints[0].Y = A;
					else if (ControlPoints[0].Y>B)
						ControlPoints[0].Y = B;
				}
			} 
		}

		public void SetSnapLine(int PointIndex,bool OpenStart,bool OpenEnd)
		{
			FormulaChart BackChart = Manager.Canvas.Chart;
			if (BackChart==null) return;
			IDataProvider idp = BackChart.DataProvider;
			if (idp==null) return;

			if (PointIndex<ControlPoints.Length)
			{
				int Bar = BackChart.DateToIndex(ControlPoints[PointIndex].X);
				int Bar1 = BackChart.DateToIndex(BackChart.StartTime);
				int Bar2 = BackChart.DateToIndex(BackChart.EndTime);
				if (!OpenStart)
					Bar1 = Bar;
				if (!OpenEnd)
					Bar2 = Bar;
				FormulaData fd = Area.FormulaDataArray[0];
				double[][] dd = new double[][]{fd["L"],fd["H"]};
				if (dd[0]==null)
					dd[0]=fd.Data;
				if (dd[1]==null)
					dd[1]=fd.Data;
				if (Bar1>=dd[0].Length)
					Bar1 = dd[0].Length-1;
				if (Bar2>=dd[0].Length)
					Bar2 = dd[0].Length-1;

				if (Bar1>=0 && Bar2>=0)
				{
					double A = float.MaxValue;
					double B = ControlPoints[PointIndex].Y;
					double C = B;
					for(int i = Bar1; i<=Bar2; i++)
					{
						for(int j=0; j<dd.Length; j++) 
						{
							double k = Math.Abs(dd[j][i]-B);
							if (k<A)
							{
								A = k;
								C = dd[j][i];
							}
						}
					}
					double M = (Area.AxisY.MaxY-Area.AxisY.MinY);
					if (A/M<snapPercent)
						ControlPoints[PointIndex].Y = C;
				}
			}
		}

		/// <summary>
		/// Swap A and B
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		public void Swap(ref double A,ref double B)
		{
			double C = A;
			A = B;
			B = C;
		}

		/// <summary>
		/// Swap A and B
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		public void Swap(ref float A,ref float B)
		{
			float C = A;
			A = B;
			B = C;
		}

		/// <summary>
		/// Swap A and B
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		public void Swap(ref int A,ref int B)
		{
			int C = A;
			A = B;
			B = C;
		}

		/// <summary>
		/// Calculate the distance of p1 and p2
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public double Dist(PointF p1,PointF p2)
		{
			double d1 = (p1.X-p2.X);
			double d2 = (p1.Y-p2.Y);
			return Math.Sqrt(d1*d1+d2*d2);
		}

		/// <summary>
		/// Calculate the distance from (X,Y) to line (p1,p2)
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public float Dist(int X,int Y,PointF p1,PointF p2)
		{
			float w = p1.X-p2.X;
			float h = p1.Y-p2.Y;
			float Dis=((X-p1.X)*(p2.Y-p1.Y)-(p2.X-p1.X)*(Y-p1.Y))/((float)Math.Sqrt(w*w+h*h));
			return Dis;
		}

		/// <summary>
		/// Check if (X,Y) in the segment (p1,p2)
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="PenWidth"></param>
		/// <returns></returns>
		public bool InLineSegment(int X,int Y,PointF p1,PointF p2,int PenWidth)
		{
			float MaxX = Math.Max(p1.X,p2.X);
			float MinX = Math.Min(p1.X,p2.X);
			float MaxY = Math.Max(p1.Y,p2.Y);
			float MinY = Math.Min(p1.Y,p2.Y);
			return (Math.Abs(Dist(X,Y,p1,p2))<=PenWidth && 
				X<=MaxX+PenWidth && X>=MinX-PenWidth && Y<=MaxY+PenWidth && Y>=MinY-PenWidth);
		}

		/// <summary>
		/// Check if (X,Y) in the segment (p1,p2)
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="PenWidth"></param>
		/// <returns></returns>
		public bool InLineSegment(int X,int Y,ObjectPoint p1,ObjectPoint p2,int PenWidth)
		{
			return InLineSegment(X,Y,ToPointF(p1),ToPointF(p2),PenWidth);
		}

		/// <summary>
		/// Check if (X,Y) in the segment os
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="os"></param>
		/// <param name="PenWidth"></param>
		/// <returns></returns>
		public bool InLineSegment(int X,int Y,ObjectSegment os,int PenWidth)
		{
			return InLineSegment(X,Y,os.op1,os.op2,PenWidth);
		}

		/// <summary>
		/// Check if (X,Y) in the groups of segment pfs
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="pfs"></param>
		/// <param name="PenWidth"></param>
		/// <param name="Closed"></param>
		/// <returns></returns>
		public bool InLineSegment(int X,int Y,PointF[] pfs,int PenWidth,bool Closed)
		{
			if (pfs!=null)
			for(int i=0; i<pfs.Length-(Closed?0:1); i++)
			{
				bool b = InLineSegment(X,Y,pfs[i],pfs[(i+1) % pfs.Length],PenWidth);
				if (b) 
					return b;
			}
			return false;
		}

		/// <summary>
		/// Check if (X,Y) in the groups of segment pfs
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="pfs"></param>
		/// <param name="PenWidth"></param>
		/// <returns></returns>
		public bool InLineSegment(int X,int Y,PointF[] pfs,int PenWidth)
		{
			return InLineSegment(X,Y,pfs,PenWidth,false);
		}

		/// <summary>
		/// Expand line (pf1,pf2) to current area
		/// </summary>
		/// <param name="pf1"></param>
		/// <param name="pf2"></param>
		public void ExpandLine(ref PointF pf1,ref PointF pf2)
		{
			if (Area!=null && Area.Canvas!=null && pf1!=PointF.Empty && pf2!=PointF.Empty)
			{
				float DeltaX = pf2.X-pf1.X;
				float DeltaY = pf2.Y-pf1.Y;
				float X = pf2.X;
				float Y= pf2.Y;
				Rectangle R = Area.Canvas.Rect;
				if (DeltaX<0)
					X = 0;
				else if (DeltaX>0)
					X = R.Right;

				if (DeltaX!=0)
				{
					Y = (X-pf2.X)/DeltaX*DeltaY+pf2.Y;
					if (Y<R.Top || Y>R.Bottom)
					{
						if (Y>R.Bottom)
							Y = R.Bottom;
						else if (Y<R.Top )
							Y = R.Top;
						if (DeltaY!=0)
							X = (Y-pf2.Y)/DeltaY*DeltaX+pf2.X;
					}
				} 
				else 
				{
					if (DeltaY>0)
						Y = R.Bottom;
					else Y = R.Top;
				}
				pf2.X = X;
				pf2.Y = Y;
			}
		}

		/// <summary>
		/// Expand line (pf1,pf2) to current area
		/// </summary>
		/// <param name="pf1"></param>
		/// <param name="pf2"></param>
		public void ExpandLine2(ref PointF pf1,ref PointF pf2)
		{
			ExpandLine(ref pf1,ref pf2);
			ExpandLine(ref pf2,ref pf1);
		}

		/// <summary>
		/// Check if (X,Y) in ellipse
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="CenterX"></param>
		/// <param name="CenterY"></param>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		/// <param name="PenWidth"></param>
		/// <returns></returns>
		public bool PointInEllipse(int X,int Y, float CenterX,float CenterY,float r1,float r2,int PenWidth)
		{
			PointF f1;
			PointF f2; 

			float Diff = Math.Abs(r1)-Math.Abs(r2);
			if (Diff>0) 
			{
				float Diff2 = (float)Math.Sqrt(r1*r1-r2*r2);
				f1 = new PointF(CenterX+Diff2,CenterY);
				f2 = new PointF(CenterX-Diff2,CenterY);
			} 
			else 
			{
				float Diff2 = (float)Math.Sqrt(r2*r2-r1*r1);
				f1 = new PointF(CenterX,CenterY+Diff2);
				f2 = new PointF(CenterX,CenterY-Diff2);
			}

			PointF P = new PointF(X,Y);
			float R = Math.Max(Math.Abs(r1),Math.Abs(r2));
			return Math.Abs(Dist(P,f1)+Dist(P,f2) - R*2)<=PenWidth;
		}

		/// <summary>
		/// Convert ObjectPoint to PointF
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		public PointF ToPointF(ObjectPoint op)
		{
			FormulaChart fc = Manager.Canvas.Chart;
			if (fc!=null) 
			{
				return fc.GetPointAt(AreaName,op.X,null,op.Y);
			}
			return PointF.Empty;
		}

		/// <summary>
		/// Convert array of ObjectPoint to array of PointF
		/// </summary>
		/// <param name="ops"></param>
		/// <returns></returns>
		public PointF[] ToPoints(ObjectPoint[] ops)
		{
			PointF[] pfs = new PointF[ops.Length];
			for(int i=0; i<pfs.Length; i++)
				pfs[i] = ToPointF(ops[i]);
			return pfs;
		}

		/// <summary>
		/// Object Smoothing Mode
		/// </summary>
		[DefaultValue(ObjectSmoothingMode.AntiAlias)]
		public virtual ObjectSmoothingMode SmoothingMode
		{
			get
			{
				return smoothingMode;
			}
			set
			{
				smoothingMode = value;
			}
		}

//		public SnapStyle SnapStyle
//		{
//			get
//			{
//				return snapStyle;
//			} 
//			set 
//			{
//				snapStyle = value;
//			}
//		}

		/// <summary>
		/// Pen of current Object
		/// </summary>
		public virtual PenMapper LinePen
		{
			get 
			{
				return linePen;
			}
			set 
			{
				linePen = value;
			}
		}

		public bool ShouldSerializeLinePen()
		{
			return PenMapper.NotDefault(LinePen);
		}

		/// <summary>
		/// Check if (X,Y) in current object
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <returns></returns>
		public virtual bool InObject(int X,int Y)
		{
			if (ControlPointNum==2) 
			{
				PointF p1 = ToPointF(ControlPoints[0]);
				PointF p2 = ToPointF(ControlPoints[1]);
				return RectangleF.FromLTRB(p1.X,p1.Y,p2.X,p2.Y).Contains(X,Y);
			}
			return false;
		}

		/// <summary>
		/// Get nearest Control Point from (X,Y)
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <returns></returns>
		public int GetControlPoint(int X,int Y)
		{
			for(int i=0; i<ControlPoints.Length; i++) 
			{
				float x = ToPointF(ControlPoints[i]).X-X;
				float y = ToPointF(ControlPoints[i]).Y-Y;
				if (x*x+y*y<9)
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Draw control points
		/// </summary>
		/// <param name="g"></param>
		public void DrawControlPoint(Graphics g)
		{
			int w = 6+LinePen.Width/2;
			foreach(PointF pf in ToPoints(ControlPoints))
				try
				{
					g.FillRectangle(Brushes.LightGreen,pf.X-w/2,pf.Y-w/2,w,w);
					g.DrawRectangle(Pens.Black,pf.X-w/2,pf.Y-w/2,w,w);
				}
				catch
				{
				}
		}

		public void InitDraw(Graphics g)
		{
			if (SmoothingMode==ObjectSmoothingMode.AntiAlias)
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			else if (SmoothingMode==ObjectSmoothingMode.Default)
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			CurrentGraphics = g;
		}

		/// <summary>
		/// Draw object
		/// </summary>
		/// <param name="g"></param>
		public virtual void Draw(Graphics g)
		{
			InitDraw(g);
#if(TRIAL)
			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment =  StringAlignment.Center;
			g.DrawString("DEMO",new Font("verdana",20,FontStyle.Bold),
				new SolidBrush(Color.FromArgb(100,255,0,0)),GetMaxRect(),sf);
#endif
		}

		/// <summary>
		/// Get Maximum rectangle of current object
		/// </summary>
		/// <param name="pfs"></param>
		/// <param name="w"></param>
		/// <returns></returns>
		public RectangleF GetMaxRect(PointF[] pfs,int w)
		{
			float MinX = float.MaxValue;
			float MinY = float.MaxValue;
			float MaxX = float.MinValue;
			float MaxY = float.MinValue;
			foreach(PointF pf in pfs)
			{
				if (pf.X<MinX) MinX = pf.X;
				if (pf.X>MaxX) MaxX = pf.X;
				if (pf.Y<MinY) MinY = pf.Y;
				if (pf.Y>MaxY) MaxY = pf.Y;
			}
			RectangleF R = new RectangleF(MinX-w,MinY-w,MaxX-MinX+w*2,MaxY-MinY+w*2);
			return R;
		}

		/// <summary>
		/// Get Maximum Rectangle of array of PointF
		/// </summary>
		/// <param name="pfs"></param>
		/// <returns></returns>
		public virtual RectangleF GetMaxRect(PointF[] pfs) 
		{
			RectangleF R = GetMaxRect(pfs,LinePen.Width+6);
			if (R.X<0) R.X = 0;
			if (R.Y<0) R.Y= 0;
			return R;
		}

		/// <summary>
		/// Get Maximum rectangle of current object
		/// </summary>
		/// <returns></returns>
		public virtual RectangleF GetMaxRect()
		{
			return GetMaxRect(ToPoints(ControlPoints));
		}

		/// <summary>
		/// Get Region of current object
		/// </summary>
		/// <returns></returns>
		public virtual Region GetRegion()
		{
			return new Region(GetMaxRect());
		}

		[Browsable(false)]
		public virtual int ControlPointNum
		{
			get
			{
				return 2;
			}
		}

		[Browsable(false)]
		public virtual int InitNum
		{
			get
			{
				return 2;
			}
		}

		/// <summary>
		/// Replace format
		/// {D:yyyy-MM-dd}
		/// {C:f2}
		/// {0:f2}
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ReplaceTag(ObjectPoint op,string s)
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

					FormulaChart BackChart = Manager.Canvas.Chart;
					IDataProvider idp = BackChart.DataProvider;
					double[] dd = idp["DATE"];
					int Bar = BackChart.DateToIndex(op.X);
					if (string.Compare(s2,"D")==0)
					{
						if (s3=="")
							s3 = "yyyy-MM-dd";
						s2 = BackChart.IndexToDate(Bar).ToString(s3);
					} 
					else 
					{
						FormulaData fd = null;
						try
						{
							i = int.Parse(s2);
							if (i<Area.FormulaDataArray.Count)
								dd = Area.FormulaDataArray[i].Data;
						} 
						catch
						{
							fd = Area.FormulaDataArray[s2];
							if (object.Equals(fd,null)) 
							{
								foreach(FormulaData f in Area.FormulaDataArray)
								{
									dd = f[s2];
									if (dd!=null)
										break;
								}
							} 
							else dd = fd.Data;
						}
						if (dd!=null && Bar<dd.Length) 
						{
							if (s3=="")
								s3 = "f2";
							s2 = dd[Bar].ToString(s3);
						} else s2 = "NaN";
					} 
					s = s.Substring(0,i1)+s2+s.Substring(i2+1);
				} 
				else break;
			}
			return s;
		}

		public virtual ObjectInit[] RegObject()
		{
			return null;
		}
	}
}