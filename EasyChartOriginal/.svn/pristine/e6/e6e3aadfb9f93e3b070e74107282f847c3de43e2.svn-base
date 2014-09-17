using System;
using System.Collections;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectCollection.
	/// </summary>
	public class ObjectCollection:CollectionBase
	{
		public ObjectCollection()
		{
		}

		public virtual void Add(BaseObject bo)
		{
			this.List.Add(bo);
		}

		public virtual void Insert(int Index,BaseObject bo)
		{
			this.List.Insert(Index,bo);
		}

		public virtual BaseObject this[int Index] 
		{
			get
			{
				return (BaseObject)this.List[Index];
			}
		}

		public int IndexOf(BaseObject ob) 
		{
			return List.IndexOf(ob);
		}

		public void Remove(BaseObject value) 
		{
			List.Remove(value);
		}
	}
}
