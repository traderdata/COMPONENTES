using System;
using System.IO;
using System.Web;
using System.Globalization;

namespace EasyTools
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Tools
	{
		public Tools()
		{
		}

		static public string AppRoot 
		{
			get 
			{
				string s = HttpContext.Current.Request.ApplicationPath;
				if (s!="/")
					s += "/";
				return s;
			}
		}

		static public void Log(string Msg) 
		{
			string s=HttpRuntime.AppDomainAppPath+"log\\";
			if (!Directory.Exists(s))
				Directory.CreateDirectory(s);
			s +=DateTime.Today.ToString("yyyy-MM-dd")+".log";

			using (StreamWriter sw = File. AppendText(s))
				sw.WriteLine(DateTime.Now.ToString()+":"+Msg);
		}

		static public int ToIntDef(string s,int Def)
		{
			try
			{
				return int.Parse(s);
			}
			catch
			{
				return Def;
			}
		}

		static public int ToIntDef(object o,int Def) 
		{
			try
			{
				return int.Parse(o.ToString());
			} 
			catch
			{
				return Def;
			}
		}

		static public DateTime ToDateDef(string s, DateTime Def) 
		{
			try 
			{
				return DateTime.Parse(s);
			}
			catch
			{
				return Def;
			}
		}

		public static DateTime ToDateDef(string s,string[] Format,DateTime Def)
		{
			try 
			{
				return DateTime.ParseExact(s,Format,null,DateTimeStyles.None );
			}
			catch 
			{
				return Def;
			}
		}

		public static DateTime ToDateDef(string s,string Format,DateTime Def)
		{
			return ToDateDef(s,new string[]{Format},Def);
		}

		public static double ToDoubleDef(string s, double Def) 
		{
			try 
			{
				return double.Parse(s);
			}
			catch 
			{
				return Def;
			}
		}


		static public System.Web.UI.WebControls.Unit ToUnitDef(string s,string Def) 
		{
			try 
			{
				if (s!=null && s!="")
					return System.Web.UI.WebControls.Unit.Parse(s);
			} 
			catch 
			{
			}
			return System.Web.UI.WebControls.Unit.Parse(Def);
		}
	}
}
