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
using System.Drawing.Drawing2D;
using Easychart.Finance;
using Easychart.Finance.Objects;
using Easychart.Finance.DataProvider;

namespace WebDemos.Objects
{
	/// <summary>
	/// Summary description for DynamicObject.
	/// </summary>
	public class DynamicObject : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.CheckBox cbPriceLabel;
		protected System.Web.UI.WebControls.CheckBox cbArrowLine;
		protected System.Web.UI.WebControls.TextBox tbArrowStopDate;
		protected System.Web.UI.WebControls.TextBox tbArrowStartPrice;
		protected System.Web.UI.WebControls.TextBox tbArrowStopPrice;
		protected System.Web.UI.WebControls.TextBox tbArrowStartDate;
		protected System.Web.UI.WebControls.CheckBox cbRegression;
		protected System.Web.UI.WebControls.TextBox tbRegStartDate;
		protected System.Web.UI.WebControls.TextBox tbRegStopDate;
		protected System.Web.UI.WebControls.Button btnDraw;
		protected System.Web.UI.WebControls.TextBox tbPriceDate;
		protected System.Web.UI.WebControls.TextBox tbLablePrice;
		protected System.Web.UI.WebControls.ImageButton ibChart;
	
		
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
			this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private ObjectPoint GetObjectPoint(string D,string P)
		{
			return new ObjectPoint(DateTime.Parse(D).ToOADate(),double.Parse(P));
		}

		private void btnDraw_Click(object sender, System.EventArgs e)
		{
			IDataManager idm = new DBDataManager();
			FormulaChart fc = FormulaChart.CreateChart(idm["MSFT"]);
			fc.EndTime = new DateTime(2004,1,9);
			fc.StartTime = fc.EndTime.AddMonths(-8);

			ObjectManager om = ObjectManager.FromChart(fc);
			if (cbPriceLabel.Checked)
			{
				LabelObject lo = new LabelObject();
				lo.InitPriceDateLabel();
				lo.ControlPoints[0] = GetObjectPoint(tbPriceDate.Text,tbLablePrice.Text);
				om.AddObject(lo);
			};

			if (cbArrowLine.Checked)
			{
				LineObject lo = new LineObject();
				lo.LinePen.Width = 5;
				lo.LinePen.Color = Color.Red;
				lo.LinePen.Alpha = 100;
				lo.LinePen.DashStyle = DashStyle.DashDotDot;
				lo.InitArrowCap();
				lo.ControlPoints[0] = GetObjectPoint(tbArrowStartDate.Text,tbArrowStartPrice.Text);
				lo.ControlPoints[1] = GetObjectPoint(tbArrowStopDate.Text,tbArrowStopPrice.Text);
				om.AddObject(lo);
			}

			if (cbRegression.Checked)
			{
				LinearRegressionObject lro = new LinearRegressionObject();
				lro.InitChannel();
				lro.ControlPoints[0] = GetObjectPoint(tbRegStartDate.Text,"10");
				lro.ControlPoints[1] = GetObjectPoint(tbRegStopDate.Text,"10");
				lro.ShowAuxLine = true;
				om.AddObject(lro);
			}
			ibChart.ImageUrl = fc.SaveToWeb(800,600);
		}
	}
}
