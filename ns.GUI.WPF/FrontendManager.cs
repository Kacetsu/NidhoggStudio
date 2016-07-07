using ns.Base.Manager;
using ns.Communication.Client;
using ns.Communication.Models;
using ns.GUI.WPF.Events;
using System;
using System.Threading;
using System.Windows;

namespace ns.GUI.WPF {

    public class FrontendManager : BaseManager {
        private static DataStorageConsumer _dataStorageConsumer;
        private static bool _isRunning = false;
        private static Lazy<FrontendManager> _lazyInstance = new Lazy<FrontendManager>(() => new FrontendManager());
        private static IPluginModel _selectedModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontendManager"/> class.
        /// </summary>
        public FrontendManager() : base() {
            _dataStorageConsumer = new DataStorageConsumer();
        }

        public delegate void ConfigNodeHandler(object sender, NodeSelectionChangedEventArgs<object> e);

        /// <summary>
        /// Occurs when [configuration node handler changed].
        /// </summary>
        public event ConfigNodeHandler ConfigNodeHandlerChanged;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static FrontendManager Instance { get { return _lazyInstance.Value; } }

        /// <summary>
        /// Used to simplify GUI binding usage.
        /// </summary>
        public static bool IsNotRunning {
            get { return !_isRunning; }
            set {
                _isRunning = !value;
                Instance.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the Processor is running.
        /// </summary>
        public static bool IsRunning {
            get { return _isRunning; }
            set {
                _isRunning = value;
                Instance.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>
        /// The selected node.
        /// </value>
        public static IPluginModel SelectedModel {
            get { return _selectedModel; }
            set {
                if (_selectedModel != null && _selectedModel != value) {
                    ISelectableModel selectable = (_selectedModel as ISelectableModel);
                    if (selectable != null) {
                        selectable.IsSelected = false;
                    }
                } else {
                    _selectedModel = value;
                    ISelectableModel selectable = (_selectedModel as ISelectableModel);
                    if (selectable != null) {
                        selectable.IsSelected = true;
                    }

                    Instance.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Called when [node configuration clicked].
        /// </summary>
        /// <param name="control">The control.</param>
        public static void OnNodeConfigurationClicked(INodeControl control) {
            Instance.ConfigNodeHandlerChanged?.Invoke(control, new NodeSelectionChangedEventArgs<object>(control.Model));
        }

        /// <summary>
        /// Sets the language dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public static void SetLanguageDictionary() {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString()) {
                case "de-DE":
                dict.Source = new Uri("/ns.GUI.WPF;component/Languages/de_DE.xaml", UriKind.Relative);
                break;

                default:
                dict.Source = new Uri("/ns.GUI.WPF;component/Languages/en_US.xaml", UriKind.Relative);
                break;
            }
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void ProcessorStarted() {
            IsRunning = true;
            IsNotRunning = !IsRunning;
        }

        private void ProcessorStopped() {
            IsRunning = false;
            IsNotRunning = !IsRunning;
        }
    }
}