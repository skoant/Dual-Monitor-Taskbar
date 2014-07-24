using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DualMonitor.Entities
{
    [Serializable]
    public class CustomFont
    {
        public string Family { get; set; }
        public float Size { get; set; }
        public int Style { get; set; }
        public CustomColor Color { get; set; }

        public static explicit operator Font(CustomFont f)
        {
            return new Font(f.Family, f.Size, (FontStyle)f.Style);
        }

        public static explicit operator CustomFont(Font c)
        {
            return new CustomFont
            {
                Family = c.FontFamily.Name,
                Size = c.Size,
                Style = (int)c.Style
            };
        }
    }

    [Serializable]
    public class CustomColor
    {
        public byte R;
        public byte G;
        public byte B;

        public static explicit operator Color(CustomColor c)
        {
            return Color.FromArgb(c.R, c.G, c.B);
        }

        public static explicit operator CustomColor(Color c)
        {
            return new CustomColor
            {
                R = c.R,
                G = c.G,
                B = c.B
            };
        }
    }
}
