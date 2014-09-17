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
using EasyTools;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using WebDemos;
using Easychart.Finance.DataProvider;

namespace WebDemos.DBDemos
{
	public class StockList : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lTotal;
		protected System.Web.UI.WebControls.Label lCondition;
		protected System.Web.UI.WebControls.DataGrid dgList;
		int ChangeColumnIndex;
		int SortColumn;
		string SortImage;
		protected System.Web.UI.WebControls.Label lDefaultSort;
		protected System.Web.UI.WebControls.Label lCount;
		protected System.Web.UI.WebControls.TextBox tbSymbol;
		IDataManager idm;

		private void Bind()
		{
			string ConditionId = Request.QueryString["ConditionId"];
			string Key = Request.QueryString["Key"];
			string Exchange = Request.QueryString["E"];

			string SortExpression = "";
			if (Session["SortExpression"]!=null)
				SortExpression = Session["SortExpression"].ToString();
			if (SortExpression=="" && lDefaultSort!=null)
				SortExpression = lDefaultSort.Text;

			if (ConditionId!=null) 
			{
				DataRow drCond = DB.GetFirstRow("select * from Condition where ConditionId in ("+ConditionId+")");
				if (drCond!=null) 
				{
					if (drCond["Exchange"].ToString()!="")
						lCondition.Text  = "Exchange:"+drCond["Exchange"]+";";
					lCondition.Text += " Filtered by formula:"+drCond["Condition"];
					string Total = drCond["Total"].ToString();
					object Scaned = Cache[ConditionId];
					if (Total!="" && Scaned!=null)
						lCondition.Text +="; "+(((float)(int)Scaned)/int.Parse(Total)).ToString("p2");
					object o1 = drCond["StartTime"];
					object o2 = drCond["EndTime"];
					if (o1!=DBNull.Value) 
					{
						DateTime d2 = DateTime.Now;
						if (o2!=DBNull.Value)
							d2 = (DateTime)o2;
						lCondition.Text +=  "; "+(d2-(DateTime)o1).TotalSeconds.ToString("f2")+"s";
					}
				}
			}

			int ResultCount = idm.SymbolCount(Exchange,ConditionId,Key);
			if (ResultCount>=0)
			{
				lTotal.Text = "Total :"+ResultCount;
			
				dgList.VirtualItemCount = ResultCount;
				DataTable dt = idm.GetStockList(Exchange,ConditionId,Key,SortExpression,
					dgList.CurrentPageIndex*dgList.PageSize, dgList.PageSize);

				SortColumn = -1;
				string se = SortExpression;
				int j = se.IndexOf('.');
				if (j>=0)
					se = se.Substring(j+1);

				for(int i=0; i<dgList.Columns.Count; i++) 
				{
					if (dgList.Columns[i].HeaderText == "Change") 
						ChangeColumnIndex = i;
					if (se!="")
						if (dgList.Columns[i] is BoundColumn)
						{

							if (se.StartsWith((dgList.Columns[i] as BoundColumn).DataField)) 
							{
								SortColumn = i;
								if (se.IndexOf(' ')>0)
									SortImage = "dn";
								else SortImage = "up";
	
							} 
							else 
								dgList.Columns[i].HeaderImageUrl = null;
						}
				}

				dgList.DataSource = dt;
				dgList.DataBind();
			}
		}
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			ChangeColumnIndex = -1;
			if (!IsPostBack)
				Bind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			idm = Utils.GetDataManager(Config.DefaultDataManager);
			InitializeComponent();

			if (dgList.Columns.Count==0)
				if (idm.StockListColumns!=null)
					foreach(DataGridColumn dgc in idm.StockListColumns)
						dgList.Columns.Add(dgc);
			base.OnInit(e);
		}
		
		/// <summary>
		/// </summary>
		private void InitializeComponent()
		{    
			this.dgList.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgList_PageIndexChanged);
			this.dgList.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgList_SortCommand);
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
			if (e.Item.ItemType==ListItemType.Item || e.Item.ItemType==ListItemType.AlternatingItem)
			{
				if (ChangeColumnIndex>=0)
				{
					TableCell tc = e.Item.Cells[ChangeColumnIndex];
					string s =tc.Text;
					if (s.StartsWith("-")) 
					{
						tc.ForeColor = Color.Red;
					}
					else 
					{
						tc.ForeColor = Color.DarkGreen;
						if (string.Compare(s,"&nbsp;")!=0)
							tc.Text = "+"+s;
					}
				}
			}
			else if (e.Item.ItemType == ListItemType.Header) 
			{
				Literal l = new Literal();
				l.Text = "<img src=../images/"+SortImage+".gif>";
				if (SortColumn>=0) 
				{
					e.Item.Cells[SortColumn].Controls.Add(l); 
					e.Item.Cells[SortColumn].BackColor = Color.WhiteSmoke;
				}
			}
		}

		private void dgList_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			dgList.CurrentPageIndex =0;

			string s1 = (string)Session["SortExpression"];
			string s2 = e.SortExpression;

			if (s1!=null && s1.EndsWith("desc")) 
				s1 = s1.Substring(0,s1.Length-5);
			if (s2!=null && s2.EndsWith("desc")) 
				s2 = s2.Substring(0,s2.Length-5);

			// if click on the same field , swtich asc/desc
			if (s1 == s2)
			{
				s1 = (string)Session["SortExpression"];
				if (s1.EndsWith("desc")) 
					s1 = s1.Substring(0,s1.Length-5);
				else s1 +=" desc";
				Session["SortExpression"]  = s1;
			} 
			else 
				//else using the default sort expression
				Session["SortExpression"]  = e.SortExpression;
			Bind();
		}
	}
}