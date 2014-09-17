using System;
using System.Web;
using System.Web.UI.WebControls;

namespace WebDemos
{
	/// <summary>
	/// Summary description for MSDataManager.
	/// </summary>
	public class MSDataManager : Easychart.Finance.DataProvider.MSDataManager
	{
		public MSDataManager():base(HttpRuntime.AppDomainAppPath+"MetaDaily")
		{
		}

		public MSDataManager(string FilePath):base(FilePath)
		{
		}

		public override System.Web.UI.WebControls.DataGridColumn[] StockListColumns
		{
			get
			{
				return new DataGridColumn[]{
											   CreateHyperLinkColumn("Chart","Symbol","CustomChart.aspx?"+Config.SymbolParameterName+"={0}"),
											   CreateBoundColumn("Code","Symbol","Symbol",null),
											   CreateBoundColumn("Name","Name","Name",null),
				};
			}
		}
	}
}

