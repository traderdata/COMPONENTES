using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Interaction logic for IndicatorDialog.xaml
    /// </summary>
    public partial class IndicatorDialog
    {
        private Indicator _indicator;
        private Indicator _styleIndicator;

        /// <summary>
        /// internal usage
        /// </summary>
        public event EventHandler OnOk = delegate { };
        /// <summary>
        /// internal usage
        /// </summary>
        public event EventHandler OnCancel = delegate { };

        internal IndicatorDialog()
        {
            InitializeComponent();

            DataContext = this;

            for (int i = 0; i < 10; i++)
            {
                GetTextBox(i).GotFocus += OnGotFocus;
                GetComboBox(i).GotFocus += OnGotFocus;
            }

            Loaded += (sender, args) => GetComboBox(0).Focus();

            Closing += OnClosing;

            CanClose = true;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = !CanClose;
        }

        ///<summary>
        ///</summary>
        ///<param name="sender"></param>
        ///<param name="index"></param>
        ///<param name="description"></param>
        public delegate void NeedDescriptionHandler(IndicatorDialog sender, int index, out string description);
        ///<summary>
        ///</summary>
        public event NeedDescriptionHandler NeedDescription;

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var e = NeedDescription;
            if (e != null)
            {
                string desc;
                e(this, Convert.ToInt32(((Control)sender).Name.Substring(3)), out desc);
                lblDesc.Text = desc;
            }
        }

        internal double StrokeThicknes { get; set; }

        internal Indicator Indicator
        {
            get { return _indicator; }
            set
            {
                _indicator = value;
                if (_styleIndicator == null)
                    canvasColor.Background = new SolidColorBrush(_indicator.StrokeColor);
            }
        }

        internal Indicator StyleIndicator
        {
            get
            {
                if (_styleIndicator == null)
                    return _indicator;
                return _styleIndicator;
            }
            set
            {
                _styleIndicator = value;
                canvasColor.Background = new SolidColorBrush(_styleIndicator.StrokeColor);
            }
        }

        internal bool CanClose { get; set; }

        internal ComboBox GetComboBox(int index)
        {
            return FindName("cmb" + index) as ComboBox;
        }

        internal TextBlock GetTextBlock(int index)
        {
            return FindName("lbl" + index) as TextBlock;
        }

        internal TextBox GetTextBox(int index)
        {
            return FindName("txt" + index) as TextBox;
        }

        internal void ShowHidePanel(int index, bool hide, bool showTextBox)
        {
            Visibility v = hide ? Visibility.Collapsed : Visibility.Visible;
            GetTextBox(index).Visibility = v;
            GetTextBlock(index).Visibility = v;
            GetComboBox(index).Visibility = v;

            if (hide)
                return;

            if (showTextBox)
                GetComboBox(index).Visibility = Visibility.Collapsed;
            else
                GetTextBox(index).Visibility = Visibility.Collapsed;
        }

        private void canvasColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorPickerDialog dlg =
              new ColorPickerDialog
                {
                    Owner = this,
                    //            Owner = Application.Current != null && Application.Current.Windows.Count > 0
                    //                      ? Application.Current.Windows[0]
                    //                      : null,
                    StrokeThickness = StyleIndicator.StrokeThickness,
                    SelectedColor = StyleIndicator.StrokeColor
                };
            if (dlg.ShowDialog() != true) return;
            StyleIndicator.StrokeThickness = dlg.StrokeThickness;
            StyleIndicator.StrokeColor = dlg.SelectedColor;
            canvasColor.Background = new SolidColorBrush(StyleIndicator.StrokeColor);
            if (StyleIndicator.Panel._chartX.IndicatorTwinTitleVisibility == System.Windows.Visibility.Collapsed)
            {
                foreach (Series series in StyleIndicator._linkedSeries)
                {
                    Indicator ind = series as Indicator;
                    if (ind != null)
                    {
                        ind.StrokeColor = _styleIndicator._strokeColor;
                        ind.StrokePattern = _styleIndicator._strokePattern;
                        ind.StrokeThickness = _styleIndicator._strokeThickness;
                    }
                }
            }
        }

        internal bool _userCanceled;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Indicator.SetUserInput();
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show(this, ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(this, ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_indicator._inputError)
            {
                if (_userCanceled)
                    btnCancel_Click(this, e);
                return;
            }

            CanClose = true;
            Indicator._dialogShown = false;
            DialogResult = true;
            Close();

            OnOk(this, EventArgs.Empty); //raise event
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CanClose = true;
            Indicator._dialogShown = false;
            Indicator._chartPanel._chartX._locked = false;
            Indicator.OnCancelDialog();
            DialogResult = false;
            Close();

            OnCancel(this, EventArgs.Empty); //raise event
        }

        #region Dependancy Properties

        #region LabelFontSizeProperty (DependencyProperty)

        /// <summary>
        /// LabelFontSize summary
        /// </summary>
        public double LabelFontSize
        {
            get
            {
                return (double)GetValue(LabelFontSizeProperty);
            }
            set
            {
                SetValue(LabelFontSizeProperty, value);
            }
        }

        /// <summary>
        /// LabelFontSize
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty =
          DependencyProperty.Register("LabelFontSize", typeof(double), typeof(IndicatorDialog),
                                      new PropertyMetadata(11.0, OnLabelFontSizeChanged));

        private static void OnLabelFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((IndicatorDialog)d).OnLabelFontSizeChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLabelFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region LabelFontForegroundProperty (DependencyProperty)

        /// <summary>
        /// LabelForeground summary
        /// </summary>
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelFontForegroundProperty); }
            set { SetValue(LabelFontForegroundProperty, value); }
        }

        /// <summary>
        /// LabelForeground
        /// </summary>
        public static readonly DependencyProperty LabelFontForegroundProperty =
          DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(IndicatorDialog),
                                      new PropertyMetadata(new SolidColorBrush(Colors.Black), OnLabelFontForegroundChanged));

        private static void OnLabelFontForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((IndicatorDialog)d).OnLabelFontForegroundChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLabelFontForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #endregion
    }
}
