using ns.Base.Attribute;
using ns.Base.Extensions;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;

namespace ns.Plugin.Base {
    [Visible, Serializable]
    public class IterationCounter : Tool {

        private int _iterations = 0;
        private DateTime _lastTime;

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
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public override string Description {
            get {
                return "Counts the iteration of the operation it is the child of.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterationCounter"/> class.
        /// </summary>
        public IterationCounter() {
            DisplayName = "Iteration Counter";
            AddChild(new IntegerProperty("Iterations", true));
            AddChild(new IntegerProperty("ElapsedMs", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _iterations = 0;
            return true;
        }

        public override bool Run() {
            if (_iterations == int.MaxValue) _iterations = 0;
            _iterations++;
            return true;
        }

        public override bool PostRun() {
            base.PostRun();

            IntegerProperty propIteration = GetProperty("Iterations") as IntegerProperty;
            IntegerProperty propElapsedMs = GetProperty("ElapsedMs") as IntegerProperty;
            propIteration.Value = _iterations;

            DateTime endTime = DateTime.Now;
            if (_lastTime.Ticks > 0)
                propElapsedMs.Value = endTime.Subtract(_lastTime).TotalMilliseconds;
            _lastTime = endTime;

            return true;
        }

        public override bool Finalize() {
            base.Finalize();

            _iterations = 0;
            return true;
        }
    }
}
