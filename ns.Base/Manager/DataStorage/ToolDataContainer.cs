using ns.Base.Plugins;

namespace ns.Base.Manager.DataStorage {

    public class ToolDataContainer : DataContainer {

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolDataContainer"/> class.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public ToolDataContainer(Tool tool) : base(tool) {
        }
    }
}