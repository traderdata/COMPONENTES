using System;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for Helper.
	/// </summary>
	public class ObjectHelper
	{
		public ObjectHelper()
		{
		}

		public static string GetRoot()
		{
			string s = Assembly.GetExecutingAssembly().Location;
			return s.Substring(0,s.Length-Path.GetFileName(s).Length);
		}

		public static string GetImageRoot()
		{
			return GetRoot()+"Images\\";
		}

		public static string GetObjectRoot()
		{
			return GetRoot()+"Object\\";
		}
		
		public static string GetObjectFile(string Symbol)
		{
			return GetObjectRoot()+Symbol+".xml";
		}

		public static string GetIconRoot()
		{
			return GetRoot()+"Icon\\";
		}

		public static string GetIconFile(string IconName)
		{
			if (IconName!=null)
				return GetIconRoot()+IconName+".gif";
			else return "";
		}

		public static void CreateObjectPath()
		{
			Directory.CreateDirectory(GetObjectRoot());
		}

		public static CultureInfo enUS = new CultureInfo("en-US");

	}
}
