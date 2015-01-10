using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UeFGame.GameComponents;
namespace MapImporter
{
    /// <summary>
    /// Permet le chargement de ressources par le système de fichiers.
    /// Ces ressources seront surtout utilisées par l'éditeur.
    /// </summary>
    public static class FileRessourceProvider
    {
        public static string ContentDir = "..\\..\\..\\UsineEnFolieContent\\";

        /* ----------------------------------------------------------------------------------------------------
         * Tilesets
         * --------------------------------------------------------------------------------------------------*/
        #region Tilesets
        public static List<string> LoadModifiedMapList()
        {
            return UeFGame.Tools.Serializer.Deserialize<List<string>>(ContentDir + "DesignTimeAssets\\Data\\ModifiedMaps.xml", true);
        }
        public static List<Geex.Edit.Common.Project.MapInfo> LoadMapInfo()
        {
            return UeFGame.Tools.Serializer.Deserialize<List<Geex.Edit.Common.Project.MapInfo>>(ContentDir + "RunTimeAssets\\Data\\MapInfos.xml", true);
        }
        public static List<Tileset> LoadTilesets()
        {
            return UeFGame.Tools.Serializer.Deserialize<List<Tileset>>(ContentDir + "RunTimeAssets\\Data\\Tilesets.ueftilesets", true);
        }
        public static string SaveTilesets(List<Tileset> tilesets)
        {
            string filename = ContentDir + "RunTimeAssets\\Data\\Tilesets.ueftilesets";
            UeFGame.Tools.Serializer.Serialize<List<Tileset>>(tilesets, filename);
            return filename;
        }
        public static Tileset LoadTileset(int id)
        {
            return GameDatabase.GetTileset(id);
        }
        public static TilesetMask LoadTilesetMask(int tilesetId)
        {
            return UeFGame.Tools.Serializer.Deserialize<TilesetMask>(ContentDir + "RunTimeAssets\\Data\\TilesetMasks\\Tileset" + tilesetId.ToString().PadLeft(4, '0') + ".xml");
        }
        public static void SaveTilesetMask(int tilesetId, TilesetMask mask)
        {
            UeFGame.Tools.Serializer.Serialize<TilesetMask>(mask, ContentDir + "RunTimeAssets\\Data\\TilesetMasks\\Tileset" + tilesetId.ToString().PadLeft(4, '0') + ".xml");
        }

        public static System.Drawing.Bitmap LoadTilesetTexture(string textureName)
        {
            return new System.Drawing.Bitmap(ContentDir + "RunTimeAssets\\Graphics\\Tilesets\\" + textureName + ".png");
        }
        #endregion
        /* ----------------------------------------------------------------------------------------------------
         * Map
         * --------------------------------------------------------------------------------------------------*/
        #region Map
        public static MapInitializingData LoadMap(int mapId)
        {
            return UeFGame.Tools.Serializer.Deserialize<MapInitializingData>(ContentDir + "RunTimeAssets\\Data\\Maps\\Map" + mapId.ToString().PadLeft(4, '0') + ".uefmap");
        }
        public static string SaveMapInfos(List<Geex.Edit.Common.Project.MapInfo> mapInfos)
        {
            string filename = ContentDir + "RunTimeAssets\\Data\\MapInfos.xml";
            UeFGame.Tools.Serializer.Serialize<List<Geex.Edit.Common.Project.MapInfo>>(mapInfos, filename);
            return filename;
        }
        public static string SaveMap(int mapId, MapInitializingData map)
        {
            string filename = ContentDir + "RunTimeAssets\\Data\\Maps\\Map" + mapId.ToString().PadLeft(4, '0') + ".uefmap";
            UeFGame.Tools.Serializer.Serialize<MapInitializingData>(map, filename);
            return filename;
        }
        #endregion
    }

}
