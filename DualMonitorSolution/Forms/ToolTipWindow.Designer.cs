using DualMonitor.Controls;
namespace DualMonitor.Forms
{
    partial class ToolTipWindow
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
            this.panelTitle = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelTitle
            // 
            this.panelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTitle.Location = new System.Drawing.Point(10, 10);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(256, 32);
            this.panelTitle.TabIndex = 0;
            this.panelTitle.Click += new System.EventHandler(this.panelTitle_Click);
            this.panelTitle.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTitle_Paint);
            this.panelTitle.MouseEnter += new System.EventHandler(this.panelTitle_MouseEnter);
            this.panelTitle.MouseLeave += new System.EventHandler(this.panelTitle_MouseLeave);
            this.panelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseMove);
            // 
            // ToolTipWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 53);
            this.ControlBox = false;
            this.Controls.Add(this.panelTitle);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToolTipWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ToolTipWindow";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ToolTipWindow_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ToolTipWindow_Paint);
            this.MouseEnter += new System.EventHandler(this.ToolTipWindow_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ToolTipWindow_MouseLeave);
            this.ResumeLayout(false);

        }        

        #endregion

        private System.Windows.Forms.Panel panelTitle;
    }
}