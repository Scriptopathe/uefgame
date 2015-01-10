using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Geex.Edit.Common.Extensibility
{
    /// <summary>
    /// Manages the loaded assemblies.
    /// All the assemblies needed by third-party plugins will be loaded
    /// here from the "Assemblies" folder of Geex.Edit.
    /// </summary>
    public static class AssemblyManager
    {
        /// <summary>
        /// Loaded assemblies.
        /// </summary>
        static Dictionary<string, Assembly> m_assemblies = new Dictionary<string,Assembly>();
        /// <summary>
        /// Gets an assembly given its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string name)
        {
            return m_assemblies[name];
        }
        /// <summary>
        /// Loads an assembly given its filename, and memorize it.
        /// The GetAssembly method will return the loaded assembly if the name argument
        /// is the name of the loaded assembly.
        /// </summary>
        /// <param name="name"></param>
        public static Assembly LoadAssembly(string filename)
        {
            Assembly a = Assembly.LoadFrom(filename);
            m_assemblies[a.GetName().Name] = a;
            return a;
        }
        /// <summary>
        /// Loads all the assemblies in the "Assemblies" folder.
        /// </summary>
        public static void Init()
        {
            string dir = AppRessources.BaseDir() + "Assemblies\\";
            IEnumerable<string> filenames;
            try
            {
                filenames = System.IO.Directory.EnumerateFiles(dir, "*.dll", System.IO.SearchOption.AllDirectories);
            }
            catch
            {
                return;
            }
            foreach(string filename in filenames)
            {
                try
                {
                    LoadAssembly(filename);
                }
                catch { }
            }
        }
    }
}
