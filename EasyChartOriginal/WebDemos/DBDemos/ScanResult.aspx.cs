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

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for ScanResult.
	/// </summary>
	public class ScanResult : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid dgScanResult;
	
		private void Bind() 
		{
			DataTable dt = DB.GetDataTable("select * from Condition order by ConditionId desc");
			dt.Columns.Add("ScanTime");
			foreach(DataRow dr in dt.Rows)
			{
				object o1 = dr["EndTime"];
				object o2 = dr["StartTime"];
				if (o1!=DBNull.Value && o2!=DBNull.Value) 
				{
					dr["ScanTime"] = ((TimeSpan)((DateTime)o1-(DateTime)o2)).TotalSeconds.ToString("f2")+"s";
				}
			}
				
			dgScanResult.DataSource = dt;
			dgScanResult.DataBind();
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
			this.dgScanResult.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgScanResult_PageIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void dgScanResult_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgScanResult.CurrentPageIndex = e.NewPageIndex;
			Bind();
		}
	}
}
