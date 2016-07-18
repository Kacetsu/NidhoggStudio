using System;
using System.Diagnostics;
using System.Reflection;

namespace ns.Base.Log {

    public class Trace {
        private static Lazy<Trace> _instance = new Lazy<Trace>(() => new Trace());
        private int _id;
        private TraceSource _traceSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trace"/> class.
        /// </summary>
        public Trace() {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null) {
                _traceSource = new TraceSource("TraceSource");
            } else {
                _traceSource = new TraceSource(entryAssembly.GetName().Name);
            }
            SourceSwitch sourcSwitch = new SourceSwitch("SourceSwitch", "All");
            _traceSource.Switch = sourcSwitch;
        }

        /// <summary>
        /// Gets the listeners.
        /// </summary>
        /// <value>
        /// The listeners.
        /// </value>
        public static TraceListenerCollection Listeners => _instance.Value._traceSource.Listeners;

        /// <summary>
        /// Write a trace line with message and category.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        public static void WriteLine(string message, TraceEventType category) {
            try {
#if DEBUG

                _instance.Value._traceSource.TraceData(category, _instance.Value._id, message);

#else
                if (category != TraceEventType.Verbose) {
                    _instance.Value._traceSource.TraceData(category, _instance.Value._id, message);
                }
#endif
                _instance.Value._id++;
                if (_instance.Value._id == int.MaxValue) {
                    _instance.Value._id = 0;
                }

                _instance.Value._traceSource.Flush();
            } catch (StackOverflowException ex) {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Write a trace libe with message, stack trace and category.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="category">The category.</param>
        public static void WriteLine(string message, string stackTrace, TraceEventType category) {
            WriteLine(message + Environment.NewLine + "Stack Trace: " + Environment.NewLine + stackTrace, category);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="category">The category.</param>
        public static void WriteLine(Exception exception, TraceEventType category) {
            WriteLine(exception.ToString(), category);
        }
    }
}