using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Entities;
using System.Drawing;
using DualMonitor.GraphicUtils;

namespace DualMonitor.VisualStyle
{
    public class ProgressBarDecorator
    {
        public static void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle _buttonBounds, Entities.TaskbarProgress _progress)
        {
            if (_progress.State == TaskbarProgressState.NoProgress || _progress.Value == -1) return;

            int width = (int)(_buttonBounds.Width * (double)_progress.Value / 100.0);

            width = Utils.Clamp(width, 1, _buttonBounds.Width);

            Rectangle bounds = new Rectangle(_buttonBounds.Location, new Size(width, _buttonBounds.Height - 1));
            
            switch (_progress.State)
            {
                case TaskbarProgressState.Indeterminate:
                case TaskbarProgressState.Normal:
                    g.FillGradientRectangle(Theme.ProgressBarNormalDark, Theme.ProgressBarNormalLight, bounds, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    break;
                case TaskbarProgressState.Error:
                    g.FillGradientRectangle(Theme.ProgressBarErrorDark, Theme.ProgressBarErrorLight, bounds, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    break;
                case TaskbarProgressState.Paused:
                    g.FillGradientRectangle(Theme.ProgressBarPausedDark, Theme.ProgressBarPausedLight, bounds, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    break;
            }
        }
    }
}
