using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DualMonitor.Entities;
using DualMonitor.Win32;

namespace DualMonitor.GraphicUtils
{
    public static class Extensions
    {        
        /// <summary>
        /// Restrict the text length to a maximum value
        /// </summary>
        public static string Clamp(this string text, int maxCharacters)
        {
            if (text.Length > maxCharacters)
            {
                text = text.Substring(0, maxCharacters - 3) + "...";
            }
            return text;
        }

        /// <summary>
        /// Calculate closest Rectangle edge to the specified Point
        /// </summary>
        public static Native.ABEdge GetEdgeByPoint(this Rectangle bounds, Point e)
        {
            int dx = -Math.Min(0, Math.Min(bounds.Left, bounds.Right));
            int dy = -Math.Min(0, Math.Min(bounds.Top, bounds.Bottom));

            bounds.Offset(dx, dy);
            e.Offset(dx, dy);

            Point center = new Point((bounds.Left + bounds.Right) / 2, (bounds.Top + bounds.Bottom) / 2);

            int left = e.X - bounds.Left;
            int top = e.Y - bounds.Top;
            int right = bounds.Right - e.X;
            int bottom = bounds.Bottom - e.Y;

            if (top < center.Y)
            {
                if (left < center.X && left < top)
                {
                    return Native.ABEdge.Left;
                }
                else if (right < center.X && right < top)
                {
                    return Native.ABEdge.Right;
                }
                else
                {
                    return Native.ABEdge.Top;
                }
            }
            else
            {
                if (left < center.X && left < bottom)
                {
                    return Native.ABEdge.Left;
                }
                else if (right < center.X && right < bottom)
                {
                    return Native.ABEdge.Right;
                }
                else
                {
                    return Native.ABEdge.Bottom;
                }
            }

        }
    }
}
