using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;
using Line = System.Windows.Shapes.Line;
#if SILVERLIGHT
using ModulusFE.SL;
using ModulusFE.SL.Utils;
#endif


namespace ModulusFE
{
    public partial class PanelsContainer : Canvas
    {
        private enum StateEnum
        {
            Normal,
            ResizingPanel,
            MovingPanel,
            MightZoom,
            Zooming,
            Dragging,
        }

        private readonly List<ChartPanel> _panels = new List<ChartPanel>();
        private readonly Storyboard _maximizationStoryboard;
        private readonly Storyboard _minimizationStoryBoard;
        private readonly Storyboard _restoreMinimizedStoryboard;

        private bool _layoutChanged;
        //used when a panel is maximized and the window changes its size, after restoring back the panel must resize all panels

        private StateEnum _state = StateEnum.Normal;
        private MoveSeriesIndicator _moveSeriesIndicator;
        private double _zoomStartX;
        //    private double _dragStartX;
        private ChartPanelsDivider _panelsDivider;
        private ChartPanel _panelToResize;

        //internal readonly Canvas _panelsHolder; //this canvas will be resized automatically, then on it will place the panels
        internal InfoPanel _infoPanel;
        internal ChartPanel _maximizedPanel;
        internal StockChartX _chartX;
        internal Line _verticalCrossHair;
        internal Line _horizontalCrossHair;

        /// <summary>
        /// List used to remember panels that need to be repainted. It is used in case
        /// when PriceStyle != psStandard, in this case panel with OHLC series must first be calculated
        /// this will ensure correct values in xMap that will be used to paint the rest of chart
        /// </summary>
        internal List<ChartPanel> _panelsToBeRepainted = new List<ChartPanel>();

        internal LineStudyContextMenu _lineStudyContextMenu;

        ///<summary>
        ///</summary>
        public PanelsContainer()
        {
            Resources.Add("max_animation", _maximizationStoryboard = new Storyboard());
            Resources.Add("min_animation", _minimizationStoryBoard = new Storyboard());
            Resources.Add("restoremin_animation", _restoreMinimizedStoryboard = new Storyboard());

            //      _panelsHolder = new Canvas
            //                        {
            //#if WPF
            //                          ClipToBounds = true
            //#else
            //Clip = new RectangleGeometry() 
            //#endif
            //                          VerticalAlignment = VerticalAlignment.Stretch,
            //                          HorizontalAlignment = HorizontalAlignment.Stretch,
            //                        };
            //      Children.Add(_panelsHolder);
            //      _panelsHolder.Background = Background;
            HookPanelsHolderMouseEvents(true);

            _lineStudyContextMenu = new LineStudyContextMenu { Visibility = Visibility.Collapsed };
            Children.Add(_lineStudyContextMenu);

            SizeChanged += OnSizeChanged;
#if SILVERLIGHT
            Mouse.RegisterMouseMoveAbleElement(this);
            MouseMove += (sender, e) => Mouse.UpdateMousePosition(this, e.GetPosition(this));
#endif
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_panelsHolder.Arrange(new Rect(new Point(0, 0), e.NewSize));

            _size = e.NewSize;
            ResizePanels(ResizeType.Proportional);
        }

        internal Point ToPanelsHolder(MouseButtonEventArgs eventArgs)
        {
            //return eventArgs.GetPosition(_panelsHolder);
            return eventArgs.GetPosition(this);
        }

        internal Point ToPanelsHolder(MouseEventArgs eventArgs)
        {
            //return eventArgs.GetPosition(_panelsHolder);
            return eventArgs.GetPosition(this);
        }

        internal ChartPanel AddPanel(ChartPanel.PositionType position)
        {
            return AddPanel(position, false);
        }

        internal ChartPanel AddPanel(ChartPanel.PositionType position, bool heatMap)
        {
            //find the right position in the vector of panels
            int iIndexToInsertAt;
            //
            //
            if (!CanAddNewPanel(position, out iIndexToInsertAt))
                return null;

            ChartPanel panel = !heatMap
                                                     ? new ChartPanel(_chartX, position) { _panelsContainer = this }
                                                     : new ChartPanel_HeatMap(_chartX) { _panelsContainer = this };

            panel.Visible = false;
            panel.ScalingType = _chartX.ScalingType;

            _panels.Insert(iIndexToInsertAt, panel);
            //_panelsHolder.Children.Add(panel);
            Children.Add(panel);
#if SILVERLIGHT
            panel.ApplyTemplate();
#endif

            if (_maximizedPanel == null)
            {
                panel.State = ChartPanel.StateType.New;
            }
            else
            {
                panel.State = ChartPanel.StateType.Minimized;
                // when a panel is maximized and we add a new panel, assign 50% of height to it
                panel._normalHeightPct = 0.1;
                _chartX.AddMinimizedPanel(panel);
            }

            // update panels internal index
            int iIndex = 0;
            _panels.ForEach(p => { p._index = iIndex++; });

            panel.OnMaximizeClick += (sender, e) => MaxMinPanel((ChartPanel)sender);
            panel.OnMinimizeClick += (sender, e) => MinimizePanel((ChartPanel)sender);
            panel.OnCloseClick += (sender, e) =>
            {
                if (panel.State == ChartPanel.StateType.Maximized)
                {
                    MessageBox.Show(
#if WPF
                        _chartX.OwnerWindow,
#endif
                        "A maximized panel can't be closed.");
                    return;
                }

                ClosePanel((ChartPanel)sender);
            };

            // resize evenly all panels
            ResizePanels(ResizeType.NewPanel, panel);

            if (_chartX.CrossHairs && _verticalCrossHair != null)
            {
                SetZIndex(_verticalCrossHair, ZIndexConstants.CrossHairs);
                SetZIndex(_horizontalCrossHair, ZIndexConstants.CrossHairs);
            }

            return panel;
        }

