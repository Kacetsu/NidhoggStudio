using ns.Base;

namespace ns.Plugin.AForge {

    [Visible]
    public class AForgeLibraryInformation : LibraryInformation {

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
    }
}