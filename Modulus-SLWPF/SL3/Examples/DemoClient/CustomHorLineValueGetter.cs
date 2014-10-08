using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using ModulusFE;
using ModulusFE.Interfaces;
using ModulusFE.LineStudies;
using ModulusFE.SL;

namespace TestChart
{
    public class CustomHorLineValueGetter : IValueBridge<LineStudy>
    {
        private readonly StockChartX _chart;

        public CustomHorLineValueGetter(StockChartX chart)
        {
            _chart = chart;
        }

        #region Implementation of ILineStudyValue

        public void AttachDataSupplier(LineStudy lineStudy, Type[] parameterTypes)
        {
            //we are expecting one parameter of type double
            Debug.Assert(parameterTypes != null && parameterTypes.Length > 0 && parameterTypes[0] == typeof(double));
        }

        public void NotifyDataChanged(params object[] values)
        {
            double value = Convert.ToDouble(values[0]);
            var c = _chart;
            string scalePrecision = ".00";
            if (c.ScalePrecision > 0)
                scalePrecision = ".".PadRight(c.ScalePrecision + 1, '0');
            Value = value.ToString(scalePrecision);

            Series s = _chart.GetSeriesByName(_chart.Symbol + ".close");
            if (s == null)
            {
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xED, 0x6C));
                Foreground = Brushes.Black;
            }
            else
            {
                double m = (s.Max + s.Min) / 2;
                if (value > m)
                {
                    Background = Brushes.Green;
                    Foreground = Brushes.White;
                }
                else
                {
                    Background = Brushes.Red;
                    Foreground = Brushes.White;
                }
            }
        }

        #endregion

        private Brush _background = Brushes.Green;
        public Brush Background
        {
            get { return _background; }
            set
            {
                if (_background == value)
                    return;
                _background = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Background"));
            }
        }

        private Brush _foreground = Brushes.White;
        public Brush Foreground
        {
            get { return _foreground; }
            set
            {
                if (_foreground == value)
                    return;
                _foreground = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Foreground"));
            }
        }

        private string _value = string.Empty;
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Value"));
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }

}
