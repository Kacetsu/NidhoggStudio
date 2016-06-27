using System.Collections.Generic;

namespace ns.Base.Plugins {

    public interface INode {

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        bool Initialize();

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        bool Finalize();

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="child">The child.</param>
        void AddChild(Node child);

        /// <summary>
        /// Adds the childs.
        /// </summary>
        /// <param name="childs">The childs.</param>
        void AddChilds(List<Node> childs);

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        void RemoveChild(Node child);

        /// <summary>
        /// Removes the childs.
        /// </summary>
        void RemoveChilds();

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        void SetParent(Node parent);
    }
}