using ns.Base.Attribute;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.Test {

    [Visible, DataContract]
    public class DummyTool : Tool {

        public DummyTool() : base() {
            DisplayName = nameof(DummyTool);
        }
    }
}