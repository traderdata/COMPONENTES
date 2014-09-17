using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Easychart.Finance.Objects
{
	public class ArrowCapConverter :ExpandableObjectConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType==null ||  sourceType==typeof(string))
				return true;
			return base.CanConvertFrom (context, sourceType);
		}
//
//		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
//		{
//			if (destinationType==null || destinationType==typeof(string))
//				return true;
//			return base.CanConvertTo (context, destinationType);
//		}
//
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value==null)
				return null;
			else if (value is string)
			{
				string[] ss = (value as string).Split(',');
				if (ss.Length==3)
					return new ArrowCap(int.Parse(ss[0]),int.Parse(ss[1]),bool.Parse(ss[2]));
				else return null;
			}
			return base.ConvertFrom (context, culture, value);
		}
//
//		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
//		{
//			if (destinationType==typeof(string) )
//			{
//				if (value == null)
//					return null;
//				else if (value is ArrowCap) 
//				{
//					ArrowCap ac = value as ArrowCap;
//					return ac.Width+","+ac.Height+","+ac.Filled;
//				}
//			}
//			return base.ConvertTo (context, culture, value, destinationType);
//		}

		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			string[] ss = new string[]{"10,10,false","10,10,true",""};
			return new StandardValuesCollection(ss);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}
	}

	[TypeConverter(typeof(ArrowCapConverter))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Serializable]
	public class ArrowCap
	{
		int width;
		int height;
		bool filled;

		public ArrowCap()
		{
		}

		public ArrowCap(int Width, int Height, bool Filled)
		{
			this.width = Width;
			this.height = Height;
			this.filled = Filled;
		}

		[XmlAttribute]
		public int Width 
		{
			get 
			{
				return width;
			}
			set 
			{
				width = value;
				if (height==0)
					height = value;
			}
		}

		[XmlAttribute]
		public int Height
		{
			get 
			{
				return height;
			}
			set 
			{
				height = value;
				if (width==0)
					width = value;
			}
		}

		[XmlAttribute]
		public bool Filled
		{
			get 
			{
				return filled;
			}
			set 
			{
				filled = value;
			}
		}

		public override string ToString()
		{
			return ""+width+","+height+","+filled;
		}
	}
}