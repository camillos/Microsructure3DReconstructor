﻿#pragma checksum "..\..\MatchingControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "64E117459CB112CDEF2F3D95D977F4FF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using System.Windows.Shell;


namespace MetalSoft {
    
    
    /// <summary>
    /// MatchingControl
    /// </summary>
    public partial class MatchingControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\MatchingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border bFrame;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\MatchingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image iPattern;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\MatchingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image iMatched;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MetalSoft;component/matchingcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MatchingControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\MatchingControl.xaml"
            ((MetalSoft.MatchingControl)(target)).MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.onMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 8 "..\..\MatchingControl.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.onMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.bFrame = ((System.Windows.Controls.Border)(target));
            
            #line 9 "..\..\MatchingControl.xaml"
            this.bFrame.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.onMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 13 "..\..\MatchingControl.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.onMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 5:
            this.iPattern = ((System.Windows.Controls.Image)(target));
            
            #line 21 "..\..\MatchingControl.xaml"
            this.iPattern.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.onMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 23 "..\..\MatchingControl.xaml"
            ((System.Windows.Controls.Separator)(target)).MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.onMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 7:
            this.iMatched = ((System.Windows.Controls.Image)(target));
            
            #line 25 "..\..\MatchingControl.xaml"
            this.iMatched.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.onMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

