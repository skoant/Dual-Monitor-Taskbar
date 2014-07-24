using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.VisualStyle;
using DualMonitor.Win32;
using System.Drawing.Imaging;
using DualMonitor.Entities;
using System.Threading;

namespace DualMonitor.Forms
{
    public partial class StartButton : PerPixelAlphaForm
    {
        public enum State
        {
            Normal,
            Hover,
            Active
        }

        private Point _location;
        private State _state;
        private SecondaryTaskbar _mainForm;

        public StartButton(SecondaryTaskbar mainForm)
        {            
            InitializeComponent();

            _mainForm = mainForm;
            this.Font = new System.Drawing.Font(Font, FontStyle.Bold);
            _state = State.Normal;
            this.Load += new EventHandler(StartButton_Load);
            this.ShowInTaskbar = false;
        }

        void StartButton_Load(object sender, EventArgs e)
        {
            AeroDecorator.Instance.DisableLivePreview(this.Handle);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // click through
                cp.ExStyle |= (int)Native.WindowExtendedStyles.WS_EX_LAYERED;
                return cp;
            }
        }

        public void Draw()
        {
            if (_mainForm.IsHidden) return;

            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (isVisualTheme)
            {
                Bitmap bmp = null;

                switch (_state)
                {
                    case State.Normal:
                        bmp = DualMonitor.Properties.Resources.start_big_normal;
                        break;
                    case State.Hover:
                        bmp = DualMonitor.Properties.Resources.start_big_hover;
                        break;
                    case State.Active:
                        bmp = DualMonitor.Properties.Resources.start_big_active;
                        break;
                    default:
                        return;
                }

                using (Bitmap b = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb))
                {
                    Graphics g = Graphics.FromImage(b);
                    g.DrawImage(bmp, 0, 0);

                    base.SetBitmap(b);
                }
            }
            else
            {                
                using (Bitmap b = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb))
                {
                    Graphics g = Graphics.FromImage(b);

                    g.Clear(Color.FromKnownColor(KnownColor.ButtonFace));
                    if (_state == State.Active)
                    {
                        ButtonBorderDecorator.Draw(g, 0, 0, this.Width - 4, this.Height - 2, true);
                    }
                    else
                    {
                        ButtonBorderDecorator.Draw(g, 0, 0, this.Width - 4, this.Height - 2, false);
                    }

                    g.DrawImage(Properties.Resources.start_classic, 1, (this.Height - Properties.Resources.start_classic.Height) / 2);

                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

                    SizeF sizeOfText = g.MeasureString("Start", Font);

                    g.DrawString("Start", Font, new SolidBrush(Color.FromKnownColor(KnownColor.ControlText)), 16, (this.Height - sizeOfText.Height) / 2);

                    base.SetBitmap(b);
                }
            }
        }

        private void CalculatePosition(SecondaryTaskbar form)
        {
            var taskbarLocation = form.TaskbarLocation;
            if (Native.IsThemeActive() == 0)
            {
                if (taskbarLocation == Win32.Native.ABEdge.Top)
                {                    
                    this._location = new Point(form.Location.X + 2, form.Location.Y + 3);
                }
                else if (taskbarLocation == Win32.Native.ABEdge.Bottom)
                {
                    this._location = new Point(form.Location.X + 2, form.Location.Y + 4);
                }
                else if (taskbarLocation == Native.ABEdge.Right)
                {
                    this._location = new Point(form.Location.X + 3, form.Location.Y + 3);
                }
                else
                {
                    this._location = new Point(form.Location.X + form.Width - this.Width - 2, form.Location.Y + 3);
                }
            }
            else
            {
                if (taskbarLocation == Win32.Native.ABEdge.Top
                       || taskbarLocation == Win32.Native.ABEdge.Bottom)
                {
                    int yoffset = 0;
                    if (!TaskbarPropertiesManager.Instance.Properties.SmallIcons)
                    {
                        yoffset = 1;
                    }
                    else if (form.Height < this.Height)
                    {
                        if (taskbarLocation == Win32.Native.ABEdge.Bottom)
                        {
                            yoffset = -2;
                        }
                        else
                        {
                            yoffset = 4;
                        }
                    }

                    this._location = new Point(form.Location.X, form.Location.Y + (form.Height - this.Height) / 2 + yoffset);
                }
                else
                {
                    this._location = new Point(form.Location.X + (form.Width - this.Width) / 2, form.Location.Y + 4);
                }
            }            
        }

        private void CalculateSize()
        {
            if (Native.IsThemeActive() != 0)
            {
                this.Width = 54;
                this.Height = 38;
            }
            else
            {
                this.Width = 52;
                var taskbarLocation = _mainForm.TaskbarLocation;

                if (TaskbarPropertiesManager.Instance.Properties.SmallIcons)
                {                    
                    if (taskbarLocation == Native.ABEdge.Top)
                    {
                        this.Height = 21;
                    }
                    else
                    {
                        this.Height = 23;
                    }
                }
                else
                {
                    if (taskbarLocation == Native.ABEdge.Top)
                    {
                        this.Height = 37;
                    }
                    else
                    {
                        this.Height = 39;
                    }
                }
            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            this.Location = this._location;
            this.CalculateSize();
            Draw();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (Native.IsStartMenuVisible())
            {
                _state = State.Active;
                Draw();
            }
            else if (_state == State.Normal)
            {
                _state = State.Hover;
                Draw();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_state == State.Hover)
            {
                _state = State.Normal;
                Draw();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (_state == State.Hover)
            {
                if (!Native.IsStartMenuVisible())
                {
                    Win32Window taskbar = Win32Window.FromClassName(Native.TaskbarClassName);
                    Native.SendMessage(taskbar.Handle, (uint)Native.WindowMessage.WM_COMMAND, new IntPtr(Native.SC_TASKLIST), IntPtr.Zero);
                }
            }
            else 
            {
                _mainForm.Focus();
                this.Focus();
            }
        }

        public void SetActiveState(bool visible)
        {
            this._state = visible ? State.Active : State.Normal;
            Draw();
        }

        public void UpdateAndShow(SecondaryTaskbar form)
        {
            if (Native.IsStartMenuVisible())
            {
                this._state = State.Active;
            }
            else
            {
                this._state = State.Normal;
            }

            this.CalculateSize();

            this.Show();

            this.TopMost = true;

            this.CalculatePosition(form);

            this.Location = this._location;
            this.Draw();
        }
    }
}
