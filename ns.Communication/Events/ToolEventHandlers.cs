namespace ns.Communication.Events {

    /// <summary>
    /// Raised if tool is added.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="CollectionChangedEventArgs"/> instance containing the event data.</param>
    public delegate void ToolAddedEventHandler(object sender, CollectionChangedEventArgs e);

    /// <summary>
    /// Raised if tool is removed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="CollectionChangedEventArgs"/> instance containing the event data.</param>
    public delegate void ToolRemovedEventHandler(object sender, CollectionChangedEventArgs e);
}