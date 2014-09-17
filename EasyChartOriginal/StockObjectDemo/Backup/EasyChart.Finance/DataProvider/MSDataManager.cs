using System;
using System.IO;
using System.Text;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

namespace Easychart.Finance.DataProvider
{

	public enum MetaStockTimeFrame:byte {Daily=(byte)'D',Intraday=(byte)'I',Week=(byte)'W',Month=(byte)'M',Year=(byte)'Y',Quarter=(byte)'Q'};
	/// <summary>
	/// DataManager for meta stock
	/// </summary>
	public class MSDataManager : DataManagerBase
	{
		string FilePath;
		//string IndexFile;
		int  Fields = 7;
		MetaStockTimeFrame TimeFrame = MetaStockTimeFrame.Daily;

		Master[] Masters;
		XMaster[] XMasters;

		public MSDataManager(string FilePath)
		{
			FilePath = Path.GetDirectoryName(FilePath);
			if (!FilePath.EndsWith("\\")) FilePath +="\\";
			this.FilePath = FilePath;
			if (File.Exists(FilePath + "XMASTER"))
				LoadXMaster();
			if (File.Exists(FilePath + "MASTER"))
				LoadMaster();
			else throw new Exception("MetaStock Path Not Found!"+FilePath + "MASTER");
		}

		private string TrimToZero(string s)
		{
			for(int i=0; i<s.Length; i++)
				if (s[i]=='\0')
					return s.Substring(0,i);
			return s;
		}

