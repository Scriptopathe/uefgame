using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
//using Microsoft.Build.Evaluation;
using Microsoft.Build.Construction;
namespace Geex.Edit
{
    static class Program
    {
        static ModulesManager s_pluginManager;
        static MainFormManager s_formManager;
        public static ModulesManager PluginManagerInstance
        {
            get { return s_pluginManager; }
        }
        /// <summary>
        /// The main entry point for the application.
        /// Important initialization calls will be made in that order :
        ///     - Project + DataBase loading
        ///     - MapView Initialization        Constructor Call
        ///     - FormHandler Initialization    Constructor Call
        ///     - MainForm Initialization       Constructor Call
        ///     - FormHandler.InitControls (needs the MainForm to be set up)
        ///     - FormHandler.InitEvents (needs the controls, MapView, MapViewControler etc... to be set up)
        ///             - why ? In order to initialize the toolbar buttons correctly for example, we need
        ///                 to have a look at the controler's state.
        ///     - MapView.SetupEvents (needs the controls of the form handler to be set up)
        /// </summary>
        [STAThread()]
        static void Main(string[] args)
        {
            // Initializes the lang module
            new Common.Lang();
            Common.Extensibility.AssemblyManager.Init();
            Common.Globals.Settings = Common.GeexSettings.Load();
            // Enables visual styles for the win forms
            System.Windows.Forms.Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
            System.Windows.Forms.Application.EnableVisualStyles();
            XnaControl.XnaFrameworkDispatcherService.Init();

            // MainForm initialization
            Common.Globals.MainForm = new Common.MainForm.MainFormClass();

            // ----- Starts loading stuff
            s_pluginManager = new ModulesManager();
            s_pluginManager.LoadModules();

            s_formManager = new MainFormManager();
            s_formManager.SetupControls();

            // Here we can load A plugin if needed


            // Run the app
            Application.Run(Common.Globals.MainForm);
            // When the application Exits 
            if (Common.Globals.MapView != null)
            {
                Common.Globals.MapView.Dispose();
            }
        }
    }
}

