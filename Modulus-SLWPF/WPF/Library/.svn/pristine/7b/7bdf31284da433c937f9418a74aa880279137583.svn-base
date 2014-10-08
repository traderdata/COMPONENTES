using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    /// <summary>
    /// Used when moving panel, it shows if the panel can be moved to a new place or not
    /// </summary>
    public partial class ChartPanelMoveShadow : Control
    {
#if WPF
    static ChartPanelMoveShadow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelMoveShadow), new FrameworkPropertyMetadata(typeof(ChartPanelMoveShadow)));

      IsOkToMoveProperty = DependencyProperty.Register("IsOkToMove", typeof (bool), typeof (ChartPanelMoveShadow),
                                                       new PropertyMetadata(false));
    }
#endif

#if SILVERLIGHT
        ///<summary>
        /// ctor
        ///</summary>
        public ChartPanelMoveShadow()
        {
            DefaultStyleKey = typeof(ChartPanelMoveShadow);
            BackgroundEx = Brushes.DarkRed;
        }
#endif


#if SILVERLIGHT
        ///<summary>
        ///</summary>
        public static readonly DependencyProperty BackgroundExProperty =
          DependencyProperty.Register("BackgroundEx", typeof(Brush), typeof(ChartPanelMoveShadow),
                                      new PropertyMetadata((d, e) => ((ChartPanelMoveShadow)d).OnBackgroundExChanged()));

        ///<summary>
        /// Gets or sets the background brush
        ///</summary>
        public Brush BackgroundEx
        {
            get { return (Brush)GetValue(BackgroundExProperty); }
            set { SetValue(BackgroundExProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnBackgroundExChanged()
        {
        }

        ///<summary>
        ///</summary>
        public static readonly DependencyProperty IsOkToMoveProperty =
          DependencyProperty.Register("IsOkToMove", typeof(bool), typeof(ChartPanelMoveShadow),
                                      new PropertyMetadata((d, e) => ((ChartPanelMoveShadow)d).OnIsOkToMoveChanged(d, e)));

        ///<summary>
        /// Gets or sets whether is ok to move
        ///</summary>
        public bool IsOkToMove
        {
            get { return (bool)GetValue(IsOkToMoveProperty); }
            set { SetValue(IsOkToMoveProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected virtual void OnIsOkToMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            BackgroundEx = newValue ? Brushes.LightBlue : Brushes.DarkRed;
        }
#endif
#if WPF
    ///<summary>
    ///</summary>
    public static readonly DependencyProperty IsOkToMoveProperty;
    ///<summary>
    ///</summary>
    public bool IsOkToMove
    {
      get { return (bool)GetValue(IsOkToMoveProperty); }
      set { SetValue(IsOkToMoveProperty, value); }
    }
#endif

        internal void InitFromPanel(ChartPanel chartPanel)
        {
            Rect rcPanelBounds = chartPanel.CanvasRect;
            Canvas.SetTop(this, chartPanel.Top);
            Canvas.SetLeft(this, rcPanelBounds.Left);
            Width = rcPanelBounds.Width;
            Height = rcPanelBounds.Height + chartPanel.TitleBarHeight;
            IsOkToMove = false;
        }

        internal bool Visible
        {
            get { return Visibility == Visibility.Visible; }
            set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        internal double Top
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }
    }
}

