using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.Base.Extensions;

namespace ns.Base.Log {
    public static class Trace {
        /// <summary>
        /// Write a trace line with message and category.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        public static void WriteLine(string message, LogCategory category) {
#if DEBUG
            try {
                System.Diagnostics.Trace.WriteLine(message, category.GetDescription());
            } catch (StackOverflowException ex) {
                Console.WriteLine(ex);
            }
#else            
            if(category != LogCategory.Debug)
                System.Diagnostics.Trace.WriteLine(message, category.GetDescription());
#endif
        }

        /// <summary>
        /// Write a trace libe with message, stack trace and category.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="category">The category.</param>
        public static void WriteLine(string message, string stackTrace, LogCategory category) {
            Trace.WriteLine(message + Environment.NewLine + "Stack Trace: " + Environment.NewLine + stackTrace, category);
        }

    }
}
