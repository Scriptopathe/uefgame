namespace Geex.Edit.UeF.Views
{
    partial class MapPropertiesForm
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
            this.m_sizeGroupbox = new System.Windows.Forms.GroupBox();
            this.m_widthLabel = new System.Windows.Forms.Label();
            this.m_heightLabel = new System.Windows.Forms.Label();
            this.m_widthUpdown = new System.Windows.Forms.NumericUpDown();
            this.m_heightUpdown = new System.Windows.Forms.NumericUpDown();
            this.m_tilesetLabel = new System.Windows.Forms.Label();
            this.m_tilesetCombo = new System.Windows.Forms.ComboBox();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_cancelButton = new System.Windows.Forms.Button();
            this.m_nameLabel = new System.Windows.Forms.Label();
            this.m_mapNameTextbox = new System.Windows.Forms.TextBox();
            this.m_sizeGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_widthUpdown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_heightUpdown)).BeginInit();
            this.SuspendLayout();
            // 
            // m_sizeGroupbox
            // 
            this.m_sizeGroupbox.Controls.Add(this.m_heightUpdown);
            this.m_sizeGroupbox.Controls.Add(this.m_widthUpdown);
            this.m_sizeGroupbox.Controls.Add(this.m_heightLabel);
            this.m_sizeGroupbox.Controls.Add(this.m_widthLabel);
            this.m_sizeGroupbox.Location = new System.Drawing.Point(12, 52);
            this.m_sizeGroupbox.Name = "m_sizeGroupbox";
            this.m_sizeGroupbox.Size = new System.Drawing.Size(232, 70);
            this.m_sizeGroupbox.TabIndex = 0;
            this.m_sizeGroupbox.TabStop = false;
            this.m_sizeGroupbox.Text = "Taille de la map (tiles)";
            // 
            // m_widthLabel
            // 
            this.m_widthLabel.AutoSize = true;
            this.m_widthLabel.Location = new System.Drawing.Point(7, 20);
            this.m_widthLabel.Name = "m_widthLabel";
            this.m_widthLabel.Size = new System.Drawing.Size(49, 13);
            this.m_widthLabel.TabIndex = 0;
            this.m_widthLabel.Text = "Largeur :";
            // 
            // m_heightLabel
            // 
            this.m_heightLabel.AutoSize = true;
            this.m_heightLabel.Location = new System.Drawing.Point(117, 20);
            this.m_heightLabel.Name = "m_heightLabel";
            this.m_heightLabel.Size = new System.Drawing.Size(51, 13);
            this.m_heightLabel.TabIndex = 1;
            this.m_heightLabel.Text = "Hauteur :";
            // 
            // m_widthUpdown
            // 
            this.m_widthUpdown.Location = new System.Drawing.Point(10, 37);
            this.m_widthUpdown.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.m_widthUpdown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_widthUpdown.Name = "m_widthUpdown";
            this.m_widthUpdown.Size = new System.Drawing.Size(104, 20);
            this.m_widthUpdown.TabIndex = 2;
            this.m_widthUpdown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // m_heightUpdown
            // 
            this.m_heightUpdown.Location = new System.Drawing.Point(120, 37);
            this.m_heightUpdown.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.m_heightUpdown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_heightUpdown.Name = "m_heightUpdown";
            this.m_heightUpdown.Size = new System.Drawing.Size(104, 20);
            this.m_heightUpdown.TabIndex = 3;
            this.m_heightUpdown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // m_tilesetLabel
            // 
            this.m_tilesetLabel.AutoSize = true;
            this.m_tilesetLabel.Location = new System.Drawing.Point(19, 125);
            this.m_tilesetLabel.Name = "m_tilesetLabel";
            this.m_tilesetLabel.Size = new System.Drawing.Size(44, 13);
            this.m_tilesetLabel.TabIndex = 2;
            this.m_tilesetLabel.Text = "Tileset :";
            // 
            // m_tilesetCombo
            // 
            this.m_tilesetCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_tilesetCombo.FormattingEnabled = true;
            this.m_tilesetCombo.Location = new System.Drawing.Point(22, 141);
            this.m_tilesetCombo.Name = "m_tilesetCombo";
            this.m_tilesetCombo.Size = new System.Drawing.Size(214, 21);
            this.m_tilesetCombo.TabIndex = 3;
            // 
            // m_okButton
            // 
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(169, 201);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(75, 23);
            this.m_okButton.TabIndex = 4;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            // 
            // m_cancelButton
            // 
            this.m_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_cancelButton.Location = new System.Drawing.Point(88, 201);
            this.m_cancelButton.Name = "m_cancelButton";
            this.m_cancelButton.Size = new System.Drawing.Size(75, 23);
            this.m_cancelButton.TabIndex = 5;
            this.m_cancelButton.Text = "Annuler";
            this.m_cancelButton.UseVisualStyleBackColor = true;
            // 
            // m_nameLabel
            // 
            this.m_nameLabel.AutoSize = true;
            this.m_nameLabel.Location = new System.Drawing.Point(19, 9);
            this.m_nameLabel.Name = "m_nameLabel";
            this.m_nameLabel.Size = new System.Drawing.Size(84, 13);
            this.m_nameLabel.TabIndex = 6;
            this.m_nameLabel.Text = "Nom de la map :";
            // 
            // m_mapNameTextbox
            // 
            this.m_mapNameTextbox.Location = new System.Drawing.Point(22, 26);
            this.m_mapNameTextbox.Name = "m_mapNameTextbox";
            this.m_mapNameTextbox.Size = new System.Drawing.Size(214, 20);
            this.m_mapNameTextbox.TabIndex = 7;
            // 
            // MapPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 236);
            this.Controls.Add(this.m_mapNameTextbox);
            this.Controls.Add(this.m_nameLabel);
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.m_tilesetCombo);
            this.Controls.Add(this.m_tilesetLabel);
            this.Controls.Add(this.m_sizeGroupbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapPropertiesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Propriétés";
            this.m_sizeGroupbox.ResumeLayout(false);
            this.m_sizeGroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_widthUpdown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_heightUpdown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox m_sizeGroupbox;
        private System.Windows.Forms.Label m_heightLabel;
        private System.Windows.Forms.Label m_widthLabel;
        private System.Windows.Forms.NumericUpDown m_heightUpdown;
        private System.Windows.Forms.NumericUpDown m_widthUpdown;
        private System.Windows.Forms.Label m_tilesetLabel;
        private System.Windows.Forms.ComboBox m_tilesetCombo;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.Button m_cancelButton;
        private System.Windows.Forms.Label m_nameLabel;
        private System.Windows.Forms.TextBox m_mapNameTextbox;
    }
}