using System.Text;

namespace ModulusFE.ChartElementProperties
{
    internal partial class ChartElementStrokeThicknessProperty : ChartElementPropertyBase, IChartElementProperty
    {
        public ChartElementStrokeThicknessProperty(string title)
            : base(title)
        {
            _valuePresenter = new TextBoxPropertyPresenter();
        }

        #region Implementation of IChartElementProperty

        public string Title
        {
            get { return _title; }
        }

        public bool Validate(StringBuilder sb)
        {
            double thickness;
            string value = ValuePresenter.Value.ToString();
            if (double.TryParse(value, out thickness))
                return true;

            sb.Append(string.Format("The value {0} is not a valid number", value));
            return false;
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
