using ns.Base.Log;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for RectanglePropertyControl.xaml
    /// </summary>
    public partial class RectanglePropertyControl : PropertyControl {
        private ns.Base.Plugins.Properties.Property _property;

        public string DisplayName {
            get { return _property.Name; }
        }

        public RectanglePropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectanglePropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public RectanglePropertyControl(ns.Base.Plugins.Properties.Property property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            DataContext = this;
            _property = property;
            property.PropertyChanged += Property_PropertyChanged;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(this.ContentGrid as Panel, this.ConnectImage);
            } else {
                if (property is RectangleProperty) {
                    RectangleProperty rectangleProperty = property as RectangleProperty;
                    this.XNumberBox.Text = rectangleProperty.X.ToString();
                    this.YNumberBox.Text = rectangleProperty.Y.ToString();
                    this.WidthNumberBox.Text = rectangleProperty.Width.ToString();
                    this.HeightNumberBox.Text = rectangleProperty.Height.ToString();
                } else {
                    Base.Log.Trace.WriteLine("Wrong property type " + property.GetType() + " in " + MethodBase.GetCurrentMethod() + "!", TraceEventType.Error);
                }
            }
        }

        private void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (sender is RectangleProperty) {
                RectangleProperty property = sender as RectangleProperty;
                this.XNumberBox.Text = property.X.ToString();
                this.YNumberBox.Text = property.Y.ToString();
                this.WidthNumberBox.Text = property.Width.ToString();
                this.HeightNumberBox.Text = property.Height.ToString();
            }
        }

        /// <summary>
        /// Changes the value.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>Success of the operation.</returns>
        private bool ChangeValue(double step, TextBox box) {
            bool result = false;

            try {
                double currentValue = Convert.ToDouble(box.Text);
                double newValue = currentValue + step;
                box.Text = newValue.ToString();
                result = true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        private void ChangeTextValue(TextBox box) {
            if (string.IsNullOrEmpty(box.Text)) return;
            if (box.Text == "-") return;
            if (box.Text.EndsWith(",")) return;
            try {
                if (_property != null) {
                    double value = Convert.ToDouble(box.Text);
                    if (box == XNumberBox)
                        ((RectangleProperty)_property).X = value;
                    else if (box == YNumberBox)
                        ((RectangleProperty)_property).Y = value;
                    else if (box == WidthNumberBox)
                        ((RectangleProperty)_property).Width = value;
                    else if (box == HeightNumberBox)
                        ((RectangleProperty)_property).Height = value;
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Warning);
            } finally {
                if (box == XNumberBox)
                    box.Text = ((RectangleProperty)_property).X.ToString();
                else if (box == YNumberBox)
                    box.Text = ((RectangleProperty)_property).Y.ToString();
                else if (box == WidthNumberBox)
                    box.Text = ((RectangleProperty)_property).Width.ToString();
                else if (box == HeightNumberBox)
                    box.Text = ((RectangleProperty)_property).Height.ToString();
            }
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == this.ConnectButton) {
                this.ConnectClicked(this.ContentGrid as Panel, this.ConnectImage);
            } else if (sender == this.XPosButton) {
                ChangeValue(1.0, XNumberBox);
            } else if (sender == this.XNegButton) {
                ChangeValue(-1.0, XNumberBox);
            } else if (sender == this.YPosButton) {
                ChangeValue(1.0, YNumberBox);
            } else if (sender == this.YNegButton) {
                ChangeValue(-1.0, YNumberBox);
            } else if (sender == this.WidthPosButton) {
                ChangeValue(1.0, WidthNumberBox);
            } else if (sender == this.WidthNegButton) {
                ChangeValue(-1.0, WidthNumberBox);
            } else if (sender == this.HeightPosButton) {
                ChangeValue(1.0, HeightNumberBox);
            } else if (sender == this.HeightNegButton) {
                ChangeValue(-1.0, HeightNumberBox);
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the NumberBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e) {
            ChangeTextValue(sender as TextBox);
        }
    }
}