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
using System.Threading;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos.Admin
{
	/// <summary>
	/// Summary description for UpdatePreScan.
	/// </summary>
	public class UpdatePreScan : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox tbPreScan;
		protected System.Web.UI.WebControls.Button btnSave;
		protected System.Web.UI.WebControls.Button btnPreScan;
		protected System.Web.UI.WebControls.Label lStatus;
		private static DateTime StartTime = DateTime.MinValue;

		private void Bind() 
		{
			tbPreScan.Text = 
				string.Join("\r\n",Config.PreScan.Split(';'));
			if (StartTime==DateTime.MinValue)
				lStatus.Text = "Stopped";
			else 
			{
				string s = "";
				if (Cache["PreScan"]!=null)
					s = Cache["PreScan"].ToString() ;
				lStatus.Text = "Started:"+(DateTime.Now-StartTime).TotalSeconds.ToString("f2")+"s; "+s;
			}
		}

		static private Hashtable htCache = new Hashtable();
		static private DateTime LastCacheTime = DateTime.MinValue;

		static public void ClearCache()
		{
			htCache.Clear();
			LastCacheTime = DateTime.Now;
		}

		static public IDataProvider GetDataProvider(DataManagerBase dmb, string Symbol,int N)
		{
			IDataProvider idp = null;
			if (Config.PrescanLoadToMemory) 
			{
				if ((DateTime.Now-LastCacheTime).TotalDays>1)
					ClearCache();
				idp = (IDataProvider)htCache[Symbol];
			}
			if (idp==null) 
			{
				idp = dmb[Symbol,N];
				if (Config.PrescanLoadToMemory) 
					htCache[Symbol] = idp;
			}
			return idp;
		}

		static private string GetWhere(string Field)
		{
			DateTime D = DateTime.Today;
			return " where ScanType>0 and "+Field+">='"+D.ToString("yyyy-MM-dd")+
				"' and "+Field+"<'"+D.AddDays(1).ToString("yyyy-MM-dd")+"'";
		}

		static public void PreScan(object Sender) 
		{
			Tools.Log("Prescan starting");
			if (StartTime>DateTime.MinValue) return;
			StartTime = DateTime.Now;
			try 
			{
				string[] PreExchange = Config.PreScanExchange.Split(';');
				for(int i=0; i<PreExchange.Length; i++)
					PreExchange[i] = Utils.GetPart1(PreExchange[i]);

				string[] PreScan = Config.PreScan.Split(';');
				Hashtable htConditionIdMap = new Hashtable();
				DbParam[] dpPreScan = new DbParam[]{
					new DbParam("@Condition",DbType.String,""),
					new DbParam("@Exchange",DbType.String,""),
					new DbParam("@StartTime",DbType.DateTime,DateTime.Now),
					new DbParam("@ScanType",DbType.Int32,1),
				};

				// Insert pre-defined scan to condition
				// Get condition id .
				BaseDb bd = DB.Open(false);
				try 
				{
					string s = bd.GetCommaValues("select ConditionId from Condition "+GetWhere("EndTime"),"");
					if (s!="") 
					{
						bd.DoCommand("delete from ScanedQuote where ConditionId in ("+s+")");
						bd.DoCommand("delete from Condition "+GetWhere("EndTime"));
					}

					Tools.Log("PreScan="+PreScan.Length+";PreExchange="+PreExchange.Length);

					for(int i=0; i<PreScan.Length; i++)
						for(int j=0; j<PreExchange.Length; j++)
						{
							dpPreScan[0].Value = Utils.GetName(PreScan[i]);
							dpPreScan[1].Value = PreExchange[j];
							dpPreScan[3].Value = Utils.GetParam(PreScan[i],"1");
							bd.DoCommand("insert into Condition (Condition,Exchange,StartTime,Scaned,ScanType) values (?,?,?,0,?)",dpPreScan);
						}

					DataTable dtPreScan = 
						bd.GetDataTable("select ConditionId,Condition,Exchange from Condition "+GetWhere("StartTime"),null, PreScan.Length*PreExchange.Length);
					foreach(DataRow dr in dtPreScan.Rows) 
						htConditionIdMap[dr["Condition"].ToString().Trim()+dr["Exchange"].ToString().Trim()] = dr["ConditionId"].ToString();
				} 
				finally
				{
					bd.Close();
				}

				Tools.Log("Get scan formulas");

				// Get scan formulas
				FormulaBase[] fbs = new FormulaBase[PreScan.Length];
				int[] Ns = new int[PreScan.Length];
				int N = 0;
				for(int i=0; i<fbs.Length; i++) 
				{
					fbs[i] = FormulaBase.GetFormulaByName("Scan."+Utils.GetValue(PreScan[i]));
					Tools.Log(fbs[i].FullName);
					if (fbs[i]!=null) 
					{
						Ns[i] = fbs[i].DataCountAtLeast();
						N = Math.Max(N,Ns[i]);
					}
				}
				if (Config.PrescanLoadToMemory) 
					N = Config.MaxDataForPull;

				Tools.Log("Pre-Scan- N = "+N);

				DataManagerBase dmb = Utils.GetDataManager(Config.DefaultDataManager);
				DataTable dt = dmb.GetStockList();
				if (dt==null) return;
				Tools.Log(dt.Rows.Count.ToString());

				// Scan
				int Progress=0;
				Hashtable htTotal = new Hashtable();
				Hashtable htResultCount = new Hashtable();
				ArrayList al = new ArrayList();
				try 
				{
					foreach(DataRow dr in dt.Rows)
						try
						{
							string Symbol = dr[0].ToString();

							IDataProvider idp = GetDataProvider(dmb,Symbol,N);

							if (!Utils.VerifyVolumeAndDate(idp))
								continue;
							string NowExchange = dr["Exchange"].ToString();
							foreach(string s in PreExchange) 
							{
								if (s.Length<=NowExchange.Length)
									if (string.Compare(s,NowExchange.Substring(0,s.Length),true)==0)
									{
										NowExchange = s;
										break;
									}
							}

							for(int j=0; j<fbs.Length; j++)
								try
								{
									if (fbs[j]!=null)
									{
										idp.MaxCount = Ns[j];
										FormulaPackage fp = fbs[j].Run(idp);
										string ConditionId = (string)htConditionIdMap[Utils.GetName(PreScan[j])+NowExchange];
										if (ConditionId!=null)
										{
											FormulaData fd = fp[fp.Count-1];
											if (fd.Length>0)
												if (fd.LASTDATA>0) 
												{
													al.Add(ConditionId+","+Symbol);
													htResultCount[ConditionId] = Utils.ObjPlusDef(htResultCount[ConditionId],1);
												}
											htTotal[ConditionId] = Utils.ObjPlusDef(htTotal[ConditionId],1);
										}
										Progress++;
										if ((Progress % 10)==0) 
											HttpRuntime.Cache["PreScan"] = Progress;
									}
								}
								catch (Exception e)
								{
									Tools.Log(Symbol+"/"+fbs[j]+"/"+ e);
								}
							Thread.Sleep(1);
						}
						catch (Exception e)
						{
							Tools.Log("Pre-scan symbol loop:"+e.Message);
						}
				}
				finally
				{
					Utils.BulkInsert(al);
				}
				
				// Update pre-scan conditions
				dpPreScan = new DbParam[]{
											 new DbParam("@Scaned",DbType.Int32,0),
											 new DbParam("@Total",DbType.Int32,0),
											 new DbParam("@ResultCount",DbType.Int32,0),
											 new DbParam("@EndTime",DbType.DateTime,DateTime.Now),
				};
				bd = DB.Open(false);
				try 
				{
					for(int i=0; i<PreScan.Length; i++)
						for(int j=0; j<PreExchange.Length; j++)
						{
							string ConditionId = (string)htConditionIdMap[Utils.GetName(PreScan[i])+PreExchange[j]];
							dpPreScan[0].Value = Utils.ObjDef(htTotal[ConditionId],0);
							dpPreScan[1].Value = Utils.ObjDef(htTotal[ConditionId],0);
							dpPreScan[2].Value = Utils.ObjDef(htResultCount[ConditionId],0);

							bd.DoCommand("update Condition set Scaned=?,Total=?,ResultCount=?,EndTime=? where ConditionId="+
								ConditionId,dpPreScan);
						}
				}
				finally
				{
					bd.Close();
				}
			} 
			catch (Exception e)
			{
				Tools.Log("Update pre-scan service:"+e.Message);
			}
			finally 
			{
				StartTime = DateTime.MinValue;
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
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnPreScan.Click += new System.EventHandler(this.btnPreScan_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnPreScan_Click(object sender, System.EventArgs e)
		{
			Tools.Log("PreScan button clicked");
			ThreadPool.QueueUserWorkItem(new WaitCallback(PreScan));
			Thread.Sleep(100);
			Bind();
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			Config.PreScan = tbPreScan.Text.Trim().Replace("\r\n",";");
			//Bind();
		}
	}
}
