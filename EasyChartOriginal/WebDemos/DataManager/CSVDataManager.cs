using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Threading;
using Easychart.Finance.DataProvider;

namespace WebDemos
{
	/// <summary>
	/// Summary description for CSVDataManager.
	/// </summary>
	public class CSVDataManager:DataManagerBase
	{
		public CSVDataManager()
		{
		}

		private double ToDoubleDef(string s,double Def, NumberFormatInfo nfi)
		{
			try
			{
				return double.Parse(s,nfi);
			}
			catch
			{
				return Def;
			}
		}

		public CommonDataProvider GetDataFromString(string s)
		{
			string[] ss = s.Trim().Split('\n');
			ArrayList al = new ArrayList();
			for(int i=0; i<ss.Length; i++) 
			{
				ss[i] = ss[i].Trim();
				if (!ss[i].StartsWith("<!--") && ss[i]!="")
					al.Add(ss[i]);
			}

			int N = al.Count;
			double[] CLOSE = new double[N];
			double[] OPEN = new double[N];
			double[] HIGH = new double[N];
			double[] LOW = new double[N];
			double[] VOLUME = new double[N];
			double[] DATE = new double[N];

			string Code = null;
			string Name = null;
			string Exchange = null;

			DateTimeFormatInfo dtfi = DateTimeFormatInfo.InvariantInfo;
			NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
			for (int i=0; i<N; i++) 
			{
				string[] sss = ((string)al[i]).Split(',');
				int j = i;//N-i-1;
				DATE[j] = DateTime.ParseExact(sss[1],"yyyyMMdd",dtfi).ToOADate();
				OPEN[j] = ToDoubleDef(sss[2],0,nfi);
				HIGH[j] = ToDoubleDef(sss[3],0,nfi);
				LOW[j] = ToDoubleDef(sss[4],0,nfi);
				CLOSE[j] = ToDoubleDef(sss[5],0,nfi);
				VOLUME[j] = ToDoubleDef(sss[6],0,nfi);
				Code = sss[0];
				if (string.Compare(Code,"PRN",true)==0)
					Code = "PRNN";
				if (sss.Length>7)
					Name = sss[7];
				if (sss.Length>8)
					Exchange = sss[8];
			}

			CommonDataProvider cdp = new CommonDataProvider(this);
			if (Code!=null)
				cdp.SetStringData("Code",Code);
			if (Name!=null)
				cdp.SetStringData("Name",Name);
			if (Exchange!=null)
				cdp.SetStringData("Exchange",Exchange);
			cdp.LoadBinary(new double[][]{OPEN,HIGH,LOW,CLOSE,VOLUME,DATE});
			return cdp;
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			string FileName = Config.CSVDataPath + Code+Config.CSVExt;
			if (File.Exists(FileName))
			{
				FileStream fs = new FileStream(FileName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
				StreamReader sr = new StreamReader(fs);
				try
				{
					string s = sr.ReadToEnd();
					CommonDataProvider cdp = GetDataFromString(s);
					if (cdp!=null)
						cdp.SetStringData("Code",Code);
					return cdp;
				}
				finally
				{
					sr.Close();
					fs.Close();
				}
			}
			return base.GetData (Code, Count);
		}
	}
}