using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DualMonitor.Entities;
using DualMonitor.GraphicUtils;

namespace DualMonitor.VisualStyle
{
    public class HotTrackDecorator
    {
        private static bool? _taskbarAnimations;
        public static bool IsTaskbarAnimated
        {
            get
            {
                if (_taskbarAnimations.HasValue) return _taskbarAnimations.Value;

                _taskbarAnimations = RegistryProxy.GetKey<int>("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\", "TaskbarAnimations") == 1;
                return _taskbarAnimations.Value;
            }
        }

        private static Color ConvertToColor(Bitmap bitmap, Rectangle bounds)
        {
            int tr = 0;
            int tg = 0;
            int tb = 0;

            // average icon's most saturated colors
            int count = 0;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(x, y);

                    if (!IsColorSaturated(pixel))
                    {
                        continue;
                    }

                    tr += pixel.R;
                    tg += pixel.G;
                    tb += pixel.B;

                    count++;
                }
            }

            if (count == 0) return Color.Empty;

            byte r = (byte)Math.Floor((double)(tr / count));
            byte g = (byte)Math.Floor((double)(tg / count));
            byte b = (byte)Math.Floor((double)(tb / count));

            // if it's not "colored" enough, use default color from registry

            Color value = Color.FromArgb(255, r, g, b);

            if (!IsColorSaturated(value))
            {
                return Color.Empty;
            }

            return value;
        }

        /// <summary>
        /// Paint the hot track effect using the icon to determine the representative color
        /// </summary>
        public static void Paint(Graphics g, Entities.SecondDisplayProcess proc, Bitmap bitmap, Rectangle _buttonBounds, Point point)
        {
            if (proc.HotTrackBrush == Color.Empty && bitmap != null)
            {
                proc.HotTrackBrush = HotTrackDecorator.ConvertToColor(bitmap, _buttonBounds);
            }

            if (proc.HotTrackBrush == Color.Empty)
            {
                proc.HotTrackBrush = Theme.ButtonBackgroundHover.Color;
            }

            int dx = 0;
            if (point.X == 0) point.X = _buttonBounds.X;
            if (point.X >= _buttonBounds.Right) point.X = _buttonBounds.Right - 1;
            if (IsTaskbarAnimated && _buttonBounds.Contains(point))
            {
                dx = point.X - _buttonBounds.Left - _buttonBounds.Width / 2;
            }
            
            var originalclip = g.Clip;
            g.Clip = new Region(_buttonBounds);            
            
            g.FillGradientEllipse(proc.HotTrackBrush, Color.FromArgb(0, proc.HotTrackBrush),
                new Rectangle(_buttonBounds.Left - 2 * _buttonBounds.Width / 3 + dx, _buttonBounds.Top - _buttonBounds.Height / 2, 
                    7 * _buttonBounds.Width / 3, _buttonBounds.Height * 3));

            g.FillGradientEllipse(Theme.ButtonHighlight, Theme.ButtonHighlightTransparent,
                new Rectangle(_buttonBounds.Left + dx, _buttonBounds.Bottom - _buttonBounds.Height / 2 + 5, _buttonBounds.Width, _buttonBounds.Height));

            g.Clip = originalclip;            
        }

        /// <summary>
        /// Attempts to determine if a color is "colored" enough to be used as hot track
        /// </summary>
        private static bool IsColorSaturated(Color color)
        {
            float brightness = color.GetBrightness();
            float saturation = color.GetSaturation();

            if (saturation < .25) return false;
            if (saturation < .8) return brightness < .8;
            return true;
        }
    }
}
