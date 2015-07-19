using ns.Base;
using ns.Base.Extensions;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ns.Core {
    /// <summary>
    /// Handles the use of all operations.
    /// </summary>
    public class Processor {

        private ProjectManager _projectManager;
        private DataStorageManager _dataStorageManager;
        private PropertyManager _propertyManager;
        private ExtensionManager _extensionManager;
        private List<ExecutionContext> _nexuses;
        private bool _isStoped = false;
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        public Processor() {
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            _dataStorageManager = CoreSystem.Managers.Find(m => m.Name.Contains("DataStorageManager")) as DataStorageManager;
            _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;
            _extensionManager = CoreSystem.Managers.Find(m => m.Name.Contains("ExtensionManager")) as ExtensionManager;
            _nexuses = new List<ExecutionContext>();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool Start() {
            _isStoped = false;
            bool initializeResult = InitializeOperations();
            if(initializeResult == true)
                StartOperations();

            if (initializeResult) {
                if (this.Started != null)
                    this.Started();
            }

            _isRunning = initializeResult;

            return initializeResult;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool Stop() {
            bool resultFinalize = false;
            bool resultTerminate = false;

            _isStoped = true;

            resultTerminate = TerminateOperations();
            resultFinalize = FinalizeOperations();

            _nexuses.Clear();

            if (resultFinalize && resultTerminate) {
                if (this.Stopped != null)
                    this.Stopped();
                _isRunning = false;
                return true;
            } else {
                _isRunning = false;
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
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                ExecutionContext context = _nexuses.Find(o => o != null && o.Plugin == operation) as ExecutionContext;
                if (context != null) {
                    if(context.Thread.Join(1000) == true)
                        if (operation.Finalize() == false)
                            return false;
                } else {
                    if (operation.Finalize() == false)
                        return false;
                }
            }
            _nexuses.Clear();
            return true;
        }

        /// <summary>
        /// Terminates the operations.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        private bool TerminateOperations() {
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                if (operation.Terminate() == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Starts the operations.
        /// </summary>
        private void StartOperations() {
            foreach (Operation operation in _projectManager.Configuration.Operations) {
                ListProperty triggerList = operation.GetProperty("Trigger") as ListProperty;
                string trigger = triggerList.Value.ToString();

                if (trigger != OperationTrigger.Continuous.GetDescription()) continue;
                StartOperation(operation, null);
            }
        }

        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        private void StartOperation(Operation operation, ExecutionContext context) {
            ExecutionContext executionContext = context;

            if (executionContext == null) {
                executionContext = new ExecutionContext();
                executionContext.Plugin = operation;
            }

            Thread t = new Thread(new ThreadStart(() => {
                if (_isFinalize) return;

                operation.StatusChangedEvent += OperationStatusChangedHandle;

                while (!_isStoped) {
                    if (operation.PreRun() == true) {
                        if ((executionContext.Result = operation.Run()) == true) {
                            if (!operation.PostRun())
                                Trace.WriteLine("Operation post run failed!", LogCategory.Error);
                        } else {
                            Trace.WriteLine("Operation run failed!", LogCategory.Error);
                        }
                    } else {
                        Trace.WriteLine("Operation pre run failed!", LogCategory.Error);
                    }

                    ListProperty triggerList = operation.GetProperty("Trigger") as ListProperty;
                    string trigger = triggerList.Value.ToString();

                    if (trigger != OperationTrigger.Continuous.GetDescription()) break;
                }

                operation.StatusChangedEvent -= OperationStatusChangedHandle;
            }));
            executionContext.Thread = t;
            _nexuses.Add(executionContext);
            t.Start();
        }

        /// <summary>
        /// Operations the status changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Base.Event.PluginStatusChangedEventArgs"/> instance containing the event data.</param>
        private void OperationStatusChangedHandle(object sender, Base.Event.PluginStatusChangedEventArgs e) {
            // TODO: Start Operations that are configurated to start after this Operation did finished.
            // TODO: Start Operations that are configurated to start after this Operation did started.

            Operation operation = sender as Operation;
            PluginStatus status = e.Status;

            if (_isFinalize == true)
                return;

            List<Operation> connectedOperations = new List<Operation>();

            foreach (Operation o in _projectManager.Configuration.Operations) {
                Property linkedProperty = o.GetProperty("LinkedOperation");
                if (linkedProperty.ConnectedToUID == operation.UID)
                    connectedOperations.Add(o);
            }

            switch (status) {
                case PluginStatus.Finished:
                    AddOperationContextToDataStorage(operation);
                    if (_nexuses.Count > 0) {
                        ExecutionContext executionContext = _nexuses.Find(o => o != null && o.Plugin == operation) as ExecutionContext;
                        if (executionContext != null && _nexuses.Contains(executionContext)) {
                            ListProperty triggerList = operation.GetProperty("Trigger") as ListProperty;
                            string trigger = triggerList.Value.ToString();

                            if (_isStoped || trigger != OperationTrigger.Continuous.GetDescription())
                                _nexuses.Remove(executionContext);

                            foreach (Operation o in connectedOperations) {
                                Property triggerProperty = o.GetProperty("Trigger");
                                if (triggerProperty.Value.ToString() == OperationTrigger.Finished.GetDescription()) {
                                    if (o.PreRun()) {
                                        o.Run();
                                        o.PostRun();
                                    }
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
                            if (o.PreRun()) {
                                o.Run();
                                o.PostRun();
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Adds the operation context to data storage.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void AddOperationContextToDataStorage(Node parent) {
            foreach (Node child in parent.Childs) {
                if (child is Property) {
                    Property property = child as Property;
                    if (property.IsOutput && property.IsMonitored) {
                        _dataStorageManager.Add(property);
                    }
                }
                AddOperationContextToDataStorage(child);
            }
        }
    }
}
