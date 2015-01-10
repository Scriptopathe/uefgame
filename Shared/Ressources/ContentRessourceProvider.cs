using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.Ressources
{
    /// <summary>
    /// Permet le chargement de ressources compilées (xnb) par l'intermédiaire de Content.
    /// Fonctionne en contexte de jeu.
    /// </summary>
    public static class ContentRessourceProvider
    {
        #region Tilesets

        /// <summary>
        /// Domaine de validité : 
        /// </summary>
        /// <returns></returns>
        public static List<GameComponents.Tileset> LoadTilesets()
        {
            return Globals.Content.Load<List<GameComponents.Tileset>>("RunTimeAssets\\Data\\Tilesets");
        }

        public static GameComponents.Tileset LoadTileset(int tilesetId)
        {
            return GameDatabase.GetTileset(tilesetId);
        }

        public static Microsoft.Xna.Framework.Graphics.Texture2D LoadTilesetTexture(string textureName)
        {
            return Globals.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("RunTimeAssets\\Graphics\\Tilesets\\" + textureName);
        }

        public static GameComponents.MapInitializingData LoadMap(int mapId)
        {
            return Globals.Content.Load<GameComponents.MapInitializingData>("RunTimeAssets\\Data\\Maps\\Map" + mapId.ToString().PadLeft(4, '0'));
        }
        #endregion
        /* ----------------------------------------------------------------------------------------------------
         * Map
         * --------------------------------------------------------------------------------------------------*/
        #region Map


        #endregion
    }
}
