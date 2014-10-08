using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModulusFE.ChartElementProperties;
using ModulusFE.Indicators;

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public partial class ChartPanelTitleCaptionEntry : Control
    {
        private UIElement _root;
        private bool _isMouseOver;

#if WPF
    static ChartPanelTitleCaptionEntry()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof (ChartPanelTitleCaptionEntry),
                                               new FrameworkPropertyMetadata(typeof (ChartPanelTitleCaptionEntry)));
    }
#endif

        ///<summary>
        ///</summary>
        public ChartPanelTitleCaptionEntry()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartPanelTitleCaptionEntry);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _root = (UIElement)GetTemplateChild("PART_Root");

            if (_root == null)
            {
                throw new NullReferenceException("Root part must exists.");
            }

            _root.MouseLeftButtonUp += RootOnMouseLeftButtonUp;
            _root.MouseEnter += (sender, args) =>
                                  {
                                      _isMouseOver = true;
                                      GoToState(true);
                                  };
            _root.MouseLeave += (sender, args) =>
                                  {
                                      _isMouseOver = false;
                                      GoToState(true);
                                  };
            GoToState(false);
        }

        private void GoToState(bool useTransitions)
        {
            var title = DataContext as SeriesTitleLabel;

            if (title == null || title.ShowFrame == Visibility.Collapsed)
            {
                return;
            }

            //  Go to states in NormalStates state group
            VisualStateManager.GoToState(this, _isMouseOver ? "MouseOver" : "Normal", useTransitions);
        }


        private void RootOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            Series series = ((SeriesTitleLabel)DataContext).Series;
            
            //if (!(series is IChartElementPropertyAble)) return;

            if (series is Indicator)
            {
                Indicator ind = (Indicator)series;

                //07-06-2012 - Comentado por Felipe, inveertido pelo bloco abaixo
                //ind.ShowParametersDialog();

                ind.FireEditIndicator(ind);


                //fim da alteração

                return;
            }
            else
            {
                series.FireEditSerie(series);

                return;
            }

            IChartElementPropertyAble propertyAble = (IChartElementPropertyAble)series;
            List<IChartElementProperty> properties = new List<IChartElementProperty>(propertyAble.Properties);
            PropertiesDialog dialog = new PropertiesDialog(propertyAble.Title, properties)
                                        {
#if WPF
                                    Owner = series._chartPanel._chartX.OwnerWindow,
#endif
                                            Background = series._chartPanel._chartX.LineStudyPropertyDialogBackground
                                        };

#if SILVERLIGHT
            dialog.Show();
#endif
#if WPF
      dialog.ShowDialog();
#endif
        }
    }
}
