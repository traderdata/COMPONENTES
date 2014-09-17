using System;
using System.Windows.Forms;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Used to add key message filter to current application
	/// </summary>
	public class KeyMessageFilter:IMessageFilter 
	{
		private const int WM_KEYDOWN = 0x0100;
		private static KeyMessageFilter KeyFilter;
		private event KeyEventHandler keyEventHandler;

		/// <summary>
		/// Add KeyEventHandler to current application
		/// </summary>
		/// <param name="keh"></param>
		static public void AddMessageFilter(KeyEventHandler keh)
		{
			if (KeyFilter==null)
			{
				KeyFilter = new KeyMessageFilter(new KeyEventHandler(keh));
				Application.AddMessageFilter(KeyFilter);
			}
		}

		/// <summary>
		/// Add KeyEventHandler to ChartWinControl
		/// </summary>
		/// <param name="chartWinControl"></param>
		static public void AddMessageFilter(ChartWinControl chartWinControl)
		{
			AddMessageFilter(new KeyEventHandler(chartWinControl.HandleKeyEvent));
		}

		/// <summary>
		/// Create instance of KeyMessageFilter
		/// </summary>
		/// <param name="keyEventHandler"></param>
		public KeyMessageFilter(KeyEventHandler keyEventHandler)
		{
			this.keyEventHandler = keyEventHandler ;
		}

		#region IMessageFilter Members
		/// <summary>
		/// Implement the IMessageFilter interface
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == WM_KEYDOWN)
			{
				KeyEventArgs kea = new KeyEventArgs((Keys)m.WParam.ToInt32() | Form.ModifierKeys);
				keyEventHandler(this,kea);
				if (kea.Handled)
					return kea.Handled;
			}
			return false;
		}
		#endregion
	}
}