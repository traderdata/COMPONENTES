using System;
using System.Collections;
using System.IO;
using System.Globalization;

namespace Easychart.Finance.DataProvider
{
	public enum MergeCycleType{HIGH,LOW,OPEN,CLOSE,ADJCLOSE,SUM};
	/// <summary>
	/// CSV Data Provider 
	/// 1.Get data from yahoo csv data
	/// 2.Save data as binary
	/// 3.Load data from binary
	/// </summary>
	public class CommonDataProvider:IDataProvider
	{
		private Hashtable htData = new Hashtable();
		private Hashtable htRealtime = new Hashtable();
		private Hashtable htConstData = new Hashtable();
		private Hashtable htStringData = new Hashtable();
		private Hashtable htAllCycle = new Hashtable();
		private Hashtable htGroupping = new Hashtable();
		private DataCycle dataCycle = DataCycle.Day;
		private IDataProvider baseDataProvider;
		private int maxCount = -1;
		private bool adjusted = true;
		private IDataManager dm;
		private static string[] Keys = {"DATE","OPEN","HIGH","LOW","CLOSE","VOLUME","ADJCLOSE"};

		static public CommonDataProvider Empty
		{
			get
			{
				CommonDataProvider cdp = new CommonDataProvider(null);
				cdp.LoadByteBinary(new byte[]{});
				return cdp;
			}
		}

		public ICollection AllDataType
		{
			get
			{
				return htData.Keys;
			}
		}

		private bool isPointAndFigure;
		public bool IsPointAndFigure
		{
			get
			{
				return isPointAndFigure;
			}
			set
			{
				isPointAndFigure = value;
			}
		}

		private int futureBars;
		public int FutureBars
		{
			get
			{
				return futureBars;
			}
			set
			{
				futureBars = value;
			}
		}

		private ExchangeIntraday intradayInfo;
		/// <summary>
		/// Intraday information
		/// </summary>
		public ExchangeIntraday IntradayInfo
		{
			get
			{
				return intradayInfo;
			}
			set
			{
				intradayInfo = value;
				htAllCycle.Clear();
			}
		}

		/// <summary>
		/// Get or Set the current data cycle
		/// </summary>
		public DataCycle DataCycle
		{
			get { return dataCycle; }
			set { dataCycle = value; }
		}

		int weekAdjust;
		/// <summary>
		/// Used to adjust weekend when week grouping
		/// </summary>
		public int WeekAdjust
		{
			get { return weekAdjust; }
			set { weekAdjust = value; }
		}

		MergeCycleType dateMergeType = MergeCycleType.CLOSE;//OPEN; //MergeCycleType.CLOSE; //
		/// <summary>
		/// Date merge type in data grouping.  Possible value is MergeCycleType.CLOSE or MergeCycleType.OPEN
		/// </summary>
		public MergeCycleType DateMergeType
		{
			get{ return dateMergeType;}
			set{ dateMergeType = value; }
		}

		/// <summary>
		/// Adjust the date of current DataProvider according to BaseDataProvider
		/// </summary>
		public IDataProvider BaseDataProvider
		{
			get { return baseDataProvider; }
			set { baseDataProvider = value; }
		}

		/// <summary>
		/// Create the data provider
		/// </summary>
		/// <param name="dm"></param>
		public CommonDataProvider(IDataManager dm)
		{
			this.dm = dm;
		}

		public void LoadStreamingBinary(Stream stream)
		{
			byte[] bs = new Byte[stream.Length];
			stream.Read(bs,0,(int)stream.Length);
			LoadStreamingBinary(bs);
		}

		public void LoadStreamingBinary(string FileName) 
		{
			using (FileStream fs = File.OpenRead(FileName))
			{
				LoadStreamingBinary(fs);
			} 
		}

		public void LoadStreamingBinary(byte[] bs)
		{
			int N = bs.Length/24;
			htData.Clear();
			double[] CLOSE = new double[N];
			double[] VOLUME = new double[N];
			double[] DATE = new double[N];
			
			double[] ds = new double[N*3];
			Buffer.BlockCopy(bs,0,ds,0,bs.Length);

			for(int i=0; i<N; i++) 
			{
				DATE[i] = ds[i*3];
				CLOSE[i] = ds[i*3+1];
				VOLUME[i] = ds[i*3+2];
			}
			htData.Add("CLOSE",CLOSE);
			htData.Add("VOLUME",VOLUME);
			htData.Add("DATE",DATE);
			htAllCycle.Clear();
		}
		
