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
using System.Threading;
using System.Net;
using EasyTools;
using Easychart.Finance.DataProvider;

namespace WebDemos.Admin
{
	/// <summary>
	/// Summary description for FixWrongData.
	/// </summary>
	public class FixWrongData : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btnFixWrongData;
		protected System.Web.UI.WebControls.Label lFixProgress;
		protected System.Web.UI.WebControls.DataGrid dgWrongList;

		static private int TotalCount = 0;
		static private int NowIndex = 0;
		static private int FixedCount = 0;
		static private int FixErrorCount;
	
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
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnFixWrongData.Click += new System.EventHandler(this.btnFixWrongData_Click);
			this.dgWrongList.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgWrongList_DeleteCommand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		public enum FixDataResult {OK,NoData,Others};
		static private FixDataResult TestGood(IDataManager idm,string Symbol,bool OnlyUp2Date)
		{
			IDataProvider idp = idm[Symbol,Config.FixDataTestDays];
			double[] High = idp["HIGH"];
			double[] Low = idp["LOW"];
			double[] Date = idp["DATE"];
			FixDataResult fdr = FixDataResult.OK;
			for(int i=Date.Length-1; i>=1; i--)
			{
				if (!OnlyUp2Date)
				{
					double d1 = Low[i]/High[i-1];//dd[i]/dd[i-1];
					double d2 = High[i]/Low[i-1];
					double Delta = Config.FixDataDifference/100.0;
					if (d2<(1.0-Delta) || d1>(1.0+Delta)) 
					{
						Tools.Log(Symbol+":Delta = "+d1+","+d2+":"+DateTime.FromOADate(Date[i]));
						fdr = FixDataResult.Others;
					}

					if (Date[i]-Date[i-1]>Config.FixDataGapDays) 
					{
						Tools.Log(Symbol+":GapDays:"+DateTime.FromOADate(Date[i]));
						fdr = FixDataResult.Others;
					}
				}
				if (i==Date.Length-1)
					if ((DateTime.Now.ToOADate()-Date[i])>Config.FixDataNoDataDays) 
					{
						Tools.Log(Symbol+":NoData:"+DateTime.FromOADate(Date[i]));
						fdr = FixDataResult.NoData;
					}
			}
			if (Date.Length==0)
				fdr = fdr = FixDataResult.NoData;
			return fdr; 
		}

		static private void InsertQuote(int Id,string Symbol)
		{
			try
			{
				DB.DoCommand("insert into ScanedQuote (ConditionId,QuoteCode) values ("+Id+",'"+Symbol+"')");
			}
			catch
			{
			}
		}
	
		/// <summary>
		/// Test days : FixData.TestDays
		/// When data increase or decline beyond +/-"FixData.Difference"% , reload data
		/// When data lost for "FixData.GapDays" days, reload data, still no data then delete
		/// When no data for latest "FixData.NoDataDays" days, reload data, still no data then delete
		/// 
		/// Result saved in QuoteCode
		/// -1 : Stocks should be deleted
		/// -2 : Network error, Should redownload
		/// -3 : No need to fix again
		/// -4 : Fixed symbols
		/// </summary>
		/// <param name="Sender"></param>
		static public void FixData(object Sender)
		{
			Tools.Log("FixData started");
			try
			{
				DataTable dtSkip = DB.GetDataTable("select QuoteCode from ScanedQuote where ConditionId=-3");
				ArrayList alSkip = new ArrayList();
				foreach(DataRow dr in dtSkip.Rows)
					alSkip.Add(dr[0].ToString());

				DB.DoCommand("delete from ScanedQuote where ConditionId<0 and ConditionId<>-3");
				DataManagerBase dmb = Utils.GetDefaultDataManager();
				DataTable dt = dmb.GetSymbolList();
				DBDataManager ddm = new DBDataManager();
				FixedCount = 0;
				FixErrorCount = 0;
				TotalCount = dt.Rows.Count;
				for(int j=0; j<dt.Rows.Count; j++)
				{
					NowIndex = j+1;
					DataRow dr = dt.Rows[j];
					string Symbol = dr["QuoteCode"].ToString();
					try
					{
						if (TestGood(ddm,Symbol,false)!=FixDataResult.OK)
						{
							if (alSkip.IndexOf(Symbol)>=0) 
							{
								if (TestGood(ddm,Symbol,true)==FixDataResult.OK) 
									continue;
								else 
									DB.DoCommand("delete from ScanedQuote where ConditionId=-3 and QuoteCode='"+Symbol+"'");
							}
							
							for(int k=0; k<3; k++)
								try
								{
									Utils.DownloadYahooHistory(Symbol,true,false);
									break;
								} 
								catch (WebException)
								{
									if (k==2) throw;
								}

							FixDataResult fdr1 = TestGood(ddm,Symbol,false);
							FixDataResult fdr2 = TestGood(ddm,Symbol,true);
							if (fdr2==FixDataResult.NoData)
								InsertQuote(-1,Symbol);
							else if (fdr1==FixDataResult.Others && fdr2==FixDataResult.OK)
								InsertQuote(-3,Symbol);
							else InsertQuote(-4,Symbol);
							FixedCount++;
						}
					}
					catch (Exception e)
					{
						if (e.Message.IndexOf("404")>=0)
							InsertQuote(-1,Symbol);
						else InsertQuote(-2,Symbol);
						Tools.Log("FixData:"+Symbol+";"+e.Message+";"+e.GetType());
						FixErrorCount++;
					}
					Thread.Sleep(1);
				}
				Tools.Log("FixData finished");
			} 
			finally
			{
				FixDataThread = null;
			}
		}

