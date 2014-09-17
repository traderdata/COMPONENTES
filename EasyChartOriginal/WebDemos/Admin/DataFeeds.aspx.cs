using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Globalization;
using EasyTools;
using Easychart.Finance.DataClient;
using Easychart.Finance.DataProvider;

namespace WebDemos.Admin
{
	public enum DownloadMode {DownloadIfNoData,DownloadAndOverride}

	/// <summary>
	/// Summary description for DataFeeds.
	/// </summary>
	public class DataFeeds : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DropDownList ddlDataFeed;
		protected System.Web.UI.WebControls.Button btnImportSymbol;
		protected System.Web.UI.WebControls.DropDownList ddlExchange;
		protected System.Web.UI.WebControls.TextBox tbUsername;
		protected System.Web.UI.WebControls.TextBox tbPassword;
		protected System.Web.UI.WebControls.Label lUsername;
		protected System.Web.UI.WebControls.Label lPassword;
		protected System.Web.UI.WebControls.Label lMsg;
		protected System.Web.UI.WebControls.Literal lDescription;
		protected System.Web.UI.WebControls.HyperLink hlUrl;
		
		protected System.Web.UI.WebControls.Button btnImportHistorical;
		protected System.Web.UI.WebControls.Button btnImportEod;
		protected System.Web.UI.WebControls.TextBox tbStartDate;
		protected System.Web.UI.WebControls.TextBox tbEndDate;
		protected System.Web.UI.HtmlControls.HtmlTableRow trMsg;
		protected System.Web.UI.HtmlControls.HtmlTableRow trFunction;
		protected System.Web.UI.WebControls.Label lFunction;

