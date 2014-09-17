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
using System.Globalization;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for PreScan.
	/// </summary>
	public class PreScan : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid dgPreScan1;
		protected System.Web.UI.WebControls.DataGrid dgPreScan2;
		protected System.Web.UI.WebControls.Label lDate1;
		protected System.Web.UI.WebControls.Label lDate2;
		protected System.Web.UI.HtmlControls.HtmlTable tblTechnical;
		protected System.Web.UI.HtmlControls.HtmlTable tblCandle;
	
		private BoundColumn GetBoundColumn(string DataField,string HeaderText)
		{
			BoundColumn bc = new BoundColumn();
			bc.DataField = DataField;
			if (HeaderText=="")
				bc.HeaderText = DataField;
			else bc.HeaderText = HeaderText;
			return bc;
		}

		private void Bind(DataGrid dg, Label lDate,HtmlTable tbl,int ScanType) 
		{
			tbl.Visible = true;

			DateTime D1 = DateTime.Today;
			DateTime D2 = Tools.ToDateDef(Request.QueryString["D"],DateTime.MinValue);
		
			if (D2==DateTime.MinValue) 
			{
				DataRow drFirst = DB.GetFirstRow("select EndTime from Condition where ScanType="+ScanType+" and EndTime is not null order by EndTime desc");
				if (drFirst!=null && drFirst[0] is DateTime)
					D1 = (DateTime)drFirst[0];
			} else D1 = D2;
			
			DataTable dt = DB.GetDataTable("select * from Condition where ScanType="+ScanType+
				" and EndTime>='"+D1.ToString("yyyy-MM-dd")+
				"' and EndTime<'"+D1.AddDays(1).ToString("yyyy-MM-dd")+"' order by Condition");
			if (dt.Rows.Count>0) 
			{
				object o = dt.Rows[0]["StartTime"];
				if (o is DateTime) 
					lDate.Text = ((DateTime)o).AddHours(Config.AdjustHours).ToString("MMM dd yyyy",DateTimeFormatInfo.InvariantInfo);
			}
			DataTable dtTrans = new DataTable();
			dtTrans.Columns.Add("Indicators");
			dtTrans.PrimaryKey = new DataColumn[]{dtTrans.Columns[0]};
			bool b = dg.Columns.Count==0;

			if (b)
				dg.Columns.Add(GetBoundColumn("Indicators",""));
			foreach(string s in Config.PreScanExchange.Split(';'))
			{
				string s1 = Utils.GetPart1(s);
				if (b) 
				{
					string s2 = Utils.GetPart2(s);
					dg.Columns.Add(GetBoundColumn(s1,s2));
				}
				dtTrans.Columns.Add(s1);
			}
			if (b)
				dg.Columns.Add(GetBoundColumn("Total",""));
			dtTrans.Columns.Add("Total");

			foreach(DataRow dr in dt.Rows)
			{
				DataRow drTrans = dtTrans.Rows.Find(dr["Condition"]);
				if (drTrans==null) 
				{
					drTrans = dtTrans.NewRow();
					drTrans["Indicators"] = dr["Condition"];
					dtTrans.Rows.Add(drTrans);
				}
				drTrans[dr["Exchange"].ToString()] = dr["ConditionId"]+","+dr["ResultCount"];
			}

			foreach(DataRow dr in dtTrans.Rows)
			{
				int Sum = 0;
				string SumId = "";
				for(int i=1; i<dtTrans.Columns.Count-1; i++)
				{
					string[] ss = dr[i].ToString().Split(',');
					if (ss.Length==2) 
					{
						dr[i] = "<a href=StockList.aspx?ConditionId="+ss[0]+">"+ss[1]+"</a>";
						Sum += Tools.ToIntDef(ss[1],0);
						if (SumId!="") SumId +=",";
						SumId += ss[0];
					}
				}
				dr[dtTrans.Columns.Count-1] = "<a href=StockList.aspx?ConditionId="+SumId+">"+Sum+"</a>";;
			}

			dg.DataSource = dtTrans;
			dg.DataBind();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			string Type=Request.QueryString["Type"];
			if (Type=="1" || Type==null)
				Bind(dgPreScan1,lDate1,tblTechnical, 1);
			if (Type=="2" || Type==null)
				Bind(dgPreScan2,lDate2,tblCandle,2);
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
