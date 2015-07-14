using ns.Base.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Extensions {
    public static class Marshal {
        public static T DeepClone<T>(this T source) {
            if (!typeof(T).IsSerializable) {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null)) {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            object obj = null;
            using (stream) {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                try {
                    obj = (T)formatter.Deserialize(stream);
                } catch (Exception ex) {
                    Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                }
            }
            return (T)obj;
        }
    }
}
