namespace Geex.Edit.Common.Tools.Controls
{
    partial class SliderUpDown
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
            this.m_layout = new System.Windows.Forms.TableLayoutPanel();
            this.m_trackbar = new System.Windows.Forms.TrackBar();
            this.m_updown = new System.Windows.Forms.NumericUpDown();
            this.m_layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_trackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_updown)).BeginInit();
            this.SuspendLayout();
            // 
            // m_layout
            // 
            this.m_layout.ColumnCount = 2;
            this.m_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.m_layout.Controls.Add(this.m_trackbar, 0, 0);
            this.m_layout.Controls.Add(this.m_updown, 1, 0);
            this.m_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_layout.Location = new System.Drawing.Point(0, 0);
            this.m_layout.Name = "m_layout";
            this.m_layout.RowCount = 1;
            this.m_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_layout.Size = new System.Drawing.Size(245, 40);
            this.m_layout.TabIndex = 0;
            // 
            // m_trackbar
            // 
            this.m_trackbar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.m_trackbar.Location = new System.Drawing.Point(3, 3);
            this.m_trackbar.Name = "m_trackbar";
            this.m_trackbar.Size = new System.Drawing.Size(159, 34);
            this.m_trackbar.TabIndex = 0;
            // 
            // m_updown
            // 
            this.m_updown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.m_updown.Location = new System.Drawing.Point(168, 10);
            this.m_updown.Name = "m_updown";
            this.m_updown.Size = new System.Drawing.Size(74, 20);
            this.m_updown.TabIndex = 1;
            // 
            // SliderUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_layout);
            this.Name = "SliderUpDown";
            this.Size = new System.Drawing.Size(245, 40);
            this.m_layout.ResumeLayout(false);
            this.m_layout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_trackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_updown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel m_layout;
        private System.Windows.Forms.TrackBar m_trackbar;
        private System.Windows.Forms.NumericUpDown m_updown;
    }
}
