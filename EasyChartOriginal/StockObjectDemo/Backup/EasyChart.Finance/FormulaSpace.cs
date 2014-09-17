using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web;
using Microsoft.CSharp;

namespace Easychart.Finance
{
	/// <summary>
	/// Financial Formula program namespace,used by formula editors
	/// </summary>
	[XmlRoot(IsNullable = false,ElementName="Namespace")]
	public class FormulaSpace
	{
		int LineNum;
		#region Fields will be saved to xml file
		[XmlAttribute]
		public string Name;
		[XmlAttribute]
		public string Version;
		public string Description;
		public bool GroupOnly;
		[XmlArrayItem(ElementName="Namespace")]
		public FormulaSpaceCollection Namespaces;
		[XmlArrayItem(ElementName="Program")]
		public ProgramCollection Programs;
		#endregion

		public FormulaSpace()
		{
			Namespaces = new FormulaSpaceCollection();
			Programs = new ProgramCollection();
		}

		public void AddVersion()
		{
			if (Version==null || Version=="")
				Version = "1.0.0.0";
			else 
			{
				string[] ss = Version.Split('.');
				if (ss.Length>0)
				{
					int i = 0;
					try
					{
						i = int.Parse(ss[ss.Length-1])+1;
					}
					catch
					{
					}
					ss[ss.Length-1] = i.ToString();
				}
				Version = string.Join(".",ss);
			}
		}

		public FormulaSpace(string Name):this()
		{
			this.Name = Name;
		}

		public void Write(TextWriter writer)
		{
			XmlSerializer serializer = 
				new XmlSerializer(typeof(FormulaSpace));
			serializer.Serialize(writer, this, 
				new XmlSerializerNamespaces(
				new XmlQualifiedName[]{new XmlQualifiedName("Formula","http://finance.easychart.net")}));
		}

		public void Write(string FileName) 
		{
			TextWriter writer = new StreamWriter(FileName);
			writer.NewLine = "\r\n";
			Write(writer);
			writer.Close();
		}

		public FormulaProgram FindFormulaProgram(string Path,FormulaBase fb) 
		{
			if (Path=="")  
				Path = Name;
			else 
			{
				if (!GroupOnly)
					Path +="."+Name;
			}

			foreach(FormulaProgram fp in Programs)
			{
				if (string.Compare(Path+"."+fp.Name,fb.GetType().ToString(),true)==0)
					return fp;
			}

			foreach(FormulaSpace fs in Namespaces) 
			{
				FormulaProgram fp = fs.FindFormulaProgram(Path, fb);
				if (fp!=null)
					return fp;
			}
			return null;
		}

		public FormulaProgram FindFormulaProgram(FormulaBase fb) 
		{
			return FindFormulaProgram("",fb);
		}

		public static FormulaSpace Read(TextReader reader) 
		{
			XmlSerializer serializer = 
				new XmlSerializer(typeof(FormulaSpace));
			//string s = reader.ReadToEnd();
			// compatible with old version
			//s = s.Replace("ParamType=\"string\"","ParamType=\"String\"").Replace("ParamType=\"double\"","ParamType=\"Double\"");
			//MemoryStream ms = new MemoryStream();
			FormulaSpace fs = (FormulaSpace)serializer.Deserialize(reader);
			return fs;
		}

		public static FormulaSpace Read(Stream s) 
		{
			TextReader reader = new StreamReader(s);
			return Read(reader);
		}

		public static FormulaSpace Read(string FileName) 
		{
			TextReader reader = new StreamReader(FileName);
			FormulaSpace fs = Read(reader);
			reader.Close();
			return fs;
		}

		public string GetSource(string Tab,ref int StartLine)
		{
			LineNum = StartLine;
			
			string s = Tab+"namespace "+Name+"\r\n";
			string NextTab = Tab;
			StartLine++;
			if (GroupOnly)
				s = Tab+"#region Formula Group "+Name+"\r\n";
			else 
			{
				s +=Tab+"{\r\n"; 
				NextTab +="\t";
				StartLine++;
			}

			if (Namespaces!=null) 
				foreach(FormulaSpace fs in Namespaces) 
				{
					s +=fs.GetSource(NextTab,ref StartLine)+"\r\n";
					StartLine++;
				}
			if (Programs!=null)
				foreach(FormulaProgram p in Programs)
				{
					s +=p.GetSource(NextTab,ref StartLine)+"\r\n";
					StartLine++;
				}
			
			if (GroupOnly)
				s +=Tab+"#endregion\r\n";
			else 
				s +=Tab+"} // namespace "+Name+"\r\n";
			StartLine++;
			return s;
		}

