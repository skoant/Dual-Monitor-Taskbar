using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Win32;
using DualMonitor.Entities;
using System.Windows.Forms;
using System.IO;

namespace DualMonitor
{
    class RunningTasksProxy
    {
        private static Win32Window GetRunningApplicationsWnd()
        {
            return Win32Window.FromClassName("Shell_TrayWnd")
                .FindWindow("ReBarWindow32")
                .FindWindow("MSTaskSwWClass");
        }

        private static bool Is64Bits()
        {
            return IntPtr.Size == 8;
        }

        /// <summary>
        /// Inject an unmanaged dll into explorer and hook WndProc
        /// </summary>
        public static void HookRunningApps(IntPtr handle)
        {
            var runningAppsWnd = GetRunningApplicationsWnd();
            if (runningAppsWnd.Handle == IntPtr.Zero) return;

            uint pid;
            uint threadId = Native.GetWindowThreadProcessId(runningAppsWnd.Handle, out pid);

            // detect 64 bits OS - explorer must be running in 64 bits
            string suffix = Is64Bits() ? "64" : "32";

            string dll = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ExplorerHook" + suffix + ".dll");

            IntPtr hModule = Native.LoadLibrary(dll);
            UIntPtr hProc = Native.GetProcAddress(hModule, "GetMessageProc");

            IntPtr hr = Native.SetWindowsHookEx(Native.HookType.WH_GETMESSAGE, hProc, hModule, threadId);
            hr = Native.SetWindowsHookEx(Native.HookType.WH_CALLWNDPROC, hProc, hModule, threadId);

            Native.PostMessage(runningAppsWnd.Handle, (uint)Native.WindowMessage.WM_APP + 1600, handle, IntPtr.Zero);
        }
    }
}
