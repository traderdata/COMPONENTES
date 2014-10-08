using System.Text;

namespace ModulusFE.ChartElementProperties
{
    ///<summary>
    /// Defines a delegate for <see cref="IChartElementProperty.SetChartElementPropertyValue"/> event
    ///</summary>
    ///<param name="valuePresenter"></param>
    public delegate void SetChartElementPropertyValueHandler(IValuePresenter valuePresenter);

    ///<summary>
    /// Defines the interface for property for elements that implements <see cref="IChartElementPropertyAble"/> interface
    ///</summary>
    public interface IChartElementProperty
    {
        ///<summary>
        /// Gets the property title
        ///</summary>
        string Title { get; }

        ///<summary>
        /// Validates the value
        ///</summary>
        ///<param name="sb">Will have the error message if validation fails</param>
        ///<returns></returns>
        bool Validate(StringBuilder sb);

        ///<summary>
        /// Gets the value presenter for this property
        ///</summary>
        IValuePresenter ValuePresenter { get; }

        ///<summary>
        /// Event raised when a property value has been changed
        ///</summary>
        event SetChartElementPropertyValueHandler SetChartElementPropertyValue;

        ///<summary>
        /// Invoke the event for property vlaue changed
        ///</summary>
        void InvokeSetChatElementPropertyValue();
    }
}
