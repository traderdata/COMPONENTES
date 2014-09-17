using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Globalization;
using System.Configuration;
using System.Reflection;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using EasyTools;

namespace WebDemos
{
	/// <summary>
	/// Draw stock chart
	/// </summary>
	public class Chart : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			int Height = 800;
			int Width = 600;
			Bitmap B;
			try 
			{
				long Start = DateTime.Now.Ticks;

				ExchangeIntraday ei = null;
				DateTime StartTime = DateTime.Today.AddMonths(-6);
				DateTime EndTime = DateTime.Today;

				string Param = Request.QueryString["Span"];
				if (Param!=null && Param!="") 
				{
					DataCycle dc;
					if (Param.Length>2)
						dc = DataCycle.Parse(Param);
					else dc = new DataCycle(DataCycleBase.MONTH,Tools.ToIntDef(Param,6));
					StartTime = EndTime-dc;
					//StartTime = EndTime.AddMonths(-Tools.ToIntDef(Param,6));
				}

				Param = Request.QueryString["E"];
				if (Param!=null && Param!="")
					ei = ExchangeIntraday.GetExchangeIntraday(Param);

				//Create financial chart instance
				FormulaChart fc = new FormulaChart();
				
				string Main = Request.QueryString["Main"];
				if (Main==null || Main=="")
					fc.AddArea("MAIN",3);
				else fc.AddArea(Main,3);

				string QuoteCode = Request.QueryString["Code"];
				if (QuoteCode==null)
					QuoteCode = "";

				DataManagerBase dmb;
				Param = Request.QueryString["Provider"];
				if (Param==null)
					Param = "";
				if (string.Compare(Param,"DB",true)==0) 
				{
					//Create DBDataManager , Get stock data from sql server.
					dmb = new DBDataManager();
					((DBDataManager)dmb).AutoYahooToDB = 
						Request.QueryString["His"]!="0" && Config.AutoYahooToDB==1;
				}
				else if (Param.StartsWith("MS("))
				{
					dmb = new MSDataManager(Param.Substring(3,Param.Length-4));
				}
				else if (Param=="" || string.Compare(Param,"Yahoo",true)==0)
				{
					//Create YahooDataManager , Get stock data from yahoo.
					dmb = new YahooDataManager();
					Utils.SetYahooCacheRoot(dmb);
					if (Config.IncludeTodayQuote==1)
						((DataManagerBase)dmb).DownloadRealTimeQuote = true;
				}
				else 
				{
					string ManagerName = "WebDemos."+Param+"DataManager";
					Type t = Type.GetType(ManagerName);
					if (t==null)
						throw new Exception(ManagerName+" not found!");
					dmb = (DataManagerBase)Activator.CreateInstance(t);
					if (dmb is IntraDataManagerBase)
					{
						if (ei==null)
							ei = ExchangeIntraday.GetExchangeIntraday(Utils.GetExchange(QuoteCode));

						CacheDataManagerBase cdmb = (CacheDataManagerBase)dmb;
						cdmb.CacheTimeSpan = TimeSpan.FromMilliseconds(Config.IntradayCacheTime*1000);
						cdmb.EnableMemoryCache = true;
						//((CacheDataManagerBase)dmb).EnableFileCache = false;
						
						StartTime = ei.GetCurrentTradingDay();
						EndTime = StartTime.AddSeconds(3600*24-1);
						QuoteCode = Utils.GetPart1(QuoteCode);
					}
					dmb.EndTime = Utils.ToDateDef(Request.QueryString["End"], EndTime);
					dmb.StartTime = Utils.ToDateDef(Request.QueryString["Start"],StartTime);
				}
				if (Request.QueryString["RT"]=="1")
					((DataManagerBase)dmb).DownloadRealTimeQuote = true;

				int MainIndex = 0;
				
				if (Config.SymbolCase=="Upper")
					QuoteCode = QuoteCode.ToUpper();
				else if (Config.SymbolCase=="Lower")
					QuoteCode = QuoteCode.ToLower();

				string[] QuoteCodes = QuoteCode.Split(',');
				string Over = "";
				QuoteCode = QuoteCodes[0];
				for(int i=1; i<QuoteCodes.Length; i++) 
				{
					if (Over!="")
						Over += ";";
					Over += "Compare("+QuoteCodes[i]+")";
				}

				Param = Request.QueryString["IND"];
				string[] Inds = null;
				if (Param!=null && Param!="") 
					Inds = Param.Split(';');

				//Set stock chart size
				Param = Request.QueryString["Size"];
				try 
				{
					if (Param!=null && Param!="") 
					{
						int i = Param.IndexOf('*');
						if (i>0) 
						{
							Width = int.Parse(Param.Substring(0,i));
							Height = int.Parse(Param.Substring(i+1));
						} 
						else 
						{
							Width = int.Parse(Param);
							Height = Width *3/4;
							if (Inds!=null && Inds.Length>1)
								Height +=Height/5*(Inds.Length-2);
							Height +=36;
						}
					}
				} 
				catch 
				{
				}

				CommonDataProvider DataProvider = (CommonDataProvider)dmb[QuoteCode];

				Param = Request.QueryString["Adj"];
				DataProvider.Adjusted = Param!="0";

				if (DataProvider==null)
					throw new Exception(QuoteCode+" not found!");
				if (dmb is IntraDataManagerBase) 
				{
					fc.FixedTime = object.Equals(Request.QueryString["Fix"],"1");
					if (fc.FixedTime)
						DataProvider.IntradayInfo = ei;
				}

				if (DataProvider.Count==0)
					throw new Exception("No data found!");
				fc[MainIndex].Formulas[0].Name = QuoteCode;

				//Add MA lines to main stock view
				Param = Request.QueryString["MA"];
				if (Param!=null && Param!="")
					foreach(string s in Param.Split(';'))
						if (s!="")
							fc[MainIndex].AddFormula("FML.MA("+s+")");

				//Add EMA lines to main stock view
				Param = Request.QueryString["EMA"];
				if (Param!=null && Param!="")
					foreach(string s in Param.Split(';'))
						if (s!="")
							fc[MainIndex].AddFormula("FML.EMA("+s+")");

				//Add override lines to main stock view
				Param = Request.QueryString["OVER"];
				if (Param!=null && Param!="" ) 
				{
					if (Over!="")
						Param = Param+";"+Over;
				}
				else Param = Over;

//				if (dmb is IntraDataManagerBase) 
//				{
//					string s = DataProvider.GetStringData("LastPrice");
//					if (s!=null && s!="")
//					{
//						double d = double.Parse(s);
//						
//						s = "DotLine("+d.ToString("f"+FormulaHelper.TestBestFormat(d,0))+")";
//						if (Param!=null && Param!="")
//							Param +=";"+s;
//						else Param = s;
//					}
//				}

				if (Param!=null && Param!="") 
				{
					//foreach(string s in Param.Split(';'))
					string[] ss = Param.Split(';');
					for(int i=0; i<ss.Length; i++) 
					{
						string s = ss[i];
						if (s!="")
						{
							if (s.ToUpper()=="AREABB")
								fc[MainIndex].InsertFormula(0,"FML."+s);
							else fc[MainIndex].AddFormula("FML."+s);
						}
					}
				}

				//Add indicators to stock chart
				if (Inds!=null)
					foreach(string s in Inds)
						if (s!="")
						{ 
							int k = s.IndexOf("{U}");
							if (k>0) 
							{
								fc.InsertArea(MainIndex,"FML."+s.Substring(0,k));
								MainIndex++;
							}
							else 
								fc.AddArea("FML."+s);
						}

				//Apply build-in stock chart skin
				Param = Request.QueryString["Skin"];
				if (Param==null || Param=="")
					Param = Config.DefaultSkin;
				FormulaSkin fs = FormulaSkin.GetSkinByName(Param);
				if (fs!=null)
				{
					if (Config.YAxisFormat!="") 
						fs.AxisY.Format = Config.YAxisFormat;
					
					Param = Request.QueryString["LastX"];
					if (Param=="1" || Config.ShowXAxisInLastArea)
						fs.ShowXAxisInLastArea = true;

					fc.SetSkin(fs);
				}
				
				Param = Request.QueryString["LatestValueType"];
				try
				{
					if (Param==null || Param=="")
						Param = Config.LatestValueType;
					fc.LatestValueType =  (LatestValueType)Enum.Parse(typeof(LatestValueType), Param,true);
				}
				catch
				{
				}

				//Add compare line to other stocks
				Param = Request.QueryString["COMP"];
				if (Param!=null && Param!="") 
				{
					foreach(string s in Param.Split(';',','))
						if (s!="")
							fc[MainIndex].AddFormula("COMPARE("+s+")");
					fc[MainIndex].AxisY.ShowAsPercent = true;
					fc[MainIndex].AxisY.Format = "p1";
					fc.LatestValueType = LatestValueType.None;
				}

				//Set stock chart time period
				fc.EndTime = Utils.ToDateDef(Request.QueryString["End"],EndTime);
				fc.StartTime = Utils.ToDateDef(Request.QueryString["Start"],StartTime);
				DataProvider.DataCycle = DataCycle.Parse(Request.QueryString["Cycle"]);

				//Set X-Axis format
				string XFormat = Request.QueryString["XFormat"];
				if (XFormat!=null && XFormat!="")
				{
					fc.AxisXFormat = XFormat;
					fc.AllXFormats = null;
				}

				//Set Y-Axis format
				string YFormat = Request.QueryString["YFormat"];
				if (YFormat!=null && YFormat!="")
					fc.AxisYFormat = YFormat;

				//Set X-Axis cycle
				string XCycle = Request.QueryString["XCycle"];
				if (XCycle!=null && XCycle!="")
					fc.DataCycle = DataCycle.Parse(XCycle);

				//Set render type : Bar , Candle or Line
				Param = Request.QueryString["Type"];
				if (Param!=null && Param!="") 
				{
					StockRenderType srt;
					if (Param.Length==1)
						 srt = (StockRenderType)int.Parse(Param);
					else srt = (StockRenderType)Enum.Parse(typeof(StockRenderType),Param,true);
					fc[MainIndex].StockRenderType = srt;
				}
				
				//Set Scale type : Normal or Log
				Param = Request.QueryString["Scale"];
				if (Param!=null && Param!="") 
				{
					ScaleType st;
					if (Param.Length==1)
						st = (ScaleType)int.Parse(Param);
					else st = (ScaleType)Enum.Parse(typeof(ScaleType),Param,true);
					fc[MainIndex].AxisY.Scale= st;
				}

				//Bottom margin
				Param = Request.QueryString["BMargin"];
				if (Param!=null && Param!="")
					fc[MainIndex].BottomMargin  = Tools.ToDoubleDef(Param,0);

				//Bind stock data
				fc.DataProvider = DataProvider;

				bool IsCompare = false;
				for(int i=1; i<fc[MainIndex].FormulaDataArray.Count; i++)
					if (!fc[MainIndex].AxisY.ShowAsPercent && 
						fc[MainIndex][i].ParentFormula.FormulaName.StartsWith("COMPARE"))
					{
						//FormulaAxisY fay = fc[MainIndex].AddNewAxisY(AxisPos.Left);
						FormulaData fd = fc[MainIndex][i];
						fd.Transform = Transform.Normal;
						fc[MainIndex][i].ParentFormula.AxisYIndex = 1;
						IsCompare = true;
						//fd.AxisYIndex = 1;
						break;
					}
				fc.ExtendYAxis(TwoYAxisType.AreaDifferent);

				for(int i=0;  i<fc[MainIndex].AxisYs.Count; i++)
					if (fc[MainIndex].IsMain(i) || IsCompare)
						fc[MainIndex].AxisYs[i].AutoMultiply = false;

				fc[MainIndex].AxisY.MajorTick.ShowLine = Config.ShowMainAreaLineY;
				fc[MainIndex].AxisX.MajorTick.ShowLine = Config.ShowMainAreaLineX;

				//Set indicator line width
				Param = Request.QueryString["Width"];
				if (Param!=null && Param!="") 
				{
					float LineWidth = float.Parse(Param);
					for(int i=0; i<fc.Areas.Count; i++)
						fc[i].LinePen.Width = LineWidth;
				}
				fc[MainIndex].RemoveAutoMultiplyForStockYAxis();// AxisY.AutoMultiply = false;

				if (Request.QueryString["SV"]=="0")
					fc.ShowValueLabel = false;
				fc.StickRenderType = Config.StickRenderType;
				
				
				bool b1 = Request.QueryString["HideX"]=="1";
				bool b2 = Request.QueryString["HideY"]=="1";
				foreach(FormulaArea fa in fc.Areas) 
				{
					fa.AxisY.Visible = fa.AxisY.Visible && !b2;
					fa.AxisX.Visible = fa.AxisX.Visible && !b1;
				}

				Rectangle Rect = new Rectangle(0,0,Width,Height);
				Param = Request.QueryString["Layout"];
				if (Param==null || Param=="")
					Param = "Default";
				string[] Layouts = Param.Split(';',',');
				Layout L = Layout.ParseString(Config.Read(Layouts[0]+"Layout","ChartRect=(0,0,0,0)"),Rect);//,DataProvider);
				for(int j=1; j<Layouts.Length; j++)
					L.Merge(Layout.ParseString(Config.Read(Layouts[j]+"Layout","ChartRect=(0,0,0,0)"),Rect));//,DataProvider));
				
				L.CompanyName = Config.CompanyName;
				L.URL = Config.URL;
				L.StartTick = Start;
				fc.Rect = L.ChartRect;
				fc.PriceLabelFormat = "{CODE}";
				fc.NativePaint+=new NativePaintHandler(fc_NativePaint);
				B = fc.GetBitmap(Width,Height,fc.Rect);

				Graphics g = Graphics.FromImage(B);

				int X = Tools.ToIntDef(Request.QueryString["X"],0);
				int Y = Tools.ToIntDef(Request.QueryString["Y"],0);
				if (X>0 && Y>0)
				{
					fc.SetCursorPos(X,Y);
					fc.Rect = new Rectangle(0,0,Width,Height);
					fc.ShowCursorLabel = true;
					fc.DrawCursor(g,X,Y);
				}
				else 
				{

					int Pos = fc.DateToIndex(fc.EndTime);
					if (Pos>=DataProvider.Count)
						Pos = DataProvider.Count-1;
					fc.CursorPos = Pos;
				}
				L.Render(g,Rect,fc,Point.Empty,fc.CursorPos);//-1 ; fc.CursorPos

			}
			catch (Exception ex)
			{
				B = new Bitmap(Width,Height);
				Graphics g = Graphics.FromImage(B);
				
				g.Clear(Color.White);

				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.DrawString(ex.ToString()/*.Message*/,new Font("Verdana",12),
					new SolidBrush(Color.FromArgb(196,Color.Black)),new Rectangle(0,0,Width,Height),sf);
			}

