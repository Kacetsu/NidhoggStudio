using ns.GUI.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Nidhogg_Studio {
    public class EditorNavigationTarget : NavigationTarget {
        public EditorNavigationTarget(string name) : base(name) { }
        public EditorNavigationTarget(string name, BitmapImage icon) : base(name, icon) { }

        public override void CallAction() {
            base.CallAction();
        }
    }

    public class MonitorNavigationTarget : NavigationTarget {
        public MonitorNavigationTarget(string name) : base(name) { }
        public MonitorNavigationTarget(string name, BitmapImage icon) : base(name, icon) { }

        public override void CallAction() {
            base.CallAction();
        }
    }

    public class StatisticNavigationTarget : NavigationTarget {
        public StatisticNavigationTarget(string name) : base(name) { }
        public StatisticNavigationTarget(string name, BitmapImage icon) : base(name, icon) { }

        public override void CallAction() {
            base.CallAction();
        }
    }

    public class LogNavigationTarget : NavigationTarget {
        public LogNavigationTarget(string name) : base(name) { }
        public LogNavigationTarget(string name, BitmapImage icon) : base(name, icon) { }

        public override void CallAction() {
            base.CallAction();
        }
    }
}
