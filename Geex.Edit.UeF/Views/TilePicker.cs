using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Geex.Edit.UeF
{
    /// <summary>
    /// Control used to select tiles
    /// </summary>
    public class TilePicker : ScrollableControl
    {
        /* ---------------------------------------------------------------------
         * Delegates and events
         * --------------------------------------------------------------------*/
        #region Delegates and events
        /// <summary>
        /// Delegate for the AreaSelected Event
        /// </summary>
        /// <param name="rect"></param>
        public delegate void AreaSelectedEventHandler(Rectangle rect);
        /// <summary>
        /// This event occurs when an area has been selected.
        /// </summary>
        public event AreaSelectedEventHandler AreaSelected;
        #endregion
        /* ---------------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------------*/
        #region Variables
        /// <summary>Brush used to draw the back of the control</summary>
        TextureBrush m_backBrush;
        /// <summary>Rectangle containing the selected area, in tiles</summary>
        Rectangle m_selectedArea = new Rectangle(1, 1, 2, 2);
        /// <summary>Point containing the coordinates in tiles where the selection started</summary>
        Point m_startPoint = new Point();
        /// <summary>Point containing the coordinates in tiles of the current mouse position</summary>
        Point m_currentPoint = new Point();
        /// <summary>Font used to display errors</summary>
        Font m_errorFont;
        /// <summary>Brush used to display errors</summary>
        Brush m_errorBrush;
        #endregion
        /* ---------------------------------------------------------------------
         * Properties
         * --------------------------------------------------------------------*/
        #region Properties
        string _oldTextureName = "";
        Bitmap _cachedTileset;
        /// <summary>
        /// Returns the bitmap corresponding to the tileset
        /// </summary>
        Bitmap TilesetBitmap
        {
            get
            {
                if (UeFGlobals.MapDataWrapper.Map != null)
                {
                    string textureName = UeFGlobals.Project.Database.Tilesets[UeFGlobals.MapDataWrapper.Map.TilesetId].TextureName;
                    if (_oldTextureName != textureName || _cachedTileset == null)
                    {
                        _cachedTileset = Common.Tools.Graphics.SystemBitmapCache.CachedBitmap(UeFGame.Ressources.FileRessourceProvider.GetTilesetTextureFullPath(textureName));
                    }
                    return _cachedTileset;
                }
                return new Bitmap(64, 64);
            }
        }
        /// <summary>
        /// Gets or sets the selected area.
        /// </summary>
        public Rectangle SelectedArea
        {
            get { return m_selectedArea; }
            set
            {
                if (TilesetBitmap == null)
                    return;
                m_selectedArea.X = Math.Max(0, value.X);
                m_selectedArea.Y = Math.Max(0, value.Y);
                m_selectedArea.Width = Math.Min(MaxTileWidth-m_selectedArea.X, value.Width);
                m_selectedArea.Height = Math.Min(MaxTileHeight-m_selectedArea.Y, value.Height);
            }
        }
        /// <summary>
        /// Returns the maximum number of tiles in the control's width
        /// </summary>
        int MaxTileWidth
        {
            get { return TilesetBitmap.Width / Project.GameOptions.TileSize; }
        }
        /// <summary>
        /// Returns the maximum number of tiles in the control's height
        /// </summary>
        int MaxTileHeight
        {
            get { return TilesetBitmap.Height / Project.GameOptions.TileSize; }
        }
        /// <summary>
        /// Gets the scroll X value
        /// </summary>
        int ScrollX
        {
            get { return -AutoScrollPosition.X; }
            set { AutoScrollPosition = new Point(-value, AutoScrollPosition.Y); }
        }
        /// <summary>
        /// Gets the scroll Y value
        /// </summary>
        int ScrollY
        {
            get { return -AutoScrollPosition.Y; }
            set { AutoScrollPosition = new Point(AutoScrollPosition.X, -value); }
        }
        #endregion
        /* ---------------------------------------------------------------------
         * Methods
         * --------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public TilePicker()
            : base()
        {
            if (!UeFGame.Globals.ExecuteInEditor)
                return;
            UeFGlobals.TilePicker = this;
            this.ClientSize = new System.Drawing.Size(256, -1);
            InitScrollbars();
            m_backBrush = new TextureBrush(Common.AppRessources.RessourceSystemBitmap("stiple.png"));
            // Event handlers
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseMove += new MouseEventHandler(OnMouseMotion);
            this.Resize += new EventHandler(OnResize);

            UeFGlobals.MapDataWrapper.MapLoaded += new Project.MapLoadedDelegate(OnMapLoaded);
            // Font set up
            m_errorFont = new Font(FontFamily.GenericMonospace, 10.0f, FontStyle.Bold);
            m_errorBrush = Brushes.Red;
        }
        /// <summary>
        /// Gets the id of the tile which is at the given (x, y) position on the display.
        /// The first tile represents NOTHING
        /// The first row represents autotiles, and the the others things the tileset.
        /// 
        /// For the tiles of the first row (including autotiles), the return value is
        ///     - 0            : blank tile
        ///     - -autotile_id : where autotile id represents the "column" where to look for the autotile. 
        /// </summary>
        public int GetMatrixId(int x, int y)
        {
            // Autotiles row
            if (y == 0)
            {
                /* Autotile code 
                if (x < Project.RpgGameOptions.NumberOfAutotiles + 1) // includes the "blank" 
                {
                    return (short)-x;
                }*/
                return -1;
            }
            else
            {
                // If the x is in the tileset, and not in the outer area
                if (x <= UeFGame.GameComponents.Tileset.TilesetWidthInTiles)
                {
                    // x + (y-1)*WidthOfTheTileset + FirstTilesetId
                    return (x + (y - 1) * (UeFGame.GameComponents.Tileset.TilesetWidthInTiles));
                }
            }
            return -1;
        }
        /// <summary>
        /// Called when a new map is loaded. Updates this picker.
        /// </summary>
        /// <param name="map"></param>
        void OnMapLoaded(UeFGame.GameComponents.MapInitializingData map)
        {
            OnTilesetChanged();
            Invalidate();
        }
        /// <summary>
        /// This event occurs when the tileset changes / is modified.
        /// </summary>
        public void OnTilesetChanged()
        {
            UpdateScrollbarsRange();
            Invalidate();
        }
        /// <summary>
        /// This event occurs when the control must be painted
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Paint(e.Graphics);
            //UpdateScrollbarsRange();
        }
        /// <summary>
        /// Paints the control the given graphics
        /// </summary>
        new void Paint(System.Drawing.Graphics g)
        {
            if (!UeFGame.Globals.ExecuteInEditor)
                return;

            if (TilesetBitmap == null)
            {   
                // If the Tileset can't be loaded, draws a error message and return.
                SolidBrush brush = new SolidBrush(Color.FromArgb(192, 192, 192));
                g.FillRectangle(brush, this.ClientRectangle);

                // Draws a border
                Pen pen = new Pen(Color.FromArgb(242, 242, 242));
                brush.Color = Color.FromArgb(204, 204, 204);
                int sY = 0;
                int h = 25;
                int w = (int)(this.Width*0.8f);
                int sX = this.Width/2 - w/2;
                g.FillRectangle(brush, sX, sY, w, h);
                g.DrawRectangle(pen, sX, sY, w, h);
                // Draws the text
                g.DrawString("Tileset not found", m_errorFont, m_errorBrush, sX+5, sY+5);
                return;
            }
            // Draws the background
            g.FillRectangle(m_backBrush, this.ClientRectangle);

            // Draws the tileset
            // OLD : g.DrawImage(TilesetBitmap, new Point(-ScrollX, -ScrollY));
            // ------------------------
            // Draws the tileset bitmap
            Rectangle srcRect = new Rectangle(0,
                    0,
                    TilesetBitmap.Width,
                    TilesetBitmap.Height);
            Rectangle destRect = new Rectangle(-ScrollX, Project.GameOptions.TileSize-ScrollY, srcRect.Width, srcRect.Height);
            g.DrawImage(TilesetBitmap, destRect, srcRect, GraphicsUnit.Pixel);
            // ------------------------
            // Draws the autotiles
            destRect = new Rectangle(Project.GameOptions.TileSize-ScrollX, -ScrollY, 
                Project.GameOptions.TileSize, Project.GameOptions.TileSize);
            srcRect = new Rectangle(0, 0, Project.GameOptions.TileSize, Project.GameOptions.TileSize);
            // ------------------------
            // Draws the selection
            int dec = 2; // This is used as an offset, due to the brush's size
            Rectangle selRect = new Rectangle(m_selectedArea.X * Project.GameOptions.TileSize - ScrollX+dec,
                m_selectedArea.Y * Project.GameOptions.TileSize - ScrollY+dec,
                m_selectedArea.Width * Project.GameOptions.TileSize-dec*2, m_selectedArea.Height * Project.GameOptions.TileSize-dec*2);
            g.DrawRectangle(new Pen(Color.Black, 5), selRect);
            g.DrawRectangle(new Pen(Color.White, 3), selRect);
        }
        /// <summary>
        /// Paints the control on a new BufferedGraphics
        /// </summary>
        new void Paint()
        {
            System.Drawing.Graphics t = System.Drawing.Graphics.FromHwndInternal(this.Handle);
            BufferedGraphics g = BufferedGraphicsManager.Current.Allocate(t, this.ClientRectangle);
            Paint(g.Graphics);
            g.Render();
            g.Dispose();
        }
        #endregion
        /* ---------------------------------------------------------------------
         * Input / Event Handling
         * --------------------------------------------------------------------*/
        #region Input / Event Handling
        /// <summary>
        /// Method called to process a mouse click down
        /// </summary>
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                OnStartSelection(e);
            }
        }
        /// <summary>
        /// Method called to process a mouse click up
        /// </summary>
        void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                OnEndSelection(e);
            }
        }
        /// <summary>
        /// Method called to process mouse motion
        /// </summary>
        void OnMouseMotion(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                OnContinueSelection(e);
            }
        }
        /// <summary>
        /// Method called to process a left click
        /// </summary>
        void OnStartSelection(MouseEventArgs e)
        {
            // Starts the selection
            m_startPoint = MouseToTile(e.Location);
            m_currentPoint = m_startPoint;
            SelectedArea = GetRect(m_startPoint, m_currentPoint);
            Paint();
        }
        /// <summary>
        /// Method called when the left button is pressed and the mouse is moving :
        /// updates the selection.
        /// </summary>
        /// <param name="e"></param>
        void OnContinueSelection(MouseEventArgs e)
        {
            m_currentPoint = MouseToTile(e.Location);
            SelectedArea = GetRect(m_startPoint, m_currentPoint);
            Paint();
        }
        /// <summary>
        /// Method called to process a mouse left up event
        /// </summary>
        void OnEndSelection(MouseEventArgs e)
        {
            m_startPoint.X = m_startPoint.Y = 0;
            m_currentPoint = m_startPoint;
            Paint();
            if(AreaSelected != null)
                AreaSelected(SelectedArea);
        }
        /// <summary>
        /// Returns the coordinates in tiles given the mouse position in pixels
        /// </summary>
        /// <param name="p"></param>
        Point MouseToTile(Point p)
        {
            return new Point((p.X + ScrollX)/Project.GameOptions.TileSize,
                (p.Y + ScrollY)/Project.GameOptions.TileSize);
        }
        /// <summary>
        /// Returns a rectangle created using 2 points
        /// </summary>
        Rectangle GetRect(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X)+1, Math.Abs(p1.Y - p2.Y)+1);
        }
        #endregion
        /* ---------------------------------------------------------------------
         * Scrollbars
         * --------------------------------------------------------------------*/
        #region Scrollbars
        /// <summary>
        /// Updates the scrollbar's range
        /// </summary>
        void UpdateScrollbarsRange()
        {
            this.AutoScrollMinSize = GetRealSize();
            if (TilesetBitmap != null)
            {
                // Horizontal scrollbar
                if (TilesetBitmap.Width < this.ClientSize.Width)
                {
                    ScrollX = 0;
                    Paint();
                }
                // Vertical scrollbar
                if (TilesetBitmap.Height < this.ClientSize.Height)
                {
                    ScrollY = 0;
                    Paint();
                }
            }
        }
        /// <summary>
        /// Gets the real size of the control.
        /// Used to update the scrollbars.
        /// </summary>
        /// <returns></returns>
        Size GetRealSize()
        {
            if (TilesetBitmap != null)
                return new Size(TilesetBitmap.Width,
                    TilesetBitmap.Height + Project.GameOptions.TileSize);
            else
                return new Size(this.Width, this.Height);
        }
        /// <summary>
        /// Initializes the scrollbars
        /// </summary>
        /// <param name="form"></param>
        void InitScrollbars()
        {
            this.AutoScroll = true;          
            this.Scroll += new ScrollEventHandler(OnScrollChanged);
        }
        /// <summary>
        /// Occurs when the scrolling position changes
        /// </summary>
        /// <param name="e"></param>
        void OnScrollChanged(object sender, ScrollEventArgs e)
        {
            Paint();
        }
        /// <summary>
        /// Called when the control is resized
        /// </summary>
        void OnResize(object sender, EventArgs e)
        {
            if(UeFGlobals.MapDataWrapper != null) // all the stuff has not been created yet
                UpdateScrollbarsRange();
        }
        #endregion
    }
}
