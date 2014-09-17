using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Net;

namespace Easychart.Finance
{
	/// <summary>
	/// Stock formula plugin manager.
	/// Load formulas dlls at runtime.
	/// </summary>
	/// <example>
	/// PluginManager.Load(Environment.CurrentDirectory+"\\Plugins\\");
	/// PluginManager.OnPluginChanged +=new FileSystemEventHandler(OnPluginChange);
	/// </example>
	public class PluginManager
	{
#if (vs2005)
        static private Hashtable htAssembly = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
#else
        static private Hashtable htAssembly = new Hashtable(CaseInsensitiveHashCodeProvider.Default,CaseInsensitiveComparer.Default);
#endif

		static private Hashtable htShadow = new Hashtable();
		static private string PluginsDir;
		static private Hashtable htFormulaSpace = new Hashtable();

		static public event FileSystemEventHandler OnPluginChanged;

		static private string GetAssemblyHash(byte[] bs) 
		{
			int Sum = 0;
			for(int i=0; i<bs.Length; i++)
				Sum +=bs[i];
			return Sum.ToString();
		}

		static private byte[] GetByteFromFile(string FileName) 
		{
			if (File.Exists(FileName)) 
			{
				using (FileStream fs = File.OpenRead(FileName)) 
				{
					using (BinaryReader br = new BinaryReader(fs))
						return br.ReadBytes((int)fs.Length);
				}
			} 
			else return null;
		}

		static private byte[] GetByteFromWeb(string FileName)
		{
			WebClient wc = new WebClient();
			try
			{
				return wc.DownloadData(FileName);
			} 
			catch
			{
				return null;
			}
		}

		static private void LoadAssembly(string FileName)
		{
			byte[] bs;
			if (FileName.StartsWith("http"))
			{
				Assembly a = Assembly.LoadFrom(FileName);
				FormulaBase.RegAssembly(a.GetHashCode().ToString(),a);
			}
			else 
			{
				bs = GetByteFromFile(FileName);
				Assembly a = Assembly.Load(bs);
				htAssembly[FileName] = GetAssemblyHash(bs);
				FormulaBase.RegAssembly(htAssembly[FileName].ToString(),a);
			}
		}

		/// <summary>
		/// Register assembly
		/// </summary>
		/// <param name="a"></param>
		static public void RegAssembly(Assembly a)
		{
			FormulaBase.RegAssembly(a.GetHashCode().ToString(),a);
		}

		/// <summary>
		/// Unregister current assemblies,then register a assembly
		/// </summary>
		/// <param name="a"></param>
		static public void SetAssembly(string FileName)
		{
			if (File.Exists(FileName))
			{
				FormulaBase.UnregAllAssemblies();
				htAssembly.Clear();
				LoadAssembly(FileName);
			}
		}

		static public void RegExecutingAssembly()
		{
			RegAssembly(Assembly.GetCallingAssembly());
		}

		static public void RegAssemblyFromMemory()
		{
			Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly a in ass) 
				if (a.FullName.IndexOf("_fml")>=0)
					RegAssembly(a);
		}

		/// <summary>
		/// Load formulas from a dll file.
		/// And cache it in memory.
		/// </summary>
		/// <param name="FileName">Formula file name</param>
		/// <returns></returns>
		static public Assembly LoadShadowAssembly(string FileName)
		{
			byte[] bs = GetByteFromFile(FileName);
			string OldHash = (string)htAssembly[FileName];
			string NewHash = GetAssemblyHash(bs);
			Assembly a;
			if (OldHash==NewHash)
				a = (Assembly)htShadow[NewHash];
			else 
			{
				htAssembly[FileName] = NewHash;
				a = Assembly.Load(bs);
				htShadow[NewHash] = a;
			}
			return a;
		}

		static private void OnFileChange(object source, FileSystemEventArgs e)
		{
			try 
			{
				byte[] bs = GetByteFromFile(e.FullPath);
				if (bs.Length!=0 || e.ChangeType==WatcherChangeTypes.Deleted) 
				{
					string NewHash = GetAssemblyHash(bs);
					string OldHash = htAssembly[e.FullPath].ToString();
					if (OldHash != NewHash)
					{
						FormulaBase.UnregAssembly(OldHash);
						if (bs.Length>0) 
						{
							FormulaBase.RegAssembly(NewHash,Assembly.Load(bs));
							htAssembly[e.FullPath] = NewHash;
						}

						if (OnPluginChanged!=null)
							OnPluginChanged(null,e);
					}
				}
			}
			catch 
			{
			}
		}

		/// <summary>
		/// Load dlls from a certain path
		/// </summary>
		/// <param name="Path">Plugin path</param>
		public static void Load(string Path) 
		{
			PluginsDir = Path;
			if (Directory.Exists(PluginsDir))
			{
				FileSystemWatcher fsw = new FileSystemWatcher(PluginsDir,"*.dll");
				fsw.Created += new FileSystemEventHandler(OnFileChange);
				fsw.Changed += new FileSystemEventHandler(OnFileChange);
				fsw.Deleted += new FileSystemEventHandler(OnFileChange);
				foreach(string s in Directory.GetFiles(PluginsDir,"*.dll"))
					LoadAssembly(s);
				fsw.EnableRaisingEvents = true;
			}
		}

		public static void LoadFromWeb(string Path)
		{
			if (!Path.EndsWith("/")) Path +="/";
			Path +="Plugins/";
			PluginsDir = Path;
			LoadAssembly(Path+"Basic_fml.dll");
		}

		public static void LoadFromWeb()
		{
			string s = Assembly.GetExecutingAssembly().CodeBase;
			int i = s.LastIndexOf("/");
			if (i>0)
				s = s.Substring(0,i);
			LoadFromWeb(s);
		}

		/// <summary>
		/// Get formula source xml file of a certain formula.
		/// </summary>
		/// <param name="fb"></param>
		/// <returns></returns>
		public static string GetFormulaFile(FormulaBase fb) 
		{
			Assembly a =fb.GetType().Assembly;
			string Key = FormulaBase.GetAssemblyKey(a);
			if (Key!=null) 
			{
				foreach(string s in htAssembly.Keys) 
				{
					if (htAssembly[s].ToString()==Key)
						return s;
				}
			}
			return null;
		}
		
		public static string DllToFml(string Filename)
		{
			if (Filename!=null)
			{
				if (Filename.EndsWith("_fml.dll")) 
					return Filename.Substring(0,Filename.Length-8)+".fml";
			}
			return null;
		}

//		private static FormulaProgram GetFormulaProgram(string Filename,FormulaBase fb) 
//		{
//			if (Filename!=null)
//			{
//				FormulaSpace fs = (FormulaSpace)htFormulaSpace[Filename];
//				if (fs==null && File.Exists(Filename)) 
//				{
//					fs = FormulaSpace.Read(Filename);
//					htFormulaSpace[Filename] = fs;
//				}
//				if (fs!=null)
//					return fs.FindFormulaProgram(fb);
//			}
//			return null;
//		}
//
//		public static FormulaProgram GetFormulaProgram(FormulaBase fb) 
//		{
//			return GetFormulaProgram(DllToFml(GetFormulaFile(fb)),fb);
//		}

		private PluginManager()
		{
		}
	}
}
