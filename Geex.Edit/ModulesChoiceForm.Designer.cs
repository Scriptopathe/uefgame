namespace Geex.Edit
{
    partial class ModulesChoiceForm
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
            this.m_label = new System.Windows.Forms.Label();
            this.m_modulesListBox = new System.Windows.Forms.ListBox();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_rememberCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // m_label
            // 
            this.m_label.AutoSize = true;
            this.m_label.Location = new System.Drawing.Point(13, 13);
            this.m_label.Name = "m_label";
            this.m_label.Size = new System.Drawing.Size(216, 13);
            this.m_label.TabIndex = 0;
            this.m_label.Text = "Please choose the module you want to use :";
            // 
            // m_modulesListBox
            // 
            this.m_modulesListBox.FormattingEnabled = true;
            this.m_modulesListBox.IntegralHeight = false;
            this.m_modulesListBox.Location = new System.Drawing.Point(13, 30);
            this.m_modulesListBox.Name = "m_modulesListBox";
            this.m_modulesListBox.Size = new System.Drawing.Size(344, 140);
            this.m_modulesListBox.TabIndex = 1;
            // 
            // m_okButton
            // 
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(282, 176);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(75, 23);
            this.m_okButton.TabIndex = 2;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            // 
            // m_rememberCheckbox
            // 
            this.m_rememberCheckbox.AutoSize = true;
            this.m_rememberCheckbox.Location = new System.Drawing.Point(13, 181);
            this.m_rememberCheckbox.Name = "m_rememberCheckbox";
            this.m_rememberCheckbox.Size = new System.Drawing.Size(128, 17);
            this.m_rememberCheckbox.TabIndex = 3;
            this.m_rememberCheckbox.Text = "Remember my choice";
            this.m_rememberCheckbox.UseVisualStyleBackColor = true;
            // 
            // ModulesChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 205);
            this.Controls.Add(this.m_rememberCheckbox);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.m_modulesListBox);
            this.Controls.Add(this.m_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModulesChoiceForm";
            this.Text = "PluginChoiceForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_label;
        private System.Windows.Forms.ListBox m_modulesListBox;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.CheckBox m_rememberCheckbox;
    }
}