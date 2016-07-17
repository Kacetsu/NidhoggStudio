using ns.Base.Plugins.Properties;
using ns.Communication.Client;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for NumberPropertyControl.xaml
    /// </summary>
    public partial class DoublePropertyControl : NumberPropertyControl<DoubleProperty, double> {

        /// <summary>
        /// Initializes a new instance of the <see cref="DoublePropertyControl"/> class.
        /// </summary>
        public DoublePropertyControl() : base() {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoublePropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public DoublePropertyControl(DoubleProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(ContentGrid as Panel, ConnectImage);
            } else {
                if (property != null) {
                    Value = property.Value;
                } else {
                    Base.Log.Trace.WriteLine(string.Format("Wrong property type {0} in {1}!", _property.GetType(), MethodBase.GetCurrentMethod()), TraceEventType.Error);
                }
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the PropertyControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void PropertyControl_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(Value))) {
                try {
                    double newValue = Value;
                    if (newValue > _property.Max) newValue = _property.Max;
                    else if (newValue < _property.Min) newValue = _property.Min;

                    ClientCommunicationManager.ProjectService.ChangePropertyValue(newValue, _property.UID);
                    _property.Value = newValue;
                } catch (FaultException ex) {
                    Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Warning);
                } finally {
                    if (_property != null) {
                        Value = _property.Value;
                    } else {
                        Base.Log.Trace.WriteLine(string.Format("Wrong property type {0} in {1}!", _property.GetType(), MethodBase.GetCurrentMethod()), TraceEventType.Error);
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == ConnectButton) {
                ConnectClicked(ContentGrid as Panel, ConnectImage);
            } else if (sender == PosButton) {
                ChangeValue(1);
            } else if (sender == NegButton) {
                ChangeValue(-1);
            }
        }

        private void ChangeValue(double step) {
            try {
                double currentValue = Value;
                double newValue = currentValue + step;
                if (newValue > _property.Max) newValue = _property.Max;
                else if (newValue < _property.Min) newValue = _property.Min;

                ClientCommunicationManager.ProjectService.ChangePropertyValue(newValue, _property.UID);

                Value = newValue;
                _property.Value = newValue;
            } catch (FaultException ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }
        }

        private void NumberBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key.Equals(Key.Enter)) {
                NumberBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}