using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.Models;
using ModulusFE.PaintObjects;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    /// <summary>
    /// Standard type of series. Usually used in a group of OHLC series.
    /// </summary>
    public sealed partial class Standard : Series
    {
        private readonly PaintObjectsManager<PaintObjects.Line> _lines = new PaintObjectsManager<PaintObjects.Line>();

        private bool _subscribedToBarBrush;
        private bool _volumeUpDown;
        private bool _isOscillator;
        private Series _seriesClose;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Unique series name. </param>
        /// <param name="seriesType">Series Type</param>
        /// <param name="seriesTypeOHLC">Series OHLC type</param>
        /// <param name="chartPanel">Reference to a chart panel where it will be placed.</param>
        public Standard(string name, SeriesTypeEnum seriesType, SeriesTypeOHLC seriesTypeOHLC,
                        ChartPanel chartPanel)
            : base(name, seriesType, seriesTypeOHLC, chartPanel)
        {
            Init();
        }

        internal override void UnSubscribe()
        {
            base.UnSubscribe();

            if (_subscribedToBarBrush)
                _chartPanel._chartX.OnCandleCustomBrush -= ChartXOnCandlCustomBrush;
        }

        /// <summary>
        /// Force the series to be painted as an oscilator (histogram)
        /// </summary>
        public bool ForceOscilatorPaint { get; set; }

        ///<summary>
        /// Forces the series to be painted as linear, <see cref="ForceOscilatorPaint"/> must be set to false
        ///</summary>
        public bool ForceLinearPaint { get; set; }

        internal override void Paint()
        {
            if (Painted || !_visible || RecordCount < 0 || RecordCount < _chartPanel._chartX._startIndex) return;
            Painted = true;

            InitPainting();

            if (_chartPanel._chartX.OptimizePainting)
            {
                PaintOptimized();
                return;
            }

            Brush strokeUpBrush = new SolidColorBrush(UpColor == null ? _chartPanel._chartX.UpColor : UpColor.Value);
            strokeUpBrush.Freeze();
            Brush strokeDownBrush = new SolidColorBrush(DownColor == null ? _chartPanel._chartX.DownColor : DownColor.Value);
            strokeDownBrush.Freeze();
            Brush strokeNormalBrush = new SolidColorBrush(_strokeColor);
            strokeNormalBrush.Freeze();
            Brush currentBrush = strokeNormalBrush;

            _lines.C = _chartPanel._rootCanvas;
            _lines.Start();

            double x2 = _chartPanel._chartX.GetXPixel(0);
            double? y1;
            double? y2 = null;

            int cnt = 0;
            for (int i = _chartPanel._chartX._startIndex; i < _chartPanel._chartX._endIndex; i++, cnt++)
            {
                //cnt++;
                double x1 = _chartPanel._chartX.GetXPixel(cnt);
                y1 = this[i].Value;
                if (!y1.HasValue) continue;
                y1 = GetY(y1.Value);
                if (i == _chartPanel._chartX._startIndex)
                    y2 = y1.Value;

                if (_volumeUpDown)
                {
                    if (i > 0)
                    {
                        if (_seriesClose[i].Value > _seriesClose[i - 1].Value)
                            currentBrush = _seriesTypeOHLC == SeriesTypeOHLC.Volume ? strokeUpBrush : strokeDownBrush;  //up
                        else if (_seriesClose[i].Value < _seriesClose[i - 1].Value)
                            currentBrush = _seriesTypeOHLC == SeriesTypeOHLC.Volume ? strokeDownBrush : strokeUpBrush; //down
                        else
                            currentBrush = strokeNormalBrush;
                    }
                    else
                        currentBrush = strokeNormalBrush;
                }
                else if ((_chartPanel._chartX._useLineSeriesColors || _upColor.HasValue)) // +/- change colors
                {
                    if (!_isOscillator)
                    {
                        if (i > 0)
                        {
                            if (this[i].Value > this[i - 1].Value)
                                currentBrush = strokeUpBrush;
                            else currentBrush = this[i].Value < this[i - 1].Value ? strokeDownBrush : strokeNormalBrush;
                        }
                        else
                            currentBrush = strokeNormalBrush;
                    }
                    else
                    {
                        if (this[i].Value > 0)
                            currentBrush = strokeUpBrush;
                        else currentBrush = this[i].Value < 0 ? strokeDownBrush : strokeNormalBrush;
                    }
                }

                if (_seriesType == SeriesTypeEnum.stVolumeChart || ForceOscilatorPaint)
                {
                    if (this[i].Value.HasValue && y2.HasValue)
                    {
                        // Make sure at least 2 or 3 pixels show
                        // if the value is the same as the min Y.
                        double minY = GetY(SeriesEntry._min);
                        double nY1 = y1.Value;
                        if (minY - 3 < nY1)
                            nY1 -= 3;
                        DrawLine(x1, nY1, x1, _isOscillator ? GetY(0) : minY, _chartPanel._chartX.GetBarBrush(FullName, i, currentBrush));
                    }
                }
                else if ((_seriesType == SeriesTypeEnum.stLineChart || _seriesType == SeriesTypeEnum.stIndicator)
                  && i > _chartPanel._chartX._startIndex)
                {
                    if (this[i].Value.HasValue && y2.HasValue)
                        DrawLine(x1, y1.Value, x2, y2.Value, currentBrush);
                }

                y2 = y1;
                x2 = x1;
            }

            _lines.Stop();

            _lines.Do(l =>
                        {
                            l.ZIndex = ZIndexConstants.PriceStyles1;
                            if (l._line.Tag == null)
                                l._line.Tag = this;
                        });

            if (Selected)
                ShowSelection();
        }

        private Path _pathDown, _pathUp, _pathNormal;
        private void PaintOptimized()
        {
            SeriesStandardModel model
              = new SeriesStandardModel(_chartPanel._chartX._startIndex, _chartPanel._chartX._endIndex,
                _isOscillator, _seriesType, this, _volumeUpDown)
                  {
                      ForceOscilatorPaint = ForceOscilatorPaint,
                      CloseSeries = _seriesClose,
                      SeriesTypeOHLC = _seriesTypeOHLC
                  };

            Color upColor = UpColor == null ? _chartPanel._chartX.UpColor : UpColor.Value;
            Color downColor = DownColor == null ? _chartPanel._chartX.DownColor : DownColor.Value;
            Color normalColor = _strokeColor;

            if (_pathDown == null)
            {
                _pathDown = new Path { Tag = this };
                _chartPanel._rootCanvas.Children.Add(_pathDown);
                Canvas.SetZIndex(_pathDown, ZIndexConstants.PriceStyles1);

                _pathUp = new Path { Tag = this };
                _chartPanel._rootCanvas.Children.Add(_pathUp);
                Canvas.SetZIndex(_pathUp, ZIndexConstants.PriceStyles1);

                _pathNormal = new Path { Tag = this };
                _chartPanel._rootCanvas.Children.Add(_pathNormal);
                Canvas.SetZIndex(_pathNormal, ZIndexConstants.PriceStyles1);
            }

            _pathDown.StrokeThickness = _pathNormal.StrokeThickness = _pathUp.StrokeThickness = _strokeThickness;
            _pathDown.Opacity = _pathNormal.Opacity = _pathUp.Opacity = _opacity;
            Types.SetShapePattern(_pathDown, _strokePattern);
            Types.SetShapePattern(_pathNormal, _strokePattern);
            Types.SetShapePattern(_pathUp, _strokePattern);
            _pathDown.Stroke = new SolidColorBrush(downColor);
            _pathDown.Stroke.Freeze();
            _pathNormal.Stroke = new SolidColorBrush(normalColor);
            _pathNormal.Stroke.Freeze();
            _pathUp.Stroke = new SolidColorBrush(upColor);
            _pathUp.Stroke.Freeze();

            StringBuilder sbDown = new StringBuilder();
            StringBuilder sbUp = new StringBuilder();
            StringBuilder sbNormal = new StringBuilder();

            foreach (var value in model.Values)
            {
                switch (value.Brush)
                {
                    case SeriesStandardModel.BrushType.Up:
                        sbUp.AddDataPathLine(new Point(value.X1, value.Y1), new Point(value.X2, value.Y2));
                        break;
                    case SeriesStandardModel.BrushType.Normal:
                        sbNormal.AddDataPathLine(new Point(value.X1, value.Y1), new Point(value.X2, value.Y2));
                        break;
                    case SeriesStandardModel.BrushType.Down:
                        sbDown.AddDataPathLine(new Point(value.X1, value.Y1), new Point(value.X2, value.Y2));
                        break;
                }
            }

            _pathDown.Data = sbDown.GetDataPathGeometry();
            _pathUp.Data = sbUp.GetDataPathGeometry();
            _pathNormal.Data = sbNormal.GetDataPathGeometry();
        }

        private void InitPainting()
        {
            if (!_subscribedToBarBrush)
            {
                _subscribedToBarBrush = true;
                _chartPanel._chartX.OnCandleCustomBrush += ChartXOnCandlCustomBrush;
            }

            _volumeUpDown = _chartPanel._chartX._useVolumeUpDownColors && (_seriesTypeOHLC == SeriesTypeOHLC.Volume);
            _isOscillator = false;

            if (!ForceOscilatorPaint)
            {
                if (!ForceLinearPaint)
                {
                    for (int i = _chartPanel._chartX._startIndex; i < _chartPanel._chartX._endIndex; i++)
                    {
                        if (!this[i].Value.HasValue || this[i].Value >= 0.0) continue;
                        _isOscillator = true;
                        break;
                    }
                }
            }
            else
            {
                _isOscillator = true;
            }

            _seriesClose = _chartPanel._chartX.GetSeriesOHLCV(this, SeriesTypeOHLC.Close);
            if (_seriesClose == null)
                _volumeUpDown = false;
        }

        private void ChartXOnCandlCustomBrush(string seriesName, int index, Brush brush)
        {

        }

        internal override void SetStrokeColor()
        {
            Brush newBrush = new SolidColorBrush(_strokeColor);
            if (!_chartPanel._chartX.OptimizePainting)
            {
                _lines.Do(line => line.Stroke = newBrush);
            }
            else if (_pathDown != null)
            {
                _pathDown.Stroke = newBrush;
                _pathNormal.Stroke = newBrush;
                _pathUp.Stroke = newBrush;
            }
        }

        internal override void SetStrokeThickness()
        {
            if (!_chartPanel._chartX.OptimizePainting)
            {
                _lines.Do(line => line.StrokeThickness = _strokeThickness);
            }
            else if (_pathDown != null)
            {
                _pathDown.StrokeThickness = _pathUp.StrokeThickness = _pathNormal.StrokeThickness = _strokeThickness;
            }
        }

        internal override void SetOpacity()
        {
            if (!_chartPanel._chartX.OptimizePainting)
            {
                _lines.Do(line => line.Opacity = _opacity);
            }
            else if (_pathDown != null)
            {
                _pathDown.Opacity = _pathUp.Opacity = _pathNormal.Opacity = _opacity;
            }
        }

        internal override void SetStrokeType()
        {
            if (!_chartPanel._chartX.OptimizePainting)
            {
                _lines.Do(line => Types.SetShapePattern(line._line, _strokePattern));
            }
            else if (_pathDown != null)
            {
                Types.SetShapePattern(_pathDown, _strokePattern);
                Types.SetShapePattern(_pathNormal, _strokePattern);
                Types.SetShapePattern(_pathUp, _strokePattern);
            }
        }

        internal override void RemovePaint()
        {
            if (!_chartPanel._chartX.OptimizePainting)
            {
                _lines.RemoveAll();
            }
            else if (_pathDown != null)
            {
                Canvas c = _chartPanel._rootCanvas;
                c.Children.Remove(_pathUp);
                c.Children.Remove(_pathNormal);
                c.Children.Remove(_pathDown);

                _pathUp = _pathDown = _pathNormal = null;
            }
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush strokeBrush)
        {
            Utils.DrawLine(x1, y1, x2, y2, strokeBrush, _strokePattern, _strokeThickness, _opacity, _lines);
        }

        internal override void MoveToPanel(ChartPanel chartPanel)
        {
            RemovePaint();

            _chartPanel.DeleteSeries(this);
            _chartPanel = chartPanel;
            _chartPanel.AddSeries(this);

            base.MoveToPanel(chartPanel);
        }
    }
}
