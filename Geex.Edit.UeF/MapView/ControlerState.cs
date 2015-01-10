using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.UeF.MapView
{
    public enum ControlerMode
    {
        GameObjects,
        GameObjectsEditMode,
        Tile
    }
    public enum DrawMode
    {
        Pen,
        Rectangle,
        FloodFill,
    }
    /// <summary>
    /// Object owned by the controler which defines its state
    /// (draw mode : square, point etc...)
    /// </summary>
    public class ControlerState
    {
        /* ---------------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------------*/
        #region Variables
        ControlerMode m_mode = ControlerMode.Tile;
        DrawMode m_drawMode = DrawMode.Rectangle;

        #endregion
        /* ---------------------------------------------------------------------
         * Properties
         * --------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Mode of the controler.
        /// </summary>
        public ControlerMode Mode
        {
            get { return m_mode; }

            set { m_mode = value; }
        }
        /// <summary>
        /// Drawing mode if the controler mode is on Tile.
        /// </summary>
        public DrawMode DrawMode
        {
            get { return m_drawMode; }
            set { m_drawMode = value; }
        }
        #endregion
    }
}
