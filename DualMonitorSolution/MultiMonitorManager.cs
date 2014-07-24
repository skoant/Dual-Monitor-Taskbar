using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.VisualStyle;
using DualMonitor.Forms;
using DualMonitor.Rules;
using DualMonitor.Entities;
using System.Drawing;

namespace DualMonitor
{
    /// <summary>
    /// Creates a taskbar for each non-primary monitor.
    /// Receives hooked Explorer messages (from DualMonitorForm) and dispatches them to taskbars
    /// </summary>
    class MultiMonitorManager
    {
        private static List<SecondaryTaskbar> _taskbars = new List<SecondaryTaskbar>();
        private static RuleManager _ruleManager;

        public static List<SecondaryTaskbar> AllTaskbars
        {
            get
            {
                return _taskbars;
            }
        }

        internal static void Start()
        {
            _ruleManager = new RuleManager(TaskbarPropertiesManager.Instance.Properties.Rules, WindowManager.Instance);
            _ruleManager.Init();

            ProcessMonitor.Instance.OnFocus += new WindowActionDelegate(processLogic_OnFocus);
            ProcessMonitor.Instance.OnWindowMoved += new WindowActionDelegate(processLogic_OnWindowMoved);
            ProcessMonitor.Instance.OnWindowDestroyed += new WindowActionDelegate(processLogic_OnWindowDestroyed);

            foreach (var screen in Screen.AllScreens.Where(s => !s.Primary))
            {
                var tb = new SecondaryTaskbar(screen);
                _taskbars.Add(tb);
                tb.Show();
            }            
        }

        internal static void UpdateNotificationIcons(Message m)
        {
            _taskbars.ForEach(tb => tb.UpdateNotificationIcons(m));            
        }

        internal static void UpdateTaskbarButton(Message m)
        {
            var window = Win32Window.FromHandle(m.LParam);
            var taskbar = _taskbars.FirstOrDefault(tb => tb.CurrentScreen.Equals(window.Screen));

            if (taskbar != null)
            {
                taskbar.UpdateTaskbarButton(window, m);
            }
        }

        internal static void OnShellHook(Message m)
        {
            if ((int)m.WParam == 1) // new window created
            {
                if (Screen.AllScreens.Length > 1)
                {
                    _ruleManager.MatchRule(Win32Window.FromHandle(m.LParam));
                }
            }
            else
            {
                var window = Win32Window.FromHandle(m.LParam);
                var taskbar = _taskbars.FirstOrDefault(tb => tb.CurrentScreen.Equals(window.Screen));

                if (taskbar != null) 
                {
                    taskbar.OnShellHook(window, m.WParam);
                }
            }
        }

        static void processLogic_OnFocus(Win32Window window)
        {
            if (_taskbars.Any(tb => tb.WindowIsMine(window))) return;

            if (SystemMenuProxy.IsOpening)
            {
                SystemMenuProxy.EndOpenSystemMenu(window);
            }
        }

        static void processLogic_OnWindowMoved(Win32Window window)
        {
            if (window.IsMinimized) return;

            bool abort;
            WindowManager.Instance.WindowMoved(window, out abort);

            if (abort)
            {
                return;
            }

            if (!window.Screen.Primary || TaskbarPropertiesManager.Instance.Properties.MirrorButtons)
            {
                Rectangle r = window.Bounds;

                // some windows, when maximized are actually moved to -32000,-32000 or similar location - ignore them
                if (r.Left < -10000 || r.Left > 10000)
                {
                    return;
                }

                WindowManager.Instance.MoveToSecondary(window);
            }
        }

        static void processLogic_OnWindowDestroyed(Win32Window window)
        {
            WindowManager.Instance.WindowDestroyed(window);
        }

        public static bool IsTaskbarWindow(IntPtr handle)
        {
            return _taskbars.Exists(tb => tb.Handle == handle);
        }
    }
}
