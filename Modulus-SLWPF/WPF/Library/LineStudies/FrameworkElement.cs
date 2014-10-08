using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.Exceptions;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;

namespace ModulusFE
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_WpfFrameworkElement()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.FrameworkElement,
              typeof(LineStudies.FrameworkElement), "FrameworkElement");
        }
    }
}


namespace ModulusFE.LineStudies
{
    ///<summary>
    /// WPF UI Element
    ///</summary>
    public class FrameworkElement : LineStudy
    {
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="stroke"></param>
        ///<param name="chartPanel"></param>
        public FrameworkElement(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.FrameworkElement;
        }

        internal override void SetArgs(params object[] args)
        {
            Debug.Assert(args.Length > 0);

            Element = args[0] as System.Windows.FrameworkElement;
            if (Element == null)
                throw new ChartException("FrameworkElement class can work only objects derived from System.Windows.FrameworkElement");

            Element.Visibility = Visibility.Collapsed;
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (Element == null)
            {
                Debug.WriteLine("FrameworkElement NULL");
                return;
            }

            if (lineStatus == LineStatus.StartPaint)
            {
                C.Children.Add(Element);

                Canvas.SetZIndex(Element, ZIndexConstants.LineStudies1);
                Element.Tag = this;
                Element.SetBinding(Canvas.TopProperty, this.CreateOneWayBinding("CanvasTop"));
                Element.SetBinding(Canvas.LeftProperty, this.CreateOneWayBinding("CanvasLeft"));
                Element.SetBinding(System.Windows.FrameworkElement.WidthProperty, this.CreateOneWayBinding("ElementWidth"));
                Element.SetBinding(System.Windows.FrameworkElement.HeightProperty, this.CreateOneWayBinding("ElementHeight"));

                _internalObjectCreated = true;

                xEnumVisuals(Element);

                return;
            }

            if (Element.Visibility == Visibility.Collapsed)
                Element.Visibility = Visibility.Visible;

            rect.Normalize();
            //Canvas.SetLeft(Element, rect.Left);
            //Canvas.SetTop(Element, rect.Top);
            //Element.Width = rect.Width;
            //Element.Height = rect.Height;
            CanvasLeft = rect.Left;
            CanvasTop = rect.Top;
            ElementWidth = rect.Width;
            ElementHeight = rect.Height;
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            List<SelectionDotInfo> res =
              new List<SelectionDotInfo>
          {
            new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
            new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = _newRect.TopRight},
            new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
            new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
          };
            return res;
        }

        internal override void SetCursor()
        {
            if (_selectionVisible && Element.Cursor != Cursors.Hand)
            {
                Element.Cursor = Cursors.Hand;
                return;
            }
            if (_selectionVisible || Element.Cursor == Cursors.Arrow) return;
            Element.Cursor = Cursors.Arrow;
        }

        internal override void RemoveLineStudy()
        {
            C.Children.Remove(Element);
        }

        internal override void SetOpacity()
        {
            Element.Opacity = Opacity;
        }

        private void xEnumVisuals(System.Windows.FrameworkElement root)
        {
            Stack<System.Windows.FrameworkElement> children
              = new Stack<System.Windows.FrameworkElement>();

            children.Push(root);

            while (children.Count > 0)
            {
                System.Windows.FrameworkElement child = children.Pop();

                child.Tag = this;

                if (child is ContentControl)
                {
                    System.Windows.FrameworkElement contentElement
                      = ((ContentControl)child).Content as System.Windows.FrameworkElement;
                    if (contentElement != null)
                        children.Push(contentElement);
                    continue;
                }

                if (child is Panel)
                {
                    Panel panelElement = child as Panel;
                    foreach (System.Windows.FrameworkElement panelChild in panelElement.Children)
                    {
                        children.Push(panelChild);
                    }
                }
            }
        }

        ///<summary>
        /// Returns a reference to the internal <see cref="System.Windows.FrameworkElement"/>
        ///</summary>
        public System.Windows.FrameworkElement Element { get; private set; }

        #region CanvasLeft

        private double _canvasLeft;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs CanvasLeftChangedEventsArgs =
          ObservableHelper.CreateArgs<StaticText>(_ => _.CanvasLeft);

        ///<summary>
        /// Gets or sets the left position of text object
        ///</summary>
        public double CanvasLeft
        {
            get { return _canvasLeft; }
            set
            {
                if (_canvasLeft != value)
                {
                    _canvasLeft = value;
                    InvokePropertyChanged(CanvasLeftChangedEventsArgs);
                }
            }
        }

        #endregion

        #region CanvasTop

        private double _canvasTop;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs CanvasTopChangedEventsArgs =
          ObservableHelper.CreateArgs<StaticText>(_ => _.CanvasTop);

        ///<summary>
        /// Gets or sets the top position of text object
        ///</summary>
        public double CanvasTop
        {
            get { return _canvasTop; }
            set
            {
                if (_canvasTop != value)
                {
                    _canvasTop = value;
                    InvokePropertyChanged(CanvasTopChangedEventsArgs);
                }
            }
        }

        #endregion

        #region ElementWidth

        private double _elementWidth;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs ElementWidthChangedEventsArgs =
          ObservableHelper.CreateArgs<FrameworkElement>(_ => _.ElementWidth);

        /// <summary>
        /// Gets or sets the element width
        /// </summary>
        public double ElementWidth
        {
            get { return _elementWidth; }
            set
            {
                if (_elementWidth != value)
                {
                    _elementWidth = value;
                    InvokePropertyChanged(ElementWidthChangedEventsArgs);
                }
            }
        }

        #endregion

        #region ElementHeight

        private double _elementHeight;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs ElementHeightChangedEventsArgs =
          ObservableHelper.CreateArgs<FrameworkElement>(_ => _.ElementHeight);

        /// <summary>
        /// Gets or sets the element height
        /// </summary>
        public double ElementHeight
        {
            get { return _elementHeight; }
            set
            {
                if (_elementHeight != value)
                {
                    _elementHeight = value;
                    InvokePropertyChanged(ElementHeightChangedEventsArgs);
                }
            }
        }

        #endregion
    }
}
