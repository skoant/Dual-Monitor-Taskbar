using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DualMonitor.Entities;
using System.Runtime.InteropServices;
using DualMonitor.Win32;

namespace DualMonitor.VisualStyle
{
    public class BackgroundDecorator
    {
        /// <summary>
        /// Paint the background. If Aero is activated, paint with the default color and blur it. 
        /// If Aero is not activated, paint with the theme's background color.
        /// </summary>
        public static void Paint(Graphics g, IntPtr handle)
        {
            try
            {
                if (Native.IsThemeActive() == 0)
                {
                    g.Clear(Color.FromKnownColor(KnownColor.ButtonFace));
                } 
                else if (AeroDecorator.Instance.IsDwmCompositionEnabled)
                {
                    g.Clear(AeroDecorator.Instance.DwmColorizationColor);
                    AeroDecorator.Instance.BlurWindow(handle);
                }
                else
                {
                    g.Clear(Theme.TaskbarBackground);
                }
            }
            catch (ExternalException)
            {
                // no idea why this happens, just bury it for now !
                // maybe I could invalidate the window here and force it to redraw itself later
            }
        }
    }
}
