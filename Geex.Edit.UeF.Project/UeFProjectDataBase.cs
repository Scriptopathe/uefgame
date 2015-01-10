using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapInfo = Geex.Edit.Common.Project.MapInfo;
using UeFGame.GameComponents;
namespace Geex.Edit.UeF.Project
{
    /// <summary>
    /// Describes the project's data base.
    /// </summary>
    public class UeFProjectDataBase : Common.Project.Database
    {
        #region Variables
        /// <summary>
        /// List of the map infos.
        /// </summary>
        public List<MapInfo> MapInfos;
        public List<Tileset> Tilesets;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public UeFProjectDataBase(Common.Project.GeexProject proj)
            : base(proj)
        {

        }
        /// <summary>
        /// Saves the database to the current data folder.
        /// </summary>
        public override void Save(Common.Project.GeexProject proj)
        {
            base.Save(proj);
            UeFGeexProject project = (UeFGeexProject)proj;
            // Adds a default tileset in the list if it's empty.
            if(Tilesets.Count == 0)
                Tilesets.Add(new Tileset());
            
            Common.Tools.Serializer.Serialize<List<MapInfo>>(MapInfos, project.ContentDirectory + "\\RunTimeAssets\\Data\\MapInfos.xml");
            Common.Tools.Serializer.Serialize<List<Tileset>>(Tilesets, project.ContentDirectory + "\\RunTimeAssets\\Data\\Tilesets.ueftilesets");
        }
        /// <summary>
        /// Loads the database from the current data folder, defined by The GeexProject.DataDirectory path.
        /// </summary>
        public override void Load(Common.Project.GeexProject proj)
        {
            base.Load(proj);
            UeFGeexProject project = (UeFGeexProject)proj;
            MapInfos = Common.Tools.Serializer.Deserialize<List<MapInfo>>(project.ContentDirectory + "\\RunTimeAssets\\Data\\MapInfos.xml", true);
            Tilesets = Common.Tools.Serializer.Deserialize<List<Tileset>>(project.ContentDirectory + "\\RunTimeAssets\\Data\\Tilesets.ueftilesets", true);
        }
        /// <summary>
        /// Resets the database
        /// </summary>
        public override void Reset(Common.Project.GeexProject proj)
        {
             base.Reset(proj);
             if(Tilesets != null)    
                Tilesets.Clear();
        }
    }
    
}