		static private DataClientBase[] dcbs;
		private DataClientBase dcbSelected;
		static public string Msg;
		protected System.Web.UI.WebControls.Button btnRefresh;
		static private string FuncMsg;
		protected System.Web.UI.WebControls.Button btnStop;
		static private DateTime FuncStartTime;
		protected System.Web.UI.WebControls.Button btnDeleteExchange;
		protected System.Web.UI.WebControls.Label lAutoUpdateEodString;
		protected System.Web.UI.WebControls.DropDownList ddlDownloadMode;
		protected System.Web.UI.WebControls.TextBox tbHistoryStart;
		static private Thread tCurrent;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack) 
			{
				tbStartDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
				tbEndDate.Text = tbStartDate.Text;
			}
		}

		static private void LoadDataFeeds()
		{
			if (dcbs==null) 
			{
				dcbs = DataClientBase.GetAllDataFeeds();
			}
		}

		private string ReadParam(string key)
		{
			string s = Request.Form[key];
			if (s==null || s=="")
			{
				HttpCookie hc = Request.Cookies[key];
				if (hc!=null)
					s = hc.Value;
			}
			return s;
		}

		private void SetControlCookie(TextBox tb)
		{
			Response.SetCookie(new HttpCookie(tb.ID,tb.Text));
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			LoadDataFeeds();
		
			dcbSelected = null;
			string SelectedFeed = ReadParam(ddlDataFeed.ID);

			if (dcbs!=null)
				foreach(DataClientBase dcb in dcbs)
					if (SelectedFeed==dcb.GetType().ToString())
						dcbSelected =  dcb;
			
			ddlDataFeed.Items.Clear();
			foreach(DataClientBase dcb in dcbs) 
			{
				ListItem li = new ListItem(dcb.ToString(),dcb.GetType().ToString());
				li.Selected=dcb==dcbSelected;
				ddlDataFeed.Items.Add(li);
			}
				
			ddlExchange.Items.Clear();

			if (dcbSelected==null && dcbs.Length>0)
				dcbSelected = dcbs[0];

			if (dcbSelected!=null) 
			{
				Response.SetCookie(new HttpCookie(ddlDataFeed.ID,dcbSelected.GetType().ToString()));
				string[] ssExchanges = dcbSelected.GetExchanges();
				ddlExchange.Visible = ssExchanges!=null;
				btnImportSymbol.Enabled = ssExchanges!=null;
				btnDeleteExchange.Enabled = ssExchanges!=null;
				if (ssExchanges!=null) 
					foreach(string s in ssExchanges) 
					{
						int i = s.IndexOf('=');
						string r1 = s;
						string r2 = s;
						if (i>=0) 
						{
							r1 = s.Substring(0,i);
							r2 = s.Substring(i+1);
						}
						ddlExchange.Items.Add(new ListItem(r2,r1));
					}
				lDescription.Text = dcbSelected.Description;
				hlUrl.NavigateUrl = dcbSelected.RegURL;
				hlUrl.Text = dcbSelected.RegURL;
			}
			ddlDownloadMode.Items.Clear();
			string[] ss = Enum.GetNames(typeof(DownloadMode));

			for(int i=0; i<ss.Length; i++)
				ddlDownloadMode.Items.Add(new ListItem(ss[i],i.ToString()));

			tbUsername.Text = ReadParam(tbUsername.ID);
			tbPassword.Text = ReadParam(tbPassword.ID);

			SetControlCookie(tbUsername);
			SetControlCookie(tbPassword);
			
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			this.btnImportSymbol.Click += new System.EventHandler(this.btnImportSymbol_Click);
			this.btnImportEod.Click += new System.EventHandler(this.btnImportEod_Click);
			this.btnImportHistorical.Click += new System.EventHandler(this.btnImportHistorical_Click);
			this.btnDeleteExchange.Click += new System.EventHandler(this.btnDeleteExchange_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.DataFeeds_PreRender);

		}
		#endregion

		static private bool DataFeedAvailable(DataClientBase dcb,string Username,string Password)
		{
			if (dcb==null)
			{
				Msg = "No selected data feed";
				return false;
			}

			if (!dcb.Logined)
				dcb.Login(Username,Password);
			bool b = dcb.Logined || !dcb.NeedLogin;
			if (!b)
				Msg = "Invalid username/password";
			return b;
		}

		static public void ImportSymbol(DataClientBase dcb,string Username,string Password,string Exchanges)
		{
			if (FuncMsg==null)
			{
				FuncMsg = "Importing symbols, Data Feed = "+dcb+";Exchanges="+Exchanges;
				FuncStartTime = DateTime.Now;
				try
				{
					if (DataFeedAvailable(dcb,Username,Password))
					{
						string[] ss = dcb.LookupSymbols("",Exchanges,"","");
						DataManagerBase dmb = Utils.GetDataManager(Config.DefaultDataManager);
						int succ;
						int failed;
						dmb.SaveSymbolList(ss,out succ,out failed);
						Msg = "Import symbols:exchanges="+Exchanges+";succ="+succ+";failed="+failed;
					}
				} 
				finally
				{
					FuncMsg = null;
				}
			}
		}

		private void btnImportSymbol_Click(object sender, System.EventArgs e)
		{
			BackgroundExecute(
				new Function("WebDemos.Admin.DataFeeds","ImportSymbol",
				new object[]{dcbSelected,tbUsername.Text,tbPassword.Text,ddlExchange.SelectedValue.ToString()}));

			//ImportSymbol(dcbSelected,tbUsername.Text,tbPassword.Text,ddlExchange.SelectedValue.ToString());
		}

		static public void ImportHistoricalData(DataClientBase dcb,string Username,string Password,DateTime StartTime,DateTime EndTime,DownloadMode DownloadMode)
		{
			if (FuncMsg==null)
			{
				FuncMsg = "Importing historical data, Data Feed = "+dcb+";Mode="+DownloadMode+";Start date="+StartTime.ToString("yyyy-MM-dd");
				FuncStartTime = DateTime.Now;
				try
				{
					if (DataFeedAvailable(dcb,Username,Password))
					{
						DataManagerBase dmb = Utils.GetDataManager(Config.DefaultDataManager);
						int i = 1;
						string[] ss = dmb.GetSymbolStrings(null,null,null);
						int succ = 0;
						int failed = 0;
						string LastError = "";
						if (ss.Length==0)
							Msg = "No symbols in current database";
						foreach(string s in ss)
							try
							{
								CommonDataProvider cdp;
								bool b = true;
								if (DownloadMode==DownloadMode.DownloadIfNoData)
								{
									cdp = (CommonDataProvider)dmb[s,1];
									b = cdp.Count==0;
								}
								if (b)
								{
									Msg = "Importing historical data "+i+"/"+ss.Length+";"+s+";succ="+succ+";failed="+failed+";LastError="+LastError;
									cdp = dcb.GetHistoricalData(s,StartTime,EndTime);
									if (cdp.Count!=0)
										dmb.SaveData(s,cdp,false);
								}
								succ++;
								i++;
							}
							catch(Exception e)
							{
								LastError = e.Message;
								failed++;
							}
					}
				}
				finally
				{
					FuncMsg = null;
				}
			}
		}

		private void btnImportHistorical_Click(object sender, System.EventArgs e)
		{
			BackgroundExecute(
				new Function("WebDemos.Admin.DataFeeds","ImportHistoricalData",
				new object[]{dcbSelected,
								tbUsername.Text,tbPassword.Text,
								DateTime.Parse(tbHistoryStart.Text),DateTime.Today.AddDays(1),
								(DownloadMode)int.Parse(ddlDownloadMode.SelectedValue)
							}));

//			ImportHistoricalData(dcbSelected,
//				tbUsername.Text,tbPassword.Text,
//				DateTime.Parse(tbHistoryStart.Text),DateTime.Today.AddDays(1),
//				(DownloadMode)int.Parse(ddlDownloadMode.SelectedValue));
		}

		static public bool ImportEod(DataClientBase dcb,string Username,string Password,DateTime StartTime,DateTime EndTime,string Exchanges)
		{
			if (FuncMsg==null)
			{
				FuncMsg = "Importing end of day data, Data Feed = "+dcb+
					";StartTime="+StartTime.ToString("yyyy-MM-dd")+";EndTime="+EndTime.ToString("yyyy-MM-dd")+
					";Exchanges="+Exchanges;
				FuncStartTime = DateTime.Now;
				try
				{
					int Days = 0;
					int Records = 0;
					DataManagerBase dmb = Utils.GetDataManager(Config.DefaultDataManager);

					Hashtable ht = new Hashtable();
					if (DataFeedAvailable(dcb,Username,Password))
					{
						int LocalMergeMax = 100000;
						int LocalMerge = 0;
						for(DateTime D=StartTime; D<=EndTime; D=D.AddDays(1))
						{
							Msg = "Downloading end of day data "+D.ToString("yyyy-MM-dd");
							DataPacket[] dps = dcb.GetEodData(Exchanges,null,D);
							Days++;
							if (dps!=null)
								foreach(DataPacket dp in dps) 
								{
									CommonDataProvider cdp = (CommonDataProvider)ht[dp.Symbol];
									if (cdp==null) 
									{
										cdp = CommonDataProvider.Empty;
										ht[dp.Symbol] = cdp;
									}
									LocalMerge++;
									Records++;
									cdp.Merge(dp);
								}
							if (LocalMerge<LocalMergeMax && D!=EndTime)
								continue;

							if (ht.Count!=0)
							{
								int i=0;
								foreach(string s in ht.Keys) 
									try
									{
										Msg = "Importing end of day data "+(++i)+"/"+ht.Count;
										CommonDataProvider cdp = (CommonDataProvider)ht[s];
										dmb.UpdateEod(s,cdp);
										if ((i % 10)==0)
											GC.Collect(); //avoid hosting recolloect the web application
									}
									catch(Exception e)
									{
										Msg = e.ToString();
										Tools.Log("ImportEod:"+s+":"+e.Message);
									}
							
								ht.Clear();
								LocalMerge = 0;
							}
						}
						Msg = "Update success , total "+Days+" days , and "+Records+" records";
						return Records!=0;
					}
				} 
				finally
				{
					FuncMsg = null;
				}
			}
			return false;
		}

		static public bool ImportEod(string DataClientName,string Username,string Password,string Exchanges)
		{
			try
			{
				LoadDataFeeds();
				if (dcbs!=null)
				{
					DataClientBase dcb = null;
					foreach(DataClientBase d in dcbs)
						if (d.GetType().ToString()==DataClientName)
							dcb = d;

					dcb.Proxy = Config.WebProxy;

					DateTime D = DateTime.UtcNow.AddHours(-4).Date;
					return ImportEod(dcb,Username,Password,D,D,Exchanges);
				}
			}
			catch(Exception e)
			{
				Tools.Log("Import Eod:"+e.ToString());
			}
			return false;
		}

		private void btnImportEod_Click(object sender, System.EventArgs e)
		{
			BackgroundExecute(
				new Function("WebDemos.Admin.DataFeeds","ImportEod",
				new object[]{dcbSelected,
								tbUsername.Text,tbPassword.Text,
								DateTime.Parse(tbStartDate.Text),DateTime.Parse(tbEndDate.Text),ddlExchange.SelectedValue}));

//			ImportEod(dcbSelected,
//				tbUsername.Text,tbPassword.Text,
//				DateTime.Parse(tbStartDate.Text),DateTime.Parse(tbEndDate.Text),ddlExchange.SelectedValue);
		}

		private void BackgroundExecute(Function f)
		{
			if (FuncMsg==null)
			{
				tCurrent = new Thread(new ThreadStart(f.Execute));
				tCurrent.Start();
				Thread.Sleep(100);
			}
		}

		private void DataFeeds_PreRender(object sender, EventArgs e)
		{
			lMsg.Text = Msg;
			lFunction.Text = FuncMsg;
			if (FuncMsg!=null)
				lFunction.Text ="Running ("+(DateTime.Now-FuncStartTime).TotalSeconds.ToString("f2")+"s) "+FuncMsg;
			bool b =FuncMsg==null;
			btnStop.Visible = !b;
			foreach(Control c in trFunction.Controls[0].Controls)
				if (c is WebControl)
					(c as WebControl).Enabled = b;
			
			tbPassword.Attributes["value"] = tbPassword.Text;
			lAutoUpdateEodString.Text = "&lt;add key=\"ServiceX\" value=\"WebDemos.Admin.DataFeeds.ImportEod("+ddlDataFeed.SelectedValue+","+tbUsername.Text+","+tbPassword.Text+","+ddlExchange.SelectedValue+"),19:10(-4)\"/&gt;";
			if (b && dcbSelected!=null) 
			{
				btnDeleteExchange.Enabled = ddlExchange.Visible;
				btnImportSymbol.Enabled = dcbSelected.SupportSymbolList;
				btnImportEod.Enabled = dcbSelected.SupportEod;
				tbStartDate.Enabled = dcbSelected.SupportEod;
				tbEndDate.Enabled = dcbSelected.SupportEod;
				if (!dcbSelected.SupportEod)
					lAutoUpdateEodString.Text = "NO";
				tbUsername.Enabled = dcbSelected.NeedLogin;
				tbPassword.Enabled = dcbSelected.NeedLogin;
			}
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
			if (tCurrent!=null)
			{
				tCurrent.Abort();
				tCurrent.Join();
				tCurrent = null;
			}
		}

		private void btnDeleteExchange_Click(object sender, System.EventArgs e)
		{
			DataManagerBase dmb = Utils.GetDefaultDataManager();
			Msg = dmb.DeleteSymbols(ddlExchange.SelectedValue,null,false,false,false)+" symbols deleted";
		}
	}

	internal class Function
	{
		public string ClassName;
		public string FuncName;
		public object[] Params;

		public Function(string ClassName,string FuncName,object[] Params)
		{
			this.ClassName = ClassName;
			this.FuncName = FuncName;
			this.Params = Params;
		}

		public void Execute()
		{
			try
			{
				Type t = Type.GetType(ClassName);
				t.InvokeMember(FuncName,BindingFlags.Static | BindingFlags.Public |  BindingFlags.InvokeMethod,
					null,null,Params);
			}
			catch (Exception e)
			{
				if (e.InnerException!=null)
					e = e.InnerException;

				DataFeeds.Msg = "Background service error:"+e;
			}
		}
	}
}
