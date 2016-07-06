using ns.Base;
using ns.Base.Extensions;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ns.Core {

    /// <summary>
    /// Handles the use of all operations.
    /// </summary>
    public class Processor : NotifiableObject {
        private ExtensionManager _extensionManager;
        private bool _isFinalize = false;
        private List<AsyncNanoProcessor> _nexuses;
        private ProjectManager _projectManager;
        private PropertyManager _propertyManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        public Processor() {
            _projectManager = CoreSystem.FindManager<ProjectManager>();
            _propertyManager = CoreSystem.FindManager<PropertyManager>();
            _extensionManager = CoreSystem.FindManager<ExtensionManager>();
            _nexuses = new List<AsyncNanoProcessor>();
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public ProcessorState State { get; private set; } = ProcessorState.Idle;

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool Pause() {
            return true;
        }

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool Restart() {
            return true;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool Start() {
            if (State == ProcessorState.Running) return true;
            bool initializeResult = InitializeOperations();
            if (initializeResult == true)
                StartOperations();

            if (!initializeResult) {
                FinalizeOperations();
            }

            State = initializeResult ? ProcessorState.Running : ProcessorState.StartFailed;

            return initializeResult;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool Stop() {
            bool resultFinalize = false;
            bool resultTerminate = false;

            resultTerminate = TerminateOperations();
            resultFinalize = FinalizeOperations();

            _nexuses.Clear();

            if (resultFinalize && resultTerminate) {
                State = ProcessorState.Idle;
                return true;
            } else {
                State = ProcessorState.Idle;
                return false;
            }
        }

        /// <summary>
        /// Finalizes the operations.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        private bool FinalizeOperations() {
            _isFinalize = true;
            bool result = false;
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                AsyncNanoProcessor context = _nexuses.Find(o => o != null && o.Operation == operation) as AsyncNanoProcessor;
                if (context != null) {
                    context.Wait();
                    result = context.Operation.Finalize();
                } else {
                    result = operation.Finalize();
                }
            }
            _nexuses.Clear();
            return result;
        }

        /// <summary>
        /// Initializes the operations.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        private bool InitializeOperations() {
            _isFinalize = false;
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                if (operation.Childs.Count < 1 || operation.Initialize() == false) {
                    Base.Log.Trace.WriteLine("Cannot start operation [" + operation.Name + "]!"
                        + Environment.NewLine + "May the operation is empty or something happend while initializing it.", TraceEventType.Warning);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Operations the status changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OperationPropertyChangedHandle(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Status") return;

            Operation operation = sender as Operation;
            PluginStatus status = operation.Status;

            if (_isFinalize == true)
                return;

            List<Operation> connectedOperations = new List<Operation>();

            foreach (Operation o in _projectManager.Configuration.Operations) {
                Property linkedProperty = o.GetProperty<Property>("LinkedOperation");
                if (linkedProperty.ConnectedUID == operation.UID)
                    connectedOperations.Add(o);
            }

            switch (status) {
                case PluginStatus.Failed:
                case PluginStatus.Finished:
                if (_nexuses.Count > 0) {
                    AsyncNanoProcessor executionContext = _nexuses.Find(o => o != null && o.Operation == operation) as AsyncNanoProcessor;
                    if (executionContext != null && _nexuses.Contains(executionContext)) {
                        ListProperty triggerList = operation.GetProperty<ListProperty>("Trigger");
                        string trigger = triggerList.Value.ToString();

                        foreach (Operation o in connectedOperations) {
                            Property triggerProperty = o.GetProperty<Property>("Trigger");
                            if ((triggerProperty as IValue<object>)?.Value.ToString() == OperationTrigger.Finished.GetDescription()) {
                                NanoProcessor nanoProcessor = new NanoProcessor(o);
                                nanoProcessor.Start();
                            }
                        }
                    } else
                        Base.Log.Trace.WriteLine("Unknown execution context! Careful this could end in memory leak!", TraceEventType.Error);
                }
                _extensionManager.RunAll();
                break;

                case PluginStatus.Started:
                foreach (Operation o in connectedOperations) {
                    Property triggerProperty = o.GetProperty<Property>("Trigger");
                    if ((triggerProperty as IValue<object>)?.Value.ToString() == OperationTrigger.Started.GetDescription()) {
                        NanoProcessor nanoProcessor = new NanoProcessor(o);
                        nanoProcessor.Start();
                    }
                }
                break;

                default:
                break;
            }
        }

        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        private void StartOperation(Operation operation) {
            AsyncNanoProcessor nanoProcessor = _nexuses.Find(n => n.Operation == operation);
            operation.PropertyChanged += OperationPropertyChangedHandle;

            if (nanoProcessor == null) {
                nanoProcessor = new AsyncNanoProcessor(operation);
                _nexuses.Add(nanoProcessor);
            }

            if (nanoProcessor.Status != TaskStatus.Running)
                nanoProcessor.Start();
        }

        /// <summary>
        /// Starts the operations.
        /// </summary>
        private void StartOperations() {
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                // ToDo: Refactor!
                //ListProperty triggerList = operation.GetProperty<ListProperty>("Trigger");
                //string trigger = triggerList.Value.ToString();
                //if (trigger != OperationTrigger.Continuous.GetDescription()) continue;
                StartOperation(operation);
            }
        }

        /// <summary>
        /// Terminates the operations.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        private bool TerminateOperations() {
            bool result = true;
            foreach (AsyncNanoProcessor nanoProcessor in _nexuses) {
                nanoProcessor.Operation.PropertyChanged -= OperationPropertyChangedHandle;
                bool tmpResult = false;
                tmpResult = nanoProcessor.Stop();
                result = !tmpResult ? false : true;
            }
            return result;
        }
    }
}