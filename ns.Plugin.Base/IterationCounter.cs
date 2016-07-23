using ns.Base.Attribute;
using ns.Base.Extensions;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Runtime.Serialization;

namespace ns.Plugin.Base {

    [Visible, DataContract]
    public sealed class IterationCounter : Tool {
        private int _iterations = 0;
        private DateTime _lastTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="IterationCounter"/> class.
        /// </summary>
        public IterationCounter() {
            DisplayName = "Iteration Counter";
            AddChild(new IntegerProperty("Iterations", true));
            AddChild(new DoubleProperty("ElapsedMs", true));
        }

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
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            _iterations = 0;
            base.Close();
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            _iterations = 0;
            return true;
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool TryPostRun() {
            base.TryPostRun();

            IntegerProperty propIteration = GetProperty<IntegerProperty>("Iterations");
            DoubleProperty propElapsedMs = GetProperty<DoubleProperty>("ElapsedMs");
            propIteration.Value = _iterations;

            DateTime endTime = DateTime.Now;
            if (_lastTime.Ticks > 0)
                propElapsedMs.Value = endTime.Subtract(_lastTime).TotalMilliseconds;
            _lastTime = endTime;

            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool TryRun() {
            if (_iterations == int.MaxValue) _iterations = 0;
            _iterations++;
            return true;
        }
    }
}