        private int GetIndexToInsertAt(ChartPanel.PositionType position)
        {
            int iIndexToInsertAt = 0;
            switch (position)
            {
                case ChartPanel.PositionType.AlwaysTop: //find next panel that hasn't AlwayTop value
                    iIndexToInsertAt +=
                        _panels.TakeWhile(chartPanel => chartPanel._position == ChartPanel.PositionType.AlwaysTop).Count();
                    break;
                case ChartPanel.PositionType.AlwaysBottom: //insert at the bottom
                    iIndexToInsertAt = _panels.Count;
                    break;
                default: //find first AlwaysBottom and insert before it
                    iIndexToInsertAt +=
                        _panels.TakeWhile(chartPanel => chartPanel._position != ChartPanel.PositionType.AlwaysBottom).Count();
                    break;
            }

            return iIndexToInsertAt;
        }

        internal int VisiblePanelsCount
        {
            get { return _panels.Count(_ => _.Visible); }
        }

        internal int PrevVisiblePanelIndex(int currentIndex)
        {
            int iIndex = currentIndex - 1;
            while (iIndex > -1 && !_panels[iIndex].Visible && _panels[iIndex] != _maximizedPanel)
            {
                iIndex--;
            }

            return iIndex >= 0 ? iIndex : -1;
        }

        internal ChartPanel PrevVisiblePanel(ChartPanel current)
        {
            return PrevVisiblePanel(current._index);
        }

        internal ChartPanel PrevVisiblePanel(int index)
        {
            int i = PrevVisiblePanelIndex(index);
            return i != -1 ? _panels[i] : null;
        }

        internal int NextVisiblePanelIndex(int currentIndex)
        {
            int iIndex = currentIndex + 1;
            while (iIndex < _panels.Count && !_panels[iIndex].Visible && _panels[iIndex] != _maximizedPanel)
            {
                iIndex++;
            }

            return iIndex < _panels.Count ? iIndex : -1;
        }

        internal ChartPanel NextVisiblePanel(ChartPanel current)
        {
            return NextVisiblePanel(current._index);
        }

        internal ChartPanel NextVisiblePanel(int index)
        {
            int i = NextVisiblePanelIndex(index);
            return i == -1 ? null : _panels[i];
        }

        internal ChartPanel PanelByY(double Y)
        {
            return _maximizedPanel != null
                             ? null
                             : _panels.FirstOrDefault(panel => panel.Visible && Utils.Between(Y, panel.Top, panel.Top + panel.ActualHeight));
        }

        internal void RecyclePanels()
        {
            for (int i = 0; i < _panels.Count; i++)
            {
                if (_panels[i].SeriesCount != 0 || (_panels[i] is ChartPanel_HeatMap))
                {
                    continue;
                }

                ClosePanel(_panels[i]);
                i--;
            }
        }

        internal void PostResetPanels()
        {
            ResetHeatMapPanels();

            if (!Utils.GetIsInDesignMode(this))
                RecyclePanels();
        }

        private int _panelsToBePaintedCount;

        /// <summary>
        /// repaints and deletes empty panels
        /// </summary>
        internal void ResetPanels()
        {
            //paint first regular panels. this will make sure all indicators are calculated, 
            //cause later the heat map panel will us them
            _panelsToBePaintedCount = 0;
            _panels.Where(_ => !_.IsHeatMap)
                .ToList()
                .ForEach(p =>
                {
                    _panelsToBePaintedCount++;
                    p._afterPaintAction = () =>
                    {
                        _panelsToBePaintedCount--;
                        if (_panelsToBePaintedCount == 0)
                            PostResetPanels();
                    };
                    p.Paint();
                });
        }

        internal void ResetHeatMapPanels()
        {
            _panels.ForEach(p =>
            {
                if (!p.IsHeatMap)
                    return;

                p.Paint();
            });
        }

        internal enum ResizeType
        {
            Even,
            Proportional,

            /// <summary>
            /// when minimizing the panel give its height to all other visible panels
            /// </summary>
            PanelMinimized,

            /// <summary>
            /// when adding a new panel it will take the half of above or below panel
            /// </summary>
            NewPanel,

            /// <summary>
            /// used when inserting back a minimized panel. it is used its saved percentage of height
            /// </summary>
            InsertExisting,

            /// <summary>
            /// mostly used after rearranging panels. just reposition, not resize
            /// </summary>
            Reposition,

            /// <summary>
            /// Used to reverse the NewPanel method
            /// </summary>
            DeletePanel
        }

        private Size _size;

        internal void ResizePanels(ResizeType resizeType, params object[] args)
        {
            ResizePanels(resizeType, false, args);
        }

