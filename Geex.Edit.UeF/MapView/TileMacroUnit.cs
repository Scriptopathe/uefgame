using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.UeF.MapView
{
    /// <summary>
    /// Tile info struct
    /// </summary>
    public struct UeFTileInfo
    {
        public int TileX;
        public int TileY;
        public int Layer;
        public int NewTilesetId;
        public int OldTilesetId;
        /// <summary>
        /// Constructor of the struct.
        /// </summary>
        public UeFTileInfo(int x, int y, int layer, int newId, int oldId)
        {
            TileX = x;
            TileY = y;
            NewTilesetId = newId;
            OldTilesetId = oldId;
            Layer = layer;
        }
    }
    /// <summary>
    /// Tile Macro Unit class
    /// This is a unit of information for tile macro recording.
    /// </summary>
    public class UeFTileMacroUnit : MacroUnit
    {
        /// <summary>
        /// Tile infos of the macro unit.
        /// </summary>
        public List<UeFTileInfo> TileInfos;
        public Dictionary<int, Dictionary<int, UeFTileInfo>> m_tileInfoAt = new Dictionary<int,Dictionary<int,UeFTileInfo>>();
        /// <summary>
        /// Constructor, initializes the macro unit.
        /// </summary>
        public UeFTileMacroUnit()
        {
            TileInfos = new List<UeFTileInfo>(256);
        }
        /// <summary>
        /// Gets the tileinfo at the given position.
        /// Returns null if it doesn't exist.
        /// </summary>
        public UeFTileInfo? GetTileInfoAt(int x, int y)
        {
            if (m_tileInfoAt.ContainsKey(x))
                if (m_tileInfoAt[x].ContainsKey(y))
                    return m_tileInfoAt[x][y];
            return null;
        }
        /// <summary>
        /// Adds a tile to the unit
        /// </summary>
        /// <param name="x">x coordinate of the tile</param>
        /// <param name="y">y coordinate of the tile</param>
        /// <param name="layer">layer of the tile</param>
        /// <param name="newTilesetId">the tileset id that replaced the old tileset id</param>
        /// <param name="oldTilesetId">the old tileset id</param>
        public UeFTileInfo Add(int x, int y, int layer, int newTilesetId, int oldTilesetId)
        {
            UeFTileInfo inf = new UeFTileInfo(x, y, layer, newTilesetId, oldTilesetId);
            if (m_tileInfoAt.Keys.Contains(x))
            {
                if (m_tileInfoAt[x].Keys.Contains(y))
                {
                    // Just change the values
                    UeFTileInfo info = m_tileInfoAt[x][y];
                    info.NewTilesetId = inf.NewTilesetId;
                    info.OldTilesetId = inf.OldTilesetId;
                    info.Layer = inf.Layer;
                    TileInfos.Add(inf);
                    return info;
                }
                m_tileInfoAt[x].Add(y, inf);
            }
            else
                m_tileInfoAt.Add(x, new Dictionary<int, UeFTileInfo>() { { y, inf } });
            TileInfos.Add(inf);
            return inf;
        }
    }
}
