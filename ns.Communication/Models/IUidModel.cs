using System;

namespace ns.Communication.Models {

    public interface IUidModel {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid Id { get; }
    }
}