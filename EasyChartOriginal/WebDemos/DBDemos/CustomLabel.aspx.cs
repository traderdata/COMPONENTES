using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using Easychart.Finance.DataProvider;
using Easychart.Finance;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for CustomLabel.
	/// </summary>
	public class CustomLabel : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox tbStart;
		protected System.Web.UI.WebControls.TextBox tbEnd;
		protected System.Web.UI.WebControls.Button btnOK;
		protected System.Web.UI.WebControls.TextBox tbStartDate;
		protected System.Web.UI.WebControls.TextBox tbEndDate;
		protected System.Web.UI.WebControls.TextBox tbDate;
		protected System.Web.UI.WebControls.TextBox tbText;
		protected System.Web.UI.WebControls.DropDownList ddlDataType;
		protected System.Web.UI.WebControls.CheckBox cbAddPrice;
		protected System.Web.UI.WebControls.DropDownList ddlAlign;
		protected System.Web.UI.WebControls.DropDownList ddlSkin;
		protected System.Web.UI.WebControls.TextBox tbSymbol;
		protected System.Web.UI.WebControls.HyperLink hlChart;
		protected System.Web.UI.WebControls.Label lWidth;
		protected System.Web.UI.WebControls.Label lHeight;
		private FormulaChart Chart;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Bind Data 
			DataManagerBase dmb = Utils.GetDataManager(Config.DefaultDataManager);

			// Create Chart
			Chart = FormulaChart.CreateChart(
				"Main(3);VOLMA","MA(14);MA(28)",
				dmb[tbSymbol.Text],ddlSkin.SelectedItem.Value);
			Chart.PriceLabelFormat = "{CODE}";
			
			// Set Custom Event Handler
			Chart.NativePaint+=new NativePaintHandler(fc_NativePaint);

			int Width = 800;
			int Height = 600;
			if (lWidth!=null)
				Width = Tools.ToIntDef(lWidth.Text,Width);
			if (lHeight!=null)
				Height = Tools.ToIntDef(lHeight.Text,Height);
			
			// Show Chart
			hlChart.ImageUrl = Chart.SaveToWeb(Width,Height,new Rectangle(0,18,Width,Height-18*2),ImageFormat.Png);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			tbStartDate.Text = DateTime.Today.AddDays(-10).ToString("yyyy-MM-dd");
			tbEndDate.Text = DateTime.Today.AddDays(-40).ToString("yyyy-MM-dd");
			tbDate.Text = DateTime.Today.AddDays(-50).ToString("yyyy-MM-dd");
			Utils.AddSkinToDropList(ddlSkin);

			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void fc_NativePaint(object sender, NativePaintArgs e)
		{
			// Draw Color Band
			Brush B = new SolidBrush(Color.FromArgb(64,Color.Yellow));
			PointF p1 = Chart.GetPointAt(DateTime.Parse(tbStartDate.Text),0);
			PointF p2 = Chart.GetPointAt(DateTime.Parse(tbEndDate.Text),0);
			e.Graphics.FillRectangle(B,RectangleF.FromLTRB(p2.X,e.Rect.Top,p1.X,e.Rect.Bottom));

			// Draw Label
			// Get Point of (Date,Price)
			DateTime D = DateTime.Parse(tbDate.Text);
			double Price = Chart.GetPriceAt(D,ddlDataType.SelectedItem.Value);
			PointF p = Chart.GetPointAt(D,Price);
			if (p!=PointF.Empty)
			{
				// Create ObjectLabel
				ObjectLabel ol = new ObjectLabel();
				// Set Stick Align
				ol.StickAlignment = (StickAlignment)Enum.Parse(typeof(StickAlignment),ddlAlign.SelectedItem.Value,true);
				// Set Color and Pos
				ol.BackColor = Color.FromArgb(255,192,64);
				ol.BorderColor = Color.Black;
				ol.Left = (int)p.X;
				ol.Top = (int)p.Y;
				if (tbText.Text!=null)
				{
					// Set Text
					ol.Text = tbText.Text;
					if (cbAddPrice.Checked)
						ol.Text += "\n"+D.ToString("yyyy-MM-dd")+"\n"+Price.ToString("c2");
					// Draw Label
					ol.Draw(e.Graphics);
				}
			}

			// Draw Title & Footer
			Font F = new Font("Verdana",10,FontStyle.Bold);
			StringFormat sf = new StringFormat(StringFormat.GenericDefault);
			sf.Alignment = StringAlignment.Center;

			// Draw Title
			RectangleF R = new RectangleF(0,0,Chart.Rect.Width,18);
			e.Graphics.FillRectangle(Brushes.White,R);
			e.Graphics.DrawString(Chart.DataProvider.GetStringData("Name")+"("+Chart.DataProvider.GetStringData("Code")+")",
				F,Brushes.Black,R,sf);

			// Draw Footer
			R = new RectangleF(0,Chart.Rect.Height+18,Chart.Rect.Width,18);
			e.Graphics.FillRectangle(Brushes.White,R);
			e.Graphics.DrawString(Config.CompanyName+"("+Config.URL+")",
				F,Brushes.Black,R,sf);
		}
	}
}