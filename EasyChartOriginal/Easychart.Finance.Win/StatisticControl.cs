using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Statistic window for the chart control
	/// </summary>
	[ToolboxItem(false)]
	public class StatisticControl : System.Windows.Forms.Panel
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private string Data;
		private string[][] sss;
		private PointF StartDrag = PointF.Empty;
		private Graphics CurrentG;
		private Rectangle CloseRect = new Rectangle(4,4,10,10);
		public event EventHandler OnHide;

		/// <summary>
		/// Title height
		/// </summary>
		public int TitleHeight = 18;
		/// <summary>
		/// Row height
		/// </summary>
		public int RowHeight = 20;
		/// <summary>
		/// Row space
		/// </summary>
		public int RowSpace = 2;
		/// <summary>
		/// Column space
		/// </summary>
		public int ColumnSpace = 2;
		/// <summary>
		/// Adjust the height automatically
		/// </summary>
		public bool AutoHeight = true;
		/// <summary>
		/// Adjust the width automatically
		/// </summary>
		public bool AutoWidth = true;
		/// <summary>
		/// Column width
		/// </summary>
		public int[] ColumnWidth = new int[]{80,80};
		/// <summary>
		/// Row brushes
		/// </summary>
		public Brush[] RowBrushs = new Brush[]{Brushes.Khaki,Brushes.Beige,Brushes.WhiteSmoke};
		/// <summary>
		/// Frame color
		/// </summary>
		public Color FrameColor = Color.LightGray;

		public bool EnableMove = true;

		/// <summary>
		/// Prevent flicker
		/// </summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			if (DesignMode || Data==null)
				base.OnPaintBackground(pevent);
		}

		public bool HasData()
		{
			return Data!=null && Data!="";
		}

		/// <summary>
		/// Create StatisticControl instance
		/// </summary>
		public StatisticControl()
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
			// StatisticControl
			// 
			this.BackColor = System.Drawing.SystemColors.Info;
			this.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.ForeColor = System.Drawing.Color.Black;
			this.Size = new System.Drawing.Size(208, 344);
			this.Visible = false;
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.StatisticControl_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.StatisticControl_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StatisticControl_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StatisticControl_MouseDown);

		}
		#endregion

		/// <summary>
		/// Show data in the statistic control, data was separated by semi colon
		/// </summary>
		/// <param name="s">Data</param>
		public void RefreshData(string s) 
		{
			Data = s;
			if (s!=null)
			{
				if (s.EndsWith(";"))
					s = s.Substring(0,s.Length-1);
				string[] ss = s.Split(';');
				sss = new string[ss.Length][];
				for(int i=0; i<ss.Length; i++)
					sss[i] = ss[i].Split('=');
				if (CurrentG==null)
					CurrentG = this.CreateGraphics();

				if (AutoWidth)
				{
					ColumnWidth[0] = int.MinValue;
					ColumnWidth[1] = int.MinValue;
					for(int j=0; j<2; j++)
					{
						for(int i=0; i<ss.Length; i++)
						{
							SizeF size = CurrentG.MeasureString(sss[i][j],Font);
							ColumnWidth[j] = Math.Max(ColumnWidth[j],(int)size.Width);
						}
					}
					int w = ColumnWidth[0]+ColumnWidth[1]+ColumnSpace*5;
					if (w>Width && EnableMove)
						Width = w;
				}
				if (AutoHeight)
				{
					RowHeight = (int)CurrentG.MeasureString(sss[0][0],Font,1000).Height;
					if (EnableMove)
						Height = ss.Length*(RowHeight+RowSpace)+TitleHeight+RowSpace+2;
				}
				Invalidate();
			}
		}

		public void RefreshData() 
		{
			RefreshData(Data);
		}

		/// <summary>
		/// Paint the statistic control to a Graphics
		/// </summary>
		/// <param name="g"></param>
		public void PaintTo(Graphics g)
		{
			if (Visible)
				PaintTo(g,new Rectangle(Location.X,Location.Y,Size.Width,Size.Height-TitleHeight),false);
		}

		/// <summary>
		/// Paint the statistic control to a Graphics
		/// </summary>
		/// <param name="g">Graphics to show the control</param>
		/// <param name="Rect">Rectangle to show the control</param>
		/// <param name="ShowTitle">Show the statistic title</param>
		public void PaintTo(Graphics g,Rectangle Rect,bool ShowTitle)
		{
			if (Data==null || Data=="") 
			{
				//HideMe();
				g.FillRectangle(Brushes.WhiteSmoke,Rect);
				return;
			}
			
			if (ShowTitle)
			{
				g.FillRectangle(Brushes.WhiteSmoke,0,0,Rect.Width-1,TitleHeight);
				g.DrawRectangle(Pens.Black,0,0,Rect.Width-1,TitleHeight);
				g.DrawLine(Pens.Black,CloseRect.X,CloseRect.Y,CloseRect.Right,CloseRect.Bottom);
				g.DrawLine(Pens.Black,CloseRect.Right,CloseRect.Y,CloseRect.Left,CloseRect.Bottom);
				Rect.Y +=TitleHeight;
				Rect.Height -=TitleHeight;
			}

			Pen FramePen = new Pen(FrameColor);
			Rectangle ClientR = Rect;
			ClientR.Inflate(-1,-1);
			g.DrawRectangle(new Pen(Color.Black,2),ClientR);
			ClientR.Height--;
			g.SetClip(ClientR);

			int LastY = 0;
			for(int i=0; i<sss.Length; i++)
			{
				for(int j=0; j<2; j++)
				{
					Rectangle R = new Rectangle(
						ColumnSpace+Rect.X+ (j==1?(ColumnWidth[0]+ColumnSpace):0)+j,
						2+Rect.Y+(RowHeight+RowSpace)*i,
						j==0?(ColumnWidth[j]+ColumnSpace):(ClientR.Width-ColumnWidth[0]-ColumnSpace*3),
						RowHeight+RowSpace-1);
					LastY = R.Bottom;

					g.DrawRectangle(FramePen,R);
					g.FillRectangle(RowBrushs[i%RowBrushs.Length],R);
					StringFormat sf = new StringFormat();
					sf.LineAlignment = StringAlignment.Center;
					if (j==0)
						sf.Alignment = StringAlignment.Far;
					g.DrawString(sss[i][j],Font,new SolidBrush(ForeColor),R,sf);
				}
			}
			g.FillRectangle(Brushes.WhiteSmoke,2,LastY+1,Width-4,Height-LastY-1);
		}

		private void StatisticControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			PaintTo(e.Graphics,ClientRectangle,true);
		}

		private void HideMe()
		{
			Visible = false;
			if (OnHide!=null)
				OnHide(this,new EventArgs());
		}

		private void StatisticControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			
			if (CloseRect.Contains(e.X,e.Y))  
			{
				HideMe();
			}
			else if (EnableMove)
				StartDrag = new PointF(e.X,e.Y);
		}

		private void StatisticControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (StartDrag!=PointF.Empty)
				this.Location = Point.Round(new PointF(Location.X+e.X-StartDrag.X,Location.Y+e.Y-StartDrag.Y));
		}

		private void StatisticControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			StartDrag = PointF.Empty;
		}
	}
}