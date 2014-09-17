using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Used to control the scroll/zoom/reset of the ChartWinControl
	/// </summary>
	public class SizeToolControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.HScrollBar hsbView;
		private System.Windows.Forms.ToolBar tnControl;
		private bool Scrolling;
		private ChartWinControl chartControl;
		private System.Windows.Forms.ToolBarButton tbbSizeAll;
		private System.Windows.Forms.ImageList ilToolBar;
		private System.Windows.Forms.ToolBarButton tbbZoomIn;
		private System.Windows.Forms.ToolBarButton tbbZoomOut;
		private System.ComponentModel.IContainer components;

		private int LastValue;
		
		/// <summary>
		/// The ChartWinControl to be controled
		/// </summary>
		public ChartWinControl ChartControl
		{
			get
			{
				return chartControl;
			}
			set
			{
				chartControl = value;
				if (value!=null) 
				{
					chartControl.ViewChanged -=new ViewChangedHandler(chartControl_ViewChanged);
					chartControl.ViewChanged +=new ViewChangedHandler(chartControl_ViewChanged);
				}
			}
		}

		/// <summary>
		/// Create instance of SizeToolControl
		/// </summary>
		public SizeToolControl()
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SizeToolControl));
			this.hsbView = new System.Windows.Forms.HScrollBar();
			this.tnControl = new System.Windows.Forms.ToolBar();
			this.tbbSizeAll = new System.Windows.Forms.ToolBarButton();
			this.tbbZoomIn = new System.Windows.Forms.ToolBarButton();
			this.tbbZoomOut = new System.Windows.Forms.ToolBarButton();
			this.ilToolBar = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// hsbView
			// 
			this.hsbView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.hsbView.Location = new System.Drawing.Point(0, 0);
			this.hsbView.Name = "hsbView";
			this.hsbView.Size = new System.Drawing.Size(648, 20);
			this.hsbView.TabIndex = 2;
			this.hsbView.ValueChanged += new System.EventHandler(this.hsbView_ValueChanged);
			// 
			// tnControl
			// 
			this.tnControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tnControl.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.tnControl.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						 this.tbbSizeAll,
																						 this.tbbZoomIn,
																						 this.tbbZoomOut});
			this.tnControl.ButtonSize = new System.Drawing.Size(24, 24);
			this.tnControl.Divider = false;
			this.tnControl.Dock = System.Windows.Forms.DockStyle.None;
			this.tnControl.DropDownArrows = true;
			this.tnControl.ImageList = this.ilToolBar;
			this.tnControl.Location = new System.Drawing.Point(656, -2);
			this.tnControl.Name = "tnControl";
			this.tnControl.ShowToolTips = true;
			this.tnControl.Size = new System.Drawing.Size(80, 30);
			this.tnControl.TabIndex = 3;
			this.tnControl.Wrappable = false;
			this.tnControl.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tnControl_ButtonClick);
			// 
			// tbbSizeAll
			// 
			this.tbbSizeAll.ImageIndex = 0;
			this.tbbSizeAll.ToolTipText = "Reset";
			// 
			// tbbZoomIn
			// 
			this.tbbZoomIn.ImageIndex = 1;
			this.tbbZoomIn.ToolTipText = "Zoom In";
			// 
			// tbbZoomOut
			// 
			this.tbbZoomOut.ImageIndex = 2;
			this.tbbZoomOut.ToolTipText = "Zoom Out";
			// 
			// ilToolBar
			// 
			this.ilToolBar.ImageSize = new System.Drawing.Size(20, 20);
			this.ilToolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilToolBar.ImageStream")));
			this.ilToolBar.TransparentColor = System.Drawing.Color.White;
			// 
			// SizeToolControl
			// 
			this.Controls.Add(this.tnControl);
			this.Controls.Add(this.hsbView);
			this.Name = "SizeToolControl";
			this.Size = new System.Drawing.Size(736, 20);
			this.ResumeLayout(false);

		}
		#endregion

		private void hsbView_ValueChanged(object sender, System.EventArgs e)
		{
			if (!Scrolling && ChartControl!=null)
			{
				Scrolling = true;
				ChartControl.MoveChartXBars(hsbView.Value-LastValue);
				LastValue = hsbView.Value;
			}
		}

		private void chartControl_ViewChanged(object sender, ViewChangedArgs e)
		{
			if (!Scrolling)
			{
				Scrolling = true;
				try
				{
					hsbView.Minimum = e.FirstBar;
					hsbView.Maximum = e.LastBar;
					hsbView.LargeChange = e.EndBar-e.StartBar+1;
					hsbView.Value = e.StartBar;
					LastValue = e.StartBar;
				}
				catch
				{
				}
				finally
				{
					Scrolling = false;
				}
			} else Scrolling = false;
		}

		private void AdjustSize(double Multiply)
		{
			ChartControl.ScaleChart(Multiply);
		}

		private void tnControl_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (ChartControl!=null)
			{
				if (e.Button==tbbSizeAll)
				{
					ChartControl.Reset(5);
				} 
				else if (e.Button == tbbZoomIn)
				{
					AdjustSize(0.2);
				} 
				else if (e.Button==tbbZoomOut)
				{
					AdjustSize(-0.2);
				}
			}
		}
	}
}