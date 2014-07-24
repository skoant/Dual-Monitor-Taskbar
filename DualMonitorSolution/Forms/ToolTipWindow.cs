using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;
using DualMonitor.Controls;
using DualMonitor.GraphicUtils;
using DualMonitor.Entities;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;

namespace DualMonitor.Forms
{    
    public partial class ToolTipWindow : Form
    {
        private LiveThumbnail _thumbnail;        
        private TaskbarButton _button;
        private bool _doHide;
        private bool _hideSleeping;
        private bool _panelHover;
        private bool _overCloseButton;
        private Rectangle _buttonRectangle;
        private SolidBrush _fontColor;
        private DelayedAction _delayedActionPreview;
        private DelayedAction _delayedActionTooltip;
        private ToolTip _toolTip;

        public event EventHandler<TooltipEventArgs> OnActivate;
        public event EventHandler<TooltipEventArgs> OnClose;
        public event EventHandler<TooltipEventArgs> OnCustomEnter;
        public event EventHandler<TooltipEventArgs> OnCustomLeave;

        // The only way to draw on top of DWM Thumbnail, seems to be using a different Form
        private CloseButtonForm _closeForm;

        private StringFormat _appTitleStringFormat;

        public Form CloseForm
        {
            get
            {
                return _closeForm;
            }
        }

        public bool PanelHover
        {
            get { return _panelHover; }
            set
            {
                _panelHover = value;
                panelTitle.Invalidate();            
            }
        }

        private void ShowDefaultTooltip()
        {
            string title = (_button.Tag as SecondDisplayProcess).Title;

            _toolTip.AutomaticDelay = 500;
            _toolTip.SetToolTip(panelTitle, title);
            _toolTip.ShowAlways = true;
        }
        
        public ToolTipWindow()
        {
            InitializeComponent();

            this.BackColor = Theme.TaskbarBackground;
            panelTitle.BackColor = Color.Transparent;
            _closeForm = new CloseButtonForm();
            UpdateFont();

            _toolTip = new ToolTip();
            _delayedActionPreview = new DelayedAction(this);
            _delayedActionTooltip = new DelayedAction(this);

            _appTitleStringFormat = new StringFormat(StringFormat.GenericDefault);
            _appTitleStringFormat.Trimming = StringTrimming.EllipsisCharacter;
        }

        private bool ShowPreview()
        {
            if (AeroDecorator.Instance.IsDwmCompositionEnabled)
            {
                if (WindowsConstants.ExtendedUIHoverTime >= 2000)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public void RefreshProperties()
        {
            UpdateFont();
        }

        private void UpdateFont()
        {
            var prop = TaskbarPropertiesManager.Instance.Properties;

            this.Font = prop.UseCustomFont
                ? new Font(prop.CustomFont.Family, prop.CustomFont.Size, (FontStyle)prop.CustomFont.Style)
                : SystemFonts.DefaultFont;
           
            if (Native.IsThemeActive() == 0)
            {
                this._fontColor = new SolidBrush(prop.UseCustomFont ? (Color)prop.CustomFont.Color : Color.FromKnownColor(KnownColor.ControlText));
            }
            else
            {
                this._fontColor = prop.UseCustomFont ? new SolidBrush((Color)prop.CustomFont.Color) : Theme.ButtonTextColor;
            }
        }

        public new void Hide()
        {
            if (SystemMenuProxy.IsOpening) return;
            if (!this.Visible) return;

            if (_doHide) return;

            _doHide = true;

            // thread is already waiting to hide, no need to start another one
            if (_hideSleeping) return;

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                _hideSleeping = true;
                Thread.Sleep(ThumbnailConstants.Delay);
                _hideSleeping = false;

                if (_doHide)
                {
                    this.Invoke(new MethodInvoker(ForceHide));
                }
            });
        }

