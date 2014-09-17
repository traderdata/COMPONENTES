using System;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace FML.NATIVE //Easychart.Finance.
{
	/// <summary>
	/// An error message formula class, This will show an error message in the formula area
	/// </summary>
	public class ERROR:FormulaBase
	{
		public string MSG="";
		public ERROR():base()
		{
			AddParam("MSG","Errors","0","0");
		}
		public override FormulaPackage Run(IDataProvider dp)
		{
			this.DataProvider = dp;
			FormulaData A=NAN; A.Name="A";
			SETNAME(MSG);
			SETTEXTVISIBLE(A,FALSE);
			return new FormulaPackage(new FormulaData[]{A},"");
		}
	} //class ERROR
}
