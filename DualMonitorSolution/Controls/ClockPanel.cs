using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Entities;
using Microsoft.Win32;
using DualMonitor.Forms;
using System.Drawing.Drawing2D;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;

namespace DualMonitor.Controls
{
    public partial class ClockPanel : Panel
    {
        public static readonly int PanelWidth = 70;

        public SecondaryTaskbar MainForm { get { return this.FindForm() as SecondaryTaskbar; } }

        private SolidBrush _fontColor;
        protected bool _isClicked;
        protected bool _hover = false;

        private PushPanelDecorator _decorator;

        private Timer _timer;
        private ToolTip _toolTip;
        private StringFormat _format;

        public ClockPanel()
        {
            InitializeComponent();

            this.BackColor = Color.Transparent;
            this.DoubleBuffered = true;

            this.VisibleChanged += new EventHandler(ClockPanel_VisibleChanged);
            this.MouseHover += new EventHandler(ClockPanel_MouseHover);

            _toolTip = new ToolTip();

            StartTimer();
          
            SystemEvents.TimeChanged += new EventHandler(SystemEvents_TimeChanged);

            _format = (StringFormat.GenericDefault.Clone() as StringFormat);
            _format.LineAlignment = StringAlignment.Center;
            _format.Alignment = StringAlignment.Center;

            UpdateFont();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            _isClicked = false;

            _toolTip.Hide(this);
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            if (mevent.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ClockPanelProxy.ShowClockPanel();

                _isClicked = false;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {           
            base.OnMouseDown(mevent);

            if (mevent.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _isClicked = true;
                Invalidate();
            }
        }

        void ClockPanel_MouseHover(object sender, EventArgs e)
        {
            var taskbarLocation = this.MainForm.TaskbarLocation;

            bool leftToRight = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;

            int y = 0;

            if (taskbarLocation == Native.ABEdge.Top)
            {
                y = this.Height;
            }
            else if (taskbarLocation != Native.ABEdge.Bottom)
            {
                y = -20;
            }

            _toolTip.Show(DateTime.Now.ToLongDateString(), this, 0, y);
        }

        void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                _timer_Tick(sender, e);
            }
        }

        void ClockPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                StartTimer();
            }
            else
            {
                StopTimer();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_decorator != null) _decorator.Dispose();
            _decorator = null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // due to a bug, sometimes Paint method is called when Height = 2 
            // this causes PushPanelDecorator : LinearGradientBrush to throw OutOfMemory exception because the two points are equal (framework bug !?)
            if (Width <= 5 || Height <= 5) return; 

            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (isVisualTheme)
            {
                base.OnPaint(e);
            }

            if (MainForm.IsHidden) return;

            if (isVisualTheme)
            {
                if (_decorator == null)
                {
                    var taskbarLocation = this.MainForm.TaskbarLocation;

                    bool leftToRight = taskbarLocation == Native.ABEdge.Top
                        || taskbarLocation == Native.ABEdge.Bottom;

                    _decorator = new PushPanelDecorator(leftToRight, Width, Height, Padding.Empty, this);
                }
                _decorator.Paint(e.Graphics, _isClicked, _hover);
            }

            IntPtr hdc = e.Graphics.GetHdc();
            Native.SetTextCharacterExtra(hdc, 0);
            e.Graphics.ReleaseHdc(hdc);

            
            if (isVisualTheme)
            {
               
                string str;

                str = DateTime.Now.ToShortTimeString();
                if (this.Parent.Parent.Height > Constants.BigTaskbarSize)
                {
                    str += Environment.NewLine + DateTime.Now.ToString("dddd") + Environment.NewLine + DateTime.Now.ToShortDateString();
                }
                else if (this.Parent.Parent.Height == Constants.BigTaskbarSize)
                {
                    str += Environment.NewLine + DateTime.Now.ToShortDateString();
                }

                if (AeroDecorator.Instance.IsDwmCompositionEnabled)
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                }
                else
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                }
                e.Graphics.DrawString(str, Font, _fontColor, this.Width / 2, this.Height / 2 + 2, _format);
            }
            else
            {
                string str;

                str = DateTime.Now.ToShortTimeString();
                if (!TaskbarPropertiesManager.Instance.Properties.SmallIcons)
                {
                    var taskbarLocation = this.MainForm.TaskbarLocation;

                    if (TaskbarPropertiesManager.Instance.Properties.HeightMultiplier > 1
                        || taskbarLocation == Native.ABEdge.Left
                        || taskbarLocation == Native.ABEdge.Right)
                    {
                        str += Environment.NewLine + DateTime.Now.ToString("dddd") + Environment.NewLine + DateTime.Now.ToShortDateString();
                    }
                    else
                    {
                        str += Environment.NewLine + DateTime.Now.ToShortDateString();
                    }
                }
                else
                {
                    var taskbarLocation = this.MainForm.TaskbarLocation;

                    if (TaskbarPropertiesManager.Instance.Properties.HeightMultiplier >= 3
                        || taskbarLocation == Native.ABEdge.Left
                        || taskbarLocation == Native.ABEdge.Right)
                    {
                        str += Environment.NewLine + DateTime.Now.ToString("dddd") + Environment.NewLine + DateTime.Now.ToShortDateString();
                    }
                    else if (TaskbarPropertiesManager.Instance.Properties.HeightMultiplier == 2)
                    {
                        str += Environment.NewLine + DateTime.Now.ToShortDateString();
                    }
                }
                
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                Font font = new System.Drawing.Font(Font.FontFamily, Font.Size - 0.5f);
                e.Graphics.DrawString(str, font, _fontColor, this.Width / 2, this.Height / 2, _format);
            }
        }

        private void StopTimer()
        {
            if (_timer != null) _timer.Stop();
        }

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Tick += new EventHandler(_timer_Tick);
            }

            if (!_timer.Enabled)
            {
                _timer.Interval = GetMillisecondsToWait();
                _timer.Start();
            }
        }

        private int GetMillisecondsToWait()
        {           
            int secInternal;
            var now = DateTime.Now;

            if (System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern.ToLower().Contains("ss"))
            {
                secInternal = 1;
            }
            else
            {
                secInternal = 60 - now.Second;
            }
            
            var next = now.AddSeconds(secInternal);
            int ms = (int)(new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, next.Second, 0) - now).TotalMilliseconds;
            if (ms <= 0) ms = 1;

            return ms;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Interval = GetMillisecondsToWait();
            _timer.Start();

            Invalidate();
        }


        internal void RefreshProperties()
        {
            UpdateFont();
        }

        private void UpdateFont()
        {
            var prop = TaskbarPropertiesManager.Instance.Properties;

            this.Font = prop.UseCustomFont
                ? new Font(prop.CustomFont.Family, prop.CustomFont.Size, (FontStyle)prop.CustomFont.Style)
                : SystemFonts.IconTitleFont;

            if (Native.IsThemeActive() == 0)
            {
                this._fontColor = new SolidBrush(prop.UseCustomFont ? (Color)prop.CustomFont.Color : Color.FromKnownColor(KnownColor.ControlText));
            }
            else
            {
                this._fontColor = prop.UseCustomFont ? new SolidBrush((Color)prop.CustomFont.Color) : Theme.ClockColor;
            }
        }
    }
}
