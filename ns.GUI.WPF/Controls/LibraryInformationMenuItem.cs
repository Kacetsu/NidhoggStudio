using ns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {
    public class LibraryInformationMenuItem : MenuItem {
        private LibraryInformation _information;

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public LibraryInformation Information {
            get { return _information; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryInformationMenuItem"/> class.
        /// </summary>
        /// <param name="information">The information.</param>
        public LibraryInformationMenuItem(LibraryInformation information)
            : base() {
                _information = information;
                this.Header = information.Name;
                this.Style = new Style(GetType(), this.FindResource(typeof(MenuItem)) as Style);
        }
    }
}
