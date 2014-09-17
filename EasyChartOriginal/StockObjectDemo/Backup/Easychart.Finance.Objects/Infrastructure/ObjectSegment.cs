using System;
using System.Collections;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectSegment.
	/// </summary>
	public class ObjectSegment
	{
		public ObjectPoint op1;
		public ObjectPoint op2;

		public ObjectSegment(ObjectPoint op1,ObjectPoint op2)
		{
			this.op1 = op1;
			this.op2 = op2;
		}
	}

	public class SegmentCollection:CollectionBase
	{
		public virtual void Add(ObjectSegment os)
		{
			this.List.Add(os);
		}

		public virtual void Add(ObjectPoint op1,ObjectPoint op2)
		{
			Add(new ObjectSegment(op1,op2));
		}

		public virtual ObjectSegment this[int Index] 
		{
			get
			{
				return (ObjectSegment)this.List[Index];
			}
		}

	}
}
