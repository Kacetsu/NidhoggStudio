using ns.Base.Plugins.Properties;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaktionslogik für StringPropertyControl.xaml
    /// </summary>
    public partial class StringPropertyControl : PropertyControl<StringProperty> {

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
                ConnectButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyControl"/> class.
        /// </summary>
        public StringPropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="property">The property.</param>
        public StringPropertyControl(StringProperty property)
            : base(property) {
            InitializeComponent();
            DataContext = this;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(ContentBox as Control, ConnectImage);
            } else {
                ContentBox.Text = property.Value as string;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public StringPropertyControl(StringProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            DataContext = this;
            _property = property;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(ContentBox as Control, ConnectImage);
            } else {
                ContentBox.Text = property.Value as string;
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
            ContentBox.Text = content;
        }

        /// <summary>
        /// Handles the TextChanged event of the ContentBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void ContentBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (TextChanged != null) {
                if (Property != null) {
                    Property.Value = ContentBox.Text;
                } else {
                    TextChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the ConnectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ConnectButton_Click(object sender, RoutedEventArgs e) {
            ConnectClicked(this.ContentBox as Control, this.ConnectImage);
        }
    }
}