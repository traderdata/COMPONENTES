using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ModulusFE.Indicators;

namespace ModulusFE
{
    internal partial class ToolTip : System.Windows.Controls.ToolTip
    {
        private readonly Indicator _indicator;
        private ItemsControl _itemsControl;

        private readonly ObservableCollection<string> _parameters = new ObservableCollection<string>();

#if WPF
    static ToolTip()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolTip), new FrameworkPropertyMetadata(typeof(ToolTip)));
    }
#endif

#if SILVERLIGHT
        public ToolTip()
        {
            DefaultStyleKey = typeof(ToolTip);
        }
#endif

        public ToolTip(Indicator indicator)
        {
            _indicator = indicator;
            if (_indicator is TwinIndicator)
                _indicator = ((TwinIndicator)_indicator)._indicatorParent;
        }

        private void BuildParameters()
        {
            _parameters.Clear();

            List<StockChartX_IndicatorsParameters.IndicatorParameter> parameters =
              StockChartX_IndicatorsParameters.GetIndicatorParameters(_indicator.IndicatorType);

            _itemsControl.Height = parameters.Count * (_indicator._chartPanel._chartX.GetTextHeight("W") + 4);
            _parameters.Clear();
            int index = 0;
            foreach (var parameter in parameters)
            {
                _parameters.Add(string.Format("{0} = {1}", parameter.Name, _indicator.GetParameterValue(index++)));
            }
            _itemsControl.ItemsSource = _parameters;
        }

        internal void RebuildParameters()
        {
            if (_itemsControl == null) return;
            BuildParameters();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsControl = GetTemplateChild("ItemsControl") as ItemsControl;
            if (_itemsControl == null) throw new NullReferenceException();

            BuildParameters();
        }
    }
}

