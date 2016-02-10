using ns.Base.Log;
using ns.Base.Plugins;
using ns.Core;
using ns.Core.Manager;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF {
    /// <summary>
    /// Interaktionslogik für Editor.xaml
    /// </summary>
    public partial class Editor2 : UserControl {

        private GuiManager _guiManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Editor"/> class.
        /// </summary>
        public Editor2() {
            try {
                InitializeComponent();
                this.Loaded += EditorLoaded;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }
        }

        private void EditorLoaded(object sender, RoutedEventArgs e) {
            _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;

            if (_guiManager == null) {
                _guiManager = new GuiManager();
                _guiManager.Initialize();
                CoreSystem.Managers.Add(_guiManager);
            }

            CoreSystem.Processor.Started += ProcessorStarted;
            CoreSystem.Processor.Stopped += ProcessorStopped;
        }

        private void ProcessorStarted() {
            this.StopButton.IsEnabled = true;
            this.StartButton.IsEnabled = false;
        }

        private void ProcessorStopped() {
            this.StopButton.IsEnabled = false;
            this.StartButton.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;

            if (button == this.StartButton)
                CoreSystem.Processor.Start();
            else if (button == this.StopButton)
                CoreSystem.Processor.Stop();
            else if (button == this.PauseButton)
                CoreSystem.Processor.Pause();

        }

        private void AnalyticsTabControl_Loaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            ExtensionManager extensionManager = CoreSystem.Managers.Find(m => m.Name.Contains("ExtensionManager")) as ExtensionManager;

            foreach (Plugin plugin in extensionManager.Plugins) {
                if (plugin is UIExtension) {
                    UIExtension extension = plugin as UIExtension;

                    switch (extension.Position) {
                        case UIExtensionPosition.Bottom:
                            AddExtensionToAnalyticsTabControl(extension);
                            break;
                        case UIExtensionPosition.Top:
                        default:
                            break;
                    }
                }
            }
        }

        private void AddExtensionToAnalyticsTabControl(UIExtension extension) {
            string displayName = extension.DisplayName;
            TabItem tabItem = new TabItem();
            tabItem.Header = displayName;

            if(extension.Control is UserControl)
                tabItem.Content = extension.Control as UserControl;

            this.AnalyticsTabControl.Items.Add(tabItem);
        }
    }
}
