namespace TilesetMaskGenerator
{
    partial class MaskGeneratorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaskGeneratorForm));
            this.m_layout = new System.Windows.Forms.TableLayoutPanel();
            this._topBarPanel = new System.Windows.Forms.Panel();
            this.m_saveButton = new System.Windows.Forms.Button();
            this.m_loadButton = new System.Windows.Forms.Button();
            this.m_tilesetEdit = new TilesetMaskGenerator.TilesetEditControl();
            this.m_layout.SuspendLayout();
            this._topBarPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_layout
            // 
            this.m_layout.ColumnCount = 1;
            this.m_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_layout.Controls.Add(this._topBarPanel, 0, 0);
            this.m_layout.Controls.Add(this.m_tilesetEdit, 0, 1);
            this.m_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_layout.Location = new System.Drawing.Point(0, 0);
            this.m_layout.Name = "m_layout";
            this.m_layout.RowCount = 2;
            this.m_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.m_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_layout.Size = new System.Drawing.Size(288, 469);
            this.m_layout.TabIndex = 0;
            // 
            // _topBarPanel
            // 
            this._topBarPanel.BackColor = System.Drawing.Color.SkyBlue;
            this._topBarPanel.Controls.Add(this.m_saveButton);
            this._topBarPanel.Controls.Add(this.m_loadButton);
            this._topBarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._topBarPanel.Location = new System.Drawing.Point(3, 3);
            this._topBarPanel.Name = "_topBarPanel";
            this._topBarPanel.Size = new System.Drawing.Size(282, 64);
            this._topBarPanel.TabIndex = 0;
            // 
            // m_saveButton
            // 
            this.m_saveButton.Image = ((System.Drawing.Image)(resources.GetObject("m_saveButton.Image")));
            this.m_saveButton.Location = new System.Drawing.Point(81, 4);
            this.m_saveButton.Name = "m_saveButton";
            this.m_saveButton.Size = new System.Drawing.Size(72, 57);
            this.m_saveButton.TabIndex = 1;
            this.m_saveButton.Text = "Exporter";
            this.m_saveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_saveButton.UseVisualStyleBackColor = true;
            // 
            // m_loadButton
            // 
            this.m_loadButton.Image = ((System.Drawing.Image)(resources.GetObject("m_loadButton.Image")));
            this.m_loadButton.Location = new System.Drawing.Point(3, 4);
            this.m_loadButton.Name = "m_loadButton";
            this.m_loadButton.Size = new System.Drawing.Size(72, 57);
            this.m_loadButton.TabIndex = 0;
            this.m_loadButton.Text = "Charger";
            this.m_loadButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_loadButton.UseVisualStyleBackColor = true;
            // 
            // m_tilesetEdit
            // 
            this.m_tilesetEdit.AutoScroll = true;
            this.m_tilesetEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tilesetEdit.EditedBitmap = null;
            this.m_tilesetEdit.Location = new System.Drawing.Point(3, 73);
            this.m_tilesetEdit.Name = "m_tilesetEdit";
            this.m_tilesetEdit.Size = new System.Drawing.Size(282, 393);
            this.m_tilesetEdit.TabIndex = 1;
            // 
            // MaskGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 469);
            this.Controls.Add(this.m_layout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MaskGeneratorForm";
            this.Text = "Editeur de masque de tileset";
            this.m_layout.ResumeLayout(false);
            this._topBarPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel m_layout;
        private System.Windows.Forms.Panel _topBarPanel;
        private System.Windows.Forms.Button m_saveButton;
        private System.Windows.Forms.Button m_loadButton;
        private TilesetEditControl m_tilesetEdit;
    }
}

