using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Data;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Net;
using EasyTools;
using System.Globalization;
using Easychart.Finance.DataClient;
using Easychart.Finance.DataProvider;
using System.Threading;
using Easychart.Finance;

namespace WebDemos
{
	/// <summary>
	/// Summary description for StartModule.
	/// </summary>
	public class StartModule :IHttpModule
	{
		static public int Active;
		static public Thread tUpdateStreaming = null;
		static public Thread tAutoService;
//		static DateTime LastAssamblyTime;

		static StartModule()
		{
			try
			{
				if (Config.PluginsDirectory==null) 
				{
					PluginManager.RegAssemblyFromMemory();
				}
				else PluginManager.Load(Config.PluginsDirectory);
			} 
			catch (Exception e)
			{
				Tools.Log("Load Plugins Error:"+e.Message);
			}

			FormulaHelper.WebTimeout = Config.WebTimeout*1000;
			if (Config.WebProxy!=null && Config.WebProxy!="") 
			{
				FormulaHelper.DownloadProxy = new WebProxy(Config.WebProxy);
				Tools.Log("Download Proxy:"+Config.WebProxy);
			}
		}

#if(!LITE)
		static public void SaveHashtable(Hashtable ht,string Filename)
		{
			string s = HttpRuntime.AppDomainAppPath+"Log\\";
			if (!Directory.Exists(s))
				Directory.CreateDirectory(s);
			string FileName = s +Filename; //"Update.log"
			StreamWriter sw = new StreamWriter(FileName);
			foreach(string r in ht.Keys) 
				sw.WriteLine(r+"="+((DateTime)ht[r]).ToString());
			sw.Close();
		}

		static private void SetUSCulture()
		{
//			NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
//			nfi.NumberDecimalSeparator = ".";
//			nfi.NumberGroupSeparator = ",";
//			CultureInfo ci1 = new CultureInfo("en-US",true);
//			ci1.NumberFormat = nfi;
//			CultureInfo ci2 = new CultureInfo("en",true);
//			ci2.NumberFormat = nfi;
//			Thread.CurrentThread.CurrentCulture = ci1;
//			Thread.CurrentThread.CurrentUICulture = ci2;
		}

		static public void LoadHashtable(Hashtable ht,string Filename) 
		{
			try 
			{
				ht.Clear();
				string s = HttpRuntime.AppDomainAppPath+"Log\\";
				string FileName = s +Filename; //"Update.log"
				if (File.Exists(FileName)) 
				{
					StreamReader sr = new StreamReader(FileName);
					try 
					{
						string r;
						while ((r = sr.ReadLine())!=null) 
						{
							int i = r.IndexOf('=');
							if (i>0) 
								ht[r.Substring(0,i)]=DateTime.Parse(r.Substring(i+1));
						}
					} 
					finally 
					{
						sr.Close();
					}
				}
			} 
			catch
			{
			}
		}

		static private void GetTime(string r,out DateTime Time,out DateTime CurrentTime) 
		{
			int TimeDiff = 0;
			int i = r.IndexOf("(");
			int j = r.IndexOf(")",i+1);
			if (i>0 && j>0) 
			{
				TimeDiff = Tools.ToIntDef(r.Substring(i+1,j-i-1),TimeDiff);
				r = r.Remove(i,j-i+1);
			}
			Time = DateTime.Parse(r);
			CurrentTime = DateTime.UtcNow.AddHours(TimeDiff);
		}

		/// <summary>
		/// Update end of day data automatically
		/// </summary>
//		public static void AutoUpdate() 
//		{
//			Thread.Sleep(10000);
//			Impersonate.ChangeToAdmin();
//			Tools.Log("Auto Update: Started");
//			string[] ss = Config.AutoUpdate.Split(',');
//			Hashtable ht = new Hashtable();
//			while (true) 
//				try 
//				{
//					LoadHashtable(ht,"Update.txt");
//					foreach(string s in ss) 
//					{
//						int i = s.IndexOf("=");
//						string Exchange = s.Substring(0,i);
//						string r = s.Substring(i+1);
//
//						DateTime LastTime = DateTime.MinValue;
//						if (ht[Exchange]!=null)
//							LastTime = (DateTime)ht[Exchange];
//
//						DateTime ServiceTime;
//						DateTime CurrentTime;
//						GetTime(r,out ServiceTime,out CurrentTime);
//						
//						if (CurrentTime.DayOfWeek!=DayOfWeek.Saturday && CurrentTime.DayOfWeek!=DayOfWeek.Sunday)
//						{
//							if (CurrentTime.TimeOfDay>ServiceTime.TimeOfDay && LastTime.Date<CurrentTime.Date) 
//								try 
//								{
//									Tools.Log(r+";ServiceTime="+ServiceTime+";CurrentTime="+CurrentTime);
//									if (Config.AutoUpdateSource(CurrentTime,Exchange))
//									{
//										ht[Exchange] = CurrentTime;
//										SaveHashtable(ht,"Update.txt");
//									}
//								}
//								catch (Exception ex)
//								{
//									Tools.Log("AutoUpdate:UpdateQuote:"+ex.Message);
//								}
//						}
//					}
//					Thread.Sleep(60000);
//				}
//				catch (Exception ex)
//				{
//					Tools.Log("AutoUpdate:Loop:"+ex.Message);
//					Thread.Sleep(60000);
//				}
//		}

