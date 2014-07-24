using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DualMonitor.Win32;
using DualMonitor.Entities;
using DualMonitor.Controls;

namespace DualMonitor.VisualStyle
{
    class OneBorderDecorator
    {
        public static void Draw(Graphics g, Control ctrl, Native.ABEdge taskbarLocation)
        {
            if (Native.IsThemeActive() != 0)
            {
                Pen pen1 = Theme.TaskbarTopLine1;
                Pen pen2 = Theme.TaskbarTopLine2;

                switch (taskbarLocation)
                {
                    case Native.ABEdge.Left:
                        g.DrawLine(pen1, ctrl.Size.Width - 1, 0, ctrl.Size.Width - 1, ctrl.Size.Height);
                        g.DrawLine(pen2, ctrl.Size.Width - 2, 0, ctrl.Size.Width - 2, ctrl.Size.Height);
                        break;
                    case Native.ABEdge.Top:
                        g.DrawLine(pen1, 0, ctrl.Size.Height - 1, ctrl.Size.Width, ctrl.Size.Height - 1);
                        g.DrawLine(pen2, 0, ctrl.Size.Height - 2, ctrl.Size.Width, ctrl.Size.Height - 2);
                        break;
                    case Native.ABEdge.Right:
                        g.DrawLine(pen1, 0, 0, 0, ctrl.Size.Height);
                        g.DrawLine(pen2, 1, 0, 1, ctrl.Size.Height);
                        break;
                    case Native.ABEdge.Bottom:
                        g.DrawLine(pen1, 0, 0, ctrl.Size.Width, 0);
                        g.DrawLine(pen2, 0, 1, ctrl.Size.Width, 1);
                        break;
                }
            }
            else
            {
                Pen darkPen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));
                Pen lighterPen = new Pen(Color.FromKnownColor(KnownColor.ControlLightLight));
                Pen darkerPen = new Pen(Color.FromKnownColor(KnownColor.ControlDarkDark));
                Pen lightPen = new Pen(Color.FromKnownColor(KnownColor.ControlLight));

                switch (taskbarLocation)
                {
                    case Native.ABEdge.Left:
                        g.DrawLine(darkerPen, ctrl.Size.Width - 1, 0, ctrl.Size.Width - 1, ctrl.Size.Height);
                        g.DrawLine(darkPen, ctrl.Size.Width - 2, 0, ctrl.Size.Width - 2, ctrl.Size.Height);
                        break;
                    case Native.ABEdge.Top:
                        g.DrawLine(darkerPen, 0, ctrl.Size.Height - 1, ctrl.Size.Width, ctrl.Size.Height - 1);
                        g.DrawLine(darkPen, 0, ctrl.Size.Height - 2, ctrl.Size.Width, ctrl.Size.Height - 2);
                        break;
                    case Native.ABEdge.Right:
                        g.DrawLine(lightPen, 0, 0, 0, ctrl.Size.Height);
                        g.DrawLine(lighterPen, 1, 0, 1, ctrl.Size.Height);
                        break;
                    case Native.ABEdge.Bottom:
                        g.DrawLine(lightPen, 0, 0, ctrl.Size.Width, 0);
                        g.DrawLine(lighterPen, 0, 1, ctrl.Size.Width, 1);
                        break;
                }
            }
        }
    }
}