        private Rect ResizePanels(ResizeType resizeType, bool bOnlyCalculate, params object[] args)
        {
            if (_panels.Count == 0)
            {
                return new Rect();
            }

            Rect rcBounds = new Rect(0, 0, _size.Width, _size.Height);
            double dPanelHeight;
            double dExtraHeight;
            double dTop;

            if (_maximizedPanel != null)
            {
                _maximizedPanel.Bounds = rcBounds;
                _layoutChanged = true;
                return new Rect();
            }

            switch (resizeType)
            {
                case ResizeType.Reposition:
                    dTop = 0;
                    foreach (ChartPanel panel in _panels)
                    {
                        if (panel == _maximizedPanel || panel.State != ChartPanel.StateType.Normal)
                        {
                            continue;
                        }

                        panel.Top = dTop;
                        dTop += panel.Height;
                    }
                    break;
                case ResizeType.Proportional:
                    double dOldPanelsHeight = 0;
                    _panels.ForEach(p =>
                    {
                        if (!p.Visible)
                        {
                            return;
                        }

                        dOldPanelsHeight += p.Bounds.Height;
                    });

                    double dMultiplier = dOldPanelsHeight == 0 ? 1 : rcBounds.Height / dOldPanelsHeight;
                    _panels.ForEach(p =>
                    {
                        if (p == _maximizedPanel || p.State != ChartPanel.StateType.Normal)
                        {
                            return;
                        }

                        Rect rcPanelBounds = p.Bounds;
                        Rect rcPanel = new Rect(rcBounds.Left, rcPanelBounds.Top * dMultiplier,
                            rcBounds.Width, rcPanelBounds.Height * dMultiplier);
                        p.Bounds = rcPanel;
                    });
                    break;
                case ResizeType.Even:
                    dPanelHeight = rcBounds.Height / _panels.Count;
                    double dPanelTop = rcBounds.Top;
                    _panels.ForEach(p =>
                    {
                        if (!p.Visible)
                        {
                            return;
                        }

                        Rect rcPanel = new Rect(rcBounds.Left, dPanelTop, rcBounds.Width, dPanelHeight);
                        p.Bounds = rcPanel;
                        dPanelTop += dPanelHeight;
                    });
                    break;
                case ResizeType.PanelMinimized:
                    dPanelHeight = (double)args[0];
                    dExtraHeight = dPanelHeight / VisiblePanelsCount;
                    dTop = 0;
                    foreach (ChartPanel panel in _panels)
                    {
                        if (!panel.Visible)
                        {
                            continue;
                        }

                        //_panelsHolder.BringToFront(panel);
                        this.BringToFront(panel);
                        Rect rcPanelBounds = panel.Bounds;
                        panel.Bounds = new Rect(rcPanelBounds.Left, dTop, rcPanelBounds.Width,
                                                                        rcPanelBounds.Height + dExtraHeight);
                        dTop += (rcPanelBounds.Height + dExtraHeight);
                    }
                    break;
                case ResizeType.InsertExisting:
                    Debug.Assert(args.Length > 0);
                    ChartPanel panelToRestore = (ChartPanel)args[0];
                    double dPanelHeightPct = panelToRestore._normalHeightPct; //it is in percents 0.xx
                    if (dPanelHeightPct >= 0.99 && VisiblePanelsCount > 1)
                        dPanelHeightPct /= 2;

                    //take from all panels the part of height needed by the restored panel
                    //its state must be changes to Normal and it must be removed from minimized list of panels
                    double dPanelsRemainingPct = 1 - dPanelHeightPct;
                    dTop = 0;
                    foreach (ChartPanel panel in _panels)
                    {
                        if (!panel.Visible && !bOnlyCalculate)
                        {
                            continue;
                        }

                        double dPanelNewHeight;
                        if (panel._index != panelToRestore._index)
                        {
                            double dPanelCurPct = panel.Height / rcBounds.Height;
                            double dPanelNewPct = dPanelCurPct * dPanelsRemainingPct;
                            dPanelNewHeight = rcBounds.Height * dPanelNewPct;
                        }
                        else
                        {
                            dPanelNewHeight = rcBounds.Height * dPanelHeightPct;
                        }

                        if (!bOnlyCalculate)
                        {
                            panel.Top = dTop;
                            panel.Height = dPanelNewHeight;
                            panel.Left = rcBounds.Left;
                            panel.Width = rcBounds.Width;
                        }
                        else
                        {
                            if (panel._index == panelToRestore._index)
                            {
                                return new Rect(rcBounds.Left, dTop, rcBounds.Width, dPanelNewHeight);
                            }
                        }
                        dTop += dPanelNewHeight;
                    }
                    break;
                case ResizeType.NewPanel:
                    Debug.Assert(args.Length > 0);
                    ChartPanel newPanel = (ChartPanel)args[0];
                    ChartPanel abovePanel = PrevVisiblePanel(newPanel);
                    if (abovePanel == null) //no panels
                    {
                        if (VisiblePanelsCount == 0)
                        {
                            newPanel.Bounds = new Rect(rcBounds.Left, 0, rcBounds.Width, rcBounds.Height);
                        }
                        else
                        {
                            ChartPanel nextPanel = NextVisiblePanel(newPanel);
                            double dNewHeight = nextPanel.Height / 2;
                            nextPanel.Top += dNewHeight;
                            nextPanel.Height -= dNewHeight;
                            newPanel.Bounds = new Rect(rcBounds.Left, 0, rcBounds.Width, dNewHeight);
                        }
                    }
                    else
                    {
                        double dNewHeight = abovePanel.Height / 2;
                        abovePanel.Height = dNewHeight;
                        newPanel.Bounds = new Rect(rcBounds.Left, abovePanel.Top + dNewHeight, rcBounds.Width, dNewHeight);
                    }

                    newPanel.Visible = true;
                    newPanel.State = ChartPanel.StateType.Normal;
                    break;
                case ResizeType.DeletePanel:
                    dPanelHeight = (double)args[0];
                    int panelIndex = (int)args[1];
                    ChartPanel prev_Panel = PrevVisiblePanel(panelIndex);
                    ChartPanel next_Panel = NextVisiblePanel(panelIndex - 1);
                    if (prev_Panel != null)
                    {
                        Rect rcPanelBounds = prev_Panel.Bounds;
                        prev_Panel.Bounds = new Rect(
                            rcPanelBounds.Left,
                            rcPanelBounds.Top,
                            rcPanelBounds.Width,
                            rcPanelBounds.Height + dPanelHeight);
                    }
                    else if (next_Panel != null)
                    {
                        Rect rcPanelBounds = next_Panel.Bounds;
                        next_Panel.Bounds = new Rect(
                            rcPanelBounds.Left,
                            rcPanelBounds.Top - dPanelHeight,
                            rcPanelBounds.Width,
                            rcPanelBounds.Height + dPanelHeight);
                    }
                    break;
            }
            return new Rect();
        }

        private bool CanAddNewPanel(ChartPanel.PositionType position, out int at)
        {
            at = GetIndexToInsertAt(position);

            int aboveIndex = PrevVisiblePanelIndex(at);
            if (aboveIndex == -1) //no panels
            {
                return true;
            }

            if (_panels[aboveIndex].ActualHeight == 0)
            {
                return true;
            }

            return _panels[aboveIndex].ActualHeight / 2 >= PanelAllowedMinimumHeight;
        }

        #region Ovverides

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="availableSize"></param>
        //    /// <returns></returns>
        //    protected override Size MeasureOverride(Size availableSize)
        //    {
        //      return base.MeasureOverride(availableSize);
        //
        //Size availableSize = new Size(constraint.Width, double.PositiveInfinity);
        //      Size childSize = availableSize;
        //
        //      _panelsHolder.Measure(childSize);
        //return availableSize;
        //      return _panelsHolder.DesiredSize;
        //    }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="arrangeSize"></param>
        //    /// <returns></returns>
        //    protected override Size ArrangeOverride(Size arrangeSize)
        //    {
        //      return base.ArrangeOverride(arrangeSize);
        //
        //      _panelsHolder.Arrange(new Rect(new Point(0, 0), arrangeSize));
        //
        //      _size = arrangeSize;
        //      ResizePanels(ResizeType.Proportional);
        //
        //      return arrangeSize;
        //    }

