using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public class PopupSL
    {
        private readonly Popup _popup;

        ///<summary>
        /// Ctor
        ///</summary>
        ///<param name="popup"></param>
        public PopupSL(Popup popup)
        {
            _popup = popup;

            Placement = PlacementMode.Bottom;
        }

        ///<summary>
        ///</summary>
        public UIElement PlacementTarget { get; set; }

        ///<summary>
        ///</summary>
        public PlacementMode Placement { get; set; }

        ///<summary>
        ///</summary>
        public bool IsOpen
        {
            get { return _popup.IsOpen; }
            set
            {
                if (value)
                {
                    SetPosition();
                }
                _popup.IsOpen = value;
            }
        }

        private void SetPosition()
        {
            GeneralTransform objGeneralTransform = PlacementTarget.TransformToVisual(Application.Current.RootVisual);
            Point point = objGeneralTransform.Transform(new Point(0, 0));
            Rect rcElement = new Rect(point, PlacementTarget.RenderSize);

            _popup.HorizontalOffset = rcElement.Left;
            switch (Placement)
            {
                case PlacementMode.Bottom:
                    _popup.VerticalOffset = rcElement.Top + rcElement.Height;
                    break;
                case PlacementMode.Top:
                    _popup.Child.UpdateLayout();
                    _popup.VerticalOffset = rcElement.Top - _popup.Child.RenderSize.Height;
                    break;
            }
        }
    }
}
