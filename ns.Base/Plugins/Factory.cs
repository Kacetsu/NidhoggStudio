using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    [DataContract]
    public abstract class Factory : Plugin {

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory"/> class.
        /// </summary>
        protected Factory()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        protected Factory(Factory other)
            : base(other) {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize() {
            base.Initialize();
        }
    }
}