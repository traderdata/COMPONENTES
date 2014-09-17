using System;
using System.Net;
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
using System.IO;

namespace WebDemos
{
	/// <summary>
	/// Summary description for TestInternet.
	/// </summary>
	public class TestInternet : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox tbURL;
		protected System.Web.UI.WebControls.Button btnFetch;
		protected System.Web.UI.WebControls.Button btnTestRead;
		protected System.Web.UI.WebControls.TextBox tbResult;
	
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
			this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
			this.btnTestRead.Click += new System.EventHandler(this.btnTestRead_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnFetch_Click(object sender, System.EventArgs e)
		{
			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(tbURL.Text);
			hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705; .NET CLR 1.1.4322)";
			hwr.Accept = "*/*";

			if (Config.WebProxy!=null && Config.WebProxy!="")
				hwr.Proxy = new WebProxy(Config.WebProxy);

			HttpWebResponse hws= (HttpWebResponse)hwr.GetResponse();
			StreamReader sr  = new StreamReader(hws.GetResponseStream(),Encoding.ASCII);
			StringBuilder sb = new StringBuilder();
			while (sr.Peek()>0)
				sb.Append(sr.ReadLine());
			tbResult.Text = sb.ToString();
		}

		private void btnTestRead_Click(object sender, System.EventArgs e)
		{
			using (StreamReader sr = File.OpenText(Server.MapPath("~/"+tbURL.Text)))
			{
				string s = sr.ReadToEnd();
				tbResult.Text = s;
			}
		}
	}
}
