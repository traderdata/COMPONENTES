using System.Text;
using System.Windows.Controls;

namespace ModulusFE.ChartElementProperties
{
    internal partial class ChartElementColorProperty : ChartElementPropertyBase, IChartElementProperty
    {
        public ChartElementColorProperty(string title)
            : base(title)
        {
            _valuePresenter = new ColorPropertyPresenter();
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
