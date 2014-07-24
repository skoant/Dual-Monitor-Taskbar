using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DualMonitor.Win32;
using System.Runtime.InteropServices;

namespace DualMonitor.VisualStyle
{
    public class AeroDecorator
    {
        public static AeroDecorator Instance = new AeroDecorator();

        private AeroDecorator() { }

        private DwmApi.DWM_BLURBEHIND _bbParams;
        public Color DwmColorizationColor { get; private set; }
        public bool IsDwmCompositionEnabled { get; private set; }

        /// <summary>
        /// Initialize Dwm Composition parameters
        /// </summary>
        public void Init()
        {
            bool dwmEnabled;
            DwmApi.DwmIsCompositionEnabled(out dwmEnabled);

            IsDwmCompositionEnabled = dwmEnabled;

            if (dwmEnabled)
            {
                uint color;
                bool opaqueBlend;
                DwmApi.DwmGetColorizationColor(out color, out opaqueBlend);

                DwmColorizationColor = Color.FromArgb((int)color);
                if (DwmColorizationColor.A < 100)
                    DwmColorizationColor = Color.FromArgb(100, DwmColorizationColor);
            }

            _bbParams = new DwmApi.DWM_BLURBEHIND();
            _bbParams.dwFlags = DwmApi.DwmFlags.DWM_BB_ENABLE;
            _bbParams.fEnable = true;
            _bbParams.hRgnBlur = IntPtr.Zero;
        }

        /// <summary>
        /// Activate/Disable live preview for current window. This window will remain visible and all others will fade out
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="flag"></param>
        public void LivePreview(IntPtr hwnd, bool flag)
        {
            if (!IsDwmCompositionEnabled) return;
            DwmApi.DwmpActivateLivePreview((uint)(flag ? 1 : 0), hwnd, 0, 1);
        }

        /// <summary>
        /// Activate the glass effect for this window
        /// </summary>
        public void BlurWindow(IntPtr hwnd)
        {
            DwmApi.DwmEnableBlurBehindWindow(hwnd, ref _bbParams);
        }

        /// <summary>
        /// Get handle for a window's thumbnail
        /// </summary>
        public bool RegisterThumbnail(IntPtr destHandle, IntPtr srcHandle, out IntPtr thumbnail)
        {
            thumbnail = IntPtr.Zero;
            if (!IsDwmCompositionEnabled) return false;

            return DwmApi.DwmRegisterThumbnail(destHandle, srcHandle, out thumbnail) == 0;
        }

        /// <summary>
        /// Get thumbnail size
        /// </summary>
        public void GetThumbnailSize(IntPtr thumbnail, out Size size)
        {            
            DwmApi.DWM_THUMBNAIL_PROPERTIES dskThumbProps = new DwmApi.DWM_THUMBNAIL_PROPERTIES();
            dskThumbProps.dwFlags = DwmApi.DWM_THUMBNAIL_FLAGS.DWM_TNP_SOURCECLIENTAREAONLY;
            dskThumbProps.fSourceClientAreaOnly = true;
            dskThumbProps.fVisible = false;
            dskThumbProps.opacity = 255;

            DwmApi.DwmUpdateThumbnailProperties(thumbnail, ref dskThumbProps);

            DwmApi.DwmQueryThumbnailSourceSize(thumbnail, out size);
        }

        /// <summary>
        /// Make thumbnail visible
        /// </summary>
        public void DrawThumbnail(Rectangle r, IntPtr thumbnail)
        {
            Native.RECT dest = new Native.RECT()
            {
                left = r.Left,
                top = r.Top,
                right = r.Right,
                bottom = r.Bottom
            };

            DwmApi.DWM_THUMBNAIL_PROPERTIES dskThumbProps = new DwmApi.DWM_THUMBNAIL_PROPERTIES();
            dskThumbProps.dwFlags = DwmApi.DWM_THUMBNAIL_FLAGS.DWM_TNP_SOURCECLIENTAREAONLY 
                | DwmApi.DWM_THUMBNAIL_FLAGS.DWM_TNP_VISIBLE
                | DwmApi.DWM_THUMBNAIL_FLAGS.DWM_TNP_RECTDESTINATION;
            dskThumbProps.fSourceClientAreaOnly = true;
            dskThumbProps.fVisible = true;
            dskThumbProps.opacity = 255;
            dskThumbProps.rcDestination = dest;

            DwmApi.DwmUpdateThumbnailProperties(thumbnail, ref dskThumbProps);
        }

        /// <summary>
        /// Prevent this window from fading when Live Preview or Aero Peek is performed
        /// </summary>
        public void DisableLivePreview(IntPtr hwnd)
        {
            var status = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(status, 1); // true
            int res = DwmApi.DwmSetWindowAttribute(hwnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_EXCLUDED_FROM_PEEK, status, sizeof(int));
        }
    }
}
