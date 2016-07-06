using System.IO;

namespace ns.Base.Manager {

    public interface IGenericConfigurationManager<T> : IManager {

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        T Configuration { get; set; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        void Load();

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        void Load(string path);

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        T Load(Stream stream);

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        /// <summary>
        /// Saves the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        void Save(string path);

        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void Save(Stream stream);
    }
}