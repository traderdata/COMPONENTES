using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace WindowsDemo
{
	/// <summary>
	/// Summary description for BondTextDataManager.
	/// </summary>
	public class BondTextDataManager: IntraDataManagerBase
	{
		public BondTextDataManager()
		{
		}

		public override IDataProvider GetData(string Code,int Count)
		{
			CommonDataProvider cdp = new CommonDataProvider(this);
		
			string FileName = FormulaHelper.Root+"Data\\"+Code+".csv";

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
					{
						string r = ss[i].Trim();
						if (r!="")
							al.Add(r);
					}

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
						int j = N-1-i;
						DATE[j] = DateTime.ParseExact(sss[0],"yyyy-MM-dd H:mm",dtfi).ToOADate();
						OPEN[j] = double.Parse(sss[2]);
						HIGH[j] = double.Parse(sss[3]);
						LOW[j] = double.Parse(sss[4]);
						CLOSE[j] = double.Parse(sss[5]);
						VOLUME[j] = double.Parse(sss[6]);;
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
