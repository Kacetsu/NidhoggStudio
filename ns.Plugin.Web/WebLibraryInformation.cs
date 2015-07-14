using ns.Base;
using ns.Base.Attribute;

namespace ns.Plugin.Web {
    [Visible]
    public class WebLibraryInformation : LibraryInformation {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name {
            get {
                return "Nidhogg Studio Monitor";
            }
        }

        /// <summary>
        /// Gets the documentation link.
        /// </summary>
        /// <value>
        /// The documentation link.
        /// </value>
        public override string DocumentationLink {
            get {
                return "http://localhost:9999";
            }
        }
    }
}
