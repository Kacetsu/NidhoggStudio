using ns.Base;
using ns.Core;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public class ProcessorInfoModel {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorInfoModel"/> class.
        /// </summary>
        public ProcessorInfoModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorInfoModel"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public ProcessorInfoModel(Processor processor) : this() {
            State = processor.State;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [DataMember]
        public ProcessorState State { get; private set; } = ProcessorState.Idle;
    }
}