		private FileStream ReadData(string Filename)
		{
			FileStream fs = null;
			for(int i=5; i>=0; i--)
				try
				{
					fs = new FileStream(Filename,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
					return fs;
				}
				catch
				{
					if (i==0)
						throw;
					Thread.Sleep(100);
				}
			return fs;
		}

		private void LoadXMaster()
		{
			using (FileStream fs =  ReadData(FilePath + "XMASTER"))
			{
				XMasters = new XMaster[fs.Length / 150-1];
				using (BinaryReader br = new BinaryReader(fs))
				{
					br.ReadBytes(150);
					int i = 0;
					do 
					{
						XMasters[i] = new XMaster();
						XMasters[i].Unknown1 = br.ReadByte();

						XMasters[i].StockSymbol = TrimToZero(Encoding.ASCII.GetString(br.ReadBytes(15)));
						XMasters[i].StockName= TrimToZero(Encoding.ASCII.GetString(br.ReadBytes(46)));
						XMasters[i].TimeFrame = (MetaStockTimeFrame)br.ReadByte();
						XMasters[i].Unknown2 = br.ReadBytes(2);
						XMasters[i].Fn = br.ReadInt16();
						XMasters[i].Unknown3 = br.ReadBytes(13);
						XMasters[i].EndDate = br.ReadInt32();
						XMasters[i].Unknown4 = br.ReadBytes(20);
						XMasters[i].StartDate = br.ReadInt32();
						XMasters[i].StartDate2 = br.ReadInt32();
						XMasters[i].Unknown5 = br.ReadBytes(4);
						XMasters[i].EndDate2 = br.ReadInt32();
						XMasters[i].Unknown6 = br.ReadBytes(30);
						i++;
					} while (i<XMasters.Length);
				}
			}
		}

		private void LoadMaster()
		{
			using (FileStream fs =  ReadData(FilePath + "MASTER"))
			{
				Masters = new Master[fs.Length / 53-1];
				using (BinaryReader br = new BinaryReader(fs))
				{
					br.ReadBytes(53);
					int i = 0;
					do 
					{
						Masters[i] = new Master();
						Masters[i].file_num = br.ReadByte();
						Masters[i].file_type = br.ReadBytes(2);
						Masters[i].rec_len = br.ReadByte();
						Masters[i].num_fields = br.ReadByte();
						Masters[i].reserved1 = br.ReadBytes(2);
						Masters[i].issue_name = Encoding.ASCII.GetString(br.ReadBytes(16)).Trim();
						Masters[i].reserved2 = br.ReadByte();
						Masters[i].CT_v2_8_flag = br.ReadByte();
						Masters[i].first_date = br.ReadSingle();
						Masters[i].last_date = br.ReadSingle();
						Masters[i].time_frame = (MetaStockTimeFrame)br.ReadByte();
						Masters[i].ida_time = br.ReadInt16();
						Masters[i].symbol = Encoding.ASCII.GetString(br.ReadBytes(14)).Trim();
						Masters[i].reserved3 = br.ReadByte();
						Masters[i].flag = br.ReadByte();
						Masters[i].reserved4 = br.ReadByte();
						i++;
					} while (i<Masters.Length);
				}
			}
		}

		private byte[] GetEmptyByteArray(int Count,byte Fill)
		{
			byte[] bs = new byte[Count];
			for(int i=0; i<bs.Length; i++)
				bs[i] = Fill;
			return bs;
		}

		private byte[] StringToBytes(string s,int Count, byte Fill)
		{
			byte[] bs =  GetEmptyByteArray(Count,Fill);
			Encoding.ASCII.GetBytes(s,0,s.Length,bs,0);
			return bs;
		}

		private void SaveMaster()
		{
			string Filename =  this.FilePath + "MASTER";
			using (FileStream fs =  File.Create(Filename))
			{
				using (BinaryWriter bw = new BinaryWriter(fs))
				{
					foreach(Master m in Masters)
					{
						bw.Write(m.file_num);
						bw.Write(m.file_type);
						bw.Write(m.rec_len);
						bw.Write((byte)Fields);
						bw.Write(m.reserved1);

						if (m.issue_name==null || m.issue_name=="")
							m.issue_name = m.symbol;
						bw.Write(StringToBytes(m.issue_name,16,32));

						bw.Write(m.reserved2);
						bw.Write(m.CT_v2_8_flag);
						bw.Write(m.first_date);
						bw.Write(m.last_date);
						bw.Write((byte)m.time_frame);
						bw.Write(m.ida_time);
						bw.Write(StringToBytes(m.symbol,14,32));
						bw.Write(m.reserved3);
						bw.Write(m.flag);
						bw.Write(m.reserved4);
					}
				}
			}
		}

		private void SaveXMaster() 
		{
			string Filename = FilePath + "XMaster";
			using (FileStream fs =  File.Create(Filename))
			{
				using (BinaryWriter bw = new BinaryWriter(fs))
				{
					if (XMasters!=null)
					foreach(XMaster m in XMasters)
					{
						bw.Write(m.Unknown1);
						bw.Write(StringToBytes(m.StockSymbol,15,0));								// 15 bytes
						bw.Write(StringToBytes(m.StockName,46,0));								// 46 bytes
						bw.Write((byte)m.TimeFrame);

						bw.Write(m.Unknown2);				// 2 bytes
						bw.Write(m.Fn);											// the number n in Fn.MWD
						bw.Write(m.Unknown3); 
						bw.Write(m.EndDate);
						bw.Write(m.Unknown4);
						bw.Write(m.StartDate);
						bw.Write(m.StartDate2);
						bw.Write(m.Unknown5);
						bw.Write(m.EndDate2);
						bw.Write(m.Unknown6);
					}
				}
			}
		}

		public string LookupDataFile(string Code,CommonDataProvider cdp)
		{
			return LookupDataFile(Code,cdp,ref Fields,ref TimeFrame);
		}

		public string LookupDataFile(string Code,CommonDataProvider cdp, ref int Fields, ref MetaStockTimeFrame TimeFrame)
		{
			if (cdp!=null)
				cdp.SetStringData("Code",Code);
			foreach(Master m in Masters)
			{
				if (string.Compare(m.symbol,Code,true)==0)
				{
					if (cdp!=null)
						cdp.SetStringData("Name",m.issue_name);
					Fields = m.num_fields;
					TimeFrame = m.time_frame;
					return FilePath+"F"+m.file_num+".DAT";
				}
			}
			if (XMasters!=null)
			foreach(XMaster m in XMasters)
			{
				if (string.Compare(m.StockSymbol,Code,true)==0)
				{
					if (cdp!=null)
						cdp.SetStringData("Name",m.StockName);
					TimeFrame = m.TimeFrame;
					return FilePath+"F"+m.Fn+".MWD";
				}
			}
			return "";
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			CommonDataProvider cdp = new CommonDataProvider(this);
			string s = LookupDataFile(Code,cdp,ref Fields,ref TimeFrame);

			if (s!="" && File.Exists(s))
			{
				using (FileStream fs = ReadData(s))
				{
					byte[] bb = new byte[Fields*4];
					byte[] bs = new byte[fs.Length-bb.Length];
					fs.Read(bb,0,bb.Length);
					fs.Read(bs,0,bs.Length);

					float[] ff = new float[bs.Length/4];
					Buffer.BlockCopy(bs,0,ff,0,bs.Length);
					fmsbin2ieee(ff);
					int N = ff.Length/Fields;
					double[] Date = new double[N];
					double[] Open = new double[N];
					double[] High = new double[N];
					double[] Low = new double[N];
					double[] Close = new double[N];
					double[] Volume = new double[N];
					double[] OpenInt = new double[N];
					if (Fields==5)
					{
						Open = Close;
						OpenInt = Close;
					}

					for(int i=0; i<N; i++)
					{
						int D =(int)ff[i*Fields];
						DateTime DD = new DateTime(D/10000+1900, (D/100) % 100, D % 100);
						int j = 0;
						if (Fields==8 || TimeFrame==MetaStockTimeFrame.Intraday) 
						{
							int T = (int)ff[i*Fields+1];
							DD +=new TimeSpan(T/10000,(T/100) % 100, T % 100);
							j = 1;
						}
						Date[i] =  DD.ToOADate();

						if (Fields>=6) 
						{
							Open[i] = ff[i*Fields+1+j];
							High[i] = ff[i*Fields+2+j];
							Low[i] = ff[i*Fields+3+j];
							Close[i] = ff[i*Fields+4+j];
							Volume[i] = ff[i*Fields+5+j];
							OpenInt[i] = Close[i];
						} 
						else
						{
							High[i] = ff[i*Fields+1];
							Low[i] = ff[i*Fields+2];
							Close[i] = ff[i*Fields+3];
							Volume[i] = ff[i*Fields+4];
						}
					}
					cdp.LoadBinary(new double[][]{Open,High,Low,Close,Volume,Date,OpenInt});
					return cdp;
				}
			}
			else cdp.LoadByteBinary(new byte[]{});
			return cdp;
		}

		private int MaxNum
		{
			get
			{
				int i = 1;
				if (Masters!=null)
				foreach(Master m in Masters) 
					if (i<m.file_num)
						i = m.file_num;
				if (XMasters!=null)
					foreach(XMaster m in XMasters)
						if (i<m.Fn)
							i = m.Fn;
				return i;
			}
		}

		public override void SaveData(string Symbol,IDataProvider idp,Stream OutStream,DateTime Start,DateTime End,bool Intraday)
		{
			CommonDataProvider cdp = (CommonDataProvider)idp;
			if (Symbol!=null && Symbol!="")
			{
				int Count = cdp.Count;
				XMaster xm; 
				Master m = FindBySymbol(Symbol,out xm);
				bool NeedSave = false;
				bool NeedSaveX = false;

				Fields = (byte)(7+(Intraday?1:0));
				if (m==null && xm==null) 
				{
					int NowNumber = MaxNum+1;
					if (NowNumber>255)
					{
						xm = new XMaster();
						xm.Fn = (short)NowNumber;
						ArrayList al = new ArrayList(XMasters);
						xm.StockSymbol = Symbol;
						xm.StockName = cdp.GetStringData("Name");
						if (xm.StockName==null)
							xm.StockName = Symbol;
						al.Add(xm);
						XMasters = (XMaster[])al.ToArray(typeof(XMaster));
						NeedSaveX = true;
					}
					else
					{
						m = new Master();
						m.file_num = (byte)NowNumber;
						ArrayList al = new ArrayList(Masters);
						m.symbol = Symbol;
						m.num_fields = (byte)Fields;
						m.issue_name = cdp.GetStringData("Name");
						if (m.issue_name==null)
							m.issue_name = Symbol;
						al.Add(m);
						Masters = (Master[])al.ToArray(typeof(Master));
						NeedSave = true;
					}
				}

				double[] Date =(double[])cdp["DATE"];
				double[] Open =(double[])cdp["OPEN"];
				double[] High = (double[])cdp["HIGH"];
				double[] Low = (double[])cdp["LOW"];
				double[] Close = (double[])cdp["CLOSE"];
				double[] Volume = (double[])cdp["VOLUME"];
				double[] OpenInt = (double[])cdp["ADJCLOSE"];
				float[] ff = new float[Count*Fields];
				
				for(int i=0; i<Count; i++)
				{
					int j = 0;
					DateTime D = DateTime.FromOADate(Date[i]);
					ff[i*Fields+j] = (D.Year-1900)*10000+D.Month*100+D.Day;
					if (Fields==8) 
					{
						j = 1;
						ff[i*Fields+j] = D.Hour*10000+D.Minute*100+D.Second;
					}
					ff[i*Fields+1+j] = (float)Open[i];
					ff[i*Fields+2+j] = (float)High[i];
					ff[i*Fields+3+j] = (float)Low[i];
					ff[i*Fields+4+j] = (float)Close[i];
					ff[i*Fields+5+j] = (float)Volume[i];
					ff[i*Fields+6+j] = (float)Close[i];
				}
				fieee2msbin(ff);
				byte[] bs = new byte[ff.Length*4];
				Buffer.BlockCopy(ff,0,bs,0,bs.Length);

				string s = LookupDataFile(Symbol,cdp);//FilePath+"F"+m.file_num+".DAT";
				using (FileStream fs  = File.Create(s))
					fs.Write(bs,0,bs.Length);
				if (NeedSave)
					SaveMaster();
				if (NeedSaveX)
					SaveXMaster();
			}
		}

		/* Microsoft Basic floating point format to IEEE floating point format */
		void fmsbin2ieee(float[] ff) 
		{
			uint[] ii = new uint[ff.Length];
			Buffer.BlockCopy(ff,0,ii,0,ff.Length*4);
			uint man;
			uint exp;
			for(int i=0; i<ff.Length; i++)
			{
				if (ii[i]!=0) 
				{		/* not zero */
					man =ii[i] >> 16;
					exp = (man & 0xff00) - 0x0200;
					//if ((exp & 0x8000) != (man & 0x8000))
					//{
					//	ii[i] = 0;
					//	continue;
						//throw new Exception("exponent overflow!");
					//}
					man = man & 0x7f | (man << 8) & 0x8000;	/* move sign */
					man |= exp >> 1;
					ii[i]= ii[i] & 0xffff | man << 16;
				}
			}
			Buffer.BlockCopy(ii,0,ff,0,ff.Length*4);
		}

		/* IEEE floating point format to Microsoft Basic floating point format */
		void fieee2msbin(float[] ff)
		{
			uint[] ii = new uint[ff.Length];
			Buffer.BlockCopy(ff,0,ii,0,ff.Length*4);

			uint man;
			uint exp;

			for(int i=0; i<ff.Length; i++)
			{
				if (ii[i]!=0) 
				{		/* not zero */
					man = ii[i] >> 16;
					exp = ((man << 1) & 0xff00) + 0x0200;
					if ((exp & 0x8000)!=((man << 1) & 0x8000))
						continue;	/* exponent overflow */
					man = man & 0x7f | (man >> 8) & 0x80;	/* move sign */
					man |= exp;
					ii[i] = ii[i] & 0xffff | man << 16;
				}
			}
			Buffer.BlockCopy(ii,0,ff,0,ff.Length*4);
		}

		public DataTable GetTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Symbol");
			dt.Columns.Add("Name");
			
			if (Masters!=null)
				foreach(Master m in Masters) 
					dt.Rows.Add(new object[]{m.symbol,m.issue_name});

			if (XMasters!=null)
				foreach(XMaster m in XMasters) 
					dt.Rows.Add(new object[]{m.StockSymbol,m.StockName});
			return dt;
		}

