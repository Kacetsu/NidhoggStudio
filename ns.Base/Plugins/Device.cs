using ns.Base.Extensions;
using ns.Base.Plugins.Properties;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    [DataContract]
    public class Device : Plugin {

        public Device() : base() {
        }

        public Device(Device other) : base(other) {
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            Device clone = this.DeepClone();
            clone.UID = GenerateUID();
            return clone;
        }

        /// <summary>
        /// Finalize the Node.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            bool result = base.Finalize();

            foreach (Property childProperty in Childs.Where(c => c is Property)) {
                IValue<object> valueProperty = childProperty as IValue<object>;
                if (valueProperty == null) continue;

                if (childProperty.IsOutput)
                    valueProperty.Value = null;
                else if (!string.IsNullOrEmpty(childProperty.ConnectedUID))
                    valueProperty.Value = (childProperty as IConnectable<object>)?.InitialValue;
            }

            return result;
        }
    }
}