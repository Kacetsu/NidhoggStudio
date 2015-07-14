namespace ns.Base.Log {
    public class LogData {
        public string Timestamp { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }

        public LogData () { }

        public LogData (string timestamp, string message, string category) {
            Timestamp = timestamp;
            Message = message;
            Category = category;
        }
    }
}
