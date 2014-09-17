using System;
using System.Configuration;

namespace WindowsDemo
{
	/// <summary>
	/// Summary description for Config.
	/// </summary>
	public class Config
	{
		public Config()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string FavoriteFormulas = ConfigurationSettings.AppSettings["FavoriteFormulas"];
		public static string DefaultFormulas = ConfigurationSettings.AppSettings["DefaultFormulas"];
	}
}
