namespace DualMonitor.Forms
{
    partial class NewVersionAlert
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewVersionAlert));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.lblNewVersion = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnRemindMeLater = new System.Windows.Forms.Button();
            this.btnDisable = new System.Windows.Forms.Button();
            this.txtChangelist = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(357, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "A new Dual Monitor Taskbar version is available. Do you want to update ?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Your version:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Latest version:";
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(95, 42);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(35, 13);
            this.lblCurrentVersion.TabIndex = 3;
            this.lblCurrentVersion.Text = "label4";
            // 
            // lblNewVersion
            // 
            this.lblNewVersion.AutoSize = true;
            this.lblNewVersion.Location = new System.Drawing.Point(95, 63);
            this.lblNewVersion.Name = "lblNewVersion";
            this.lblNewVersion.Size = new System.Drawing.Size(35, 13);
            this.lblNewVersion.TabIndex = 4;
            this.lblNewVersion.Text = "label5";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 268);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "Yes";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnRemindMeLater
            // 
            this.btnRemindMeLater.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRemindMeLater.Location = new System.Drawing.Point(83, 268);
            this.btnRemindMeLater.Name = "btnRemindMeLater";
            this.btnRemindMeLater.Size = new System.Drawing.Size(135, 23);
            this.btnRemindMeLater.TabIndex = 6;
            this.btnRemindMeLater.Text = "Remind me later";
            this.btnRemindMeLater.UseVisualStyleBackColor = true;
            // 
            // btnDisable
            // 
            this.btnDisable.Location = new System.Drawing.Point(224, 268);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(178, 23);
            this.btnDisable.TabIndex = 7;
            this.btnDisable.Text = "Disable auto check for updates";
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // txtChangelist
            // 
            this.txtChangelist.Location = new System.Drawing.Point(12, 88);
            this.txtChangelist.Multiline = true;
            this.txtChangelist.Name = "txtChangelist";
            this.txtChangelist.ReadOnly = true;
            this.txtChangelist.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChangelist.Size = new System.Drawing.Size(390, 174);
            this.txtChangelist.TabIndex = 8;
            // 
            // NewVersionAlert
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnRemindMeLater;
            this.ClientSize = new System.Drawing.Size(418, 303);
            this.Controls.Add(this.txtChangelist);
            this.Controls.Add(this.btnDisable);
            this.Controls.Add(this.btnRemindMeLater);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblNewVersion);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewVersionAlert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Version Available";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.Label lblNewVersion;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnRemindMeLater;
        private System.Windows.Forms.Button btnDisable;
        private System.Windows.Forms.TextBox txtChangelist;
    }
}