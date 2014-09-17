using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using System.Drawing;
using System.Globalization;
using Microsoft.CSharp;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance
{
	/// <summary>
	/// Formula render type
	/// </summary>
	public enum FormulaRenderType 
	{
		NORMAL,
		COLORSTICK,
		VOLSTICK,
		STICKLINE,
		ICON,
		TEXT,
		POLY,
		FILLRGN,
		FILLAREA,
		PARTLINE,
		LINE,
		VERTLINE,
		AXISY,
		AXISYTEXT,
		STOCK,
		MONOSTOCK,
	};

	/// <summary>
	/// Formula data type
	/// </summary>
	public enum FormulaType 
	{
		Const,
		Array
	};

	/// <summary>
	/// Formula line type
	/// </summary>
	public enum FormulaDot 
	{
		NORMAL,
		CROSSDOT,
		POINTDOT,
		CIRCLEDOT
	};

	public enum FormulaAlign 
	{
		Center=0,
		Right=1,
		Left=2,
	}

	public enum VerticalAlign
	{
		VCenter = 0,
		Top = 1,
		Bottom = 2,
		ScreenCenter =3,
		ScreenTop = 4,
		ScreenBottom = 5
	};


	/// <summary>
	/// Stock render type
	/// </summary>
	public enum StockRenderType 
	{
		HLCBars,
		OHLCBars,
		Line,
		Candle,
		Default,
	}

	/// <summary>
	/// Stick render type, include COLORSTICK and VOLSTICK
	/// </summary>
	public enum StickRenderType
	{
		Column,
		ThickColumn,
		SingleLine,
		ThickLine,
		Default,
		//FilledColumn,
	}

	/// <summary>
	/// Stock Formula base class, define all the basic Formula functions
	/// </summary>
	public class FormulaBase
	{
		//private static Hashtable Cache = new Hashtable();
		/// <summary>
		/// Formula parameter collection
		/// </summary>
		public ParamCollection Params;
		/// <summary>
		/// DataProvider
		/// </summary>
		public IDataProvider DataProvider;
		/// <summary>
		/// Formula Name
		/// </summary>
		public string Name;
		public string Quote;
		public int AxisYIndex;
		/// <summary>
		/// Show parameter in DisplayName
		/// </summary>
		public bool ShowParam = true;
		/// <summary>
		/// Determine if show text in the value label
		/// </summary>
		public bool TextInvisible;
		public ValueTextMode ValueTextMode = ValueTextMode.Default;

		public bool HideYGridLine;
		public double[] CustomLine;
		public double MinY = double.NaN;
		public double MaxY = double.NaN;

		protected const bool TRUE = true;
		protected const bool FALSE = false;
		protected const double NAN = double.NaN;

		public static bool Testing=false;
		public static int DefaultTestCount = 5;
		public static int MaxTestCount = 100;
		public static int ZigTestCount = 300;
		public static int DMATestCount = 200;
		public static int MaxForAllScan = 300;
		public static SortedList SupportedAssemblies = new SortedList();

		#region Constructor 
		static FormulaBase() 
		{
		}

		public FormulaBase()
		{
			Params = new ParamCollection();
			Name = GetType().Name;
		}

		public FormulaBase(IDataProvider DataProvider):this()
		{
			this.DataProvider = DataProvider;
		}
		#endregion

		#region Parameter
		private void SetParam(FormulaParam fp,string Value)
		{
			object o = Value;
			if (fp.ParamType==FormulaParamType.Double)
			{
				double d;
				try
				{
					d = double.Parse(Value,NumberFormatInfo.InvariantInfo);
				} 
				catch
				{
					try
					{
						d = double.Parse(fp.DefaultValue,NumberFormatInfo.InvariantInfo);
					} 
					catch
					{
						d = 0;
					}
				}
				if (d>double.Parse(fp.MaxValue,NumberFormatInfo.InvariantInfo)) Value = fp.MaxValue;
				if (d<double.Parse(fp.MinValue,NumberFormatInfo.InvariantInfo)) Value = fp.MinValue;
				o = d;
			} 
			fp.Value = Value;
			GetType().InvokeMember(fp.Name.ToUpper(), 
				BindingFlags.DeclaredOnly | 
				BindingFlags.Public | BindingFlags.NonPublic | 
				BindingFlags.Instance | BindingFlags.SetField
				, null, this, new object[]{o});
		}

		public string GetParam(string ParamName) 
		{
			FormulaParam fp = Params[ParamName];
			if (fp!=null)
				return fp.Value;
			else return "";
		}

		public void SetParam(string ParamName,string Value) 
		{
			foreach(FormulaParam fp in Params)
				if (string.Compare(fp.Name,ParamName,true)==0)
					SetParam(fp,Value);
		}

		public void SetParams(double[] P)
		{
			if (P!=null)
				for(int i=0; i<P.Length && i<Params.Count; i++) 
					SetParam(Params[i],P[i].ToString());
		}

		public void SetParams(string[] ss)
		{
			if (ss!=null)
				for(int i=0; i<ss.Length && i<Params.Count; i++) 
					SetParam(Params[i],ss[i].Trim('"'));
		}

		public void AddParam(string ParamName,string DefaultValue,string MinValue,string MaxValue,string Description,FormulaParamType ParamType) 
		{
			FormulaParam fp = new FormulaParam(ParamName,DefaultValue,MinValue,MaxValue,Description,ParamType);
			SetParam(fp,DefaultValue);
			Params.Add(fp);
		}

		public void AddParam(string ParamName,string DefaultValue,string MinValue,string MaxValue,FormulaParamType ParamType) 
		{
			AddParam(ParamName,DefaultValue,MinValue,MaxValue,"",ParamType);
		}

		public void AddParam(string ParamName,double DefaultValue,double MinValue,double MaxValue) 
		{
			AddParam(ParamName,DefaultValue.ToString(),MinValue.ToString(),MaxValue.ToString(),FormulaParamType.Double);
		}

		public void AddParam(string ParamName,string DefaultValue,string MinValue,string MaxValue)
		{
			AddParam(ParamName,DefaultValue,MinValue,MaxValue,FormulaParamType.String);
		}
		#endregion

		#region Reflection
		static public void RegAssembly(string Key,Assembly a) 
		{
			if (!SupportedAssemblies.ContainsValue(a)) 
			{
				SupportedAssemblies[Key] = a;
				ClearCache();
			}
		}

		static public void UnregAssembly(string Key) 
		{
			SupportedAssemblies.Remove(Key);
			ClearCache();
		}

		static public void UnregAllAssemblies() 
		{
			SupportedAssemblies.Clear();
			ClearCache();
		}

		static public string GetAssemblyKey(Assembly a) 
		{
			foreach(string s in SupportedAssemblies.Keys)
			{
				if (SupportedAssemblies[s] == a)
					return s;
			}
			return null;
		}

		static public MemberInfo[] GetAllMembers() 
		{
			Type t = typeof(FormulaBase);
			return t.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
		}

		static public string[] GetAllNativeMethods() 
		{
			MemberInfo[] mis = GetAllMembers();
			ArrayList al = new ArrayList();
			for(int i =0; i<mis.Length; i++) 
			{
				object[] os = mis[i].GetCustomAttributes(false);
				if (os.Length>0)
					al.Add(mis[i].Name);
			}
			return (string[])al.ToArray(typeof(string));
		}

		static public FormulaBase[] GetAllFormulas()
		{
			ArrayList al = new ArrayList();
			foreach(Assembly a in SupportedAssemblies.Values)
			{
				Type[] ts = a.GetTypes();
				foreach(Type t in ts)
				{
					if (t.IsSubclassOf(typeof(FormulaBase)))
					{
						FormulaBase fb = (FormulaBase)Activator.CreateInstance(t);
						al.Add(fb);
					}
				}
			}
			return (FormulaBase[])al.ToArray(typeof(FormulaBase));
		}

		static public string[] GetAllFormulasStrings()
		{
			FormulaBase[] fbs = GetAllFormulas();
			ArrayList al = new ArrayList();
			foreach(FormulaBase fb in fbs)
				al.Add(fb.TypeName);
			return (string[])al.ToArray(typeof(string));
		}

		static Hashtable htFormulaCache = new Hashtable();
		static public FormulaBase GetFormulaByName(Assembly a,string Name)
		{
			string CacheName = Name;
			FormulaBase fb = (FormulaBase)htFormulaCache[CacheName];
			if (fb!=null)
				return fb;

			int i1 = Name.IndexOf("(");
			int i2 = Name.LastIndexOf(")");
			string[] ss = null;
			if (i2>i1) 
			{
				string Param = Name.Substring(i1+1,i2-i1-1);
				Name = Name.Substring(0,i1)+Name.Substring(i2+1);
				ss = FormulaHelper.Split(Param);// Param.Split(',');
			}
			
			i1 = Name.IndexOf('[');
			i2 = Name.IndexOf(']');
			if (i2>i1)
				Name = Name.Substring(0,i1);

			if (!Name.StartsWith("FML"))
				Name = "FML."+Name.ToUpper();
			fb = (FormulaBase)a.CreateInstance(Name,true);
			if (fb!=null) 
			{
				fb.SetParams(ss);
				htFormulaCache[CacheName] = fb;
				return fb;
			}
			return null;
		}

		static public FormulaBase GetFormulaByName(string Name)
		{
			foreach(object o in SupportedAssemblies.Values)
			{
				FormulaBase fb = GetFormulaByName((Assembly)o,Name);
				if (fb!=null) return fb;
			}
			//throw new Exception("Undefined formula.("+Name+")");
			FormulaBase fbError = new FML.NATIVE.ERROR();
			fbError.SetParam("MSG","Undefined formula \""+Name+"\"");
			return fbError;
		}

		#endregion

		#region GetDataCount Needed
		public int DataCountAtLeast() 
		{
			IDataProvider idp = new RandomDataManager(false,10000)["MSFT"];
			Testing = true;
			try
			{
				FormulaPackage fp = Run(idp);
				if (fp.Count==0)
					return 0;
				FormulaData fd = fp[fp.Count-1];
				if (fd.Length==0)
					return 0;
				for(int i=0; i<fd.Length; i++)
				{
					if (!double.IsNaN(fd[i]))
						return i+1;
				};
				return fd.Length;
			}
			finally
			{
				DataProvider = null;
				Testing = false;
			}
		}

		public static FormulaData TestData(int N, params FormulaData[] fs) 
		{
			FormulaData.MakeSameLength(fs);
			FormulaData nf = new FormulaData(fs[0].Length);
			for(int i=0; i<fs[0].Length; i++) 
			{
				bool b = false;
				for(int j=0; j<fs.Length; j++) 
				{
					if (double.IsNaN(fs[j][i]))
					{
						b = true;
						break;
					}
				}
				if (b)
					nf[i] = double.NaN;
				else 
				{
					for(int k=i; k<fs[0].Length; k++) 
					{
						if (k<i+N)
							nf[k] = double.NaN;
						else nf[k] = 0;
					}
					break;
				}
			}
			return nf;
		}
		#endregion

		static public void ClearCache()
		{
			htFormulaCache.Clear();
		}

		/// <summary>
		/// Create a package of stock Formula data. Must be implemented by child classes
		/// </summary>
		/// <param name="dp"></param>
		/// <returns></returns>
		public virtual FormulaPackage Run(IDataProvider dp)
		{
			return null;
		}

		/// <summary>
		/// Create a package of stock Formula data by current Formula and stock data provider
		/// </summary>
		/// <param name="dp">Stock data provider</param>
		/// <param name="ss">Stock Formula parameter</param>
		/// <returns></returns>
		public FormulaPackage Run(IDataProvider dp,string[] ss)
		{
			SetParams(ss);
			return Run(dp);
		}

		public double[] AdjustDateTime(double[] D1,double[] D2,double[] Data)
		{
			if (D1==null || D2==null)
				return Data;
			//FormulaData nf = new FormulaData(D1.Length);
			double[] DR = new double[D1.Length];

			int i,j;
			for(i=0,j=0; i<D1.Length && j<D2.Length;) 
			{
				if (D1[i]<D2[j])
				{
					if (i>0)
						DR[i] = DR[i-1];
					else DR[i] = double.NaN;
					i++;
				}
				else if (D1[i]>D2[j])
				{
					if (i>0 && D1[i-1]<D2[j])
						DR[i-1] = Data[j];
					j++;
				}
				else 
				{
					DR[i] = Data[j];
					i++;
					j++;
				}
			}
			for(; i<D1.Length; i++) 
				if (i>0)
					DR[i] = DR[i-1];
				else DR[i] = double.NaN;
				//DR[i] = double.NaN;
			return DR;
		}

		public double[] AdjustDateTime(double[] Date,double[] Data)
		{
			return AdjustDateTime(DataProvider["DATE"],Date,Data);
		}

		/// <summary>
		/// Adjust the date time of current Formula data by another stock data provider
		/// </summary>
		/// <param name="BaseDate">Date array should be followed</param>
		/// <param name="dp">Another stock data provider</param>
		/// <param name="f">The stock Formula data</param>
		/// <param name="BaseDataCycle">Base data cycle</param>
		/// <returns>The adjusted stock Formula data</returns>
		public FormulaData AdjustDateTime(double[] BaseDate,IDataProvider dp,FormulaData f,DataCycle BaseDataCycle) 
		{
			if (DataProvider==null || f.FormulaType==FormulaType.Const || 
				(dp!=null && dp==DataProvider && dp.DataCycle.ToString()==DataProvider.DataCycle.ToString() && BaseDataCycle.ToString()==dp.DataCycle.ToString())
			)
				return f;
			else 
			{
				double[] D = dp["DATE"];
				double[] DR = AdjustDateTime(BaseDate,D,f.Data);
				FormulaData nf = new FormulaData(DR);
				if (f.SubData!=null)
				foreach(string Key in f.SubData.Keys)
					nf[Key] = AdjustDateTime(BaseDate,D,(double[])f.SubData[Key]);
				return nf;
			}
		}

		private bool BasicFormula(string s)
		{
			int i = s.IndexOf('#');
			if (i>0)
				s = s.Substring(0,i);
			s = s.ToUpper();
			return s=="C" || s=="O" || s=="H" || s=="L" || s=="V" || 
				s=="OPEN" || s=="CLOSE" || s=="HIGH" || s=="LOW" || 
				s=="VOL" || s=="VOLUME" || s=="DATE" || s=="STOCK";
		}

		public void BindFormulaCycle(IDataProvider dp,ref string FormulaName)
		{
			int i=FormulaName.IndexOf('#');
			if (i>0 && dp!=null) 
			{
				string s = FormulaName.Substring(i+1);
				dp.DataCycle = DataCycle.Parse(s);
				FormulaName = FormulaName.Substring(0,i);
			} 
			else 
			{
				if (DataProvider!=null && dp!=null)
					dp.DataCycle = DataProvider.DataCycle;
			}
		}

		/// <summary>
		/// Get stock Formula data from another stock Formula
		/// </summary>
		/// <param name="dp">Stock data provider</param>
		/// <param name="FormulaName">Another Formula name</param>
		/// <returns>Stock data array of the Formula</returns>
		public FormulaData GetFormulaData(IDataProvider dp,string FormulaName)
		{
			//			if (DataProvider!=null && dp!=null)
			//				dp.DataCycle = DataProvider.DataCycle;
			
			double[] BaseDate = null;
			DataCycle OldDataCycle = null;
			if (DataProvider!=null) 
			{
				BaseDate = DataProvider["DATE"];
				OldDataCycle = DataProvider.DataCycle;
			}
			try 
			{
				int i1 = FormulaName.IndexOf('(');
				int i2 = FormulaName.LastIndexOf(')');
				string[] P = {};
				if (i2>i1) 
				{
					string s= FormulaName.Substring(i1+1,i2-i1-1);
					P = FormulaHelper.Split(s);// s.Split(',');
					FormulaName = FormulaName.Substring(0,i1)+FormulaName.Substring(i2+1);
				}

				i1 = FormulaName.IndexOf('[');
				i2 = FormulaName.IndexOf(']');
				string F1 = FormulaName;
				string F2 = "";
				if (!BasicFormula(FormulaName)) 
				{
					if (i2>i1)
					{
						F1 = FormulaName.Substring(0,i1)+FormulaName.Substring(i2+1);
						F2 = FormulaName.Substring(i1+1,i2-i1-1);
					};
					//Assembly a = Assembly.GetExecutingAssembly();
					BindFormulaCycle(dp,ref F1);
					FormulaBase fb = GetFormulaByName("FML."+F1);
					if (fb.GetType()!= typeof(FML.NATIVE.ERROR))
					{
						//Map parent parameters
						for(int i=0; i<P.Length; i++)
						{
							FormulaParam fpThis =  Params[P[i]];
							if (fpThis!=null)
								P[i] = fpThis.Value;
						}

						FormulaPackage fp = fb.Run(dp,P);
						FormulaData fd;
						if (F2=="")
							fd = fp[fp.Count-1];
						else fd = fp[F2];

						if (object.Equals(fd,null))
							return double.NaN;
						else return AdjustDateTime(BaseDate,dp["DATE"],fd.Data);
					}
					else return double.NaN;
				} 
				else 
				{
					BindFormulaCycle(dp,ref FormulaName);
					FormulaBase Instance = this;
					if (this.DataProvider!=dp)
						Instance = new FormulaBase(dp);

					object o = null;
					if (FormulaName=="C" || FormulaName=="CLOSE")
						o = Instance.CLOSE;
					else if (FormulaName=="O" || FormulaName=="OPEN")
						o = Instance.OPEN;
					else if (FormulaName=="H" || FormulaName=="HIGH")
						o = Instance.HIGH;
					else if (FormulaName=="L" || FormulaName=="LOW")
						o = Instance.LOW;
					else if (FormulaName=="V" || FormulaName=="VOL" || FormulaName=="VOLUME")
						o = Instance.VOLUME;
					else if (FormulaName=="DATE")
						o = Instance.DATE;
					else if (FormulaName=="STOCK")
						o = Instance.STOCK;
					else 
					{
						Type t = typeof(FormulaBase);
						o = t.InvokeMember(
							FormulaName,
							BindingFlags.Public | 
							BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.GetProperty
							,null,Instance,null);
					}

					if (o is FormulaData)
						//return AdjustDateTime(BaseDate,dp["DATE"],(o as FormulaData).Data);  // This is good for multi time frame, "C#WEEK"; C;
						return AdjustDateTime(BaseDate,dp,o as FormulaData,OldDataCycle);// This is bad for multi time frame
					else return double.NaN;
				}
			}
			finally
			{
				if (DataProvider!=null)
					DataProvider.DataCycle = OldDataCycle;
			}
		}

		/// <summary>
		/// Define the text display format of this stock Formula
		/// </summary>
		public string DisplayName
		{
			get 
			{
				return ToString();
			}
		}

		/// <summary>
		/// Parameters separated by comma
		/// </summary>
		/// <returns></returns>
		public string GetParamString() 
		{
			string s = Params.ToString();
			if (Params.Count>0)
				s ="("+s+")";
			return s;
		}

		/// <summary>
		/// Formula's type name, did not include FML.
		/// </summary>
		public string TypeName
		{
			get
			{
				return GetType().ToString().Substring(4);
			}
		}

		/// <summary>
		/// Formula's full name, combination of Type and Parameters
		/// </summary>
		public string FullName 
		{
			get 
			{
				return GetType()+GetParamString();
			}
		}

		/// <summary>
		/// Formula name used to create the formula
		/// </summary>
		public string CreateName
		{
			get
			{
				return FullName.Substring(4)+(AxisYIndex==1?"!":"");
			}
		}

		/// <summary>
		/// Formula's long name, defined in the formula editor
		/// </summary>
		public virtual string LongName
		{
			get 
			{
				return "";
			}
		}

		/// <summary>
		/// Formula's description, defined in the formula editor
		/// </summary>
		public virtual string Description
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// The combination of formula name and formula's long name, e.g. RSI (Relative Strength Index)
		/// </summary>
		public string CombineName
		{
			get
			{
				string s = LongName;
				if (s!="")
					return Name+" ("+s+")";
				return Name;
			}
		}

		public override string ToString()
		{
			string s = Name;
			if (Quote!=null && Quote!="")
				s +="@"+Quote;

			if (ShowParam)
				return s+GetParamString();
			else return s;
		}

		public string FormulaName 
		{
			get 
			{
				string s = TypeName.ToUpper();// GetType().ToString().ToUpper();
				//if (s.StartsWith("FML."))
				//	s = s.Substring(4);
				return s+GetParamString();
			}
		}

		#region functions, should be started with @
		[Description("Set formula line name. should be start with @."),Category("1.Functions")]
		public void SETNAME(FormulaData f,string Name) 
		{
			f.Name = Name;
		}

		[Description("Set formula line name, should be start with @. \r\n ShowParam : show parameters in formula name"),Category("1.Functions")]
		public void SETNAME(string Name,bool ShowParam)
		{
			this.Name = Name;
			this.ShowParam= ShowParam;
		}

		[Description("Set formula name. should be start with @."),Category("1.Functions")]
		public void SETNAME(string Name)
		{
			SETNAME(Name,false);
		}

		[Description("Set formula line name visibility. should be start with @."),Category("1.Functions")]
		public void SETTEXTVISIBLE(FormulaData f,bool Visible) 
		{
			//f.TextInvisible = !Visible;
			f.ValueTextMode = Visible?ValueTextMode.Both:ValueTextMode.None;
		}

		[Description("Set formula value text mode, the posible value is None,TextOnly,ValueOnly,Both. should be start with @."),Category("1.Functions")]
		public void SETVALUETEXTMODE(string Mode)
		{
			try
			{
				ValueTextMode = (ValueTextMode)Enum.Parse(typeof(ValueTextMode),Mode,true);
			}
			catch
			{
			}
		}

		[Description("Set formula name visibility. should be start with @."),Category("1.Functions")]
		public void SETTEXTVISIBLE(bool Visible) 
		{
			TextInvisible = !Visible;
		}

		[Description("Set attribute of the formula. should be start with @."),Category("1.Functions")]
		public void SETATTR(FormulaData f,string s)
		{
			f.SetAttrs(s);
		}

		[Description("false if hide y-axis grid line. should be start with @."),Category("1.Functions")]
		public void SHOWYGRIDLINE(bool Visible)
		{
			HideYGridLine = !Visible;
		}

		[Description("Custom horizontal line, e.g. 30,50,70. should be start with @."),Category("1.Functions")]
		public void SETHLINE(params double[] ss)
		{
			CustomLine = ss;
		}

		[Description("Set min,max value of Y-axis, e.g. 0,100. should be start with @."),Category("1.Functions")]		
		public void SETYMINMAX(double MinY,double MaxY)
		{
			this.MinY = MinY;
			this.MaxY = MaxY;
		}
		#endregion

		#region OPEN/CLOSE/HIGH/LOW/VOLUME/AMOUNT , 2
		private FormulaData GetDataFromProvider(string DataName)
		{
			FormulaData fd = DataProvider[DataName];
			fd.Name = DataName;
			return fd;
		}

		/// <summary>
		/// Get the stock closeprice
		/// </summary>
		[Description("Get the stock close price"),Category("2.Basic Data")]
		public FormulaData CLOSE
		{
			get 
			{
				return GetDataFromProvider("CLOSE");
			}
		}
		
		/// <summary>
		/// Shortcut of CLOSE
		/// </summary>
		[Description("Shortcut of CLOSE"),Category("2.Basic Data")]
		public FormulaData C
		{
			get 
			{
				return CLOSE;
			}
		}

		/// <summary>
		/// Get the stock open price
		/// </summary>
		[Description("Get the stock open price"),Category("2.Basic Data")]
		public FormulaData OPEN 
		{
			get 
			{
				return GetDataFromProvider("OPEN");
			}
		}

		/// <summary>
		/// Shortcut of OPEN
		/// </summary>
		[Description("Shortcut of OPEN"),Category("2.Basic Data")]
		public FormulaData O
		{
			get 
			{
				return OPEN;
			}
		}

		/// <summary>
		/// Get the stock highest price
		/// </summary>
		[Description("Get the stock highest price"),Category("2.Basic Data")]
		public FormulaData HIGH
		{
			get 
			{
				return GetDataFromProvider("HIGH");
			}
		}

		/// <summary>
		/// Shortcut of HIGH
		/// </summary>
		[Description("Shortcut of HIGH"),Category("2.Basic Data")]
		public FormulaData H
		{
			get 
			{
				return HIGH;
			}
		}

		/// <summary>
		/// Get the stock lowest price
		/// </summary>
		[Description("Get the stock lowest price"),Category("2.Basic Data")]
		public FormulaData LOW
		{
			get 
			{
				return GetDataFromProvider("LOW");
			}
		}

		/// <summary>
		/// Shortcut of LOW
		/// </summary>
		[Description("Shortcut of LOW"),Category("2.Basic Data")]
		public FormulaData L
		{
			get 
			{
				return LOW;
			}
		}

		/// <summary>
		/// Get the stock VOLUME
		/// </summary>
		[Description("Get the stock VOLUME"),Category("2.Basic Data")]
		public FormulaData VOLUME
		{
			get 
			{
				return GetDataFromProvider("VOLUME");
			}
		}

		/// <summary>
		/// Shortcut of VOLUME
		/// </summary>
		[Description("Shortcut of VOLUME"),Category("2.Basic Data")]
		public FormulaData VOL
		{
			get 
			{
				return VOLUME;
			}
		}

		/// <summary>
		/// Shortcut of VOLUME
		/// </summary>
		[Description("Shortcut of VOLUME"),Category("2.Basic Data")]
		public FormulaData V
		{
			get 
			{
				return VOLUME;
			}
		}

		/// <summary>
		/// Get the stock amount
		/// </summary>
		[Description("Get the stock amount"),Category("2.Basic Data")]
		public FormulaData AMOUNT
		{
			get 
			{
				return GetDataFromProvider("AMOUNT");
			}
		}

		[Description("Get rising numbers of current exchange"),Category("2.Basic Data")]
		public FormulaData ADVANCE
		{
			get 
			{
				return GetDataFromProvider("ADVANCE");
			}
		}

		[Description("Get falling numbers of current exchange"),Category("2.Basic Data")]
		public FormulaData DECLINE
		{
			get 
			{
				return GetDataFromProvider("DECLINE");
			}
		}

		[Description("Get Nth ask price"),Category("2.Basic Data")]
		public FormulaData ASKPRICE(double N)
		{
			return GetDataFromProvider("ASKPRICE"+(int)N);
		}

		[Description("Get Nth ask volume"),Category("2.Basic Data")]
		public FormulaData ASKVOL(double N)
		{
			return GetDataFromProvider("ASKVOL"+(int)N);
		}

		[Description("Get Nth bid price"),Category("2.Basic Data")]
		public FormulaData BIDPRICE(double N)
		{
			return GetDataFromProvider("BIDPRICE"+(int)N);
		}

		[Description("Get Nth bid volume"),Category("2.Basic Data")]
		public FormulaData BIDVOL(double N)
		{
			return GetDataFromProvider("BIDVOL"+(int)N);
		}

		[Description("Get Nth buy volume"),Category("2.Basic Data")]
		public FormulaData BUYVOL(double N)
		{
			return GetDataFromProvider("BUYVOL"+(int)N);
		}

		[Description("Get Nth sell volume"),Category("2.Basic Data")]
		public FormulaData SELLVOL(double N)
		{
			return GetDataFromProvider("SELLVOL"+(int)N);
		}

		[Description("Reference data from data provider. For example : OrgData('CLOSE')"),Category("2.Basic Data")]
		public FormulaData ORGDATA(string DataName)
		{
			return GetDataFromProvider(DataName);
		}
		#endregion

		#region Other basic data
		[Description("Get Nth extra data, can be defined by data provider"),Category("2.Basic Data")]
		public FormulaData EXTDATA(double N)
		{
			return DataProvider["EXTDATA"+(int)N];
		}

		[Description("Is buy order,should be supported by data provider"),Category("2.Basic Data")]
		public FormulaData ISBUYORDER
		{
			get 
			{
				return DataProvider["ISBUYORDER"];
			}
		}

		[Description("Basic finance data , should be supported by data provider, FINANCE1...FINANCE100"),Category("3.Extra Basic Data")]
		public double FINANCE(double N)
		{
			return DataProvider.GetConstData("FINANCE"+N);
		}

		[Description("Volume unit 100 or 1000, should be supported by data provider"),Category("3.Extra Basic Data")]
		public double VOLUNIT
		{
			get 
			{
				return DataProvider.GetConstData("VOLUNIT");
			}
		}

		[Description("Capital of current stock, should be supported by data provider"),Category("3.Extra Basic Data")]
		public double CAPITAL
		{
			get 
			{
				return DataProvider.GetConstData("CAPITAL");
			}
		}

		[Description("Dynamic information, should be supported by data provider,DYNAINFO1..DYNAINFO100"),Category("3.Extra Basic Data")]
		public double DYNAINFO(double N) 
		{
			return DataProvider.GetConstData("DYNAINFO"+N);
		}

		[Description("Bars count of current data provider"),Category("3.Extra Basic Data")]
		public double DATACOUNT
		{
			get 
			{
				return DataProvider.Count;
			}
		}
		# endregion

		#region Date & time functions, 4
		[Description("Get date array, the format is Year*10000+Month*100+Day"),Category("4.Date & Time")]
		public FormulaData DATE
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = d.Year*10000+d.Month*100+d.Day;
				}
				return f;
			}
		}

		[Description("Get ole date array"),Category("4.Date & Time")]
		public FormulaData OLEDATE
		{
			get
			{
				return new FormulaData(DataProvider["DATE"]);
			}
		}

		[Description("Get double date array"),Category("4.Date & Time")]
		public FormulaData DOUBLEDATE
		{
			get
			{
				return OLEDATE;
			}
		}

		[Description("Return last double date"),Category("4.Date & Time")]
		public double LASTDOUBLEDATE
		{
			get
			{
				FormulaData dd = DOUBLEDATE;
				if (dd.Length>0)
					return (int)dd[dd.Length-1];
				return DateTime.Today.ToOADate();
			}
		}

		[Description("Get day array."),Category("4.Date & Time")]
		public FormulaData DAY
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = d.Day;
				}
				return f;
			}
		}

		[Description("Get hour array."),Category("4.Date & Time")]
		public FormulaData HOUR
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = d.Hour;
				}
				return f;
			}
		}

		[Description("Get minute array."),Category("4.Date & Time")]
		public FormulaData MINUTE
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = d.Minute;
				}
				return f;
			}
		}

		[Description("Get month array."),Category("4.Date & Time")]
		public FormulaData MONTH
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = d.Month;
				}
				return f;
			}
		}

		[Description("Get time array,the format is Hour*10000+Minute*100+Second"),Category("4.Date & Time")]
		public FormulaData TIME
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = d.Hour*10000+d.Minute*100+d.Second;
				}
				return f;
			}
		}

		[Description("Get week array,0..6, 0 means sunday,6 means saturday"),Category("4.Date & Time")]
		public FormulaData WEEK
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = (int)d.DayOfWeek;
				}
				return f;
			}
		}

		[Description("Get week array,0..6, 0 means sunday,6 means saturday"),Category("4.Date & Time")]
		public FormulaData WEEKDAY
		{
			get 
			{
				return WEEK;
			}
		}

		[Description("Get year array"),Category("4.Date & Time")]
		public FormulaData YEAR
		{
			get 
			{
				FormulaData f = new FormulaData(DataProvider["DATE"]);
				for(int i=0; i<f.Length; i++) 
				{
					DateTime d = DateTime.FromOADate(f.Data[i]);
					f.Data[i] = d.Year;
				}
				return f;
			}
		}

		[Description("return 1 if the trading date is near or equal to (D,T), D :Year*10000+Month*100+Day, T: Hour*10000+Minute*100+Second"),Category("4.Date & Time")]
		public FormulaData NEARESTTIME(double D, double T)
		{
			if (Testing) return TestData(1);
			bool EveryDay = D==0.0;
			int i1 = (int)D;
			int i2 = (int)T;

			int Year = i1/10000;
			if (Year<1) Year = 1;
			int Month = (i1 / 100) % 100;
			if (Month<1) Month = 1;
			if (Month>12) Month = 12;
			int Day = i1 % 100;
			if (Day<1) Day = 1;
			if (Day>31) Day = 31;

			double dt = 0;
			try
			{
				dt = new DateTime(Year,Month,Day ,i2/10000,(i2/100) % 100, i2 % 100).ToOADate();
			}
			catch
			{
			}

			FormulaData f = new FormulaData(DataProvider.Count);
			double[] Date = DataProvider["DATE"];
			f.Set(0);
			double Last = 0;
			bool Find = false;
			for(int i=0; i<Date.Length; i++) 
			{
				double d = Date[i];
				if (dt<1)
					d = d-(int)d;
				if (dt>=Last && dt<d) 
				{
					double d1 = dt - Last;
					double d2 = d - dt;
					if (i==0 || d1>d2)
						f.Data[i] = 1;
					else f.Data[i-1] = 1;
					Find = true;
					if (!EveryDay)
						break;
				}
				Last = d;
			}
			if (!Find && f.Length>0)
				f.Data[f.Length-1] = 1;
			return f;
		}
		
