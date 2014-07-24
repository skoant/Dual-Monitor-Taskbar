using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Entities;
using DualMonitor.Forms;
using System.Drawing;
using DualMonitor.GraphicUtils;
using Gma.UserActivityMonitor;
using DualMonitor.Win32;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;

namespace DualMonitor.Controls
{
    public partial class TaskbarFlow : FlowLayoutPanel
    {
        private BaseTaskbarButton _draggedButton;
        private Point _draggedPoint;

        //private static readonly Semaphore _syncEvent = new Semaphore(1, 1);
        private IntPtr _lastAddButtonHandle = IntPtr.Zero;

        private List<TaskbarButton> _taskbarButtons = new List<TaskbarButton>();
        private List<TaskbarPinnedButton> _taskbarPinnedButtons = new List<TaskbarPinnedButton>();

        /// <summary>
        /// Buttons are grouped by path (better would be to group by AppId like windows does, but I have no way of obtaining that AppId)
        /// This way, all windows of the same process appear grouped
        /// </summary>
        private ProcessGroupManager _groups;

        private SecondaryTaskbar _mainForm = null;

        public SecondaryTaskbar MainForm
        {
            get
            {
                if (_mainForm == null)
                {
                    _mainForm = this.FindForm() as SecondaryTaskbar;
                }

                return _mainForm;
            }
        }

        public bool IsMoving { get; private set; }

        private bool IsBig { get { return MainForm.IsBig; } }

        public Button CurrentButton { get; private set; }

