using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;
using Line = System.Windows.Shapes.Line;

namespace ModulusFE
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_TrendLine()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.TrendLine, typeof(TrendLine), "Trend Line");
        }
    }
}


namespace ModulusFE.LineStudies
{
    ///<summary>
    /// Trend line. An arbitrary line painted by the user or by the programmer.
    ///</summary>
    public partial class TrendLine : LineStudy, IContextAbleLineStudy
    {
        private Line _line;
        private ContextLine _contextLine;
        private readonly Segment _segment = new Segment();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public TrendLine(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.TrendLine;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~TrendLine()
        {
            //      _chartPanel.UnRegisterWatchableTrendLine(this);
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (_line == null && lineStatus != LineStatus.StartPaint)
            {
                DrawLineStudy(rect, LineStatus.StartPaint);
            }

            if (lineStatus == LineStatus.StartPaint)
            {
                _line = new Line { Stroke = Stroke, StrokeThickness = StrokeThickness, Tag = this };
                Types.SetShapePattern(_line, StrokeType);
                C.Children.Add(_line);
                Canvas.SetZIndex(_line, ZIndexConstants.LineStudies1);

                if (_contextLine == null)
                {
                    _contextLine = new ContextLine(this);
                    _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
                }

                _internalObjectCreated = true;
                _segment.BindToLine(_line);

                return;
            }

            if (double.IsInfinity(rect.Top))
            {
                return;
            }

            //      _line.X1 = rect.Left;
            //      _line.Y1 = rect.Top;
            //
            //      _line.X2 = rect.Right;
            //      _line.Y2 = rect.Bottom;
            _segment.X1 = rect.Left;
            _segment.Y1 = rect.Top;
            _segment.X2 = rect.Right;
            _segment.Y2 = rect.Bottom;
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _segment.Start},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _segment.End},
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
                return;

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
            if (C != null)
            {
                C.Children.Remove(_line);
            }
        }

        internal override void SetOpacity()
        {
            _line.Opacity = Opacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paintableRect"></param>
        protected override void SetClipingAreaInternal(Rect? paintableRect)
        {
            if (_line != null && paintableRect != null && Clip)
            {
                _line.Clip = GetClip(paintableRect.Value.Left, paintableRect.Value.Top, paintableRect);
            }
        }

        ///<summary>
        /// Gets or sets whether the LineStudy will be clipsed by the paintable area of the chart.
        ///</summary>
        public bool Clip { get; set; }

        private bool _watchable;
        ///<summary>
        /// Gets or sets the watchable attribut for current trend line
        ///</summary>
        public bool WatchAble
        {
            get { return _watchable; }
            set
            {
                if (_watchable == value)
                {
                    return;
                }

                _watchable = value;
                _selectable = !value;

                if (_watchable)
                {
                    _chartPanel.RegisterWatchableTrendLine(this);
                }
                else
                {
                    _chartPanel.UnRegisterWatchableTrendLine(this);
                }
            }
        }

        /// <summary>
        /// Represents the information about intersection of a trenlinde and a bar from chart
        /// </summary>
        public class BarIntersection
        {
            /// <summary>
            /// Gets the Record number from X axis of intersection. 0-index based.
            /// </summary>
            public int Record { get; internal set; }

            /// <summary>
            /// Gets the price value from Y axis of intersection
            /// </summary>
            public double Price { get; internal set; }
        }
        /// <summary>
        /// Gets a collection of intersections between the current TrendLine and all the
        /// bars that this instance of trendline intersects
        /// </summary>
        public IEnumerable<BarIntersection> BarsIntersection
        {
            get
            {
                if (_segment.X2 == _segment.X1 || _segment.Length < 1)
                    yield break;

                Series open = _chartPanel.SeriesCollection.FirstOrDefault(s => s.OHLCType == SeriesTypeOHLC.Open);
                Series close = _chartPanel.SeriesCollection.FirstOrDefault(s => s.OHLCType == SeriesTypeOHLC.Close);

                if (open == null || close == null)
                    yield break;

                var chart = _chartX;
                int startIndex = chart.GetReverseX(Math.Min(_segment.X1, _segment.X2)) - 1 + chart._startIndex;
                int endIndex = chart.GetReverseX(Math.Max(_segment.X1, _segment.X2)) + chart._startIndex;
                startIndex = Math.Max(0, startIndex);
                endIndex = Math.Min(endIndex, chart.RecordCount);

                double slope = (_segment.Y1 - _segment.Y2) / (_segment.X1 - _segment.X2);

                //double k = slope * (_line.X1 - _line.X2);

                for (int i = startIndex; i < endIndex; i++)
                {
                    double y = _segment.Y1 - slope * (_segment.X1 - chart.GetXPixel(i - chart._startIndex));
                    double priceValue = _chartPanel.GetReverseY(y);

                    double? openValue = open[i].Value;
                    if (!openValue.HasValue)
                        continue;
                    double? closeValue = close[i].Value;
                    if (!closeValue.HasValue)
                        continue;

                    double min = Math.Min(openValue.Value, closeValue.Value);
                    double max = Math.Max(openValue.Value, closeValue.Value);

                    if (priceValue >= min && priceValue <= max)
                        yield return new BarIntersection
                                       {
                                           Record = i,
                                           Price = priceValue
                                       };
                }
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
    }
}
