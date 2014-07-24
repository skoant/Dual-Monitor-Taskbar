using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace DualMonitor.Win32
{
    public class Native
    {
        #region Win32 Constants
        public const string TaskbarClassName = "Shell_TrayWnd";

        public const uint WINEVENT_OUTOFCONTEXT = 0;
        public const uint EVENT_SYSTEM_FOREGROUND = 3;
        public const uint EVENT_SYSTEM_MOVESIZEEND = 0xB;
        public const uint EVENT_OBJECT_CREATE = 0x00008000;
        public const uint EVENT_OBJECT_DESTROY = 0x00008001;
        public const uint EVENT_OBJECT_VALUECHANGE = 0x0000800c;
        public const uint EVENT_SYSTEM_MINIMIZESTART = 22;
        public const uint EVENT_SYSTEM_LOCATIONCHANGE = 0x0000800b;
        public const uint EVENT_OBJECT_SHOW = 0x00008002;
        public const uint EVENT_OBJECT_HIDE = 0x8003;
        public const uint EVENT_SYSTEM_SWITCHSTART = 0x0014;
        public const uint EVENT_SYSTEM_SWITCHEND = 0x0015;

        public const int ShowWinCmdNormal = 1;
        public const int ShowWinCmdMinimized = 2;
        public const int ShowWinCmdShow = 5;
        public const int ShowWinCmdRestore = 9;

        public const int HSHELL_REDRAW = 6;
        public const int HSHELL_HIGHBIT = 0x8000;
        public const int HSHELL_FLASH = HSHELL_REDRAW | HSHELL_HIGHBIT;

        public enum WindowMessage : uint
        {
            WM_NULL = 0x0000,
            WM_CLOSE = 0x0010,
            WM_COMMAND = 0x0111,
            WM_GETICON = 0x007F,
            WM_GETTEXT = 0x000D,
            WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
            WM_DWMCOMPOSITIONCHANGED = 0x31E,
            WM_SYSCOMMAND = 0x0112,           
            WM_USER = 0x0400,
            WM_PAINT = 0x000F,
            WM_APP = 0x8000,
            TB_GETBUTTON = WM_USER + 23,
            TB_BUTTONCOUNT = WM_USER + 24,
            TB_GETBUTTONINFO = WM_USER + 63,
            TB_GETBUTTONTEXT = WM_USER + 75,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205
        }

        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int OBJID_WINDOW = 0;
        public const int SC_TASKLIST = 305;

        [Flags]
        public enum WindowExtendedStyles : int
        {
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_TRANSPARENT = 0x20,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_TOPMOST = 0x00000008
        }

        [Flags]
        public enum WindowStyles : int
        {
            WS_CHILD = 0x40000000,
            WS_VISIBLE = 0x10000000,
            WS_SYSMENU = 0x80000,
            WS_DLGFRAME = 0x00400000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_CHILDWINDOW = 0x40000000
        }

        public enum RedrawFlags : uint
        {
            RDW_INVALIDATE = 0x0001,
            RDW_INTERNALPAINT = 0x0002,
            RDW_ERASE = 0x0004,

            RDW_VALIDATE = 0x0008,
            RDW_NOINTERNALPAINT = 0x0010,
            RDW_NOERASE = 0x0020,

            RDW_NOCHILDREN = 0x0040,
            RDW_ALLCHILDREN = 0x0080,

            RDW_UPDATENOW = 0x0100,
            RDW_ERASENOW = 0x0200,

            RDW_FRAME = 0x0400,
            RDW_NOFRAME = 0x0800
        }

        [Flags()]
        public enum SetWindowPosFlags : uint
        {
            SynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        #endregion

        #region Win32 Structs
        
        public class TPM 
        {
            public const int TPM_LEFTALIGN = 0x0000;
            public const int TPM_CENTERALIGN = 0x0004;
            public const int TPM_RIGHTALIGN = 0x0008;

            public const int TPM_TOPALIGN = 0x0000;
            public const int TPM_VCENTERALIGN = 0x0010;
            public const int TPM_BOTTOMALIGN = 0x0020;

            public const int TPM_NONOTIFY = 0x0080;
            public const int TPM_RETURNCMD = 0x0100;

            public const int TPM_LEFTBUTTON = 0x0000;
            public const int TPM_RIGHTBUTTON = 0x0002;

            public const int TPM_HORNEGANIMATION = 0x0800;
            public const int TPM_HORPOSANIMATION = 0x0400;
            public const int TPM_NOANIMATION = 0x4000;
            public const int TPM_VERNEGANIMATION = 0x2000;
            public const int TPM_VERPOSANIMATION = 0x1000;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SysTrayData
        {
            public Int32 hwnd;
            public Int32 uID;
            public Int32 wParam;
            public UInt32 nMsg;
        }

        [Flags]
        public enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }

            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public struct POINTAPI
        {
            public int x;
            public int y;

            public static implicit operator Point(POINTAPI point)
            {
                return new Point(point.x, point.y);
            }
        }

        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINTAPI ptMinPosition;
            public POINTAPI ptMaxPosition;
            public RECT rcNormalPosition;
        }

        public enum ABNotify : int
        {
            ABN_STATECHANGE = 0,
            ABN_POSCHANGED,
            ABN_FULLSCREENAPP,
            ABN_WINDOWARRANGE
        }

        public enum ABMsg : uint
        {
            New = 0x00000000,
            Remove = 0x00000001,
            QueryPos = 0x00000002,
            SetPos = 0x00000003,
            GetState = 0x00000004,
            GetTaskbarPos = 0x00000005,
            Activate = 0x00000006,
            GetAutoHideBar = 0x00000007,
            SetAutoHideBar = 0x00000008,
            WindowPosChanged = 0x00000009,
            SetState = 0x0000000A
        }

        public enum ABEdge : uint
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        public static class ABState
        {
            public const int Autohide = 0x0000001;
            public const int AlwaysOnTop = 0x0000002;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public ABEdge uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static explicit operator Rectangle(RECT r)
            {
                return Rectangle.FromLTRB(r.left, r.top, r.right, r.bottom);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public EnumDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASS
        {
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public EnumDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszClassName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TBBUTTON32
        {
            public int iBitmap;
            public int idCommand;
            public byte fsState;
            public byte fsStyle;
            public byte bReserved0;
            public byte bReserved1;
            public ulong dwData;
            public int iString;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TBBUTTON64
        {
            public int iBitmap;
            public int idCommand;
            public byte fsState;
            public byte fsStyle;
            public byte bReserved0;
            public byte bReserved1;
            public byte bReserved2;
            public byte bReserved3;
            public byte bReserved4;
            public byte bReserved5;
            public ulong dwData;
            public int iString;
        }

        public enum GetAncestor_Flags
        {
            GetParent = 1,
            GetRoot = 2,
            GetRootOwner = 3
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Delegate for the EnumChildWindows method
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
        /// <returns>True to continue enumerating, false to bail.</returns>
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        public delegate void WinEventDelegate(IntPtr hWinEventHook,
           uint eventType, IntPtr hwnd, int idObject,
           int idChild, uint dwEventThread, uint dwmsEventTime);

        public delegate bool EnumDelegate(IntPtr hWnd, int lParam); 
        #endregion

        #region Win32 External Functions

        [DllImport("uxtheme", ExactSpelling = true)]
        public extern static int IsThemeActive();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, UIntPtr lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y,
           int nReserved, IntPtr hWnd, IntPtr prcRect);

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINTAPI lpPoint);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int SetTextCharacterExtra(
            IntPtr hdc,    // DC handle
            int nCharExtra // extra-space value 
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINTAPI Point);

        [DllImport("user32.dll")]
        public static extern bool GetClassInfo(IntPtr hInstance, string lpClassName,
           out WNDCLASS lpWndClass);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean GetClassInfoEx(IntPtr hInstance, String lpClassName, ref WNDCLASSEX lpWndClass);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out IntPtr lpdwResult);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeout", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint SendMessageTimeoutText(
            IntPtr hWnd,
            int Msg,              // Use WM_GETTEXT
            int countOfChars,
            StringBuilder text,
            SendMessageTimeoutFlags flags,
            uint uTImeoutj,
            out IntPtr result);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestor_Flags gaFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();        

        [DllImport("user32.dll", EntryPoint = "EnumWindows",
         ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags); 

        [DllImport("user32.dll")]
        public static extern int GetClientRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern long SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(ABMsg dwMessage, [In] ref APPBARDATA pData);

        [DllImport("User32.dll", ExactSpelling = true,
            CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool MoveWindow
            (IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string msg);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin,
            uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(HandleRef hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(HandleRef hWnd, int nIndex);       

        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(
           string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi,
           uint cbSizeFileInfo, uint uFlags);

        [DllImport("comctl32")]
        public extern static IntPtr ImageList_GetIcon(
            IntPtr himl,
            int i,
            int flags);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool RegisterShellHookWindow(IntPtr hWnd);

        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        public static extern long FindExecutableA(
          string lpFile, string lpDirectory, StringBuilder lpResult);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("user32.dll")]
        public static extern bool ValidateRect(IntPtr hWnd, ref RECT lpRect);

        #endregion

        public static string FindExecutable(string pv_strFilename)
        {
            StringBuilder objResultBuffer = new StringBuilder(1024);
            long lngResult = 0;

            lngResult = FindExecutableA(pv_strFilename, string.Empty, objResultBuffer);

            if (lngResult >= 32)
            {
                return objResultBuffer.ToString();
            }

            return null;
        }

        /// <summary>
        /// Returns a list of child windows
        /// </summary>
        /// <param name="parent">Parent of the windows to return</param>
        /// <returns>List of child windows</returns>
        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        /// <summary>
        /// Callback method to be used when enumerating windows.
        /// </summary>
        /// <param name="handle">Handle of the next window</param>
        /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
        /// <returns>True to continue the enumeration, false to bail</returns>
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list != null)
            {
                list.Add(handle);
            }
            
            return true;
        }


        public static string GetWindowText(IntPtr hwnd)
        {
            int len = Native.GetWindowTextLength(hwnd);
            StringBuilder sb = new StringBuilder(len + 1);
            Native.GetWindowText(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetWindowTextWithTimeout(IntPtr hwnd, uint timeout)
        {
            IntPtr result;
            int len = Native.GetWindowTextLength(hwnd);
            StringBuilder sb = new StringBuilder(len + 1);

            SendMessageTimeoutText(hwnd, (int)WindowMessage.WM_GETTEXT, len + 1, sb, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, timeout, out result);
            
            //Native.GetWindowText(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetClassName(IntPtr hwnd)
        {
            StringBuilder sbClassName = new StringBuilder(255);
            int ret = Native.GetClassName(hwnd, sbClassName, sbClassName.Capacity);
            return sbClassName.ToString().Substring(0, ret);
        }

        internal static Icon GetIcon(IntPtr hwnd)
        {
            IntPtr hIcon = Native.SendMessage(hwnd, (uint)WindowMessage.WM_GETICON, new IntPtr(1), new IntPtr(0));
            if (hIcon != IntPtr.Zero)
            {
                return Icon.FromHandle(hIcon);
            }
            
            hIcon = Native.GetClassLongPtr(new HandleRef(null, hwnd), -14);

            if (hIcon != IntPtr.Zero)
            {
                return Icon.FromHandle(hIcon);
            }
            return null;            
        }

        internal static Icon GetIconWithTimeout(IntPtr hwnd, uint timeout)
        {
            IntPtr hIcon;
            Native.SendMessageTimeout(hwnd, (uint)WindowMessage.WM_GETICON, new UIntPtr(1), new IntPtr(0), SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, timeout, out hIcon);
            if (hIcon != IntPtr.Zero)
            {
                return Icon.FromHandle((IntPtr)hIcon);
            }

            hIcon = (IntPtr)Native.GetClassLongPtr(new HandleRef(null, hwnd), -14);

            if (hIcon != IntPtr.Zero)
            {
                return Icon.FromHandle((IntPtr)hIcon);
            }
            return null;
        }

        internal static Icon GetSmallIcon(IntPtr hwnd)
        {
            // try method 1
            IntPtr hIcon = Native.SendMessage(hwnd, (uint)WindowMessage.WM_GETICON, new IntPtr(0), new IntPtr(0));
            if (hIcon != IntPtr.Zero)
            {
                return Icon.FromHandle(hIcon);
            }

            return null;
        }

        internal static Icon GetSmallIconWithTimeout(IntPtr hwnd, uint timeout)
        {
            // try method 1
            IntPtr hIcon;
            Native.SendMessageTimeout(hwnd, (uint)WindowMessage.WM_GETICON, new UIntPtr(0), new IntPtr(0), SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, timeout, out hIcon);

            if (hIcon != IntPtr.Zero)
            {
                return Icon.FromHandle((IntPtr)hIcon);
            }

            return null;
        }

        private static IntPtr GetClassLongPtr(HandleRef hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }            

        // https://wpfklip.svn.codeplex.com/svn/Dev/WpfKlip/Core/Win/ShellHook.cs
        public static bool IsAltTabVisible(IntPtr hWnd)
        {
            int style = Native.GetWindowLong(hWnd, (int)Native.GWL_STYLE);
            if ((style & (int)Native.WindowStyles.WS_SYSMENU) == 0) return false;
            if ((style & (int)Native.WindowStyles.WS_CHILDWINDOW) == (int)Native.WindowStyles.WS_CHILDWINDOW) return false;

            if (Native.IsWindowVisible(hWnd))
            {
                if ((Native.GetWindowLong(hWnd, (int)Native.GWL_EXSTYLE) & (int)Native.WindowExtendedStyles.WS_EX_TOOLWINDOW) == 0)
                {
                    if (Native.GetParent(hWnd) != null)
                    {
                        IntPtr hwndOwner = Native.GetWindow(hWnd, (int)GetAncestor_Flags.GetRootOwner);
                        if (hwndOwner != null &&
                        ((Native.GetWindowLong(hwndOwner, (int)Native.GWL_STYLE) & ((int)Native.WindowStyles.WS_VISIBLE | (int)Native.WindowStyles.WS_CLIPCHILDREN)) != ((int)Native.WindowStyles.WS_VISIBLE | (int)Native.WindowStyles.WS_CLIPCHILDREN)) ||
                        (Native.GetWindowLong(hwndOwner, (int)Native.GWL_EXSTYLE) & (int)Native.WindowExtendedStyles.WS_EX_TOOLWINDOW) != 0)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static bool IsWindowValidForFinder(ref IntPtr hwnd, List<IntPtr> ignoreHandles, out string fileName)
        {
            fileName = null;
            hwnd = Native.GetAncestor(hwnd, GetAncestor_Flags.GetRoot);

            if (ignoreHandles.Contains(hwnd)) return false;
            if (hwnd == IntPtr.Zero) return false;
            if (!Native.IsWindow(hwnd)) return false;
            if (hwnd == Native.GetShellWindow()) return false;

            fileName = ProcessUtil.GetProcessPathByWindowHandle(hwnd);
            if (string.IsNullOrEmpty(fileName)) return false;

            return true;
        }

        public static bool IsStartMenuVisible()
        {
            IntPtr startMenu = Native.FindWindow("DV2ControlHost", null);
            return (Native.GetWindowLong(startMenu, (int)Native.GWL_STYLE) & (int)Native.WindowStyles.WS_VISIBLE) == (int)Native.WindowStyles.WS_VISIBLE;
        }
        
        internal static bool IsFullScreen(IntPtr hwnd, Rectangle screenRect)
        {
            // get the placement
            WINDOWPLACEMENT forePlacement = new WINDOWPLACEMENT();
            forePlacement.length = Marshal.SizeOf(forePlacement);
            GetWindowPlacement(hwnd, ref forePlacement);

            RECT r;
            GetWindowRect(hwnd, out r);

            // And now check the forePlacement structs properties to see if window is maximised
            if ((forePlacement.showCmd == 3 || forePlacement.showCmd == 1)
                && r.left == screenRect.Left
                && r.right == screenRect.Right
                && r.top == screenRect.Top
                && r.bottom == screenRect.Bottom)
            {
                return true;
            }
            return false;
        }

        internal static bool IsVisible(IntPtr hwnd)
        {
            // get the placement
            WINDOWPLACEMENT forePlacement = new WINDOWPLACEMENT();
            forePlacement.length = Marshal.SizeOf(forePlacement);
            GetWindowPlacement(hwnd, ref forePlacement);

            // And now check the forePlacement structs properties to see if window is maximised
            if (forePlacement.showCmd != 0)
            {
                return true;
            }
            return false;
        }

        public static Point GetCursorPosition()
        {
            POINTAPI lpPoint;
            GetCursorPos(out lpPoint);

            return lpPoint;
        }        
    }      
}
