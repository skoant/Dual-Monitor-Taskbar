using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using DualMonitor.Entities;
using System.Windows.Forms;
using DualMonitor.Win32;

namespace DualMonitor.VisualStyle
{
    public class PushPanelDecorator : IDisposable
    {
        private const int ClickedFadeWidth = 30;

        private bool _leftToRight;
        private int _width;
        private int _height;

        private Pen _darkTop;
        private Pen _darkBottom;
        private Pen _lightTop;
        private Pen _lightBottom;
        private PathGradientBrush _pgp;
        private GraphicsPath _gp;
        private Rectangle _upperBounds;
        private Rectangle _lowerBounds;
        private Region _clipRegion;
        private Padding _padding;
        private Control _ctrl;

        private LinearGradientBrush _upperGradient;
        private LinearGradientBrush _lowerGradient;

        public PushPanelDecorator(bool leftToRight, int width, int height, Padding padding, Control ctrl)
        {
            _leftToRight = leftToRight;
            _width = width;
            _height = height;
            _ctrl = ctrl;

            _gp = new GraphicsPath();

            if (_leftToRight)
            {
                _darkTop = new Pen(new LinearGradientBrush(new Point(0, 2), new Point(0, _height / 2 + 1), Theme.DarkLineFrom, Theme.DarkLineTo));
                _darkBottom = new Pen(new LinearGradientBrush(new Point(0, _height / 2), new Point(0, _height - 1), Theme.DarkLineTo, Theme.DarkLineFrom));

                _lightTop = new Pen(new LinearGradientBrush(new Point(0, 2), new Point(0, _height / 2 + 1), Theme.BrightLineFrom, Theme.BrightLineTo));
                _lightBottom = new Pen(new LinearGradientBrush(new Point(0, _height / 2), new Point(0, _height - 1), Theme.BrightLineTo, Theme.BrightLineFrom));

                _gp.AddEllipse((_width - 20) / 2, (_height - 10), 20, 20);
            }
            else
            {
                _darkTop = new Pen(new LinearGradientBrush(new Point(1, 0), new Point(_width / 2 + 1, 0), Theme.DarkLineFrom, Theme.DarkLineTo));
                _darkBottom = new Pen(new LinearGradientBrush(new Point(_width / 2, 0), new Point(_width - 1, 0), Theme.DarkLineTo, Theme.DarkLineFrom));

                _lightTop = new Pen(new LinearGradientBrush(new Point(1, 0), new Point(_width / 2 + 1, 0), Theme.BrightLineFrom, Theme.BrightLineTo));
                _lightBottom = new Pen(new LinearGradientBrush(new Point(_width / 2, 0), new Point(_width - 1, 0), Theme.BrightLineTo, Theme.BrightLineFrom));

                _gp.AddEllipse((_width - 30) / 2, (_height - 15), 30, 30);
                _clipRegion = new Region(new Rectangle(0, 0, _width, _height - 3));
            }

            _pgp = new PathGradientBrush(_gp);
            _pgp.CenterPoint = new PointF(_width / 2, _height);
            _pgp.CenterColor = Theme.HoverBackLightFrom;
            _pgp.SurroundColors = new Color[] { Theme.HoverBackLightTo };

            _upperBounds = new Rectangle(1, _height / 2 - ClickedFadeWidth, _width - (2 + _padding.Left + _padding.Right), ClickedFadeWidth);
            _lowerBounds = new Rectangle(1, _height / 2, _width - (2 + _padding.Left + _padding.Right), ClickedFadeWidth);

            _upperGradient = new LinearGradientBrush(_upperBounds, Color.Transparent, Theme.PinPressedBackDark, 90f);
            _lowerGradient = new LinearGradientBrush(_lowerBounds, Theme.PinPressedBackDark, Color.Transparent, 90f);
        }

        public void Paint(Graphics g, bool isClicked, bool hover)
        {
            if (Native.IsThemeActive() != 0)
            {
                if (isClicked)
                {
                    g.FillRectangle(_upperGradient, _upperBounds);
                    g.FillRectangle(_lowerGradient, _lowerBounds);
                }

                if (hover)
                {
                    if (_leftToRight)
                    {
                        g.DrawLine(_darkTop, new Point(_padding.Left, 2), new Point(_padding.Left, _height / 2));
                        g.DrawLine(_darkBottom, new Point(_padding.Left, _height / 2), new Point(_padding.Left, _height - 1));
                        g.DrawLine(_darkTop, new Point(_width - (2 + _padding.Right), 2), new Point(_width - (2 + _padding.Right), _height / 2));
                        g.DrawLine(_darkBottom, new Point(_width - (2 + _padding.Right), _height / 2), new Point(_width - (2 + _padding.Right), _height - 1));

                        g.DrawLine(_lightTop, new Point(_padding.Left + 1, 2), new Point(_padding.Left + 1, _height / 2));
                        g.DrawLine(_lightBottom, new Point(_padding.Left + 1, _height / 2), new Point(_padding.Left + 1, _height - 1));
                        g.DrawLine(_lightTop, new Point(_width - (1 + _padding.Right), 2), new Point(_width - (1 + _padding.Right), _height / 2));
                        g.DrawLine(_lightBottom, new Point(_width - (1 + _padding.Right), _height / 2), new Point(_width - (1 + _padding.Right), _height - 1));
                    }
                    else
                    {
                        g.DrawLine(_darkTop, new Point(2, 1), new Point(_width / 2 + 1, 1));
                        g.DrawLine(_darkBottom, new Point(_width / 2, 1), new Point(_width - 1, 1));
                        g.DrawLine(_darkTop, new Point(2, _height - 4), new Point(_width / 2 + 1, _height - 4));
                        g.DrawLine(_darkBottom, new Point(_width / 2, _height - 4), new Point(_width - 1, _height - 4));

                        g.DrawLine(_lightTop, new Point(2, 2), new Point(_width / 2 + 1, 2));
                        g.DrawLine(_lightBottom, new Point(_width / 2, 2), new Point(_width - 1, 2));
                        g.DrawLine(_lightTop, new Point(2, _height - 3), new Point(_width / 2 + 1, _height - 3));
                        g.DrawLine(_lightBottom, new Point(_width / 2, _height - 3), new Point(_width - 1, _height - 3));

                        g.Clip = _clipRegion;
                    }

                    g.FillPath(_pgp, _gp);
                }
            }
            else
            {
                if (hover)
                {
                    ButtonBorderDecorator.Draw(g, 0, 4, _ctrl.Width - 4, _ctrl.Height - 3, isClicked);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_darkTop != null)
            {
                _darkTop.Brush.Dispose();
                _darkTop.Dispose();

                _darkBottom.Brush.Dispose();
                _darkBottom.Dispose();

                _lightBottom.Brush.Dispose();
                _lightBottom.Dispose();

                _lightTop.Brush.Dispose();
                _lightTop.Dispose();

                _pgp.Dispose();
                _gp.Dispose();

                _lowerGradient.Dispose();
                _upperGradient.Dispose();
            }

            if (_clipRegion != null)
            {
                _clipRegion.Dispose();
            }
        }

        #endregion
    }
}
