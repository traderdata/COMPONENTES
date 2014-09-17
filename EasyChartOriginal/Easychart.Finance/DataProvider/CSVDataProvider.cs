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
	public class CSVDataProvider:IDataProvider
	{
		Hashtable htData = new Hashtable();
		Hashtable htRealtime = new Hashtable();
		Hashtable htConstData = new Hashtable();
		Hashtable htStringData = new Hashtable();
		Hashtable htAllCycle = new Hashtable();
		private DataCycle dataCycle = DataCycle.Day();
		private IDataProvider baseDataProvider;
		private int maxCount = -1;
		private bool adjusted = true;

		IDataManager dm;

		/// <summary>
		/// Get or Set the current data cycle
		/// </summary>
		public DataCycle DataCycle
		{
			get 
			{
				return dataCycle;
			}
			set 
			{
				dataCycle = value;
			}
		}

		/// <summary>
		/// Adjust the date of current DataProvider according to BaseDataProvider
		/// </summary>
		public IDataProvider BaseDataProvider
		{
			get 
			{
				return baseDataProvider;
			}
			set 
			{
				baseDataProvider = value;
			}
		}

		static Random Rnd = new Random();
		/// <summary>
		/// Not implement yet
		/// </summary>
		public void AddRealtimeData() 
		{
			Random Rnd = new Random();
			int M = 1;
			int N = 240*M;
			double[] CLOSE = new double[N];
			double[] VOLUME = new double[N];
			double[] DATE = new double[N];
			CLOSE[0] = 20;
			for(int j=0,k=0; j<M; j++) 
			{
				for(int i=0; i<N/M; i++,k++) 
				{
					DATE[k] = (new DateTime(2003,8,8+j,9,30,0)).ToOADate();
					DATE[k] +=(double)i/60/24;
					if (i>=120)
						DATE[k] +=1.5f/24;
					if (k>0)
						CLOSE[k] = CLOSE[k-1] + (double)(Rnd.Next(19)-9)/10;
					VOLUME[k] = Rnd.Next(10000);
				}
			}

			htRealtime.Add("CLOSE",CLOSE);
			htRealtime.Add("VOLUME",VOLUME);
			htRealtime.Add("DATE",DATE);
		}

		/// <summary>
		/// Create the data provider
		/// </summary>
		/// <param name="dm"></param>
		public CSVDataProvider(IDataManager dm)
		{
			this.dm = dm;
		}

		/// <summary>
		/// Create the data provider by stream
		/// </summary>
		/// <param name="dm">The Data Manager</param>
		/// <param name="stream">Stream used to create the data provider</param>
		public CSVDataProvider(IDataManager dm,Stream stream):this(dm)
		{
			//AddRealtimeData();
			StreamReader sr = new StreamReader(stream);
			string s = sr.ReadToEnd().Trim();
			string[] ss = s.Split('\n');
			ArrayList al = new ArrayList();
			for(int i=1; i<ss.Length; i++) 
			{
				ss[i] = ss[i].Trim();
				if (!ss[i].StartsWith("<!--"))
					al.Add(ss[i]);
			}

			int N = al.Count;
			double[] CLOSE = new double[N];
			double[] OPEN = new double[N];
			double[] HIGH = new double[N];
			double[] LOW = new double[N];
			double[] VOLUME = new double[N];
			double[] DATE = new double[N];
			double[] ADJCLOSE = new double[N];

			DateTimeFormatInfo dtfi = DateTimeFormatInfo.InvariantInfo;
			NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
			for (int i=0; i<N; i++) 
			{
				string[] sss = ((string)al[i]).Split(',');
				if (sss.Length<7)
				{
					string[] rrr = new string[7];
					for(int k=0; k<sss.Length; k++)
						rrr[k] = sss[k];
					if (sss.Length==6)
						rrr[6] = sss[4];
					//Format: 3-Mar-1904,13
					if (sss.Length==2) 
						for(int k=2; k<rrr.Length; k++)
							rrr[k] = sss[1];

					sss = rrr;
				}
				int j = N-i-1;
				DATE[j] = DateTime.ParseExact(sss[0],"%d-MMM-yy",dtfi).ToOADate();
				OPEN[j] = double.Parse(sss[1],nfi);
				HIGH[j] = double.Parse(sss[2],nfi);
				LOW[j] = double.Parse(sss[3],nfi);
				CLOSE[j] = double.Parse(sss[4],nfi);
				VOLUME[j] = double.Parse(sss[5],nfi);
				ADJCLOSE[j] = double.Parse(sss[6],nfi);
			}

			htData.Add("CLOSE",CLOSE);
			htData.Add("OPEN",OPEN);
			htData.Add("HIGH",HIGH);
			htData.Add("LOW",LOW);
			htData.Add("VOLUME",VOLUME);
			htData.Add("DATE",DATE);
			htData.Add("ADJCLOSE",ADJCLOSE);
		}

		/// <summary>
		/// Create data provider by file
		/// </summary>
		/// <param name="dm">The Data Manager</param>
		/// <param name="FileName">File used to create the data provider</param>
		public CSVDataProvider(IDataManager dm,string FileName):this(dm,File.OpenRead(FileName))
		{
		}

		/// <summary>
		/// Create data provider by byte array
		/// </summary>
		/// <param name="dm">The Data Manager</param>
		/// <param name="data">Byte Array used to create the data provider</param>
		public CSVDataProvider(IDataManager dm,byte[] data):this(dm,new MemoryStream(data) )
		{
		}

		/// <summary>
		/// Load binary stock data from stream
		/// </summary>
		/// <param name="stream"></param>
		public void LoadBinary(Stream stream)
		{
			byte[] bs = new Byte[stream.Length];
			stream.Read(bs,0,(int)stream.Length);
			LoadBinary(bs);
		}

		/// <summary>
		/// Load binary stock data from File
		/// </summary>
		/// <param name="FileName"></param>
		public void LoadBinary(string FileName) 
		{
			FileStream fs = File.OpenRead(FileName);
			try 
			{
				LoadBinary(fs);
			} 
			finally 
			{
				fs.Close();
			}
		}

		/// <summary>
		/// Load binary stock data from byte array
		/// </summary>
		/// <param name="bs"></param>
		public void LoadBinary(byte[] bs)
		{
			LoadBinary(bs,bs.Length/DataPackage.PackageByteSize);
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
			
			float[] fs = new float[N*DataPackage.PackageSize];
			Buffer.BlockCopy(bs,0,fs,0,bs.Length);

			for(int i=0; i<N; i++) 
			{
				Buffer.BlockCopy(fs,i*DataPackage.PackageByteSize,DATE,i*8,8);
				OPEN[i] = fs[i*DataPackage.PackageSize+2];
				HIGH[i] = fs[i*DataPackage.PackageSize+3];
				LOW[i] = fs[i*DataPackage.PackageSize+4];
				CLOSE[i] = fs[i*DataPackage.PackageSize+5];
				Buffer.BlockCopy(fs,(i*DataPackage.PackageSize+6)*4 ,VOLUME,i*8,8);
				ADJCLOSE[i] = fs[i*DataPackage.PackageSize+8];
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
			htData[DataType] =ds;
			htAllCycle.Clear();
		}

		/// <summary>
		/// Marge DataPackage to the byte data array
		/// </summary>
		/// <param name="bs">Byte data array</param>
		/// <param name="dp">DataPackage to be merged</param>
		/// <returns></returns>
		public static byte[] MergeOneQuote(byte[] bs,DataPackage dp) 
		{
			float[] fs = new float[bs.Length / 4+DataPackage.PackageSize];
			System.Buffer.BlockCopy(bs,0,fs,0,bs.Length);
			DateTime d2 = dp.Date.Date;
			int Count = fs.Length/DataPackage.PackageSize-1;
			for(int i=Count-1; i>=-1; i--)
			{
				DateTime d1 = DateTime.MinValue;
				if (i>-1)
					d1 = DataPackage.GetDateTime(fs,i).Date;

				int Extra = 0;
				if (d1<=d2) 
				{
					if (d1<d2) 
					{
						if (i<Count-1 && Count>0)
							System.Buffer.BlockCopy(
								fs,(i+1)*DataPackage.PackageByteSize,fs,
								(i+2)*DataPackage.PackageByteSize,
								(Count-i-1)*DataPackage.PackageByteSize);
						Extra = 1;
					}
					float[] fsCurrent = dp.GetFloat();
					System.Buffer.BlockCopy(fsCurrent,0,fs,(i+Extra)*DataPackage.PackageByteSize,DataPackage.PackageByteSize);
					bs = new byte[fs.Length*4-(1-Extra)*DataPackage.PackageByteSize];
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
			BinaryWriter bw = new BinaryWriter(stream);
			try
			{
				double[] CLOSE = (double[])htData["CLOSE"];
				double[] OPEN = (double[])htData["OPEN"];
				double[] HIGH = (double[])htData["HIGH"];
				double[] LOW = (double[])htData["LOW"];
				double[] VOLUME = (double[])htData["VOLUME"];
				double[] DATE = (double[])htData["DATE"];
				double[] ADJCLOSE = (double[])htData["ADJCLOSE"];

				int N=CLOSE.Length;
				//bw.Write(N);
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
			finally 
			{
				bw.Close();
			}
		}

		/// <summary>
		/// Save binary stock data to file
		/// </summary>
		/// <param name="FileName"></param>
		public void SaveBinary(string FileName) 
		{
			SaveBinary(File.OpenWrite(FileName));
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

		private void MergeCycle(double[] ODATE,int[] NEWDATE,double[] CLOSE,double[] ADJCLOSE,double[] ht,double[]htCycle, MergeCycleType mct,bool DoAdjust)
		{
			int Last = -1;
			int j = -1;
			for(int i=0; i<ODATE.Length; i++) 
			{
				double Factor = 1;
				if (DoAdjust)
					Factor = ADJCLOSE[i]/CLOSE[i];
				if (Last!=NEWDATE[i]) 
				{
					j++;
					htCycle[j] = ht[i]*Factor;
				} 
				else 
				{
					if (mct==MergeCycleType.HIGH)
						htCycle[j] = Math.Max(htCycle[j],ht[i]*Factor);
					else if (mct==MergeCycleType.LOW)
						htCycle[j] = Math.Min(htCycle[j],ht[i]*Factor);
					else if (mct==MergeCycleType.CLOSE)
						htCycle[j] = ht[i]*Factor;
					else if (mct==MergeCycleType.ADJCLOSE)
						htCycle[j] = ht[i]/Factor;
					else if (mct!=MergeCycleType.OPEN) 
						htCycle[j] += ht[i]*Factor;
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
				Hashtable htCycle = (Hashtable)htAllCycle[dc.ToString()];
				if (htCycle==null)
				{
					Hashtable ht;
					if (DataCycle.CycleBase<DataCycleBase.DAY)
						ht = htRealtime;
					else ht = htData;
					if (ht==null) return ht;

					double[] ODATE = (double[])ht["DATE"];
					if (ODATE==null)
						return null;

					int[] NEWDATE = new int[ODATE.Length];

					int Last = int.MinValue;
					int j=-1;
					int Num;
					for(int i=0; i<ODATE.Length; i++) 
					{
						Num = DataCycle.GetSequence(ODATE[i]);
						if (Num>Last) j++;
						NEWDATE[i] = j;
						Last = Num;
					}

					if (ht["CLOSE"]!=null)
					{
						if (ht["OPEN"]==null) ht["OPEN"] = ht["CLOSE"];
						if (ht["HIGH"]==null) ht["HIGH"] = ht["CLOSE"];
						if (ht["LOW"]==null) ht["LOW"] = ht["CLOSE"];
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
						try
						{
							mct = (MergeCycleType)Enum.Parse(typeof(MergeCycleType),s);
						} 
						catch
						{
							DoAdjust = false;
							if (s=="VOLUME" || s=="AMOUNT") mct = MergeCycleType.SUM;
							else 
								mct = MergeCycleType.CLOSE;
						}
						MergeCycle(ODATE,NEWDATE,CLOSE,ADJCLOSE,(double[])ht[s],(double[])htCycle[s], mct,DoAdjust);
					}
					htAllCycle[dc.ToString()] = htCycle;
				}
				return htCycle;
			}
		}

//		public Hashtable GetCycleData(DataCycle dc)
//		{
//			if (dc.CycleBase==DataCycleBase.DAY && dc.Repeat==1 && !Adjusted)
//				return htData;
//			else 
//			{
//				Hashtable htCycle = (Hashtable)htAllCycle[dc.ToString()];
//				if (htCycle==null)
//				{
//					Hashtable ht;
//					if (DataCycle.CycleBase<DataCycleBase.DAY)
//						ht = htRealtime;
//					else ht = htData;
//					if (ht==null) return ht;
//		
//					double[] OCLOSE = (double[])ht["CLOSE"];
//					double[] OOPEN = (double[])ht["OPEN"];
//					if (OOPEN==null) OOPEN = OCLOSE;
//					double[] OHIGH = (double[])ht["HIGH"];
//					if (OHIGH==null) OHIGH = OCLOSE;
//					double[] OLOW = (double[])ht["LOW"];
//					if (OLOW==null) OLOW = OCLOSE;
//					double[] OVOLUME = (double[])ht["VOLUME"];
//					double[] ODATE = (double[])ht["DATE"];
//					double[] OADJCLOSE = (double[])ht["ADJCLOSE"];
//		
//					if (ODATE==null)
//						return null;
//		
//					int[] NewDate = new int[ODATE.Length];
//		
//					int Last = int.MinValue;
//					int j=-1;
//					int Num;
//					for(int i=0; i<ODATE.Length; i++) 
//					{
//						Num = DataCycle.GetSequence(ODATE[i]);
//						if (Num>Last) j++;
//						NewDate[i] = j;
//						Last = Num;
//					}
//		
//					double[] CLOSE = new double[j+1];
//					double[] OPEN = new double[j+1];
//					double[] HIGH = new double[j+1];
//					double[] LOW = new double[j+1];
//					double[] VOLUME = new double[j+1];
//					double[] DATE = new double[j+1];
//					double[] ADJCLOSE = new double[j+1];
//					Last = -1;
//					j = -1;
//							
//					for(int i=0; i<ODATE.Length; i++) 
//					{
//						double Factor = 1;
//						if (Adjusted && OADJCLOSE!=null && OCLOSE!=null)
//							Factor = OADJCLOSE[i]/OCLOSE[i];
//						if (Last!=NewDate[i]) 
//						{
//							j++;
//							if (OCLOSE!=null) CLOSE[j] = OCLOSE[i]*Factor;
//							if (OOPEN!=null) OPEN[j] = OOPEN[i]*Factor;
//							if (OHIGH!=null) HIGH[j] = OHIGH[i]*Factor;
//							if (OLOW!=null) LOW[j] = OLOW[i]*Factor;
//							if (OVOLUME!=null) VOLUME[j] = OVOLUME[i];
//							if (ODATE!=null) DATE[j] = ODATE[i];
//							if (OADJCLOSE!=null) ADJCLOSE[j] = OADJCLOSE[i]/Factor;
//						} 
//						else 
//						{
//							if (OCLOSE!=null) CLOSE[j] = OCLOSE[i]*Factor;
//							if (OADJCLOSE!=null) ADJCLOSE[j] = OADJCLOSE[i]/Factor;
//							if (OHIGH!=null) HIGH[j] = Math.Max(HIGH[j],OHIGH[i]*Factor);
//							if (OLOW!=null) LOW[j] = Math.Min(LOW[j],OLOW[i]*Factor);
//							if (OVOLUME!=null) VOLUME[j] +=OVOLUME[i];
//							if (ODATE!=null) DATE[j] = ODATE[i];
//						}
//						Last = NewDate[i];
//					}
//		
//					htCycle = new Hashtable();
//					htCycle.Add("CLOSE",CLOSE);
//					htCycle.Add("OPEN",OPEN);
//					htCycle.Add("HIGH",HIGH);
//					htCycle.Add("LOW",LOW);
//					htCycle.Add("VOLUME",VOLUME);
//					htCycle.Add("DATE",DATE);
//					htCycle.Add("ADJCLOSE",ADJCLOSE);
//					htAllCycle[dc.ToString()] = htCycle;
//				}
//				return htCycle;
//			}
//		}

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
				double[] dd = (double[])ht[DataType];
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
			return (string)htConstData[DataType];
		}

		/// <summary>
		/// Set the string value of this provider,such as stock name, stock code etc.
		/// </summary>
		/// <param name="DataType">Data type, such as Name,Code etc</param>
		/// <param name="Value">The value of the DataType</param>
		public void SetStringData(string DataType,string Value) 
		{
			htConstData[DataType] = Value;
		}

		/// <summary>
		/// Get DataPackage of certain bars
		/// </summary>
		/// <param name="Index">The bar index</param>
		/// <returns>The DataPackage at position of Index</returns>
		public DataPackage GetDataPackage(int Index) 
		{
			DataPackage dp = new DataPackage(
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
		public DataPackage GetLastPackage() 
		{
			return GetDataPackage(Count-1);
		}

		/// <summary>
		/// Get lastest DataPackages
		/// </summary>
		/// <param name="Count">DataPackage count</param>
		/// <returns>Array of DataPackage</returns>
		public DataPackage[] GetLastDataPackages(int Count) 
		{
			return GetLastDataPackages(this.Count-Count,Count);
		}

		/// <summary>
		/// Get DataPackages
		/// </summary>
		/// <param name="Start">Start bars of the DataPackage</param>
		/// <param name="Count">DataPackage count</param>
		/// <returns>Array of DataPackage</returns>
		public DataPackage[] GetLastDataPackages(int Start,int Count) 
		{
			DataPackage[] dps = new DataPackage[Count];
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
					dps[i-Start] = new DataPackage(
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
		public void Merge(IDataProvider idp)
		{
			string[] Keys = {"DATE","OPEN","HIGH","LOW","CLOSE","VOLUME","ADJCLOSE"};
			ArrayList[] als1 = new ArrayList[Keys.Length];
			ArrayList[] als2 = new ArrayList[Keys.Length];
			
			for(int i=0; i<als1.Length; i++)
			{
				als1[i] = new ArrayList();
				als1[i].AddRange(this[Keys[i]]);
				als2[i] = new ArrayList();
				als2[i].AddRange(idp[Keys[i]]);
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

		public void Merge(DataPackage dp)
		{
			if (dp==null) return;
			string[] Keys = {"DATE","OPEN","HIGH","LOW","CLOSE","VOLUME","ADJCLOSE"};
			ArrayList[] als = new ArrayList[Keys.Length];
			for(int i=0; i<als.Length; i++)
			{
				als[i] = new ArrayList();
				als[i].AddRange(this[Keys[i]]);
			}

			for(int i=0; i<=als[0].Count; i++)
			{
				if (i<als[0].Count)
				{
					if ((int)(double)als[0][i]>=(int)dp.DoubleDate) 
					{
						if ((int)(double)als[0][i]>(int)dp.DoubleDate) 
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

		/// <summary>
		/// Delete data between "FromTime" and "ToTime"
		/// </summary>
		/// <param name="FromTime"></param>
		/// <param name="ToTime"></param>
		public void DeleteData(DateTime FromTime,DateTime ToTime)
		{
			string[] Keys = {"DATE","OPEN","HIGH","LOW","CLOSE","VOLUME","ADJCLOSE"};
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