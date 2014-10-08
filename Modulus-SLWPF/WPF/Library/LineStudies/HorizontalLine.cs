using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;
using Line = System.Windows.Shapes.Line;
#if WPF
using System.Windows.Threading;
#endif
   
namespace ModulusFE
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_HorizontalLine()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.HorizontalLine, typeof(HorizontalLine), "Horizontal Line");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Horizontal line
    /// </summary>
    public partial class HorizontalLine : LineStudy, IContextAbleLineStudy, IMouseAble
    {
        private Line _line;
        private HorizontalLineStudyValuePresenter _vp;
        private bool _textVisible;
        public bool Alert { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public HorizontalLine(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.HorizontalLine;
            _textVisible = true;
        }

        internal override void SetArgs(params object[] args)
        {
            _extraArgs = args;

            if (args == null) return;

            if (args.Length > 0)
                _textVisible = Convert.ToBoolean(args[0]);
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (_valuePresenter == null && lineStatus != LineStatus.StartPaint)
            {
                DrawLineStudy(rect, LineStatus.StartPaint);
                return;
            }

            if (lineStatus == LineStatus.StartPaint)
            {
                _line = new Line { Stroke = Stroke, StrokeThickness = 1, Tag = this };
                Types.SetShapePattern(_line, StrokeType);
                C.Children.Add(_line);

                Canvas.SetZIndex(_line, ZIndexConstants.LineStudies1);
                _line.MouseLeftButtonDown += (sender, args) => MouseDown(sender, args);
                _line.MouseEnter += (sender, args) => MouseEnter(sender, args);
                _line.MouseLeave += (sender, args) => MouseLeave(sender, args);
                _line.MouseMove += (sender, args) => MouseMove(sender, args);
                _line.MouseLeftButtonUp += (sender, args) => MouseUp(sender, args);

                CreateValuePresenter();

                if (LineStudyValue == null)
                {
                    LineStudyValue = new HorizontalLineDefStudyValue
                                       {
                                           FontFamily = new FontFamily(_chartX.FontFace),
                                           FontSize = _chartX.FontSize,
                                           Foreground = _chartX.FontForeground,
                                       };
                }

                new ContextLine(this);

                _internalObjectCreated = true;

                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));

                return;
            }

            _line.X1 = 0;
            _line.X2 = C.ActualWidth;
            _line.Y1 = _line.Y2 = rect.Bottom;

            //Debug.WriteLine("Bottom: " + rect.Bottom);

            SetDisplayValue();
        }

        private void SetDisplayValue()
        {
            if (_line == null || _vp == null)
            {
                return;
            }

            Action a
              = () =>
                  {

                      _vp.Visibility = _textVisible ? Visibility.Visible : Visibility.Collapsed;

                      if (!_textVisible) return;

                      LineStudyValue.NotifyDataChanged(_chartPanel.GetReverseY(_line.Y2));

#if WPF
              if (!_vp.IsLoaded)
#endif
                      _vp.UpdateLayout();

                      switch (ValuePresenterAlignment)
                      {
                          case ValuePresenterAlignmentType.Right:
                              Canvas.SetLeft(_vp, C.ActualWidth - _vp.ActualWidth);
                              break;
                          case ValuePresenterAlignmentType.Left:
                              Canvas.SetLeft(_vp, 0);
                              break;
                      }

                      Canvas.SetTop(_vp, _line.Y2 - _vp.ActualHeight);
                  };

#if WPF
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, a);
#endif
#if SILVERLIGHT
            a();