//		[Description("Get chinese calendar year array"),Category("Date & Time")]
//		public FormulaData LYEAR
//		{
//			get 
//			{
//				FormulaData f = new FormulaData(DataProvider["DATE"]);
//				for(int i=0; i<f.Length; i++) 
//				{
//					DateTime d = DateTime.FromOADate(f.Data[i]);
//					f.Data[i] = Chinese.Lunar(d).Year;
//				}
//				return f;
//			}
//		}
//
//		[Description("Get chinese calendar month array"),Category("Date & Time")]
//		public FormulaData LMONTH
//		{
//			get 
//			{
//				FormulaData f = new FormulaData(DataProvider["DATE"]);
//				for(int i=0; i<f.Length; i++) 
//				{
//					DateTime d = DateTime.FromOADate(f.Data[i]);
//					f.Data[i] = Chinese.Lunar(d).Month;
//				}
//				return f;
//			}
//		}
//
//		[Description("Get chinese calendar day array"),Category("Date & Time")]
//		public FormulaData LDAY
//		{
//			get 
//			{
//				FormulaData f = new FormulaData(DataProvider["DATE"]);
//				for(int i=0; i<f.Length; i++) 
//				{
//					DateTime d = DateTime.FromOADate(f.Data[i]);
//					f.Data[i] = Chinese.Lunar(d).Day;
//				}
//				return f;
//			}
//		}

		[Description("Days between the two data arrays"),Category("4.Date & Time")]
		public FormulaData DATEDIFF(FormulaData f1,FormulaData f2) 
		{
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++) 
			{
				TimeSpan ts = DateTime.FromOADate(f1.Data[i])-DateTime.FromOADate(f2.Data[i]);
				nf.Data[i] = ts.Days;
			}
			return nf;
		}