		public static void FixData()
		{
			FixData(null);
		}

		public static void DeleteWrongData(object sender)
		{
			string s = DB.GetCommaValues("select a.QuoteCode from ScanedQuote a,StockData b where ConditionId=-1 and a.QuoteCode=b.QuoteCode and b.Exchange<>'Economic'",",");
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			string[] ss = s.Split(',');
			dmb.DeleteSymbols(null,ss,false,false,false);
			if (s.Trim()!="")
				DB.DoCommand("delete from ScanedQuote where QuoteCode in ('"+string.Join("','",ss)+"') and ConditionId=-1");
		}

		public static void DeleteWrongData()
		{
			DeleteWrongData(null);
		}

		private void Bind()
		{
			btnFixWrongData.Text = FixDataThread==null?"Fix wrong data":"Stop fix wrong data";
			DataTable dt = new DataTable();
			dt.Columns.Add("ConditionId",typeof(int));
			dt.Columns.Add("Description");
			dt.Columns.Add("Count",typeof(int));
			DataTable dtFixWrong = DB.GetDataTable("select ConditionId,count(*) from ScanedQuote where ConditionId<0 group by ConditionId");
			dt.Rows.Add(new object[]{-1,"Stocks should be deleted",0});
			dt.Rows.Add(new object[]{-2,"Network error, Should redownload",0});
			dt.Rows.Add(new object[]{-3,"No need to fix again",0});
			dt.Rows.Add(new object[]{-4,"Fixed symbols",0});
			foreach(DataRow dr1 in dt.Rows)
			{
				foreach(DataRow dr2 in dtFixWrong.Rows)
					if (dr1[0].ToString()==dr2[0].ToString())
					{
						dr1[2] = dr2[1];
						break;
					}
			}
			dgWrongList.DataSource = dt;
			dgWrongList.DataBind();

			if (FixDataThread!=null)
				lFixProgress.Text = "Total:"+TotalCount+";Progress:"+NowIndex+";Fixed:"+FixedCount+";Error:"+FixErrorCount;
			else lFixProgress.Text = "Stopped";
		}

		static private Thread FixDataThread;
		private void btnFixWrongData_Click(object sender, System.EventArgs e)
		{
			if (FixDataThread==null)
			{
				FixDataThread = new Thread(new ThreadStart(FixData));
				FixDataThread.Start();
			} 
			else 
			{
				FixDataThread.Abort();
				FixDataThread.Join();
				FixDataThread = null;
			}
			Bind();
		}

		private void dgWrongList_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.Item.ItemIndex==0)
				DeleteWrongData();
			Bind();
		}
	}
}