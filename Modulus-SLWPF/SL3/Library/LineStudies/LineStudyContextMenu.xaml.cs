using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.PaintObjects;

namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Interaction logic for LineStudyContextMenu.xaml
    /// </summary>
    public partial class LineStudyContextMenu
    {
        private readonly SolidColorBrush MouseOverBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFD, 0xD8, 0x6C));

        ///<summary>
        ///</summary>
        public event EventHandler<EventArgs<int>> MenuItemClick;

        internal ContextLine ContextLine { get; set; }

        private bool _subscribedToEvents;
        ///<summary>
        ///</summary>
        public LineStudyContextMenu()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender1, RoutedEventArgs args1)
        {
            if (_subscribedToEvents) return;

            _subscribedToEvents = true;

            Canvas.SetZIndex(this, ZIndexConstants.LineStudyContextMenu);

            int panelIndex = 0;
            foreach (UIElement child in panelItemsContainer.Children)
            {
                if (!(child is StackPanel)) continue;

                StackPanel panel = (StackPanel)child;
                panel.MouseEnter
                  += (sender, args) =>
                       {
                           panel.Background = MouseOverBrush;
                       };
                panel.MouseLeave
                  += (sender, args) =>
                       {
                           panel.Background = null;
                       };
                panel.MouseLeftButtonUp += PanelOnMouseUp;
                panel.Tag = panelIndex++;
            }

            MouseLeftButtonUp += (sender, args) =>
                                   {
                                       Visibility = Visibility.Collapsed;
                                   };
        }

        private void PanelOnMouseUp(object sender, MouseButtonEventArgs args)
        {
            Visibility = Visibility.Collapsed;

            int panelIndex = Convert.ToInt16(((StackPanel)sender).Tag);

            if (MenuItemClick != null)
                MenuItemClick(this, new EventArgs<int>(panelIndex));

            ContextLine.LsContextMenuChoose(panelIndex);
        }
        ///<summary>
        ///</summary>
        ///<param name="position"></param>
        public void Show(Point position)
        {
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            Width = ((Panel)Parent).ActualWidth;
            Height = ((Panel)Parent).ActualHeight;

            Canvas.SetLeft(gridContainer, position.X);
            Canvas.SetTop(gridContainer, position.Y);
            Visibility = Visibility.Visible;
        }
    }
}
