using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Win32;
using DualMonitor.Entities;
using DualMonitor.Rules;

namespace DualMonitor.Forms
{
    public partial class PropertiesWindow : Form
    {
        private bool _dirty = false;
        public event Action<TaskbarProperties.ChangedFields> OnChanged;
        private Form _mainForm;

        public PropertiesWindow(Form mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();

            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            InitRules();
        }
        
        internal void InitializeWithProperties()
        {
            chShowLabels.Checked = TaskbarPropertiesManager.Instance.Properties.ShowLabels;
            chAutoStart.Checked = AutostartUtil.IsAutoStartEnabled();
            chAutoHide.Checked = TaskbarPropertiesManager.Instance.Properties.AutoHide;
            chShowClock.Checked = TaskbarPropertiesManager.Instance.Properties.ShowClock;
            chSmallIcons.Checked = TaskbarPropertiesManager.Instance.Properties.SmallIcons;
            chMirrorButtons.Checked = TaskbarPropertiesManager.Instance.Properties.MirrorButtons;
            chNotif.Checked = TaskbarPropertiesManager.Instance.Properties.ShowNotificationArea;
            chCheckForUpdates.Checked = TaskbarPropertiesManager.Instance.Properties.CheckForUpdates;
            chStart.Checked = TaskbarPropertiesManager.Instance.Properties.ShowStartMenu;
            chCustomFont.Checked = btnFont.Enabled = TaskbarPropertiesManager.Instance.Properties.UseCustomFont;
            pnlHideDelay.Enabled = TaskbarPropertiesManager.Instance.Properties.AutoHide;
            nudAutohideDelay.Value = TaskbarPropertiesManager.Instance.Properties.AutoHideDelay;
        }        

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!_dirty) return;

