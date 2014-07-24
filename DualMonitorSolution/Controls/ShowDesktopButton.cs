using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Win32;
using System.Drawing;
using System.Windows.Forms;
using DualMonitor.GraphicUtils;
using DualMonitor.Entities;
using System.Drawing.Drawing2D;
using DualMonitor.VisualStyle;

namespace DualMonitor.Controls
{
    public class ShowDesktopButton : BaseTaskbarButton
    {
        private static readonly string TooltipText = "Show desktop";
        private ToolTip _toolTip;
        private Rectangle _buttonBounds;

        public ShowDesktopButton()
        {
            _toolTip = new ToolTip();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _buttonBounds = new Rectangle(0, 0, this.Width, this.Height);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            var taskbarLocation = this.MainForm.TaskbarLocation;

            bool leftToRight = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;

            if (taskbarLocation == Native.ABEdge.Bottom)
            {
                _toolTip.Show(TooltipText, this, 0, 0);
            }
            else if (taskbarLocation == Native.ABEdge.Top)
            {
                _toolTip.Show(TooltipText, this, 0, this.Height);
            }
            else
            {
                _toolTip.Show(TooltipText, this, 0, -20);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _toolTip.Hide(this);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            Invalidate();
            if (mevent.Button == System.Windows.Forms.MouseButtons.Left)
            {
                WindowManager.Instance.MinimizeAllFromScreen(this.MainForm.CurrentScreen);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (isVisualTheme)
            {
                PaintVisual(pevent);
            }
            else
            {
                PaintClassic(pevent);
            }
        }

        protected override bool IsNearEdge(Native.ABEdge edge)
        {
            return false;
        }

        private void PaintClassic(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            base.Paint(g);

            if (!MainForm.IsHidden)
            {
                Pen darkPen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));
                Pen lighterPen = new Pen(Color.FromKnownColor(KnownColor.ControlLightLight));

                var taskbarLocation = this.MainForm.TaskbarLocation;

                if (taskbarLocation == Native.ABEdge.Bottom
                    || taskbarLocation == Native.ABEdge.Top)
                {
                    g.DrawLine(darkPen, 0, 0, Width, 0);
                }
                else
                {
                    g.DrawLine(darkPen, 0, 0, 0, Height - 1);
                }

                g.DrawLine(lighterPen, 0, Height - 1, Width, Height - 1);
                g.DrawLine(lighterPen, Width - 1, Height - 1, Width - 1, 0);

                if (Hover)
                {
                    ButtonBorderDecorator.DrawSingle(g, 1, 1, this.Width - 2, this.Height - 2, _isClicked);
                }

                Image image = Properties.Resources.show_desktop_classic;
                Point p = new Point((this.Width - image.Width) / 2, (this.Height - image.Height) / 2);
                g.DrawImage(image, p);
            }
        }

        private void PaintVisual(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            base.Paint(g);

            if (!MainForm.IsHidden)
            {
                var taskbarLocation = this.MainForm.TaskbarLocation;

                // paint background
                GlassButtonDecorator.Paint(g, taskbarLocation, _buttonBounds, Hover, _isClicked);
            }
        }

        public void CalculateSizeAndPosition()
        {
            bool isVisualTheme = Native.IsThemeActive() != 0;

            var taskbarLocation = this.MainForm.TaskbarLocation;

            if (taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom)
            {
                this.Dock = DockStyle.Right;
                this.Width = isVisualTheme ? 15 : 20;
                this.Height = this.Height;
            }
            else
            {
                this.Dock = DockStyle.Bottom;
                this.Width = this.Width;
                this.Height = isVisualTheme ? 15 : 20;
            }
        }
    }
}
