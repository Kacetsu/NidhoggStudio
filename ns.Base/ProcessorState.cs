using System.Runtime.Serialization;

namespace ns.Base {

    [DataContract]
    public enum ProcessorState {

        [EnumMember]
        Idle,

        [EnumMember]
        Running,

        [EnumMember]
        StartFailed
    }
}