            ApplySettings();            
        }

        private void ApplySettings()
        {
            if (chAutoStart.Checked)
            {
                AutostartUtil.SetAutoStart();
            }
            else
            {
                AutostartUtil.UnSetAutoStart();
            }

            TaskbarProperties.ChangedFields changes = TaskbarProperties.ChangedFields.None;

            TaskbarPropertiesManager.Instance.Properties.ShowLabels = chShowLabels.Checked;
            TaskbarPropertiesManager.Instance.Properties.ShowClock = chShowClock.Checked;
            TaskbarPropertiesManager.Instance.Properties.SmallIcons = chSmallIcons.Checked;
            if (TaskbarPropertiesManager.Instance.Properties.AutoHide != chAutoHide.Checked)
            {
                changes |= TaskbarProperties.ChangedFields.AutoHide;
                TaskbarPropertiesManager.Instance.Properties.AutoHide = chAutoHide.Checked;
            }
            TaskbarPropertiesManager.Instance.Properties.AutoHideDelay = (int)nudAutohideDelay.Value;
            TaskbarPropertiesManager.Instance.Properties.ShowNotificationArea = chNotif.Checked;
            TaskbarPropertiesManager.Instance.Properties.ShowStartMenu = chStart.Checked;
            TaskbarPropertiesManager.Instance.Properties.CheckForUpdates = chCheckForUpdates.Checked;
            TaskbarPropertiesManager.Instance.Properties.UseCustomFont = chCustomFont.Checked;

            foreach (var control in panelLocations.Controls)
            {
                var cmb = control as ComboBox;
                if (cmb == null) continue;
                var deviceName = cmb.Tag as string;
                TaskbarPropertiesManager.Instance.Properties.SetTaskbarLocation(deviceName, ScreenLocationFromIndex(cmb.SelectedIndex));
            }

            if (TaskbarPropertiesManager.Instance.Properties.MirrorButtons != chMirrorButtons.Checked)
            {
                changes |= TaskbarProperties.ChangedFields.MirrorButtons;
                TaskbarPropertiesManager.Instance.Properties.MirrorButtons = chMirrorButtons.Checked;
            }

            if (_ruleManager != null)
            {
                TaskbarPropertiesManager.Instance.Properties.Rules.Clear();

                List<DualMonitor.Entities.Rule> rules = RuleManager.Clone(_ruleManager.GetRules());
                TaskbarPropertiesManager.Instance.Properties.Rules.AddRange(rules);
            }

            TaskbarPropertiesManager.Instance.Save();

            if (OnChanged != null) OnChanged(changes);
        }

        private int IndexFromScreenLocation(Native.ABEdge location)
        {
            switch (location)
            {
                case Native.ABEdge.Left:
                    return 1;
                case Native.ABEdge.Top:
                    return 3;
                case Native.ABEdge.Right:
                    return 2;
                default:
                    return 0;
            }
        }

        private Native.ABEdge ScreenLocationFromIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Native.ABEdge.Bottom;
                case 1:
                    return Native.ABEdge.Left;
                case 2:
                    return Native.ABEdge.Right;
                case 3:
                    return Native.ABEdge.Top;
            }

            return Native.ABEdge.Bottom;
        }

        void chAutoHide_CheckedChanged(object sender, System.EventArgs e)
        {
            pnlHideDelay.Enabled = chAutoHide.Checked;
            OnFieldChanged(sender, e);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SetDirty(false);
            ApplySettings();
        }

        private void OnFieldChanged(object sender, EventArgs e)
        {
            SetDirty(true);
        }

        private void SetDirty(bool p)
        {
            _dirty = p;
            btnApply.Enabled = p;
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.FontMustExist = true;
            fd.Font = (Font)TaskbarPropertiesManager.Instance.Properties.CustomFont;
            fd.Color = (Color)TaskbarPropertiesManager.Instance.Properties.CustomFont.Color;
            fd.ShowColor = true;
            fd.AllowScriptChange = false;

            try
            {
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TaskbarPropertiesManager.Instance.Properties.CustomFont = (CustomFont)fd.Font;
                    TaskbarPropertiesManager.Instance.Properties.CustomFont.Color = (CustomColor)fd.Color;

                    SetDirty(true);
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show(this, "Only TrueType fonts are supported. Please select a different font.", "Dual Monitor Taskbar", MessageBoxButtons.OK);
            }
        }

        private void chCustomFont_CheckedChanged(object sender, EventArgs e)
        {
            OnFieldChanged(sender, e);

            btnFont.Enabled = chCustomFont.Checked;
        }

        private void nudAutohideDelay_ValueChanged(object sender, EventArgs e)
        {
            SetDirty(true);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://sourceforge.net/projects/dualmonitortb/");
        }

        private void PropertiesWindow_Load(object sender, EventArgs e)
        {
            int index = 1;
            int ypos = 4;

            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Primary) continue;

                var lblLocation = new Label();
                lblLocation.Name = "lblLocation" + index;
                lblLocation.AutoSize = false;
                lblLocation.AutoEllipsis = true;
                lblLocation.Size = new System.Drawing.Size(265, 18);
                lblLocation.Location = new Point(7, ypos);
                lblLocation.Text = "Taskbar location on screen " + screen.DeviceName;
                panelLocations.Controls.Add(lblLocation);

                var cbLocation = new System.Windows.Forms.ComboBox();
                cbLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                cbLocation.FormattingEnabled = true;
                cbLocation.Items.AddRange(new object[] {
                    "Bottom",
                    "Left",
                    "Right",
                    "Top"});
                cbLocation.Location = new System.Drawing.Point(278, ypos - 3);
                cbLocation.Name = "cbLocation";
                cbLocation.Size = new System.Drawing.Size(121, 21);
                cbLocation.TabIndex = index - 1;
                cbLocation.SelectedIndexChanged += new System.EventHandler(OnFieldChanged);
                cbLocation.SelectedIndex = IndexFromScreenLocation(TaskbarPropertiesManager.Instance.Properties.GetTaskbarLocation(screen.DeviceName));
                cbLocation.Tag = screen.DeviceName;
                cbLocation.SelectedIndexChanged += new EventHandler(OnFieldChanged);
                panelLocations.Controls.Add(cbLocation);

                index++;
                ypos += cbLocation.Height + cbLocation.Margin.Vertical;
            }

            InitRulesOnLoad();

            SetDirty(false);
        }
    }
}
