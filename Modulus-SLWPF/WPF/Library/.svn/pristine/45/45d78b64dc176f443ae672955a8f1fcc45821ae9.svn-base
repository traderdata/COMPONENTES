using System.Text;

namespace ModulusFE.ChartElementProperties
{
    internal partial class ChartElementOpacityProperty : ChartElementPropertyBase, IChartElementProperty
    {
        public ChartElementOpacityProperty(string title)
            : base(title)
        {
            _valuePresenter = new SliderPropertyPresenter(0.0, 1.0);
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
