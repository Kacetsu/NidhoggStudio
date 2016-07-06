namespace ns.Communication.Models {

    public interface IGenericModel<T> : IUidModel {

        /// <summary>
        /// Gets the fullname.
        /// </summary>
        /// <value>
        /// The fullname.
        /// </value>
        string Fullname { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
    }
}