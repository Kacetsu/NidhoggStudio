namespace ns.Base.Plugins {

    public class UIExtension : Extension {
        private UIExtensionPosition _position = UIExtensionPosition.Bottom;

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <value>
        /// The control.
        /// </value>
        public virtual object Control { get; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public UIExtensionPosition Position {
            get { return _position; }
            set { _position = value; }
        }
    }
}