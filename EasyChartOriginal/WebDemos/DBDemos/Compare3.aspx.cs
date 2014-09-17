using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for Compare3.
	/// </summary>
	public class Compare3 : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btnUpdate;
		protected System.Web.UI.WebControls.RadioButtonList rblScale;
		protected System.Web.UI.WebControls.ImageButton ibChart;
		protected System.Web.UI.WebControls.DropDownList ddlSymbol1;
		protected System.Web.UI.WebControls.DropDownList ddlSymbol2;
		protected System.Web.UI.WebControls.DropDownList ddlSymbol3;
		protected System.Web.UI.WebControls.Label lDefault1;
		protected System.Web.UI.WebControls.Button btnUpdate2;
		protected System.Web.UI.WebControls.Literal lSymbols;
		protected SelectDateRange SelectDateRange;

		private void BindLookup(DropDownList ddl,DataTable dt,int Index,string Default) 
		{
			if (dt.Rows.Count>0)
			{
				ddl.DataSource = dt;
				if (Index>=dt.Rows.Count)
					Index = dt.Rows.Count-1;

				ddl.DataBind();
				ListItem li = ddl.Items.FindByValue(Default);
				if (li!=null)
					ddl.SelectedValue = li.Value;
				else ddl.SelectedValue = dt.Rows[Index][0].ToString();
			}
		}
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			ArrayList al = new ArrayList();
			foreach(Control c in Controls[1].Controls)
			{
				if (c is DropDownList)
					al.Add(c);
			}

			if (!IsPostBack) 
			{
				SelectDateRange.rblRange.SelectedValue = "max";
				SelectDateRange.BindValue();
				
				DataTable dt;
				DataManagerBase dmb = Utils.GetDataManager(Config.DefaultDataManager);
				if (lSymbols!=null && lSymbols.Text!="")
					dt = dmb.GetStockList("","NoRealtime_"+lSymbols.Text,null);
				else dt = dmb.GetStockList("Economic",null,null);

				//DataTable dt = DB.GetDataTable("select QuoteCode,QuoteName from StockData where Exchange='Economic'");
				BindLookup(ddlSymbol1,dt,0,lDefault1.Text);
				DataRow dr = dt.NewRow();
				dr[0] = "";
				dr[1] = "N/A";
				dt.Rows.InsertAt(dr,0);
				
				foreach(DropDownList ddl in al)
					if (ddl!=ddlSymbol1)
						BindLookup(ddl,dt,0,"");
				//BindLookup(ddlSymbol2,dt,0,"");
				//BindLookup(ddlSymbol3,dt,0,"");
			}

			string Scale =rblScale.SelectedItem.Value; 
			string Skin = Config.DefaultSkin;

			string Comp = "";
			foreach(DropDownList ddl in al)
				if (ddl!=ddlSymbol1)
				{
					string s = ddl.SelectedValue;
					if (s!="" && s!=null)
						Comp +="COMPARE("+s+");";
				}

			ibChart.ImageUrl = "../Chart.aspx?Provider=DB&Code="+
				ddlSymbol1.SelectedValue+
				"&Over="+Comp.TrimEnd(';')+// COMPARE("+ddlSymbol2.SelectedValue+");COMPARE("+ddlSymbol3.SelectedValue+")"+
				"&Scale="+Scale+
				"&Skin="+Skin+
				"&Size=700*500"+
				"&Start="+SelectDateRange.Start+
				"&End="+SelectDateRange.End+
				"&Cycle="+SelectDateRange.Cycle+
				"&Type=Line"+
				"&His=0"+
				"&X="+Tools.ToIntDef(Request.Form[ibChart.ID+".x"],0)+
				"&Y="+Tools.ToIntDef(Request.Form[ibChart.ID+".y"],0);
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
