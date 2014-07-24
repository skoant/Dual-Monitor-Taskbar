using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Entities;
using DualMonitor.Win32;

namespace DualMonitor
{
    public class ExtendedTaskbarMenu
    {
        private static Win32Window GetUserToolbar()
        {
            return Win32Window.FromClassName("Shell_TrayWnd")
                .FindWindow("ReBarWindow32");
                //.FindWindow("MSTaskSwWClass")
                //.FindWindow("MSTaskListWClass");
        }

        public static void Initialize()
        {
        }
    }
}
