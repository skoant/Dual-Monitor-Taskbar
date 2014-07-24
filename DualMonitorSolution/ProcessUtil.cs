using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using DualMonitor.Win32;
using System.Management;

namespace DualMonitor
{
    public class ProcessUtil
    {
        public static Process GetProcessByWindowHandle(IntPtr hwnd)
        {
            uint pid;
            Native.GetWindowThreadProcessId(hwnd, out pid);

            return Process.GetProcessById((int)pid);
        }

        public static string GetProcessPathByWindowHandle(IntPtr hwnd)
        {
            try
            {
                var p = GetProcessByWindowHandle(hwnd);
                if (p == null || p.MainModule == null) return null;
                return p.MainModule.FileName;
            }
            catch
            {
                return null;
            }
        }

        public static int LaunchProcess(string path, string arguments)
        {
            Process p = Process.Start(new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                UseShellExecute = true
            });

            return p.Id;
        }

        public static string GetCommandLineArguments(IntPtr hwnd)
        {
            try
            {
                var p = GetProcessByWindowHandle(hwnd);
                return GetCommandLineArguments(p.MainModule.ModuleName);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetCommandLineArguments(string processName)
        {
            try
            {
                string arguments = "";
                string wmiQuery = string.Format("select CommandLine from Win32_Process where Name='{0}'", processName);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
                ManagementObjectCollection retObjectCollection = searcher.Get();
                foreach (var item in retObjectCollection)
                {
                    if (item["CommandLine"] != null)
                    {
                        arguments = item["CommandLine"].ToString();

                        if (arguments[0] == '"')
                        {
                            arguments = arguments.Substring(arguments.IndexOf("\"", 1) + 1);
                        }
                        else
                        {
                            arguments = arguments.Substring(arguments.IndexOf(" ", 1) + 1);
                        }
                        break;
                    }
                }

                return arguments;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
