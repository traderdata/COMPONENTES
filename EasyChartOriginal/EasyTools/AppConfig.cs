using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace EasyTools
{
	/// <summary>
	/// Summary description for Config.
	/// </summary>
	public class AppConfig
	{
		static NameValueCollection nv;
		static string FileName = Assembly.GetEntryAssembly().Location+".dat";
		static AppConfig()
		{
			nv = new NameValueCollection();
			Load();
		}

		static private void Load()
		{
			nv.Clear();
			if (File.Exists(FileName))
			using (StreamReader sr = File.OpenText(FileName))
			{
				while(true)
				{
					string r = sr.ReadLine();
					if (r==null || r=="") break;
					int i=r.IndexOf('=');
					if (i>=0) 
						nv[r.Substring(0,i)] = r.Substring(i+1);
				}
			}
		}

		static public void Save()
		{
			using (StreamWriter sw = new StreamWriter(FileName,false))
			{
				for(int i=0; i<nv.Count; i++)
					sw.WriteLine(nv.Keys[i]+"="+nv[nv.Keys[i]]);
			}
		}

		static public string Read(string Key,string Def)
		{
			string s = nv[Key];
			if (s==null) 
			{
				if (Def!=null && Def!="")
					Write(Key,Def);
				return Def;
			}
			return s.Trim();
		}

		static public string Read(string Key)
		{
			return Read(Key,"");
		}

		static public int Read(string Key,int Def)
		{
			try
			{
				return int.Parse(Read(Key,Def.ToString()));
			}
			catch
			{
			}
			return Def;
		}

		static public bool Read(string Key,bool Def)
		{
			try
			{
				return bool.Parse(Read(Key,Def.ToString()));
			} 
			catch
			{
			}
			return Def;
		}

		static public void Write(string Key,string Value)
		{
			nv[Key] = Value;
		}

		static public void Write(string Key,bool Value)
		{
			Write(Key,Value.ToString());
		}

		static public void Write(string Key,int Value)
		{
			Write(Key,Value.ToString());
		}
	}
}