//		public FormulaData DATAPERIOD
//		{
//			get 
//			{
//				return new FormulaData(DataProvider["DATAPERIOD"]);
//			}
//		}
//
//		public FormulaData LSOLARTERMDATE(FormulaData f) 
//		{
//			FormulaData nf = new FormulaData(f.Length);
//			for(int i=0; i<f.Length; i++) 
//			{
//				DateTime d = DateTime.FromOADate(f.Data[i]);
//				f.Data[i] = d.Year;
//			}
//			return f;
//		}
//
//		public FormulaData LSOLARTERM(FormulaData f)
//		{
//			FormulaData nf = new FormulaData(f.Length);
//			for(int i=0; i<f.Length; i++) 
//			{
//				DateTime d = DateTime.FromOADate(f.Data[i]);
//
//				f.Data[i] = d.Year;
//			}
//			return f;
//		}
//
		#endregion
		
		#region Reference functions, 5
		[Description("Multiple from the first bar"),Category("5.Reference functions")]
		public static FormulaData MUL(FormulaData f) 
		{
			if (Testing) return TestData(MaxForAllScan,f);
			FormulaData nf = new FormulaData(f.Length);
			double d = 1;
			for(int i=0; i<f.Length; i++) 
			{
				if (!double.IsNaN(f.Data[i])) 
				{
					d *= f.Data[i];
					nf.Data[i] = d;
				}
			}
			return nf;
		}

		[Description("Sum last N days value"),Category("5.Reference functions")]
		public static FormulaData SUM(FormulaData f,double N) 
		{
			int NI = (int)N;
			if (Testing) return TestData(NI+1,f);
			FormulaData nf = new FormulaData(f.Length);
			double d = 0;
			for(int i=0; i<f.Length; i++) 
			{
				if (!double.IsNaN(f.Data[i])) 
				{
					d += f.Data[i];
					if (NI!=0) 
					{
						if ( i>NI && !double.IsNaN(f.Data[i-NI-1])) 
						{
							d -= f.Data[i-NI-1];
							nf.Data[i] = d;
						}
						else nf.Data[i] = double.NaN;
					} 
					else nf.Data[i] = d;
				} 
				else nf.Data[i] = double.NaN;
			}
			return nf;
		}

		[Description("Sum from the first bar"),Category("5.Reference functions")]
		public static FormulaData SUM(FormulaData f) 
		{
			return SUM(f,0);
		}

		[Description("True count of last N days"),Category("5.Reference functions")]
		public static FormulaData COUNT(FormulaData f,double N) 
		{
			return SUM(f>0,N);
		}

		[Description("Reference value of N days before"),Category("5.Reference functions")]
		public static FormulaData REF(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=f.Length+Math.Min(NI-1,-1); i>=Math.Max(0,NI) ;  i--) 
				nf.Data[i] = f.Data[i-NI];
			if (NI<0)
				for(int i=nf.Length-1; i>=nf.Length+NI; i--)
					nf.Data[i] = double.NaN;
			else if (NI<=nf.Length)
				for(int i=NI-1; i>=0; i--)
					nf.Data[i] = double.NaN;
			return nf;
		}

		[Description("Extend last value to last bar"),Category("5.Reference functions")]
		public static FormulaData EXTEND(FormulaData f)
		{
			FormulaData nf = new FormulaData(f);
			for(int i=f.Length-1; i>=0;  i--) 
				if (!double.IsNaN(f.Data[i])) 
				{
					for(int j=i+1; j<f.Length; j++)
						nf.Data[j] = nf.Data[i];
					break;
				}
			return nf;
		}

