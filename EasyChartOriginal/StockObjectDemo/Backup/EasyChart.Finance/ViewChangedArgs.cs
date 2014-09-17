using System;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for ViewChangedArgs.
	/// </summary>
	public class ViewChangedArgs
	{
		public int StartBar;
		public int EndBar;
		public int FirstBar;
		public int LastBar;
		public ViewChangedArgs(int StartBar,int EndBar,int FirstBar,int LastBar)
		{
			this.StartBar = StartBar;
			this.EndBar = EndBar;
			this.FirstBar = FirstBar;
			this.LastBar = LastBar;
		}
	}
	public delegate void ViewChangedHandler(object sender,ViewChangedArgs e);

}