        #endregion

        #region Maximize/Restore/Minimize panels

        /// <summary>
        /// maximize and restores a panel
        /// </summary>
        /// <param name="panel"></param>
        internal void MaxMinPanel(ChartPanel panel)
        {
            if (_panels.Count - _chartX._panelsBar.PanelCount <= 1)
            {
                //throw new InvalidOperationException("Last visible panel can't be maximized.");
                MessageBox.Show(
#if WPF
                    _chartX.OwnerWindow,
#endif
                    "Last visible panel can't be maximized.", "Error", MessageBoxButton.OK
#if WPF
                    , MessageBoxImage.Error
#endif
                    );
                return;
            }

            Rect rcBounds = new Rect(new Point(), _size);
            //current panel's bounds
            Rect rcPanelBounds = panel.Bounds;
            //remeber the reference
            _maximizedPanel = panel;
            if (panel.State == ChartPanel.StateType.Normal) //maximizing
            {
                panel.State = ChartPanel.StateType.Maximized;
                //remember panel's position and size
                panel._normalTopPosition = rcPanelBounds.Top;
                panel._normalHeight = rcPanelBounds.Height;

                if (_chartX.ShowAnimations)
                {
                    CreateAnimationObject(_maximizationStoryboard, MaximizationCompletedEvent);
                    CreateMinMaxRect();
                    InitAnimation(_maximizationStoryboard, rcPanelBounds, rcBounds);
                    EnsureMinMaxRectVisible();

                    Storyboard.SetTarget(_maximizationStoryboard, _rcMinMax);
                    _maximizationStoryboard.Begin();
                }
                else
                {
                    MaximizePanel();
                }
            }
            else
            {
                panel.State = ChartPanel.StateType.Normal;
                Rect rcNormalSizePanel = new Rect(rcBounds.Left, panel._normalTopPosition, rcBounds.Width, panel._normalHeight);
                //minimizing
                if (_chartX.ShowAnimations)
                {
                    InitAnimation(_maximizationStoryboard, rcBounds, rcNormalSizePanel);
                    EnsureMinMaxRectVisible();

                    //_rcMinMax.BeginStoryboard(_maximizationStoryboard);
                    Storyboard.SetTarget(_maximizationStoryboard, _rcMinMax);
                    _maximizationStoryboard.Begin();
                }
                else
                {
                    RestoreMaximizedPanel();
                }
            }
        }

        private void MaximizePanel()
        {
            Rect rcBounds = new Rect(new Point(), _size);

            _maximizedPanel.Bounds = rcBounds;
            //hide all panels except maximized
            _panels.ForEach(p => { if (p != _maximizedPanel) p.Visible = false; });
        }

        private void RestoreMaximizedPanel()
        {
            Rect rcBounds = new Rect(new Point(), _size);
            Rect rcNormalSizePanel = new Rect(rcBounds.Left, _maximizedPanel._normalTopPosition, rcBounds.Width,
                                                                                _maximizedPanel._normalHeight);
            //show again all panels non-minimized panels
            _panels.ForEach(p => p.Visible = true);

            _maximizedPanel.Bounds = rcNormalSizePanel;
            _maximizedPanel = null;
            if (_layoutChanged)
                ResizePanels(ResizeType.Proportional);
            _layoutChanged = false;
        }

        private ChartPanel _minimizedPanel;

        internal void MinimizePanel(ChartPanel panel)
        {
            if (VisiblePanelsCount == 1)
            {
                //throw new ChartException("Last visible panel can't be minimized.");
                MessageBox.Show(
#if WPF
                    _chartX.OwnerWindow,
#endif
                    "Last visible panel can't be minimized.", "Error", MessageBoxButton.OK
#if WPF
                    , MessageBoxImage.Error
#endif
                    );
                return;
            }
            _minimizedPanel = panel;
            //panel._normalHeightPct = panel.ActualHeight / _panelsHolder.ActualHeight;
            panel._normalHeightPct = panel.ActualHeight / ActualHeight;
            panel._normalHeight = panel.ActualHeight;
            if (_chartX.ShowAnimations)
            {
                CreateMinMaxRect();
                CreateAnimationObject(_minimizationStoryBoard, MinimizationCompletedEvent);

                panel._minimizedRect = _chartX._panelsBar.GetNextRectToMinimize;
                panel._minimizedRect.Y += ActualHeight;
                InitAnimation(_minimizationStoryBoard, panel.Bounds, panel._minimizedRect);
                EnsureMinMaxRectVisible();

                Storyboard.SetTarget(_minimizationStoryBoard, _rcMinMax);
                _minimizationStoryBoard.Begin();
                //_minimizationStoryBoard.Begin(_rcMinMax);
            }
            else
            {
                DoMinimizePanel(panel);
            }
        }

        private void DoMinimizePanel(ChartPanel chartPanel)
        {
            chartPanel.Visible = false;
            chartPanel.State = ChartPanel.StateType.Minimized;
            _chartX.AddMinimizedPanel(chartPanel);
            ResizePanels(ResizeType.PanelMinimized, chartPanel._normalHeight);
            HideMinMaxRect();
        }

        private void MinimizationCompletedEvent(object sender, EventArgs e)
        {
            DoMinimizePanel(_minimizedPanel);
            _minimizedPanel = null;
        }

        internal void RestorePanel(ChartPanel chartPanel)
        {
            if (_maximizedPanel != null)
            {
                //throw new InvalidOperationException("Can't restore a minimized panel while there is a maximized panel.");
                MessageBox.Show(
#if WPF
                    _chartX.OwnerWindow,
#endif
                    "Can't restore a minimized panel while there is a maximized panel.",
                    "Error",
                    MessageBoxButton.OK
#if WPF
                    , MessageBoxImage.Error
#endif
                    );
                return;
            }
            if (_chartX.ShowAnimations)
            {
                CreateMinMaxRect();
                CreateAnimationObject(_restoreMinimizedStoryboard, RestoreMinimizedCompletedEvent);

                Rect rcWhere = ResizePanels(ResizeType.InsertExisting, true, chartPanel);
                InitAnimation(_restoreMinimizedStoryboard, chartPanel._minimizedRect, rcWhere);
                EnsureMinMaxRectVisible();

                _minimizedPanel = chartPanel;

                Storyboard.SetTarget(_restoreMinimizedStoryboard, _rcMinMax);
                _restoreMinimizedStoryboard.Begin();
                //_restoreMinimizedStoryboard.Begin(_rcMinMax);
            }
            else
            {
                DoRestorePanel(chartPanel);
            }
        }

