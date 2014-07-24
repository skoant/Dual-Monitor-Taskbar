using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Entities;
using DualMonitor.Win32;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace DualMonitor
{
    public class WindowManager
    {
        public class LaunchedProcess
        {
            public int PID;
            public Screen LaunchedFrom;
        }

        #region Simple Singleton (no need for concurrency or lazy etc...)
        private static WindowManager _instance = new WindowManager();
        public static WindowManager Instance { get { return _instance; } } 
        #endregion

        public event ProcessMovedDelegate OnProcessMovedToSecondary;
        public event WindowActionDelegate OnProcessMovedToPrimary;

        // processes launched from our taskbars - they need to be moved on proper screen after launch
        private List<LaunchedProcess> _launchedProcesses = new List<LaunchedProcess>();

        // COM object to interact with main taskbar (add/remove buttons from it)
        private TaskbarList.TaskbarList taskbarList;

        // get some data about processes and cache it for convenience (and performance)
        private SyncProcessCache CachedProcesses;

        // remember windows minimized by taskbar button (for each screen) so we can restore them
        private Dictionary<Screen, List<SecondDisplayProcess>> _minimizedByButton = new Dictionary<Screen, List<SecondDisplayProcess>>();

        private WindowManager()
        {
            CachedProcesses = new SyncProcessCache();
            taskbarList = new TaskbarList.TaskbarList();
            taskbarList.HrInit();
        }

        /// <summary>
        /// Force move window to secondary screen if it was launched from secondary taskbar
        /// </summary>
        private void MoveWindowIfLaunchedFromSecondary(Win32Window window)
        {
            uint pid;
            Native.GetWindowThreadProcessId(window.Handle, out pid);

            var launchedProcess = _launchedProcesses.FirstOrDefault(l => l.PID == (int)pid);

            if (launchedProcess == null)
            {
                return;
            }            

            Process process = Process.GetProcessById((int)pid);
            if (process.MainWindowHandle == window.Handle)
            {
                window.MoveWindowTo(launchedProcess.LaunchedFrom);

                _launchedProcesses.Remove(launchedProcess);
            }
        }

        /// <summary>
        /// Move window from one screen to another
        /// </summary>
        public void MoveWindowBetweenScreens(Win32Window window, Screen source, Screen target)
        {
            if (window.IsMinimized && !target.Primary)
            {
                MoveToSecondary(window);
                SecondDisplayProcess proc = CachedProcesses.Get(window.Handle.ToInt32());
                if (proc != null)
                {
                    proc.MoveToScreenOnFirstShow = target;
                }
            }
            else
            {
                Rectangle r = window.Bounds;

                int dx = r.Left - source.WorkingArea.Left;
                int dy = r.Top - source.WorkingArea.Top;

                Native.SetWindowPos(window.Handle, IntPtr.Zero, target.WorkingArea.Left + dx, target.WorkingArea.Top + dy, 
                    r.Width, r.Height, 0);
            }
        }

        /// <summary>
        /// Detect windows that appear on secondary monitors and move them to secondary taskbars
        /// </summary>
        public void MoveProgramsToTaskbars()
        {
            bool succ = Native.EnumWindows(new Native.EnumDelegate(delegate(IntPtr hwnd, int lParam)
            {
                if (Native.IsAltTabVisible(hwnd))
                {
                    Win32Window window = Win32Window.FromHandle(hwnd);
                    bool moveToSecondary = TaskbarPropertiesManager.Instance.Properties.MirrorButtons || !window.Screen.Primary;
                    if (moveToSecondary)
                    {
                        if (window.Title.Length > 0)
                        {
                            MoveToSecondary(window);
                        }
                    }
                }
                return true;
            }), IntPtr.Zero);
        }

        /// <summary>
        /// Delete button from main taskbar (except when mirror mode) and add it to secondary taskbar
        /// </summary>
        public void MoveToSecondary(Win32Window window)
        {
            if (!Native.IsAltTabVisible(window.Handle)) return;
            var processForDisplay = TransformProcessForDisplay(window);

            if (processForDisplay == null) return;

            if (!TaskbarPropertiesManager.Instance.Properties.MirrorButtons)
            {
                DeleteTab(window.Handle.ToInt32());
            }

            OnProcessMovedToSecondary(processForDisplay);
        }

        /// <summary>
        /// Detect programs that appear on secondary screen and move them back to primary taskbar
        /// </summary>
        public void MoveProgramsToPrimary()
        {
            bool succ = Native.EnumWindows(new Native.EnumDelegate(delegate(IntPtr hwnd, int lParam)
            {
                if (Native.IsAltTabVisible(hwnd))
                {
                    Win32Window window = Win32Window.FromHandle(hwnd);

                    Screen s = window.Screen;
                    if (!s.Primary)
                    {
                        if (window.Title.Length > 0)
                        {
                            AddTab(window.Handle.ToInt32());
                        }
                    }
                }
                return true;
            }), IntPtr.Zero);
        }

        /// <summary>
        /// Move a program to primary taskbar
        /// </summary>
        public void MoveProgramToPrimary(Win32Window window)
        {
            var processForDisplay = TransformProcessForDisplay(window);
            if (!TaskbarPropertiesManager.Instance.Properties.MirrorButtons)
            {
                AddTab(window.Handle.ToInt32());
            }
            OnProcessMovedToPrimary(processForDisplay);
        }

        private SecondDisplayProcess TransformProcessForDisplay(Win32Window window)
        {
            SecondDisplayProcess result;

            result = CachedProcesses.Get(window.Handle.ToInt32());
            if (result != null) return result;

            result = SecondDisplayProcess.FromHandle(window.Handle);
            if (result.Title.Length == 0 || result.Icon == null || string.IsNullOrEmpty(result.Path)) return null;

            return CachedProcesses.Add(window.Handle.ToInt32(), result);
        }

        public bool CloseProcess(Win32Window window)
        {
            window.ActivateWindow(false);

            CachedProcesses.Remove(window.Handle.ToInt32());

            Native.PostMessage(window.Handle, (uint)Native.WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            return true;
        }

        public void LaunchProcess(string path, string arguments, Screen launchedFrom)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    int pid = ProcessUtil.LaunchProcess(path, arguments);
                    _launchedProcesses.Add(new LaunchedProcess { PID = pid, LaunchedFrom = launchedFrom });
                }
                catch
                {
                    // Ignore for now. Later, we can offer to remove the pinned item (if broken shortcut)
                }
            }
        }

        public void LaunchProcess(SecondDisplayProcess proc, Screen currentScreen)
        {
            LaunchProcess(proc.Path, proc.Arguments, currentScreen);
        }

        /// <summary>
        /// Delete tab from main taskbar
        /// </summary>
        /// <param name="hwnd"></param>
        public void ReDeleteTab(int hwnd)
        {
            DeleteTab(hwnd);
        }

        private void DeleteTab(int hwnd)
        {
            taskbarList.DeleteTab(hwnd);
        }

        private void AddTab(int hwnd)
        {
            taskbarList.AddTab(hwnd);
        }

        public void OnWindowTitleChanged(Win32Window window)
        {
            SecondDisplayProcess proc = CachedProcesses.Get(window.Handle.ToInt32());
            if (proc != null)
            {
                proc.Refresh();
            }
        }

        public void WindowMoved(Win32Window window, out bool abort)
        {
            abort = false;
            var proc = CachedProcesses.Get(window.Handle.ToInt32());
            if (proc != null && proc.MoveToScreenOnFirstShow != null)
            {
                window.MoveWindowTo(proc.MoveToScreenOnFirstShow);
                proc.MoveToScreenOnFirstShow = null;
                abort = true;
            }

            MoveWindowIfLaunchedFromSecondary(window);
        }

        public void WindowDestroyed(Win32Window window)
        {
            CachedProcesses.Remove(window.Handle.ToInt32());
        }

        public void MinimizeAllPrimary()
        {
            bool succ = Native.EnumWindows(new Native.EnumDelegate(delegate(IntPtr hwnd, int lParam)
            {
                if (Native.IsAltTabVisible(hwnd))
                {
                    Win32Window window = Win32Window.FromHandle(hwnd);

                    Screen s = window.Screen;
                    if (s.Primary)
                    {
                        if (window.Title.Length > 0)
                        {
                            window.Minimize();
                        }
                    }
                }
                return true;
            }), IntPtr.Zero);
        }

        public void MinimizeAllFromScreen(Screen screen)
        {
            List<SecondDisplayProcess> list;
            if (!_minimizedByButton.TryGetValue(screen, out list))
            {
                list = new List<SecondDisplayProcess>();
                _minimizedByButton.Add(screen, list);
            }

            if (CachedProcesses.Any(sp => sp.Screen.Equals(screen) && !sp.IsMinimized))
            {
                list.Clear();
                CachedProcesses.ForEach(delegate(SecondDisplayProcess p)
                {
                    if (!p.IsMinimized)
                    {
                        if (!p.Screen.Equals(screen))
                        {
                            return;
                        }

                        try { p.Minimize(); }
                        catch { }
                        list.Add(p);
                    }
                });
            }
            else
            {
                if (list.Count == 0)
                {
                    CachedProcesses.ForEach(delegate(SecondDisplayProcess p)
                    {
                        if (!p.Screen.Equals(screen))
                        {
                            return;
                        }

                        try { p.Restore(); }
                        catch { }
                    });
                }
                else
                {
                    list.ForEach(delegate(SecondDisplayProcess p)
                    {
                        try { p.Restore(); }
                        catch { }
                    });

                    list.Clear();
                }
            }
        }        
    }
}
