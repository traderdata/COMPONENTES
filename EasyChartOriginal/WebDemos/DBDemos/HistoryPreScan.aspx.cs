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
using System.Text;
using EasyTools;

namespace WebDemos.DBDemos
{
	public class HistoryPreScan : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Literal lPrescan;
		protected System.Web.UI.WebControls.Label lFormat;
		protected System.Web.UI.WebControls.Label lSeparator;
		string ScanType;

		private void Page_Load(object sender, System.EventArgs e)
		{
			ScanType=Request.QueryString["Type"];
			string Where = " where EndTime is not null ";
			if (ScanType!=null) 
			{
				Where += " and ScanType="+ScanType;
				ScanType = "&Type="+ScanType;
			}
			else Where +=" and ScanType>0";

			DataTable dt = DB.GetDataTable("select EndTime from condition "+Where+" group by EndTime order by EndTime");

			StringBuilder sb = new StringBuilder();
			foreach(DataRow dr in dt.Rows)
			{
				if (sb.Length>0)
					sb.Append(lSeparator.Text);
				if (dr[0] is DateTime)
				{
					DateTime D = ((DateTime)dr[0]);
					string D1 = string.Format(lFormat.Text,D.AddHours(Config.AdjustHours));
					string D2 = string.Format("{0:yyyy-MM-dd}",D);
					sb.Append("<a href=Prescan.aspx?D="+D2+ScanType+">"+D1+"</a>");
				}
			}
			lPrescan.Text = sb.ToString();
		}

		#region Web Designer
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
