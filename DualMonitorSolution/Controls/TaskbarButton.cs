using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using DualMonitor.GraphicUtils;
using DualMonitor.Entities;
using DualMonitor.Forms;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;
using System.Windows.Forms.VisualStyles;

namespace DualMonitor.Controls
{
    public partial class TaskbarButton : BaseTaskbarButton
    {
        private static Timer _timer;
        private bool _flashActive;
        private ToolTipManager _tooltipManager;

        private SolidBrush _textColor;

        private string _overlayIconPath;
        private Icon _overlayIcon;
        private System.Threading.Timer _overlayTimer;

        private TaskbarProgress _progress;

        static TaskbarButton() 
        {
            _timer = new Timer();
            _timer.Interval = ButtonConstants.HoverDelay;            
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        static void _timer_Tick(object sender, EventArgs e)
        {
            if (_timer.Tag != null)
            {
                TaskbarButton button = _timer.Tag as TaskbarButton;
                button.OnCustomHover();                    
            }

            _timer.Stop();
        }

        private void OnCustomHover()
        {
            if (!this.ContextMenuStrip.Visible)
            {
                _tooltipManager.ToolTipWindow.Show(this);
            }
        }

        private StringFormat _format;
        private Rectangle _buttonBounds;
        
        private bool _showLabel;
        private int? _maxWidth;

        public bool Flashing
        {
            get
            {
                return _flashActive;
            }
        }

        public bool ShowLabel 
        { 
            get { return _showLabel; }
            set
            {
                _showLabel = value;
                CalculateWidth();                
            }
        }

        public int? MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (value == ButtonConstants.WidthWithLabel) _maxWidth = null;
                else _maxWidth = value;
                CalculateWidth();
            }
        }

        private void CalculateWidth()
        {
            this.Width = ShowLabel 
                ? Math.Min((MaxWidth ?? int.MaxValue), ButtonConstants.WidthWithLabel) 
                : (IsBig ? ButtonConstants.WidthWithoutLabel : ButtonConstants.SmallWidthWithoutLabel);
        }

        public TaskbarButton(ToolTipManager tooltipManager)
            : base()
        {
            this.EnableDragging = true;
            this.AddedToTaskbar = DateTime.Now;

            _tooltipManager = tooltipManager;            
            InitializeComponent();

            _overlayIconPath = null;
            _overlayIcon = null;
            _progress = new TaskbarProgress();

            this.AllowDrop = true;

            _format = (StringFormat.GenericDefault.Clone() as StringFormat);
            _format.Trimming = StringTrimming.EllipsisCharacter;
            _format.LineAlignment = StringAlignment.Center;
            _format.FormatFlags = _format.FormatFlags | StringFormatFlags.NoWrap;

            UpdateFont();            
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            this.Hover = false;
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            this.Hover = true;

            var process = this.Tag as SecondDisplayProcess;
            if (process == null) return;

            process.ActivateWindow(false);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            var process = this.Tag as SecondDisplayProcess;
            if (process == null) return;

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                WindowManager.Instance.LaunchProcess(process, this.MainForm.CurrentScreen);
                return;
            }

