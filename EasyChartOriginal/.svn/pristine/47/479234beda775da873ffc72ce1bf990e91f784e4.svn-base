using System;
using System.Data;
using System.Reflection;
using System.IO;
using System.Globalization;
using Easychart.Finance.DataProvider;
using Easychart.Finance;

//Xml format :
//<?xml version="1.0" encoding="utf-8" ?>
//<table>
//<record>
//	<Date>1/1/2004</Date>
//	<Open>12</Open>
//	<High>13.2</High>
//	<Low>11.0</Low>
//	<Close>12.1</Close>
//	<Volume>122078</Volume>
//</record>
//<record>
//	<Date>2/1/2004</Date>
//	<Open>12.1</Open>
//	<High>14</High>
//	<Low>11.9</Low>
//	<Close>12.4</Close>
//	<Volume>122536</Volume>
//</record>
//<record>
//	<Date>3/1/2004</Date>
//	<Open>13.3</Open>
//	<High>14</High>
//	<Low>12</Low>
//	<Close>13</Close>
//	<Volume>143232</Volume>
//</record>
//</table>

namespace WindowsDemo

{
	/// <summary>
	/// Summary description for XmlDataManager.
	/// </summary>
	public class XmlDataManager:DataManagerBase
	{
		public XmlDataManager()
		{
		}

		public override IDataProvider GetData(string Code, int Count)
		{
			CommonDataProvider cdp = new CommonDataProvider(this);
			cdp.SetStringData("Code",Code);
			string s =  FormulaHelper.Root+"Data\\"+Code+".xml";
			if (File.Exists(s))
			{
				DataSet ds = new DataSet();
				ds.ReadXml(s);
				if (ds.Tables.Count>0)
				{
					DataTable dt = ds.Tables[0];
					int N = dt.Rows.Count;
					double[] OPEN = new double[N];
					double[] HIGH =  new double[N];
					double[] LOW =  new double[N];
					double[] CLOSE =  new double[N];
					double[] VOLUME =  new double[N];
					double[] AMOUNT =  new double[N];
					double[] DATE = new double[N];

					for(int i=0; i<N; i++)
					{
						OPEN[i] = double.Parse(dt.Rows[i]["Open"].ToString());
						HIGH[i] = double.Parse(dt.Rows[i]["High"].ToString());
						LOW[i] = double.Parse(dt.Rows[i]["Low"].ToString());
						CLOSE[i] = double.Parse(dt.Rows[i]["Close"].ToString());
						VOLUME[i] = double.Parse(dt.Rows[i]["Volume"].ToString());
						DATE[i] = DateTime.Parse(dt.Rows[i]["Date"].ToString(),DateTimeFormatInfo.InvariantInfo).ToOADate();
					}
					cdp.LoadBinary(new double[][]{OPEN,HIGH,LOW,CLOSE,VOLUME,DATE});
					return cdp;
				}
			}
			return base.GetData (Code, Count);
		}
	}
}
