using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geex.Edit.Common.Tools;
using System.Xml;
namespace Geex.Edit.Common.Project
{
    /// <summary>
    /// Base class for all project types.
    /// </summary>
    [Serializable]
    public class GeexProject
    {
        const int MapFilenameIndexLengh = 4;
        /* --------------------------------------------------------------------------------
         * Static
         * ------------------------------------------------------------------------------*/
        #region Static
        /// <summary>
        /// Loads a project, given its filename.
        /// </summary>
        /// <param name="filename"></param>
        public static T Load<T>(string filename) where T : GeexProject, new()
        {
            T project = Serializer.Deserialize<T>(filename);
            project.CurrentProjectFilename = filename;
            return project;
        }
        /// <summary>
        /// Saves the project as a GeexProject instance, for Geex.Edit to be able
        /// to recognize the object.
        /// Does not save the database.
        /// </summary>
        public static void SaveProjectFile(GeexProject proj, string filename)
        {
            GeexProject temp = new GeexProject();
            temp.m_projectName = proj.m_projectName;
            temp.m_identifier.AssemblyName = proj.m_identifier.AssemblyName;
            temp.m_identifier.KindName = proj.m_identifier.KindName;
            temp.m_identifier.TypeFullName = proj.m_identifier.TypeFullName;
            temp.m_identifier.Version = proj.m_identifier.Version;
            temp.m_contentProjFilename = proj.m_contentProjFilename;
            temp.m_csProjFilename = proj.m_csProjFilename;
            temp.m_preferedModuleName = proj.m_preferedModuleName;
            Common.Tools.Serializer.Serialize<Common.Project.GeexProject>(temp, filename);
        }
        #endregion
        /* --------------------------------------------------------------------------------
         * Instance variables
         * ------------------------------------------------------------------------------*/
        #region Instance Variables (Serialized)
        /// <summary>
        /// The project's name.
        /// </summary>
        protected string m_projectName;
        /// <summary>
        /// The project's identifier
        /// </summary>
        protected ProjectIdentifier m_identifier = new Common.Project.ProjectIdentifier();
        /// <summary>
        /// The project's main cs project, which host the content project.
        /// </summary>
        protected string m_csProjFilename;
        /// <summary>
        /// The project's content project.
        /// </summary>
        protected string m_contentProjFilename;
        /// <summary>
        /// This is used to get the prefered plugin name when they are two or more plug-ins able
        /// to load this project.
        /// </summary>
        protected string m_preferedModuleName;
        #endregion
        /* --------------------------------------------------------------------------------
         * Non serialized instance variables.
         * ------------------------------------------------------------------------------*/
        #region Instance Variables (Non Serialized)
        /// <summary>
        /// Database instance.
        /// </summary>
        [NonSerialized]
        protected Database m_database;
        /// <summary>
        /// Current filename stored for convenience but not serialized.
        /// </summary>
        [NonSerialized]
        protected string m_currentProjectFilename;
        /// <summary>
        /// Content work of the project.
        /// </summary>
        [NonSerialized]
        protected ContentWork m_contentWork;
        /// <summary>
        /// Content work of the project.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public ContentWork ContentWork
        {
            get { return m_contentWork; }
            set { m_contentWork = value; }
        }
        #endregion
        /* --------------------------------------------------------------------------------
         * Methods
         * ------------------------------------------------------------------------------*/
        #region Instance Methods (4 virtual)
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public GeexProject()
        {

        }
        /// <summary>
        /// Saves the complete project.
        /// </summary>
        public virtual void Save()
        {
            Save(CurrentProjectFilename);
        }
        /// <summary>
        /// Saves the complete project.
        /// Override it in your subclass of GeexProject.
        /// The Project must save itself as a GeexProject instance, so it must
        /// be serialized using GeexProject.Save(proj, filename).
        /// </summary>
        public virtual void Save(string filename)
        {
            m_database.Save(this);
            GeexProject.SaveProjectFile(this, filename);
        }
        /// <summary>
        /// Gets a map object given its id.
        /// Override it in your subclass of GeexProject.
        /// </summary>
        /// <returns>An object</returns>
        public virtual object GetMapObject(int id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns the path to the map with the given index
        /// </summary>
        public virtual string MapFileName(int index)
        {
            return String.Format("{0}\\Map{1}.xml",
                MapDirectory,
                index.ToString().PadLeft(MapFilenameIndexLengh, '0'));
        }
        /// <summary>
        /// Make a full load of the project, given the information in the GeexProject project.
        /// Override it in your subclass of GeexProject.
        /// The database must be loaded here from the override.
        /// </summary>
        /// <param name="project"></param>
        public virtual void LoadFrom(GeexProject project)
        {
            this.m_currentProjectFilename = project.CurrentProjectFilename;
            this.m_projectName = project.ProjectName;
            this.m_identifier = project.Identifier;
            this.m_csProjFilename = project.m_csProjFilename;
            this.m_contentProjFilename = project.m_contentProjFilename;
            this.m_preferedModuleName = project.m_preferedModuleName;
            this.FindContentDirectory();
        }
        #endregion
        /* --------------------------------------------------------------------------------
         * Properties
         * ------------------------------------------------------------------------------*/
        #region Properties (1 virtual -paths-)
        /// <summary>
        /// Contains the path to the content project filename.
        /// Relative to the path of the geex project file.
        /// </summary>
        public virtual string ContentProjectFilename
        {
            get { return m_contentProjFilename; }
            set { m_contentProjFilename = value; }
        }
        /// <summary>
        /// Contains the path to the cs project filename.
        /// Relative to the path of the geex project file.
        /// </summary>
        public virtual string CsProjectFilename
        {
            get { return m_csProjFilename; }
            set { m_csProjFilename = value; }
        }
        /// <summary>
        /// Contains the path to the project directory. (NOT followed by \\)
        /// It is the folder containing the contentproj file.
        /// Every project must implement it.
        /// </summary>
        public virtual string ContentDirectory
        {
            get
            {
                string baseDir = System.IO.Path.GetDirectoryName(m_currentProjectFilename);
                string contentProjFilename = baseDir + "\\" + m_contentProjFilename;
                return System.IO.Path.GetDirectoryName(contentProjFilename);
            }
        }
        /// <summary>
        /// Finds the content directory and assigns m_contentProjFilename to it.
        /// </summary>
        void FindContentDirectory()
        {
            string baseDir = System.IO.Path.GetDirectoryName(m_currentProjectFilename);
            string contentProjFilename = baseDir + "\\" + m_contentProjFilename;
            // Gets the content proj filename !!
            Microsoft.Build.Construction.ProjectRootElement csProj;
            csProj = Microsoft.Build.Construction.ProjectRootElement.Open(baseDir + "\\" + m_csProjFilename);
            foreach (Microsoft.Build.Construction.ProjectItemGroupElement group in csProj.ItemGroups)
            {
                foreach (Microsoft.Build.Construction.ProjectItemElement item in group.Items)
                {
                    if (item.ItemType == "ProjectReference")
                        if (item.Include.Contains(".contentproj"))
                            contentProjFilename = System.IO.Path.GetDirectoryName(m_csProjFilename) + "\\" + item.Include;
                }
            }
            // Now we have the full content proj filename.
            // We want a relative path from the geexproj file.
            // contentProjFilename = "GameTest\GameTestContent\GameTestContent.contentproj"
            m_contentProjFilename = System.IO.Path.GetDirectoryName(contentProjFilename) + "\\" + System.IO.Path.GetFileName(contentProjFilename);
            m_contentProjFilename = FileHelper.GetCorrectPath(m_contentProjFilename);
        }
        /// <summary>
        /// Contains the path to the data directory (NOT followed by \\)
        /// Every project must implement it.
        /// </summary>
        public virtual string DataDirectory { get { return ContentDirectory+"\\Data"; } }
        /// <summary>
        /// Contains the path to the assemblies directory (NOT followed by \\)
        /// Every project must implement it.
        /// </summary>
        public virtual string AssembliesDirectory { get { return ContentDirectory + "\\Assemblies"; } }
        /// <summary>
        /// Contains the path to the opened project file name.
        /// </summary>
        public string CurrentProjectFilename
        {
            get { return m_currentProjectFilename; }
            set { m_currentProjectFilename = value; }
        }
        /// <summary>
        /// Gets the directory of the current geex project file.
        /// </summary>
        public string CurrentProjectDirectory
        {
            get { return System.IO.Path.GetDirectoryName(CurrentProjectFilename); }
        }
        /// <summary>
        /// Gets the identifier of the project.
        /// </summary>
        public ProjectIdentifier Identifier
        {
            get { return m_identifier; }
            set { m_identifier = value; }
        }
        /// <summary>
        /// Gets or sets the project's name
        /// </summary>
        public string ProjectName
        {
            get { return m_projectName; }
            set { m_projectName = value; }
        }
        /// <summary>
        /// Gets or sets the project's prefered module name.
        /// This is used to get the prefered plugin name when they are two or more plug-ins able
        /// to load this project.
        /// </summary>
        public string PreferedModuleName
        {
            get { return m_preferedModuleName; }
            set { m_preferedModuleName = value; }
        }
        #endregion

        #region Paths
        /// <summary>
        /// Contains the path to the map directory.
        /// It must be implemented in all project, because the MapTreeView is generic and will
        /// take the maps' directory from here.
        /// </summary>
        public virtual string MapDirectory { get { return ""; } }
        /// <summary>
        /// Contains the path to the map info file.
        /// </summary>
        public virtual string MapInfoPath { get { return ""; } }
        #endregion
    }
}