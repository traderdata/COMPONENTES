﻿#pragma checksum "..\..\..\Controls\ColorDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A73F8A694A9217EBC5445DA025397DCD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ModulusFE.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ModulusFE.Controls {
    
    
    /// <summary>
    /// ColorDialog
    /// </summary>
    public partial class ColorDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\Controls\ColorDialog.xaml"
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Controls\ColorDialog.xaml"
        internal System.Windows.Controls.Canvas canvasOldColor;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Controls\ColorDialog.xaml"
        internal System.Windows.Controls.Canvas canvasnewColor;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Controls\ColorDialog.xaml"
        internal ModulusFE.Controls.ColorPicker colorPicker;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Controls\ColorDialog.xaml"
        internal System.Windows.Controls.Button btnOK;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Controls\ColorDialog.xaml"
        internal System.Windows.Controls.Button btnCancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ModulusFE.StockChartX;component/controls/colordialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\ColorDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.canvasOldColor = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.canvasnewColor = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.colorPicker = ((ModulusFE.Controls.ColorPicker)(target));
            return;
            case 5:
            this.btnOK = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\Controls\ColorDialog.xaml"
            this.btnOK.Click += new System.Windows.RoutedEventHandler(this.btnOK_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\Controls\ColorDialog.xaml"
            this.btnCancel.Click += new System.Windows.RoutedEventHandler(this.btnCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
