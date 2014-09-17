using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for ObjectArrayEditor.
	/// </summary>
	public class FloatArrayEditor : System.ComponentModel.Design.ArrayEditor
	{

		public FloatArrayEditor(Type type):base(type)
		{
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object o = base.EditValue (context, provider, value);
			if (o==null || (o as Array).Length==0)
				return null;
			float[] fs = new float[(o as Array).Length];
			for(int i=0; i<fs.Length; i++)
				fs[i] = (float)(o as Array).GetValue(i);
			return  fs;
		}

		protected override object SetItems(object editValue, object[] value)
		{
			return value;
		}

	}
}
