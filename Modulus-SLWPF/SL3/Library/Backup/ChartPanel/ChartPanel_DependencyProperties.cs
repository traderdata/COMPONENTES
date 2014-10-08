using System.ComponentModel;
using System.Windows;
using ModulusFE.LineStudies;

namespace ModulusFE
{
    using System.Windows.Media;

    /// <summary>
    /// ChartPanel - container for all series and line studies.
    /// </summary>
    public partial class ChartPanel
    {
        #region ScalingType

        private ScalingTypeEnum _scalingType = ScalingTypeEnum.Linear;

        /// <summary>
        /// Provides <see cref="PropertyChangedEventArgs"/> for property <see cref="ScalingType"/>
        /// </summary>
        public static readonly PropertyChangedEventArgs ScalingTypeChangedEventsArgs =
            ObservableHelper.CreateArgs<ChartPanel>(_ => _.ScalingType);

        /// <summary>
        /// Gets or sets the ScalingType value.
        /// </summary>
        public ScalingTypeEnum ScalingType
        {
            get { return _scalingType; }
            set
            {
                if (_scalingType != value)
                {
                    _scalingType = value;
                    InvokePropertyChanged(ScalingTypeChangedEventsArgs);
                }
            }
        }

        #endregion

        #region IncludeLineStudiesInSeriesMinMaxProperty (DependencyProperty)

        /// <summary>
        /// Gets or sets whether to analize the position of <see cref="LineStudy"/> when getting min &amp; max to plot the Y axis
        /// By default only values from <see cref="Series"/> are taken.
        /// </summary>
        public bool IncludeLineStudiesInSeriesMinMax
        {
            get { return (bool)GetValue(IncludeLineStudiesInSeriesMinMaxProperty); }
            set { SetValue(IncludeLineStudiesInSeriesMinMaxProperty, value); }
        }

        /// <summary>
        /// IncludeLineStudiesInSeriesMinMax
        /// </summary>
        public static readonly DependencyProperty IncludeLineStudiesInSeriesMinMaxProperty =
          DependencyProperty.Register("IncludeLineStudiesInSeriesMinMax", typeof(bool), typeof(ChartPanel),
                                      new PropertyMetadata(false, OnIncludeLineStudiesInSeriesMinMaxChanged));

        private static void OnIncludeLineStudiesInSeriesMinMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartPanel)d).OnIncludeLineStudiesInSeriesMinMaxChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="IncludeLineStudiesInSeriesMinMax"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnIncludeLineStudiesInSeriesMinMaxChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AutoResetIncludeLineStudiesMinMaxProperty (DependencyProperty)

        /// <summary>
        /// Gets os sets whether the min &amp; max values from painted LineStudies will affect 
        /// the Y scale paiting after user chose to show all non-visible LineStudies.
        /// </summary>
        public bool AutoResetIncludeLineStudiesMinMax
        {
            get { return (bool)GetValue(AutoResetIncludeLineStudiesMinMaxProperty); }
            set { SetValue(AutoResetIncludeLineStudiesMinMaxProperty, value); }
        }

        /// <summary>
        /// AutoResetIncludeLineStudiesMinMax
        /// </summary>
        public static readonly DependencyProperty AutoResetIncludeLineStudiesMinMaxProperty =
          DependencyProperty.Register("AutoResetIncludeLineStudiesMinMax", typeof(bool), typeof(ChartPanel),
                                      new PropertyMetadata(true, OnAutoResetIncludeLineStudiesMinMaxChanged));

        private static void OnAutoResetIncludeLineStudiesMinMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartPanel)d).OnAutoResetIncludeLineStudiesMinMaxChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="AutoResetIncludeLineStudiesMinMax"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnAutoResetIncludeLineStudiesMinMaxChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

#if WPF

        #region UseAliasedEdgeModeProperty (DependencyProperty)

    /// <summary>
    /// </summary>
    public bool UseAliasedEdgeMode
    {
      get { return (bool)GetValue(UseAliasedEdgeModeProperty); }
      set { SetValue(UseAliasedEdgeModeProperty, value); }
    }

    /// <summary>
    /// UseAliasedEdgeMode
    /// </summary>
    public static readonly DependencyProperty UseAliasedEdgeModeProperty =
      DependencyProperty.Register("UseAliasedEdgeMode", typeof (bool), typeof (ChartPanel),
                                  new PropertyMetadata(true, OnUseAliasedEdgeModeChanged));

    private static void OnUseAliasedEdgeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartPanel)d).OnUseAliasedEdgeModeChanged(e);
    }

    /// <summary>
    /// Method called when <see cref="UseAliasedEdgeMode"/> property changes
    /// </summary>
    /// <param name="e">Arguments</param>
    protected virtual void OnUseAliasedEdgeModeChanged(DependencyPropertyChangedEventArgs e)
    {
      ToggleEdgeMode();
    }

    #endregion
#endif

        #region TitleBarBackgroundProperty (DependencyProperty)

        /// <summary>
        /// Gets or sets the chartpanel's title bar background
        /// </summary>
        public Brush TitleBarBackground
        {
            get
            {
                return (Brush)GetValue(TitleBarBackgroundProperty);
            }
            set
            {
                SetValue(TitleBarBackgroundProperty, value);
            }
        }

        /// <summary>
        /// TitleBarBackground
        /// </summary>
        public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register(
          "TitleBarBackground", typeof(Brush), typeof(ChartPanel),
          new PropertyMetadata(ChartPanelTitleBar.DefaultBrush, OnTitleBarBackgroundChanged));

        private static void OnTitleBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartPanel)d).OnTitleBarBackgroundChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="TitleBarBackground"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnTitleBarBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_titleBar != null)
            {
                _titleBar.Background = (Brush)e.NewValue;
            }
        }

        #endregion

        #region TitleBarButtonForeground

        private Brush _titleBarButtonForeground = new SolidColorBrush(Colors.White);

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs TitleBarButtonForegroundChangedEventsArgs =
          ObservableHelper.CreateArgs<ChartPanel>(_ => _.TitleBarButtonForeground);

        ///<summary>
        /// Gets or sets the title bar button foreground (for all 3 buttons)
        ///</summary>
        public Brush TitleBarButtonForeground
        {
            get { return _titleBarButtonForeground; }
            set
            {
                if (_titleBarButtonForeground != value)
                {
                    _titleBarButtonForeground = value;
                    InvokePropertyChanged(TitleBarButtonForegroundChangedEventsArgs);
                }
            }
        }

        #endregion
    }
}

