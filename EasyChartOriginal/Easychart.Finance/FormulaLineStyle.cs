using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for FormulaLineStyle.
	/// </summary>
	public class FormulaLineStyle
	{
		public FormulaLineStyle()
		{
		}

		private string formulaName;
		[XmlAttribute]
		public string FormulaName
		{
			get
			{
				return formulaName;
			}
			set
			{
				formulaName = value;
			}
		}

		private Color upColor;
		[XmlIgnore()] 
		public Color UpColor
		{
			get
			{
				return upColor;
			}
			set
			{
				upColor = value;
			}
		}

		[XmlAttribute(AttributeName="UpColor")]
		[Browsable(false)]
		[DefaultValue("Black")]
		public string XmlUpColor
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(null,FormulaHelper.enUS,upColor);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				upColor = (Color)tc.ConvertFromString(null,FormulaHelper.enUS,value);
			}
		}

		private Color downColor;
		[XmlIgnore()] 
		public Color DownColor
		{
			get
			{
				return downColor;
			}
			set
			{
				downColor = value;
			}
		}

		[XmlAttribute(AttributeName="DownColor")]
		[Browsable(false)]
		[DefaultValue("Black")]
		public string XmlDownColor
		{
			get
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				return tc.ConvertToString(null,FormulaHelper.enUS,downColor);
			}
			set
			{
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
				downColor = (Color)tc.ConvertFromString(null,FormulaHelper.enUS,value);
			}
		}

		private float lineWidth = 1;
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(1),XmlAttribute]
		public float LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
			}
		}

		private DashStyle lineStyle = DashStyle.Solid;
		[XmlAttribute]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(DashStyle.Solid)]
		public DashStyle LineStyle
		{
			get
			{
				return lineStyle;
			}
			set
			{
				lineStyle = value;
			}
		}
	}

	public class FormulaLineStyleCollection:CollectionBase
	{
		public int Add(FormulaLineStyle fls)
		{
			return List.Add(fls);
		}

		public FormulaLineStyle this[int Index] 
		{
			get
			{
				return (FormulaLineStyle)this.List[Index];
			}
		}

		public FormulaLineStyle this[string Name]
		{
			get
			{
				foreach(object o in List)
					if (((FormulaLineStyle)o).FormulaName==Name)
						return (FormulaLineStyle)o;
				return null;
			}
		}

		public void Remove(FormulaLineStyle fls)
		{
			List.Remove(fls);
		}
	}
}