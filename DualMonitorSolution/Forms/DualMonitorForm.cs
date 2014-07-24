using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Win32;

namespace DualMonitor.Forms
{
    /// <summary>
    /// Hidden form that will monitor Explorer and dispatch events to all Taskbars
    /// </summary>
    public partial class DualMonitorForm : Form
    {
        private int WM_ShellHook;

        public DualMonitorForm()
        {
            InitializeComponent();

            NotificationAreaProxy.HookNotificationArea(this.Handle);
            RunningTasksProxy.HookRunningApps(this.Handle);

            HookWindows();
        }

        protected override void WndProc(ref Message m)
        {            
            // Notification from hook dll - need to update icons
            if (m.Msg == (int)Native.WindowMessage.WM_APP + 1516)
            {
                MultiMonitorManager.UpdateNotificationIcons(m);                
            }
            // Taskbar button progress
            else if (m.Msg == (int)Native.WindowMessage.WM_APP + 1601
                || m.Msg == (int)Native.WindowMessage.WM_APP + 1602
                || m.Msg == (int)Native.WindowMessage.WM_APP + 1603)
            {
                MultiMonitorManager.UpdateTaskbarButton(m);                
            }
            else if (m.Msg == WM_ShellHook)
            {
                MultiMonitorManager.OnShellHook(m);                
            }

            base.WndProc(ref m);
        }

        private void HookWindows()
        {
            if (Native.RegisterShellHookWindow(this.Handle))
            {
                WM_ShellHook = Native.RegisterWindowMessage("SHELLHOOK");
            }
        }

        private void DualMonitorForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
