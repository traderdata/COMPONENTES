using System;
using System.Reflection;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectInit.
	/// </summary>
	public class ObjectInit
	{
		public string Name;
		public Type BaseType;
		public string InitMethod;
		public string Icon;
		public string Category;
		public int Order;
		public int CategoryOrder;

		public ObjectInit(string Name,Type BaseType,string InitMethod,string Category,string Icon,int CategoryOrder,int Order)
		{
			this.Name = Name;
			this.BaseType = BaseType;
			this.InitMethod = InitMethod;
			this.Category = Category;
			this.Icon = Icon;
			this.CategoryOrder = CategoryOrder;
			this.Order = Order;
		}

		public ObjectInit(string Name,Type BaseType,string InitMethod,string Category,string Icon,int CategoryOrder):this(Name,BaseType,InitMethod,Category,Icon,CategoryOrder,0)
		{
		}

		public ObjectInit(string Name,Type BaseType,string InitMethod,string Category,string Icon):this(Name,BaseType,InitMethod,Category,Icon,int.MaxValue)
		{
		}

		public ObjectInit(string Name,Type BaseType,string InitMethod,string Category):this(Name,BaseType,InitMethod,Category,null)
		{
		}

		public ObjectInit(string Name,Type BaseType,string InitMethod):this(Name,BaseType,InitMethod,null)
		{
		}

		public ObjectInit(string Name,Type BaseType):this(Name,BaseType,null)
		{
		}

		public ObjectInit(Type BaseType,string InitMethod):this(BaseType.Name,BaseType,InitMethod)
		{
		}

		public ObjectInit(Type BaseType):this(BaseType.Name,BaseType,null)
		{
		}

		public BaseObject Invoke()
		{
			BaseObject bo = (BaseObject)BaseType.InvokeMember(null,
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance
				,null,null,null);

			if (InitMethod!=null)
				BaseType.InvokeMember(InitMethod,BindingFlags.InvokeMethod,null,bo,null);
			
			if (bo.ControlPoints.Length!=bo.ControlPointNum)
				bo.Init();
			bo.ObjectType = this;
			return bo;
		}
	}
}