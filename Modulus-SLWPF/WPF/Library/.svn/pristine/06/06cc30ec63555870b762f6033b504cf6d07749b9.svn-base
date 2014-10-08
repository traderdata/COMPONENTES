using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Ink;

namespace ModulusFE
{
    /// <summary>
    /// For internal usage only
    /// </summary>
    public partial class ColorPickerDialog
    {
        DrawingAttributes _selectedDA;

        private bool _initialized;

        /// <summary>
        /// Initializes a new instance of the <seealso cref="ColorPickerDialog"/> class.
        /// </summary>
        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        ///<summary>
        /// Get or sets the selected color
        ///</summary>
        public Color SelectedColor
        {
            get { return _selectedDA.Color; }
            set
            {
                if (_initialized)
                    _selectedDA.Color = value;

                UpdateControlValues();
                UpdateControlVisuals();
            }
        }

        ///<summary>
        /// Gets or sets the stroke thickness value
        ///</summary>
        public double StrokeThickness
        {
            get { return _selectedDA.Width; }
            set
            {
                if (_initialized)
                    _selectedDA.Width = value;
                UpdateControlValues();
                UpdateControlVisuals();
            }
        }

        /// <summary>
        /// Completes initialization after all XAML member vars have been initialized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _selectedDA = new DrawingAttributes { Color = Colors.Red, Width = 2 };
            UpdateControlValues();
            UpdateControlVisuals();

            colorComb.ColorSelected += colorComb_ColorSelected;
            brightnessSlider.ValueChanged += brightnessSlider_ValueChanged;
            opacitySlider.ValueChanged += opacitySlider_ValueChanged;
            decrementThickness.Click += decrementThickness_Click;
            incrementThickness.Click += incrementThickness_Click;

            acceptButton.Click += acceptButton_Click;
            cancelButton.Click += cancelButton_Click;
            _initialized = true;
        }

        //
        // Interface

        internal DrawingAttributes SelectedDrawingAttributes
        {
            get
            {
                return _selectedDA;
            }
            set
            {
                _selectedDA = value;
                UpdateControlValues();
                UpdateControlVisuals();
            }
        }

        //
        // Implementation

        bool _notUserInitiated;

        // Updates values of controls when new DA is set (or upon initialization).
        void UpdateControlValues()
        {
            _notUserInitiated = true;
            try
            {
                // Set nominal color on comb.
                Color nc = _selectedDA.Color;
                float f = Math.Max(Math.Max(nc.ScR, nc.ScG), nc.ScB);
                nc = f < 0.001f ? Color.FromScRgb(1f, 1f, 1f, 1f) : Color.FromScRgb(1f, nc.ScR / f, nc.ScG / f, nc.ScB / f);
                colorComb.SelectedColor = nc;

                // Set brightness and opacity.
                brightnessSlider.Value = f;
                if (_selectedDA.IsHighlighter)
                {
                    opacitySlider.Value = 0.5;
                    opacitySlider.IsEnabled = false;
                }
                else
                    opacitySlider.Value = _selectedDA.Color.ScA;
            }
            finally
            {
                _notUserInitiated = false;
            }
        }

        // Updates visual properties of all controls, in response to any change.
        void UpdateControlVisuals()
        {
            Color c = _selectedDA.Color;

            // Update LGB for brightnessSlider
            Border sb1 = brightnessSlider.Parent as Border;
            if (sb1 != null)
            {
                LinearGradientBrush lgb1 = sb1.Background as LinearGradientBrush;
                if (lgb1 != null) lgb1.GradientStops[1].Color = colorComb.SelectedColor;
            }

            // Update LGB for opacitySlider
            Color c2a = Color.FromScRgb(0f, c.ScR, c.ScG, c.ScB);
            Color c2b = Color.FromScRgb(1f, c.ScR, c.ScG, c.ScB);
            Border sb2 = opacitySlider.Parent as Border;
            if (sb2 != null)
            {
                LinearGradientBrush lgb2 = sb2.Background as LinearGradientBrush;
                if (lgb2 != null) lgb2.GradientStops[0].Color = c2a;
                if (lgb2 != null) lgb2.GradientStops[1].Color = c2b;
            }

            // Update thickness
            _selectedDA.Width = Math.Round(_selectedDA.Width, 2);
            thicknessTextbox.Text = _selectedDA.Width.ToString();

            linePreview.StrokeThickness = _selectedDA.Width;
            linePreview.Stroke = new SolidColorBrush(_selectedDA.Color);
        }

        //
        // Event Handlers

        void colorComb_ColorSelected(object sender, ColorEventArgs e)
        {
            if (_notUserInitiated) return;

            float a = (float)opacitySlider.Value;
            float f = (float)brightnessSlider.Value;

            Color nc = e.Color;
            float r = f * nc.ScR;
            float g = f * nc.ScG;
            float b = f * nc.ScB;

            _selectedDA.Color = Color.FromScRgb(a, r, g, b);
            UpdateControlVisuals();
        }

        void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_notUserInitiated) return;

            Color nc = colorComb.SelectedColor;
            float f = (float)e.NewValue;

            float a = (float)opacitySlider.Value;
            float r = f * nc.ScR;
            float g = f * nc.ScG;
            float b = f * nc.ScB;

            _selectedDA.Color = Color.FromScRgb(a, r, g, b);
            UpdateControlVisuals();
        }

        void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_notUserInitiated) return;

            Color c = _selectedDA.Color;
            float a = (float)e.NewValue;

            _selectedDA.Color = Color.FromScRgb(a, c.ScR, c.ScG, c.ScB);
            UpdateControlVisuals();
        }

        void incrementThickness_Click(object sender, RoutedEventArgs e)
        {
            if (_notUserInitiated) return;

            if (_selectedDA.Width < 1.0)
            {
                _selectedDA.Width += 0.1;
                _selectedDA.Height += 0.1;
            }
            else if (_selectedDA.Width < 10.0)
            {
                _selectedDA.Width += 0.5;
                _selectedDA.Height += 0.5;
            }
            else
            {
                _selectedDA.Width += 1d;
                _selectedDA.Height += 1d;
            }

            UpdateControlVisuals();
        }

        void decrementThickness_Click(object sender, RoutedEventArgs e)
        {
            if (_notUserInitiated) return;

            if (_selectedDA.Width < 0.1001)
                return;

            if (_selectedDA.Width < 1.001)
            {
                _selectedDA.Width -= 0.1;
                _selectedDA.Height -= 0.1;
            }
            else if (_selectedDA.Width < 10.001)
            {
                _selectedDA.Width -= 0.5;
                _selectedDA.Height -= 0.5;
            }
            else
            {
                _selectedDA.Width -= 1d;
                _selectedDA.Height -= 1d;
            }

            UpdateControlVisuals();
        }

        void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            // Setting this property closes the dialog, when shown modally.
            DialogResult = true;
        }

        void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Setting this property closes the dialog, when shown modally.
            DialogResult = false;
        }

    }
}