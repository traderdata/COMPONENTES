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
using System.Xml;
using System.IO;
using System.Web.Caching;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for gallery.
	/// </summary>
	public class Gallery : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlTableCell tdSymbols;
		protected System.Web.UI.WebControls.Button btnGo;
		protected System.Web.UI.WebControls.DataList dlGallery;
		protected System.Web.UI.WebControls.DropDownList ddlDuration;
		protected System.Web.UI.WebControls.DropDownList ddlType;
		private Random Rnd = new Random();
	
		public string Root 
		{
			get 
			{
				string s = HttpRuntime.AppDomainAppVirtualPath;
				if (!s.EndsWith("/"))
					s+="/";
				return s;
			}
		}

		public string GetParam(string Key,string FieldName,DataListItem Container)
		{
			DataRowView drv = (DataRowView)Container.DataItem;
			if (drv.DataView.Table.Columns.IndexOf(FieldName)>=0)
				return "&"+Key+"="+drv[FieldName];
			return "";
		}

		public string ChartFooter(DataListItem Container) 
		{
			DataRowView drv = (DataRowView)Container.DataItem;
			string Name = drv["Name"].ToString(); 
			if (Name!="") Name +="<br>";

			string Text = "<B>";

			string StartDate = drv["StartDate"].ToString();
			if (StartDate.Length>4)
				StartDate = StartDate.Substring(0,4);
			if (StartDate=="" || StartDate==null)
				try
				{
					StartDate = DateTime.Now.AddMonths(-Tools.ToIntDef(drv["Span"],6)).Year.ToString();
				}
				catch
				{
				}
			Text +=StartDate+" - "+DateTime.Now.Year + " "+drv["CycleText"]+"</b>";
			return Text;
		}

		private void Bind(DataTable dt) 
		{
			dlGallery.DataSource = dt;
			dlGallery.DataBind();
		}

		private void BindXml(string Filename) 
		{
			DataSet ds = new DataSet();
			string s = Request.MapPath(Request.PathInfo)+"\\"+Filename;
			if (File.Exists(s)) 
			{
				ds.ReadXml(s);
				Bind(ds.Tables[0]);
			} else Bind(null);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack) 
				BindXml(ddlType.SelectedValue);
		}

		private void CreateSymbolBox() 
		{
			for(int i=0; i<10; i++)
			{
				TextBox tb = new TextBox();
				tb.ID = "Symbol"+i;
				tb.Width = 80;
				tdSymbols.Controls.Add(tb);
				if (i % 5==4)
				{
					Literal l = new Literal();
					l.Text = "<br>";
					tdSymbols.Controls.Add(l);
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			CreateSymbolBox();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.ddlType.SelectedIndexChanged += new System.EventHandler(this.ddlType_SelectedIndexChanged);
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnGo_Click(object sender, System.EventArgs e)
		{
			DataSet ds = new DataSet();
			string Filename = Request.MapPath(Request.PathInfo)+@"\index.xml";
			if (File.Exists(Filename)) 
				ds.ReadXmlSchema(Filename);
			DataTable dt = ds.Tables[0];

			string s = ddlDuration.SelectedValue;
			DateTime Start = DateTime.Now;
			string Cycle = "Day";
			string BigCycle = "Day";
			string XFormat = "MMM";
			string XCycle = "Month1";
			string YFormat = "f2";
			switch (s.ToUpper())
			{
				case "MONTH2":
					Start = Start.AddMonths(-2);
					break;
				case "MONTH6":
					Start = Start.AddMonths(-6);
					Cycle = "DAY2";
					break;
				case "YEAR1":
					Start = Start.AddYears(-1);
					Cycle = "WEEK";
					BigCycle = "Day3";
					XCycle = "Month3";
					break;
			}

			foreach(Control c in tdSymbols.Controls)
				if (c is TextBox)
				{
					TextBox tb = c as TextBox;
					if (tb.Text!="") 
						dt.Rows.Add(new object[]{tb.Text,"",Cycle,BigCycle,"",XCycle,XFormat,Start.ToString("yyyyMMdd"),"",YFormat});
				}
			Bind(dt);
		}

		private void ddlType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BindXml(ddlType.SelectedValue);
		}
	}
}
