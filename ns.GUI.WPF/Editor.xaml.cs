﻿using ns.Base.Plugins;
using ns.Communication.Client;
using ns.Communication.Models;
using ns.GUI.WPF.Events;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ns.GUI.WPF {

    /// <summary>
    /// Interaktionslogik für Editor.xaml
    /// </summary>
    public partial class Editor : UserControl, INotifyPropertyChanged {
        private Controls.AddToolControl _addToolControl;
        private FrontendManager _guiManager;
        private string _lockedToolName = string.Empty;
        private Controls.ProjectExplorer _projectExplorer;
        private Controls.PropertyEditor _propertyEditor;

        public Editor() {
            InitializeComponent();
            DataContext = this;
            //LoopExecutionToggleButton.DataContext = CoreSystem.Processor;
            HeaderGrid.Height = 0;
            Loaded += Editor_Loaded;
            _projectExplorer = ProjectExplorer;
            //_projectExplorer.ConfigNodeHandlerChanged += ProjectExplorer_ConfigNodeHandlerChanged;
            ProjectExplorer.AddToolButton.Click += ProjectExplorer_AddToolButton_Click;
            FrontendManager.Instance.ConfigNodeHandlerChanged += ProjectExplorer_ConfigNodeHandlerChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string LockedToolName {
            get { return _lockedToolName; }
            set {
                if (!_lockedToolName.Equals(value)) {
                    _lockedToolName = value;
                    OnPropertyChanged();
                }
            }
        }

        private void Callback_ProcessorStateChanged(object sender, Communication.Events.ProcessorStateEventArgs e) {
            LoopExecutionToggleButton.IsChecked = e.ProcessorInfoModel.State == Base.ProcessorState.Running;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            if (_addToolControl != null && sender == _addToolControl.CloseButton) {
                RemoveControl(_addToolControl);
                _addToolControl = null;
            } else if (_propertyEditor != null && sender == _propertyEditor.CloseButton) {
                RemoveControl(_propertyEditor);
                _propertyEditor = null;
            }
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e) {
            GuiHelper.DoubleAnimateControl(300, ControlGrid, WidthProperty);
            GuiHelper.DoubleAnimateControl(60, HeaderGrid, HeightProperty, TimeSpan.FromSeconds(0.3));

            try {
                ClientCommunicationManager.ProcessorService.Callback.ProcessorStateChanged += Callback_ProcessorStateChanged;
                ProcessorInfoModel processorInfoModel = ClientCommunicationManager.ProcessorService.GetState();
                LoopExecutionToggleButton.IsChecked = processorInfoModel.State == Base.ProcessorState.Running;
            } catch (Exception) {
                throw;
            }

            //_guiManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(FrontendManager))) as FrontendManager;
            //if (_guiManager != null)
            //    _guiManager.SelectedItemChanged += _guiManager_SelectedItemChanged;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //private void _guiManager_SelectedItemChanged(object sender, Base.Event.NodeSelectionChangedEventArgs e) {
        //    if (e.SelectedNode is Tool) {
        //        LockedToolName = (e.SelectedNode as Tool).DisplayName;
        //    } else if (e.SelectedNode is Operation) {
        //        LockedToolName = (e.SelectedNode as Operation).DisplayName;
        //    }
        //}

        private void ProjectExplorer_AddToolButton_Click(object sender, RoutedEventArgs e) {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                if (_addToolControl == null) {
                    _addToolControl = new Controls.AddToolControl();
                    _addToolControl.CloseButton.Click += CloseButton_Click;
                }
                ControlGrid.Children.Add(_addToolControl);
                GuiHelper.DoubleAnimateControl(300, ControlGrid, WidthProperty, TimeSpan.FromSeconds(0.2));
            };
            ControlGrid.BeginAnimation(WidthProperty, animation);
        }

        private void ProjectExplorer_ConfigNodeHandlerChanged(object sender, NodeSelectionChangedEventArgs<object> e) {
            IPluginModel model = e.SelectedNode as IPluginModel;
            if (model == null) return;

            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                if (_propertyEditor == null) {
                    _propertyEditor = new Controls.PropertyEditor(model);
                    _propertyEditor.CloseButton.Click += CloseButton_Click;
                    _propertyEditor.RemoveToolButton.YesButton.Click += YesButton_Click;
                }
                if (!ControlGrid.Children.Contains(_propertyEditor))
                    ControlGrid.Children.Add(_propertyEditor);
                GuiHelper.DoubleAnimateControl(300, ControlGrid, WidthProperty, TimeSpan.FromSeconds(0.2));
            };
            ControlGrid.BeginAnimation(WidthProperty, animation);
        }

        private void RemoveControl(UIElement control) {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                ControlGrid.Children.Remove(control);
                if (!ControlGrid.Children.Contains(_projectExplorer))
                    ControlGrid.Children.Add(_projectExplorer);
                GuiHelper.DoubleAnimateControl(300, ControlGrid, WidthProperty, TimeSpan.FromSeconds(0.2));
            };
            ControlGrid.BeginAnimation(WidthProperty, animation);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            if (sender == ResultsViewToggleButton) {
                GuiHelper.DoubleAnimateControl(200, ResultsView, HeightProperty);
            } else if (sender == HistogramViewToggleButton) {
                GuiHelper.DoubleAnimateControl(200, HistogramView, HeightProperty);
            } else if (sender == LoopExecutionToggleButton) {
                ClientCommunicationManager.ProcessorService.Start();
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            if (sender == ResultsViewToggleButton) {
                GuiHelper.DoubleAnimateControl(0, ResultsView, HeightProperty);
            } else if (sender == HistogramViewToggleButton) {
                GuiHelper.DoubleAnimateControl(0, HistogramView, HeightProperty);
            } else if (sender == LoopExecutionToggleButton) {
                ClientCommunicationManager.ProcessorService.Stop();
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e) {
            if (_propertyEditor != null && sender == _propertyEditor.RemoveToolButton.YesButton) {
                Tool toolToRemove = _propertyEditor.Model as Tool;
                Operation operationToRemove = _propertyEditor.Model as Operation;

                //if (_projectManager == null)
                //    _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;

                //RemoveControl(_propertyEditor);
                //_propertyEditor = null;
                //if (toolToRemove != null) _projectManager.Remove(toolToRemove);
                //else if (operationToRemove != null) _projectManager.Remove(operationToRemove);
            }
        }
    }
}