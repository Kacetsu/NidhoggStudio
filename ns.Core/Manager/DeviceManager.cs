using ns.Base;
using ns.Base.Manager;
using ns.Base.Plugins.Devices;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ns.Core.Manager {

    public class DeviceManager : NodeManager<Device>, INodeManager<Device> {

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceManager"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DeviceManager([CallerMemberName] string name = null) : base(name) { }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize() {
            base.Initialize();
            Items = new Base.Collections.ObservableConcurrentDictionary<Guid, Node>(CoreSystem.Instance.Plugins.Items.Where(i => i.Value is Device));
        }
    }
}