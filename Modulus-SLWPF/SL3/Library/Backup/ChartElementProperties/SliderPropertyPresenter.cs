using System;
using System.Windows;
using System.Windows.Controls;

namespace ModulusFE.ChartElementProperties
{
    ///<summary>
    /// Represents a slider
    ///</summary>
    public partial class SliderPropertyPresenter : IValuePresenter
    {
        private readonly Slider _slider;

        ///<summary>
        /// Ctor
        ///</summary>
        ///<param name="min"></param>
        ///<param name="max"></param>
        public SliderPropertyPresenter(double min, double max)
        {
            _slider = new Slider { Minimum = min, Maximum = max, SmallChange = (max - min) / 100.0, LargeChange = (max - min) / 10 };
        }

        #region Implementation of IValuePresenter

        ///<summary>
        ///</summary>
        public object Value
        {
            get { return _slider.Value; }
            set { _slider.Value = Convert.ToDouble(value); }
        }

        ///<summary>
        ///</summary>
        public FrameworkElement Control
        {
            get { return _slider; }
        }

        #endregion
    }
}
