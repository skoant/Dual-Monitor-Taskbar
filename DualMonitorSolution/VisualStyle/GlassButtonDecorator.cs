using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DualMonitor.GraphicUtils;
using DualMonitor.Entities;
using System.Drawing.Imaging;
using DualMonitor.Win32;

namespace DualMonitor.VisualStyle
{
    public class GlassButtonDecorator
    {        
        /// <summary>
        /// Paint the highlight effect on show desktop buttons
        /// </summary>
        public static void Paint(Graphics graphics, Native.ABEdge taskbarLocation, Rectangle bounds, bool hover, bool isClicked)
        {
            if (!hover)
            {
                graphics.FillRectangle(Theme.DarkBackground, bounds);
            }

            if (taskbarLocation == Win32.Native.ABEdge.Top
                || taskbarLocation == Win32.Native.ABEdge.Bottom)
            {
                graphics.DrawImage(Properties.Resources.show_desktop_vertical, bounds.Location);
                graphics.FillGradientRectangle(Theme.ButtonHighlightTransparent, Theme.ButtonHighlight,
                    new Rectangle(bounds.X + 1, bounds.Bottom - 15, bounds.Width, 16), LinearGradientMode.Vertical);
            }
            else
            {
                graphics.DrawImage(Properties.Resources.show_desktop_horizontal, bounds.Location);
                graphics.FillGradientRectangle(Theme.ButtonHighlightTransparent, Theme.ButtonHighlight,
                    new Rectangle(bounds.Y - 30, bounds.Top, 31, bounds.Height), LinearGradientMode.Horizontal);
            }

            graphics.DrawRectangle(Theme.ButtonOuterBorder, new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1));
            graphics.DrawRectangle(Theme.ButtonInnerBorderLighter, new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 3, bounds.Height - 3));

            if (isClicked)
            {
                graphics.FillRectangle(Theme.ButtonBackgroundFocused, 2, 2, bounds.Width - 2, bounds.Height - 2);
            }
        }        
    }
}
