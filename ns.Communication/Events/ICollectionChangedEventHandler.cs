using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.Events {

    public interface ICollectionChangedEventHandler {

        /// <summary>
        /// Occurs when [collection changed].
        /// </summary>
        event CollectionChangedEventHandler CollectionChanged;
    }
}