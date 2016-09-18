using ns.Base;
using ns.Base.Extensions;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace ns.Plugin.Base {

    [Visible, DataContract]
    public sealed class Sleep : Tool {
        private int _milliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sleep"/> class.
        /// </summary>
        public Sleep() {
            DisplayName = "Sleep";
            AddChild(new IntegerProperty("Milliseconds", 1000));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sleep"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public Sleep(Sleep other) : base(other) {
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public override string Category => ToolCategory.Common.GetDescription();

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new Sleep(this);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            base.Initialize();

            try {
                _milliseconds = GetProperty<IntegerProperty>("Milliseconds").Value;
                return true;
            } catch (Exception ex) {
                ns.Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool TryPreRun() {
            base.TryPreRun();

            try {
                _milliseconds = GetProperty<IntegerProperty>("Milliseconds").Value;
            } catch (Exception ex) {
                ns.Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool TryRun() {
            Thread.Sleep(_milliseconds);
            return true;
        }
    }
}