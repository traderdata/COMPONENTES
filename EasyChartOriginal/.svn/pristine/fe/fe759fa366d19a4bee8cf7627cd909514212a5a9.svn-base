using System;
using System.Globalization;
using System.Text;
using System.Net;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// This class manage stock data of certain day
	/// </summary>
	public class DataPacket
	{
		/// <summary>
		/// The float array size of DataPackage
		/// </summary>
		static public int PacketSize = 9;
		/// <summary>
		/// The byte array size of DataPackate
		/// </summary>
		public static int PacketByteSize = PacketSize*4;
		public static int MaxValue = 365*1000;
		public double Open;
		public double High;
		public double Low;
		public double Close;
		public double Last;
		public double Volume;
		public double AdjClose;
		public string Symbol;
		public string StockName;
		public string Exchange;
		public double TimeZone;
		public DateTime Date;
		public double DoubleDate
		{
			get
			{
				return Date.ToOADate();
			}
		}

		public double this[string Type]
		{
			get 
			{
				switch (Type.ToUpper())
				{
					case "OPEN":
						return Open;
					case "DATE":
						return DoubleDate;
					case "HIGH":
						return High;
					case "LOW":
						return Low;
					case "CLOSE":
						return Close;
					case "VOLUME":
						return Volume;
					case "ADJCLOSE":
						return AdjClose;
				}
				return double.NaN;
			}
		}

		/// <summary>
		/// Create DataPackage
		/// </summary>
		/// <param name="Date"></param>
		/// <param name="Open"></param>
		/// <param name="High"></param>
		/// <param name="Low"></param>
		/// <param name="Close"></param>
		/// <param name="Volume"></param>
		/// <param name="AdjClose"></param>
		public DataPacket(
			string Symbol,
			DateTime Date,
			double Open,
			double High,
			double Low,
			double Close,
			double Volume,
			double AdjClose,
			double Last)
		{
			this.Symbol = Symbol;
			this.Date= Date;
			//this.DoubleDate =Date.ToOADate();
			this.Close = Close;

			if (AdjClose==0) 
				this.AdjClose = Close;
			else this.AdjClose = AdjClose; 

			if (Open==0) 
				this.Open = Close;
			else this.Open = Open;

			if (High==0) 
				this.High = Close;
			else this.High = High;
			
			if (Low==0) 
				this.Low = Close;
			else this.Low = Low;

			this.Volume = Volume;
			this.Last = Last;
		}

		public DataPacket(
			string Symbol,
			DateTime Date,
			double Open,
			double High,
			double Low,
			double Close,
			double Volume,
			double AdjClose) :this(Symbol,Date,Open,High,Low,Close,Volume,AdjClose,0)
		{
		}

		public DataPacket(
			string Symbol,
			double DoubleDate,
			double Open,
			double High,
			double Low,
			double Close,
			double Volume,
			double AdjClose) :this(Symbol,DoubleDate,Open,High,Low,Close,Volume,AdjClose,0)
		{
		}

		/// <summary>
		/// Create DataPackage
		/// </summary>
		/// <param name="DoubleDate"></param>
		/// <param name="Open"></param>
		/// <param name="High"></param>
		/// <param name="Low"></param>
		/// <param name="Close"></param>
		/// <param name="Volume"></param>
		/// <param name="AdjClose"></param>
		public DataPacket(
			string Symbol,
			double DoubleDate,
			double Open,
			double High,
			double Low,
			double Close,
			double Volume,
			double AdjClose,
			double Last) :this(Symbol,DateTime.FromOADate(DoubleDate),Open,High,Low,Close,Volume,AdjClose,Last)
		{
		}

		public DataPacket(string Symbol,DateTime QuoteTime,double Price,double Volume) :
			//this(Symbol,QuoteTime,float.NaN,float.NaN,float.NaN,Price,Volume,Price,float.NaN)
			this(Symbol,QuoteTime,0,0,0,Price,Volume,Price,0)
		{
		}

		public DataPacket(string Symbol,double DoubleDate,double Price,double Volume) :
			this(Symbol,DateTime.FromOADate(DoubleDate),Price,Volume)
		{
		}

		/// <summary>
		/// Get date time from float array
		/// </summary>
		/// <param name="fs">Float data array</param>
		/// <param name="i">The index of the date time</param>
		/// <returns>DateTime</returns>
		public static DateTime GetDateTime(float[] fs,int i) 
		{
			double[] ds = new double[1];
			try 
			{
				Buffer.BlockCopy(fs,i*PacketByteSize,ds,0,8);
				return DateTime.FromOADate(ds[0]);
			} 
			catch (Exception ex)
			{
				throw new Exception(ex.Message+";"+ds[0]+";i="+i+";Length="+(fs.Length/PacketByteSize));
			}
		}

		/// <summary>
		/// Get date time from float array
		/// </summary>
		/// <param name="fs">Float data array</param>
		/// <returns></returns>
		public static DateTime GetDateTime(float[] fs) 
		{
			return GetDateTime(fs,0);
		}

		/// <summary>
		/// Get volume from float array
		/// </summary>
		/// <param name="fs"></param>
		/// <returns></returns>
		public static double GetVolume(float[] fs) 
		{
			double[] ds = new double[1];
			Buffer.BlockCopy(fs,24,ds,0,8);
			return ds[0];
		}

		/// <summary>
		/// Create the DataPackage by parse data from EODData.com
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static DataPacket ParseEODData(string s) 
		{
			string[] ss = s.Split(',');
			IFormatProvider fp = FormulaHelper.USFormat;
				//new System.Globalization.CultureInfo("en-US", true);
			
			try 
			{
				for(int i=0; i<ss.Length; i++)
					ss[i] = ss[i].Trim();
				DataPacket dp = new DataPacket(
					ss[0],
					DateTime.Parse(ss[1], fp),//must use fp for none-US machine,
					double.Parse(ss[2],fp),
					double.Parse(ss[3],fp),
					double.Parse(ss[4],fp),
					double.Parse(ss[5],fp),
					double.Parse(ss[6],fp) ,
					double.Parse(ss[5],fp));
				
				
				return dp;
			}
			catch// (Exception e)
			{
				//throw new Exception(e.Message+";"+s);
			}
			return null;
		}

		/// <summary>
		/// Remove " " of the input
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string RemoveQuotation(string s)
		{
			return s.Trim(' ','"','\r','\n');
//			s = s.Trim();
//			if (s.StartsWith("\"") && s.EndsWith("\""))
//				s = s.Substring(1,s.Length-2);
//			return s;
		}

		public static double ToDouble(string s,double Def)
		{
			try 
			{
				//CultureInfo ci = new CultureInfo("en-US",false);
				IFormatProvider fp = FormulaHelper.USFormat;
				return double.Parse(s,fp);
			}
			catch 
			{
				return Def;
			}
		}

		public static float ToFloat(string s,float Def)
		{
			try 
			{
				return float.Parse(s,NumberFormatInfo.InvariantInfo);
			}
			catch 
			{
				return Def;
			}
		}


		/// <summary>
		/// Parse real-time data from yahoo
		/// </summary>
		/// <param name="s">"MSFT","MICROSOFT CP","7/8/2004","4:00pm",27.88,"27.55 - 28.15",27.64,59132496,28.10,"NasdaqNM"</param>
		/// <returns></returns>
		public static DataPacket ParseYahoo(string s)
		{
			try 
			{
				string[] ss = FormulaHelper.Split(s,',');// s.Split(',');
				string[] sss = RemoveQuotation(ss[5]).Split('-');
				double Last = 0;

				if (ss.Length>8)
					Last =ToDouble(ss[8],Last);

				IFormatProvider fp = DateTimeFormatInfo.InvariantInfo;
				DataPacket dp = 
					new DataPacket(
					RemoveQuotation(ss[0]),
					DateTime.ParseExact(RemoveQuotation(ss[2])+" "+RemoveQuotation(ss[3]),"M/%d/yyyy h:mmtt",
					fp,DateTimeStyles.AllowWhiteSpaces),
					//DateTime.Parse(RemoveQuotation(ss[2])+" "+RemoveQuotation(ss[3]),fp),
					ToDouble(ss[4],0),
					ToDouble(sss[1],0),
					ToDouble(sss[0],0),
					ToDouble(ss[6],0),
					ToDouble(ss[7],0),
					ToDouble(ss[6],0),
					Last);
				dp.StockName = RemoveQuotation(ss[1]);
				dp.TimeZone = ExchangeIntraday.YahooTimeZone;
				if (ss.Length>9)
					dp.Exchange = RemoveQuotation(ss[9]);

				return dp;
			} 
			catch
			{
				return null;
				//throw new Exception(e.Message+";"+s);
			}
		}

		private static string GetFromYahoo(string Code)
		{
			return YahooDataManager.DownloadRealtimeFromYahoo(Code,"snd1t1oml1vpx");
		}

		/// <summary>
		/// Download real-time stock data from yahoo finance
		/// </summary>
		/// <param name="Code">Symbol</param>
		/// <returns>DataPackage</returns>
		public static DataPacket DownloadFromYahoo(string Code) 
		{
			string s = GetFromYahoo(Code);
			return DataPacket.ParseYahoo(s);
		}

		/// <summary>
		/// Download multi-symbol real-time stock data from yahoo finance
		/// </summary>
		/// <param name="Codes">Symbols</param>
		/// <returns>Array of DataPackage</returns>
		public static DataPacket[] DownloadMultiFromYahoo(string Codes) 
		{
			string s = GetFromYahoo(Codes);
			string[] ss= s.Trim().Split('\r');
			DataPacket[] dps = new DataPacket[ss.Length];
			for(int i=0; i<ss.Length; i++)
				dps[i] = DataPacket.ParseYahoo(ss[i]);
			return dps;
		}

		/// <summary>
		/// Get the float array of current DataPackage
		/// </summary>
		/// <returns>Array of float</returns>
		public float[] GetFloat()
		{
			float[] fs = new float[PacketSize];
			double[] ds = {DoubleDate};
			Buffer.BlockCopy(ds,0,fs,0,8);
			fs[2] = (float)Open;
			fs[3] = (float)High;
			fs[4] = (float)Low;
			fs[5] = (float)Close;
			ds[0] = Volume;
			Buffer.BlockCopy(ds,0,fs,24,8);
			fs[8] = (float)AdjClose;
			return fs;
		}

		static public DataPacket FromBytes(byte[] bs)
		{
			float[] fs = new float[PacketSize];
			Buffer.BlockCopy(bs,0,fs,0,PacketByteSize);
			double[] ds = {0};
			Buffer.BlockCopy(fs,0,ds,0,8);
			double DoubleDate = ds[0];
			float Open = fs[2];
			float High = fs[3];
			float Low = fs[4];
			float Close = fs[5];
			Buffer.BlockCopy(fs,24,ds,0,8);
			double Volume = ds[0];
			float AdjClose = fs[8];
			return new DataPacket("",DoubleDate,Open,High,Low,Close,Volume,AdjClose);
		}

		/// <summary>
		/// Merge streaming data
		/// </summary>
		/// <param name="sd">Streaming Data</param>
		/// <returns>true if merged, false if this is a new data</returns>
		public bool Merge(DataPacket dp,DataCycle dc)
		{
			if (!dc.SameSequence(dp.Date,Date))
			//if (dp.Date>=Date.AddMinutes(1)) 
			{
				Open = dp.Close;
				High = dp.Close;
				Low = dp.Close;
				Close = dp.Close;
				AdjClose = dp.Close;
				Volume = dp.Volume;
				Date = dp.Date;
				return false;
			} 
			else 
			{
				Close = dp.Close;
				AdjClose = Close;
				High = Math.Max(High,Close);
				Low = Math.Min(Low,Close);
				Volume =dp.Volume;
				Date = dp.Date;
				return true;
			}
		}

		/// <summary>
		/// Get the byte array of current DataPackage
		/// </summary>
		/// <returns></returns>
		public byte[] ToByte() 
		{
			byte[] bs = new Byte[PacketByteSize];
			float[] fs = GetFloat();
			Buffer.BlockCopy(fs,0,bs,0,bs.Length);
			return bs;
		}

		public bool IsZeroValue
		{
			get 
			{
				return Open==0 || High==0 || Low==0 || Close==0;
			}
		}

		public DateTime GetExchangeTime(ExchangeIntraday ei)
		{
			if (TimeZone==double.MaxValue)
				return Date;
			return Date.AddHours(ei.TimeZone);
		}
	}
}