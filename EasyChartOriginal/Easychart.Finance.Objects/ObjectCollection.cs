using System;
using System.Collections;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectCollection.
	/// </summary>
	[Serializable]
	public class ObjectCollection:CollectionBase
	{
		public ObjectCollection()
		{
		}

		public virtual void Add(ObjectBase ob)
		{
			this.List.Add(ob);
		}

		public virtual void Insert(int Index,ObjectBase ob)
		{
			this.List.Insert(Index,ob);
		}

		public virtual ObjectBase this[int Index] 
		{
			get
			{
				return (ObjectBase)this.List[Index];
			}
		}

		public int IndexOf(ObjectBase ob) 
		{
			return List.IndexOf(ob);
		}

		public void Remove(ObjectBase value) 
		{
			List.Remove(value);
		}
	}
}
