namespace Geex.Edit.UeF.Views
{
    partial class GameObjectPropertiesView
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
            this.m_mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.m_propertiesTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.m_moduleStrip = new System.Windows.Forms.ToolStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_compileButton = new System.Windows.Forms.Button();
            this.m_codeContainer = new System.Windows.Forms.SplitContainer();
            this.m_consoleTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.m_warningTab = new System.Windows.Forms.TabPage();
            this.m_editControl = new Geex.Edit.UeF.Views.ObjectEditControl();
            this.m_codeEditor = new Geex.Edit.UeF.Controls.TextEditor();
            this.m_errorsListView = new Geex.Edit.UeF.Views.ErrorsListView();
            this.m_warningListView = new Geex.Edit.UeF.Views.ErrorsListView();
            this.m_mainLayout.SuspendLayout();
            this.m_propertiesTableLayout.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_codeContainer)).BeginInit();
            this.m_codeContainer.Panel1.SuspendLayout();
            this.m_codeContainer.Panel2.SuspendLayout();
            this.m_codeContainer.SuspendLayout();
            this.m_consoleTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.m_warningTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_mainLayout
            // 
            this.m_mainLayout.ColumnCount = 2;
            this.m_mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.m_mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_mainLayout.Controls.Add(this.m_propertiesTableLayout, 0, 0);
            this.m_mainLayout.Controls.Add(this.panel1, 1, 1);
            this.m_mainLayout.Controls.Add(this.m_codeContainer, 1, 0);
            this.m_mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_mainLayout.Location = new System.Drawing.Point(5, 5);
            this.m_mainLayout.Name = "m_mainLayout";
            this.m_mainLayout.RowCount = 2;
            this.m_mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.m_mainLayout.Size = new System.Drawing.Size(977, 647);
            this.m_mainLayout.TabIndex = 1;
            // 
            // m_propertiesTableLayout
            // 
            this.m_propertiesTableLayout.ColumnCount = 1;
            this.m_propertiesTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_propertiesTableLayout.Controls.Add(this.m_moduleStrip, 0, 0);
            this.m_propertiesTableLayout.Controls.Add(this.m_editControl, 0, 1);
            this.m_propertiesTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_propertiesTableLayout.Location = new System.Drawing.Point(3, 3);
            this.m_propertiesTableLayout.Name = "m_propertiesTableLayout";
            this.m_propertiesTableLayout.RowCount = 2;
            this.m_propertiesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.m_propertiesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_propertiesTableLayout.Size = new System.Drawing.Size(244, 610);
            this.m_propertiesTableLayout.TabIndex = 1;
            // 
            // m_moduleStrip
            // 
            this.m_moduleStrip.Location = new System.Drawing.Point(0, 0);
            this.m_moduleStrip.Name = "m_moduleStrip";
            this.m_moduleStrip.Size = new System.Drawing.Size(244, 25);
            this.m_moduleStrip.TabIndex = 1;
            this.m_moduleStrip.Text = "toolStrip1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_okButton);
            this.panel1.Controls.Add(this.m_compileButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(253, 619);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(721, 25);
            this.panel1.TabIndex = 4;
            // 
            // m_okButton
            // 
            this.m_okButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(634, 0);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(84, 23);
            this.m_okButton.TabIndex = 4;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            // 
            // m_compileButton
            // 
            this.m_compileButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.m_compileButton.Location = new System.Drawing.Point(3, 0);
            this.m_compileButton.Name = "m_compileButton";
            this.m_compileButton.Size = new System.Drawing.Size(84, 23);
            this.m_compileButton.TabIndex = 3;
            this.m_compileButton.Text = "Compiler";
            this.m_compileButton.UseVisualStyleBackColor = true;
            // 
            // m_codeContainer
            // 
            this.m_codeContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_codeContainer.Location = new System.Drawing.Point(253, 3);
            this.m_codeContainer.Name = "m_codeContainer";
            this.m_codeContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_codeContainer.Panel1
            // 
            this.m_codeContainer.Panel1.Controls.Add(this.m_codeEditor);
            this.m_codeContainer.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // m_codeContainer.Panel2
            // 
            this.m_codeContainer.Panel2.Controls.Add(this.m_consoleTabs);
            this.m_codeContainer.Size = new System.Drawing.Size(721, 610);
            this.m_codeContainer.SplitterDistance = 409;
            this.m_codeContainer.TabIndex = 5;
            // 
            // m_consoleTabs
            // 
            this.m_consoleTabs.Controls.Add(this.tabPage1);
            this.m_consoleTabs.Controls.Add(this.m_warningTab);
            this.m_consoleTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_consoleTabs.Location = new System.Drawing.Point(0, 0);
            this.m_consoleTabs.Name = "m_consoleTabs";
            this.m_consoleTabs.SelectedIndex = 0;
            this.m_consoleTabs.Size = new System.Drawing.Size(721, 197);
            this.m_consoleTabs.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.m_errorsListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(713, 171);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Erreurs";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // m_warningTab
            // 
            this.m_warningTab.Controls.Add(this.m_warningListView);
            this.m_warningTab.Location = new System.Drawing.Point(4, 22);
            this.m_warningTab.Name = "m_warningTab";
            this.m_warningTab.Padding = new System.Windows.Forms.Padding(3);
            this.m_warningTab.Size = new System.Drawing.Size(713, 171);
            this.m_warningTab.TabIndex = 1;
            this.m_warningTab.Text = "Warnings";
            this.m_warningTab.UseVisualStyleBackColor = true;
            // 
            // m_editControl
            // 
            this.m_editControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_editControl.Location = new System.Drawing.Point(3, 28);
            this.m_editControl.Name = "m_editControl";
            this.m_editControl.Size = new System.Drawing.Size(238, 579);
            this.m_editControl.TabIndex = 0;
            // 
            // m_codeEditor
            // 
            this.m_codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_codeEditor.IsReadOnly = false;
            this.m_codeEditor.Location = new System.Drawing.Point(0, 0);
            this.m_codeEditor.Name = "m_codeEditor";
            this.m_codeEditor.Size = new System.Drawing.Size(721, 409);
            this.m_codeEditor.TabIndex = 2;
            this.m_codeEditor.Text = "#uefinitialize {\r\n\r\n}";
            // 
            // m_errorsListView
            // 
            this.m_errorsListView.BackColor = System.Drawing.Color.MistyRose;
            this.m_errorsListView.Display = Geex.Edit.UeF.Views.ErrorDisplay.Errors;
            this.m_errorsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_errorsListView.Errors = null;
            this.m_errorsListView.Font = new System.Drawing.Font("Miramonte", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_errorsListView.FullRowSelect = true;
            this.m_errorsListView.GridLines = true;
            this.m_errorsListView.Location = new System.Drawing.Point(3, 3);
            this.m_errorsListView.Name = "m_errorsListView";
            this.m_errorsListView.Size = new System.Drawing.Size(707, 165);
            this.m_errorsListView.TabIndex = 0;
            this.m_errorsListView.UseCompatibleStateImageBehavior = false;
            this.m_errorsListView.View = System.Windows.Forms.View.Details;
            // 
            // m_warningListView
            // 
            this.m_warningListView.BackColor = System.Drawing.Color.LemonChiffon;
            this.m_warningListView.Display = Geex.Edit.UeF.Views.ErrorDisplay.Warning;
            this.m_warningListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_warningListView.Errors = null;
            this.m_warningListView.FullRowSelect = true;
            this.m_warningListView.GridLines = true;
            this.m_warningListView.Location = new System.Drawing.Point(3, 3);
            this.m_warningListView.Name = "m_warningListView";
            this.m_warningListView.Size = new System.Drawing.Size(707, 165);
            this.m_warningListView.TabIndex = 0;
            this.m_warningListView.UseCompatibleStateImageBehavior = false;
            this.m_warningListView.View = System.Windows.Forms.View.Details;
            // 
            // GameObjectPropertiesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 657);
            this.Controls.Add(this.m_mainLayout);
            this.Name = "GameObjectPropertiesView";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "GameObjectProperties";
            this.m_mainLayout.ResumeLayout(false);
            this.m_propertiesTableLayout.ResumeLayout(false);
            this.m_propertiesTableLayout.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.m_codeContainer.Panel1.ResumeLayout(false);
            this.m_codeContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_codeContainer)).EndInit();
            this.m_codeContainer.ResumeLayout(false);
            this.m_consoleTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.m_warningTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ObjectEditControl m_editControl;
        private System.Windows.Forms.TableLayoutPanel m_mainLayout;
        private System.Windows.Forms.TableLayoutPanel m_propertiesTableLayout;
        private Controls.TextEditor m_codeEditor;
        private System.Windows.Forms.ToolStrip m_moduleStrip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button m_compileButton;
        private System.Windows.Forms.SplitContainer m_codeContainer;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.TabControl m_consoleTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage m_warningTab;
        private ErrorsListView m_errorsListView;
        private ErrorsListView m_warningListView;

    }
}