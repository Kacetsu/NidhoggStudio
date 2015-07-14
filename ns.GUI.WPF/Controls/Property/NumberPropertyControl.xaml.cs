using ns.Base.Log;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls.Property {
    /// <summary>
    /// Interaction logic for NumberPropertyControl.xaml
    /// </summary>
    public partial class NumberPropertyControl : PropertyControl {
        private ns.Base.Plugins.Properties.Property _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberPropertyControl"/> class.
        /// </summary>
        public NumberPropertyControl() {
            InitializeComponent();
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
            this.NameLabel.Content = property.Name;
            _property = property;

            if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                ConnectClicked(this.ContentGrid as Panel, this.ConnectImage);
            } else {
                if (property is IntegerProperty) {
                    this.NumberBox.Text = ((int)property.Value).ToString();
                } else if (property is DoubleProperty) {
                    this.NumberBox.Text = ((double)property.Value).ToString();
                } else {
                    Trace.WriteLine("Wrong property type " + property.GetType() + " in " + MethodInfo.GetCurrentMethod() + "!", LogCategory.Error);
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
                    if (_property is IntegerProperty) {
                        int currentValue = Convert.ToInt32(this.NumberBox.Text);
                        int newValue = currentValue + step;
                        if(newValue > (int)((IntegerProperty)_property).Max)
                            newValue = (int)((IntegerProperty)_property).Max;
                        else if (newValue < (int)((IntegerProperty)_property).Min)
                            newValue = (int)((IntegerProperty)_property).Min;
                        this.NumberBox.Text = newValue.ToString();
                        _property.Value = newValue;
                        result = true;
                    } else if(_property is DoubleProperty) {
                        double currentValue = Convert.ToDouble(this.NumberBox.Text);
                        double newValue = currentValue + (double)step;
                        if (newValue > (double)((DoubleProperty)_property).Max)
                            newValue = (double)((DoubleProperty)_property).Max;
                        else if (newValue < (double)((DoubleProperty)_property).Min)
                            newValue = (double)((DoubleProperty)_property).Min;
                        this.NumberBox.Text = newValue.ToString();
                        _property.Value = newValue;
                        result = true;
                    }
                }
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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
            if (string.IsNullOrEmpty(this.NumberBox.Text)) return;
            if (this.NumberBox.Text == "-") return;
            if (this.NumberBox.Text.EndsWith(",")) return;
            try {
                if (_property != null) {
                    if (_property is IntegerProperty) {
                        int value = Convert.ToInt32(this.NumberBox.Text);
                        if (value > (int)((IntegerProperty)_property).Max)
                            value = (int)((IntegerProperty)_property).Max;
                        else if (value < (int)((IntegerProperty)_property).Min)
                            value = (int)((IntegerProperty)_property).Min;
                        _property.Value = value;
                    } else if (_property is DoubleProperty) {
                        double value = Convert.ToDouble(this.NumberBox.Text);
                        if (value > (double)((DoubleProperty)_property).Max)
                            value = (double)((DoubleProperty)_property).Max;
                        else if (value < (double)((DoubleProperty)_property).Min)
                            value = (double)((DoubleProperty)_property).Min;
                        _property.Value = value;
                    }
                }
            }catch(Exception ex){
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Warning);
            } finally {
                if (_property is IntegerProperty) {
                    this.NumberBox.Text = ((int)_property.Value).ToString();
                } else if (_property is DoubleProperty) {
                    this.NumberBox.Text = ((double)_property.Value).ToString();
                }
            }
        }
    }
}
