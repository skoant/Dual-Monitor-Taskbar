using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Win32;
using DualMonitor.Entities;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;

namespace DualMonitor
{
    public class NotificationAreaProxy
    {
        private static Win32Window GetUserToolbar()
        {
            return Win32Window.FromClassName("Shell_TrayWnd")
                .FindWindow("TrayNotifyWnd")
                .FindWindow("SysPager")
                .FindWindow("ToolbarWindow32");
        }

        private static bool Is64Bits()
        {
            return IntPtr.Size == 8;
        }

        /// <summary>
        /// Inject an unmanaged dll into explorer and hook WndProc
        /// </summary>
        public static void HookNotificationArea(IntPtr handle)
        {
            var toolbarWindow = GetUserToolbar();
            if (toolbarWindow.Handle == IntPtr.Zero) return;

            uint pid;
            uint threadId = Native.GetWindowThreadProcessId(toolbarWindow.Handle, out pid);

            // detect 64 bits OS - explorer must be running in 64 bits
            string suffix = Is64Bits() ? "64" : "32";

            string dll = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ExplorerHook" + suffix + ".dll");

            IntPtr hModule = Native.LoadLibrary(dll);
            UIntPtr hProc = Native.GetProcAddress(hModule, "CallWndProc");

            IntPtr hr = Native.SetWindowsHookEx(Native.HookType.WH_CALLWNDPROC, hProc, hModule, threadId);

            Native.SendMessage(toolbarWindow.Handle, (uint)Native.WindowMessage.WM_APP + 1514, handle, IntPtr.Zero);
        }

        /// <summary>
        /// Ask explorer to paint an icon for us
        /// </summary>
        public static void PaintIcon(int index, int x, int y, IntPtr window)
        {
            var toolbarWindow = GetUserToolbar();
            if (toolbarWindow.Handle == IntPtr.Zero) return;

            int param = (y << 8) | x;
            param = (param << 8) | index;

            Native.SendMessage(toolbarWindow.Handle, (uint)Native.WindowMessage.WM_APP + 1515, (IntPtr)param, window);
        }

        /// <summary>
        /// Get information about all icons
        /// </summary>
        public static List<NotificationIconInfo> GetVisibleIcons()
        {
            try
            {
                var toolbarWindow = GetUserToolbar();
                var result = new List<NotificationIconInfo>();
                if (toolbarWindow.Handle == IntPtr.Zero) return result;

                int count = (int)Native.SendMessage(toolbarWindow.Handle, (uint)Native.WindowMessage.TB_BUTTONCOUNT, IntPtr.Zero, IntPtr.Zero);
                if (count == 0) return result;

                uint pid;
                Native.GetWindowThreadProcessId(toolbarWindow.Handle, out pid);

                bool hr;
                IntPtr hproc = IPCNative.OpenProcess(IPCNative.ProcessAccess.AllAccess, false, (int)pid);

                for (int i = 0; i < count; i++)
                {
                    int sizeOfTbButton = Marshal.SizeOf(Is64Bits() ? typeof(Native.TBBUTTON64) : typeof(Native.TBBUTTON32));

                    // Allocate memory for TBBUTTON structure
                    IntPtr pTBBUTTON = IPCNative.VirtualAllocEx(hproc, IntPtr.Zero, (uint)sizeOfTbButton,
                        IPCNative.AllocationType.Commit, IPCNative.MemoryProtection.ReadWrite);

                    // Ask explorer.exe to fill the structure we just allocated
                    Native.SendMessage(toolbarWindow.Handle, (uint)Native.WindowMessage.TB_GETBUTTON, new IntPtr(i), pTBBUTTON);

                    // Read the structure from explorer.exe's memory
                    int read;
                    object obj;
                    if (Is64Bits())
                    {
                        obj = new Native.TBBUTTON64();
                    }
                    else
                    {
                        obj = new Native.TBBUTTON32();
                    }
                    hr = IPCNative.ReadProcessMemory(hproc, pTBBUTTON, obj, sizeOfTbButton, out read);
                    Native.TBBUTTON32 tbbutton = ConvertToTBButton32(obj);

                    hr = IPCNative.VirtualFreeEx(hproc, pTBBUTTON, sizeOfTbButton, IPCNative.FreeType.Decommit | IPCNative.FreeType.Release);

                    // Get data associated with icon
                    IntPtr data = new IntPtr((int)tbbutton.dwData);

                    obj = new Native.SysTrayData();
                    IPCNative.ReadProcessMemory(hproc, data, obj, Marshal.SizeOf(typeof(Native.SysTrayData)), out read);
                    Native.SysTrayData trayData = (Native.SysTrayData)obj;

                    FixTrayDataAnyCPU(ref trayData);

                    if (trayData.wParam < 0) continue;

                    // Get tooltip length
                    int size = (int)Native.SendMessage(toolbarWindow.Handle, (uint)Native.WindowMessage.TB_GETBUTTONTEXT, (IntPtr)tbbutton.idCommand, IntPtr.Zero);
                    size *= 2; // because it is unicode

                    // Alloc memory for explorer.exe to write tooltip to
                    IntPtr pText = IPCNative.VirtualAllocEx(hproc, IntPtr.Zero, (uint)size, IPCNative.AllocationType.Commit, IPCNative.MemoryProtection.ReadWrite);
                    Native.SendMessage(toolbarWindow.Handle, (uint)Native.WindowMessage.TB_GETBUTTONTEXT, (IntPtr)tbbutton.idCommand, pText);

                    // Read tooltip from memory
                    byte[] objstr = new byte[size];
                    IPCNative.ReadProcessMemory(hproc, pText, objstr, size, out read);

                    hr = IPCNative.VirtualFreeEx(hproc, pText, size, IPCNative.FreeType.Decommit | IPCNative.FreeType.Release);

                    string tooltip = UnicodeEncoding.Unicode.GetString(objstr);

                    NotificationIconInfo icon = new NotificationIconInfo
                    {
                        BitmapIndex = tbbutton.iBitmap,
                        DataIdentifier = tbbutton.dwData,
                        Data = trayData,
                        Tooltip = tooltip
                    };

                    result.Add(icon);
                }

                hr = IPCNative.CloseHandle(hproc);
                
                return result;
            }
            catch (OverflowException)
            {
                return new List<NotificationIconInfo>();
            }
        }

