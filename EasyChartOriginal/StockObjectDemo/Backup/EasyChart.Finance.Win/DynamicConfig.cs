using System;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using System.IO;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Save the config of certain control to app.config
	/// </summary>
	public class DynamicConfig
	{
		private DynamicConfig()
		{
		}

		static public XmlDocument LoadFromFile()
		{
			return LoadFromFile(Application.ExecutablePath+".config");
		}

		static public XmlDocument LoadFromFile(string Filename)
		{
			XmlDocument xd = new XmlDocument();
			if (File.Exists(Filename))
			{
				xd.Load(Filename);
				return xd;
			}
			return null;
		}

		static public void SaveToFile(XmlDocument xd)
		{
			string s = Application.ExecutablePath+".config";
			xd.Save(s);
		}

		/// <summary>
		/// Save the dynamic settings of a control
		/// </summary>
		/// <param name="C"></param>
		static public void Save(Control C)
		{
			XmlDocument xd =LoadFromFile();
			try
			{
				if (C is ChartWinControl)
					(C as ChartWinControl).SaveChartProperties();
				Save(xd,C);
			} 
			finally
			{
				SaveToFile(xd);
			}
		}

		static public void Save(XmlDocument xd,string ConfigTag,string Key,string Value)
		{
			XmlNode xns = xd.SelectSingleNode("/configuration/"+ConfigTag);
			if (xns==null)
			{
				xns = xd.SelectSingleNode("/configuration");
				XmlNode xnConfig = xd.CreateNode(XmlNodeType.Element,ConfigTag,"");
				xns.AppendChild(xnConfig);
				xns = xnConfig;
			}
			for(int i=0; i<xns.ChildNodes.Count; i++)
				if (xns.ChildNodes[i] is XmlElement)
				{
					XmlElement xe = xns.ChildNodes[i] as XmlElement;
					string XmlKey = xe.GetAttribute("key").ToString();
					if (XmlKey==Key)
					{
						if (Value!=null) 
						{
							xe.SetAttribute("value",Value.ToString());
							return;
						}
					}
				}
			XmlElement xeNew = (XmlElement)xd.CreateNode(XmlNodeType.Element,"add","");
			xeNew.SetAttribute("key",Key);
			xeNew.SetAttribute("value",Value);
			xns.AppendChild(xeNew);
		}

		static public void Save(XmlDocument xd,string Key,string Value)
		{
			Save(xd,"appSettings",Key,Value);
		}

		static public void Save(XmlDocument xd,Control C)
		{
			XmlNode xns = xd.SelectSingleNode("/configuration/appSettings");
			for(int i=0; i<xns.ChildNodes.Count; i++)
				if (xns.ChildNodes[i] is XmlElement)
				{
					XmlElement xe = xns.ChildNodes[i] as XmlElement;
					string Key = xe.GetAttribute("key").ToString();
					string ControlName = "";
					int j = Key.IndexOf('.');

					if (j>0) 
					{
						ControlName = Key.Substring(0,j);
						Key = Key.Substring(j+1);
					}
					if (C.Name==ControlName)
						try
						{
							object Value = C.GetType().InvokeMember(Key,BindingFlags.GetProperty,null,C,new object[]{});
							if (Value!=null)
								xe.SetAttribute("value",Value.ToString());
						} 
						catch
						{
						}
				}
		}
	}
}