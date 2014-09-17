using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Globalization;
using System.Threading;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace WindowsDemo
{
	/// <summary>
	/// Summary description for NasserDataManager.
	/// </summary>
	public class ForexTextDataManager: IntraDataManagerBase
	{
		public ForexTextDataManager()
		{
		}

		public override IDataProvider GetData(string Code,int Count)
		{
			CommonDataProvider cdp = new CommonDataProvider(this);
			string FileName = FormulaHelper.Root+"Data\\"+Code+".txt";
			if (File.Exists(FileName))
				try
				{
					string s = "";
					using (FileStream fs = new FileStream(FileName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
					{
						using (StreamReader sr = new StreamReader(fs))
							s = sr.ReadToEnd();
					}

					string[] ss = s.Trim().Split('\n');
					ArrayList al = new ArrayList();
					for(int i=0; i<ss.Length; i++) 
						al.Add(ss[i].Trim());

					int N = al.Count;
					double[] CLOSE = new double[N];
					double[] OPEN = new double[N];
					double[] HIGH = new double[N];
					double[] LOW = new double[N];
					double[] VOLUME = new double[N];
					double[] DATE = new double[N];

					DateTimeFormatInfo dtfi = DateTimeFormatInfo.InvariantInfo;
					NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
					for (int i=0; i<N; i++) 
					{
						string[] sss = ((string)al[i]).Split(',');
						int j = i;
						DATE[j] = DateTime.ParseExact(sss[1]+" "+sss[2],"MM\\/dd\\/yy HH:mm",dtfi).ToOADate();
						OPEN[j] = double.Parse(sss[3]);
						HIGH[j] = double.Parse(sss[4]);
						LOW[j] = double.Parse(sss[5]);
						CLOSE[j] = double.Parse(sss[6]);
						VOLUME[j] = 0;
					}

					cdp.SetStringData("Code",Code);
					cdp.LoadBinary(new double[][]{OPEN,HIGH,LOW,CLOSE,VOLUME,DATE});
					return cdp;
				}
				catch
				{
				}
			return CommonDataProvider.Empty;
		}
	}
}