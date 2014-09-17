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
using WebDemos;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for PriceList.
	/// </summary>
	public class PriceList : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox tbSymbol;
		protected System.Web.UI.WebControls.DataGrid dgList;
		protected System.Web.UI.WebControls.Label lCount;
		protected System.Web.UI.WebControls.Button btnOK;
		IDataManager idm;
		int ChangeColumnIndex;
		private void Bind()
		{
			int Count = Tools.ToIntDef(lCount.Text,30);
			CommonDataProvider cdp = (CommonDataProvider)idm[tbSymbol.Text,Count+1];
			double[] Date = cdp["DATE"];
			double[] Open = cdp["OPEN"];
			double[] High = cdp["HIGH"];
			double[] Low = cdp["LOW"];
			double[] Close = cdp["CLOSE"];
			double[] Volume = cdp["VOLUME"];
			DataTable dt = new DataTable();
			dt.Columns.Add("SYMBOL");
			dt.Columns.Add("DATE",typeof(DateTime));
			dt.Columns.Add("OPEN",typeof(double));
			dt.Columns.Add("HIGH",typeof(double));
			dt.Columns.Add("LOW",typeof(double));
			dt.Columns.Add("CLOSE",typeof(double));
			dt.Columns.Add("VOLUME",typeof(double));
			dt.Columns.Add("CHANGE",typeof(double));
			string Symbol = cdp.GetStringData("CODE");
			for(int i=Date.Length-1; i>0; i--)
				dt.Rows.Add(new object[]{
											Symbol,DateTime.FromOADate(Date[i]),Open[i],High[i],Low[i],Close[i],Volume[i],(Close[i]-Close[i-1])/Close[i-1]
										});

			ChangeColumnIndex = dt.Columns.IndexOf("CHANGE");
			dgList.DataSource = dt;
			dgList.DataBind();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				Bind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			idm = Utils.GetDataManager(Config.DefaultDataManager);
			InitializeComponent();
			if (dgList.Columns.Count==0)
				if (idm.StockListColumns!=null)
					foreach(DataGridColumn dgc in idm.StockListColumns)
						dgList.Columns.Add(dgc);
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.dgList.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgList_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Bind();
		}

		private void dgList_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType==ListItemType.Item || e.Item.ItemType==ListItemType.AlternatingItem)
			{
				if (ChangeColumnIndex>=0)
				{
					TableCell tc = e.Item.Cells[ChangeColumnIndex];
					string s =tc.Text;
					if (s.StartsWith("-")) 
					{
						tc.ForeColor = Color.Red;
					}
					else 
					{
						tc.ForeColor = Color.DarkGreen;
						if (string.Compare(s,"&nbsp;")!=0)
							tc.Text = "+"+s;
					}
				}
			}
		}
	}
}
