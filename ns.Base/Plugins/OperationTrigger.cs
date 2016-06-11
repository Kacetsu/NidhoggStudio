using System.ComponentModel;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    [DataContract]
    public enum OperationTrigger : int {

        [Description("Continuous"), EnumMember]
        Continuous,

        [Description("Off"), EnumMember]
        Off,

        [Description("Finished"), EnumMember]
        Finished,

        [Description("Started"), EnumMember]
        Started
    }
}