using ns.Base.Configuration;
using System.Collections.Generic;

namespace ns.Core.Configuration {

    public class LogConfiguration : BaseConfiguration {
        private List<string> _categories = new List<string>();

        /// <summary>
        /// Gets or sets the log categories.
        /// </summary>
        public List<string> Categories {
            get { return _categories; }
            set { _categories = value; }
        }
    }
}