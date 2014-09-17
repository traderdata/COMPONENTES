using System;
using System.Data;
using System.Reflection;
using System.IO;
using System.Globalization;
using Easychart.Finance.DataProvider;
using Easychart.Finance;

namespace WindowsDemo
{
	/// <summary>
	/// Summary description for TextDataManager.
	/// </summary>
	public class TextDataManager:DataManagerBase
	{
		public TextDataManager()
		{
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			CommonDataProvider cdp = new CommonDataProvider(this);
			cdp.SetStringData("Code",Code);
			string s =  FormulaHelper.Root+"Data\\"+Code+".txt";
			if (File.Exists(s))
			{
				using (StreamReader sr = File.OpenText(s))
				{
					string r = sr.ReadToEnd();
					string[] ss = r.Trim().Split('\n');

					int N = ss.Length;
					double[] CLOSE =  new double[N];
					double[] VOLUME=  new double[N];
					double[] DATE = new double[N];
					
					for(int i=0; i<ss.Length; i++)
					{
						int j = N-i-1;
						string[] sss = ss[i].Split(' ');
						DATE[j] = DateTime.ParseExact(sss[0],"yyyy.MM.dd",DateTimeFormatInfo.InvariantInfo).ToOADate();
						CLOSE[j] = double.Parse(sss[2]);
						VOLUME[j] = 0;
					}
					cdp.LoadBinary(new double[][]{CLOSE,CLOSE,CLOSE,CLOSE,VOLUME,DATE});
					return cdp;
				}
			}
			return base.GetData (Code, Count);
		}

	}
}
