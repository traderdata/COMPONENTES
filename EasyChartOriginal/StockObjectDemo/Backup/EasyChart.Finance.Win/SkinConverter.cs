using System;
using System.ComponentModel;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Converter used by form designer to list all build-in skins
	/// </summary>
	public class SkinConverter : StringConverter
	{
		/// <summary>
		/// Create the instance of SkinConverter
		/// </summary>
		public SkinConverter()
		{
		}

		/// <summary>
		/// List all build-in skins
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			string[] ss = FormulaSkin.GetBuildInSkins();
			return new StandardValuesCollection(ss);
		}

		/// <summary>
		/// return true
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// return true
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
