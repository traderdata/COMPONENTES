using System;
using System.Collections;
using System.Xml.Serialization;
using System.Text;

namespace Easychart.Finance
{
	public enum FormulaParamType {
		[XmlEnum("double")]
		Double,
		[XmlEnum("string")]
		String,
		[XmlEnum("symbol")]
		Symbol,
		[XmlEnum("indicator")]
		Indicator};

	/// <summary>
	/// Stock Formula parameter, Used by formula editor
	/// </summary>
	public class FormulaParam
	{
		[XmlIgnore]
		public string Value = "0";
		#region Fields will be saved to xml file
		[XmlAttribute]
		public string Name;
		[XmlAttribute]
		public string DefaultValue = "0";
		[XmlAttribute]
		public string MinValue = "0";
		[XmlAttribute]
		public string MaxValue = "0";
		[XmlAttribute]
		public string Description;
		[XmlAttribute]
		public string Step;
		[XmlAttribute]
		public FormulaParamType ParamType = FormulaParamType.Double;
		//public string ParamType = "double";
		//public string Format;
		#endregion

		public FormulaParam() 
		{
		}
		
		public FormulaParam(string ParamName,string DefaultValue,string MinValue,string MaxValue,FormulaParamType ParamType) :this()
		{
			this.Name = ParamName;
			this.DefaultValue = DefaultValue;
			this.MinValue = MinValue;
			this.MaxValue = MaxValue;
			this.ParamType = ParamType;
		}

		public FormulaParam(string ParamName,string DefValue,string MinValue,string MaxValue,string Description,FormulaParamType ParamType):this(ParamName,DefValue,MinValue,MaxValue,ParamType)
		{
			this.Description = Description;
		}

		public FormulaParam(string ParamName,double DefValue,double MinValue,double MaxValue):
			this(ParamName,DefValue.ToString(),MinValue.ToString(),MaxValue.ToString(),FormulaParamType.Double)
		{
		}
	}

	/// <summary>
	/// Collection of FormulaParam
	/// </summary>
	public class ParamCollection:CollectionBase
	{
		public int Add(FormulaParam fp)
		{
			return List.Add(fp);
		}

		public FormulaParam this[int Index] 
		{
			get
			{
				return (FormulaParam)this.List[Index];
			}
		}

		public FormulaParam this[string Name] 
		{
			get
			{
				foreach(object o in List)
					if (((FormulaParam)o).Name==Name)
						return (FormulaParam)o;
				return null;
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for(int i=0; i<this.Count; i++) 
			{
				if (i!=0) 
					sb.Append(",");
				string s = this[i].Value;
				if (s.IndexOf(',')>=0)
					s = "\""+s+"\"";
				sb.Append(s);
			}
			return sb.ToString();
		}

		public void Remove(FormulaParam fp)
		{
			List.Remove(fp);
		}

		public string[] GetParamList()
		{
			ArrayList al = new ArrayList();
			foreach(object o in List)
				al.Add(((FormulaParam)o).Name);
			return (string[])al.ToArray(typeof(string));
		}
	}
}
