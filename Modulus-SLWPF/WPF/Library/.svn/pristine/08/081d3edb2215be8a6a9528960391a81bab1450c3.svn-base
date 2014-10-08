using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ModulusFE.ChartElementProperties
{
    /// <summary>
    /// Interaction logic for PropertiesDialog.xaml
    /// </summary>
    public partial class PropertiesDialog
    {
        internal List<IChartElementProperty> _properties;

        ///<summary>
        ///</summary>
        public event EventHandler OnOK = delegate { };

        ///<summary>
        ///</summary>
        ///<param name="title"></param>
        ///<param name="properties"></param>
        public PropertiesDialog(string title, IEnumerable<IChartElementProperty> properties)
        {
            InitializeComponent();

            Title = title;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _properties = new List<IChartElementProperty>(properties);

            OnLoaded();
        }

        private void OnLoaded()
        {
            int row = 0;
            foreach (IChartElementProperty property in _properties)
            {
                gridProps.RowDefinitions.Add(new RowDefinition());

                TextBlock txt = new TextBlock
                                  {
                                      Text = property.Title,
                                      VerticalAlignment = VerticalAlignment.Center,
                                      Margin = new Thickness(1)
                                  };
                Grid.SetRow(txt, row);

                FrameworkElement content = property.ValuePresenter.Control;
                content.Margin = new Thickness(1);
                Grid.SetColumn(content, 1);
                Grid.SetRow(content, row);

                gridProps.Children.Add(txt);
                gridProps.Children.Add(content);

                row++;
            }
            Height = row * 22 + 43 + 28;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sbErrors = new StringBuilder();
            foreach (IChartElementProperty property in _properties)
            {
                property.Validate(sbErrors);
            }

            if (sbErrors.Length > 0)
            {
                MessageBox.Show(sbErrors.ToString());
                return;
            }

            //validation ok, invoke property changed
            foreach (IChartElementProperty property in _properties)
            {
                property.InvokeSetChatElementPropertyValue();
            }


            if (OnOK != null)
                OnOK(this, EventArgs.Empty);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
