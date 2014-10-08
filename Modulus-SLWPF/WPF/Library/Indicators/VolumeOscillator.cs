using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_VolumeOscillator()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Volume (eg "msft.volume")
              2. paramInt1 = Short Term Period (eg 9)
              3. paramInt2 = Long Term Period (eg 21)   
              4. paramInt3 = Points or Percent (1 for points or 2 for percent)
            */
            RegisterIndicatorParameters(IndicatorType.VolumeOscillator, typeof(VolumeOscillator),
                                        "Volume Oscillator",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Volume (eg "msft.volume")
                                          Name = Indicator.GetParamName(ParameterType.ptVolume),
                                          ParameterType = ParameterType.ptVolume,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Short Term Period (eg 9)
                                          Name = Indicator.GetParamName(ParameterType.ptShortTerm),
                                          ParameterType = ParameterType.ptShortTerm,
                                          DefaultValue = 9,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Short Term Period (eg 9)
                                          Name = Indicator.GetParamName(ParameterType.ptLongTerm),
                                          ParameterType = ParameterType.ptLongTerm,
                                          DefaultValue = 21,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Points or Percent (1 for points or 2 for percent)
                                          Name = Indicator.GetParamName(ParameterType.ptPointsOrPercent),
                                          ParameterType = ParameterType.ptPointsOrPercent,
                                          DefaultValue = "Points",
                                          ValueType = typeof (string)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Volume Oscillator shows a spread of two different moving averages of volume over a specified period of time.
    /// </summary>
    /// <remarks>The Volume Oscillator offers a clear view of whether or not volume is increasing or decreasing.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Volume</term></item>
    /// <item><term>int Short Term</term></item>
    /// <item><term>int Long Term</term></item>
    /// <item><term>int Points Or Percent (output scale type)</term></item>
    /// </list>
    /// </remarks>
    public class VolumeOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public VolumeOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.VolumeOscillator;

            Init();
        }

        /// <summary>
        /// Action to be executd for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            // Validate
            int iSize = _chartPanel._chartX.RecordCount;
            if (iSize == 0)
                return false;

            int paramInt1 = ParamInt(1);
            int paramInt2 = ParamInt(2);
            if (paramInt1 > paramInt2 || paramInt1 < 1)
            {
                ProcessError("Invalid Short Term Period for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            if (paramInt2 < 8)
            {
                ProcessError("Invalid Long Term Period (min 8) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt3 = ParamInt(3);
            if (paramInt3 != 1 && paramInt3 != 2)
            {
                ProcessError("Invalid Points or Percent for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            Field pVolume = SeriesToField("Volume", paramStr0, iSize);
            if (!EnsureField(pVolume, paramStr0)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pVolume);
            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.VolumeOscillator(pNav, pVolume, paramInt1, paramInt2, paramInt3, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < paramInt2 ? null : pInd.Value(FullName, n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}

