using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ModulusFE.PaintObjects;
using Rectangle = System.Windows.Shapes.Rectangle;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public partial class PanelsContainer
    {
        private Rectangle _rcMinMax;

        private void EnsureMinMaxRectVisible()
        {
            if (_rcMinMax == null) return;
            _rcMinMax.Visibility = Visibility.Visible;
            //_panelsHolder.BringToFront(_rcMinMax);
            this.BringToFront(_rcMinMax);
            //this.BringToFront(_rcMinMax);
            //Canvas.SetZIndex(_rcMinMax, 100);
        }

        private void HideMinMaxRect()
        {
            if (_rcMinMax != null)
                _rcMinMax.Visibility = Visibility.Collapsed;
        }

        private static void InitAnimation(Storyboard storyboard, Rect rcFrom, Rect rcTo)
        {
            ((DoubleAnimation)storyboard.Children[0]).From = rcFrom.Width;
            ((DoubleAnimation)storyboard.Children[1]).From = rcFrom.Height;
            ((DoubleAnimation)storyboard.Children[2]).From = rcFrom.Top;
            ((DoubleAnimation)storyboard.Children[3]).From = rcFrom.Left;

            ((DoubleAnimation)storyboard.Children[0]).To = rcTo.Width;
            ((DoubleAnimation)storyboard.Children[1]).To = rcTo.Height;
            ((DoubleAnimation)storyboard.Children[2]).To = rcTo.Top;
            ((DoubleAnimation)storyboard.Children[3]).To = rcTo.Left;
        }

        private void CreateMinMaxRect()
        {
            if (_rcMinMax != null) return;
            //_panelsHolder.Children.Add(
            Children.Add(
              _rcMinMax = new Rectangle
                            {
                                Stroke = Brushes.White,
                                Fill = new SolidColorBrush(Color.FromArgb(0xCC, 0xC0, 0xC0, 0xC0)),
                                StrokeThickness = 2,
                                RenderTransform = new TransformGroup(),
                                RenderTransformOrigin = new Point(0.5, 0.5)
                            });
            ((TransformGroup)_rcMinMax.RenderTransform).Children.Add(new RotateTransform());
        }

        private static void CreateAnimationObject(Storyboard animation, EventHandler compEvent)
        {
            if (animation.Children.Count > 0)
                return;

            //_maximizationStoryboard.Duration = TimeSpan.FromSeconds(2);
            if (compEvent != null)
                animation.Completed += compEvent;

            TimeSpan duration = TimeSpan.FromMilliseconds(250);

            DoubleAnimation widthAnimation = new DoubleAnimation();
            animation.Children.Add(widthAnimation);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));
            widthAnimation.Duration = duration;

            DoubleAnimation heightAnimation = new DoubleAnimation();
            animation.Children.Add(heightAnimation);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));
            heightAnimation.Duration = duration;

            DoubleAnimation topAnimation = new DoubleAnimation();
            animation.Children.Add(topAnimation);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath(Canvas.TopProperty));
            topAnimation.Duration = duration;

            DoubleAnimation leftAnimation = new DoubleAnimation();
            animation.Children.Add(leftAnimation);
            Storyboard.SetTargetProperty(leftAnimation, new PropertyPath(Canvas.LeftProperty));
            leftAnimation.Duration = duration;

            /*DoubleAnimation rotateAnimation = new DoubleAnimation();
            animation.Children.Add(rotateAnimation);
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("RenderTransform.Children[0].Angle"));
            rotateAnimation.From = 360;
            rotateAnimation.To = 0.0;
            rotateAnimation.Duration = duration;*/
        }

        private void MaximizationCompletedEvent(object sender, EventArgs args)
        {
            _rcMinMax.Visibility = Visibility.Collapsed;
            if (_maximizedPanel.State == ChartPanel.StateType.Maximized) //maximized
            {
                MaximizePanel();
            }
            else
            {
                RestoreMaximizedPanel();
            }
        }
    }
}

