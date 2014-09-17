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
using EasyTools;
using Easychart.Finance.DataProvider;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for Lookup.
	/// </summary>
	public class Lookup : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.DropDownList ddlExchange;
		protected System.Web.UI.WebControls.TextBox tbKey;
	
		private void Bind()
		{
			Utils.BindExchange(ddlExchange);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (!IsPostBack)
				Bind();
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
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			if (tbKey!=null) 
			{
				string Exchange = ddlExchange.SelectedItem.Value;
				Exchange = "&E="+Exchange;
				Response.Redirect("StockList.aspx?Key="+tbKey.Text+Exchange);
			}
		}
	}
}
