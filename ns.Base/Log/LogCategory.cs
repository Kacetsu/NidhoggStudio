using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Log {
    /// <summary>
    /// Logging category.
    /// </summary>
    public enum LogCategory : byte {
        [Description("ERROR")]
        Error,
        [Description("WARNING")]
        Warning,
        [Description("INFO")]
        Info,
        [Description("DEBUG")]
        Debug
    }
}
