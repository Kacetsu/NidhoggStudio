using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ns.GUI.WPF {
    public sealed class NativeMethods {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(
            IntPtr hWnd,
            UInt32 msg,
            IntPtr wParam,
            IntPtr lParam);
    }
}
