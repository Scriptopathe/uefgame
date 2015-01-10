using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Geex.Edit.Common.Project
{
    /// <summary>
    /// Represents a Database, common to all the project.
    /// Create a subclass of this Database object in your subclass of GeexProject.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Extensions of the database.
        /// </summary>
        Dictionary<string, DatabaseExtension> m_extensions;
        /// <summary>
        /// Public constructor.
        /// </summary>
        public Database(GeexProject proj)
        {
            LoadExtensions(proj);
            Reset(proj);
        }
        /// <summary>
        /// Returns a database extension given by its name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DatabaseExtension this[string id]
        {
            get { return m_extensions[id]; }
        }

        #region Virtual
        /// <summary>
        /// Saves the Data files of the database.
        /// </summary>
        /// <param name="proj">project reference to get the paths where to put the files</param>
        public virtual void Save(GeexProject proj)
        {
            Tools.ErrorReporter reporter = new Tools.ErrorReporter(Common.Globals.MainForm);
            foreach (DatabaseExtension ext in m_extensions.Values)
            {
#if !DEBUG
                try
                {
#endif
                    ext.Save(proj);
#if!DEBUG
                }
                catch (Exception e)
                {
                    reporter.Add(e.Message);
                }
#endif
            }
            reporter.ShowErrors();
        }
        /// <summary>
        /// Loads the Data files of the database.
        /// </summary>
        /// <param name="proj">project reference to get the paths where to search the files</param>
        public virtual void Load(GeexProject proj)
        {
            Tools.ErrorReporter reporter = new Tools.ErrorReporter(Common.Globals.MainForm);
            foreach (DatabaseExtension ext in m_extensions.Values)
            {
                try
                {
                    ext.Load(proj);
                }
                catch(Exception e)
                {
                    reporter.Add(e.Message);
                }
            }
            reporter.ShowErrors();
        }
        /// <summary>
        /// Resets the database
        /// </summary>
        public virtual void Reset(GeexProject proj)
        {
            LoadExtensions(proj);
        }
        /// <summary>
        /// Loads the extensions in memory. Does not load the data of the extensions.
        /// </summary>
        /// <param name="proj"></param>
        void LoadExtensions(GeexProject proj)
        {
            m_extensions = new Dictionary<string, DatabaseExtension>();
            string dirName = proj.DataDirectory + "\\DatabaseExtensions\\";
            try
            {
                foreach (string str in Directory.EnumerateFiles(dirName, "*.xml", SearchOption.TopDirectoryOnly))
                {
                    DatabaseExtension ext = new DatabaseExtension(str);
                    m_extensions.Add(ext.Name, ext);
                }
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
        }
        #endregion

    }
}
