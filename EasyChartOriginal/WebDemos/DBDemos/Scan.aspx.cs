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
using EasyTools;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using System.Reflection;
using System.Threading;
using System.Configuration;
using System.IO;

namespace WebDemos.DBDemos
{
	public class Scan : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DropDownList ddlExchange;
		protected System.Web.UI.WebControls.DropDownList ddlFormula;
		protected System.Web.UI.WebControls.Button btnScan;
		protected System.Web.UI.WebControls.Label lTotal;
		protected System.Web.UI.WebControls.Label lParam;
		protected System.Web.UI.WebControls.Label lFullName;
		protected System.Web.UI.WebControls.Label lCode;
		protected System.Web.UI.WebControls.Label lDescription;
		string CurrentFullName;
		string Exchange;
		string ConditionId;
		string Condition;
		FormulaProgram CurrentProgram;

		private void FindFormula(FormulaSpace fs,string Formula) 
		{
			bool b = ddlFormula.Items.Count==0;
			foreach(FormulaProgram fp in fs.Programs)
			{
				if (Formula==null)
				{
					if (CurrentProgram==null)
						CurrentProgram = fp;
				}
				else if (Formula==fp.Name) 
					CurrentProgram = fp;

				if (!IsPostBack && b)
					ddlFormula.Items.Add(new ListItem(fp.FullName,fp.Name));
			}
			
			foreach(FormulaSpace fsc in fs.Namespaces)
				FindFormula(fsc,Formula);
		}

		private void Bind()
		{
			string FmlFile = Config.PluginsDirectory+"Scan.fml";
			FormulaSpace fs = FormulaSpace.Read(FmlFile);
			string s = Request.Form[ddlFormula.UniqueID];
			if (s==null && ddlFormula.Items.Count>0)
				s = ddlFormula.Items[0].Value;

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

					lParam.Text +="<input Name="+r+" value="+Value+"></td><td>";
					lParam.Text +=fpm.MinValue+"</td><td>";
					lParam.Text +=fpm.MaxValue+"</td></tr>";
				}
				lParam.Text +="</table><br>";
				lFullName.Text = CurrentProgram.FullName;
				lDescription.Text = CurrentProgram.Description.Replace("\n","<br>");
				lCode.Text = Server.HtmlEncode(CurrentProgram.Code).Replace("\n","<br>");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				Bind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			if (ddlExchange.Items.Count==0)
			{
				DataManagerBase ddm = (DataManagerBase)Utils.GetDataManager(Config.DefaultDataManager);
				ddlExchange.DataSource = ddm.Exchanges;
				ddlExchange.DataBind();
			}

			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.ddlFormula.SelectedIndexChanged += new System.EventHandler(this.ddlFormula_SelectedIndexChanged);
			this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		public static void AddCache(string Key,object Value)
		{
			HttpRuntime.Cache.Remove(Key);
			HttpRuntime.Cache.Add(Key, Value,null,System.Web.Caching.Cache.NoAbsoluteExpiration,
				System.Web.Caching.Cache.NoSlidingExpiration,System.Web.Caching.CacheItemPriority.NotRemovable,null);
		}


