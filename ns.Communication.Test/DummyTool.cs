using ns.Base;
using ns.Base.Plugins;
using System.Runtime.Serialization;

namespace ns.Communication.Test {

    [Visible, DataContract]
    public class DummyTool : Tool {

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyTool"/> class.
        /// </summary>
        public DummyTool() : base() {
            DisplayName = nameof(DummyTool);
        }
    }
}