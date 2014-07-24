using DualMonitor.Controls;
namespace DualMonitor.Forms
{
    partial class PropertiesWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesWindow));
            this.chShowLabels = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chShowClock = new System.Windows.Forms.CheckBox();
            this.chSmallIcons = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlHideDelay = new System.Windows.Forms.Panel();
            this.nudAutohideDelay = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.btnFont = new System.Windows.Forms.Button();
            this.chCustomFont = new System.Windows.Forms.CheckBox();
            this.chStart = new System.Windows.Forms.CheckBox();
            this.chNotif = new System.Windows.Forms.CheckBox();
            this.chAutoHide = new System.Windows.Forms.CheckBox();
            this.chMirrorButtons = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTaskbar = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.panelLocations = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chCheckForUpdates = new System.Windows.Forms.CheckBox();
            this.chAutoStart = new System.Windows.Forms.CheckBox();
            this.tabWindowManager = new System.Windows.Forms.TabPage();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lbRules = new System.Windows.Forms.ListBox();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbMoveTo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.ckProgram = new System.Windows.Forms.CheckBox();
            this.txtProgram = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectWindow = new System.Windows.Forms.Button();
            this.ckCaption = new System.Windows.Forms.CheckBox();
            this.ckClass = new System.Windows.Forms.CheckBox();
            this.txtCaption = new System.Windows.Forms.TextBox();
            this.txtClass = new System.Windows.Forms.TextBox();
            this.pbFindWindow = new System.Windows.Forms.PictureBox();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.pnlHideDelay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutohideDelay)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabTaskbar.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabWindowManager.SuspendLayout();
            this.panelDetails.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFindWindow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // chShowLabels
            // 
            this.chShowLabels.AutoSize = true;
            this.chShowLabels.Location = new System.Drawing.Point(10, 21);
            this.chShowLabels.Name = "chShowLabels";
            this.chShowLabels.Size = new System.Drawing.Size(83, 17);
            this.chShowLabels.TabIndex = 0;
            this.chShowLabels.Text = "Show &labels";
            this.chShowLabels.UseVisualStyleBackColor = true;
            this.chShowLabels.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(406, 385);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(487, 385);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chShowClock
            // 
            this.chShowClock.AutoSize = true;
            this.chShowClock.Location = new System.Drawing.Point(10, 44);
            this.chShowClock.Name = "chShowClock";
            this.chShowClock.Size = new System.Drawing.Size(82, 17);
            this.chShowClock.TabIndex = 1;
            this.chShowClock.Text = "Show cloc&k";
            this.chShowClock.UseVisualStyleBackColor = true;
            this.chShowClock.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // chSmallIcons
            // 
            this.chSmallIcons.AutoSize = true;
            this.chSmallIcons.Location = new System.Drawing.Point(10, 90);
            this.chSmallIcons.Name = "chSmallIcons";
            this.chSmallIcons.Size = new System.Drawing.Size(99, 17);
            this.chSmallIcons.TabIndex = 3;
            this.chSmallIcons.Text = "Use small &icons";
            this.chSmallIcons.UseVisualStyleBackColor = true;
            this.chSmallIcons.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pnlHideDelay);
            this.groupBox1.Controls.Add(this.btnFont);
            this.groupBox1.Controls.Add(this.chCustomFont);
            this.groupBox1.Controls.Add(this.chStart);
            this.groupBox1.Controls.Add(this.chNotif);
            this.groupBox1.Controls.Add(this.chAutoHide);
            this.groupBox1.Controls.Add(this.chMirrorButtons);
            this.groupBox1.Controls.Add(this.chShowLabels);
            this.groupBox1.Controls.Add(this.chSmallIcons);
            this.groupBox1.Controls.Add(this.chShowClock);
            this.groupBox1.Location = new System.Drawing.Point(6, 84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(611, 144);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Taskbar appearance";
            // 
            // pnlHideDelay
            // 
            this.pnlHideDelay.Controls.Add(this.nudAutohideDelay);
            this.pnlHideDelay.Controls.Add(this.label5);
            this.pnlHideDelay.Enabled = false;
            this.pnlHideDelay.Location = new System.Drawing.Point(295, 113);
            this.pnlHideDelay.Name = "pnlHideDelay";
            this.pnlHideDelay.Size = new System.Drawing.Size(241, 21);
            this.pnlHideDelay.TabIndex = 17;
            // 
            // nudAutohideDelay
            // 
            this.nudAutohideDelay.Location = new System.Drawing.Point(129, 0);
            this.nudAutohideDelay.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudAutohideDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAutohideDelay.Name = "nudAutohideDelay";
            this.nudAutohideDelay.Size = new System.Drawing.Size(59, 20);
            this.nudAutohideDelay.TabIndex = 1;
            this.nudAutohideDelay.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.nudAutohideDelay.ValueChanged += new System.EventHandler(this.nudAutohideDelay_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-3, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Hide delay (milliseconds):";
            // 
            // btnFont
            // 
            this.btnFont.Enabled = false;
            this.btnFont.Location = new System.Drawing.Point(384, 64);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(36, 23);
            this.btnFont.TabIndex = 7;
            this.btnFont.Text = "...";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // chCustomFont
            // 
            this.chCustomFont.AutoSize = true;
            this.chCustomFont.Location = new System.Drawing.Point(278, 68);
            this.chCustomFont.Name = "chCustomFont";
            this.chCustomFont.Size = new System.Drawing.Size(106, 17);
            this.chCustomFont.TabIndex = 6;
            this.chCustomFont.Text = "Use custom &font:";
            this.chCustomFont.UseVisualStyleBackColor = true;
            this.chCustomFont.CheckedChanged += new System.EventHandler(this.chCustomFont_CheckedChanged);
            // 
            // chStart
            // 
            this.chStart.AutoSize = true;
            this.chStart.Location = new System.Drawing.Point(278, 44);
            this.chStart.Name = "chStart";
            this.chStart.Size = new System.Drawing.Size(148, 17);
            this.chStart.TabIndex = 5;
            this.chStart.Text = "Show &Start button (BETA)";
            this.chStart.UseVisualStyleBackColor = true;
            this.chStart.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // chNotif
            // 
            this.chNotif.AutoSize = true;
            this.chNotif.Location = new System.Drawing.Point(278, 21);
            this.chNotif.Name = "chNotif";
            this.chNotif.Size = new System.Drawing.Size(131, 17);
            this.chNotif.TabIndex = 4;
            this.chNotif.Text = "Show &notification area";
            this.chNotif.UseVisualStyleBackColor = true;
            this.chNotif.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // chAutoHide
            // 
            this.chAutoHide.AutoSize = true;
            this.chAutoHide.Location = new System.Drawing.Point(278, 90);
            this.chAutoHide.Name = "chAutoHide";
            this.chAutoHide.Size = new System.Drawing.Size(127, 17);
            this.chAutoHide.TabIndex = 8;
            this.chAutoHide.Text = "Auto-&hide the taskbar";
            this.chAutoHide.UseVisualStyleBackColor = true;
            this.chAutoHide.CheckedChanged += new System.EventHandler(this.chAutoHide_CheckedChanged);
            // 
            // chMirrorButtons
            // 
            this.chMirrorButtons.AutoSize = true;
            this.chMirrorButtons.Location = new System.Drawing.Point(10, 67);
            this.chMirrorButtons.Name = "chMirrorButtons";
            this.chMirrorButtons.Size = new System.Drawing.Size(81, 17);
            this.chMirrorButtons.TabIndex = 2;
            this.chMirrorButtons.Text = "&Mirror mode";
            this.chMirrorButtons.UseVisualStyleBackColor = true;
            this.chMirrorButtons.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(568, 385);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "&Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabTaskbar);
            this.tabControl1.Controls.Add(this.tabWindowManager);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(631, 367);
            this.tabControl1.TabIndex = 0;
            // 
            // tabTaskbar
            // 
            this.tabTaskbar.Controls.Add(this.groupBox5);
            this.tabTaskbar.Controls.Add(this.groupBox2);
            this.tabTaskbar.Controls.Add(this.groupBox1);
            this.tabTaskbar.Location = new System.Drawing.Point(4, 22);
            this.tabTaskbar.Name = "tabTaskbar";
            this.tabTaskbar.Padding = new System.Windows.Forms.Padding(3);
            this.tabTaskbar.Size = new System.Drawing.Size(623, 341);
            this.tabTaskbar.TabIndex = 0;
            this.tabTaskbar.Text = "Taskbar";
            this.tabTaskbar.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.panelLocations);
            this.groupBox5.Location = new System.Drawing.Point(6, 232);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(611, 103);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Taskbar location";
            // 
            // panelLocations
            // 
            this.panelLocations.AutoScroll = true;
            this.panelLocations.Location = new System.Drawing.Point(7, 19);
            this.panelLocations.Name = "panelLocations";
            this.panelLocations.Size = new System.Drawing.Size(598, 78);
            this.panelLocations.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chCheckForUpdates);
            this.groupBox2.Controls.Add(this.chAutoStart);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(611, 72);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // chCheckForUpdates
            // 
            this.chCheckForUpdates.AutoSize = true;
            this.chCheckForUpdates.Location = new System.Drawing.Point(8, 42);
            this.chCheckForUpdates.Name = "chCheckForUpdates";
            this.chCheckForUpdates.Size = new System.Drawing.Size(113, 17);
            this.chCheckForUpdates.TabIndex = 1;
            this.chCheckForUpdates.Text = "Check for &updates";
            this.chCheckForUpdates.UseVisualStyleBackColor = true;
            this.chCheckForUpdates.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // chAutoStart
            // 
            this.chAutoStart.AutoSize = true;
            this.chAutoStart.Location = new System.Drawing.Point(8, 19);
            this.chAutoStart.Name = "chAutoStart";
            this.chAutoStart.Size = new System.Drawing.Size(180, 17);
            this.chAutoStart.TabIndex = 0;
            this.chAutoStart.Text = "Automatically start with &Windows";
            this.chAutoStart.UseVisualStyleBackColor = true;
            this.chAutoStart.CheckedChanged += new System.EventHandler(this.OnFieldChanged);
            // 
            // tabWindowManager
            // 
            this.tabWindowManager.Controls.Add(this.btnRemove);
            this.tabWindowManager.Controls.Add(this.lbRules);
            this.tabWindowManager.Controls.Add(this.panelDetails);
            this.tabWindowManager.Controls.Add(this.btnAdd);
            this.tabWindowManager.Location = new System.Drawing.Point(4, 22);
            this.tabWindowManager.Name = "tabWindowManager";
            this.tabWindowManager.Padding = new System.Windows.Forms.Padding(3);
            this.tabWindowManager.Size = new System.Drawing.Size(623, 341);
            this.tabWindowManager.TabIndex = 1;
            this.tabWindowManager.Text = "Window Manager";
            this.tabWindowManager.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.Location = new System.Drawing.Point(99, 7);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(44, 31);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lbRules
            // 
            this.lbRules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbRules.BackColor = System.Drawing.Color.White;
            this.lbRules.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbRules.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbRules.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRules.FormattingEnabled = true;
            this.lbRules.ItemHeight = 40;
            this.lbRules.Location = new System.Drawing.Point(7, 44);
            this.lbRules.Name = "lbRules";
            this.lbRules.Size = new System.Drawing.Size(142, 280);
            this.lbRules.TabIndex = 2;
            this.lbRules.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbRules_DrawItem);
            this.lbRules.SelectedIndexChanged += new System.EventHandler(this.lbRules_SelectedIndexChanged);
            // 
            // panelDetails
            // 
            this.panelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDetails.Controls.Add(this.txtName);
            this.panelDetails.Controls.Add(this.groupBox4);
            this.panelDetails.Controls.Add(this.groupBox3);
            this.panelDetails.Controls.Add(this.pbIcon);
            this.panelDetails.Location = new System.Drawing.Point(149, 7);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(468, 321);
            this.panelDetails.TabIndex = 3;
            this.panelDetails.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDetails_Paint);
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(15, 15);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(301, 30);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "New Rule";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.cbMoveTo);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(15, 247);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(437, 60);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Action";
            // 
            // cbMoveTo
            // 
            this.cbMoveTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMoveTo.FormattingEnabled = true;
            this.cbMoveTo.Location = new System.Drawing.Point(58, 19);
            this.cbMoveTo.Name = "cbMoveTo";
            this.cbMoveTo.Size = new System.Drawing.Size(219, 21);
            this.cbMoveTo.TabIndex = 1;
            this.cbMoveTo.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Move to";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnBrowse);
            this.groupBox3.Controls.Add(this.ckProgram);
            this.groupBox3.Controls.Add(this.txtProgram);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.btnSelectWindow);
            this.groupBox3.Controls.Add(this.ckCaption);
            this.groupBox3.Controls.Add(this.ckClass);
            this.groupBox3.Controls.Add(this.txtCaption);
            this.groupBox3.Controls.Add(this.txtClass);
            this.groupBox3.Controls.Add(this.pbFindWindow);
            this.groupBox3.Location = new System.Drawing.Point(15, 53);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(437, 188);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Match by";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(357, 82);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // ckProgram
            // 
            this.ckProgram.AutoSize = true;
            this.ckProgram.Location = new System.Drawing.Point(9, 86);
            this.ckProgram.Name = "ckProgram";
            this.ckProgram.Size = new System.Drawing.Size(65, 17);
            this.ckProgram.TabIndex = 4;
            this.ckProgram.Text = "Program";
            this.ckProgram.UseVisualStyleBackColor = true;
            this.ckProgram.CheckedChanged += new System.EventHandler(this.ckProgram_CheckedChanged);
            // 
            // txtProgram
            // 
            this.txtProgram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProgram.Location = new System.Drawing.Point(115, 84);
            this.txtProgram.Name = "txtProgram";
            this.txtProgram.Size = new System.Drawing.Size(235, 20);
            this.txtProgram.TabIndex = 5;
            this.txtProgram.TextChanged += new System.EventHandler(this.txtProgram_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(126, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "or";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Find window";
            // 
            // btnSelectWindow
            // 
            this.btnSelectWindow.Location = new System.Drawing.Point(151, 140);
            this.btnSelectWindow.Name = "btnSelectWindow";
            this.btnSelectWindow.Size = new System.Drawing.Size(281, 23);
            this.btnSelectWindow.TabIndex = 9;
            this.btnSelectWindow.Text = "&Select window";
            this.btnSelectWindow.UseVisualStyleBackColor = true;
            this.btnSelectWindow.Click += new System.EventHandler(this.btnSelectWindow_Click);
            // 
            // ckCaption
            // 
            this.ckCaption.AutoSize = true;
            this.ckCaption.Location = new System.Drawing.Point(9, 55);
            this.ckCaption.Name = "ckCaption";
            this.ckCaption.Size = new System.Drawing.Size(103, 17);
            this.ckCaption.TabIndex = 2;
            this.ckCaption.Text = "Window caption";
            this.ckCaption.UseVisualStyleBackColor = true;
            this.ckCaption.CheckedChanged += new System.EventHandler(this.ckCaption_CheckedChanged);
            // 
            // ckClass
            // 
            this.ckClass.AutoSize = true;
            this.ckClass.Checked = true;
            this.ckClass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckClass.Location = new System.Drawing.Point(9, 24);
            this.ckClass.Name = "ckClass";
            this.ckClass.Size = new System.Drawing.Size(92, 17);
            this.ckClass.TabIndex = 0;
            this.ckClass.Text = "Window class";
            this.ckClass.UseVisualStyleBackColor = true;
            this.ckClass.CheckedChanged += new System.EventHandler(this.ckClass_CheckedChanged);
            // 
            // txtCaption
            // 
            this.txtCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCaption.Location = new System.Drawing.Point(115, 53);
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new System.Drawing.Size(316, 20);
            this.txtCaption.TabIndex = 3;
            this.txtCaption.TextChanged += new System.EventHandler(this.txtCaption_TextChanged);
            // 
            // txtClass
            // 
            this.txtClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClass.Location = new System.Drawing.Point(115, 22);
            this.txtClass.Name = "txtClass";
            this.txtClass.Size = new System.Drawing.Size(316, 20);
            this.txtClass.TabIndex = 1;
            this.txtClass.TextChanged += new System.EventHandler(this.txtClass_TextChanged);
            // 
            // pbFindWindow
            // 
            this.pbFindWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFindWindow.Image = global::DualMonitor.Properties.Resources.find_window_normal;
            this.pbFindWindow.Location = new System.Drawing.Point(82, 138);
            this.pbFindWindow.Name = "pbFindWindow";
            this.pbFindWindow.Size = new System.Drawing.Size(34, 35);
            this.pbFindWindow.TabIndex = 0;
            this.pbFindWindow.TabStop = false;
            this.pbFindWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbFindWindow_MouseDown);
            // 
            // pbIcon
            // 
            this.pbIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbIcon.Location = new System.Drawing.Point(420, 15);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(32, 32);
            this.pbIcon.TabIndex = 2;
            this.pbIcon.TabStop = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Location = new System.Drawing.Point(7, 7);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(86, 31);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(179, 391);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(35, 13);
            this.lblVersion.TabIndex = 11;
            this.lblVersion.Text = "label5";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(9, 390);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(164, 13);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Dual Monitor Taskbar Homepage";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // PropertiesWindow
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(655, 420);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Taskbar Properties";
            this.Load += new System.EventHandler(this.PropertiesWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlHideDelay.ResumeLayout(false);
            this.pnlHideDelay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutohideDelay)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabTaskbar.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabWindowManager.ResumeLayout(false);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFindWindow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chAutoStart;
        private System.Windows.Forms.CheckBox chShowLabels;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chShowClock;
        private System.Windows.Forms.CheckBox chSmallIcons;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabTaskbar;
        private System.Windows.Forms.TabPage tabWindowManager;
        private System.Windows.Forms.ListBox lbRules;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbMoveTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtCaption;
        private System.Windows.Forms.TextBox txtClass;
        private System.Windows.Forms.PictureBox pbFindWindow;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.CheckBox ckCaption;
        private System.Windows.Forms.CheckBox ckClass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectWindow;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.CheckBox ckProgram;
        private System.Windows.Forms.TextBox txtProgram;
        private System.Windows.Forms.CheckBox chMirrorButtons;
        private System.Windows.Forms.CheckBox chAutoHide;
        private System.Windows.Forms.CheckBox chNotif;
        private System.Windows.Forms.CheckBox chStart;
        private System.Windows.Forms.Button btnFont;
        private System.Windows.Forms.CheckBox chCustomFont;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chCheckForUpdates;
        private System.Windows.Forms.Panel pnlHideDelay;
        private System.Windows.Forms.NumericUpDown nudAutohideDelay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panelLocations;
    }
}