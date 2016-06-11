using ns.Base;
using ns.Base.Attribute;

namespace ns.Plugin.AForge {

    [Visible]
    public class AForgeLibraryInformation : LibraryInformation {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name {
            get {
                return "AForge.NET Framework";
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
                return "http://www.aforgenet.com/";
            }
        }
    }
}