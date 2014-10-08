using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using ModulusFE.Data;
using ModulusFE.Interfaces;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    /// <summary>
    /// A default value bridge for Tick Box
    /// </summary>
    public class SeriesDefValueBridge : IValueBridge<Series>
    {
        private Series _series;
        private double _lastUnshownValue;
        private Series _open, _close;

        /// <summary>
        /// Gets or sets the brush to represent the up-trend of the price.
        /// </summary>
        public Brush UpBrush { get; set; }

        /// <summary>
        /// Gets or sets the brush to represent the down-trend of the price.
        /// </summary>
        public Brush DownBrush { get; set; }

        #region Implementation of INotifyPropertyChanged

        ///<summary>
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly PropertyChangedEventArgs _valueArgs = new PropertyChangedEventArgs("Value");
        private void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Implementation of IValueBridge<Series>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectDataSupplier"></param>
        /// <param name="parametersType"></param>
        public void AttachDataSupplier(Series objectDataSupplier, Type[] parametersType)
        {
            _series = objectDataSupplier;

            NotifyDataChanged(_lastUnshownValue);
            _open = _series._chartPanel._chartX.SeriesCollection.FirstOrDefault(_ => _.OHLCType == SeriesTypeOHLC.Open);
            _close = _series._chartPanel._chartX.SeriesCollection.FirstOrDefault(_ => _.OHLCType == SeriesTypeOHLC.Close);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void NotifyDataChanged(params object[] values)
        {
            if (_series == null)
            {
                _lastUnshownValue = Convert.ToDouble(values[0]);
                return;
            }

            double value = Convert.ToDouble(values[0]);

            Value = string.Format(_series._chartPanel.FormatYValueString, value);

            if (_open != null && _close != null)
            {
                DataEntry dataEntryOpen = _series.DM.LastVisibleDataEntry(_open.SeriesIndex);
                DataEntry dataEntryClose = _series.DM.LastVisibleDataEntry(_close.SeriesIndex);
                BackgroundEx = dataEntryOpen.Value > dataEntryClose.Value ? DownBrush : UpBrush;
            }
        }

        #endregion

        #region Value

        private string _value;

        ///<summary>
        /// Value
        ///</summary>
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                InvokePropertyChanged(_valueArgs);
            }
        }

        #endregion

        #region BackgroundEx

        private Brush _backgroundEx;

        /// <summary>
        /// Provides <see cref="PropertyChangedEventArgs"/> for property <see cref="BackgroundEx"/>
        /// </summary>
        public static readonly PropertyChangedEventArgs BackgroundExChangedEventsArgs =
            ObservableHelper.CreateArgs<SeriesDefValueBridge>(_ => _.BackgroundEx);

        /// <summary>
        /// Gets or sets the BackgroundEx value.
        /// </summary>
        public Brush BackgroundEx
        {
            get { return _backgroundEx; }
            set
            {
                if (_backgroundEx != value)
                {
                    _backgroundEx = value;
                    InvokePropertyChanged(BackgroundExChangedEventsArgs);
                }
            }
        }

        #endregion
    }
}
