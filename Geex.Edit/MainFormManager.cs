using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Geex.Edit.Common;
using System.Reflection;
using GeexProject = Geex.Edit.Common.Project.GeexProject;
using MainForm = Geex.Edit.Common.MainForm.MainFormClass;
namespace Geex.Edit
{
    public class MainFormManager
    {
        #region Variables
        /// <summary>
        /// Menu item containing the recent files.
        /// It is update each time the recent files are modified.
        /// </summary>
        ToolStripMenuItem m_recentFilesMenuItem;
        #endregion

        #region Properties
        /// <summary>
        /// Shortcut to the mainform object
        /// </summary>
        MainForm MainForm
        {
            get { return Globals.MainForm; }
        }
        #endregion

        #region Setup
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainFormManager()
        {

        }
        /// <summary>
        /// Sets up the controls of the main frame.
        /// </summary>
        public void SetupControls()
        {
            MainForm.Padding = new Padding(5, 0, 5, 0);
            try
            {
                MainForm.Icon = new System.Drawing.Icon(Common.AppRessources.RessourceDir() + "\\" + "geex.ico");
            }
            catch { }
            SetupMenu();
            SetupStatusBar();
        }
        /// <summary>
        /// Sets up the status bar
        /// </summary>
        void SetupStatusBar()
        {
            MainForm form = MainForm;
            StatusStrip strip = new StatusStrip();
            strip.Dock = DockStyle.Bottom;
            form.Controls.Add(strip);
            MainForm.StatusBar = strip;
        }
        /// <summary>
        /// Sets up the menu.
        /// </summary>
        void SetupMenu()
        {
            MainForm form = MainForm;
            ToolStripMenuItem file = form.FileMenu;

            // Here add our components
            ToolStripMenuItem open = new ToolStripMenuItem(Lang.I["MainForm_MenuFile_Open"]);
            file.DropDownItems.Add(open);
            open.Click += new EventHandler(OnOpen);
            // Recent files
            ToolStripMenuItem recent = new ToolStripMenuItem(Lang.I["MainForm_MenuFile_RecentProject"]);
            file.DropDownItems.Add(recent);
            m_recentFilesMenuItem = recent;
            UpdateRecentProjectMenu();
            // DEBUG
            ToolStripMenuItem openGeex = new ToolStripMenuItem("[Geex.Rpg]--DEBUG");
            ToolStripMenuItem openUeF = new ToolStripMenuItem("[Geex.UeF]--DEBUG");
            file.DropDownItems.Add(openGeex);
            file.DropDownItems.Add(openUeF);
            openGeex.Click += new EventHandler(OnOpenGeex);
            openUeF.Click += new EventHandler(OnOpenUeF);
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when the user clicks on the "Open" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnOpen(object sender, EventArgs args)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Geex Project Files (*.geexproj)|*.geexproj";
            dlg.AutoUpgradeEnabled = true;
            dlg.CheckFileExists = true;
            if ((dlg.ShowDialog(Globals.MainForm) & DialogResult.OK) == DialogResult.OK)
            {
                Open(dlg.FileName);
            }
        }
        /// <summary>
        /// Updates the "Recent Project" menu.
        /// </summary>
        void UpdateRecentProjectMenu()
        {
            m_recentFilesMenuItem.DropDownItems.Clear();
            foreach (string filename in Globals.Settings.RecentProjects)
            {
                ToolStripMenuItem rctFile = new ToolStripMenuItem(System.IO.Path.GetFileName(filename));
                m_recentFilesMenuItem.DropDownItems.Add(rctFile);
                // We must use a local variable in order to get the work done.
                string tempFilename = filename;
                rctFile.Click += delegate(object o, EventArgs a)
                {
                    Open(string.Copy(tempFilename));
                };
            }
        }
        /// <summary>
        /// Opens a project with the given filename.
        /// </summary>
        /// <param name="filename"></param>
        void Open(string filename)
        {
            Program.PluginManagerInstance.LoadProject(filename);
            if (Common.Globals.Project != null)
            {
                MainForm.Text = "Geex.Edit - " + Common.Globals.Project.ProjectName + " [" +
                    Program.PluginManagerInstance.ActiveModule.GetModuleName() +
                    "-" + Program.PluginManagerInstance.ActiveModule.GetModuleVersion().ToString() + "]";
                Common.Globals.Settings.AddRecentProject(filename);
                UpdateRecentProjectMenu();
                Common.Globals.Settings.Save();
            }
        }
        #endregion

        #region TEST
        /// <summary>
        /// Called when the user clicks on "open"
        /// </summary>
        void OnOpenGeex(object sender, EventArgs arg)
        {
            string filename = "Project2\\lolz.geexproj";
            Open(filename);
        }
        /// <summary>
        /// Opens the uef project
        /// </summary>
        void OnOpenUeF(object sender, EventArgs args)
        {
            string filename = @"C:\Users\Scriptopathe\Documents\Josue\[Projets]\Projets de jeu\Usine en Folie\UeF.geexproj";
            Open(filename);
        }
        #endregion
    }
}