		public static void AppendStreamingData(string Path,StreamingData sd)
		{
			using(FileStream fs = File.OpenWrite(string.Format(Path,sd.Symbol)))
			{
				using (BinaryWriter bw = new BinaryWriter(fs)) 
				{
					bw.Write(sd.QuoteTime.ToOADate());
					bw.Write(sd.Price);
					bw.Write(sd.Volume);
				}
			}
		}

		/// <summary>
		/// Load binary stock data from stream
		/// </summary>
		/// <param name="stream"></param>
		public void LoadBinary(Stream stream)
		{
			byte[] bs = new Byte[stream.Length];
			stream.Read(bs,0,(int)stream.Length);
			LoadByteBinary(bs);
		}

		/// <summary>
		/// Load binary stock data from File
		/// </summary>
		/// <param name="FileName"></param>
		public void LoadBinary(string FileName) 
		{
			using (FileStream fs = new FileStream(FileName,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite))
				LoadBinary(fs);
		}

		/// <summary>
		/// Load binary stock data from byte array
		/// </summary>
		/// <param name="bs"></param>
		public void LoadByteBinary(byte[] bs)
		{
			LoadBinary(bs,bs.Length/DataPacket.PacketByteSize);
		}

		/// <summary>
		/// Load lastest N days binary stock data from byte array
		/// </summary>
		/// <param name="bs">Byte array</param>
		/// <param name="N">How many days to be loaded</param>
		public void LoadBinary(byte[] bs,int N)
		{
			htData.Clear();
			double[] CLOSE = new double[N];
			double[] OPEN = new double[N];
			double[] HIGH = new double[N];
			double[] LOW = new double[N];
			double[] VOLUME = new double[N];
			double[] DATE = new double[N];
			double[] ADJCLOSE = new double[N];
			
			float[] fs = new float[N*DataPacket.PacketSize];
			Buffer.BlockCopy(bs,0,fs,0,bs.Length);

			for(int i=0; i<N; i++) 
			{
				Buffer.BlockCopy(fs,i*DataPacket.PacketByteSize,DATE,i*8,8);
				CLOSE[i] = fs[i*DataPacket.PacketSize+5];

				OPEN[i] = fs[i*DataPacket.PacketSize+2];
				if (OPEN[i]==0)
					OPEN[i] = CLOSE[i];
				HIGH[i] = fs[i*DataPacket.PacketSize+3];
				if (HIGH[i]==0)
					HIGH[i] = CLOSE[i];
				LOW[i] = fs[i*DataPacket.PacketSize+4];
				if (LOW[i]==0)
					LOW[i] = CLOSE[i];
				Buffer.BlockCopy(fs,(i*DataPacket.PacketSize+6)*4 ,VOLUME,i*8,8);
				ADJCLOSE[i] = fs[i*DataPacket.PacketSize+8];
			}
			htData.Add("CLOSE",CLOSE);
			htData.Add("OPEN",OPEN);
			htData.Add("HIGH",HIGH);
			htData.Add("LOW",LOW);
			htData.Add("VOLUME",VOLUME);
			htData.Add("DATE",DATE);
			htData.Add("ADJCLOSE",ADJCLOSE);
			htAllCycle.Clear();
		}

		/// <summary>
		/// Load binary stock data from two dimension double data array
		/// ds[0] : OPEN
		/// ds[1] : HIGH
		/// ds[2] : LOW
		/// ds[3] : CLOSE
		/// ds[4] : VOLUME
		/// ds[5] : DATE
		/// ds[6] : ADJCLOSE (optional)
		/// </summary>
		/// <param name="ds">The two dimension double data array</param>
		public void LoadBinary(double[][] ds)
		{
			if (ds.Length>4) 
			{
				htData.Clear();
				htData.Add("OPEN",ds[0]);
				htData.Add("HIGH",ds[1]);
				htData.Add("LOW",ds[2]);
				htData.Add("CLOSE",ds[3]);
				htData.Add("VOLUME",ds[4]);
				htData.Add("DATE",ds[5]);
				if (ds.Length>6)
					htData.Add("ADJCLOSE",ds[6]);
				else 
				{
					double[] ADJCLOSE = new double[ds[0].Length];
					Buffer.BlockCopy(ds[3],0,ADJCLOSE,0,ds[0].Length*8);
					htData.Add("ADJCLOSE",ADJCLOSE);
				}
			}
			htAllCycle.Clear();
		}

