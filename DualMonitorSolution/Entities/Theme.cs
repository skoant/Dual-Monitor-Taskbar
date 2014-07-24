using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace DualMonitor.Entities
{
    public class Theme
    {
        private Color _ButtonHighlight = Color.FromArgb(100, Color.White);
        private Color _ButtonHighlightTransparent = Color.FromArgb(0, Color.White);
        private Color _TaskbarBackground = Color.FromArgb(255, 129, 148, 170);
        private SolidBrush _TooltipBackground = new SolidBrush(Color.FromArgb(150, 140, 156, 174));
        private Pen _TooltipBorder = new Pen(Color.FromArgb(255, 163, 177, 194));
        private SolidBrush _ButtonTextColor = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
        private SolidBrush _ButtonBackgroundHover = new SolidBrush(Color.FromArgb(180, 0, 102, 204));
        private SolidBrush _ButtonBackgroundFocused = new SolidBrush(Color.FromArgb(95, 255, 255, 255));
        private SolidBrush _DarkBackground = new SolidBrush(Color.FromArgb(70, Color.Black));
        private Color _Transparent = Color.FromArgb(0, 129, 148, 170);
        private Pen _TaskbarTopLine1 = new Pen(Color.FromArgb(255, 67, 77, 88));
        private Pen _TaskbarTopLine2 = new Pen(Color.FromArgb(255, 201, 216, 234));
        private Pen _ButtonOuterBorder = new Pen(Color.FromArgb(150, 61, 70, 81));
        private Pen _ButtonInnerBorder = new Pen(Color.FromArgb(150, 217, 223, 230));
        private Pen _ButtonInnerBorderLighter = new Pen(Color.FromArgb(70, 217, 223, 230));
        private SolidBrush _ClockColor = new SolidBrush(Color.White);

        private Color _DarkLineFrom = Color.FromArgb(0, 74, 87, 103);
        private Color _DarkLineTo = Color.FromArgb(255, 74, 87, 103);
        private Color _BrightLineFrom = Color.FromArgb(0, 166, 179, 193);
        private Color _BrightLineTo = Color.FromArgb(255, 166, 179, 193);

        private Color _HoverBackLightFrom = Color.FromArgb(200, 217, 243, 255);
        private Color _HoverBackLightTo = Color.FromArgb(0, 197, 223, 252);

        private Color _ButtonFlashBackgroundFrom = Color.FromArgb(255, 243, 235, 114);
        private Color _ButtonFlashBackgroundTo = Color.FromArgb(255, 227, 131, 60);

        private Color _PinPressedBackDark = Color.FromArgb(100, 0, 0, 0);

        private Color _ProgressBarErrorLight = Color.FromArgb(255, 221, 49, 49);
        private Color _ProgressBarErrorDark = Color.FromArgb(255, 160, 40, 40);

        private Color _ProgressBarNormalLight = Color.FromArgb(255, 48, 231, 24);
        private Color _ProgressBarNormalDark = Color.FromArgb(255, 78, 141, 89);

        private Color _ProgressBarPausedLight = Color.FromArgb(255, 250, 250, 21);
        private Color _ProgressBarPausedDark = Color.FromArgb(255, 149, 149, 21);

        public static Color ProgressBarNormalLight
        {
            get { return Instance._ProgressBarNormalLight; }
        }

        public static Color ProgressBarNormalDark
        {
            get { return Instance._ProgressBarNormalDark; }
        }

        public static Color ProgressBarPausedLight
        {
            get { return Instance._ProgressBarPausedLight; }
        }

        public static Color ProgressBarPausedDark
        {
            get { return Instance._ProgressBarPausedDark; }
        }

        public static Color ProgressBarErrorLight
        {
            get { return Instance._ProgressBarErrorLight; }
        }

        public static Color ProgressBarErrorDark
        {
            get { return Instance._ProgressBarErrorDark; }
        }

        public static Color PinPressedBackDark
        {
            get { return Instance._PinPressedBackDark; }
        }

        public static Color ButtonFlashBackgroundFrom
        {
            get { return Instance._ButtonFlashBackgroundFrom; }
        }

        public static Color ButtonFlashBackgroundTo
        {
            get { return Instance._ButtonFlashBackgroundTo; }
        }

        public static Color HoverBackLightFrom
        {
            get { return Instance._HoverBackLightFrom; }
        }

        public static Color HoverBackLightTo
        {
            get { return Instance._HoverBackLightTo; }
        }

        public static Color DarkLineFrom
        {
            get { return Instance._DarkLineFrom; }
        }

        public static Color DarkLineTo
        {
            get { return Instance._DarkLineTo; }
        }

        public static Color BrightLineFrom
        {
            get { return Instance._BrightLineFrom; }
        }

        public static Color BrightLineTo
        {
            get { return Instance._BrightLineTo; }
        }
     
        public static Color ButtonHighlight
        {
            get { return Instance._ButtonHighlight; }
        }

        public static Color ButtonHighlightTransparent
        {
            get { return Instance._ButtonHighlightTransparent; }
        }

        public static Color TaskbarBackground
        {
            get { return Instance._TaskbarBackground; }
        }

        public static SolidBrush TooltipBackground
        {
            get { return Instance._TooltipBackground; }
        }

        public static Pen TooltipBorder
        {
            get { return Instance._TooltipBorder; }
        }

        public static Pen TaskbarTopLine1
        {
            get { return Instance._TaskbarTopLine1; }
        }

        public static Pen TaskbarTopLine2
        {
            get { return Instance._TaskbarTopLine2; }
        }

        public static Pen ButtonOuterBorder
        {
            get { return Instance._ButtonOuterBorder; }
        }

        public static Pen ButtonInnerBorder
        {
            get { return Instance._ButtonInnerBorder; }
        }

        public static Pen ButtonInnerBorderLighter
        {
            get { return Instance._ButtonInnerBorderLighter; }
        }

        public static SolidBrush ButtonTextColor
        {
            get { return Instance._ButtonTextColor; }
        }

        public static SolidBrush ButtonBackgroundHover
        {
            get { return Instance._ButtonBackgroundHover; }
        }

        public static SolidBrush ButtonBackgroundFocused
        {
            get { return Instance._ButtonBackgroundFocused; }
        }

        public static Color Transparent
        {
            get { return Instance._Transparent; }
        }

        public static SolidBrush ClockColor
        {
            get { return Instance._ClockColor; }
        }

        public static Brush DarkBackground
        {
            get { return Instance._DarkBackground; }
        }
        
        private static Theme Instance = new Theme();
        
    }
}
