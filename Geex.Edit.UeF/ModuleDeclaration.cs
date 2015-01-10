using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Geex.Edit.UeF
{
    /// <summary>
    /// Main entry point of the plug-in.
    /// </summary>
    public class ModuleDeclaration : Common.Extensibility.IModuleDeclaration
    {
        const string ModuleKind = "Geex-UeF";
        static readonly Common.Version Version = new Common.Version(0, 0, 0, 0);

        /// <summary>
        /// Constructor.
        /// </summary>
        public ModuleDeclaration()
        {

        }
        /// <summary>
        /// Starts the plugin : initializes its components.s
        /// </summary>
        public void InitModule()
        {
            m_disposed = false;
            // Loads first the constants for the UeF globals
            UeFGame.Globals.ExecuteInEditor = true;
            UeFGame.Globals.EditorContentRootDirectory = Common.Globals.Project.ContentDirectory;


            // MapView Creation
            Common.Globals.MapView = new UeF.MapView.MapView();
            UeFGlobals.Init();

            // Initializes the controls (which are for some of them device-dependent)
            // when the Content of the MapEditorControl will be loaded.
            Common.Globals.MapView.ContentLoaded += delegate
            {
                Common.Globals.MainFormUIManager.InitControls(Common.MainForm.MainFormClass.Instance.ProjectAreaPanel);
                Common.Globals.MainFormUIManager.InitEvents();
                UeFGame.Globals.EditorGraphicsDevice = Common.Globals.MapView.GraphicsDevice;
                UeFGlobals.Init();
            };


            // FormHandler creation
            Common.Globals.MainFormUIManager = new UeF.MainForm.UIHandler();
            UeFGlobals.Init();

            // Adds the map view
            ((Common.MainForm.MainFormClass)Common.Globals.MainForm).ProjectAreaPanel.Controls.Add((Control)Common.Globals.MapView);


            UeFGame.DrawingRoutines.InitTextures();
            
        }
        bool m_disposed;
        /// <summary>
        /// Disposes the plugin.
        /// </summary>
        public void DisposeModule()
        {
            Common.Globals.MapView.Dispose();
            Common.Globals.MapTreeView.Dispose();
            Common.Globals.MainFormUIManager.Dispose();
            
            Common.Globals.MapView = null;
            Common.Globals.MapTreeView = null;
            Common.Globals.MainFormUIManager = null;
            UeFGame.DrawingRoutines.DisposeTextures();
            m_disposed = true;
            /// Free all the plugins/assemblies loaded in the PluginProject :
        }
        /// <summary>
        /// Returns true if the plug-in is disposed.
        /// </summary>
        /// <returns></returns>
        public bool IsDisposed()
        {
            return m_disposed;
        }
        /// <summary>
        /// Gets the plugin name displayed to the user.
        /// </summary>
        /// <returns></returns>
        public string GetModuleName()
        {
            return ModuleKind;
        }
        /// <summary>
        /// Gets the plugin kind. See <see cref="Geex.Edit.Common.IModuleDeclaration.GetModuleKind()"/>
        /// </summary>
        /// <returns></returns>
        public string GetModuleKind()
        {
            return ModuleKind;
        }
        /// <summary>
        /// Gets the plugin version displayed to the user.
        /// </summary>
        /// <returns></returns>
        public Common.Version GetModuleVersion()
        {
            return Version;
        }
    }
}
