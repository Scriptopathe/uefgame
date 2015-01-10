using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geex.Edit.Common.MainForm
{
    /// <summary>
    /// The main form is the form what will be displayed at first.
    /// It's layout is managed by the MainWindow.Rpg.Handler class.
    /// </summary>
    public class MainFormClass : Form
    {
        public static MainFormClass Instance;

        #region Variables
        /// <summary>
        /// Main tool strip container of the form.
        /// </summary>
        public ToolStripContainer MainToolStripContainer;
        /// <summary>
        /// Main menu strip of the form.
        /// </summary>
        public MenuStrip MainMenu;
        /// <summary>
        /// File menu of the form
        /// </summary>
        public ToolStripMenuItem FileMenu;
        /// <summary>
        /// Status bar of the form.
        /// </summary>
        public StatusStrip StatusBar;
        /// <summary>
        /// Panel which contains all the stuff loaded by the project plugins.
        /// </summary>
        public Panel ProjectAreaPanel;
        #endregion

        /// <summary>
        /// Constructor if the main form.
        /// </summary>
        public MainFormClass()
        {
            this.Text = "Geex.Edit";
            // Singleton instance of this form
            Instance = this;
            // Initial size of the form
            this.Size = new System.Drawing.Size(800, 600);

            // Create the main components
            MainToolStripContainer = new ToolStripContainer();
            MainToolStripContainer.Dock = DockStyle.Top;
            MainToolStripContainer.Visible = true;
            MainToolStripContainer.Height = 24;

            MainMenu = new MenuStrip();
            MainMenu.Dock = DockStyle.Top;
            FileMenu = new ToolStripMenuItem(Lang.I["MainForm_Menu_File"]);
            MainMenu.Items.Add(FileMenu);

            ProjectAreaPanel = new Panel();
            ProjectAreaPanel.Dock = DockStyle.Fill;
            EnableGeexImage();

            // Adds the components to the controls of the forms
            Controls.Add(ProjectAreaPanel);
            Controls.Add(MainToolStripContainer);
            Controls.Add(MainMenu);
        }
        /// <summary>
        /// Sets up the Geex Image
        /// </summary>
        void EnableGeexImage()
        {
            ProjectAreaPanel.BackgroundImageLayout = ImageLayout.Center;
            ProjectAreaPanel.BackgroundImage = Common.Tools.Graphics.SystemBitmapCache.CachedBitmap(
                Common.AppRessources.RessourceDir() + "\\logo-geex.png");
        }
        /// <summary>
        /// Disables the Geex image drawing.
        /// </summary>
        void DisableGeexImage()
        {
            ProjectAreaPanel.BackgroundImage = null;
        }
        /// <summary>
        /// Prepares the form to load a plug-in.
        /// </summary>
        public void PrepareLoad()
        {
            DisableGeexImage();
        }
    }
}
