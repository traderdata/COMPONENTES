using System;
using System.Collections.Generic;
using ModulusFE.Indicators;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_CustomIndicator()
        {
            /*  
             * Custom indicators have user defined parameters.
            */
            RegisterIndicatorParameters(IndicatorType.CustomIndicator, typeof(CustomIndicator),
                                        "Custom Indicator",
                                        new List<IndicatorParameter>());
        }
    }
}


namespace ModulusFE.Indicators
{
    /// <summary>
    /// Custom indicator is used to let user use his algorithm to calculate the indicator
    /// </summary>
    public class CustomIndicator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public CustomIndicator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.CustomIndicator;

            Init();
        }

        /// <summary>
        /// Action executed when Indicator needs to be recalculated
        /// </summary>
        /// <returns></returns>
        protected override bool TrueAction()
        {
            int size = _chartPanel._chartX.RecordCount;
            if (size == 0)
                return false;

            double?[] values = new double?[size];

            StockChartX.CustomIndicatorNeedsDataEventArgs args =
              new StockChartX.CustomIndicatorNeedsDataEventArgs(this, values);

            for (int i = 0; i < size; i++)
                values[i] = this[i].Value;

            _chartPanel._chartX.FireCustomIndicatorNeedsData(args);

            int usersValuesLength = args.Values.Length;
            int minLength = Math.Min(size, usersValuesLength);

            Clear();
            for (int i = 0; i < minLength; i++)
            {
                AppendValue(_chartPanel._chartX._dataManager.GetTimeStampByIndex(i), args.Values[i]);
            }

            _chartPanel._enforceSeriesSetMinMax = true;
            _chartPanel.SetMaxMin();

            return _calculateResult = PostCalculate();
        }

        //    internal override bool Calculate()
        //    {
        //      // Get input from user
        //      if (!GetUserInput())
        //        return false;
        //
        //      if (!_chartPanel._chartX.CustomIndicatorNeedsDataIsHooked()) return base.Calculate();
        //
        //      int size = _chartPanel._chartX.RecordCount;
        //      if (size == 0)
        //        return false;
        //
        //      double?[] values = new double?[size];
        //
        //      StockChartX.CustomIndicatorNeedsDataEventArgs args =
        //        new StockChartX.CustomIndicatorNeedsDataEventArgs(this, values);
        //
        //      for (int i = 0; i < size; i++)
        //        values[i] = this[i].Value;
        //
        //      _chartPanel._chartX.FireCustomIndicatorNeedsData(args);
        //
        //      int usersValuesLength = args.Values.Length;
        //      int minLength = Math.Min(size, usersValuesLength);
        //
        //      Clear();
        //      for (int i = 0; i < minLength; i++)
        //      {
        //        AppendValue(_chartPanel._chartX._dataManager.GetTimeStampByIndex(i), args.Values[i]);
        //      }
        //
        //      _chartPanel._enforceSeriesSetMinMax = true;
        //      _chartPanel.SetMaxMin();
        //
        //      return base.Calculate();
        //    }

        /// <summary>
        /// Adds a parameter that will be shown in indicators dialog
        /// </summary>
        /// <param name="parameterName">Parameter name. Is this is an empty string then the default name will be taken.</param>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="valueType">Value type</param>
        public void AddParameter(string parameterName, ParameterType parameterType, object defaultValue, Type valueType)
        {
            if (defaultValue.GetType() != valueType)
                throw new ArgumentException("Parameter type mismatch.", "valueType");

            _parameters.Add(new StockChartX_IndicatorsParameters.IndicatorParameter
                              {
                                  DefaultValue = defaultValue,
                                  Name = string.IsNullOrEmpty(parameterName) ? GetParamName(parameterType) : parameterName,
                                  ParameterType = parameterType,
                                  ValueType = valueType
                              });
            _params.Add(defaultValue);
        }
    }
}
