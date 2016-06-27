using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Communication.CommunicationModels.Properties {

    [DataContract]
    public class DevicePropertyModel : PropertyModel {

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyModel"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public DevicePropertyModel(Property property) : base(property) {
            IListProperty<Device> valueProperty = property as IListProperty<Device>;

            IListProperty<Device> listProperty = property as IListProperty<Device>;

            DeviceProperty propertyCopy = new DeviceProperty(property.Name, listProperty.Value);
            propertyCopy.SelectedItem = listProperty.SelectedItem;

            if (valueProperty.SelectedItem != null) {
                Device device = new Device(valueProperty.SelectedItem);
                listProperty.SelectedItem = device;
            }

            List<Device> devices = new List<Device>();
            foreach (Device d in valueProperty.Value) {
                devices.Add(new Device(d));
            }

            propertyCopy.Value = devices;

            Property = propertyCopy;
        }
    }
}