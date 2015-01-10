using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using TGraphics = System.Drawing.Graphics;
namespace Geex.Edit.Common.Tools.Controls
{
    /// <summary>
    /// Represents a set of options of the Graphics Parser.
    /// Add objects in subclasses of GraphicsParserOptions for them to be displayed
    /// and editable in an object edition view.
    /// The methods : GetSpecialProperty and SetSpecialProperty are used to handle events
    /// raised by the subclasses of GraphicsParser.
    /// 
    /// Ex : SetProperty("MouseClick", new Point(x, y), instanceOfGraphicsParser)
    /// </summary>
    public abstract class GraphicsParserOptions
    {
        #region Variables
        protected Size m_framesCut;
        protected int m_selectedFrameId;
        #endregion
        /// <summary>
        /// Returns a Size whose width indicates the number of frames in the width of the
        /// given bitmap as well as the number of frames in the height of the given bitmap.
        /// I.E. if a charset is containing 16 frames (4 on each row and 4 on each line), this function
        /// shall return a Size whose width and height are 4.
        /// Each of these frames is represented in other functions by an id, created like this :
        /// id = width*y + x
        /// </summary>
        /// <returns></returns>
        public virtual Size GetFramesCut(System.Drawing.Bitmap bmp)
        {
            return m_framesCut;
        }
        /// <summary>
        /// Gets or sets the selected frame id. (see GetFramesCut for more details about frame ids).
        /// </summary>
        /// <returns></returns>
        public virtual int SelectedFrameId
        {
            get { return m_selectedFrameId; }
            set { m_selectedFrameId = value; }
        }

        /// <summary>
        /// Gets the internal values.
        /// This dictionnary is the one which will be modified by the GraphicsParser.
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<string, object> Values { get; set; }
    }