		public FormulaProgram GetProgramByLineNum(int Line) 
		{
			if (Namespaces!=null) 
				foreach(FormulaSpace fs in Namespaces) 
				{
					FormulaProgram fp = fs.GetProgramByLineNum(Line);
					if (fp!=null)
						return fp;
				}
			if (Programs!=null)
				foreach(FormulaProgram p in Programs)
				{
					if (p.InProgram(Line))
						return p;
				}
			return null;
		}

		public string GetSource(string Tab)
		{
			int StartLine=2;
			return GetSource(Tab,ref StartLine);
		}

		public string GetSource() 
		{
			return GetSource("");
		}

		public string CSharpSource(bool AddAssemblyVersion) 
		{
			string s = 
				"using Easychart.Finance;\r\n"+
				"using Easychart.Finance.DataProvider;\r\n";

			if (AddAssemblyVersion)
				s += "using System.Reflection;\r\n"+
					    "using System.Runtime.CompilerServices;\r\n"+
					    "[assembly: AssemblyVersion(\""+Version+"\")]\r\n";

			s +=GetSource();
			return s;
		}

		public void SaveCShartSource(string FileName) 
		{
			TextWriter writer = new StreamWriter(FileName);
			writer.Write(CSharpSource(true));
			writer.Close();
		}

		public static void ThrowCompileException(System.CodeDom.Compiler.CompilerErrorCollection ces) 
		{
			if (ces.Count > 0)
			{
				string msg = "CompilerError :\n";
				foreach(System.CodeDom.Compiler.CompilerError ce in ces )
					msg += String.Format( "line:{0} column:{1} error:{2} '{3}'\n", ce.Line, ce.Column,ce.ErrorNumber, ce.ErrorText);
				throw new InvalidProgramException(msg);
			}
		}

		private CompilerResults GetCompiledAssembly(string Filename,string ReferenceRoot) 
		{
			CompilerResults cr = Compile(CSharpSource(false),Filename,ReferenceRoot);
			System.CodeDom.Compiler.CompilerErrorCollection ces = cr.Errors;
			if (ces.Count > 0)
				throw new FormulaErrorException(this,ces);
			return cr;
		}

		public void Compile(string Filename)
		{
			Compile(Filename,"");
		}

		public void Compile(string Filename,string ReferenceRoot)
		{
			//SaveCShartSource(Path.ChangeExtension(Filename,".cs"));
			CompilerResults cr = GetCompiledAssembly(Filename,ReferenceRoot);
			cr = null;
		}

		public Assembly CompileInMemory(string ReferenceRoot) 
		{
			FormulaBase.ClearCache();
			CompilerResults cr = GetCompiledAssembly("",ReferenceRoot);
			return cr.CompiledAssembly;
		}

		public Assembly CompileInMemory() 
		{
			return CompileInMemory(HttpRuntime.BinDirectory);
		}

		static public CompilerResults Compile(string Code,string DestFileName,string ReferenceRoot)
		{
			CSharpCodeProvider cp = new  CSharpCodeProvider();
#if (!vs2005)
            ICodeCompiler ic = cp.CreateCompiler();
#endif
			CompilerParameters cpar = new CompilerParameters();
			cpar.GenerateExecutable = false;
			cpar.IncludeDebugInformation = false;
			
			if (DestFileName!=null && DestFileName!="")
				cpar.OutputAssembly = DestFileName;
			else cpar.GenerateInMemory = true;

			cpar.ReferencedAssemblies.Add(ReferenceRoot+"Easychart.Finance.dll");
#if (vs2005)
            CompilerResults cr =
                cp.CompileAssemblyFromSource(cpar, Code);
#else
			CompilerResults cr = 
				ic.CompileAssemblyFromSource(cpar,Code);
#endif
			return cr;
		}
	}

	public class FormulaSpaceCollection:CollectionBase
	{
		public FormulaSpace this[int index] 
		{
			get 
			{
				return (FormulaSpace)List[index];
			}
			set 
			{
				List[index] = value;
			}
		}

		public int Add(FormulaSpace value)
		{
			return List.Add(value);
		}

		public void Remove(FormulaSpace value)
		{
			List.Remove(value);
		}
	}
}
