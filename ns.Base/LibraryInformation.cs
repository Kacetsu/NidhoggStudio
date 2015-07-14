using ns.Base.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base {
    [Visible]
    public class LibraryInformation {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the documentation link.
        /// </summary>
        /// <value>
        /// The documentation link.
        /// </value>
        public virtual string DocumentationLink {
            get { return string.Empty; }
        }
    }
}
