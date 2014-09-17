using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections;

namespace Easychart.Finance.Objects
{
	public enum SnapType {None,Price,Band};
	/// <summary>
	/// Summary description for RectangleObject.
	/// </summary>
	public class RectangleObject : FillPolygonObject
	{
		int roundWidth = 0;
		[DefaultValue(0),XmlAttribute]
		public int RoundWidth
		{
			get
			{
				return roundWidth;
			}
			set
			{
				roundWidth = value;
			}
		}

		SnapType snap = SnapType.None;
		[DefaultValue(SnapType.None),XmlAttribute]
		public SnapType Snap
		{
			get
			{
				return snap;
			}
			set
			{
				snap = value;
			}
		}

		public RectangleObject()
		{
		}

		public void Round20()
		{
			Brush.Alpha = 0;
			roundWidth = 20;
		}

		public void SnapPrice()
		{
			snap = SnapType.Price;
			Brush.BrushStyle = BrushMapperStyle.Solid;
			Brush.Color = Color.FromArgb(160,160,64);
			Brush.Alpha = 40;
		}

		public void SnapBand()
		{
			snap = SnapType.Band;
			Brush.BrushStyle = BrushMapperStyle.Solid;
			Brush.Color = Color.FromArgb(160,160,64);
			Brush.Alpha = 40;
		}

		public override System.Drawing.PointF[] CalcPoint()
		{
			SetSnapPrice(snap);
			PointF[] pfs = ToPoints(ControlPoints);

			if (pfs[0].X>pfs[1].X)
			{
				float A = pfs[0].X;
				pfs[0].X = pfs[1].X;
				pfs[1].X = A;
			}
			if (pfs[0].Y>pfs[1].Y)
			{
				float A = pfs[0].Y;
				pfs[0].Y = pfs[1].Y;
				pfs[1].Y = A;
			}

			if (snap==SnapType.Band)
			{
				Rectangle RR = Area.Canvas.Rect;
				pfs[0].Y = RR.Top-30;
				pfs[1].Y = RR.Bottom +30;
			}
			
			float R = Math.Min(roundWidth,Math.Min(pfs[1].X-pfs[0].X,pfs[1].Y-pfs[0].Y)/2);

			ArrayList al = new ArrayList();
			al.Add(pfs[0]);
			al.Add(new PointF(pfs[1].X,pfs[0].Y));
			al.Add(pfs[1]);
			al.Add(new PointF(pfs[0].X,pfs[1].Y));
			pfs = (PointF[])al.ToArray(typeof(PointF));

			GraphicsPath gp = new GraphicsPath();
			if (R>0)
				gp.AddArc(pfs[0].X,pfs[0].Y,R*2,R*2,180f,90f);
			gp.AddLine(pfs[0].X+R,pfs[0].Y,pfs[1].X-R,pfs[1].Y);

			if (R>0)
				gp.AddArc(pfs[1].X-R*2,pfs[0].Y,R*2,R*2,270f,90f);
			gp.AddLine(pfs[1].X,pfs[1].Y+R,pfs[2].X,pfs[2].Y-R);

			if (R>0)
				gp.AddArc(pfs[2].X-R*2,pfs[2].Y-R*2,R*2,R*2,0f,90f);
			gp.AddLine(pfs[2].X-R,pfs[2].Y,pfs[3].X+R,pfs[3].Y);
			if (R>0)
				gp.AddArc(pfs[3].X,pfs[3].Y-R*2,R*2,R*2,90f,90f);
			gp.AddLine(pfs[3].X,pfs[3].Y-R,pfs[0].X,pfs[0].Y+R);
			gp.Flatten();
			return gp.PathPoints;
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
									   new ObjectInit("Rectangle",typeof(RectangleObject),"Empty","Shape","Rect",700),
									   new ObjectInit("Price Rectangle",typeof(RectangleObject),"SnapPrice","Shape","RectPrice"),
									   new ObjectInit("Band Rectangle",typeof(RectangleObject),"SnapBand","Shape","RectBand"),
									   new ObjectInit("Round Rectangle",typeof(RectangleObject),"Round20","Shape","RectRound"),
				};
		}
	}
}