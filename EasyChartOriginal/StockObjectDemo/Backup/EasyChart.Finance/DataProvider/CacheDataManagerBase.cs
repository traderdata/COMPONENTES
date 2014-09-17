using System;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// Summary description for CacheDataManagerBase.
	/// </summary>
	public class CacheDataManagerBase : DataManagerBase
	{
		public string CacheRoot;
		public bool EnableMemoryCache = false;
		public bool EnableFileCache = true;
		public TimeSpan CacheTimeSpan = TimeSpan.Zero;

		public CacheDataManagerBase()
		{
		}

		public CommonDataProvider MergeRealtime(CommonDataProvider cdp,string Code)
		{
			if (cdp!=null)
			{
				if (DownloadRealTimeQuote && CacheTimeSpan.Days>=1)
				{
					DataPacket dp = DataPacket.DownloadFromYahoo(Code);
					cdp.Merge(dp);
					cdp.SetStringData("Name",dp.StockName);
				}
				SetStrings(cdp,Code);
			}
			return cdp;
		}

		private string GetKey(string Code)
		{
			return "Cache_"+Code+"_"+StartTime.ToString("yyyyMMddHHmmss")+"_"+EndTime.ToString("yyyyMMddHHmmss");
		}

		/// <summary>
		/// Implement the interface
		/// </summary>
		public override IDataProvider this[string Code,int Count]
		{
			get 
			{
				if (CacheTimeSpan==TimeSpan.Zero)
					return base[Code,Count];
				if (EnableMemoryCache && HttpContext.Current!=null) 
				{
					object o = HttpContext.Current.Cache[GetKey(Code)];
					if (o!=null)
						return MergeRealtime((CommonDataProvider)o,Code);
				}

				string Cache = "";
				if (EnableFileCache && CacheRoot!=null && CacheRoot!="")
					try
					{
						if (CacheRoot.EndsWith("\\"))
							CacheRoot = CacheRoot.Substring(0,CacheRoot.Length-1);
						
						if (!Directory.Exists(CacheRoot))
							Directory.CreateDirectory(CacheRoot);

						CacheRoot +="\\";
						Cache = CacheRoot+GetKey(Code);
		
						if (File.Exists(Cache))
							if (File.GetLastWriteTime(Cache).Add(CacheTimeSpan)>DateTime.Now)
							{
								CommonDataProvider cdp = new CommonDataProvider(this);
								cdp.LoadBinary(Cache);
								return MergeRealtime(cdp,Code);
							}
					} 
					catch
					{
						Cache = "";
					}

				IDataProvider idp = base[Code,Count];
				bool FileCached = false;
				if (idp is CommonDataProvider && idp.Count>0) 
				{
					try
					{
						if (EnableFileCache && Cache!="") 
						{
							(idp as CommonDataProvider).SaveBinary(Cache);
							FileCached = true;
						}
					}
					catch
					{
					}
				}

				if (EnableMemoryCache && HttpContext.Current!=null && !FileCached && idp!=null)
					HttpContext.Current.Cache.Add(GetKey(Code),idp,null,DateTime.Now.Add(CacheTimeSpan),TimeSpan.Zero,CacheItemPriority.Default,null);
				return MergeRealtime((CommonDataProvider)idp,Code);
			}
		}
	}
}
