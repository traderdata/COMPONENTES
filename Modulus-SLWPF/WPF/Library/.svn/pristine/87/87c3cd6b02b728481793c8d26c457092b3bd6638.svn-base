using System;
using System.Windows;
using System.Windows.Media;

namespace ModulusFE.Controls
{
    /// <summary>
    /// Interaction logic for ColorDialog.xaml
    /// </summary>
    public partial class ColorDialog
    {
        ///<summary>
        ///</summary>
        public event EventHandler OnOK = delegate { };

        ///<summary>
        ///</summary>
        public ColorDialog()
        {
            InitializeComponent();

            colorPicker.ColorSelected += ColorPickerOnColorSelected;
        }

        private void ColorPickerOnColorSelected(Color color)
        {
            canvasnewColor.Background = new SolidColorBrush(color);
        }

        ///<summary>
        ///</summary>
        public Color CurrentColor
        {
            get { return colorPicker.SelectedColor; }
            set
            {
                colorPicker.SelectedColor = value;
                canvasOldColor.Background = new SolidColorBrush(value);
                canvasnewColor.Background = new SolidColorBrush(value);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (OnOK != null)
                OnOK(this, EventArgs.Empty);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
