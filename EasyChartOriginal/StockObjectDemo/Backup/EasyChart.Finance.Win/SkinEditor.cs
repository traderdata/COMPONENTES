using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Drawing;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Easychart.Finance.DataProvider;
using Easychart.Finance;

namespace Easychart.Finance.Win
{
	/// <summary>
	/// Custom skin editor, not implement
	/// </summary>
	[ToolboxItem(false)]
	public class SkinEditor: System.Windows.Forms.Form
	{
		private System.Windows.Forms.PropertyGrid pg;
		private System.Windows.Forms.Panel pnClient;
		private System.Windows.Forms.Splitter spVerticle;
		private System.Windows.Forms.ComboBox cbSkin;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Panel pnRight;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCreateNew;
		private Easychart.Finance.Win.ChartWinControl ChartControl;
		private System.Windows.Forms.Label lBuildin;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Create the SkinForm instance
		/// </summary>
		public SkinEditor()
		{
			//
			// Required for Windows Form Designer support
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            Easychart.Finance.ExchangeIntraday exchangeIntraday1 = new Easychart.Finance.ExchangeIntraday();
            this.pg = new System.Windows.Forms.PropertyGrid();
            this.pnClient = new System.Windows.Forms.Panel();
            this.pnRight = new System.Windows.Forms.Panel();
            this.lBuildin = new System.Windows.Forms.Label();
            this.ChartControl = new Easychart.Finance.Win.ChartWinControl();
            this.btnCreateNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbSkin = new System.Windows.Forms.ComboBox();
            this.spVerticle = new System.Windows.Forms.Splitter();
            this.pnClient.SuspendLayout();
            this.pnRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // pg
            // 
            this.pg.Dock = System.Windows.Forms.DockStyle.Left;
            this.pg.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pg.Location = new System.Drawing.Point(0, 0);
            this.pg.Name = "pg";
            this.pg.Size = new System.Drawing.Size(250, 510);
            this.pg.TabIndex = 0;
            this.pg.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pg_PropertyValueChanged);
            // 
            // pnClient
            // 
            this.pnClient.Controls.Add(this.pnRight);
            this.pnClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnClient.Font = new System.Drawing.Font("Verdana", 8.5F);
            this.pnClient.Location = new System.Drawing.Point(250, 0);
            this.pnClient.Name = "pnClient";
            this.pnClient.Size = new System.Drawing.Size(470, 510);
            this.pnClient.TabIndex = 1;
            // 
            // pnRight
            // 
            this.pnRight.Controls.Add(this.lBuildin);
            this.pnRight.Controls.Add(this.ChartControl);
            this.pnRight.Controls.Add(this.btnCreateNew);
            this.pnRight.Controls.Add(this.btnSave);
            this.pnRight.Controls.Add(this.btnOK);
            this.pnRight.Controls.Add(this.cbSkin);
            this.pnRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnRight.Location = new System.Drawing.Point(0, 0);
            this.pnRight.Name = "pnRight";
            this.pnRight.Size = new System.Drawing.Size(470, 510);
            this.pnRight.TabIndex = 3;
            // 
            // lBuildin
            // 
            this.lBuildin.AutoSize = true;
            this.lBuildin.Location = new System.Drawing.Point(7, 7);
            this.lBuildin.Name = "lBuildin";
            this.lBuildin.Size = new System.Drawing.Size(94, 14);
            this.lBuildin.TabIndex = 6;
            this.lBuildin.Text = "Build-in skins:";
            // 
            // ChartControl
            // 
            this.ChartControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChartControl.CausesValidation = false;
            this.ChartControl.DefaultFormulas = "MAIN;VOLMA;RSI(14)#RSI(28);MACD";
            this.ChartControl.Designing = false;
            this.ChartControl.EndTime = new System.DateTime(((long)(0)));
            this.ChartControl.FavoriteFormulas = "VOLMA;RSI;CCI;OBV;ATR;FastSTO;SlowSTO;ROC;TRIX;WR;AD;CMF;PPO;StochRSI;ULT;BBWidth" +
    ";PVO";
            exchangeIntraday1.TimeZone = -5D;
            this.ChartControl.IntradayInfo = exchangeIntraday1;
            this.ChartControl.LatestValueType = Easychart.Finance.LatestValueType.None;
            this.ChartControl.Location = new System.Drawing.Point(12, 33);
            this.ChartControl.MaxPrice = 0D;
            this.ChartControl.MinPrice = 0D;
            this.ChartControl.Name = "ChartControl";
            this.ChartControl.PriceLabelFormat = null;
            this.ChartControl.ShowStatistic = false;
            this.ChartControl.Size = new System.Drawing.Size(448, 434);
            this.ChartControl.StartTime = new System.DateTime(((long)(0)));
            this.ChartControl.StockBars = 70;
            this.ChartControl.Symbol = "Test";
            this.ChartControl.TabIndex = 5;
            // 
            // btnCreateNew
            // 
            this.btnCreateNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateNew.Location = new System.Drawing.Point(240, 479);
            this.btnCreateNew.Name = "btnCreateNew";
            this.btnCreateNew.Size = new System.Drawing.Size(87, 21);
            this.btnCreateNew.TabIndex = 4;
            this.btnCreateNew.Text = "Create New";
            this.btnCreateNew.Click += new System.EventHandler(this.btnCreateNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(333, 479);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 21);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(400, 479);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(62, 21);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Close";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbSkin
            // 
            this.cbSkin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSkin.Location = new System.Drawing.Point(87, 3);
            this.cbSkin.Name = "cbSkin";
            this.cbSkin.Size = new System.Drawing.Size(100, 21);
            this.cbSkin.TabIndex = 0;
            this.cbSkin.SelectedIndexChanged += new System.EventHandler(this.cbSkin_SelectedIndexChanged);
            // 
            // spVerticle
            // 
            this.spVerticle.Location = new System.Drawing.Point(250, 0);
            this.spVerticle.Name = "spVerticle";
            this.spVerticle.Size = new System.Drawing.Size(2, 510);
            this.spVerticle.TabIndex = 2;
            this.spVerticle.TabStop = false;
            // 
            // SkinEditor
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(720, 510);
            this.Controls.Add(this.spVerticle);
            this.Controls.Add(this.pnClient);
            this.Controls.Add(this.pg);
            this.Name = "SkinEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Skin Manager";
            this.Load += new System.EventHandler(this.SkinForm_Load);
            this.pnClient.ResumeLayout(false);
            this.pnRight.ResumeLayout(false);
            this.pnRight.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void SkinForm_Load(object sender, System.EventArgs e)
		{
			cbSkin.Items.AddRange(FormulaSkin.GetBuildInSkins());
			ChartControl.DataManager = new RandomDataManager();
		}

