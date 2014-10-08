using System;
using System.Text;

namespace ModulusFE.ChartElementProperties
{
    internal partial class ChartElementStrokeTypeProperty : ChartElementPropertyBase, IChartElementProperty
    {
        public ChartElementStrokeTypeProperty(string title)
            : base(title)
        {
            _valuePresenter = new ComboBoxPropertyPresenter(Enum.GetNames(typeof(LinePattern)));
        }

        #region Implementation of IChartElementProperty

        public string Title
        {
            get { return _title; }
        }

        public bool Validate(StringBuilder sb)
        {
            return true;
        }

        public IValuePresenter ValuePresenter
        {
            get { return _valuePresenter; }
        }

        public event SetChartElementPropertyValueHandler SetChartElementPropertyValue;
        public void InvokeSetChatElementPropertyValue()
        {
            if (SetChartElementPropertyValue != null)
                SetChartElementPropertyValue(ValuePresenter);
        }

        #endregion
    }
}
