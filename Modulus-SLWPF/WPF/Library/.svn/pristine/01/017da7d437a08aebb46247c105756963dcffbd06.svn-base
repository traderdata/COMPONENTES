using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.ChartElementProperties;
using ModulusFE.LineStudies;

namespace ModulusFE.PaintObjects
{
    internal interface IContextAbleLineStudy
    {
        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        UIElement Element { get; }
        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        Segment Segment { get; }
        /// <summary>
        /// Parent where <see cref="Element"/> belongs
        /// </summary>
        Canvas Parent { get; }
        /// <summary>
        /// Gets if <see cref="Element"/> is selected
        /// </summary>
        bool IsSelected { get; }
        /// <summary>
        /// Z Index of <see cref="Element"/>
        /// </summary>
        int ZIndex { get; }
        /// <summary>
        /// Gets the chart object associated with <see cref="Element"/> object
        /// </summary>
        StockChartX Chart { get; }
        /// <summary>
        /// Gets the reference to <see cref="LineStudies.LineStudy"/> 
        /// </summary>
        LineStudy LineStudy { get; }


    }

    internal class ContextLine
    {
        private const int ContextLineStrokeThickness = 14;

        private Point[] _surroundPolygon;
        private readonly Line _line;

        public IContextAbleLineStudy ContextAbleLineStudy { get; private set; }

        public ContextLine(IContextAbleLineStudy contextAble)
        {
            ContextAbleLineStudy = contextAble;

            ContextAbleLineStudy.Element.MouseEnter += ParentOnMouseEnter;
            ContextAbleLineStudy.Element.MouseMove += ParentOnMouseMove;
            ContextAbleLineStudy.Element.MouseLeave += ParentOnMouseLeave;

            _line = new Line();
            _line.AddTo(ContextAbleLineStudy.Parent);
            _line.Visible = false;
            _line._line.StrokeStartLineCap = PenLineCap.Round;
            _line._line.StrokeEndLineCap = PenLineCap.Round;
            _line._line.StrokeThickness = ContextLineStrokeThickness;
            _line.Stroke = new SolidColorBrush(Color.FromArgb(0x55, 0xCC, 0xCC, 0xCC));
            _line._line.Cursor = Cursors.Hand;
            _line._line.Tag = this;
            _line.ZIndex = ContextAbleLineStudy.ZIndex + 1;

            _line._line.MouseLeftButtonUp += LineOnMouseLeftButtonUp;
            _line._line.MouseLeave += LineOnMouseLeave;
        }

        public bool Visible
        {
            get { return _line != null && _line.Visible; }
            private set { _line.Visible = value; }
        }

        internal void LsContextMenuChoose(int menuId)
        {
            switch (menuId)
            {
                case 0: //properties
                    if (ContextAbleLineStudy.LineStudy != null)
                    {
                        IChartElementPropertyAble propertyAble = ContextAbleLineStudy.LineStudy;

                        List<IChartElementProperty> properties = new List<IChartElementProperty>(propertyAble.Properties);
                        PropertiesDialog dialog = new PropertiesDialog(propertyAble.Title, properties)
                                                    {
#if WPF
                                          Owner = ContextAbleLineStudy.Chart.OwnerWindow,
#endif
                                                        Background = ContextAbleLineStudy.Chart.LineStudyPropertyDialogBackground
                                                    };

#if SILVERLIGHT
                        //dialog.Show(Dialog.DialogStyle.ModalDimmed);
                        dialog.Show();
#endif
#if WPF
            dialog.ShowDialog();
#endif
                    }
                    break;
                case 1: //delete
                    ContextAbleLineStudy.Chart.RemoveObject(ContextAbleLineStudy.LineStudy);
                    break;
            }
        }

        private void LineOnMouseLeave(object sender, MouseEventArgs args)
        {
            Visible = false;
            //      _contextAble.Element.MouseEnter += ParentOnMouseEnter;
            //      _contextAble.Element.MouseLeave += ParentOnMouseLeave;
        }

        private void LineOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            //      _contextAble.Element.MouseEnter += ParentOnMouseEnter;
            //      _contextAble.Element.MouseLeave += ParentOnMouseLeave;

            Visible = false;

            if (ContextAbleLineStudy.Chart.InvokeLineStudyContextMenu(ContextAbleLineStudy.LineStudy))
            {
                return; //host program canceled context menu
            }

            //show context menu
            ContextAbleLineStudy.Chart.ShowLineStudyContextMenu(args.GetPosition(ContextAbleLineStudy.Chart), this);
        }

        private void ParentOnMouseLeave(object sender, MouseEventArgs args)
        {
            Visible = false;
        }

        private bool _eventsRemoved;
        private void ParentOnMouseMove(object sender, MouseEventArgs args)
        {
            if (!ContextAbleLineStudy.IsSelected || ContextAbleLineStudy.LineStudy.IsContextMenuDisabled)
            {
                return;
            }

            if (_surroundPolygon == null)
            {
                PositionLine();
            }

            Visible = args.GetPosition(ContextAbleLineStudy.Parent).InPolygon(_surroundPolygon);
            if (Visible)
            {
                _eventsRemoved = true;
                ContextAbleLineStudy.Element.MouseEnter -= ParentOnMouseEnter;
                ContextAbleLineStudy.Element.MouseLeave -= ParentOnMouseLeave;
            }
            else if (_eventsRemoved)
            {
                _eventsRemoved = false;
                ContextAbleLineStudy.Element.MouseEnter += ParentOnMouseEnter;
                ContextAbleLineStudy.Element.MouseLeave += ParentOnMouseLeave;
            }
        }

        private void ParentOnMouseEnter(object sender, MouseEventArgs args)
        {
            if (!ContextAbleLineStudy.IsSelected || Visible || ContextAbleLineStudy.LineStudy.IsContextMenuDisabled)
            {
                return;
            }

            PositionLine();
            //      _contextAble.Element.MouseEnter -= ParentOnMouseEnter;
            //      _contextAble.Element.MouseLeave -= ParentOnMouseLeave;
        }

        private void PositionLine()
        {
            Segment segment = ContextAbleLineStudy.Segment;
            _surroundPolygon = segment.SurroundRectangle(ContextLineStrokeThickness);
            _line._line.X1 = segment.X1;
            _line._line.Y1 = segment.Y1;
            _line._line.X2 = segment.X2;
            _line._line.Y2 = segment.Y2;
        }
    }
}
