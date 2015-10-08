using ns.Base.Plugins;
using System;
using System.Threading.Tasks;

namespace ns.Core {
    /// <summary>
    /// Runs the Operation on a async way.
    /// </summary>
    public class AsyncNanoProcessor : NanoProcessor {
        private const int MAX_TIMEOUT = 10000;
        private Task _task;
        private bool _terminate = false;

        /// <summary>
        /// Gets the Status of the async execution.
        /// </summary>
        public TaskStatus Status {
            get {
                if (_task == null) return TaskStatus.Faulted;
                return _task.Status;
            }
        }

        /// <summary>
        /// Base Constructor.
        /// </summary>
        /// <param name="operation">The Operation to run.</param>
        public AsyncNanoProcessor(Operation operation) : base(operation) {
            _task = new Task(new Action(() => {
                while (!_terminate) {
                    Execute();
                }
            }));
        }

        /// <summary>
        /// Starts the async execution.
        /// </summary>
        /// <returns>true if successful.</returns>
        public override bool Start() {
            if (_task == null) return false;
            _terminate = false;
            _task.Start();
            return true;
        }

        /// <summary>
        /// Stops the async execution.
        /// This will block the Thread till the execution ends.
        /// </summary>
        /// <returns>true if successful.</returns>
        public bool Stop() {
            if (_task == null) return false;
            _terminate = true;
            return _task.Wait(MAX_TIMEOUT);
        }
        
        /// <summary>
        /// Waits till the Operation ends.
        /// This will block the Thread till the executions ends.
        /// </summary>
        /// <returns>true if successful.</returns>
        public bool Wait() {
            if (_task == null) return false;
            _task.Wait(MAX_TIMEOUT);
            return true;
        }
        
    }
}
