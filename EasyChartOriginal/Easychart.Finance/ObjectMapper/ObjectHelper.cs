using System;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for Helper.
	/// </summary>
	public class ObjectHelper
	{
		private static string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\";
		public ObjectHelper()
		{
		}

		public static string Root
		{
			get
			{
				return root;
			}
		}

		public static string ImageRoot
		{
			get
			{
				return Root+"Images\\";
			}
		}

		public static string ObjectRoot
		{
			get
			{
				return Root+"Object\\";
			}
		}
		
		public static string GetObjectFile(string Symbol)
		{
			return ObjectRoot+Symbol+".xml";
		}

		public static string IconRoot
		{
			get
			{
				return Root+"Icon\\";
			}
		}

		public static string GetIconFile(string IconName)
		{
			if (IconName!=null)
				return IconRoot+IconName+".gif";
			else return "";
		}

		public static void CreateObjectPath()
		{
			Directory.CreateDirectory(ObjectRoot);
		}

		public static CultureInfo enUS = new CultureInfo("en-US");
	}
}