            if (this != this.MainForm.CurrentButton)
            {
                process.ActivateWindow(false);
            }
            else
            {
                process.ActivateWindow(true);
            }    
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            PrepareBounds();            
        }

        private void PrepareBounds()
        {
            var taskbarLocation = this.MainForm.TaskbarLocation;

            bool horizontal = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;
            int spaceAfterX = horizontal ? ButtonConstants.SpaceAfter : 1;
            int spaceAfterY = horizontal ? 0 : ButtonConstants.SpaceAfter;

            bool isVisualTheme = Native.IsThemeActive() != 0;
            if (!isVisualTheme)
            {
                if (horizontal)
                {
                    if (taskbarLocation == Native.ABEdge.Bottom)
                    {
                        _buttonBounds = new Rectangle(0, 5, Width - spaceAfterX - 1, Height - spaceAfterY - 6);
                    }
                    else
                    {
                        _buttonBounds = new Rectangle(0, 3, Width - spaceAfterX - 1, Height - spaceAfterY - 6);
                    }
                }
                else
                {
                    _buttonBounds = new Rectangle(0, 4, Width - spaceAfterX - 1, Height - spaceAfterY - 2);
                }
            }
            else
            {
                _buttonBounds = new Rectangle(1, 1, Width - spaceAfterX - 1, Height - spaceAfterY - 1);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (_tooltipManager.ToolTipWindow.Visible)
            {
                _tooltipManager.ToolTipWindow.PanelHover = true;

                OnCustomHover();
            }
            else
            {
                _timer.Tag = this;
                _timer.Start();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _tooltipManager.ToolTipWindow.PanelHover = false;
            _tooltipManager.ToolTipWindow.Hide();

            _timer.Tag = null;
            _timer.Stop();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            if (mevent.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                WindowManager.Instance.LaunchProcess(this.Tag as SecondDisplayProcess, this.MainForm.CurrentScreen);
            }
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);

            if (!Hover)
            {
                Hover = true; // ensure hover is true while roll over
            }
            else
            {
                // if hover is true but animations are active we need to keep drawing on each mouse move
                if (HotTrackDecorator.IsTaskbarAnimated)
                {
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {            
            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (_buttonBounds == Rectangle.Empty)
            {
                PrepareBounds();
            }

            if (isVisualTheme)
            {
                PaintVisual(pevent);
            }
            else
            {
                PaintClassic(pevent);
            }
        }

        private void PaintClassic(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            base.Paint(g);

            var taskbarLocation = this.MainForm.TaskbarLocation;
            bool horizontal = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;

            int spaceAfterX = horizontal ? ButtonConstants.SpaceAfter : 1;
            int spaceAfterY = horizontal ? 0 : ButtonConstants.SpaceAfter;

            if (!horizontal) spaceAfterY = -1;

            Pen darkPen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));
            Pen lighterPen = new Pen(Color.FromKnownColor(KnownColor.ControlLightLight));
            Pen darkerPen = new Pen(Color.FromKnownColor(KnownColor.ControlDarkDark));
            Pen lightPen = new Pen(Color.FromKnownColor(KnownColor.ControlLight));

            if (!MainForm.IsHidden)
            {
                // paint background
                ProgressBarDecorator.Paint(g, _buttonBounds, _progress);

                if (_flashActive)
                {
                    g.FillRectangle(new LinearGradientBrush(_buttonBounds, Theme.ButtonFlashBackgroundFrom, Theme.ButtonFlashBackgroundTo, 90f), _buttonBounds);
                }
                                        
                if (this.Focused || this._isClicked)
                {
                    if (!_flashActive)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromKnownColor(KnownColor.ButtonHighlight)), _buttonBounds);
                    }

                    #region Border
                    // draw border over background

                    if (this.MainForm.TaskbarLocation == Native.ABEdge.Top)
                    {
                        ButtonBorderDecorator.Draw(g, 0, 3, Width - spaceAfterX - 1, Height - spaceAfterY - 3, true);
                    }
                    else
                    {
                        ButtonBorderDecorator.Draw(g, 0, 4, Width - spaceAfterX - 1, Height - spaceAfterY - 3, true);
                    }
                    #endregion
                }
            }

            if (!this.Focused && !this._isClicked)
            {              
                #region Border
                // draw border over background     
                if (taskbarLocation == Native.ABEdge.Top)
                {
                    ButtonBorderDecorator.Draw(g, 0, 3, Width - spaceAfterX - 1, Height - spaceAfterY - 3, false);
                }
                else
                {
                    ButtonBorderDecorator.Draw(g, 0, 4, Width - spaceAfterX - 1, Height - spaceAfterY - 3, false);
                }

                #endregion
            }

            if (MainForm.IsHidden) return;

            DrawImageAndText(g, horizontal, spaceAfterX, spaceAfterY);            
        }

        private void DrawImageAndText(Graphics g, bool horizontal, int spaceAfterX, int spaceAfterY)
        {
            bool showLabel = ShowLabel && (this.Width > (IsBig ? ButtonConstants.WidthWithoutLabel : ButtonConstants.SmallWidthWithoutLabel));

            var taskbarLocation = this.MainForm.TaskbarLocation;

            int iconPosY = !IsBig ? ButtonConstants.SmallIconPosYWithLabel(taskbarLocation) : ButtonConstants.BigIconPosYWithLabel(taskbarLocation);
            if (!horizontal)
            {
                iconPosY += 1;
            }

            bool renderImage = this.Image != null;

            #region Draw text and image
            if (showLabel)
            {
                int iconPosX = !IsBig
                    ? (showLabel ? ButtonConstants.SmallIconPosXWithLabel : ButtonConstants.SmallIconPosXWithoutLabel)
                    : (showLabel ? ButtonConstants.BigIconPosXWithLabel : ButtonConstants.BigIconPosXWithoutLabel);
                int textPosX = !IsBig ? ButtonConstants.SmallTextPosX : ButtonConstants.BigTextPosX;
                if (renderImage)
                {
                    Point location = new Point(iconPosX + (_isClicked ? ButtonConstants.ClickedOffset : 0), iconPosY + (_isClicked ? ButtonConstants.ClickedOffset : 0));
                    g.DrawImage(this.Image, location);

                    // paint overlay icon, if exists
                    if (IsBig && this._overlayIcon != null)
                    {
                        location.Offset(16, 16);
                        g.DrawIcon(this._overlayIcon, new Rectangle(location, new Size(16, 16)));
                    }
                }

                Rectangle textRect = new Rectangle(textPosX, 0, Width - (textPosX + spaceAfterX + iconPosX), Height - spaceAfterY + 2);
                if (_isClicked)
                {
                    textRect.Offset(ButtonConstants.ClickedOffset, ButtonConstants.ClickedOffset);
                }

                if (AeroDecorator.Instance.IsDwmCompositionEnabled)
                {
                    // need to find a way to use antialiasing when Aero is on
                    // this doesn't look perfect, but better.
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                }
                else
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                }

                if (Native.IsThemeActive() == 0)
                {
                    Font f = this.Focused || this._isClicked ? new System.Drawing.Font(Font, FontStyle.Bold) : Font;
                    g.DrawString(Text, f, _textColor, textRect, _format);
                }
                else
                {
                    g.DrawString(Text, Font, _textColor, textRect, _format);
                }

            }
            else
            {
                if (renderImage)
                {
                    g.DrawImage(this.Image,
                        (this.Width - this.Image.Width) / 2 + (_isClicked ? ButtonConstants.ClickedOffset : 0),
                        iconPosY + (_isClicked ? ButtonConstants.ClickedOffset : 0));
                }
            }
            #endregion
        }

        private void PaintVisual(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            base.Paint(g);

            var taskbarLocation = this.MainForm.TaskbarLocation;

            bool horizontal = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;

            int spaceAfterX = horizontal ? ButtonConstants.SpaceAfter : 1;
            int spaceAfterY = horizontal ? 0 : ButtonConstants.SpaceAfter;

            if (!MainForm.IsHidden)
            {
                // paint background
                Region originalClip = g.Clip;
                using (Region r = new System.Drawing.Region(g.ClipBounds))
                {
                    // exclude corners since they should be rounded
                    r.Exclude(new Rectangle(0, 0, 2, 2));
                    r.Exclude(new Rectangle(Width - spaceAfterX - 1, 0, 2, 2));
                    r.Exclude(new Rectangle(0, Height - 2, 2, 2));
                    r.Exclude(new Rectangle(Width - spaceAfterX - 1, Height - 2, 2, 2));
                    g.Clip = r;

                    ProgressBarDecorator.Paint(g, _buttonBounds, _progress);

                    if (_flashActive)
                    {
                        g.FillRectangle(new LinearGradientBrush(_buttonBounds, Theme.ButtonFlashBackgroundFrom, Theme.ButtonFlashBackgroundTo, 90f), _buttonBounds);
                    }
                    else if (Hover)
                    {
                        HotTrackDecorator.Paint(g, this.Tag as SecondDisplayProcess, this.Image as Bitmap, _buttonBounds, this.PointToClient(Cursor.Position));
                    }

                    HighlightDecorator.Paint(g, _buttonBounds);

                    if ((this.Focused || this._isClicked) && !_flashActive)
                    {
                        g.FillRectangle(Theme.ButtonBackgroundFocused, 2, 2, Width - spaceAfterX - 2, Height - spaceAfterY - 2);
                    }
                }
                g.Clip = originalClip;
            }

            #region Border
            // draw border over background
            GraphicsPath path = RoundedRectangle.Create(new Rectangle(0, 0, Width - spaceAfterX, Height - spaceAfterY - 1), 2, RoundedRectangle.RectangleCorners.All);
            g.DrawPath(Theme.ButtonOuterBorder, path);

            path = RoundedRectangle.Create(new Rectangle(1, 1, Width - spaceAfterX - 2, Height - spaceAfterY - 3), 2, RoundedRectangle.RectangleCorners.All);
            g.DrawPath(Theme.ButtonInnerBorder, path);

            #endregion

            if (MainForm.IsHidden) return;

            DrawImageAndText(g, horizontal, spaceAfterX, spaceAfterY);            
        }

        private void TryOpenOverlayIcon()
        {
            if (string.IsNullOrEmpty(_overlayIconPath)) return;

            try
            {                
                _overlayIcon = new Icon(_overlayIconPath);
            }
            catch (System.IO.IOException) 
            {
                // this is to be expected because out explorer hook didn't release the file yet.
                // this is why we have the _overlayTimer to try again later
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            _flashActive = false;
        }

        /// <summary>
        /// Flash or stop flashing button in the taskbar
        /// </summary>
        public void Flash(bool active)
        {
            _flashActive = active;
            Invalidate();
        }

        /// <summary>
        /// Set progress bar value for taskbar button
        /// </summary>
        public void SetProgressValue(int value)
        {
            if (value < 0 || value > 0xFFFE) return;

            _progress.Value = (int) (value * 99.0 / 0xFFFE) + 1;
            if (_progress.State == TaskbarProgressState.NoProgress)
            {
                _progress.State = TaskbarProgressState.Normal;
            }

            Invalidate();
        }

        /// <summary>
        /// Set progress bar state (error, normal, paused...)
        /// </summary>
        public void SetProgressState(TaskbarProgressState state)
        {
            _progress.State = state;

            if (_progress.State == TaskbarProgressState.NoProgress)
            {
                _progress.Value = -1;
            }

            Invalidate();
        }

        /// <summary>
        /// Set overlay icon (this will be displayed over the app icon in taskbar)
        /// </summary>
        public void SetOverlayIcon(string iconPath)
        {
            _overlayIconPath = iconPath;

            if (!string.IsNullOrEmpty(_overlayIconPath))
            {
                TryOpenOverlayIcon();

                if (_overlayIcon != null)
                {
                    Invalidate();
                }
                else
                {
                    _overlayTimer = new System.Threading.Timer(OnOverlayTimer, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
                }
            }
            else
            {
                _overlayIcon = null;
                _overlayTimer = null;
                Invalidate();
            }
        }

        private void OnOverlayTimer(object state)
        {
            if (this._overlayIcon == null)
            {
                TryOpenOverlayIcon();
            }

            if (_overlayIcon != null)
            {
                Invalidate();
            }

            _overlayTimer = null;
        }

        internal void UpdateFont()
        {
            var prop = TaskbarPropertiesManager.Instance.Properties;
            this.Font = prop.UseCustomFont
                ? (Font) prop.CustomFont
                : SystemFonts.IconTitleFont;

            if (Native.IsThemeActive() == 0)
            {
                this._textColor = new SolidBrush(prop.UseCustomFont ? (Color)prop.CustomFont.Color : Color.FromKnownColor(KnownColor.ControlText));
            }
            else
            {
                this._textColor = prop.UseCustomFont ? new SolidBrush((Color)prop.CustomFont.Color) : Theme.ButtonTextColor;
            }
        }
    }
}
