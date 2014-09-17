namespace WebDemos
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Globalization;
	using EasyTools;

	public abstract class DatePicker : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DropDownList ddlMonth;
		protected System.Web.UI.WebControls.DropDownList ddlDay;
		protected System.Web.UI.WebControls.DropDownList ddlYear;

		private DateTime date = new DateTime(1930,1,1);
		private int maxYear = DateTime.Now.Year;
		public string ChangeScript;

		public int MaxYear 
		{
			get
			{
				return maxYear;
			}
			set
			{
				maxYear = value;
				try
				{
					CreateYearDropList();
					ddlYear.SelectedValue = Date.Year.ToString();
				}
				catch
				{
				}
			}
		}

		int GetSelValue(DropDownList ddl,int Def)
		{
			ListItem li = ddl.SelectedItem;
			if (li!=null) 
			{
				return Tools.ToIntDef(li.Value,Def);
			}
			return Def;
		}

		private void BindDate()
		{
			Date = new DateTime(
				Tools.ToIntDef(Request.Form[ddlYear.UniqueID],1930),
				Tools.ToIntDef(Request.Form[ddlMonth.UniqueID],1),
				Tools.ToIntDef(Request.Form[ddlDay.UniqueID],1));
		}

		public void CreateYearDropList() 
		{
			ddlYear.Items.Clear();
			for(int i=MaxYear; i>=Config.DatePickerStartYear; i--)
				ddlYear.Items.Add(i.ToString());
		}

		public DateTime Date 
		{
			get 
			{
				return date;
			}
			set 
			{
				date = value;
				string sYear = date.Year.ToString();
				string sMonth = date.Month.ToString();
				string sDay = date.Day.ToString();

				try
				{
					ddlYear.SelectedValue = sYear;
					ddlMonth.SelectedValue = sMonth;
					ddlDay.SelectedValue = sDay;
				} 
				catch
				{
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (ChangeScript!=null && ChangeScript!="") 
			{
				ddlYear.Attributes["OnChange"] = ChangeScript;
				ddlMonth.Attributes["OnChange"] = ChangeScript;
				ddlDay.Attributes["OnChange"] = ChangeScript;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			CreateYearDropList();
			for(int i=1; i<13; i++)
			{
				DateTime dt = new DateTime(2000,i,1);
				ddlMonth.Items.Add(new ListItem(dt.ToString("MMM",DateTimeFormatInfo.InvariantInfo),i.ToString()));
			}

			for(int i=1; i<32; i++)
				ddlDay.Items.Add(i.ToString());
			BindDate();
			
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
