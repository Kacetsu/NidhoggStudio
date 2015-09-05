using System.Collections.Generic;

namespace ns.Base.Plugins {
    interface INode {
        bool Initialize();
        bool Finalize();
        void AddChild(Node child);
        void AddChilds(List<Node> childs);
        void RemoveChild(Node child);
        void RemoveChilds();
        void SetParent(Node parent);
    }
}
