using System.Windows;
using System.Windows.Controls;
#if SILVERLIGHT
using System.Windows.Media;
using ModulusFE.SL;
#endif

namespace ModulusFE
{
#if SILVERLIGHT
    /// <summary>
    /// Internal usage only
    /// </summary>
    [TemplatePart(Name = BackgroundElement, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TextElement, Type = typeof(TextBlock))]
#endif
    /// <summary>
    /// Internal usage only
    /// </summary>
    public partial class MoveSeriesIndicator : Control
    {
#if WPF
    static MoveSeriesIndicator()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveSeriesIndicator), new FrameworkPropertyMetadata(typeof(MoveSeriesIndicator)));

      MoveStatusProperty =
        DependencyProperty.Register("MoveStatus", typeof (MoveStatusEnum),
                                    typeof (MoveSeriesIndicator),
                                    new FrameworkPropertyMetadata(MoveStatusEnum.CantMove, OnMoveStatusChanged));
    }

    private static void OnMoveStatusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
    }
    ///<summary>
    ///</summary>
    public static readonly DependencyProperty MoveStatusProperty;
    ///<summary>
    ///</summary>
    public MoveStatusEnum MoveStatus
    {
      get { return (MoveStatusEnum)GetValue(MoveStatusProperty); }
      set { SetValue(MoveStatusProperty, value); }
    }
#endif
#if SILVERLIGHT
        private const string BackgroundElement = "PART_Background";
        private const string TextElement = "PART_Text";
        ///<summary>
        /// ctor
        ///</summary>
        public MoveSeriesIndicator()
        {
            DefaultStyleKey = typeof(MoveSeriesIndicator);
        }

        ///<summary>
        ///</summary>
        public static readonly DependencyProperty BackgroundExProperty =
          DependencyProperty.Register("BackgroundEx", typeof(Brush), typeof(MoveSeriesIndicator),
                                      new PropertyMetadata(Brushes.Red, (d, e) => ((MoveSeriesIndicator)d).OnBackgroundExChanged(d, e)));

        ///<summary>
        /// Gets or sets the Background brush
        ///</summary>
        public Brush BackgroundEx
        {
            get { return (Brush)GetValue(BackgroundExProperty); }
            set { SetValue(BackgroundExProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected virtual void OnBackgroundExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        ///<summary>
        ///</summary>
        public static readonly DependencyProperty TextExProperty =
          DependencyProperty.Register("TextEx", typeof(string), typeof(MoveSeriesIndicator),
                                      new PropertyMetadata("Can't move", (d, e) => ((MoveSeriesIndicator)d).OnTextExChanged(d, e)));

        ///<summary>
        /// Gets or sets the text
        ///</summary>
        public string TextEx
        {
            get { return (string)GetValue(TextExProperty); }
            set { SetValue(TextExProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected virtual void OnTextExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        ///<summary>
        ///</summary>
        public static readonly DependencyProperty MoveStatusProperty =
          DependencyProperty.Register("MoveStatus", typeof(MoveStatusEnum), typeof(MoveSeriesIndicator),
                                      new PropertyMetadata(MoveStatusEnum.CantMove, (d, e) => ((MoveSeriesIndicator)d).OnMoveStatusChanged(d, e)));

        ///<summary>
        /// Gets or sets the Move status
        ///</summary>
        public MoveStatusEnum MoveStatus
        {
            get { return (MoveStatusEnum)GetValue(MoveStatusProperty); }
            set { SetValue(MoveStatusProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected virtual void OnMoveStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MoveStatusEnum newStatus = (MoveStatusEnum)e.NewValue;
            MoveStatusEnum oldStatus = (MoveStatusEnum)e.OldValue;
            if (newStatus == oldStatus) return;

            MoveSeriesIndicator moveSeriesIndicator = (MoveSeriesIndicator)d;
            switch (newStatus)
            {
                case MoveStatusEnum.CantMove:
                    moveSeriesIndicator.BackgroundEx = Brushes.Red;
                    moveSeriesIndicator.TextEx = "Can't move";
                    break;
                case MoveStatusEnum.MoveToExistingPanel:
                    moveSeriesIndicator.BackgroundEx = Brushes.Blue;
                    moveSeriesIndicator.TextEx = "Move to existing panel";
                    break;
                case MoveStatusEnum.MoveToNewPanel:
                    moveSeriesIndicator.BackgroundEx = Brushes.Green;
                    moveSeriesIndicator.TextEx = "Move to new panel";
                    break;
            }
        }
#endif

        ///<summary>
        ///</summary>
        public enum MoveStatusEnum
        {
            /// <summary>
            /// series can't be moved
            /// 1. cause it is droped on same panel
            /// 2. cause the unique series from panel is used to create a new panel
            /// </summary>
            CantMove,
            /// <summary>
            /// only a series from a panel with multiple series can be used to create a new panel
            /// this flag also includes that series can be droped on an existing panel.
            /// </summary>
            MoveToNewPanel,
            /// <summary>
            /// any series can be droped on an existing panel
            /// </summary>
            MoveToExistingPanel
        }

        internal double X
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }

        internal double Y
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }
    }
}
