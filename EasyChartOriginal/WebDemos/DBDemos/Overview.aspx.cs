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

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for Overview.
	/// </summary>
	public class Overview : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.HyperLink hlDaily;
		protected System.Web.UI.WebControls.HyperLink hlWeekly;
		protected System.Web.UI.WebControls.HyperLink hlChart;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			string Symbol = Request.QueryString[Config.SymbolParameterName];
			if (Symbol==null || Symbol=="")
				Symbol = "MSFT";
			string s1 = "(Symbol)";
			string s2 = Config.SymbolParameterName+"="+Symbol;
			string s3 = "Code="+Symbol;

			hlDaily.NavigateUrl = hlDaily.NavigateUrl.Replace(s1,s2);
			hlWeekly.NavigateUrl = hlWeekly.NavigateUrl.Replace(s1,s2);
			hlDaily.ImageUrl = hlDaily.ImageUrl.Replace(s1,s3);
			hlWeekly.ImageUrl = hlWeekly.ImageUrl.Replace(s1,s3);

//			Chart.aspx?Provider=Yahoo&Code=MSFT&Type=3&Scale=0&Start=20040424&End=20041024&
//				Cycle=DAY1&MA=&EMA=10;50;200&IND=MACD&OVER=SAR&COMP=&
//											 Skin=RedWhite&Size=640&Layout=2Line;Default;Price;HisDate&Width=1

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
