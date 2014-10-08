
using System.ComponentModel;

namespace ModulusFE
{
    public partial class StockChartX : INotifyPropertyChanged
    {
        internal const string Property_StartIndex = "StartIndex";
        internal const string Property_EndIndex = "EndIndex";
        internal const string Property_NewRecord = "NewRecord";
        internal const string Property_Zoomed = "Zoomed";

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void InvokePropertyChanged(PropertyChangedEventArgs args)
        {
            var pc = PropertyChanged;
            if (pc != null)
                pc(this, args);
        }
    }
}
