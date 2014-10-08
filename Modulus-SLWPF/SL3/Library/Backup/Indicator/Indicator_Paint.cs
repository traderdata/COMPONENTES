using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.PaintObjects;
#if SILVERLIGHT
using ModulusFE.SL;
#endif
#if WPF

#endif

namespace ModulusFE.Indicators
{
    public partial class Indicator
    {
        private Path _pathUp;
        private Path _pathDown;
        private Path _pathNormal;

        internal IEnumerable<Path> Paths
        {
            get
            {
                if (_pathUp == null)
                {
                    yield break;
                }

                yield return _pathUp;
                yield return _pathDown;
                yield return _pathNormal;
            }
        }

        internal void PaintGeometry()
        {
            if (!_visible || RecordCount == 0 || RecordCount < _chartPanel._chartX._startIndex) return;

            Brush strokeUpBrush = new SolidColorBrush(UpColor == null ? _chartPanel._chartX.UpColor : UpColor.Value);
            Brush strokeDownBrush = new SolidColorBrush(DownColor == null ? _chartPanel._chartX.DownColor : DownColor.Value);
            Brush strokeNormalBrush = new SolidColorBrush(_strokeColor);

            bool isOscillator = !ForceLinearChart && ForceOscilatorPaint;

            if (_pathUp == null)
            {
                var c = _chartPanel._rootCanvas;

                _pathUp = new Path
                            {
                                Tag = this
                            };
                c.Children.Add(_pathUp);
                Canvas.SetZIndex(_pathUp, ZIndexConstants.Indicators1);

                _pathDown = new Path
                              {
                                  Tag = this
                              };
                c.Children.Add(_pathDown);
                Canvas.SetZIndex(_pathDown, ZIndexConstants.Indicators1);

                _pathNormal = new Path
                                {
                                    Tag = this,
                                };
                c.Children.Add(_pathNormal);
                Canvas.SetZIndex(_pathNormal, ZIndexConstants.Indicators1);

                SolidColorBrush brush = new SolidColorBrush(_strokeColor);
                Paths.ForEach(_ =>
                                {
                                    _.StrokeThickness = _strokeThickness;
                                    _.Stroke = brush;
                                    _.Opacity = _opacity;
                                    Types.SetShapePattern(_, _strokePattern);
                                });
            }

            _pathUp.Stroke = (Brush)strokeUpBrush.GetAsFrozen();
            _pathDown.Stroke = (Brush)strokeDownBrush.GetAsFrozen();
            _pathNormal.Stroke = (Brush)strokeNormalBrush.GetAsFrozen();

            double? x2 = null;
            double? y2 = null;
            double yZero = GetY(0);

            GeometryGroup groupUp = new GeometryGroup();
            GeometryGroup groupDown = new GeometryGroup();
            GeometryGroup groupNormal = new GeometryGroup();

            GeometryGroup currentGroup = groupNormal;

            int cnt = 0;
            for (int i = _chartPanel._chartX._startIndex; i < _chartPanel._chartX._endIndex; i++, cnt++)
            {
                double x1 = _chartPanel._chartX.GetXPixel(cnt);
                double? y1;
                double? value = y1 = this[i].Value;
                if (!y1.HasValue)
                    continue;

                y1 = GetY(y1.Value);
                if (i > 0 && i == _chartPanel._chartX._startIndex)
                    y2 = y1.Value;

                #region brush logic

                if (_chartPanel._chartX._useLineSeriesColors || _upColor.HasValue) // +/- change colors
                {
                    if (!isOscillator)
                    {
                        if (i > 0)
                        {
                            if (this[i].Value > this[i - 1].Value)
                            {
                                currentGroup = groupUp;
                            }
                            else
                            {
                                currentGroup = this[i].Value < this[i - 1].Value ? groupDown : groupNormal;
                            }
                        }
                        else
                        {
                            currentGroup = groupNormal;
                        }
                    }
                    else
                    {
                        if (this[i].Value > 0)
                        {
                            currentGroup = groupUp;
                        }
                        else
                        {
                            currentGroup = this[i].Value < 0 ? groupDown : groupNormal;
                        }
                    }
                }

                #endregion

                switch (_seriesType)
                {
                    case SeriesTypeEnum.stVolumeChart:
                        if (this[i].Value.HasValue && y2.HasValue)
                        {
                            //currentSb.AddSegment(x1, (double)y1, x1, GetY(SeriesEntry._min));
                            currentGroup.Children.Add(new LineGeometry
                                                        {
                                                            StartPoint = new Point(x1, y1.Value),
                                                            EndPoint = new Point(x1, GetY(SeriesEntry._min))
                                                        });
                        }
                        break;
                    case SeriesTypeEnum.stIndicator:
                    case SeriesTypeEnum.stLineChart:
                        if (_indicatorType == IndicatorType.MACDHistogram || isOscillator)
                        {
                            if (value > 0)
                            {
                                y1 = GetY(value.Value);
                                y2 = yZero;
                            }
                            else
                            {
                                y1 = yZero;
                                y2 = GetY(value.Value);
                            }

                            if (this[i].Value.HasValue)
                            {
                                currentGroup.Children.Add(new LineGeometry
                                                            {
                                                                StartPoint = new Point(x1, y1.Value),
                                                                EndPoint = new Point(x1, y2.Value),
                                                            });
                            }
                        }
                        else
                        {
                            if (i > _chartPanel._chartX._startIndex)
                            {
                                if ( /*this[i].Value.HasValue && */y2.HasValue && x2.HasValue && Math.Abs(x1 - x2.Value) > 1)
                                {
                                    currentGroup.Children.Add(new LineGeometry
                                                                {
                                                                    StartPoint = new Point(x2.Value, y2.Value),
                                                                    EndPoint = new Point(x1, y1.Value),
                                                                });
                                }
                            }
                        }
                        break;
                    default:
                        throw new IndicatorException("Indicator has an unsuported series type.", this);
                }

                //if (linePainted || !y2.HasValue)
                y2 = y1;

                //if (linePainted || (i == _chartPanel._chartX._startIndex))
                x2 = x1;
            }


            _pathUp.Data = (System.Windows.Media.Geometry)groupUp.GetAsFrozen();
            _pathDown.Data = (System.Windows.Media.Geometry)groupDown.GetAsFrozen();
            _pathNormal.Data = (System.Windows.Media.Geometry)groupNormal.GetAsFrozen();

            if (_selected)
                ShowSelection();
        }

        internal override void Paint()
        {
            PaintGeometry();
        }

        internal override void RemovePaint()
        {
            _lines.C = _chartPanel._rootCanvas;
            _lines.RemoveAll();

            if (_pathDown != null)
            {
                _chartPanel._rootCanvas.Children.Remove(_pathDown);
                _chartPanel._rootCanvas.Children.Remove(_pathUp);
                _chartPanel._rootCanvas.Children.Remove(_pathNormal);
            }
        }
    }
}
