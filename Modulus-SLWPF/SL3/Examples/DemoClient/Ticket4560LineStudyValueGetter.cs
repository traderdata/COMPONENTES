using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ModulusFE;
using ModulusFE.Interfaces;
using ModulusFE.LineStudies;
using ModulusFE.SL;

namespace TestChart
{
    public class Ticket4560LineStudyValueGetter : HorizontalLineDefStudyValue
    {
        private string _text = "Initial Text...";
        ///<summary>
        /// Value
        ///</summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Text"));
            }
        }
    }
}
