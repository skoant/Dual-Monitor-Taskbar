using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DualMonitor.Win32
{
    public class DwmApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [DllImport("dwmapi.dll")]
        public static extern void DwmGetColorizationColor(out uint ColorizationColor, out bool ColorizationOpaqueBlend); 

        /// <summary>
        /// Undocumented
        /// </summary>
        [DllImport("dwmapi.dll", EntryPoint = "#113", SetLastError = true)]
        public static extern uint DwmpActivateLivePreview(uint activate, IntPtr hWnd, uint c, uint d);

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public DwmFlags dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [Flags]
        public enum DwmFlags : int
        {
            DWM_BB_ENABLE = 0x1,
            DWM_BB_BLURREGION = 0x2,
            DWM_BB_TRANSITIONONMAXIMIZED = 0x4,
        }

        [Flags]
        public enum DWMWINDOWATTRIBUTE : uint
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_LAST
        }

        [Flags]
        public enum DWMNCRenderingPolicy
        {
            UseWindowStyle,
            Disabled,
            Enabled,
            Last
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, IntPtr attrValue, uint attrSize);

        [DllImport("dwmapi.dll", SetLastError = true)]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumbnail, ref DWM_THUMBNAIL_PROPERTIES props);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out System.Drawing.Size size);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {
            public DWM_THUMBNAIL_FLAGS dwFlags;
            public Native.RECT rcDestination;
            public Native.RECT rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        public enum DWM_THUMBNAIL_FLAGS : uint
        {
            DWM_TNP_RECTDESTINATION = 0x00000001,
            DWM_TNP_RECTSOURCE = 0x00000002,
            DWM_TNP_OPACITY = 0x00000004,
            DWM_TNP_VISIBLE = 0x00000008,
            DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010
        }
    }
}

