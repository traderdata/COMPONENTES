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
using System.Reflection;
using System.IO;
using System.Web.Caching;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace WebDemos
{
	public class Formula : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Literal lChart;
		protected System.Web.UI.WebControls.TextBox tbCode;
		protected System.Web.UI.WebControls.TextBox tbProgramScript;
		protected System.Web.UI.WebControls.TextBox tbParamName1;
		protected System.Web.UI.WebControls.TextBox tbDefValue1;
		protected System.Web.UI.WebControls.TextBox tbMinValue1;
		protected System.Web.UI.WebControls.TextBox tbMaxValue1;
		protected System.Web.UI.WebControls.TextBox tbParamName2;
		protected System.Web.UI.WebControls.TextBox tbDefValue2;
		protected System.Web.UI.WebControls.TextBox tbMinValue2;
		protected System.Web.UI.WebControls.TextBox tbMaxValue2;
		protected System.Web.UI.WebControls.TextBox tbParamName3;
		protected System.Web.UI.WebControls.TextBox tbDefValue3;
		protected System.Web.UI.WebControls.TextBox tbMinValue3;
		protected System.Web.UI.WebControls.TextBox tbMaxValue3;
		protected System.Web.UI.WebControls.TextBox tbParamName4;
		protected System.Web.UI.WebControls.TextBox tbDefValue4;
		protected System.Web.UI.WebControls.TextBox tbMinValue4;
		protected System.Web.UI.WebControls.TextBox tbMaxValue4;
		protected System.Web.UI.WebControls.TextBox tbFormulaName;
		protected System.Web.UI.WebControls.Button btnOK;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
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
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			//Create a Formula namespace
			FormulaSpace fms = new FormulaSpace("FML");

			//Create a Formula program , set Formula name and script code on the fly
			FormulaProgram fp = new FormulaProgram();
			fp.Name = tbFormulaName.Text;
			fp.Code = tbProgramScript.Text;

			//Add the script program to the Formula namespace
			fms.Programs.Add(fp);

			//Add parameters to Formula program
			for(int i=1; i<5; i++) 
			{
				if (Request.Form["tbParamName"+i]!="") 
				{
					fp.Params.Add(new FormulaParam(
						Request.Form["tbParamName"+i],
						Request.Form["tbDefValue"+i],
						Request.Form["tbMinValue"+i],
						Request.Form["tbMaxValue"+i],FormulaParamType.Double));
				}
			}

			try 
			{
				//Compile the Formula script on the fly
				Assembly a = fms.CompileInMemory();
				FormulaBase fb = FormulaBase.GetFormulaByName(a,fms.Name+"."+fp.Name);

				//Create YahooDataManager , Get stock data from yahoo.
				YahooDataManager ydm = new YahooDataManager();
				ydm.CacheRoot = HttpRuntime.AppDomainAppPath+"Cache\\";
				CommonDataProvider DataProvider = (CommonDataProvider)ydm[tbCode.Text];
			
				//Create financial chart instance
				FormulaChart fc = new FormulaChart();
				fc.PriceLabelFormat = "{CODE}";
				fc.LatestValueType = LatestValueType.Custom;
				fc.AddArea("MAIN",3);
				fc.AddArea(fb);
				fc.DataProvider = DataProvider;
				fc.SetSkin(Config.DefaultSkin);

				//Show the temp image just created
				lChart.Text = "<img src=ImageFromCache.aspx?CacheId="+fc.SaveToImageStream(440,440,ImageFormat.Png,0,0)+">";
			}
			catch (FormulaErrorException fee)
			{
				//Show the compile result if the script has some errors
				lChart.Text = fee.ToHtml();
			}
		}
	}
}
