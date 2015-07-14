using ns.Base.Attribute;
using ns.Base.Extensions;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Threading;

namespace ns.Plugin.Base {
    [Visible, Serializable]
    public class Sleep : Tool {

        private int _milliseconds;

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public override string Category {
            get {
                return ToolCategory.Common.GetDescription();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sleep"/> class.
        /// </summary>
        public Sleep() {
            DisplayName = "Sleep";
            AddChild(new IntegerProperty("Milliseconds", 1000));
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            base.Initialize();

            try {
                _milliseconds = (int)GetProperty("Milliseconds").Value;
                return true;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool PreRun() {
            base.PreRun();

            try {
                _milliseconds = (int)GetProperty("Milliseconds").Value;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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
        public override bool Run() {
            Thread.Sleep(_milliseconds);
            return true;
        }
    }
}
