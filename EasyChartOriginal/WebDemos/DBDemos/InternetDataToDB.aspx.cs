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
using System.Net;
using System.IO;
using System.Text;
using System.Globalization;
using EasyDb;
using WebDemos.Code;
using System.Data.SqlClient;
using Easychart.Finance.DataProvider;

namespace WebDemos
{
	public class InternetDataToDB : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lExchangeMsg;
		protected System.Web.UI.WebControls.TextBox tbDate;
		protected System.Web.UI.WebControls.Button btnUpdate;
		protected System.Web.UI.WebControls.DropDownList ddlExchange;
		protected System.Web.UI.WebControls.Button btnGet;
		protected System.Web.UI.WebControls.TextBox tbAllQuotes;
		protected System.Web.UI.WebControls.Button btnUpdateWeb;
		protected System.Web.UI.WebControls.Button btnDownload;
		string Exchange;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				tbDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
			Exchange = ddlExchange.SelectedItem.Value;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN：该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.btnUpdateWeb.Click += new System.EventHandler(this.btnUpdateWeb_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private string[] DownloadData(string URL) 
		{
			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(URL);
			hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705; .NET CLR 1.1.4322)";
			hwr.Accept = "*/*";
			hwr.KeepAlive = false;
			hwr.Referer = "http://www.eoddata.com";
			hwr.Headers["Accept-Encoding"] = "gzip, deflate";
			hwr.Headers["Accept-Language"] = "zh-cn";
			hwr.Headers["Pragma"] = "no-cache";
			hwr.Headers["Cache-Control"] = "no-cache";
			hwr.CookieContainer = new CookieContainer();
			hwr.CookieContainer.Add(new Cookie("chartdata","e=support%40easychart%2Enet","/","www.eoddata.com"));
			hwr.CookieContainer.Add(new Cookie("chartdata","e=support%40easychart%2Enet","/","eoddata.com"));

			HttpWebResponse hws= (HttpWebResponse)hwr.GetResponse();
			StreamReader sr = new StreamReader(hws.GetResponseStream(),Encoding.ASCII);
			ArrayList al = new ArrayList();
			while (true) 
			{
				string r  = sr.ReadLine();
				if (r==null) break;
				al.Add(r);
			}
			sr.Close();
			hws.Close();
			return (string[])al.ToArray(typeof(string));
		}

		private void btnGet_Click(object sender, System.EventArgs e)
		{
			string[] ss = DownloadData("http://eoddata.com/SymbolList.asp?e="+Exchange);

			DbParam[] dps = new DbParam[]{
											 new DbParam("@QuoteCode",DbType.String,""),
											 new DbParam("@QuoteName",DbType.String,""),
											 new DbParam("@Exchange",DbType.String,Exchange),
			};
			int succ=0;
			int failed=0;
			BaseDb bd = DB.Open(false);
			try
			{
				for(int i=1; i<ss.Length; i++) 
				{
					string[] rr = ss[i].Split('\t');
					if (rr.Length!=2) continue;
					if (Exchange=="INDEX")
						dps[0].Value = "^"+rr[0].Trim();
					else dps[0].Value = rr[0].Trim();
					dps[1].Value = rr[1].Trim();
					try 
					{
						bd.DoCommand("insert into stockdata (QuoteCode,QuoteName,Exchange) values (?,?,?)",dps);
						succ++;
					} 
					catch
					{
						failed++;
					}
				}
			} 
			finally 
			{
				bd.Close();
			}
			lExchangeMsg.Text ="succ: "+succ+"; failed "+failed;
		}

