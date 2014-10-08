using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_FibonacciArcs()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.FibonacciArcs, typeof(FibonacciArcs), "Fibonacci Arcs");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Fibonacci Arcs line study
    /// </summary>
    public partial class FibonacciArcs : LineStudy, IContextAbleLineStudy
    {
        private readonly System.Windows.Shapes.Ellipse[] _ellipses = new System.Windows.Shapes.Ellipse[3];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public FibonacciArcs(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.FibonacciArcs;
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            rect.Normalize();

            if (lineStatus == LineStatus.StartPaint)
            {
                _internalObjectCreated = true;

                for (int i = 0; i < _ellipses.Length; i++)
                {
                    _ellipses[i] =
                      new System.Windows.Shapes.Ellipse
                        {
                            Tag = this,
                            Stroke = Stroke,
                            StrokeThickness = StrokeThickness,
                            Fill = Brushes.Transparent
                        };
                    C.Children.Add(_ellipses[i]);
                }

                new ContextLine(this);

                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));

                return;
            }

            double elSize = Math.Max(rect.Width, rect.Height);

            Rect rc = new Rect(rect.Left, rect.Top, elSize, elSize);
            for (int i = 0; i < _ellipses.Length; i++)
            {
                if (rc.IsEmpty) break;
#if SILVERLIGHT
                rc.X -= -elSize * 0.1;
                rc.Y -= -elSize * 0.1;
                rc.Width += 2 * (-elSize * 0.1);
                rc.Height += 2 * (-elSize * 0.1);
#endif
                Canvas.SetLeft(_ellipses[i], rc.Left);
                Canvas.SetTop(_ellipses[i], rc.Top);
                _ellipses[i].Height = rc.Width;
                _ellipses[i].Width = rc.Height;
#if WPF
        rc.Inflate(-elSize * 0.1, -elSize * 0.1);
#endif
            }

            foreach (System.Windows.Shapes.Ellipse ellipse in _ellipses)
            {
                Canvas.SetZIndex(ellipse, ZIndexConstants.LineStudies1);
            }
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            List<SelectionDotInfo> res =
              new List<SelectionDotInfo>
          {
            new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
            new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = _newRect.TopRight},
            new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
            new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
          };
            return res;
        }

        internal override void SetCursor()
        {
            if (_selectionVisible)
            {
                foreach (System.Windows.Shapes.Ellipse ellipse in _ellipses)
                {
                    ellipse.Cursor = Cursors.Hand;
                }
                return;
            }
            if (_selectionVisible || _ellipses[0].Cursor == Cursors.Arrow) return;
            foreach (System.Windows.Shapes.Ellipse ellipse in _ellipses)
            {
                ellipse.Cursor = Cursors.Arrow;
            }
        }

        internal override void SetStrokeThickness()
        {
            foreach (System.Windows.Shapes.Ellipse ellipse in _ellipses)
            {
                ellipse.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            foreach (System.Windows.Shapes.Ellipse ellipse in _ellipses)
            {
                ellipse.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            foreach (System.Windows.Shapes.Ellipse ellipse in _ellipses)
            {
                Types.SetShapePattern(ellipse, StrokeType);
            }
        }

        internal override void SetOpacity()
        {
            foreach (var ellipse in _ellipses)
            {
                ellipse.Opacity = Opacity;
            }
        }

        internal override void RemoveLineStudy()
        {
            foreach (System.Windows.Shapes.Ellipse ellips in _ellipses)
            {
                C.Children.Remove(ellips);
            }
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _ellipses[0]; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_newRect.Left, _newRect.Top, _newRect.Right, _newRect.Bottom); }
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


