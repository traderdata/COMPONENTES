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
using System.Globalization;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos.DBDemos
{
	/// <summary>
	/// Summary description for FormulaValue.
	/// </summary>
	public class FormulaValue : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lTotal;
		protected System.Web.UI.WebControls.Label lDate;
		protected System.Web.UI.WebControls.Label lChartPage;
		protected System.Web.UI.WebControls.Label lReplace;
		protected System.Web.UI.WebControls.DataGrid dgList;
		protected System.Web.UI.WebControls.Label lGroup;
		protected System.Web.UI.WebControls.Literal lGroupLink;
		private Hashtable htReplace;
		private Hashtable htFormat;
		protected System.Web.UI.WebControls.Label lFormat;
		protected System.Web.UI.WebControls.Label lColumn;
		private Hashtable htGroup;

		private void GetGroup()
		{
			htGroup = new Hashtable();
			if (lGroup!=null)
			{
				string[] ss = lGroup.Text.Split('|');
				if (lGroupLink!=null)
					lGroupLink.Text = "";
				foreach(string s in ss)
				{
					string[] rr = s.Split(';');
					if (rr.Length>2)
					{
						htGroup[rr[0]] = rr;
						if (lGroupLink!=null) 
						{
							if (lGroupLink.Text!="")
								lGroupLink.Text +=" | ";
							lGroupLink.Text += "<a href=?GroupId="+rr[0]+">"+rr[1]+"</a>";
						}
					}
				}
			}
		}

		private void Bind() 
		{
			DataTable dtFormulaValue = null;
			DataTable dtMain = null;
			DataRow drDate = DB.GetFirstRow("select Max(CalculateTime) from FormulaValue");
			if (drDate!=null)
			{
				object o = drDate[0];
				if (o is DateTime) 
				{
					DateTime d =(DateTime)o; 
					d = d.AddHours(Config.AdjustHours);
					lDate.Text = d.ToString("MMM dd yyyy",DateTimeFormatInfo.InvariantInfo);
					for(int i=0; i<dgList.Columns.Count; i++)
					{
						if (dgList.Columns[i] is BoundColumn)
						{
							BoundColumn bc = dgList.Columns[i] as BoundColumn;
							if (bc.HeaderText=="Close")
								bc.HeaderText = d.ToString("yyyy-MM-dd");
							else if (bc.HeaderText=="Last")
								bc.HeaderText = d.AddDays(-1).ToString("yyyy-MM-dd");
						}
					}
				}
			}
			IDataManager idm = Utils.GetDataManager(Config.DefaultDataManager);

			string Exchange = "";
			string ConditionId = null;

			string GroupId = Request.QueryString["GroupId"];
			string Filter = null;
			if (GroupId!=null)
			{
				string[] ssGroup = (string[])htGroup[GroupId];
				if (ssGroup!=null) 
				{
					if (ssGroup.Length>2)
						Exchange = ssGroup[2];
					if (ssGroup.Length>3)
						ConditionId = "_"+ssGroup[3];
					if (ssGroup.Length>4)
						Filter = ssGroup[4];
				}
			}

			if (Filter!=null) 
			{
				ConditionId = "_"+ DB.GetCommaValues("select QuoteCode from FormulaValue where "+Filter,null,",");
				if (ConditionId=="_")
					ConditionId="0";
				Tools.Log("Filter:"+ConditionId);
			}

			dgList.VirtualItemCount = idm.SymbolCount(Exchange,ConditionId,null);
			lTotal.Text = "Total :"+dgList.VirtualItemCount;

			dtMain = idm.GetStockList(Exchange,"NoRealtime"+ConditionId,null,"",dgList.CurrentPageIndex*dgList.PageSize,dgList.PageSize);
			DataColumn dcName = dtMain.Columns["QuoteName"];

			int RemoveStart = 1;
			string[] ss=Config.AutoPullFormulaData.Split(';');
			foreach(string s in ss)
				if (string.Compare(Utils.GetValue(s),"Name",true)==0) 
				{
					dtMain.Columns[1].ColumnName = Utils.GetName(s);
					RemoveStart = 2;
				}


			while (dtMain.Columns.Count>RemoveStart)
				dtMain.Columns.RemoveAt(RemoveStart);

			//idm.GetSymbolList("",null,null,"",dgList.CurrentPageIndex*dgList.PageSize,dgList.PageSize);
			dtMain.PrimaryKey = new DataColumn[]{dtMain.Columns[0]};
			if (dtMain.Rows.Count>0)
			{
				string s1 = dtMain.Rows[0][0].ToString();
				string s2 = dtMain.Rows[dtMain.Rows.Count-1][0].ToString();
				dtFormulaValue = DB.GetDataTable("select * from FormulaValue where QuoteCode>='"+s1+"' and  QuoteCode<='"+s2+"' order by QuoteCode");
			}
			
			foreach(string s in ss)
				try
				{
					dtMain.Columns.Add(Utils.GetName(s));
				}
				catch
				{
				}
			
			if (dtFormulaValue!=null)
			{
				foreach(DataRow dr in dtFormulaValue.Rows)
				{
					DataRow drMain = dtMain.Rows.Find(dr["QuoteCode"]);
					if (drMain!=null)
					{
						object o = dr["FormulaValue"];
						double d = double.NaN;
						string s = "";
						if (o!=DBNull.Value)
							d = float.Parse(o.ToString());
						string Format = null;
						string FormulaName = dr["FormulaName"].ToString();
						if (htFormat[FormulaName]!=null)
							Format = htFormat[FormulaName].ToString();
						
						if (!double.IsNaN(d)) 
						{
							if (Format==null)
								Format="f2";
							else if (Format=="Z")
								Format = "f"+FormulaHelper.TestBestFormat(d,0);
							
							s = d.ToString(Format);
						}
						if (s!="")
							drMain[dr["FormulaName"].ToString()] = s;
					}
				}
			}

			dgList.DataSource = dtMain;
			dgList.DataBind();
		}
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			htReplace = new Hashtable();
			if (lReplace!=null)
			{
				string[] ss = lReplace.Text.Trim().Split(';');
				foreach(string s in ss)
				{
					int i = s.IndexOf("=");
					string r1 = s.Substring(0,i);
					string r2 = s.Substring(i+1);

					string[] rr = r2.Split('|');
					Hashtable ht = new Hashtable();
					foreach(string r in rr)
					{
						int j = r.IndexOf(':');
						string t1 = r.Substring(0,j);
						string t2 = r.Substring(j+1);
						ht[t1] = t2;
					}
					htReplace[r1] = ht;
				}
			}
			htFormat = new Hashtable();
			if (lFormat!=null)
			{
				string[] ss = lFormat.Text.Split(';');
				foreach(string s in ss)
				{
					string[] rr = s.Split('=');
					if (rr.Length==2)
						htFormat[rr[0]] = rr[1];
				}
			}

			GetGroup();

			if (!IsPostBack)
				Bind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			dgList.Columns.Clear();
			HyperLinkColumn hlc = new HyperLinkColumn();
			hlc.DataNavigateUrlField = "QuoteCode";
			hlc.DataNavigateUrlFormatString = lChartPage.Text+"?"+Config.SymbolParameterName+"={0}";
			hlc.DataTextField = "QuoteCode";
			hlc.HeaderText = "Symbol";
			dgList.Columns.Add(hlc);

			string[] ss = Config.AutoPullFormulaData.Split(';');
			bool b = lColumn!=null && lColumn.Text!="";
			if (b)
				ss = lColumn.Text.Split(';');

			foreach(string s in ss)
			{
				BoundColumn bc = new BoundColumn();
				if (b) 
				{
					int i = s.IndexOf('=');
					if (i>0)
					{
						bc.DataField = s.Substring(0,i);
						bc.HeaderText = s.Substring(i+1);
					} 
					else continue;
				} 
				else 
				{
					bc.DataField = Utils.GetName(s);
					bc.HeaderText = bc.DataField;
				}
				bc.SortExpression = bc.DataField;
				dgList.Columns.Add(bc);
			}

			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.dgList.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgList_PageIndexChanged);
			this.dgList.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgList_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void dgList_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgList.CurrentPageIndex =e.NewPageIndex;
			Bind();
		}

		private void dgList_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			for(int i=0; i<dgList.Columns.Count; i++)
			{
				object o1 = htReplace[dgList.Columns[i].HeaderText];
				if (o1 is Hashtable) 
				{
					object o2 = (o1 as Hashtable)[e.Item.Cells[i].Text];
					if (o2!=null)
						e.Item.Cells[i].Text = o2.ToString();
				}
			}
		}
	}
}