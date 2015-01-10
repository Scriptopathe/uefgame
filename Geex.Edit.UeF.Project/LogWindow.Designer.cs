namespace Geex.Edit.UeF.Project
{
    partial class LogWindow
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
            this.m_logTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_logTextbox
            // 
            this.m_logTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_logTextbox.Location = new System.Drawing.Point(0, 0);
            this.m_logTextbox.Multiline = true;
            this.m_logTextbox.Name = "m_logTextbox";
            this.m_logTextbox.ReadOnly = true;
            this.m_logTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.m_logTextbox.Size = new System.Drawing.Size(479, 254);
            this.m_logTextbox.TabIndex = 0;
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 254);
            this.Controls.Add(this.m_logTextbox);
            this.Name = "LogWindow";
            this.Text = "Compilation UeF.Build";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_logTextbox;
    }
}