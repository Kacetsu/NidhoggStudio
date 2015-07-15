using ns.Base;
using ns.Base.Event;
using ns.Base.Log;
using ns.Base.Manager;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ns.Core.Manager {
    public class DataStorageManager : BaseManager {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataStorageContainerChangedEventArgs"/> instance containing the event data.</param>
        public delegate void ContainerCollectionChangedHandler(object sender, DataStorageContainerChangedEventArgs e);

        /// <summary>
        /// Occurs when [container added event].
        /// </summary>
        public event ContainerCollectionChangedHandler ContainerAddedEvent;

        /// <summary>
        /// Occurs when [container removed event].
        /// </summary>
        public event ContainerCollectionChangedHandler ContainerRemovedEvent;

        /// <summary>
        /// Occurs when [container changed event].
        /// </summary>
        public event ContainerCollectionChangedHandler ContainerChangedEvent;

        private const int MAX_VALUES = 100;
        private DataStorage _dataStorage;

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            _dataStorage = new DataStorage();
            return true;
        }

        /// <summary>
        /// Saves the manager to a MemoryStream.
        /// </summary>
        /// <param name="stream">Reference to the MemoryStream.</param>
        /// <returns>
        /// Success of the operation.
        /// </returns>
        public override bool Save(ref System.IO.MemoryStream stream) {
            try {
                XmlSerializer serializer = new XmlSerializer(_dataStorage.GetType());
                serializer.Serialize(stream, _dataStorage);
                stream.Flush();
                return true;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
        }

        /// <summary>
        /// Loads the manager from a FileStream.
        /// </summary>
        /// <param name="stream">The FileStream.</param>
        /// <returns>
        /// The manager object. NULL if any error happend.
        /// </returns>
        public override object Load(System.IO.FileStream stream) {
            try {
                object obj;
                XmlSerializer serializer = new XmlSerializer(_dataStorage.GetType());
                obj = serializer.Deserialize(stream);
                return obj;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return null;
            }
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Add(Base.Node node) {
            if(node is Property){
                Property property = node as Property;
                DataStorageContainer container;

                lock (_dataStorage.Containers) {
                    try {
                        container = _dataStorage.Containers.Find(c => c.TreeName == property.TreeName);
                        if (container.Values.Count > MAX_VALUES)
                            container.Values.RemoveAt(0);
                        container.Values.Add(property.Value);
                        if (this.ContainerChangedEvent != null) {
                            this.ContainerChangedEvent(this, new DataStorageContainerChangedEventArgs(property, container));
                        }
                    } catch {
                        container = new DataStorageContainer(property.Name, property.TreeName, property.UID);
                        _dataStorage.Containers.Add(container);
                        if (this.ContainerAddedEvent != null) {
                            this.ContainerAddedEvent(this, new DataStorageContainerChangedEventArgs(property, container));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Remove(Node node) {
            if (node is Property) {
                Property property = node as Property;
                DataStorageContainer container;

                lock (_dataStorage.Containers) {
                    try {
                        container = _dataStorage.Containers.Find(c => c.TreeName == property.TreeName);
                        if (this.ContainerRemovedEvent != null) {
                            this.ContainerRemovedEvent(this, new DataStorageContainerChangedEventArgs(property, container));
                        }
                        _dataStorage.Containers.Remove(container);
                    } catch(Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                    }
                }

            }
        }
    }
}
