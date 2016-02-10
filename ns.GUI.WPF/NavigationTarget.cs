using ns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ns.GUI.WPF {
    public class NavigationTarget : NotifyObject {
        private string _name;
        private string _displayName;
        private List<NavigationTarget> _childTargets;
        private NavigationTarget _parent;
        private BitmapImage _icon;

        public string Name {
            get { return _name; }
        }
        
        public string DisplayName {
            get { return _displayName; }
        }

        public BitmapImage Icon {
            get { return _icon; }
        }

        public NavigationTarget Parent {
            get { return _parent; }
            protected set { _parent = value; }
        }

        public NavigationTarget() { }

        public NavigationTarget(string name) {
            _name = name;
            _displayName = name;
        }

        public NavigationTarget(string name, BitmapImage icon) {
            _name = name;
            _displayName = name;
            _icon = icon;
        }

        public NavigationTarget(string name, string displayName) {
            _name = name;
            _displayName = displayName;
        }

        public NavigationTarget(string name, string displayName, BitmapImage icon) {
            _name = name;
            _displayName = displayName;
            _icon = icon;
        }

        public bool Add(NavigationTarget child) {
            bool result = false;

            if (_childTargets == null)
                _childTargets = new List<NavigationTarget>();

            if (!_childTargets.Contains(child)) {
                child.Parent = this;
                _childTargets.Add(child);
                result = true;
            }

            return result;
        }

        public virtual void CallAction() {

        }
    }
}
