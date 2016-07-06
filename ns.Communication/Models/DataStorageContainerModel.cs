using ns.Base.Manager.DataStorage;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public class DataStorageContainerModel : GenericModel<DataContainer> {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageContainerModel"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public DataStorageContainerModel(DataContainer container) : base(container) {
        }
    }
}