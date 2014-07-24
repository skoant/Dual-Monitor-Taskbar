using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using DualMonitor.Entities;
using DualMonitor.Win32;
using DualMonitor.Controls;
using DualMonitor.VisualStyle;

namespace DualMonitor
{
    public delegate void ProcessMovedDelegate(SecondDisplayProcess process);
    public delegate void WindowActionDelegate(IntPtr hwnd);
    public delegate void StartMenuActionDelegate(IntPtr hwnd, bool visible);

    public class ProcessLogic
    {
        public event ProcessMovedDelegate OnProcessMovedToSecond;
        public event WindowActionDelegate OnProcessMovedToPrimary;
        public event WindowActionDelegate OnFocus;
        public event WindowActionDelegate OnWindowMoved;
        public event WindowActionDelegate OnWindowDestroyed;
        public event WindowActionDelegate OnLocationChanged;
        public event StartMenuActionDelegate OnStartMenu;
        public event WindowActionDelegate OnWindowHidden;

        private IntPtr m_hhook_focus;
        private IntPtr m_hhook_move;
        private IntPtr m_hhook_destroy;
        private IntPtr m_hhook_show;
        private IntPtr m_hhook_loc;
        private IntPtr m_hhook_hide;
        private IntPtr m_hhook_startswitch;

        private List<int> _launchedProcesses = new List<int>();

        private bool _monitoring;

        private TaskbarList.TaskbarList taskbarList;
        private static DualMonitor.Win32.Native.WinEventDelegate _winEventProc;

        private SyncProcessCache CachedProcesses;

        public ProcessLogic()
        {
            CachedProcesses = new SyncProcessCache();

            _winEventProc = new Native.WinEventDelegate(WinEventProc);

            GC.KeepAlive(_winEventProc);
            
            taskbarList = new TaskbarList.TaskbarList();
            taskbarList.HrInit();
        }

        public void StartMonitoring(IntPtr parentHandle)
        {
            m_hhook_focus = Native.SetWinEventHook(Native.EVENT_SYSTEM_FOREGROUND,
               Native.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
               _winEventProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);

            m_hhook_move = Native.SetWinEventHook(Native.EVENT_SYSTEM_MOVESIZEEND,
               Native.EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero,
               _winEventProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);

            m_hhook_destroy = Native.SetWinEventHook(Native.EVENT_OBJECT_DESTROY,
               Native.EVENT_OBJECT_DESTROY, IntPtr.Zero,
               _winEventProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);
            
            m_hhook_loc = Native.SetWinEventHook(Native.EVENT_SYSTEM_LOCATIONCHANGE,
                Native.EVENT_SYSTEM_LOCATIONCHANGE, IntPtr.Zero,
               _winEventProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);

            m_hhook_show = Native.SetWinEventHook(Native.EVENT_OBJECT_SHOW,
                Native.EVENT_OBJECT_SHOW, IntPtr.Zero,
               _winEventProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);

            m_hhook_hide = Native.SetWinEventHook(Native.EVENT_OBJECT_HIDE,
                Native.EVENT_OBJECT_HIDE, IntPtr.Zero,
               _winEventProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);

            m_hhook_startswitch = Native.SetWinEventHook(Native.EVENT_SYSTEM_SWITCHSTART,
                Native.EVENT_SYSTEM_SWITCHSTART, IntPtr.Zero,
               _winEventProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);

            _monitoring = true;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (!_monitoring) return;

            if (eventType == Native.EVENT_SYSTEM_SWITCHSTART)
            {
                MoveTaskSwitcherToSecond(hwnd);                
            }

            if (idObject != Native.OBJID_WINDOW)
            {
                return;
            }

            if (eventType == Native.EVENT_SYSTEM_FOREGROUND)
            {
                if (TaskbarPropertiesManager.Instance.Properties.ShowStartMenu && Native.GetClassName(hwnd).Equals("DV2ControlHost"))
                {
                    OnStartMenu(hwnd, Native.IsStartMenuVisible());                    
                }                
                else
                {
                    var proc = CachedProcesses.Get(hwnd.ToInt32());
                    if (proc != null && proc.MoveToScreenOnFirstShow != null)
                    {
                        MoveWindowToSecond(hwnd);
                        proc.MoveToScreenOnFirstShow = null;
                        return;
                    }

                    OnWindowMoved(hwnd);
                    OnFocus(hwnd);
                }
            }
            else if (eventType == Native.EVENT_SYSTEM_MOVESIZEEND)
            {
                OnWindowMoved(hwnd);
            }
            else if (eventType == Native.EVENT_OBJECT_DESTROY)
            {                
                OnLocationChanged(hwnd);
                OnWindowDestroyed(hwnd);
                CachedProcesses.Remove(hwnd.ToInt32());
            }            
            else if (eventType == Native.EVENT_SYSTEM_LOCATIONCHANGE || eventType == Native.EVENT_OBJECT_SHOW)
            {
                string className = Native.GetClassName(hwnd);

                if (TaskbarPropertiesManager.Instance.Properties.ShowStartMenu && className.Equals("DV2ControlHost"))
                {
                    OnStartMenu(hwnd, Native.IsStartMenuVisible());
                }

                if (!Screen.FromPoint(Cursor.Position).Primary && className.Contains("ClockFlyoutWindow"))
                {                    
                    OnClockWindow(hwnd);                    
                }

                OnLocationChanged(hwnd);                

                OnWindowMoved(hwnd);

                MoveWindowIfLaunchedFromSecond(hwnd);                
            }
            else if (eventType == Native.EVENT_OBJECT_HIDE)
            {                
                OnWindowHidden(hwnd);
            }            
        }