        private void RestoreMinimizedCompletedEvent(object sender, EventArgs args)
        {
            DoRestorePanel(_minimizedPanel);
        }

        private void DoRestorePanel(ChartPanel chartPanel)
        {
            chartPanel.State = ChartPanel.StateType.Normal;
            chartPanel.Visible = true;
            ResizePanels(ResizeType.InsertExisting, chartPanel);
            _chartX.DeleteMinimizedPanel(chartPanel);
            HideMinMaxRect();
        }

        private void ClosePanel(ChartPanel panel)
        {
            /*if (VisiblePanelsCount == 1)
            {
                MessageBox.Show("Last visible panel can't be closed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } */
            if ((panel.SeriesCount > 0 || panel.IsHeatMap) && _chartX.FireChartPanelBeforeClose(panel)) return;
            if (!panel.CanBeDeleted)
            {
                MessageBox.Show(
#if WPF
                    _chartX.OwnerWindow,
#endif
                    panel.ReasonCantBeDeleted, "Error", MessageBoxButton.OK
#if WPF
                    , MessageBoxImage.Error
#endif
                    );
                return;
            }

            DestroyChartPanel(panel);
        }

        private void DestroyChartPanel(ChartPanel panel)
        {
            double dExtraHeight = panel.Height;
            int panelIndex = panel.Index;
            _panels.Remove(panel);
            //_panelsHolder.Children.Remove(panel);
            Children.Remove(panel);
            panel.UnRegisterSeriesFromDataManager();
            ResizePanels(ResizeType.DeletePanel, dExtraHeight, panelIndex);

            //reindex panels
            int iIndex = 0;
            _panels.ForEach(p => p._index = iIndex++);

            _chartX.ReCalc = true; //cause panel was removed we must recalculate the chart. 
        }

        internal void CloseHeatMap()
        {
            ChartPanel heatMap = _panels.FirstOrDefault(p => p.IsHeatMap);
            if (heatMap == null)
                return;

            DestroyChartPanel(heatMap);
        }

        internal void ClearAll()
        {
            foreach (var panel in _panels)
            {
                panel.ClearAll();
                //_panelsHolder.Children.Remove(panel);
                Children.Remove(panel);
            }

            _panels.Clear();
            _panelToMove = null;
            _panelToResize = null;
            _maximizedPanel = null;
        }

        #endregion

        internal List<ChartPanel> Panels
        {
            get { return _panels; }
        }

        #region Panel Resizing

        private void HookPanelsHolderMouseEvents(bool bHook)
        {
            if (bHook)
            {
                MouseMove += PanelsHolder_OnMouseMove;
#if WPF
                //_panelsHolder.MouseLeave += (sender, e) => Mouse.OverrideCursor = null;
                MouseLeave += (sender, e) => Mouse.OverrideCursor = null;
#endif
                MouseLeftButtonUp += PanelsHolder_OnMouseUp;
                MouseLeftButtonDown += PanelsHolder_OnMouseDown;
                KeyDown += PanelsHolder_OnKeyDown;
            }
            else
            {
                MouseMove -= PanelsHolder_OnMouseMove;
                MouseLeftButtonUp -= PanelsHolder_OnMouseUp;
                MouseLeftButtonDown -= PanelsHolder_OnMouseDown;
                KeyDown -= PanelsHolder_OnKeyDown;
            }
        }

        private bool CanResizePanel(double X, double Y)
        {
            if (_chartX.Status != StockChartX.ChartStatus.Ready) return false;

            if (_panelToResize != null)
                _panelToResize.Cursor = null;

            _panelToResize = null;
            //if (VisiblePanelsCount > 1 && X < _panelsHolder.ActualWidth - 100)
            if (VisiblePanelsCount > 1 && X < ActualWidth - 100)
            {
                foreach (ChartPanel panel in _panels)
                {
                    if (!panel.Visible || panel._index <= 0 || !Utils.Between(Y, panel.Top, panel.Top + 2)) continue;
                    _panelToResize = panel;
                    break;
                }
            }
#if WPF
            //Mouse.OverrideCursor = _panelToResize != null ? Cursors.SizeNS : null;
#endif
            //#if SILVERLIGHT
            if (_panelToResize != null)
                _panelToResize.Cursor = Cursors.SizeNS;
            //#endif

            return _panelToResize != null;
        }

        private void EnsurePanelsDividerVisible()
        {
            if (_panelsDivider == null)
            {
                _panelsDivider = new ChartPanelsDivider();
                //_panelsHolder.Children.Add(_panelsDivider);
                Children.Add(_panelsDivider);
                _panelsDivider.ApplyTemplate();
            }
            _panelsDivider.Visible = true;
            //_panelsHolder.BringToFront(_panelsDivider);
            this.BringToFront(_panelsDivider);
            //_panelsDivider.Width = _panelsHolder.ActualWidth;
            _panelsDivider.Width = ActualWidth;
        }


        internal void StartResizePanel(double Y)
        {
            _panelToResize.State = ChartPanel.StateType.Resizing;
            //_panelsHolder.CaptureMouse();
            CaptureMouse();
            EnsurePanelsDividerVisible();
            _panelsDivider.Y = Y;
        }

        internal void DoResizePanel(double Y)
        {
            if (Y < _panelToResize.Top) //rezise up
            {
                ChartPanel abovePanel = PrevVisiblePanel(_panelToResize);
                Debug.Assert(abovePanel != null);
                _panelsDivider.IsOK = Y > (abovePanel.Top + abovePanel.TitleBarHeight);
            }
            else if (Y > (_panelToResize.Top + _panelToResize.TitleBarHeight))
            {
                _panelsDivider.IsOK = Y <= (_panelToResize.Top + _panelToResize.ActualHeight);
            }

            _panelsDivider.Y = Y;
        }

