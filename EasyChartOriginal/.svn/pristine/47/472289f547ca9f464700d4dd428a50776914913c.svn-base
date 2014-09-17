using System;
using System.Collections;
using System.Text;

namespace Easychart.Finance
{
	/// <summary>
	/// Package of stock Formula data
	/// </summary>
	public class FormulaPackage:CollectionBase
	{
		//private int LastFormulaCount;

		//public FormulaDataCollection FormulaDataArray;
		public string Description;
		public string Name;

		/// <summary>
		/// Return the formula data by name
		/// </summary>
		public FormulaData this[string Name]
		{
			get 
			{
				for(int i=0; i<List.Count; i++)
				{
					FormulaData fd = (FormulaData)List[i];
						if (Name==i.ToString() || (fd.Name!=null && string.Compare(fd.Name.Trim() , Name , true)==0))
							return fd;

				}
				return null;
			}
		}

		public virtual int Add(FormulaData fd)
		{
			return List.Add(fd);
		}

		public virtual void Insert(int Index,FormulaData fd)
		{
			this.List.Insert(Index,fd);
		}

		public void AddRange(ICollection ic)
		{
			foreach(object o in ic)
				Add((FormulaData)o);
		}

		/// <summary>
		/// Return the formula data by index
		/// </summary>
		public FormulaData this[int i]
		{
			get 
			{
				return (FormulaData)List[i];
			}
			set
			{
				List[i] = value;
			}
		}

		public FormulaPackage():base() 
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="FormulaDatas">FormulaData array</param>
		/// <param name="Name">Name of this package</param>
		/// <param name="Description">Description of this package</param>
		public FormulaPackage(FormulaData[] FormulaDatas,string Name,string Description):this()
		{
			AddRange(FormulaDatas);
			this.Description = Description;
			this.Name = Name;
		}

		public FormulaPackage(FormulaData[] FormulaDatas,string Name):this(FormulaDatas,Name,"") 
		{
		}

		public FormulaPackage(FormulaData[] FormulaDatas):this(FormulaDatas,"") 
		{
		}
		
		public FormulaData GetFormulaData(string Name) 
		{
			return this[Name];
		}

		public int IndexOf(FormulaData fa) 
		{
			return List.IndexOf(fa);
		}

		public void Remove(FormulaData value) 
		{
			List.Remove(value);
		}
		
		public void Remove(string Name) 
		{
			List.Remove(this[Name]);
		}
	}

	/// <summary>
	/// Collection of FormulaPackage
	/// </summary>
	public class FormulaPackageCollection:CollectionBase
	{
		public virtual int Add(FormulaPackage fp)
		{
			return List.Add(fp);
		}

		public virtual FormulaPackage this[int Index] 
		{
			get
			{
				return (FormulaPackage)this.List[Index];
			}
		}

		public FormulaPackage this[string Name] 
		{
			get
			{
				foreach(object o in List)
					if (((FormulaPackage)o).Name==Name)
						return (FormulaPackage)o;
				return null;
			}
		}
	}
}
