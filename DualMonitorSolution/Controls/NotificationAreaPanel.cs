using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Entities;
using DualMonitor.Win32;
using System.Drawing;
using Gma.UserActivityMonitor;
using DualMonitor.VisualStyle;
using System.Drawing.Drawing2D;
using DualMonitor.GraphicUtils;
using DualMonitor.Forms;
using System.Threading;

namespace DualMonitor.Controls
{
    public class NotificationAreaPanel : Panel
    {
        private bool _moreIconsVisible;
        private bool _buttonVisible;
        private MoreIconsButton _button;
        private bool _showButtonOnTheSameLine;
        private SecondaryTaskbar MainForm { get { return this.FindForm() as SecondaryTaskbar; } }

        private const int HMargin = 3;

        public NotificationAreaPanel()
        {
            this.BackColor = Color.Transparent;
        }

        public bool MoreIconsVisible
        {
            get
            {
                return _moreIconsVisible;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Native.IsThemeActive() != 0)
            {
                base.OnPaint(e);
            }
        }

        public void Initialize()
        {
            _buttonVisible = NotificationAreaProxy.IsShowMoreButtonVisible();

            _button = new MoreIconsButton();

            bool isVisualTheme = Native.IsThemeActive() != 0;

            if (isVisualTheme)
            {
                _button.Width = ButtonConstants.NotifButtonWidth;
                _button.Height = ButtonConstants.NotifButtonWidth;
            }
            _button.Click += new EventHandler(button_Click);

            TryUpdateIcons();            
        }

