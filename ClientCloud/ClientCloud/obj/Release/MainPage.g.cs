﻿#pragma checksum "..\..\MainPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6F6225BF39E997FB3A5A7473D20D0794E537E1C9"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using ClientCloud;
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace ClientCloud {
    
    
    /// <summary>
    /// MainPage
    /// </summary>
    public partial class MainPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock welcome;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listFiles;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button downloadButton;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button uploadButton;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button refreshButton;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button deleteButton;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button createFolderButton;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.BusyIndicator loading;
        
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
            System.Uri resourceLocater = new System.Uri("/ClientCloud;component/mainpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainPage.xaml"
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
            
            #line 10 "..\..\MainPage.xaml"
            ((ClientCloud.MainPage)(target)).MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.listFilesMouseRightButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.welcome = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.listFiles = ((System.Windows.Controls.ListBox)(target));
            
            #line 15 "..\..\MainPage.xaml"
            this.listFiles.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.listFilesMouseRightButtonDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.downloadButton = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\MainPage.xaml"
            this.downloadButton.Click += new System.Windows.RoutedEventHandler(this.DownloadClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.uploadButton = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\MainPage.xaml"
            this.uploadButton.Click += new System.Windows.RoutedEventHandler(this.UploadClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.refreshButton = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\MainPage.xaml"
            this.refreshButton.Click += new System.Windows.RoutedEventHandler(this.RefreshClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.deleteButton = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\MainPage.xaml"
            this.deleteButton.Click += new System.Windows.RoutedEventHandler(this.DeleteFile);
            
            #line default
            #line hidden
            return;
            case 8:
            this.createFolderButton = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\MainPage.xaml"
            this.createFolderButton.Click += new System.Windows.RoutedEventHandler(this.createFolderButtonClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.loading = ((Xceed.Wpf.Toolkit.BusyIndicator)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

