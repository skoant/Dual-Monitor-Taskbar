using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DualMonitor.Entities;
using DualMonitor.GraphicUtils;
using System.Drawing.Drawing2D;
using Gma.UserActivityMonitor;
using System.IO;
using DualMonitor.Win32;
using System.Diagnostics;
using DualMonitor.Rules;

namespace DualMonitor.Forms
{
    public partial class PropertiesWindow
    {
        private StringFormat _format;
        private SolidBrush _backgroundBrush;
        private SolidBrush _selectionBrush;
        private GraphicsPath _panelBorder;
        private Pen _borderPen;
        private Pen _openBorderPen;
        private bool hooked;
        private IntPtr _crtHwnd = IntPtr.Zero;
        private bool _ignoreEvents;
        private RuleManager _ruleManager;

        private void InitRules()
        {
            CreateNewRuleManager();
            
            _format = (StringFormat.GenericDefault.Clone() as StringFormat);
            _format.Trimming = StringTrimming.EllipsisCharacter;
            _format.LineAlignment = StringAlignment.Center;
            _format.FormatFlags = _format.FormatFlags | StringFormatFlags.NoWrap;

            _backgroundBrush = new SolidBrush(lbRules.BackColor);
            _selectionBrush = new SolidBrush(Color.FromArgb(240, 245, 251));
            _borderPen = new Pen(Color.FromArgb(150, 160, 180));
            _openBorderPen = new Pen(_selectionBrush.Color);

            _panelBorder = RoundedRectangle.Create(panelDetails.ClientRectangle.Left, panelDetails.ClientRectangle.Top, 
                panelDetails.ClientRectangle.Width-1, panelDetails.ClientRectangle.Height-1, 5);

            panelDetails.Visible = false;
        }

        private void InitRulesOnLoad()
        {
            this.cbMoveTo.Items.Clear();
            this.cbMoveTo.Items.Add(Rule.MOVE_MONITOR_WITH_CURSOR);
           
            foreach (var screen in Screen.AllScreens)
            {
                this.cbMoveTo.Items.Add(screen.DeviceName);
            }
        }

        private void CreateNewRuleManager()
        {
            List<DualMonitor.Entities.Rule> rules = RuleManager.Clone(TaskbarPropertiesManager.Instance.Properties.Rules);
            _ruleManager = new RuleManager(rules, null);

            lbRules.Items.Clear();
            foreach (Rule r in _ruleManager.GetRules())
            {
                lbRules.Items.Add(r);
            }
        }

        private void lbRules_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillRectangle(_backgroundBrush, e.Bounds);

            Rule rule = lbRules.Items[e.Index] as Rule;

            if (e.State.HasFlag(DrawItemState.Selected))
            {
                var path = RoundedRectangle.Create(e.Bounds.Left + 1, e.Bounds.Top + 1, e.Bounds.Width - 1, e.Bounds.Height - 2,
                    5, RoundedRectangle.RectangleCorners.TopLeft | RoundedRectangle.RectangleCorners.BottomLeft);
                e.Graphics.FillPath(_selectionBrush, path);

                e.Graphics.DrawPath(_borderPen, path);
                e.Graphics.DrawLine(_openBorderPen, e.Bounds.Right-1, e.Bounds.Top + 2, e.Bounds.Right-1, e.Bounds.Bottom - 2);
                panelDetails.Invalidate();
            }

            e.Graphics.DrawString(rule.Name, lbRules.Font, Brushes.Black, new Rectangle(e.Bounds.Left + 10, e.Bounds.Top, e.Bounds.Width-10, e.Bounds.Height), _format);
        }

        private void panelDetails_Paint(object sender, PaintEventArgs e)
        {
            if (lbRules.SelectedIndex == -1) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillPath(_selectionBrush, _panelBorder);

            e.Graphics.DrawPath(_borderPen, _panelBorder);
            Rectangle selectedItemBounds = panelDetails.RectangleToClient(lbRules.RectangleToScreen(lbRules.GetItemRectangle(lbRules.SelectedIndex)));

            e.Graphics.DrawLine(_openBorderPen, selectedItemBounds.Right, selectedItemBounds.Top + 1, selectedItemBounds.Right, selectedItemBounds.Bottom - 1);
        }

