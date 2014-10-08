using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE.PaintObjects
{
    internal partial class Stock : IPaintObject
    {
        private double? _open;
        private double _high;
        private double _low;
        private double _close;
        private double _space;
        private int _index;
        private Series _series;

        private Brush _up;
        private Brush _down;
        private Brush _customBrush;
        private Brush _currentBrush;

        private bool _brushMustBeChanged;

        private readonly Line[] _lines = new[]
                                       {
                                         new Line(), new Line(), new Line()
                                       };

        public Stock()
        {
            _brushMustBeChanged = true;
        }

        public void Init(Series series)
        {
            _series = series;

            _lines[0]._line.Tag = series;
            _lines[1]._line.Tag = series;
            _lines[2]._line.Tag = series;
        }

        public void SetArgs(params object[] args)
        {
            Debug.Assert(args.Length == 3);
            _up = (Brush)args[0];
            _down = (Brush)args[1];
            _customBrush = (Brush)args[2];
            _brushMustBeChanged = true;
        }

        public void SetValues(double? open, double high, double low, double close, double space, int index)
        {
            if (!_brushMustBeChanged)
                _brushMustBeChanged = (_open != open) ||
                                      (_high != high) ||
                                      (_low != low) ||
                                      (_close != close) ||
                                      (_index != index) ||
                                      (_series._chartPanel._chartX.GetBarBrushChanged(_series.FullName, index + _series._chartPanel._chartX._startIndex));

            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _space = space;
            _index = index;

            Paint();
        }

        public void AddTo(Canvas canvas)
        {
            foreach (Line line in _lines)
            {
                if (line != null)
                {
                    line.AddTo(canvas);
                }
            }
        }

        public void RemoveFrom(Canvas canvas)
        {
            foreach (Line line in _lines)
            {
                line._line.Tag = null;
                line.RemoveFrom(canvas);
            }
        }

        public int ZIndex
        {
            get { return _lines.Length == 0 ? 0 : _lines[0].ZIndex; }
            set { _lines.ForEach(line => line.ZIndex = value); }
        }

        internal void Paint()
        {
            StockChartX chartX = _series._chartPanel._chartX;

            double x1 = chartX.GetXPixel(_index);
            double x2 = x1;
            double y1 = _series._chartPanel.GetY(_high);
            double y2 = _series._chartPanel.GetY(_low);

            SetCurrentBrush();

            //Vertical line
            if (_high != _low)
            {
                PaintLine(x1, y1, x2, y2, 0, true);
                if (_brushMustBeChanged)
                {
                    _lines[0].Stroke = _currentBrush;
                }
            }
            else
            {
                PaintLine(0, 0, 0, 0, 0, false);
            }

            // try to paint left half wick - line
            if (_open.HasValue && _series._seriesType != SeriesTypeEnum.stStockBarChartHLC)
            {
                x2 = x1 - _space;
                y1 = _series._chartPanel.GetY(_open.Value);
                PaintLine(x1, y1, x2, y1, 1, true);
                if (_brushMustBeChanged)
                {
                    _lines[1].Stroke = _currentBrush;
                }
            }
            else
            {
                PaintLine(0, 0, 0, 0, 1, false);
            }

            //paint right half wick - line
            x2 = x1 + _space;
            y1 = _series._chartPanel.GetY(_close);
            PaintLine(x1, y1, x2, y1, 2, true);
            if (_brushMustBeChanged)
            {
                _lines[2].Stroke = _currentBrush;
            }

            _brushMustBeChanged = false;
        }

        private void PaintLine(double x1, double y1, double x2, double y2, int index, bool paint)
        {
            if (!paint && _lines[index] != null)
            {
                _lines[index]._line.Visibility = Visibility.Collapsed;
                return;
            }

            System.Windows.Shapes.Line l = _lines[index]._line;


            _lines[index]._line.X1 = x1;
            _lines[index]._line.Y1 = y1;
            _lines[index]._line.X2 = x2;
            _lines[index]._line.Y2 = y2;
            _lines[index]._line.StrokeThickness = _series.StrokeThickness;
            if (_lines[index]._line.Visibility == Visibility.Collapsed)
            {
                _lines[index]._line.Visibility = Visibility.Visible;
            }
        }

        public void SetBrushes(Brush up, Brush down, Brush customBrush)
        {
            if (!Utils.BrushesEqual(_up, up))
            {
                _brushMustBeChanged = true;
                _up = up;
            }
            if (!Utils.BrushesEqual(_down, down))
            {
                _brushMustBeChanged = true;
                _down = down;
            }
            if (!Utils.BrushesEqual(_customBrush, customBrush))
            {
                _brushMustBeChanged = true;
                _customBrush = customBrush;
            }
        }

        internal void ForceCandlePaint()
        {
            _brushMustBeChanged = true;
            Paint();
        }

        private void SetCurrentBrush()
        {
            if (!_brushMustBeChanged)
                return;

            if (_open.HasValue && _series._seriesType != SeriesTypeEnum.stStockBarChartHLC)
                _currentBrush = _open.Value > _close ? _down : _up;
            else
                _currentBrush = _customBrush;

            if (_series._chartPanel._chartX.GetBarBrushChanged(_series.FullName, _index + _series._chartPanel._chartX._startIndex))
                _currentBrush = _series._chartPanel._chartX.GetBarBrush(_series.FullName, _index + _series._chartPanel._chartX._startIndex, _currentBrush);
        }

        internal void SetStrokeThickness(double thickness)
        {
            SetLineThickness(0, thickness);
            SetLineThickness(1, thickness);
            SetLineThickness(2, thickness);
        }

        private void SetLineThickness(int index, double thickness)
        {
            if (_lines[index]._line.Visibility == Visibility.Collapsed)
                return;

            _lines[index]._line.StrokeThickness = thickness;
        }
    }
}
