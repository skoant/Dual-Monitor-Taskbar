using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Entities;
using DualMonitor.Win32;
using System.Windows.Forms;

namespace DualMonitor
{
    /// <summary>
    /// Control the system menu of an external window
    /// </summary>
    public class SystemMenuProxy
    {
        public static bool IsOpening;
        private static IntPtr _tooltipWindowHandle;

        public static void BeginOpenSystemMenu(Win32Window window, IntPtr tooltipWindowHandle)
        {
            IsOpening = true;
            _tooltipWindowHandle = tooltipWindowHandle;
            window.ActivateWindow(false);
        }

        public static void EndOpenSystemMenu(Win32Window window)
        {            
            IntPtr hmenu = Native.GetSystemMenu(window.Handle, false);

            int command = Native.TrackPopupMenu(hmenu, Native.TPM.TPM_LEFTALIGN | Native.TPM.TPM_BOTTOMALIGN | Native.TPM.TPM_RETURNCMD
                | Native.TPM.TPM_LEFTBUTTON | Native.TPM.TPM_RIGHTBUTTON | Native.TPM.TPM_NOANIMATION,
               Cursor.Position.X, Cursor.Position.Y, 0, _tooltipWindowHandle, IntPtr.Zero);            

            if (command != 0)
            {
                Native.SendMessage(window.Handle, (uint)Native.WindowMessage.WM_SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
            }
            else
            {
                Native.SendMessage(_tooltipWindowHandle, (uint)Native.WindowMessage.WM_NULL, IntPtr.Zero, IntPtr.Zero);    
            }

            IsOpening = false;
        }
    }
}
