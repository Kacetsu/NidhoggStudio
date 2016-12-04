using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;
using System.Threading;

namespace ns.Plugin.Base {

    [Visible, DataContract]
    public sealed class Sleep : Tool {

        /// <summary>
        /// Initializes a new instance of the <see cref="Sleep"/> class.
        /// </summary>
        public Sleep() : base() {
            DisplayName = "Sleep";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sleep"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public Sleep(Sleep other) : base(other) {
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public override string Category => "Common";

        /// <summary>
        /// Gets the milliseconds.
        /// </summary>
        /// <value>
        /// The milliseconds.
        /// </value>
        public IntegerProperty Milliseconds => FindOrAdd<IntegerProperty, int>(1000, 1, int.MaxValue);

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new Sleep(this);

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool TryRun() {
            Thread.Sleep(Milliseconds.Value);
            return true;
        }
    }
}