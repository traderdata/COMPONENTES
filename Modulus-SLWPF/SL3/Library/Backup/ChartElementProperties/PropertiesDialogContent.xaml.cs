using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE.ChartElementProperties
{
    ///<summary>
    ///</summary>
    public partial class PropertiesDialogContent
    {
        internal PropertiesDialog ParentDialog;
        internal string Title;
        internal List<IChartElementProperty> Properties;

        ///<summary>
        ///</summary>
        public PropertiesDialogContent()
        {
            InitializeComponent();

            btnOK.Click += BtnOkOnClick;
            btnCancel.Click += (sender, args) => ParentDialog.Close();
            Loaded += OnLoaded;
        }

        private void BtnOkOnClick(object sender, RoutedEventArgs args)
        {
            StringBuilder sbErrors = new StringBuilder();
            foreach (IChartElementProperty property in Properties)
            {
                property.Validate(sbErrors);
            }

            if (sbErrors.Length > 0)
            {
                MessageBox.Show(sbErrors.ToString());
                return;
            }

            //validation ok, invoke property changed
            foreach (IChartElementProperty property in Properties)
            {
                property.InvokeSetChatElementPropertyValue();
            }

            ParentDialog.OkClose();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Loaded -= OnLoaded;

            LayoutRoot.Background = ParentDialog.Background;

            txtProps.Text = Title;

            int row = 0;
            foreach (IChartElementProperty property in Properties)
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
                Grid.SetColumn(content, 1);
                Grid.SetRow(content, row);

                content.Margin = new Thickness(1);

                gridProps.Children.Add(txt);
                gridProps.Children.Add(content);

                row++;
            }
            Height = row * 23 + 21 + 28;
        }
    }

    internal class PropertiesDialog : ChildWindow
    {
        public event EventHandler OnOK = delegate { };

        public PropertiesDialog(string title, IEnumerable<IChartElementProperty> properties)
        {
            Content = new PropertiesDialogContent
                         {
                             ParentDialog = this,
                             Title = title,
                             Properties = new List<IChartElementProperty>(properties)
                         };
            Title = title;

            Background = new SolidColorBrush(Colors.White);

            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            if (DialogResult == true)
                OnOK(this, EventArgs.Empty);
        }

        internal void OkClose()
        {
            DialogResult = true;
        }
    }
}
