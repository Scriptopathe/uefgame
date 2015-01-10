using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapInit = UeFGame.GameComponents.MapInitializingData;
using CU = UeFGame.ConvertUnits;
using UeFGame.GameObjects;
using GameObject = UeFGame.GameObjects.GameObject;
using System.Reflection;
using MapInfo = Geex.Edit.Common.Project.MapInfo;
using UeFGame.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Geex.Edit.UeF.Project
{
    public delegate void MapLoadedDelegate(MapInit map);
    /// <summary>
    /// Wrapper for easy access to the data of the current map.
    /// </summary>
    public class MapDataWrapper
    {
        /* ------------------------------------------------------------------------------------------
         * EVENTS
         * ----------------------------------------------------------------------------------------*/
        #region Events
        /// <summary>
        /// Event raised when a new map is loaded.
        /// This event should in practice tell the RenderManager to re-load its data.
        /// </summary>
        public event MapLoadedDelegate MapLoaded;
        #endregion

        #region Variables
        /// <summary>
        /// The map stored in the wrapper.
        /// </summary>
        MapInit m_map;
        /// <summary>
        /// Map info stored in the wrapper.
        /// </summary>
        MapInfo m_mapInfo;
        /// <summary>
        /// Game objects loaded in the map.
        /// They won't be modified directly but their initializing data will.
        /// </summary>
        
        List<UeFGame.GameObjects.GameObject> m_gameObjects;
        /// <summary>
        /// Tileset ref.
        /// </summary>
        Tileset m_tileset;
        /// <summary>
        /// Tileset texture
        /// </summary>
        Texture2D m_tilesetTexture;
        #endregion

        #region Properties
        /// <summary>
        /// Reference to the current tileset texture.
        /// </summary>
        public Texture2D TilesetTexture
        {
            get { return m_tilesetTexture; }
            set { m_tilesetTexture = value; }
        }
        /// <summary>
        /// Reference to the current tileset object.
        /// </summary>
        public Tileset Tileset
        {
            get 
            {
                return m_tileset;
            
            }
            set { m_tileset = value; }
        }
        /// <summary>
        /// Gets the current editing map.
        /// </summary>
        public MapInit Map
        {
            get { return m_map; }
        }
        /// <summary>
        /// Gets the current map info of the current editing map.
        /// </summary>
        public MapInfo MapInfo
        {
            get { return m_mapInfo; }
        }
        /// <summary>
        /// Gets the current map width in pixels
        /// </summary>
        public int MapWidth
        {
            get { return (int)(CU.ToDisplayUnits(m_map.SimSize.X)); }
        }
        /// <summary>
        /// Gets the current map height in pixels.
        /// </summary>
        public int MapHeight
        {
            get { return (int)(CU.ToDisplayUnits(m_map.SimSize.Y)); }
        }
        /// <summary>
        /// Gets the game objects loaded in the map.
        /// </summary>
        public List<UeFGame.GameObjects.GameObject> GameObjects
        {
            get { return m_gameObjects; }
            protected set { m_gameObjects = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new MapDataWrapper instance.
        /// </summary>
        public MapDataWrapper()
        {

        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Loads the data from a map given it's id.
        /// </summary>
        /// <param name="mapId">id of the map to load</param>
        public void LoadMap(MapInfo info)
        {
            int mapId = info.Id;
            if (mapId == -1)
                return;
            MapInit map;
            try
            {
                map = Project.UeFMapWork.Load(Common.Globals.Project.MapFileName(mapId));
            }
            catch(System.InvalidOperationException)
            {
                System.Windows.Forms.MessageBox.Show("Fichier corrompu.");
                return;
            }
            catch (System.IO.IOException)
            {
                return;
            }
            LoadMap(map, info);
        }
        /// <summary>
        /// Loads the data from a map
        /// </summary>
        /// <param name="map"></param>
        public void LoadMap(MapInit map, MapInfo info)
        {
            // If we are loading the same map, return :
            if (this.m_map == map)
                return;
            this.m_mapInfo = info;
            // First dispose the old map's objects instances.
            if (this.m_map != null)
                DisposeMapObjectsInstances();
            // Then registers the new map
            this.m_map = map;
            // And load the new map's objects instances.
            LoadGameObjectsInstances();
            // Load tileset
            m_tileset = ((UeFGeexProject)Common.Globals.Project).Database.Tilesets[map.TilesetId];
            m_tilesetTexture = TilesetWork.GetTexture(m_tileset);
            // Fires the map loaded event.
            if (MapLoaded != null)
                MapLoaded(map);
        }
        /// <summary>
        /// Refreshes the tileset's texture.
        /// </summary>
        public void RefreshTilesetTexture()
        {
            // Load tileset
            m_tileset = ((UeFGeexProject)Common.Globals.Project).Database.Tilesets[m_map.TilesetId];
            m_tilesetTexture = TilesetWork.GetTexture(m_tileset);
        }
        /// <summary>
        /// Deletes an object.
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteObject(GameObject obj)
        {
            obj.Dispose();
            m_map.GameObjects.Remove(obj.InitializingData);
            m_gameObjects.Remove(obj);
        }
        /// <summary>
        /// Adds a game object.
        /// </summary>
        /// <param name="obj"></param>
        public void AddGameObject(GameObject obj)
        {
            m_map.GameObjects.Add(obj.InitializingData);
            m_gameObjects.Add(obj);
        }

        #endregion

        #region Private
        /// <summary>
        /// Dispose the map object's instances.
        /// </summary>
        void DisposeMapObjectsInstances()
        {
            if (m_gameObjects != null)
            {
                foreach (GameObject obj in m_gameObjects)
                {
                    obj.Dispose();
                }
            }
            m_gameObjects = null;
        }
        /// <summary>
        /// Dispose.
        /// </summary>
        void Dispose()
        {
            DisposeMapObjectsInstances();
            m_tilesetTexture.Dispose();
        }
        /// <summary>
        /// Loads the game object's instances.
        /// </summary>
        void LoadGameObjectsInstances()
        {
            if (m_gameObjects != null)
                throw new Exception("The game objects must have been disposed before loading GameObjects instancee of a new map");
            m_gameObjects = new List<GameObject>();
            foreach (UeFGame.GameObjects.GameObjectInit dat in m_map.GameObjects)
            {
                if (dat.Type == null || dat.Type == "")
                    continue;
                Assembly assembly = Assembly.GetAssembly(typeof(GameObject));
                Type objType = assembly.GetType(dat.Type);

                // Ignore errors.
                if (objType == null)
                    continue;

                // Creates a game object instance from the type given in the init data.
                GameObject obj = (GameObject)Activator.CreateInstance(objType, new object[] { });

                // Initializes the object
                obj.InitializingData = dat;
                obj.Initialize();

                // Registers the object
                m_gameObjects.Add(obj);
            }
        }
        
        #endregion
        #endregion
    }
}
