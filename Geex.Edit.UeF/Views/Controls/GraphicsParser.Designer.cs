namespace Geex.Edit.UeF.Views
{
    partial class GraphicsParser
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
            this.m_listbox = new System.Windows.Forms.ListView();
            this.m_viewer = new Geex.Edit.Common.Tools.Controls.AdvancedPictureBoxPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_filesLabel = new System.Windows.Forms.Label();
            this.m_previewLabel = new System.Windows.Forms.Label();
            this.m_viewer.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_listbox
            // 
            this.m_listbox.Location = new System.Drawing.Point(6, 25);
            this.m_listbox.Name = "m_listbox";
            this.m_listbox.Size = new System.Drawing.Size(206, 478);
            this.m_listbox.TabIndex = 0;
            this.m_listbox.UseCompatibleStateImageBehavior = false;
            this.m_listbox.View = System.Windows.Forms.View.List;
            // 
            // m_viewer
            // 
            this.m_viewer.Controls.Add(this.button1);
            this.m_viewer.Image = null;
            this.m_viewer.Location = new System.Drawing.Point(218, 25);
            this.m_viewer.Name = "m_viewer";
            this.m_viewer.ShowSaturation = true;
            this.m_viewer.Size = new System.Drawing.Size(455, 478);
            this.m_viewer.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(304, 224);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(598, 509);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // m_okButton
            // 
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(517, 509);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(75, 23);
            this.m_okButton.TabIndex = 3;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            // 
            // m_filesLabel
            // 
            this.m_filesLabel.AutoSize = true;
            this.m_filesLabel.Location = new System.Drawing.Point(8, 5);
            this.m_filesLabel.Name = "m_filesLabel";
            this.m_filesLabel.Size = new System.Drawing.Size(28, 13);
            this.m_filesLabel.TabIndex = 4;
            this.m_filesLabel.Text = "Files";
            // 
            // m_previewLabel
            // 
            this.m_previewLabel.AutoSize = true;
            this.m_previewLabel.Location = new System.Drawing.Point(215, 5);
            this.m_previewLabel.Name = "m_previewLabel";
            this.m_previewLabel.Size = new System.Drawing.Size(45, 13);
            this.m_previewLabel.TabIndex = 5;
            this.m_previewLabel.Text = "Preview";
            // 
            // GraphicsParser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 540);
            this.Controls.Add(this.m_previewLabel);
            this.Controls.Add(this.m_filesLabel);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.m_viewer);
            this.Controls.Add(this.m_listbox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphicsParser";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "GraphicsParser";
            this.m_viewer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView m_listbox;
        private Common.Tools.Controls.AdvancedPictureBoxPanel m_viewer;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label m_filesLabel;
        private System.Windows.Forms.Label m_previewLabel;
    }
}