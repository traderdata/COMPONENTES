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
using System.Threading;
using System.Reflection;
using System.Globalization;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for Intraday.
	/// </summary>
	public class Intraday : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DropDownList ddlSymbols;
		protected System.Web.UI.WebControls.Button btnShowChart;
		protected System.Web.UI.WebControls.Image imgChart;
		protected System.Web.UI.WebControls.DropDownList ddlCycle;
		protected System.Web.UI.WebControls.CheckBox cbFixedTime;
		protected System.Web.UI.WebControls.DropDownList ddlSkin;
		protected System.Web.UI.WebControls.DropDownList ddlTotal;
	
		private void LoadSymbolList()
		{
			if (ddlSymbols.Items.Count==0)
			{
				string[] ss = Config.IntradaySymbols.Split(';');
				foreach(string s in ss) 
					ddlSymbols.Items.Add(Utils.GetPart1(s));
			}
		}

		private void BindChart()
		{
			string Provider = Config.IntraDataManager;
			if (!IsPostBack)
			{
				string s = Request.QueryString["Symbol"];
				ListItem li = ddlSymbols.Items.FindByValue(s);
				if (li!=null) 
					li.Selected = true;
			}
			string Symbol = ddlSymbols.SelectedItem.Value.ToString();
			string IntradayStr = Utils.GetExchange(Symbol);
			ExchangeIntraday ei = ExchangeIntraday.GetExchangeIntraday(IntradayStr);
			DateTime StartDate = ei.GetCurrentTradingDay();
			DateTime EndDate = StartDate.Date.AddDays(1).AddMinutes(-1);

			string Skin = Config.IntradaySkin;
			if (Skin=="")
				Skin = ddlSkin.SelectedItem.Value;
			string Cycle = ddlCycle.SelectedItem.Value;
			string T = ddlTotal.SelectedItem.Value.ToString();
			int TotalDay = 1;
			if (T.Length==1) 
			{
				TotalDay = Tools.ToIntDef(T,1);
				StartDate = StartDate.AddDays(-TotalDay+1);
				double TradingHours = ei.GetOpenTimePerDay()*TotalDay*24;
				if (TradingHours>120)
					Cycle = "Hour1";
				else if (TradingHours>60)
					Cycle = "Minute30";
				else if (TradingHours>30)
					Cycle = "Minute15";
				else if (TradingHours>20)
					Cycle = "Minute10";
				else if (TradingHours>10)
					Cycle = "Minute5";
			}
			else 
			{
				Utils.PreRange(T,out StartDate,out EndDate,out Cycle);
				Provider = Config.CustomChartDataManager;
			}

			string Overlay = "PriorClose";
			string Main = "MainArea";
			string Indicator = "";
			string Size = Config.IntradayDefaultSize;
			string Type = "1";
			string YFormat = "f0";
			string BMargin = "20";
			if (Size==Config.IntradayDefaultSize)
			{
				Overlay += ";MA(7);MA(14)";
				Main = "";
				Indicator = Config.IntradayIndicators;
				Type = "3";
				YFormat = "";
				BMargin = "0";
			}

			imgChart.ImageUrl = "~/Chart.aspx?Provider="+Provider+"&Code="+Symbol+
				"&Start="+StartDate.ToString("yyyyMMddHHmmss")+"&End="+EndDate.ToString("yyyyMMddHHmmss")+
				"&Main="+Main+"&Over="+Overlay+"&IND="+Indicator+"&Type="+Type+"&Scale=0&Skin="+Skin+"&size="+Size+
				"&YFormat="+YFormat+"&XCycle=Hour2&Cycle="+Cycle+"&Layout="+Config.LayoutForIntraday+
				"&SV=0&BMargin="+BMargin+"&Fix="+(cbFixedTime.Checked?"1":"0")+"&E="+IntradayStr; 
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			ddlCycle.Visible = Config.IntradayShowCycle;
			ddlSkin.Visible = Config.IntradaySkin=="";
			if (!IsPostBack)
				BindChart();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			LoadSymbolList();
			Utils.AddSkinToDropList(ddlSkin);
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnShowChart.Click += new System.EventHandler(this.btnShowChart_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnShowChart_Click(object sender, System.EventArgs e)
		{
			BindChart();
		}
	}
}
