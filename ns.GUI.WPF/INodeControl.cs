using ns.Communication.Models;

namespace ns.GUI.WPF {

    public interface INodeControl {

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        IPluginModel Model { get; }
    }
}