        private void TryUpdateIcons()
        {
            try
            {
                UpdateIcons();
            }
            catch (OverflowException)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Thread.Sleep(1000);
                    this.Invoke(new MethodInvoker(TryUpdateIcons));
                });
            }
        }

        /// <summary>
        /// Show & positon or hide "more icons" button.
        /// </summary>
        private void ShowButton()
        {
            if (_buttonVisible)
            {
                if (this.Controls.Count == 0 || !(this.Controls[0] is Button))
                {
                    this.Controls.Add(_button);
                }

                bool isVisualTheme = Native.IsThemeActive() != 0;
                var taskbarLocation = this.MainForm.TaskbarLocation;

                if (isVisualTheme)
                {
                    _button.Width = ButtonConstants.NotifButtonWidth;
                    _button.Height = ButtonConstants.NotifButtonWidth;

                    if (taskbarLocation == Native.ABEdge.Left
                        && _buttonVisible)
                    {
                        _button.Left = this.ClientRectangle.Right - this.Controls[0].Width - 3;
                    }
                    else
                    {
                        _button.Left = this.ClientRectangle.Left + 3;
                    }

                    if (taskbarLocation == Native.ABEdge.Left ||
                        taskbarLocation == Native.ABEdge.Right)
                    {
                        _button.Top = 5;
                    }
                    else
                    {
                        _button.Top = (this.Height - _button.Height) / 2;
                    }
                }
                else
                {
                    switch (taskbarLocation)
                    {
                        case Native.ABEdge.Left:
                            _button.Left = this.Width - 25;
                            _button.Top = 0;
                            _button.Width = 24;
                            _button.Height = 24;
                            break;
                        case Native.ABEdge.Top:
                            _button.Left = 0;
                            _button.Top = 1;
                            _button.Width = 20;
                            _button.Height = this.Height - 2;
                            break;
                        case Native.ABEdge.Right:
                            _button.Left = 1;
                            _button.Top = 0;
                            _button.Width = 24;
                            _button.Height = 24;
                            break;
                        case Native.ABEdge.Bottom:
                            _button.Left = 0;
                            _button.Top = 1;
                            _button.Width = 20;
                            _button.Height = this.Height - 2;
                            break;
                        default:
                            break;
                    }
                    
                
                }
            }
            else
            {
                if (this.Controls.Count == 0 || !(this.Controls[0] is Button))
                {
                    return;
                }

                this.Controls.RemoveAt(0);
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Point location = button.PointToScreen(button.Location);
            int x, y;
            AnchorStyles anchor;

            // calculate position for more icons window
            Rectangle taskbarBounds = MainForm.Bounds;
            switch (this.MainForm.TaskbarLocation)
            {
                case Native.ABEdge.Left:
                    x = taskbarBounds.Right;
                    y = location.Y + button.Height / 2;
                    anchor = AnchorStyles.Left;
                    break;
                case Native.ABEdge.Top:
                    x = location.X + button.Width / 2;
                    y = taskbarBounds.Bottom;
                    anchor = AnchorStyles.Top;
                    break;
                case Native.ABEdge.Right:
                    x = taskbarBounds.Left;
                    y = location.Y + button.Height / 2;
                    anchor = AnchorStyles.Right;
                    break;
                case Native.ABEdge.Bottom:
                    x = location.X + button.Width / 2;
                    y = taskbarBounds.Top;
                    anchor = AnchorStyles.Bottom;
                    break;
                default:
                    return;
            }

            _moreIconsVisible = !_moreIconsVisible;
            NotificationAreaProxy.ShowMoreIcons(_moreIconsVisible, x, y, anchor);

            if (_moreIconsVisible)
            {
                HookManager.MouseClick += HookManager_MouseClick;
            }
            else
            {
                HookManager.MouseClick -= HookManager_MouseClick;
            }
        }
        
        void HookManager_MouseClick(object sender, MouseEventArgs e)
        {
            var window = Win32Window.FromClassName("NotifyIconOverflowWindow");
            Rectangle r1 = window.Bounds;

            // if we click outside of more icons window, hide it
            Rectangle r2 = _button.RectangleToScreen(_button.Bounds);
            if (!r1.Contains(e.Location) && !r2.Contains(e.Location))
            {
                _button.PerformClick();
            }
        }

        private void UpdateIcon(int index)
        {
            if (_buttonVisible) index++;

            if (index < this.Controls.Count)
            {
                this.Controls[index].Invalidate();
            }
        }

        public void UpdateIcons()
        {
            var icons = NotificationAreaProxy.GetVisibleIcons();

            this.Controls.Clear();

            this.SuspendLayout();

            ShowButton();

            // add a button for each icon and calculate its position
            Native.ABEdge edge = this.MainForm.TaskbarLocation;
            int index = _buttonVisible && _showButtonOnTheSameLine && edge != Native.ABEdge.Left ? 1 : 0;
            int top = _buttonVisible && !_showButtonOnTheSameLine ? 30 : (Native.IsThemeActive() == 0 ? 0 : 4);

            foreach (var icon in icons)
            {
                var button = new NotificationIcon(icon);
                this.Controls.Add(button);

                button.Init();
                button.AutoSize = false;
                button.Padding = new System.Windows.Forms.Padding(0);
                button.Margin = new System.Windows.Forms.Padding(0);

                button.Left = index * button.Width + HMargin;
                button.Top = top + 1;

                index++;

                if ((index + 1) * button.Width + HMargin> this.Width)
                {
                    index = _buttonVisible && (edge == Native.ABEdge.Bottom || edge == Native.ABEdge.Top) ? 1 : 0;
                    top += button.Height;
                }
            }
            this.ResumeLayout();

            CalculateSize(icons.Count);
        }

        /// <summary>
        /// Autosize this panel based on number of icons
        /// </summary>
        public void CalculateSize(int count)
        {
            var taskbarLocation = this.MainForm.TaskbarLocation;
            bool isLeftToRight = taskbarLocation == Native.ABEdge.Top
                || taskbarLocation == Native.ABEdge.Bottom;

            if (isLeftToRight)
            {
                int maxIconsPerRow = (int)Math.Ceiling((double)count / TaskbarPropertiesManager.Instance.Properties.HeightMultiplier);

                if (_buttonVisible) maxIconsPerRow++;

                this.Width = maxIconsPerRow * ButtonConstants.NotifIconWidth + HMargin;
                this.Height = this.Parent.Height;

                _showButtonOnTheSameLine = true;
            }
            else
            {
                double totalWidth = (double)count * ButtonConstants.NotifIconWidth + HMargin;
                int rows = (int)Math.Ceiling(totalWidth / this.Parent.Width);
                if (_buttonVisible)
                {
                    if (rows > 1 || ((count + 1) * ButtonConstants.NotifIconWidth + HMargin)> this.Parent.Width)
                    {                        
                        rows++;
                    }
                }

                _showButtonOnTheSameLine = rows == 1;

                this.Height = rows * ButtonConstants.NotifIconHeight;
                this.Width = this.Parent.Width;
            }
        }

        public void DispatchMessage(Message m)
        {
            if (this.Visible)
            {
                int index = (int)m.WParam;
                if (index == -1)
                {
                    _buttonVisible = NotificationAreaProxy.IsShowMoreButtonVisible();
                    UpdateIcons();
                }
                else
                {
                    // only update icon, not tooltip
                    UpdateIcon(index);
                }
            }
        }
    }
}
