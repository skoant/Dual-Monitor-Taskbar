using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Win32;
using DualMonitor.Entities;

namespace DualMonitor.Rules
{
    public class RuleActionFactory
    {
        public static BaseRuleAction CreateAction(RuleActionType ruleActionType, string moveWhere, Win32Window window, WindowManager windowManager)
        {
            switch (ruleActionType)
            {
                case RuleActionType.Move:
                    return new MoveRuleAction(moveWhere, window, windowManager);
                default:
                    return null;
            }
        }
    }
}
