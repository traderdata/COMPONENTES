namespace WebDemos.DBDemos
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for SelectDateRange.
	/// </summary>
	public class SelectDateRange : System.Web.UI.UserControl
	{
		public System.Web.UI.WebControls.DropDownList ddlCycle;
		public System.Web.UI.WebControls.RadioButtonList rblRange;

		public DatePicker dpStart;
		public DatePicker dpEnd;
		protected string AllCycles;

		private string GroupName = "RangeType";

		public string Start;
		public string End;
		public string Cycle;

		public string RType = "0";
		public string IsCheck(int Id) 
		{
			return RType == Id.ToString()?" checked":"";
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Page.RegisterClientScriptBlock("clientScript",
				"<script language=JavaScript> function UpdateRangeType(index) {"
				+ "document.wfCustomChart."+GroupName+"[index].checked = true;}" 
				+ "</script>");

			ddlCycle.Attributes["OnChange"] ="UpdateRangeType(1)";
			dpStart.ChangeScript = "UpdateRangeType(1)";
			dpEnd.ChangeScript = "UpdateRangeType(1)";
		}

		public void BindValue()
		{
			try
			{
				Start = dpStart.Date.ToString("yyyyMMdd"); 
				End = dpEnd.Date.ToString("yyyyMMdd");
				Cycle = Request.Form[ddlCycle.UniqueID];
				string Range = Request.Form[rblRange.UniqueID];
				if (Range==null)
					Range = rblRange.SelectedValue;

				if (Request.Form[GroupName]!=null)
					RType = Request.Form[GroupName];
				string __EVENTTARGET = Request.Form["__EVENTTARGET"];
				if (__EVENTTARGET!=null && __EVENTTARGET.StartsWith(rblRange.ClientID))
				{
					RType = "0";
				}

				if (RType == "0" && Range!=null)
					Utils.PreRange(Range,out Start,out End,out Cycle);
				base.DataBind ();
			} 
			catch
			{
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			BindValue();
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