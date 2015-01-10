using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace Geex.Edit.UeF.Graphics
{

    /// <summary>
    /// Class used to manages tiles
    /// Uses the options from the GraphicsManager to display correctly the tiles.
    /// This component is hosted by the GraphicsManager.
    /// </summary>
    public class TileRenderer
    {
        Texture2D m_backTexture;
        static float[] s_depths;
        /// <summary>
        /// Automaticaly generates the s_depths (lazy initialization), then returns it.
        /// </summary>
        public static float[] DEPTHS
        {
            get {
                if (s_depths == null)
                {
                    
                    float[] fl = new float[Project.GameOptions.NumberOfLayers];
                    for (int i = 0; i < Project.GameOptions.NumberOfLayers; i++)
                    {
                        fl[i] = 0.2f + 0.01f * (float)i;
                    }
                    s_depths = fl;
                }
                return s_depths;
            }
        }
        /* ------------------------------------------------------------------------------------------
         * PROPERTIES
         * ----------------------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Quick access / shortcut to the tile data
        /// </summary>
        int[][,]TileData
        {
            get { return UeFGlobals.MapDataWrapper.Map.TileIds; }
        }
        /// <summary>
        /// Quick access / shortcut to the tileset bitmap
        /// </summary>
        Texture2D TilesetBitmap
        {
            get { return UeFGlobals.MapDataWrapper.TilesetTexture; }
        }
        /// <summary>
        /// <summary>
        /// Returns the client rectangle of the main form
        /// </summary>
        System.Drawing.Rectangle ClientRectangle
        {
            get
            {
                return Common.Globals.MainForm.ClientRectangle;
            }
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * METHODS
         * ----------------------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        public TileRenderer()
        {
            
        }
        /// <summary>
        /// Draws the tileset on the given sprite batch
        /// </summary>
        /// <param name="spriteBatch">Sprite batch</param>
        public void DrawTiles(SpriteBatch spriteBatch)
        {
            if(m_backTexture == null)
                m_backTexture = UeFGame.TextureRessourceCache.Cached("EditorAssets\\background");

            // If the file does not exist then the Bitmap is null at this point.
            if (TilesetBitmap == null)
                return;
            Vector2 origin = new Vector2(0, 0);
            Vector2 posBuff = new Vector2(0, 0);

            // Scaling of the bitmap
            float scaling = UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom;
            Vector2 scale = new Vector2(scaling, scaling);

            // Tone
            Color defaultCol = Color.White;
            Color darkCol = new Color(125, 125, 125, 255);
            Color transCol = new Color(130, 130, 130, 125);

            // The color that will be used to draw each sprite
            Color col;

            // Layer depth
            float depth = 1.0f;

            // Clipping rect
            System.Drawing.Rectangle clipRect = ClientRectangle;

            // Calculates the real tilesize
            int tilesize = (int)( Project.GameOptions.TileSize * UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom);

            // Clipping
            int minX = Math.Max(0, UeFGlobals.MapView.GraphicsManager.GetScrolling().X / tilesize);
            int maxX = Math.Min(minX + clipRect.Width / tilesize + 2, TileData[0].GetLength(0));
            int minY = Math.Max(0, UeFGlobals.MapView.GraphicsManager.GetScrolling().Y / tilesize);
            int maxY = Math.Min(minY + clipRect.Height / tilesize + 2, TileData[0].GetLength(1));

            // These are used to position the sprite at the good pixel
            int decX = -UeFGlobals.MapView.GraphicsManager.GetScrolling().X % tilesize;
            int decY = -UeFGlobals.MapView.GraphicsManager.GetScrolling().Y % tilesize;

            // Used to store the display x coordinate instead of calculating it more often
            int dispX;

            // Get a data wrapper reference
            // We do it once for all as the RpgGlobals.RpgMapView property makes operations
            // before returning the object.
            var mapView = UeFGlobals.MapView;
                            
            // Main drawing loop
            Rectangle srcRect = new Rectangle();
            int z = -1;


            // Background
            col = Color.White;
            depth = DEPTHS[0]-0.1f;
            for (int x = minX; x < maxX; x++)
            {
                dispX = x - minX;
                for (int y = minY; y < maxY; y++)
                {
                    posBuff.X = dispX * tilesize + clipRect.X + decX;
                    posBuff.Y = (y - minY) * tilesize + clipRect.Y + decY;
                    spriteBatch.Draw(m_backTexture,
                        posBuff,
                        null,
                        col,
                        0.0f,
                        origin,
                        scale,
                        SpriteEffects.None,
                        depth);
                }
            }
            
            
            for (z = 0; z < TileData.Count(); z++)
            {
                // If the layer must be drawn
                if (mapView.GraphicsManager.RenderOptions.MustDrawLayer(z))
                {
                    // Changes the drawing color of the sprites if the GreyInactiveLayers
                    // options is enabled.
                    if (mapView.GraphicsManager.RenderOptions.GreyInactiveLayers)
                        if (z < mapView.GraphicsManager.RenderOptions.ActiveLayer)
                            col = darkCol;
                        else if (z > mapView.GraphicsManager.RenderOptions.ActiveLayer)
                            col = transCol;
                        else
                            col = defaultCol;
                    else
                        col = defaultCol;
                    
                    depth = DEPTHS[z];
                    for (int x = minX; x < maxX; x++)
                    {
                        dispX = x - minX;
                        for (int y = minY; y < maxY; y++)
                        {
                            if (TileData[z][x,y] != -1)
                            {
                                posBuff.X = dispX * tilesize + clipRect.X + decX;
                                posBuff.Y = (y - minY) * tilesize + clipRect.Y + decY;
                                UeFGame.GameComponents.TilesetWork.GetSrcRect(TileData[z][x, y], ref srcRect);
                                spriteBatch.Draw(TilesetBitmap,
                                    posBuff,
                                    srcRect,
                                    col,
                                    0.0f,
                                    origin,
                                    scale,
                                    SpriteEffects.None,
                                    depth);
                                
                            }

                        }
                    }
                }
            }
        }
        #endregion
    }
}