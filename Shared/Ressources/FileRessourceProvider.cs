using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.Ressources
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
        /// <summary>
        /// Charge les tilesets depuis le disque.
        /// </summary>
        /// <returns></returns>
        public static List<GameComponents.Tileset> LoadTilesets()
        {
            return Tools.Serializer.Deserialize<List<GameComponents.Tileset>>(ContentDir + "RunTimeAssets\\Data\\Tilesets.ueftilesets", true);
        }

        /// <summary>
        /// Sauvegarde les tilesets.
        /// </summary>
        /// <param name="tilesets"></param>
        /// <returns></returns>
        public static string SaveTilesets(List<GameComponents.Tileset> tilesets)
        {
            string filename = ContentDir + "RunTimeAssets\\Data\\Tilesets.ueftilesets";
            Tools.Serializer.Serialize<List<GameComponents.Tileset>>(tilesets, filename);
            return filename;
        }
        /// <summary>
        /// Retourne le tileset de la GameDatabase avec l'id donné si l'appel est fait depuis le jeu,
        /// s'il est lancé depuis l'éditeur, lance une exception.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GameComponents.Tileset LoadTileset(int id)
        {
            if (Globals.ExecuteInEditor)
                throw new Exception();
            return GameDatabase.GetTileset(id);
        }

        public static GameComponents.TilesetMask LoadTilesetMask(int tilesetId)
        {
            return Tools.Serializer.Deserialize<GameComponents.TilesetMask>(ContentDir + "RunTimeAssets\\Data\\TilesetMasks\\Tileset" + tilesetId.ToString().PadLeft(4, '0') + ".xml");
        }

        public static void SaveTilesetMask(int tilesetId, GameComponents.TilesetMask mask)
        {
            Tools.Serializer.Serialize<GameComponents.TilesetMask>(mask, ContentDir + "RunTimeAssets\\Data\\TilesetMasks\\Tileset" + tilesetId.ToString().PadLeft(4, '0') + ".xml");
        }

        public static string GetTilesetTextureFullPath(string textureName)
        {
            return ContentDir + "RunTimeAssets\\Graphics\\Tilesets\\" + textureName + ".png";
        }

        public static System.Drawing.Bitmap LoadTilesetTexture(string textureName)
        {
            return new System.Drawing.Bitmap(GetTilesetTextureFullPath(textureName));
        }
        #endregion
        /* ----------------------------------------------------------------------------------------------------
         * Map
         * --------------------------------------------------------------------------------------------------*/
        #region Map
        public static List<Geex.Edit.Common.Project.MapInfo> LoadMapInfos()
        {
            return Tools.Serializer.Deserialize<List<Geex.Edit.Common.Project.MapInfo>>(ContentDir + "RunTimeAssets\\Data\\MapInfos.xml");
        }
        public static GameComponents.MapInitializingData LoadMap(int mapId)
        {
            return Tools.Serializer.Deserialize<GameComponents.MapInitializingData>(ContentDir + "RunTimeAssets\\Data\\Maps\\Map" + mapId.ToString().PadLeft(4, '0') + ".uefmap");
        }
        public static string SaveMapInfos(List<Geex.Edit.Common.Project.MapInfo> mapInfos)
        {
            string filename = ContentDir + "RunTimeAssets\\Data\\MapInfos.xml";
            Tools.Serializer.Serialize<List<Geex.Edit.Common.Project.MapInfo>>(mapInfos, filename);
            return filename;
        }
        public static string SaveMap(int mapId, GameComponents.MapInitializingData map)
        {
            string filename = ContentDir + "RunTimeAssets\\Data\\Maps\\Map" + mapId.ToString().PadLeft(4, '0') + ".uefmap";
            Tools.Serializer.Serialize<GameComponents.MapInitializingData>(map, filename);
            return filename;
        }
        #endregion
    }

}
