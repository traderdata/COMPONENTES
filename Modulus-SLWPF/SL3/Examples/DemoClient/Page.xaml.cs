using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModulusFE;
using ModulusFE.DS.Random;
using ModulusFE.Indicators;
using ModulusFE.LineStudies;
using ModulusFE.OMS.Interface;
using ModulusFE.SL;

namespace TestChart
{
    public partial class Page : INotifyPropertyChanged
    {
        public Page()
        {
            InitializeComponent();

            DataContext = this;

            //_stockChartX.AppRoot = LayoutRoot;

            LayoutRoot.Loaded += (sender, e) =>
            {
                _stockChartX.ApplyTemplate();
                CreateChart();

                cmbIndicators.ItemsSource = from indicator in StockChartX_IndicatorsParameters.Indicators
                                            where indicator.IndicatorType != IndicatorType.CustomIndicator
                                            orderby indicator.IndicatorRealName
                                            select indicator;
                cmbIndicators.SelectedIndex = 0;

                cmbLineStudies.ItemsSource = StockChartX_LineStudiesParams.LineStudiesTypes;
                cmbLineStudies.SelectedIndex = 0;
            };

            // How dynamic the data feed is.
            _dataFeed.Mess = 0.1;
        }

        // Keep global references of these elements to make life a bit easier when referencing them late.
        // As a general practice, this is not a bad idea BUT it is not advised to used this method as it 
        // is a bit too simple. In a real implementation a developer is going to want to control these
        // values better OR simply reference them directly using different calls into the control.
        ChartPanel _topPanel;
        ChartPanel _volumePanel;
        Series[] _ohlcSeries;
        Series _seriesVolume;

        private void CreateChart()
        {
            _stockChartX.ClearAll();
            _stockChartX.Symbol = RandomSymbol;

            _topPanel = _stockChartX.AddChartPanel();
            _volumePanel = _stockChartX.AddChartPanel(ChartPanel.PositionType.AlwaysBottom);

            _topPanel.Background = _volumePanel.Background = _chartPanelsBrush;

            _ohlcSeries = _stockChartX.AddOHLCSeries(_stockChartX.Symbol, _topPanel.Index);
            _seriesVolume = _stockChartX.AddVolumeSeries(_stockChartX.Symbol, _volumePanel.Index);

            _stockChartX.CrossHairs = true;

            ManualResetEvent eventHistory = new ManualResetEvent(false);
            List<BarData> data = new List<BarData>();

            // here, you have to connect to a WebService 
            // get data
            // append to chart
            // AJAX
            // RIA Services

            _dataFeed.GetHistory(new HistoryRequest
                                   {
                                       BarCount = _barCount,
                                       BarSize = 1,
                                       Periodicity = Periodicity.Minutely,
                                       Symbol = _stockChartX.Symbol
                                   }, datas =>
                                        {
                                            data.AddRange(datas);
                                            eventHistory.Set();
                                        });
            eventHistory.WaitOne();


            for (int row = 0; row < data.Count; row++)
            {
                BarData bd = data[row];
                _stockChartX.AppendOHLCValues(_stockChartX.Symbol, bd.TradeDate, bd.OpenPrice, bd.HighPrice, bd.LowPrice, bd.ClosePrice);
                _stockChartX.AppendVolumeValue(_stockChartX.Symbol, bd.TradeDate, bd.Volume);
            }

            //_ohlcSeries[OPEN].StrokeColor = ColorsEx.Lime;
            //_ohlcSeries[HIGH].StrokeColor = ColorsEx.Lime;
            //_ohlcSeries[LOW].StrokeColor = ColorsEx.Lime;
            _ohlcSeries[CLOSE].StrokeColor = ColorsEx.Lime;

            _ohlcSeries[CLOSE].TickBox = TickBoxPosition.Right;

            _seriesVolume.StrokeColor = Colors.Green;
            _seriesVolume.StrokeThickness = 1;
            // Note: If dividing volume by millions as we have above,
            // you should add an "M" to the volume panel like this:
            //StockChartX1.VolumePostfix = "M"; // M for "millions"
            // You could also divide by 1000 and show "K" for "thousands".
            _stockChartX.VolumePostfixLetter = "M";
            _stockChartX.VolumeDivisor = (int)1E6;

            //_stockChartX.ScaleAlignment = ScaleAlignmentTypeEnum.Left;
            _stockChartX.RightChartSpace = 0;
            _stockChartX.RealTimeXLabels = true;

            double totalPanelHeight = _stockChartX.PanelsCollection.Sum(p => p.Height);
            _stockChartX.SetPanelHeight(0, totalPanelHeight * 0.75);

            int visableBarCount = Math.Min(150, data.Count);
            int endIndex = data.Count;
            int startIndex = data.Count - (int)(visableBarCount);

            _stockChartX.FirstVisibleRecord = startIndex;
            _stockChartX.LastVisibleRecord = endIndex;
            _stockChartX.GridStroke = new SolidColorBrush(Color.FromArgb(0x33, 0xCC, 0xCC, 0xCC));
            _stockChartX.ThreeDStyle = false;
            _stockChartX.YGridStepType = YGridStepType.NiceStep;
            _stockChartX.KeepZoomLevel = true;
            _stockChartX.DisableZoomArea = true;
            _stockChartX.DarvasBoxes = false;

            _stockChartX.OptimizePainting = true;
            _stockChartX.Update();

            var lbl = _stockChartX.GetPanelByIndex(0).ChartPanelLabel;
            lbl.FontSize = 60;
            Canvas.SetLeft(lbl, 20);
            Canvas.SetTop(lbl, 20);
            lbl.Foreground = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xCC));
            lbl.Text = _stockChartX.Symbol;

