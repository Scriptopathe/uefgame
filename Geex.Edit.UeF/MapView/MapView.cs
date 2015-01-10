using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GraphicsManager = Geex.Edit.UeF.Graphics.GraphicsManager;
using MapDataWrapper = Geex.Edit.UeF.Project.MapDataWrapper;
using Map = UeFGame.GameComponents.MapInitializingData;
using MapInfo = Geex.Edit.Common.Project.MapInfo;
namespace Geex.Edit.UeF.MapView
{
    /// <summary>
    /// This class manages the map editor control and receves its events.
    /// When an event is thrown, this class tells the controler that event happened for the changes to take place.
    /// 
    /// It also has the ownership of :
    ///     - A RenderManager object
    ///     - A MapDataWrapper object
    ///     - A IControler object
    ///     - A Graphics Manager object
    ///     
    /// This class has no interdependencies with the main map editor window.
    /// </summary>
    public class MapView : WinFormsGraphicsDevice.GraphicsDeviceControl, Common.MapView.IMapView
    {
        #region Delegates and events
        public event Common.MapView.ContentLoadedEventHandler ContentLoaded;
        #endregion
        /* ------------------------------------------------------------------------------------------
         * VARIABLES
         * ----------------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>Indicates if the Xna area must be repainted.
        /// Value = 0 : no repaint. Value > 0 indicates number of repaints needed.
        /// This is used because the call to Update is not made at the good time by Xna.</summary>
        short m_dirtyness = 2;
        /// <summary>Graphics manager. Manages the drawing of the sprites.</summary>
        GraphicsManager m_graphicsManager;
        /// <summary>SpriteBatch used to draw sprites</summary>
        SpriteBatch spriteBatch;
        /// <summary>The controler used to modify data</summary>
        Controler m_controler;
        /// <summary>The data wrapper used to provide fast access to static data</summary>
        MapDataWrapper m_mapDataWrapper;
        /// <summary>Content manager</summary>
        ContentManager m_content;
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Properties
         * ----------------------------------------------------------------------------------------*/
        #region Properties
        public GraphicsManager GraphicsManager { get { return m_graphicsManager; } } 
        public Controler Controler { get { return m_controler; } }
        public MapDataWrapper MapDataWrapper { get { return m_mapDataWrapper; } }
        public bool IsDirty { get { return m_dirtyness > 0; } set { m_dirtyness = value ? (short)1 : (short)0; } }
        public int ScrollX { get { return -AutoScrollPosition.X; } }
        public int ScrollY { get { return -AutoScrollPosition.Y; } }
        public ContentManager Content { get { return m_content; } set { m_content = value; } }
        public object CurrentMapObject { get { return m_mapDataWrapper.Map; } }
        public Map CurrentMap { get { return m_mapDataWrapper.Map; } }
        protected new System.Drawing.Rectangle ClientRectangle
        {
            get { return Common.Globals.MainForm.ClientRectangle; }
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * METHODS
         * ----------------------------------------------------------------------------------------*/
        #region Methods
        /* ------------------------------------------------------------------------------------------
         * Initialization METHODS
         * ----------------------------------------------------------------------------------------*/
        #region Initialization
        /// <summary>
        /// Initializes the xna game
        /// </summary>
        public MapView()
            : base()
        {
            // Docking
            this.Dock = DockStyle.Fill;
        }
        /// <summary>
        /// Initializes the MapEditorControl
        /// </summary>
        protected override void Initialize()
        {
            LoadContent();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected void LoadContent()
        {
            // -- Setup of the graphics
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SamplerState state = new SamplerState();
            state.Filter = TextureFilter.Point;
            spriteBatch.GraphicsDevice.SamplerStates[0] = state;
            // Associate the sprites with this GraphicsDevice.
            m_graphicsManager = new GraphicsManager(GraphicsDevice);
            // Scrolling delegate fot the sprites
            m_graphicsManager.GetScrolling = new UeF.Graphics.GetScrollingDelegate(GetScrolling);
            // -- Setup of components
            // Now creates the render manager, map data wrapper and controler
            m_mapDataWrapper = new MapDataWrapper();
            m_controler = new Controler();
            
            // Here perform initialization that needs everything to be set up
            if (ContentLoaded != null)
                ContentLoaded();

            // Now call the setup events method of the controler
            m_controler.SetupEvents();

            // -- Finalization
            RefreshMap();

            // -- Events set up
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(OnMouseMotion);
            this.MouseLeave += new EventHandler(OnMouseLeave);
            this.MouseEnter += new EventHandler(OnMouseEnter);
            this.MouseWheel += new MouseEventHandler(OnMouseWheelNew);
            this.PreviewKeyDown += new PreviewKeyDownEventHandler(OnKeyDown);
            this.GraphicsManager.RenderOptions.ZoomChanged += new Graphics.RenderOptions.ZoomChangedDelegate(OnRenderOptions_ZoomChanged);
            
            System.Windows.Forms.Application.Idle += new EventHandler(Update);
        }

        #endregion
        /* ------------------------------------------------------------------------------------------
         * Scrollbars handling METHODS
         * ----------------------------------------------------------------------------------------*/
        #region Scrollbars
        /// <summary>
        /// Call that event when the map is resized : resize the scrollable area.
        /// Or when the zoom changed.
        /// Call it when some map assets changed, like tileset.
        /// </summary>
        public void RefreshMap()
        {
            AutoScroll = true;
            if (m_mapDataWrapper.Map != null)
            {
                this.AutoScrollMinSize = new System.Drawing.Size((int)(this.m_mapDataWrapper.MapWidth * GraphicsManager.RenderOptions.Zoom),
                    (int)(this.m_mapDataWrapper.MapHeight * GraphicsManager.RenderOptions.Zoom));

                m_mapDataWrapper.RefreshTilesetTexture();
                IsDirty = true;
            }
            else
                this.AutoScrollMinSize = this.ClientSize;
        }
        /// <summary>
        /// This event occurs when the resize of the form has ended
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnResizeEnd(object sender, EventArgs e)
        {
            this.GraphicsDevice.ScissorRectangle = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, 
                this.ClientRectangle.Width, this.ClientRectangle.Height);
            IsDirty = true;
        }
        /// <summary>
        /// Appears when the scroll changed.
        /// </summary>
        void OnScrollChanged(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            IsDirty = true;
        }
        /// <summary>
        /// Used as a delegate for Sprite to get the scrolling
        /// </summary>
        /// <returns></returns>
        public Point GetScrolling()
        {
            return new Point(ScrollX, ScrollY);
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Mouse event handling METHODS
         * ----------------------------------------------------------------------------------------*/
        #region Mouse event / Keyboard handling
        /// <summary>
        /// Gets the keys which could be considered as input keys.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        /// <summary>
        /// Called when a key is pressed.
        /// Used for :
        ///     - Zoom
        /// </summary>
        void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            m_controler.OnKeyDown(e);
        }
        /// <summary>
        /// Called when the mouse leaves the XnaWindow
        /// </summary>
        void OnMouseLeave(object sender, EventArgs e)
        {
            m_controler.OnMouseLeave();
        }
        /// <summary>
        /// Called when the mouse enters the XnaWindow
        /// </summary>
        void OnMouseEnter(object sender, EventArgs e)
        {
            m_controler.OnMouseEnter();
        }
        /// <summary>
        /// Called when the mouse will value changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMouseWheelNew(object sender, MouseEventArgs e)
        {
            if ((System.Windows.Forms.Control.ModifierKeys & Keys.Control) != 0)
            {
                if (e.Delta > 0)
                {
                    System.Drawing.Point oldScroll = AutoScrollPosition;
                    GraphicsManager.RenderOptions.Zoom *= 2.0f;
                    AutoScrollPosition = new System.Drawing.Point(oldScroll.X*2, oldScroll.Y*2);
                }
                else
                {
                    System.Drawing.Point oldScroll = AutoScrollPosition;
                    GraphicsManager.RenderOptions.Zoom /= 2.0f;
                    AutoScrollPosition = new System.Drawing.Point(oldScroll.X / 2, oldScroll.Y / 2);
                }

                IsDirty = true;
            }
        }
        /// <summary>
        /// Called when the mouse if moving.
        /// </summary>
        void OnMouseMotion(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if(m_controler.ActionStartedLeft)
                    m_controler.OnUpdateAction(e.Location, (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (m_controler.SelectionStarted)
                    m_controler.OnUpdateRightAction(e.Location);
            }
            m_controler.OnMouseMotion(e.Location);
        }
        /// <summary>
        /// Called when a mouse button is released.
        /// </summary>
        void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (m_controler.ActionStartedLeft)
                    m_controler.OnEndAction(e.Location, (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (m_controler.SelectionStarted)
                    m_controler.OnEndRightAction(e.Location);
            }
        }
        /// <summary>
        /// This is produced when the mouse is clicked (but not released).
        /// If it's a left click, it will start an action.
        /// If it's a right click when an action is started, it will abort the action.
        /// </summary>
        void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Focus();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_controler.OnStartAction(e.Location, (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right) 
            {
                // Aborts the action if it is started
                if (m_controler.ActionStartedLeft)
                {
                    m_controler.AbortAction();
                }
                else if (!m_controler.SelectionStarted)
                {
                    m_controler.OnStartRightAction(e.Location);
                }
            }
        }
        /// <summary>
        /// Returns the scrolled value in pixel of the point p, in mouse pixels.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        System.Drawing.Point Scrolled(System.Drawing.Point p)
        {
            System.Drawing.Point newP = new System.Drawing.Point();
            newP.X = Math.Max(p.X - ClientRectangle.X + ScrollX, 0);
            newP.Y = Math.Max(p.Y - ClientRectangle.Y + ScrollY, 0);
            return newP;
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Event Handling METHODS
         * ----------------------------------------------------------------------------------------*/
        #region Event handling
        /// <summary>
        /// Called when the zoom changed.
        /// </summary>
        void OnRenderOptions_ZoomChanged(float newValue)
        {
            RefreshMap();
        }
        /// <summary>
        /// This method captures input and sends events corresponding to that Input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void Update(object sender, EventArgs args)
        {
            // Do this in order not to take all the CPU usage
            Thread.Sleep(10);
            if(IsDirty)
            {
                m_dirtyness -= 1;
                BeginDraw();
                Draw();
                EndDraw();
            }
        }

        /// <summary>
        /// Routine for the Draw(GameTime) method
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.Black);
            // Draws all the sprites
            GraphicsManager.DrawAll(spriteBatch);
        }

        #endregion
        /* ------------------------------------------------------------------------------------------
         * Dispose METHODS
         * ----------------------------------------------------------------------------------------*/
        #region Dispose
        /// <summary>
        /// Disposes all the allocated ressources.
        /// </summary>
        public void Dispose()
        {
            base.Dispose();
            m_controler.Dispose();
            System.Windows.Forms.Application.Idle -= new EventHandler(Update);
            UeFGame.TextureRessourceCache.FreeAndDisposeAll();
        }

        #endregion

        #region Misc
        /// <summary>
        /// Returns the screen rectangle in tiles.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Rectangle GetScreenTileRectangle()
        {
            System.Drawing.Point p1 = new System.Drawing.Point(ScrollX, ScrollY);
            System.Drawing.Point p2 = new System.Drawing.Point(
                ClientRectangle.Width,
                ClientRectangle.Height);
            int ts = (int)(Project.GameOptions.TileSize * GraphicsManager.RenderOptions.Zoom);
            return new System.Drawing.Rectangle(p1.X / ts, p1.Y / ts, p2.X / ts, p2.Y / ts);
        }
        #endregion
        #endregion
    }
}