        public void Show(TaskbarButton button)
        {
            CancelHide();

            PanelHover = true;
            if (button == _button)
            {
                return;
            }

            SecondDisplayProcess proc = button.Tag as SecondDisplayProcess;
            _button = button;

            if (_thumbnail != null)
            {
                _thumbnail.Dispose();
                _thumbnail = null;
            }
            
            CalculateBounds(proc);

            this.StartPosition = FormStartPosition.Manual;
            Rectangle parentBounds = button.Parent.RectangleToScreen(button.Bounds);
            Screen buttonScreen = proc.Screen;
            var taskbarLocation = TaskbarPropertiesManager.Instance.Properties.GetTaskbarLocation(buttonScreen.DeviceName);

            if (taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom)
            {
                int x = (parentBounds.Left + parentBounds.Width / 2) - this.Width / 2;
                if (x < buttonScreen.WorkingArea.Left) x = buttonScreen.WorkingArea.Left;
                if (x + this.Width > buttonScreen.WorkingArea.Right) x = buttonScreen.WorkingArea.Right - this.Width;

                int y = taskbarLocation == Native.ABEdge.Bottom ? (parentBounds.Top - this.Height) : parentBounds.Bottom;
                this.Location = new Point(x, y);
            }
            else
            {
                int x = taskbarLocation == Native.ABEdge.Left ? parentBounds.Right : (parentBounds.Left - this.Width);
                int y = (parentBounds.Top + parentBounds.Height / 2) - this.Height / 2;
                if (y < buttonScreen.WorkingArea.Top) y = buttonScreen.WorkingArea.Top;
                if (y + this.Height > buttonScreen.WorkingArea.Bottom) y = buttonScreen.WorkingArea.Bottom - this.Height;
                this.Location = new Point(x, y);
            }

            _buttonRectangle = new Rectangle(panelTitle.Width - ThumbnailConstants.IconMargin - ThumbnailConstants.CloseButWidth,
                10, ThumbnailConstants.CloseButWidth, ThumbnailConstants.CloseButWidth);
            if (this.Visible)
            {
                this.Refresh();
            }
            else
            {
                this.Visible = true;
            }
            
            _closeForm.Show();

            Point location = panelTitle.PointToScreen(new Point(panelTitle.Width - ThumbnailConstants.IconMargin - ThumbnailConstants.CloseButWidth, 10));
            _closeForm.Left = location.X;
            _closeForm.Top = location.Y;
            _closeForm.Draw(DualMonitor.Properties.Resources.close);

            ShowDefaultTooltip();
        }

        private void CalculateBounds(SecondDisplayProcess proc)
        {
            _thumbnail = LiveThumbnail.FromHandle(proc.Handle, this.Handle);

            Size size;
            if (_thumbnail != null && _thumbnail.IsValid && _thumbnail.SafeGetSize(out size))
            {
                this.Height = Math.Min(size.Height, ThumbnailConstants.MaxHeight) + panelTitle.Top * 2 + ThumbnailConstants.ThumbPadding * 2;
                this.Width = Math.Min(size.Width, ThumbnailConstants.MaxWidth) + panelTitle.Left * 2 + ThumbnailConstants.ThumbPadding * 2;

                using (GraphicsPath gp = RoundedRectangle.Create(this.ClientRectangle, 5))
                {
                    Region hrgn = new System.Drawing.Region(gp);
                    Graphics g = Graphics.FromHwnd(this.Handle);
                    Native.SetWindowRgn(this.Handle, hrgn.GetHrgn(g), true);
                }
            }
            else
            {
                this.Height = 51;
                this.Width = panelTitle.Left * 2 + CalculateWidth(proc.Title.Clamp(Constants.MaxProgramNameLength));

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRectangle(this.ClientRectangle);
                    Region hrgn = new System.Drawing.Region(gp);
                    Graphics g = Graphics.FromHwnd(this.Handle);
                    Native.SetWindowRgn(this.Handle, hrgn.GetHrgn(g), true);
                }
            }

        }

        private int CalculateWidth(string text)
        {
            int width = (int) this.CreateGraphics().MeasureString(text, this.Font).Width;
            return width + ThumbnailConstants.IconSize + ThumbnailConstants.IconMargin * 4 + ThumbnailConstants.CloseButWidth;
        }

        private void CancelHide()
        {            
            _doHide = false;
        }

