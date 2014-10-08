using System.Windows.Media;

namespace ModulusFE.LineStudies
{
    ///<summary>
    /// An interface that specifies that an object is able to have different properties specified to a shape
    ///</summary>
    public interface IShapeAble
    {
        ///<summary>
        /// Gets or sets the Brush that specifies how the shape's interior is filled
        ///</summary>
        Brush Fill { get; set; }
    }

    ///<summary>
    ///</summary>
    public interface IMouseAble
    {
        ///<summary>
        /// Occurs when any mouse button is pressed while the pointer is over this element.
        ///</summary>
        event System.Windows.Input.MouseButtonEventHandler MouseDown;

        ///<summary>
        /// Occurs when the mouse pointer enters the bounds of this element.
        ///</summary>
        event System.Windows.Input.MouseEventHandler MouseEnter;

        ///<summary>
        /// Occurs when the mouse pointer leaves the bounds of this element.
        ///</summary>
        event System.Windows.Input.MouseEventHandler MouseLeave;

        ///<summary>
        /// Occurs when the mouse pointer moves while over this element.
        ///</summary>
        event System.Windows.Input.MouseEventHandler MouseMove;

        ///<summary>
        /// Occurs when any mouse button is released over this element.
        ///</summary>
        event System.Windows.Input.MouseButtonEventHandler MouseUp;
    }
}
