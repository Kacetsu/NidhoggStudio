using ns.Base;
using ns.Base.Plugins;
using ns.Core.Manager;
using System;
using System.Diagnostics;

namespace ns.Core {

    /// <summary>
    /// Runs the Operation on a sync way.
    /// </summary>
    public class NanoProcessor : NotifiableObject {
        private DataStorageManager _dataStorageManager;
        private Operation _operation;
        private bool _result = false;

        /// <summary>
        /// Base Constructor.
        /// </summary>
        /// <param name="operation">The Operation to run.</param>
        public NanoProcessor(Operation operation) {
            _operation = operation;
            _operation.PropertyChanged += OperationPropertyChanged;
            _dataStorageManager = CoreSystem.FindManager<DataStorageManager>();
        }

        public NanoProcessor(Operation operation, DataStorageManager dataStorageManager) {
            _operation = operation;
            _operation.PropertyChanged += OperationPropertyChanged;
            _dataStorageManager = dataStorageManager;
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

        protected bool Execute() {
            bool preResult = false;
            bool postResult = false;
            bool runResult = false;

            try {
                if ((preResult = _operation.PreRun()) == true) {
                    if ((runResult = _operation.Run()) == false)
                        Base.Log.Trace.WriteLine("Run operation [" + _operation.Name + "] failed!", TraceEventType.Error);
                } else {
                    Base.Log.Trace.WriteLine("Prerun operation [" + _operation.Name + "] failed!", TraceEventType.Error);
                }

                if ((postResult = _operation.PostRun()) == false)
                    Base.Log.Trace.WriteLine("Postrun operation [" + _operation.Name + "] failed!", TraceEventType.Error);
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                preResult = false;
                runResult = false;
                postResult = false;
            }

            Result = preResult && runResult && postResult;
            return Result;
        }

        private void OperationPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName != "Status") return;

            switch (_operation.Status) {
                case PluginStatus.Failed:
                case PluginStatus.Finished:
                _dataStorageManager.AddContext(_operation);
                break;

                default:
                break;
            }
        }
    }
}