        public TaskbarFlow()
        {
            InitializeComponent();

            _groups = new ProcessGroupManager(this);

            this.VerticalScroll.SmallChange = this.VerticalScroll.LargeChange = this.Height;
            this.HorizontalScroll.SmallChange = this.HorizontalScroll.LargeChange = this.Width;

            try
            {
                string path = GetTempIconsFolder();
                if (System.IO.Directory.Exists(path))
                {
                    foreach (string file in Directory.EnumerateFiles(path, "*.ico"))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // just ignore this
            }
        }
        
        /// <summary>
        /// Show all pinned items saved on disk
        /// </summary>
        public void AddPinnedButtons(ContextMenuStrip processMenu)
        {
            var apps = PinnedManager.Instance.GetApps();

            foreach (var app in apps)
            {
                AddPinnedButton(app, processMenu, false);
            }
        }

        /// <summary>
        /// Get a list of all windows that appear on the second taskbar
        /// </summary>
        public List<SecondDisplayProcess> GetDisplayProcesses()
        {
            List<SecondDisplayProcess> result = new List<SecondDisplayProcess>();

            return _taskbarButtons.ConvertAll(tb => tb.Tag as SecondDisplayProcess);
        }

        /// <summary>
        /// Reset all buttons after properties have changed
        /// </summary>
        public void RefreshProperties()
        {
            this.Width = Parent.Width;
            this.Height = Parent.Height;

            if (MainForm.IsHidden)
            {
                this.AutoScroll = false;
                HorizontalScroll.Enabled = false;
                VerticalScroll.Enabled = false;
            }
            else
            {
                this.AutoScroll = true;
                if (FlowDirection == System.Windows.Forms.FlowDirection.LeftToRight)
                {
                    HorizontalScroll.Enabled = false;
                    VerticalScroll.Enabled = true;
                    this.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
                    this.VerticalScroll.SmallChange = this.VerticalScroll.LargeChange = this.Height;
                }
                else
                {
                    HorizontalScroll.Enabled = true;
                    VerticalScroll.Enabled = false;
                    this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
                    this.HorizontalScroll.SmallChange = this.HorizontalScroll.LargeChange = this.Width;
                }
            }

            foreach (var item in this.Controls)
            {
                TaskbarButton button = item as TaskbarButton;
                if (button != null)
                {
                    button.UpdateFont();
                    button.SetDefaultHeight();
                    button.ShowLabel = TaskbarPropertiesManager.Instance.Properties.ShowLabels;

                    var process = button.Tag as SecondDisplayProcess;
                    if (process.Icon != null)
                    {
                        button.Image = (this.IsBig ? process.Icon : process.SmallIcon).ConvertToBitmap(this.IsBig);
                    }
                }
                else
                {
                    (item as TaskbarPinnedButton).Init();
                }
            }

            ArrangeButtons();
        }

        /// <summary>
        /// Add a new button to the taskbar
        /// </summary>
        /// <param name="process">Window wrapper</param>
        /// <param name="tooltipManager">Reference to the tooltip manager</param>
        /// <param name="processMenu">Button's context menu</param>
        public void AddButton(SecondDisplayProcess process, ToolTipManager tooltipManager, ContextMenuStrip processMenu)
        {
            // multiple windows messages call this method, in the same thread.
            // so we need to ignore same message when repeated
            if (_lastAddButtonHandle == process.Handle) return;

            _lastAddButtonHandle = process.Handle;

            TaskbarButton button = _taskbarButtons.Find(tb =>
            {
                var sdp = (tb.Tag as SecondDisplayProcess);
                return sdp.Handle == process.Handle;
            });

            if (button != null)
            {
                _lastAddButtonHandle = IntPtr.Zero;
                return;
            }

            // can't use locks since the thread is the same
            //if (_syncEvent.WaitOne(500))
            {
                button = _taskbarButtons.Find(tb =>
                {
                    var sdp = (tb.Tag as SecondDisplayProcess);
                    return sdp.Handle == process.Handle;
                });
                if (button != null)
                {
                     _lastAddButtonHandle = IntPtr.Zero;
                    //_syncEvent.Release();
                    return;
                }

                button = new TaskbarButton(tooltipManager);
                button.Tag = process;
                _taskbarButtons.Add(button);

                //_syncEvent.Release();
            }
            /*
            else
            {
                _lastAddButtonHandle = IntPtr.Zero;
                return;
            }
            */
            _lastAddButtonHandle = IntPtr.Zero;

            this.SuspendLayout();

            try
            {
                this.Controls.Add(button);
            }
            catch (Win32Exception e)
            {
                var p = System.Diagnostics.Process.GetCurrentProcess();

                if (MessageBox.Show(string.Format("Err: {1}{0}Thread Count: {2}{0}Handle Count: {3}", Environment.NewLine, e.Message, p.Threads.Count, p.HandleCount), 
                    "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Debugger.Break();
                }

                _taskbarButtons.Remove(button);
                this.ResumeLayout();
                return;
            }

            button.AutoSize = false;
            button.Text = process.Title;
            if (process.Icon != null)
            {
                button.Image = (IsBig ? process.Icon : process.SmallIcon).ConvertToBitmap(IsBig);
            }
            button.Padding = new System.Windows.Forms.Padding(0);
            button.Margin = new System.Windows.Forms.Padding(0);
            button.ShowLabel = TaskbarPropertiesManager.Instance.Properties.ShowLabels;
            button.SetDefaultHeight();
            
            button.ContextMenuStrip = processMenu;

            _groups.AddToGroup(process.Path, button);

            ArrangeButtons();
            this.ResumeLayout();

            button.Refresh();
        }

        /// <summary>
        /// Resize buttons to fit taskbar
        /// </summary>
        public void ArrangeButtons()
        {
            if (_taskbarButtons.Count == 0)
                return;

            if (FlowDirection == System.Windows.Forms.FlowDirection.LeftToRight)
            {
                // if only icon mode, buttons are already at minimum size
                if (!TaskbarPropertiesManager.Instance.Properties.ShowLabels)
                    return;

                // calculate space occupied by buttons and available space
                int buttonCrtWidth = _taskbarButtons[0].Width;
                int buttonsCount = _taskbarButtons.Count;
                int pinnedWidth = _taskbarPinnedButtons.Count > 0 ? _taskbarPinnedButtons.Sum(tpb => tpb.Visible ? tpb.Width : 0) : 0;

                int spaceForButtons = this.ClientRectangle.Width - pinnedWidth - 10;

                int min, max, minThreshold;
                ButtonConstants.GetWidthLimits(IsBig, out min, out minThreshold, out max);

                int totalWidth = buttonsCount * buttonCrtWidth;
                int width = spaceForButtons / buttonsCount;

                if (totalWidth > spaceForButtons)
                {
                    if (width < minThreshold)
                    {
                        width = min;
                    }

                    SetButtonsMaxWidth(width);
                }
                else
                {
                    if (width < minThreshold)
                    {
                        width = min;
                    }

                    if (width > max)
                    {
                        width = max;
                    }

                    SetButtonsMaxWidth(width);
                }
            }
            else
            {
                bool isVisualTheme = Native.IsThemeActive() != 0;

                if (isVisualTheme)
                {
                    // vertical layout
                    SetButtonsMaxWidth(this.Width);
                    _taskbarButtons.ForEach(tb => tb.Width = this.Width);
                }
                else
                {
                    SetButtonsMaxWidth(this.Width - 7);
                    _taskbarButtons.ForEach(tb => tb.Width = this.Width - 7);
                }
            }
        }

        /// <summary>
        /// Check if a window appears on taskbar
        /// </summary>
        public bool ButtonExists(Win32Window window)
        {
            return FindProcessButtonByHandle(window) != null;
        }

        /// <summary>
        /// Removes button from taskbar
        /// </summary>
        public void RemoveButton(Win32Window window)
        {
            Button button = FindProcessButtonByHandle(window);
            if (button != null)
            {
                var tb = button as TaskbarButton;

                this.Controls.Remove(tb);
                _taskbarButtons.Remove(tb);

                _groups.RemoveFromGroup((tb.Tag as SecondDisplayProcess).Path, tb);

                ArrangeButtons();
            }
        }

        /// <summary>
        /// Focus a button if it exists
        /// </summary>
        public void ActivateOrDefault(Win32Window window, Button dummyButton)
        {
            Button button = FindProcessButtonByHandle(window);

            if (button != null)
            {
                button.Focus();
                CurrentButton = button;
            }
            else
            {
                dummyButton.Focus();
                CurrentButton = dummyButton;
            }
        }

        /// <summary>
        /// Update button caption
        /// </summary>
        public void UpdateButtonTag(Win32Window window)
        {
            Button b = FindProcessButtonByHandle(window);
            if (b != null)
            {
                SecondDisplayProcess info = ((b as TaskbarButton).Tag as SecondDisplayProcess);
                b.Text = info.Title;
                if (info.Icon != null)
                {
                    b.Image = info.Icon.ConvertToBitmap(IsBig);
                }
            }
        }

        /// <summary>
        /// Add a new pinned button
        /// </summary>
        /// <param name="app">Pinned application</param>
        /// <param name="processMenu">Context menu</param>
        public void AddPinnedButton(PinnedApp app, ContextMenuStrip processMenu)
        {
            AddPinnedButton(app, processMenu, true);
        }

        /// <summary>
        /// Add a new pinned button
        /// </summary>
        /// <param name="app">Pinned application</param>
        /// <param name="processMenu">Context menu</param>
        private void AddPinnedButton(PinnedApp app, ContextMenuStrip processMenu, bool saveOrder)
        {
            var button = new TaskbarPinnedButton(app);
            this.Controls.Add(button);

            button.Tag = app;
            button.Init();
            button.AutoSize = false;
            button.Padding = new System.Windows.Forms.Padding(0);
            button.Margin = new System.Windows.Forms.Padding(0);
            button.ContextMenuStrip = processMenu;

            _groups.AddToGroup(app.Path, button);
            _taskbarPinnedButtons.Add(button);
            ArrangeButtons();

            if (saveOrder)
            {
                ReorderPinnedButtons();
            }
        }

        /// <summary>
        /// Remove a pinned button
        /// </summary>
        /// <param name="app">Pinned application</param>
        public void RemovePinnedButton(PinnedApp app)
        {
            var button = _taskbarPinnedButtons.Find(tpb => tpb.Tag == app);
            if (button != null)
            {
                this.Controls.Remove(button);
                _groups.RemoveFromGroup(app.Path, button);
                _taskbarPinnedButtons.Remove(button);
                ArrangeButtons();
            }
        }

        /// <summary>
        /// Flash button
        /// </summary>
        public void FlashWindow(Win32Window window, bool active)
        {
            TaskbarButton button = FindProcessButtonByHandle(window);
            if (button == null) return;
            button.Flash(active);
        }

        /// <summary>
        /// Check if any buttons are flashing currently
        /// </summary>
        /// <returns></returns>
        public bool HasFlashingButtons()
        {
            foreach (var item in _taskbarButtons)
            {
                if (item.Flashing) return true;
            }

            return false;
        }

        /// <summary>
        /// Remove all taskbar buttons
        /// </summary>
        public void Reset()
        {
            _taskbarButtons.ForEach(tb =>
            {
                this.Controls.Remove(tb);
                _groups.RemoveFromGroup((tb.Tag as SecondDisplayProcess).Path, tb);
            });

            _taskbarButtons.Clear();

            ArrangeButtons();
        }


        private TaskbarButton FindProcessButtonByHandle(Win32Window window)
        {
            return _taskbarButtons.Find(tb => {
                var sdp = (tb.Tag as SecondDisplayProcess);
                
                return sdp.Handle == window.Handle;
            });
        }

        private void SetButtonsMaxWidth(int width)
        {
            _taskbarButtons.ForEach(tb => tb.MaxWidth = width);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!TaskbarPropertiesManager.Instance.Properties.Locked)
                {
                    HookManager.MouseMove += HookManager_MouseMove;
                    HookManager.MouseUp += HookManager_MouseUp;

                    IsMoving = true;
                }
            }
        }

