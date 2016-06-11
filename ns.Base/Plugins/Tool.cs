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
        private DateTime _timeMeasureStart;
        private DoubleProperty _executionTimeMs;

        /// <summary>
        /// Base Class for all Tools.
        /// Creates the field: Properties.
        /// </summary>
        public Tool() : base() {
            DoubleProperty executionTimeMs = new DoubleProperty("ExecutionTimeMs", true);
            executionTimeMs.Tolerance = new Tolerance<double>(0, 1000);
            AddChild(executionTimeMs);
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public override string Name {
            get {
                if (string.IsNullOrEmpty(_name)) {
                    if (!string.IsNullOrEmpty(DisplayName))
                        _name = DisplayName;
                    else
                        _name = this.GetType().Name;
                }
                return _name;
            }
            set {
                base.Name = value;
            }
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
        /// Clones the Tools as deep Clone.
        /// Generates a new UID for the clones Tool.
        /// </summary>
        /// <returns>Returns the cloned Tool.</returns>
        public override object Clone() {
            Tool clone = this.DeepClone();
            clone.UID = GenerateUID();
            return clone;
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            _executionTimeMs = GetProperty("ExecutionTimeMs") as DoubleProperty;
            return base.Initialize();
        }

        /// <summary>
        /// Finalize the Node.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            bool result = base.Finalize();

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

            return result;
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool PreRun() {
            _timeMeasureStart = DateTime.Now;

            return base.PreRun();
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool PostRun() {
            if (_timeMeasureStart != null)
                _executionTimeMs.Value = DateTime.Now.Subtract(_timeMeasureStart).TotalMilliseconds;

            return base.PostRun();
        }
    }
}