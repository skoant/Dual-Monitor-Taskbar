using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Win32;
using System.Drawing.Imaging;
using DualMonitor.VisualStyle;

namespace DualMonitor.Forms
{
    public partial class CloseButtonForm : PerPixelAlphaForm
    {
        public CloseButtonForm()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // click through
                cp.ExStyle |= (int)Native.WindowExtendedStyles.WS_EX_TRANSPARENT | (int)Native.WindowExtendedStyles.WS_EX_LAYERED;
                return cp;
            }
        }

        public void Draw(Bitmap bmp)
        {
            using (Bitmap b = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb))
            {
                Graphics g = Graphics.FromImage(b);
                g.DrawImage(bmp, 0, 0);

                base.SetBitmap(b);
            }
        }


        void CloseButtonForm_Load(object sender, System.EventArgs e)
        {
            AeroDecorator.Instance.DisableLivePreview(this.Handle);
        }
    }
}