		public void LoadBinary(string DataType,double[] ds)
		{
			htData[DataType.ToUpper()] =ds;
			htAllCycle.Clear();
		}

		/// <summary>
		/// Set the groupping method for DataType, sample SetGroupping("DATE",MergeCycleType.SUM);
		/// </summary>
		/// <param name="DataType"></param>
		/// <param name="mct"></param>
		public void SetGroupping(string DataType,MergeCycleType mct)
		{
			htGroupping[DataType] = mct;
			htAllCycle.Clear();
		}

		public void TrimTime()
		{
			double[] Date = (double[])htData["DATE"];
			for(int i=0; i<Date.Length; i++)
				Date[i] = (int)Date[i];
		}

		/// <summary>
		/// Marge DataPackage to the byte data array
		/// </summary>
		/// <param name="bs">Byte data array</param>
		/// <param name="dp">DataPackage to be merged</param>
		/// <returns></returns>
		public static byte[] MergeOneQuote(byte[] bs,DataPacket dp) 
		{
			float[] fs = new float[bs.Length / 4+DataPacket.PacketSize];
			System.Buffer.BlockCopy(bs,0,fs,0,bs.Length);
			DateTime d2 = dp.Date.Date;
			int Count = fs.Length/DataPacket.PacketSize-1;
			for(int i=Count-1; i>=-1; i--)
			{
				DateTime d1 = DateTime.MinValue;
				if (i>-1)
					d1 = DataPacket.GetDateTime(fs,i).Date;

				int Extra = 0;
				if (d1<=d2) 
				{
					if (d1<d2) 
					{
						if (i<Count-1 && Count>0)
							System.Buffer.BlockCopy(
								fs,(i+1)*DataPacket.PacketByteSize,fs,
								(i+2)*DataPacket.PacketByteSize,
								(Count-i-1)*DataPacket.PacketByteSize);
						Extra = 1;
					}
					float[] fsCurrent = dp.GetFloat();
					System.Buffer.BlockCopy(fsCurrent,0,fs,(i+Extra)*DataPacket.PacketByteSize,DataPacket.PacketByteSize);
					bs = new byte[fs.Length*4-(1-Extra)*DataPacket.PacketByteSize];
					System.Buffer.BlockCopy(fs,0,bs,0,bs.Length);
					//}
					return bs;
				};
			}
			return bs;
		}
	
		/// <summary>
		/// Save binary stock data to stream
		/// </summary>
		/// <param name="stream"></param>
		public void SaveBinary(Stream stream) 
		{
			using (BinaryWriter bw = new BinaryWriter(stream))
			{
				double[] CLOSE = (double[])htData["CLOSE"];
				double[] OPEN = (double[])htData["OPEN"];
				double[] HIGH = (double[])htData["HIGH"];
				double[] LOW = (double[])htData["LOW"];
				double[] VOLUME = (double[])htData["VOLUME"];
				double[] DATE = (double[])htData["DATE"];
				double[] ADJCLOSE = (double[])htData["ADJCLOSE"];

				int N=CLOSE.Length;
				for(int i=0; i<N; i++) 
				{
					bw.Write(DATE[i]);
					bw.Write((float)OPEN[i]);
					bw.Write((float)HIGH[i]);
					bw.Write((float)LOW[i]);
					bw.Write((float)CLOSE[i]);
					bw.Write(VOLUME[i]);
					bw.Write((float)ADJCLOSE[i]);
				}
			}
		}

		/// <summary>
		/// Save binary stock data to file
		/// </summary>
		/// <param name="FileName"></param>
		public void SaveBinary(string FileName) 
		{
			using (FileStream fs = new FileStream(FileName,FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite))
				SaveBinary(fs);
		}

