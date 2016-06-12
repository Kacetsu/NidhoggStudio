using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for NumberPropertyControl.xaml
    /// </summary>
    public partial class IntegerPropertyControl : PropertyControl<IntegerProperty> {
        private string _stringValue = string.Empty;

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <value>
        /// The string value.
        /// </value>
        public string StringValue {
            get { return _stringValue; }
            set {
                if (!_stringValue.Equals(value)) {
                    _stringValue = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerPropertyControl"/> class.
        /// </summary>
        public IntegerPropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerPropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public IntegerPropertyControl(IntegerProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            DataContext = property;
            DataContext = this;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(ContentGrid as Panel, ConnectImage);
            } else {
                if (property != null) {
                    StringValue = property.Value.ToString();
                } else {
                    Base.Log.Trace.WriteLine(string.Format("Wrong property type {0} in {1}!", _property.GetType(), MethodBase.GetCurrentMethod()), TraceEventType.Error);
                }
            }
        }

        /// <summary>
        /// Changes the value.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>Success of the operation.</returns>
        private bool ChangeValue(int step) {
            bool result = false;

            try {
                int currentValue = Convert.ToInt32(StringValue);
                int newValue = currentValue + step;
                if (newValue > _property.Max) newValue = _property.Max;
                else if (newValue < _property.Min) newValue = _property.Min;

                StringValue = newValue.ToString();
                _property.Value = newValue;
                result = true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == ConnectButton) {
                ConnectClicked(ContentGrid as Panel, ConnectImage);
            } else if (sender == PosButton) {
                ChangeValue(1);
            } else if (sender == NegButton) {
                ChangeValue(-1);
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the NumberBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrEmpty(StringValue)) return;
            if (StringValue == "-") return;
            if (StringValue.EndsWith(",")) return;
            try {
                int newValue = Convert.ToInt32(StringValue);
                if (newValue > _property.Max) newValue = _property.Max;
                else if (newValue < _property.Min) newValue = _property.Min;

                _property.Value = newValue;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Warning);
            } finally {
                if (_property != null) {
                    StringValue = _property.Value.ToString();
                } else {
                    Base.Log.Trace.WriteLine(string.Format("Wrong property type {0} in {1}!", _property.GetType(), MethodBase.GetCurrentMethod()), TraceEventType.Error);
                }
            }
        }
    }
}