using ns.Base;
using ns.Base.Extensions;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ns.Core {
    /// <summary>
    /// Handles the use of all operations.
    /// </summary>
    public class Processor : NotifiableObject {

        private ProjectManager _projectManager;
        private DataStorageManager _dataStorageManager;
        private PropertyManager _propertyManager;
        private ExtensionManager _extensionManager;
        private List<AsyncNanoProcessor> _nexuses;
        private bool _isFinalize = false;
        private bool _isRunning = false;

        [XmlIgnore]
        public Action Stopped;

        [XmlIgnore]
        public Action Started;

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning {
            get { return _isRunning; }
            protected set {
                if(_isRunning != value) {
                    _isRunning = value;
                    OnPropertyChanged("IsRunning");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        public Processor() {
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            _dataStorageManager = CoreSystem.Managers.Find(m => m.Name.Contains("DataStorageManager")) as DataStorageManager;
            _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;
            _extensionManager = CoreSystem.Managers.Find(m => m.Name.Contains("ExtensionManager")) as ExtensionManager;
            _nexuses = new List<AsyncNanoProcessor>();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool Start() {
            if (IsRunning) return true;
            bool initializeResult = InitializeOperations();
            if(initializeResult == true)
                StartOperations();

            if (initializeResult) {
                if (this.Started != null)
                    this.Started();
            } else {
                FinalizeOperations();
            }

            IsRunning = initializeResult;

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
                if (this.Stopped != null)
                    this.Stopped();
                IsRunning = false;
                return true;
            } else {
                IsRunning = false;
                return false;
            }
        }

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
        /// Initializes the operations.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        private bool InitializeOperations() {
            _isFinalize = false;
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                if (operation.Childs.Count < 1 || operation.Initialize() == false) {
                    Trace.WriteLine("Cannot start operation [" + operation.Name + "]!" 
                        + Environment.NewLine + "May the operation is empty or something happend while initializing it.", LogCategory.Warning);
                    return false;
                }
            }

            return true;
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
        /// Terminates the operations.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        private bool TerminateOperations() {
            bool result = true;
            foreach(AsyncNanoProcessor nanoProcessor in _nexuses) {
                nanoProcessor.Operation.PropertyChanged -= OperationPropertyChangedHandle;
                bool tmpResult = false;
                tmpResult = nanoProcessor.Stop();
                result = !tmpResult ? false : true;
            }
            return result;
        }

        /// <summary>
        /// Starts the operations.
        /// </summary>
        private void StartOperations() {
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                ListProperty triggerList = operation.GetProperty("Trigger") as ListProperty;
                string trigger = triggerList.Value.ToString();

                if (trigger != OperationTrigger.Continuous.GetDescription()) continue;
                    StartOperation(operation);
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
            
            if(nanoProcessor.Status != TaskStatus.Running)
                nanoProcessor.Start();
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
                Property linkedProperty = o.GetProperty("LinkedOperation");
                if (linkedProperty.ConnectedToUID == operation.UID)
                    connectedOperations.Add(o);
            }

            switch (status) {
                case PluginStatus.Failed:
                case PluginStatus.Finished:
                    if (_nexuses.Count > 0) {
                        AsyncNanoProcessor executionContext = _nexuses.Find(o => o != null && o.Operation == operation) as AsyncNanoProcessor;
                        if (executionContext != null && _nexuses.Contains(executionContext)) {
                            ListProperty triggerList = operation.GetProperty("Trigger") as ListProperty;
                            string trigger = triggerList.Value.ToString();

                            foreach (Operation o in connectedOperations) {
                                Property triggerProperty = o.GetProperty("Trigger");
                                if (triggerProperty.Value.ToString() == OperationTrigger.Finished.GetDescription()) {
                                    NanoProcessor nanoProcessor = new NanoProcessor(o, _dataStorageManager);
                                    nanoProcessor.Start();
                                }
                            }
                        } else
                            Trace.WriteLine("Unknown execution context! Careful this could end in memory leak!", LogCategory.Error);
                    }
                    _extensionManager.RunAll();
                    break;

                case PluginStatus.Started:
                    foreach (Operation o in connectedOperations) {
                        Property triggerProperty = o.GetProperty("Trigger");
                        if (triggerProperty.Value.ToString() == OperationTrigger.Started.GetDescription()) {
                            NanoProcessor nanoProcessor = new NanoProcessor(o, _dataStorageManager);
                            nanoProcessor.Start();
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
