using System.Windows.Media;

namespace ModulusFE.Indicators
{
    /// <summary>
    /// this type of indicator is used when an indicator has more than one lines
    /// </summary>
    internal class TwinIndicator : Indicator
    {
        internal Indicator _indicatorParent;

        public TwinIndicator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            Init();

            _isTwin = true;

            _indicatorType = IndicatorType.Unknown;
        }

        private bool _strokeColorSet;
        internal void SetStrokeColor(Color color, bool forceSet)
        {
            if (_strokeColorSet && !forceSet) return;

            _strokeColor = color;
            _strokeColorSet = true;
        }

        private bool _strokeThicknessSet;
        internal void SetStrokeThickness(double thickness, bool forceSet)
        {
            if (_strokeThicknessSet && !forceSet) return;
            _strokeThickness = thickness;
            _strokeThicknessSet = true;
        }

        private bool _strokePatternSet;
        internal void SetStrokePattern(LinePattern pattern, bool forceSet)
        {
            if (_strokePatternSet && !forceSet) return;
            _strokePattern = pattern;
            _strokePatternSet = true;
        }

        protected override bool TrueAction()
        {
            return true; //nothing fancy here, its values are calculated by its owner
        }
    }
}
