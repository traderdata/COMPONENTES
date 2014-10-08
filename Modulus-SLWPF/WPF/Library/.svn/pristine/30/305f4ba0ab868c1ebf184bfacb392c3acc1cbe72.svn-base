using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE.PaintObjects
{
    internal partial class CandleHeikinAshi : IPaintObject
    {
        private Data.DataEntry _open;
        private Data.DataEntry _high;
        private Data.DataEntry _low;
        private Data.DataEntry _close;
        private Series _series;
        private double _space;
        private double _halfwick;
        private System.Windows.Shapes.Line _lineWick;
        private System.Windows.Shapes.Rectangle _rectCandle;
        private int _index;

        private Brush _up;
        private Brush _down;
        private Brush _candleUpOutline;
        private Brush _candleDownOutline;

        /// <summary>
        /// Midpoint of the previous ba
        /// </summary>
        private double _xOpen;
        /// <summary>
        /// Highest value in the set 
        /// </summary>
        private double _xHigh;
        /// <summary>
        /// Lowest value in the set 
        /// </summary>
        private double _xLow;
        /// <summary>
        /// Average price of the current bar 
        /// </summary>
        private double _xClose;

        public void Init(Series series)
        {
            _series = series;
        }

        //private bool _eventsSet;
        public void SetValues(Data.DataEntry open, Data.DataEntry high, Data.DataEntry low, Data.DataEntry close,
          double nSpace, double halfwick, int index)
        {
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _space = nSpace;
            _halfwick = halfwick;
            _index = index;

            //      if (!_eventsSet)
            //      {
            //        _high.PropertyChanged += Wick_OnPropertyChanged;
            //        _low.PropertyChanged += Wick_OnPropertyChanged;
            //
            //        _open.PropertyChanged += Candle_OnPropertyChanged;
            //        _close.PropertyChanged += Candle_OnPropertyChanged;
            //
            //        _eventsSet = true;
            //      }

            RePaint();
        }

        //    private void Candle_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //    {
        //      PaintCandle();
        //    }
        //
        //    private void Wick_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //    {
        //      PaintWick();
        //    }
        //
        internal void PaintWick()
        {
            ChartPanel chartPanel = _series._chartPanel;
            StockChartX chartX = chartPanel._chartX;

            CalculateXValues();
            //if (_xHigh == _xLow) return;

            double x = chartX.GetXPixel(_index);
            double y1 = _series.GetY(_xHigh);
            double y2 = _series.GetY(_xLow);

            if (y2 == y1) y1 = y2 - 2;
            _lineWick.StrokeThickness = _series._strokeThickness;
            _lineWick.Stroke = new SolidColorBrush(_series._strokeColor);
            _lineWick.X1 = _lineWick.X2 = x;
            _lineWick.Y1 = y1;
            _lineWick.Y2 = y2;
            _lineWick.Tag = _series;
        }

        internal void PaintCandle()
        {
            ChartPanel chartPanel = _series._chartPanel;
            StockChartX chartX = chartPanel._chartX;

            double x = chartX.GetXPixel(_index);

            Rect rc;
            double y1;
            double y2;

            CalculateXValues();

            _rectCandle.Tag = _series;
            if (_xOpen > _xClose)
            {
                y1 = _series.GetY(_xOpen);
                y2 = _series.GetY(_xClose);
                if (y1 + 3 > y2) y2 += 2;
                rc = new Rect(x - _space - _halfwick, y1, 2 * (_space + _halfwick), y2 - y1);
                _rectCandle.Fill = chartX.GetBarBrush(_index + chartX._startIndex, _down);
                if (_candleDownOutline == null)
                    _rectCandle.StrokeThickness = 0;
                else
                {
                    _rectCandle.StrokeThickness = 1;
                    _rectCandle.Stroke = _candleDownOutline;
                }

                Canvas.SetLeft(_rectCandle, rc.Left);
                Canvas.SetTop(_rectCandle, rc.Top);
                _rectCandle.Width = rc.Width;
                _rectCandle.Height = rc.Height;
            }
            else if (_xOpen < _xClose)
            {
                y1 = _series.GetY(_xClose);
                y2 = _series.GetY(_xOpen);
                if (y1 + 3 > y2) y2 += 2;
                rc = new Rect(x - _space - _halfwick, y1, 2 * (_space + _halfwick), y2 - y1);
                _rectCandle.Fill = chartX.GetBarBrush(_index + chartX._startIndex, _up);
                if (_candleUpOutline == null)
                    _rectCandle.StrokeThickness = 0;
                else
                {
                    _rectCandle.StrokeThickness = 1;
                    _rectCandle.Stroke = _candleUpOutline;
                }
                Canvas.SetLeft(_rectCandle, rc.Left);
                Canvas.SetTop(_rectCandle, rc.Top);
                _rectCandle.Width = rc.Width;
                _rectCandle.Height = rc.Height;
            }
            else
            {
                y1 = _series.GetY(_xClose);
                y2 = _series.GetY(_xOpen);
                if (y2 == y1) y1 = y2 - 1;
                rc = new Rect(x - _space - _halfwick, y1, 2 * (_space + _halfwick), y2 - y1);
                _rectCandle.Fill = chartX.GetBarBrush(_index + chartX._startIndex, _down);
                _rectCandle.StrokeThickness = 0;
                Canvas.SetLeft(_rectCandle, rc.Left);
                Canvas.SetTop(_rectCandle, rc.Top);
                _rectCandle.Width = rc.Width;
                _rectCandle.Height = rc.Height;
            }
        }

        internal void RePaint()
        {
            PaintWick();
            PaintCandle();
        }

        public void SetArgs(params object[] args)
        {
            Debug.Assert(args.Length == 2);
            _up = (Brush)args[0];
            _down = (Brush)args[1];
        }

        public void AddTo(Canvas canvas)
        {
            _lineWick = new System.Windows.Shapes.Line();
            canvas.Children.Add(_lineWick);
            _rectCandle = new System.Windows.Shapes.Rectangle();
            canvas.Children.Add(_rectCandle);
        }

        public void RemoveFrom(Canvas canvas)
        {
            Debug.Assert(_lineWick != null);
            Debug.Assert(_rectCandle != null);

            //      _high.PropertyChanged -= Wick_OnPropertyChanged;
            //      _low.PropertyChanged -= Wick_OnPropertyChanged;
            //
            //      _open.PropertyChanged -= Candle_OnPropertyChanged;
            //      _close.PropertyChanged -= Candle_OnPropertyChanged;
            //
            canvas.Children.Remove(_lineWick);
            canvas.Children.Remove(_rectCandle);
        }

        public int ZIndex
        {
            get { return Canvas.GetZIndex(_lineWick); }
            set
            {
                Canvas.SetZIndex(_lineWick, value);
                Canvas.SetZIndex(_rectCandle, value + 1);
            }
        }

        private void CalculateXValues()
        {
            StockChartX chartX = _series._chartPanel._chartX;

            _xClose = (double)((_open.Value + _high.Value + _low.Value + _close.Value) / 4);
            _xOpen = (double)(_index + chartX._startIndex == 0 ? _open.Value :
              (_high._collectionOwner[_index + chartX._startIndex - 1].Value + _low._collectionOwner[_index + chartX._startIndex - 1].Value) / 2);
            _xHigh = Math.Max(_high.Value.Value, Math.Max(_xOpen, _xClose));
            _xLow = Math.Min(_low.Value.Value, Math.Max(_xOpen, _xClose));
        }

        public void SetBrushes(Brush up, Brush down, Brush candleUpOutline, Brush candleDownOutline)
        {
            _up = up;
            _down = down;
            _candleUpOutline = candleUpOutline;
            _candleDownOutline = candleDownOutline;
        }
    }
}

