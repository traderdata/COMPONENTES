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
using System.IO;
using System.Web.Caching;
using System.Text;
using System.Drawing.Imaging;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos
{
	/// <summary>
	/// Summary description for Custom.
	/// </summary>
	public class Extern : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox tbSymbol;
		protected System.Web.UI.WebControls.TextBox tbLine1;
		protected System.Web.UI.WebControls.TextBox tbLine2;
		protected System.Web.UI.WebControls.TextBox tbName1;
		protected System.Web.UI.WebControls.TextBox tbName2;
		protected System.Web.UI.WebControls.Image iChart;
		protected System.Web.UI.WebControls.Button btnUpdate;
		Random Rnd = new Random();

		double[] TextToDoubleArray(string s) 
		{
			string[] ss = s.Trim().Split('\r');
			double[] dd = new double[ss.Length];
			for(int i=0; i<ss.Length; i++)
				dd[dd.Length-1-i] = Tools.ToDoubleDef(ss[i],0);
			return dd;
		}

		void GetDateAndData(string s,out double[] Date,out double[] Data)
		{
			string[] ss = s.Trim().Split('\r');
			Date  = new double[ss.Length];
			Data = new double[ss.Length];

			for(int i=0; i<ss.Length; i++) 
			{
				string[] sss = ss[i].Split('=');
				if (sss.Length==2)
				{
					Date[ss.Length-1-i] = Tools.ToDateDef(sss[0].Trim(),"yyyy-MM-dd",DateTime.Now).ToOADate();
					Data[ss.Length-1-i] = Tools.ToDoubleDef(sss[1],0);
				}
			}
		}

		private void BindChart()
		{
			FormulaChart fc = new FormulaChart();
			FML.NATIVE.CustomFormula cf = new FML.NATIVE.CustomFormula("Custom");
			cf.Add(tbName1.Text,TextToDoubleArray(tbLine1.Text));

			double[] Date;
			double[] Data;
			GetDateAndData(tbLine2.Text,out Date,out Data);
			cf.Add(tbName2.Text,Date,Data);

			fc.AddArea("MAIN",2);
			fc.AddArea(cf);
			fc.SetSkin(FormulaSkin.GetSkinByName(Config.DefaultSkin));

			DBDataManager ddm = new DBDataManager();
			fc.DataProvider = ddm[tbSymbol.Text];

			FormulaBase fb = fc[0].Formulas[0];
			fb.Name = fc.DataProvider.GetStringData("Code")+"("+
				fc.DataProvider.GetStringData("Name")+")";
			fb.TextInvisible = false;

			iChart.ImageUrl= "../ImageFromCache.aspx?CacheId="+fc.SaveToImageStream(600,400,ImageFormat.Png,0,0);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			//if (!IsPostBack)
			BindChart();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			StringBuilder sb1= new StringBuilder();
			StringBuilder sb2 = new StringBuilder();
			double d1 = 50;
			double d2 = 50;
			for(int i=0; i<200; i++)
			{
				d1 +=Rnd.NextDouble()*4-2;
				d2 +=Rnd.NextDouble()*4-2;
				sb1.Append(d1.ToString("f2"));
				sb1.Append("\r\n");
				sb2.Append(DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd")+"="+d2.ToString("f2"));
				sb2.Append("\r\n");
			}
			tbLine1.Text = sb1.ToString();
			tbLine2.Text = sb2.ToString();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