		private void cbSkin_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FormulaSkin fs = FormulaSkin.GetSkinByName(cbSkin.SelectedItem.ToString());
			pg.SelectedObject = fs;
			fs.CollectionValueChanged-=new EventHandler(fs_CollectionValueChanged);
			fs.CollectionValueChanged+=new EventHandler(fs_CollectionValueChanged);
			ChartControl.Skin = cbSkin.SelectedItem.ToString();
		}

		private void pg_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			ChartControl.ApplySkin((FormulaSkin)pg.SelectedObject);
		}

		private void fs_CollectionValueChanged(object sender, EventArgs e)
		{
			ChartControl.ApplySkin((FormulaSkin)pg.SelectedObject);
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			XmlSerializer xs = new XmlSerializer(typeof(FormulaSkin));
			Directory.CreateDirectory(FormulaHelper.SkinRoot);
			FormulaSkin skin = (FormulaSkin)pg.SelectedObject;
			skin.Save();
		}

		private void btnCreateNew_Click(object sender, System.EventArgs e)
		{
			string s = "CustomSkin";
			for (int i=1; i<10; i++)
			{
				if (FormulaSkin.CheckSkinName(s+i))
				{
					FormulaSkin skin = (FormulaSkin)pg.SelectedObject;
					skin = skin.Clone();
					skin.SkinName = s+i;
					skin.Save();
					cbSkin.Items.Add(skin.SkinName);
					cbSkin.SelectedIndex= cbSkin.Items.Count-1;
					cbSkin_SelectedIndexChanged(this,null);
					break;
				}
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}