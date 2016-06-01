using System.Diagnostics;

namespace ns.Base.Log {

    public class LogData {
        public string Timestamp { get; private set; }
        public string Message { get; private set; }
        public TraceEventType Category { get; private set; }

        public LogData() {
        }

        public LogData(string timestamp, string message, TraceEventType category) {
            Timestamp = timestamp;
            Message = message;
            Category = category;
        }
    }
}