		static public bool Execute(object state)
		{
			string ClassName = (string)state;
			int j1 = ClassName.IndexOf('(');
			int j2 = ClassName.LastIndexOf(')');
			object[] Params = new object[]{"Dummy"};
			if (j2>j1)
			{
				string s = ClassName.Substring(j1+1,j2-j1-1);
				if (s!="") 
				{
					string[] ss = s.Split(',');
					Params = new object[ss.Length];
					for(int k=0; k<ss.Length; k++)
						Params[k] = ss[k];
				}
				ClassName = ClassName.Substring(0,j1);
			}

			int i=ClassName.LastIndexOf('.');
			if (i>0) 
			{
				string TypeName = ClassName.Substring(0,i);
				ClassName = ClassName.Substring(i+1);
				Type t = Type.GetType(TypeName);
				object result = t.InvokeMember(ClassName,BindingFlags.Static | BindingFlags.Public |  BindingFlags.InvokeMethod,
					null,null,Params);
				Tools.Log("Running "+ClassName);
				if (result is bool)
					return (bool)result;
			}
			return true;
		}

		static private string[] Split(char c,string s)
		{
			ArrayList al = new ArrayList();
			int j=0;
			int k=0;
			for(int i=0; i<s.Length; i++)
			{
				if (s[i]==c && k==0) 
				{
					al.Add(s.Substring(j,i-j));
					j = i+1;
				} 
				else if (s[i]==')')
					k--;
				else if (s[i]=='(')
					k++;
			}
			if (j<s.Length)
				al.Add(s.Substring(j));
			return (string[])al.ToArray(typeof(string));
		}

		/// <summary>
		/// Running back ground services at curtain time.
		/// The service and curtain time are defined in web.config
		/// </summary>
		static public void AutoService() 
		{
			Thread.Sleep(1000);
			SetUSCulture();

			//Tools.Log("Auto Service: Started"+",ThreadId="+Thread.CurrentThread.GetHashCode()); //+",ThreadId="+AppDomain.GetCurrentThreadId()
			Hashtable ht = new Hashtable();
			while (true)
				try 
				{
					LoadHashtable(ht,"Service.txt");
					for(int i=1; i<10; i++) 
					{
						string ServiceName = "Service"+i;
						string s = ConfigurationSettings.AppSettings[ServiceName];
						//Tools.Log(ServiceName+"="+s);

						if (s!=null)
						{
							string[] ss = Split(',',s);// s.Split(',');
							if (ss.Length>=2)
							{
								int[] WeekDay = {1,2,3,4,5};
								if (ss.Length==3) 
								{
									string[] sss = ss[2].Split(';');
									WeekDay = new int[sss.Length];
									for(int j=0; j<sss.Length; j++)
										WeekDay[j] = int.Parse(sss[j]);
								}
								DateTime LastTime = DateTime.MinValue;
								if (ht[ServiceName]!=null)
									LastTime = (DateTime)ht[ServiceName];

								DateTime ServiceTime;
								DateTime CurrentTime;
								GetTime(ss[1],out ServiceTime,out CurrentTime);

								if (Array.IndexOf(WeekDay,(int)CurrentTime.DayOfWeek)>=0)
								{
									if (CurrentTime.TimeOfDay>ServiceTime.TimeOfDay && LastTime.Date<CurrentTime.Date)
										try 
										{
											if (Execute(ss[0]))
											{
												ht[ServiceName] = CurrentTime;
												SaveHashtable(ht,"Service.txt");
											}
										}
										catch (Exception ex)
										{
											Tools.Log(ServiceName+":"+ex.Message+",ThreadId="+Thread.CurrentThread.GetHashCode());
										}
								}
							}
						}
						Thread.Sleep(1000);
					}
				}
				catch (Exception ex)
				{
					//Tools.Log("AutoService:Loop:"+ex.Message+",ThreadId="+Thread.CurrentThread.GetHashCode()); //+",ThreadId="+AppDomain.GetCurrentThreadId()
					if (ex is ThreadAbortException)
						tAutoService = null;
					Thread.Sleep(10000);
				}
		}

