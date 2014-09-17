using System;
using System.Collections;
using Easychart.Finance.DataProvider;
using Easychart.Finance;

namespace FML.NATIVE
{
	/// <summary>
	/// Create formulas by custom data
	/// </summary>
	/// <example>
	/// DBDemos\Extern.aspx.cs
	/// </example>
	public class CustomFormula:FormulaBase
	{
		private ArrayList alFormulas = new ArrayList();
		public CustomFormula()
		{
		}

		/// <summary>
		/// Create instance
		/// </summary>
		/// <param name="Name">The name of this formula</param>
		public CustomFormula(string Name)
		{
			this.Name = Name;
		}

		/// <summary>
		/// Add custom data to this formula
		/// </summary>
		/// <param name="Data"></param>
		public void Add(double[] Data)
		{
			Add("",Data);
		}

		/// <summary>
		/// Add custom data to this formula
		/// </summary>
		/// <param name="Name">Formula Name</param>
		/// <param name="Data">Formula data array</param>
		public void Add(string Name, double[] Data)
		{
			Add(Name,(double[])null,Data);
		}

		/// <summary>
		/// Add custom data to this formula
		/// </summary>
		/// <param name="Name">Formula Name</param>
		/// <param name="Date">Formula date data array</param>
		/// <param name="Data">Formula data array</param>
		public void Add(string Name, double[] Date,double[] Data)
		{
			alFormulas.Add(new CustomData(Name,Date,Data));
		}

		/// <summary>
		/// Add custom data to this formula
		/// </summary>
		/// <param name="Name">Formula Name</param>
		/// <param name="Date">Formula date data array</param>
		/// <param name="Data">Formula data array</param>
		public void Add(string Name, DateTime[] Date,double[] Data) 
		{
			double[] D = new double[Date.Length];
			for(int i=0; i<Date.Length; i++)
				D[i] = Date[i].ToOADate();
			Add(Name,D,Data);
		}

		/// <summary>
		/// Create formula data package
		/// </summary>
		/// <param name="dp"></param>
		/// <returns></returns>
		public override FormulaPackage Run(IDataProvider dp)
		{
			this.DataProvider = dp;

			FormulaData[] fds = new FormulaData[alFormulas.Count];
			for(int i=0; i<fds.Length; i++)
			{
				CustomData cd = (CustomData)alFormulas[i];
				if (cd.Date!=null)
					fds[i] = AdjustDateTime(cd.Date,cd.Data);
				else 
					fds[i] = cd.Data;
				fds[i].Name = cd.Name;
			}
			return new FormulaPackage(fds,"");
		}
	}

	public class CustomData
	{
		public double[] Date;
		public double[] Data;
		public string Name;

		public CustomData(string Name,double[] Date,double[] Data) 
		{
			this.Name = Name;
			this.Date = Date;
			this.Data = Data;
		}
	}
}