		/// <summary>
		/// Save binary stock data to byte array
		/// </summary>
		/// <returns></returns>
		public byte[] SaveBinary() 
		{
			MemoryStream ms = new MemoryStream();
			SaveBinary(ms);
			return ms.ToArray();
		}

		public void SaveStreamingBinary(Stream stream) 
		{
			using (BinaryWriter bw = new BinaryWriter(stream))
			{
				double[] CLOSE = (double[])htData["CLOSE"];
				double[] VOLUME = (double[])htData["VOLUME"];
				double[] DATE = (double[])htData["DATE"];

				int N=CLOSE.Length;
				for(int i=0; i<N; i++) 
				{
					bw.Write(DATE[i]);
					bw.Write(CLOSE[i]);
					bw.Write(VOLUME[i]);
				}
			}
		}

		public void SaveStreamingBinary(string FileName) 
		{
			SaveStreamingBinary(File.OpenWrite(FileName));
		}

		public byte[] SaveStreamingBinary() 
		{
			MemoryStream ms = new MemoryStream();
			SaveStreamingBinary(ms);
			return ms.ToArray();
		}

		private double Min(double d1,double d2)
		{
			if (double.IsNaN(d1))
				return d2;
			else return Math.Min(d1,d2);
		}

		private double Max(double d1,double d2)
		{
			if (double.IsNaN(d1))
				return d2;
			else return Math.Max(d1,d2);
		}

		private double Sum(double d1,double d2)
		{
			if (double.IsNaN(d1))
				return d2;
			else return d1+d2;
		}
		
		private double First(double d1,double d2)
		{
			if (double.IsNaN(d1))
				return d2;
			else return d1;
		}

		private void MergeCycle(double[] ODATE,int[] NEWDATE,double[] CLOSE,double[] ADJCLOSE,double[] ht,double[]htCycle, MergeCycleType mct,bool DoAdjust)
		{
			int Last = -1;
			int j = -1;
			for(int i=0; i<ODATE.Length; i++) 
			{
				double Factor = 1;
				if (DoAdjust && ADJCLOSE!=null)
					Factor = ADJCLOSE[i]/CLOSE[i];
				double d = ht[i]*Factor;
				if (Factor!=1)
					d = Math.Round(d,2);
				if (Last!=NEWDATE[i]) 
				{
					j++;
					htCycle[j] = d;
				} 
				else 
				{
					if (!double.IsNaN(d))
					{
						if (mct==MergeCycleType.HIGH)
							htCycle[j] = Max(htCycle[j],d);
						else if (mct==MergeCycleType.LOW)
							htCycle[j] = Min(htCycle[j],d);
						else if (mct==MergeCycleType.CLOSE)
							htCycle[j] = d;
						else if (mct==MergeCycleType.ADJCLOSE)
							htCycle[j] = ht[i];
						else if (mct==MergeCycleType.OPEN) 
							htCycle[j] = First(htCycle[j],d);
						else htCycle[j] = Sum(htCycle[j],d);
					}
				}
				Last = NEWDATE[i];
			}
		}

