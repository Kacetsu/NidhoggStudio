using System.Collections.Generic;

namespace ns.Base.Plugins.Properties {

    public interface IListProperty<T> : IValue<List<T>> {

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        int Index { get; }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        T SelectedItem { get; set; }
    }
}