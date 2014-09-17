using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ChannelObject.
	/// </summary>
	public class ChannelObject:LineGroupObject
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
		
		public ChannelObject()
		{
		}

		public void Equal()
		{
			split = new float[]{0f,0.25f,0.5f,0.75f,1f};
		}

		public void Fibonacci()
		{
			split = new float[]{0f,1/3f,3/8f,1/2f,5/8f,2/3f,1f};
		}

		public void Multi()
		{
			int Count = 10;
			split = new float[Count*2+1];
			for(int i=-Count; i<=Count; i++)
				split[i+Count] = i;
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
			pfStart = new PointF[split.Length];
			pfEnd = new PointF[split.Length];
			if (pfs.Length==3)
			{
				float w1 = pfs[2].X-pfs[0].X;
				float h1 = pfs[2].Y-pfs[0].Y;
				float w2 = pfs[2].X-pfs[1].X;
				float h2 = pfs[2].Y-pfs[1].Y;
				for(int i=0; i<split.Length; i++)
				{
					pfStart[i] = new PointF(pfs[0].X+w1*split[i],pfs[0].Y+h1*split[i]);
					if (split[i]==1)
						pfEnd[i] = new PointF(pfs[0].X-pfs[1].X+pfs[2].X,pfs[0].Y-pfs[1].Y+pfs[2].Y);
					else pfEnd[i] = new PointF(pfs[1].X+w2*split[i],pfs[1].Y+h2*split[i]);
				}
			}
			OpenStartEnd(true,true);
		}

//		public static void RegObject()
//		{
//			ObjectManager.RegObjects(
//				new ObjectInit[]{
//									new ObjectInit("Equal Channel",typeof(ChannelObject),"Equal","Channel","ChannelE",600),
//									new ObjectInit("Fibonacci Channel",typeof(ChannelObject),"Fibonacci","Channel","ChannelF"),
//									new ObjectInit("Channels",typeof(ChannelObject),"Multi","Channel","ChannelM"),
//			}
//				);
//		}

		public override ObjectInit[] RegObject()
		{
			return 	
				new ObjectInit[]{
									new ObjectInit("Equal Channel",typeof(ChannelObject),"Equal","Channel","ChannelE",600),
									new ObjectInit("Fibonacci Channel",typeof(ChannelObject),"Fibonacci","Channel","ChannelF"),
									new ObjectInit("Channels",typeof(ChannelObject),"Multi","Channel","ChannelM"),
				};
		}
	}
}