#endif
        }

        private void CreateValuePresenter()
        {
            if (_valuePresenter != null)
            {
                return;
            }

            _valuePresenter = new HorizontalLineStudyValuePresenter
                                {
                                    FontFamily = new FontFamily(_chartX.FontFace),
                                    FontSize = _chartX.FontSize,
                                    Foreground = _chartX.FontForeground,
                                    Tag = this,
                                    Visibility = Visibility.Collapsed
                                };
            _vp = (HorizontalLineStudyValuePresenter)_valuePresenter;

            if (_chartX.HorizontalLineValuePresenterTemplate != null)
            {
                _vp.SetValue(Control.TemplateProperty, _chartX.HorizontalLineValuePresenterTemplate);
            }

            C.Children.Add(_vp);
            Canvas.SetZIndex(_vp, ZIndexConstants.LineStudies1);

            if (LineStudyValue != null)
            {
                _vp.DataContext = LineStudyValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ReSetLineStudyValue()
        {
            //set outside of if. cause it can be set by client program. 
            //this code is called only once
            LineStudyValue.AttachDataSupplier(this, new[] { typeof(double) });
            if (_vp != null)
            {
                _vp.DataContext = LineStudyValue;
            }

            SetDisplayValue();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void XYValuesChanged()
        {
            SetDisplayValue();
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo
                   {Corner = Types.Corner.BottomCenter, Position = new Point(C.ActualWidth / 2, _y2)}
               };
        }

        internal override void SetCursor()
        {
            if (_selectionVisible)
            {
                _line.Cursor = Cursors.Hand;
                return;
            }

            if (_selectionVisible || _line.Cursor == Cursors.Arrow)
            {
                return;
            }

            _line.Cursor = Cursors.Arrow;
        }

        internal override void SetStrokeThickness()
        {
            if (_line != null)
            {
                _line.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            if (_line != null)
            {
                _line.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            if (_line != null)
            {
                Types.SetShapePattern(_line, StrokeType);
            }
        }

        internal override void RemoveLineStudy()
        {
            C.Children.Remove(_line);
            C.Children.Remove(_vp);
        }

        internal override void SetOpacity()
        {
            _line.Opacity = _vp.Opacity = Opacity;
        }

        /// <summary>
        /// Gets whether the underlying <see cref="LineStudy"/> is visible.
        /// </summary>
        /// <returns></returns>
        public override LSVisibility IsCurrentlyVisible()
        {
            if (_line == null)
            {
                return LSVisibility.Unknown;
            }

            double price = _chartPanel.GetReverseY(_line.Y2);

            if (price >= _chartPanel.Max)
            {
                return LSVisibility.NotVisible_Above;
            }

            if (price <= _chartPanel.Min)
            {
                return LSVisibility.NotVisible_Below;
            }

            return LSVisibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public override void GetYMinMax(out double min, out double max)
        {
            if (_line == null)
            {
                min = max = 0.0;
            }
            else
            {
                min = max = _chartPanel.GetReverseY(_line.Y2);
            }
        }

        ///<summary>
        /// Ensure this lineStudy is visible
        ///</summary>
        ///<param name="position"></param>
        public override void EnsureVisible(EnsureVisibilityPosition position)
        {
            int startPos = _chartX.FirstVisibleRecord;
            int endPos = _chartX.LastVisibleRecord;
            int windowSize = _chartX.LastVisibleRecord - _chartX.FirstVisibleRecord - 1;
            int bestStartPos = startPos;
            double bestFit = double.MaxValue;
            double lineValue = _chartPanel.GetReverseY(_line.Y2);
            double min, max;

            List<int> seriesIdxs = new List<int>();
            seriesIdxs.AddRange(_chartPanel.SeriesCollection.Select(_ => _.SeriesIndex));

            //try left
            while (startPos >= 0)
            {
                _chartX._dataManager.MinMaxFromInterval(seriesIdxs, startPos, endPos, out min, out max);
                double m = (max + min) / 2;
                double diff = Math.Abs(m - lineValue);
                if (diff < bestFit)
                {
                    bestFit = diff;
                    bestStartPos = startPos;
                }

                startPos--;
                endPos--;
            }

            //go right
            startPos = _chartX.FirstVisibleRecord;
            endPos = _chartX.LastVisibleRecord;
            while (endPos < _chartX.RecordCount)
            {
                _chartX._dataManager.MinMaxFromInterval(seriesIdxs, startPos, endPos, out min, out max);
                double m = (max + min) / 2;
                double diff = Math.Abs(m - lineValue);
                if (diff < bestFit)
                {
                    bestFit = diff;
                    bestStartPos = startPos;
                }

                startPos++;
                endPos++;
            }

            if (bestFit == double.MaxValue) //means no records
            {
                return;
            }

            _chartX._startIndex = bestStartPos;
            _chartX._endIndex = bestStartPos + windowSize;

            _chartX.ResetYScale(_chartPanel.Index);

            _chartX.Update();
        }

        /// <summary>
        /// Shows the LineStudy
        /// </summary>
        /// <param name="show"></param>
        protected override void ShowInternal(bool show)
        {
            _line.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            _valuePresenter.Show(show);
        }

        /// <summary>
        /// Gets the boundaries that are needed for a LineStudy to become visible
        /// </summary>
        public override Types.RectEx NeededVisibleBounds
        {
            get
            {
                if (_line == null)
                {
                    return Types.RectEx.Empty;
                }

                return new Types.RectEx(
                  double.MinValue, _chartPanel.GetReverseY(_line.Y2 - 2),
                  double.MaxValue, _chartPanel.GetReverseY(_line.Y2 + 2));
            }
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _line; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_line.X1, _line.Y1, _line.X2, _line.Y2).Inflate(-20); }
        }

        /// <summary>
        /// Parent where <see cref="IContextAbleLineStudy.Element"/> belongs
        /// </summary>
        public Canvas Parent
        {
            get { return C; }
        }

        /// <summary>
        /// Gets if <see cref="IContextAbleLineStudy.Element"/> is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Z Index of <see cref="IContextAbleLineStudy.Element"/>
        /// </summary>
        public int ZIndex
        {
            get { return ZIndexConstants.LineStudies1; }
        }

        /// <summary>
        /// Gets the chart object associated with <see cref="IContextAbleLineStudy.Element"/> object
        /// </summary>
        public StockChartX Chart
        {
            get { return _chartX; }
        }

        /// <summary>
        /// Gets the reference to <see cref="LineStudies.LineStudy"/> 
        /// </summary>
        public LineStudy LineStudy
        {
            get { return this; }
        }

        #endregion

        ///<summary>
        ///</summary>
        public event MouseButtonEventHandler MouseDown = delegate { };
        ///<summary>
        ///</summary>
        public event MouseEventHandler MouseEnter = delegate { };
        ///<summary>
        ///</summary>
        public event MouseEventHandler MouseLeave = delegate { };
        ///<summary>
        ///</summary>
        public event MouseEventHandler MouseMove = delegate { };
        ///<summary>
        ///</summary>
        public event MouseButtonEventHandler MouseUp = delegate { };

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_line == null)
            {
                return string.Empty;
            }

            var c = _chartX;
            string scalePrecision = ".00";
            if (c.ScalePrecision > 0)
            {
                scalePrecision = ".".PadRight(c.ScalePrecision + 1, '0');
            }

            return _chartPanel.GetReverseY(_line.Y2).ToString(scalePrecision);
        }
    }

    ///<summary>
    /// Default class for value representation of HorizontalLineStudy
    ///</summary>
    public partial class HorizontalLineDefStudyValue : Interfaces.IValueBridge<LineStudy>
    {
        private LineStudy _lineStudy;

        #region Implementation of ILineStudyValue

        /// <summary>
        /// Attaches data supplier
        /// </summary>
        /// <param name="lineStudy"></param>
        /// <param name="parameterTypes"></param>
        public void AttachDataSupplier(LineStudy lineStudy, Type[] parameterTypes)
        {
            _lineStudy = lineStudy;
        }

        /// <summary>
        /// Notifies that internal data was changed
        /// </summary>
        /// <param name="values"></param>
        public void NotifyDataChanged(params object[] values)
        {
            //Debug.WriteLine("Value: " + _lineStudy);

            Value = _lineStudy.ToString();
        }

        #endregion

        private string _value;
        ///<summary>
        /// Value
        ///</summary>
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Value"));
            }
        }

        #region Foreground Partial Property

        private Brush _foreground;
        partial void ForegroundChanged();

        ///<summary>
        ///</summary>
        public Brush Foreground
        {
            get { return _foreground; }
            set
            {
                if (_foreground == value)
                    return;
                _foreground = value;
                ForegroundChanged();
            }
        }

        #endregion

        #region FontSize Partial Property

        private double _fontSize;
        partial void FontSizeChanged();

        ///<summary>
        ///</summary>
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize == value)
                    return;
                _fontSize = value;
                FontSizeChanged();
            }
        }

        #endregion

        #region FontFamily Partial Property

        private FontFamily _fontFamily;
        partial void FontFamilyChanged();

        ///<summary>
        ///</summary>
        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (_fontFamily == value)
                    return;
                _fontFamily = value;
                FontFamilyChanged();
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        ///<summary>
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        #endregion

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Convert.ToString(_value);
        }
    }
}
