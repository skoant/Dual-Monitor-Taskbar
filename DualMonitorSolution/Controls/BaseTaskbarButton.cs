using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Forms;
using System.Drawing;
using DualMonitor.Entities;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;
using Gma.UserActivityMonitor;

namespace DualMonitor.Controls
{
    public partial class BaseTaskbarButton : Button
    {
        protected bool _isDragged = false;
        protected Point _initialDraggingPoint;

        public DateTime AddedToTaskbar { get; protected set; }

        public SecondaryTaskbar MainForm { get { return this.FindForm() as SecondaryTaskbar; } }
        protected bool _isClicked = false;
        protected bool _hover = false;
        protected bool IsBig { get { return MainForm.IsBig; } }

        protected bool EnableDragging
        {
            get;
            set;
        }

        public bool Hover
        {
            get { return _hover; }
            set
            {
                if (value || this.ContextMenuStrip == null || !this.ContextMenuStrip.Visible)
                {
                    _hover = value;
                    Invalidate();
                }
            }
        }

        public BaseTaskbarButton()
        {
            
        }

        public void SetDefaultHeight()
        {
            var taskbarLocation = this.MainForm.TaskbarLocation;

            Height = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom
                ? (this.IsBig ? ButtonConstants.BigHorizontalHeight(taskbarLocation) : ButtonConstants.SmallHorizontalHeight(taskbarLocation)) 
                : (this.IsBig ? ButtonConstants.BigVerticalHeight : ButtonConstants.SmallVerticalHeight);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Hover = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Hover = false;

            if (_isClicked)
            {
                _isClicked = false;
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            if (_isClicked)
            {
                _isClicked = false;

                if (EnableDragging)
                {
                    HookManager_MouseUp(this, mevent);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);

            if (mevent.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _isClicked = true;

                if (EnableDragging)
                {
                    HookManager.MouseMove += HookManager_MouseMove;
                    _initialDraggingPoint = this.PointToScreen(new Point(mevent.X, mevent.Y));
                }
            }
        }

        protected virtual bool IsNearEdge(Native.ABEdge edge)
        {
            switch (edge)
            {
                case Native.ABEdge.Left:
                    return this.Right == this.Parent.Width;
                case Native.ABEdge.Top:
                    return this.Bottom == this.Parent.Height;
                case Native.ABEdge.Right:
                    return this.Left == 0;
                case Native.ABEdge.Bottom:
                    return this.Top == 0;
            }

            return false;
        }

        protected new void Paint(Graphics g)
        {
            BackgroundDecorator.Paint(g, this.Handle);
            var taskbarLocation = this.MainForm.TaskbarLocation;

            if (!IsNearEdge(taskbarLocation))
            {
                return;
            }

            OneBorderDecorator.Draw(g, this, taskbarLocation);
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            TaskbarFlow flow = this.Parent as TaskbarFlow;

            if (_isDragged)
            {
                flow.OnButtonMove(this, e.X, e.Y);
            }
            else if (!_isDragged)
            {
                var taskbarLocation = this.MainForm.TaskbarLocation;
                if (taskbarLocation == Native.ABEdge.Top
                    || taskbarLocation == Native.ABEdge.Bottom)
                {
                    if (Math.Abs(_initialDraggingPoint.X - e.X) < 15)
                    {
                        return;
                    }
                }
                else
                {
                    if (Math.Abs(_initialDraggingPoint.Y - e.Y) < 15)
                    {
                        return;
                    }
                }

                HookManager.MouseUp += HookManager_MouseUp;
                _isDragged = true;
            }
        }

        private void HookManager_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                HookManager.MouseMove -= HookManager_MouseMove;
            }
            catch { }

            if (_isDragged)
            {
                try
                {
                    HookManager.MouseUp -= HookManager_MouseUp;
                }
                catch { }

                _isDragged = false;
            }

            TaskbarFlow flow = this.Parent as TaskbarFlow;
            flow.OnButtonRelease(this);

            OnMouseUp(e);
        }
    }
}
