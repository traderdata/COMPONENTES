using System;
using System.Collections.Generic;
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
        internal static void Register_VerticalLine()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.VerticalLine, typeof(VerticalLine), "Vertical Line");
        }
    }
}


namespace ModulusFE.LineStudies
{
    ///<summary>
    /// Vertical line
    ///</summary>
    public partial class VerticalLine : LineStudy, IContextAbleLineStudy
    {
        private readonly Line _line;
        private TextBlock _txt;
        private bool _showRecordNumber = true;
        private bool _showLineText = true;
        private string _customDateFormat;
        private ContextLine _contextLine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public VerticalLine(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.VerticalLine;
            _line = new Line();
        }


        internal override void SetArgs(params object[] args)
        {
            _extraArgs = args;

            _showRecordNumber = true;
            _showLineText = true;
            if (args.Length > 0)
                _showRecordNumber = Convert.ToBoolean(args[0]);
            if (args.Length > 1)
                _showLineText = Convert.ToBoolean(args[1]);
            if (args.Length > 2)
                _customDateFormat = Convert.ToString(args[2]);
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (_txt == null && lineStatus != LineStatus.StartPaint)
                DrawLineStudy(rect, LineStatus.StartPaint);
            if (lineStatus == LineStatus.StartPaint)
            {
                _line.Stroke = Stroke;
                _line.StrokeThickness = StrokeThickness;
								Types.SetShapePattern(_line, StrokeType);
                _line.Tag = this;

                C.Children.Add(_line);
                Canvas.SetZIndex(_line, ZIndexConstants.LineStudies1);

                _txt = new TextBlock
                         {
                             FontFamily = new FontFamily(_chartX.FontFace),
                             FontSize = _chartX.FontSize,
                             Foreground = Stroke,
                             Tag = this
                         };
                C.Children.Add(_txt);
                Canvas.SetZIndex(_txt, ZIndexConstants.LineStudies1);

                if (_contextLine == null)
                {
                    _contextLine = new ContextLine(this);
                    _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
                }

                _internalObjectCreated = true;

                return;
            }

            if (_txt == null)
            {
                return;
            }

            _txt.Visibility = _showLineText ? Visibility.Visible : Visibility.Collapsed;

            int recordIndex = (int)_chartX.GetReverseXInternal(rect.Right) + _chartX._startIndex;
            if (_showLineText)
            {
                if (_showRecordNumber)
                {
                    _txt.Text = (recordIndex + 1).ToString(); //+1 cause we need record number, not record index
                }
                else
                {
                    DateTime timestamp = _chartPanel._chartX._dataManager.GetTimeStampByIndex(recordIndex);
                    _txt.Text = string.IsNullOrEmpty(_customDateFormat) ? timestamp.ToString() : timestamp.ToString(_customDateFormat);
                }

                Canvas.SetTop(_txt, 2);
                double textWidth = _chartX.GetTextWidth(_txt.Text);
                if (rect.Right + textWidth + 2 < C.ActualWidth)
                {
                    Canvas.SetLeft(_txt, rect.Right + 2);
                }
                else
                {
                    Canvas.SetLeft(_txt, rect.Right - textWidth - 2);
                }
            }

            _line.X1 = _line.X2 = rect.Right;
            _line.Y1 = 0;
            _line.Y2 = C.ActualHeight;
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.MiddleRight,
                     Position = new Point(_x2, C.ActualHeight/2)
                   }
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
            _line.StrokeThickness = StrokeThickness;
        }

        internal override void SetStroke()
        {
            _line.Stroke = Stroke;
            _txt.Foreground = Stroke;
        }

        internal override void SetStrokeType()
        {
            Types.SetShapePattern(_line, StrokeType);
        }

        internal override void RemoveLineStudy()
        {
            C.Children.Remove(_line);
            C.Children.Remove(_txt);
        }

        internal override void SetOpacity()
        {
            _line.Opacity = Opacity;
            _txt.Opacity = Opacity;
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
