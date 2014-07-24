using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.VisualStyle;
using DualMonitor.Entities;
using DualMonitor.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace DualMonitor.Controls
{
    public class NotificationIcon : BaseTaskbarButton
    {
        private NotificationIconInfo _data;
        private PushPanelDecorator _decorator;
        private ToolTip _toolTip;
        private System.Windows.Forms.Timer _singleClickTimer = new System.Windows.Forms.Timer();

        public NotificationIcon(NotificationIconInfo data)
        {
            _data = data;
            _toolTip = new ToolTip();
            _toolTip.ShowAlways = true;

            _singleClickTimer.Interval = SystemInformation.DoubleClickTime;
            _singleClickTimer.Tick += new EventHandler(_singleClickTimer_Tick);
        }

        void _singleClickTimer_Tick(object sender, EventArgs e)
        {
            NotificationAreaProxy.LeftClickIcon(_data.Data);
            _singleClickTimer.Stop();
        }   

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            var taskbarLocation = this.MainForm.TaskbarLocation;

            bool leftToRight = taskbarLocation == Native.ABEdge.Top || taskbarLocation == Native.ABEdge.Bottom;

            if (taskbarLocation == Native.ABEdge.Bottom)
            {
                _toolTip.Show(_data.Tooltip, this, 0, 0);
            }
            else if (taskbarLocation == Native.ABEdge.Top)
            {
                _toolTip.Show(_data.Tooltip, this, 0, this.Height);
            }
            else
            {
                _toolTip.Show(_data.Tooltip, this, 0, -20);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _toolTip.Hide(this);
        }
        
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            Invalidate();

            if (mevent.Button == System.Windows.Forms.MouseButtons.Right)
            {
                NotificationAreaProxy.RightClickIcon(_data.Data);
            }
            else if (mevent.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // use timer to detect double clicks
                // we can't use the OnDoubleClick event because it doesn't work if we handle OnMouseDown (in base class)
                if (!_singleClickTimer.Enabled)
                {
                    _singleClickTimer.Start();
                }
                else
                {
                    NotificationAreaProxy.DefaultAction(_data.Data);
                    _singleClickTimer.Stop();
                }
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // need to handle paint message here for more control, since we are using explorer to render the icons
            if (m.Msg == (int)Native.WindowMessage.WM_PAINT)
            {
                CustomPaint();
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        protected override bool IsNearEdge(Native.ABEdge edge)
        {
            return false;
        }
     
        protected void CustomPaint()
        {
            using (Graphics g = this.CreateGraphics())
            {
                base.Paint(g);

                if (!MainForm.IsHidden)
                {
                    if (Native.IsThemeActive() != 0)
                    {
                        if (_decorator == null)
                        {
                            var taskbarLocation = this.MainForm.TaskbarLocation;

                            bool leftToRight = taskbarLocation == Native.ABEdge.Top || taskbarLocation == Native.ABEdge.Bottom;
                            _decorator = new PushPanelDecorator(true, Width, Height, Padding, this);
                        }
                        _decorator.Paint(g, _isClicked, _hover);
                    }
                    NotificationAreaProxy.PaintIcon(_data.BitmapIndex,
                        (this.Width - ButtonConstants.SmallIconSize) / 2,
                        (this.Height - ButtonConstants.SmallIconSize) / 2, this.Handle);
                }
            }

            // Tell Windows that the button is now rendered. This will prevent it from calling paint again until actually needed.
            Native.RECT rect = new Native.RECT()
            {
                left = this.ClientRectangle.Left,
                right = this.ClientRectangle.Right,
                top = this.ClientRectangle.Top,
                bottom = this.ClientRectangle.Bottom
            };
            Native.ValidateRect(this.Handle, ref rect);
        }

        public void Init()
        {
            var taskbarLocation = this.MainForm.TaskbarLocation;
            bool isLeftToRight = taskbarLocation == Native.ABEdge.Top || taskbarLocation == Native.ABEdge.Bottom;

            if (isLeftToRight)
            {
                SetDefaultHeight();
                Height = Height - 9;
                Width = ButtonConstants.NotifIconWidth;
            }
            else
            {
                Width = ButtonConstants.NotifIconWidth;
                Height = ButtonConstants.NotifIconHeight;
            }

            this.Padding = System.Windows.Forms.Padding.Empty;

            if (_decorator != null) _decorator.Dispose();
            _decorator = null;
        }
    }
}
