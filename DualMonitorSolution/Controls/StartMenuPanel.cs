using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Forms;
using Gma.UserActivityMonitor;
using DualMonitor.Entities;
using DualMonitor.Win32;
using System.Drawing;
using DualMonitor.VisualStyle;

namespace DualMonitor.Controls
{
    public partial class StartMenuPanel : Panel
    {
        private SecondaryTaskbar MainForm { get { return (SecondaryTaskbar)this.FindForm(); } }

        public bool IsBig { get { return MainForm.IsBig; } }

        public StartMenuPanel()
        {
            InitializeComponent();

            this.Font = new Font(Font, FontStyle.Bold);

            this.BackColor = Color.Transparent;
            this.DoubleBuffered = true;

            this.MouseEnter += new EventHandler(StartMenuPanel_MouseEnter);
            this.MouseLeave += new EventHandler(StartMenuPanel_MouseLeave);
            this.Click += new EventHandler(StartMenuPanel_Click);
        }

        void StartMenuPanel_Click(object sender, EventArgs e)
        {
            if (!Native.IsStartMenuVisible())
            {
                Win32Window taskbar = Win32Window.FromClassName(Native.TaskbarClassName);
                Native.SendMessage(taskbar.Handle, (uint)Native.WindowMessage.WM_COMMAND, new IntPtr(Native.SC_TASKLIST), IntPtr.Zero);
            }
            else
            {
                Win32Window startMenu = Win32Window.FromClassName("DV2ControlHost");
                Native.ShowWindowAsync(startMenu.Handle, 0);
            }
        }

        void StartMenuPanel_MouseLeave(object sender, EventArgs e)
        {
            this.Hover = false;
            Invalidate();
        }

        void StartMenuPanel_MouseEnter(object sender, EventArgs e)
        {
            this.Hover = true;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (isVisualTheme)
            {
                DrawVisualThemedButton(e);
            }
            else
            {
                DrawClassicButton(e);
            }
        }

        private void DrawClassicButton(PaintEventArgs e)
        {
            int x;
            int y = this.MainForm.TaskbarLocation == Native.ABEdge.Top ? 0 : 2;

            if (IsBig)
            {
                x = (this.Width - DualMonitor.Properties.Resources.start_big_active.Width) / 2;
            }
            else
            {
                x = (this.Width - DualMonitor.Properties.Resources.start_small_hover.Width) / 2;
            }

            if (StartMenuVisible)
            {
                ButtonBorderDecorator.Draw(e.Graphics, x, y, this.Width, this.Height, true);
            }
            else
            {
                ButtonBorderDecorator.Draw(e.Graphics, x, y, this.Width, this.Height, false);
            }

            e.Graphics.DrawString("Start", Font, new SolidBrush(Color.FromKnownColor(KnownColor.ControlText)), 20, 20);
        }

        private void DrawVisualThemedButton(PaintEventArgs e)
        {
            if (AeroDecorator.Instance.IsDwmCompositionEnabled)
            {
                e.Graphics.Clear(BackColor);
            }
            else
            {
                e.Graphics.Clear(Theme.TaskbarBackground);
            }

            int y = this.MainForm.TaskbarLocation == Native.ABEdge.Top ? 0 : 2;

            if (IsBig)
            {
                int x = (this.Width - DualMonitor.Properties.Resources.start_big_active.Width) / 2;

                if (StartMenuVisible)
                {
                    e.Graphics.DrawImage(DualMonitor.Properties.Resources.start_big_active, x, y, 54, 38);
                }
                else if (Hover)
                {
                    e.Graphics.DrawImage(DualMonitor.Properties.Resources.start_big_hover, x, y, 54, 38);
                }
                else
                {
                    e.Graphics.DrawImage(DualMonitor.Properties.Resources.start_big_normal, x, y, 54, 38);
                }
            }
            else
            {
                int x = (this.Width - DualMonitor.Properties.Resources.start_small_hover.Width) / 2;

                if (Hover || StartMenuVisible)
                {
                    e.Graphics.DrawImage(DualMonitor.Properties.Resources.start_small_hover, x, y);
                }
                else
                {
                    e.Graphics.DrawImage(DualMonitor.Properties.Resources.start_small_normal, x, y);
                }
            }

            switch (this.MainForm.TaskbarLocation)
            {
                case Native.ABEdge.Left:
                    e.Graphics.DrawLine(Theme.TaskbarTopLine1, this.Size.Width - 1, 0, this.Size.Width - 1, this.Size.Height);
                    e.Graphics.DrawLine(Theme.TaskbarTopLine2, this.Size.Width - 2, 0, this.Size.Width - 2, this.Size.Height);
                    break;
                case Native.ABEdge.Top:
                    e.Graphics.DrawLine(Theme.TaskbarTopLine1, 0, this.Size.Height - 1, this.Size.Width, this.Size.Height - 1);
                    e.Graphics.DrawLine(Theme.TaskbarTopLine2, 0, this.Size.Height - 2, this.Size.Width, this.Size.Height - 2);
                    break;
                case Native.ABEdge.Right:
                    e.Graphics.DrawLine(Theme.TaskbarTopLine1, 0, 0, 0, this.Size.Height);
                    e.Graphics.DrawLine(Theme.TaskbarTopLine2, 1, 0, 1, this.Size.Height);
                    break;
                case Native.ABEdge.Bottom:
                    e.Graphics.DrawLine(Theme.TaskbarTopLine1, 0, 0, this.Size.Width, 0);
                    e.Graphics.DrawLine(Theme.TaskbarTopLine2, 0, 1, this.Size.Width, 1);
                    break;
            }  
        }

        public bool Hover { get; set; }

        public bool StartMenuVisible { get; set; }
    }
}
