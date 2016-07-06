using ns.Base.Plugins;

namespace ns.Base.Manager.DataStorage {

    public class OperationDataContainer : DataContainer {

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationDataContainer"/> class.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public OperationDataContainer(Operation operation) : base(operation) {
        }
    }
}