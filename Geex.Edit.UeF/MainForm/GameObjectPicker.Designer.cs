namespace Geex.Edit.UeF.MainForm
{
    partial class GameObjectPicker
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
            this.m_objectsList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // m_objectsList
            // 
            this.m_objectsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_objectsList.FormattingEnabled = true;
            this.m_objectsList.Location = new System.Drawing.Point(0, 0);
            this.m_objectsList.Name = "m_objectsList";
            this.m_objectsList.Size = new System.Drawing.Size(253, 277);
            this.m_objectsList.TabIndex = 0;
            // 
            // GameObjectPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_objectsList);
            this.Name = "GameObjectPicker";
            this.Size = new System.Drawing.Size(253, 277);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox m_objectsList;
    }
}
