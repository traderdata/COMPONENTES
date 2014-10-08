using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE.Controls
{
    ///<summary>
    ///</summary>
    public partial class ColorDialogInternal
    {
        internal ColorDialog ParentDialog { get; set; }

        ///<summary>
        ///</summary>
        public ColorDialogInternal()
        {
            InitializeComponent();

            colorPicker.ColorSelected += colorPicker_ColorSelected;
            btnOK.Click += (sender, e) => ParentDialog.OkClose();
            btnCancel.Click += (sender, e) => ParentDialog.Close();
        }

        ///<summary>
        ///</summary>
        public Color SelectedColor
        {
            get { return colorPicker.SelectedColor; }
            set
            {
                colorPicker.SelectedColor = value;
                canvasOldColor.Background = new SolidColorBrush(value);
                canvasnewColor.Background = new SolidColorBrush(value);
            }
        }

        void colorPicker_ColorSelected(Color c)
        {
            canvasnewColor.Background = new SolidColorBrush(c);
        }
    }

    ///<summary>
    ///</summary>
    public class ColorDialog : ChildWindow
    {
        private readonly ColorDialogInternal _dialog;

        ///<summary>
        ///</summary>
        public event EventHandler OnOK = delegate { };

        ///<summary>
        /// Ctor
        ///</summary>
        public ColorDialog()
        {
            Title = "Choose a color";

            _dialog = new ColorDialogInternal { ParentDialog = this };
            Content = _dialog;

            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            if (DialogResult == true)
                OnOK(this, EventArgs.Empty);
        }

        ///<summary>
        ///</summary>
        public Color CurrentColor
        {
            get { return _dialog.SelectedColor; }
            set { _dialog.SelectedColor = value; }
        }

        internal void OkClose()
        {
            DialogResult = true;
        }
    }
}
