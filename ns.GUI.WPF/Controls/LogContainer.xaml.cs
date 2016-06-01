﻿using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für LogContainer.xaml
    /// </summary>
    public partial class LogContainer : UserControl {
        private string _timestamp;
        private string _message;
        private TraceEventType _category;
        private SolidColorBrush _logBrush;
        private SolidColorBrush _errorBrush = new SolidColorBrush(Color.FromRgb(200, 0, 0));
        private SolidColorBrush _warningBrush = new SolidColorBrush(Color.FromRgb(184, 89, 50));
        private SolidColorBrush _infoBrush = new SolidColorBrush(Color.FromRgb(0, 122, 204));
        private SolidColorBrush _debugBrush = new SolidColorBrush(Color.FromRgb(150, 170, 57));

        public string Timestamp {
            get { return _timestamp; }
        }

        public string Message {
            get { return _message; }
        }

        public SolidColorBrush LogBrush {
            get { return _logBrush; }
        }

        public SolidColorBrush ErrorBrush {
            get { return _errorBrush; }
        }

        public SolidColorBrush WarningBrush {
            get { return _warningBrush; }
        }

        public SolidColorBrush InfoBrush {
            get { return _infoBrush; }
        }

        public LogContainer(string timestamp, string message, TraceEventType category) {
            InitializeComponent();
            _timestamp = timestamp;
            _message = message;
            _category = category;

            switch (category) {
                case TraceEventType.Error:
                _logBrush = _errorBrush;
                break;

                case TraceEventType.Warning:
                _logBrush = _warningBrush;
                break;

                case TraceEventType.Information:
                _logBrush = _infoBrush;
                break;

                case TraceEventType.Verbose:
                _logBrush = _debugBrush;
                break;

                default:
                break;
            }

            DataContext = this;
        }
    }
}