using ns.Communication.Models;
using System;

namespace ns.Communication.Events {

    public class ProcessorStateEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorStateEventArgs"/> class.
        /// </summary>
        /// <param name="processorInfoModel">The processor information model.</param>
        public ProcessorStateEventArgs(ProcessorInfoModel processorInfoModel) : base() {
            ProcessorInfoModel = processorInfoModel;
        }

        /// <summary>
        /// Gets the processor infor model.
        /// </summary>
        /// <value>
        /// The processor infor model.
        /// </value>
        public ProcessorInfoModel ProcessorInfoModel { get; private set; }
    }
}