		/// <summary>
		/// Get stock data of certain data cycle
		/// </summary>
		/// <param name="dc">The data cycle</param>
		/// <returns></returns>
		public Hashtable GetCycleData(DataCycle dc)
		{
			if (dc.CycleBase==DataCycleBase.DAY && dc.Repeat==1 && !Adjusted)
				return htData;
			else 
			{
				dc.WeekAdjust = weekAdjust;
				Hashtable htCycle = (Hashtable)htAllCycle[dc.ToString()];
				if (htCycle==null)
				{
					if (htData==null) return htData;

					Hashtable ht = htData;
					if (intradayInfo!=null)
						ht = DoExpandMinute(ht);

					if (futureBars!=0)
						ht = ExpandFutureBars(ht);

					if (ht["CLOSE"]!=null)
					{
						if (ht["OPEN"]==null) ht["OPEN"] =ht["CLOSE"];
						if (ht["HIGH"]==null) ht["HIGH"] = ht["CLOSE"];
						if (ht["LOW"]==null) ht["LOW"] = ht["CLOSE"];
					}

					double[] ODATE = (double[])ht["DATE"];
					if (ODATE==null)
						return null;

					int[] NEWDATE = new int[ODATE.Length];

					int Last = int.MinValue;
					int j=-1;
					int Num;
					for(int i=0; i<ODATE.Length; i++) 
					{
						if (DataCycle.CycleBase==DataCycleBase.TICK)
							Num = i / DataCycle.Repeat;
						else Num = DataCycle.GetSequence(ODATE[i]);
						if (Num>Last) j++;
						NEWDATE[i] = j;
						Last = Num;
					}

					htCycle = new Hashtable();
					foreach(string s in ht.Keys)
						htCycle[s] = new double[j+1];
					
					bool NeedAdjust = (Adjusted && ht["ADJCLOSE"]!=null && ht["CLOSE"]!=null);
					double[] CLOSE = (double[])ht["CLOSE"];
					double[] ADJCLOSE =(double[])ht["ADJCLOSE"];

					foreach(string s in ht.Keys)
					{
						bool DoAdjust = NeedAdjust;
						MergeCycleType mct;
						DoAdjust = false;
						if (htGroupping[s]!=null)
							mct = (MergeCycleType)htGroupping[s];
						else if (s=="DATE")
							mct = dateMergeType;// MergeCycleType.OPEN;
						else if (s=="VOLUME" || s=="AMOUNT")
							mct = MergeCycleType.SUM;
						else 
							try
							{
								mct = (MergeCycleType)Enum.Parse(typeof(MergeCycleType),s);
								DoAdjust = true;
							} 
							catch
							{
								mct = MergeCycleType.CLOSE;
							}
						MergeCycle(ODATE,NEWDATE,CLOSE,ADJCLOSE,(double[])ht[s],(double[])htCycle[s], mct,DoAdjust);
					}
					htAllCycle[dc.ToString()] = htCycle;
				}
				return htCycle;
			}
		}

		private Hashtable ExpandFutureBars(Hashtable ht)
		{
			double[] Date = (double[])ht["DATE"];
			if (Date!=null && Date.Length>0)
			{
				Hashtable htRst = new Hashtable();
				double[] dc = (double[])ht["CLOSE"];
				foreach(string s in ht.Keys)
				{
					double[] d1 =(double[])ht[s];
					double[] d2 = new double[d1.Length+futureBars];
					for(int i=0; i<d1.Length; i++)
						d2[i] = d1[i];

					double date = DateTime.Today.ToOADate();
					if (s=="DATE" && d1.Length>0)
						date = d1[d1.Length-1];

					for(int i=d1.Length; i<d2.Length; i++)
						if (s=="DATE")
							d2[i] = date + (i-d1.Length)+1;
						else d2[i] = double.NaN;
					htRst[s] = d2;
				}
				return htRst;
			}
			return ht;
		}

		/// <summary>
		/// If there is no data of certain minute , Insert a duplicate value. Used for fixed time intraday chart.
		/// </summary>
		private Hashtable DoExpandMinute(Hashtable ht)
		{
			double[] Date = (double[])ht["DATE"];
			if (Date!=null && Date.Length>0)
			{
				double Minute1 = 1.0/24/60;
				double start = (int)Date[0];
				double end = (int)(Date[Date.Length-1]+1);

				ArrayList al = new ArrayList();
				ArrayList alPos = new ArrayList();
				double D = (int)start;

				for(int j=1; j<Date.Length; j++)
				{
					int i1 = (int)Date[j];
					int i2 = (int)Date[j-1];
					if ((i1-i2)>1)
						intradayInfo.AddRemoveDays(i2,i1);
				}

				int i = 0;
				while (D<=end)
				{
					if (intradayInfo.InTimePeriod(D))
					{
						if (i>=Date.Length)
						{
							al.Add(D);
							alPos.Add(-1);
						} 
						else if (Date[i]<D-Minute1*0.0001)
						{
							i++;
							continue;
						}
						else if (Date[i]<D+Minute1*0.9999)
						{
							al.Add(Date[i]);
							alPos.Add(i);
							i++;
							continue;
						}
						else
						{
							if (al.Count==0 || (double)al[al.Count-1]<D+Minute1/100)
							{
								al.Add(D+Minute1/100);
								if (intradayInfo.InTimePeriod(Date[i]))
									if (alPos.Count>0)
										alPos.Add(alPos[alPos.Count-1]);
									else alPos.Add(0);
								else 
									alPos.Add(i);
							}
						}
					}
					D +=Minute1;
					while (intradayInfo.InRemoveDays(D))
						D +=1;
				}

				Hashtable htRst = new Hashtable();
				double[] dc = (double[])ht["CLOSE"];
				foreach(string s in ht.Keys)
				{
					double[] d1 =(double[])ht[s];
					double[] d2 = new double[al.Count];
					for(int j=0; j<al.Count; j++)
					{
						if (s=="DATE")
							d2[j] = (double)al[j];
						else 
						{
							int k = (int)alPos[j];
							if (k<0)
								d2[j] = double.NaN;
							else 
							{
								if ((j>0 && (int)alPos[j]==(int)alPos[j-1]) ||
									(j==0 && (int)alPos[j]==0))
								{
									if (s=="VOLUME")
										d2[j] = 0;
									else d2[j] = dc[k];
								} 
								else 
									d2[j] = d1[k];
							}
						}
					}
					htRst[s] = d2;
				}
				return htRst;
			}
			return ht;
		}