        void OnClockWindow(IntPtr hwnd)
        {
            Screen sec = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
            if (sec != null)
            {
                RECT r;
                Native.GetWindowRect(hwnd, out r);

                int width = r.right - r.left;
                int height = r.bottom - r.top;

                int dx = AeroDecorator.Instance.IsDwmCompositionEnabled ? 12 : 0;
                int dy = AeroDecorator.Instance.IsDwmCompositionEnabled ? 11 : 0;

                switch (TaskbarPropertiesManager.Instance.Properties.ScreenLocation)
                {
                    case ABEdge.Bottom:
                        Native.MoveWindow(hwnd, sec.WorkingArea.Right - width - dx, sec.WorkingArea.Bottom - height - dy, width, height, true);
                        break;
                    case ABEdge.Left:
                        Native.MoveWindow(hwnd, sec.WorkingArea.Left + dx, sec.WorkingArea.Bottom - height - dy, width, height, true);
                        break;
                    case ABEdge.Right:
                        Native.MoveWindow(hwnd, sec.WorkingArea.Right - width - dx, sec.WorkingArea.Bottom - height - dy, width, height, true);
                        break;
                    case ABEdge.Top:
                        Native.MoveWindow(hwnd, sec.WorkingArea.Right - width - dx, sec.WorkingArea.Top + dy, width, height, true);
                        break;
                }
            }
        }

        public void OnWindowTitleChanged(IntPtr hwnd)
        {
            SecondDisplayProcess proc = CachedProcesses.Get(hwnd.ToInt32());
            if (proc != null)
            {
                proc.Title = Native.GetWindowText(hwnd);
            }
        }

        private void MoveTaskSwitcherToSecond(IntPtr hwnd)
        {
            Point mousePos = Native.GetCursorPosition();
            if (!Screen.FromPoint(mousePos).Primary)
            {
                MoveWindowToSecond(hwnd);                
            }
        }

        private void MoveWindowToSecond(IntPtr hwnd)
        {
            if (WindowManager.DetectScreen(hwnd).Primary)
            {
                RECT r;
                Native.GetWindowRect(hwnd, out r);

                int dx = r.left - Screen.PrimaryScreen.Bounds.Left;
                int dy = r.top - Screen.PrimaryScreen.Bounds.Top;

                Screen sec = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
                if (sec != null)
                {
                    Native.MoveWindow(hwnd, sec.Bounds.Left + dx, sec.Bounds.Top + dy, r.right - r.left, r.bottom - r.top, true);
                }
            }
        }

        private void MoveWindowIfLaunchedFromSecond(IntPtr hwnd)
        {
            uint pid;
            Native.GetWindowThreadProcessId(hwnd, out pid);

            if (!_launchedProcesses.Contains((int)pid))
            {
                return;
            }

            Process process = Process.GetProcessById((int)pid);
            if (process.MainWindowHandle == hwnd)
            {
                MoveWindowToSecond(hwnd);

                _launchedProcesses.Remove(process.Id);
            }
        }

        public void MoveWindowBetweenScreens(IntPtr hwnd, Screen source, Screen target)
        {
            if (Native.IsIconic(hwnd) && !target.Primary)
            {
                MoveProgramToSecondary(hwnd);
                SecondDisplayProcess proc = CachedProcesses.Get(hwnd.ToInt32());
                if (proc != null)
                {
                    proc.MoveToScreenOnFirstShow = target;
                }
            }
            else
            {
                RECT r;
                Native.GetWindowRect(hwnd, out r);

                int dx = r.left - source.WorkingArea.Left;
                int dy = r.top - source.WorkingArea.Top;

                Native.SetWindowPos(hwnd, IntPtr.Zero, target.WorkingArea.Left + dx, target.WorkingArea.Top + dy, r.right - r.left, r.bottom - r.top, 0);
            }
        }
      
