using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Win32;
using DualMonitor.VisualStyle;
using System.Drawing;

namespace DualMonitor.Controls
{
    public class LiveThumbnail: IDisposable
    {
        private IntPtr _thumbnail;
        private LiveThumbnail(IntPtr thumbnailHandle)
        {
            this._thumbnail = thumbnailHandle;
        }

        public void Dispose()
        {
            if (_thumbnail != IntPtr.Zero)
            {
                DwmApi.DwmUnregisterThumbnail(_thumbnail);
            }    
        }

        public static LiveThumbnail FromHandle(IntPtr src, IntPtr dest)
        {
            IntPtr _thumbnailHandle;
            AeroDecorator.Instance.RegisterThumbnail(dest, src, out _thumbnailHandle);

            if (_thumbnailHandle != null)
            {
                return new LiveThumbnail(_thumbnailHandle);
            }

            return null;
        }

        private Size Size
        {
            get
            {
                Size size;
                AeroDecorator.Instance.GetThumbnailSize(_thumbnail, out size);
                return size;
            }
        }

        public bool SafeGetSize(out Size size)
        {
            try
            {
                size = Size;
                return true;
            }
            catch (Exception) // ArgumentException ?
            {
                size = Size.Empty;
                // ignore it - happens when the window is closed, some race condition
                return false;
            }
        }

        public void Draw(Rectangle r)
        {
            AeroDecorator.Instance.DrawThumbnail(r, _thumbnail);
        }

        public bool IsValid
        {
            get
            {
                return _thumbnail != IntPtr.Zero;
            }
        }
    }
}
