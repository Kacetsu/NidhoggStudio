using ns.Base.Extensions;
using ns.Base.Plugins.Properties;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class for all Tools.
    /// </summary>
    [DataContract]
    public class Tool : Plugin {
        private DoubleProperty _executionTimeMs;
        private DateTime _timeMeasureStart;

        /// <summary>
        /// Base Class for all Tools.
        /// Creates the field: Properties.
        /// </summary>
        public Tool() : base() {
            DoubleProperty executionTimeMs = new DoubleProperty("ExecutionTimeMs", true);
            executionTimeMs.Tolerance = new Tolerance<double>(0, 1000);
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
        /// Gets or sets the Name.
        /// </summary>
        [DataMember]
        public override string Name {
            get {
                if (string.IsNullOrEmpty(base.Name)) {
                    if (!string.IsNullOrEmpty(DisplayName))
                        base.Name = DisplayName;
                    else
                        base.Name = GetType().Name;
                }
                return base.Name;
            }
            set {
                base.Name = value;
            }
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

            foreach (Property childProperty in this.Childs.Where(c => c is Property)) {
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
            _executionTimeMs = GetProperty<DoubleProperty>("ExecutionTimeMs");
            return base.Initialize();
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool TryPostRun() {
            if (_timeMeasureStart != null)
                _executionTimeMs.Value = DateTime.Now.Subtract(_timeMeasureStart).TotalMilliseconds;

            return base.TryPostRun();
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool TryPreRun() {
            _timeMeasureStart = DateTime.Now;

            return base.TryPreRun();
        }
    }
}