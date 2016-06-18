using ns.Base;
using ns.Base.Event;
using ns.Base.Manager;
using ns.Communication.CommunicationModels;
using System;
using System.Threading;
using System.Windows;

namespace ns.GUI.WPF {

    public class FrontendManager : BaseManager {
        private object _selectedNode;
        private bool _isRunning = false;

        /// <summary>
        /// Gets or sets if the Processor is running.
        /// </summary>
        public bool IsRunning {
            get { return _isRunning; }
            set {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Used to simplify GUI binding usage.
        /// </summary>
        public bool IsNotRunning {
            get { return !_isRunning; }
            set {
                _isRunning = !value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>
        /// The selected node.
        /// </value>
        public object SelectedNode {
            get { return _selectedNode; }
            set {
                if (_selectedNode != null && _selectedNode != value) {
                    ISelectable selectable = (_selectedNode as ISelectable);
                    if (selectable != null) {
                        selectable.IsSelected = false;
                    }
                } else {
                    _selectedNode = value;
                    ISelectable selectable = (_selectedNode as ISelectable);
                    if (selectable != null) {
                        selectable.IsSelected = true;
                    }

                    OnPropertyChanged();
                }
            }
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