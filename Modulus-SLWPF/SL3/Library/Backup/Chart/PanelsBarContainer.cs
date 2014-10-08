using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
#if SILVERLIGHT
using ModulusFE.SL;
#else
using System.Windows.Media;
#endif

namespace ModulusFE
{
    /// <summary>
    /// The bar that keeps the buttons for minimized panels.
    /// </summary>
    public partial class PanelsBarContainer : Control
    {
        private readonly Dictionary<ChartPanel, Button> _panels = new Dictionary<ChartPanel, Button>();
        private StackPanel _rootPanel;

        internal StockChartX _chartX;

        /// <summary>
        /// Event click
        /// </summary>
        public class PanelsBarButtonClick : EventArgs
        {
            internal ChartPanel _chartPanel;
            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="chartPanel"></param>
            public PanelsBarButtonClick(ChartPanel chartPanel)
            {
                _chartPanel = chartPanel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PanelsBarButtonClick> OnButtonClicked;

#if WPF
    static PanelsBarContainer()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelsBarContainer), new FrameworkPropertyMetadata(typeof(PanelsBarContainer)));
    }
#endif
#if SILVERLIGHT
        ///<summary>
        /// Ctor
        ///</summary>
        public PanelsBarContainer()
        {
            DefaultStyleKey = typeof(PanelsBarContainer);
        }
#endif

        internal void AddPanel(ChartPanel chartPanel)
        {
            Button btnPanel = CreatePanelButton(chartPanel);
            if (_rootPanel != null)
                _rootPanel.Children.Add(btnPanel);
            _panels.Add(chartPanel, btnPanel);
        }

        internal void DeletePanel(ChartPanel chartPanel)
        {
            Button btnPanel;
            if (!_panels.TryGetValue(chartPanel, out btnPanel)) return;
            _panels.Remove(chartPanel);
            _rootPanel.Children.Remove(btnPanel);
        }

        internal bool Visible
        {
            get { return _panels.Count > 0; }
        }

        internal int PanelCount
        {
            get { return _panels.Count; }
        }

        internal void UpdateVisibility()
        {
            Visibility = Visible ? Visibility.Visible : Visibility.Collapsed;
        }

        internal Rect GetNextRectToMinimize
        {
            get
            {
                double dPanelButtonWidth = PanelButtonWidth;
                return new Rect(dPanelButtonWidth * _panels.Count, 1, dPanelButtonWidth, ActualHeight > 0 ? ActualHeight - 2 : 0);
            }
        }

        internal double PanelButtonWidth
        {
            get
            {
                return _panels.Count == 0
                         ? Constants.PanelsBarButtonWidth
                         : Math.Min(Constants.PanelsBarButtonWidth, ActualWidth / _panels.Count);
            }
        }

        internal void EnableButtons(bool bEnable)
        {
            foreach (Button button in _rootPanel.Children)
            {
                button.IsEnabled = bEnable;
            }
        }

        private Button CreatePanelButton(ChartPanel chartPanel)
        {
            string title = chartPanel.Title;
            PanelsBarButton btnPanel = new PanelsBarButton
            {
                Content = title,
                Width = Constants.PanelsBarButtonWidth,
                Margin = new Thickness(2, 1, 1, 1),
                Foreground = Brushes.White,
#if WPF
        ToolTip = title,
#endif
            };
            btnPanel.Click +=
              (sender, e) => { if (OnButtonClicked != null) OnButtonClicked(this, new PanelsBarButtonClick(chartPanel)); };
            return btnPanel;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootPanel = GetTemplateChild("rootPanel") as StackPanel;
            if (_rootPanel == null) throw new NullReferenceException("Root panel is null");
            foreach (KeyValuePair<ChartPanel, Button> pair in _panels)
            {
                _rootPanel.Children.Add(pair.Value);
            }
        }


        /*protected override void OnRender(DrawingContext drawingContext)
        {
          base.OnRender(drawingContext);

          Rect rcBounds = new Rect(0, 0, ActualWidth, ActualHeight);

          drawingContext.DrawRectangle(Background, null, rcBounds);

          if (_panels.Count == 0)
            drawingContext.DrawText(new FormattedText("No Panels", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                                    new Typeface(_chartX.FontFace), _chartX.FontSize, _chartX.FontForeground),
                                  new Point(10, 2));
          else
          {
            double dPanelButtonWidth = PanelButtonWidth;
            double dLeft = 0;
            Pen penRect = new Pen(_chartX.Foreground, 1);
        
            foreach (ChartPanel panel in _panels)
            {
              drawingContext.DrawRectangle(panel.Background, penRect, new Rect(dLeft, 1, dPanelButtonWidth, ActualHeight - 2));
              drawingContext.DrawText(new FormattedText(panel.Title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                                    new Typeface(_chartX.FontFace), _chartX.FontSize, _chartX.FontForeground),
                                                    new Point(dLeft + 2, 2));
              dLeft += dPanelButtonWidth;
            }
          }
        }*/
    }
}
