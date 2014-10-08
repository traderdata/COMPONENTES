using System;
using System.ComponentModel;

namespace ModulusFE.Interfaces
{
    ///<summary>
    /// Represents a "bridge" between various chart objects, such as LineStudies, Indicators, ... and 
    /// their extra value presentation.
    /// In most of the cases instances of objects derived from this interface will server as
    /// DataContext source for ContentPresenter
    ///</summary>
    public interface IValueBridge<T> : INotifyPropertyChanged
    {
        ///<summary>
        /// Method called by the data provider that let's bind the data provider reference to 
        /// its extended value presenter. In most of the cases you just hold the reference in an interval field
        ///</summary>
        ///<param name="objectDataSupplier">A reference to data provider object(LineStudy, Indicator, ...)</param>
        ///<param name="parametersType">An array of parameter types that will be send to <see cref=" NotifyDataChanged"/> method</param>
        void AttachDataSupplier(T objectDataSupplier, Type[] parametersType);

        ///<summary>
        /// Method called by the data provider informing that its interval values have been changed.
        /// Every data provider has its own number nad types of values
        ///</summary>
        ///<param name="values"></param>
        void NotifyDataChanged(params object[] values);
    }
}
