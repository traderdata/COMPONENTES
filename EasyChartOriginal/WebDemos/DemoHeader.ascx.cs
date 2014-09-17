namespace WebDemos
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	public abstract class DemoHeader : System.Web.UI.UserControl
	{
	
//		public string Path 
//		{
//			get 
//			{
//				string s = HttpRuntime.AppDomainAppVirtualPath;
//				if (!s.EndsWith("/"))
//					s +="/";
//				return s;
//			}
//		}

		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
