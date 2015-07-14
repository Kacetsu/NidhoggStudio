using ns.Base.Attribute;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ns.Plugin.WPF.Statistic {
    [Visible]
    public class StatisticExtension : UIExtension {

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticExtension"/> class.
        /// </summary>
        public StatisticExtension() {
            try {
                DisplayName = Application.Current.FindResource("Statistic") as string;
            } catch (Exception) {
                DisplayName = "Statistic";
            }
        }

        private StatisticView _statisticView;

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <value>
        /// The control.
        /// </value>
        public override object Control {
            get {
                if (_statisticView == null) _statisticView = new StatisticView();
                return _statisticView;
            }
        }
    }
}
