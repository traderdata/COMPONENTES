using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Easychart.Finance.Objects
{
	public enum SnapStyle {DateTime,BarsStart,BarsEnd,Screen,Chart};
	public enum ObjectSmoothingMode {Default,AntiAlias};

	/// <summary>
	/// Summary description for Base.
	/// </summary>
	[Serializable]
	public class ObjectBase
	{
		private ObjectPoint[] controlPoints;
		private ObjectPen linePen = new ObjectPen();
		private SnapStyle snapStyle;
		private ObjectSmoothingMode smoothingMode;
		private string areaName;
		private FormulaArea area;

		[NonSerialized]
		public bool InSetup;
		[NonSerialized]
		public bool InMove;
		[NonSerialized]
		protected ObjectManager Manager;
		[NonSerialized]
		protected Graphics CurrentGraphics;

		[Browsable(false)]
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

		[XmlIgnore]
		public PointF[] InitPoints;
		public void SetObjectManager(ObjectManager Manager)
		{
			this.Manager = Manager;
		}

		public ObjectBase()
		{
			ControlPoints = new ObjectPoint[ControlPointNum];
			smoothingMode = ObjectSmoothingMode.AntiAlias;
		}

		public ObjectBase(ObjectManager Manager):this()
		{
			SetObjectManager(Manager);
		}

		public float Dist(int X,int Y,PointF p1,PointF p2)
		{
			float w = p1.X-p2.X;
			float h = p1.Y-p2.Y;
			float Dis=((X-p1.X)*(p2.Y-p1.Y)-(p2.X-p1.X)*(Y-p1.Y))/((float)Math.Sqrt(w*w+h*h));
			return Dis;
		}

		public bool InLineSegment(int X,int Y,PointF p1,PointF p2,int PenWidth)
		{
			float MaxX = Math.Max(p1.X,p2.X);
			float MinX = Math.Min(p1.X,p2.X);
			float MaxY = Math.Max(p1.Y,p2.Y);
			float MinY = Math.Min(p1.Y,p2.Y);
			return (Math.Abs(Dist(X,Y,p1,p2))<=PenWidth && 
				X<=MaxX+PenWidth && X>=MinX-PenWidth && Y<=MaxY+PenWidth && Y>=MinY-PenWidth);
		}

		public bool InLineSegment(int X,int Y,ObjectPoint p1,ObjectPoint p2,int PenWidth)
		{
			return InLineSegment(X,Y,ToPointF(p1),ToPointF(p2),PenWidth);
		}

		public bool InLineSegment(int X,int Y,ObjectSegment os,int PenWidth)
		{
			return InLineSegment(X,Y,os.op1,os.op2,PenWidth);
		}

		public bool InLineSegment(int X,int Y,PointF[] pfs,int PenWidth,bool Closed)
		{
			for(int i=0; i<pfs.Length-(Closed?0:1); i++)
			{
				bool b = InLineSegment(X,Y,pfs[i],pfs[(i+1) % pfs.Length],PenWidth);
				if (b) 
					return b;
			}
			return false;
		}

		public bool InLineSegment(int X,int Y,PointF[] pfs,int PenWidth)
		{
			return InLineSegment(X,Y,pfs,PenWidth,false);
		}

		public double Dist(PointF p1,PointF p2)
		{
			double d1 = (p1.X-p2.X);
			double d2 = (p1.Y-p2.Y);
			return Math.Sqrt(d1*d1+d2*d2);
		}

		public void ExpandLine(ref PointF pf1,ref PointF pf2)
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
				else Y = 0;
			}
			pf2.X = X;
			pf2.Y = Y;
		}

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

		public PointF ToPointF(ObjectPoint op)
		{
			FormulaChart fc = Manager.Canvas.BackChart;
			if (fc!=null) 
			{
				return fc.GetPointAt(AreaName,op.X,null,op.Y);
			}
			return PointF.Empty;
		}

		public PointF[] ToPoints(ObjectPoint[] ops)
		{
			PointF[] pfs = new PointF[ops.Length];
			for(int i=0; i<pfs.Length; i++)
				pfs[i] = ToPointF(ops[i]);
			return pfs;
		}

		public ObjectSmoothingMode SmoothingMode
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

		public SnapStyle SnapStyle
		{
			get
			{
				return snapStyle;
			} 
			set 
			{
				snapStyle = value;
			}
		}

		public ObjectPen LinePen
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

		[TypeConverter(typeof(PointArrayConverter))]
		public ObjectPoint[] ControlPoints
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

		public void DrawControlPoint(Graphics g)
		{
			int w = 6+LinePen.Width/2;
			foreach(PointF pf in ToPoints(ControlPoints))
			{
				g.FillRectangle(Brushes.LightGreen,pf.X-w/2,pf.Y-w/2,w,w);
				g.DrawRectangle(Pens.Black,pf.X-w/2,pf.Y-w/2,w,w);
			}
		}

		public virtual void Draw(Graphics g)
		{
			if (SmoothingMode==ObjectSmoothingMode.AntiAlias)
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			else if (SmoothingMode==ObjectSmoothingMode.Default)
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			CurrentGraphics = g;
		}

		public virtual RectangleF GetMaxRect(PointF[] pfs)
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
			int w = LinePen.Width+6;
			RectangleF R = new RectangleF(MinX-w,MinY-w,MaxX-MinX+w*2,MaxY-MinY+w*2);
			if (R.X<0) R.X = 0;
			if (R.Y<0) R.Y= 0;
			return R;
		}

		public virtual RectangleF GetMaxRect()
		{
			return GetMaxRect(ToPoints(ControlPoints));
		}

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
	}
}