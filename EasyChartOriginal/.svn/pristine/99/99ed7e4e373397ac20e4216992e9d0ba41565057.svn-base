using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;
using Microsoft.CSharp;
using System.Reflection;
using System.ComponentModel;

namespace Easychart.Finance
{
	/// <summary>
	/// Stock Formula program, Used by formula editor
	/// </summary>
	public class FormulaProgram
	{
		int ExtLine;
		int[] ExtColumn;
		int StartLineNum;
		int EndLineNum;

		#region Fields will be saved to xml file
		[XmlAttribute]
		public string Name;
		[XmlAttribute]
		public bool IsMainView;
		public string FullName = "";
		public string Description = "";
		public string Code;
		public DataCycle DefaultCycle;
		public DataCycleCollection DisabledCycle;
		public ParamCollection Params;
		#endregion

		public FormulaProgram()
		{
			Params = new ParamCollection();
			DisabledCycle = new DataCycleCollection();
		}

		private string GetAttrs(string s,out string Attrs) 
		{
			int j = 0;
			for(int i=0; i<s.Length; i++)
			{
				if (s[i]==',' && j == 0)
				{
					Attrs = s.Substring(i+1);
					return s.Substring(0,i);
				}
				else if (s[i]=='(') j++;
				else if (s[i]==')') j--;
			}
			Attrs = null;
			return s;
		}

		private string AppendAttrs(string s,string Var,string Attrs)
		{
			if (Attrs!=null)
				s +=Var.ToUpper()+".SetAttrs(\""+Attrs+"\");";
			return s;
		}

		private string ReplaceRef(string s) 
		{
			for(int Start=0; Start<s.Length; )
			{
				int i = s.IndexOf('"',Start);
				if (i>=0) 
				{
					int j = s.IndexOf('"',i+1);
					if (j>i) 
					{
						string r = s.Substring(i+1,j-i-1);
						r = "FML(DP,\""+r+"\")";
						s = s.Remove(i,j-i+1).Insert(i,r);
						Start = i+r.Length;
					}
				}
				else break;
			}
			return s;
		}

		public string GetParam(string Tab)
		{
			string s = "";
			string r = "";
			foreach(FormulaParam fp in Params) 
			{
				if (fp.ParamType==FormulaParamType.Double) 
					s +=Tab+"\tpublic double "+fp.Name.ToUpper()+"=0;\r\n";
				else 
					s +=Tab+"\tpublic string "+fp.Name.ToUpper()+"=\"\";\r\n";
				r +=Tab+"\t\tAddParam(\""+fp.Name+"\",\""+fp.DefaultValue+"\",\""+fp.MinValue+"\",\""+fp.MaxValue+"\",\""+fp.Description+"\",FormulaParamType."+fp.ParamType+");\r\n";
			}
			return s+
				Tab+"\tpublic "+Name+"():base()\r\n"+
				Tab+"\t{\r\n"+
				r+
				Tab+"\t}\r\n";
		}

		public string GetSource()
		{
			return GetSource("");
		}

		public bool InProgram(int Line)
		{
			return (Line>=StartLineNum && Line<=EndLineNum);
		}

		public string GetSource(string Tab,ref int StartLine) 
		{
			StartLineNum = StartLine;
			string s =GetSource(Tab,Code); //.ToUpper()
			for(int i=0; i<s.Length; i++)
				if (s[i]=='\r')
					StartLine++;
			EndLineNum = StartLine;
			return s;
		}

		public string GetSource(string Tab) 
		{
			return GetSource(Tab,Code); //.ToUpper()
		}

		private string ReplaceDollar(string s)
		{
			for(int i=0; i<s.Length; i++)
			{
				if (s[i]=='$')
				{
					string r;
					int j;
					for(j=i+1; j<s.Length; j++)
						if (!Char.IsLetterOrDigit(s,j) && s[j]!='_')
						{
							break;
						}
					r = s.Substring(i+1,j-i-1);
					s = s.Remove(i,j-i).Insert(i,"ORGDATA(\""+r+"\")");
				}
			}
			return s;
		}

		public string ToUpper(string s)
		{
			StringBuilder sb = new StringBuilder();
			int k = 0;
			for(int i=0; i<s.Length; i++)
			{
				if (s[i]=='\'' || s[i]=='"')
					k = 1-k;
				if (k==0)
					sb.Append(char.ToUpper(s[i]));
				else sb.Append(s[i]);
			}
			return sb.ToString();
		}

