namespace Geex.Edit.UeF.MainForm
{
    partial class LateralPanel
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_splitter = new System.Windows.Forms.SplitContainer();
            this.m_objectPropertiesTab = new System.Windows.Forms.TabControl();
            this.m_gameObjectsTab = new System.Windows.Forms.TabPage();
            this.m_gameObjectPicker = new Geex.Edit.UeF.MainForm.GameObjectPicker();
            this.m_objectPropertiesPage = new System.Windows.Forms.TabPage();
            this.m_objectPropertiesLayout = new System.Windows.Forms.TableLayoutPanel();
            this.m_objectEditControl = new Geex.Edit.UeF.Views.ObjectEditControl();
            this.m_moduleStrip = new System.Windows.Forms.ToolStrip();
            this.m_tilesetsPage = new System.Windows.Forms.TabPage();
            this.m_tilePicker = new Geex.Edit.UeF.TilePicker();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
            this.m_splitter.Panel1.SuspendLayout();
            this.m_splitter.SuspendLayout();
            this.m_objectPropertiesTab.SuspendLayout();
            this.m_gameObjectsTab.SuspendLayout();
            this.m_objectPropertiesPage.SuspendLayout();
            this.m_objectPropertiesLayout.SuspendLayout();
            this.m_tilesetsPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_splitter
            // 
            this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitter.Location = new System.Drawing.Point(0, 0);
            this.m_splitter.Name = "m_splitter";
            this.m_splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_splitter.Panel1
            // 
            this.m_splitter.Panel1.Controls.Add(this.m_objectPropertiesTab);
            this.m_splitter.Size = new System.Drawing.Size(263, 520);
            this.m_splitter.SplitterDistance = 331;
            this.m_splitter.TabIndex = 0;
            // 
            // m_objectPropertiesTab
            // 
            this.m_objectPropertiesTab.Controls.Add(this.m_gameObjectsTab);
            this.m_objectPropertiesTab.Controls.Add(this.m_objectPropertiesPage);
            this.m_objectPropertiesTab.Controls.Add(this.m_tilesetsPage);
            this.m_objectPropertiesTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_objectPropertiesTab.HotTrack = true;
            this.m_objectPropertiesTab.Location = new System.Drawing.Point(0, 0);
            this.m_objectPropertiesTab.Name = "m_objectPropertiesTab";
            this.m_objectPropertiesTab.SelectedIndex = 0;
            this.m_objectPropertiesTab.Size = new System.Drawing.Size(263, 331);
            this.m_objectPropertiesTab.TabIndex = 0;
            // 
            // m_gameObjectsTab
            // 
            this.m_gameObjectsTab.Controls.Add(this.m_gameObjectPicker);
            this.m_gameObjectsTab.Location = new System.Drawing.Point(4, 22);
            this.m_gameObjectsTab.Name = "m_gameObjectsTab";
            this.m_gameObjectsTab.Padding = new System.Windows.Forms.Padding(3);
            this.m_gameObjectsTab.Size = new System.Drawing.Size(255, 305);
            this.m_gameObjectsTab.TabIndex = 0;
            this.m_gameObjectsTab.Text = "GameObjects";
            this.m_gameObjectsTab.UseVisualStyleBackColor = true;
            // 
            // m_gameObjectPicker
            // 
            this.m_gameObjectPicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_gameObjectPicker.Location = new System.Drawing.Point(3, 3);
            this.m_gameObjectPicker.Name = "m_gameObjectPicker";
            this.m_gameObjectPicker.Size = new System.Drawing.Size(249, 299);
            this.m_gameObjectPicker.TabIndex = 0;
            // 
            // m_objectPropertiesPage
            // 
            this.m_objectPropertiesPage.Controls.Add(this.m_objectPropertiesLayout);
            this.m_objectPropertiesPage.Location = new System.Drawing.Point(4, 22);
            this.m_objectPropertiesPage.Name = "m_objectPropertiesPage";
            this.m_objectPropertiesPage.Padding = new System.Windows.Forms.Padding(3);
            this.m_objectPropertiesPage.Size = new System.Drawing.Size(255, 305);
            this.m_objectPropertiesPage.TabIndex = 1;
            this.m_objectPropertiesPage.Text = "Propriétés";
            this.m_objectPropertiesPage.UseVisualStyleBackColor = true;
            // 
            // m_objectPropertiesLayout
            // 
            this.m_objectPropertiesLayout.ColumnCount = 1;
            this.m_objectPropertiesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_objectPropertiesLayout.Controls.Add(this.m_objectEditControl, 0, 1);
            this.m_objectPropertiesLayout.Controls.Add(this.m_moduleStrip, 0, 0);
            this.m_objectPropertiesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_objectPropertiesLayout.Location = new System.Drawing.Point(3, 3);
            this.m_objectPropertiesLayout.Name = "m_objectPropertiesLayout";
            this.m_objectPropertiesLayout.RowCount = 2;
            this.m_objectPropertiesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.m_objectPropertiesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_objectPropertiesLayout.Size = new System.Drawing.Size(249, 299);
            this.m_objectPropertiesLayout.TabIndex = 1;
            // 
            // m_objectEditControl
            // 
            this.m_objectEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_objectEditControl.Location = new System.Drawing.Point(3, 53);
            this.m_objectEditControl.Name = "m_objectEditControl";
            this.m_objectEditControl.Size = new System.Drawing.Size(243, 243);
            this.m_objectEditControl.TabIndex = 0;
            // 
            // m_moduleStrip
            // 
            this.m_moduleStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_moduleStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_moduleStrip.Location = new System.Drawing.Point(0, 0);
            this.m_moduleStrip.Name = "m_moduleStrip";
            this.m_moduleStrip.Size = new System.Drawing.Size(249, 50);
            this.m_moduleStrip.TabIndex = 1;
            this.m_moduleStrip.Text = "toolStrip1";
            // 
            // m_tilesetsPage
            // 
            this.m_tilesetsPage.Controls.Add(this.m_tilePicker);
            this.m_tilesetsPage.Location = new System.Drawing.Point(4, 22);
            this.m_tilesetsPage.Name = "m_tilesetsPage";
            this.m_tilesetsPage.Padding = new System.Windows.Forms.Padding(3);
            this.m_tilesetsPage.Size = new System.Drawing.Size(255, 305);
            this.m_tilesetsPage.TabIndex = 2;
            this.m_tilesetsPage.Text = "Tileset";
            this.m_tilesetsPage.UseVisualStyleBackColor = true;
            // 
            // m_tilePicker
            // 
            this.m_tilePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tilePicker.Location = new System.Drawing.Point(3, 3);
            this.m_tilePicker.Name = "m_tilePicker";
            this.m_tilePicker.SelectedArea = new System.Drawing.Rectangle(1, 1, 2, 2);
            this.m_tilePicker.Size = new System.Drawing.Size(249, 299);
            this.m_tilePicker.TabIndex = 0;
            this.m_tilePicker.Text = "TilePicker";
            // 
            // LateralPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_splitter);
            this.Name = "LateralPanel";
            this.Size = new System.Drawing.Size(263, 520);
            this.m_splitter.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
            this.m_splitter.ResumeLayout(false);
            this.m_objectPropertiesTab.ResumeLayout(false);
            this.m_gameObjectsTab.ResumeLayout(false);
            this.m_objectPropertiesPage.ResumeLayout(false);
            this.m_objectPropertiesLayout.ResumeLayout(false);
            this.m_objectPropertiesLayout.PerformLayout();
            this.m_tilesetsPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer m_splitter;
        private System.Windows.Forms.TabControl m_objectPropertiesTab;
        private System.Windows.Forms.TabPage m_gameObjectsTab;
        private System.Windows.Forms.TabPage m_objectPropertiesPage;
        private GameObjectPicker m_gameObjectPicker;
        private Views.ObjectEditControl m_objectEditControl;
        private System.Windows.Forms.TabPage m_tilesetsPage;
        private TilePicker m_tilePicker;
        private System.Windows.Forms.TableLayoutPanel m_objectPropertiesLayout;
        private System.Windows.Forms.ToolStrip m_moduleStrip;
    }
}
