using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins {
    public class UIExtension : Extension {
        private UIExtensionPosition _position = UIExtensionPosition.Bottom;

        public UIExtensionPosition Position {
            get { return _position; }
            set { _position = value; }
        }

        public virtual object Control {
            get { throw new NotImplementedException(); }
        }
    }
}
