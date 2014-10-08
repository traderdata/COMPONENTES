using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ModulusFE.Interfaces;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    public partial class ChartPanel
    {
        private ChartPanelMoreIndicator _moreSeriesTop;
        private ChartPanelMoreIndicator _moreSeriesBottom;
        private readonly List<WeakReference> _notVisibleLineStudiesAbove = new List<WeakReference>();
        private readonly List<WeakReference> _notVisibleLineStudiesBelow = new List<WeakReference>();

        private void ShowMoreSeriesIndicator()
        {
            _timers.StopTimerWork(TimerLineStudiesNotVisibleChanged);

            EnsureMoreSeriesIndicatorCreated();

            _moreSeriesTop.Visible = _notVisibleLineStudiesAbove.Count > 0;
            _moreSeriesBottom.Visible = _notVisibleLineStudiesBelow.Count > 0;

            if (!_moreSeriesBottom.Visible)
                return;

            if (_moreSeriesBottom.ActualHeight == 0)
                _moreSeriesBottom.UpdateLayout();

            _moreSeriesBottom.Top = _rootCanvas.ActualHeight - _moreSeriesBottom.ActualHeight;
        }

        private void EnsureMoreSeriesIndicatorCreated()
        {
            if (_moreSeriesTop != null)
                return;

            _moreSeriesTop = new ChartPanelMoreIndicator
                               {
                                   Margin = new Thickness(2),
                                   Content = "...",
                                   FontSize = _chartX.FontSize,
                                   Background = new SolidColorBrush(Colors.Yellow),
                                   Foreground = new SolidColorBrush(
#if WPF
                             Colors.Navy
#elif SILVERLIGHT
ColorsEx.Navy
#endif
),
                                   Visible = false,
                                   Position = ChartPanelMoreIndicatorPosition.TopLeft,
                               };
            _moreSeriesTop.Click += MoreSeriesTopOnClick;
            _rootCanvas.Children.Add(_moreSeriesTop);


            _moreSeriesBottom = new ChartPanelMoreIndicator
                                  {
                                      Margin = new Thickness(2, -2, 0, 4),
                                      Content = "...",
                                      FontSize = _chartX.FontSize,
                                      Background = new SolidColorBrush(Colors.Yellow),
                                      Foreground = new SolidColorBrush(
#if WPF
                                Colors.Navy
#elif SILVERLIGHT
ColorsEx.Navy
#endif
),
                                      Visible = false,
                                      Position = ChartPanelMoreIndicatorPosition.BottomLeft,
                                  };
            _moreSeriesBottom.Click += MoreSeriesTopOnClick;
            _rootCanvas.Children.Add(_moreSeriesBottom);

            SetMoreIndicatorTemplate();
        }

        internal void SetMoreIndicatorTemplate()
        {
            if (_moreSeriesTop == null)
                return;

            if (_chartX.ChartPanelMoreIndicatorTemplate != null)
            {
                _moreSeriesTop.SetValue(TemplateProperty, _chartX.ChartPanelMoreIndicatorTemplate);
                _moreSeriesBottom.SetValue(TemplateProperty, _chartX.ChartPanelMoreIndicatorTemplate);
            }
        }

        /// <summary>
        /// This will not show the popup menu with LineStudies, instead will show them all on click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void MoreSeriesTopOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            IncludeLineStudiesInSeriesMinMax = true;
            LineStudies.LineStudy.EnsureVisibleMultiple(this);
            if (AutoResetIncludeLineStudiesMinMax)
                IncludeLineStudiesInSeriesMinMax = false;
        }

        private Popup _morePanelPopup;
        private void MoreSeriesTopOnClickOld(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_morePanelPopup != null && _morePanelPopup.IsOpen)
            {
                _morePanelPopup.IsOpen = false;
                return;
            }

            ChartPanelMoreIndicator indicator = (ChartPanelMoreIndicator)sender;

            var list = indicator.Position == ChartPanelMoreIndicatorPosition.BottomLeft ? _notVisibleLineStudiesBelow : _notVisibleLineStudiesAbove;
            IChartPanelMoreIndicatorPanel panel = MoreIndicatorPanel ?? new ChartPanelMoreIndicatorPanel();
            panel.Init(this, list.Where(_ => _.IsAlive).Select(_ => (LineStudies.LineStudy)_.Target), indicator.Position);

            if (_morePanelPopup == null)
            {
                _morePanelPopup = new Popup
                                    {
#if WPF
                              PopupAnimation = PopupAnimation.Slide,
                              StaysOpen = false,
#endif
                                    };
            }
            _morePanelPopup.Child = panel.ElementToShow;

#if WPF
      _morePanelPopup.PlacementTarget = indicator.Position == ChartPanelMoreIndicatorPosition.BottomLeft
                                          ? _moreSeriesBottom
                                          : _moreSeriesTop;
      _morePanelPopup.Placement = indicator.Position == ChartPanelMoreIndicatorPosition.BottomLeft
                                    ? PlacementMode.Top
                                    : PlacementMode.Bottom;
      _morePanelPopup.IsOpen = true;
#elif SILVERLIGHT
            PopupSL p = new PopupSL(_morePanelPopup);
            p.PlacementTarget = indicator.Position == ChartPanelMoreIndicatorPosition.BottomLeft
                                                ? _moreSeriesBottom
                                                : _moreSeriesTop;
            p.Placement = indicator.Position == ChartPanelMoreIndicatorPosition.BottomLeft
                                          ? PlacementMode.Top
                                          : PlacementMode.Bottom;
            p.IsOpen = true;
#endif


        }

        #region Public Methos related to More Indicator
        ///<summary>
        ///</summary>
        ///<param name="closePopup"></param>
        ///<param name="toDo"></param>
        public void MoreIndicatorPanelItemSelected(bool closePopup, Action toDo)
        {
            if (closePopup && _morePanelPopup != null)
                _morePanelPopup.IsOpen = false;
            if (toDo != null)
                toDo();
        }
        #endregion
    }
}
