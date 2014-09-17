using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for GridObject.
	/// </summary>
	public class GridObject:LineGroupObject
	{
		int gridCount = 10;
		[DefaultValue(10),XmlAttribute]
		public int GridCount
		{
			get
			{
				return gridCount;
			}
			set
			{
				gridCount = value;
			}
		}

		public GridObject()
		{
		}

		public override void CalcPoint()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			float W = pfs[1].X-pfs[0].X;
			float H = pfs[1].Y-pfs[0].Y;

			pfStart = new PointF[gridCount*4+2];
			pfEnd = new PointF[gridCount*4+2];
			for(int i=-gridCount; i<=gridCount; i++)
			{
				int A = (i+gridCount)*2;
				PointF p1 = new PointF(pfs[0].X+W*i,pfs[0].Y-H*i);
				PointF p2 = new PointF(pfs[1].X+W*i,pfs[1].Y-H*i);
				ExpandLine(ref p1,ref p2);
				ExpandLine(ref p2,ref p1);
				pfStart[A] = p1;
				pfEnd[A] = p2;

				p1 = new PointF(pfs[0].X+W*i,pfs[0].Y+H*i);
				p2 = new PointF(pfs[0].X+W+W*i,pfs[0].Y-H+H*i);
				ExpandLine(ref p1,ref p2);
				ExpandLine(ref p2,ref p1);
				pfStart[A+1] = p1;
				pfEnd[A+1] = p2;
			}
		}

		public override ObjectInit[] RegObject()
		{
			return 
				new ObjectInit[]{
							   new ObjectInit("Grid",typeof(GridObject),null,"Channel","ChannelGrid")
						   };
		}

	}
}