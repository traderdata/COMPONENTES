using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModulusFE.Interfaces;

namespace ModulusFE
{
    ///<summary>
    /// Base class for all series types used in the chart
    ///</summary>
    public partial class Series
    {
        ///<summary>
        /// The "bridge" used to send data from Series about last price to its tick box value presenter
        ///</summary>
        public IValueBridge<Series> SeriesValueBridge
        {
            get { return _seriesValueBridge; }
            set
            {
                _seriesValueBridge = value;

                if (_seriesTickBoxValuePresenterLeft != null)
                {
                    SetValuePresenterDataContext();
                }

            }
        }

        private void SetValuePresenterDataContext()
        {
            _seriesTickBoxValuePresenterLeft.DataContext = SeriesValueBridge;
            _seriesTickBoxValuePresenterRight.DataContext = SeriesValueBridge;

            _seriesValueBridge.AttachDataSupplier(this, new[] { typeof(double) });
        }

        private IValueBridge<Series> _seriesValueBridge;

        private TickBoxPosition _showTickBox;
        private SeriesTickBoxValuePresenter _seriesTickBoxValuePresenterLeft;
        private SeriesTickBoxValuePresenter _seriesTickBoxValuePresenterRight;

        ///<summary>
        /// Places a tick box on one of the Y axes
        ///</summary>
        public TickBoxPosition TickBox
        {
            get { return _showTickBox; }
            set
            {
                //        if (_showTickBox == value) return;
                _showTickBox = value;
                CreateTickBoxValuePresenter();
                if (_seriesTickBoxValuePresenterLeft == null)
                {
                    return;
                }

                var t = _chartPanel._chartX.SeriesTickBoxValuePresenterTemplate;
                if (t != null)
                {
                    _seriesTickBoxValuePresenterLeft.SetValue(Control.TemplateProperty, t);
                    _seriesTickBoxValuePresenterRight.SetValue(Control.TemplateProperty, t);
                }

                if (SeriesValueBridge == null)
                {
                    SeriesValueBridge = new SeriesDefValueBridge();
                    SeriesDefValueBridge bridge = (SeriesDefValueBridge)SeriesValueBridge;
                    bridge.UpBrush = new SolidColorBrush(UpColor != null ? UpColor.Value : _chartPanel._chartX.UpColor);
                    bridge.DownBrush = new SolidColorBrush(DownColor != null ? DownColor.Value : _chartPanel._chartX.DownColor);
                    bridge.BackgroundEx = bridge.UpBrush;
                }

                SetValuePresenterDataContext();

                switch (_showTickBox)
                {
                    case TickBoxPosition.Left:
                        _seriesTickBoxValuePresenterLeft.Visibility = Visibility.Visible;
                        _seriesTickBoxValuePresenterRight.Visibility = Visibility.Collapsed;
                        _seriesTickBoxValuePresenterLeft.Show();
                        break;
                    case TickBoxPosition.Right:
                        _seriesTickBoxValuePresenterLeft.Visibility = Visibility.Collapsed;
                        _seriesTickBoxValuePresenterRight.Visibility = Visibility.Visible;
                        _seriesTickBoxValuePresenterRight.Show();
                        break;
                    default:
                        _seriesTickBoxValuePresenterLeft.Visibility = Visibility.Collapsed;
                        _seriesTickBoxValuePresenterRight.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private bool _tickBoxNeedsCreated;
        private void CreateTickBoxValuePresenter()
        {
            if (_chartPanel == null || _chartPanel._leftYAxis == null)
            {
                _tickBoxNeedsCreated = true;
                return;
            }

            if (_seriesTickBoxValuePresenterLeft != null)
            {
                return;
            }

            _seriesTickBoxValuePresenterLeft = new SeriesTickBoxValuePresenter(this);
            _chartPanel._leftYAxis.Children.Add(_seriesTickBoxValuePresenterLeft);
            _seriesTickBoxValuePresenterRight = new SeriesTickBoxValuePresenter(this);
            _chartPanel._rightYAxis.Children.Add(_seriesTickBoxValuePresenterRight);

            _seriesTickBoxValuePresenterLeft.Background = StrokeColorBrush;
            _seriesTickBoxValuePresenterLeft.Foreground = _chartPanel._chartX.Foreground;
            _seriesTickBoxValuePresenterLeft.FontFamily = new FontFamily(_chartPanel._chartX.FontFace);
            _seriesTickBoxValuePresenterLeft.FontSize = _chartPanel._chartX.FontSize;

            _seriesTickBoxValuePresenterRight.Background = StrokeColorBrush;
            _seriesTickBoxValuePresenterRight.Foreground = _chartPanel._chartX.Foreground;
            _seriesTickBoxValuePresenterRight.FontFamily = new FontFamily(_chartPanel._chartX.FontFace);
            _seriesTickBoxValuePresenterRight.FontSize = _chartPanel._chartX.FontSize;

            //SeriesValueBridge = new SeriesDefValueBridge();
        }

        internal void CheckTickBoxNeedCreated()
        {
            if (!_tickBoxNeedsCreated)
            {
                return;
            }

            _tickBoxNeedsCreated = false;
            TickBox = TickBox;
        }

    }
}
