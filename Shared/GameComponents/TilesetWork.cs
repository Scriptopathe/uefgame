using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Does operations on tilesets.
    /// </summary>
    public class TilesetWork
    {
        /// <summary>
        /// Gets the source rect from the texture to draw, corresponding to the area of the
        /// tile with the given Id.
        /// </summary>
        /// <param name="tileId"></param>
        /// <returns></returns>
        public static void GetSrcRect(int tileId, ref Rectangle rect)
        {
            int y = tileId / Tileset.TilesetWidthInTiles;
            int x = tileId % Tileset.TilesetWidthInTiles;
            GetSrcRect(x, y, ref rect);
        }
        /// <summary>
        /// Gets the source rect from the texture to draw, corresponding to the area of the
        /// tile with the given coordinates on the tileset.
        /// </summary>
        /// <param name="tileId"></param>
        /// <returns></returns>
        public static void GetSrcRect(int tileX, int tileY, ref Rectangle rect)
        {
            int s = GameConstants.Tilesize;
            rect.X = tileX * s;
            rect.Y = tileY * s;
            rect.Width = rect.Height = s;
        }
        /// <summary>
        /// Gets the texture associated to this tileset.
        /// </summary>
        /// <returns></returns>
        public static Texture2D GetTexture(Tileset tileset)
        {
            return TextureRessourceCache.Cached("RunTimeAssets\\Graphics\\Tilesets\\" + tileset.TextureName); ///Ressources.GameContentRessourceProvider.LoadTilesetTexture(TextureName);
        }
    }
}