		/// <summary>
		/// Adjust date time according to the BaseDataProvider
		/// </summary>
		/// <param name="Date">The date time array of the data, It must have the same length of the data</param>
		/// <param name="dd">The data array to be adjusted</param>
		/// <returns>adjusted data array</returns>
		private double[] AdjustByBase(double[] Date,double[] dd) 
		{
			double[] BaseDate = BaseDataProvider["DATE"];
			double[] nd = new double[BaseDate.Length];
			for(int i=0; i<BaseDate.Length; i++)
				nd[i] = double.NaN;
			int j=dd.Length-1;
			for(int i=BaseDate.Length-1; i>=0 && j>=0;)
			{
				if (BaseDate[i]==Date[j])
					nd[i--] = dd[j--];
				else if (BaseDate[i]>Date[j])
					i--;
				else 
					j--;
			}
			return nd;
		}

		/// <summary>
		/// Get stock data of current data cycle
		/// </summary>
		/// <param name="DataType">Stock data type , CLOSE,OPEN,HIGH,LOW,VOLUME,AMOUNT etc.</param>
		/// <returns>Certain data array</returns>
		private double[] GetData(string DataType) 
		{
			Hashtable ht = GetCycleData(DataCycle);
			if (ht==null)
				throw new Exception("Quote data "+DataType+ " "+DataCycle+" not found");
			else 
			{
				double[] dd = (double[])ht[DataType.ToUpper()];
				if (dd==null)
					throw new Exception("The name "+DataType + " does not exist.");
				else 
				{
					if (BaseDataProvider!=null && BaseDataProvider!=this)
						dd = AdjustByBase((double[])ht["DATE"],dd);
					if (MaxCount==-1 || dd.Length<=MaxCount)
						return dd;
					else 
					{
						double[] dsm = new double[MaxCount];
						Array.Copy(dd,dd.Length-MaxCount,dsm,0,MaxCount);
						return dsm;
					}
				}
			}
		}

		/// <summary>
		/// Get stock data by name, which name is CLOSE, OPEN,HIGH,LOW,VOLUME,AMOUNT etc.
		/// </summary>
		public double[] this[string Name]
		{
			get 
			{
				return GetData(Name);
			}
		}

		/// <summary>
		/// Get constant stock data
		/// </summary>
		/// <param name="DataType">The key of the constant data</param>
		/// <returns>Return the value of the DataType</returns>
		public double GetConstData(string DataType)
		{
			return (double)htConstData[DataType];
		}

		/// <summary>
		/// Get the string value of this provider , such as stock name, stock code etc.
		/// </summary>
		/// <param name="DataType">Data type , such as Name,Code</param>
		/// <returns>Return the value of the DataType</returns>
		public string GetStringData(string DataType)
		{
			return (string)htStringData[DataType.ToUpper()];
		}

		/// <summary>
		/// Set the string value of this provider,such as stock name, stock code etc.
		/// </summary>
		/// <param name="DataType">Data type, such as Name,Code etc</param>
		/// <param name="Value">The value of the DataType</param>
		public void SetStringData(string DataType,string Value) 
		{
			htStringData[DataType.ToUpper()] = Value;
		}

