using System.ComponentModel;

namespace ns.Base.Plugins {
    public enum ToolCategory {
        [Description("Common")]
        Common,
        [Description("Acquisition")]
        Acquisition,
        [Description("Transformation")]
        Transformation,
        [Description("ImageProcessing")]
        ImageProcessing,
        [Description("Filter")]
        Filter,
        [Description("Communication")]
        Communication
    }
}
