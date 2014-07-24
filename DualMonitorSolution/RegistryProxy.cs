using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace DualMonitor
{
    public class RegistryProxy
    {
        public static T GetKey<T>(string path, string value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path);
            if (key == null) return default(T);


            return SafeGetKey<T>(key.GetValue(value));
        }

        public static void SetKey(string RunLocation, string KeyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RunLocation);

            key.SetValue(KeyName, assemblyLocation);
        }

        public static void DeleteKey(string RunLocation, string KeyName)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RunLocation);
            if (key.GetValueNames().Contains(KeyName))
            {
                key.DeleteValue(KeyName);
            }
        }

        private static T SafeGetKey<T>(object p)
        {
            if (p is T)
            {
                return (T)p;
            }

            return default(T);
        }
    }
}
