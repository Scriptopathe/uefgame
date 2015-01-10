using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Geex.Edit.Common;
using Geex.Edit.Common.Extensibility;
using System.Windows.Forms;
using GeexProject = Geex.Edit.Common.Project.GeexProject;
using MainForm = Geex.Edit.Common.MainForm.MainFormClass;
namespace Geex.Edit
{
    /// <summary>
    /// Manages the plugin.
    /// </summary>
    public class ModulesManager
    {
        /// <summary>
        /// The directory of the plugins
        /// </summary>
        const string ModulesDirectory = "Modules";
        const string ModuleClassName = ".ModuleDeclaration";
        /* ---------------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Dictionnary containing the Plugins indexed by their Name.
        /// </summary>
        Dictionary<string, IModuleDeclaration> m_modules;
        /// <summary>
        /// Current plugin loaded and initialized.
        /// </summary>
        IModuleDeclaration m_activeModule;
        #endregion
        /* ---------------------------------------------------------------------
         * Properties
         * --------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets the current active ProjectPlugin
        /// </summary>
        public IModuleDeclaration ActiveModule
        {
            get { return m_activeModule; }
        }
        #endregion
        /* ---------------------------------------------------------------------
         * Methods
         * --------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public ModulesManager()
        {
            
        }
        /// <summary>
        /// Loads all the project plugins in memory
        /// </summary>
        public void LoadModules()
        {
            m_modules = new Dictionary<string, IModuleDeclaration>();
            string dir = Common.AppRessources.BaseDir() + ModulesDirectory;
            foreach (string filename in Directory.EnumerateFiles(dir, "*.dll", SearchOption.TopDirectoryOnly))
            {
                if (!filename.Contains("Common"))
                {
                    IModuleDeclaration plug = LoadModule(filename);
                    if (plug != null)
                    {
                        m_modules.Add(plug.GetModuleName(), plug);
                    }
                }
            }
        }
        /// <summary>
        /// Loads and returns a plugin.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        IModuleDeclaration LoadModule(string filename)
        {
            try
            {
                Assembly a = AssemblyManager.LoadAssembly(filename);
                // The name of the type must be the name of the assembly + the class name "ModuleDeclaration".
                string typename = a.GetName().Name + ModuleClassName;
                Type type = a.GetType(typename);
                IModuleDeclaration plug = (IModuleDeclaration)Activator.CreateInstance(type);
                return plug;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Initializes a project plugin.
        /// </summary>
        /// <param name="index"></param>
        public void InitModule(string index)
        {
            InitModule(m_modules[index]);
        }

        /// <summary>
        /// Initializes the given project plugin.
        /// </summary>
        /// <param name="plugin"></param>
        public void InitModule(IModuleDeclaration plugin)
        {
            if (m_activeModule != null)
            {
                m_activeModule.DisposeModule();
                m_activeModule = null;
            }
            
            m_activeModule = plugin;
            MainForm form = (MainForm)Common.Globals.MainForm;
            form.PrepareLoad();
            form.SuspendLayout();
            m_activeModule.InitModule();
            form.ResumeLayout();
        }
        /// <summary>
        /// Makes a full load of a project, including the project files,
        /// plugin initialization, etc...
        /// </summary>
        /// <param name="filename"></param>
        public void LoadProject(string filename)
        {
            // First loads the project file.
            GeexProject project;
            try
            {
                project = GeexProject.Load<GeexProject>(filename);
            }
#if !DEBUG
            catch
            {

                MessageBox.Show(MainForm.Instance, 
                    Lang.I["ProjectLoad_FileLoadFail_Text"].Replace("#", filename),
                    Lang.I["ProjectLoad_FileLoadFail_Caption"]);
                return;
            }
#endif
            catch (Exception e)
            {
                throw e;
            }
            // Project loaded, now load the right project type.
            Assembly a;
            Type projType;
            List<IModuleDeclaration> plugs; // Plugins which could be used
            try
            {
                a = AssemblyManager.GetAssembly(project.Identifier.AssemblyName);
                projType = a.GetType(project.Identifier.TypeFullName);
                plugs = FindCorrespondingPlugin(project);
                if (plugs.Count == 0)
                    throw new Exception();
            }
            catch
            {
                string projTypeStr = project.Identifier.KindName;
                // Tells the user the project type is invalid.
                MessageBox.Show(MainForm.Instance,
                    Lang.I["ProjectLoad_UnsupportedProjectType_Text"].Replace("#", projTypeStr),
                    Lang.I["ProjectLoad_UnsupportedProjectType_Caption"]);
                return;
            }
            // Create an instance of the right project type
            GeexProject newProj = (GeexProject)Activator.CreateInstance(projType);

            // Loads the information of the right project type from the loaded project.
            newProj.LoadFrom(project);

            // Choose the good plugin
            IModuleDeclaration choosenModule = null;
            if (plugs.Count > 1)
            {
                foreach (IModuleDeclaration module in plugs)
                {
                    if (module.GetModuleName() == project.PreferedModuleName)
                    {
                        choosenModule = module;
                    }
                }
                if (choosenModule == null)
                {
                    // Prompts the user to choose the plugin he wants to load.
                    ModulesChoiceForm form = new ModulesChoiceForm(plugs);
                    DialogResult result = form.ShowDialog(Common.Globals.MainForm);
                    if (result == DialogResult.OK)
                    {
                        choosenModule = form.ChoosenModule;
                        if (form.Remember)
                        {
                            newProj.PreferedModuleName = choosenModule.GetModuleName();
                            newProj.Save();
                        }
                    }
                    else
                    {
                        // Maybe change this comportement
                        return;
                    }
                }
            }
            else
            {
                choosenModule = plugs.First();
            }

            // Initializes the corresponding plugin.
            Common.Globals.Project = newProj;
            InitModule(choosenModule);
        }
        /// <summary>
        /// Finds a list of corresponding plugin for this project kind.
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        List<IModuleDeclaration> FindCorrespondingPlugin(GeexProject proj)
        {
            List<IModuleDeclaration> plugins = new List<IModuleDeclaration>();
            foreach (IModuleDeclaration plugin in m_modules.Values)
            {
                if (plugin.GetModuleKind() == proj.Identifier.KindName &&
                    plugin.GetModuleVersion().CompatibilityNumber == proj.Identifier.Version.CompatibilityNumber)
                {
                    plugins.Add(plugin);
                }
            }
            return plugins;
        }
        #endregion
    }
}