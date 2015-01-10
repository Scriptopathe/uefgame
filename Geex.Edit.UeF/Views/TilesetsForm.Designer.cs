namespace Geex.Edit.UeF.Views
{
    partial class TilesetsForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.m_tilesetsListbox = new System.Windows.Forms.ListBox();
            this.m_picturePicker = new Geex.Edit.Common.Tools.Controls.PicturePicker();
            this._panel = new System.Windows.Forms.Panel();
            this.m_idGroupbox = new System.Windows.Forms.GroupBox();
            this.m_nameLabel = new System.Windows.Forms.Label();
            this.m_nameTextbox = new System.Windows.Forms.TextBox();
            this.m_addButton = new System.Windows.Forms.Button();
            this.m_deleteButton = new System.Windows.Forms.Button();
            this._panelOK = new System.Windows.Forms.Panel();
            this.m_okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this._panel.SuspendLayout();
            this.m_idGroupbox.SuspendLayout();
            this._panelOK.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_idGroupbox, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(576, 623);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.m_tilesetsListbox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_picturePicker, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this._panel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this._panelOK, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 54);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 566F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(570, 566);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // m_tilesetsListbox
            // 
            this.m_tilesetsListbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tilesetsListbox.FormattingEnabled = true;
            this.m_tilesetsListbox.Location = new System.Drawing.Point(3, 3);
            this.m_tilesetsListbox.Name = "m_tilesetsListbox";
            this.m_tilesetsListbox.Size = new System.Drawing.Size(164, 527);
            this.m_tilesetsListbox.TabIndex = 0;
            // 
            // m_picturePicker
            // 
            this.m_picturePicker.DisplaySrcRect = new System.Drawing.Rectangle(0, 0, -100, -100);
            this.m_picturePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_picturePicker.GP_Directory = null;
            this.m_picturePicker.GP_Filename = null;
            this.m_picturePicker.GP_GraphicsParserType = null;
            this.m_picturePicker.GP_Options = null;
            this.m_picturePicker.GP_ValidityPredicate = null;
            this.m_picturePicker.Location = new System.Drawing.Point(173, 3);
            this.m_picturePicker.Name = "m_picturePicker";
            this.m_picturePicker.ParentWindow = null;
            this.m_picturePicker.Size = new System.Drawing.Size(394, 527);
            this.m_picturePicker.TabIndex = 1;
            // 
            // _panel
            // 
            this._panel.Controls.Add(this.m_deleteButton);
            this._panel.Controls.Add(this.m_addButton);
            this._panel.Location = new System.Drawing.Point(3, 536);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(164, 27);
            this._panel.TabIndex = 2;
            // 
            // m_idGroupbox
            // 
            this.m_idGroupbox.Controls.Add(this.m_nameLabel);
            this.m_idGroupbox.Controls.Add(this.m_nameTextbox);
            this.m_idGroupbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_idGroupbox.Location = new System.Drawing.Point(3, 3);
            this.m_idGroupbox.Name = "m_idGroupbox";
            this.m_idGroupbox.Size = new System.Drawing.Size(570, 45);
            this.m_idGroupbox.TabIndex = 1;
            this.m_idGroupbox.TabStop = false;
            this.m_idGroupbox.Text = "Identification";
            // 
            // m_nameLabel
            // 
            this.m_nameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_nameLabel.AutoSize = true;
            this.m_nameLabel.Location = new System.Drawing.Point(9, 20);
            this.m_nameLabel.Name = "m_nameLabel";
            this.m_nameLabel.Size = new System.Drawing.Size(35, 13);
            this.m_nameLabel.TabIndex = 1;
            this.m_nameLabel.Text = "Nom :";
            // 
            // m_nameTextbox
            // 
            this.m_nameTextbox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_nameTextbox.Location = new System.Drawing.Point(50, 17);
            this.m_nameTextbox.Name = "m_nameTextbox";
            this.m_nameTextbox.Size = new System.Drawing.Size(117, 20);
            this.m_nameTextbox.TabIndex = 0;
            // 
            // m_addButton
            // 
            this.m_addButton.Location = new System.Drawing.Point(3, 3);
            this.m_addButton.Name = "m_addButton";
            this.m_addButton.Size = new System.Drawing.Size(75, 23);
            this.m_addButton.TabIndex = 0;
            this.m_addButton.Text = "Ajouter";
            this.m_addButton.UseVisualStyleBackColor = true;
            // 
            // m_deleteButton
            // 
            this.m_deleteButton.Location = new System.Drawing.Point(81, 3);
            this.m_deleteButton.Name = "m_deleteButton";
            this.m_deleteButton.Size = new System.Drawing.Size(75, 23);
            this.m_deleteButton.TabIndex = 1;
            this.m_deleteButton.Text = "Supprimer";
            this.m_deleteButton.UseVisualStyleBackColor = true;
            // 
            // _panelOK
            // 
            this._panelOK.Controls.Add(this.m_okButton);
            this._panelOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelOK.Location = new System.Drawing.Point(173, 536);
            this._panelOK.Name = "_panelOK";
            this._panelOK.Size = new System.Drawing.Size(394, 27);
            this._panelOK.TabIndex = 3;
            // 
            // m_okButton
            // 
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(316, 3);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(75, 23);
            this.m_okButton.TabIndex = 0;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            // 
            // TilesetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 623);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TilesetsForm";
            this.Text = "TilesetsForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this._panel.ResumeLayout(false);
            this.m_idGroupbox.ResumeLayout(false);
            this.m_idGroupbox.PerformLayout();
            this._panelOK.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox m_tilesetsListbox;
        private System.Windows.Forms.GroupBox m_idGroupbox;
        private System.Windows.Forms.Label m_nameLabel;
        private System.Windows.Forms.TextBox m_nameTextbox;
        private Common.Tools.Controls.PicturePicker m_picturePicker;
        private System.Windows.Forms.Panel _panel;
        private System.Windows.Forms.Button m_deleteButton;
        private System.Windows.Forms.Button m_addButton;
        private System.Windows.Forms.Panel _panelOK;
        private System.Windows.Forms.Button m_okButton;
    }
}