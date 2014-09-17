namespace WebDemos
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Xml;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for RssClient.
	/// </summary>
	public class RssClient : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgNews;

		protected System.Web.UI.WebControls.Label lTitle;
		protected System.Web.UI.WebControls.HyperLink hlTitle;
	
		private string rssUrl;
		public string RssUrl
		{
			get
			{
				return rssUrl;
			}
			set
			{
				rssUrl = value;
			}
		}

		private bool showDate;
		public bool ShowDate
		{
			get
			{
				return showDate;
			}
			set
			{
				showDate = value;
			}
		}

		private string title;
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string s = Utils.DownloadString(rssUrl);
				XmlDocument xd = new XmlDocument();
				xd.LoadXml(s);
				XmlNode xn = xd.SelectSingleNode("/rss/channel/title");
				if (title!="" && title!=null)
					lTitle.Text = title;
				else if (xn!=null)
					lTitle.Text = xn.InnerText;

				xn = xd.SelectSingleNode("/rss/channel/image/url");
				if (xn!=null)
					hlTitle.ImageUrl = xn.InnerText;

				xn = xd.SelectSingleNode("/rss/channel/image/link");
				
				if (xn!=null)
					hlTitle.NavigateUrl = xn.InnerText;
				hlTitle.Attributes["Style"] = "top:2px;right:2px;position:relative;";

				XmlNodeList xnl = xd.SelectNodes("/rss/channel/item");
				DataTable dt = new DataTable();
				dt.Columns.Add("title");
				dt.Columns.Add("description");
				dt.Columns.Add("link");
				dt.Columns.Add("pubdate");
				if (xnl!=null)
					foreach(XmlNode xn1 in xnl)
					{
						DataRow dr = dt.NewRow();
						foreach(XmlNode xn2 in xn1.ChildNodes)
						{
							string r = xn2.Name;
							if (dt.Columns.Contains(r))
								dr[r] = xn2.InnerText;
						}
						dt.Rows.Add(dr);
					}
				dgNews.DataSource = dt;
				dgNews.DataBind();
			} 
			catch
			{
			}
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