		public override DataTable GetSymbolList(string Exchange, string ConditionId, string Key, string Sort, int StartRecords, int MaxRecords)
		{
			return RecordRange(GetTable(),StartRecords,MaxRecords);
		}

		public override DataTable GetStockList(string Exchange, string ConditionId, string Key, string Sort, int StartRecords, int MaxRecords)
		{
			return RecordRange(GetTable(),StartRecords,MaxRecords);
		}

		public override int SymbolCount(string Exchange, string ConditionId, string Key)
		{
			return GetTable().Rows.Count;
		}

		public override DataTable Exchanges
		{
			get
			{
				DataTable dt = new DataTable();
				dt.Columns.Add("Value");
				dt.Columns.Add("Text");
				dt.Rows.Add(new object[]{"","ALL"});
				return dt;
			}
		}

		public string GetSymbol(string Number)
		{
			Master m = FindByNumber(Number);
			if (m==null)
				return "";
			else return m.symbol.Trim();
		}

		public Master FindByNumber(string Number)
		{
			foreach(Master m in Masters) 
				if ("F"+m.file_num == Number)
					return m;
			return null;
		}

		public Master FindBySymbol(string Symbol,out XMaster xm)
		{
			xm = null;
			if (Masters!=null)
				foreach(Master m in Masters) 
					if (m.symbol==Symbol)
						return m;
			
			if (XMasters!=null)
				foreach(XMaster m in XMasters)
					if (m.StockSymbol==Symbol) 
					{
						xm = m;
						return null;
					}
			return null;
		}

