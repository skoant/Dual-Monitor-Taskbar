using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.VisualStyle;
using System.Drawing.Drawing2D;
using DualMonitor.GraphicUtils;
using DualMonitor.Entities;
using System.Drawing;
using DualMonitor.Win32;
using DualMonitor.Forms;

namespace DualMonitor.Controls
{
    public class MoreIconsButton : BaseTaskbarButton
    {
        private GraphicsPath _outerPath = null;
        private GraphicsPath _innerPath = null;
        private GraphicsPath _arrow = null;

        private DualMonitor.Win32.Native.ABEdge _lastPosition;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            UpdatePaths();
        }

        private void UpdatePaths()
        {
            if (_outerPath != null)
            {
                _outerPath.Dispose();
                _innerPath.Dispose();
            }

            _outerPath = RoundedRectangle.Create(new Rectangle(0, 0, Width - 1, Height - 1), 3, RoundedRectangle.RectangleCorners.All);
            _innerPath = RoundedRectangle.Create(new Rectangle(1, 1, Width - 3, Height - 3), 3, RoundedRectangle.RectangleCorners.All);
        }

        private void UpdateArrowForVisualTheme()
        {
            if (_arrow != null)
            {
                _arrow.Dispose();
            }

            _lastPosition = this.MainForm.TaskbarLocation;
            _arrow = new GraphicsPath();
            Point[] points = null;

            switch (_lastPosition)
            {
                case DualMonitor.Win32.Native.ABEdge.Left:
                    points = new Point[] {
                        new Point(Width / 2 + 3, Height / 2 - 1),
                        new Point(Width / 2 - 2, Height / 2 - 5),
                        new Point(Width / 2 - 2, Height / 2 + 3),
                        new Point(Width / 2 + 3, Height / 2 - 1)
                    };
                    break;
                case DualMonitor.Win32.Native.ABEdge.Top:
                    points = new Point[] {
                        new Point(Width / 2 - 1, Height / 2 + 3),
                        new Point(Width / 2 - 5, Height / 2 - 2),
                        new Point(Width / 2 + 3, Height / 2 - 2),
                        new Point(Width / 2 - 1, Height / 2 + 3)
                    };
                    break;
                case DualMonitor.Win32.Native.ABEdge.Right:
                    points = new Point[] {
                        new Point(Width / 2 - 4, Height / 2 - 1),
                        new Point(Width / 2 + 1, Height / 2 - 5),
                        new Point(Width / 2 + 1, Height / 2 + 3),
                        new Point(Width / 2 - 4, Height / 2 - 1)
                    };
                    break;
                case DualMonitor.Win32.Native.ABEdge.Bottom:
                    points = new Point[] {
                        new Point(Width / 2 - 1, Height / 2 - 3),
                        new Point(Width / 2 - 5, Height / 2 + 2),
                        new Point(Width / 2 + 3, Height / 2 + 2),
                        new Point(Width / 2 - 1, Height / 2 - 3)
                    };
                    break;
                default:
                    break;
            }

            _arrow.AddLines(points);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (isVisualTheme 
                && (_lastPosition != this.MainForm.TaskbarLocation 
                    || _arrow == null))
            {
                UpdateArrowForVisualTheme();
            }

            if (_outerPath == null || _innerPath == null)
            {
                UpdatePaths();
            }

            base.Paint(pevent.Graphics);

            if (MainForm.IsHidden) return;

            if (isVisualTheme)
            {
                PaintVisual(pevent);
            }
            else
            {
                PaintClassic(pevent);
            }

            
        }

        private void PaintClassic(PaintEventArgs pevent)
        {
            if (_hover)
            {
                ButtonBorderDecorator.DrawSingle(pevent.Graphics, 0, 0, Width - 1, Height - 1);
            }

            Bitmap arrow = null;
            switch (this.MainForm.TaskbarLocation)
            {
                case DualMonitor.Win32.Native.ABEdge.Left:
                    arrow = Properties.Resources.more_classic_right;
                    break;
                case DualMonitor.Win32.Native.ABEdge.Top:
                    arrow = Properties.Resources.more_classic_down;
                    break;
                case DualMonitor.Win32.Native.ABEdge.Right:
                    arrow = Properties.Resources.more_classic_left;
                    break;
                case DualMonitor.Win32.Native.ABEdge.Bottom:
                    arrow = Properties.Resources.more_classic_up;
                    break;
            }

            pevent.Graphics.DrawImage(arrow,
                (this.Width - arrow.Width) / 2,
                (this.Height - arrow.Height) / 2);
        }

        private void PaintVisual(PaintEventArgs pevent)
        {
            if (_hover)
            {
                pevent.Graphics.DrawPath(Theme.ButtonOuterBorder, _outerPath);
                pevent.Graphics.DrawPath(Theme.ButtonInnerBorder, _innerPath);

                if (_isClicked)
                {
                    pevent.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.White)), new Rectangle(2, 2, Width - 4, Height - 4));
                }

                pevent.Graphics.FillGradientRectangle(Color.FromArgb(200, Color.White), Color.FromArgb(50, Color.White),
                    new Rectangle(2, 2, Width - 4, Height / 2 - 2), LinearGradientMode.Vertical);
            }

            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.FillPath(Brushes.White, _arrow);
            pevent.Graphics.DrawPath(Theme.ButtonOuterBorder, _arrow);
        }
    }
}
