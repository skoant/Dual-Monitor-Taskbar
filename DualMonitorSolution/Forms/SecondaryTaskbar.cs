using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using DualMonitor.Entities;
using DualMonitor.Controls;
using DualMonitor.GraphicUtils;
using Gma.UserActivityMonitor;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;
using DualMonitor.Rules;

namespace DualMonitor.Forms
{
    public partial class SecondaryTaskbar : Form
    {
        private PropertiesWindow propertiesWindow;
        private ToolTipManager tooltipManager;
        private StartButton _startButton;

        #region Public properties
        public bool IsHidden { get; private set; }

        public Native.ABEdge TaskbarLocation
        {
            get
            {
                return TaskbarPropertiesManager.Instance.Properties.GetTaskbarLocation(CurrentScreen.DeviceName);
            }
        }

        public Screen CurrentScreen
        {
            get; private set;
        }

        public bool IsBig
        {
            get
            {
                return !TaskbarPropertiesManager.Instance.Properties.SmallIcons;
            }
        }

        public Button CurrentButton
        {
            get
            {
                return flowPanel.CurrentButton;
            }
        }
        #endregion

        public SecondaryTaskbar(Screen screen)
        {
            this.CurrentScreen = screen;

            // monitor window & process events
            ProcessMonitor.Instance.OnFocus += new WindowActionDelegate(processLogic_OnFocus);
            ProcessMonitor.Instance.OnWindowMoved += new WindowActionDelegate(processLogic_OnWindowMoved);
            ProcessMonitor.Instance.OnWindowDestroyed += new WindowActionDelegate(processLogic_OnWindowDestroyed);
            ProcessMonitor.Instance.OnLocationChanged += new WindowActionDelegate(processLogic_OnLocationChanged);
            ProcessMonitor.Instance.OnStartMenu += new StartMenuActionDelegate(processLogic_OnStartMenu);
            ProcessMonitor.Instance.OnWindowHidden += new WindowActionDelegate(processLogic_OnWindowHidden);

            WindowManager.Instance.OnProcessMovedToPrimary += new WindowActionDelegate(windowManager_OnProcessMovedToPrimary);
            WindowManager.Instance.OnProcessMovedToSecondary += new ProcessMovedDelegate(windowManager_OnProcessMovedToSecondary);

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            tooltipManager = new ToolTipManager(this);

            InitializeComponent();            
            
            pnlNotificationArea.Initialize();
            pnlNotificationArea.SizeChanged += new EventHandler(pnlNotificationArea_SizeChanged);

            tsmiLockTaskbar.Checked = TaskbarPropertiesManager.Instance.Properties.Locked;
            pnlResize.BringToFront();
            
            this.AllowDrop = true;
        }

        public void UpdateNotificationIcons(Message m)
        {
            pnlNotificationArea.DispatchMessage(m);
        }

        public void UpdateTaskbarButton(Win32Window window, Message m)
        {
            flowPanel.DispatchMessage(window, m);
        }

        public void OnShellHook(Win32Window window, IntPtr action)
        {
            if ((int)action == Native.HSHELL_FLASH) // flash window
            {
                flowPanel.FlashWindow(window, true);
            }
            else if ((int)action == Native.HSHELL_REDRAW) // stop flash window & text or icon change
            {
                flowPanel.FlashWindow(window, false);
                
                WindowManager.Instance.OnWindowTitleChanged(window);
                flowPanel.UpdateButtonTag(window);  
            }
        }

        #region Events
        #region Form Events
        private void OnLoad(object sender, System.EventArgs e)
        {
            AeroDecorator.Instance.DisableLivePreview(this.Handle);

            RegisterBar();

            Properties_OnChanged(TaskbarProperties.ChangedFields.AutoHide);

            pnlClock.ContextMenuStrip = this.taskbarMenu;

            taskbarMenu.Opened += new EventHandler(taskbarMenu_Opened);

            this.ActiveControl = dummyButton;

            flowPanel.AddPinnedButtons(processMenu);

            ProcessMonitor.Instance.StartMonitoring(this.Handle);            
        }        