		public static void Scanning(object o)
		{
			ScanId si =(ScanId)o;
			FormulaBase fb = FormulaBase.GetFormulaByName("Scan."+si.Condition);
			DataManagerBase dmb = (DataManagerBase)Utils.GetDataManager(Config.DefaultDataManager);

			if (fb!=null)
			{
				int N = fb.DataCountAtLeast();
				int M = N;
				if (Config.PrescanLoadToMemory) 
					N = FormulaBase.MaxForAllScan;
			
				string Where = "";
				if (si.Exchange!="")
					Where = " where Exchange = '"+si.Exchange+"'";
				//DataTable dt = DB.GetDataTable("select QuoteCode from StockData"+Where);
				string[] ss = dmb.GetSymbolStrings(si.Exchange,null,null);
				if (ss!=null)
				{
					DB.DoCommand("update Condition set Total=?,Scaned=0,StartTime=? where ConditionId=?",
						new DbParam[]{
										 new DbParam("@Total",DbType.Int32,ss.Length),
										 new DbParam("@StartTime",DbType.DateTime,DateTime.Now),
										 new DbParam("@ConditionId",DbType.Int32,si.ConditionId)
									 });

					int i=0;
					ArrayList al = new ArrayList();
					try 
					{
						foreach(string s in ss)
						{
							try
							{
								//ddm.AutoYahooToDB = false;
								//ddm.DownloadRealTimeQuote = false;
								IDataProvider idp = Admin.UpdatePreScan.GetDataProvider(dmb,s,N);
								idp.MaxCount = M;
								//IDataProvider idp = ddm[s,N];
								if (Utils.VerifyVolumeAndDate(idp)) 
								{
									FormulaPackage fp = fb.Run(idp);
									FormulaData fd = fp[fp.Count-1];
									if (fd.Length>0)
										if (fd.LASTDATA>0)
											al.Add(si.ConditionId+","+s);
								}
							}
							catch (Exception ex)
							{
								Tools.Log("Scanning:"+ex.Message);
							}
							i++;
							if ((i % 10)==0)
							{
								//HttpRuntime.Cache[si.ConditionId] = i;
								AddCache(si.ConditionId,i);
								if ((i % 100)==0)
									Thread.Sleep(1);
							}
						}
					}
					finally
					{
						Utils.BulkInsert(al);
					}
				
					//HttpRuntime.Cache[si.ConditionId] = ss.Length;
					AddCache(si.ConditionId,ss.Length);

					DB.DoCommand("update Condition set Scaned=?,ResultCount=?,EndTime=? where ConditionId="+si.ConditionId,
						new DbParam[]{
										 new DbParam("@Total",DbType.Int32,ss.Length),
										 new DbParam("@ResultCount",DbType.Int32,al.Count),
										 new DbParam("@EndDate",DbType.DateTime,DateTime.Now)
									 });
				}
			}
		}

		private void btnScan_Click(object sender, System.EventArgs e)
		{
			CurrentFullName = ddlFormula.SelectedItem.Value;
			Exchange = ddlExchange.SelectedItem.Value;
			
			string Param = "";
			foreach(string s in Request.Form)
				if (s.StartsWith("__Param")) 
				{
					if (Param!="")
						Param +=",";
					Param +=Request.Form[s]; //double.Parse()
				}
			Condition = CurrentFullName;
			if (Param!="")
				Condition = CurrentFullName+"("+Param+")";

			DbParam[] dps = new DbParam[]{
											new DbParam("@Condition",DbType.String,Condition),
											new DbParam("@Exchange",DbType.String,Exchange),
											new DbParam("@StartTime",DbType.DateTime,DateTime.Now.AddHours(-
												Tools.ToIntDef(ConfigurationSettings.AppSettings["ScanCacheTime"],0)))
										 };
			bool b = true;
			BaseDb bd = DB.Open(false);
			try 
			{
				DataRow dr = bd.GetFirstRow("select * from Condition where Condition=? and Exchange=? and StartTime>?",dps);
				if (dr!=null && !Config.KeepLatestScanResultOnly)
				{
					ConditionId = dr["ConditionId"].ToString();
					b = false;
				} 
				else 
				{
					dps[2] = null;
					if (Config.KeepLatestScanResultOnly) 
					{
						DataTable dt = bd.GetDataTable("select ConditionId from Condition where Condition=? and Exchange=?",dps);
						foreach(DataRow drDel in dt.Rows)
							bd.DoCommand("delete from ScanedQuote where ConditionId = "+drDel["ConditionId"]);
						bd.DoCommand("delete from Condition where Condition=? and Exchange=?",dps);
					}
					bd.DoCommand("insert into Condition (Condition,Exchange,Scaned) values (?,?,0)",dps);
					ConditionId = bd.GetFirstRow("select max(ConditionId) from Condition")[0].ToString();
				}
			}
			finally 
			{
				bd.Close();
			}

			if (b) 
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(Scanning),new ScanId(ConditionId,Condition,Exchange));
				Response.Redirect("Progress.aspx?ConditionId="+ConditionId);
			} 
			else 
			Response.Redirect("StockList.aspx?ConditionId="+ConditionId);
		}

		private void ddlFormula_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Bind();
		}
	}

	public class ScanId
	{
		public string ConditionId;
		public string Condition;
		public string Exchange;

		public ScanId(string ConditionId,string Condition,string Exchange)
		{
			this.Condition = Condition;
			this.ConditionId = ConditionId;
			this.Exchange = Exchange;
		}
	}
}