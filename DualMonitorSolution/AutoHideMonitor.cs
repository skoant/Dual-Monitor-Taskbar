using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Forms;
using Gma.UserActivityMonitor;
using System.Windows.Forms;

namespace DualMonitor
{
    /// <summary>
    /// Auto hides taskbar when mouse is not rolling over
    /// </summary>
    class AutoHideMonitor : IDisposable
    {
        private SecondaryTaskbar _mainForm;
        private Timer _timer;

        public event Action OnHide;
        
        public AutoHideMonitor(SecondaryTaskbar mainForm)
        {
            _mainForm = mainForm;
            _timer = new Timer();
            _timer.Interval = Math.Max(1, TaskbarPropertiesManager.Instance.Properties.AutoHideDelay);
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {            
            OnHide();
        }

        internal void Start()
        {
            HookManager.MouseMove += HookManager_MouseMove;
        }

        void HookManager_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_mainForm.ShouldStayVisible())
            {
                if (_timer.Enabled)
                {
                    _timer.Stop();
                }
            }
            else
            {
                if (!_timer.Enabled)
                {
                    _timer.Start();
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            HookManager.MouseMove -= HookManager_MouseMove;
            _timer.Stop();
            _timer.Dispose();
        }

        #endregion
    }
}
