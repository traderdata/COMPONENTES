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

		public ObjectInit(string Name,Type BaseType,string InitMethod)
		{
			this.Name = Name;
			this.BaseType = BaseType;
			this.InitMethod = InitMethod;
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

		public ObjectBase Invoke()
		{
			ObjectBase ob = (ObjectBase)Activator.CreateInstance(BaseType);
			if (InitMethod!=null)
				BaseType.InvokeMember(InitMethod,BindingFlags.InvokeMethod,null,ob,null);
			return ob;
		}
	}
}
