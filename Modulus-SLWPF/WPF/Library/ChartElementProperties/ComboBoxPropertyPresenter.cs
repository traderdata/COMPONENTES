using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ModulusFE.ChartElementProperties
{
    ///<summary>
    ///</summary>
    public partial class ComboBoxPropertyPresenter : IValuePresenter
    {
        private readonly ComboBox _comboBox;

        ///<summary>
        ///</summary>
        ///<param name="items"></param>
        public ComboBoxPropertyPresenter(IEnumerable items)
        {
            _comboBox = new ComboBox { ItemsSource = items };
        }

        #region Implementation of IValuePresenter

        ///<summary>
        ///</summary>
        public object Value
        {
            get { return _comboBox.SelectedItem; }
            set { _comboBox.SelectedItem = value; }
        }

        ///<summary>
        ///</summary>
        public FrameworkElement Control
        {
            get { return _comboBox; }
        }

        #endregion
    }
}
