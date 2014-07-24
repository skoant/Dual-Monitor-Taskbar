using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Controls;
using System.Drawing;

namespace DualMonitor.Entities
{
    public class ProcessGroup : List<BaseTaskbarButton>
    {
        public Rectangle GetBounds()
        {
            var buttons = this.Where(tb => tb.Visible);

            return new Rectangle(
                buttons.Min(tb => tb.Left),
                buttons.Min(tb => tb.Top),
                buttons.Max(tb => tb.Right) - buttons.Min(tb => tb.Left),
                buttons.Max(tb => tb.Bottom) - buttons.Min(tb => tb.Top));
        }

        public int GetFirstIndex(System.Windows.Forms.Control.ControlCollection controlCollection)
        {
            return this.Min(tb => controlCollection.GetChildIndex(tb));
        }

        public int GetLastIndex(System.Windows.Forms.Control.ControlCollection controlCollection)
        {
            return this.Max(tb => controlCollection.GetChildIndex(tb));
        }

        public void Arrange(System.Windows.Forms.Control.ControlCollection controlCollection, int index)
        {
            this.Sort((x, y) => x.AddedToTaskbar.CompareTo(y.AddedToTaskbar));
            foreach (var item in this)
            {
                controlCollection.SetChildIndex(item, index);
                index++;
            }
        }

        public TaskbarPinnedButton GetPinnedButton()
        {
            return this.Find(tb => tb is TaskbarPinnedButton) as TaskbarPinnedButton;
        }
    }
}
