using ns.Base.Manager;
using ns.Communication.CommunicationModels;
using System;
using System.Threading;
using System.Windows;

namespace ns.GUI.WPF {

    public class FrontendManager : BaseManager {
        private static Lazy<FrontendManager> _lazyInstance = new Lazy<FrontendManager>(() => new FrontendManager());
        private static object _selectedModel;
        private static bool _isRunning = false;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static FrontendManager Instance { get { return _lazyInstance.Value; } }

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
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>
        /// The selected node.
        /// </value>
        public static object SelectedModel {
            get { return _selectedModel; }
            set {
                if (_selectedModel != null && _selectedModel != value) {
                    ISelectable selectable = (_selectedModel as ISelectable);
                    if (selectable != null) {
                        selectable.IsSelected = false;
                    }
                } else {
                    _selectedModel = value;
                    ISelectable selectable = (_selectedModel as ISelectable);
                    if (selectable != null) {
                        selectable.IsSelected = true;
                    }

                    Instance.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontendManager"/> class.
        /// </summary>
        public FrontendManager() : base() {
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

        public override bool Initialize() {
            base.Initialize();

            //CoreSystem.Processor.Started += ProcessorStarted;
            //CoreSystem.Processor.Stopped += ProcessorStopped;

            return true;
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