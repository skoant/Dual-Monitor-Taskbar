using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Entities;
using DualMonitor.Win32;
using System.Drawing;
using DualMonitor.VisualStyle;

namespace DualMonitor
{
    /// <summary>
    /// Workaround for clock panel
    /// </summary>
    class ClockPanelProxy
    {
        public static void ShowClockPanel()
        {
            Win32Window clock = Win32Window.FromClassName("Shell_TrayWnd")
                .FindWindow("TrayNotifyWnd")
                .FindWindow("TrayClockWClass");

            if (clock.Handle == IntPtr.Zero) return;

            Rectangle r = clock.Bounds;

            // workaround till I find a better solution :)
            // if the main taskbar is hidden (autohide) this approach has some problems...
            Point original = Cursor.Position;
            Cursor.Position = new Point(r.Left + 5, r.Top + 5);
            SendInputApi.DoMouse(SendInputApi.MouseEventFlags.MOUSEEVENTF_LEFTDOWN, new Point(r.Left + 5, r.Top + 5));
            SendInputApi.DoMouse(SendInputApi.MouseEventFlags.MOUSEEVENTF_LEFTUP, new Point(r.Left + 5, r.Top + 5));
            Cursor.Position = original;
        }

        public static void MoveClockPanel(Win32Window window, Screen activeScreen)
        {
            Native.RECT r;
            Native.GetWindowRect(window.Handle, out r);

            int width = r.right - r.left;
            int height = r.bottom - r.top;

            int dx = AeroDecorator.Instance.IsDwmCompositionEnabled ? 12 : 0;
            int dy = AeroDecorator.Instance.IsDwmCompositionEnabled ? 11 : 0;

            var taskbarLocation = TaskbarPropertiesManager.Instance.Properties.GetTaskbarLocation(activeScreen.DeviceName);

            switch (taskbarLocation)
            {
                case Native.ABEdge.Bottom:
                    Native.MoveWindow(window.Handle, activeScreen.WorkingArea.Right - width - dx, activeScreen.WorkingArea.Bottom - height - dy, width, height, true);
                    break;
                case Native.ABEdge.Left:
                    Native.MoveWindow(window.Handle, activeScreen.WorkingArea.Left + dx, activeScreen.WorkingArea.Bottom - height - dy, width, height, true);
                    break;
                case Native.ABEdge.Right:
                    Native.MoveWindow(window.Handle, activeScreen.WorkingArea.Right - width - dx, activeScreen.WorkingArea.Bottom - height - dy, width, height, true);
                    break;
                case Native.ABEdge.Top:
                    Native.MoveWindow(window.Handle, activeScreen.WorkingArea.Right - width - dx, activeScreen.WorkingArea.Top + dy, width, height, true);
                    break;
            }
        }
    }
}