            ResetChartSettings();
        }





        #region Tools

        private readonly DsRandom _dataFeed = new DsRandom();

        private int _barCount = 950;

        private SolidColorBrush _chartPanelsBrush = new SolidColorBrush(Colors.Black);

        private readonly Random _r = new Random();

        private string RandomSymbol
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < 5; i++)
                    sb.Append((char)_r.Next(65, 82));

                return sb.ToString();
            }
        }

        private void ResetChartSettings()
        {
            radioButtonLinear.IsChecked = true;
            checkBoxSideVolumeBars.IsChecked = false;
            checkBoxDarvasBoxes.IsChecked = false;
            checkBoxTickBox.IsChecked = true;
            checkBoxMouseZoomArea.IsChecked = false;
            radioButtonInfoPanelMouse.IsChecked = true;
        }

        // Used to make the code a bit easier to read.
        private const int OPEN = 0;
        private const int HIGH = 1;
        private const int LOW = 2;
        private const int CLOSE = 3;

        #endregion Tools





        #region DataBound Properties

        private Point _crossHairPosition = new Point();
        public Point CrossHairPosition
        {
            get
            {
                return _crossHairPosition;
            }
            set
            {
                if (_crossHairPosition == value) return;
                _crossHairPosition = value;
                NotifyPropertyChanged("CrossHairPosition");
                NotifyPropertyChanged("Ticket4489Content");
            }
        }

        private ScalingTypeEnum _scalingType = ScalingTypeEnum.Linear;
        public ScalingTypeEnum ScalingType
        {
            get
            {
                return _scalingType;
            }
            set
            {
                if (_scalingType == value) return;
                _scalingType = value;
                if (_stockChartX != null)
                {
                    _stockChartX.ScalingType = value;
                    _stockChartX.Update();
                }
                NotifyPropertyChanged("ScalingType");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion DataBount Properties





        #region Top Bar Buttons

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            CreateChart();
        }

        private int _indicatorCount;
        private bool _newPanelCreated;
        private void btnAddIndicator_Click(object sender, RoutedEventArgs e)
        {
            StockChartX_IndicatorsParameters.IndicatorParameters indicator =
              cmbIndicators.SelectedItem as StockChartX_IndicatorsParameters.IndicatorParameters;
            if (indicator == null)
            {
                MessageBox.Show("Could not get indicator");
                return;
            }
            if (indicator.IndicatorType == IndicatorType.CustomIndicator)
            {
                MessageBox.Show("Custom indicator can't be added via this. Use menu point [Data/Add Custom Indicator]", "Error",
                  MessageBoxButton.OK);
                return;
            }

            //mark a flag whether a new panel was created, must update chart if it was and user pressed Cancel in indicator dialog
            _newPanelCreated = !_stockChartX.IsOverlayIndicator(indicator.IndicatorType);
            string name = indicator.IndicatorRealName + (_indicatorCount > 0 ? _indicatorCount.ToString() : "");
            ChartPanel panel = !_newPanelCreated ? _stockChartX.GetPanelByIndex(0) : _stockChartX.AddChartPanel();

            if (panel == null)
            {
                MessageBox.Show("Chart does not have enough place for a new panel. Resize existings or remove some.",
                                "Not enough space", MessageBoxButton.OK);
                return;
            }

            if (_newPanelCreated)
                panel.Background = _chartPanelsBrush;

            _stockChartX.AddIndicator(indicator.IndicatorType, name, panel, true);

            _stockChartX.Update();
            _indicatorCount++;
        }

        private void _stockChartX_IndicatorAddComplete(object sender, StockChartX.IndicatorAddCompletedEventArgs e)
        {
            //this code will remove the unneeded empty panel created above when adding an indicator
            if (e.CanceledByUser && _newPanelCreated)
                _stockChartX.Update();
        }

        private void btnAddBar_Click(object sender, RoutedEventArgs e)
        {
            if (_stockChartX.ChartType == ChartTypeEnum.OHLC)
            {
                var barData = _dataFeed.GetNextBar(_stockChartX.Symbol);
                _stockChartX.AppendOHLCValues(_stockChartX.Symbol, barData.TradeDate, barData.OpenPrice, barData.HighPrice,
                                             barData.LowPrice, barData.ClosePrice);
                _stockChartX.AppendVolumeValue(_stockChartX.Symbol, barData.TradeDate, barData.Volume);
            }
            else
            {
                double price, volume;
                DateTime timeStamp;
                _dataFeed.GetNextTick(_stockChartX.Symbol, out price, out volume, out timeStamp);

                _stockChartX.AppendTickValue(_stockChartX.Symbol, timeStamp, price, volume);
            }
            _stockChartX.Update();
        }

        private void btnAddTick_Click(object sender, RoutedEventArgs e)
        {
            _stockChartX.TickCompressionType = TickCompressionEnum.Ticks;
            _stockChartX.TickPeriodicity = 5; //Create a bar every 5 ticks

            double price, volume;
            DateTime timeStamp;
            if (!_dataFeed.GetNextTick(_stockChartX.Symbol, out price, out volume, out timeStamp))
            {
                MessageBox.Show("Wrong symbol.");
                return;
            }

            _stockChartX.AppendTickValue(_stockChartX.Symbol, timeStamp, price, volume);

            _stockChartX.Update();
        }

        private Timer _timerTicks;
        private bool _timerEnabled;
        private void btnStartTickTimer_Click(object sender, RoutedEventArgs e)
        {
            if (_timerTicks == null)
            {
                _stockChartX.TickCompressionType = TickCompressionEnum.Ticks;
                _stockChartX.TickPeriodicity = 5; //Create a candle every 5 ticks

                _timerTicks =
                  new Timer(state =>
                            Dispatcher.BeginInvoke(
                              () =>
                              {
                                  double price, volume;
                                  DateTime timeStamp;
                                  _dataFeed.GetNextTick(_stockChartX.Symbol, out price, out volume, out timeStamp);

                                  _stockChartX.AppendTickValue(_stockChartX.Symbol, timeStamp, price, volume);

                                  _stockChartX.Update();
                              }));
            }

            _timerEnabled = !_timerEnabled;

            btnAddBar.IsEnabled = !_timerEnabled;
            btnAddTick.IsEnabled = !_timerEnabled;

            btnStartTickTimer.Content = _timerEnabled ? "Stop Tick Timer" : "Start Tick Timer";

            if (_timerEnabled)
            {
                _timerTicks.Change((int)TimeSpan.FromSeconds(1).TotalMilliseconds,
                                   (int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                _stockChartX.KeepZoomLevel = true; //this will scroll chart when adding a new candle
            }
            else
            {
                _timerTicks.Change(Timeout.Infinite, Timeout.Infinite);
                _stockChartX.KeepZoomLevel = false; //this will compress candles when adding a new one
            }
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();

            switch (tag)
            {
                case "zoomin": _stockChartX.ZoomIn(2); break;
                case "zoomout": _stockChartX.ZoomOut(2); break;
                case "resetzoom": _stockChartX.ResetZoom(); break;
            }
        }

        private void btnAddCustomIndicator_Click(object sender, RoutedEventArgs e)
        {
            //count indicator, just to give an unique name for a new indicator
            _indicatorCount = _stockChartX.GetIndicatorCountByType(IndicatorType.CustomIndicator);
            //create inidcator name
            string indicatorName = "Custom Indicator";
            if (_indicatorCount > 0)
                indicatorName += _indicatorCount;
            //get a reference to the custom indicator
            CustomIndicator indicator = (CustomIndicator)_stockChartX.AddIndicator(IndicatorType.CustomIndicator,
                                                                                   indicatorName, _stockChartX.AddChartPanel(),
                                                                                   true);
            //Add some parameters to the custom indicator, these parameters willl apear in indicator's dialog
            //where user will be able to set them manually
            indicator.AddParameter("My Param1", ParameterType.ptPeriods, 10, typeof(int));
            indicator.AddParameter("My Param2", ParameterType.ptPointsOrPercent, 12, typeof(int));
            indicator.AddParameter("My Param3", ParameterType.ptSymbol, "", typeof(string));
            indicator.AddParameter(null, ParameterType.ptLimitMoveValue, 16, typeof(int));

            indicator.SetParameterValue(0, 10);
            indicator.SetParameterValue(1, 12);
            indicator.SetParameterValue(2, _stockChartX.Symbol + ".CLOSE");
            indicator.SetParameterValue(3, 2);

            //after we set parameters call Update, this will raise the event OnCustomIndicatorNeedsData
            //where you use your own formulas to calculate the indicator values
            _stockChartX.Update();
        }

        private void _stockChartX_CustomIndicatorNeedsData(object sender, StockChartX.CustomIndicatorNeedsDataEventArgs e)
        {
            double?[] data = new double?[_stockChartX.RecordCount];

            //do here some calculations and pass them back to the chart
            int startValue = _r.Next(20, 40);
            double lastValue = startValue;
            //e.Values has the values already calculated from previous call
            //Here we just re-fill the array with random values
            for (int i = 0; i < data.Length; i++)
            {
                if (_r.NextDouble() > 0.5)
                    lastValue += _r.NextDouble() * 0.25;
                else
                    lastValue -= _r.NextDouble() * 0.25;

                data[i] = lastValue;
            }
            //pass calculated values back to the chart
            e.Values = data;
        }

        private void btnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            if (_stockChartX.FirstVisibleRecord == 0) return;
            _stockChartX.LastVisibleRecord--;
            _stockChartX.FirstVisibleRecord--;
        }

        private void btnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            if (_stockChartX.LastVisibleRecord == _stockChartX.RecordCount) return;
            _stockChartX.LastVisibleRecord++;
            _stockChartX.FirstVisibleRecord++;
        }

        #endregion Top Bar Buttons





        #region Side Bar Elements

        #region Line Studies

        private void btnAddLineStudy_Click(object sender, RoutedEventArgs e)
        {

            LineStudy.StudyTypeEnum studyTypeEnum =
              ((StockChartX_LineStudiesParams.LineStudyParams)cmbLineStudies.SelectedItem).StudyTypeEnum;
            object[] args = new object[0];
            double strokeThicknes = 1;

            //set some extra parameters to line studies
            switch (studyTypeEnum)
            {
                case LineStudy.StudyTypeEnum.StaticText:
                    args = new object[] { "Some text for testing" };
                    strokeThicknes = 12; //for text objects is FontSize
                    break;
                case LineStudy.StudyTypeEnum.VerticalLine:
                    //when first parameter is false, vertical line will display DataTime instead on record number
                    args = new object[]
                     {
                       false, //true - show record number, false - show datetime
                       true, //true - show text with line, false - show only line
                       "d", //custom datetime format, when args[0] == false. See MSDN:DateTime.ToString(string) for legal values
                     };
                    break;
                case LineStudy.StudyTypeEnum.ImageObject:
                    args = new object[]
                   {
                     "Res/open.png"
                   };
                    break;
                default:
                    break;
            }

            string studyName = studyTypeEnum.ToString();
            int count = _stockChartX.GetLineStudyCountByType(studyTypeEnum);
            if (count > 0)
                studyName += count;
            LineStudy lineStudy = _stockChartX.AddLineStudy(studyTypeEnum, studyName, new SolidColorBrush(colorPicker.SelectedColor), args);
            lineStudy.StrokeThickness = strokeThicknes;

            switch (studyTypeEnum)
            {
                case LineStudy.StudyTypeEnum.StaticText:
                    //if linestudy is a text object we can change its text directly
                    ((StaticText)lineStudy).Text = "Some other text for testing";
                    break;
                case LineStudy.StudyTypeEnum.HorizontalLine:
                    //change the appearance of horizontal Line
                    lineStudy.StrokeType = LinePattern.DashDot;
                    break;
                case LineStudy.StudyTypeEnum.ImageObject:
                    //set an additional ImageAlign property, this is very usefull when setting 
                    //images for close and open price, when you want to put image below or above a candle
                    //for Open price you'd use ImageObject.ImageAlign.BottomMiddle
                    //for Close price you'd use ImageObject.ImageAlign.TopMiddle
                    ((ImageObject)lineStudy).Align = ImageObject.ImageAlign.Center;
                    break;
            }

            btnAddLineStudy.IsEnabled = false;

            if (studyTypeEnum == LineStudy.StudyTypeEnum.HorizontalLine)
            {
                lineStudy.ValuePresenterAlignment = LineStudy.ValuePresenterAlignmentType.Left;
                lineStudy.LineStudyValue = new CustomHorLineValueGetter(_stockChartX);
            }
        }

        private void _stockChartX_UserDrawingComplete(object sender, StockChartX.UserDrawingCompleteEventArgs e)
        {
            btnAddLineStudy.IsEnabled = true;
        }

        #endregion Line Studies





        #region Chart Tools

        private void btnDeleteCurrentObject_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            bool needUpdate = false;
            foreach (object selectedObject in _stockChartX.SelectedObjectsCollection)
            {
                if (selectedObject is Indicator)
                {
                    needUpdate = true;
                    _stockChartX.RemoveSeries((Series)selectedObject);
                }
                else if (selectedObject is LineStudy)
                    _stockChartX.RemoveObject((LineStudy)selectedObject);
            }

            if (needUpdate)
                _stockChartX.Update();
        }

        private void btnIndProps_Click(object sender, RoutedEventArgs e)
        {
            Indicator selectedIndicator = _stockChartX.SelectedObjectsCollection.OfType<Indicator>().FirstOrDefault();

            if (selectedIndicator != null)
            {
                selectedIndicator.ShowParametersDialog();
                return;
            }

            MessageBox.Show("No indicator selected.");
        }

        private void btnQuietAddIndicator_Click(object sender, RoutedEventArgs e)
        {
            SimpleMovingAverage simpleMovingAverage =
              (SimpleMovingAverage)_stockChartX.AddIndicator(IndicatorType.SimpleMovingAverage,
                                                              Guid.NewGuid().ToString(), _stockChartX.GetPanelByIndex(0),
                                                              false);
            simpleMovingAverage.SetParameterValue(0, _stockChartX.Symbol + ".close");
            simpleMovingAverage.SetParameterValue(1, 14);
            simpleMovingAverage.UpColor = simpleMovingAverage.DownColor = Colors.Cyan;
            _stockChartX.Update();
        }

        private void btnSaveAsImage_Click(object sender, RoutedEventArgs e)
        {
            _stockChartX.SaveToFile(StockChartX.ImageExportType.Png);
        }

        private void radioButtonScalingType_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == radioButtonSemiLog)
                ScalingType = ScalingTypeEnum.Semilog;
            else
                ScalingType = ScalingTypeEnum.Linear;
        }

        private void checkBoxSideVolumeBars_Click(object sender, RoutedEventArgs e)
        {
            ChartPanel tp = _stockChartX.GetPanelByIndex(0);
            tp.SideVolumeDepthBars = checkBoxSideVolumeBars.IsChecked ?? false ? 5 : 0;
        }

        private void checkBoxDarvasBoxes_Click(object sender, RoutedEventArgs e)
        {
            _stockChartX.DarvasBoxes = checkBoxDarvasBoxes.IsChecked ?? false;
        }

        private void checkBoxTickBox_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTickBox.IsChecked.HasValue && checkBoxTickBox.IsChecked.Value)
            {
                _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".close").TickBox = TickBoxPosition.Right;
            }
            else
            {
                _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".close").TickBox = TickBoxPosition.None;
            }
        }

        private void checkBoxMouseZoomArea_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxMouseZoomArea.IsChecked.HasValue && checkBoxMouseZoomArea.IsChecked.Value)
                _stockChartX.DisableZoomArea = false;
            else
                _stockChartX.DisableZoomArea = true;
        }

        private void radioButtonInfoPanelType_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == radioButtonInfoPanelMouse)
                _stockChartX.InfoPanelPosition = InfoPanelPositionEnum.FollowMouse;
            if (sender == radioButtonInfoPanelFixed)
                _stockChartX.InfoPanelPosition = InfoPanelPositionEnum.FixedPosition;
            if (sender == radioButtonInfoPanelHidden)
                _stockChartX.InfoPanelPosition = InfoPanelPositionEnum.Hidden;
        }

        private void btnApplyPriceStyle_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((Control)cmbPriceStyles.SelectedItem).Tag.ToString();
            PriceStyleEnum priceStyle = PriceStyleEnum.psStandard;

            switch (tag)
            {
                case "standard": priceStyle = PriceStyleEnum.psStandard; break;
                case "kagi":
                    priceStyle = PriceStyleEnum.psKagi;
                    _stockChartX.SetPriceStyleParam(0, 0); //Reversal size
                    _stockChartX.SetPriceStyleParam(1, (double)ChartDataType.Points);
                    break;
                case "equivolume": priceStyle = PriceStyleEnum.psEquiVolume; break;
                case "candlevolume": priceStyle = PriceStyleEnum.psCandleVolume; break;
                case "equivolumeshadow": priceStyle = PriceStyleEnum.psEquiVolumeShadow; break;
                case "pointandfigure":
                    priceStyle = PriceStyleEnum.psPointAndFigure;
                    _stockChartX.SetPriceStyleParam(0, 0); //Allow StockChartX to figure the box size
                    _stockChartX.SetPriceStyleParam(1, 3); //Reversal size
                    break;
                case "renko":
                    priceStyle = PriceStyleEnum.psRenko;
                    _stockChartX.SetPriceStyleParam(0, 1); //Box size
                    break;
                case "threelinebreak":
                    priceStyle = PriceStyleEnum.psThreeLineBreak;
                    _stockChartX.SetPriceStyleParam(0, 3); //Three line break (could be 1 to 50 line break)
                    break;
                case "heikinashi": priceStyle = PriceStyleEnum.psHeikinAshi; break;
                case "barchart":
                    priceStyle = PriceStyleEnum.psStandard;
                    _ohlcSeries[OPEN].SeriesType = SeriesTypeEnum.stStockBarChart;
                    _ohlcSeries[HIGH].SeriesType = SeriesTypeEnum.stStockBarChart;
                    _ohlcSeries[LOW].SeriesType = SeriesTypeEnum.stStockBarChart;
                    _ohlcSeries[CLOSE].SeriesType = SeriesTypeEnum.stStockBarChart;
                    break;
                case "lineChart":
                    priceStyle = PriceStyleEnum.psStandard;
                    _ohlcSeries[OPEN].SeriesType = SeriesTypeEnum.stLineChart;
                    _ohlcSeries[HIGH].SeriesType = SeriesTypeEnum.stLineChart;
                    _ohlcSeries[LOW].SeriesType = SeriesTypeEnum.stLineChart;
                    _ohlcSeries[CLOSE].SeriesType = SeriesTypeEnum.stLineChart;
                    break;
                case "barchartHLC":
                    priceStyle = PriceStyleEnum.psStandard;
                    _ohlcSeries[OPEN].SeriesType = SeriesTypeEnum.stStockBarChartHLC;
                    _ohlcSeries[HIGH].SeriesType = SeriesTypeEnum.stStockBarChartHLC;
                    _ohlcSeries[LOW].SeriesType = SeriesTypeEnum.stStockBarChartHLC;
                    _ohlcSeries[CLOSE].SeriesType = SeriesTypeEnum.stStockBarChartHLC;
                    break;
                case "candleChart":
                    priceStyle = PriceStyleEnum.psStandard;
                    _ohlcSeries[OPEN].SeriesType = SeriesTypeEnum.stCandleChart;
                    _ohlcSeries[HIGH].SeriesType = SeriesTypeEnum.stCandleChart;
                    _ohlcSeries[LOW].SeriesType = SeriesTypeEnum.stCandleChart;
                    _ohlcSeries[CLOSE].SeriesType = SeriesTypeEnum.stCandleChart;
                    break;

            }

            _stockChartX.PriceStyle = priceStyle;
            _stockChartX.Update();
        }

        #region Serialization

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StockChartX.SerializationTypeEnum serType =
              (StockChartX.SerializationTypeEnum)System.Enum.Parse(typeof(StockChartX.SerializationTypeEnum),
              cmbSerType.SelectionBoxItem.ToString(), true);
            SaveFileDialog sfd = new SaveFileDialog();

            switch (serType)
            {
                case StockChartX.SerializationTypeEnum.All:
                    sfd.Filter = "StockChartX file (*.scx)|*.scx";
                    sfd.DefaultExt = "*.scx";
                    break;
                case StockChartX.SerializationTypeEnum.General:
                    sfd.Filter = "General Template file (*.scg)|*.scg";
                    sfd.DefaultExt = "*scg";
                    break;
                case StockChartX.SerializationTypeEnum.Objects:
                    sfd.Filter = "Object template file (*.sco)|*.sco";
                    sfd.DefaultExt = "*.sco";
                    break;
            }

            if (sfd.ShowDialog() == false)
                return;

            byte[] bytes = _stockChartX.SaveFile(serType);

            using (Stream stream = sfd.OpenFile())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            MessageBox.Show("Chart saved. Bytes Count: " + bytes.Length);
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytes;
            StockChartX.SerializationTypeEnum serType =
              (StockChartX.SerializationTypeEnum)System.Enum.Parse(typeof(StockChartX.SerializationTypeEnum),
              cmbSerType.SelectionBoxItem.ToString(), true);
            OpenFileDialog ofd = new OpenFileDialog();

            switch (serType)
            {
                case StockChartX.SerializationTypeEnum.All:
                    ofd.Filter = "StockChartX file (*.scx)|*.scx";
                    break;
                case StockChartX.SerializationTypeEnum.General:
                    ofd.Filter = "General Template file (*.scg)|*.scg";
                    break;
                case StockChartX.SerializationTypeEnum.Objects:
                    ofd.Filter = "Object template file (*.sco)|*.sco";
                    break;
            }

            if (ofd.ShowDialog() == false)
                return;

            if (serType == StockChartX.SerializationTypeEnum.All)
            {
                _stockChartX.ClearAll();//we must clear the chart when loading ALL
                _stockChartX.Update();
            }

            using (Stream stream = ofd.File.OpenRead())
            {
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                int res = _stockChartX.LoadFile(bytes, serType);
                if (res == 0)
                {
                    int count = _stockChartX.RecordCount;
                    int maxBarCount = Math.Min(500, count);
                    int endIndex = count - (int)(maxBarCount * 0.25);
                    int startIndex = count - maxBarCount;
                    _stockChartX.FirstVisibleRecord = startIndex;
                    _stockChartX.LastVisibleRecord = endIndex;

                    _stockChartX.Update();
                    MessageBox.Show("Chart succesfully deserialized.");
                }
                else
                {
                    MessageBox.Show("Could not deserialize the data. Error " + res);
                }
            }
        }

        #endregion Serialization

        #endregion Chart Tools





        #region Other Tools

        /// <summary>
        /// Some settings to show how the style of some of the elements in the control can be set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAltStyle_Click(object sender, RoutedEventArgs e)
        {
            foreach (var chartPanel in _stockChartX.PanelsCollection)
            {
                chartPanel.Background = Brushes.White;
                chartPanel.TitleBarBackground
                  = new LinearGradientBrush
                  {
                      StartPoint = new Point(0.5, 0),
                      EndPoint = new Point(0.5, 1),
                      GradientStops = new GradientStopCollection
                                  {
                                    new GradientStop {Color = Color.FromArgb(0xFF, 0xD6, 0xD6, 0xD6), Offset = 0},
                                    new GradientStop {Color = Colors.White, Offset = 0.5},
                                    new GradientStop {Color = Color.FromArgb(0xFF, 0xD6, 0xD6, 0xD6), Offset = 1},
                                  }
                  };
                chartPanel.YAxesBackground = Brushes.White;
                chartPanel.TitleBarButtonForeground = Brushes.Black;
            }

            _stockChartX[SeriesTypeOHLC.Close].StrokeColor = Color.FromArgb(0xFF, 0x00, 0x80, 0x00);
            _stockChartX[SeriesTypeOHLC.Close].TitleBrush = Brushes.Green;
            _stockChartX[SeriesTypeOHLC.Close].TickBox = TickBoxPosition.Right;

            _stockChartX.FontForeground = Brushes.Black;
            _stockChartX.CalendarBackground = Brushes.White;
            _stockChartX.XGrid = true;
            _stockChartX.InfoPanelPosition = InfoPanelPositionEnum.FixedPosition;
            _stockChartX.InfoPanelValuesBackground = Brushes.Transparent;
            _stockChartX.InfoPanelValuesForeground = Brushes.Black;
            _stockChartX.InfoPanelLabelsBackground = Brushes.Transparent;
            _stockChartX.InfoPanelLabelsForeground = Brushes.Black;
            _stockChartX.InfoPanelFontSize = 12;
            _stockChartX.Background = Brushes.White;

            _stockChartX.Update();
        }

        /// <summary>
        /// Playing with the style of the InfoPanel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStyleInfoPanel_Click(object sender, RoutedEventArgs e)
        {
            _stockChartX.InfoPanelPosition = InfoPanelPositionEnum.FixedPosition;
            _stockChartX.InfoPanelValuesBackground = Brushes.Transparent;
            _stockChartX.InfoPanelValuesForeground = Brushes.Red;
            _stockChartX.InfoPanelLabelsBackground = Brushes.Transparent;
            _stockChartX.InfoPanelLabelsForeground = Brushes.Red;
            _stockChartX.InfoPanelFontSize = 12;
        }

        /// <summary>
        /// Code for Support Ticket 4348. User wanted to do some tests with a small dataset on the chart.
        /// This code simply changes the amount of data that will be loaded into the chart and then completely
        /// refreshes the chart on the screen. It will delete all of the users actions such as indicators
        /// and line studies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSmallDataSetChart_Click(object sender, RoutedEventArgs e)
        {
            _barCount = 10;
            CreateChart();
        }

        /// <summary>
        /// Code for Support Ticket 4396. This bit of code will wipe out the chart and replace it with one that
        /// only has a single line for the close data of a ticker.
        /// Also, it sets the scroll of the chart to the max so that the user does not have to scroll to see all
        /// of the data on the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseLineChart_Click(object sender, RoutedEventArgs e)
        {
            _stockChartX.ClearAll();
            _stockChartX.Symbol = RandomSymbol;

            ChartPanel topPanel = _stockChartX.AddChartPanel();
            ChartPanel volumePanel = _stockChartX.AddChartPanel(ChartPanel.PositionType.AlwaysBottom);

            topPanel.Background = volumePanel.Background = _chartPanelsBrush;

            Series seriesClose = _stockChartX.AddSeries(_stockChartX.Symbol, topPanel.Index);
            Series seriesVolume = _stockChartX.AddVolumeSeries(_stockChartX.Symbol, volumePanel.Index);

            ManualResetEvent eventHistory = new ManualResetEvent(false);
            List<BarData> data = new List<BarData>();
            _dataFeed.GetHistory(new HistoryRequest
            {
                BarCount = _barCount,
                BarSize = 1,
                Periodicity = Periodicity.Minutely,
                Symbol = _stockChartX.Symbol
            }, datas =>
            {
                data.AddRange(datas);
                eventHistory.Set();
            });
            eventHistory.WaitOne();

            for (int row = 0; row < data.Count; row++)
            {
                BarData bd = data[row];
                _stockChartX.AppendValue(_stockChartX.Symbol, bd.TradeDate, bd.ClosePrice);
                _stockChartX.AppendVolumeValue(_stockChartX.Symbol, bd.TradeDate, bd.Volume);
            }

            _stockChartX.SetPanelHeight(0, _stockChartX.ActualHeight * 0.75);

            _stockChartX.FirstVisibleRecord = 0;
            _stockChartX.LastVisibleRecord = _stockChartX.RecordCount - 1;

            _stockChartX.ChartScrollerProperties.IsVisible = false;
            _stockChartX.KeepZoomLevel = true;

            _stockChartX.Update();
        }


        /// <summary>
        /// The user wanted to be able to apply a set of indicators from one chart and apply it to another one that is 
        /// newly created. There are some tricks involved in making this work, but this is the main aspect.
        /// Store all of the information that you think that you are going to need and then set the data back into 
        /// a new chart. You must take into account places where the parameters may no longer be valid, and you need
        /// to know some layout information about the chart before you start.
        /// It is also logical that the application should store all of the users's style settings too so that they are not
        /// reset to defaults when this code is executed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4132_Click(object sender, RoutedEventArgs e)
        {
            // Store all of the information about the current indicators
            ////////////////////////////////////////////////////////////

            // Start with the number of panels so that they can be re-created.
            int panelCount = _stockChartX.PanelsCollection.Count();

            List<Ticket4132Helper> data = new List<Ticket4132Helper>();
            foreach (Indicator indicator in _stockChartX.IndicatorsCollection)
            {
                // We do not want or need the twin indicators (indicators that have more than one line)
                if (indicator.IsTwin) continue;

                // Grab the basic information, Add more to this if you have the need.
                // The class is defined below
                Ticket4132Helper d = new Ticket4132Helper();
                d.IndType = indicator.IndicatorType;
                d.Name = indicator.FullName;
                d.PanelIndex = indicator.Panel.Index;

                // Loop through the 'official' list of indicators in the system.
                int i = 0;
                foreach (var parameter in StockChartX_IndicatorsParameters.GetIndicatorParameters(indicator.IndicatorType))
                {
                    switch (parameter.ParameterType)
                    {
                        // The ptSource and ptVolume parameter types reference other series. Therefore when you store or re-load them
                        // you might want to use some additional magic/logic OR defaults so that things do not break. I'm just going to use
                        // the assumption (though it could be false and wrong) that the symbol.series is the format being used and I will
                        // just capture the series name... This might not be logical in your implementation and could break things.
                        case ParameterType.ptSource:
                        case ParameterType.ptVolume:
                            string s = (string)indicator.GetParameterValue(i);
                            s = s.Replace(_stockChartX.Symbol, "");
                            d.Params.Add(s);
                            break;
                        case ParameterType.ptSymbol:
                            d.Params.Add("");
                            break;
                        default:
                            // For everything else, just store the value
                            d.Params.Add(indicator.GetParameterValue(i));
                            break;
                    }
                    i++;
                }
                data.Add(d);
            }

            // Clear out the chart and get a new one to prove it's all working.
            CreateChart();

            // Throw the indicators back onto the chart
            ///////////////////////////////////////////

            // First, set up the extra panels that were there before. Your implementation might do this 
            // differently, but this is a simple way to do things.
            for (int i = _stockChartX.PanelsCollection.Count(); i < panelCount; i++)
                _stockChartX.AddChartPanel();

            foreach (Ticket4132Helper d in data)
            {
                ChartPanel panel = _stockChartX.GetPanelByIndex(d.PanelIndex);

                // Create the indicator (note that the boolean does not have any affect)
                Indicator indicator = _stockChartX.AddIndicator(d.IndType, d.Name, panel, false);

                // Loop through the parameters again and set up the new probably vaild values.
                // Just having the parameters defined will stop the window from poping.
                int i = 0;
                foreach (var parameter in StockChartX_IndicatorsParameters.GetIndicatorParameters(indicator.IndicatorType))
                {
                    switch (parameter.ParameterType)
                    {
                        case ParameterType.ptSource:
                        case ParameterType.ptVolume:
                        case ParameterType.ptSymbol:
                            string s = _stockChartX.Symbol + (string)d.Params[i];
                            indicator.SetParameterValue(i, s);
                            break;
                        default:
                            indicator.SetParameterValue(i, d.Params[i]);
                            break;
                    }
                    i++;
                }
            }
        }
        internal class Ticket4132Helper
        {
            public IndicatorType IndType { get; set; }
            public string Name { get; set; }
            public int PanelIndex { get; set; }
            public List<object> Params = new List<object>();
        }

        /// <summary>
        /// Code for Support Ticket 4360. User wanted to know how to load lots of data into the chart at once.
        /// This loads four different lines into the chart at one time (using just the OHLC data from a standard random
        /// data set). The user could throw this into a loop and see how much they could load at once.
        /// The speed of loading data into a chart is not really an issue of the Silverlight application, it has more
        /// to do with the data access issues (where is the data coming from? and how fast can you get it here?).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4360_Click(object sender, RoutedEventArgs e)
        {
            // Change the current OHLC series to a line (Just to clean up the screen)
            Series open = _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".OPEN");
            Series high = _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".HIGH");
            Series low = _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".LOW");
            Series close = _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".CLOSE");
            open.SeriesType = SeriesTypeEnum.stLineChart;
            high.SeriesType = SeriesTypeEnum.stLineChart;
            low.SeriesType = SeriesTypeEnum.stLineChart;
            close.SeriesType = SeriesTypeEnum.stLineChart;

            // Create some random series to hold a line
            string randomStmbol1 = RandomSymbol;
            string randomStmbol2 = RandomSymbol;
            string randomStmbol3 = RandomSymbol;
            string randomStmbol4 = RandomSymbol;
            _stockChartX.AddSeries(randomStmbol1, 0);
            _stockChartX.AddSeries(randomStmbol2, 0);
            _stockChartX.AddSeries(randomStmbol3, 0);
            _stockChartX.AddSeries(randomStmbol4, 0);

            // Use the standard data creation, a whack load of random data.
            ManualResetEvent eventHistory = new ManualResetEvent(false);
            List<BarData> data = new List<BarData>();
            _dataFeed.GetHistory(new HistoryRequest
            {
                BarCount = _stockChartX.RecordCount,
                BarSize = 1,
                Periodicity = Periodicity.Minutely,
                Symbol = randomStmbol1
            }, datas =>
            {
                data.AddRange(datas);
                eventHistory.Set();
            });
            eventHistory.WaitOne();

            for (int row = 0; row < data.Count; row++)
            {
                BarData bd = data[row];
                _stockChartX.AppendValue(randomStmbol1, _stockChartX.GetTimestampByIndex(row).Value, bd.OpenPrice);
                _stockChartX.AppendValue(randomStmbol2, _stockChartX.GetTimestampByIndex(row).Value, bd.HighPrice);
                _stockChartX.AppendValue(randomStmbol3, _stockChartX.GetTimestampByIndex(row).Value, bd.LowPrice);
                _stockChartX.AppendValue(randomStmbol4, _stockChartX.GetTimestampByIndex(row).Value, bd.ClosePrice);
            }

            _stockChartX.Update();
        }

        /// <summary>
        /// Little demo for ticket 4489 to allow the user to see the current y and x values of the crosshairs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4489_Click(object sender, RoutedEventArgs e)
        {
            Point p = CrossHairPosition;
            if (p.X == 0)
                p.X = 200;
            if (p.Y == 0)
                p.Y = 200;

            p.X = (int)p.X / 2;
            p.Y = (int)p.Y / 2;
            CrossHairPosition = p;
        }
        public string Ticket4489Content
        {
            get
            {
                return CrossHairPosition.X.ToString() + " - " + CrossHairPosition.Y.ToString();
            }
        }

        /// <summary>
        /// User wanted to be able to have better control over the way that indicators that have multiple
        /// lines are handled. These indicators actually have several series. The extra series are called
        /// 'twin' series (as a general term). The client wanted to know how to find the twin series, which
        ///  was not actually all that easy. A new feature has been added to facilitate this discovery.
        ///  ALSO, the user wanted to have only the main indicator show up on the title bar at the top 
        ///  of the panel. Again, a new feature (dependancy propery) has been added that will allow the
        ///  client to turn off the twin's headings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4541_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the IndicatorTwinTitleVisibility variable.
            _stockChartX.IndicatorTwinTitleVisibility = _stockChartX.IndicatorTwinTitleVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

            // Getting a single panel (assuming you know it's number)
            ChartPanel chartPanel = _stockChartX[0];
            Debug.WriteLine("ChartPanel: " + chartPanel.ToString());

            // Looping throught the panels (uses IEnumerator so use your favourite tool)
            foreach (ChartPanel p in _stockChartX.PanelsCollection)
            {
                Debug.WriteLine("ChartPanel: " + p.ToString());
            }

            // Now to take a look through the series on the chart.
            foreach (Series s in _stockChartX.SeriesCollection)
            {
                Debug.WriteLine("Series: " + s.ToString());
            }

            // Or the series on a specific Panel
            foreach (Series s in _stockChartX[0].SeriesCollection)
            {
                Debug.WriteLine("Series: " + s.ToString());
            }

            // Same deal with Indicators (which do not show up on the Series list)
            foreach (Indicator i in _stockChartX.IndicatorsCollection)
            {
                Debug.WriteLine("Indicator: " + i.ToString());
            }
            foreach (Indicator i in _stockChartX[0].IndicatorsCollection)
            {
                Debug.WriteLine("Indicator: " + i.ToString());
            }

            // Though the real question here was how to find out about which indicators 'belong' to 
            // other ones. This is when indicators like BollingerBands are being used as the 
            // bollinger band adds three series to the panel. Strictly speaking the way that this
            // is done is through matching the 'Name' attribute. Therefore, as long as you know
            // the name (and sub names) of each of the parts of the indicator you can go directly
            // to the indicator series in question (this is in fact how things are done 'under the 
            // hood'). BUT the main issue here for the average developer is knowing which series
            // are the correct ones when there is a slight difference in the name, and for the 
            // developer to know all of the subtle differences in naming (though you can get the
            // names from the title bar in the panel).
            // A new feature has been added to help with this... There is a concept of 'Twin' 
            // Indicators in the system. These are the indicators that are subordinate to the
            // main indicator (therefore Bollinger bands has two twins). There was always a 'IsTwin'
            // property, but there is also a 'TwinsParentIndicator' that has now been added which
            // will make it easier to be sure that you have the correct indicator objects.

            List<Indicator> indicators = new List<Indicator>();

            // Probably a good way to get all of the indicators would be to know the name
            // of the main indicator and go and get just it, and of course it's 'children'.
            // Here would be a good way of doing that.
            // Of course this assumes that you know the programed names of the children for
            // each of the different types of indicators (which is a bit of a pain).
            string knownIndicatorName = "Bollinger Bands"; // known name
            indicators.Add((Indicator)_stockChartX.GetSeriesByName(knownIndicatorName));
            indicators.Add((Indicator)_stockChartX.GetSeriesByName(knownIndicatorName + " Top"));
            indicators.Add((Indicator)_stockChartX.GetSeriesByName(knownIndicatorName + " Bottom"));

            // Here is another way to get all of the indicators (this assumes that you already know
            // all of the names of the indicators you have added, which is true as it is a 
            // string you had to supply when you created the indicator) (NOTE: This particular
            // test will only return valid data if you have already added a Bollinger Indicator
            // before running test)
            indicators.Clear();
            foreach (Indicator i in _stockChartX.IndicatorsCollection)
            {
                if (i.Name.Contains(knownIndicatorName))
                    indicators.Add(i);
            }

            // Of course the above method of looping is going to have some problems if you
            // have one indicator named BOB and the other one BOBS as all of the indicator's
            // series will be included in the .Contains check. Therefore the newly added
            // method to check for parent will allow you to get everything done.
            // This is the suggested method to get all of the series for a particular
            // indicator. The developer should still know the main name of the indicator
            // to make this smoother, but we can be sure we get the correct children.
            indicators.Clear();
            indicators.Add((Indicator)_stockChartX.GetSeriesByName(knownIndicatorName));
            foreach (Indicator i in _stockChartX.IndicatorsCollection)
            {
                if (i.IsTwin && i.TwinsParentIndicator == indicators[0])
                    indicators.Add(i);
            }

        }


        /// <summary>
        /// The user was experiencing some problems with the different styles of a series
        /// on the main chart. This exercises many of the different styles of a new series 
        /// that has been added to the chart (the new series is a simple line that just 
        /// changes the OPEN data a bit so that there is something on the screen).
        /// As a note, this is also a good example of how to add a simple line into the
        /// chart using the current timestamps.
        /// If the user continues to click the button, different things will happen on the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4601_Click(object sender, RoutedEventArgs e)
        {
            _stockChartX.Update();

            switch (counter4601)
            {
                case 0:
                    lineSeries = _stockChartX.AddSeries("Ticket4601", 0);
                    lineSeries.StrokeColor = Colors.Orange;
                    lineSeries.StrokePattern = LinePattern.Dot;
                    lineSeries.StrokeThickness = 2.0;
                    lineSeries.Opacity = 0.5;

                    Series series = _stockChartX.SeriesCollection.First();
                    for (int i = 0; i < _stockChartX.RecordCount; i++)
                    {
                        _stockChartX.AppendValue("Ticket4601", series[i].TimeStamp, series[i].Value * 1.1);
                    }
                    break;
                case 1:
                    lineSeries.Opacity = 0.9;
                    break;
                case 2:
                    lineSeries.StrokePattern = LinePattern.Dash;
                    break;
                case 3:
                    lineSeries.StrokeThickness = 3.0;
                    break;
                case 4:
                    lineSeries.StrokeColor = Colors.Purple;
                    break;
                default:
                    _stockChartX.RemoveSeries(lineSeries);
                    counter4601 = -1;
                    break;
            }
            _stockChartX.Update();
            counter4601++;
        }
        int counter4601 = 0;
        private Series lineSeries;

        /// <summary>
        /// There is a new calendar in town. This might not be something that all users will want
        /// but the new version of the calendar allows the user to play with the display settings 
        /// of a given calendar, and it will also change the way that the data displays on the 
        /// screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalendarV2_Click(object sender, RoutedEventArgs e)
        {
            switch (counterCalendarV2)
            {
                case 0:
                    _stockChartX.CalendarVersion = CalendarVersionType.Version2;
                    break;
                case 1:
                    _stockChartX.CalendarV2LabelGap = 10;
                    break;
                case 2:
                    _stockChartX.CalendarV2LabelGap = 1;
                    break;
                case 3:
                    _stockChartX.CalendarV2CalendarLabelBlockOutput = CalendarLabelBlockOutputType.FirstValid;
                    break;
                case 4:
                    _stockChartX.CalendarV2CurrentTimeStamp = false;
                    break;
                case 5:
                    List<CalendarScaleData> scales = _stockChartX.CalendarV2CalendarScaleDataList;
                    foreach (CalendarScaleData scale in scales)
                    {
                        // The user can play with these values here as much as they like.
                        // Though unless you have a good idea of how they are structured, and how they
                        // work, it's probably not the best idea. The one thing that a user might want
                        // to change is the output format values. 
                        // WARNING. If you do play with these values make sure that they are ordered from
                        // smallest scale to largest (1 second to 1 year sort of thing). This is w.r.t. the
                        // first two values (scale and scale type). The algorythm needs to have them in order
                        // to display the values properly.

                        // Just for interest's sake. The data is going to be thrown to stdout.
                        Debug.WriteLine("=====================================================================");
                        Debug.WriteLine("Scale                        : " + scale.Scale);
                        Debug.WriteLine("ScaleType                    : " + scale.ScaleType);
                        Debug.WriteLine("OutputFormat                 : " + scale.OutputFormat);
                        Debug.WriteLine("HorozontalAlign              : " + scale.HorozontalAlign);
                        Debug.WriteLine("SecondaryScale               : " + scale.SecondaryScale);
                        Debug.WriteLine("SecondaryScaleType           : " + scale.SecondaryScaleType);
                        Debug.WriteLine("SecondaryOutputFormat        : " + scale.SecondaryOutputFormat);
                        Debug.WriteLine("SecondaryHorozontalAlign     : " + scale.SecondaryHorozontalAlign);
                        Debug.WriteLine("CurrentTimeStampOutputFormat : " + scale.CurrentTimeStampOutputFormat);
                        Debug.WriteLine("=====================================================================");
                    }
                    break;
                default:
                    _stockChartX.CalendarVersion = CalendarVersionType.Version1;
                    _stockChartX.CalendarV2LabelGap = 3;
                    _stockChartX.CalendarV2CalendarLabelBlockOutput = CalendarLabelBlockOutputType.Beginning;
                    _stockChartX.CalendarV2CurrentTimeStamp = true;
                    counterCalendarV2 = -1;
                    break;
            }
            counterCalendarV2++;

        }
        int counterCalendarV2 = 0;


        /// <summary>
        /// The user wanted to be able to play with the text that shows up on the chart. First of all with a horizontal line and then with a
        /// simple bit of text that will show up on the screen. The first of these two is rather complicated, but the later is not bad at all.
        /// The user wanted to be able to float some text on top of the horizontal line 100px from the right of the chart.
        /// The user also wanted to float some text on the chart 100px from the top.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4560_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();

            switch (counter4560)
            {
                case 0:
                    // Simple case to show that we can play around with the horizontal line value presenter. The Horizontal line is actually
                    // one of the few elements in the control that allow you to play with the template. The basic rule with playing with 
                    // templates is that they 'should' conform to the same structure. This can be ignored, but probably should not be.
                    // To look at the base structure, check out the generic.xaml file in the source code.
                    _stockChartX.HorizontalLineValuePresenterTemplate = null;
                    lineStudy0 = (HorizontalLine)_stockChartX.AddLineStudy(LineStudy.StudyTypeEnum.HorizontalLine, rand.Next().ToString(), new SolidColorBrush(Colors.Purple), null);
                    lineStudy0.ValuePresenterAlignment = LineStudy.ValuePresenterAlignmentType.Left;
                    break;
                case 1:
                    // Here the default (null) template is replaced with the simple version from the resources. This version is basically 
                    // a hard coded version of the template. The text will not be changeable. Simply set the template before the
                    // line study is added to the chart.
                    _stockChartX.HorizontalLineValuePresenterTemplate = (ControlTemplate)Resources["Ticket4560SimpleControlTemplate"];
                    lineStudy1 = (HorizontalLine)_stockChartX.AddLineStudy(LineStudy.StudyTypeEnum.HorizontalLine, rand.Next().ToString(), new SolidColorBrush(Colors.Green), null);
                    break;
                case 2:
                    // This is the more complicated version. It will allow the user to play with the template, but also allows the user to 
                    // make the content of the template to be changeable (databound). This is done by extending the internal 'Value Getter'.
                    // You should check out the base class, but if not, just look at Ticket4560LineStudyValueGetter which is the special
                    // case for this experiment. As a note, this is some more advanced Silverlight/WPF concepts here. You can change your
                    // class to match the template, but it was done this way to mimic the internal class.
                    // Be sure to set the instance of the class into the line study once it is created.
                    _stockChartX.HorizontalLineValuePresenterTemplate = (ControlTemplate)Resources["Ticket4560DataBoundControlTemplate"];
                    lineStudy2 = (HorizontalLine)_stockChartX.AddLineStudy(LineStudy.StudyTypeEnum.HorizontalLine, rand.Next().ToString(), new SolidColorBrush(Colors.Orange), null);
                    lineStudy2.ValuePresenterAlignment = LineStudy.ValuePresenterAlignmentType.Right;
                    valueGetter = new Ticket4560LineStudyValueGetter()
                    {
                        FontFamily = new FontFamily(_stockChartX.FontFace),
                        FontSize = _stockChartX.FontSize,
                        Foreground = _stockChartX.Foreground,
                    };
                    lineStudy2.LineStudyValue = valueGetter;
                    break;
                case 3:
                    // And here is the final case where the user is able to edit the value of the text. It is event driven and should update
                    // without the need of an Update().
                    valueGetter.Text = "Whatever...";
                    break;
                case 4:
                    // The other thing that the user wanted to do was have some static text on the chart. This can be acheived by playing with the
                    // ChartPanelLabel that is a part of every panel in the control. We then abuse the attached properties of the canvas of the label
                    // to set where is it located on the screen.
                    var lbl = _stockChartX.GetPanelByIndex(0).ChartPanelLabel;
                    Canvas.SetLeft(lbl, 20);
                    Canvas.SetTop(lbl, 100);
                    lbl.FontSize = 10;
                    lbl.Text = "Some Text...";
                    lbl.Foreground = new SolidColorBrush(Colors.Yellow);
                    lbl.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    // Reset everything back to basics.
                    _stockChartX.HorizontalLineValuePresenterTemplate = (ControlTemplate)Resources["HorLinesValueControlTemplate"];
                    _stockChartX.RemoveObject(lineStudy0);
                    _stockChartX.RemoveObject(lineStudy1);
                    _stockChartX.RemoveObject(lineStudy2);
                    _stockChartX.GetPanelByIndex(0).ChartPanelLabel.Visibility = System.Windows.Visibility.Collapsed;
                    counter4560 = -1;
                    break;
            }
            _stockChartX.Update();
            counter4560++;
        }
        int counter4560 = 0;
        HorizontalLine lineStudy0;
        HorizontalLine lineStudy1;
        HorizontalLine lineStudy2;
        Ticket4560LineStudyValueGetter valueGetter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4580_Click(object sender, RoutedEventArgs e)
        {
            switch (counter4580)
            {
                case 0:
                    _stockChartX.ScalingType = ScalingTypeEnum.Semilog;
                    _stockChartX.Update();
                    break;
                case 1:
                    _stockChartX.ClearAll();
                    _stockChartX.Symbol = RandomSymbol;

                    ChartPanel topPanel = _stockChartX.AddChartPanel();
                    ChartPanel volumePanel = _stockChartX.AddChartPanel(ChartPanel.PositionType.AlwaysBottom);

                    Series[] ohlcSeries = _stockChartX.AddOHLCSeries(_stockChartX.Symbol, topPanel.Index);
                    Series seriesVolume = _stockChartX.AddVolumeSeries(_stockChartX.Symbol, volumePanel.Index);

                    int endIndex = 100;
                    DateTime dt = DateTime.Now;
                    Random rand = new Random();
                    double bump = 0;
                    for (int i = 2; i < endIndex; i++)
                    {
                        double open = Math.Pow(1.07, i) + bump;
                        double high = open + rand.NextDouble();
                        double low = open - rand.NextDouble();
                        _stockChartX.AppendOHLCValues(_stockChartX.Symbol, dt, open, high, low, open);
                        _stockChartX.AppendVolumeValue(_stockChartX.Symbol, dt, Math.Floor(rand.NextDouble() * 100));
                        dt = dt.AddMinutes(1);
                        if (i == 20)
                            bump = 1;
                        else if (i > 20 && i <= 25)
                            bump += bump;
                        else if (i > 25 && i < 30)
                            bump = bump - (bump / 2);
                        else
                            bump = 0;
                    }
                    _stockChartX.ScalingType = ScalingTypeEnum.Linear;
                    _stockChartX.OptimizePainting = true;
                    _stockChartX.Update();
                    break;
                case 2:
                    _stockChartX.ScalingType = ScalingTypeEnum.Semilog;
                    _stockChartX.Update();
                    break;
                default:
                    _stockChartX.ScalingType = ScalingTypeEnum.Linear;
                    CreateChart();
                    counter4580 = -1;
                    break;
            }
            counter4580++;
        }
        int counter4580 = 0;


        /// <summary>
        /// User wants to be able to play around with the settings for the candles. There is a lot going on 
        /// in this section of code and I am going to try to define all of the possibilities here.
        /// Also, it is important that these settings may not work on the other display styles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4638_Click(object sender, RoutedEventArgs e)
        {
            // The user supplied me with the followin bit of code.
            // The intention was to have hollow candles.
            if (counter4638 == int.MinValue) // just want to show this code but not run it
            {
                _ohlcSeries[OPEN].StrokeColor = Colors.White;
                _ohlcSeries[HIGH].StrokeColor = Colors.White;
                _ohlcSeries[LOW].StrokeColor = Colors.White;
                _ohlcSeries[CLOSE].StrokeColor = Colors.White;
                _ohlcSeries[CLOSE].UpColor = Colors.Black;
                _ohlcSeries[CLOSE].DownColor = Colors.Black;
                _stockChartX.CandleDownOutlineColor = Colors.White;
                _stockChartX.CandleUpOutlineColor = Colors.White;
            }

            switch (counter4638)
            {
                case 0:
                    // The first thing to be aware of... Is that the application uses the CLOSE series as the basis of most settings
                    // when dealing with the candle styles. Therefore the follow lines of code have no meaning at all.
                    _ohlcSeries[OPEN].StrokeColor = Colors.White;
                    _ohlcSeries[HIGH].StrokeColor = Colors.White;
                    _ohlcSeries[LOW].StrokeColor = Colors.White;
                    // But in the case of the Candles the StrokeColor is also not used. So the above lines of code are doubly meaningless
                    // and the line below, though it is the right series, is still not used.
                    _ohlcSeries[CLOSE].StrokeColor = Colors.White;
                    break;
                case 1:
                    // The next thing for the developer to know is the precidence of variables.
                    // There are variables called UpColor and DownColor both in the chart and in each series.
                    // These values are somewhat hierarchical. Therefore a change at the chart level will change
                    // all candles, and then a change on the CLOSE will change the OHLC series group.
                    _stockChartX.UpColor = Colors.Yellow;
                    _stockChartX.DownColor = Colors.Yellow;
                    break;
                case 2:
                    // The user wanted the series to be hollow candles so I will set them to transparent.
                    // NOTE: The chart color of Yellow at the chart level is being overwritten here to give you nothing at all.
                    // At this point the chart is going to look mighty strange, as it is going to be basically invisible.
                    // NOTE: Unless there is a good reason, I would use the chart level varibles not the series level, but it
                    // is being shown here for completeness
                    _ohlcSeries[CLOSE].UpColor = Colors.Transparent;
                    _ohlcSeries[CLOSE].DownColor = Colors.Transparent;
                    break;
                case 3:
                    // Now the Candle Up and Down outline colors need to be turned on.
                    // Again, the chart is not going to look quite right, but we will get to that...
                    _stockChartX.CandleDownOutlineColor = Colors.Yellow;
                    _stockChartX.CandleUpOutlineColor = Colors.Yellow;
                    break;
                case 4:
                    // The problem that we have now is that the wicks are not showing up on the screen (or they are
                    // but they are there as transparent). Therefore we need to match the color of the wick to the
                    // outline and not to the fill.
                    _stockChartX.CandleDownWickMatchesOutlineColor = true;
                    _stockChartX.CandleUpWickMatchesOutlineColor = true;
                    break;
                case 5:
                    // So... As a nice looking example putting everything together, here is one that will use light and dark for 
                    // up and down, basically using a monocrome style layout.
                    CreateChart();
                    _stockChartX.DownColor = Colors.Transparent;
                    _stockChartX.CandleDownOutlineColor = Colors.White;
                    _stockChartX.CandleDownWickMatchesOutlineColor = true;

                    _stockChartX.UpColor = Colors.White;
                    _stockChartX.CandleUpOutlineColor = null;
                    _stockChartX.CandleUpWickMatchesOutlineColor = false;
                    break;
                case 6:
                    // LETS START ALL OVER AGAIN.
                    // -------------------------
                    // There is another method for playing with the styles of the candles. These methods allow for a lot more
                    // customization, but it is also a bit more complicated. This is the 'Use Enhanced Coloring' section.
                    // First I am going reset everything...
                    CreateChart();
                    _stockChartX.UpColor = ColorsEx.Lime;
                    _stockChartX.DownColor = Colors.Red;
                    _stockChartX.CandleDownOutlineColor = null;
                    _stockChartX.CandleUpOutlineColor = null;
                    _stockChartX.CandleDownWickMatchesOutlineColor = false;
                    _stockChartX.CandleUpWickMatchesOutlineColor = false;
                    break;
                case 7:
                    // As noted before, everything is done on the CLOSE series for this... Start with setting
                    // the UseEnhancedColoring variable to true;
                    _ohlcSeries[CLOSE].UseEnhancedColoring = true;

                    // Now, a bunch of new setting are available to you...
                    //
                    // WickUpStroke
                    // WickDownStroke
                    // WickStrokeThickness
                    // CandleUpFill
                    // CandleDownFill
                    // CandleUpStroke
                    // CandleDownStroke
                    // CandleStrokeThickness
                    //
                    // The Thickness values are doubles and should be obvious what they can do
                    // The Stroke and Fill values are all Brushes, so you have a lot that you can do with them
                    // from a style point of view. Play around with their fades, their types and all of that.
                    //
                    // Though as a note, it is important that some values here would make the chart work in very
                    // strange ways (especially playing with the thickness values.
                    //
                    // I am only going to reproduce somthing that has hollow candles.

                    _ohlcSeries[CLOSE].WickUpStroke = new SolidColorBrush(Colors.White);
                    _ohlcSeries[CLOSE].WickDownStroke = new SolidColorBrush(Colors.White);
                    _ohlcSeries[CLOSE].WickStrokeThickness = 1;

                    _ohlcSeries[CLOSE].CandleUpFill = new SolidColorBrush(Colors.White);
                    _ohlcSeries[CLOSE].CandleDownFill = new SolidColorBrush(Colors.Transparent);
                    _ohlcSeries[CLOSE].CandleUpStroke = new SolidColorBrush(Colors.White);
                    _ohlcSeries[CLOSE].CandleDownStroke = new SolidColorBrush(Colors.White);
                    _ohlcSeries[CLOSE].CandleStrokeThickness = 0;
                    break;
                default:
                    // Put everything back to normal.
                    CreateChart();
                    _stockChartX.UpColor = ColorsEx.Lime;
                    _stockChartX.DownColor = Colors.Red;
                    _stockChartX.CandleDownOutlineColor = null;
                    _stockChartX.CandleUpOutlineColor = null;
                    _stockChartX.CandleDownWickMatchesOutlineColor = false;
                    _stockChartX.CandleUpWickMatchesOutlineColor = false;
                    counter4638 = -1;
                    break;
            }
            _stockChartX.Update();
            counter4638++;
        }
        int counter4638 = 0;

        /// <summary>
        /// User wanted to be able to hide a particular panel. The request was that there should be 
        /// a dependancy property, but that is not viable with the current structure of the application
        /// but a user should be able to control everything that they need with the following tools.
        /// Additional comments are below about what is happening in each step and why.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTicket4679_Click(object sender, RoutedEventArgs e)
        {
            switch (index4679)
            {
                case 0:
                    /// The user found that the Collapsed setting on the visibility did not return expected results.
                    /// What is happening here is actually exactly as expected when you are aware that the panels are
                    /// not 'floating' elements. Each panel's location is tightly controled by the panelContainer.
                    /// Therefore hiding the panel does just that, but it does not affect any of the other panels 
                    /// in the panelContainer.
                    _volumePanel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 1:
                    /// Bring back the panel so that we can work with it
                    _volumePanel.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 2:
                    /// A new Dependency Property was added to the StockChartX class so that the user is now able
                    /// to control the PanelMinHeight. There is a setting for the system that stops the panels from
                    /// getting too small. Both for visibility issues and because some strange things happen when 
                    /// a panel is shrunk to nothing. But the user wanted to make the Volume panel go away and this
                    /// is a good way to do that. It is important to set the PanelMinHeight back to a viable number
                    /// after setting the panel height. Having a 0 value in that field during normal operations
                    /// is not a good idea (possible, but not suggested)
                    double temp = _stockChartX.PanelMinHeight;
                    _stockChartX.PanelMinHeight = 0;
                    _stockChartX.SetPanelHeight(_volumePanel.Index, 0);
                    _stockChartX.PanelMinHeight = temp;
                    break;
                case 3:
                    /// Returning things to normal
                    _stockChartX.SetPanelHeight(_volumePanel.Index, 150);
                    break;
                case 4:
                    /// Another new set of tools that have been added is the ability for the client to control some
                    /// of the state of the panel from the code. Therefore the Restore, Minimize, and Maximize
                    /// methods have been added to the panel class.
                    /// NOTE: You can not go from Min to Max or Max to Min. Nothing will happen. You can use the 
                    /// new State property to find the current state of the panel.
                    _volumePanel.Minimize();
                    break;
                case 5:
                    _volumePanel.Restore();
                    break;
                case 6:
                    _volumePanel.Maximize();
                    break;
                case 7:
                    _volumePanel.Restore();
                    break;
                default:
                    index4679 = -1;
                    CreateChart();
                    break;
            }
            _stockChartX.Update();
            index4679++;
        }
        private int index4679 = 0;

        #endregion Other Tools

        private void colorPicker_ColorSelected(Color c)
        {

        }

        #endregion Side Bar Elements
    }
}
