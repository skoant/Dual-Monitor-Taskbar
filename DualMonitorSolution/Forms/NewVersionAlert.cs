using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace DualMonitor.Forms
{
    public partial class NewVersionAlert : Form
    {
        public NewVersionAlert(Version current, Version latest, string changes)
        {
            InitializeComponent();

            txtChangelist.Text = changes;

            lblCurrentVersion.Text = current.ToString(2);
            lblNewVersion.Text = latest.ToString(2);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Process.Start("https://sourceforge.net/projects/dualmonitortb/");
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            try
            {
                TaskbarPropertiesManager.Instance.Properties.CheckForUpdates = false;
                TaskbarPropertiesManager.Instance.Save();
                this.Close();
            }
            catch
            {
            }
        }
    }
}
