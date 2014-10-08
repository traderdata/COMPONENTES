#if WPF
using System.Windows;
#endif
using System.Windows.Controls;

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public partial class PanelsBarButton : Button
    {
#if WPF
    static PanelsBarButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelsBarButton), new FrameworkPropertyMetadata(typeof(PanelsBarButton)));
    }
#else
        ///<summary>
        /// ctor
        ///</summary>
        public PanelsBarButton()
        {
            DefaultStyleKey = typeof(PanelsBarButton);
        }
#endif
    }
}



