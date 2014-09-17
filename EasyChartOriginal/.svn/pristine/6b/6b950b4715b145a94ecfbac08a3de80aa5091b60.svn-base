using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for CrossObject.
	/// </summary>
	public class CrossObject:LineGroupObject
	{
		int lineCount = 2;
		[DefaultValue(2),XmlAttribute]
		public int LineCount
		{
			get
			{
				return lineCount;
			}
			set
			{
				lineCount = value;
			}
		}
		
		public CrossObject()
		{
		}

		public override int InitNum
		{
			get
			{
				return 3;
			}
		}

		public override int ControlPointNum
		{
			get
			{
				return 3;
			}
		}

		public override void CalcPoint()
		{
			PointF[] pfs = ToPoints(ControlPoints);
			pfStart = new PointF[3+lineCount*2];
			pfEnd = new PointF[3+lineCount*2];
			pfStart[0] = pfs[0];  pfEnd[0] = pfs[1];
			//pfStart[1] = pfs[1];  pfEnd[1] = pfs[2];
			pfStart[2] = pfs[0];  pfEnd[2] = new PointF((pfs[1].X+pfs[2].X)/2,(pfs[1].Y+pfs[2].Y)/2);
			float w1 = pfEnd[2].X-pfs[0].X;
			float h1 = pfEnd[2].Y-pfs[0].Y;
			float w2 = pfEnd[2].X-pfs[1].X;
			float h2 = pfEnd[2].Y-pfs[1].Y;

			for(int i=0; i<lineCount*2; i++)
			{
				int j = i-lineCount;
				if (j>=0) j++;
				pfStart[3+i] = new PointF(pfEnd[2].X-w2*j,pfEnd[2].Y-h2*j);
				pfEnd[3+i] = new PointF(pfStart[3+i].X+w1*Math.Abs(j),pfStart[3+i].Y+h1*Math.Abs(j));
				ExpandLine(ref pfStart[3+i],ref pfEnd[3+i]);
			}
			pfStart[1] = pfStart[3];  pfEnd[1] = pfStart[pfStart.Length-1];
			ExpandLine(ref pfStart[2],ref pfEnd[2]);
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
									   new ObjectInit("Cross Channel",typeof(CrossObject),null,"Channel","ChannelCross")
								   };
		}
	}
}
