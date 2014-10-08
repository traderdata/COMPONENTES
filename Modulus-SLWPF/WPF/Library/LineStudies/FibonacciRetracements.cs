using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        internal static void Register_FibonacciRetracements()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.FibonacciRetracements, typeof(FibonacciRetracements), "Fibonacci Retracements");
        }
    }
}

namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Fibonacci Retracements line study
    /// </summary>
    public partial class FibonacciRetracements : LineStudy, IContextAbleLineStudy
    {
        private const int LinesCount = 5;

        private readonly Line[] _lines = new Line[LinesCount + 1];
        private readonly TextBlock[] _txts = new TextBlock[LinesCount + 1];
        private Line _handle = new Line();
        private ContextLine _contextLine;
        private readonly List<double> _params = new List<double>();
        /// <summary>
        /// Used to determine the direction of sorting
        /// </summary>
        private double? _firstY;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public FibonacciRetracements(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.FibonacciRetracements;

            // decide how to sort values based on mouse position and first painting start point
            _firstY = null;
        }

        internal override void SetArgs(params object[] args)
        {
            _params.Clear();
            if (args == null || args.Length == 0)
            {
                return;
            }

            foreach (object o in args)
            {
                _params.Add(Convert.ToDouble(o));
            }
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            int i;
            if (lineStatus == LineStatus.StartPaint)
            {
                for (i = 0; i < LinesCount + 1; i++)
                {
                    _lines[i] = new Line
                                  {
                                      Stroke = Stroke,
                                      StrokeThickness = StrokeThickness,
                                      Tag = this,
                                  };
                    C.Children.Add(_lines[i]);
                    _txts[i] = new TextBlock
                                 {
                                     Foreground = _chartX.FontForeground,
                                     FontFamily = new FontFamily(_chartX.FontFace),
                                     FontSize = _chartX.FontSize,
                                     Tag = this,
                                 };
                    C.Children.Add(_txts[i]);

                    Canvas.SetZIndex(_lines[i], ZIndexConstants.LineStudies1);
                    Canvas.SetZIndex(_txts[i], ZIndexConstants.LineStudies1);
                }

                _handle = new Line
                            {
                                Stroke = Stroke,
                                StrokeThickness = StrokeThickness,
                                Tag = this
                            };
                C.Children.Add(_handle);
                Canvas.SetZIndex(_handle, ZIndexConstants.LineStudies1);

                if (_contextLine == null)
                {
                    _contextLine = new ContextLine(this);
                }

                _internalObjectCreated = true;
                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));

                return;
            }

            if (!_firstY.HasValue)
            {
                _firstY = rect.Top;
            }

            bool upsideDown = _firstY > rect.Bottom;

            if (lineStatus == LineStatus.Moving || lineStatus == LineStatus.Painting)
            {
                _handle.Visibility = Visibility.Visible;
                _handle.X1 = rect.Left;
                _handle.Y1 = rect.Top;
                _handle.X2 = rect.Right;
                _handle.Y2 = rect.Bottom;
            }
            else
            {
                _handle.Visibility = Visibility.Collapsed;
            }

            rect.Normalize();

            rect.Right = C.ActualWidth;

            double max = _chartPanel.GetY(rect.Top);
            rect.Right -= _chartX.GetTextWidth(string.Format("{0:f2}            ", max));

            //Comentado por Felipe em 29-08
            //if (_params.Count > 1)
            //{
            //    upsideDown = _params[0] < _params[1];
            //}
            if ((_params.Count>5)&&(_params[5] == 1))
                upsideDown = false;
            else
                upsideDown = !upsideDown;
            //bloco alterado por Felipe


            double fibNum = 1.618033;
            List<double> values = new List<double>();

            double minFib = double.MaxValue;
            double maxFib = double.MinValue;
            // prepare values to be painted
            //Thx to Rekhender Dhawan
            if (upsideDown)
            {
                // prepare values to be painted
                for (i = 0; i < LinesCount; i++)
                {
                    if (i < _params.Count)
                    {
                        fibNum = _params[i];
                    }
                    else
                    {
                        fibNum *= 0.618;
                    }

                    values.Add(fibNum);

                    minFib = Math.Min(minFib, fibNum);
                    maxFib = Math.Max(maxFib, fibNum);
                }
            }
            else
            {
                for (i = LinesCount - 1; i >= 0; i--)
                {
                    if (i < _params.Count)
                    {
                        fibNum = _params[i];
                    }
                    else
                    {
                        fibNum *= 0.618;
                    }

                    values.Add(fibNum);

                    minFib = Math.Min(minFib, fibNum);
                    maxFib = Math.Max(maxFib, fibNum);
                }
            }


            double k = maxFib - minFib;

            // paint values
            string formatString = "{0:f" + _chartX.ScalePrecision + "}";
            for (i = 0; i < LinesCount && k != 0; i++)
            {
                fibNum = values[i];

                double norm = (fibNum - minFib) / k;
                
                //Comenatado por Felipe em 29-08
                //if (upsideDown)
                //    norm = 1 - norm;

                double textTop = rect.Top + rect.Height * norm;

                _lines[i].X1 = rect.Left;
                _lines[i].Y1 = _lines[i].Y2 = textTop;
                _lines[i].X2 = rect.Right;

                //string strNum1 = string.Format("{0:0}%", !upsideDown ? (1 - norm) * 100.0 : norm * 100.0);
                string strNum1 = (!upsideDown) ? (values[(values.Count - 1) - i] * 100.0).ToString("N1") + "%" : (norm * 100.0).ToString("N1") + "%";

                double y = _chartPanel.GetReverseY(textTop);
                string strNum2 = string.Format(formatString, y);

                _txts[i].Text = string.Format("{0} ({1})", strNum2, strNum1);
                Canvas.SetLeft(_txts[i], rect.Right + 2);
                Canvas.SetTop(_txts[i], textTop);
            }

            _lines[i].X1 = rect.Left;
            _lines[i].Y1 = _lines[i].Y2 = rect.Bottom;
            _lines[i].X2 = rect.Right;
        }

        internal override void SetCursor()
        {
            if (_lines[0] == null)
            {
                return;
            }

            if (_selectionVisible)
            {
                foreach (Line line in _lines)
                {
                    line.Cursor = Cursors.Hand;
                }

                return;
            }

            if (_selectionVisible || _lines[0].Cursor == Cursors.Arrow)
            {
                return;
            }

            foreach (Line line in _lines)
            {
                line.Cursor = Cursors.Arrow;
            }
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
                 new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = _newRect.TopRight},
                 new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
               };
        }

        internal override void SetStrokeThickness()
        {
            foreach (Line line in _lines)
            {
                line.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            foreach (Line line in _lines)
            {
                line.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            foreach (Line line in _lines)
            {
                Types.SetShapePattern(line, StrokeType);
            }
        }

        internal override void RemoveLineStudy()
        {
            foreach (Line line in _lines)
            {
                C.Children.Remove(line);
            }

            foreach (TextBlock txt in _txts)
            {
                C.Children.Remove(txt);
            }

            C.Children.Remove(_handle);
        }

        internal override void SetOpacity()
        {
            foreach (var line in _lines)
            {
                line.Opacity = Opacity;
            }

            foreach (var txt in _txts)
            {
                txt.Opacity = Opacity;
            }
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _handle; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_newRect.Left, _newRect.Top, _newRect.Right, _newRect.Bottom).Normalize(); }
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
        /// A very efficient way to calculate Fibonacci number. Does not use recurssion, works in a linear time O(n)
        ///</summary>
        ///<param name="number"></param>
        ///<returns></returns>
        public static int CalculateFibonacciNumber(int number)
        {
            if (number == 0)
            {
                return 0;
            }

            if (number == 1)
            {
                return 1;
            }

            int n1 = 0, n2 = 1, r = 0;

            for (int i = 2; i <= number; i++)
            {
                r = n1 + n2;
                n1 = n2;
                n2 = r;
            }

            return r;
        }
    }
}
