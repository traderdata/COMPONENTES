using System.Windows;
using System.Windows.Controls;
using ModulusFE.Interfaces;

namespace ModulusFE.LineStudies
{
    ///<summary>
    ///</summary>
    public class HorizontalLineStudyValuePresenter : ContentControl, IValuePresenter
    {
#if WPF
    static HorizontalLineStudyValuePresenter()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalLineStudyValuePresenter), 
        new FrameworkPropertyMetadata(typeof(HorizontalLineStudyValuePresenter)));
    }
#endif

#if SILVERLIGHT

        ///<summary>
        ///</summary>
        public HorizontalLineStudyValuePresenter()
        {
            DefaultStyleKey = typeof(HorizontalLineStudyValuePresenter);
        }
#endif

        #region Implementation of IValuePresenter

        ///<summary>
        /// Reference to object
        ///</summary>
        public object ValuePresenter
        {
            get { return this; }
        }

        ///<summary>
        /// Show the control
        ///</summary>
        ///<param name="show"></param>
        public void Show(bool show)
        {
            Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}

