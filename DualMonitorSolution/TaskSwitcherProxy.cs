using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DualMonitor.Win32;

namespace DualMonitor
{
    class TaskSwitcherProxy
    {
        public static void MoveTaskSwitcherToSecond(Entities.Win32Window window)
        {
            // maybe implement this differently later - like mirror this window on both screens

            Point mousePos = Native.GetCursorPosition();
            var targetScreen = Screen.FromPoint(mousePos);
            if (!targetScreen.Primary)
            {
                window.MoveWindowTo(targetScreen);
            }
        }
    }
}