        protected void HookManager_MouseUp(object sender, MouseEventArgs e)
        {
            HookManager.MouseMove -= HookManager_MouseMove;
            HookManager.MouseUp -= HookManager_MouseUp;

            IsMoving = false;
            MainForm.OnMove(e, true);
        }

        protected void HookManager_MouseMove(object sender, MouseEventArgs e)
        {            
            MainForm.OnMove(e, false);
        }

        public void DispatchMessage(Win32Window window, Message m)
        {
            TaskbarButton button = FindProcessButtonByHandle(window);
            if (button == null) return;

            switch (m.Msg)
            {
                case (int)Native.WindowMessage.WM_APP + 1601:
                    button.SetProgressValue((int)m.LParam);
                    break;
                case (int)Native.WindowMessage.WM_APP + 1602:
                    button.SetProgressState((TaskbarProgressState)m.LParam);
                    break;
                case (int)Native.WindowMessage.WM_APP + 1603:
                    if (m.LParam == IntPtr.Zero)
                    {
                        button.SetOverlayIcon(null);
                    }
                    else
                    {
                        button.SetOverlayIcon(GetTempIconsFolder() + "\\overlayicon_" + (int)m.LParam + ".ico");
                    }
                    break;
            }
        }

        private string GetTempIconsFolder()
        {
            string path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "DualMonitor", "TempIcons");

