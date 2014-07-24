using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gma.UserActivityMonitor;
using DualMonitor.Forms;
using System.Drawing;
using DualMonitor.Entities;
using DualMonitor.Win32;

namespace DualMonitor.Controls
{
    public partial class ResizePanel : TransparentPanel
    {
        private SecondaryTaskbar MainForm { get { return this.FindForm() as SecondaryTaskbar; } }
        private bool _resizing;
        private AutoHideMonitor _autoHideMonitor;

        public bool IsResizing
        {
            get
            {
                return _resizing;
            }
        }

        public ResizePanel()
        {
            InitializeComponent();
            _resizing = false;
        }

        void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            // resize based on mouse position

            Native.ABEdge location = this.MainForm.TaskbarLocation;
            if (location == Native.ABEdge.Top || location == Native.ABEdge.Bottom)
            {
                int h = MainForm.Height;
                int dy = location == Native.ABEdge.Bottom ? (MainForm.CurrentScreen.Bounds.Bottom - e.Y) : (e.Y - MainForm.CurrentScreen.Bounds.Top);

                int multiplier = (int)(dy / MainForm.BaseSize);
                int nh = multiplier * MainForm.BaseSize;
                if (nh > 0 && nh != h && nh <= MainForm.CurrentScreen.Bounds.Height / 2)
                {
                    TaskbarPropertiesManager.Instance.Properties.HeightMultiplier = multiplier;

                    MainForm.SetPosition(false, this.MainForm.TaskbarLocation);
                }
            }
            else
            {
                int dy = location == Native.ABEdge.Left ? (e.X - MainForm.CurrentScreen.Bounds.Left) : (MainForm.CurrentScreen.Bounds.Right - e.X);
                if (dy < Constants.VerticalTaskbarWidth) dy = Constants.VerticalTaskbarWidth;

                if (dy != TaskbarPropertiesManager.Instance.Properties.Width && dy <= MainForm.CurrentScreen.Bounds.Width / 2)
                {
                    TaskbarPropertiesManager.Instance.Properties.Width = dy;
                    MainForm.SetPosition(false, this.MainForm.TaskbarLocation);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // when mouse is rolling over, show the taskbar (if in autohide mode)
            if (TaskbarPropertiesManager.Instance.Properties.AutoHide)
            {
                if (MainForm.IsHidden)
                {
                    MainForm.Autohide(false);

                    if (_autoHideMonitor != null)
                    {
                        _autoHideMonitor.Dispose();
                        _autoHideMonitor = null;
                    }
                }

                if (_autoHideMonitor == null)
                {
                    _autoHideMonitor = new AutoHideMonitor(MainForm);
                    _autoHideMonitor.OnHide += new Action(_autoHideMonitor_OnHide);
                    _autoHideMonitor.Start();
                }
            }
        }

        void _autoHideMonitor_OnHide()
        {
            if (_autoHideMonitor != null)
            {
                _autoHideMonitor.Dispose();
                _autoHideMonitor = null;
            }

            // AutoHideMonitor has decided that we can hide the taskbar now
            MainForm.Invoke(new MethodInvoker(delegate {
                MainForm.Autohide(true);
            }));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_resizing) return;
            if (TaskbarPropertiesManager.Instance.Properties.Locked) return;

            // drag taskbar to other screen edges
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                HookManager.MouseMove += HookManager_MouseMove;
                _resizing = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (TaskbarPropertiesManager.Instance.Properties.Locked) return;

            // finished dragging the taskbar
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (_resizing)
                {
                    HookManager.MouseMove -= HookManager_MouseMove;
                    _resizing = false;
                }
                MainForm.SetPosition(!TaskbarPropertiesManager.Instance.Properties.AutoHide, 
                    this.MainForm.TaskbarLocation);
            }
        }
    }
}
