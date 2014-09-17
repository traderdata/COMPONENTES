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
	public class Progress : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lCondition;
		protected System.Web.UI.WebControls.Label lProgress;
		protected System.Web.UI.HtmlControls.HtmlGenericControl ProgressLabel;
		protected System.Web.UI.WebControls.Label lTotal;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			string ConditionId = Request.QueryString["ConditionId"];
			if (ConditionId!=null) 
			{
				DataRow drCond = DB.GetFirstRow("select * from Condition where ConditionId="+ConditionId);
				if (drCond!=null) 
				{

					if (drCond["Exchange"].ToString()!="")
						lCondition.Text  = "Exchange:"+drCond["Exchange"]+";";
					lCondition.Text += " Filtered by formula:"+drCond["Condition"];
					string Total = drCond["Total"].ToString();
					object Scaned = Cache[ConditionId];
					double Percent = 0;
					if (Total!="" && Scaned!=null)
						Percent = ((float)(int)Scaned)/int.Parse(Total);
						lCondition.Text +="; "+Percent.ToString("p2");

					lProgress.Text = "<table bgcolor=red cellpadding=0 cellspacing=1>"+
												"<tr>"+
												"	<td>"+
												"		<table width=200 bgcolor=white>"+
												"		<tr height=14>"+
												"			<td width="+(int)(Percent*200)+" bgcolor=red></td><td width="+(int)(200-Percent*200)+">"+
												"			</td></tr>"+
												"		</table>"+
												"	</td>"+
												"	</tr>"+
												"</table>";

					if (Percent==1)
						Response.Redirect("StockList.aspx?ConditionId="+ConditionId);

					object o1 = drCond["StartTime"];
					object o2 = drCond["EndTime"];
					if (o1!=DBNull.Value) 
					{
						DateTime d2 = DateTime.Now;
						if (o2!=DBNull.Value)
							d2 = (DateTime)o2;

						lCondition.Text +=  "; "+(d2-(DateTime)o1).TotalSeconds.ToString("f2")+"s";
					}//  else Response.Redirect("StockList.aspx");
				} else Response.Redirect("StockList.aspx");
			} else Response.Redirect("StockList.aspx");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN：该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
