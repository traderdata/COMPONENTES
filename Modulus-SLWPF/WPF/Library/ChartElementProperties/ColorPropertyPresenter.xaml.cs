using System.Windows;
using System.Windows.Media;
using ModulusFE.Controls;

namespace ModulusFE.ChartElementProperties
{
    ///<summary>
    ///</summary>
    public partial class ColorPropertyPresenter : IValuePresenter
    {
        private SolidColorBrush _value;
        ///<summary>
        /// ctor
        ///</summary>
        public ColorPropertyPresenter()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
                        {
                            if (_value != null)
                                canvas.Background = _value;
                        };
        }

        private void btnChooseColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.OnOK += (o, args) => canvas.Background = new SolidColorBrush(dlg.CurrentColor);
            dlg.CurrentColor = ((SolidColorBrush)canvas.Background).Color;
#if SILVERLIGHT
            //dlg.Show(Dialog.DialogStyle.ModalDimmed);
            dlg.Show();
#endif
#if WPF
            dlg.ShowDialog();
#endif
        }

        #region Implementation of IValuePresenter

        ///<summary>
        /// 
        ///</summary>
        public object Value
        {
            get { return canvas.Background; }
            set
            {
                if (canvas != null)
                    canvas.Background = (SolidColorBrush)value;
                else
                    _value = (SolidColorBrush)value;
            }
        }

        ///<summary>
        ///</summary>
        public FrameworkElement Control
        {
            get { return this; }
        }

        #endregion
    }
}
