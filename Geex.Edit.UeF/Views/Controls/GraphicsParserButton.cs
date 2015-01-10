using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Geex.Edit.Common.Tools.Controls;
using ValidityState = Geex.Edit.Common.Project.ValidityState;
// TODO : corriger les erreurs dues à la modification de GraphicsParser (en particulier sur PicturePicker 
// et ParseButton.
namespace Geex.Edit.UeF.Views
{
    /// <summary>
    /// A button used to parse graphics and display the name of the selected graphics.
    /// Use :
    /// - Create the button using its constructor.
    /// - Call InitializeButton, indicating which folder will contain the files which will be
    ///     shown to the user.
    /// - Set the Value property to the default value. The Options property of the value
    ///     will determine additional parameters the user can select.
    /// - Subscribe to the SelectionChanged event if you need to.
    /// - Each time the button is clicked and the user selects a new file,
    ///     the new value will be put in the Value property.
    /// </summary>
    public class GraphicParserButton : Button
    {
        // Bitmaps.
        static Bitmap s_okBitmap = Common.Tools.Graphics.SystemBitmapCache.CachedBitmap(Common.AppRessources.RessourceDir() + "\\ok.png");
        static Bitmap s_noneBitmap = Common.Tools.Graphics.SystemBitmapCache.CachedBitmap(Common.AppRessources.RessourceDir() + "\\none.png");
        static Bitmap s_invalidBitmap = Common.Tools.Graphics.SystemBitmapCache.CachedBitmap(Common.AppRessources.RessourceDir() + "\\warning.png");
        /// <summary>
        /// Used by the event SelectionChanged.
        /// The object given as argument may be :
        /// A Run.AudioFile object (if the file kind is Audio).
        /// A GraphicsParserOutput object '(if the file kind is Graphics).
        /// </summary>
        /// <param name="value"></param>
        public delegate void SelectionChangedDelegate(GraphicsParserOutput value);
        /// <summary>
        /// Called when the button has been clicked and a file has been selected.
        /// The function will recive different kind of arguments depending on the 
        /// specified FileKind :
        /// - Graphics : a Common.Tools.Controls.GraphicsParserOutput object.
        /// - Audio : a Run.AudioFile object.
        /// </summary>
        public event SelectionChangedDelegate SelectionChanged;
        
        #region Variables
        /// <summary>
        /// Current value hold by the parser.
        /// </summary>
        GraphicsParserOutput m_value;
        /// <summary>
        /// The directory, starting from the content root, where to parse the files.
        /// </summary>
        string m_directory;
        /// <summary>
        /// Parent form of the dialog which will be displayed.
        /// </summary>
        public Form ParentDialogForm
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the filename of the value hold by this button.
        /// </summary>
        public string Filename
        {
            get { return m_value.Filename; }
            set
            {
                m_value.Filename = value;
                UpdatePicto(value);
                Text = value == null ? "" : value;
                UpdatePicto(value);
            }
        }
        /// <summary>
        /// Gets or sets the options of the value hold by this button.
        /// </summary>
        public GraphicsParserOptions Options
        {
            get { return m_value.Options; }
            set { m_value.Options = value; }
        }
        /// <summary>
        /// Sets the value hold by this button.
        /// To get or set individual elements of the value, see Filename and Options properties.
        /// </summary>
        public GraphicsParserOutput Value
        {
            set
            {
                m_value = value;
                Text = value.Filename == null ? "" : value.Filename;
                UpdatePicto(value.Filename);
            }
        }
        /// <summary>
        /// Directory containing the pictures that the user can select.
        /// It is used as a filter for the content.
        /// /!\ It starts from the Content root !
        /// </summary>
        public string GP_Directory
        {
            get { return m_directory; }
            set { m_directory = value; }
        }
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// If you use it, call the InitButton method after it to initialize correctly the parse button.
        /// </summary>
        public GraphicParserButton()
            : base()
        {
            m_value = new GraphicsParserOutput();
            TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            TextAlign = ContentAlignment.MiddleLeft;
            // --- Event handling
            this.Click += new EventHandler(ParseButton_Click);
        }
        /// <summary>
        /// Paints the button.
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
        }
        /// <summary>
        /// Initializes the parse button, using a SetValueDelegate instead of the default behavior.
        /// </summary>
        /// <param name="directory">The directory, starting from the content root, where to parse the files.</param>
        public void InitButton(string directory)
        {
            m_directory = directory;
        }
        /// <summary>
        /// Called when this button is clicked.
        /// Starts the parsing.
        /// </summary>
        void ParseButton_Click(object sender, EventArgs e)
        {
            DialogResult result;
            
            // Creates the parser corresponding to the kind of data the user will have
            // to choose.
            Form parent = ParentDialogForm == null ? Common.Globals.MainForm : ParentDialogForm;
            GraphicsParser parser = new GraphicsParser(GP_Directory);
            parser.Options = Options;
            parser.SetDefaultFilename(Filename);
            result = parser.ShowDialog(parent);

            // User clicked OK :
            if (result == DialogResult.OK)
            {
                string fn = parser.Output.Filename;
                if(SelectionChanged != null)
                    SelectionChanged(parser.Output);
                Value = parser.Output;
            }
        }

        /// <summary>
        /// Updates the Pictogramme indicating the file validity.
        /// </summary>
        void UpdatePicto(string filename)
        {
            switch (Project.GameRessources.GetGraphicsFileValidity(Common.Globals.Project.ContentDirectory + "\\" + m_directory + "\\" + filename))
            {
                case ValidityState.OK:
                    this.Image = s_okBitmap;
                    break;
                case ValidityState.Invalid:
                    this.Image = s_invalidBitmap;
                    break;
                case ValidityState.DoesNotExist:
                    this.Image = s_noneBitmap;
                    break;
            }
        }
    }
}
