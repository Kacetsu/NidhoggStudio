using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Communication.Models.Properties {

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
            propertyCopy.UID = property.UID;

            List<Device> devices = new List<Device>();
            foreach (Device device in valueProperty.Value) {
                Device deviceCopy = new Device(device);
                devices.Add(deviceCopy);
                if (deviceCopy.UID.Equals(listProperty.SelectedItem?.UID)) {
                    propertyCopy.SelectedItem = deviceCopy;
                }
            }

            propertyCopy.Value = devices;

            Property = propertyCopy;
        }
    }
}