        public void MoveProgramsToSecond()
        {
            bool succ = Native.EnumWindows(new EnumDelegate(delegate(IntPtr hwnd, int lParam) {                
                if (Native.IsAltTabVisible(hwnd))
                {
                    bool moveToSecond = TaskbarPropertiesManager.Instance.Properties.MirrorButtons || !WindowManager.DetectScreen(hwnd).Primary;
                    if (moveToSecond)
                    {
                        if (Native.GetWindowText(hwnd).Length > 0)
                        {
                            MoveToSecond(hwnd);
                        }
                    }
                }
                return true;
            }), IntPtr.Zero);
        }

        public void MoveProgramToSecondary(IntPtr hwnd)
        {
            MoveToSecond(hwnd);
        }

        private void MoveToSecond(IntPtr hwnd)
        {
            if (!Native.IsAltTabVisible(hwnd)) return;            
            var processForDisplay = TransformProcessForDisplay(hwnd);

            if (processForDisplay == null) return;

            if (!TaskbarPropertiesManager.Instance.Properties.MirrorButtons)
            {
                DeleteTab(hwnd.ToInt32());
            }

            OnProcessMovedToSecond(processForDisplay);
        }
        
        public void MoveProgramsToPrimary()
        {
            bool succ = Native.EnumWindows(new EnumDelegate(delegate(IntPtr hwnd, int lParam)
            {
                if (Native.IsAltTabVisible(hwnd))
                {
                    Screen s = WindowManager.DetectScreen(hwnd);
                    if (!s.Primary)
                    {
                        if (Native.GetWindowText(hwnd).Length > 0)
                        {
                            AddTab(hwnd.ToInt32());
                        }
                    }
                }
                return true;
            }), IntPtr.Zero);
        }        
        
        public void MoveProgramToPrimary(IntPtr hwnd)
        {
            var processForDisplay = TransformProcessForDisplay(hwnd);
            if (!TaskbarPropertiesManager.Instance.Properties.MirrorButtons)
            {
                AddTab(hwnd.ToInt32());
            }
            OnProcessMovedToPrimary(processForDisplay.MainWindowHandle);
        }

        public SecondDisplayProcess TransformProcessForDisplay(IntPtr hwnd)
        {
            SecondDisplayProcess result;

            result = CachedProcesses.Get(hwnd.ToInt32());
            if (result != null) return result;

            result = new SecondDisplayProcess();
            result.MainWindowHandle = hwnd;

            result.Title = Native.GetWindowText(hwnd);
            result.Icon = Native.GetIcon(hwnd);
            result.SmallIcon = Native.GetSmallIcon(hwnd);
            result.Path = ProcessUtil.GetProcessPathByWindowHandle(hwnd);

            if (result.SmallIcon == null) result.SmallIcon = result.Icon;

            if (result.Title.Length == 0 || result.Icon == null) return null;

            return CachedProcesses.Add(hwnd.ToInt32(), result);
        }

        public bool CloseProcess(IntPtr hwnd)
        {
            CachedProcesses.Remove(hwnd.ToInt32());

            Native.PostMessage(hwnd, (uint)Native.WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            return true;
        }

        public void OnClose()
        {
            Native.UnhookWinEvent(m_hhook_startswitch);
            Native.UnhookWinEvent(m_hhook_hide);
            Native.UnhookWinEvent(m_hhook_show);
            Native.UnhookWinEvent(m_hhook_loc);
            Native.UnhookWinEvent(m_hhook_destroy);
            Native.UnhookWinEvent(m_hhook_move);
            Native.UnhookWinEvent(m_hhook_focus);
        }

        public void LaunchProcess(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                int pid = ProcessUtil.LaunchProcess(path);
                _launchedProcesses.Add(pid);
            }
        }

        public void LaunchProcess(SecondDisplayProcess proc)
        {
            LaunchProcess(proc.Path);
        }

        public void DeleteTab(int hwnd)
        {
            taskbarList.DeleteTab(hwnd);
        }

        private void AddTab(int hwnd)
        {            
            taskbarList.AddTab(hwnd);
        }

        public void PauseMonitoring()
        {
            _monitoring = false;
        }

        public void ResumeMonitoring()
        {
            _monitoring = true;
        }        
    }
}
