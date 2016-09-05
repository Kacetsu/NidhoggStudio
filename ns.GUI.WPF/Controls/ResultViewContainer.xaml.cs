using ns.Base.Plugins.Properties;
using System.Windows.Controls;
using System.Windows.Data;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Logic for <see cref="ResultViewContainer"/>.
    /// </summary>
    public partial class ResultViewContainer : UserControl {
        private Base.Plugins.Properties.Property _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultViewContainer"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public ResultViewContainer(Base.Plugins.Properties.Property property) {
            InitializeComponent();
            DataContext = property;
            _property = property;

            if (!(property is INumerical)) {
                MinTextBox.IsEnabled = false;
                MaxTextBox.IsEnabled = false;
                BindingOperations.ClearBinding(MinTextBox, TextBox.TextProperty);
                BindingOperations.ClearBinding(MaxTextBox, TextBox.TextProperty);
            }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public Base.Plugins.Properties.Property Property {
            get { return _property; }
        }

        /// <summary>
        /// Updates the property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void UpdateProperty(Base.Plugins.Properties.Property property) {
            IValue valueProperty = property as IValue;

            if (valueProperty != null) {
                IValue currentValueProperty = _property as IValue;
                if (currentValueProperty != null) {
                    currentValueProperty.ValueObj = valueProperty.ValueObj;
                }
            }
        }
    }
}