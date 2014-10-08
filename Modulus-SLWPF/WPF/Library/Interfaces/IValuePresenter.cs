namespace ModulusFE.Interfaces
{
    ///<summary>
    /// Basic interface for various value presenter for LineStudies, Series Tick Boxes and other chart elements
    ///</summary>
    public interface IValuePresenter
    {
        ///<summary>
        /// Reference to a control that will present the values
        ///</summary>
        object ValuePresenter { get; }

        ///<summary>
        /// Show <see cref="ValuePresenter"/>
        ///</summary>
        ///<param name="show"></param>
        void Show(bool show);
    }
}
