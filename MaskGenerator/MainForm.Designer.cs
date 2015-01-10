namespace MaskGenerator
{
    partial class MainForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.m_srcLabel = new System.Windows.Forms.Label();
            this.m_srcFile = new System.Windows.Forms.TextBox();
            this.m_srcFilesButton = new System.Windows.Forms.Button();
            this.m_generateButton = new System.Windows.Forms.Button();
            this.m_verticesDisplay = new MaskGenerator.VerticesDisplay();
            this.SuspendLayout();
            // 
            // m_srcLabel
            // 
            this.m_srcLabel.AutoSize = true;
            this.m_srcLabel.Location = new System.Drawing.Point(13, 13);
            this.m_srcLabel.Name = "m_srcLabel";
            this.m_srcLabel.Size = new System.Drawing.Size(79, 13);
            this.m_srcLabel.TabIndex = 0;
            this.m_srcLabel.Text = "Fichier source :";
            // 
            // m_srcFile
            // 
            this.m_srcFile.Location = new System.Drawing.Point(13, 30);
            this.m_srcFile.Name = "m_srcFile";
            this.m_srcFile.ReadOnly = true;
            this.m_srcFile.Size = new System.Drawing.Size(931, 20);
            this.m_srcFile.TabIndex = 1;
            // 
            // m_srcFilesButton
            // 
            this.m_srcFilesButton.Location = new System.Drawing.Point(950, 28);
            this.m_srcFilesButton.Name = "m_srcFilesButton";
            this.m_srcFilesButton.Size = new System.Drawing.Size(95, 23);
            this.m_srcFilesButton.TabIndex = 2;
            this.m_srcFilesButton.Text = "Parcourir...";
            this.m_srcFilesButton.UseVisualStyleBackColor = true;
            // 
            // m_generateButton
            // 
            this.m_generateButton.Location = new System.Drawing.Point(13, 57);
            this.m_generateButton.Name = "m_generateButton";
            this.m_generateButton.Size = new System.Drawing.Size(1032, 53);
            this.m_generateButton.TabIndex = 3;
            this.m_generateButton.Text = "Générer !";
            this.m_generateButton.UseVisualStyleBackColor = true;
            // 
            // m_verticesDisplay
            // 
            this.m_verticesDisplay.Image = null;
            this.m_verticesDisplay.Location = new System.Drawing.Point(13, 117);
            this.m_verticesDisplay.Name = "m_verticesDisplay";
            this.m_verticesDisplay.Size = new System.Drawing.Size(1032, 730);
            this.m_verticesDisplay.TabIndex = 4;
            this.m_verticesDisplay.Vertices = null;
            this.m_verticesDisplay.Zoom = 0F;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 859);
            this.Controls.Add(this.m_verticesDisplay);
            this.Controls.Add(this.m_generateButton);
            this.Controls.Add(this.m_srcFilesButton);
            this.Controls.Add(this.m_srcFile);
            this.Controls.Add(this.m_srcLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Générateur de masque";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_srcLabel;
        private System.Windows.Forms.TextBox m_srcFile;
        private System.Windows.Forms.Button m_srcFilesButton;
        private System.Windows.Forms.Button m_generateButton;
        private VerticesDisplay m_verticesDisplay;
    }
}

