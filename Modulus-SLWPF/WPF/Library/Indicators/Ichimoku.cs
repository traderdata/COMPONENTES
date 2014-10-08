using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;
using System;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_Ichimoku()
        {
            RegisterIndicatorParameters(IndicatorType.Ichimoku, typeof(Indicators.Ichimoku),
                                        "Ichimoku",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Symbol (eg "msft")
                                          Name = Indicator.GetParamName(ParameterType.ptSymbol),
                                          ParameterType = ParameterType.ptSymbol,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods1),
                                          ParameterType = ParameterType.ptPeriods1,
                                          DefaultValue = 9,
                                          ValueType = typeof (int),
                                        },
                                      new IndicatorParameter
                                        {
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods2),
                                          ParameterType = ParameterType.ptPeriods2,
                                          DefaultValue = 26,
                                          ValueType = typeof (int),
                                        },
                                      new IndicatorParameter
                                        {
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods3),
                                          ParameterType = ParameterType.ptPeriods3,
                                          DefaultValue = 52,
                                          ValueType = typeof (int),
                                        },
                                    });
        }
    }
}


namespace ModulusFE.Indicators
{
    /// <summary>
    /// Ichimoku Kinko Hyo is a technical indicator published over 30 years ago in Japan. It measures market 
    /// momentum and trend and also outlines levels of support and resistance. Ichimoku means ‘one look’ 
    /// in Japanese and this reflects the indicators intent to measure multiple aspects of the market at once. 
    /// This indicator was developed so that a trader can gauge an asset’s trend, momentum and support and 
    /// resistance points without the need of any other technical indicator.
    /// </summary>
    public class Ichimoku : Indicator
    {
        private Recordset _calcs;
        private Path _leadingSpanA;
        private Path _leadingSpanB;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public Ichimoku(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.Ichimoku;

            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool TrueAction()
        {
            int size = _chartPanel._chartX.RecordCount;
            if (size == 0)
            {
                return false;
            }

            int paramInt1 = ParamInt(1);
            if (paramInt1 > size)
            {
                return false;
            }

            int paramInt2 = ParamInt(2);
            if (paramInt2 > size)
            {
                return false;
            }

            int paramInt3 = ParamInt(3);
            if (paramInt3 > size)
            {
                return false;
            }

            string symbol = ParamStr(0);

            Field pHigh = SeriesToField("High", symbol + ".high", size);
            if (!EnsureField(pHigh, symbol + ".high")) return false;
            Field pLow = SeriesToField("Low", symbol + ".low", size);
            if (!EnsureField(pLow, symbol + ".low")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);

            pNav.Recordset_ = pRS;

            Tasdk.Ichimoku ichimoku = new Tasdk.Ichimoku();
            _calcs = ichimoku.Calc(pNav, pHigh, pLow, paramInt1, paramInt2, paramInt3);

            TwinIndicator baseLine = (TwinIndicator)EnsureSeries(FullName + " BL");
            TwinIndicator leadingSpanA = (TwinIndicator)EnsureSeries(FullName + "LSA");
            TwinIndicator leadingSpanB = (TwinIndicator)EnsureSeries(FullName + "LSP");
            TwinIndicator lagginSpan = (TwinIndicator)EnsureSeries(FullName + "LS");

            for (int i = 0; i < size; i++)
            {
                DateTime dt = DM.TS(i);
                AppendValue(dt, i < paramInt1 ? null : _calcs.Value("Conversion Line", i + 1));
                baseLine.AppendValue(dt, i < paramInt2 ? null : _calcs.Value("Base Line", i + 1));
                leadingSpanA.AppendValue(dt, i < paramInt2 ? null : _calcs.Value("Leading Span A", i + 1));
                leadingSpanB.AppendValue(dt, i < paramInt2 ? null : _calcs.Value("Leading Span B", i + 1));
                //lagginSpan.AppendValue(dt, _calcs.Value("Lagging Span"));
            }

            return true;
        }

        internal override void Paint()
        {
            if (!_visible || RecordCount == 0 || RecordCount < _chartPanel._chartX._startIndex)
            {
                return;
            }

            EnsurePathsCreated();

            GeometryGroup gLeadingSpanA = new GeometryGroup();
            GeometryGroup gLeadingSpanB = new GeometryGroup();

            double x2 = _chartPanel._chartX.GetXPixel(0);
            double? y1;
            double? y2 = null;
            int cnt = 0;

            for (int i = _chartPanel._chartX._startIndex; i < _chartPanel._chartX._endIndex; i++, cnt++)
            {
                double x1 = _chartPanel._chartX.GetXPixel(cnt);
                double? value = y1 = this[i].Value;
                if (!y1.HasValue)
                {
                    continue;
                }

                y1 = GetY(y1.Value);
                if (i > 0 && i == _chartPanel._chartX._startIndex)
                {
                    y2 = y1.Value;
                }
            }
        }

        private void EnsurePathsCreated()
        {
            if (_leadingSpanA != null)
            {
                return;
            }

            _leadingSpanA = new Path();
            _leadingSpanA.SetBinding(Shape.StrokeProperty, this.CreateOneWayBinding("LeadingSpanAStroke"));
            _leadingSpanA.SetBinding(Shape.StrokeThicknessProperty, this.CreateOneWayBinding("LeadingSpansStrokeThickness"));

            _leadingSpanB = new Path();
            _leadingSpanB.SetBinding(Shape.StrokeProperty, this.CreateOneWayBinding("LeadingSpanBStroke"));
            _leadingSpanB.SetBinding(Shape.StrokeThicknessProperty, this.CreateOneWayBinding("LeadingSpansStrokeThickness"));

            var c = _chartPanel._rootCanvas;
            c.Children.Add(_leadingSpanA);
            c.Children.Add(_leadingSpanB);

            Canvas.SetZIndex(_leadingSpanA, ZIndexConstants.Indicators1);
            Canvas.SetZIndex(_leadingSpanB, ZIndexConstants.Indicators1);
        }

        #region UpCloudFill

        private Brush _upCloudFill;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs UpCloudFillChangedEventsArgs =
          ObservableHelper.CreateArgs<Ichimoku>(_ => _.UpCloudFill);

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to fill Up Cloud
        /// </summary>
        public Brush UpCloudFill
        {
            get { return _upCloudFill; }
            set
            {
                if (_upCloudFill != value)
                {
                    _upCloudFill = value;
                    InvokePropertyChanged(UpCloudFillChangedEventsArgs);
                }
            }
        }

        #endregion

        #region DownCloudFill

        private Brush _downCloudFill;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs DownCloudFillChangedEventsArgs =
          ObservableHelper.CreateArgs<Ichimoku>(_ => _.DownCloudFill);

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> used to fill Down Cloud
        /// </summary>
        public Brush DownCloudFill
        {
            get { return _downCloudFill; }
            set
            {
                if (_downCloudFill != value)
                {
                    _downCloudFill = value;
                    InvokePropertyChanged(DownCloudFillChangedEventsArgs);
                }
            }
        }

        #endregion

        #region LeadingSpanAStroke

        private Brush _leadingSpanAStroke = new SolidColorBrush(Colors.Green);

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs LeadingSpanAStrokeChangedEventsArgs =
          ObservableHelper.CreateArgs<Ichimoku>(_ => _.LeadingSpanAStroke);

        /// <summary>
        /// Gets or sets the stroke color for LeadingSpanA
        /// </summary>
        public Brush LeadingSpanAStroke
        {
            get { return _leadingSpanAStroke; }
            set
            {
                if (_leadingSpanAStroke != value)
                {
                    _leadingSpanAStroke = value;
                    InvokePropertyChanged(LeadingSpanAStrokeChangedEventsArgs);
                }
            }
        }

        #endregion

        #region LeadingSpanBStroke

        private Brush _leadingSpanBStroke = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs LeadingSpanBStrokeChangedEventsArgs =
          ObservableHelper.CreateArgs<Ichimoku>(_ => _.LeadingSpanBStroke);

        /// <summary>
        /// Gets or sets the stroke color for LeadingSpanB
        /// </summary>
        public Brush LeadingSpanBStroke
        {
            get { return _leadingSpanBStroke; }
            set
            {
                if (_leadingSpanBStroke != value)
                {
                    _leadingSpanBStroke = value;
                    InvokePropertyChanged(LeadingSpanBStrokeChangedEventsArgs);
                }
            }
        }

        #endregion

        #region LeadingSpansStrokeThickness

        private double _leadingSpansStrokeThickness = 1.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs LeadingSpansStrokeThicknessChangedEventsArgs =
          ObservableHelper.CreateArgs<Ichimoku>(_ => _.LeadingSpansStrokeThickness);

        /// <summary>
        /// Gets or sets the stroke color for LeadingSpans
        /// </summary>
        public double LeadingSpansStrokeThickness
        {
            get { return _leadingSpansStrokeThickness; }
            set
            {
                if (_leadingSpansStrokeThickness != value)
                {
                    _leadingSpansStrokeThickness = value;
                    InvokePropertyChanged(LeadingSpansStrokeThicknessChangedEventsArgs);
                }
            }
        }

        #endregion
    }
}
