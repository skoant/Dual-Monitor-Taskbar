using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Entities;
using System.Drawing;
using DualMonitor.Forms;
using System.Drawing.Drawing2D;
using DualMonitor.GraphicUtils;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;

namespace DualMonitor.Controls
{
    public partial class TaskbarPinnedButton : BaseTaskbarButton
    {
        private PinnedApp _app;
        private ToolTip _toolTip;

        private const int BigWidth = 63;
        private const int SmallWidth = 46;
        private const int ClickedOffset = 1;        

        private PushPanelDecorator _decorator;        

        public TaskbarPinnedButton(PinnedApp app)
            : base()
        {
            this.EnableDragging = true;
            this.AddedToTaskbar = DateTime.MinValue;

            InitializeComponent();
            _app = app;
            _toolTip = new ToolTip();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            var taskbarLocation = this.MainForm.TaskbarLocation;

            bool leftToRight = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;

            if (taskbarLocation == Native.ABEdge.Bottom)
            {
                _toolTip.Show(_app.Name, this, 0, 0);
            }
            else if (taskbarLocation == Native.ABEdge.Top)
            {
                _toolTip.Show(_app.Name, this, 0, this.Height);
            }
            else
            {
                _toolTip.Show(_app.Name, this, 0, -20);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
             _toolTip.Hide(this);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            WindowManager.Instance.LaunchProcess(_app.Path, _app.Arguments, this.MainForm.CurrentScreen);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            base.Paint(g);

            if (MainForm.IsHidden) return;

            if (_decorator == null)
            {
                var taskbarLocation = this.MainForm.TaskbarLocation;
                bool leftToRight = taskbarLocation == Native.ABEdge.Top
                    || taskbarLocation == Native.ABEdge.Bottom;
                _decorator = new PushPanelDecorator(leftToRight, Width, Height, Padding, this);
            }
            _decorator.Paint(g, _isClicked, _hover);
            
            if (this.Image != null)
            {
                int iconPosX = (this.Width - this.Image.Width) / 2;
                int iconPosY = (this.Height - this.Image.Height) / 2;
            
                g.DrawImage(this.Image, iconPosX + (_isClicked ? ClickedOffset : 0), iconPosY + (_isClicked ? ClickedOffset : 0));
            }
        }

        public void Init()
        {
            SetDefaultHeight();

            var taskbarLocation = this.MainForm.TaskbarLocation;

            bool leftToRight = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;
            Width = leftToRight ? (IsBig ? BigWidth : SmallWidth) : (this.Parent.Width);

            bool isVisualTheme = Native.IsThemeActive() != 0;
            if (!isVisualTheme && !leftToRight)
            {
                Width -= 5;
                this.Padding = new Padding(0, 0, 0, 0);
            }
            else
            {
                this.Padding = new Padding(1, 0, 2, 0);
            }
            if (_decorator != null) _decorator.Dispose();
            _decorator = null;

            int iconSize = this.IsBig ? ButtonConstants.BigIconSize : ButtonConstants.SmallIconSize;
            this.Image = _app.Icon.ResizeBitmap(iconSize, iconSize);            
        }
    }
}
