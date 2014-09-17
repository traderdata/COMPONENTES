namespace WebDemos.DBDemos
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Easychart.Finance;

	/// <summary>
	///		Summary description for SelectFormula.
	/// </summary>
	public class SelectFormula : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DropDownList ddlFormula;
		protected System.Web.UI.WebControls.Label lFullName;
		protected System.Web.UI.WebControls.Label lCode;
		protected System.Web.UI.WebControls.Label lDescription;
		protected System.Web.UI.WebControls.Label lParam;

		public FormulaProgram CurrentProgram;
		public string FmlFile;

		public string SelectedFormula
		{
			get 
			{
				if (CurrentProgram==null)
					return "";
				else 
				{
					return CurrentProgram.Name+"("+Params+")";
				}
			}
		}

		public string Params 
		{
			get 
			{
				string r = "";
				foreach(string s in Request.Form)
					if (s.StartsWith("__Param")) 
					{
						if (r!="")
							r+=",";
						r+=double.Parse(Request.Form[s]);
					}
				return r;
			}
		}

		private void FindFormula(FormulaSpace fs,string Formula) 
		{
			foreach(FormulaProgram fp in fs.Programs)
			{
				if (Formula==null)
				{
					if (CurrentProgram==null)
						CurrentProgram = fp;
				}
				else if (Formula==fp.Name) 
					CurrentProgram = fp;

				if (!IsPostBack)
					ddlFormula.Items.Add(new ListItem(fp.FullName,fp.Name));
			}
			
			foreach(FormulaSpace fsc in fs.Namespaces)
				FindFormula(fsc,Formula);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			FormulaSpace fs = FormulaSpace.Read(FmlFile);
			string s = Request.Form[ddlFormula.UniqueID];
			FindFormula(fs,s);
			if (CurrentProgram!=null)
			{
				lParam.Text = "<table border=1 cellspacing=0 cellpadding=3><tr><td>Name</td><td>Default Value</td><td>Minimum Value</td><td>Maxmum Value</td></tr>";
				foreach(FormulaParam fpm in CurrentProgram.Params) 
				{
					lParam.Text +="<tr><td>";
					lParam.Text +=fpm.Name +"</td><td>";
					string Value = fpm.DefaultValue;
					string r = "__Param"+fpm.Name;
					if (Request.Form[r]!=null)
						Value = Request.Form[r];
					lParam.Text +="<input Name="+r+" value="+Value+"></td><td>";
					lParam.Text +=fpm.MinValue+"</td><td>";
					lParam.Text +=fpm.MaxValue+"</td></tr>";
				}
				lParam.Text +="</table>";
				lFullName.Text = CurrentProgram.FullName;
				lDescription.Text = CurrentProgram.Description.Replace("\n","<br>");
				lCode.Text = Server.HtmlEncode(CurrentProgram.Code).Replace("\n","<br>");
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
