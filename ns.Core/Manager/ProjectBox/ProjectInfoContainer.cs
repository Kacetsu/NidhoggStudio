using ns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Core.Manager.ProjectBox {
    public class ProjectInfoContainer : NotifyObject {
        private string _path = string.Empty;
        private string _name = string.Empty;
        private bool _isUsed = false;

        public string Path {
            get { return _path; }
        }

        public string Name {
            get { return _name; }
            set {
                if (_name.Equals(value)) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public bool IsUsed {
            get { return _isUsed; }
            set {
                if (_isUsed == value) return;
                _isUsed = value;
                OnPropertyChanged("IsUsed");
            }
        }

        public ProjectInfoContainer(string path, string name) {
            _path = path;
            _name = name;
        }
    }
}
