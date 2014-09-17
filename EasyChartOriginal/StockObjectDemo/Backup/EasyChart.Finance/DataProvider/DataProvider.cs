using System;
using System.Collections;

namespace Easychart.Finance.DataProvider
{
	/// <summary>
	/// Provide stock data to stock chart
	/// </summary>
	public interface IDataProvider
	{
		double[] this[string Name] {get;}
		double GetConstData(string DataType);
		string GetStringData(string DataType);
		int Count {get;}
		IDataManager DataManager{get;}
		DataCycle DataCycle {get; set;}
		IDataProvider BaseDataProvider {get; set;}
		string GetUnique();
		int MaxCount {get; set;}
	}
}