		/// <summary>
		/// Get DataPackage of certain bars
		/// </summary>
		/// <param name="Index">The bar index</param>
		/// <returns>The DataPackage at position of Index</returns>
		public DataPacket GetDataPackage(int Index) 
		{
			DataPacket dp = new DataPacket(
				GetStringData("Code"),
				this["DATE"][Index],
				(float)this["OPEN"][Index],
				(float)this["HIGH"][Index],
				(float)this["LOW"][Index],
				(float)this["CLOSE"][Index],
				this["VOLUME"][Index],
				(float)this["ADJCLOSE"][Index]
				);
			return dp;
		}

		/// <summary>
		/// Get the lastest DataPackage
		/// </summary>
		/// <returns></returns>
		public DataPacket GetLastPackage() 
		{
			return GetDataPackage(Count-1);
		}

		/// <summary>
		/// Get lastest DataPackages
		/// </summary>
		/// <param name="Count">DataPackage count</param>
		/// <returns>Array of DataPackage</returns>
		public DataPacket[] GetLastDataPackages(int Count) 
		{
			return GetLastDataPackages(this.Count-Count,Count);
		}

		/// <summary>
		/// Get DataPackages
		/// </summary>
		/// <param name="Start">Start bars of the DataPackage</param>
		/// <param name="Count">DataPackage count</param>
		/// <returns>Array of DataPackage</returns>
		public DataPacket[] GetLastDataPackages(int Start,int Count) 
		{
			DataPacket[] dps = new DataPacket[Count];
			double[] Date = this["DATE"];
			double[] Open = this["OPEN"];
			double[] High = this["HIGH"];
			double[] Low = this["LOW"];
			double[] Close = this["CLOSE"];
			double[] Volume = this["VOLUME"];
			double[] AdjClose = this["ADJCLOSE"];
			for(int i=Start; i<Start+Count; i++) 
				if (i>=0 && i<this.Count) 
				{
					dps[i-Start] = new DataPacket(
						GetStringData("Code"),
						Date[i],
						(float)Open[i],
						(float)High[i],
						(float)Low[i],
						(float)Close[i],
						Volume[i],
						(float)AdjClose[i]);
				}
			return dps;
		}

		/// <summary>
		/// Merge data from another DataProvider
		/// </summary>
		/// <param name="idp">DataProvider to be merged</param>
		public void Merge(CommonDataProvider cdp)
		{
			ArrayList[] als1 = new ArrayList[Keys.Length];
			ArrayList[] als2 = new ArrayList[Keys.Length];
			
			for(int i=0; i<als1.Length; i++)
			{
				als1[i] = new ArrayList();
				als1[i].AddRange((double[])htData[Keys[i]]);
				als2[i] = new ArrayList();
				als2[i].AddRange((double[])cdp.htData[Keys[i]]);
			}
			for(int i=0,j=0; j<als2[0].Count; )
			{
				if (i<als1[0].Count) 
				{
					if ((double)als1[0][i]<(double)als2[0][j]) 
						i++;
					else if ((double)als1[0][i]>=(double)als2[0][j])
					{
						if ((double)als1[0][i]>(double)als2[0][j])
							for(int k=0; k<Keys.Length; k++) 
								als1[k].Insert(i,als2[k][j]);
						else 
							for(int k=1; k<Keys.Length; k++) 
								als1[k][i] =als2[k][j];
						i++;
						j++;
					}
				}
				else
				{
					for(int a=j; a<als2[0].Count; a++)
						for(int k=0; k<Keys.Length; k++) 
							als1[k].Add(als2[k][a]);
					break;
				}
			}
			htData.Clear();
			for(int i=0; i<Keys.Length; i++) 
				htData.Add(Keys[i],(double[])als1[i].ToArray(typeof(double)));
			htAllCycle.Clear();
		}

