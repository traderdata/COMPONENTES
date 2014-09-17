using System;
using System.ComponentModel;
using System.Drawing;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for DataCycleConverter.
	/// </summary>
	public class DataCycleConverter:ExpandableObjectConverter
	{
		public DataCycleConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType==typeof(string))
				return true;
			return base.CanConvertFrom (context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType==typeof(string))
				return true;
			return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string)
				return DataCycle.Parse((string)value);
			return base.ConvertFrom (context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType==typeof(string))
				return value.ToString();
			return base.ConvertTo (context, culture, value, destinationType);
		}
	}
}
