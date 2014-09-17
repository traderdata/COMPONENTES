using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for FormulaUserSkin.
	/// </summary>
	[XmlRoot(IsNullable = false,ElementName="Root")]
	public class FormulaUserSkin
	{
		static private XmlSerializer xs;
		const string Ext = ".skin";

		private string userSkinName;
		[XmlAttribute]
		public string UserSkinName
		{
			get
			{
				return userSkinName;
			}
			set
			{
				userSkinName = value;
			}
		}

		public FormulaUserSkin()
		{
		}

		private FormulaLineStyleCollection lineStyles;
		[XmlElement(typeof(FormulaLineStyle))]
		public FormulaLineStyleCollection FormulaLineStyles
		{
			get
			{
				return lineStyles;
			}
			set
			{
				lineStyles = value;
			}
		}

		public static  void InitSerializer()
		{
			if (xs==null)
				xs = new XmlSerializer(typeof(FormulaUserSkin));
		}

		public void SaveToStream(Stream s)
		{
			InitSerializer();
			xs.Serialize(s,this);
		}

		public string GetSkinFile(string FileName)
		{
			return FormulaHelper.SkinRoot+FileName+Ext;
		}

		public string GetSkinFile()
		{
			return GetSkinFile(userSkinName);
		}

		public void Save()
		{
			using (FileStream fs = File.Create(GetSkinFile()))
				SaveToStream(fs);
		}
	}
}