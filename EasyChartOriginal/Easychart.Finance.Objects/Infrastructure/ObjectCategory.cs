using System;
using System.Collections;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Object category for tools panel
	/// </summary>
	public class ObjectCategory
	{
		public string CategoryName;
		public int Order;
		public ArrayList ObjectList = new ArrayList();
		
		public ObjectCategory(string CategoryName,int Order)
		{
			this.CategoryName = CategoryName;
			this.Order = Order;
		}
	}
}