		static DbParam[] dps = new DbParam[]{
												new DbParam("@Symbol",DbType.String,""),
												new DbParam("@QuoteTime",DbType.DateTime,null),
												new DbParam("@Price",DbType.Double,0),
												new DbParam("@Volume",DbType.Double,0),
		};
		static bool StreamingStopped = true;
		/// <summary>
		/// Update streaming data to database
		/// </summary>
		static public void UpdateStreaming()
		{
			SetUSCulture();
			Tools.Log("Streaming service started!"+",ThreadId="+Thread.CurrentThread.GetHashCode());

			try
			{
				while (true)
				{
					try
					{
						Thread.Sleep(1000);

						DB.DoCommand("delete from Intraday where QuoteTime<?",
							new DbParam[]{
											 new DbParam("@QuoteTime",DbType.DateTime,DateTime.Today.AddDays(-7))
										 });

					
						DataClientBase dcb;
						if (Config.StreamingDataClient=="YahooDataClient")
							dcb =new YahooDataClient();
						else dcb = new EasyChartDataClient();
						Tools.Log(dcb.GetType().ToString());

						dcb.Proxy = Config.WebProxy;
						dcb.OnStreamingData +=new StreamingDataChanged(dcb_OnStreamingData); 
						dcb.OnStreamingStopped+=new EventHandler(dcb_OnStreamingStopped);
						while (true)
						{
							if (StreamingStopped)
							{
								StreamingStopped = false;
								Tools.Log("Streaming service start again!"+",ThreadId="+Thread.CurrentThread.GetHashCode());
								dcb.StartStreaming(Config.IntradaySymbols);
							}
							Thread.Sleep(100);
						}
					} 
					catch (Exception e)
					{
						if (e is ThreadAbortException)
							tUpdateStreaming = null;

						Tools.Log("Streaming service:"+e+",ThreadId="+Thread.CurrentThread.GetHashCode());
					}
					Thread.Sleep(1000);
				}
			}
			finally
			{
				tUpdateStreaming = null;
			}
		}

		static private void dcb_OnStreamingStopped(object sender, EventArgs e)
		{
			DataClientBase dcb = sender as DataClientBase;
			if (dcb.LastError!=null) 
			{
				Tools.Log("Streaming service error:"+dcb.LastError);
				if (dcb.LastError is ThreadAbortException)
					tUpdateStreaming = null;
				dcb.ClearLastError();
			}
			StreamingStopped = true;
		}

#endif

		#region IHttpModule Members

		public void Init(HttpApplication context)
		{
			context.BeginRequest+=new EventHandler(context_BeginRequest);
		}

		private void context_BeginRequest(object sender, EventArgs e)
		{
#if(!LITE)
			if (Config.EnableAutoService && tAutoService==null) 
			{
				tAutoService = new Thread(new ThreadStart(AutoService));
				tAutoService.Start();
			}

			if (Config.EnableStreaming && tUpdateStreaming==null)
			{
				tUpdateStreaming = new Thread(new ThreadStart(UpdateStreaming));
				tUpdateStreaming.Start();
			}

			if (FormulaBase.SupportedAssemblies.Count==0)
				PluginManager.RegExecutingAssembly();
//			{
//				Tools.Log("Before Register Assembly");
//				PluginManager.RegAssemblyFromMemory();
//				Tools.Log("Register Assembly "+FormulaBase.SupportedAssemblies.Count);
//			}

//			if (LastAssamblyTime.AddMinutes(10)<DateTime.Now)
//			{
//				try
//				{
//					foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies()) 
//						try
//						{
//							Tools.Log(a.FullName);
//						}
//						catch
//						{
//						}
//					Tools.Log("Domain:"+Thread.GetDomainID()+":"+  Thread.GetDomain());
//				}
//				catch (Exception ex)
//				{
//				}
//				LastAssamblyTime = DateTime.Now;
//			}


#endif
		}

		public void Dispose()
		{
			// TODO:  Add StartModule.Dispose implementation
		}

		#endregion

#if(!LITE)
		private static void dcb_OnStreamingData(object sender, DataPacket dp)
		{
//			Active++;
//			if ((Active % 10)==0)
//				GC.Collect(); //avoid hosting recollect the web application
			dps[0].Value = dp.Symbol;
			dps[1].Value = dp.Date;
			dps[2].Value = dp.Close;
			dps[3].Value = dp.Volume;
			DB.DoCommand("insert into Intraday (Symbol,QuoteTime,Price,Volume) values (?,?,?,?)",dps);
		}
#endif

	}
}