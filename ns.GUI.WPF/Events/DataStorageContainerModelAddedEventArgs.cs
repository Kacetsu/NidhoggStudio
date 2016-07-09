using ns.Communication.Models;
using System;

namespace ns.GUI.WPF.Events {

    internal class DataStorageContainerModelAddedEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageContainerModelAddedEventArgs"/> class.
        /// </summary>
        /// <param name="containerModel">The container model.</param>
        public DataStorageContainerModelAddedEventArgs(DataStorageContainerModel containerModel) : base() {
            ContainerModel = containerModel;
        }

        /// <summary>
        /// Gets the container model.
        /// </summary>
        /// <value>
        /// The container model.
        /// </value>
        public DataStorageContainerModel ContainerModel { get; private set; }
    }
}