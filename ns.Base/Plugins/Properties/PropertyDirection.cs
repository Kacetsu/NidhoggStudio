using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public enum PropertyDirection {

        /// <summary>
        /// The in
        /// </summary>
        [EnumMember]
        In,

        /// <summary>
        /// The out
        /// </summary>
        [EnumMember]
        Out
    }
}