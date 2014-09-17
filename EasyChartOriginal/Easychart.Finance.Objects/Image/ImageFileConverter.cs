using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ImageFileConverter.
	/// </summary>
	public class ImageFileConverter : StringConverter
	{
		public ImageFileConverter()
		{
		}
		
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			string s = Assembly.GetExecutingAssembly().Location;
			s = s.Substring(0,s.Length-Path.GetFileName(s).Length)+"Images\\";
			string[] ss = Directory.GetFiles(s);
			for(int i=0; i<ss.Length; i++)
				ss[i] = Path.GetFileName(ss[i]);
			return new StandardValuesCollection(ss);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
