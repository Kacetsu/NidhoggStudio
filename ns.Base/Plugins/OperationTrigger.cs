using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins {
    public enum OperationTrigger : int {
        [Description("Continuous")]
        Continuous,
        [Description("Off")]
        Off,
        [Description("Finished")]
        Finished,
        [Description("Started")]
        Started
    }
}
