using ns.Base.Collections;
using ns.Base.Plugins.Properties;
using ns.Base.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Devices {

    [DataContract]
    [KnownType(typeof(ImageProperty))]
    public abstract class Device : Plugin {
        private Guid _selectedTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        protected Device()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        protected Device(Device other)
            : base(other) {
        }

        /// <summary>
        /// Gets or sets the selected template.
        /// </summary>
        /// <value>
        /// The selected template.
        /// </value>
        [DataMember]
        public Guid SelectedTemplate {
            get { return _selectedTemplate; }
            set {
                if (_selectedTemplate != value) {
                    _selectedTemplate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        public StringProperty SerialNumber => FindOrAdd<StringProperty, string>("Unknown", PropertyDirection.Out);

        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>
        /// The templates.
        /// </value>
        [DataMember]
        public ObservableConcurrentDictionary<Guid, DeviceTemplate> Templates { get; set; } = new ObservableConcurrentDictionary<Guid, DeviceTemplate>();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            foreach (Property childProperty in Items.Values.OfType<Property>()) {
                IValue<object> valueProperty = childProperty as IValue<object>;
                if (valueProperty == null) continue;

                if (childProperty.Direction != PropertyDirection.In)
                    valueProperty.Value = null;
                else if (!Guid.Empty.Equals((childProperty.ConnectedId)))
                    valueProperty.Value = (childProperty as IConnectable<object>)?.InitialValue;
            }

            base.Close();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize() {
            base.Initialize();
            PropertyChanged -= PropertyChangedHandler;
            PropertyChanged += PropertyChangedHandler;

            if (SelectedTemplate != Guid.Empty) {
                UpdateSettingsByTemplate(SelectedTemplate);
            }
        }

        /// <summary>
        /// Updates the settings by template.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        protected virtual void UpdateSettingsByTemplate(Guid templateId) {
            if (!Templates.ContainsKey(templateId)) {
                throw new KeyNotFoundException(templateId.ToString());
            }

            foreach (Property property in Templates[templateId].Items.OfType<Property>()) {
                Property deviceProperty = Items.Values.OfType<Property>().FirstOrDefault(n => n.Name.Equals(property.Name, StringComparison.Ordinal));
                if (deviceProperty != null) {
                    deviceProperty = property;
                }
            }
        }

        private void PropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(SelectedTemplate), StringComparison.Ordinal)) {
                UpdateSettingsByTemplate(SelectedTemplate);
            }
        }
    }
}