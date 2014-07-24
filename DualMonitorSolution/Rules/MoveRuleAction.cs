using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Entities;
using System.Windows.Forms;

namespace DualMonitor.Rules
{
    public class MoveRuleAction : BaseRuleAction
    {
        private string _moveWhere;

        public MoveRuleAction(string moveWhere, Win32Window window, WindowManager windowManager)
            : base (window, windowManager)
        {
            _moveWhere = moveWhere;
        }

        public override void Handle()
        {
            Screen source = Target.Screen;
            if (string.IsNullOrEmpty(_moveWhere)) return;

            Screen destination;
            if (_moveWhere == Rule.MOVE_MONITOR_WITH_CURSOR)
            {
                destination = Screen.FromPoint(Cursor.Position);
            }
            else
            {
                destination = Screen.AllScreens.FirstOrDefault(s => s.DeviceName.Equals(_moveWhere));
            }

            if (destination == null || destination.DeviceName.Equals(source.DeviceName))
            {
                return;
            }

            WindowManager.MoveWindowBetweenScreens(Target, source, destination);
        }
    }
}