            return path;
        }

        public void OnButtonMove(BaseTaskbarButton button, int x, int y)
        {
            _draggedButton = button;
            
            Point clientCoords = this.PointToClient(new Point(x, y));
            x = clientCoords.X;
            y = clientCoords.Y;
            _draggedPoint = clientCoords;

            bool moved = _groups.MoveButtonToPoint(clientCoords, button);

            if (moved)
            {
                ReorderPinnedButtons();
            }
        }

        public void OnButtonRelease(BaseTaskbarButton button)
        {
            _draggedButton = null;
        }

        private void ReorderPinnedButtons()
        {
            TaskbarPropertiesManager.Instance.Properties.PinnedPrograms.Clear();

            this._taskbarPinnedButtons.Sort((x, y) => this.Controls.GetChildIndex(x).CompareTo(this.Controls.GetChildIndex(y)));

            foreach (var item in this._taskbarPinnedButtons)
            {
                TaskbarPinnedButton pinnedButton = item as TaskbarPinnedButton;
                PinnedApp pinnedApp = pinnedButton.Tag as PinnedApp;
                TaskbarPropertiesManager.Instance.Properties.PinnedPrograms.Add(pinnedApp.Shortcut);
            }

            try
            {
                TaskbarPropertiesManager.Instance.Save();
            }
            catch
            {
                // bury... log ?
            }
        }
    }
}
