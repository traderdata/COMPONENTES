using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ModulusFE.Controls;
using ModulusFE.PaintObjects;
#if SILVERLIGHT
using ModulusFE.SL.Utils;
#endif

namespace ModulusFE
{
    using System.Diagnostics;

    /// <summary>
    /// Defines methods that handle scrolling and zoomming
    /// </summary>
    public partial class StockChartX
    {
        private const string TimerResize = "TimerResize";
        private const string TimerMove = "TimerMove";
        private const string TimerUpdate = "TimerUpdate";
        private const string TimerCrossHairs = "TimerCrossHairs";
        private const string TimerInfoPanel = "TimerInfoPanel";
        private const int PixelsGapToResize = 5;

        private readonly ChartTimers _timers = new ChartTimers();

        private bool _calendarMouseLeftButtonDown;
#if WPF
    private bool _calendarMouseRightButtonDown;
#endif
        private double _calendarResizeStartX;
        private double _calendarMoveStartX;
        private bool _calendarResizing;
        private bool _calendarMoving;
        private bool _scrollerUpdating;
        private int _oldZoomLevel;

        private ChartScrollerEx _scroller;
        private ChartScrollerProperties _scrollerProperties;

        private void SetupChartScroller(ChartScrollerProperties properties)
        {
            if (_scrollerProperties == properties)
                return;

            if (properties == null)
            {
                if (_scroller != null)
                {
                    _scroller.Visibility = Visibility.Collapsed;
                }

                _scrollerProperties = null;
                return;
            }

            _scrollerProperties = properties;
            _scrollerProperties.Chart = this;
            _scroller.SetProperties(_scrollerProperties);
            _scroller.Visibility = IsChartScrollerVisible ? Visibility.Visible : Visibility.Collapsed;

            UpdateScrollerVisual();
        }

        internal bool CtrlDown { get; set; }

        internal void ShowInfoPanelInternal()
        {
            if (_panelsContainer == null)
            {
                return;
            }

            _panelsContainer.ShowInfoPanelInternal();
        }

        internal void StartShowingInfoPanel()
        {
            _timers.StartTimerWork(TimerInfoPanel);
            _panelsContainer.ShowInfoPanelInternal();
        }

        internal void StopShowingInfoPanel()
        {
            _timers.StopTimerWork(TimerInfoPanel);
            _panelsContainer.HideInfoPanel();
        }

#if WPF
    private void Calendar_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      _calendar.ReleaseMouseCapture();
      _calendarMouseRightButtonDown = false;
      _timers.StopTimerWork(TimerMove);
      _calendar.Cursor = Cursors.Arrow;
    }

    private void Calendar_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      _calendarMouseRightButtonDown = true;
      _calendar.CaptureMouse();
      _calendarMoveStartX = e.GetPosition(_calendar).X;
      _timers.StartTimerWork(TimerMove);
      _calendar.Cursor = Cursors.ScrollWE;
    }
