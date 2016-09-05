using ns.Base.Plugins.Properties;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class for all Tools.
    /// </summary>
    [DataContract]
    public class Tool : Plugin {
        private IntegerProperty _executionTimeMs;
        private Stopwatch _stopwatch;

        /// <summary>
        /// Base Class for all Tools.
        /// Creates the field: Properties.
        /// </summary>
        public Tool() : base() {
            Name = string.IsNullOrEmpty(DisplayName) ? GetType().Name : DisplayName;
            IntegerProperty executionTimeMs = new IntegerProperty("ExecutionTimeMs", true);
            executionTimeMs.Tolerance = new Tolerance<int>(0, 1000);
            AddChild(executionTimeMs);
        }

        public Tool(Tool other) : base(other) {
            Name = other.Name;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public virtual string Category {
            get { return string.Empty; }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() => new Tool(this);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            if (_executionTimeMs != null)
                _executionTimeMs.Value = 0;

            foreach (Property childProperty in this.Items.Where(c => c is Property)) {
                IValue<object> valueProperty = childProperty as IValue<object>;
                if (valueProperty == null) continue;

                if (childProperty.IsOutput) {
                    valueProperty.Value = null;
                } else if (!string.IsNullOrEmpty(childProperty.ConnectedUID)) {
                    valueProperty.Value = (childProperty as IConnectable<object>)?.InitialValue;
                }
            }

            base.Close();
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            _executionTimeMs = GetProperty<IntegerProperty>("ExecutionTimeMs");
            return base.Initialize();
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool TryPostRun() {
            _stopwatch?.Stop();
            _executionTimeMs.Value = _stopwatch != null ? (int)_stopwatch.ElapsedMilliseconds : -1;

            return base.TryPostRun();
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool TryPreRun() {
            _stopwatch = Stopwatch.StartNew();

            return base.TryPreRun();
        }
    }
}