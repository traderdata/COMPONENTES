using System.Windows.Controls;

namespace ModulusFE
{
    public partial class ChartPanel
    {
        private TextBlock _chartPanelLabel;
        ///<summary>
        /// A TextBlock associated with every panel on chart. Used at user's discretion
        ///</summary>
        public TextBlock ChartPanelLabel
        {
            get
            {
                EnsureChartPanelLabelCreated();
                return _chartPanelLabel;
            }
        }

        private void EnsureChartPanelLabelCreated()
        {
            if (_chartPanelLabel != null)
                return;

            _chartPanelLabel = new TextBlock();
            _rootCanvas.Children.Add(_chartPanelLabel);
            Canvas.SetZIndex(_chartPanelLabel, ZIndexConstants.TextLabelTitle);
        }
    }
}