using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ns.Core {
    public class ExecutionContext {
        private Thread _thread;
        private bool _result = false;
        private Plugin _plugin = null;

#if STOPWATCH
        private System.Diagnostics.Stopwatch _stopwatch;

        public System.Diagnostics.Stopwatch Stopwatch {
            get { return _stopwatch; }
        }
#endif

        /// <summary>
        /// Gets or sets the thread.
        /// </summary>
        /// <value>
        /// The thread.
        /// </value>
        public Thread Thread { 
            get { return _thread; }
            set { _thread = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExecutionContext"/> is result.
        /// </summary>
        /// <value>
        ///   <c>true</c> if result; otherwise, <c>false</c>.
        /// </value>
        public bool Result { 
            get { return _result; }
            set { _result = value; }
        }

        /// <summary>
        /// Gets or sets the plugin.
        /// </summary>
        /// <value>
        /// The plugin.
        /// </value>
        public Plugin Plugin {
            get { return _plugin; }
            set { _plugin = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
        /// </summary>
        public ExecutionContext() {
#if STOPWATCH
            _stopwatch = new System.Diagnostics.Stopwatch();
            _stopwatch.Start();
#endif
        }
    }
}
