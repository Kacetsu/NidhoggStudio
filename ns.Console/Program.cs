using ns.Base.Log;
using ns.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Console {
    class Program {
        static void Main(string[] args) {
            if (CoreSystem.Initialize(false) == false) {
                Trace.WriteLine("Fatal error while initializing CoreSystem!", LogCategory.Error);
            }


            System.Console.ReadKey();
        }
    }
}
