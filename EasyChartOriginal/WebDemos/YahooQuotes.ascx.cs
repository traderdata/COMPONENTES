namespace WebDemos
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Text;
	using System.Globalization;
	using Easychart.Finance.DataProvider;
	using Easychart.Finance;
	using EasyTools;

	/// <summary>
	///		Summary description for Quotes.
	/// </summary>
	public class YahooQuotes : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lHigh;
		protected System.Web.UI.WebControls.Label lEPS;
		protected System.Web.UI.WebControls.Label lTradeTime;
		protected System.Web.UI.WebControls.Label lLow;
		protected System.Web.UI.WebControls.Label lEPSnextyear;
		protected System.Web.UI.WebControls.Label l52weekhigh;
		protected System.Web.UI.WebControls.Label lBid;
		protected System.Web.UI.WebControls.Label l52weeklow;
		protected System.Web.UI.WebControls.Label lPriceSale;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.Label lAsk;
		protected System.Web.UI.WebControls.Label l50dayMA;
		protected System.Web.UI.WebControls.Label lPEGGrowth;
		protected System.Web.UI.WebControls.Label lVolume;
		protected System.Web.UI.WebControls.Label l200dayMA;
		protected System.Web.UI.WebControls.Label lDividendShare;
		protected System.Web.UI.WebControls.Label lAvgVolume;
		protected System.Web.UI.WebControls.Label lMarketCap;
		protected System.Web.UI.WebControls.Label lDivYield;
		protected System.Web.UI.WebControls.Label lOpen;
		protected System.Web.UI.WebControls.Label lBookValue;
		protected System.Web.UI.WebControls.Label lShortRatio;
		protected System.Web.UI.WebControls.Label lPrvClose;
		protected System.Web.UI.WebControls.Label lPriceBook;
		protected System.Web.UI.WebControls.Label lStockExch;
		protected System.Web.UI.WebControls.Label lLastTrade;
		protected System.Web.UI.WebControls.Label lLastTradePriceOnly;
		protected System.Web.UI.WebControls.Label lDaysHigh;
		protected System.Web.UI.WebControls.Label lEPSEstimateCurrentYear;
		protected System.Web.UI.WebControls.Label lLastTradeTime;
		protected System.Web.UI.WebControls.Label lDaysLow;
		protected System.Web.UI.WebControls.Label lPrice_Sales;
		protected System.Web.UI.WebControls.Label l50dayMovingAverage;
		protected System.Web.UI.WebControls.Label lPEG_Ratio;
		protected System.Web.UI.WebControls.Label l200dayMovingAverage;
		protected System.Web.UI.WebControls.Label lDividend_Share;
		protected System.Web.UI.WebControls.Label lAverageDailyVolume;
		protected System.Web.UI.WebControls.Label lMarketCapitalization;
		protected System.Web.UI.WebControls.Label lDividendYield;
		protected System.Web.UI.WebControls.Label lPreviousClose;
		protected System.Web.UI.WebControls.Label lPrice_Book;
		protected System.Web.UI.WebControls.Label lStockExchange;
		protected System.Web.UI.WebControls.Label lChangePercentChange;
		protected System.Web.UI.WebControls.Panel pnQuotes;
		protected System.Web.UI.WebControls.Label lEPSEstimateNextYear;
		protected System.Web.UI.WebControls.Label lPrice_EPSEstimateNextYear;
		protected System.Web.UI.WebControls.Label lSymbol;
	
		private string symbolControl;
		protected System.Web.UI.WebControls.Label lName;
		static public Hashtable htYahooMap;
	
		static YahooQuotes()
		{
			//http://gummy-stuff.org/Yahoo-data.htm
			htYahooMap = new Hashtable(new CaseInsensitiveHashCodeProvider(),new CaseInsensitiveComparer());
			htYahooMap["Ask"] = "a";
			htYahooMap["AverageDailyVolume"] = "a2";
			htYahooMap["AskSize"] = "a5";

			htYahooMap["Bid"] = "b";
			htYahooMap["AskRealtime"] = "b2"; //real-time
			htYahooMap["BidRealtime"] = "b3"; //real-time

			htYahooMap["BookValue"] = "b4";
			htYahooMap["BidSize"] = "b6";
			htYahooMap["ChangePercentChange"] = "c";

			htYahooMap["Change"] = "c1";
			htYahooMap["Commission "] = "c3";
			htYahooMap["ChangeRealtime"] = "c6"; //real-time

			htYahooMap["AfterHoursChangeRealtime"] = "c8"; //real-time
			htYahooMap["Dividend_Share"] = "d"; 
			htYahooMap["LastTradeDate"] = "d1"; 

			htYahooMap["TradeDate"] = "d2";
			htYahooMap["Earnings_Share "] = "e"; 
			htYahooMap["ErrorIndication"] = "e1"; 

			htYahooMap["EPSEstimateCurrentYear"] = "e7";
			htYahooMap["EPSEstimateNextYear"] = "e8";
			htYahooMap["EPSEstimateNextQuarter"] = "e9";

			htYahooMap["FloatShares"] = "f6";
			htYahooMap["DaysLow"] = "g";
			htYahooMap["DaysHigh"] = "h";

			htYahooMap["52weekLow"] = "j";
			htYahooMap["52weekHigh"] = "k";
			htYahooMap["HoldingsGainPercent"] = "g1";

			htYahooMap["AnnualizedGain"] = "g3";
			htYahooMap["HoldingsGain"] = "g4";
			htYahooMap["HoldingsGainPercentRealtime) "] = "g5";

			htYahooMap["HoldingsGainRealtime"] = "g6"; //real-time
			htYahooMap["MoreInfo"] = "i"; 
			htYahooMap["OrderBookRealtime"] = "i5"; 

			htYahooMap["MarketCapitalization"] = "j1";
			htYahooMap["MarketCapRealtime"] = "j3";
			htYahooMap["EBITDA"] = "j4";

			htYahooMap["ChangeFrom52weekLow"] = "j5";
			htYahooMap["PercentChangeFrom52weekLow"] = "j6";
			htYahooMap["LastTradeRealtimeWithTime"] = "k1";

			htYahooMap["ChangePercent"] = "k2";
			htYahooMap["LastTradeSize"] = "k3";
			htYahooMap["ChangeFrom52weekHigh"] = "k4";

			htYahooMap["PercentChangeFrom52weekHigh"] = "k5";
			htYahooMap["LastTradeWithTime"] = "l";
			htYahooMap["LastTradePriceOnly"] = "l1";

			htYahooMap["HighLimit"] = "l2";
			htYahooMap["LowLimit"] = "l3";
			htYahooMap["DaysRange"] = "m";

			htYahooMap["DaysRangeRealtime"] = "m2"; //real-time
			htYahooMap["50dayMovingAverage"] = "m3";
			htYahooMap["200dayMovingAverage"] = "m4";

			htYahooMap["ChangeFrom200dayMovingAverage"] = "m5";
			htYahooMap["PercentChangeFrom200dayMovingAverage"] = "m6";
			htYahooMap["ChangeFrom50dayMovingAverage"] = "m7";

			htYahooMap["PercentChangeFrom50dayMovingAverage"] = "m8";
			htYahooMap["Name"] = "n";
			htYahooMap["Notes"] = "n4";

			htYahooMap["Open"] = "o";
			htYahooMap["PreviousClose"] = "p";
			htYahooMap["PricePaid"] = "p1";

			htYahooMap["ChangeinPercent"] = "p2";
			htYahooMap["Price_Sales"] = "p5";
			htYahooMap["Price_Book"] = "p6";

			htYahooMap["ExDividendDate"] = "q";
			htYahooMap["P_ERatio"] = "r";
			htYahooMap["DividendPayDate"] = "r1";

			htYahooMap["P_ERatioRealtime"] = "r2"; //real-time
			htYahooMap["PEG_Ratio"] = "r5"; 
			htYahooMap["Price_EPSEstimateCurrentYear"] = "r6"; 

			htYahooMap["Price_EPSEstimateNextYear"] = "r7";
			htYahooMap["Symbol"] = "s";
			htYahooMap["SharesOwned"] = "s1";

			htYahooMap["ShortRatio"] = "s7";
			htYahooMap["LastTradeTime"] = "t1";
			htYahooMap["TradeLinks"] = "t6";

			htYahooMap["TickerTrend"] = "t7";
			htYahooMap["1yrTargetPrice"] = "t8";
			htYahooMap["Volume"] = "v";

			htYahooMap["HoldingsValue"] = "v1";
			htYahooMap["HoldingsValueRealtime"] = "v7";
			htYahooMap["52weekRange"] = "w";

			htYahooMap["DaysValueChange"] = "w1";
			htYahooMap["DaysValueChangeRealtime"] = "w4";
			htYahooMap["StockExchange"] = "x";

			htYahooMap["DividendYield"] = "y";
		}

		public string SymbolControl
		{
			get
			{ 
				return symbolControl;
			}
			set
			{
				symbolControl = value;
			}
		}

		private Unit width;
		public Unit Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		static private void BindData(string s,Panel pnQuotes,
			Label lChangePercentChange, Label lVolume, Label lAverageDailyVolume, Label lDividendYield
			)
		{
			if (s!=null && s!="")
			{
				ArrayList alControl = new ArrayList();
				foreach(Control c in pnQuotes.Controls)
					if (c is Label)
					{
						if (htYahooMap.ContainsKey((c as Label).ID.Substring(1))) 
							alControl.Add(c);
					}

				string[] ss = s.Trim().Split(',');
				for(int i=0; i<ss.Length && i<alControl.Count; i++)
					(alControl[i] as Label).Text = ss[i].Trim('"');

				if (lChangePercentChange!=null)
				{
					string t = lChangePercentChange.Text;
					t = t.Replace(" - ","(")+")";
					lChangePercentChange.Text = t;
					if (t.StartsWith("+")) 
						lChangePercentChange.ForeColor = Color.Green;
					else if (t.StartsWith("-"))
						lChangePercentChange.ForeColor = Color.Red;
				}

				if (lVolume!=null)
					lVolume.Text = Tools.ToDoubleDef(lVolume.Text,0).ToString("N0");
				if (lAverageDailyVolume!=null)
					lAverageDailyVolume .Text = Tools.ToDoubleDef(lAverageDailyVolume.Text,0).ToString("N0",NumberFormatInfo.InvariantInfo);
				if (lDividendYield!=null)
					lDividendYield.Text = (Tools.ToDoubleDef(lDividendYield.Text,-1)!=-1)?(lDividendYield.Text+"%"):lDividendYield.Text;
			}
		}

		static public void Bind(string Symbol,Panel pnQuotes, Label lSymbol,Unit width,
			Label lChangePercentChange, Label lVolume, Label lAverageDailyVolume, Label lDividendYield
			)
		{
			if (lSymbol==null || lSymbol.Text!=Symbol)
			{
				StringBuilder sb = new StringBuilder();
				foreach(Control c in pnQuotes.Controls)
					if (c is Label)
					{
						string s = (c as Label).ID.Substring(1);
						if (htYahooMap.ContainsKey(s)) 
							sb.Append((string)htYahooMap[s]);
					}

				string r = YahooDataManager.TryDownloadRealtimeFromYahoo(Symbol ,sb.ToString());

				if (r!=null)
				{
					string[] rr = r.Trim().Split('\r');
					if (rr.Length>0)
						BindData(rr[0],pnQuotes,lChangePercentChange, lVolume, lAverageDailyVolume, lDividendYield);
				}
			}
			pnQuotes.Width = width;
		}

		private void Quotes_PreRender(object sender, EventArgs e)
		{
			if (symbolControl!=null && symbolControl!="") 
			{
				Control tbSymbol = Parent.FindControl(symbolControl);
				if (tbSymbol is TextBox) 
				{
					string Symbol = (tbSymbol as TextBox).Text;
					Bind(Symbol,pnQuotes,lSymbol, width,lChangePercentChange,lVolume,lAverageDailyVolume,lDividendYield);
					if (Config.YahooQuoteEmptySymbolBox)
						(tbSymbol as TextBox).Text = "";
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.PreRender+=new EventHandler(Quotes_PreRender);
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

		}
		#endregion

	}
}