    /// <summary>
    /// Control displaying a picture that enables the user to
    /// double-click it in order to choose the picture among the files
    /// of a pre-defined directory.
    /// It uses the GraphicsParser to display the list of files 
    /// of the directory ; consequently they will be taken from the 
    /// content work.
    /// --------------------------------------------------------------
    /// - The path to the picture can be accessed using the
    ///     Filename property
    /// - The directory name (used as a filter for the content) can be accesed
    ///   using the
    ///     Directory property
    /// The GraphicsParserType and ParentWindow must also be affected
    /// in order to get the picker work.
    /// </summary>
    public sealed class PicturePicker : UserControl
    {
        /* -----------------------------------------------------------
         * Variables
         * ---------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Margin of the canvas.
        /// </summary>
        const int CanvasMargin = 2;

        /// <summary>
        /// Filename of the picture, contained in the specified directory,
        /// which can be accessed using the m_directory variable.
        /// </summary>
        string m_filename;

        /// <summary>
        /// Bitmap displayed to the user.
        /// </summary>
        Bitmap m_bitmap;

        /// <summary>
        /// Directory containing the pictures that the user can select.
        /// It is used as a filter for the content.
        /// </summary>
        string m_directory;

        /// <summary>
        /// Pen used to draw the canvas.
        /// </summary>
        Pen m_canvasPen;

        /// <summary>
        /// Pen used to draw the focus rect.
        /// </summary>
        Pen m_focusPen;

        /// <summary>
        /// Type of the graphics parser from which an instance
        /// will be created.
        /// </summary>
        Type m_graphicsParserType;
        /// <summary>
        /// Value indicating if the bitmap needs to be reloaded.
        /// </summary>
        bool m_bitmapNeedRefresh = false;

        /// <summary>
        /// Validity predicate used in the graphics parser.
        /// </summary>
        GraphicsParser.ValidityPredicateDelegate m_validityPredicate;

        /// <summary>
        /// Source rectangle of the image to draw.
        /// </summary>
        Rectangle m_srcRect = new Rectangle(0, 0, -100, -100);

        /// <summary>
        /// Options used in the GraphicsParser.
        /// </summary>
        GraphicsParserOptions m_graphicsParserOptions;
        #endregion

        /* -----------------------------------------------------------
         * Properties
         * ---------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets or sets the validity predicate used in the graphics parser.
        /// </summary>
        public GraphicsParser.ValidityPredicateDelegate GP_ValidityPredicate
        {
            get { return m_validityPredicate; }
            set { m_validityPredicate = value; }
        }

        /// <summary>
        /// Filename of the picture, contained in the specified directory,
        /// which can be accessed using the Directory property.
        /// </summary>
        public string GP_Filename
        {
            get { return m_filename;  }
            set { m_filename = value; m_bitmapNeedRefresh = true; }
        }
        /// <summary>
        /// Directory containing the pictures that the user can select.
        /// It is used as a filter for the content.
        /// /!\ It starts from the Content root !
        /// </summary>
        public string GP_Directory
        {
            get { return m_directory;  }
            set { m_directory = value; }
        }
        /// <summary>
        /// Type of the graphics parser from which an instance
        /// will be created.
        /// Must inherit from GraphicsParser.
        /// </summary>
        public Type GP_GraphicsParserType
        {
            get { return m_graphicsParserType; }
            set { m_graphicsParserType = value; }
        }
        /// <summary>
        /// (Optional)
        /// Parent window of the GraphicsParser form.
        /// </summary>
        public IWin32Window ParentWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Returns true if the needed properties are non-null, and the
        /// picker is ready to work.
        /// </summary>
        bool IsOK
        {
            get { return GP_Directory != null && GP_Filename != null & GP_GraphicsParserType != null; }
        }
        /// <summary>
        /// Gets or sets the source rectangle of the image to draw.
        /// If 0 > w/h > -1 : the width/height is expressed in percent of the total width.
        /// </summary>
        public Rectangle DisplaySrcRect
        {
            get { return m_srcRect; }
            set { m_srcRect = value;}
        }
        /// <summary>
        /// Gets the GraphicsParserOptions used in the Graphics Parser.
        /// </summary>
        public GraphicsParserOptions GP_Options
        {
            get { return m_graphicsParserOptions; }
            set { m_graphicsParserOptions = value; }
        }

        public delegate void SelectionChangedEventHandler(GraphicsParserOutput newSelection);
        /// <summary>
        /// Fired the selection changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;
        #endregion

        /* -----------------------------------------------------------
         * Methods
         * ---------------------------------------------------------*/
        #region Methods
        /* -----------------------------------------------------------
         * Public methods
         * ---------------------------------------------------------*/
        #region Public
        /// <summary>
        /// Creates a new instance of the PicturePicker class.
        /// </summary>
        public PicturePicker()
            : base()
        {
            SetupEvents();
            m_canvasPen = new Pen(Color.Gray);
            m_focusPen = new Pen(Color.Gray);
            m_focusPen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
            m_focusPen.DashPattern = new float[] { 2, 2 };
        }
        /// <summary>
        /// Performs basic control initializations.
        /// </summary>
        /// <param name="directory">cf Directory</param>
        /// <param name="default_filename">cf Filename</param>
        /// <param name="showSaturation">cd ShowSaturation</param>
        public void Initialize(string directory, string default_filename, Type graphicsParserType, GraphicsParserOptions opts)
        {
            GP_Directory = directory;
            GP_Filename = default_filename;
            GP_Options = opts;
            GP_GraphicsParserType = graphicsParserType;
        }
        #endregion
        /* -----------------------------------------------------------
         * Protected / Event handling methods
         * ---------------------------------------------------------*/
        #region Protected / Event handling
        /// <summary>
        /// Suscribes to the needed events.
        /// </summary>
        void SetupEvents()
        {
            this.DoubleClick += new EventHandler(OnDoubleClick);
            this.GotFocus += delegate(object o, EventArgs e)
            {
                this.Invalidate();
            };
            this.LostFocus += delegate(object o, EventArgs e)
            {
                this.Invalidate();
            };
        }
        /// <summary>
        /// When the mouse is double clicked, open the graphics parser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDoubleClick(object sender, EventArgs e)
        {
            // Throws an exception if the PicturePicker is not OK.
            ThrowIfNotOK();

            GraphicsParser parser = (GraphicsParser)Activator.CreateInstance(GP_GraphicsParserType, m_directory);
            // Sets the default filename and saturation of the parser.
            parser.SetDefaultFilename(GP_Filename);
            parser.ValidityPredicate = GP_ValidityPredicate;
            var result = parser.ShowDialog(ParentWindow);
            if (result == DialogResult.OK)
            {
                ReloadBitmap();
                if (SelectionChanged != null)
                    SelectionChanged(parser.Output);
            }
            parser.Dispose();
        }
        /// <summary>
        /// Paints the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            PaintPicker(e.Graphics);
        }
        /// <summary>
        /// Paints the control on the given Graphics.
        /// </summary>
        /// <param name="g"></param>
        void PaintPicker(TGraphics g)
        {
            // Reloads the bitmap if needed
            ReloadBitmap();

            // Paints the background.
            g.Clear(Color.White);

            // Paints the canvas.
            Rectangle drawRect = new Rectangle(CanvasMargin,
                CanvasMargin, this.ClientSize.Width - CanvasMargin * 2,
                this.ClientSize.Height - CanvasMargin * 2);
            g.DrawRectangle(m_canvasPen, drawRect);

            // Paints the image.
            if (m_bitmap != null)
            {
                // Creates the source rectangle
                Rectangle srcRect = new Rectangle();
                if (DisplaySrcRect.X < 0 && DisplaySrcRect.X >= -100)
                    srcRect.X = (-m_bitmap.Width * DisplaySrcRect.X)/100;
                else
                    srcRect.X = DisplaySrcRect.X;
                if (DisplaySrcRect.Width <= 0 && DisplaySrcRect.Width >= -100)
                    srcRect.Width = (-m_bitmap.Width * DisplaySrcRect.Width)/100;
                else
                    srcRect.Width = DisplaySrcRect.Width;
                if (DisplaySrcRect.Y < 0 && DisplaySrcRect.Y >= -100)
                    srcRect.Y = (-m_bitmap.Height * DisplaySrcRect.Y)/100;
                else
                    srcRect.Y = DisplaySrcRect.Y;
                if (DisplaySrcRect.Height <= 0 && DisplaySrcRect.Height >= -100)
                    srcRect.Height = (-m_bitmap.Height * DisplaySrcRect.Height) / 100;
                else
                    srcRect.Height = DisplaySrcRect.Height;

                // If the bitmap is larger than the drawRect, then scale it :
                if (srcRect.Width > drawRect.Width | srcRect.Height > drawRect.Height)
                {
                    g.DrawImage(m_bitmap, drawRect, srcRect, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(m_bitmap, 
                        new Rectangle(drawRect.X + (drawRect.Width - srcRect.Width)/2,
                        drawRect.Y + (drawRect.Height - srcRect.Height)/2,
                        srcRect.Width, srcRect.Height),
                        srcRect, 
                        GraphicsUnit.Pixel);
                }
            }

            // Paints the focus rectangle
            if (Focused)
            {
                drawRect.X += 3;
                drawRect.Y += 3;
                drawRect.Width -= 6;
                drawRect.Height -= 6;
                g.DrawRectangle(m_focusPen, drawRect);
            }
        }
        /// <summary>
        /// Throws an exception indicating some errors if the PicturePicker
        /// is not OK.
        /// </summary>
        void ThrowIfNotOK()
        {
            List<string> missing = new List<string>();
            if (GP_Filename == null)
                missing.Add("Filename");
            if (GP_Directory == null)
                missing.Add("Directory");
            if (GP_GraphicsParserType == null)
                missing.Add("GraphicsParserType");

            if (missing.Count != 0)
            {
                StringBuilder b = new StringBuilder();
                foreach (string prop in missing)
                {
                    b.Append(prop);
                    b.Append(',');
                }
                b.Remove(b.Length - 1, 1);
                throw new Exception(String.Format("The following properties must be assigned before using that instance : {0}.",
                    b));
            }
        }
        /// <summary>
        /// Reloads the bitmap.
        /// </summary>
        void ReloadBitmap()
        {
            // Reload only if the bitmap needs to be refreshed.
            if(!m_bitmapNeedRefresh || Globals.Project == null)
                return;
            m_bitmapNeedRefresh = false;

            // Path to the bitmap file.
            string path = String.Format("{0}\\{1}\\{2}",
                Globals.Project.ContentDirectory, GP_Directory, GP_Filename);

            // Try to create the bitmap.
            try
            {
                m_bitmap = Common.Tools.Graphics.SystemBitmapCache.CachedBitmap(path);
            }
            catch
            {
                m_bitmap = null;
            }
        }
        #endregion
        /* -----------------------------------------------------------
         * Overriden methods
         * ---------------------------------------------------------*/
        #region Override

        #endregion
        #endregion
    }
}
