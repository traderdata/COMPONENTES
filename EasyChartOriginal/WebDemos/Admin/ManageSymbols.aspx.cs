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
using System.Text;
using System.Threading;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos.Admin
{
	/// <summary>
	/// Summary description for ManageSymbols.
	/// </summary>
	public class ManageSymbols : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btnGet;
		protected System.Web.UI.WebControls.Label lExchangeMsg;
		protected System.Web.UI.WebControls.TextBox tbRemain;
		protected System.Web.UI.WebControls.Button btnRemain;
		protected System.Web.UI.WebControls.Button btnDelete;
		protected System.Web.UI.WebControls.TextBox tbAddSymbol;
		protected System.Web.UI.WebControls.Button btnAdd;
		protected System.Web.UI.WebControls.Label lAddMsg;
		protected System.Web.UI.WebControls.TextBox tbExport;
		protected System.Web.UI.WebControls.Button btnExport;
		protected System.Web.UI.WebControls.Button btnUpdateRealtime;
		protected System.Web.UI.WebControls.Button btnDeleteAll;
		protected System.Web.UI.WebControls.Button btnClearCache;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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
			this.btnRemain.Click += new System.EventHandler(this.btnRemain_Click);
			this.btnDelete.Click += new System.EventHandler(this.btnRemain_Click);
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			this.btnUpdateRealtime.Click += new System.EventHandler(this.btnUpdateRealtime_Click);
			this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
			this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void ClearCache() 
		{
			Cache.Remove("QuoteList");
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			string[] ss = tbAddSymbol.Text.Split('\n');
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			int succ;
			int failed;
			dmb.SaveSymbolList(ss,out succ,out failed);
			lAddMsg.Text = "succ:"+succ+" failed:"+failed;
			ClearCache();
		}

		private void btnExport_Click(object sender, System.EventArgs e)
		{
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			DataTable dt = dmb.GetStockList();
			StringBuilder sb = new StringBuilder();
			foreach(DataRow dr in dt.Rows) 
				sb.Append(dr[0]+";"+dr[1]+";"+dr["Exchange"]+"\r");
			tbExport.Text = sb.ToString();
		}

		static public void UpdateRealtime(object Sender)
		{
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			DataTable dt = dmb.GetSymbolList();
			foreach(DataRow dr in dt.Rows)
			{
				string Code = dr["QuoteCode"].ToString();
				IDataProvider idp = dmb[Code,2];
				Utils.UpdateRealtime(Code,idp);
				Thread.Sleep(1);
			}
		}

		private void btnUpdateRealtime_Click(object sender, System.EventArgs e)
		{
			UpdateRealtime(this);
		}

		private void btnClearCache_Click(object sender, System.EventArgs e)
		{
			ClearCache();
		}

		private void btnRemain_Click(object sender, System.EventArgs e)
		{
			string[] ss = tbRemain.Text.Split('\r');
			for(int i=0; i<ss.Length; i++) 
			{
				string[] sss = ss[i].Split(';');
				if (sss.Length>0)
					ss[i] = sss[0].Trim();
				else ss[i] = ss[i].Trim();
			}
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			dmb.DeleteSymbols(null,ss,sender == btnRemain,false,false);
			ClearCache();
		}

		private void btnDeleteAll_Click(object sender, System.EventArgs e)
		{
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			dmb.DeleteSymbols(null,new string[]{},true,true,true);
		}
	}
}
