using System;
using System.ComponentModel;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for PointArrayConverter.
	/// </summary>
	public class PointArrayConverter:ArrayConverter
	{
		public PointArrayConverter()
		{
		}

//		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
//		{
//			if (sourceType==null ||  sourceType==typeof(string))
//				return true;
//			return base.CanConvertFrom (context, sourceType);
//		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType==null || destinationType==typeof(string))
				return true;
			return base.CanConvertTo (context, destinationType);
		}

//		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
//		{
//			if (value is string)
//			{
//				string[] ss = (value as string).Split(';');
//				PointF[] pfs = new PointF[ss.Length];
//				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Point));
//
//				for(int i=0; i<pfs.Length; i++)
//					pfs[i] = (PointF)tc.ConvertFromString(ss[i]);
//				return pfs;
//			}
//			return base.ConvertFrom (context, culture, value);
//		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType==typeof(string) && value is ObjectPoint[])
			{
				string[] ss = new string[(value as ObjectPoint[]).Length];
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(ObjectPoint));
				for(int i=0; i<ss.Length; i++)
					ss[i] =  tc.ConvertToString((value as ObjectPoint[])[i]);
				return string.Join(";",ss);

			}
			return base.ConvertTo (context, culture, value, destinationType);
		}
	}
}