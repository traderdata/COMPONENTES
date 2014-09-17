using System;
using System.Drawing;
using System.Drawing.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Net;
using System.Text;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for Helper.
	/// </summary>
	public class FormulaHelper
	{
		static private string root;// = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\";
		static public WebProxy DownloadProxy;
		static public int WebTimeout;
		static private NumberFormatInfo nfi;

		static public NumberFormatInfo USFormat
		{
			get
			{
				if (nfi==null)
				{
					nfi = new NumberFormatInfo();
					nfi.NumberDecimalSeparator = ".";
					nfi.NumberGroupSeparator = ",";
				}
				return nfi;
			}
		}

		public FormulaHelper()
		{
		}

		public static string Root
		{
			get
			{
				if (root==null) 
				{
					root = Assembly.GetExecutingAssembly().CodeBase;
					if (root.StartsWith("file:///"))
						root = root.Substring(8).Replace("/","\\");
					root = root.Substring(0,root.Length-Path.GetFileName(root).Length);
				}
				return root;
			}
		}

		public static string DataFeedRoot
		{
			get
			{
				return root+"DataFeed\\";
			}
		}

		public static string SkinRoot
		{
			get
			{
				return Root+"Skin\\";
			}
		}

		public static string ImageRoot
		{
			get
			{
				return Root+"Images\\";
			}
		}

		public static string GetImageFile(string ImageName)
		{
			return ImageRoot+ImageName;
		}

		private static string objectRoot;
		public static string ObjectRoot
		{
			get
			{
				if (objectRoot==null || objectRoot=="")
					return Root+"Object\\";
				return objectRoot;
			}
			set
			{
				objectRoot = value;
			}
		}

		public static void CreateObjectRoot()
		{
			Directory.CreateDirectory(ObjectRoot);
		}
		
		public static string GetObjectFile(string Symbol)
		{
			if (Symbol.ToUpper().StartsWith("PRN"))
				Symbol ="_"+Symbol;
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

		static public string[] Split(string s)
		{
			return Split(s,',');
		}

		static public string[] Split(string s,char separator)
		{
			ArrayList al = new ArrayList();
			int k1 = 0;
			int k2 = 0;
			int j = 0;
			for(int i=0; i<s.Length; i++)
			{
				if (s[i]=='"')
					k1++;
				else if (s[i]=='(')
					k2++;
				else if (s[i]==')')
					k2--;
				else if (s[i]==separator && ((k1 % 2)==0) && (k2==0)) 
				{
					al.Add(s.Substring(j,i-j));
					j = i+1;
				}
			}
			if (s.Length>j)
				al.Add(s.Substring(j,s.Length-j));

			string[] ss = (string[])al.ToArray(typeof(string));
			return ss;
		}

		public static byte[] DownloadData(string URL)
		{
			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(URL);
			if (WebTimeout!=0)
				hwr.Timeout = WebTimeout;

			if (DownloadProxy!=null)
				hwr.Proxy = DownloadProxy;
			hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
			ArrayList al = new ArrayList();
			using (HttpWebResponse hws= (HttpWebResponse)hwr.GetResponse())
			{
				BinaryReader br = new BinaryReader(hws.GetResponseStream());
				byte[] bs;
				do 
				{
					bs = br.ReadBytes(100000);
					al.AddRange(bs);
				} while (bs.Length>0);
				return (byte[])al.ToArray(typeof(byte));
			}
		}

		public static string DownloadString(string URL)
		{
			byte[] bs = DownloadData(URL);
			return Encoding.UTF8.GetString(bs);
		}

		public static int TestBestFormat(double d,int Last)
		{
			if (d>10000)
				return Last;
			string[] rr = {"000",".000"};
			int k = 3;
			if (d>=1000) 
			{
				rr = new string[]{"0",".0"};
				k = 1;
			} 
			else if (d>=10)
			{
				rr = new string[]{"0",".0"};
				k = 1;
			}
			for(int j=Last; j<5; j++)
			{
				if (d.ToString("f"+j)+rr[j==0?1:0]==d.ToString("f"+(j+k))) 
					return j;
			}
			return 5;
		}

		static public int ToIntDef(string s, int Def)
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

		static public string FormatDouble(double d,string format)
		{
			if (format.StartsWith("Y")) 
			{
				int i = ToIntDef(format.Substring(1),4);
				int j = (int)d;
				d = j+(d-j)*32/100;
				NumberFormatInfo nfi = new NumberFormatInfo();
				nfi.NumberDecimalSeparator = ":";
				return d.ToString("f"+i,nfi);
			}
			else 
			{
				if (format .StartsWith("Z"))
					format = "f"+format.Substring(1);
				return d.ToString(format, NumberFormatInfo.InvariantInfo);
			}
		}


	}
}