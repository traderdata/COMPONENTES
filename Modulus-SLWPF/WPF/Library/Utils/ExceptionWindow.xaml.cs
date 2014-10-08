using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModulusFE
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {
        ///<summary>
        ///</summary>
        public ExceptionWindow(Exception exception)
        {
            InitializeComponent();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(exception.Message);
            sb.AppendLine(exception.ToString());
            sb.AppendLine(exception.StackTrace);

            Exception innerException = exception.InnerException;
            int exceptionIndex = 0;
            while (innerException != null && exceptionIndex < 10)
            {
                sb.AppendLine("");
                sb.Append("---- Inner Exception ").Append(exceptionIndex++).AppendLine(" -----");
                sb.AppendLine(innerException.Message);
                sb.AppendLine(innerException.StackTrace);
                innerException = innerException.InnerException;
            }

            textBoxError.Text = sb.ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(textBoxError.Text);
        }
    }
}
