using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Win32;

namespace DualMonitor.Entities
{
    public class Constants
    {
        public const int VerticalTaskbarWidth = 88;
        public const int BigTaskbarSize = 40;
        public const int SmallTaskbarSize = 30;

        public const int MaxProgramNameLength = 40;
        public const int LiveMaxProgramNameLength = 30;

    }

    public class ButtonConstants
    {
        public static int BigHorizontalHeight(Native.ABEdge taskbarLocation)
        {
            if (Native.IsThemeActive() != 0)
            {
                return 40;
            }
            else
            {
                if (taskbarLocation == Native.ABEdge.Top)
                {
                    return 41;
                }
                else
                {
                    return 44;
                }
            }
        }

        public static int SmallHorizontalHeight(Native.ABEdge taskbarLocation)
        {
            if (Native.IsThemeActive() != 0)
            {
                return 30;
            }
            else
            {
                if (taskbarLocation == Native.ABEdge.Top)
                {
                    return 25;
                }
                else
                {
                    return 28;
                }
            }
        }

        public const int BigVerticalHeight = 46;
        public static int SmallVerticalHeight
        {
            get
            {
                if (Native.IsThemeActive() != 0)
                {
                    return 36;
                }
                else
                {
                    return 26;
                }
            }
        }

        public const int BigIconSize = 32;
        public const int SmallIconSize = 16;

        public const int NotifIconWidth = 24;
        public const int NotifIconHeight = 28;
        public const int NotifButtonWidth = 20;

        public const int ClickedOffset = 1;

        public const int HoverDelay = 800;
        public const int SpaceAfter = 3;
        public const int BigTextPosX = 43;
        public const int SmallTextPosX = 26;

        public const int BigIconPosXWithLabel = 8;
        public const int BigIconPosXWithoutLabel = 14;

        public static int BigIconPosYWithLabel(Native.ABEdge taskbarLocation)
        {
            if (Native.IsThemeActive() != 0)
            {
                return 4;
            }
            else
            {
                if (taskbarLocation == Native.ABEdge.Top)
                {
                    return 5;
                }
                else
                {
                    return 7;
                }
            }
        }

        public static int SmallIconPosXWithLabel
        {
            get
            {
                if (Native.IsThemeActive() != 0)
                {
                    return 8;
                }
                else
                {
                    return 5;
                }
            }
        }

        public const int SmallIconPosXWithoutLabel = 14;
        public static int SmallIconPosYWithLabel(Native.ABEdge taskbarLocation)
        {
            if (Native.IsThemeActive() != 0)
            {
                return 6;
            }
            else
            {
                if (taskbarLocation == Native.ABEdge.Top)
                {
                    return 4;
                }
                else
                {
                    return 6;
                }
            }
        }

        private const int DefaultWidthWithLabel = 163;
        private static int? _buttonWithLabelWidth;
        /// <summary>
        /// Get the MinWidth from the HKCU registry
        /// </summary>
        /// <returns>The registry value</returns>
        private static int GetButtonWithLabelWidth()
        {
            if (_buttonWithLabelWidth.HasValue)
            {
                return _buttonWithLabelWidth.Value;
            }
            else
            {
                string minWidthValue = RegistryProxy.GetKey<String>(@"Control Panel\Desktop\WindowMetrics\", "MinWidth");

                // If the key exists, convert it to an integer, otherwise use the default value
                int minWidth;
                if (minWidthValue != null && Int32.TryParse(minWidthValue, out minWidth))
                    _buttonWithLabelWidth = minWidth;
                else
                    _buttonWithLabelWidth = DefaultWidthWithLabel;

                return _buttonWithLabelWidth.Value;
            }
        }

        public static int WidthWithLabel = GetButtonWithLabelWidth();

        public const int WidthWithoutLabel = 63;
        public const int SmallWidthWithoutLabel = 47;

        public static void GetWidthLimits(bool isBig, out int min, out int minThreshold, out int max)
        {
            if (isBig)
            {
                min = WidthWithoutLabel;
                minThreshold = WidthWithoutLabel + 20;
                max = WidthWithLabel;
            }
            else
            {
                min = SmallWidthWithoutLabel;
                minThreshold = SmallWidthWithoutLabel + 20;
                max = WidthWithLabel;
            }
        }
    }

    public class ThumbnailConstants
    {
        public const int IconSize = 16;
        public const int IconMargin = 11;
        public const int LiveIconMargin = 8;
        public const int CloseButWidth = 14;
        public const int Delay = 400;
        public const int ThumbPadding = 10;
        public const int MaxWidth = 196;
        public const int MaxHeight = 126;
    }

    public class WindowsConstants
    {
        private static int _taskbarLivePreviewDelay = -1;

        public static int TaskbarLivePreviewDelay
        {
            get
            {
                if (_taskbarLivePreviewDelay == -1)
                {
                    _taskbarLivePreviewDelay = 
                        RegistryProxy.GetKey<int?>(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ThumbnailLivePreviewHoverTime") ?? 400;
                }

                return _taskbarLivePreviewDelay;
            }
        }

        private static int _extendedUIHoverTime = -1;

        public static int ExtendedUIHoverTime
        {
            get
            {
                if (_extendedUIHoverTime == -1)
                {
                    _extendedUIHoverTime =
                        RegistryProxy.GetKey<int?>(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ExtendedUIHoverTime") ?? 0;
                }

                return _extendedUIHoverTime;
            }
        }
    }
}
