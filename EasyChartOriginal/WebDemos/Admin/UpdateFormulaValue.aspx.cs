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
using EasyTools;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace WebDemos.Admin
{
	/// <summary>
	/// Summary description for UpdateFormulaValue.
	/// </summary>
	public class UpdateFormulaValue : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox tbPullFormulas;
		protected System.Web.UI.WebControls.Button btnSave;
		protected System.Web.UI.WebControls.Button btnUpdate;
		protected System.Web.UI.WebControls.Label lStatus;
		private static DateTime StartTime = DateTime.MinValue;
		static Thread tUpdate = null;
		static string Msg = "";
	
		private void Bind() 
		{
			tbPullFormulas.Text = string.Join("\r\n",Config.AutoPullFormulaData.Split(';'));
			if (StartTime==DateTime.MinValue)
				lStatus.Text = "Stopped";
			else lStatus.Text = "Started:"+(DateTime.Now-StartTime).TotalSeconds.ToString("f2")+"s"+Msg;
			btnUpdate.Enabled = tUpdate==null;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack) Bind();
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
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		public static void UpdateFormula(object sender) 
		{
			if (StartTime>DateTime.MinValue) return;
			StartTime = DateTime.Now;
			try 
			{
				string[] ss = Config.AutoPullFormulaData.Split(';');
				string[] ssName = new string[ss.Length];
				string[] ssFormula = new string[ss.Length];
				FormulaBase[] fbs = new FormulaBase[ss.Length];
				int[] Ns = new int[ss.Length];
				int N = 0;
				for(int i=0; i<ss.Length; i++) 
				{
					ssName[i] = Utils.GetName(ss[i]);
					ssFormula[i] = Utils.GetValue(ss[i]);
					fbs[i] = FormulaBase.GetFormulaByName(ssFormula[i]);
					Ns[i] = fbs[i].DataCountAtLeast();
					Tools.Log(ssName[i]+"="+Ns[i]);
					N = Math.Max(N,Ns[i]);
				}

				DataManagerBase dmb = Utils.GetDataManager(Config.DefaultDataManager);
				DataTable dt = dmb.GetSymbolList(null,null,null);
				if (dt==null) return;

				DbParam[] dps = new DbParam[]{
												 new DbParam("@QuoteCode",DbType.String,""),
												 new DbParam("@FormulaName",DbType.String,""),
												 new DbParam("@FormulaValue",DbType.Double,0),
												 new DbParam("@CalculateTime",DbType.DateTime,DateTime.Now)
											 };

				DB.DoCommand("delete from FormulaValue");
				for(int j=0; j<dt.Rows.Count; j++)
					try
					{
						DataRow dr = dt.Rows[j];
						IDataProvider idp = dmb[dr[0].ToString(),N];

						dps[0].Value = dr[0].ToString();
						for(int i=0; i<fbs.Length; i++)
						{
							idp.MaxCount = Ns[i]+10;
							dps[1].Value = ssName[i];
							if (fbs[i]!=null)
							{
								FormulaData fd = fbs[i].GetFormulaData(idp,ssFormula[i]);
								if (fd.Length>0) 
								{
									double d = fd.LASTDATA;
									if (!double.IsNaN(d))
									{
										dps[2].Value = d;
										DB.DoCommand("insert into FormulaValue (QuoteCode,FormulaName,FormulaValue,CalculateTime) values (?,?,?,?)",dps);
									} 
									else if (ssFormula[i]!="NAME")
										Tools.Log("Insert value error:"+fbs[i].Name+";Data Length="+fd.Length+";"+dr[0]);
								}
							}
						}
						Msg = ";"+j+"/"+dt.Rows.Count+";"+dr[0];
					}
					catch (Exception e)
					{
						Tools.Log("Update Formula Values: "+dt.Rows[j][0]+","+e);
					}
			}
			catch(Exception e)
			{
				Tools.Log("Update Formula Values: "+e);
			}
			finally 
			{
				StartTime = DateTime.MinValue;
				tUpdate = null;
				Msg = "";
			}
		}

		public static void UpdateFormula() 
		{
			UpdateFormula(null);
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			//ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateFormula));
			if (tUpdate==null)
			{
				tUpdate = new Thread(new ThreadStart(UpdateFormula));
				tUpdate.Start();
				Thread.Sleep(100);
				Bind();
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			Config.AutoPullFormulaData = tbPullFormulas.Text.Trim().Replace("\r\n",";");
			//Bind();
		}
	}
}