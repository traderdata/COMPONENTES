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
using System.IO;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using Easychart.Finance.Objects;

namespace WebDemos.Objects
{
	/// <summary>
	/// Summary description for WebObject.
	/// </summary>
	public class WebObject : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btnShowObject;
		protected System.Web.UI.WebControls.DropDownList ddlObject;
		protected System.Web.UI.WebControls.Xml Xml;
		protected System.Web.UI.WebControls.Button btnUpload;
		protected System.Web.UI.HtmlControls.HtmlInputFile File;
		protected System.Web.UI.WebControls.ImageButton ibChart;
	
		private void BindFileList()
		{
			string[] ss = Directory.GetFiles(Server.MapPath(""),"*.xml");
			ddlObject.Items.Clear();
			foreach(string s in ss)
				ddlObject.Items.Add(Path.GetFileName(s));
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				BindFileList();
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
			this.btnShowObject.Click += new System.EventHandler(this.btnShowObject_Click);
			this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void ShowObjectFile(string FileName)
		{
			DataManagerBase dmb = new DBDataManager();
			FormulaChart fc = ObjectManager.ShowObjectOnChart("MSFT",FileName,dmb,true);
			if (fc!=null) 
			{
				int Width = 800;
				int Height = 600;
				if (fc.Rect!=Rectangle.Empty) 
				{
					Width = fc.Rect.Width;
					Height = fc.Rect.Height;
				}
				ibChart.ImageUrl = fc.SaveToWeb(Width,Height);
			}
		}

		private void btnShowObject_Click(object sender, System.EventArgs e)
		{
			ShowObjectFile(Server.MapPath(ddlObject.SelectedItem.Value));
		}

		private void btnUpload_Click(object sender, System.EventArgs e)
		{
			if (Request.Files.Count>0)
			{
				string FileName = Server.MapPath(Path.GetFileName(Request.Files[0].FileName));
				if (FileName.ToLower().EndsWith(".xml")) 
				{
					Request.Files[0].SaveAs(FileName);
					ShowObjectFile(FileName);
					BindFileList();
					ddlObject.SelectedValue = Path.GetFileName(FileName);
				}
			}
		}
	}
}
