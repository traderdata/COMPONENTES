using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.PaintObjects;
using ModulusFE.PriceStyles.Models;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE.PriceStyles
{
    internal partial class Candles : Style
    {
        internal Brush _downBrush;
        internal Brush _upBrush;
        internal Brush _candleDownOutlineBrush;
        internal Brush _candleUpOutlineBrush;

        private bool _subscribedToCustomBrush;

        private readonly PaintObjectsManager<Candle> _candles = new PaintObjectsManager<Candle>();

        public Candles(Series stock)
            : base(stock)
        {
        }

        private bool? _old3DStyle;
        private Color? _oldUpColor;
        private Color? _oldDownColor;
        private Color? _oldCandleDownOutline;
        private Color? _oldCandleUpOutline;

        private StockChartX ChartX;
        private double _halfwick;

        private Color _upColor;
        private Color _downColor;

        private void CalculateCandleSpacing()
        {
            ChartX = _series._chartPanel._chartX;
            ChartX._xMap = new double[ChartX._xCount = 0];

            int rcnt = ChartX.VisibleRecordCount;

            double x2 = ChartX.GetXPixel(rcnt);
            double x1 = ChartX.GetXPixel(rcnt - 1);
            double space = ((x2 - x1) / 2) - ChartX._barWidth / 2;

            if (space > 0.0)
            {
                if (space > 20) space = 20;
                space = space * 0.75; //25%
            }
            else
            {
                space = 1;
            }

            ChartX._barSpacing = space;

            double wick = ChartX._barWidth;
            _halfwick = wick / 2;
        }

        private bool? _oldOptimizePainting;
        public override bool Paint()
        {
            if (_series.OHLCType == SeriesTypeOHLC.Volume) return false;
            //Find Series
            Series open = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.Open);
            if (open == null || open.RecordCount == 0 || open.Painted) return false;
            Series high = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.High);
            if (high == null || high.RecordCount == 0 || high.Painted) return false;
            Series low = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.Low);
            if (low == null || low.RecordCount == 0 || low.Painted) return false;
            Series close = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.Close);
            if (close == null || close.RecordCount == 0 || close.Painted) return false;

            _series = close;

            open.Painted = high.Painted = low.Painted = close.Painted = true;

            CalculateCandleSpacing();
            if (ChartX._barSpacing < 0)
                return false;

            _upColor = _series._upColor.HasValue ? _series._upColor.Value : ChartX.UpColor;
            _downColor = _series._downColor.HasValue ? _series._downColor.Value : ChartX.DownColor;

            if (_oldOptimizePainting.HasValue && _oldOptimizePainting.Value != ChartX.OptimizePainting)
            {
                if (!ChartX.OptimizePainting)
                {
                    Canvas c = _series._chartPanel._rootCanvas;
                    c.Children.Remove(_pathCandlesDown);
                    c.Children.Remove(_pathCandlesUp);
                    c.Children.Remove(_pathWicksUp);
                    c.Children.Remove(_pathWicksDown);

                    _pathCandlesDown = null;
                    _pathCandlesUp = null;
                    _pathWicksUp = null;
                    _pathWicksDown = null;
                }
                else
                {
                    _candles.RemoveAll();
                }
            }
            _oldOptimizePainting = ChartX.OptimizePainting;

            if (ChartX.OptimizePainting)
            {
                return PaintOptimized(new[] { open, high, low, close }, _series);
            }

            if (!_subscribedToCustomBrush)
            {
                _subscribedToCustomBrush = true;
                ChartX.OnCandleCustomBrush += ChartX_OnCandleCustomBrush;
            }


            //bool setBrushes = false;
            if (!_old3DStyle.HasValue || _old3DStyle.Value != ChartX.ThreeDStyle ||
              !_oldUpColor.HasValue || _oldUpColor.Value != _upColor ||
              !_oldDownColor.HasValue || _oldDownColor.Value != _downColor ||
              (ChartX._candleDownOutlineColor.HasValue && ChartX._candleDownOutlineColor.Value != _oldCandleDownOutline) ||
              ChartX._candleUpOutlineColor.HasValue && ChartX._candleUpOutlineColor.Value != _oldCandleUpOutline)
            {

                //setBrushes = true;
                _old3DStyle = ChartX.ThreeDStyle;
                _oldUpColor = _upColor;
                _oldDownColor = _downColor;

                _upBrush = !ChartX.ThreeDStyle
                             ? (Brush)new SolidColorBrush(_upColor)
                             : new LinearGradientBrush
                                 {
                                     StartPoint = new Point(0, 0.5),
                                     EndPoint = new Point(1, 0.5),
                                     GradientStops =
                             {
                               new GradientStop
                                 {
                                   Color = _upColor,
                                   Offset = 0
                                 },
                               new GradientStop
                                 {
                                   Color = Constants.FadeColor,
                                   Offset = 1.25
                                 }
                             }
                                 };
#if WPF
        _upBrush.Freeze();
#endif

                _downBrush = !ChartX.ThreeDStyle
                               ? (Brush)new SolidColorBrush(_downColor)
                               : new LinearGradientBrush
                                   {
                                       StartPoint = new Point(0, 0.5),
                                       EndPoint = new Point(1, 0.5),
                                       GradientStops =
                               {
                                 new GradientStop
                                   {
                                     Color = _downColor,
                                     Offset = 0
                                   },
                                 new GradientStop
                                   {
                                     Color = Constants.FadeColor,
                                     Offset = 1.25
                                   }
                               }
                                   };
#if WPF
        _downBrush.Freeze();
#endif

                if (ChartX._candleDownOutlineColor.HasValue)
                {
                    _oldCandleDownOutline = ChartX._candleDownOutlineColor.Value;
                    _candleDownOutlineBrush = new SolidColorBrush(ChartX._candleDownOutlineColor.Value);
#if WPF
          _candleDownOutlineBrush.Freeze();
#endif
                }
                if (ChartX._candleUpOutlineColor.HasValue)
                {
                    _oldCandleUpOutline = ChartX._candleUpOutlineColor.Value;
                    _candleUpOutlineBrush = new SolidColorBrush(ChartX._candleUpOutlineColor.Value);
#if WPF
          _candleUpOutlineBrush.Freeze();
#endif
                }
            }

            int n;

            _candles.C = _series._chartPanel._rootCanvas;
            _candles.Start();

            for (n = ChartX._startIndex; n < ChartX._endIndex; n++)
            {
                if (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue)
                    continue;

                Candle candle = _candles.GetPaintObject(_upBrush, _downBrush);
                candle.Init(_series);
                //if (setBrushes)   //because if we have a small number of bars, then enlarge the new brushes won't be propagated to new candles.
                candle.SetBrushes(_upBrush, _downBrush, _candleUpOutlineBrush, _candleDownOutlineBrush);
                candle.SetValues(open[n].Value.Value, high[n].Value.Value, low[n].Value.Value, close[n].Value.Value, ChartX._barSpacing,
                                 _halfwick, n - ChartX._startIndex);
            }
            _candles.Stop();

            _candles.Do(c => c.ZIndex = ZIndexConstants.PriceStyles1);

            return true;
        }

        private void ChartX_OnCandleCustomBrush(string seriesName, int index, Brush brush)
        {
            if (index >= 0 && index < _candles.Count)
                _candles[index].ForceCandlePaint();
        }

        public override void RemovePaint()
        {
            if (_subscribedToCustomBrush)
            {
                ChartX.OnCandleCustomBrush -= ChartX_OnCandleCustomBrush;
            }

            if (!ChartX.OptimizePainting)
            {
                _candles.RemoveAll();
            }
            else
            {
                Canvas c = _series._chartPanel._rootCanvas;
                c.Children.Remove(_pathCandlesDown);
                c.Children.Remove(_pathCandlesUp);
                c.Children.Remove(_pathWicksUp);
                c.Children.Remove(_pathWicksDown);

                _pathCandlesDown = null;
                _pathCandlesUp = null;
                _pathWicksUp = null;
                _pathWicksDown = null;
            }
        }

        private Path _pathCandlesDown;
        private Path _pathCandlesUp;
        private Path _pathWicksUp;
        private Path _pathWicksDown;

        public bool PaintOptimizedEx(object drawingContext, Series[] series, Series seriesClose)
        {
            return true;
        }


        public bool PaintOptimized(Series[] series, Series seriesClose)
        {
            PriceStyleStandardModel model
              = new PriceStyleStandardModel(ChartX._startIndex, ChartX._endIndex, series);

            if (_pathCandlesDown == null)
            {
                _pathWicksUp = new Path { Tag = this };
                _series._chartPanel._rootCanvas.Children.Add(_pathWicksUp);
                Canvas.SetZIndex(_pathWicksUp, ZIndexConstants.PriceStyles1);

                _pathWicksDown = new Path { Tag = this };
                _series._chartPanel._rootCanvas.Children.Add(_pathWicksDown);
                Canvas.SetZIndex(_pathWicksDown, ZIndexConstants.PriceStyles1);

                _pathCandlesDown = new Path { Tag = this };
                _series._chartPanel._rootCanvas.Children.Add(_pathCandlesDown);
                Canvas.SetZIndex(_pathCandlesDown, ZIndexConstants.PriceStyles1 + 1);

                _pathCandlesUp = new Path { Tag = this };
                _series._chartPanel._rootCanvas.Children.Add(_pathCandlesUp);
                Canvas.SetZIndex(_pathCandlesUp, ZIndexConstants.PriceStyles1 + 1);
            }

            double downBorderWidth = 0;
            double upBorderWidth = 0;
            if (!seriesClose.UseEnhancedColoring)
            {
                _pathCandlesUp.Fill = new SolidColorBrush(_upColor);
                _pathCandlesUp.Fill.Freeze();
                _pathCandlesDown.Fill = new SolidColorBrush(_downColor);
                _pathCandlesDown.Fill.Freeze();

                if (ChartX.CandleDownOutlineColor.HasValue)
                {
                    _pathCandlesDown.StrokeThickness = downBorderWidth = 1;
                    _pathCandlesDown.Stroke = new SolidColorBrush(ChartX.CandleDownOutlineColor.Value);
                    _pathCandlesDown.Stroke.Freeze();

                    if (ChartX.CandleDownWickMatchesOutlineColor)
                        _pathWicksDown.Stroke = new SolidColorBrush(ChartX.CandleDownOutlineColor.Value);
                    else
                        _pathWicksDown.Stroke = new SolidColorBrush(_downColor);
                }
                else
                {
                    _pathCandlesDown.StrokeThickness = downBorderWidth = 0;

                    _pathWicksDown.Stroke = new SolidColorBrush(_downColor);
                }

                if (ChartX.CandleUpOutlineColor.HasValue)
                {
                    _pathCandlesUp.StrokeThickness = upBorderWidth = 1;
                    _pathCandlesUp.Stroke = new SolidColorBrush(ChartX.CandleUpOutlineColor.Value);
                    _pathCandlesUp.Stroke.Freeze();

                    if (ChartX.CandleUpWickMatchesOutlineColor)
                        _pathWicksUp.Stroke = new SolidColorBrush(ChartX.CandleUpOutlineColor.Value);
                    else
                        _pathWicksUp.Stroke = new SolidColorBrush(_upColor);

                }
                else
                {
                    _pathCandlesUp.StrokeThickness = upBorderWidth = 0;

                    _pathWicksUp.Stroke = new SolidColorBrush(_upColor);
                }

                _pathWicksDown.Stroke.Freeze();
                _pathWicksUp.Stroke.Freeze();

            }
            else
            {
                _pathWicksUp.Stroke = (Brush)seriesClose.WickUpStroke.GetAsFrozen();
                _pathWicksUp.StrokeThickness = seriesClose.WickStrokeThickness;
                _pathWicksDown.Stroke = (Brush)seriesClose.WickDownStroke.GetAsFrozen();
                _pathWicksDown.StrokeThickness = seriesClose.WickStrokeThickness;
                _pathCandlesUp.Stroke = (Brush)seriesClose.CandleUpStroke.GetAsFrozen();
                _pathCandlesUp.Fill = (Brush)seriesClose.CandleUpFill.GetAsFrozen();
                _pathCandlesUp.StrokeThickness = seriesClose.CandleStrokeThickness;
                _pathCandlesDown.Stroke = (Brush)seriesClose.CandleDownStroke.GetAsFrozen();
                _pathCandlesDown.Fill = (Brush)seriesClose.CandleDownFill.GetAsFrozen();
                _pathCandlesDown.StrokeThickness = seriesClose.CandleStrokeThickness;

                downBorderWidth = upBorderWidth = seriesClose.CandleStrokeThickness;
            }

            downBorderWidth /= 2;
            upBorderWidth /= 2;

            model.Space = ChartX._barSpacing;
            model.HalfWick = _halfwick;

            GeometryGroup groupDown = new GeometryGroup();
            GeometryGroup groupUp = new GeometryGroup();
            GeometryGroup groupWicksDown = new GeometryGroup();
            GeometryGroup groupWicksUp = new GeometryGroup();
            foreach (PriceStyleStandardModel.Values value in model.WickValues)
            {
                PriceStyleStandardModel.WickValueType wick = value.WickValue;

                if (value.CandleValue.Type == PriceStyleStandardModel.CandleType.Down)
                {
                    groupDown.Children.Add(
                        new RectangleGeometry
                        {
                            Rect = new Rect(new Point(value.CandleValue.X1 + downBorderWidth, value.CandleValue.Y1 + downBorderWidth),
                                            new Point(value.CandleValue.X2 - downBorderWidth, value.CandleValue.Y2 - downBorderWidth)),
                        });
                    if (wick != null)
                    {
                        groupWicksDown.Children.Add(
                            new LineGeometry
                            {
                                StartPoint = new Point(wick.X, wick.Y1),
                                EndPoint = new Point(wick.X, value.CandleValue.Y1),
                            });
                        groupWicksDown.Children.Add(
                            new LineGeometry
                            {
                                StartPoint = new Point(wick.X, value.CandleValue.Y2),
                                EndPoint = new Point(wick.X, wick.Y2),
                            });
                    }
                }
                else
                {
                    groupUp.Children.Add(
                        new RectangleGeometry
                        {
                            Rect = new Rect(new Point(value.CandleValue.X1 + upBorderWidth, value.CandleValue.Y1 + upBorderWidth),
                                            new Point(value.CandleValue.X2 - upBorderWidth, value.CandleValue.Y2 - upBorderWidth))
                        });
                    if (wick != null)
                    {
                        groupWicksUp.Children.Add(
                            new LineGeometry
                            {
                                StartPoint = new Point(wick.X, wick.Y1),
                                EndPoint = new Point(wick.X, value.CandleValue.Y1),
                            });
                        groupWicksUp.Children.Add(
                            new LineGeometry
                            {
                                StartPoint = new Point(wick.X, value.CandleValue.Y2),
                                EndPoint = new Point(wick.X, wick.Y2),
                            });
                    }
                }
            }

            _pathWicksUp.Data = groupWicksUp;
            _pathWicksDown.Data = groupWicksDown;
            _pathCandlesUp.Data = groupUp;
            _pathCandlesDown.Data = groupDown;
            return true;
        }
    }
}