        void taskbarMenu_Opened(object sender, EventArgs e)
        {
            if (taskbarMenu.Right > CurrentScreen.Bounds.Right)
            {
                taskbarMenu.Left = this.Right - taskbarMenu.Width;
            }

            if (taskbarMenu.Bottom > CurrentScreen.Bounds.Bottom)
            {
                taskbarMenu.Top = this.Top - taskbarMenu.Height;
            }
        }

        private void SecondaryTaskbar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.None || e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                return;
            }

            RegisterBar();
            WindowManager.Instance.MoveProgramsToPrimary();
            ProcessMonitor.Instance.Dispose();
        }

        private void SecondaryTaskbar_Paint(object sender, PaintEventArgs e)
        {
            BackgroundDecorator.Paint(e.Graphics, this.Handle);

            Control ctrl = sender as Control;
            OneBorderDecorator.Draw(e.Graphics, ctrl, TaskbarLocation);
        }

        #region Drag & drop
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            ProcessFullPath pfp;

            if (AcceptDrop(drgevent.Data, out pfp))
            {
                drgevent.Effect = DragDropEffects.Link;
                return;
            }

            drgevent.Effect = DragDropEffects.None;
            return;
        }

        private bool AcceptDrop(IDataObject iDataObject, out ProcessFullPath pfp)
        {
            pfp = new ProcessFullPath();

            Array a = (Array)iDataObject.GetData(DataFormats.FileDrop);

            if (a == null || a.GetLength(0) != 1)
            {
                return false;
            }

            string fileName = (string)a.GetValue(0);
            pfp.FileName = Native.FindExecutable(fileName);

            if (pfp.FileName == null)
            {

                if (!fileName.EndsWith("lnk"))
                {
                    return false;
                }

                try
                {
                    Bitmap icon;
                    string displayName, targetPath, arguments;

                    ShortcutUtil.ParseShortcut(fileName, out icon, out displayName, out targetPath, out arguments);

                    pfp.FileName = targetPath;
                    pfp.Arguments = arguments;

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if (!string.Equals(fileName, pfp.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    pfp.Arguments = fileName;
                }
            }

            return true;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            ProcessFullPath pfp;

            if (AcceptDrop(drgevent.Data, out pfp))
            {
                drgevent.Effect = DragDropEffects.Link;

                var pa = PinnedManager.Instance.Pin(pfp);
                flowPanel.AddPinnedButton(pa, processMenu);
            }
        } 
        #endregion

        void pnlNotificationArea_SizeChanged(object sender, EventArgs e)
        {
            SetPosition(false, TaskbarLocation);
        }
        #endregion        

        #region Process Events
        private void windowManager_OnProcessMovedToSecondary(SecondDisplayProcess process)
        {
            if (TaskbarPropertiesManager.Instance.Properties.MirrorButtons || process.Screen.Equals(CurrentScreen))
            {
                flowPanel.AddButton(process, tooltipManager, processMenu);
            }
        }

        private void windowManager_OnProcessMovedToPrimary(Win32Window window)
        {
            if (!TaskbarPropertiesManager.Instance.Properties.MirrorButtons)
            {
                flowPanel.RemoveButton(window);
            }
        }

        void processLogic_OnLocationChanged(Win32Window window)
        {          
            bool display = true;
            foreach (var item in flowPanel.Controls)
            {
                TaskbarButton button = item as TaskbarButton;
                if (button == null) continue;

                SecondDisplayProcess proc = button.Tag as SecondDisplayProcess;
                if (proc == null) continue;

                if (Native.IsFullScreen(proc.Handle, CurrentScreen.Bounds))
                {
                    display = false;
                    break;
                }
            }

            if (this.Visible && !display)
            {
                this.Hide();
                this.IsHidden = true;
                DisplayStartMenu();
            }
            else if (!this.Visible && display)
            {
                this.Show();
                this.IsHidden = false;
                DisplayStartMenu();
            }

            // remote desktop window shows in primary taskbar after restoring from full screen - we need to hide it again
            if (!TaskbarPropertiesManager.Instance.Properties.MirrorButtons && flowPanel.ButtonExists(window))
            {
                WindowManager.Instance.ReDeleteTab(window.Handle.ToInt32());
            }
        }        

        void processLogic_OnWindowHidden(Win32Window window)
        {
            if (flowPanel.ButtonExists(window))
            {
                processLogic_OnWindowDestroyed(window);
            }
        }   

        void processLogic_OnStartMenu(Win32Window window, bool visible)
        {
            if (_startButton == null) return;

            _startButton.SetActiveState(visible);
           
            Point mousePos = Native.GetCursorPosition();
            if (Screen.FromPoint(mousePos).Equals(CurrentScreen))
            {
                Rectangle rect = window.Bounds;

                if (rect.Right == rect.Left || Math.Abs(rect.Right) > 10000 || Math.Abs(rect.Left) > 10000)
                    return;

                int newLeft = TaskbarLocation == Native.ABEdge.Right ? CurrentScreen.WorkingArea.Right - rect.Width : CurrentScreen.WorkingArea.Left;
                int newTop = TaskbarLocation == Native.ABEdge.Bottom ? CurrentScreen.WorkingArea.Bottom - rect.Height : CurrentScreen.WorkingArea.Top;

                Win32Window userPicture = Win32Window.FromClassName("Desktop User Picture");
                Native.SetWindowPos(window.Handle, userPicture.Handle, newLeft, newTop, rect.Width + 500, rect.Height, Native.SetWindowPosFlags.IgnoreResize);
            }            
        }
        
        private void processLogic_OnFocus(Win32Window window)
        {
            if (WindowIsMine(window)) return;

            if (this.flowPanel.ContextMenuStrip.Visible)
            {
                this.flowPanel.ContextMenuStrip.Hide();
            }

            tooltipManager.ToolTipWindow.ForceHide();

            if (processMenu.Visible)
            {
                processMenu.Hide();
            }

            flowPanel.ActivateOrDefault(window, dummyButton);
        }           

        void processLogic_OnWindowMoved(Win32Window window)
        {
            if (window.IsMinimized) return;

            if (flowPanel.ButtonExists(window))
            {
                if (window.Screen.Primary)
                {
                    WindowManager.Instance.MoveProgramToPrimary(window);
                }
            }            
        }

        void processLogic_OnWindowDestroyed(Win32Window window)
        {
            flowPanel.RemoveButton(window);            
        }        
        #endregion

        #region Context Menu Events
        private void processMenu_Opening(object sender, CancelEventArgs e)
        {
            if (tooltipManager.ToolTipWindow.Visible)
            {
                tooltipManager.ToolTipWindow.ForceHide();
            }

            ContextMenuStrip menu = sender as ContextMenuStrip;
            BaseTaskbarButton baseButton = menu.SourceControl as BaseTaskbarButton;
            baseButton.Hover = true;

            if (menu.SourceControl is TaskbarButton)
            {
                TaskbarButton button = menu.SourceControl as TaskbarButton;
                
                SecondDisplayProcess proc = button.Tag as SecondDisplayProcess;
                tsmiLaunch.Image = proc.SmallIcon.ConvertToBitmap(false);

                Process p = ProcessUtil.GetProcessByWindowHandle(proc.Handle);
                ProcessModule module = p.MainModule;
                if (module != null)
                {
                    string fileDescription = System.Diagnostics.FileVersionInfo.GetVersionInfo(module.FileName).FileDescription;
                    tsmiLaunch.Text = string.IsNullOrEmpty(fileDescription) ? (module.FileName ?? p.MainWindowTitle) : fileDescription.Clamp(Constants.MaxProgramNameLength);
                    tsmiLaunch.Tag = new ProcessFullPath { FileName = module.FileName, Arguments = ProcessUtil.GetCommandLineArguments(module.ModuleName) };
                    tsmiLaunch.Visible = true;

                    if (PinnedManager.Instance.IsPinned(p))
                    {
                        tsmiPin.Visible = false;
                        tsmiUnpin.Visible = true;
                    }
                    else
                    {
                        tsmiPin.Visible = true;
                        tsmiUnpin.Visible = false;
                    }
                }
                else
                {
                    tsmiLaunch.Visible = false;
                    tsmiPin.Visible = false;
                    tsmiUnpin.Visible = false;
                }

                tsmiCloseWindow.Visible = true;
            }
            else
            {
                TaskbarPinnedButton pinnedButton = menu.SourceControl as TaskbarPinnedButton;
                PinnedApp app = pinnedButton.Tag as PinnedApp;

                tsmiCloseWindow.Visible = false;
                tsmiLaunch.Image = app.Icon.ResizeBitmap(ButtonConstants.SmallIconSize, ButtonConstants.SmallIconSize);
                tsmiLaunch.Text = string.IsNullOrEmpty(app.Name) ? "Unknown application" : app.Name.Clamp(Constants.MaxProgramNameLength);
                tsmiLaunch.Tag = new ProcessFullPath { FileName = app.Path, Arguments = app.Arguments };
                tsmiLaunch.Visible = !string.IsNullOrEmpty(app.Path);

                if (PinnedManager.Instance.IsPinned(app))
                {
                    tsmiPin.Visible = false;
                    tsmiUnpin.Visible = true;
                }
                else
                {
                    tsmiPin.Visible = true;
                    tsmiUnpin.Visible = false;
                }
            }
        }

        void processMenu_Opened(object sender, System.EventArgs e)
        {           
            ContextMenuStrip menu = sender as ContextMenuStrip;
            BaseTaskbarButton button = menu.SourceControl as BaseTaskbarButton;
            if (button == null) return;

            var taskbarLocation = this.TaskbarLocation;

            if (taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom)
            {
                menu.Left = flowPanel.PointToScreen(new Point(button.Left + (button.Width - menu.Width) / 2, 0)).X;
                if (taskbarLocation == Native.ABEdge.Bottom)
                {
                    menu.Top = this.Location.Y - menu.Height;
                }
                else
                {
                    menu.Top = this.Bounds.Bottom;
                }
            }
            else
            {
                menu.Top = Math.Max(CurrentScreen.Bounds.Top, flowPanel.PointToScreen(new Point(0, button.Bottom)).Y - menu.Height);

                if (taskbarLocation == Native.ABEdge.Left)
                {
                    menu.Left = this.Bounds.Right;
                }
                else
                {
                    menu.Left = this.Bounds.Left - menu.Width;
                }
            }
        }   

        private void processMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            BaseTaskbarButton button = (sender as ContextMenuStrip).SourceControl as BaseTaskbarButton;
            button.Hover = false;
        }     

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            ProcessMonitor.Instance.PauseMonitoring();
            Application.Exit();
        }

        private void tsmiStartTaskManager_Click(object sender, EventArgs e)
        {
            Process.Start("taskmgr.exe");
        }
        
        private void tsmiLockTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            TaskbarPropertiesManager.Instance.Properties.Locked = tsmiLockTaskbar.Checked;            
            TaskbarPropertiesManager.Instance.Save();

            foreach (var taskbarInstance in MultiMonitorManager.AllTaskbars)
            {
                taskbarInstance.tsmiLockTaskbar.Checked = tsmiLockTaskbar.Checked;
                taskbarInstance.Properties_OnChanged(TaskbarProperties.ChangedFields.None);
            }
        }

        private void tsmiProperties_Click(object sender, EventArgs e)
        {
            if (propertiesWindow != null)
            {
                propertiesWindow.Focus();
                return;
            }

            propertiesWindow = new PropertiesWindow(this);
            propertiesWindow.FormClosed += new FormClosedEventHandler(propertiesWindow_FormClosed);
            foreach (var taskbarInstance in MultiMonitorManager.AllTaskbars)
            {
                propertiesWindow.OnChanged += new Action<TaskbarProperties.ChangedFields>(taskbarInstance.Properties_OnChanged);
            }
            propertiesWindow.InitializeWithProperties();
            propertiesWindow.ShowDialog();
        }

        void propertiesWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            propertiesWindow = null;
        }

        private void tsmiLaunch_Click(object sender, EventArgs e)
        {
            ProcessFullPath pfp = (sender as ToolStripMenuItem).Tag as ProcessFullPath;
            WindowManager.Instance.LaunchProcess(pfp.FileName, pfp.Arguments, CurrentScreen);
        }        

        private void tsmiCloseWindow_Click(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            var strip = menu.Owner as ContextMenuStrip;
            var button = strip.SourceControl as Button;

            SecondDisplayProcess process = button.Tag as SecondDisplayProcess;
            WindowManager.Instance.CloseProcess(process);
        }

        void tsmiPin_Click(object sender, System.EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            var strip = menu.Owner as ContextMenuStrip;
            var button = strip.SourceControl as Button;
            SecondDisplayProcess proc = button.Tag as SecondDisplayProcess;

            try
            {
                var pa = PinnedManager.Instance.Pin(ProcessUtil.GetProcessByWindowHandle(proc.Handle));
                flowPanel.AddPinnedButton(pa, processMenu);
            }
            catch
            {                
                try
                {
                    var pa = PinnedManager.Instance.Unpin(ProcessUtil.GetProcessByWindowHandle(proc.Handle));
                    flowPanel.RemovePinnedButton(pa);
                }
                catch { }

                MessageBox.Show("There was an error trying to pin this application to the taskbar.", "Dual Monitor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void tsmiUnpin_Click(object sender, System.EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            var strip = menu.Owner as ContextMenuStrip;
            TaskbarButton button = strip.SourceControl as TaskbarButton;

            if (button != null)
            {
                SecondDisplayProcess proc = button.Tag as SecondDisplayProcess;

                var pa = PinnedManager.Instance.Unpin(ProcessUtil.GetProcessByWindowHandle(proc.Handle));
                flowPanel.RemovePinnedButton(pa);
            }
            else
            {
                var app = (strip.SourceControl as TaskbarPinnedButton).Tag as PinnedApp;

                PinnedManager.Instance.Unpin(app);
                flowPanel.RemovePinnedButton(app);
            }
        }                   

        #endregion 

        #region Properties window
        private void Properties_OnChanged(TaskbarProperties.ChangedFields changes)
        {
            DisplayClock();

            var taskbarLocation = this.TaskbarLocation;

            SetPosition(changes.HasFlag(TaskbarProperties.ChangedFields.AutoHide), taskbarLocation);            

            if (changes.HasFlag(TaskbarProperties.ChangedFields.MirrorButtons))
            {
                if (TaskbarPropertiesManager.Instance.Properties.MirrorButtons)
                {
                    WindowManager.Instance.MoveProgramsToPrimary();
                    WindowManager.Instance.MoveProgramsToTaskbars();
                }
                else
                {
                    flowPanel.Reset();
                    WindowManager.Instance.MoveProgramsToTaskbars();
                }
            }

            pnlNotificationArea.Visible = TaskbarPropertiesManager.Instance.Properties.ShowNotificationArea;

            if (TaskbarPropertiesManager.Instance.Properties.AutoHide)
            {
                this.Autohide(true);
            }
            else
            {
                this.Autohide(false);
            }
            
            AutoUpdateManager.Run(this);
            
            ABSetPos(changes.HasFlag(TaskbarProperties.ChangedFields.AutoHide));
        }
        
        #endregion

        
        #endregion

        private void DisplayClock()
        {
            if (TaskbarPropertiesManager.Instance.Properties.ShowClock)
            {                
                pnlClock.Visible = true;               
            }
            else
            {
                pnlClock.Visible = false;
            }
        }

        private void DisplayStartMenu()
        {            
            ShowStartMenu(TaskbarPropertiesManager.Instance.Properties.ShowStartMenu && !this.IsHidden);
        }

        /// <summary>
        /// If location has changed, move taskbar to that location
        /// </summary>
        public void OnMove(MouseEventArgs e, bool notify)
        {
            Rectangle screenBounds = CurrentScreen.Bounds;
            var location = screenBounds.GetEdgeByPoint(new Point(e.X, e.Y));

            notify = notify && !TaskbarPropertiesManager.Instance.Properties.AutoHide;

            var taskbarLocation = this.TaskbarLocation;

            if (taskbarLocation != location || notify)
            {                
                SetPosition(notify, location);                
            }
        }

        /// <summary>
        /// Move taskbar to specific side and rearrange all controls
        /// </summary>
        /// <param name="notify">If TRUE it will broadcast a message, so that all windows can adjust their size based on the new workarea </param>
        /// <param name="location">Where to move it</param>
        public void SetPosition(bool notify, Native.ABEdge location)
        {
            TaskbarPropertiesManager.Instance.Properties.SetTaskbarLocation(CurrentScreen.DeviceName, location);
            ABSetPos(notify);
            DisplayStartMenu();
            
            bool isVisualTheme = Native.IsThemeActive() != 0;

            btnShowDesktop.CalculateSizeAndPosition();

            FlowDirection direction;
            if (location == Native.ABEdge.Top
                || location == Native.ABEdge.Bottom)
            {
                direction = FlowDirection.LeftToRight;                

                pnlClock.Dock = DockStyle.Right;
                pnlClock.Width = ClockPanel.PanelWidth;
                pnlClock.Height = pnlNotificationBorder.Height;

                if (pnlNotificationArea.Visible)
                {
                    pnlNotificationArea.CalculateSize(NotificationAreaProxy.GetVisibleIcons().Count);
                    pnlNotificationArea.UpdateIcons();
                    pnlNotificationArea.Left = 1;
                    pnlNotificationArea.Top = 0;
                }                               

                pnlNotificationBorder.Width = (pnlClock.Visible ? pnlClock.Width : 0) 
                    + (pnlNotificationArea.Visible ? pnlNotificationArea.Width + 5 : 0)
                    + btnShowDesktop.Width;

                pnlProgramsFlow.Width = this.Width - (_startButton != null ? 54 : 0) - pnlNotificationBorder.Width - 10;
                pnlProgramsFlow.Height = this.Height;
                pnlProgramsFlow.Left = (_startButton != null ? 54 : 0);
                pnlProgramsFlow.Top = 0;

                if (!isVisualTheme)
                {
                    pnlNotificationBorder.Height = location == Native.ABEdge.Bottom ? this.Height - 6 : this.Height - 9;
                    pnlNotificationBorder.Top = location == Native.ABEdge.Bottom ? 4 : 3;
                    pnlNotificationBorder.Left = pnlProgramsFlow.Right + 8;
                }
                else
                {
                    pnlNotificationBorder.Height = this.Height;
                    pnlNotificationBorder.Top = 0;
                    pnlNotificationBorder.Left = this.Width - pnlNotificationBorder.Width;
                }

                if (location == Native.ABEdge.Bottom)
                {
                    this.pnlResize.Dock = System.Windows.Forms.DockStyle.Top;
                }
                else
                {
                    this.pnlResize.Dock = System.Windows.Forms.DockStyle.Bottom;
                }
                this.pnlResize.Height = 4;

                if (!TaskbarPropertiesManager.Instance.Properties.Locked)
                {
                    this.pnlResize.Cursor = System.Windows.Forms.Cursors.SizeNS;
                }
                else
                {
                    this.pnlResize.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
            else
            {
                direction = FlowDirection.TopDown;                

                pnlClock.Dock = DockStyle.Bottom;
                pnlClock.Width = pnlNotificationBorder.Width;
                pnlClock.Height = 55;

                if (pnlNotificationArea.Visible)
                {
                    pnlNotificationArea.CalculateSize(NotificationAreaProxy.GetVisibleIcons().Count);
                    pnlNotificationArea.UpdateIcons();
                    pnlNotificationArea.Left = 0;
                    pnlNotificationArea.Top = 2;
                }

                pnlNotificationBorder.Height = (pnlClock.Visible ? pnlClock.Height : 0) 
                    + (pnlNotificationArea.Visible ? pnlNotificationArea.Height : 0)
                    + btnShowDesktop.Height;

                pnlProgramsFlow.Height = this.Height - (_startButton != null ? 45 : 0) - pnlNotificationBorder.Height - 10;
                pnlProgramsFlow.Width = this.Width;
                pnlProgramsFlow.Left = 0;
                pnlProgramsFlow.Top = (_startButton != null ? 45 : 0);

                if (!isVisualTheme)
                {
                    pnlNotificationBorder.Width = this.Width - 6;
                    pnlNotificationBorder.Left = (location == Native.ABEdge.Left ? 2 : 4);
                    pnlNotificationBorder.Top = pnlProgramsFlow.Bottom + 8;
                }
                else
                {
                    pnlNotificationBorder.Width = this.Width;
                    pnlNotificationBorder.Left = 0;
                    pnlNotificationBorder.Top = this.Height - pnlNotificationBorder.Height;
                }

                if (location == Native.ABEdge.Left)
                {
                    this.pnlResize.Dock = System.Windows.Forms.DockStyle.Right;
                }
                else
                {
                    this.pnlResize.Dock = System.Windows.Forms.DockStyle.Left;
                }
                this.pnlResize.Width = 4;

                if (!TaskbarPropertiesManager.Instance.Properties.Locked)
                {
                    this.pnlResize.Cursor = System.Windows.Forms.Cursors.SizeWE;
                }
                else
                {
                    this.pnlResize.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }

            pnlClock.RefreshProperties();

            flowPanel.SuspendLayout();

            flowPanel.FlowDirection = direction;
            flowPanel.RefreshProperties();

            if (tooltipManager.ToolTipWindow != null)
            {
                tooltipManager.ToolTipWindow.RefreshProperties();
            }

            if ((location == Native.ABEdge.Right ||
                location == Native.ABEdge.Left)
                && Native.IsThemeActive() == 0)
            {
                flowPanel.Padding = new Padding(3, 0, 6, 0);
            }
            else
            {
                flowPanel.Padding = new Padding(0);
            }

            flowPanel.ResumeLayout();

            Refresh();
            
            if (notify)
            {
                TaskbarPropertiesManager.Instance.Save();
            }
        }

        /// <summary>
        /// Hide or show when AutoHide is on
        /// </summary>
        public void Autohide(bool flag)
        {
            IsHidden = flag;

            pnlNotificationArea.Visible = TaskbarPropertiesManager.Instance.Properties.ShowNotificationArea && !IsHidden;

            SetPosition(false, this.TaskbarLocation);
        }

        /// <summary>
        /// Decide if the taskbar can be hidden or not when AutoHide is on
        /// </summary>
        public bool ShouldStayVisible()
        {
            Point pos = Cursor.Position;
            if (this.Bounds.Contains(pos)
                || tooltipManager.ToolTipWindow.Visible
                || flowPanel.HasFlashingButtons()
                || pnlResize.IsResizing
                || processMenu.Visible
                || taskbarMenu.Visible
                || pnlNotificationArea.MoreIconsVisible
                || flowPanel.IsMoving)
            {
                return true;
            }

            return false;
        }

        public bool WindowIsMine(Win32Window window)
        {
            if (MultiMonitorManager.IsTaskbarWindow(window.Handle)) return true;
            if (window.Handle == tooltipManager.ToolTipWindow.Handle) return true;

            // I haven't found a way to show this "close form" and make it TopMost without sending the WM_ACTIVATE message, so we need to ignore this message
            if (tooltipManager.ToolTipWindow.CloseForm != null && window.Handle == tooltipManager.ToolTipWindow.CloseForm.Handle) return true;

            return false;
        }

        void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (Screen.AllScreens.Length != 1)
            {
                if (!fBarRegistered)
                {
                    RegisterBar();
                }
                else
                {
                    ABSetPos();
                }
            }
        }

        private void ShowStartMenu(bool visible)
        {
            if (visible)
            {
                if (_startButton == null)
                {
                    _startButton = new StartButton(this);
                    _startButton.Owner = this;
                }

                _startButton.UpdateAndShow(this);
            }
            else
            {
                if (_startButton != null)
                {
                    _startButton.Close();
                    _startButton = null;
                }
            }
        }
    }
}
