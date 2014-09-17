using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Data;
using System.Web.SessionState;
using System.Threading;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using System.Reflection;
using System.Net;
using System.Globalization;
using System.Text;
using Easychart.Finance;
using Easychart.Finance.DataClient;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos 
{
	/// <summary>
	/// Global object for web application
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Global()
		{
			InitializeComponent();
		}	

//		private static ArrayList alStreamingData = new ArrayList();
//		public static void PutToBuffer(StreamingData ds)
//		{
//			lock(alStreamingData)
//			{
//				alStreamingData.Add(ds);
//			}
//		}
//
//		public static ArrayList GetBuffer(DateTime After)
//		{
//			lock(alStreamingData)
//			{
//				if (alStreamingData.Count>1000)
//					alStreamingData.RemoveRange(0,alStreamingData.Count-1000);
//
//				for(int i=alStreamingData.Count-1; i>=0; i--)
//				{
//					StreamingData sd  = (StreamingData)alStreamingData[i];
//					if (sd.Time<After) 
//					{
//						ArrayList al = new ArrayList();
//						for(int j=i+1; j<alStreamingData.Count; j++)
//							al.Add(alStreamingData[j]);
//						return al;
//					}
//				}
//				return null;
//			}
//		}
//
//		public static void DownloadIntraday()
//		{
//			Tools.Log("Download intraday started!");
//			string[] ss = Config.IntradaySymbols.Split(';');
//			DbParam[] dps = new DbParam[]{
//											 new DbParam("@Symbol",DbType.String,""),
//											 new DbParam("@QuoteTime",DbType.DateTime,null),
//											 new DbParam("@Price",DbType.Double,0),
//											 new DbParam("@Volume",DbType.Double,0),
//			};
//
//			while (true)
//				try
//				{
//					foreach(string s in ss)
//					{
//						Tools.Log("Step1:"+s);
//						ExchangeIntraday ei = ExchangeIntraday.GetExchangeIntraday(Utils.GetExchange(s));
//						DateTime D = DateTime.UtcNow.AddHours(ei.TimeZone);
//						string Symbol = Utils.GetPart1(s);
//						if (ei.IsEstimateOpen(D))
//						{
//							Tools.Log("Step2:"+s);
//							DataPacket dp = DataPacket.DownloadFromYahoo(Symbol);
//
//							Tools.Log("Step3:"+s);
//							BaseDb bd = BaseDb.FromConfig("ChartConnStr");
//							try
//							{
//								DataRow dr = bd.GetFirstRow("select * from Intraday where Symbol='"+Symbol+"' order by Id desc",null,1);
//								DateTime QuoteTime = dp.Date.Date + dp.Date.AddHours(4+ei.TimeZone).TimeOfDay;
//								
//								Tools.Log("Step4:"+s);
//								double OldPrice = double.NaN;
//								if (dr!=null) OldPrice = Math.Round((double)dr["Price"],3);
//								double NewPrice = Math.Round(dp.Close,3);
//								Tools.Log("Step5:"+s);
//
//								if (dr==null || (OldPrice!=NewPrice) || (double)dr["Volume"]!=dp.Volume 
//									|| (((DateTime)dr["QuoteTime"])!=QuoteTime))
//								{
//									dps[0].Value = Symbol;
//									dps[1].Value = QuoteTime;
//									dps[2].Value = NewPrice;
//									dps[3].Value = dp.Volume;
//									PutToBuffer(new StreamingData(Symbol,QuoteTime,dp.Close,dp.Volume));
//									Tools.Log("Step6:"+s);
//									bd.DoCommand("insert into Intraday (Symbol,QuoteTime,Price,Volume) values (?,?,?,?)",dps);
//									Tools.Log("Step7:"+s);
//								}
//							} 
//							catch (Exception e)
//							{
//								Tools.Log("Data Service:"+e.Message);
//							}
//							finally
//							{
//								bd.Close();
//							}
//							Thread.Sleep(500);
//						}
//					}
//					Thread.Sleep(500);
//				}
//				catch (Exception ex)
//				{
//					Thread.Sleep(100);
//					Tools.Log(ex.ToString());
//				}
//		}
//		
//		static public Thread IntradayThread;

		/// <summary>
		/// Start application and back ground threads
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Start(Object sender, EventArgs e)
		{
			StartModule.Active = 1;
//			if (Config.EnableYahooStreaming)
//			{
//				IntradayThread = new Thread(new ThreadStart(DownloadIntraday));
//				IntradayThread.Start();
//			}
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}