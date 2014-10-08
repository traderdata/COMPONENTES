using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ModulusFE.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ChartScrollerEx : Canvas
    {
#if WPF
    static ChartScrollerEx()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartScrollerEx), new FrameworkPropertyMetadata(typeof(ChartScrollerEx)));
    }
#endif

        private const short ZIndexBackgroundTrend = 1;
        private const short ZIndexBackground = ZIndexBackgroundTrend + 1;
        private const short ZIndexHandle = ZIndexBackground + 1;

        private Rectangle _selection;
        private Rect _selectionRect;
        private Path _backgroundTrend;
        private double _startX;
        private ChartScrollerProperties _properties;
        private ChartScrollerExHandle _leftHandle;
        private ChartScrollerExHandle _rightHandle;
        private bool _prevCanZoom = true;
        private bool _updatingChart;

        internal ActivePartType ActivePart { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public ChartScrollerEx()
        {
            ActivePart = ActivePartType.Outside;
            Height = 50;

            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            LostMouseCapture += OnLostMouseCapture;
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            PaintSelection();
            PaintTrend();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal void SetProperties(ChartScrollerProperties properties)
        {
            _properties = properties;
        }

        private ActivePartType GetActivePart(double x)
        {
            if (x.Between(_selectionRect.Left - 5, _selectionRect.Left + 5))
            {
                return ActivePartType.Left;
            }

            if (x.Between(_selectionRect.Right - 5, _selectionRect.Right + 5))
            {
                return ActivePartType.Right;
            }

            if (x.Between(_selectionRect.Left, _selectionRect.Right))
            {
                return ActivePartType.Middle;
            }

            return ActivePartType.Outside;
        }

        private void EnsureVisualsCreated()
        {
            if (_selection == null)
            {
                _selection = new Rectangle();
                Children.Add(_selection);
                SetZIndex(_selection, ZIndexBackground);
            }

            if (_backgroundTrend == null)
            {
                _backgroundTrend = new Path();
                Children.Add(_backgroundTrend);
                SetZIndex(_backgroundTrend, ZIndexBackgroundTrend);
            }

            if (_leftHandle == null)
            {
                _leftHandle = new ChartScrollerExHandle();
                Children.Add(_leftHandle);
                SetZIndex(_leftHandle, ZIndexHandle);

                _rightHandle = new ChartScrollerExHandle();
                Children.Add(_rightHandle);
                SetZIndex(_rightHandle, ZIndexHandle);
            }
        }

        internal void UpdateVisuals()
        {
            if (_properties == null || _properties.Chart == null)
            {
                return;
            }

            EnsureVisualsCreated();

            Background = _properties.Background;

            _selection.Fill = _properties.ThumbBackground;
            _selection.StrokeThickness = 1;
            _selection.Stroke = _properties.ThumbStroke;
            _selection.Opacity = 0.5;

            _backgroundTrend.Fill = _properties.TrendBackground;
            _backgroundTrend.Stroke = _properties.TrendStroke;

            _leftHandle.Stroke = _properties.HandleStroke;
            _rightHandle.Stroke = _properties.HandleStroke;

            Height = _properties.Height;

            SetHandleBackground(CanZoom());
        }

        internal void PaintSelection()
        {
            if (_properties == null || _properties.Chart == null || double.IsNaN(ActualWidth) || ActualWidth == 0)
                return;


            EnsureVisualsCreated();

            int recordCount = _properties.Chart.RecordCount;
            if (recordCount == 0)
                return;

            double left = ActualWidth * (1f * _properties.Chart.FirstVisibleRecord / recordCount);
            double right = ActualWidth * (1f * _properties.Chart.LastVisibleRecord / recordCount);

            _selectionRect = new Rect(left, 0, right - left, ActualHeight);
            SetLeft(_selection, _selectionRect.Left);
            SetTop(_selection, _selectionRect.Top);
            _selection.Width = _selectionRect.Width;
            _selection.Height = _selectionRect.Height;

            SetTop(_leftHandle, ActualHeight / 2 - _leftHandle.ActualHeight / 2);
            SetLeft(_leftHandle, _selectionRect.Left - _leftHandle.ActualWidth / 2);

            SetTop(_rightHandle, ActualHeight / 2 - _rightHandle.ActualHeight / 2);
            SetLeft(_rightHandle, _selectionRect.Right - _rightHandle.ActualWidth / 2);
        }

        private void UpdateIndexes(Point pos)
        {
            if (_properties == null || _properties.Chart == null)
                return;

            int startIndex = _properties.Chart.FirstVisibleRecord, endIndex = _properties.Chart.LastVisibleRecord;
            int recordCount = _properties.Chart.RecordCount;
            int firstVisibleRecord = startIndex, lastVisibleRecord = endIndex;
            bool flag = false;
            switch (ActivePart)
            {
                case ActivePartType.Left:
                    if (pos.X < 0)
                        startIndex = 0;
                    else if (pos.X >= (_selectionRect.Right - 5))
                        return;
                    else
                        startIndex = (int)Math.Round(pos.X * recordCount / ActualWidth);

                    flag = startIndex != _properties.Chart.FirstVisibleRecord &&
                      (endIndex - startIndex > 5);
                    if (flag)
                        firstVisibleRecord = startIndex;
                    break;
                case ActivePartType.Right:
                    if (pos.X >= ActualWidth)
                        endIndex = _properties.Chart.RecordCount;
                    else if (pos.X < _selectionRect.Left + 5)
                        return;
                    else
                        endIndex = (int)Math.Round(pos.X * recordCount / ActualWidth);

                    flag = endIndex != _properties.Chart.LastVisibleRecord &&
                      (endIndex - startIndex > 5);
                    if (flag)
                        lastVisibleRecord = endIndex;
                    break;
                case ActivePartType.Middle:
                    double dx = pos.X - _startX;
                    int indexDistance = endIndex - startIndex;
                    double left = _selectionRect.Left + dx;
                    if (dx < 0)
                    {
                        // moving left
                        if (left < 0)
                            startIndex = 0;
                        else
                            startIndex = (int)Math.Round(left * recordCount / ActualWidth);

                        endIndex = startIndex + indexDistance;
                    }
                    else
                    {
                        // moving right
                        if (left + _selectionRect.Width >= ActualWidth)
                        {
                            endIndex = _properties.Chart.RecordCount;
                            startIndex = endIndex - indexDistance;
                        }
                        else
                        {
                            startIndex = (int)Math.Round(left * recordCount / ActualWidth);
                            endIndex = startIndex + indexDistance;
                        }
                    }

                    flag = startIndex != _properties.Chart.FirstVisibleRecord &&
                           endIndex != _properties.Chart.LastVisibleRecord;
                    if (flag)
                    {
                        firstVisibleRecord = startIndex;
                        lastVisibleRecord = endIndex;
                    }

                    _startX = pos.X;
                    break;
            }

            bool canZoom = _properties.Chart.MaxVisibleRecords > (lastVisibleRecord - firstVisibleRecord);
            if (_prevCanZoom != canZoom)
            {
                SetHandleBackground(canZoom);
                //_prevCanZoom = canZoom;
            }

            if (flag && _prevCanZoom || ActivePart == ActivePartType.Middle)
            {
                if (_properties.Chart.LastVisibleRecord != lastVisibleRecord)
                    _properties.Chart.OnPropertyChanged(StockChartX.Property_EndIndex);

                _updatingChart = true;
                _properties.Chart.FirstVisibleRecord = firstVisibleRecord;
                _properties.Chart.LastVisibleRecord = lastVisibleRecord;
                //PaintSelection(); 
                _properties.Chart.Update();
                _updatingChart = false;

                // fire needed events
                if (ActivePart == ActivePartType.Middle)
                    _properties.Chart.FireChartScroll();
                else
                    _properties.Chart.FireZoom();
            }

            _prevCanZoom = canZoom;
        }

        internal void PaintTrend()
        {
            if (_properties == null || _properties.Chart == null || double.IsNaN(ActualWidth) ||
                string.IsNullOrEmpty(_properties.Chart.Symbol) ||
                _updatingChart)
                return;

            EnsureVisualsCreated();

            _backgroundTrend.Width = ActualWidth;
            _backgroundTrend.Height = ActualHeight;

            PathGeometry geometry = new PathGeometry();
            PathSegmentCollection segments = new PathSegmentCollection();
            PathFigure figure = new PathFigure();

            Series closeSeries = _properties.Chart.SeriesCollection.FirstOrDefault(_ => _.OHLCType == SeriesTypeOHLC.Close) ??
                                 _properties.Chart.SeriesCollection.FirstOrDefault();
            if (closeSeries == null)
                return;

            IList<double> values = closeSeries
                .AllValues
                .Where(_ => _ != null)
                .Select(_ => _.Value)
                .ToList();

            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (double value in values) // No LINQ here!!!
            {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }

            int i;
            double y, x;
            for (i = 0; i < values.Count; i += 5)
            {
                double value = values[i];
                x = (i * 1f / values.Count) * ActualWidth;
                if (min != max)
                    y = ActualHeight * (1f - ((value - min) / (max - min)));
                else
                    y = ActualHeight * 0.5;

                if (i == 0)
                    figure.StartPoint = new Point(x, y);
                else
                    segments.Add(new LineSegment { Point = new Point(x, y) });
            }

            x = ActualWidth;
            if (min != max && values.Count > 0)
                y = ActualHeight * (1f - ((values[values.Count - 1] - min) / (max - min)));
            else
                y = ActualHeight * 0.5;

            segments.Add(new LineSegment { Point = new Point(x, y) });

            segments.Add(new LineSegment { Point = new Point(ActualWidth, ActualHeight) });
            segments.Add(new LineSegment { Point = new Point(0, ActualHeight) });

            figure.Segments = segments;
            geometry.Figures.Add(figure);

            _backgroundTrend.Data = geometry;
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
        {
            ActivePart = ActivePartType.Outside;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            args.Handled = true;
            Point position = args.GetPosition(this);
            ActivePart = GetActivePart(position.X);
            _startX = position.X;

            switch (ActivePart)
            {
                case ActivePartType.Left:
                case ActivePartType.Right:
                    Cursor = Cursors.SizeWE;
                    CaptureMouse();
                    break;
                case ActivePartType.Middle:
                    Cursor = Cursors.Hand;
                    CaptureMouse();
                    break;
                case ActivePartType.Outside:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            args.Handled = true;
            Point position = args.GetPosition(this);

            if (ActivePart != ActivePartType.Outside)
            {
                UpdateIndexes(position);

                ActivePart = ActivePartType.Outside;
                ReleaseMouseCapture();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            Point position = args.GetPosition(this);
            if (ActivePart == ActivePartType.Outside)
            {
                switch (GetActivePart(position.X))
                {
                    case ActivePartType.Left:
                    case ActivePartType.Right:
                        Cursor = Cursors.SizeWE;
                        break;
                    case ActivePartType.Middle:
                        Cursor = Cursors.Hand;
                        break;
                    case ActivePartType.Outside:
                        Cursor = Cursors.Arrow;
                        break;
                }
            }
            else
            {
                Action a = () => UpdateIndexes(position);
#if SILVERLIGHT
                ThreadPool.QueueUserWorkItem(state => Dispatcher.BeginInvoke(a));
#endif
#if WPF
      ThreadPool.QueueUserWorkItem(state => Dispatcher.BeginInvoke(DispatcherPriority.Normal, a));
#endif
            }
        }

        private bool CanZoom()
        {
            if (_properties == null || _properties.Chart == null)
                return false;

            return _properties.Chart.MaxVisibleRecords == 0 ||
              _properties.Chart.MaxVisibleRecords >= (_properties.Chart.LastVisibleRecord - _properties.Chart.FirstVisibleRecord);
        }

        private void SetHandleBackground(bool canZoom)
        {
            if (_leftHandle == null)
                return;

            _leftHandle.Background = canZoom ? _properties.HandleBackground : _properties.HandleBackgroundProhibited;
            _rightHandle.Background = canZoom ? _properties.HandleBackground : _properties.HandleBackgroundProhibited;
        }

        internal enum ActivePartType
        {
            Left,
            Right,
            Middle,
            Outside,
        }
    }
}