        internal void CancelResizePanel()
        {
            //_panelsHolder.ReleaseMouseCapture();
            ReleaseMouseCapture();
            _panelToResize = null;
            _panelsDivider.Visible = false;
        }

        internal void EndResizePanel(double Y)
        {
            //resize the current panel, and the above one
            //_panelsHolder.ReleaseMouseCapture();
            ReleaseMouseCapture();
            _panelsDivider.Visible = false;

            if (Y <= 0)
                Y = 10;
            ResizePanel(_panelToResize, Y);

            _panelToResize.State = ChartPanel.StateType.Normal;
            _panelToResize = null;
        }

        public double PanelAllowedMinimumHeight = Constants.PanelTitleBarHeight + 10;

        internal void ResizePanel(ChartPanel panelToResize, double Y)
        {
            ChartPanel abovePanel = PrevVisiblePanel(panelToResize);
            Debug.Assert(abovePanel != null);

            if (Y <= abovePanel.Top)
                Y = abovePanel.Top + 10;

            //Debug.Assert(Y > abovePanel.Top);

            double newHeight = panelToResize.Height + (panelToResize.Top - Y);


            if (newHeight >= PanelAllowedMinimumHeight)
            {
                abovePanel.Height = Y - abovePanel.Top;

                panelToResize.Height += (panelToResize.Top - Y);
                panelToResize.Top = Y;
            }
            else
            {
                abovePanel.Height += panelToResize.Height - PanelAllowedMinimumHeight;

                panelToResize.Top = (panelToResize.Top + panelToResize.Height - PanelAllowedMinimumHeight);
                panelToResize.Height = PanelAllowedMinimumHeight;
            }
        }

        internal void ResizePanelByHeight(ChartPanel panelToResize, double newHeight)
        {
            ChartPanel belowPanel = NextVisiblePanel(panelToResize);
            ChartPanel abovePanel = PrevVisiblePanel(panelToResize);

            if (belowPanel == null && abovePanel == null) return; //not possible to resize one panel
            if (newHeight < PanelAllowedMinimumHeight) return; //new size too small

            double heightDiff = panelToResize.Height - newHeight;

            if (belowPanel == null) //the lowest one, usually the panel with volume
            {
                panelToResize.Height = newHeight;
                panelToResize.Top += heightDiff;
                abovePanel.Height += heightDiff;
                return;
            }

            double belowPanelNewSize = belowPanel.Height + heightDiff;
            if (belowPanelNewSize < PanelAllowedMinimumHeight)
            {
                double availableMinHeight = belowPanel.Height - PanelAllowedMinimumHeight;
                heightDiff += availableMinHeight;
                newHeight -= availableMinHeight;
            }

            panelToResize.Height = newHeight;
            belowPanel.Top -= heightDiff;
            belowPanel.Height += heightDiff;
        }

        #endregion

        #region Panels Moving

        private ChartPanel _panelToMove;
        private ChartPanel _panelToMoveOver;
        private ChartPanelMoveShadow _chartPanelMoveShadow;
        private ChartPanelMovePlaceholder _chartPanelMovePlaceholder;
        private double _oldY;

        /// <summary>
        /// A panel can be moved only along the panels with same style
        /// </summary>
        /// <returns></returns>
        private bool CanMovePanel(double X, double Y)
        {
            if (_chartX.Status != StockChartX.ChartStatus.Ready) return false;

            if (_panelToMove != null)
                _panelToMove.Cursor = null;

            _panelToMove = null;
            //if (VisiblePanelsCount > 1 && X < _panelsHolder.ActualWidth - 100)
            if (VisiblePanelsCount > 1 && X < ActualWidth - 100)
            {
                foreach (ChartPanel panel in _panels)
                {
                    if (!panel.Visible) continue;
                    if (!Utils.Between(Y, panel.Top, panel.Top + panel.TitleBarHeight)) continue;
                    _panelToMove = panel;
                    break;
                }
            }
            if (_panelToMove != null)
                _panelToMove.Cursor = Cursors.Hand;

            //Mouse.OverrideCursor = _panelToMove != null ? Cursors.Hand : null;
            return _panelToMove != null;
        }

        private void EnsureChartPanelMoveShadowVisible()
        {
            if (_chartPanelMoveShadow == null)
            {
                _chartPanelMoveShadow = new ChartPanelMoveShadow();
                //_panelsHolder.Children.Add(_chartPanelMoveShadow);
                Children.Add(_chartPanelMoveShadow);
            }
            _chartPanelMoveShadow.Visible = true;
            _chartPanelMoveShadow.InitFromPanel(_panelToMove);
            //_panelsHolder.BringToFront(_chartPanelMoveShadow);
            this.BringToFront(_chartPanelMoveShadow);
        }

        private void EnsureChartPanelMovePlaceholderVisible()
        {
            if (_chartPanelMovePlaceholder != null) return;
            _chartPanelMovePlaceholder = new ChartPanelMovePlaceholder();
            //_panelsHolder.Children.Add(_chartPanelMovePlaceholder);
            Children.Add(_chartPanelMovePlaceholder);
        }

        private void StartMovingPanel(double Y)
        {
            if (_chartX.Status != StockChartX.ChartStatus.Ready) return;

            _panelToMove.State = ChartPanel.StateType.Moving;
            //_panelsHolder.CaptureMouse();
            CaptureMouse();
            EnsureChartPanelMoveShadowVisible();
            EnsureChartPanelMovePlaceholderVisible();
            _chartPanelMovePlaceholder.Visible = false;
            _oldY = Y;
        }

        private void TryToMovePanel(double Y)
        {
            if (_chartX.Status != StockChartX.ChartStatus.Ready) return;
            _chartPanelMovePlaceholder.Visible = false;
            _panelToMoveOver = null;
            ChartPanel chartPanelFromY = PanelByY(Y);
            _chartPanelMoveShadow.Top += (Y - _oldY);
            _oldY = Y;

            //find if we can move the panel
            //same panel
            if (!(_chartPanelMoveShadow.IsOkToMove = (chartPanelFromY != _panelToMove) && (chartPanelFromY != null))) return;

            //different panel, check position style
            if (!(_chartPanelMoveShadow.IsOkToMove = chartPanelFromY._position == _panelToMove._position)) return;

            _panelToMoveOver = chartPanelFromY;
            //_panelsHolder.BringToFront(_chartPanelMovePlaceholder);
            this.BringToFront(_chartPanelMovePlaceholder);
            _chartPanelMovePlaceholder.Visible = true;
            _chartPanelMovePlaceholder.ShowOnPanel(_panelToMoveOver);
        }