        public void ForceHide()
        {
            if (SystemMenuProxy.IsOpening) return;

            _closeForm.Hide();

            base.Hide();
            if (_button != null)
            {
                OnCustomLeave(null, TooltipEventArgs.From(_button));

                ShowLivePreview(_button, false);
            }
            _button = null;
            _doHide = false;            
        }

        private void panelTitle_Click(object sender, EventArgs e)
        {
            if (_button == null) return;

            MouseEventArgs mea = e as MouseEventArgs;

            if (mea.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!_overCloseButton)
                {
                    OnActivate(sender, TooltipEventArgs.From(_button));
                    ForceHide();
                }
                else
                {
                    OnClose(sender, TooltipEventArgs.From(_button));
                    ForceHide();
                }
            }
            else if (mea.Button == System.Windows.Forms.MouseButtons.Middle) 
            {
                OnClose(sender, TooltipEventArgs.From(_button));
                ForceHide();
            }
            else if (mea.Button == System.Windows.Forms.MouseButtons.Right)
            {
                SystemMenuProxy.BeginOpenSystemMenu(_button.Tag as SecondDisplayProcess, this.Handle);
            }
        }       
        
        private void panelTitle_MouseEnter(object sender, EventArgs e)
        {
            PanelHover = true;

            CancelHide();
            if (_button == null) return;

            if (_delayedActionPreview.Active) _delayedActionPreview.Cancel();
            
            _delayedActionPreview.Init(delegate() { ShowLivePreview(_button, true); }, WindowsConstants.TaskbarLivePreviewDelay);

            OnCustomEnter(sender, TooltipEventArgs.From(_button));            
        }

        private void ShowLivePreview(TaskbarButton button, bool show)
        {
            if (!ShowPreview()) return;

            if (show)
            {
                SecondDisplayProcess proc = button.Tag as SecondDisplayProcess;
                AeroDecorator.Instance.LivePreview(proc.Handle, true);
                this.BringToFront();
                this._closeForm.BringToFront();
            }
            else
            {
                _delayedActionPreview.Cancel();
                SecondDisplayProcess proc = _button.Tag as SecondDisplayProcess;
                AeroDecorator.Instance.LivePreview(proc.Handle, false);
            }
        }

        private void panelTitle_MouseLeave(object sender, EventArgs e)
        {
            PanelHover = false;

            if (_button == null) return;
            ShowLivePreview(_button, false);
           
            OnCustomLeave(sender, TooltipEventArgs.From(_button));
        }

        private void ToolTipWindow_MouseEnter(object sender, EventArgs e)
        {
            CancelHide();
        }

        private void ToolTipWindow_MouseLeave(object sender, EventArgs e)
        {            
            this.Hide();            
        }

        private void panelTitle_Paint(object sender, PaintEventArgs e)
        {
            SecondDisplayProcess proc = _button.Tag as SecondDisplayProcess;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

            bool showLivePreview = ShowPreview();

            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (_panelHover)
            {
                if (isVisualTheme)
                {
                    GraphicsPath path = RoundedRectangle.Create(new Rectangle(0, 0, panelTitle.Width - 1, panelTitle.Height - 1), 3, RoundedRectangle.RectangleCorners.All);

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    e.Graphics.FillPath(Theme.TooltipBackground, path);
                    e.Graphics.DrawPath(Theme.TooltipBorder, path);
                }
                else
                {
                    ButtonBorderDecorator.DrawSingle(e.Graphics, 0, 0, panelTitle.Width - 1, panelTitle.Height - 1);
                }

                if (_overCloseButton)
                {
                    _closeForm.Draw(DualMonitor.Properties.Resources.close_hover);
                }
                else
                {
                    _closeForm.Draw(DualMonitor.Properties.Resources.close);
                }               
            }

            if (proc == null) return;

            if (!showLivePreview)
            {
                if (proc.SmallIcon != null)
                {
                    e.Graphics.DrawIcon(proc.SmallIcon, new Rectangle(ThumbnailConstants.IconMargin, 9, ThumbnailConstants.IconSize, ThumbnailConstants.IconSize));
                }

                if (proc.Title != null)
                {
                    e.Graphics.DrawString(proc.Title, this.Font, _fontColor,
                        new RectangleF(ThumbnailConstants.LiveIconMargin + 5 + ThumbnailConstants.IconSize, 9f, panelTitle.Width - (ThumbnailConstants.IconMargin + 5 + ThumbnailConstants.IconSize) * 2, this.Font.Height),
                        _appTitleStringFormat);
                }
            }
            else
            {
                if (proc.SmallIcon != null)
                {
                    e.Graphics.DrawIcon(proc.SmallIcon, new Rectangle(ThumbnailConstants.LiveIconMargin, 7, ThumbnailConstants.IconSize, ThumbnailConstants.IconSize));
                }

                if (proc.Title != null)
                {                    
                    e.Graphics.DrawString(proc.Title, this.Font, _fontColor, 
                        new RectangleF(ThumbnailConstants.LiveIconMargin + 5 + ThumbnailConstants.IconSize, 9f, panelTitle.Width - (ThumbnailConstants.LiveIconMargin + 5 + ThumbnailConstants.IconSize) * 2, this.Font.Height),
                        _appTitleStringFormat);
                }
            }
        }

        private void ToolTipWindow_Paint(object sender, PaintEventArgs e)
        {
            BackgroundDecorator.Paint(e.Graphics, this.Handle);

            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (isVisualTheme)
            {
                if (ShowPreview())
                {
                    GraphicsPath gp;
                    using (gp = RoundedRectangle.Create(0, 0, Width - 1, Height - 1, 7))
                    {
                        e.Graphics.DrawPath(Theme.TaskbarTopLine1, gp);
                    }
                    using (gp = RoundedRectangle.Create(1, 1, Width - 3, Height - 3, 7))
                    {
                        e.Graphics.DrawPath(Theme.TaskbarTopLine2, gp);
                    }
                }
                else
                {
                    e.Graphics.DrawRectangle(Theme.TaskbarTopLine1, 0, 0, Width - 1, Height - 1);
                    e.Graphics.DrawRectangle(Theme.TaskbarTopLine2, 1, 1, Width - 3, Height - 3);
                }
            }
            else
            {
                ButtonBorderDecorator.Draw(e.Graphics, 0, 0, Width - 2, Height - 2, false);
            }

            DrawThumbnail();            
        }

        private void DrawThumbnail()
        {
            if (ShowPreview() && _thumbnail != null && _thumbnail.IsValid)
            {
                Size size;
                if (!_thumbnail.SafeGetSize(out size)) return;

                Point location = panelTitle.Location;
                location.Offset(0, 18);
                Rectangle r = new Rectangle(location, new Size(panelTitle.Size.Width, panelTitle.Size.Height - 15));

                r.Inflate(-ThumbnailConstants.ThumbPadding, -ThumbnailConstants.ThumbPadding);

                int dx = 0;
                int dy = 0;

                int newHeight = (int)((float)size.Height * ((float)r.Width / (float)size.Width));
                int newWidth = (int)((float)size.Width * ((float)r.Height / (float)size.Height));
                if (newHeight < r.Height)
                {
                    dy = -(r.Height - newHeight) / 2;
                }
                else if (newWidth < r.Width)
                {
                    dx = -(r.Width - newWidth) / 2;
                }

                r.Inflate(dx, dy);

                try
                {
                    _thumbnail.Draw(r);
                }
                catch (Exception) // ArgumentException ?
                {
                    // ignore it - happens when the window is closed, some race condition
                }
            }
        }

        private void panelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_buttonRectangle.Contains(e.X, e.Y))
            {
                if (!_overCloseButton)
                {
                    _overCloseButton = true;
                    panelTitle.Invalidate(_buttonRectangle);
                }
            }
            else
            {
                if (_overCloseButton)
                {
                    _overCloseButton = false;
                    panelTitle.Invalidate(_buttonRectangle);
                }
            }
        }

        private void ToolTipWindow_Load(object sender, EventArgs e)
        {
            AeroDecorator.Instance.DisableLivePreview(this.Handle);
        }
    }

    public class TooltipEventArgs : EventArgs
    {
        public TaskbarButton Button { get; set; }
        public static TooltipEventArgs From(TaskbarButton button)
        {
            return new TooltipEventArgs { Button = button };
        }
    }
}
