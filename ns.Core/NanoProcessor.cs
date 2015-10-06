using ns.Base;
using ns.Base.Log;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Core {
    public class NanoProcessor : NotifyObject {

        private const int MAX_TIMEOUT = 10000;
        private Task _task;
        private bool _terminate = false;
        private Operation _operation;
        private bool _result = false;

        public TaskStatus Status {
            get {
                if (_task == null) return TaskStatus.Faulted;
                return _task.Status;
            }
        }

        public Operation Operation {
            get { return _operation; }
        }

        public bool Result {
            get { return _result; }
            private set {
                _result = value;
                OnPropertyChanged("Result");
            }
        }

        public NanoProcessor() { }

        public NanoProcessor(Operation operation) {
            _operation = operation;

            _task = new Task(new Action(() => {
                while (!_terminate) {
                    bool preResult = false;
                    bool postResult = false;
                    bool runResult = false;

                    try {
                        if ((preResult = _operation.PreRun()) == true) {
                            if((runResult = _operation.Run()) == false)
                                Trace.WriteLine("Run operation [" + _operation.Name + "] failed!", LogCategory.Error);
                        } else {
                            Trace.WriteLine("Prerun operation [" + _operation.Name + "] failed!", LogCategory.Error);
                        }

                        if((postResult = _operation.PostRun()) == false)
                            Trace.WriteLine("Postrun operation [" + _operation.Name + "] failed!", LogCategory.Error);

                    } catch(Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                        preResult = false;
                        runResult = false;
                        postResult = false;
                    }

                    Result = preResult && runResult && postResult;
                }
            }));
        }

        public bool Start() {
            if (_task == null) return false;
            _terminate = false;
            _task.Start();
            return true;
        }

        public bool Stop() {
            if (_task == null) return false;
            _terminate = true;
            return _task.Wait(MAX_TIMEOUT);
        }

        public bool Wait() {
            if (_task == null) return false;
            _task.Wait(MAX_TIMEOUT);
            return true;
        }
        
    }
}