        private void EndMovePanel()
        {
            _panelToMove.State = ChartPanel.StateType.Normal;
            _chartPanelMovePlaceholder.Visible = false;
            _chartPanelMoveShadow.Visible = false;

            if (_chartPanelMoveShadow.IsOkToMove)
            {
                _panels[_panelToMoveOver._index] = _panelToMove;
                _panels[_panelToMove._index] = _panelToMoveOver;
                int iIndex = 0;
                _panels.ForEach(p => p._index = iIndex++);
                ResizePanels(ResizeType.Reposition);
            }

            //_panelsHolder.ReleaseMouseCapture();
            ReleaseMouseCapture();
            _panelToMove = null;
            _panelToMoveOver = null;
        }

        private void CancelMovePanel()
        {
            _chartPanelMovePlaceholder.Visible = false;
            _panelToMove = null;
            //_panelsHolder.ReleaseMouseCapture();
            ReleaseMouseCapture();
            _chartPanelMoveShadow.Visible = false;
        }

        #endregion

        #region Mouse Event for PanelsHolder

        private void PanelsHolder_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_chartX.Status != StockChartX.ChartStatus.Ready)
            {
                return;
            }

            //Point p = e.GetPosition(_panelsHolder);
            Point p = e.GetPosition(this);
            switch (_state)
            {
                case StateEnum.Normal:

                    _chartX.CrossHairsPosition = p;

                    if (CanResizePanel(p.X, p.Y))
                    {
                        break;
                    }

                    if (_chartX.CtrlDown && CanMovePanel(p.X, p.Y))
                    {
                        break;
                    }
                    break;
                case StateEnum.ResizingPanel:
                    DoResizePanel(p.Y);
                    break;
                case StateEnum.MovingPanel:
                    TryToMovePanel(p.Y);
                    break;
                case StateEnum.MightZoom:
                case StateEnum.Zooming:
                    DoZoom(p);
                    break;
                case StateEnum.Dragging:
                    //          DoDrag(p);
                    break;
            }
        }

        private void PanelsHolder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_chartX.Status != StockChartX.ChartStatus.Ready)
                return;

            switch (_state)
            {
                case StateEnum.Normal:
                    if (_panelToResize != null)
                    {
                        //StartResizePanel(e.GetPosition(_panelsHolder).Y);
                        StartResizePanel(e.GetPosition(this).Y);
                        _state = StateEnum.ResizingPanel;
                        break;
                    }

                    if (_panelToMove != null)
                    {
                        //StartMovingPanel(e.GetPosition(_panelsHolder).Y);
                        StartMovingPanel(e.GetPosition(this).Y);
                        _state = StateEnum.MovingPanel;
                        break;
                    }

                    if (!_chartX.IndicatorsCollection.Any(_ => _.Selected) && _chartX.LineStudySelectedCount == 0
#if WPF
                        && e.ClickCount == 1
#endif
)
                        TryStartZoom(e.GetPosition(this));
                    break;
            }
        }

        private void PanelsHolder_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_chartX.Status != StockChartX.ChartStatus.Ready) return;

            switch (_state)
            {
                case StateEnum.ResizingPanel:
                    //EndResizePanel(e.GetPosition(_panelsHolder).Y);
                    EndResizePanel(e.GetPosition(this).Y);
                    _state = StateEnum.Normal;
                    break;
                case StateEnum.MovingPanel:
                    _state = StateEnum.Normal;
                    EndMovePanel();
                    break;
                case StateEnum.MightZoom:
                    _state = StateEnum.Normal;
                    break;
                case StateEnum.Zooming:
                    StopZoom();
                    break;
            }
        }

        private void PanelsHolder_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (_state)
            {
                case StateEnum.ResizingPanel:
                    if (e.Key == Key.Escape)
                    {
                        CancelResizePanel();
                        _state = StateEnum.Normal;
                    }
                    break;
                case StateEnum.MovingPanel:
                    if (e.Key == Key.Escape)
                    {
                        CancelMovePanel();
                        _state = StateEnum.Normal;
                    }
                    break;
                case StateEnum.Zooming:
                    if (e.Key == Key.Escape)
                    {
                        CancelZoom();
                        _state = StateEnum.Normal;
                    }
                    break;
            }
        }

        //    private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //    {
        //      switch (_state)
        //      {
        //        case StateEnum.Dragging:
        ////          StopDrag();
        //          break;
        //      }
        //    }
        //
        //    private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //    {
        //      e.Handled = true;
        //
        //      switch (_state)
        //      {
        //        case StateEnum.Normal:
        ////          StartDrag(e.GetPosition(this));
        //          break;
        //      }
        //    }

        #endregion

        #region Move Series Indicator

        internal void ShowMoveSeriesIndicator(Point p, MoveSeriesIndicator.MoveStatusEnum moveStatusEnum)
        {
            if (_moveSeriesIndicator == null)
            {
                _moveSeriesIndicator = new MoveSeriesIndicator();
                //_panelsHolder.Children.Add(_moveSeriesIndicator);
                Children.Add(_moveSeriesIndicator);
                _moveSeriesIndicator.ApplyTemplate();
                SetZIndex(_moveSeriesIndicator, ZIndexConstants.MoveSeriesIndicator);
            }

            if (_moveSeriesIndicator.Visibility != Visibility.Visible)
                _moveSeriesIndicator.Visibility = Visibility.Visible;

            //if (p.X + _moveSeriesIndicator.ActualWidth + 10 < _panelsHolder.ActualWidth)
            if (p.X + _moveSeriesIndicator.ActualWidth + 10 < ActualWidth)
                _moveSeriesIndicator.X = p.X + 10;
            else
                _moveSeriesIndicator.X = p.X - _moveSeriesIndicator.ActualWidth;

            //if (p.Y + _moveSeriesIndicator.ActualHeight + 5 < _panelsHolder.ActualHeight)
            if (p.Y + _moveSeriesIndicator.ActualHeight + 5 < ActualHeight)
                _moveSeriesIndicator.Y = p.Y;
            else
                _moveSeriesIndicator.Y = p.Y - _moveSeriesIndicator.ActualHeight + 10;

            _moveSeriesIndicator.MoveStatus = moveStatusEnum;
        }

        internal void HideMoveSeriesIndicator()
        {
            if (_moveSeriesIndicator != null && _moveSeriesIndicator.Visibility == Visibility.Visible)
                _moveSeriesIndicator.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region InfoPanel

        internal void EnsureInfoPanelCreated()
        {
            if (_infoPanel != null)
            {
                return;
            }

            _infoPanel = new InfoPanel(_chartX)
                                         {
                                             PanelsContainer = this
                                         };

            //_panelsHolder.Children.Add(_infoPanel);
            Children.Add(_infoPanel);
            SetZIndex(_infoPanel, ZIndexConstants.InfoPanel);
            _infoPanel.Visible = false;
        }

        private readonly ObjectFromCursor[] _goodParts = new[]
        {
            ObjectFromCursor.PanelLeftNonPaintableArea,
            ObjectFromCursor.PanelRightNonPaintableArea,
            ObjectFromCursor.PanelPaintableArea
        };

        internal void ShowInfoPanelInternal()
        {
            if (_chartX.InfoPanelPosition == InfoPanelPositionEnum.Hidden)
                return;

            EnsureInfoPanelCreated();

            Point pos = Mouse.GetPosition(this);
            bool positionChanged = (pos.X != _infoPanel.X) || (pos.Y != _infoPanel.Y);
            //if (pos.X == _infoPanel.X && pos.Y == _infoPanel.Y) return;

            object o;
            ObjectFromCursor objectFromCursor = _chartX.GetObjectFromCursor(out o);
            if (!_goodParts.Contains(objectFromCursor))
                return;

            _infoPanel.X = pos.X;
            _infoPanel.Y = pos.Y;

            ChartPanel chartPanel = (ChartPanel)o;
            if (chartPanel.Index != _infoPanel.PanelOwnerIndex)
            {
                _infoPanel.Clear();
                _infoPanel.AddInfoPanelItems(_chartX._calendar.InfoPanelItems);
                _infoPanel.AddInfoPanelItems(new ChartPanelInfoPanelAble { ChartPanel = chartPanel }.InfoPanelItems);

                _infoPanel.PanelOwnerIndex = chartPanel.Index;
            }

            if (!_infoPanel.Visible)
                _infoPanel.Visible = true;

            _infoPanel.RecalculateLayout();

            if (positionChanged && _chartX.InfoPanelPosition == InfoPanelPositionEnum.FollowMouse)
                _chartX.FireShowInfoPanel();

            if (_chartX.InfoPanelPosition != InfoPanelPositionEnum.FollowMouse)
                return;

            double offsetX = 10;
            double offsetY = 8;
            Rect chartPanelRect = chartPanel.CanvasRect;
            if (pos.X + offsetX + _infoPanel.Width > chartPanelRect.Right)
                offsetX = (_infoPanel.Width + offsetX) * -1.0;
            if (pos.Y + offsetY + _infoPanel.Height > _size.Height)
                offsetY = (_infoPanel.Height + offsetY) * -1.0;

#if WPF
            pos.Offset(offsetX, offsetY);
#elif SILVERLIGHT
            pos = pos.Offset(offsetX, offsetY);
#endif
            _infoPanel.Position = pos;
        }

        internal void EnforceInfoPanelUpdate()
        {
            if (_infoPanel == null || _chartX.InfoPanelPosition != InfoPanelPositionEnum.FixedPosition)
                return;

            _infoPanel.X = -1;
            ShowInfoPanelInternal();
        }

        internal void ResetInfoPanelContent()
        {
            if (_infoPanel != null)
                _infoPanel.PanelOwnerIndex = -1;
        }

        internal void HideInfoPanel()
        {
            if (_infoPanel != null)
                _infoPanel.Visible = false;
        }

        internal void MakeInfoPanelStatic()
        {
            EnsureInfoPanelCreated();

            Point? p = _infoPanel._position ?? new Point(10, Constants.PanelTitleBarHeight + 10);

            _infoPanel.Position = p.Value;
        }

        #endregion

        #region Cross Hairs

        internal void ShowCrossHairs()
        {
            if (!_chartX.CrossHairs)
            {
                return;
            }

            if (_verticalCrossHair == null)
            {
                _verticalCrossHair = new Line { Stroke = _chartX.CrossHairsStroke, StrokeThickness = 1, IsHitTestVisible = false };
                Children.Add(_verticalCrossHair);

                _horizontalCrossHair = new Line { Stroke = _chartX.CrossHairsStroke, StrokeThickness = 1, IsHitTestVisible = false };
                Children.Add(_horizontalCrossHair);
            }

            if (_verticalCrossHair.Visibility != Visibility.Visible)
            {
                _verticalCrossHair.Visibility = _horizontalCrossHair.Visibility = Visibility.Visible;
                SetZIndex(_verticalCrossHair, ZIndexConstants.CrossHairs);
                SetZIndex(_horizontalCrossHair, ZIndexConstants.CrossHairs);
            }

            _verticalCrossHair.X1 = _verticalCrossHair.X2 = (int)_chartX.CrossHairsPosition.X;
            _verticalCrossHair.Y1 = 0;
            _verticalCrossHair.Y2 = (int)ActualHeight;

            _horizontalCrossHair.Y1 = _horizontalCrossHair.Y2 = (int)_chartX.CrossHairsPosition.Y;
            _horizontalCrossHair.X1 = 0;
            _horizontalCrossHair.X2 = (int)ActualWidth;
        }

        internal void HideCrossHairs()
        {
            if (_verticalCrossHair == null)
            {
                return;
            }

            _verticalCrossHair.Visibility = _horizontalCrossHair.Visibility = Visibility.Collapsed;
        }

        internal void UpdateCrossHairsColor()
        {
            if (_verticalCrossHair != null)
            {
                _verticalCrossHair.Stroke = _chartX.CrossHairsStroke;
            }

            if (_horizontalCrossHair != null)
            {
                _horizontalCrossHair.Stroke = _chartX.CrossHairsStroke;
            }
        }

        #endregion
    }
}
