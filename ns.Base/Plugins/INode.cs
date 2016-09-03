using System.Collections.Generic;

namespace ns.Base.Plugins {

    public interface INode : ICloseable {

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="child">The child.</param>
        void AddChild(Node child);

        /// <summary>
        /// Adds the childs.
        /// </summary>
        /// <param name="childs">The childs.</param>
        void AddChilds(ICollection<Node> childs);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        bool Initialize();

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        void RemoveChild(Node child);

        /// <summary>
        /// Removes the childs.
        /// </summary>
        void RemoveChilds();
    }
}