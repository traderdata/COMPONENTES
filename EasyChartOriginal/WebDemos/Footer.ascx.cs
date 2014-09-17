namespace WebDemos
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for Footer.
	/// </summary>
	public class Footer : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lPerformance;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		private void Footer_PreRender(object sender, System.EventArgs e)
		{
			if (lPerformance!=null)
				lPerformance.Text = ((long)(DateTime.Now.Ticks-Context.Timestamp.Ticks)/10000).ToString("0 ms");
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
			this.PreRender += new System.EventHandler(this.Footer_PreRender);

		}
		#endregion
	}
}
