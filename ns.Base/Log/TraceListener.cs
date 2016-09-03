using ns.Base.Event;
using ns.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace ns.Base.Log {

    public class TraceListener : TextWriterTraceListener {
        private const uint MAX_BUFFERED_LOGENTRIES = 100;

        private List<string> _categoriesToLog = new List<string>();
        private string _directory;
        private List<LogData> _logEntries = new List<LogData>();
        private string _logFile;
        private DateTime _logFileDate;
        private XmlWriter _xmlWriter = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListener"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="daysToKeep">The days to keep.</param>
        public TraceListener(string directory, int daysToKeep) : base() {
            _directory = directory;
            _logFileDate = DateTime.Now;

            if (string.IsNullOrEmpty(_directory) == false) {
                if (Directory.Exists(_directory) == false) {
                    Directory.CreateDirectory(_directory);
                }
            }

            string filename = string.Format(CultureInfo.CurrentCulture, "{0:0000}{1:00}{2:00}_{3:00}{4:00}{5:00}_logFragments.xml", _logFileDate.Year, _logFileDate.Month, _logFileDate.Day, _logFileDate.Hour, _logFileDate.Minute, _logFileDate.Second);
            _logFile = _directory + Path.DirectorySeparatorChar + filename;

            DeleteOldLogFiles(daysToKeep);

            SetLoggingCategoties(new List<string>(new string[] { TraceEventType.Warning.GetDescription(), TraceEventType.Error.GetDescription() }));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.NewLineChars = "\n";
            settings.CloseOutput = true;
            settings.Indent = true;

            _xmlWriter = XmlWriter.Create(_logFile, settings);
            _xmlWriter.WriteStartElement("LOG");
        }

        ~TraceListener() {
            Dispose(false);
        }

        private delegate void TraceListenerEvent(object sender, TraceListenerEventArgs e);

        /// <summary>
        /// Occurs when [trace listener message added].
        /// </summary>
        private event TraceListenerEvent TraceListenerMessageAdded = delegate { };

        /// <summary>
        /// Gets the log entries.
        /// </summary>
        /// <value>
        /// The log entries.
        /// </value>
        public ICollection<LogData> LogEntries => _logEntries;

        /// <summary>
        /// Gets the log file date.
        /// </summary>
        /// <value>
        /// The log file date.
        /// </value>
        public DateTime LogFileDate => _logFileDate;

        /// <summary>
        /// Closes the <see cref="P:System.Diagnostics.TextWriterTraceListener.Writer" /> so that it no longer receives tracing or debugging output.
        /// </summary>
        public override void Close() {
            _xmlWriter.WriteEndElement();
            base.Close();
            CloseWriter();
        }

        /// <summary>
        /// Closes the writer.
        /// </summary>
        public void CloseWriter() {
            if (_xmlWriter != null) {
                _xmlWriter.Close();
            }
        }

        /// <summary>
        /// Sets the logging categoties.
        /// </summary>
        /// <param name="categories">The categories.</param>
        public void SetLoggingCategoties(ICollection<string> categories) {
            _categoriesToLog.Clear();
            _categoriesToLog.AddRange(categories);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data) {
            if (data == null) throw new ArgumentNullException(nameof(data));

            WriteLine(data.ToString(), eventType);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data) {
            if (data == null) throw new ArgumentNullException(nameof(data));

            foreach (object obj in data) {
                WriteLine(obj.ToString(), eventType);
            }
        }

        /// <summary>
        /// Writes a category name and a message to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener" /> class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public void WriteLine(string message, TraceEventType category) {
            try {
                lock (_categoriesToLog) {
                    string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff", CultureInfo.CurrentCulture);

                    if (LogEntries.Count > MAX_BUFFERED_LOGENTRIES) {
                        LogData logData = LogEntries.First();
                        LogEntries.Remove(logData);
                    }
                    LogEntries.Add(new LogData(timestamp, message, category));

                    TraceListenerMessageAdded(this, new TraceListenerEventArgs(timestamp, message, category));

                    if (_categoriesToLog.Contains(category.GetDescription()) == true) {
                        try {
                            _xmlWriter.WriteStartElement("ENTRY");
                            _xmlWriter.WriteAttributeString("timestamp", timestamp);
                            _xmlWriter.WriteAttributeString("thread", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.CurrentCulture));
                            _xmlWriter.WriteAttributeString("category", category.GetDescription());
                            _xmlWriter.WriteRaw(message);
                            _xmlWriter.WriteEndElement();
                        } catch (InvalidOperationException ex) {
                            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Fatal error: {0}\n{1}", ex.Message, ex.StackTrace));
                        }
                    }
                }
            } catch (StackOverflowException ex) {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Deletes the old log files.
        /// </summary>
        /// <param name="daysToKeep">The days to keep.</param>
        private void DeleteOldLogFiles(int daysToKeep) {
            //delete all the logfiles and exception images older than "logFilesKeepdate" days
            //ATTENTION: we add -logFilesKeepdate days
            DateTime keepdate = _logFileDate.AddDays(-daysToKeep);
            DirectoryInfo dir = new DirectoryInfo(_directory);

            //delete all exception images older than keepdate (filename: Exception_yyyymmdd_hhmmss_*.png)
            Regex regexImages = new Regex("^Exception_\\d{8}_\\d{6}.*\\.(bmp|png)$");
            foreach (System.IO.FileInfo file in dir.GetFiles("Exception*")) {
                if (regexImages.IsMatch(file.Name) == true) {
                    //delete old files
                    int year = Convert.ToInt32(file.Name.Substring(10, 4), CultureInfo.CurrentCulture);
                    int month = Convert.ToInt32(file.Name.Substring(14, 2), CultureInfo.CurrentCulture);
                    int day = Convert.ToInt32(file.Name.Substring(16, 2), CultureInfo.CurrentCulture);
                    DateTime fileDate = new DateTime(year, month, day);
                    if (fileDate <= keepdate) {
                        file.Delete();
                    }
                }
            }

            //actualice the logfiles.xml
            string logfileListName = _directory + Path.DirectorySeparatorChar + "logfiles.xml";
            if (File.Exists(logfileListName) == true)
                File.Delete(logfileListName);
            XmlDocument logfiles = new XmlDocument();
            logfiles.AppendChild(logfiles.CreateElement("logfiles"));
            XmlNode root = logfiles.DocumentElement;

            //read directory and search for log fragments
            Regex regex = new Regex("^\\d{8}_\\d{6}_logFragments\\.xml$");
            foreach (System.IO.FileInfo file in dir.GetFiles("*_logFragments.xml")) {
                if (regex.IsMatch(file.Name) == true) {
                    //delete old logfiles
                    int year = Convert.ToInt32(file.Name.Substring(0, 4), CultureInfo.CurrentCulture);
                    int month = Convert.ToInt32(file.Name.Substring(4, 2), CultureInfo.CurrentCulture);
                    int day = Convert.ToInt32(file.Name.Substring(6, 2), CultureInfo.CurrentCulture);
                    DateTime fileDate = new DateTime(year, month, day);
                    if (fileDate <= keepdate) {
                        file.Delete();
                        continue;
                    }
                    //add the logFragments
                    XmlElement elem = logfiles.CreateElement("logfile");
                    elem.InnerText = file.Name;
                    root.AppendChild(elem);
                }
            }
            logfiles.Save(logfileListName);
        }
    }
}