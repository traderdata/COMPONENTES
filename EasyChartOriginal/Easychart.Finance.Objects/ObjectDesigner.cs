using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using Easychart.Finance.DataProvider;
using Easychart.Finance;
using Easychart.Finance.Win;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for Designer.
	/// </summary>
	public class ObjectDesigner : System.Windows.Forms.UserControl,IObjectCanvas
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private const int WM_ERASEBKGND = 0x0014;
		private IDataProvider DataProvider= new RandomDataProvider(1000);
		private FormulaChart chart = new FormulaChart();
		private Font DesignerFont = new Font("verdana",10);
		private Brush DefaultBrush = Brushes.Black;
		private bool dragging;

		public FormulaChart BackChart
		{
			get
			{
				return chart;
			}
		}

		public Control DesignerControl 
		{
			get
			{
				return this;
			}
		}

		public bool Dragging
		{
			get
			{
				return dragging;
			}
			set
			{
				dragging = value;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg!=WM_ERASEBKGND)
				base.WndProc (ref m);
		}

		public ObjectDesigner()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ObjectDesigner
			// 
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Name = "ObjectDesigner";
			this.Size = new System.Drawing.Size(560, 424);
		}
		#endregion
	}
}