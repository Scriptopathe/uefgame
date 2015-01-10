namespace Geex.Edit.UeF.Views
{
    partial class ErrorsListView
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
            this.m_lineColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_errorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // m_lineColumn
            // 
            this.m_lineColumn.Text = "Ligne";
            this.m_lineColumn.Width = 40;
            // 
            // m_errorColumn
            // 
            this.m_errorColumn.Text = "Message d\'erreur";
            this.m_errorColumn.Width = 800;
            // 
            // ErrorsListView
            // 
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_lineColumn,
            this.m_errorColumn});
            this.FullRowSelect = true;
            this.GridLines = true;
            this.Size = new System.Drawing.Size(208, 150);
            this.View = System.Windows.Forms.View.Details;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader m_lineColumn;
        private System.Windows.Forms.ColumnHeader m_errorColumn;
    }
}
