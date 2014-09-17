using System;
using System.Drawing;
using System.Collections;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for Fan Object.
	/// </summary>
	public class FanObject:LineGroupObject
	{
		private float[] split;
		public float[] Split
		{
			get
			{
				return split;
			}
			set 
			{
				split = value;
			}
		}

		public FanObject()
		{
		}

		public void Equal3()
		{
			split = new float[]{1f/3,2f/3,1f};
		}

		public void Equal4()
		{
			split = new float[]{1f/4,1f/2,1f};
		}

		public void Fibonacci()
		{
			split = new float[]{1/3f,3/8f,1/2f,5/8f,2/3f,1f};
		}


		public override Region GetRegion()
		{
			return new Region();
		}

		public override void CalcPoint()
		{
			PointF[] pfs = ToPoints(ControlPoints);

			pfStart = new PointF[split.Length];
			pfEnd = new PointF[split.Length];
			for(int i=0; i<split.Length; i++)
			{
				pfStart[i] = pfs[0];
				pfEnd[i] = new PointF(pfs[1].X,pfs[0].Y+(pfs[1].Y-pfs[0].Y)*split[i]);
				ExpandLine(ref pfStart[i],ref pfEnd[i]);
			}
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
				new ObjectInit("Speed line",typeof(FanObject),"Equal3","Fan","Fan3",400),
				new ObjectInit("Speed line 4",typeof(FanObject),"Equal4","Fan","Fan4"),
				new ObjectInit("Fibonacci fan",typeof(FanObject),"Fibonacci","Fan","FanFib"),
				};
		}
	}
}
