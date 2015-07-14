using ns.Base;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaktionslogik für StringPropertyControl.xaml
    /// </summary>
    public partial class StringPropertyControl : PropertyControl {

        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
        public event TextChangedEventHandler TextChanged = delegate { };

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connectable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connectable; otherwise, <c>false</c>.
        /// </value>
        public override bool IsConnectable {
            get {
                return base.IsConnectable;
            }
            set {
                base.IsConnectable = value;
                if (value)
                    this.ConnectButton.Visibility = System.Windows.Visibility.Visible;
                else
                    this.ConnectButton.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyControl"/> class.
        /// </summary>
        public StringPropertyControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="property">The property.</param>
        public StringPropertyControl(ns.Base.Plugins.Properties.Property property)
            : base(property) {
            InitializeComponent();
            this.NameLabel.Content = property.Name;
            StringProperty stringProperty = property as StringProperty;

            if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                ConnectClicked(this.ContentBox as Control, this.ConnectImage);
            } else {
                this.ContentBox.Text = stringProperty.Value as string;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public StringPropertyControl(ns.Base.Plugins.Properties.Property property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            this.NameLabel.Content = property.Name;
            StringProperty stringProperty = property as StringProperty;

            if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                ConnectClicked(this.ContentBox as Control, this.ConnectImage);
            } else {
                this.ContentBox.Text = stringProperty.Value as string;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="content">The content.</param>
        public StringPropertyControl(string name, string content) {
            InitializeComponent();
            IsConnectable = false;
            this.NameLabel.Content = name;
            this.ContentBox.Text = content;
        }



        /// <summary>
        /// Handles the TextChanged event of the ContentBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void ContentBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (this.TextChanged != null) {
                if (this.Property != null) {
                    this.Property.Value = this.ContentBox.Text;
                } else {
                    this.TextChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the ConnectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ConnectButton_Click(object sender, RoutedEventArgs e) {
            this.ConnectClicked(this.ContentBox as Control, this.ConnectImage);
        }
    }
}
