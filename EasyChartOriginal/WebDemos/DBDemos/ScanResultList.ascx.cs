namespace WebDemos.DBDemos
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using EasyTools;

	/// <summary>
	///		Summary description for ScanResultList.
	/// </summary>
	public class ScanResultList : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataList dlSymbols;

		public string FormulaName;
		public string Exchange;
		public DateTime LastStartTime = DateTime.MinValue;

		private void Page_Load(object sender, System.EventArgs e)
		{
			string Where = "where Condition='"+FormulaName+"'";
			if (Exchange!="")
				Where +=" and exchange='"+Exchange+"'";

			DataTable dtCond = DB.GetDataTable("select * from Condition "+Where+" order by ConditionId desc");

			string s = "";
			foreach(DataRow dr in dtCond.Rows) 
			{
				DateTime NowStartTime = (DateTime)dr["StartTime"];
				if (LastStartTime!=DateTime.MinValue && NowStartTime!=LastStartTime)
					break;
				if (s!="")
					s+=",";
				s +=dr["ConditionId"].ToString();
				LastStartTime = (DateTime)dr["StartTime"];
			}

			if (s!="")
			{
				DataTable dt = DB.GetDataTable("select distinct QuoteCode from ScanedQuote where ConditionId in ("+s+")");
				dlSymbols.DataSource = dt;
				dlSymbols.DataBind();
			}
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
