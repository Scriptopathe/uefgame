namespace Geex.Edit.Common.Tools.Controls
{
    partial class GraphicsParser
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
            this.m_listbox = new System.Windows.Forms.ListView();
            this.Cancel = new System.Windows.Forms.Button();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_filesLabel = new System.Windows.Forms.Label();
            this.m_previewLabel = new System.Windows.Forms.Label();
            this.m_mainPanel = new System.Windows.Forms.Panel();
            this.m_parserTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.m_vTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.m_optionsEditionGrid = new DictionnaryEditControl();
            this.m_mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.m_buttonsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.m_viewer = new Geex.Edit.Common.Tools.Controls.AdvancedPictureBoxPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.m_mainPanel.SuspendLayout();
            this.m_parserTableLayout.SuspendLayout();
            this.m_vTableLayout.SuspendLayout();
            this.m_mainTableLayout.SuspendLayout();
            this.m_buttonsFlowLayout.SuspendLayout();
            this.m_viewer.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_listbox
            // 
            this.m_listbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_listbox.Location = new System.Drawing.Point(3, 28);
            this.m_listbox.Name = "m_listbox";
            this.m_listbox.Size = new System.Drawing.Size(174, 542);
            this.m_listbox.TabIndex = 0;
            this.m_listbox.UseCompatibleStateImageBehavior = false;
            this.m_listbox.View = System.Windows.Forms.View.List;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(761, 3);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // m_okButton
            // 
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(842, 3);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(75, 23);
            this.m_okButton.TabIndex = 3;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            // 
            // m_filesLabel
            // 
            this.m_filesLabel.AutoSize = true;
            this.m_filesLabel.Location = new System.Drawing.Point(3, 0);
            this.m_filesLabel.Name = "m_filesLabel";
            this.m_filesLabel.Size = new System.Drawing.Size(28, 13);
            this.m_filesLabel.TabIndex = 4;
            this.m_filesLabel.Text = "Files";
            // 
            // m_previewLabel
            // 
            this.m_previewLabel.AutoSize = true;
            this.m_previewLabel.Location = new System.Drawing.Point(183, 0);
            this.m_previewLabel.Name = "m_previewLabel";
            this.m_previewLabel.Size = new System.Drawing.Size(45, 13);
            this.m_previewLabel.TabIndex = 5;
            this.m_previewLabel.Text = "Preview";
            // 
            // m_mainPanel
            // 
            this.m_mainPanel.Controls.Add(this.m_parserTableLayout);
            this.m_mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_mainPanel.Location = new System.Drawing.Point(3, 3);
            this.m_mainPanel.Name = "m_mainPanel";
            this.m_mainPanel.Size = new System.Drawing.Size(690, 573);
            this.m_mainPanel.TabIndex = 6;
            // 
            // m_parserTableLayout
            // 
            this.m_parserTableLayout.ColumnCount = 2;
            this.m_parserTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.m_parserTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_parserTableLayout.Controls.Add(this.m_filesLabel, 0, 0);
            this.m_parserTableLayout.Controls.Add(this.m_viewer, 1, 1);
            this.m_parserTableLayout.Controls.Add(this.m_listbox, 0, 1);
            this.m_parserTableLayout.Controls.Add(this.m_previewLabel, 1, 0);
            this.m_parserTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_parserTableLayout.Location = new System.Drawing.Point(0, 0);
            this.m_parserTableLayout.Name = "m_parserTableLayout";
            this.m_parserTableLayout.RowCount = 2;
            this.m_parserTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.m_parserTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_parserTableLayout.Size = new System.Drawing.Size(690, 573);
            this.m_parserTableLayout.TabIndex = 4;
            // 
            // m_vTableLayout
            // 
            this.m_vTableLayout.ColumnCount = 2;
            this.m_vTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 696F));
            this.m_vTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_vTableLayout.Controls.Add(this.m_optionsEditionGrid, 1, 0);
            this.m_vTableLayout.Controls.Add(this.m_mainPanel, 0, 0);
            this.m_vTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_vTableLayout.Location = new System.Drawing.Point(3, 3);
            this.m_vTableLayout.Name = "m_vTableLayout";
            this.m_vTableLayout.RowCount = 1;
            this.m_vTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_vTableLayout.Size = new System.Drawing.Size(920, 579);
            this.m_vTableLayout.TabIndex = 7;
            // 
            // m_optionsEditionGrid
            // 
            this.m_optionsEditionGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_optionsEditionGrid.Location = new System.Drawing.Point(699, 3);
            this.m_optionsEditionGrid.Name = "m_optionsEditionGrid";
            this.m_optionsEditionGrid.Size = new System.Drawing.Size(218, 573);
            this.m_optionsEditionGrid.TabIndex = 7;
            // 
            // m_mainTableLayout
            // 
            this.m_mainTableLayout.ColumnCount = 1;
            this.m_mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_mainTableLayout.Controls.Add(this.m_vTableLayout, 0, 0);
            this.m_mainTableLayout.Controls.Add(this.m_buttonsFlowLayout, 0, 1);
            this.m_mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_mainTableLayout.Location = new System.Drawing.Point(5, 5);
            this.m_mainTableLayout.Name = "m_mainTableLayout";
            this.m_mainTableLayout.RowCount = 2;
            this.m_mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.m_mainTableLayout.Size = new System.Drawing.Size(926, 620);
            this.m_mainTableLayout.TabIndex = 8;
            // 
            // m_buttonsFlowLayout
            // 
            this.m_buttonsFlowLayout.Controls.Add(this.m_okButton);
            this.m_buttonsFlowLayout.Controls.Add(this.Cancel);
            this.m_buttonsFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_buttonsFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.m_buttonsFlowLayout.Location = new System.Drawing.Point(3, 588);
            this.m_buttonsFlowLayout.Name = "m_buttonsFlowLayout";
            this.m_buttonsFlowLayout.Size = new System.Drawing.Size(920, 29);
            this.m_buttonsFlowLayout.TabIndex = 8;
            // 
            // m_viewer
            // 
            this.m_viewer.Controls.Add(this.button1);
            this.m_viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_viewer.Image = null;
            this.m_viewer.Location = new System.Drawing.Point(183, 28);
            this.m_viewer.Name = "m_viewer";
            this.m_viewer.Saturation = 255;
            this.m_viewer.ShowSaturation = true;
            this.m_viewer.Size = new System.Drawing.Size(504, 542);
            this.m_viewer.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(304, 224);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // GraphicsParser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 630);
            this.Controls.Add(this.m_mainTableLayout);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphicsParser";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "GraphicsParser";
            this.m_mainPanel.ResumeLayout(false);
            this.m_parserTableLayout.ResumeLayout(false);
            this.m_parserTableLayout.PerformLayout();
            this.m_vTableLayout.ResumeLayout(false);
            this.m_mainTableLayout.ResumeLayout(false);
            this.m_buttonsFlowLayout.ResumeLayout(false);
            this.m_viewer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView m_listbox;
        private Common.Tools.Controls.AdvancedPictureBoxPanel m_viewer;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label m_filesLabel;
        private System.Windows.Forms.Label m_previewLabel;
        private System.Windows.Forms.Panel m_mainPanel;
        private System.Windows.Forms.TableLayoutPanel m_vTableLayout;
        private System.Windows.Forms.TableLayoutPanel m_mainTableLayout;
        private System.Windows.Forms.FlowLayoutPanel m_buttonsFlowLayout;
        private DictionnaryEditControl m_optionsEditionGrid;
        private System.Windows.Forms.TableLayoutPanel m_parserTableLayout;
    }
}