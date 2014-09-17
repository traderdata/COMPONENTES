using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using EasyTools;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for CustomChart.
	/// </summary>
	public class CustomChart : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DropDownList ddlSize;
		protected System.Web.UI.WebControls.DropDownList ddlSkin;
		protected System.Web.UI.WebControls.Button btnOK;
		protected System.Web.UI.WebControls.TextBox tbCode;
		protected System.Web.UI.WebControls.RadioButtonList rblScale;
		protected System.Web.UI.WebControls.RadioButtonList rblType;
		protected System.Web.UI.WebControls.Button btnUpdate;
		protected System.Web.UI.HtmlControls.HtmlTableCell tdOverLay;
		protected System.Web.UI.HtmlControls.HtmlTableCell tdIndicator;
		protected System.Web.UI.WebControls.DropDownList ddlOver;
		protected System.Web.UI.WebControls.DropDownList ddlIndi;
		protected System.Web.UI.WebControls.Literal lOver;
		protected System.Web.UI.WebControls.Literal lIndicator;
		protected System.Web.UI.WebControls.CheckBox cbRealTime;
		protected System.Web.UI.WebControls.Literal lScript;
		protected System.Web.UI.WebControls.ImageButton ibChart;
		protected System.Web.UI.WebControls.Label lRedirect;
		protected System.Web.UI.WebControls.Literal lOLname;
		protected SelectDateRange SelectDateRange;

		private string sOL = "ol";
		protected System.Web.UI.WebControls.Literal lCookieIndi;
		private string sCookieIndi = "INDICATOR";
		protected System.Web.UI.WebControls.Literal lCookieOver;
		protected System.Web.UI.WebControls.DropDownList ddlSymbol;
		private string sCookieOver = "OVERLAY";
		protected System.Web.UI.WebControls.DropDownList ddlExchange;
		private Random Rnd = new Random();
	
		private void CreateLine(string s,string Type,DropDownList ddlTemplate,Control TheParent,bool UpDown,int Min) 
		{
			if (s!="" && s!="") s +=";";
			if (s!=null) 
			{
				string[] ss = s.Split(';');
				if (ss.Length<Min) 
				{
					string[] rr = ss;
					ss = new string[Min];
					for(int j=0; j<ss.Length; j++)
						ss[j] = "";
					Array.Copy(rr,0,ss,0,rr.Length);
				}
				for(int j=0; j<ss.Length; j++) 
				{
					string r = ss[j];
					DropDownList ddl = new DropDownList();
					if (UpDown)
					{
						ddl.ID = Type+j+"UpDown";
						ddl.Items.Add(new ListItem("Below","0"));
						ddl.Items.Add(new ListItem("Above","1"));
						int k = r.IndexOf("{U}");
						if (k>0) 
						{
							r = r.Substring(0,k);
							ddl.Items[1].Selected = true;
						}
						TheParent.Controls.Add(ddl);
						ddl = new DropDownList();
					}
					ddl.ID = Type+j;
					for(int i=0; i<ddlTemplate.Items.Count; i++) 
						ddl.Items.Add(new ListItem(ddlTemplate.Items[i].Text,ddlTemplate.Items[i].Value));
					ddl.Attributes["onChange"] = Type+"Change("+j+");";
					TheParent.Controls.Add(ddl);
					TextBox tb;
					int i1=r.IndexOf('(');
					int i2=r.LastIndexOf(')');
					string[] sss = {};
					if (i2>i1) 
						sss = r.Substring(i1+1,i2-i1-1).Split(',');
					if (i1>0)
						r = r.Substring(0,i1);
					ListItem li = ddl.Items.FindByValue(r);
					if (li!=null)
						li.Selected = true;
					for(int i=0; i<3; i++)
					{
						tb= new TextBox();
						tb.ID = ddl.ID+"_"+i;
						if (sss.Length>i)
							tb.Text = sss[i];
						tb.Width=Config.ParameterDefWidth;
						TheParent.Controls.Add(tb);
					}

					Literal l = new Literal();
					l.Text = "<br>";
					TheParent.Controls.Add(l);
				}
			}
		}

		private string GetParam(string Type) 
		{
			string Result = "";
			for(int i=0; i<20; i++) 
			{
				string Name =Type+i;
				string s=Request.Form[Name];
				if (s!=null && s!="") 
				{
					string Param = "";
					for(int j=0; j<3; j++)
					{
						string r = Request.Form[Name+"_"+j];
						if (r!=null && r!="") 
						{
							if (Param!="")
								Param +=",";
							Param +=r;
						}
					}
					if (Param!="")
						s +="("+Param+")";
					
					string UpDown = Request.Form[Name+"UpDown"];
					if (UpDown=="1")
						s +="{U}";
				}
				if (s!=null && s!="") 
				{
					if (Result!="")
						Result +=";";
					Result +=s;
				}
			}
			return Result;
		}

		private string GetOneParam(int Index,string Name) 
		{
			FormulaBase fb = null;
			if (Name!="") fb = FormulaBase.GetFormulaByName(Name);
			string s = "";
			for(int i=0; i<3; i++) 
			{
				string r = "";
				if (fb!=null)
					if (i<fb.Params.Count)
						r +=fb.Params[i].DefaultValue.ToString();

				if (s!="")
					s+=";";
				s +="p"+i+".value=\""+r+"\"";
			};
			return "if(i=="+Index+"){"+s+";}";
		}

		private string GetDefaultParam(string FuncName,DropDownList ddl)
		{
			
			string s = "function "+FuncName+"(i,p0,p1,p2) {\n";
			for(int i=0; i<ddl.Items.Count; i++)
				s +=GetOneParam(i,ddl.Items[i].Value)+"\n";
			//s +="p1.setfocus();\n";
			s +="}\n";
			return s;
		}

		private void SetScript() 
		{
			
			lScript.Text = 
					"<script language=\"javascript\">\n"+
					"<!--\n"+
					"function getElementIndex(f,element){\n"+
					"	var nElement=-1;\n"+
					"	for(i=0; i<f.elements.length; i++){\n"+
					"		if(f.elements[i]==element){\n"+
					"			nElement=i;\n"+
					"			break;\n"+
					"		}\n"+
					"	}\n"+
					"	return nElement;\n"+
					"}\n"+

					"function "+sOL+"Change(i)\n"+
					"{\n"+
					"	var f=document.forms['wfCustomChart'];\n"+ //document.wfCustomChart
					"	x=getElementIndex(f,eval(\"f."+sOL+"\"+i));\n"+
					"	setOverlayParms(f.elements[x].selectedIndex,f.elements[x+1],f.elements[x+2],f.elements[x+3]);\n"+
					"}\n"+
					"\n"+
					"function indiChange(i) \n"+
					"{\n"+
					"	var f=document.forms['wfCustomChart'];\n"+//document.wfCustomChart
					"	x=getElementIndex(f,eval(\"f.indi\"+i));\n"+
					"	setIndParms(f.elements[x].selectedIndex,f.elements[x+1],f.elements[x+2],f.elements[x+3]);\n"+
					"}\n"+
					GetDefaultParam("setOverlayParms",ddlOver)+
					GetDefaultParam("setIndParms",ddlIndi)+
					"// -->"+
					"</script>\n";
		}

		private string GetCookie(string Name) 
		{
			if (!Config.ConvertQueryToCookie && !IsPostBack) 
			{
				string s = Request.QueryString[Name];
				if (s!=null) return s;
			} 
			else 
			{
				HttpCookie hc = Request.Cookies[Name];
				if (hc!=null)
					return Server.UrlDecode(hc.Value);
			}
			return "";
		}

		private void SetValue(DropDownList ddl,string Value) 
		{
			ListItem li = ddl.Items.FindByValue(Value);
			if (li!=null) 
			{
				if (ddl.SelectedItem!=null)
					ddl.SelectedItem.Selected = false;
				li.Selected = true;
			}
		}

		private void SetValue(RadioButtonList rbl,string Value) 
		{
			ListItem li = rbl.Items.FindByValue(Value);
			if (li!=null) 
			{
				if (rbl.SelectedItem!=null)
					rbl.SelectedItem.Selected = false;
				li.Selected = true;
			}
		}
		
		private void SetCookie(string Name, string Value) 
		{
			string[] KnownTags = {Config.SymbolParameterName.ToUpper(),
					sCookieOver,	
					sCookieIndi,
					"SKIN",
					"SIZE",
					"TYPE",
					"SCALE",
					"RANGE",
					"RT",
					"START",
					"END",
					"CYCLE",
					"RTYPE",
					"SPAN",
					"EXCHANGE",
			};
			

			if (Array.IndexOf(KnownTags,Name.ToUpper())<0)
				return;
			HttpCookie hc = new HttpCookie(Name,Server.UrlEncode(Value));
			hc.Path = "/";
			hc.Expires = DateTime.Now.AddYears(10);
			Response.SetCookie(hc);
		}

		private void ConvertQueryToCookie()
		{
			if (Request.QueryString.Count>0 && Request.QueryString["R"]==null)
			{
				foreach(string s in Request.QueryString) 
					SetCookie(s,Request.QueryString[s]);
				Response.Redirect(Request.Path+"?R="+Rnd.Next(),true);
			}
		}

		bool HasExchange
		{
			get
			{
				return ddlExchange!=null && ddlExchange.Visible;
			}
		}

		bool ExchangePostBack
		{
			get
			{
				return HasExchange && Request.Form["__EVENTTARGET"]==ddlExchange.ID;
			}
		}

		string Code = null;
		private string PageCode
		{
			get
			{
				if (ddlSymbol!=null && ddlSymbol.Visible && (Request.Form["__EVENTTARGET"]==ddlSymbol.ID || tbCode==null))
					return ddlSymbol.SelectedValue;
				return tbCode.Text;
			}
			set
			{
				string s1 = value;
				string s2 = "";
				int i = value.IndexOf('.');
				if (i>=0 && HasExchange)
				{
					s1 = value.Substring(0,i);
					s2 = value.Substring(i+1);
				}

				if (ExchangePostBack)
					s1 = s2;

				if (ddlSymbol!=null && ddlSymbol.Visible)
				{
					try
					{
						ddlSymbol.SelectedValue = s1;
					} 
					catch
					{
					}
				}

				if (HasExchange)
				{
					try
					{
						ddlExchange.SelectedValue = s2;
					}
					catch
					{
					}
				}

				if (tbCode!=null)
					tbCode.Text = s1;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (lOLname!=null)
				sOL = lOLname.Text;
			if (lCookieIndi!=null)
				sCookieIndi = lCookieIndi.Text;
			if (lCookieOver!=null)
				sCookieOver = lCookieOver.Text;

			if (HasExchange && !IsPostBack)
				Utils.BindExchange(ddlExchange);

			SetScript();

			string Overlay = null;
			string Indicator = null;
			string Skin = null;
			string Size = null;
			string Type = null;
			string Scale = null;
			string Range = null;
			string RT = null;
			string Start = null;
			string End = null;
			string Cycle = null;
			string RType = null; //Range Type
			string His = Request.QueryString["His"];
			if (His!=null)
				His = "&His="+His;

			if (!IsPostBack) 
			{
				if (Config.ConvertQueryToCookie)
					ConvertQueryToCookie();
				else Code = Request.QueryString[Config.SymbolParameterName];
				
				if (Code==null || Code=="")
					Code = GetCookie(Config.SymbolParameterName);

				if (tdOverLay.Visible)
					Overlay = GetCookie(sCookieOver);
				if (tdIndicator.Visible)
					Indicator = GetCookie(sCookieIndi);
				Skin = GetCookie("Skin");
				Size = GetCookie("Size");
				Type = GetCookie("Type");
				Scale = GetCookie("Scale");
				Range = GetCookie("Range");
				RT = GetCookie("RT");
				Start = GetCookie("Start");  
				if (Start=="") 
				{
					string s = GetCookie("Span");
					Start = DateTime.Today.AddMonths(-Tools.ToIntDef(s,6)).ToString("yyyyMMdd");
				}
				//End = GetCookie("End"); 
				//if (End=="") 
					End = DateTime.Today.AddDays(1).ToString("yyyyMMdd");
				Cycle = GetCookie("Cycle");
				RType = GetCookie("RType");
			} 
			
			if ((Overlay==null || Overlay=="") && tdOverLay.Visible)	Overlay = GetParam(sOL);
			if ((Indicator==null || Indicator=="") && tdIndicator.Visible)	Indicator = GetParam("indi");
			if (Skin==null || Skin=="")	Skin = ddlSkin.SelectedItem.Value;
			if (Size==null || Size=="")	Size = ddlSize.SelectedItem.Value;
			if (Type==null || Type=="")	Type = rblType.SelectedItem.Value;
			if (Scale==null || Scale=="")	Scale =rblScale.SelectedItem.Value;
			if (Range==null || Range=="")	Range = SelectDateRange.rblRange.SelectedItem.Value;
			if (RT==null || RT=="")	RT =(cbRealTime.Checked?1:0).ToString();
			if (Start==null || Start=="")	Start = SelectDateRange.Start;
			if (End==null || End=="")	End = SelectDateRange.End;
			if (Cycle==null || Cycle=="")	Cycle = SelectDateRange.Cycle;
			if (RType==null || RType=="")	RType = SelectDateRange.RType;

			if (Code==null || Code=="")
				Code =  PageCode; //tbCode.Text;
			else PageCode = Code; //tbCode.Text 
			Code = Code.Trim();

			if (ddlSymbol!=null && ddlSymbol.Visible && (!IsPostBack || ExchangePostBack))
			{
				DataManagerBase dmb = Utils.GetDefaultDataManager();

				string Exchange = null;
				if (HasExchange)
					Exchange = ddlExchange.SelectedValue;
				
				if (ExchangePostBack)
					Code=Exchange;

				ddlSymbol.DataSource = dmb.GetStockList(Exchange,null,null);
				try
				{
					ddlSymbol.DataBind();
				}
				catch
				{
				}
			}

			if (Overlay=="" || Overlay==null)
				Overlay = lOver.Text;
			if (Indicator=="" || Indicator==null)
			{
				if (IsPostBack && tdIndicator.Visible)
					Indicator ="";
				else Indicator = lIndicator.Text;
			}
			if (IsPostBack) 
			{
				SetCookie(Config.SymbolParameterName,Code);
				SetCookie(sCookieOver,Overlay);
				SetCookie(sCookieIndi,Indicator);
				SetCookie("Skin",Skin);
				SetCookie("Size",Size);
				SetCookie("Type",Type);
				SetCookie("Scale",Scale);
				SetCookie("Range",Range);
				SetCookie("RT",RT);
				SetCookie("Start",Start);
				SetCookie("End",End);
				SetCookie("Cycle",Cycle);
				SetCookie("RType",RType);
			} 
			else 
			{
				SetValue(ddlSkin,Skin);
				SetValue(ddlSize,Size);
				SetValue(rblType,Type);
				SetValue(rblScale,Scale);
				SetValue(SelectDateRange.rblRange,Range);

				SelectDateRange.BindValue();
				Start = SelectDateRange.Start;
				End = SelectDateRange.End;

				cbRealTime.Checked =RT=="1"; 
				PageCode = Code;
				SelectDateRange.dpStart.Date = DateTime.ParseExact(Start,"yyyyMMdd",null);
				SelectDateRange.dpEnd.Date = DateTime.ParseExact(End,"yyyyMMdd",null);
				SelectDateRange.RType = RType;
				SetValue(SelectDateRange.ddlCycle,Cycle);
			}

			cbRealTime.Visible = Config.RealtimeVisible();
			RT = Config.UseRealtime(cbRealTime)?"1":"0";

			if (tdOverLay.Visible)
				CreateLine(Overlay,sOL,ddlOver,tdOverLay,false,Config.MinOverlay);
			if (tdIndicator.Visible)
				CreateLine(Indicator,"indi",ddlIndi,tdIndicator,true,Config.MinIndicator);
			if (HasExchange) 
			{
				int i = Code.IndexOf('.');
				if (i>0)
					Code = Code.Substring(0,i);
				Code += "."+ddlExchange.SelectedValue;
				SetCookie(Config.SymbolParameterName,Code);
			}
			PageCode = Code;
			ibChart.ImageUrl = 
				"~/"+Config.ChartPath+"Chart.aspx?Provider="+Config.CustomChartDataManager+
					"&Code="+Server.UrlEncode(Code)+
					"&Type="+Type+
					"&Scale="+Scale+
					"&IND="+Indicator+
					"&OVER="+Overlay+
					"&Skin="+Skin+
					"&Size="+Size+
					"&RT="+RT+
					"&Start="+Start+
					"&End="+End+
					"&Layout="+Config.LayoutForCustomChart+
					"&Cycle="+Cycle+
					"&X="+Tools.ToIntDef(Request.Form[ibChart.ID+".x"],0)+
					"&Y="+Tools.ToIntDef(Request.Form[ibChart.ID+".y"],0)+His;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			Utils.AddSkinToDropList(ddlSkin);
			ddlSize.SelectedValue = Config.DefaultChartWidth;

			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.ibChart.Click += new System.Web.UI.ImageClickEventHandler(this.ibChart_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void ibChart_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (lRedirect!=null && lRedirect.Text!="")
				Response.Redirect(lRedirect.Text.Replace("{Symbol}",Code));
		}
	}
}