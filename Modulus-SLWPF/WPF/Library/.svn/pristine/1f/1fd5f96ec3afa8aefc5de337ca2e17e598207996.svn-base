using System.Windows;

namespace ModulusFE
{
    internal class ElementInvalidator
    {
        // Methods
        public static void Invalidate(FrameworkElement target, InvalidationType invalidationTypes)
        {
            if (target == null) return;

            if ((invalidationTypes & InvalidationType.Measure) != 0)
            {
                target.InvalidateMeasure();
            }
            if ((invalidationTypes & InvalidationType.Arrange) != 0)
            {
                target.InvalidateArrange();
            }
            if ((invalidationTypes & InvalidationType.Visual) != 0)
            {
                target.InvalidateVisual();
            }
            FrameworkElement parent = target.Parent as FrameworkElement;
            if (parent == null) return;
            if ((invalidationTypes & InvalidationType.ParentMeasure) != 0)
            {
                parent.InvalidateMeasure();
            }
            if ((invalidationTypes & InvalidationType.ParentArrange) != 0)
            {
                parent.InvalidateArrange();
            }
        }

        public static void PropertyChanged_InvalidateArrange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.Arrange);
        }

        public static void PropertyChanged_InvalidateMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.Measure);
        }

        public static void PropertyChanged_InvalidateMeasureArrange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.Arrange | InvalidationType.Measure);
        }

        public static void PropertyChanged_InvalidateMeasureArrangeVisual(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.Visual | InvalidationType.Arrange | InvalidationType.Measure);
        }

        public static void PropertyChanged_InvalidateMeasureArrangeVisualParentMeasureArrange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.ParentArrange | InvalidationType.ParentMeasure | InvalidationType.Visual | InvalidationType.Arrange | InvalidationType.Measure);
        }

        public static void PropertyChanged_InvalidateParentArrange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.ParentArrange);
        }

        public static void PropertyChanged_InvalidateParentMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.ParentMeasure);
        }

        public static void PropertyChanged_InvalidateVisual(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Invalidate(d as FrameworkElement, InvalidationType.Visual);
        }
    }
}
