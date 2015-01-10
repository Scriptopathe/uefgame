namespace Geex.Edit.Common.Tools.Controls
{
    partial class RessourcesManagerForm
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
            this.m_categoryTab = new System.Windows.Forms.TabControl();
            this.m_graphicsTab = new System.Windows.Forms.TabPage();
            this.m_graphicsFilesLabel = new System.Windows.Forms.Label();
            this.m_graphicsCatLabel = new System.Windows.Forms.Label();
            this.m_graphicsDeleteButton = new System.Windows.Forms.Button();
            this.m_graphicsImportButton = new System.Windows.Forms.Button();
            this.m_graphicsPreview = new Geex.Edit.Common.Tools.Controls.AdvancedPictureBoxPanel();
            this.m_graphicsFilenamesListBox = new System.Windows.Forms.ListView();
            this.m_graphicsCategoriesListBox = new System.Windows.Forms.ListView();
            this.m_audioTab = new System.Windows.Forms.TabPage();
            this.m_audioFilesLabel = new System.Windows.Forms.Label();
            this.m_audioCatLabel = new System.Windows.Forms.Label();
            this.m_musicStopButton = new System.Windows.Forms.Button();
            this.m_musicPlayButton = new System.Windows.Forms.Button();
            this.m_audioDeleteButton = new System.Windows.Forms.Button();
            this.m_audioImportButton = new System.Windows.Forms.Button();
            this.m_audioFilenames = new System.Windows.Forms.ListView();
            this.m_audioCategoriesListBox = new System.Windows.Forms.ListView();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_categoryTab.SuspendLayout();
            this.m_graphicsTab.SuspendLayout();
            this.m_audioTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_categoryTab
            // 
            this.m_categoryTab.Controls.Add(this.m_graphicsTab);
            this.m_categoryTab.Controls.Add(this.m_audioTab);
            this.m_categoryTab.Location = new System.Drawing.Point(12, 12);
            this.m_categoryTab.Name = "m_categoryTab";
            this.m_categoryTab.SelectedIndex = 0;
            this.m_categoryTab.Size = new System.Drawing.Size(687, 422);
            this.m_categoryTab.TabIndex = 0;
            // 
            // m_graphicsTab
            // 
            this.m_graphicsTab.Controls.Add(this.m_graphicsFilesLabel);
            this.m_graphicsTab.Controls.Add(this.m_graphicsCatLabel);
            this.m_graphicsTab.Controls.Add(this.m_graphicsDeleteButton);
            this.m_graphicsTab.Controls.Add(this.m_graphicsImportButton);
            this.m_graphicsTab.Controls.Add(this.m_graphicsPreview);
            this.m_graphicsTab.Controls.Add(this.m_graphicsFilenamesListBox);
            this.m_graphicsTab.Controls.Add(this.m_graphicsCategoriesListBox);
            this.m_graphicsTab.Location = new System.Drawing.Point(4, 22);
            this.m_graphicsTab.Name = "m_graphicsTab";
            this.m_graphicsTab.Padding = new System.Windows.Forms.Padding(3);
            this.m_graphicsTab.Size = new System.Drawing.Size(679, 396);
            this.m_graphicsTab.TabIndex = 0;
            this.m_graphicsTab.Text = "Graphics";
            this.m_graphicsTab.UseVisualStyleBackColor = true;
            // 
            // m_graphicsFilesLabel
            // 
            this.m_graphicsFilesLabel.AutoSize = true;
            this.m_graphicsFilesLabel.Location = new System.Drawing.Point(159, 11);
            this.m_graphicsFilesLabel.Name = "m_graphicsFilesLabel";
            this.m_graphicsFilesLabel.Size = new System.Drawing.Size(28, 13);
            this.m_graphicsFilesLabel.TabIndex = 9;
            this.m_graphicsFilesLabel.Text = "Files";
            // 
            // m_graphicsCatLabel
            // 
            this.m_graphicsCatLabel.AutoSize = true;
            this.m_graphicsCatLabel.Location = new System.Drawing.Point(6, 11);
            this.m_graphicsCatLabel.Name = "m_graphicsCatLabel";
            this.m_graphicsCatLabel.Size = new System.Drawing.Size(57, 13);
            this.m_graphicsCatLabel.TabIndex = 8;
            this.m_graphicsCatLabel.Text = "Categories";
            // 
            // m_graphicsDeleteButton
            // 
            this.m_graphicsDeleteButton.Location = new System.Drawing.Point(598, 6);
            this.m_graphicsDeleteButton.Name = "m_graphicsDeleteButton";
            this.m_graphicsDeleteButton.Size = new System.Drawing.Size(75, 23);
            this.m_graphicsDeleteButton.TabIndex = 4;
            this.m_graphicsDeleteButton.Text = "Delete";
            this.m_graphicsDeleteButton.UseVisualStyleBackColor = true;
            // 
            // m_graphicsImportButton
            // 
            this.m_graphicsImportButton.Location = new System.Drawing.Point(517, 6);
            this.m_graphicsImportButton.Name = "m_graphicsImportButton";
            this.m_graphicsImportButton.Size = new System.Drawing.Size(75, 23);
            this.m_graphicsImportButton.TabIndex = 3;
            this.m_graphicsImportButton.Text = "Import";
            this.m_graphicsImportButton.UseVisualStyleBackColor = true;
            // 
            // m_graphicsPreview
            // 
            this.m_graphicsPreview.Image = null;
            this.m_graphicsPreview.Location = new System.Drawing.Point(318, 41);
            this.m_graphicsPreview.Name = "m_graphicsPreview";
            this.m_graphicsPreview.ShowSaturation = false;
            this.m_graphicsPreview.Size = new System.Drawing.Size(355, 349);
            this.m_graphicsPreview.TabIndex = 2;
            // 
            // m_graphicsFilenamesListBox
            // 
            this.m_graphicsFilenamesListBox.HideSelection = false;
            this.m_graphicsFilenamesListBox.Location = new System.Drawing.Point(162, 41);
            this.m_graphicsFilenamesListBox.Name = "m_graphicsFilenamesListBox";
            this.m_graphicsFilenamesListBox.Size = new System.Drawing.Size(150, 349);
            this.m_graphicsFilenamesListBox.TabIndex = 1;
            this.m_graphicsFilenamesListBox.UseCompatibleStateImageBehavior = false;
            // 
            // m_graphicsCategoriesListBox
            // 
            this.m_graphicsCategoriesListBox.HideSelection = false;
            this.m_graphicsCategoriesListBox.Location = new System.Drawing.Point(9, 41);
            this.m_graphicsCategoriesListBox.Name = "m_graphicsCategoriesListBox";
            this.m_graphicsCategoriesListBox.Size = new System.Drawing.Size(147, 349);
            this.m_graphicsCategoriesListBox.TabIndex = 0;
            this.m_graphicsCategoriesListBox.UseCompatibleStateImageBehavior = false;
            // 
            // m_audioTab
            // 
            this.m_audioTab.Controls.Add(this.m_audioFilesLabel);
            this.m_audioTab.Controls.Add(this.m_audioCatLabel);
            this.m_audioTab.Controls.Add(this.m_musicStopButton);
            this.m_audioTab.Controls.Add(this.m_musicPlayButton);
            this.m_audioTab.Controls.Add(this.m_audioDeleteButton);
            this.m_audioTab.Controls.Add(this.m_audioImportButton);
            this.m_audioTab.Controls.Add(this.m_audioFilenames);
            this.m_audioTab.Controls.Add(this.m_audioCategoriesListBox);
            this.m_audioTab.Location = new System.Drawing.Point(4, 22);
            this.m_audioTab.Name = "m_audioTab";
            this.m_audioTab.Padding = new System.Windows.Forms.Padding(3);
            this.m_audioTab.Size = new System.Drawing.Size(679, 396);
            this.m_audioTab.TabIndex = 1;
            this.m_audioTab.Text = "Audio";
            this.m_audioTab.UseVisualStyleBackColor = true;
            // 
            // m_audioFilesLabel
            // 
            this.m_audioFilesLabel.AutoSize = true;
            this.m_audioFilesLabel.Location = new System.Drawing.Point(159, 11);
            this.m_audioFilesLabel.Name = "m_audioFilesLabel";
            this.m_audioFilesLabel.Size = new System.Drawing.Size(28, 13);
            this.m_audioFilesLabel.TabIndex = 7;
            this.m_audioFilesLabel.Text = "Files";
            // 
            // m_audioCatLabel
            // 
            this.m_audioCatLabel.AutoSize = true;
            this.m_audioCatLabel.Location = new System.Drawing.Point(6, 11);
            this.m_audioCatLabel.Name = "m_audioCatLabel";
            this.m_audioCatLabel.Size = new System.Drawing.Size(57, 13);
            this.m_audioCatLabel.TabIndex = 6;
            this.m_audioCatLabel.Text = "Categories";
            // 
            // m_musicStopButton
            // 
            this.m_musicStopButton.Location = new System.Drawing.Point(317, 6);
            this.m_musicStopButton.Name = "m_musicStopButton";
            this.m_musicStopButton.Size = new System.Drawing.Size(75, 23);
            this.m_musicStopButton.TabIndex = 5;
            this.m_musicStopButton.Text = "Stop";
            this.m_musicStopButton.UseVisualStyleBackColor = true;
            // 
            // m_musicPlayButton
            // 
            this.m_musicPlayButton.Location = new System.Drawing.Point(236, 6);
            this.m_musicPlayButton.Name = "m_musicPlayButton";
            this.m_musicPlayButton.Size = new System.Drawing.Size(75, 23);
            this.m_musicPlayButton.TabIndex = 4;
            this.m_musicPlayButton.Text = "Play";
            this.m_musicPlayButton.UseVisualStyleBackColor = true;
            // 
            // m_audioDeleteButton
            // 
            this.m_audioDeleteButton.Location = new System.Drawing.Point(598, 6);
            this.m_audioDeleteButton.Name = "m_audioDeleteButton";
            this.m_audioDeleteButton.Size = new System.Drawing.Size(75, 23);
            this.m_audioDeleteButton.TabIndex = 3;
            this.m_audioDeleteButton.Text = "Delete";
            this.m_audioDeleteButton.UseVisualStyleBackColor = true;
            // 
            // m_audioImportButton
            // 
            this.m_audioImportButton.Location = new System.Drawing.Point(517, 6);
            this.m_audioImportButton.Name = "m_audioImportButton";
            this.m_audioImportButton.Size = new System.Drawing.Size(75, 23);
            this.m_audioImportButton.TabIndex = 2;
            this.m_audioImportButton.Text = "Import";
            this.m_audioImportButton.UseVisualStyleBackColor = true;
            // 
            // m_audioFilenames
            // 
            this.m_audioFilenames.HideSelection = false;
            this.m_audioFilenames.Location = new System.Drawing.Point(162, 41);
            this.m_audioFilenames.Name = "m_audioFilenames";
            this.m_audioFilenames.Size = new System.Drawing.Size(511, 349);
            this.m_audioFilenames.TabIndex = 1;
            this.m_audioFilenames.UseCompatibleStateImageBehavior = false;
            // 
            // m_audioCategoriesListBox
            // 
            this.m_audioCategoriesListBox.HideSelection = false;
            this.m_audioCategoriesListBox.Location = new System.Drawing.Point(9, 41);
            this.m_audioCategoriesListBox.Name = "m_audioCategoriesListBox";
            this.m_audioCategoriesListBox.Size = new System.Drawing.Size(147, 349);
            this.m_audioCategoriesListBox.TabIndex = 0;
            this.m_audioCategoriesListBox.UseCompatibleStateImageBehavior = false;
            // 
            // m_okButton
            // 
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(620, 440);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(75, 23);
            this.m_okButton.TabIndex = 1;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            // 
            // RessourcesManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 468);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.m_categoryTab);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RessourcesManagerForm";
            this.Text = "RessourcesManagerForm";
            this.Load += new System.EventHandler(this.RessourcesManagerForm_Load);
            this.m_categoryTab.ResumeLayout(false);
            this.m_graphicsTab.ResumeLayout(false);
            this.m_graphicsTab.PerformLayout();
            this.m_audioTab.ResumeLayout(false);
            this.m_audioTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_categoryTab;
        private System.Windows.Forms.TabPage m_graphicsTab;
        private System.Windows.Forms.ListView m_graphicsFilenamesListBox;
        private System.Windows.Forms.ListView m_graphicsCategoriesListBox;
        private System.Windows.Forms.TabPage m_audioTab;
        private Common.Tools.Controls.AdvancedPictureBoxPanel m_graphicsPreview;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.ListView m_audioCategoriesListBox;
        private System.Windows.Forms.Button m_audioImportButton;
        private System.Windows.Forms.ListView m_audioFilenames;
        private System.Windows.Forms.Button m_audioDeleteButton;
        private System.Windows.Forms.Label m_graphicsFilesLabel;
        private System.Windows.Forms.Label m_graphicsCatLabel;
        private System.Windows.Forms.Label m_audioFilesLabel;
        private System.Windows.Forms.Label m_audioCatLabel;
        private System.Windows.Forms.Button m_musicStopButton;
        private System.Windows.Forms.Button m_musicPlayButton;
        private System.Windows.Forms.Button m_graphicsDeleteButton;
        private System.Windows.Forms.Button m_graphicsImportButton;
    }
}