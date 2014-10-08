using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ModulusFE.Controls
{
    /// <summary>
    /// Represents a Color Picker control which allows a user to select a color.
    /// </summary>
    public partial class ColorPicker : Control
    {
        ///<summary>
        ///</summary>
        ///<param name="c"></param>
        public delegate void ColorSelectedHandler(Color c);

        /// <summary>
        /// Event fired when a color is selected.
        /// </summary>
        public event ColorSelectedHandler ColorSelected;

        private readonly ColorSpace m_colorSpace;
        private bool m_hueMonitorMouseCaptured;
        private bool m_sampleMouseCaptured;
        private double m_huePos;
        private double m_sampleX;
        private double m_sampleY;

        private Rectangle m_hueMonitor;
        private Canvas m_sampleSelector;
        private Canvas m_hueSelector;

        private Rectangle m_selectedColorView;
        private Rectangle m_colorSample;
        private TextBlock m_hexValue;


        static ColorPicker()
        {
#if WPF
      DefaultStyleKeyProperty.OverrideMetadata(typeof (ColorPicker), new FrameworkPropertyMetadata(typeof (ColorPicker)));
#endif
        }
        ///<summary>
        /// Ctor
        ///</summary>
        public ColorPicker()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ColorPicker);
#endif

            m_colorSpace = new ColorSpace();

            //      Loaded += (sender, args) => UpdateVisuals();
        }

        /// <summary>
        /// Ovverides <see cref="OnApplyTemplate"/>
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_hueMonitor = GetTemplateChild("HueMonitor") as Rectangle;
            m_sampleSelector = GetTemplateChild("SampleSelector") as Canvas;
            m_hueSelector = GetTemplateChild("HueSelector") as Canvas;
            m_selectedColorView = GetTemplateChild("SelectedColorView") as Rectangle;
            m_colorSample = GetTemplateChild("ColorSample") as Rectangle;
            m_hexValue = GetTemplateChild("HexValue") as TextBlock;

            if (m_hueMonitor != null)
            {
                m_hueMonitor.MouseLeftButtonDown += rectHueMonitor_MouseLeftButtonDown;
                m_hueMonitor.MouseLeftButtonUp += rectHueMonitor_MouseLeftButtonUp;
                m_hueMonitor.MouseMove += rectHueMonitor_MouseMove;
            }

            if (m_colorSample != null)
            {
                m_colorSample.MouseLeftButtonDown += rectSampleMonitor_MouseLeftButtonDown;
                m_colorSample.MouseLeftButtonUp += rectSampleMonitor_MouseLeftButtonUp;
                m_colorSample.MouseMove += rectSampleMonitor_MouseMove;

                m_sampleX = m_colorSample.Width;
            }

            m_sampleY = 0;
            m_huePos = 0;

            UpdateSatValSelection();

            UpdateVisuals();
        }

        private void rectHueMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            m_hueMonitorMouseCaptured = m_hueMonitor.CaptureMouse();
            DragSliders(0, e.GetPosition((UIElement)sender).Y);
        }

        private void rectHueMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            m_hueMonitor.ReleaseMouseCapture();
            m_hueMonitorMouseCaptured = false;
            SetValue(SelectedColorProperty, GetColor());
        }

        private void rectHueMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_hueMonitorMouseCaptured)
                return;

            DragSliders(0, e.GetPosition((UIElement)sender).Y);
        }

        private void rectSampleMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            m_sampleMouseCaptured = m_colorSample.CaptureMouse();
            Point pos = e.GetPosition((UIElement)sender);
            DragSliders(pos.X, pos.Y);
        }

        private void rectSampleMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            m_colorSample.ReleaseMouseCapture();
            m_sampleMouseCaptured = false;
            SetValue(SelectedColorProperty, GetColor());
        }

        private void rectSampleMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_sampleMouseCaptured)
                return;

            Point pos = e.GetPosition((UIElement)sender);
            DragSliders(pos.X, pos.Y);
        }


        private Color GetColor()
        {
            double yComponent = 1 - (m_sampleY / m_colorSample.Height);
            double xComponent = m_sampleX / m_colorSample.Width;
            double hueComponent = (m_huePos / m_hueMonitor.Height) * 360;

            return m_colorSpace.ConvertHsvToRgb(hueComponent, xComponent, yComponent);
        }

        private void UpdateSatValSelection()
        {
            if (m_colorSample == null)
                return;

            m_sampleSelector.SetValue(Canvas.LeftProperty, m_sampleX - (m_sampleSelector.Height / 2));
            m_sampleSelector.SetValue(Canvas.TopProperty, m_sampleY - (m_sampleSelector.Height / 2));

            Color currColor = GetColor();
            m_selectedColorView.Fill = new SolidColorBrush(currColor);
            m_hexValue.Text = m_colorSpace.GetHexCode(currColor);
        }

        private void UpdateHueSelection()
        {
            if (m_hueMonitor == null)
                return;
            double huePos = m_huePos / m_hueMonitor.Height * 255;
            Color c = m_colorSpace.GetColorFromPosition(huePos);
            m_colorSample.Fill = new SolidColorBrush(c);

            double pos = m_huePos - (m_hueSelector.Height / 2);

            Canvas.SetTop(m_hueSelector, pos);

            Color currColor = GetColor();

            m_selectedColorView.Fill = new SolidColorBrush(currColor);
            m_hexValue.Text = m_colorSpace.GetHexCode(currColor);
        }

        private void UpdateVisuals()
        {
            if (m_hueMonitor == null)
                return;

            Color c = SelectedColor;
            ColorSpace cs = new ColorSpace();
            HSV hsv = cs.ConvertRgbToHsv(c);

            m_huePos = (hsv.Hue / 360 * m_hueMonitor.Height);
            m_sampleY = -1 * (hsv.Value - 1) * m_colorSample.Height;
            m_sampleX = hsv.Saturation * m_colorSample.Width;
            if (!double.IsNaN(m_huePos))
                UpdateHueSelection();
            UpdateSatValSelection();
        }

        private void DragSliders(double x, double y)
        {
            if (m_hueMonitorMouseCaptured)
            {
                m_huePos = y < 0 ? 0 : (y > m_hueMonitor.Height ? m_hueMonitor.Height : y);
                UpdateHueSelection();
            }
            else if (m_sampleMouseCaptured)
            {
                m_sampleX = x < 0 ? 0 : (x > m_colorSample.Width ? m_colorSample.Width : x);

                m_sampleY = y < 0 ? 0 : (y > m_colorSample.Height ? m_colorSample.Height : y);

                UpdateSatValSelection();
            }
        }

        #region SelectedColor Dependency Property
        /// <summary>
        /// Gets or sets the currently selected color in the Color Picker.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set
            {
                SetValue(SelectedColorProperty, value);
                UpdateVisuals();
            }
        }

        /// <summary>
        /// SelectedColor Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor",
                typeof(Color),
                typeof(ColorPicker),
                new PropertyMetadata(new PropertyChangedCallback(SelectedColorChanged)));

        private static void SelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker p = d as ColorPicker;
            if (p != null && p.ColorSelected != null)
            {
                p.ColorSelected((Color)e.NewValue);
            }
        }


        #endregion

    }
}
