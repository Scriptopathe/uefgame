using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Map = UeFGame.GameComponents.MapInitializingData;
using MapInfo = Geex.Edit.Common.Project.MapInfo;

namespace Geex.Edit.UeF.Project
{
    /// <summary>
    /// Handles project data such as paths for ressources...
    /// </summary>
    [Serializable]
    public class UeFGeexProject : Common.Project.GeexProject
    {
        #region Events / Delegates
        public delegate void ProjectSaveDelegate();
        public delegate void ProjectLoadDelegate();
        /// <summary>
        /// Event fired when the project is saved.
        /// Subclasses of GeexProject can fire it when their save work is completed.
        /// </summary>
        public event ProjectSaveDelegate OnProjectSaved;
        /// <summary>
        /// Event fired when the project is loaded.
        /// Subclasses of GeexProject can fire it when their load work is completed.
        /// </summary>
        public event ProjectLoadDelegate OnProjectLoaded;
        #endregion
        /* --------------------------------------------------------------------------------
         * Const / Readonly
         * ------------------------------------------------------------------------------*/
        #region Const / Readonly
        /// <summary>
        /// The Project Version
        /// </summary>
        static readonly Common.Version Version = new Common.Version(0, 0, 3, 0);
        /// <summary>
        /// The Project's kind
        /// </summary>
        const string Kind = "Geex.Edit.UeF";
        #endregion
        /* --------------------------------------------------------------------------------
         * Instance variables
         * ------------------------------------------------------------------------------*/
        #region Instance Variables
        [NonSerialized]
        Dictionary<MapInfo, Map> m_unsavedMaps;
        [NonSerialized]
        UeFGame.GameComponents.ModifiedMapList m_modifiedMapLists;
        #endregion
        /* --------------------------------------------------------------------------------
         * Properties
         * ------------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Contains the maps that have not been saved.
        /// </summary>
        Dictionary<MapInfo, Map> GetUnsavedMaps()
        {
            return m_unsavedMaps;
        }
        /// <summary>
        /// Gets the database of the project.
        /// </summary>
        public UeFProjectDataBase Database
        {
            get { return (UeFProjectDataBase)m_database; }
        }
        #endregion
        /* --------------------------------------------------------------------------------
         * Methods
         * ------------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public UeFGeexProject()
        {
            ProjectName = "NewProject";
            m_unsavedMaps = new Dictionary<MapInfo, Map>();
            m_identifier.KindName = "Geex-UeF";
            m_identifier.AssemblyName = typeof(UeFGeexProject).Assembly.GetName().Name;
            m_identifier.TypeFullName = typeof(UeFGeexProject).FullName;
            m_identifier.Version = new Common.Version(0, 0, 3, 0);
        }
        /* --------------------------------------------------------------------------------
         * Paths
         * ------------------------------------------------------------------------------*/
        #region Paths
        /// <summary>
        /// Run-Time Path to the Tileset
        /// </summary>
        public string RunTimeTilesetTexturesDirectory
        {
            get { return ContentDirectory + "\\RunTimeAssets\\Graphics\\Tilesets"; }
        }
        /// <summary>
        /// Design-Time path to the tileset.
        /// Without ending backslash.
        /// </summary>
        public string DesignTimeTilesetTexturesDirectory
        {
            get { return ContentDirectory + "\\DesignTimeAssets\\Graphics\\Tilesets"; }
        }