		public string GetSource(string Tab,string TheCode)
		{
			string s = TheCode;
			if (s.EndsWith(";"))
				s=s.Substring(0,s.Length-1);
			string[] ss = s.Split(';');
			ArrayList al = new ArrayList();
			Hashtable htFormula = new Hashtable();
			Hashtable htDouble = new Hashtable();

			int j;
			int k=0;
			string TypeDef;
			ExtLine = 5+4+Params.Count*2;
			ExtColumn = new int[ss.Length];

			for(int i=0; i<ss.Length; i++) 
			{
				ss[i] = ss[i].Trim();
				if (ss[i]!="") 
				{
					string Attrs;
					ss[i] = GetAttrs(ss[i],out Attrs);
					ss[i] = ReplaceRef(ss[i]);
					ss[i] = ReplaceDollar(ss[i].Replace("'","\""));
					string r = "";
					if (ss[i].StartsWith("@"))
					{
						ss[i] = ToUpper(ss[i].Substring(1))+";";;
						ExtColumn[i] = Tab.Length+1;
					}
					else if ((j = ss[i].IndexOf(":="))>0) 
					{
						r = ss[i].Substring(0,j).Trim();
						TypeDef = "";
						if (htFormula[r]==null)
							TypeDef = "FormulaData ";
						htFormula[r] = "1";
						ss[i] = TypeDef+r.ToUpper()+"="+ToUpper(ss[i].Substring(j+2))+"; "+r.ToUpper()+".Name=\""+r+"\";";
						ExtColumn[i] = (Tab.Length+2)*1+11;
					}
					else if ((j=ss[i].IndexOf(":"))>0)
					{
						r = ss[i].Substring(0,j).Trim();
						TypeDef = "";
						if (htFormula[r]==null)
							TypeDef = "FormulaData ";
						htFormula[r] = "1";
						ss[i] =TypeDef +r.ToUpper()+"="+ToUpper(ss[i].Substring(j+1))+ "; "+r.ToUpper()+".Name=\""+r+"\";";
						al.Add(r.ToUpper());
						ExtColumn[i] = (Tab.Length+2)*1+13;
					}
					else 
					{
						r = "NONAME"+k++;
						al.Add(r);
						ss[i] = "FormulaData "+r+"="+ToUpper(ss[i])+";";
						ExtColumn[i] = (Tab.Length+2)*1+14+r.Length;
					}
					if (r!="")
						ss[i] = AppendAttrs(ss[i],r,Attrs);
				}
			}
			
			return
				Tab+"public class "+Name+":FormulaBase\r\n"+
				Tab+"{\r\n"+
				GetParam(Tab)+
				Tab+"\r\n"+
				Tab+"	public override FormulaPackage Run(IDataProvider DP)\r\n"+
				Tab+"	{\r\n"+
				Tab+"		this.DataProvider = DP;\r\n"+
				Tab+"		"+string.Join("\r\n"+Tab+"\t\t",ss)+"\r\n"+
				Tab+"		return new FormulaPackage(new FormulaData[]{"+
				string.Join(",",(string[])al.ToArray(typeof(string)))+"},\"\");\r\n"+
				Tab+"	}\r\n"+
				Tab+"\r\n"+
				Tab+"	public override string LongName\r\n"+
				Tab+"	{\r\n"+
				Tab+"		get{return \""+FullName.Replace("\"","\\\"")+"\";}\r\n"+
				Tab+"	}\r\n"+
				Tab+"\r\n"+
				Tab+"	public override string Description\r\n"+
				Tab+"	{\r\n"+
				Tab+"		get{return \""+Description.Replace("\r","\\r").Replace("\n","\\n").Replace("\"","\\\"")+"\";}\r\n"+
				Tab+"	}\r\n"+
				Tab+"} //class "+Name+"\r\n";
		}

		public void AdjustErrors(System.CodeDom.Compiler.CompilerError ce) 
		{
			int j = ce.Line-ExtLine-StartLineNum;
			ce.Line = j;
			if (j<1 || j>ExtColumn.Length)
				j = 1;
			ce.Column -=ExtColumn[j-1];
		}
	
		public Assembly Compile(string FileName,string ReferenceRoot)
		{
			CompilerResults cr = FormulaSpace.Compile(
				"using Easychart.Finance;\r\n"+
				"using Easychart.Finance.DataProvider;\r\n"+
				GetSource(""),
				FileName,
				ReferenceRoot);
			StartLineNum = 2;

			if (cr.Errors.Count>0)
			{
				for(int i=0; i<cr.Errors.Count; i++)
				{
					System.CodeDom.Compiler.CompilerError ce  = cr.Errors[i];
					AdjustErrors(ce);
				}
				throw new FormulaErrorException(null,cr.Errors);
			}
			return cr.CompiledAssembly;
		}
	
		public Assembly Compile(string FileName)
		{
			return Compile(FileName,"");
		}

		public Assembly Compile()
		{
			return Compile("");
		}
	}

	public class ProgramCollection:CollectionBase
	{
		public FormulaProgram this[int index] 
		{
			get 
			{
				return (FormulaProgram)List[index];
			}
			set 
			{
				List[index] = value;
			}
		}

		public int Add(FormulaProgram value)
		{
			return List.Add(value);
		}

		public void Remove(FormulaProgram value)
		{
			List.Remove(value);
		}
	}

	public class FormulaErrorException:Exception 
	{
		public System.CodeDom.Compiler.CompilerErrorCollection ces;
		public FormulaSpace fms;
		
		public FormulaErrorException(FormulaSpace fms, System.CodeDom.Compiler.CompilerErrorCollection ces) 
		{
			this.fms = fms;
			this.ces = ces;
		}

		public string ToHtml()
		{
			string  s = "<font color=red>";
			foreach(System.CodeDom.Compiler.CompilerError ce in ces) 
			{
				if (fms!=null)
				{
					FormulaProgram fpa = fms.GetProgramByLineNum(ce.Line);
					if (fpa!=null)
					{
						fpa.AdjustErrors(ce);
						s += string.Format("Name:{0} line:{1} column:{2} error:{3} {4}<br>",fpa.Name,ce.Line,ce.Column,ce.ErrorNumber,ce.ErrorText);
					}
				}
			}
			return s+"</font>";
		}
	}
}