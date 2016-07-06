namespace ns.Communication.Events {

    public interface IProcessorStateChangedEventHandler {

        /// <summary>
        /// Occurs when [processor state changed].
        /// </summary>
        event ProcessorStateChangedEventHandler ProcessorStateChanged;
    }
}