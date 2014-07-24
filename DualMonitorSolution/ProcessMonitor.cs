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
    public delegate void WindowActionDelegate(Win32Window window);
    public delegate void StartMenuActionDelegate(Win32Window window, bool visible);

    /// <summary>
    /// Monitor system events
    /// </summary>
    public class ProcessMonitor : IDisposable
    {
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

        private bool _monitoring;

        private static DualMonitor.Win32.Native.WinEventDelegate _winEventProc;
        public static ProcessMonitor Instance = new ProcessMonitor();

        private ProcessMonitor()
        {
            _winEventProc = new Native.WinEventDelegate(WinEventProc);
            GC.KeepAlive(_winEventProc);
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

            // show alt-tab switcher on correct monitor
            if (eventType == Native.EVENT_SYSTEM_SWITCHSTART)
            {
                TaskSwitcherProxy.MoveTaskSwitcherToSecond(Win32Window.FromHandle(hwnd));
                return;
            }
            
            // ignore non-window messages
            if (idObject != Native.OBJID_WINDOW)
            {
                return;
            }

            Win32Window window = Win32Window.FromHandle(hwnd);

            if (eventType == Native.EVENT_SYSTEM_FOREGROUND)
            {
                // window was activated
                if (TaskbarPropertiesManager.Instance.Properties.ShowStartMenu && Native.GetClassName(hwnd).Equals("DV2ControlHost"))
                {
                    OnStartMenu(window, Native.IsStartMenuVisible());                    
                }                
                else
                {                    
                    OnWindowMoved(window);
                    OnFocus(window);
                }
            }
            else if (eventType == Native.EVENT_SYSTEM_MOVESIZEEND)
            {
                // window was moved or resized
                OnWindowMoved(window);
            }
            else if (eventType == Native.EVENT_OBJECT_DESTROY)
            {
                // window was closed
                OnLocationChanged(window);
                OnWindowDestroyed(window);                
            }            
            else if (eventType == Native.EVENT_SYSTEM_LOCATIONCHANGE || eventType == Native.EVENT_OBJECT_SHOW)
            {
                // window was shown or set to fullscreen
                if (TaskbarPropertiesManager.Instance.Properties.ShowStartMenu && window.ClassName.Equals("DV2ControlHost"))
                {
                    OnStartMenu(window, Native.IsStartMenuVisible());
                }

                var activeScreen = Screen.FromPoint(Cursor.Position);
                if (!activeScreen.Primary && window.ClassName.Contains("ClockFlyoutWindow"))
                {
                    ClockPanelProxy.MoveClockPanel(window, activeScreen);                                  
                }

                OnLocationChanged(window);

                OnWindowMoved(window);                
            }
            else if (eventType == Native.EVENT_OBJECT_HIDE)
            {
                // window was hidden (maybe moved to systray)
                OnWindowHidden(window);
            }            
        }

        /// <summary>
        /// Suspend monitoring when only 1 screen is detected
        /// </summary>
        public void PauseMonitoring()
        {
            _monitoring = false;
        }

        public void ResumeMonitoring()
        {
            _monitoring = true;
        }

        public void Dispose()
        {
            Native.UnhookWinEvent(m_hhook_startswitch);
            Native.UnhookWinEvent(m_hhook_hide);
            Native.UnhookWinEvent(m_hhook_show);
            Native.UnhookWinEvent(m_hhook_loc);
            Native.UnhookWinEvent(m_hhook_destroy);
            Native.UnhookWinEvent(m_hhook_move);
            Native.UnhookWinEvent(m_hhook_focus);
        }
    }
}
