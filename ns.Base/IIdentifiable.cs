using System;

namespace ns.Base {

    public interface IIdentifiable {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid Id { get; set; }
    }
}