using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;
using DualMonitor.Forms;

namespace DualMonitor.Controls
{
    public class BorderedTransparentPanel : Panel
    {
        private SecondaryTaskbar _mainForm;
        public BorderedTransparentPanel(SecondaryTaskbar mainForm)
        {
            _mainForm = mainForm;
            BackColor = Color.FromKnownColor(KnownColor.ButtonFace);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (Native.IsThemeActive() == 0)
            {
                base.OnPaint(e);

                Pen dark = new Pen(Color.FromKnownColor(KnownColor.ButtonShadow));

                e.Graphics.DrawLine(dark, 0, 0, Width, 0);
                e.Graphics.DrawLine(dark, 0, 0, 0, Height);

                Pen light = new Pen(Color.FromKnownColor(KnownColor.ButtonHighlight));

                e.Graphics.DrawLine(light, 0, Height - 1, Width, Height - 1);
                e.Graphics.DrawLine(light, Width - 1, 0, Width - 1, Height);
            }
            else
            {
                BackgroundDecorator.Paint(e.Graphics, this.Handle);
                OneBorderDecorator.Draw(e.Graphics, this, _mainForm.TaskbarLocation);
            }
        }
    }
}
