using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    [DataContract]
    public enum PluginStatus {

        [EnumMember]
        Started,

        [EnumMember]
        Finished,

        [EnumMember]
        Failed,

        [EnumMember]
        Aborted,

        [EnumMember]
        Unknown
    }
}