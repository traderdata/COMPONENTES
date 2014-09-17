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
using System.Reflection;
using Easychart.Finance;

namespace WebDemos
{
	/// <summary>
	/// Summary description for FormulaHelp.
	/// </summary>
	public class FormulaHelp : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid dgMethods;
	
		private string ReplaceType(Type t)
		{
			if (t==typeof(FormulaData))
				return "Data Array";
			else if (t==typeof(double) || t==typeof(int))
				return "Number";
			else if (t==typeof(string))
				return "String";
			else if (t==typeof(bool))
				return "TRUE or FALSE";
			return "";
		}

		protected System.Web.UI.HtmlControls.HtmlTable tblMsg;
		protected System.Web.UI.HtmlControls.HtmlTable tblAllMethod;
		protected System.Web.UI.WebControls.Panel Panel1;

		Hashtable htCategory = new Hashtable();
		private void Page_Load(object sender, System.EventArgs e)
		{
			string Filter = Request.QueryString["Filter"];
			if (Filter!=null) Filter = Filter.ToUpper();
			MemberInfo[] mis = FormulaBase.GetAllMembers();

			DataTable dtMethods = new DataTable();
			dtMethods.Columns.Add("Category");
			dtMethods.Columns.Add("Method");
			dtMethods.Columns.Add("Parameter");
			dtMethods.Columns.Add("Return");
			dtMethods.Columns.Add("Description");

			for(int i =0; i<mis.Length; i++) 
				if (Filter==null || mis[i].Name==Filter)
				{
					object[] os = mis[i].GetCustomAttributes(false);
					if (os.Length>0)
					{
						DataRow dr = dtMethods.NewRow();
						if (mis[i] is MethodInfo)
						{
							MethodInfo mi = mis[i] as MethodInfo;
							dr["Return"] = ReplaceType(mi.ReturnType);
							ParameterInfo[] pis = mi.GetParameters();
							string P = "";
							string r = "";
							for(int j=0; j<pis.Length; j++)
							{
								if (P!="")
									P +=",";
								P +=pis[j].Name;
								r += "<font color=#FF8000><B>"+pis[j].Name+"</B></font>:"+ReplaceType(pis[j].ParameterType)+"<br>";
							}
							dr["Method"] = mis[i].Name+"("+P+")";
							dr["Parameter"] = r;
						} 
						else 
						{
							PropertyInfo pi = mis[i] as PropertyInfo;
							dr["Method"] = pi.Name;
							dr["Return"] = ReplaceType(pi.PropertyType);
						}
					
						foreach(object o in os)
						{
							if (o is DescriptionAttribute)
								dr["Description"] = Server.HtmlEncode((o as DescriptionAttribute).Description);
							else if (o is CategoryAttribute)
							{
								string s = (o as CategoryAttribute).Category;
								string s1 = s.Substring(0,1);
								if (htCategory[s1]==null)
									dtMethods.Rows.Add(new object[]{s1});

								htCategory[s1] = s;
								dr["Category"] = s;
							}
						}
						dtMethods.Rows.Add(dr);
					}
				} 
				else 
				{
					tblMsg.Visible = false;
					tblAllMethod.Visible = true;
				}

			dgMethods.DataSource = new DataView(dtMethods,"1=1","Category",DataViewRowState.CurrentRows);
			dgMethods.DataBind();

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
			this.dgMethods.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgMethods_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void dgMethods_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			e.Item.Cells[0].Visible = false;
			string s = e.Item.Cells[0].Text;
			if (s.Length==1)
			{
				e.Item.Cells[1].Attributes["colspan"] = "4";
				e.Item.Cells[1].BackColor = Color.Yellow;
				e.Item.Cells[1].Text = htCategory[s].ToString();
				e.Item.Cells[1].Font.Bold = true;
				for(int i=2; i<e.Item.Cells.Count; i++)
				e.Item.Cells[i].Visible = false;
			}
		}
	}
}
