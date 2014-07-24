using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Win32;
using DualMonitor.Entities;

namespace DualMonitor
{
    public class RuleActions
    {
        public static void HandleMoveAction(MoveWhere moveWhere, IntPtr hwnd, ProcessLogic processLogic)
        {
            Screen screen = WindowManager.DetectScreen(hwnd);
            switch (moveWhere)
            {
                case MoveWhere.DontMove:
                    break;
                case MoveWhere.FollowCursor:
                    {
                        Screen target = Screen.FromPoint(Cursor.Position);
                        if (target.Primary != screen.Primary)
                        {
                            processLogic.MoveWindowBetweenScreens(hwnd, screen, target);
                            return;
                        }
                    }
                    break;
                case MoveWhere.Monitor1:
                    {
                        if (!screen.Primary)
                        {
                            processLogic.MoveWindowBetweenScreens(hwnd, screen, Screen.PrimaryScreen);
                            return;
                        }
                    }
                    break;
                case MoveWhere.Monitor2:
                    if (screen.Primary)
                    {
                        Screen target = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
                        if (target != null)
                        {
                            processLogic.MoveWindowBetweenScreens(hwnd, screen, target);
                        }
                        return;
                    }
                    break;
                default:
                    break;
            }     
        }
    }
}
