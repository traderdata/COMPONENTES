using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for Line.
	/// </summary>
	[Serializable]
	public class ObjectLine : BaseObject
	{
		private bool openStart;
		private bool openEnd;
		private int Num = 2;

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

		public ObjectLine() :base()
		{
			Num = 2;
		}

		public void InitArrowCap()
		{
			LinePen.EndCap = new ArrowCap(10,10,false);
		}

		public void InitLine()
		{
			openStart = true;
			openEnd = true;
		}

		public void InitLine3()
		{
			Num = 4;
			InitPoints = new PointF[]{
					new PointF(0,0),
					new PointF(1,3),
					new PointF(2,1),
					new PointF(3,5),
			};
			ControlPoints = new ObjectPoint[Num];
		}

		public void InitLine5()
		{
			Num = 6;
			InitPoints = new PointF[]{
										 new PointF(0,0),
										 new PointF(1,3),
										 new PointF(2,1),
										 new PointF(3,5),
										 new PointF(4,4),
										 new PointF(5,7),
			};
			ControlPoints = new ObjectPoint[Num];
		}

		public void InitLine8()
		{
			Num = 9;
			InitPoints = new PointF[]{
										 new PointF(0,0),
										 new PointF(1,3),
										 new PointF(2,1),
										 new PointF(3,5),
										 new PointF(4,4),
										 new PointF(5,7),
										 new PointF(6,5),
										 new PointF(7,6),
										 new PointF(8,4),
			};
			ControlPoints = new ObjectPoint[Num];
		}

		public void InitLine4()
		{
			Num = 5;
			InitPoints = new PointF[]{
										 new PointF(0,0),
										 new PointF(1,2),
										 new PointF(2,0),
										 new PointF(3,2),
										 new PointF(4,0),
			};
			ControlPoints = new ObjectPoint[Num];
		}

		public void InitUnlimit1()
		{
			openEnd = true;
		}

		public void InitUnlimit2()
		{
			openStart = true;
			openEnd = true;
		}


		public override int ControlPointNum
		{
			get
			{
				return Num;
			}
		}

		public PointF[] ToPoints()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			if (pfs.Length>1)
			{
				if (openStart)
					ExpandLine(ref pfs[1],ref pfs[0]);
				if (openEnd)
					ExpandLine(ref pfs[pfs.Length-2],ref pfs[pfs.Length-1]);
			}
			return pfs;
		}

		public override bool InObject(int X, int Y)
		{
			PointF[] pfs = ToPoints();
			for(int i=0; i<pfs.Length-1; i++) 
			{
				bool b = InLineSegment(X,Y,pfs[i],pfs[i+1],this.LinePen.Width+1);
				if (b) return b;
			}
			return false;
		}
		
		public override RectangleF GetMaxRect()
		{
			return base.GetMaxRect(ToPoints());
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			PointF[] pfs = ToPoints();
			g.DrawLines(LinePen.GetPen(),pfs);
		}
	}
}
