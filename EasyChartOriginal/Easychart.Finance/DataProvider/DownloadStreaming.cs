using System;
using System.Data;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Threading;

namespace Easychart.Finance.DataProvider
{
	public delegate void OnNewPacket(DataPacket dp);
	/// <summary>
	/// Run will download real-time data from yahoo finance, and add the data to stream data manager.
	/// </summary>
	public class DownloadStreaming
	{
		//public string Code;
		private ArrayList alSymbols;
		private Thread tStream;
		private int interval;
		public  event OnNewPacket NewPacket;

		public DownloadStreaming(string Symbol,int interval)
		{
			alSymbols = new ArrayList();
			AddSymbol(Symbol);
			this.interval = interval;
		}

		/// <summary>
		/// Add new symbol to the download list
		/// </summary>
		/// <param name="Symbol"></param>
		public void AddSymbol(string Symbol)
		{
			string[] ss = Symbol.Split(',',';');
			foreach(string s in ss)
			{
				string r = s.ToUpper();
				if (alSymbols.IndexOf(r)<0)
					alSymbols.Add(r);
			}
		}

		/// <summary>
		/// Remove symbol from the download list
		/// </summary>
		/// <param name="Symbol"></param>
		public void RemoveSymbol(string Symbol)
		{
			alSymbols.Remove(Symbol);
		}

		/// <summary>
		/// Set the download list
		/// </summary>
		/// <param name="Symbol"></param>
		public void SetSymbol(string Symbol)
		{
			alSymbols.Clear();
			AddSymbol(Symbol);
		}

		private void Run()
		{
			while (true)
				try
				{
					DataPacket[] dps = DataPacket.DownloadMultiFromYahoo(
						string.Join(",",(string[])alSymbols.ToArray(typeof(string))));
					for(int i=0; i<alSymbols.Count; i++) 
					{
						if (NewPacket!=null)
							NewPacket(dps[i]);
					}
					Thread.Sleep(interval);
				} 
				catch
				{
				}
		}

		public void Start()
		{
			tStream = new Thread(new ThreadStart(Run));
			tStream.Start();
		}

		public void Stop()
		{
			tStream.Abort();
		}
	}
}
