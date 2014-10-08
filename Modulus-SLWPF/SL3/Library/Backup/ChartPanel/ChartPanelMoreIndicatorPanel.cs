using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ModulusFE.Interfaces;
using ModulusFE.LineStudies;

namespace ModulusFE
{
    ///<summary>
    /// A Default apnel representation for more "sign"
    ///</summary>
    public class ChartPanelMoreIndicatorPanel : ListBox, IChartPanelMoreIndicatorPanel
    {
        private ChartPanel _chartPanel;

        ///<summary>
        /// Ctor
        ///</summary>
        public ChartPanelMoreIndicatorPanel()
        {
            SelectionChanged += OnSelectionChanged;

            ResourceDictionary rd
              = new ResourceDictionary
                  {
                      Source = new Uri(string.Format("/{0};component/Themes/generic.xaml", Utils.GetCurrentAssemblyName()),
                                       UriKind.RelativeOrAbsolute)
                  };

            ItemTemplate = rd["ChartPanelMoreIndicatorPanel_DataTemplate"] as DataTemplate;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            PanelItem item = (PanelItem)SelectedItem;
            Action a =
              () =>
              {
                  if (item == null)
                      return;
                  ((LineStudy)item.LineStudy.Target).EnsureVisible(LineStudy.EnsureVisibilityPosition.Middle);
              };
            _chartPanel.MoreIndicatorPanelItemSelected(true, a);
        }

        #region Implementation of IChartPanelMoreIndicatorPanel

        /// <summary>
        /// Gets the element to be shown
        /// </summary>
        public UIElement ElementToShow
        {
            get { return this; }
        }

        readonly ObservableCollection<PanelItem> _items = new ObservableCollection<PanelItem>();
        /// <summary>
        /// Initializes the <see cref="ChartPanelMoreIndicatorPanel"/>
        /// </summary>
        /// <param name="chartPanel"></param>
        /// <param name="lineStudies"></param>
        /// <param name="position"></param>
        public void Init(ChartPanel chartPanel, IEnumerable<LineStudy> lineStudies, ChartPanelMoreIndicatorPosition position)
        {
            _items.Clear();
            ItemsSource = _items;
            foreach (LineStudy lineStudy in lineStudies)
            {
                _items.Add(new PanelItem
                             {
                                 Name = lineStudy.StudyType.ToString(),
                                 Value = lineStudy.ToString(),
                                 LineStudy = new WeakReference(lineStudy),
                             });
            }

            _chartPanel = chartPanel;
        }
        #endregion

        ///<summary>
        /// An item information
        ///</summary>
        public class PanelItem : DependencyObject
        {
            ///<summary>
            /// Name
            ///</summary>
            public string Name { get; set; }
            ///<summary>
            /// Value
            ///</summary>
            public string Value { get; set; }
            ///<summary>
            /// WeakReference to a <see cref="LineStudy"/>
            ///</summary>
            public WeakReference LineStudy { get; set; }
        }
    }
}
