using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DualMonitor.Entities;
using DualMonitor.Win32;
using DualMonitor.Controls;
using DualMonitor.VisualStyle;

namespace DualMonitor.Forms
{
    partial class SecondaryTaskbar
    {
        private bool fBarRegistered = false;
        private uint uCallBack;

        public bool IsRegistered
        {
            get
            {
                return fBarRegistered;
            }
        }

        public int BaseSize
        {
            get
            {
                if (Native.IsThemeActive() != 0)
                {
                    return IsBig ? Constants.BigTaskbarSize : Constants.SmallTaskbarSize;
                }
                else
                {
                    return (IsBig ? Constants.BigTaskbarSize + 4 : Constants.SmallTaskbarSize - 2);
                }
            }
        }

        private void RegisterBar()
        {
            Native.APPBARDATA abd = new Native.APPBARDATA();
            abd.cbSize = (uint)Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            if (!fBarRegistered)
            {
                uCallBack = (uint)Native.RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = uCallBack;

                uint ret = Native.SHAppBarMessage(Native.ABMsg.New, ref abd);
                fBarRegistered = true;

                ABSetPos();
            }
            else
            {
                Native.SHAppBarMessage(Native.ABMsg.Remove, ref abd);
                fBarRegistered = false;
            }
        }

        /// <summary>
        /// Change size and alert all windows that the working area has changed
        /// </summary>
        private void ABSetPos()
        {
            ABSetPos(true);
        }

        /// <summary>
        /// Change size and alert all windows that the working area has changed (if signal=true)
        /// </summary>
        private void ABSetPos(bool signal)
        {
            Native.APPBARDATA abd = new Native.APPBARDATA();
            abd.cbSize = (uint)Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            abd.uEdge = TaskbarLocation;

            int height = TaskbarPropertiesManager.Instance.Properties.HeightMultiplier * BaseSize;
            int width = TaskbarPropertiesManager.Instance.Properties.Width;

            if (abd.uEdge == (int)Native.ABEdge.Left || abd.uEdge == Native.ABEdge.Right)
            {
                if (IsHidden) width = 2;

                abd.rc.top = CurrentScreen.Bounds.Top;
                abd.rc.bottom = CurrentScreen.Bounds.Bottom;
                if (abd.uEdge == (int)Native.ABEdge.Left)
                {
                    abd.rc.left = CurrentScreen.Bounds.Left;
                    abd.rc.right = abd.rc.left + width;
                }
                else
                {
                    abd.rc.right = CurrentScreen.Bounds.Right;
                    abd.rc.left = abd.rc.right - width;
                }
            }
            else
            {
                if (IsHidden) height = 2;

                abd.rc.left = CurrentScreen.Bounds.Left;
                abd.rc.right = CurrentScreen.Bounds.Right;
                if (abd.uEdge == Native.ABEdge.Top)
                {
                    abd.rc.top = CurrentScreen.Bounds.Top;
                    abd.rc.bottom = abd.rc.top + height;
                }
                else
                {
                    abd.rc.bottom = CurrentScreen.Bounds.Bottom;
                    abd.rc.top = abd.rc.bottom - height;                    
                }
            }

            // Query the system for an approved size and position. 
            Native.SHAppBarMessage(Native.ABMsg.QueryPos, ref abd);

            // Adjust the rectangle, depending on the edge to which the 
            // appbar is anchored. 
            switch (abd.uEdge)
            {
                case Native.ABEdge.Left:
                    abd.rc.right = abd.rc.left + width;
                    break;
                case Native.ABEdge.Right:
                    abd.rc.left = abd.rc.right - width;
                    break;
                case Native.ABEdge.Top:
                    abd.rc.bottom = abd.rc.top + height;
                    break;
                case Native.ABEdge.Bottom:
                    abd.rc.top = abd.rc.bottom - height;
                    break;
            }

            if (signal)
            {
                // Pass the final bounding rectangle to the system. 
                Native.SHAppBarMessage(Native.ABMsg.SetPos, ref abd);
            }

            // Move and size the appbar so that it conforms to the 
            // bounding rectangle passed to the system. 
            Native.MoveWindow(abd.hWnd, abd.rc.left, abd.rc.top,
                abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, true);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == uCallBack)
            {
                switch (m.WParam.ToInt32())
                {
                    case (int)Native.ABNotify.ABN_POSCHANGED:
                        ABSetPos();
                        break;
                }
            }
            else if (m.Msg == (int)Native.WindowMessage.WM_DWMCOMPOSITIONCHANGED || m.Msg == (int)Native.WindowMessage.WM_DWMCOLORIZATIONCOLORCHANGED)
            {
                AeroDecorator.Instance.Init();
                this.Invalidate();
                flowPanel.Invalidate();
            }           

            base.WndProc(ref m);
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)Native.WindowStyles.WS_CAPTION;
                cp.Style &= ~(int)Native.WindowStyles.WS_BORDER;
                cp.ExStyle = (int)Native.WindowExtendedStyles.WS_EX_TOOLWINDOW | (int)Native.WindowExtendedStyles.WS_EX_TOPMOST;
                return cp;
            }
        }
    }
}
