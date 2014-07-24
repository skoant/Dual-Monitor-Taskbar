using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DualMonitor.GraphicUtils
{
    public static class Utils
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
