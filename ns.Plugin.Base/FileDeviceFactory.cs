using ns.Base;
using ns.Base.Plugins;
using System.Runtime.Serialization;

namespace ns.Plugin.Base {

    [Visible]
    [DataContract]
    public class FileDeviceFactory : Factory {
        private const int DeviceCount = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDeviceFactory"/> class.
        /// </summary>
        public FileDeviceFactory()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDeviceFactory"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public FileDeviceFactory(FileDeviceFactory other)
            : base(other) { }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize() {
            base.Initialize();

            for (int index = 0; index < DeviceCount; index++) {
                ImageFileDevice device = new ImageFileDevice();
                device.SerialNumber.Value = string.Format("NSID{0:000000}", index + 1);
                Items.TryAdd(device.Id, device);
            }
        }
    }
}