			//Create stock image to Bitmap
			Response.ContentType = "Image/"+Config.ImageFormat;

			//Create chart stream
			MemoryStream ms = new MemoryStream();
			if (Config.ImageFormat == "Gif") 
			{
				Type tQuantization = (Type)Application["Quantization"];
				if (tQuantization==null)
				{
					Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
					foreach(Assembly a in ass)
					{
						if (a.FullName.StartsWith("ImageQuantization")) 
							tQuantization = a.GetType("ImageQuantization.OctreeQuantizer"); 
					}
					Application["Quantization"] = tQuantization;
				}
				
				if (tQuantization!=null)
				{
					object[] os;
					if (Config.TransparentColor.IsEmpty)
						os = new object[]{B,ms,Config.GifColors};
					else os=new object[]{B,ms,Config.GifColors,Config.TransparentColor};

					tQuantization.InvokeMember("Save",
						BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,null,null,os);
				}

				if (ms.Length==0)
					B.Save(ms,ImageFormat.Png);
			}
			else if (Config.ImageFormat == "Png")
				B.Save(ms,ImageFormat.Png);
			else if (Config.ImageFormat == "Jpeg")
				B.Save(ms,ImageFormat.Jpeg);
			
			//Output the chart stream to web browser
			ms.WriteTo(Response.OutputStream);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void fc_NativePaint(object sender, NativePaintArgs e)
		{
			//Draw Water Mark
			string s = Config.WaterMarkText;
			if (s!="")
			{
				StringFormat DrawFormat = new StringFormat();
				DrawFormat.Alignment = StringAlignment.Center;
				DrawFormat.LineAlignment = StringAlignment.Center;
				
				e.Graphics.DrawString(s,Config.WaterMarkFont,
					new SolidBrush(Config.WaterMarkColor),e.Rect,DrawFormat);
			}
		}
	}
}