#endif

        private void Calendar_OnKeyUp(object sender, KeyEventArgs e)
        {
            this.CtrlDown = false;
        }

        private void Calendar_OnKeyDown(object sender, KeyEventArgs e)
        {
#if SILVERLIGHT
            if (e.Key == Key.Ctrl)
                CtrlDown = true;
#endif
#if WPF
      if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        this.CtrlDown = true;
#endif
        }

        private void Calendar_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _calendarMouseLeftButtonDown = false;
            _calendar.ReleaseMouseCapture();

            if (_calendarResizing)
            {
                _timers.StopTimerWork(TimerResize);
            }

            if (_calendarMoving)
            {
                _timers.StopTimerWork(TimerMove);
            }

            _calendar.Cursor = Cursors.Arrow;
        }

        private void Calendar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _calendarMouseLeftButtonDown = true;
            _calendar.CaptureMouse();
            _calendarResizeStartX = e.GetPosition(_calendar).X;

            _calendarResizing = _calendarMoving = false;
            if (!this.CtrlDown)
            {
                _timers.StartTimerWork(TimerResize);
                _calendarResizing = true;
            }
            else
            {
                _timers.StartTimerWork(TimerMove);
                _calendarMoving = true;
            }

            _calendar.Cursor = Cursors.SizeWE;
        }

        private void ResizeChart()
        {
            bool update = false;
            if (!_calendarMouseLeftButtonDown || RecordCount < 5)
            {
                return;
            }

            int startIndex = _startIndex;
            int endIndex = _endIndex;

            Point p = Mouse.GetPosition(_calendar);

            // right - increase size
            if (p.X >= _calendarResizeStartX + PixelsGapToResize)
            {
                if (_endIndex - _startIndex <= 5)
                {
                    return; // 5 bars at least
                }

                _startIndex++;
                _endIndex--;
                update = true;
            }
            else if (p.X <= _calendarResizeStartX - PixelsGapToResize)
            {
                int oldStartIndex = _startIndex;
                int oldEndIndex = _endIndex;
                if (_startIndex > 0)
                {
                    _startIndex--;
                }

                if (_endIndex < RecordCount)
                {
                    _endIndex++;
                }

                update = oldStartIndex != _startIndex || oldEndIndex != _endIndex;
            }

            if (update)
            {
                _calendar.Paint();
                _panelsContainer.Panels.ForEach(panel =>
                                                  {
                                                      panel._enforceSeriesSetMinMax = true;
                                                      panel.Paint();
                                                      panel.PaintXGrid();
                                                  });
                if (startIndex != _startIndex)
                {
                    OnPropertyChanged(Property_StartIndex);
                }

                if (endIndex != _endIndex)
                {
                    OnPropertyChanged(Property_EndIndex);
                }

                FireZoom();
            }

            _calendarResizeStartX = p.X;
        }

        private void MoveChart()
        {
#if WPF
      if (!_calendarMouseRightButtonDown) return;
#endif
            bool update = false;

            Point p = Mouse.GetPosition(_calendar);

            // going right direction
            if (p.X >= _calendarMoveStartX + PixelsGapToResize)
            {
                if (_endIndex == RecordCount)
                {
                    return;
                }

                _startIndex++;
                _endIndex++;
                update = true;
            }
            else if (p.X <= _calendarMoveStartX - PixelsGapToResize)
            {
                // going left direction
                if (_startIndex == 0)
                {
                    return;
                }

                _startIndex--;
                _endIndex--;
                update = true;
            }

            if (update)
            {
                _calendar.Paint();
                _panelsContainer.Panels.ForEach(panel =>
                                                  {
                                                      panel._enforceSeriesSetMinMax = true;
                                                      panel.Paint();
                                                      panel.PaintXGrid();
                                                  });
                OnPropertyChanged(Property_StartIndex);
                OnPropertyChanged(Property_EndIndex);

                FireChartScroll();
            }

            _calendarMoveStartX = p.X;
        }

        private void MoveCrossHairs()
        {
            _panelsContainer.ShowCrossHairs();
        }

        internal bool CanResize()
        {
            return (_endIndex - _startIndex) <= MaxVisibleRecords;
        }

        internal void ChartScrollerOnPositionChanged(object sender, int left, int right, ref bool cancel)
        {
            if ((left == _startIndex && right == _endIndex) || (right - left < 3)) // we need to have at least 3 bars 
            {
                cancel = true;
                return;
            }

            //      if ((_chartScroller.PositionType != ChartScroller.MouseDownPositionType.All) && (right - left > MaxVisibleRecords))
            //      {
            //        cancel = true;
            //        return;
            //      }

            Action a =
              () =>
              {

                  if (sender != null) // in case when this method is callde from PanelsContainer
                  {
                      _scrollerUpdating = true;
                  }

                  _startIndex = left;
                  if (_endIndex != right)
                  {
                      _endIndex = right;
                      OnPropertyChanged(Property_EndIndex);
                  }

                  Update();
                  _scrollerUpdating = false;

                  if (right - left == _oldZoomLevel)
                  {
                      //return;
                  }

                  //Debug.WriteLine(_chartScroller.PositionType + " : " + right + " : " + left + " : " + _oldZoomLevel);

                  //          if (_chartScroller.PositionType == ChartScroller.MouseDownPositionType.Left ||
                  //            _chartScroller.PositionType == ChartScroller.MouseDownPositionType.Right)
                  //          {
                  //            FireZoom();
                  //          }
                  //          else if (_chartScroller.PositionType == ChartScroller.MouseDownPositionType.All)
                  //          {
                  //            FireChartScroll();
                  //          }

                  _oldZoomLevel = right - left;
              };

            //a();

#if SILVERLIGHT
            ThreadPool.QueueUserWorkItem(state => Dispatcher.BeginInvoke(a));
#endif
#if WPF
      ThreadPool.QueueUserWorkItem(state => Dispatcher.BeginInvoke(DispatcherPriority.Normal, a));
#endif
        }

        internal void UpdateScrollerVisual()
        {
            if (_scroller == null || _scrollerProperties == null)
            {
                return;
            }

            _scroller.Background = _scrollerProperties.Background;
            _scroller.UpdateVisuals();
            _scroller.PaintTrend();
            _scroller.PaintSelection();
        }

        internal void RepositionScroller()
        {
            //      if (_scrollerProperties != null)
            //      {
            //        Canvas.SetTop(_scroller, _scrollerRect.Top);
            //        Canvas.SetLeft(_scroller, _scrollerRect.Left);
            //        _scroller.Height = _scrollerRect.Height;
            //        _scroller.Width = _scrollerRect.Width;
            //
            //        _scroller.PaintTrend();
            //        _scroller.PaintSelection();
            //      }
        }
    }
}