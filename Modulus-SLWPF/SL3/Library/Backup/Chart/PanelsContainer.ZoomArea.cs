using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE
{
    public partial class PanelsContainer
    {
        private Border _zoomer;

        private int _zoomerLeftIndex, _zoomerRightIndex;

        private void EnsureZoomerCreated()
        {
            if (_zoomer != null || _chartX.DisableZoomArea)
                return;

            _zoomer = new Border
                        {
                            Visibility = Visibility.Collapsed,
                            BorderThickness = new Thickness(1),
                            BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x06, 0xAF, 0xF7)),
                            Background = new LinearGradientBrush
                                           {
                                               StartPoint = new Point(0, 0),
                                               EndPoint = new Point(1, 1),
                                               GradientStops = new GradientStopCollection
                                                       {
                                                         new GradientStop
                                                           {
                                                             Offset = 0,
                                                             Color = Color.FromArgb(0x3F, 0x12, 0x7C, 0xF1)
                                                           },
                                                         new GradientStop
                                                           {
                                                             Offset = 0.5,
                                                             Color = Color.FromArgb(0x9B, 0x57, 0x8F, 0xF3)
                                                           },
                                                         new GradientStop
                                                           {
                                                             Offset = 1,
                                                             Color = Color.FromArgb(0x3F, 0x12, 0x7C, 0xF1)
                                                           }
                                                       }
                                           }
                        };
            SetZIndex(_zoomer, ZIndexConstants.ZoomArea);
            Children.Add(_zoomer);
        }

        private void TryStartZoom(Point p)
        {
            if (_chartX.DisableZoomArea)
                return;
            object o;
            var oUnder = _chartX.GetObjectFromCursor(out o);
            if (oUnder != ObjectFromCursor.PanelPaintableArea)
                return;

            _zoomStartX = p.X;
            _state = StateEnum.MightZoom;
        }

        private void StartZoom(double x)
        {
            if (_chartX.DisableZoomArea)
                return;

            EnsureZoomerCreated();

            _state = StateEnum.Zooming;
            //CaptureMouse();
            _zoomer.Visibility = Visibility.Visible;
            SetLeft(_zoomer, x);
            SetTop(_zoomer, Constants.PanelTitleBarHeight);
            _zoomer.Width = 1;
            _zoomer.Height = ActualHeight - Constants.PanelTitleBarHeight;

            HideInfoPanel();
            HideCrossHairs();
        }

        private void DoZoom(Point p)
        {
            if (_chartX.DisableZoomArea)
                return;

            if (_state == StateEnum.MightZoom)
            {
                if (Math.Abs(p.X - _zoomStartX) < 3)
                    return;
                StartZoom(_zoomStartX);

                return;
            }

            double x = p.X;
            if (x <= _chartX.PaintableLeft)
                x = _chartX.PaintableLeft;
            else if (x > _chartX.PaintableRight)
                x = _chartX.PaintableRight;

            double tooltipLeft, tooltipRight;
            if (x > _zoomStartX)
            {
                SetLeft(_zoomer, _zoomStartX);
                _zoomer.Width = x - _zoomStartX;
                tooltipLeft = _zoomStartX;
                tooltipRight = x;
            }
            else
            {
                SetLeft(_zoomer, x);
                _zoomer.Width = _zoomStartX - x;
                tooltipLeft = x;
                tooltipRight = _zoomStartX;
            }

            _zoomerLeftIndex = _chartX.GetReverseX(tooltipLeft) + _chartX._startIndex;
            _zoomerRightIndex = _chartX.GetReverseX(tooltipRight) + _chartX._startIndex;
        }

        private void StopZoom()
        {
            if (_chartX.DisableZoomArea)
                return;

            _state = StateEnum.Normal;
            _zoomer.Visibility = Visibility.Collapsed;

            if (_zoomerRightIndex - _zoomerLeftIndex <= 3)
                return;

            StockChartX.BeforeZoomEventArgs args = new StockChartX.BeforeZoomEventArgs(_zoomerLeftIndex, _zoomerRightIndex);
            _chartX.InvokeBeforeZoom(args);
            if (args.Cancel)
                return;

            if (_zoomerRightIndex > _chartX._endIndex)
                _zoomerRightIndex = _chartX._endIndex;

            if (_zoomerRightIndex == _zoomerLeftIndex)
            {
                if (_zoomerRightIndex < _chartX._endIndex)
                    _zoomerRightIndex = _zoomerRightIndex + 1;
                else
                {
                    _zoomerLeftIndex = _zoomerLeftIndex - 1;
                    _zoomerRightIndex = _chartX._endIndex;
                }
            }

            _zoomerLeftIndex = Math.Max(0, _zoomerLeftIndex);
            _zoomerRightIndex = Math.Min(_chartX.RecordCount, _zoomerRightIndex);

            bool fireZoom = (_chartX._startIndex != _zoomerLeftIndex) || (_chartX._endIndex != _zoomerRightIndex);
            if (fireZoom)
            {
                _chartX._startIndex = _zoomerLeftIndex;
                _chartX._endIndex = _zoomerRightIndex;
                _chartX.Update();
                _chartX.FireZoom();
            }
        }

        private void CancelZoom()
        {
            if (_chartX.DisableZoomArea)
                return;

            _zoomer.Visibility = Visibility.Collapsed;
        }
    }
}
