using ns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Core {
    public class DataStorage {

        private string _projectName;
        private string _projectUID;
        private List<DataStorageContainer> _containers = new List<DataStorageContainer>();

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        public string ProjectName {
            get { return _projectName; }
            set { _projectName = value; }
        }

        /// <summary>
        /// Gets or sets the project uid.
        /// </summary>
        /// <value>
        /// The project uid.
        /// </value>
        public string ProjectUID {
            get { return _projectUID; }
            set { _projectUID = value; }
        }

        /// <summary>
        /// Gets or sets the containers.
        /// </summary>
        /// <value>
        /// The containers.
        /// </value>
        public List<DataStorageContainer> Containers {
            get { return _containers; }
            set { _containers = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorage"/> class.
        /// </summary>
        public DataStorage() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorage"/> class.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="projectUID">The project uid.</param>
        public DataStorage(string projectName, string projectUID) {
            _projectName = projectName;
            _projectUID = projectUID;
        }
    }
}
