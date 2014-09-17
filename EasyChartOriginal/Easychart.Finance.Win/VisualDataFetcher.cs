using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using Easychart.Finance;
using Easychart.Finance.DataClient;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Win
{
	public enum LoginStatus{Yes,No,Cancel};
	public delegate void OnHistoryFinish(object sender,CommonDataProvider DataProvider);
	public delegate bool OnNeedLogin(object sender);
	public delegate void OnSymbolList(object sender,string[] SymbolList);
	public delegate void OnEodData(object sender,DataPackage[] EodData);
	/// <summary>
	/// Fetch data from a datafeed, will show a progress bar
	/// </summary>
	public class VisualDataFetcher : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btnCancel;
		private PointF StartDrag = PointF.Empty;
		private Bitmap MemBmp;
		private Thread DownloadThread;

		public DataClientBase DataClient;
		public string Username;
		public string Password;

		private DateTime StartTime;
		private DateTime EndTime;
		private string Exchanges;

		public event OnHistoryFinish OnHistoryFinish;
		public event OnNeedLogin OnNeedLogin;
		public event OnSymbolList OnSymbolList;
		public event OnEodData OnEodData;
		
		private string symbol;
		public string Symbol
		{
			get
			{
				return symbol;
			}
			set
			{
				symbol = value;
				Invalidate();
			}
		}

		private string msg;
		public string Msg
		{
			get
			{
				return msg;
			}
			set
			{
				msg = value;
				Invalidate();
			}
		}

		private bool isCurrent;
		public bool IsCurrent
		{
			get 
			{
				return isCurrent;
			}
			set
			{
				isCurrent = value;
				Invalidate();
			}
		}

		private int progressValue;
		public int ProgressValue
		{
			get
			{
				return progressValue;
			}
			set
			{
				progressValue = value;
				Invalidate();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		public VisualDataFetcher()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		private LoginStatus Login()
		{
			this.Visible = true;
			Msg = "Verifing ... ...";
			DataClient.OnProgress -=new DataProgress(DataClient_OnProgress);
			if (!DataClient.Login(DataFeedLogin.Current.Username,DataFeedLogin.Current.Password))
			{
				Msg ="Login failed!";
				if (OnNeedLogin!=null)
					if (OnNeedLogin(this))
						return LoginStatus.No;
				return LoginStatus.Cancel;
			}
			DataClient.OnProgress +=new DataProgress(DataClient_OnProgress);
			Msg = "Downloading data...";
			return LoginStatus.Yes;
		}

		public void BackgroundHistory()
		{
			CommonDataProvider cdp = null;
			try
			{
				if (DataClient!=null)
				{
					while (true)
					{
						LoginStatus ls = Login();
						if (ls==LoginStatus.Yes)
							cdp = DataClient.GetHistoricalData(Symbol);
						if (ls!=LoginStatus.No)
							break;
					}
				}
			}
			finally
			{
				if (OnHistoryFinish!=null)
					OnHistoryFinish(this,cdp);
				DownloadThread = null;
			}
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

		public void GetDataProvider(DataClientBase DataClient,string Symbol)
		{
			this.DataClient = DataClient;
			this.Symbol = Symbol;
			Msg = "";
			ProgressValue = 0;
			DownloadThread = new Thread(new ThreadStart(BackgroundHistory));
			DownloadThread.Start();
		}

		public void BackgroundSymbolList()
		{
			string[] SymbolList = null;
			try
			{
				if (DataClient!=null)
				{
					while (true)
					{
						LoginStatus ls = Login();
						if (ls==LoginStatus.Yes)
							SymbolList = DataClient.GetSymbols(Exchanges);
						if (ls!=LoginStatus.No)
							break;
					}
				}
			}
			finally
			{
				if (OnSymbolList!=null)
					OnSymbolList(this,SymbolList);
				DownloadThread = null;
			}
		}

		public void GetSymbolList(DataClientBase DataClient,string Exchanges)
		{
			this.DataClient = DataClient;
			this.Exchanges = Exchanges;
			Msg = "";
			ProgressValue = 0;
			DownloadThread = new Thread(new ThreadStart(BackgroundSymbolList));
			DownloadThread.Start();
		}

		public void BackgroundEodData()
		{
			DataPackage[] EodData = null;
			try
			{
				if (DataClient!=null)
				{
					while (true)
					{
						LoginStatus ls = Login();
						if (ls==LoginStatus.Yes)
							EodData = DataClient.GetEodData(Exchanges,StartTime,EndTime);
						if (ls!=LoginStatus.No)
							break;
					}
				}
			}
			finally
			{
				if (OnEodData!=null)
					OnEodData(this,EodData);
				DownloadThread = null;
			}
		}

		public void GetEodData(DataClientBase DataClient,string Exchanges,DateTime StartTime,DateTime EndTime)
		{
			this.DataClient = DataClient;
			this.Exchanges = Exchanges;
			this.StartTime = StartTime;
			this.EndTime = EndTime;
			Msg = "";
			ProgressValue = 0;
			DownloadThread = new Thread(new ThreadStart(BackgroundEodData));
			DownloadThread.Start();
		}

		public void ToCenter()
		{
			Location = new Point((Parent.Width-Width)/2,(Parent.Height-Height)/2);
			Visible = true;
		}

		public void ToOffScreen()
		{
			Left = -1000;
			Visible = false;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCancel.Location = new System.Drawing.Point(232, 4);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(51, 17);
			this.btnCancel.TabIndex = 20;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// VisualDataFetcher
			// 
			this.Controls.Add(this.btnCancel);
			this.ForeColor = System.Drawing.Color.Black;
			this.Name = "VisualDataFetcher";
			this.Size = new System.Drawing.Size(288, 26);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VisualDataFetcher_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ProgressPanel_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VisualDataFetcher_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VisualDataFetcher_MouseDown);
			this.ResumeLayout(false);

		}
		#endregion

		private void DataClient_OnProgress(object sender, string Symbol,int CurrentValue, int MaxValue)
		{
			if (this.Symbol==Symbol) 
			{
				ProgressValue = (int)((double)CurrentValue/MaxValue*100);
				Msg = "Downloading ..."+ProgressValue+"%";
				Application.DoEvents();
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			FocusChartControl();
			DownloadThread.Abort();
			DownloadThread.Join();
		}

		private void FocusChartControl()
		{
			Form f = this.FindForm();
			if (f!=null)
			{
				foreach(Control c in f.Controls)
					if (c is ChartWinControl)
					{
						f.ActiveControl = c;
						f.ActiveControl.Focus();
					}
			}
		}

		private void Draw(Graphics g)
		{
			Rectangle R = ClientRectangle;
			R.Inflate(-1,-1);
			Color BorderColor = Color.Black;
			if (IsCurrent)
				BorderColor = Color.Red;
			g.DrawRectangle(new Pen(BorderColor,3),R);
			R.Inflate(-2,-2);
			g.DrawRectangle(new Pen(Color.White,2),R);
			
			Rectangle[] Rs = {
								 new Rectangle(R.X,R.Y,R.Width*progressValue/100,R.Height),
								 R,
			};
			Rs[1].X = Rs[0].Right;
			Rs[1].Width = R.Width-Rs[0].Right+2;

			if (Rs[0].Width>0)
			{
				LinearGradientBrush lgr = new LinearGradientBrush(Rs[0],Color.Blue,Color.FromArgb(200,200,255),90,false); 
				g.FillRectangle(lgr,Rs[0]);
			}
			g.FillRectangle(Brushes.White,Rs[1]);

			for(int i=0; i<2; i++)
			{
				g.SetClip(Rs[i]);
				Brush B = i==0?Brushes.White:Brushes.Black;
				g.DrawString(Symbol,this.Font,B,10,5,StringFormat.GenericDefault);
				g.DrawString(Msg,this.Font,B,80,5,StringFormat.GenericDefault);
			}
		}

		private void ProgressPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (MemBmp==null)
				MemBmp = new Bitmap(this.Width,this.Height,PixelFormat.Format32bppPArgb);
			Graphics g = Graphics.FromImage(MemBmp);
			Draw(g);
			e.Graphics.DrawImage(MemBmp,0,0);
		}

		private void VisualDataFetcher_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			StartDrag = new PointF(e.X,e.Y);
		}

		private void VisualDataFetcher_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (StartDrag!=PointF.Empty)
				this.Location = Point.Round(new PointF(Location.X+e.X-StartDrag.X,Location.Y+e.Y-StartDrag.Y));
		}

		private void VisualDataFetcher_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			StartDrag = PointF.Empty;
		}
	}
}