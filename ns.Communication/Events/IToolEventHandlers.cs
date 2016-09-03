namespace ns.Communication.Events {

    public interface IToolEventHandlers {

        /// <summary>
        /// Occurs when [tool added].
        /// </summary>
        event ToolAddedEventHandler ToolAdded;

        /// <summary>
        /// Occurs when [tool removed].
        /// </summary>
        event ToolRemovedEventHandler ToolRemoved;
    }
}