//		public static FormulaData REF(FormulaData f,double N,string Cycle)
//		{
//			DataCycle dc = DataCycle.Parse(Cycle);
//			FormulaData fdDate = DATE;
//		}

		[Description("N days moving average"),Category("5.Reference functions")]
		public static FormulaData MA(FormulaData f,double N) 
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			FormulaData nf = new FormulaData(f.Length);
			double d = 0;
			for(int i=0; i<f.Length; i++) 
			{
				if (!double.IsNaN(f.Data[i])) 
					d += f.Data[i];
				else nf.Data[i] = double.NaN;

				if (NI!=0 && i>=NI-1) 
				{
					if (!double.IsNaN(f.Data[i-NI+1]))
					{
						if (double.IsNaN(f.Data[i]))
							nf.Data[i] =double.NaN;
						else 
							nf.Data[i] = d/NI;
						d -= f.Data[i-NI+1];
					}
					else nf.Data[i] = double.NaN;
				} 
				else
				{
					if (NI==0)
						nf.Data[i] = d/(i+1);
					else nf.Data[i] = double.NaN;
				}
			}
			return nf;
		}

		[Description("N days exponential moving average"),Category("5.Reference functions")]
		public static FormulaData EMA(FormulaData f,double N)
		{
			if (Testing) return TestData((int)N+DMATestCount,f);
			return DMA(f,2/(N+1));
		}

		[Description("Simple moving average, equal to DMA(f,M/N), M<N"),Category("5.Reference functions")]
		public static FormulaData SMA(FormulaData f,double N,double M)
		{
			if (Testing) return TestData((int)N+DMATestCount,f);
			return DMA(f,M/N,(int)N);
		}

		[Description("Dynamic moving average, D[i] = D[i]*A+D[i-1]*(1-A),  0<A<1"),Category("5.Reference functions")]
		public static FormulaData DMA(FormulaData f,double A)
		{
			return DMA(f,A,0);
		}

		public static FormulaData DMA(FormulaData f,double A,int Start)
		{
			if (Testing) return TestData(Start+DMATestCount,f);
			FormulaData nf = new FormulaData(f.Length);
			double Sum = 0;
			for(int i=0; i<Start && i<f.Length; i++) 
			{
				Sum +=f[i];
				nf[i] = double.NaN;
			}
			if (Start>0 && Start<=f.Length)
				nf[Start-1] = Sum/Start;

			for(int i=Start; i<f.Length; i++) 
			{
				if (i>0)
				{
					if (!double.IsNaN(f.Data[i]))
					{
						if (!double.IsNaN(nf.Data[i-1]))
							nf.Data[i] = f.Data[i]*A+nf.Data[i-1]*(1-A);
						else nf.Data[i] = f.Data[i];
					}
					else nf.Data[i] = double.NaN;
				}
				else nf.Data[i] = f.Data[i];
			}
			return nf;
		}

		[Description("N days highest value"),Category("5.Reference functions")]
		public static FormulaData HHV(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) 
				if (NI==0)
					return TestData(MaxTestCount,f);
				else return TestData(NI,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<Math.Min(NI-1,f.Length); i++)
				nf.Data[i] = double.NaN;
			double M = double.MinValue;
			for(int i=Math.Max(NI-1,0); i<f.Length; i++) 
			{
				if (double.IsNaN(f.Data[i]))
					nf.Data[i] = double.NaN;
				else 
				{
					if (NI==0)
						M = Math.Max(M,f.Data[i]);
					else 
					{
						M = double.MinValue;
						for(int j=Math.Max(0,i-NI+1); j<=i; j++)
							M = Math.Max(M,f.Data[j]);
					}
					nf.Data[i] = M;
				}
			}
			return nf;
		}
		
		[Description("Highest value"),Category("5.Reference functions")]
		public static FormulaData HHV(FormulaData f)
		{
			return HHV(f,0);
		}

		[Description("N days lowest value"),Category("5.Reference functions")]
		public static FormulaData LLV(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<Math.Min(NI-1,f.Length); i++)
				nf.Data[i] = double.NaN;
			double M = double.MaxValue;
			for(int i=Math.Max(NI-1,0); i<f.Length; i++) 
			{
				if (double.IsNaN(f.Data[i]))
					nf.Data[i] = double.NaN;
				else 
				{
					if (N==0)
						M = Math.Min(M,f.Data[i]);
					else 
					{
						M = double.MaxValue;
						for(int j=Math.Max(0,i-NI+1); j<=i; j++)
							M = Math.Min(M,f.Data[j]);
					}
					nf.Data[i] = M;
				}
			}
			return nf;
		}

		[Description("Lowest value"),Category("5.Reference functions")]
		public static FormulaData LLV(FormulaData f)
		{
			return LLV(f,0);
		}

