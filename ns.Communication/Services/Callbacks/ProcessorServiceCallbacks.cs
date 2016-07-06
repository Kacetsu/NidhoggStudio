using ns.Communication.Events;
using ns.Communication.Models;

namespace ns.Communication.Services.Callbacks {

    public class ProcessorServiceCallbacks : IProcessorServiceCallbacks {

        /// <summary>
        /// Occurs when [processor state changed].
        /// </summary>
        public event ProcessorStateChangedEventHandler ProcessorStateChanged;

        /// <summary>
        /// Called when [processor started].
        /// </summary>
        public void OnProcessorStarted(ProcessorInfoModel processorInfoModel) => ProcessorStateChanged?.Invoke(this, new ProcessorStateEventArgs(processorInfoModel));

        /// <summary>
        /// Called when [processor stopped].
        /// </summary>
        public void OnProcessorStopped(ProcessorInfoModel processorInfoModel) => ProcessorStateChanged?.Invoke(this, new ProcessorStateEventArgs(processorInfoModel));
    }
}