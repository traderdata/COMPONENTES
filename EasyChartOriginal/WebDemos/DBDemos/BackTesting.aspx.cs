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
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for BackTesting.
	/// </summary>
	public class BackTesting : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid dgResult;
		protected System.Web.UI.WebControls.Button btnTesting;
		protected System.Web.UI.WebControls.TextBox tbSymbol;
		protected System.Web.UI.WebControls.TextBox tbBars;
		protected SelectFormula sfBackTesting;
		private DataTable dtResult;
		protected System.Web.UI.WebControls.Label lMsg;
		private double AllProfit;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			sfBackTesting.FmlFile = Config.PluginsDirectory+"Trading.fml";
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
			this.btnTesting.Click += new System.EventHandler(this.btnTesting_Click);
			this.dgResult.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgResult_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnTesting_Click(object sender, System.EventArgs e)
		{
			string Symbol = Request.QueryString["Symbol"];
			if (tbSymbol!=null)
				Symbol =tbSymbol.Text;
			FormulaBase fb = FormulaBase.GetFormulaByName("Trading."+sfBackTesting.SelectedFormula);
			IDataManager idm = Utils.GetDataManager(Config.DefaultDataManager);
			Utils.SetYahooCacheRoot(idm);
			IDataProvider idp = null;
			try
			{
				idp = idm[Symbol,Tools.ToIntDef(tbBars.Text,260)];
			}
			catch
			{
			}
			if (idp!=null && idp.Count>0)
			{
				FormulaPackage fp = fb.Run(idp);
				FormulaData fdEnter = fp["EnterLong"];
				FormulaData fdExit = fp["ExitLong"];
				FormulaData fdPrice = idp["CLOSE"];
				int Count = fdPrice.Length;
				dtResult = new DataTable();
				dtResult.Columns.Add("EntryDate",typeof(DateTime));
				dtResult.Columns.Add("EntryPrice",typeof(double));
				dtResult.Columns.Add("ExitDate",typeof(DateTime));
				dtResult.Columns.Add("ExitPrice",typeof(double));
				dtResult.Columns.Add("ProfitPercent",typeof(double));
				dtResult.Columns.Add("BarsHold");
				bool Entered = false;
				int EntryBars=0;
				double EntryPrice=0;
				for(int i=0; i<Count; i++)
				{
					if (Entered)
					{
						if (fdExit[i]>0) 
						{
							DataRow drLast = dtResult.Rows[dtResult.Rows.Count-1];
							drLast["ExitDate"] = DateTime.FromOADate(idp["DATE"][i]);
							double ExitPrice = fdPrice[i];
							drLast["ExitPrice"] = ExitPrice;
							drLast["BarsHold"] = i-EntryBars;
							drLast["ProfitPercent"] = (ExitPrice-EntryPrice)/EntryPrice;
							Entered = false;
						}
					} 
					else 
					{
						if (fdEnter[i]>0) 
						{
							EntryPrice = fdPrice[i];
							dtResult.Rows.Add(new object[]{DateTime.FromOADate(idp["DATE"][i])
															  ,EntryPrice});
							EntryBars = i;
							Entered = true;
						}
					}
				}
				dgResult.DataSource = dtResult;
			} else lMsg.Text = Symbol+" not found!";
			AllProfit = 1;
			dgResult.DataBind();
		}

		private void dgResult_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex>=0) 
			{
				object o = dtResult.Rows[e.Item.ItemIndex]["ProfitPercent"];
				double Profit =0;
				if (o != DBNull.Value)
					Profit = (double)o;
				if (e.Item.ItemType==ListItemType.Item || e.Item.ItemType==ListItemType.AlternatingItem)
				{
					AllProfit *= (1+Profit);
				} 
				if (Profit<0) 
				{
					for(int i=0; i<e.Item.Cells.Count; i++) 
					{
						e.Item.Cells[i].BackColor = Color.MistyRose;
					}
				}
			}
			if (e.Item.ItemType==ListItemType.Footer)
			{
				e.Item.Cells[4].Text = (AllProfit-1).ToString("p2");
			}
		}
	}
}