		public override int DeleteSymbols(string Exchange, string[] Symbols, bool Remain, bool DeleteRealtime, bool DeleteHistorical)
		{
			ArrayList al = new ArrayList();
			int Count = 0;
			if (Masters!=null)
			{
				al.AddRange(Masters);
				for(int i=0; i<al.Count; )
				{
					if (Array.IndexOf(Symbols, (al[i] as Master).symbol)>=0) 
					{
						al.RemoveAt(i);
						Count++;
					}
					else i++;
				}
				if (Count>0)
				{
					Masters = (Master[])al.ToArray(typeof(Master));
					SaveMaster();
				}
			}

			if (XMasters!=null)
			{
				al.Clear();
				Count = 0;
				al.AddRange(XMasters);
				for(int i=0; i<al.Count; )
				{
					if (Array.IndexOf(Symbols, (al[i] as XMaster).StockSymbol)>=0) 
					{
						al.RemoveAt(i);
						Count++;
					}
					else i++;
				}
				if (Count>0)
				{
					XMasters = (XMaster[])al.ToArray(typeof(XMaster));
					SaveXMaster();
				}
			}
			return Count;
		}
	}

	//record length 52 bytes
	public class Master
	{
		public byte file_num;										/* file #, i.e., F# */
		public byte[] file_type={101,0};							/* 2 bytes CT file type = 0'e' (5 or 7 flds) */
		public byte rec_len;										/* record length in bytes (4 x num_fields)*/
		public byte num_fields;									/* number of 4-byte fields in each record*/
		public byte[] reserved1= new byte[2];				/* 2 bytes in the data file */
		public string issue_name;								/* 16 bytes stock name */
		public byte reserved2;
		public byte CT_v2_8_flag;								/* if CT ver. 2.8, 'Y'; o.w., anything else */
		public float first_date;										/* yymmdd */
		public float last_date;
		public MetaStockTimeFrame time_frame;			/* data format: 'I'(IDA)/'W'/'Q'/'D'/'M'/'Y' */
		public short ida_time;									/* <b>intraday</b> (IDA) time base */
		public string symbol;										/* 14 bytes stock symbol */
		public byte reserved3;										/* <b>MetaStock</b> reserved2: must be a space */
		public byte flag;												/* ' ' or '*' for autorun */
		public byte reserved4;
	}

//	XMASTER: record length 150 bytes

//	A { @B {Start Byte} }
//	B { @B {End Byte} }
//	C { @B {Length} }
//	D { @B {Description} }
//	E { @B {Type} }

//	@Rowa A {0}	B {0}	C {1} 	D {Unknown}
//	@Rowa A {1}	B {15}	C {15} 	D {Stock symbol: ends with a byte 0}		E{String}
//	@Rowa A {16}	B {61}	C {46} 	D {Stock name: ends with a byte 0}		E{String}
//	@Rowa A {62}	B {62}	C {1} 	D {'D' maybe update type}			E{Char}
//	@Rowa A {65}	B {66}	C {2} 	D {the number n in Fn.MWD}			E{Short}
//	@Rowa A {67}	B {79}	C {13} 	D {Unknown}
//	@Rowa A {80}	B {83}	C {4} 	D {End Date e.g. 19981125}			E{Integer}
//	@Rowa A {84}	B {103}	C {20} 	D {Unknown}
//	@Rowa A {104}	B {107}	C {4} 	D {Start Date}					E{Integer}
//	@Rowa A {108}	B {111}	C {4} 	D {Start Date}					E{Integer}
//	@Rowa A {112}	B {115}	C {4} 	D {Unknown}
//	@Rowa A {116}	B {119}	C {4} 	D {End Date}					E{Integer}
//	@Rowa A {120}	B {149}	C {30} 	D {Unknown}
	public class XMaster
	{
		public byte Unknown1;
		public string StockSymbol;								// 15 bytes
		public string StockName;								    // 46 bytes
		public MetaStockTimeFrame TimeFrame;			/* data format: 'I'(IDA)/'W'/'Q'/'D'/'M'/'Y' */
		public byte[] Unknown2=new byte[2];			// 2 bytes
		public short Fn;											    // the number n in Fn.MWD
		public byte[] Unknown3 = new byte[13]; 
		public int EndDate;
		public byte[] Unknown4 = new byte[20];
		public int StartDate;
		public int StartDate2;
		public byte[] Unknown5 = new byte[4];
		public int EndDate2;
		public byte[] Unknown6 = new byte[30];
	}
}