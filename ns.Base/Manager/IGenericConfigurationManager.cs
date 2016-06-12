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
        /// <returns></returns>
        bool Load();

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool Load(string path);

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        T Load(Stream stream);

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        bool Save();

        /// <summary>
        /// Saves the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool Save(string path);

        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        bool Save(Stream stream);
    }
}