		public void Merge(DataPacket dp)
		{
			if (dp==null || dp.IsZeroValue) return;
			//string[] Keys = {"DATE","OPEN","HIGH","LOW","CLOSE","VOLUME","ADJCLOSE"};
			ArrayList[] als = new ArrayList[Keys.Length];
			for(int i=0; i<als.Length; i++)
			{
				als[i] = new ArrayList();
				als[i].AddRange((double[])htData[Keys[i]]);
			}

			for(int i=0; i<=als[0].Count; i++)
			{
				if (i<als[0].Count)
				{
					if ((double)als[0][i]>=dp.DoubleDate)  //if ((int)(double)als[0][i]>=(int)dp.DoubleDate) 
					{
						if ((double)als[0][i]>dp.DoubleDate) 
							for(int k=0; k<Keys.Length; k++) 
								als[k].Insert(i,dp[Keys[k]]);
						else 
							for(int k=1; k<Keys.Length; k++) 
								als[k][i] =dp[Keys[k]];
						break;
					}
				}
				else 
				{
					for(int k=0; k<Keys.Length; k++) 
						als[k].Add(dp[Keys[k]]);
					break;
				}
			}

			htData.Clear();
			for(int i=0; i<Keys.Length; i++) 
				htData.Add(Keys[i],(double[])als[i].ToArray(typeof(double)));
			htAllCycle.Clear();
		}

		static public DataPacket MergeFile(string Filename,DataPacket dp,DataCycle dc)
		{
			using (FileStream fs = new FileStream(Filename,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite))
			{
				byte[] bs = new byte[DataPacket.PacketByteSize];
				if (fs.Length>=bs.Length)
					fs.Seek(-bs.Length,SeekOrigin.End);
				int i = fs.Read(bs,0,bs.Length);

				DataPacket dpExist = DataPacket.FromBytes(bs);
				dpExist.Symbol = dp.Symbol;
				if (dpExist.Merge(dp,dc))
					fs.Seek(-bs.Length,SeekOrigin.End);
				bs = dpExist.ToByte();
				fs.Write(bs,0,bs.Length);
				return dpExist;
			}
		}

		/// <summary>
		/// Delete data between "FromTime" and "ToTime"
		/// </summary>
		/// <param name="FromTime"></param>
		/// <param name="ToTime"></param>
		public void DeleteData(DateTime FromTime,DateTime ToTime)
		{
			//string[] Keys = {"DATE","OPEN","HIGH","LOW","CLOSE","VOLUME","ADJCLOSE"};
			ArrayList[] als = new ArrayList[Keys.Length];
			for(int i=0; i<als.Length; i++)
			{
				als[i] = new ArrayList();
				als[i].AddRange(this[Keys[i]]);
			}

			double d1 = FromTime.ToOADate();
			double d2 = ToTime.ToOADate();
			for(int i=0; i<als[0].Count;) 
			{
				double d = (double)als[0][i];
				if (d>d1 && d<d2) 
				{
					for(int k=0; k<Keys.Length; k++)
						als[k].RemoveAt(i);
				} 
				else 
					i++;
			}
			htData.Clear();
			for(int i=0; i<Keys.Length; i++) 
				htData.Add(Keys[i],(double[])als[i].ToArray(typeof(double)));
			htAllCycle.Clear();
		}

		public void ClearData()
		{
			htData.Clear();
			for(int i=0; i<Keys.Length; i++) 
				htData.Add(Keys[i],new double[]{});
			htAllCycle.Clear();
		}

		/// <summary>
		/// Get the data count
		/// </summary>
		public int Count
		{
			get 
			{
				return ((double[])this["DATE"]).Length;
			}
		}

		public bool HasData
		{
			get
			{
				return ((double[])htData["DATE"]).Length>0;
			}
		}

		/// <summary>
		/// Get the data manager
		/// </summary>
		public IDataManager DataManager
		{
			get 
			{
				if (dm==null) 
				{
					return dm;
				}
				else return dm;
			}
			set
			{
				dm = value;
			}
		}

		public string GetUnique() 
		{
			return DataCycle.ToString();
		}

		/// <summary>
		/// Determine if the close data is managed in adjust mode
		/// </summary>
		public bool Adjusted
		{
			get
			{
				return adjusted;
			}
			set 
			{
				adjusted = value;
			}
		}

		public int MaxCount 
		{
			get 
			{
				return maxCount;
			}
			set 
			{
				maxCount = value;
			}
		}
	}
}