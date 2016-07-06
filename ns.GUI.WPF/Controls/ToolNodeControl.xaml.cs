using ns.Communication.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für ToolNodeControl.xaml
    /// </summary>
    public partial class ToolNodeControl : UserControl, INotifyPropertyChanged, INodeControl {

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolNodeControl"/> class.
        /// </summary>
        /// <param name="model">The tool.</param>
        public ToolNodeControl(ToolModel model) {
            InitializeComponent();
            Model = model;
            model.PropertyChanged += _model_PropertyChanged;
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName {
            get { return (Model as IToolModel).DisplayName; }
            protected set {
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>
        /// The tool.
        /// </value>
        public object Model { get; private set; }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ToolModel.Name))
                DisplayName = (Model as IToolModel).DisplayName;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e) {
            FrontendManager.OnNodeConfigurationClicked(this);
        }
    }
}