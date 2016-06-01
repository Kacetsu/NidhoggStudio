using ns.Core;
using System.Diagnostics;

namespace ns.Console {

    internal class Program {

        private static void Main(string[] args) {
            if (CoreSystem.Initialize(false) == false) {
                Base.Log.Trace.WriteLine("Fatal error while initializing CoreSystem!", TraceEventType.Error);
            }

            System.Console.ReadKey();
        }
    }
}