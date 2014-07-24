using System.IO;
using System.Drawing;
using DualMonitor.Controls;

namespace DualMonitor.Forms
{
    partial class SecondaryTaskbar
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
            this.components = new System.ComponentModel.Container();
            this.flowPanel = new DualMonitor.Controls.TaskbarFlow();
            this.taskbarMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiStartTaskManager = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLockTaskbar = new System.Windows.Forms.ToolStripMenuItem();
            this.dummyButton = new System.Windows.Forms.Button();
            this.processMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiLaunch = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUnpin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCloseWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlClock = new DualMonitor.Controls.ClockPanel();
            //this.pnlStartMenu = new DualMonitor.Controls.StartMenuPanel();
            this.pnlResize = new DualMonitor.Controls.ResizePanel();
            this.pnlProgramsFlow = new System.Windows.Forms.Panel();
            this.pnlNotificationArea = new NotificationAreaPanel();
            this.pnlNotificationBorder = new BorderedTransparentPanel(this);
            this.btnShowDesktop = new ShowDesktopButton();
            this.taskbarMenu.SuspendLayout();
            this.processMenu.SuspendLayout();
            this.pnlProgramsFlow.SuspendLayout();
            this.pnlNotificationArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowPanel
            // 
            this.flowPanel.AutoScroll = true;
            this.flowPanel.ContextMenuStrip = this.taskbarMenu;
            this.flowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanel.Location = new System.Drawing.Point(0, 0);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(178, 36);            
            this.flowPanel.TabIndex = 0;
            // 
            // taskbarMenu
            // 
            this.taskbarMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiStartTaskManager,
            this.toolStripSeparator1,
            this.tsmiLockTaskbar,
            this.tsmiProperties,
            this.tsmiExit});
            this.taskbarMenu.Name = "taskbarMenu";
            this.taskbarMenu.Size = new System.Drawing.Size(176, 76);
            // 
            // tsmiStartTaskManager
            // 
            this.tsmiStartTaskManager.Name = "tsmiStartTaskManager";
            this.tsmiStartTaskManager.Size = new System.Drawing.Size(175, 22);
            this.tsmiStartTaskManager.Text = "Start Task Manager";
            this.tsmiStartTaskManager.Click += new System.EventHandler(this.tsmiStartTaskManager_Click);            
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(172, 6);            
            // 
            // tsmiProperties
            // 
            this.tsmiProperties.Name = "tsmiProperties";
            this.tsmiProperties.Size = new System.Drawing.Size(175, 22);
            this.tsmiProperties.Text = "Properties";
            this.tsmiProperties.Click += new System.EventHandler(this.tsmiProperties_Click);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(175, 22);
            this.tsmiExit.Text = "E&xit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // tsmiLockTaskbar
            // 
            this.tsmiLockTaskbar.CheckOnClick = true;
            this.tsmiLockTaskbar.Name = "tsmiLockTaskbar";
            this.tsmiLockTaskbar.Size = new System.Drawing.Size(175, 22);
            this.tsmiLockTaskbar.Text = "Lock the taskbar";
            this.tsmiLockTaskbar.CheckedChanged += new System.EventHandler(this.tsmiLockTaskbar_CheckedChanged);
            // 
            // dummyButton
            // 
            this.dummyButton.Location = new System.Drawing.Point(-15, 0);
            this.dummyButton.Name = "dummyButton";
            this.dummyButton.Size = new System.Drawing.Size(10, 10);
            this.dummyButton.TabIndex = 0;
            this.dummyButton.UseVisualStyleBackColor = true;
            // 
            // btnShowDesktop
            // 
            this.btnShowDesktop.Location = new System.Drawing.Point(-15, 0);
            this.btnShowDesktop.Name = "btnShowDesktop";
            this.btnShowDesktop.Size = new System.Drawing.Size(10, 10);
            this.btnShowDesktop.TabIndex = 0;
            this.btnShowDesktop.UseVisualStyleBackColor = true;
            // 
            // processMenu
            // 
            this.processMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLaunch,
            this.tsmiUnpin,
            this.tsmiPin,
            this.tsmiCloseWindow});
            this.processMenu.Name = "processMenu";
            this.processMenu.Size = new System.Drawing.Size(248, 114);
            this.processMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.processMenu_Closed);
            this.processMenu.Opening += new System.ComponentModel.CancelEventHandler(this.processMenu_Opening);
            this.processMenu.Opened += new System.EventHandler(processMenu_Opened);
            this.processMenu.LocationChanged += new System.EventHandler(processMenu_Opened);
            // 
            // tsmiLaunch
            // 
            this.tsmiLaunch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsmiLaunch.Name = "tsmiLaunch";
            this.tsmiLaunch.Size = new System.Drawing.Size(247, 22);
            this.tsmiLaunch.Click += new System.EventHandler(this.tsmiLaunch_Click);
            // 
            // tsmiUnpin
            // 
            this.tsmiUnpin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tsmiUnpin.Image = global::DualMonitor.Properties.Resources.unpin;
            this.tsmiUnpin.Name = "tsmiUnpin";
            this.tsmiUnpin.Size = new System.Drawing.Size(247, 22);
            this.tsmiUnpin.Text = "Unpin this program from taskbar";
            this.tsmiUnpin.Click += new System.EventHandler(this.tsmiUnpin_Click);
            // 
            // tsmiPin
            // 
            this.tsmiPin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tsmiPin.Image = global::DualMonitor.Properties.Resources.pin;
            this.tsmiPin.Name = "tsmiPin";
            this.tsmiPin.Size = new System.Drawing.Size(247, 22);
            this.tsmiPin.Text = "Pin this program to taskbar";
            this.tsmiPin.Click += new System.EventHandler(this.tsmiPin_Click);
            // 
            // tsmiCloseWindow
            // 
            this.tsmiCloseWindow.Image = global::DualMonitor.Properties.Resources.close;
            this.tsmiCloseWindow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsmiCloseWindow.Name = "tsmiCloseWindow";
            this.tsmiCloseWindow.Size = new System.Drawing.Size(247, 22);
            this.tsmiCloseWindow.Text = "Close window";
            this.tsmiCloseWindow.Click += new System.EventHandler(this.tsmiCloseWindow_Click);
            // 
            // pnlClock
            // 
            this.pnlClock.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlClock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlClock.Location = new System.Drawing.Point(241, 3);
            this.pnlClock.Name = "pnlClock";
            this.pnlClock.Size = new System.Drawing.Size(40, 30);
            this.pnlClock.Padding = new System.Windows.Forms.Padding(0);
            this.pnlClock.Margin = new System.Windows.Forms.Padding(0);
            this.pnlClock.TabIndex = 0;
            // 
            // pnlStartMenu
            // 
            //this.pnlStartMenu.Dock = System.Windows.Forms.DockStyle.Left;
            //this.pnlStartMenu.Hover = false;
            //this.pnlStartMenu.Location = new System.Drawing.Point(3, 3);
            //this.pnlStartMenu.Name = "pnlStartMenu";
            //this.pnlStartMenu.Size = new System.Drawing.Size(54, 30);
            //this.pnlStartMenu.StartMenuVisible = false;
            //this.pnlStartMenu.TabIndex = 0;
            // 
            // pnlResize
            // 
            this.pnlResize.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.pnlResize.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlResize.Location = new System.Drawing.Point(0, 0);
            this.pnlResize.Name = "pnlResize";
            this.pnlResize.Size = new System.Drawing.Size(285, 4);
            this.pnlResize.TabIndex = 0;            
            // 
            // pnlProgramsFlow
            // 
            this.pnlProgramsFlow.Controls.Add(this.flowPanel);
            this.pnlProgramsFlow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlProgramsFlow.Location = new System.Drawing.Point(60, 0);
            this.pnlProgramsFlow.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProgramsFlow.Name = "pnlProgramsFlow";
            this.pnlProgramsFlow.Size = new System.Drawing.Size(178, 36);
            this.pnlProgramsFlow.TabIndex = 0;
            this.pnlProgramsFlow.BackColor = Color.Transparent;
            // 
            // pnlNotificationArea
            // 
            this.pnlNotificationArea.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlNotificationArea.Location = new System.Drawing.Point(60, 0);
            this.pnlNotificationArea.Margin = new System.Windows.Forms.Padding(0);
            this.pnlNotificationArea.Name = "pnlNotificationArea";
            this.pnlNotificationArea.Size = new System.Drawing.Size(178, 36);
            this.pnlNotificationArea.TabIndex = 0;
            this.pnlNotificationArea.BackColor = Color.Transparent;
            // 
            // SecondaryTaskbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(148)))), ((int)(((byte)(170)))));
            this.ClientSize = new System.Drawing.Size(285, 40);
            this.ControlBox = false;
            this.Controls.Add(this.dummyButton);
            this.Controls.Add(this.pnlResize);
            //this.Controls.Add(this.pnlStartMenu);
            this.Controls.Add(this.pnlProgramsFlow);
            this.Controls.Add(this.pnlNotificationBorder);            
            this.pnlNotificationBorder.Controls.Add(this.pnlClock);
            this.pnlNotificationBorder.Controls.Add(this.pnlNotificationArea);
            this.pnlNotificationBorder.Controls.Add(this.btnShowDesktop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SecondaryTaskbar";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SecondaryTaskbar";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SecondaryTaskbar_FormClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SecondaryTaskbar_Paint);
            this.taskbarMenu.ResumeLayout(false);
            this.processMenu.ResumeLayout(false);
            this.pnlProgramsFlow.ResumeLayout(false);
            this.pnlNotificationArea.ResumeLayout(false);
            this.ResumeLayout(false);

        }        
        
        #endregion

        private TaskbarFlow flowPanel;
        private System.Windows.Forms.Button dummyButton;
        private System.Windows.Forms.ContextMenuStrip taskbarMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiStartTaskManager;
        private System.Windows.Forms.ContextMenuStrip processMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiLaunch;
        private System.Windows.Forms.ToolStripMenuItem tsmiCloseWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiProperties;
        private System.Windows.Forms.ToolStripMenuItem tsmiLockTaskbar;        
        private System.Windows.Forms.Panel pnlProgramsFlow;
        private NotificationAreaPanel pnlNotificationArea;
        private ClockPanel pnlClock;
        private BorderedTransparentPanel pnlNotificationBorder;
        private ShowDesktopButton btnShowDesktop;
        //private StartMenuPanel pnlStartMenu;
        private ResizePanel pnlResize;
        private System.Windows.Forms.ToolStripMenuItem tsmiUnpin;
        private System.Windows.Forms.ToolStripMenuItem tsmiPin;
    }
}

