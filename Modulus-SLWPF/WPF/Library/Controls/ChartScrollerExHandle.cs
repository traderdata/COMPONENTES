using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE.Controls
{
    /// <summary>
    /// Scroller handle
    /// </summary>
    public class ChartScrollerExHandle : Control
    {
#if WPF
    static ChartScrollerExHandle()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartScrollerExHandle), new FrameworkPropertyMetadata(typeof(ChartScrollerExHandle)));
    }
#endif

        /// <summary>
        /// Ctor
        /// </summary>
        public ChartScrollerExHandle()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartScrollerExHandle);
            Background = new SolidColorBrush(ColorsEx.Silver);
#else
			Background = new SolidColorBrush(Colors.Silver);
#endif

            Stroke = new SolidColorBrush(Colors.White);
        }

        #region StrokeProperty

        /// <summary>
        /// Identifies the <see cref="Stroke"/> dependency property 
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
          DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartScrollerExHandle),
                                      new PropertyMetadata(null, (o, args) => ((ChartScrollerExHandle)o).StrokeChanged(args)));

        /// <summary>
        /// Gets or sets the stroke color. This is a  dependency property.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void StrokeChanged(DependencyPropertyChangedEventArgs args)
        {

        }

        #endregion
    }
}
