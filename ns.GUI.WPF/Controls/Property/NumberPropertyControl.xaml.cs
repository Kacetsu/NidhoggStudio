using ns.Base.Log;
using ns.Base.Plugins.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for NumberPropertyControl.xaml
    /// </summary>
    public partial class NumberPropertyControl : PropertyControl {
        private ns.Base.Plugins.Properties.Property _property;
        private string _stringValue = string.Empty;

        public string StringValue {
            get { return _stringValue; }
            set {
                if (!_stringValue.Equals(value)) {
                    _stringValue = value;
                    OnPropertyChanged("StringValue");
                }
            }
        }

        public string DisplayName {
            get { return _property.Name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberPropertyControl"/> class.
        /// </summary>
        public NumberPropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberPropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public NumberPropertyControl(ns.Base.Plugins.Properties.Property property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            DataContext = property;
            _property = property;
            DataContext = this;

            if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                ConnectClicked(this.ContentGrid as Panel, this.ConnectImage);
            } else {
                if (property is IntegerProperty) {
                    StringValue = ((int)property.Value).ToString();
                } else if (property is DoubleProperty) {
                    StringValue = ((double)property.Value).ToString();
                } else {
                    Base.Log.Trace.WriteLine("Wrong property type " + property.GetType() + " in " + MethodBase.GetCurrentMethod() + "!", TraceEventType.Error);
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
                if (_property != null) {
                    IntegerProperty integerProperty = _property as IntegerProperty;
                    DoubleProperty doubleProperty = _property as DoubleProperty;
                    if (integerProperty != null) {
                        int currentValue = Convert.ToInt32(StringValue);
                        int newValue = currentValue + step;
                        if (newValue > integerProperty.Max) newValue = integerProperty.Max;
                        else if (newValue < integerProperty.Min) newValue = integerProperty.Min;

                        StringValue = newValue.ToString();
                        integerProperty.Value = newValue;
                        result = true;
                    } else if (doubleProperty != null) {
                        double currentValue = Convert.ToDouble(StringValue);
                        double newValue = currentValue + step;
                        if (newValue > doubleProperty.Max) newValue = doubleProperty.Max;
                        else if (newValue < doubleProperty.Min) newValue = doubleProperty.Min;

                        StringValue = newValue.ToString();
                        doubleProperty.Value = newValue;
                        result = true;
                    }
                }
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
            if (sender == this.ConnectButton) {
                this.ConnectClicked(this.ContentGrid as Panel, this.ConnectImage);
            } else if (sender == this.PosButton) {
                ChangeValue(1);
            } else if (sender == this.NegButton) {
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
                if (_property != null) {
                    IntegerProperty integerProperty = _property as IntegerProperty;
                    DoubleProperty doubleProperty = _property as DoubleProperty;

                    if (integerProperty != null) {
                        int newValue = Convert.ToInt32(StringValue);
                        if (newValue > integerProperty.Max) newValue = integerProperty.Max;
                        else if (newValue < integerProperty.Min) newValue = integerProperty.Min;

                        integerProperty.Value = newValue;
                    } else if (doubleProperty != null) {
                        double newValue = Convert.ToDouble(StringValue);
                        if (newValue > doubleProperty.Max) newValue = doubleProperty.Max;
                        else if (newValue < doubleProperty.Min) newValue = doubleProperty.Min;

                        doubleProperty.Value = newValue;
                    }
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Warning);
            } finally {
                if (_property is IntegerProperty) {
                    StringValue = ((int)_property.Value).ToString();
                } else if (_property is DoubleProperty) {
                    StringValue = ((double)_property.Value).ToString();
                }
            }
        }
    }
}