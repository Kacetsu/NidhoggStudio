﻿using ns.Base;
using ns.Base.Event;
using ns.Base.Plugins;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für ToolNodeControl.xaml
    /// </summary>
    public partial class ToolNodeControl : UserControl, INotifyPropertyChanged {
        private Tool _tool;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void ConfigNodeHandler(object sender, NodeSelectionChangedEventArgs e);
        public event ConfigNodeHandler ConfigNodeHandlerChanged;

        public string DisplayName {
            get { return _tool.Name; }
            protected set {
                OnPropertyChanged("DisplayName");
            }
        }

        public Tool Tool {
            get { return _tool; }
        }

        public ToolNodeControl(Tool tool) {
            InitializeComponent();
            _tool = tool;
            _tool.PropertyChanged += _tool_PropertyChanged;
            DataContext = this;
        }

        private void _tool_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Name")
                DisplayName = _tool.Name;
        }

        private void OnConfigNode(Node node) {
            if (ConfigNodeHandlerChanged != null)
                ConfigNodeHandlerChanged(this, new NodeSelectionChangedEventArgs(node));
        }

        private void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e) {
            OnConfigNode(_tool);
        }
    }
}
