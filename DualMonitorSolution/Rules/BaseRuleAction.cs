using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Entities;

namespace DualMonitor.Rules
{
    public abstract class BaseRuleAction
    {
        protected Win32Window Target { get; private set; }
        protected WindowManager WindowManager { get; private set; }

        protected BaseRuleAction(Win32Window target, WindowManager windowManager)
        {
            this.Target = target;
            this.WindowManager = windowManager;
        }

        public abstract void Handle();
    }
}
