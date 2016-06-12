using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for RectanglePropertyControl.xaml
    /// </summary>
    public partial class RectanglePropertyControl : PropertyControl<RectangleProperty> {

        public RectanglePropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectanglePropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public RectanglePropertyControl(RectangleProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            DataContext = this;
            property.PropertyChanged += Property_PropertyChanged;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(ContentGrid as Panel, ConnectImage);
            } else {
                RectangleProperty rectangleProperty = property as RectangleProperty;
                XNumberBox.Text = rectangleProperty.X.ToString();
                YNumberBox.Text = rectangleProperty.Y.ToString();
                WidthNumberBox.Text = rectangleProperty.Width.ToString();
                HeightNumberBox.Text = rectangleProperty.Height.ToString();
            }
        }

        private void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            RectangleProperty property = sender as RectangleProperty;
            XNumberBox.Text = property.X.ToString();
            YNumberBox.Text = property.Y.ToString();
            WidthNumberBox.Text = property.Width.ToString();
            HeightNumberBox.Text = property.Height.ToString();
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

            if (_property != null) {
                double value = Convert.ToDouble(box.Text);
                if (box == XNumberBox)
                    _property.X = value;
                else if (box == YNumberBox)
                    _property.Y = value;
                else if (box == WidthNumberBox)
                    _property.Width = value;
                else if (box == HeightNumberBox)
                    _property.Height = value;
            }

            if (box == XNumberBox)
                box.Text = _property.X.ToString();
            else if (box == YNumberBox)
                box.Text = _property.Y.ToString();
            else if (box == WidthNumberBox)
                box.Text = _property.Width.ToString();
            else if (box == HeightNumberBox)
                box.Text = _property.Height.ToString();
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == ConnectButton) {
                ConnectClicked(ContentGrid as Panel, ConnectImage);
            } else if (sender == XPosButton) {
                ChangeValue(1.0, XNumberBox);
            } else if (sender == XNegButton) {
                ChangeValue(-1.0, XNumberBox);
            } else if (sender == YPosButton) {
                ChangeValue(1.0, YNumberBox);
            } else if (sender == YNegButton) {
                ChangeValue(-1.0, YNumberBox);
            } else if (sender == WidthPosButton) {
                ChangeValue(1.0, WidthNumberBox);
            } else if (sender == WidthNegButton) {
                ChangeValue(-1.0, WidthNumberBox);
            } else if (sender == HeightPosButton) {
                ChangeValue(1.0, HeightNumberBox);
            } else if (sender == HeightNegButton) {
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