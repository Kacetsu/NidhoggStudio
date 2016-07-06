using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.Event {

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DataStorageCollectionChangedEventArgs"/> instance containing the event data.</param>
    public delegate void DataStorageCollectionChangedEventHandler(object sender, DataStorageCollectionChangedEventArgs e);
}