        private static void FixTrayDataAnyCPU(ref Native.SysTrayData trayData)
        {
            if (!Is64Bits())
            {
                trayData.nMsg = (uint)trayData.wParam;
                trayData.wParam = trayData.uID;
            }
        }

        private static Native.TBBUTTON32 ConvertToTBButton32(object obj)
        {
            if (obj is Native.TBBUTTON32) return (Native.TBBUTTON32)obj;

            Native.TBBUTTON64 tbb64 = (Native.TBBUTTON64)obj;
            return new Native.TBBUTTON32
            {
                dwData = tbb64.dwData,
                fsState = tbb64.fsState,
                fsStyle = tbb64.fsStyle,
                iBitmap = tbb64.iBitmap,
                idCommand = tbb64.idCommand,
                iString = tbb64.iString
            };
        }

        /// <summary>
        /// Get number of user icons
        /// </summary>
        public static int GetVisibleIconsCount()
        {
            var toolbarWindow = GetUserToolbar();
            if (toolbarWindow.Handle == IntPtr.Zero) return 0;

            int count = (int)Native.SendMessage(toolbarWindow.Handle, (uint)Native.WindowMessage.TB_BUTTONCOUNT, IntPtr.Zero, IntPtr.Zero);
            return count;
        }

        /// <summary>
        /// Left click on notification icon
        /// </summary>
        public static void LeftClickIcon(Native.SysTrayData trayData)
        {
            int y = Cursor.Position.Y;
            int x = Cursor.Position.X;
            int pos = (y << 16) | x;

            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)pos, (IntPtr)((trayData.wParam << 16) | (int)Native.WindowMessage.WM_LBUTTONDOWN));
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)trayData.wParam, (IntPtr)Native.WindowMessage.WM_LBUTTONDOWN);
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)trayData.wParam, (IntPtr)Native.WindowMessage.WM_LBUTTONUP);
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)trayData.wParam, (IntPtr)0x400);
        }

        /// <summary>
        /// Right click on notification icon - context menu
        /// </summary>
        public static void RightClickIcon(Native.SysTrayData trayData)
        {
            int y = Cursor.Position.Y;
            int x = Cursor.Position.X;
            int pos = (y << 16) | x;

            // context menu
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)trayData.wParam, (IntPtr)Native.WindowMessage.WM_RBUTTONDOWN);
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)trayData.wParam, (IntPtr)Native.WindowMessage.WM_RBUTTONUP);
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)pos, (IntPtr)((trayData.wParam << 16) | 0x7B));
        }

        /// <summary>
        /// Double click on notification icon
        /// </summary>
        public static void DefaultAction(Native.SysTrayData trayData)
        {
            int y = Cursor.Position.Y;
            int x = Cursor.Position.X;
            int pos = (y << 16) | x;

            // double click
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)pos, (IntPtr)((trayData.wParam << 16) | (int)Native.WindowMessage.WM_LBUTTONDBLCLK));
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)trayData.wParam, (IntPtr)(int)Native.WindowMessage.WM_LBUTTONDBLCLK);
            Native.SendMessage((IntPtr)(trayData.hwnd), trayData.nMsg, (IntPtr)trayData.wParam, (IntPtr)0x200);
        }

        /// <summary>
        /// Check if there are any hidden icons
        /// </summary>
        public static bool IsShowMoreButtonVisible()
        {
            Win32Window btn = Win32Window.FromClassName("Shell_TrayWnd")
                 .FindWindow("TrayNotifyWnd")
                 .FindWindow("Button");

            if (btn.Handle == IntPtr.Zero) return false;

            int style = Native.GetWindowLong(btn.Handle, (int)Native.GWL_STYLE);

            return (style & (int)Native.WindowStyles.WS_VISIBLE) == (int)Native.WindowStyles.WS_VISIBLE;
        }

        /// <summary>
        /// Show the hidden icons window at specified position
        /// </summary>
        public static void ShowMoreIcons(bool visible, int x, int y, AnchorStyles anchor)
        {
            var window = Win32Window.FromClassName("NotifyIconOverflowWindow");
            Rectangle r = window.Bounds;

            switch (anchor)
            {
                case AnchorStyles.Bottom:
                    x -= r.Width / 2;
                    y -= r.Height;
                    break;
                case AnchorStyles.Left:
                    y -= r.Height / 2;
                    break;
                case AnchorStyles.Right:
                    y -= r.Height / 2;
                    x -= r.Width;
                    break;
                case AnchorStyles.Top:
                    x -= r.Width / 2;
                    break;
            }

            Native.SetWindowPos(window.Handle, IntPtr.Zero, x, y, r.Width, r.Height, 
                visible ? Native.SetWindowPosFlags.ShowWindow : Native.SetWindowPosFlags.HideWindow);
        }

    }
}
