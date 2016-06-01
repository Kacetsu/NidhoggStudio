using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ns.Base.Extensions {

    public static class Marshal {

        public static T DeepClone<T>(this T source) {
            try {
                if (!typeof(T).IsSerializable) {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                // Don't serialize a null object, simply return the default for that object
                if (ReferenceEquals(source, null)) {
                    return default(T);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                object obj = null;

                using (stream) {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    obj = (T)formatter.Deserialize(stream);
                }

                return (T)obj;
            } catch (Exception ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }
            return default(T);
        }
    }
}