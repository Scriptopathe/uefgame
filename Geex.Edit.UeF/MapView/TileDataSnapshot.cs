using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace Geex.Edit.UeF.MapView
{
    /// <summary>
    /// Class used to take snapshots of the TileData.
    /// </summary>
    class TileDataSnapshot
    {
        int[,] m_dataSnapshot;
        Rectangle m_rect;
        /// <summary>
        /// Creates the snapshots.
        /// </summary>
        /// <param name="rect">The rect of TileData to snap, in tiles of course.</param>
        /// <param name="src">The tiledata array.</param>
        /// <param name="z">The layer to record.</param>
        public TileDataSnapshot(Rectangle rect, int[][,] src, int z)
        {
            m_rect = rect;
            m_dataSnapshot = new int[rect.Width,rect.Height];
            for (int x = rect.X; x < rect.Right; x++)
            {

                for (int y = rect.Y; y < rect.Bottom; y++)
                {
                    m_dataSnapshot[x - rect.X,y - rect.Y] = src[z][x,y];
                }
            }
        }
        /// <summary>
        /// Gets the data associated with the snapshot at the given position, corresponding
        /// to the position on the tile data array.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int this[int x, int y]
        {
            get
            {
                try
                {

                    return m_dataSnapshot[x - m_rect.X, y - m_rect.Y];
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Start x of the snapshot
        /// </summary>
        public int Sx
        {
            get { return m_rect.X; }
        }
        /// <summary>
        /// Start y of the snapshot
        /// </summary>
        public int Sy
        {
            get { return m_rect.Y; }
        }
        /// <summary>
        /// Width of the snapshot
        /// </summary>
        public int Width
        {
            get { return m_rect.Width; }
        }
        /// <summary>
        /// Height of the snapshot
        /// </summary>
        public int Height
        {
            get { return m_rect.Height; }
        }
    }
}
