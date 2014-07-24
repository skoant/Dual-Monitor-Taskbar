using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Forms;
using DualMonitor.Entities;
using DualMonitor.Win32;

namespace DualMonitor
{
    public class ToolTipManager
    {
        private SecondaryTaskbar _mainForm;

        public ToolTipWindow ToolTipWindow
        {
            get;
            private set;
        }

        public ToolTipManager(SecondaryTaskbar form)
        {
            _mainForm = form;

            ToolTipWindow = new ToolTipWindow();
            ToolTipWindow.OnActivate += new EventHandler<TooltipEventArgs>(toolTipWindow_OnActivate);
            ToolTipWindow.OnClose += new EventHandler<TooltipEventArgs>(toolTipWindow_OnClose);
            ToolTipWindow.OnCustomEnter += new EventHandler<TooltipEventArgs>(toolTipWindow_OnMouseEnter);
            ToolTipWindow.OnCustomLeave += new EventHandler<TooltipEventArgs>(toolTipWindow_OnMouseLeave);     
        }

        void toolTipWindow_OnMouseLeave(object sender, TooltipEventArgs e)
        {
            e.Button.Hover = false;
            ToolTipWindow.Hide();
        }

        void toolTipWindow_OnMouseEnter(object sender, TooltipEventArgs e)
        {
            e.Button.Hover = true;
        }

        void toolTipWindow_OnClose(object sender, TooltipEventArgs e)
        {
            WindowManager.Instance.CloseProcess(e.Button.Tag as SecondDisplayProcess);
        }

        void toolTipWindow_OnActivate(object sender, TooltipEventArgs e)
        {
            (e.Button.Tag as SecondDisplayProcess).ActivateWindow(false);
        } 
    }
}