//		[Description("N days highest value"),Category("5.Reference functions")]
//		public static FormulaData HHV(FormulaData f,double N)
//		{
//			int NI = (int)N;
//			if (Testing) 
//				if (NI==0)
//					return TestData(MaxTestCount,f);
//				else return TestData(NI,f);
//			FormulaData nf = new FormulaData(f.Length);
//			double M = double.MinValue;
//			for(int i=0; i<f.Length; i++) 
//			{
//				if (NI==0)
//					M = Math.Max(M,f.Data[i]);
//				else 
//				{
//					M = double.MinValue;
//					for(int j=Math.Max(0,i-NI+1); j<=i; j++)
//						M = Math.Max(M,f.Data[j]);
//				}
//				nf.Data[i] = M;
//			}
//			return nf;
//		}
//
//		[Description("N days lowest value"),Category("5.Reference functions")]
//		public static FormulaData LLV(FormulaData f,double N)
//		{
//			int NI = (int)N;
//			if (Testing) return TestData(NI,f);
//			FormulaData nf = new FormulaData(f.Length);
//			double M = double.MaxValue;
//			for(int i=0; i<f.Length; i++) 
//			{
//				if (N==0)
//					M = Math.Min(M,f.Data[i]);
//				else 
//				{
//					M = double.MaxValue;
//					for(int j=Math.Max(0,i-NI+1); j<=i; j++)
//						M = Math.Min(M,f.Data[j]);
//				}
//				nf.Data[i] = M;
//			}
//			return nf;
//		}

		[Description("N days highest value bars count"),Category("5.Reference functions")]
		public static FormulaData HHVBARS(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			int LastMax = 0;
			double M = double.MinValue;
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++) 
			{
				if (NI==0)
				{
					if (f.Data[i]>=M)
					{
						LastMax = i;
						M = f.Data[i];
					}
				}
				else 
				{
					M = double.MinValue;
					for(int j=Math.Max(0,i-NI); j<=i; j++)
						if (f.Data[j]>=M)
						{
							LastMax = j;
							M = f.Data[j];
						}
				}
				nf.Data[i] = i-LastMax;
			}
			return nf;
		}

		[Description("N days lowest value bars count"),Category("5.Reference functions")]
		public static FormulaData LLVBARS(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			int LastMin = 0;
			double M = double.MaxValue;
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++) 
			{
				if (NI==0)
				{
					if (f.Data[i]<=M)
					{
						LastMin = i;
						M = f.Data[i];
					}
				}
				else 
				{
					M = double.MaxValue;
					for(int j=Math.Max(0,i-NI); j<=i; j++)
						if (f.Data[j]<=M)
						{
							LastMin = j;
							M = f.Data[j];
						}
				}
				nf.Data[i] = i-LastMin;
			}
			return nf;
		}
			
		[Description("Bars count"),Category("5.Reference functions")]
		public static FormulaData BARSCOUNT(FormulaData f) 
		{
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(DefaultTestCount,f);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = i;
			return nf;
		}

		[Description("Bars count when sum up to N"),Category("5.Reference functions")]
		public static FormulaData SUMBARS(FormulaData f,double N) 
		{
			if (Testing) return TestData(DefaultTestCount,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
			{
				double sum = 0;
				int j;
				for(j=i; j>=0; j--) 
				{
					sum +=f.Data[j];
					if (sum>=N)
						break;
				}
				nf.Data[i] = i-j+1;
			}
			return nf;
		}
		
		[Description("Bars count when value > 0 "),Category("5.Reference functions")]
		public static FormulaData BARSLAST(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(DefaultTestCount,f);
			int Last = -1;
			for(int i=0; i<f.Length; i++)
			{
				if (f.Data[i]!=0)
					Last = i;
				if (Last<0)
					nf.Data[i] = 0;
				else nf.Data[i] = i-Last;
			}
			return nf;
		}

		[Description("Bars count since first Cond>0"),Category("5.Reference functions")]
		public static FormulaData BARSSINCE(FormulaData Cond)
		{
			if (Testing) return TestData(DefaultTestCount,Cond);
			FormulaData nf = new FormulaData(Cond.Length);
			int Last = -1;
			for(int i=0; i<Cond.Length; i++)
			{
				if (!double.IsNaN(Cond.Data[i]) && Cond.Data[i]!=0 && Last<0)
					Last = i;
				if (Last<0)
					nf.Data[i] = 0;
				else nf.Data[i] = i-Last+1;
			}
			return nf;
		}
		
		[Description("Backset N bars to f when Cond>0"),Category("5.Reference functions")]
		public static FormulaData BACKSET(FormulaData Cond,FormulaData f, double N)
		{
			if (Testing) return TestData((int)N,Cond);
			FormulaData.MakeSameLength(Cond,f);

			FormulaData nf = new FormulaData(Cond.Length);
			nf.Set(double.NaN);
			for(int i=0; i<Cond.Length; i++)
			{
				if (Cond.Data[i]!=0)
				for(int j=0; j<N; j++)
					if ((i-j)>=0)
						nf.Data[i-j] = f[i-j];
			}
			return nf;
		}

		[Description("Backset N bars to 1 when Cond>0"),Category("5.Reference functions")]
		public static FormulaData BACKSET(FormulaData Cond,double N)
		{
			return BACKSET(Cond,1,N);
		}

		[Description("Set next N bars to 0 when Cond>0"),Category("5.Reference functions")]
		public static FormulaData FILTER(FormulaData Cond,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,Cond);
			FormulaData nf = new FormulaData(Cond.Length);
			for(int i=0; i<Cond.Length; i++)
			{
				nf.Data[i] = Cond.Data[i];
				if (Cond.Data[i]>0)
				{
					for(int j=0; j<N; j++)
						if (i<Cond.Length-1)
							nf.Data[++i] = 0;
				}
			}
			return nf;
		}

		[Description("f by the Percent between every two COND.ValueType,0:ShowValue,1:Up Percent, 2:Down Percent"),Category("5.Reference functions")]
		public static FormulaData TOVALUE(FormulaData Cond,FormulaData f,double Percent,double ValueType)
		{
			if (Testing) return TestData(ZigTestCount,f);
			FormulaData nf = new FormulaData(f.Length);
			nf.Set(double.NaN);
			double LastValue = double.NaN;
			int LastIndex = -1;
			for(int i=0; i<f.Length; i++) 
			{
				if (Cond[i]>0) 
				{
					if (!double.IsNaN(LastValue))
					{
						int k = (int)(LastIndex+(i-LastIndex)*Percent+.5);
						if (ValueType==0)
							nf[k] = f[LastIndex]+(f[i]-f[LastIndex])*Percent;
						else if (ValueType==1)
							nf[k] = f[i]/f[LastIndex];
						else if (ValueType==2)
							nf[k] = f[LastIndex]/f[i];
					}
					LastIndex = i;
					LastValue = f[i];
				}
			}
			return nf;
		}

		[Description("f by the Percent between every two COND."),Category("5.Reference functions")]
		public static FormulaData TOVALUE(FormulaData Cond,FormulaData f,double Percent)
		{
			return TOVALUE(Cond,f,Percent,0);
		}
		#endregion

		#region Logic functions, 6
		[Description("If f1>0 return f2 , otherwise return f3"),Category("6.Logic functions")]
		public static FormulaData IF(FormulaData f1,FormulaData f2,FormulaData f3)
		{
			FormulaData.MakeSameLength(f1,f2,f3);
			if (Testing) return TestData(0,f1,f2,f3);
			FormulaData nf = new FormulaData(f2.Data);

			bool NeedSubKey = false;
			if (f2.SubData!=null)
				foreach(string Key in f2.SubData.Keys)
				{
					nf[Key] = (double[])((double[])f2[Key]).Clone();
					NeedSubKey = true;
				}

			for(int i=0; i<f1.Length; i++)
			{
				if (f1.Data[i]==0)
				{
					nf.Data[i] = f3.Data[i];
					if (NeedSubKey)
						foreach(string Key in f2.SubData.Keys)
						{
							if (f3[Key]!=null)
								nf[Key][i] = f3[Key][i];
							else nf[Key][i] = f3.Data[i];
						}
						
				}
//				if (f1.Data[i]!=0)
//					nf.Data[i] = f2.Data[i];
//				else nf.Data[i] = f3.Data[i];
			}
			return nf;
		}

		[Description("If f3>f1>f2 or f2>f1>f3 return 1, otherwise return 0"),Category("6.Logic functions")]
		public static FormulaData BETWEEN(FormulaData f1,FormulaData f2,FormulaData f3)
		{
			FormulaData.MakeSameLength(f1,f2,f3);
			if (Testing) return TestData(0,f1,f2,f3);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
			{
				double R1 = f2.Data[i];
				double R2 = f3.Data[i];
				if (R1>R2) 
				{
					double R = R1;
					R2 = R1;
					R1 = R;
				}
				if (f1.Data[i]>=R1 && f1.Data[i]<=R2)
					nf.Data[i] = 1;
			}
			return nf;
		}

		[Description("f1 is near f2 in percent of P"),Category("6.Logic functions")]
		public static FormulaData NEAR(FormulaData f1,FormulaData f2,double P)
		{
			FormulaData.MakeSameLength(f1,f2);
			if (Testing) return TestData(0,f1,f2);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
				nf.Data[i] = Math.Abs(f1.Data[i]-f2.Data[i])/f2.Data[i]<P?1:0;
			return nf;
		}

		[Description("If f1 between f2 and f3 return 1, otherwise return 0"),Category("6.Logic functions")]
		public static FormulaData RANGE(FormulaData f1,FormulaData f2,FormulaData f3)
		{
			FormulaData.MakeSameLength(f1,f2,f3);
			if (Testing) return TestData(0,f1,f2,f3);
			FormulaData nf = new FormulaData(f1.Length);
			for(int i=0; i<f1.Length; i++)
			{
				if (f1.Data[i]>=f2.Data[i] && f1.Data[i]<=f3.Data[i])
					nf.Data[i] = 1;
			}
			return nf;
		}

		[Description("If f1 cross f2 from below return 1, otherwise return 0"),Category("6.Logic functions")]
		public static FormulaData CROSS(FormulaData f1,FormulaData f2) 
		{
			return LONGCROSS(f1,f2,1);
		}

		[Description("If f1 cross f2 from below and the cross last N bars return 1, otherwise return 0"),Category("6.Logic functions")]
		public static FormulaData LONGCROSS(FormulaData f1,FormulaData f2,double N) 
		{
			FormulaData.MakeSameLength(f1,f2);
			if (Testing) return TestData((int)N,f1,f2);
			FormulaData nf = new FormulaData(f1.Length);
			int Count = 0;

			for(int i=0; i<f1.Length; i++)
			{
				if (i==0)
					nf.Data[i] = 0;
				else 
				{
					nf.Data[i] = 0;	
					if (f1.Data[i]<f2.Data[i])
						Count++;
					else 
					{
						if (Count>=N) 
							nf.Data[i] = 1;
						Count = 0;
					}
				}
			}
			return nf;
		}

		[Description("If f1==0 return 1 , otherwise return 0"),Category("6.Logic functions")]
		public static FormulaData NOT(FormulaData f1)
		{
			return !f1;
		}

		[Description("Set last bar to 1 other bars to 0"),Category("6.Logic functions")]
		public FormulaData ISLASTBAR
		{
			get 
			{
				FormulaData f = CLOSE;
				FormulaData nf = new FormulaData(f.Length);
				for(int i=0; i<f.Length; i++)
					nf.Data[i] = (i==f.Length-1)?1:0;
				return nf;
			}
		}

		[Description("return 1 if current bar belongs to last day"),Category("6.Logic functions")]
		public FormulaData ISLASTDAY
		{
			get
			{
				FormulaData f = DATE;
				FormulaData nf = new FormulaData(f.Length);
				int N = f.Length;
				for(int i=0; i<N; i++)
					nf.Data[i] = ((int)f.Data[i])==((int)f.Data[N-1])?1:0;
				return nf;
			}
		}

		[Description("Set last bar has values to 1 other bars to 0"),Category("6.Logic functions")]
		public FormulaData ISLASTVALUE(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			nf.Set(0);
			for(int i=nf.Length-1 ; i>=0; i--) 
				if (!double.IsNaN(f[i])) 
				{
					nf.Data[i] = 1;
					break;
				}
			return nf;
		}

		[Description("If exist non-zero value in last N bars return 1 otherwise return 0"),Category("6.Logic functions")]
		public static FormulaData EXIST(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
			{
				double d = 0;
				for(int j=i; j>=Math.Max(0,i-NI+1); j--) 
				{
					if (f.Data[j]>0)
					{
						d = 1;
						break;
					}
				}
				nf.Data[i] = d;
			}
			return nf;
		}
			
		[Description("If exist zero value in last N bars return 0 otherwise return 1"),Category("6.Logic functions")]
		public static FormulaData EVERY(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
			{
				double d = 1;
				for(int j=i; j>=Math.Max(0,i-NI+1); j--) 
				{
					if (f.Data[j]==0)
					{
						d = 0;
						break;
					}
				}
				f.Data[i] = d;
			}
			return nf;
		}

		[Description("If exist zero value in last A bars to B bars return 0 otherwise return 1"),Category("6.Logic functions")]
		public static FormulaData LAST(FormulaData f,double A,double B)
		{
			int AI = (int)A;
			int BI = (int)B;
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(Math.Max(AI,BI),f);

			for(int i=0; i<f.Length; i++)
			{
				double d = 1;
				if (AI==0)
				{
					if (f.Data[i]==0)
						d=0;
				} 
				else 
					for(int j=Math.Max(0,i-AI+1); j<Math.Max(0,i-BI+1); j++)
					{
						if (f.Data[j]==0)
						{
							d = 0;
							break;
						}
					}
				f.Data[i] = d;
			}
			return nf;
		}
			
		[Description("If close value > open value return 1 otherwise return 0"),Category("6.Logic functions")]
		public FormulaData ISUP 
		{
			get 
			{
				return C>O;
			}
		}
		
		[Description("If close value < open value return 1 otherwise return 0"),Category("6.Logic functions")]
		public FormulaData ISDOWN
		{
			get 
			{
				return C<O;
			}
		}

		[Description("If close value = open value return 1 otherwise return 0"),Category("6.Logic functions")]
		public FormulaData ISEQUAL
		{
			get 
			{
				return C==O;
			}
		}
		#endregion

		#region Basic Math functions, 7
		public static double MAX(params double[] dd)
		{
			if (dd.Length>0)
			{
				double M = dd[0];
				for(int i=1; i<dd.Length; i++)
					M = Math.Max(M,dd[i]);
				return M;
			}
			return double.NaN;
		}

		public static double MIN(params double[] dd)
		{
			if (dd.Length>0)
			{
				double M = dd[0];
				for(int i=1; i<dd.Length; i++)
					M = Math.Min(M,dd[i]);
				return M;
			}
			return double.NaN;
		}

		[Description("Maxinum values"),Category("7.Basic Math functions")]
		public static FormulaData MAX(params FormulaData[] fds)
		{
			FormulaData.MakeSameLength(fds);
			if (Testing) return TestData(0,fds);
			FormulaData nf = new FormulaData(fds[0].Length);
			for(int i=0; i<fds[0].Length; i++) 
			{
				double d= fds[0][i];
				for(int j=1; j<fds.Length; j++)
					d = Math.Max(d,fds[j][i]);
						nf.Data[i] = d;
			}
			return nf;
		}

		[Description("Minimum values"),Category("7.Basic Math functions")]
		public static FormulaData MIN(params FormulaData[] fds)
		{
			FormulaData.MakeSameLength(fds);
			if (Testing) return TestData(0,fds);
			FormulaData nf = new FormulaData(fds[0].Length);
			for(int i=0; i<fds[0].Length; i++) 
			{
				double d= fds[0][i];
				for(int j=1; j<fds.Length; j++)
					d = Math.Min(d,fds[j][i]);
				nf.Data[i] = d;
			}
			return nf;
		}

		[Description("Absolate value, will return double."),Category("7.Basic Math functions")]
		public static double ABS(double d)
		{
			return Math.Abs(d);
		}

		[Description("Absolute values"),Category("7.Basic Math functions")]
		public static FormulaData ABS(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(0,f);
			for(int i=0; i<f.Length; i++) 
				nf.Data[i] = Math.Abs(f.Data[i]);
			nf.FormulaType = f.FormulaType;
			return nf;
		}

		[Description("The smallest whole number greater than or equal to f"),Category("7.Basic Math functions")]
		public static FormulaData FLOOR(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(0,f);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Floor(f.Data[i]);
			return nf;
		}

		[Description("The smallest whole number greater than or equal to f"),Category("7.Basic Math functions")]
		public static FormulaData CEILING(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(0,f);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Ceiling(f.Data[i]);
			return nf;
		}

		[Description("Returns the number nearest the specified value"),Category("7.Basic Math functions")]
		public static FormulaData ROUND(FormulaData f,int decimals)
		{
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(0,f);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Round(f.Data[i],decimals);
			return nf;
		}

		[Description("Returns the number nearest the specified value"),Category("7.Basic Math functions")]
		public static FormulaData ROUND(FormulaData f)
		{
			return ROUND(f,0);
		}

		[Description("The integer part of the value "),Category("7.Basic Math functions")]
		public static FormulaData INTPART(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			if (Testing) return TestData(0,f);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = (int)f.Data[i];
			return nf;
		}

		[Description("Equal to f1 % f2"),Category("7.Basic Math functions")]
		public static FormulaData MOD(FormulaData f1,FormulaData f2)
		{
			if (Testing) return TestData(0,f1,f2);
			return f1 % f2;
		}
			
		[Description("The sign of value"),Category("7.Basic Math functions")]
		public static FormulaData SGN(FormulaData f)
		{
			if (Testing) return TestData(0,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Sign(f.Data[i]);
			return nf;
		}

		[Description("Equal to -f"),Category("7.Basic Math functions")]
		public static FormulaData REVERSE(FormulaData f) 
		{
			if (Testing) return TestData(0,f);
			return -f;
		}
		#endregion

		#region Extra Math functions, 8
		[Description("Sin"),Category("8.Extra Math functions")]
		public static FormulaData SIN(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Sin(f.Data[i]);
			return nf;
		}
		
		[Description("Cos"),Category("8.Extra Math functions")]
		public static FormulaData COS(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Cos(f.Data[i]);
			return nf;
		}

		[Description("Tag"),Category("8.Extra Math functions")]
		public static FormulaData TAN(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Tan(f.Data[i]);
			return nf;
		}

		[Description("Arc Sin"),Category("8.Extra Math functions")]
		public static FormulaData ASIN(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Asin(f.Data[i]);
			return nf;
		}

		[Description("Arc Cos"),Category("8.Extra Math functions")]
		public static FormulaData ACOS(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Acos(f.Data[i]);
			return nf;
		}

		[Description("Arc Tan"),Category("8.Extra Math functions")]
		public static FormulaData ATAN(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Atan(f.Data[i]);
			return nf;
		}


		[Description("10 based Log"),Category("8.Extra Math functions")]
		public static FormulaData LOG10(FormulaData f)
		{
			if (Testing) return TestData(0,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Log10(f.Data[i]);
			return nf;
		}

		[Description("10 based Log"),Category("8.Extra Math functions")]
		public static FormulaData LOG(FormulaData f)
		{
			return LOG10(f);
		}

		[Description("N based Log"),Category("8.Extra Math functions")]
		public static FormulaData LOG(FormulaData f,double N)
		{
			if (Testing) return TestData(0,f);
			FormulaData nf = new FormulaData(f.Length);
			double  A = Math.Log(N);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Log(f.Data[i])/A;
			return nf;
		}
		
		[Description("e based Log"),Category("8.Extra Math functions")]
		public static FormulaData LN(FormulaData f)
		{
			if (Testing) return TestData(0,f);
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Log(f.Data[i]);
			return nf;
		}

		[Description("e raised to the specified power"),Category("8.Extra Math functions")]
		public static FormulaData EXP(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Exp(f.Data[i]);
			return nf;
		}

	   [Description("Square root of the value"),Category("8.Extra Math functions")]
		public static FormulaData SQRT(FormulaData f)
		{
		   if (Testing) return TestData(0,f);
		   FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Sqrt(f.Data[i]);
			return nf;
		}

		[Description("Return square root of d"),Category("8.Extra Math functions")]
		public static double SQRT(double d)
		{
			return Math.Sqrt(d);
		}

		[Description("Square of the value"),Category("8.Extra Math functions")]
		public static FormulaData SQR(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = f.Data[i]*f.Data[i];
			return nf;
		}

		[Description("N Power of the value"),Category("8.Extra Math functions")]
		public static FormulaData POWER(FormulaData f,double N)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=0; i<f.Length; i++)
				nf.Data[i] = Math.Pow(f.Data[i],N);
			return nf;
		}

		[Description("N Power of the value"),Category("8.Extra Math functions")]
		public static FormulaData POW(FormulaData f,double N)
		{
			return POWER(f,N);
		}
		#endregion

		#region Statistics functions, 9
		[Description("N days standard value"),Category("9.Statistics functions")]
		public static FormulaData STD(FormulaData f,double N)
		{
			return SQRT(VAR(f,N));
		}

		[Description("N days StdP value"),Category("9.Statistics functions")]
		public static FormulaData STDP(FormulaData f,double N)
		{
			return SQRT(VARP(f,N));
		}

		[Description("N days Variant value"),Category("9.Statistics functions")]
		public static FormulaData VAR(FormulaData f,double N)
		{
			if (Testing) return TestData((int)N,f);
			if (N==0)
				return f;
			else if (N==1)
				return VARP(f,N);
			else return VARP(f,N)*N/(N-1);
		}

		[Description("N days VarP value"),Category("9.Statistics functions")]
		public static FormulaData VARP(FormulaData f,double N)
		{
			if (Testing) return TestData((int)N,f);
			return MA(SQR(f),N)-SQR(MA(f,N));
		}

		[Description("N days average value"),Category("9.Statistics functions")]
		public static FormulaData AVEDEV(FormulaData f,double N)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			FormulaData nf = new FormulaData(f.Length);
			FormulaData A = MA(f,N);

			for(int i=0; i<NI-1 && i<nf.Length; i++)
				nf.Data[i] = double.NaN;
			for(int i=NI-1; i<f.Length; i++) 
			{
				double d = 0;
				for(int j=i-NI+1; j<=i; j++)
					d += Math.Abs(f.Data[j]-A.Data[i]);
				nf.Data[i] = d/NI;
			}
			return nf;
		}

		[Description("N days DevSQ value"),Category("9.Statistics functions")]
		public static FormulaData DEVSQ(FormulaData f,double N)
		{
			return VARP(f,N)*N;
		}

		public static void CalcLinearRegression(FormulaData f,int Start,int Count,out double a,out double b)
		{
			if (Start<Count-1) 
			{
				a = double.NaN;
				b = double.NaN;
				return;
			}
			double AVGX = ((double)Count-1)/2;
			double SUMXY = 0;
			double SUMX2 = 0;
			double AVGY = 0;
			for(int j=0; j<Count; j++)
			{
				int k = Start-Count+1+j;
				if (k>=f.Length)
					k = f.Length-1;
				SUMXY += j*f.Data[k];
				SUMX2 +=j*j;
				AVGY += f.Data[k];
			}
			AVGY /=Count;
			b = (SUMXY-Count*AVGX*AVGY)/(SUMX2-Count*AVGX*AVGX);
			a = (AVGY-b*AVGX);
		}

		// least-squares
		private static FormulaData CalLine(FormulaData f,double N,bool ForCast)
		{
			int NI = (int)N;
			if (Testing) return TestData(NI,f);
			if (NI>1) 
			{
				FormulaData nf = new FormulaData(f.Length);
				for(int i=NI-1; i<f.Length; i++) 
				{
					double b;
					double a;
					CalcLinearRegression(f,i,NI,out a,out b);
					if (ForCast)
						nf.Data[i] = a+b*(i-NI+1);
					else nf.Data[i] = b;
				}
				return nf;
			} 
			else throw new Exception("Invalid parameter");
		}

		/// <summary>
		/// Linear Regression
		/// </summary>
		/// <param name="f"></param>
		/// <param name="N"></param>
		/// <param name="Start"></param>
		/// <returns></returns>
		[Description("N days Linear Regression"),Category("9.Statistics functions")]
		public static FormulaData LR(FormulaData f,double N,double Start)
		{
			int NI = (int)N;
			int StartI = (int)Start;
			if (Testing) return TestData(NI+StartI,f);
			FormulaData nf = new FormulaData(f.Length);
			nf.Set(double.NaN);

			if (f.Length-NI-StartI>0)
			{
				double b;
				double a;
				CalcLinearRegression(f,f.Length-1-StartI,NI,out a,out b);
				int j = f.Length-NI-StartI;
				for(int i=j; i<f.Length-StartI; i++)
					nf.Data[i] = a+b*(i-j);
			}
			return nf;
		}

		[Description("Maximum value of f"),Category("9.Statistics functions")]
		public static double MAXVALUE(FormulaData f)
		{
			double M = double.MinValue;
			for(int i=0; i<f.Length; i++)
				if (M<f.Data[i])
					M = f.Data[i];
			return M;
		}

		[Description("Minimum value of f"),Category("9.Statistics functions")]
		public static double MINVALUE(FormulaData f)
		{
			double M = double.MaxValue;
			for(int i=0; i<f.Length; i++)
				if (M > f.Data[i])
					M = f.Data[i];
			return M;
		}

		/// <summary>
		/// Linear Regression
		/// </summary>
		/// <param name="f"></param>
		/// <param name="N"></param>
		/// <returns></returns>
		[Description("N days Linear Regression"),Category("9.Statistics functions")]
		public static FormulaData LR(FormulaData f,double N)
		{
			return LR(f,N,0);
		}

		[Description("N days linear regression fore cast"),Category("9.Statistics functions")]
		public static FormulaData FORCAST(FormulaData f,double N)
		{
			return CalLine(f,N,true);
		}

		[Description("N days linear regression slope"),Category("9.Statistics functions")]
		public static FormulaData SLOPE(FormulaData f,double N)
		{
			return CalLine(f,N,false);
		}

		[Description("N days correlation factor between f1 and f2"),Category("9.Statistics functions")]
		public static FormulaData CORR(FormulaData f1,FormulaData f2,double N)
		{
			int NI = (int)N;
			FormulaData.MakeSameLength(f1,f2);
			double _f1 = f1.Avg;
			double _f2 = f2.Avg;
			double d1 = 0;
			double d2 = 0;
			double d3 = 0;
			for(int i=0; i<f1.Length; i++) 
			{
				d1 +=(f1[i]-_f1)*(f2[i]-_f2);
				d2 +=(f1[i]-_f1)*(f1[i]-_f1);
				d3 +=(f2[i]-_f2)*(f2[i]-_f2);
			}
			return d1/Math.Sqrt(d2*d3);
		}
		#endregion

		#region Index function, A
		public FormulaData COST(double N) 
		{
			FormulaData NH = H;
			FormulaData NL = L;
			FormulaData NV = V;
			
			FormulaData nf = new FormulaData(H.Length);
			double SumV = 0;
			SortedList sl= new SortedList();
			for(int i=0; i<H.Length; i++) 
			{
				double m = NV.Data[i]/(NH.Data[i]-NL.Data[i]+1)/10;
				SumV +=NV.Data[i];
				for(double d=NL.Data[i]; d<=NH.Data[i]; d+=0.1) 
				{
					double d1 = Math.Round(d,1);
					if (sl[d1]==null)
						sl[d1] = m;
					else sl[d1] = (double)sl[d1]+m;
				}

				double Sum = 0;
				for(int j=0; j<sl.Count; j++) 
				{
					if (Sum/SumV>N/100) 
					{
						nf.Data[i] = (double)sl.GetKey(j);
						break;
					}
					Sum +=(double)sl.GetByIndex(j);
				}
			}
			return nf;
		}

		public FormulaData WINNER(FormulaData f)
		{
			FormulaData NH = H;
			FormulaData NL = L;
			FormulaData NV = V;
			FormulaData nf = new FormulaData(H.Length);
			double SumV = 0;
			SortedList sl= new SortedList();
			for(int i=0; i<H.Length; i++) 
			{
				double m = NV.Data[i]/(NH.Data[i]-NL.Data[i])/100;
				SumV +=NV.Data[i];
				for(double d=NL.Data[i]; d<=NH.Data[i]; d+=0.01) 
				{
					if (sl[d]==null)
						sl[d] = m;
					else sl[d] = (double)sl[d]+m;
				}

				double Sum = 0;
				for(int j=0; j<sl.Count; j++) 
				{
					if ((double)sl.GetByIndex(j)>f.Data[i]) 
					{
						nf.Data[i] = Sum/SumV;
						break;
					}
					Sum +=(double)sl.GetByIndex(j);
				}
			}
			return nf;
		}

		private FormulaData FillLinerValue(FormulaData f) 
		{
			FormulaData nf = new FormulaData(f);
			int Last = -1;
			for(int i=0; i<nf.Length; i++) 
			{
				if (!double.IsNaN(nf[i])) 
				{
					if (Last>=0) 
					{
						for(int j=Last+1; j<i; j++) 
							nf[j] = (nf[i]-nf[Last])/(i-Last)*(j-Last)+nf[Last];
					}
					Last = i;
				}
			}
			return nf;
		}

		public FormulaData OrgZig(FormulaData fMin,FormulaData fMax,double N)
		{
			if (Testing) return TestData(ZigTestCount,fMin);
//			string CacheKey = "ZIG("+fMin.Name+","+fMax.Name+","+N+")";
//			FormulaData nf = (FormulaData)Cache[CacheKey];
//			if (!object.Equals(nf,null)) return nf;

			FormulaData nf = new FormulaData(fMin.Length);
			for(int i=0; i<fMin.Length; i++)
			{
				if (i>0 && i<fMin.Length-1)
					nf.Data[i] = double.NaN;
				else nf.Data[i] = fMin.Data[i];
			}

			double Max = double.MinValue;
			double Min = double.MaxValue;
			int MaxI = -1;
			int MinI = -1;

			int Dir = 3;
			for(int i=0; i<fMin.Length; i++) 
				if (!double.IsNaN(fMin[i])  && !double.IsNaN(fMax[i]))
				{
					if (fMax.Data[i]>Max) 
					{
						Max = fMax.Data[i];
						MaxI = i;
					}
					if (fMin.Data[i]<Min)
					{
						Min = fMin.Data[i];
						MinI = i;
					}

					if (i>0 && (Dir & 1)!=0 && (i!=MinI) && (fMax.Data[i]/Min>(1+N/100) || i == fMin.Length-1))
					{
						Min = double.MaxValue;
						Max = double.MinValue;
						Dir = 2;
						nf.Data[MinI] = fMin.Data[MinI];
						if (i!=fMin.Length-1)
							i = MinI;
						else 
							nf.Data[i] = fMax.Data[i];
					}
					else if (i>0  && (Dir & 2)!=0 && (i!=MaxI) && (fMin.Data[i]/Max<(1-N/100) || i==fMax.Length-1))
					{
						Max = double.MinValue;
						Min = double.MaxValue;
						Dir = 1;
						nf.Data[MaxI] = fMax.Data[MaxI];
						if (i!=fMin.Length-1) 
							i = MaxI;
						else
							nf.Data[i] = fMin.Data[i];
					}
				}
			//Cache[CacheKey] = nf;
			return nf;
		}

		[Description("N days Zig Zag"),Category("A.Index functions")]
		public FormulaData ZIG(FormulaData fMin,FormulaData fMax,double N)
		{
			FormulaData nf = OrgZig(fMin,fMax,N);
			return FillLinerValue(nf);
		}

		[Description("N days Zig Zag"),Category("A.Index functions")]
		public FormulaData ZIG(FormulaData f,double N)
		{
			return ZIG(f,f,N);
		}

		[Description("N days Zig Zag"),Category("A.Index functions")]
		public FormulaData ZIG(double N)
		{
			return ZIG(LOW,HIGH,N);
		}

		[Description("N days Zig Zag percent value"),Category("A.Index functions")]
		public FormulaData ZIGP(FormulaData fMin,FormulaData fMax, double N)
		{
			FormulaData nf = OrgZig(fMin,fMax,N);
			if (nf.Length>1)
			{
				ArrayList A = new ArrayList();
				ArrayList B = new ArrayList();
				A.Add(nf[0]);
				B.Add(0);
				for(int i=1; i<fMin.Length-1; i++)
				{
					if (!double.IsNaN(nf[i])) 
					{
						A.Add(nf[i]);
						B.Add(i);
					}
				}
				A.Add(nf[nf.Length-1]);
				B.Add(nf.Length-1);

				FormulaData nff = new FormulaData(nf.Length);
				for(int i=1; i<A.Count-1; i++)
				{
					double a = (double)A[i] - (double)A[i-1];
					double b = (double)A[i] - (double)A[i+1];
					nff[((int)B[i-1]+(int)B[i+1]+1) / 2] = b/a;
				}
				return nff;
			}
			return nf;
		}
		
		[Description("N days Zig Zag percent value"),Category("A.Index functions")]
		public FormulaData ZIGP(FormulaData f, double N)
		{
			return ZIGP(f,f,N);
		}

		[Description("N days Zig Zag percent value"),Category("A.Index functions")]
		public FormulaData ZIGP( double N)
		{
			return ZIGP(LOW,HIGH,N);
		}

		/// <summary>
		/// This looks at all the data in the chart and evaluates when the Expr was true.
		/// Then starting from the most recent,it counts back Nth times and reports the value off.
		/// </summary>
		/// <param name="Expr"></param>
		/// <param name="f"></param>
		/// <param name="N"></param>
		/// <returns></returns>
		[Description("Count Expr was true N times and reports the value off"),Category("A.Index functions")]
		public FormulaData VALUEWHEN(FormulaData Expr,FormulaData f,double N) 
		{
			FormulaData.MakeSameLength(Expr,f);
			FormulaData nf = new FormulaData(f.Length);
			ArrayList al = new ArrayList();
			al.Add(f.Length);
			for(int i=f.Length-1; i>=0; i--) 
			{
				if (Expr[i]>0) 
				{
					al.Add(i);
					if (al.Count>N) 
					{
						int i1 = (int)al[al.Count-(int)N];
						int i2 = (int)al[al.Count-(int)N-1];
						for(int j=i1; j<i2; j++) 
							nf[j] = f[i];
					}
				}
			}
			return nf;
		}

		[Description("Count Expr was true N times and reports the bars count"),Category("A.Index functions")]
		public FormulaData VALUEWHENBARS(FormulaData Expr,double N) 
		{
			FormulaData nf = new FormulaData(Expr.Length);
			ArrayList al = new ArrayList();
			al.Add(Expr.Length);
			for(int i=Expr.Length-1; i>=0; i--) 
			{
				if (Expr[i]>0) 
				{
					al.Add(i);
					if (al.Count>N) 
					{
						int i1 = (int)al[al.Count-(int)N];
						int i2 = (int)al[al.Count-(int)N-1];
						for(int j=i1; j<i2; j++) 
							nf[j] = j-i;
					}
				}
			}
			return nf;
		}

		public FormulaData Num2FormulaData(double K) 
		{
			if (K==0)
				return O;
			else if (K==1)
				return H;
			else if (K==2)
				return L;
			else return C;
		}

		public FormulaData ZIG(double K,double N)
		{
			return ZIG(Num2FormulaData(K),N);
		}

		[Description("Find Peak"),Category("A.Index functions")]
		public FormulaData FINDPEAK(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=1; i<nf.Length-1; i++) 
			{
				if (f[i]>f[i+1] && f[i]>f[i-1])
					nf[i] = 1;
				else nf[i] = 0;
			}
			if (nf.Length>1) 
			{
				nf[0] = f[0]>f[1]?1:0;
				nf[nf.Length-1] = f[f.Length-1]>f[f.Length-2]?1:0;
			}
			return nf;
		}

		[Description("Find Peak based on Zig(N)"),Category("A.Index functions")]
		public FormulaData FINDPEAK(FormulaData fMin,FormulaData fMax,double N)
		{
			FormulaData fZig = ZIG(fMin,fMax,N);
			return FINDPEAK(fZig);
		}

		[Description("Find Peak based on Zig(N)"),Category("A.Index functions")]
		public FormulaData FINDPEAK(FormulaData f,double N)
		{
			return FINDPEAK(f,f,N);
		}

		[Description("Find Peak based on Zig(N)"),Category("A.Index functions")]
		public FormulaData FINDPEAK(double N)
		{
			return FINDPEAK(LOW,HIGH,N);
		}

		[Description("Find Trough"),Category("A.Index functions")]
		public FormulaData FINDTROUGH(FormulaData f)
		{
			FormulaData nf = new FormulaData(f.Length);
			for(int i=1; i<nf.Length-1; i++) 
			{
				if (f[i]<f[i+1] && f[i]<f[i-1])
					nf[i] = 1;
				else nf[i] = 0;
			}

			if (nf.Length>1) 
			{
				nf[0] = f[0]<f[1]?1:0;
				nf[nf.Length-1] = f[f.Length-1]<f[f.Length-2]?1:0;
			}
			return nf;
		}

		[Description("Find Trough based on Zig(N)"),Category("A.Index functions")]
		public FormulaData FINDTROUGH(FormulaData fMin,FormulaData fMax,double N)
		{
			FormulaData fZig = ZIG(fMin,fMax,N);
			return FINDTROUGH(fZig);
		}

		[Description("Find Trough based on Zig(N)"),Category("A.Index functions")]
		public FormulaData FINDTROUGH(FormulaData f,double N)
		{
			return FINDTROUGH(f,f,N);
		}

		[Description("Find Trough based on Zig(N)"),Category("A.Index functions")]
		public FormulaData FINDTROUGH(double N)
		{
			return FINDTROUGH(LOW,HIGH,N);
		}

		[Description("Mth Peak line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAK(FormulaData fMin,FormulaData fMax,double N,double M)
		{
			if (Testing) return TestData((int)N,fMin);
			FormulaData fPeak = FINDPEAK(fMin,fMax,N);
			if (fPeak.LASTDATA>0)
				M++;
			return VALUEWHEN(fPeak,fMax,M);
		}

		[Description("Mth Peak line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAK(FormulaData f,double N,double M)
		{
			return PEAK(f,f,N,M);
		}

		[Description("Peak line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAK(FormulaData f,double N)
		{
			return PEAK(f,N,1);
		}

		[Description("Peak line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAK(double N,double M)
		{
			return PEAK(LOW,HIGH,N,M);
		}

		[Description("Peak line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAK(double N)
		{
			return PEAK(N,(double)1);
		}

//		[Description("Mth Peak line based on Zig(N)"),Category("A.Index functions")]
//		public FormulaData PEAK(double K,double N,double M)
//		{
//			return PEAK(Num2FormulaData(K),N,M);
//		}

//		[Description("Peak line based on Zig(N)"),Category("A.Index functions")]
//		public FormulaData PEAK(double K,double N)
//		{
//			return PEAK(K,N,1);
//		}

		[Description("Mth Peak bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAKBARS(FormulaData fMin,FormulaData fMax,double N,double M)
		{
			if (Testing) return TestData((int)N,fMin);
			FormulaData fPeak = FINDPEAK(fMin,fMax,N);
			if (fPeak.LASTDATA>0)
				M++;
			return VALUEWHENBARS(fPeak,M);
		}

		[Description("Mth Peak bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAKBARS(FormulaData f,double N,double M)
		{
			return PEAKBARS(f,f,N,M);
		}

		[Description("Peak bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAKBARS(FormulaData f,double N)
		{
			return PEAKBARS(f,N,1);
		}

		[Description("Peak bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAKBARS(double N,double M)
		{
			return PEAKBARS(LOW,HIGH,N,M);
		}

		[Description("Peak bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAKBARS(double N)
		{
			return PEAKBARS(N,1);
		}

		[Description("Mth Peak bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData PEAKBARS(double K,double N,double M)
		{
			return PEAKBARS(Num2FormulaData(K),N,M);
		}

		[Description("Mth trough line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGH(FormulaData fMin,FormulaData fMax,double N,double M)
		{
			if (Testing) return TestData((int)N,fMin);
			FormulaData fTrough = FINDTROUGH(fMin,fMax,N);
			if (fTrough.LASTDATA>0)
				M++;
			return VALUEWHEN(fTrough,fMin,M);
		}

		[Description("Mth trough line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGH(FormulaData f,double N,double M)
		{
			return TROUGH(f,f,N,M);
		}

		[Description("Trough line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGH(FormulaData f,double N)
		{
			return TROUGH(f,f,N,1);
		}

		[Description("Mth trough line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGH(double N,double M)
		{
			return TROUGH(LOW,HIGH,N,M);
		}

		[Description("Trough line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGH(double N)
		{
			return TROUGH(N,(double)1);
		}
		
		[Description("Mth trough line based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGH(double K,double N,double M)
		{
			return TROUGH(Num2FormulaData(K),N,M);
		}

//		[Description("Trough line based on Zig(N)"),Category("A.Index functions")]
//		public FormulaData TROUGH(double K,double N)
//		{
//			return TROUGH(K,N,1);
//		}

		[Description("Mth trough bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGHBARS(FormulaData fMin,FormulaData fMax,double N,double M)
		{
			if (Testing) return TestData((int)N,fMin);
			FormulaData fTrough = FINDTROUGH(fMin,fMax,N);
			if (fTrough.LASTDATA>0)
				M++;
			return VALUEWHENBARS(fTrough,M);
		}

		[Description("Mth trough bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGHBARS(FormulaData f,double N,double M)
		{
			return TROUGHBARS(f,f,N,M);
		}

		[Description("Mth trough bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGHBARS(double N,double M)
		{
			return TROUGHBARS(LOW,HIGH,N,M);
		}

		[Description("Mth trough bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGHBARS(double N)
		{
			return TROUGHBARS(N,(double)1);
		}

		[Description("Mth trough bars based on Zig(N)"),Category("A.Index functions")]
		public FormulaData TROUGHBARS(double K,double N,double M)
		{
			return TROUGHBARS(Num2FormulaData(K),N,M);
		}

		public class SarHelper 
		{
			public double[] H;
			public double[] HH;
			public double[] L;
			public double[] LL;
			public bool Dir;
			public SarHelper(bool Dir,params FormulaData[] Datas) 
			{
				this.Dir = Dir;
				H = Datas[0].Data;
				HH = Datas[1].Data;
				L = Datas[2].Data;
				LL = Datas[3].Data;
			}
		}

		public FormulaData SAR(double N,double STEP,double MAXP,ref FormulaData TURNDATA)
		{
			if (N<1) N = 1;
			if (H.Length<N) return double.NaN;
			FormulaData HH = REF(HHV(H,N),1);
			FormulaData LL = REF(LLV(L,N),1);
			FormulaData nf = new FormulaData(HH.Length);
			STEP /=100;
			MAXP /=100;

			int Turn;
			SarHelper sh;
			if ((H[1]-H[0]) + (L[1]-L[0])>=0)
				Turn = 0;
			else Turn = 1;
			double AF = STEP;
			int Count = 0;
			SarHelper[] Sars = {new SarHelper(false,H,HH,L,LL),new SarHelper(true,L,LL,H,HH)};
		
			for(int i=0; i<N; i++)
				nf[i] = double.NaN;

			sh = Sars[Turn];
			for(int i=(int)N; i<sh.H.Length; i++)
			{
					if (Count==0)
						nf[i] = sh.LL[i];
					else 
					{
						if (sh.Dir ^ (sh.H[i]>sh.HH[i])) 
						{
							AF +=STEP;
							if (AF>MAXP)
								AF = MAXP;
						}
						nf[i] = nf[i-1]+AF*(sh.HH[i]-nf[i-1]);
					}
					if (sh.Dir ^ (nf[i]<sh.L[i])) 
						Count++;
					else
					{
						Turn = 1-Turn;
						sh = Sars[Turn];
						if (!object.Equals(TURNDATA,null)) 
							TURNDATA[i] = 1-Turn*2;
						Count = 0;
						AF = STEP;
					};
			}
			nf.Dot = FormulaDot.CIRCLEDOT;
			return nf;
		}

		[Description("Sar,N is the first bar to calculate sar"),Category("A.Index functions")]
		public FormulaData SAR(double N,double STEP,double MAXP)
		{
			if (Testing) return TestData(MaxTestCount,C);
			FormulaData nf = (FormulaData)null;
			return SAR(N,STEP,MAXP,ref nf);
		}

		[Description("Sar"),Category("A.Index functions")]
		public FormulaData SAR(double STEP,double MAXP)
		{
			return SAR(1,STEP,MAXP);
		}
		
		[Description("Sar turns, 0 ,1 or -1"),Category("A.Index functions")]
		public FormulaData SARTURN(double N,double STEP,double MAXP)
		{
			if (Testing) return TestData(MaxTestCount,C);
			FormulaData nf = new FormulaData(DataProvider.Count);
			SAR(N,STEP,MAXP,ref nf);
			return nf;
		}

		public FormulaData ATRSAR(double NAtr,double NEma, double Factor)
		{
			if (Testing) return TestData(ZigTestCount,C);
			FormulaData ARC = ATR(NAtr)*Factor;
			FormulaData HStop = H+ARC;
			FormulaData LStop = L-ARC;

			FormulaData f = EMA(C,NEma);
			FormulaData nf = new FormulaData(f.Length);

			if (f.Length>0)
			{
				bool b = false;
				double LastSar = 0;
				for(int i=0; i<ARC.Length; i++)
				{
					if (b)
					{
						if (LastSar<=f[i]) 
							nf[i] = Math.Max(LastSar,LStop[i]); 
						else  
						{ 
							b = false;
							nf[i] = HStop[i]; 
						} 
					}  
					else  
					{
						if (LastSar>=f[i])
							nf[i] = Math.Min(LastSar, HStop[i]);
						else
						{ 
							b = true;
							nf[i] = LStop[i];
						}
					}
					LastSar = nf[i];
				}
			}
			nf.Dot = FormulaDot.POINTDOT;
			return nf;
		}

		#endregion

		#region Chinese Yi Jing functions
		#endregion

		#region String functions, B
		[Description("Return current exchange, should be supported by data provider"),Category("B.String functions")]
		public string EXCHANGE
		{
			get 
			{
				return DataProvider.GetStringData("Exchange");
			}
		}

		[Description("Return current exchange, should be supported by data provider"),Category("B.String functions")]
		public string STKMARKET
		{
			get 
			{
				return EXCHANGE;
			}
		}

		[Description("Return Stock Symbol"),Category("B.String functions")]
		public string CODE
		{
			get 
			{
				return DataProvider.GetStringData("Code");
			}
		}

		[Description("Return Stock Symbol"),Category("B.String functions")]
		public string STKLABEL
		{
			get 
			{
				return CODE;
			}
		}

		[Description("Return Stock Symbol"),Category("B.String functions")]
		public string SYMBOL
		{
			get 
			{
				return CODE;
			}
		}

		[Description("Return Stock Name"),Category("B.String functions")]
		public string STOCKNAME
		{
			get 
			{
				return DataProvider.GetStringData("Name");
			}
		}

		[Description("Return Stock Name"),Category("B.String functions")]
		public string STKNAME
		{
			get 
			{
				return STOCKNAME;
			}
		}

//		public double STKINBLOCK(string s) 
//		{
//			return 1;
//		}

		[Description("Compare two strings"),Category("B.String functions")]
		public FormulaData STRCMP(string s1,string s2) 
		{
			if (Testing)
				return new FormulaData(0);
			return string.Compare(s1,s2);
		}

		[Description("Compare N chars of two strings"),Category("B.String functions")]
		public FormulaData STRNCMP(string s1,string s2,double N) 
		{
			if (Testing)
				return new FormulaData(0);
			return string.Compare(s1,0,s2,0,(int)N);
		}

		[Description("return the first occurrence of s2 in s1"),Category("B.String functions")]
		public FormulaData INDEXOF(string s1,string s2)
		{
			if (Testing)
				return new FormulaData(0);
			if (s1==null || s2==null)
				return 0;
			return s1.ToLower().IndexOf(s2.ToLower());
		}

		[Description("return 1 if s1 contains s2, otherwise return 0."),Category("B.String functions")]
		public FormulaData CONTAIN(string s1,string s2)
		{
			if (Testing)
				return new FormulaData(0);
			if (s1==null || s2==null)
				return 0;
			return s1.ToLower().IndexOf(s2.ToLower())>=0?1:0;
		}

		[Description("return s1 if Cond is true, otherwise return s2."),Category("B.String functions")]
		public string IF(FormulaData Cond,string s1,string s2)
		{
			if (Cond.Length>0 && Cond[Cond.Length-1]>0)
				return s1;
			return s2;
		}

		#endregion

		#region Draw functions, C

		[Description("Draw Text on f when Cond is True"),Category("C.Draw functions")]
		public FormulaData DRAWTEXT(FormulaData Cond,FormulaData f,string Text,params FormulaData[] fds)
		{
			FormulaData.MakeSameLength(Cond,f);
			FormulaData nf = new FormulaData(f);
			nf.RenderType = FormulaRenderType.TEXT;
			nf["COND"] = Cond.Data;
			nf["NUMBER"] = f.Data;
			nf.OwnerData["TEXT"] = Text;
			foreach(FormulaData fd in fds)
				nf["NUMBER"+fd.Name] = fd.Data;
			//nf.TextInvisible = true;
			nf.ValueTextMode = ValueTextMode.None;
			return nf;
		}

		[Description("Draw Text on f on Bar"),Category("C.Draw functions")]
		public FormulaData DRAWTEXT(double Bar,FormulaData f,string Text,params FormulaData[] fds)
		{
			FormulaData Cond = new FormulaData(C.Length);
			Cond[Cond.Length-(int)Bar-1] = 1;
			return DRAWTEXT(Cond,f,Text,fds);
		}

		public FormulaData DRAWAXISY(FormulaData f,double start,double end)
		{
			FormulaData nf = new FormulaData(f);
			nf.RenderType = FormulaRenderType.AXISY;
			nf.OwnerData["START"] = start;
			nf.OwnerData["END"] = end;
			return nf;
		}

		public FormulaData DRAWTEXTAXISY(FormulaData f,string Text,double start)
		{
			FormulaData nf = new FormulaData(f);
			nf.RenderType = FormulaRenderType.AXISYTEXT;
			nf.OwnerData["TEXT"] = Text;
			nf.OwnerData["START"] = start;
			//nf.TextInvisible = true;
			nf.ValueTextMode = ValueTextMode.None;
			return nf;
		}

		public FormulaData DRAWTEXTAXISY(FormulaData f,double Text,double start) 
		{
			return DRAWTEXTAXISY(f,Text.ToString(),start);
		}

		[Description("Draw Number on Price when Cond is True"),Category("C.Draw functions")]
		public FormulaData DRAWNUMBER(FormulaData Cond,FormulaData Price,FormulaData Number,string Format)
		{
			FormulaData.MakeSameLength(Cond,Price,Number);
			FormulaData nf = new FormulaData(Price);
			nf.RenderType = FormulaRenderType.TEXT;
			nf["COND"] = Cond.Data;
			nf["NUMBER"] = Number.Data;
			nf.OwnerData["FORMAT"] = Format;
			//nf.TextInvisible = true;
			nf.ValueTextMode = ValueTextMode.None;
			return nf;
		}

		[Description("Draw Image on f when Cond is True"),Category("C.Draw functions")]
		public FormulaData DRAWICON(FormulaData Cond,FormulaData Price,string ImageName)
		{
			FormulaData.MakeSameLength(Cond,Price);
			FormulaData nf = new FormulaData(Price);
			nf.RenderType = FormulaRenderType.ICON;
			nf["COND"] = Cond.Data;

			string s = FormulaHelper.GetImageFile(ImageName);
			if (File.Exists(s)) 
			{
				try
				{
					Image I = Bitmap.FromFile(s);
					nf.OwnerData["ICON"] = I;
				}
				catch
				{
				}
			}

			nf.ValueTextMode = ValueTextMode.None;
			//nf.TextInvisible = true;
			return nf;
		}

		[Description("Poly line,The point was defined by f when Cond is True"),Category("C.Draw functions")]
		public FormulaData POLYLINE(FormulaData Cond,FormulaData Price)
		{
			FormulaData.MakeSameLength(Cond,Price);
			FormulaData nf = new FormulaData(Price.Length);
			for(int i=0; i<Price.Length; i++)
				if (Cond[i]>0)
					nf[i] = Price[i];
				else nf[i] = double.NaN;
			return FillLinerValue(nf);
		}

		[Description("Draw line from Price when Cond is True to Price2 when Cond2 is True"),Category("C.Draw functions")]
		public FormulaData DRAWLINE(FormulaData Cond,FormulaData Price,FormulaData Cond2,FormulaData Price2,double Expand)
		{
			FormulaData.MakeSameLength(Cond,Price,Cond2,Price2);
			FormulaData nf = new FormulaData(Price);
			nf.RenderType = FormulaRenderType.LINE;
			nf["COND"] = Cond.Data;
			nf["COND2"] = Cond2.Data;
			nf["PRICE2"] = Price2.Data;
			nf.OwnerData["EXPAND"] = Expand;
			return nf;
		}

		[Description("Draw line from (Bar1,Price1) to (Bar2,Price2)"),Category("C.Draw functions")]
		public FormulaData DRAWLINE(double Bar1,double Price1,double Bar2,double Price2,double Expand)
		{
			FormulaData nf = new FormulaData(CLOSE.Length);
			nf.Set(double.NaN);
			nf[nf.Length-(int)Bar1-1] = Price1;
			nf[nf.Length-(int)Bar2-1] = Price2;
			if (Expand==1.0)
				nf[nf.Length-1] = Price2-(Price1-Price2)/(Bar1-Bar2)*Bar2;
			return FillLinerValue(nf);
		}

		[Description("Draw stick line from Price to Price2 when Cond is True"),Category("C.Draw functions")]
		public FormulaData STICKLINE(FormulaData Cond,FormulaData Price,FormulaData Price2,double Width,double Empty)
		{
			FormulaData.MakeSameLength(Cond,Price,Price2);
			FormulaData nf = new FormulaData(Price);
			nf.RenderType = FormulaRenderType.STICKLINE;
			nf["COND"] = Cond.Data;
			nf["PRICE2"] = Price2.Data;
			nf.OwnerData["WIDTH"] = Width;
			nf.OwnerData["EMPTY"] = Empty;
			return nf;
		}

		[Description("Draw stick line from Price to Price2 when Cond is True"),Category("C.Draw functions")]
		public FormulaData STICKLINE(FormulaData Cond,FormulaData Price,FormulaData Price2)
		{
			return STICKLINE(Cond,Price,Price2,0.9,0);
		}

		[Description("Fill region Price to Price2 when Cond is True"),Category("C.Draw functions")]
		public FormulaData FILLRGN(FormulaData Cond,FormulaData Price,FormulaData Price2)
		{
			FormulaData.MakeSameLength(C,Cond,Price,Price2);
			FormulaData nf = new FormulaData(Price);
			nf.RenderType = FormulaRenderType.FILLRGN;
			nf["COND"] = Cond.Data;
			nf["PRICE2"] = Price2.Data;
			//nf.TextInvisible = true;
			nf.ValueTextMode = ValueTextMode.None;
			return nf;
		}

		[Description("Fill area below f1"),Category("C.Draw functions")]
		public FormulaData FILLAREA(FormulaData f1) 
		{
			FormulaData nf = new FormulaData(f1);
			nf.RenderType = FormulaRenderType.FILLAREA;
			//nf.TextInvisible = true;
			nf.ValueTextMode = ValueTextMode.None;
			return nf;
		}

		[Description("Draw partial line"),Category("C.Draw functions")]
		public FormulaData PARTLINE(FormulaData Cond,FormulaData f)
		{
			FormulaData.MakeSameLength(Cond,f);
			FormulaData nf = new FormulaData(f);
			nf.RenderType = FormulaRenderType.PARTLINE;
			nf["COND"] = Cond.Data;
			//nf.TextInvisible = true;
			nf.ValueTextMode = ValueTextMode.None;
			return nf;
		}

		/// <summary>
		/// VERTLINE(DAY>10);
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		[Description("Draw verticle line"),Category("C.Draw functions")]
		public FormulaData VERTLINE(FormulaData f)
		{
			FormulaData nf = new FormulaData(f);
			nf.RenderType = FormulaRenderType.VERTLINE;
			return nf;
		}

		[Description("Draw stock bar based on fO,fC,fH,fL"),Category("C.Draw functions")]
		public FormulaData GETSTOCK(FormulaData fO,FormulaData fC,FormulaData fH,FormulaData fL)
		{
			FormulaData.MakeSameLength(fO,fC,fH,fL);
			FormulaData nf = new FormulaData(fC);
			nf.RenderType = FormulaRenderType.STOCK;
			nf["O"] = fO.Data;
			nf["H"] = fH.Data;
			nf["L"] = fL.Data;
			return nf;
		}

		[Description("Draw stock bar based on O,C,H,L"),Category("C.Draw functions")]
		public FormulaData STOCK
		{
			get 
			{
				return GETSTOCK(O,C,H,L);
			}
		}

		#endregion

		#region Advanced functions, D
		public FormulaData FML(IDataProvider dp,string FormulaName,string Cycle)
		{
			if (Cycle!=null && Cycle!="")
				FormulaName +="#"+Cycle;
			return GetFormulaData(dp,FormulaName);
		}

		public FormulaData FML(IDataProvider dp,string FormulaName)
		{
			string Cycle = null;
			if (FormulaName.IndexOf("#")<00)
				Cycle = DataProvider.DataCycle.ToString();
			return FML(dp,FormulaName,Cycle);
			//return GetFormulaData(dp,FormulaName);
		}

		[Description("Reference other formula with cycle. For example : FML('MSFT','MACD[DIFF]','WEEK2') "),Category("D.Advanced functions")]
		public FormulaData FML(string Symbol,string FormulaName,string Cycle)
		{
			if (DataProvider.DataManager==null)
				throw new Exception(Symbol+ " data not found!");
			if (Cycle==null || Cycle=="")
				return FML(DataProvider.DataManager[Symbol],FormulaName);
			else return FML(DataProvider.DataManager[Symbol],FormulaName,Cycle);
		}

		[Description("Reference other formula. For example : FML('MSFT','MACD[DIFF]')"),Category("D.Advanced functions")]
		public FormulaData FML(string Symbol,string FormulaName)
		{
			return FML(Symbol,FormulaName,"");
		}

//		public FormulaData FML(string Symbol,string FormulaName,int i,int j)
//		{
//			return FML(Symbol,FormulaName);
//		}

		[Description("Reference other formula. For example : FML('MACD[DIFF]')"),Category("D.Advanced functions")]
		public FormulaData FML(string FormulaName)
		{
			return FML(DataProvider,FormulaName);
		}

		static public int DateToBar(IDataProvider DataProvider,double d,int Dir,bool ExpandDate)
		{
			double[] dd = DataProvider["DATE"];
			if (dd.Length>0 && d>dd[dd.Length-1] && ExpandDate)
			{
				double LastDay = dd[dd.Length-1];
				int Bars = (int)(d-LastDay);
				DataCycleBase dcb = DataProvider.DataCycle.CycleBase;
				switch (dcb)
				{
					case DataCycleBase.WEEK:
						Bars /=7;
						break;
					case DataCycleBase.MONTH:
					case DataCycleBase.QUARTER:
					case DataCycleBase.YEAR:
						DateTime d1 = DateTime.FromOADate(LastDay);
						DateTime d2 = DateTime.FromOADate(d);
						if (dcb==DataCycleBase.YEAR)
							Bars = d2.Year-d1.Year;
						else 
						{
							Bars = d2.Year*12+d2.Month-d1.Year*12-d1.Month;
							if (dcb==DataCycleBase.QUARTER)
								Bars /=3;
						}
						break;
					default:
						int Left = Bars % 7;
						if ((int)(d-1) % 7 < (int)(LastDay-1) % 7)
							Left -=2;
						if (Left<0) Left = 0;
						Bars = (Bars / 7 * 5) + Left;
						break;
				}
				return dd.Length-1+Bars;
			}

			int i = 0;
			int j = dd.Length-1;
			while (i<j) 
			{
				int k=(i+j-Dir)/2;
				double delta = dd[k]-d;
				if (dd[k]<d-1e-7)
					i = k+Dir+1;
				else if (dd[k]>d+1e-7)
					j = k+Dir;
				else return k;
			}
			return i;
		}

		[Description("Convert string date to bar index,Date format is yyyy-MM-dd"),Category("D.Advanced functions")]
		public int DATE2BAR(string Date)
		{
			return DataProvider.Count-DateToBar(
				DataProvider,
				DateTime.Parse(Date,DateTimeFormatInfo.InvariantInfo).ToOADate(),
				0,
				true);
		}
		#endregion

		#region Indicator functions , E
		[Description("N days average true range"),Category("E.Indicator functions")]
		public FormulaData ATR(double N)
		{
			FormulaData LC =REF(CLOSE ,1);
			FormulaData TR = MAX(HIGH-LOW,ABS(LC-HIGH),ABS(LC-LOW));
			return SMA(TR,N,1);
		}
		#endregion

	}

	public class FormulaCollection:CollectionBase
	{
		public int Add(FormulaBase fb)
		{
			return List.Add(fb);
		}

		public int Add(string Name)
		{
			return Add(Name,"");
		}

		public int Add(string Name,string Quote)
		{
			if (Name!=null && Name.Trim()!="" && Name!="!")
			{
				FormulaBase fb= FormulaBase.GetFormulaByName(Name.Trim('!'));
				fb.AxisYIndex = Name.EndsWith("!")?1:0;
				fb.Quote = Quote;
				return Add(fb);
			}
			return -1;
		}

		public void Insert(int Index,FormulaBase fb)
		{
			this.List.Insert(Index,fb);
		}

		public void Insert(int Index,string Name)
		{
			if (Name!=null && Name.Trim()!="")
			{
				FormulaBase CurrentFormula = FormulaBase.GetFormulaByName(Name);
				Insert(Index,CurrentFormula);
			}
		}

		public FormulaBase this[int Index] 
		{
			get
			{
				return (FormulaBase)this.List[Index];
			}
		}

		public FormulaBase this[string Name] 
		{
			get
			{
				foreach(object o in List)
					if (((FormulaBase)o).GetType().ToString()==Name)
						return (FormulaBase)o;
				return null;
			}
		}

		public void Remove(FormulaBase fb) 
		{
			this.List.Remove(fb);
		}

		public void Remove(string Name) 
		{
			foreach(object o in List ) 
				if (((FormulaBase)o).GetType().ToString()== Name)
					Remove((FormulaBase)o);
		}
	}
}