using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_HighLowActivator()
        {
            /*  Required inputs for this indicator:
              1. paramStr[0] = Source (eg "msft.close")
              2. paramInt[1] = Periods (eg 14)
            */
            RegisterIndicatorParameters(IndicatorType.HighLowActivator, typeof(HighLowActivator),
                                        "High Low Activator",
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
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods),
                                          ParameterType = ParameterType.ptPeriods,
                                          DefaultValue = 3,
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
    public class HighLowActivator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public HighLowActivator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.HighLowActivator;

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
            Field pSourceHigh = SeriesToField("High", ParamStr(0) + ".high", size);
            Field pSourceLow = SeriesToField("Low", ParamStr(0) + ".low", size);
            Field pSourceClose = SeriesToField("Close", ParamStr(0) + ".close", size);

            if (!EnsureField(pSourceHigh, ParamStr(0) + ".high"))
                return false;

            if (!EnsureField(pSourceLow, ParamStr(0) + ".low"))
                return false;

            if (!EnsureField(pSourceClose, ParamStr(0) + ".close"))
                return false;

            Navigator pNavClose = new Navigator();
            Recordset pRSClose = new Recordset();

            pRSClose.AddField(pSourceClose);
            pNavClose.Recordset_ = pRSClose;

            Navigator pNavHigh = new Navigator();
            Recordset pRSHigh = new Recordset();
            
            pRSHigh.AddField(pSourceHigh);
            pNavHigh.Recordset_ = pRSHigh;

            Navigator pNavLow = new Navigator();
            Recordset pRSLow = new Recordset();

            pRSLow.AddField(pSourceLow);
            pNavLow.Recordset_ = pRSLow;

            // Calculate the indicator
            MovingAverage ta = new MovingAverage();
            Recordset pIndHigh = ta.SimpleMovingAverage(pNavHigh, pSourceHigh, iParam, "High");
            Recordset pIndLow = ta.SimpleMovingAverage(pNavLow, pSourceLow, iParam, "Low");

            // Output the indicator values
            Clear();
            for (int n = 0; n < size; n++)
            {
                double? dValue = 0;
                double? ultimo = pRSClose.Value("Close", n+1);
                double? mediaMaxima = pIndHigh.Value("High", n );
                double? mediaMinima = pIndLow.Value("Low", n );

                if (ultimo > mediaMaxima)
                    dValue = mediaMinima;
                else
                    dValue = mediaMaxima;
                                
                
                AppendValue(DM.TS(n), dValue);
            }

            return _calculateResult = PostCalculate();
        }
    }
}
