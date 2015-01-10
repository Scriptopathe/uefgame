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
using CU = UeFGame.ConvertUnits;
namespace Geex.Edit.UeF.Graphics
{
    /* -------------------------------------------------------------
    * Delegates
    * -----------------------------------------------------------*/
    #region Delegates
    public delegate Point GetScrollingDelegate();
    #endregion

    /// <summary>
    /// This class manages the graphics.
    /// It updates the display when the DrawSprites() method is called.
    /// It draws the sprites and the tiles (from the TileManager)
    /// 
    /// TODO : implement the drawing of the objects of the map, using the
    /// access to the map data, in "DrawAll"
    /// </summary>
    public class GraphicsManager
    {
        /// <summary>Rectangle meaning Null.</summary>
        public static Rectangle NullRectangle = new Rectangle(-1, -1, -1, -1);
        
        /* -------------------------------------------------------------
         * Variables
         * -----------------------------------------------------------*/
        #region Variables
        /// <summary>Allocated sprites.</summary>
        private List<Sprite> m_allocatedSprites = new List<Sprite>(50);
        /// <summary>Offset of the sprites calculated by the Available drawing area</summary>
        public Vector2 Offset = new Vector2();
        /// <summary>Graphics device on which are rendered the sprites</summary>
        private readonly GraphicsDevice m_device;
        /// <summary>Clipping rectangle</summary>
        public Rectangle ClipRect;
        /// <summary>Method used to get the scrolling</summary>
        public GetScrollingDelegate GetScrolling;
        /// <summary>Render options</summary>
        public RenderOptions RenderOptions = new RenderOptions();
        /// <summary>Panorama renderer</summary>
        public PanoramaRenderer PanoramaRenderer = new PanoramaRenderer();
        /// <summary>
        /// Tile renderer.
        /// </summary>
        public TileRenderer TileRenderer = new TileRenderer();
        /// <summary>
        /// Cursor.
        /// </summary>
        public Cursor Cursor;
        #endregion

        /* -------------------------------------------------------------
         * Properties
         * -----------------------------------------------------------*/
        #region Properties
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

        /* -------------------------------------------------------------
         * Methods
         * -----------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="device">The device which handles</param>
        public GraphicsManager(GraphicsDevice device)
        {
            m_device = device;
            Cursor = new Cursor(this);
        }

        /// <summary>
        /// Draw all sprites
        /// </summary>
        public void DrawAll(SpriteBatch batch)
        {
            // Client area
            System.Drawing.Rectangle rect = ClientRectangle;
            // Conversion from form rect to xna rect
            // Represents the visible area of the screen
            ClipRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            // Calculates the offset of the sprites depending on scrolling
            // and client area
            Point scroll = GetScrolling();
            Offset.X = rect.X - scroll.X;
            Offset.Y = rect.Y - scroll.Y;
            try
            {
                batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            }
            catch
            {
                batch.End();
                batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            }

            // -------------------------------------------
            // - HERE Draw stuff
            Vector2 scrollVect = new Vector2(scroll.X, scroll.Y);
            UeFGame.Editor.GameObjectRenderOptions ro = new UeFGame.Editor.GameObjectRenderOptions();
            ro.Zoom = RenderOptions.Zoom;

            if (UeFGlobals.Controler.State.Mode == MapView.ControlerMode.GameObjectsEditMode)
            {
                // If we are editing an object, darkens the outside of the object.
                Color col = new Color(0, 0, 0, 120);
                var obj = UeFGlobals.Controler.SelectedObject;
                FarseerPhysics.Collision.AABB aabb = obj.BoundingBox;
                Rectangle hole = new Rectangle((int)CU.ToDisplayUnits(aabb.LowerBound.X*ro.Zoom)-scroll.X, 
                    (int)CU.ToDisplayUnits(aabb.LowerBound.Y*ro.Zoom)-scroll.Y,
                    (int)CU.ToDisplayUnits((aabb.UpperBound.X - aabb.LowerBound.X)*ro.Zoom),
                    (int)CU.ToDisplayUnits((aabb.UpperBound.Y - aabb.LowerBound.Y)*ro.Zoom));
                Rectangle clientRect= new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y,
                    this.ClientRectangle.Width, this.ClientRectangle.Height);
                UeFGame.DrawingRoutines.FillExceptHole(batch, clientRect, hole, 1.0f, col);
            }

            // If we are making a selection :
            // ---------------------------------------------------------
            if (UeFGlobals.Controler.State.Mode == MapView.ControlerMode.GameObjects)
            {
                if (UeFGlobals.Controler.GameObjectSelectionRect.HasValue)
                {
                    Color col = new Color(0, 0, 0, 120);
                    Rectangle clientRect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y,
                    this.ClientRectangle.Width, this.ClientRectangle.Height);
                    var selRect = UeFGlobals.Controler.GameObjectSelectionRect.Value;
                    Rectangle hole = new Rectangle(selRect.Left,
                        selRect.Top,
                        selRect.Width,
                        selRect.Height);
                    UeFGame.DrawingRoutines.FillExceptHole(batch, clientRect, hole, 1.0f, col);
                }
            }

            // Draws the objects.
            if (UeFGlobals.MapDataWrapper.Map != null)
            {
                foreach (UeFGame.GameObjects.GameObject obj in UeFGlobals.MapDataWrapper.GameObjects)
                {
                    ro.IsSelected = UeFGlobals.MapView.Controler.SelectedObjects.Contains(obj);
                    obj.DrawInEditor(batch, scrollVect, ro);
                }
            }

            // Draws the tiles
            // ---------------------------------------------------------
            TileRenderer.DrawTiles(batch);

            // Draws the sprites
            // ---------------------------------------------------------
            foreach (Sprite spr in m_allocatedSprites)
            {
                spr.Draw(batch);
            }

            // Ends the drawing
            try
            {
                batch.End();
            }
            catch (AccessViolationException)
            {
                // Catches this exception which occurs randomly
            }
        }
        /// <summary>
        /// Registers the given sprite on the GraphicsManager
        /// </summary>
        public void RegisterSprite(Sprite spr)
        {
            m_allocatedSprites.Add(spr);
        }

        /// <summary>
        /// Unregisters the given sprite on the GraphicsManager
        /// </summary>
        public void UnregisterSprite(Sprite spr)
        {
            m_allocatedSprites.Remove(spr);
        }

        #endregion
    }
}
