using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace DualMonitor
{
    public class AutostartUtil
    {
        private const string RunLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string KeyName = "dualmonitor";

        /// <summary>
        /// Sets the autostart value for the assembly.
        /// </summary>
        /// <param name="keyName">Registry Key Name</param>
        /// <param name="assemblyLocation">Assembly location (e.g. Assembly.GetExecutingAssembly().Location)</param>
        public static void SetAutoStart()
        {
            string assemblyLocation = Application.ExecutablePath;
            RegistryProxy.SetKey(RunLocation, KeyName, assemblyLocation);            
        }

        /// <summary>
        /// Returns whether auto start is enabled.
        /// </summary>
        /// <param name="keyName">Registry Key Name</param>
        /// <param name="assemblyLocation">Assembly location (e.g. Assembly.GetExecutingAssembly().Location)</param>
        public static bool IsAutoStartEnabled()
        {
            string value = RegistryProxy.GetKey<string>(RunLocation, KeyName);
            if (value == null)
                return false;

            return string.Equals(value, Application.ExecutablePath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Unsets the autostart value for the assembly.
        /// </summary>
        /// <param name="keyName">Registry Key Name</param>
        public static void UnSetAutoStart()
        {
            RegistryProxy.DeleteKey(RunLocation, KeyName);            
        }
    }
}
