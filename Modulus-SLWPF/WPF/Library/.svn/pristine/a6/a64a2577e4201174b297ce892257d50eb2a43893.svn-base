using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.PaintObjects;
using Label = ModulusFE.PaintObjects.Label;
using Line = ModulusFE.PaintObjects.Line;

#if SILVERLIGHT
using ModulusFE.SL;
using ModulusFE.SL.Utils;
#endif

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public class YAxisCanvas : Canvas
    {

        private double _min;
        private double _max;
        internal ChartPanel _chartPanel;
        internal bool _isLeftAligned;
        internal bool _painted;

#if WPF
    private bool _mouseRightButtonDown;
#endif
        private Path _linesPath;
        private readonly PaintObjectsManager<Label> _labels = new PaintObjectsManager<Label>();

        ///<summary>
        /// Constructor
        ///</summary>
        public YAxisCanvas()
        {
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
#if WPF
      MouseRightButtonDown += OnMouseRightButtonDown;
      MouseRightButtonUp += OnMouseRightButtonUp;
#endif
#if SILVERLIGHT
            Mouse.RegisterMouseMoveAbleElement(this);
            MouseMove += (sender, e) => Mouse.UpdateMousePosition(this, e.GetPosition(this));
#endif

            _labels.NewObjectCreated += label =>
                                          {
                                              label._textBlock.FontSize = _chartPanel._chartX.FontSize;
                                              label._textBlock.Foreground = _chartPanel._chartX.FontForeground;
                                              label._textBlock.FontFamily = new FontFamily(_chartPanel._chartX.FontFace);
                                              label.ZIndex = ZIndexConstants.DarvasBoxes1;
                                          };
        }

        internal void UpdateFontInformation()
        {
            _labels.Do(label =>
                        {
                            label._textBlock.FontSize = _chartPanel._chartX.FontSize;
                            label._textBlock.Foreground = _chartPanel._chartX.FontForeground;
                            label._textBlock.FontFamily = new FontFamily(_chartPanel._chartX.FontFace);
                        });
        }

        private bool _resizing;
        private bool _moving;
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            Cursor = Cursors.Arrow;

            if (_resizing)
                _chartPanel.StopYResize(this);

            if (_moving)
                _chartPanel.StopYMoveUpDown(this);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();
            Cursor = Cursors.SizeNS;

            _resizing = _moving = false;
            if (!_chartPanel._chartX.CtrlDown)
            {
                _chartPanel.StartYResize(this);
                _resizing = true;
            }
            else
            {
                _chartPanel.StartYMoveUpDown(this);
                _moving = true;
            }
        }

#if WPF
    private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (_mouseRightButtonDown)
        ReleaseMouseCapture();

      _mouseRightButtonDown = false;
      Cursor = Cursors.Arrow;
      _chartPanel.StopYMoveUpDown(this);
    }

    private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      _mouseRightButtonDown = true;
      CaptureMouse();
      Cursor = Cursors.ScrollNS;
      _chartPanel.StartYMoveUpDown(this);
    }
#endif

        internal void SetMinMax(double min, double max)
        {
            SetMinMax(min, max, true);
        }

        internal void SetMinMax(double min, double max, bool render)
        {
            _min = min;
            _max = max;
            _painted = false;

            if (render)
                Render();
        }

        internal int LabelCount { set; private get; }

        internal double GridStep { set; private get; }

        internal void Render()
        {
            if (_linesPath == null)
            {
                _linesPath = new Path
                               {
                                   StrokeThickness = 1
                               };
                Children.Add(_linesPath);
                SetZIndex(_linesPath, ZIndexConstants.GridLines);

                _linesPath.SetBinding(Shape.StrokeProperty, _chartPanel._chartX.CreateOneWayBinding("GridStroke"));
            }

            _labels.C = this;
            _labels.Start();

            Rect rcBounds = new Rect(0, 0, ActualWidth, ActualHeight);

            if (Utils.GetIsInDesignMode(this))
            {
                _min = 0;
                _max = 1;
            }

            if (rcBounds.Height < 2)
                return;

            //int decimals = _chartPanel._hasVolume ? 0 : _chartPanel._chartX.ScalePrecision;
            string formatString = _chartPanel.FormatYValueString;
            bool isVolume = _chartPanel._hasVolume; // && (_chartPanel._chartX.VolumePostfixLetter.Length > 0);


            double k = rcBounds.Height / LabelCount;

            double min = _chartPanel.ScalingType == ScalingTypeEnum.Linear || isVolume
              ? _min
              : (_min > 0 ? Math.Log10(_min) : 0);
            double max = _chartPanel.ScalingType == ScalingTypeEnum.Linear || isVolume ? _max : Math.Log10(_max);

            double startValue = min + (max - min) * (_chartPanel._yOffset / rcBounds.Height);

            GridStep = (max - min) / LabelCount;

            _chartPanel.StartPaintingYGridLines();

            GeometryGroup gLines = new GeometryGroup();

            StringBuilder stringBuilder = new StringBuilder();
            double textHeight = _chartPanel._chartX.GetTextHeight("9");
            for (int i = 0; i < LabelCount; i++)
            {
                double y = rcBounds.Height - (i * k);
                if (double.IsNaN(y))
                    continue;

                if (y < 0)
                    break;

                if (i > 0)
                {
                    gLines.Children.Add(new LineGeometry
                                          {
                                              StartPoint = new Point(_isLeftAligned ? rcBounds.Width - 10 : 0, y),
                                              EndPoint = new Point(_isLeftAligned ? rcBounds.Width : 10, y),
                                          });

                    _chartPanel.PaintYGridLine(y);
                }

                stringBuilder.Length = 0;
                double value = startValue + (GridStep * i);
                if (_chartPanel.ScalingType == ScalingTypeEnum.Semilog && !isVolume)
                    value = Math.Pow(10, value);

                if (isVolume)
                    value /= _chartPanel._chartX.VolumeDivisor;

                stringBuilder.AppendFormat(formatString, value);

                if (isVolume && !string.IsNullOrEmpty(_chartPanel._chartX.VolumePostfixLetter))
                    stringBuilder.Append(" ").Append(_chartPanel._chartX.VolumePostfixLetter);

                if (!string.IsNullOrEmpty(_chartPanel.YAxisPostFix))
                    stringBuilder.Append(_chartPanel.YAxisPostFix);

                Label tb = _labels.GetPaintObject();
                tb.Text = stringBuilder.ToString();
                tb.Left = _isLeftAligned ? rcBounds.Width -
                  _chartPanel._chartX.GetTextWidth(stringBuilder.ToString()) - 2 : 2;
                tb.Top = y - textHeight - 2;
            }

            gLines.Children.Add(new LineGeometry
                                  {
                                      StartPoint = new Point(_isLeftAligned ? rcBounds.Width - 1 : 1, 0),
                                      EndPoint = new Point(_isLeftAligned ? rcBounds.Width - 1 : 1, rcBounds.Height),
                                  });

            _chartPanel.StopPaintingYGridLines();
            _painted = true;
            _labels.Stop();
            _linesPath.Data = gLines;
        }
    }
}
