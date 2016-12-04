using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class for all Tools.
    /// </summary>
    [DataContract]
    public abstract class Tool : Plugin {
        private Stopwatch _stopwatch;

        /// <summary>
        /// Base Class for all Tools.
        /// Creates the field: Properties.
        /// </summary>
        public Tool()
            : base() {
            Name = string.IsNullOrEmpty(DisplayName) ? GetType().Name : DisplayName;
        }

        public Tool(Tool other)
            : base(other) {
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public virtual string Category { get; } = string.Empty;

        /// <summary>
        /// Gets the execution time ms.
        /// </summary>
        /// <value>
        /// The execution time ms.
        /// </value>
        public IntegerProperty ExecutionTimeMs {
            get {
                IntegerProperty property = FindOrAdd<IntegerProperty, int>(0, PropertyDirection.Out);
                property.Tolerance = new Tolerance<int>(0, 1000);
                return property;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            ExecutionTimeMs.Value = 0;

            foreach (Property childProperty in Items.Values.OfType<Property>()) {
                IValue<object> valueProperty = childProperty as IValue<object>;
                if (valueProperty == null) continue;

                if (childProperty.Direction != PropertyDirection.In) {
                    valueProperty.Value = null;
                } else if (!Guid.Empty.Equals(childProperty.ConnectedId)) {
                    valueProperty.Value = (childProperty as IConnectable<object>)?.InitialValue;
                }
            }

            base.Close();
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool TryPostRun() {
            _stopwatch?.Stop();
            ExecutionTimeMs.Value = _stopwatch != null ? (int)_stopwatch.ElapsedMilliseconds : -1;

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