        public string MapDirectoryName
        {
            get { return ContentDirectory + "\\RunTimeAssets\\Data\\Maps"; }
        }
        /// <summary>
        /// Returns the path to the map directory.$
        /// (without the ending backslash)
        /// </summary>
        /// <returns></returns>
        public override string MapDirectory
        {
            get { return ContentDirectory + "\\RunTimeAssets\\Data\\Maps"; }
        }
        /// <summary>
        /// Returns the path to the map info file
        /// </summary>
        /// <returns></returns>
        public override string MapInfoPath
        {
            get { return ContentDirectory + "\\RunTimeAssets\\Data\\Mapinfos.xml"; }
        }
        /// <summary>
        /// Gets the full path to the game executable.
        /// </summary>
        public string GameExecutablePath
        {
            get { return CurrentProjectDirectory + "\\UsineEnFolie\\bin\\x86\\Debug\\UsineEnFolie.exe"; }
        }
        #endregion
        /* --------------------------------------------------------------------------------
         * Overrides
         * ------------------------------------------------------------------------------*/
        #region Overrides
        /// <summary>
        /// Gets a map object using its id. If it isn't loaded in memory, loads it from the disk.
        /// </summary>
        public override object GetMapObject(int id)
        {
            Map map = RetrieveUnsaved(id);
            if (map == null)
            {
                if (System.IO.File.Exists(MapFileName(id)))
                    try
                    {
                        return UeFMapWork.Load(MapFileName(id));
                    }
                    catch (System.IO.IOException)
                    {
                        return null;
                    }
                else
                    return null; // the map does not exist
            }
            return map;
        }
        /// <summary>
        /// Removes a map from the list of the unsaved maps.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void RemoveUnsavedMap(int id)
        {
            MapInfo key = null;
            foreach (KeyValuePair<MapInfo, Map> kvp in m_unsavedMaps)
            {
                if (id == kvp.Key.Id)
                    key = kvp.Key;
            }
            if (key != null)
            {
                m_unsavedMaps.Remove(key);
            }
        }
        /// <summary>
        /// Creates an instance of RpgGeexProject from an instance of GeexProject.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public override void LoadFrom(Common.Project.GeexProject project)
        {
            base.LoadFrom(project);
            // Database and unsaved maps
            this.m_database = new UeFProjectDataBase(this);
            this.m_unsavedMaps = new Dictionary<MapInfo, Map>();
            // Ressource provider
            UeFGame.Ressources.FileRessourceProvider.ContentDir = project.ContentDirectory + "\\";
            m_modifiedMapLists = UeFGame.GameComponents.ModifiedMapList.Load(ContentDirectory + "\\DesignTimeAssets\\Data\\ModifiedMaps.xml");
            this.Database.Load(this);
            if (OnProjectLoaded != null)
                OnProjectLoaded();
        }
        /// <summary>
        /// Saves the project data to the current project file name
        /// </summary>
        public override void Save()
        {
            Save(CurrentProjectFilename);
        }
        /// <summary>
        /// The saves the project data to the given path.
        /// Saves the DataBase too.
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            base.Save(filename); // saves the database
            ContentWork.Save();
            SaveMaps();
            m_modifiedMapLists.Save(ContentDirectory + "\\DesignTimeAssets\\Data\\ModifiedMaps.xml");
            if (OnProjectSaved != null)
                OnProjectSaved();
        }
        /// <summary>
        /// Returns the path to the map with the given index
        /// </summary>
        public override string MapFileName(int index)
        {
            return String.Format("{0}\\Map{1}.uefmap",
                MapDirectory,
                index.ToString().PadLeft(4, '0'));
        }
        #endregion
        /* --------------------------------------------------------------------------------
         * Save / Load
         * ------------------------------------------------------------------------------*/
        #region Save / Load
        /// <summary>
        /// Adds the map to the list of the unsaved maps.
        /// </summary>
        public void AddUnsavedMap(MapInfo mapInfo, Map map)
        {
            foreach (KeyValuePair<MapInfo, Map> tup in m_unsavedMaps)
            {
                if (tup.Key.Id == mapInfo.Id)
                {
                    if(!m_modifiedMapLists.GetMaps().Contains(mapInfo.Id))
                        m_modifiedMapLists.Add(mapInfo.Id);
                    return;
                }
            }
            m_unsavedMaps[mapInfo] = map;

            // Here adds to the modified map list
            if (!m_modifiedMapLists.GetMaps().Contains(mapInfo.Id))
                m_modifiedMapLists.Add(mapInfo.Id);
        }
        /// <summary>
        /// Prepares the project for the build.
        /// </summary>
        public void PreBuild()
        {
            m_modifiedMapLists.Save(ContentDirectory + "\\DesignTimeAssets\\Data\\ModifiedMaps.xml");
        }
        /// <summary>
        /// Builds the project.
        /// </summary>
        public void Build()
        {
            PreBuild();
            Save();
            string projFile = CurrentProjectDirectory + "\\UeF.Build\\UeF.Build.sln";
            string errors;
            Project.SolutionBuilder builder = new Project.SolutionBuilder();
            bool success = builder.Compile(projFile, CurrentProjectDirectory + "\\build.uef.log.txt", out errors);
            PostBuild(success);
        }
        /// <summary>
        /// Finished the build process.
        /// </summary>
        public void PostBuild(bool buildSuccess)
        {
            if (buildSuccess)
            {
                m_modifiedMapLists.Clear();
                m_modifiedMapLists.Save(ContentDirectory + "\\DesignTimeAssets\\Data\\ModifiedMaps.xml");
            }
        }
        /// <summary>
        /// Retrieves the unsaved map corresponding to the given map info.
        /// If it doesn't exists returns null.
        /// </summary>
        /// <param name="mapInfo"></param>
        /// <returns></returns>
        public Map RetrieveUnsaved(MapInfo mapInfo)
        {
            return RetrieveUnsaved(mapInfo.Id);
        }
        /// <summary>
        /// Retrieves the unsaved map corresponding to the given map id
        /// If it doesn't exists returns null.
        /// </summary>
        /// <param name="mapInfo"></param>
        /// <returns></returns>
        protected Map RetrieveUnsaved(int id)
        {
            foreach (KeyValuePair<MapInfo, Map> kvp in m_unsavedMaps)
            {
                if (id == kvp.Key.Id)
                    return kvp.Value;
            }
            return null;
        }

        /// <summary>
        /// Saves the unsaved maps.
        /// </summary>
        protected void SaveMaps()
        {
            foreach(KeyValuePair<MapInfo, Map> tup in m_unsavedMaps)
            {
                string filename = Common.Globals.Project.MapFileName(tup.Key.Id);
                UeFMapWork.Save(tup.Value, filename);
            }
            m_unsavedMaps.Clear();
        }

        /// <summary>
        /// Creates a new GeexProjectFile
        /// Doesn't load the new project
        /// </summary>
        public static UeFGeexProject CreateNew(string filename)
        {
            UeFGeexProject project = new UeFGeexProject();
            project.Save(filename);
            return project;
        }
        #endregion
        #endregion
    }
}
