namespace Teste01
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Easychart.Finance.ExchangeIntraday exchangeIntraday1 = new Easychart.Finance.ExchangeIntraday();
            this.chartWinControl1 = new Easychart.Finance.Win.ChartWinControl();
            this.SuspendLayout();
            // 
            // chartWinControl1
            // 
            this.chartWinControl1.CausesValidation = false;
            this.chartWinControl1.DefaultFormulas = null;
            this.chartWinControl1.Designing = false;
            this.chartWinControl1.EndTime = new System.DateTime(((long)(0)));
            this.chartWinControl1.FavoriteFormulas = "VOLMA;RSI;CCI;OBV;ATR;FastSTO;SlowSTO;ROC;TRIX;WR;AD;CMF;PPO;StochRSI;ULT;BBWidth" +
    ";PVO";
            exchangeIntraday1.TimeZone = -5D;
            this.chartWinControl1.IntradayInfo = exchangeIntraday1;
            this.chartWinControl1.Location = new System.Drawing.Point(212, 27);
            this.chartWinControl1.MaxPrice = 0D;
            this.chartWinControl1.MinPrice = 0D;
            this.chartWinControl1.Name = "chartWinControl1";
            this.chartWinControl1.PriceLabelFormat = null;
            this.chartWinControl1.Size = new System.Drawing.Size(584, 448);
            this.chartWinControl1.StartTime = new System.DateTime(((long)(0)));
            this.chartWinControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 503);
            this.Controls.Add(this.chartWinControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Easychart.Finance.Win.ChartWinControl chartWinControl1;

    }
}

