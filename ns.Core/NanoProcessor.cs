using ns.Base;
using ns.Base.Manager.DataStorage;
using ns.Base.Plugins;
using ns.Core.Manager;
using System;
using System.Diagnostics;
using System.Linq;

namespace ns.Core {

    /// <summary>
    /// Runs the Operation on a sync way.
    /// </summary>
    public class NanoProcessor : NotifiableObject {
        private DataStorageManager _dataStorageManager = null;
        private Operation _operation;
        private bool _result = false;

        /// <summary>
        /// Base Constructor.
        /// </summary>
        /// <param name="operation">The Operation to run.</param>
        public NanoProcessor(Operation operation) {
            _operation = operation;
            _dataStorageManager = CoreSystem.Instance.DataStorage;
        }

        /// <summary>
        /// Gets the the referred Operation.
        /// </summary>
        public Operation Operation => _operation;

        /// <summary>
        /// Gets the Result.
        /// </summary>
        public bool Result {
            get { return _result; }
            private set {
                _result = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Starts the execution of the Operation.
        /// </summary>
        /// <returns>true if successful.</returns>
        public virtual bool Start() {
            return Execute();
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        protected bool Execute() {
            bool preResult = false;
            bool postResult = false;
            bool runResult = false;

            try {
                if ((preResult = _operation.TryPreRun()) == true) {
                    if ((runResult = _operation.TryRun()) == false)
                        Base.Log.Trace.WriteLine("Run operation [" + _operation.Name + "] failed!", TraceEventType.Error);
                } else {
                    Base.Log.Trace.WriteLine("Prerun operation [" + _operation.Name + "] failed!", TraceEventType.Error);
                }

                if ((postResult = _operation.TryPostRun()) == false) {
                    Base.Log.Trace.WriteLine("Postrun operation [" + _operation.Name + "] failed!", TraceEventType.Error);
                }

                _dataStorageManager.Add(new OperationDataContainer(_operation));
                foreach (Tool tool in _operation.Items.Values.OfType<Tool>()) {
                    _dataStorageManager.Add(new ToolDataContainer(tool));
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                preResult = false;
                runResult = false;
                postResult = false;
            }

            Result = preResult && runResult && postResult;
            return Result;
        }
    }
}