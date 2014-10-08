using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_SimpleMovingAverage()
        {
            /*  Required inputs for this indicator:
              1. paramStr[0] = Source (eg "msft.close")
              2. paramInt[1] = Periods (eg 14)
            */
            RegisterIndicatorParameters(IndicatorType.SimpleMovingAverage, typeof(SimpleMovingAverage),
                                        "Simple Moving Average",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Source (eg "msft.close")
                                          Name = Indicator.GetParamName(ParameterType.ptSource),
                                          ParameterType = ParameterType.ptSource,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {  
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods),
                                          ParameterType = ParameterType.ptPeriods,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Simple Moving Average is simply an average of values over a specified period of time.
    /// </summary>
    /// <remarks>A Moving Average is most often used to average values for a smoother representation of the underlying price or indicator.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class SimpleMovingAverage : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public SimpleMovingAverage(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.SimpleMovingAverage;

            Init();
        }

        /// <summary>
        /// Action to be executd for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            // Validate
            int size = _chartPanel._chartX.RecordCount;
            if (size == 0)
                return false;
            if (_params.Count < _parameters.Count)
                return false;

            int iParam = ParamInt(1);
            if (iParam < 1 || iParam > size )
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            Field pSource = SeriesToField("Source", ParamStr(0), size);
            if (!EnsureField(pSource, ParamStr(0)))
                return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;

            // Calculate the indicator
            MovingAverage ta = new MovingAverage();
            Recordset pInd = ta.SimpleMovingAverage(pNav, pSource, iParam, _name);

            // Output the indicator values
            Clear();
            for (int n = 0; n < size; n++)
            {
                double? dValue = n <= iParam ? null : pInd.Value(_name, n + 1);
                AppendValue(DM.TS(n), dValue);
            }

            return _calculateResult = PostCalculate();
        }
    }
}