        private void lbRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool selected = lbRules.SelectedIndex >= 0;
            if (selected)
            {
                PrepareDetailsPanel(lbRules.SelectedItem as Rule);
            }
            panelDetails.Visible = selected;
            btnRemove.Enabled = selected;
        }

        private void PrepareDetailsPanel(Rule rule)
        {
            _ignoreEvents = true;

            txtName.Text = rule.Name;
            lbRules.Invalidate();
            
            txtClass.Text = rule.Class;
            ckClass.Checked = rule.UseClass;

            txtCaption.Text = rule.Caption;
            ckCaption.Checked = rule.UseCaption;

            txtProgram.Text = rule.Program;
            ckProgram.Checked = rule.UseProgram;

            if (rule.Icon != null)
            {
                using (Image img = Image.FromFile(rule.Icon))
                {
                    pbIcon.Image = (Image)img.Clone();
                }
            }
            else
            {
                pbIcon.Image = null;
            }

            _ignoreEvents = false;

            if (string.IsNullOrEmpty(rule.MoveAction))
            {
                cbMoveTo.SelectedIndex = 0;
            }
            else
            {
                cbMoveTo.SelectedItem = (string)rule.MoveAction;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            SetDirty(true);
            int index = lbRules.SelectedIndex;
            Rule rule = lbRules.Items[index] as Rule;
            _ruleManager.RemoveRule(rule);

            lbRules.Items.RemoveAt(index);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SetDirty(true);
            Rule r = new Rule();
            r.Id = Guid.NewGuid();
            r.Name = "New Rule";
            _ruleManager.AddRule(r);
            int index = lbRules.Items.Add(r);

            lbRules.SelectedIndex = index;
        }

        private void pbFindWindow_MouseDown(object sender, MouseEventArgs e)
        {
            pbFindWindow.Image = DualMonitor.Properties.Resources.find_window_used;
            Cursor.Current = new System.Windows.Forms.Cursor(new MemoryStream(DualMonitor.Properties.Resources.searchw));

            if (!hooked)
            {
                hooked = true;
                HookManager.MouseMove += new MouseEventHandler(HookManager_MouseMove);
                HookManager.MouseUp += new MouseEventHandler(HookManager_MouseUp);
            }
        }

        void HookManager_MouseUp(object sender, MouseEventArgs e)
        {
            if (hooked)
            {
                HookManager.MouseMove -= new MouseEventHandler(HookManager_MouseMove);
                HookManager.MouseUp -= new MouseEventHandler(HookManager_MouseUp);
                hooked = false;
            }
            Cursor.Current = this.Cursor;
            pbFindWindow.Image = DualMonitor.Properties.Resources.find_window_normal;
        }

        void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            IntPtr hwnd = Native.WindowFromPoint(new Native.POINTAPI() { x = e.X + 16, y = e.Y + 16 }); // middle of cursor - instead of setting hotspot
            if (hwnd == IntPtr.Zero) return;

            string fileName;
            if (!Native.IsWindowValidForFinder(ref hwnd, 
                new List<IntPtr>() { _crtHwnd, this.Handle, _mainForm == null ? IntPtr.Zero : _mainForm.Handle }, out fileName)) return;            

            _crtHwnd = hwnd;
            Rule rule = CreateRuleFromHandle(hwnd, fileName);
            PrepareDetailsPanel(rule);
        }

        private Rule CreateRuleFromHandle(IntPtr hwnd, string fileName)
        {
            SetDirty(true);
            Rule rule = lbRules.SelectedItem as Rule;

            rule.Class = Native.GetClassName(hwnd);
            rule.Program = fileName;

            rule.Caption = Native.GetWindowText(hwnd);

            string title = System.Diagnostics.FileVersionInfo.GetVersionInfo(fileName).FileDescription;
            if (!rule.Name.Equals(title))
            {
                rule.Name = title;
            }

            Icon icon = Native.GetIcon(hwnd);
            if (icon != null)
            {
                string path = _ruleManager.SaveIcon(icon);
                rule.Icon = path;
            }
            else
            {
                rule.Icon = null;
            }

            rule.UseClass = true;
            rule.UseCaption = false;
            rule.UseProgram = true;

            return rule;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DereferenceLinks = true;
            ofd.Filter = "Program Files (*.exe)|*.exe";
            ofd.Multiselect = false;
            ofd.Title = "Choose a program file";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var rule = lbRules.SelectedItem as Rule;
                rule.Program = ofd.FileName;
                PrepareDetailsPanel(rule);
            }
        }

        private void txtClass_TextChanged(object sender, EventArgs e)
        {
            if (_ignoreEvents) return;
            SetDirty(true);
            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;
            rule.Class = txtClass.Text;
            ckClass.Checked = txtClass.Text.Length > 0;
        }

        private void ckClass_CheckedChanged(object sender, EventArgs e)
        {
            if (_ignoreEvents) return;
            SetDirty(true);
            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;
            rule.UseClass = ckClass.Checked;
        }

        private void txtCaption_TextChanged(object sender, EventArgs e)
        {
            if (_ignoreEvents) return;
            SetDirty(true);
            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;
            rule.Caption = txtCaption.Text;
            ckCaption.Checked = txtCaption.Text.Length > 0;
        }

        private void ckCaption_CheckedChanged(object sender, EventArgs e)
        {
            if (_ignoreEvents) return;
            SetDirty(true);
            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;
            rule.UseCaption = ckCaption.Checked;
        }

        private void txtProgram_TextChanged(object sender, EventArgs e)
        {
            if (_ignoreEvents) return;
            SetDirty(true);
            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;
            rule.Program = txtProgram.Text;
            ckProgram.Checked = txtProgram.Text.Length > 0;
        }

        private void ckProgram_CheckedChanged(object sender, EventArgs e)
        {
            if (_ignoreEvents) return;
            SetDirty(true);
            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;
            rule.UseProgram = ckProgram.Checked;
        }

        private void btnSelectWindow_Click(object sender, EventArgs e)
        {
            var form = new SelectProgram(new List<IntPtr> { this.Handle, _mainForm == null ? IntPtr.Zero : _mainForm.Handle });
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK && form.SelectedWindow != null)
            {
                DualMonitor.Entities.Rule rule = CreateRuleFromHandle(form.SelectedWindow.HWND, form.SelectedWindow.FileName);

                PrepareDetailsPanel(rule);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;

            rule.MoveAction = (string) cbMoveTo.SelectedItem;
            
            SetDirty(true);
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "") return;

            var rule = lbRules.SelectedItem as DualMonitor.Entities.Rule;
            rule.Name = txtName.Text;
            SetDirty(true);
            lbRules.Invalidate();
        }
    }
}