		private byte[] MergeOneQuote(byte[] bs,DataPackage dp) 
		{
			float[] fs = new float[bs.Length / 4+7];
			System.Buffer.BlockCopy(bs,0,fs,0,bs.Length);
			DateTime d2 = DateTime.FromOADate(dp.DATE);
			int Count = fs.Length/7-1;
			for(int i=Count-1; i>=-1; i--)
			{
				DateTime d1 = DateTime.MinValue;
				if (i>-1)
					d1 = DateTime.FromOADate(fs[i*7]);

				if (d1<=d2) 
				{
					if (d1<d2) 
					{
						if (i<Count-1 && Count>0)
							System.Buffer.BlockCopy(fs,(i+1)*7*4,fs,(i+2)*7*4,(Count-i-1)*7*4);
						float[] fsCurrent = dp.GetFloat();
						System.Buffer.BlockCopy(fsCurrent,0,fs,(i+1)*7*4,7*4);
						bs = new byte[fs.Length*4];
						System.Buffer.BlockCopy(fs,0,bs,0,fs.Length*4);
					}
					return bs;
				};
			}
			return bs;
		}

		private void MergeOneDay(string[] ss) 
		{
			Hashtable ht = new Hashtable();
			IFormatProvider fp =
				new System.Globalization.CultureInfo("en-US", true);
			for (int i=1; i<ss.Length; i++) 
			{
				string[] sss = ss[i].Split(',');
				ht.Add(sss[0],new DataPackage(
					(float)DateTime.Parse(sss[1],fp).ToOADate(),
					float.Parse(sss[2]),
					float.Parse(sss[3]),
					float.Parse(sss[4]),
					float.Parse(sss[5]),
					float.Parse(sss[6]),
					float.Parse(sss[5])));
			}
			
			DbParam[] dps = new DbParam[]{
											 new DbParam("@HistoryData",DbType.Binary,null),
											 new DbParam("@QuoteCode",DbType.String,null),
			};
			
			BaseDb UpdateDb = BaseDb.FromConfig("ConnStr");
			SQL bd = (SQL)DB.Open(false);
			try 
			{
				SqlDataReader sdr = bd.GetDataReader("select * from StockData");
				
				while (sdr.Read()) 
				{
					string QuoteCode = sdr["QuoteCode"].ToString();
					DataPackage dp =(DataPackage)ht[QuoteCode];
					if (dp!=null) 
					{
						object o = sdr["HistoryData"];
						if (o==DBNull.Value)
							o = new byte[0];
						byte[] bs = MergeOneQuote((byte[])o,dp);
						dps[0].Value = bs;
						dps[1].Value = QuoteCode;
						UpdateDb.DoCommand("update StockData set HistoryData=? where QuoteCode=?",dps);
					}
				}
				sdr.Close();
			} 
			finally 
			{
				bd.Close();
			}
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			string s = tbAllQuotes.Text;
			string[] ss = s.Split('\n');
			MergeOneDay(ss);
		}

		private void btnDownload_Click(object sender, System.EventArgs e)
		{
			DataTable dt = DB.GetDataTable("select QuoteCode,QuoteName from StockData order by QuoteCode");
			string URL = "http://table.finance.yahoo.com/table.csv?s={0}&d=8&e=13&f=2004&g=d&a=6&b=30&c=1980&ignore=.csv";
			WebClient wc = new WebClient();
			DbParam[] dps = new DbParam[]{
											 new DbParam("@HistoryData",DbType.Binary,null),
											 new DbParam("@QuoteCode",DbType.String,null),
			};
			int i=0;
			foreach(DataRow dr in dt.Rows) 
			{
				string QuoteCode = dr["QuoteCode"].ToString();
				string s = string.Format(URL,QuoteCode);
				byte[] bs = wc.DownloadData(s);
				CSVDataProvider cdpn = new CSVDataProvider(null,bs);
				bs = cdpn.SaveBinary();
				dps[0].Value = bs;
				dps[1].Value = QuoteCode;
				DB.DoCommand("update StockData set HistoryData=? where QuoteCode=?",dps);
				if (i++>40) break;
			}
		}

		private void btnUpdateWeb_Click(object sender, System.EventArgs e)
		{
			string s = DateTime.Parse(tbDate.Text).ToString("yyyyMMMdd",DateTimeFormatInfo.InvariantInfo);
			string[] ss = DownloadData("http://www.eoddata.com/Data.asp?e=NYSE&d="+s);
			MergeOneDay(ss);
		}
	}
}
