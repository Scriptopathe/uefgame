namespace Geex.Edit.UeF.Views
{
    partial class ObjectEditControl
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
            this.m_listView = new Geex.Edit.UeF.Controls.ListViewEx();
            this.m_propertyColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_valueHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // m_listView
            // 
            this.m_listView.AllowColumnReorder = true;
            this.m_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_propertyColumn,
            this.m_valueHeader});
            this.m_listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_listView.DoubleClickActivation = false;
            this.m_listView.Font = new System.Drawing.Font("Microsoft Tai Le", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_listView.FullRowSelect = true;
            this.m_listView.GridLines = true;
            this.m_listView.HideSelection = false;
            this.m_listView.Location = new System.Drawing.Point(0, 0);
            this.m_listView.MultiSelect = false;
            this.m_listView.Name = "m_listView";
            this.m_listView.Size = new System.Drawing.Size(430, 317);
            this.m_listView.TabIndex = 0;
            this.m_listView.UseCompatibleStateImageBehavior = false;
            this.m_listView.View = System.Windows.Forms.View.Details;
            // 
            // m_propertyColumn
            // 
            this.m_propertyColumn.Text = "Propriétés";
            this.m_propertyColumn.Width = 259;
            // 
            // m_valueHeader
            // 
            this.m_valueHeader.Text = "Valeur";
            this.m_valueHeader.Width = 166;
            // 
            // ObjectEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_listView);
            this.Name = "ObjectEditControl";
            this.Size = new System.Drawing.Size(430, 317);
            this.ResumeLayout(false);

        }

        #endregion

        private UeF.Controls.ListViewEx m_listView;
        private System.Windows.Forms.ColumnHeader m_propertyColumn;
        private System.Windows.Forms.ColumnHeader m_valueHeader;
    }
}
