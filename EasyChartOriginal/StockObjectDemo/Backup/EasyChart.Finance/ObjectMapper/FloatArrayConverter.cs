using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for FloatArrayConverter.
	/// </summary>
	public class FloatArrayConverter:ArrayConverter
	{
		public FloatArrayConverter()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType==null || destinationType==typeof(string) || destinationType==typeof(float[]))
				return true;
			return base.CanConvertTo (context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType==null || sourceType==typeof(string) || sourceType==typeof(float[]))
				return true;
			return base.CanConvertFrom (context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value==null)
				return null;
			else if (value is string)
			{
				string[] ss = (value as string).Split(',');
				if (ss.Length==0)
					return null;
				float[] ff = new float[ss.Length];
				for(int i=0; i<ff.Length; i++)
					ff[i] = float.Parse(ss[i],NumberFormatInfo.InvariantInfo);
				return ff;
			} else if (value is float[])
				return ((float[])value).Clone();
			return base.ConvertFrom (context, culture, value);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return base.CreateInstance (context, propertyValues);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType==typeof(string) && value is float[])
			{
				string s = "";
				foreach(float f in (float[])value) 
				{
					if (s.Length!=0) s +=",";
					s +=f.ToString();
				}
				return s;
			}
			return base.ConvertTo (context, culture, value, destinationType);
		}
	}
}
