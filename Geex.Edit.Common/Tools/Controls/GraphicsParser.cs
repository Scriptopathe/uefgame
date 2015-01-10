using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geex.Edit.Common.Tools.Controls
{
    /// <summary>
    /// Represents a form which displays a list of pictures
    /// in the directory indicated in the constructor.
    /// When the dialog ends, the Ouput property gives information on the selection.
    /// 
    /// --> The GraphicsParser is able to show some parameters by modifying the Options
    ///     property. The values contained in the Options will be pushed into the
    ///     output.
    /// 
    /// --> The ContentWork and ValidityPredicate properties must be overriden
    /// in subclasses of the GraphicsParser.
    /// </summary>
    public abstract partial class GraphicsParser : Form
    {
        /* --------------------------------------------------------------
         * Delegates
         * ------------------------------------------------------------*/
        #region Delegate
        /// <summary>
        /// Delegate used to get the validity state of a file.
        /// </summary>
        /// <param name="filename">Name of the file to test.</param>
        /// <returns>validity of the tested graphics file.</returns>
        public delegate Common.Project.ValidityState ValidityPredicateDelegate(string filename);
        #endregion
        /* --------------------------------------------------------------
         * Variables
         * ------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The directory from where the files are searched.
        /// Relative the content root. (ex : Graphics\\Tilesets)
        /// </summary>
        string m_directory;
        /// <summary>
        /// The output data produced by the GraphicsParser.
        /// </summary>
        protected GraphicsParserOutput m_output = new GraphicsParserOutput();
        /// <summary>
        /// Graphics Parser Options.
        /// These options are displayed in a object edition layer, and the user
        /// can edit them.
        /// See GraphicsParserOptions for further details.
        /// </summary>
        protected GraphicsParserOptions m_options;
        #endregion
        /* --------------------------------------------------------------
         * Properties
         * ------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Graphics Parser Options.
        /// These options are displayed in a object edition layer, and the user
        /// can edit them.
        /// See GraphicsParserOptions for further details.
        /// </summary>
        public GraphicsParserOptions Options
        {
            get { return m_options; }
            set
            {
                m_options = value;
                if (value != null)
                    m_optionsEditionGrid.LoadObject(m_options.Values);
            }
        }
        /// <summary>
        /// Gets the output data produced by this graphics parser when it closes.
        /// </summary>
        public GraphicsParserOutput Output
        {
            get { return m_output; }
        }
        /// <summary>
        /// Gets the selected filename, relative to the directory given as argument.
        /// Returns null if the "None" element was selected or if there was no selection.
        /// </summary>
        protected string SelectedFilename
        {
            get 
            {
                if (m_listbox.SelectedItems.Count != 0)
                    if(m_listbox.SelectedIndices[0] == 0)
                        return null; // returns null if the "none" element was selected.
                    else
                        return m_listbox.SelectedItems[0].Text;
                else
                    return null;
            }
        }
        /// <summary>
        /// Gets a reference to the Viewer of this GraphicsParser.
        /// </summary>
        public AdvancedPictureBoxPanel Viewer
        {
            get { return m_viewer; }
        }
        /// <summary>
        /// Content work used to display the items
        /// This property must be overriden to return the
        /// good content work.
        /// </summary>
        public abstract Common.Project.ContentWork ContentWork
        {
            get;
        }
        /// <summary>
        /// Delegate used to get the validity of a file.
        /// This property must be overriden to return a correct
        /// delegate.
        /// </summary>
        public abstract ValidityPredicateDelegate ValidityPredicate
        {
            get;
            set;
        }
        #endregion
        /* --------------------------------------------------------------
         * Methods
         * ------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new instance of GraphicsParser.
        /// </summary>
        public GraphicsParser(string directory)
        {
            m_directory = directory;
            // Initialization
            InitializeComponent();
            m_optionsEditionGrid.InitializeControl();
            InitializeLang();
            InitializeFilesList();
            
            this.CenterToParent();
        }
        /// <summary>
        /// Sets the default filename selected to the given filename.
        /// </summary>
        /// <param name="filename"></param>
        public void SetDefaultFilename(string filename)
        {
            int j = -1;
            for (int i = 0; i < m_listbox.Items.Count; i++)
            {
                string itemName = m_listbox.Items[i].Text.ToString();
                if (m_listbox.Items[i].Text == filename)
                {   
                    j = i;
                    break;
                }
            }
            if (j != -1)
            {
                m_listbox.SelectedIndices.Clear();
                m_listbox.SelectedIndices.Add(j);
                OnFileSelectionChanged(this, new EventArgs());
            }
        }
        /// <summary>
        /// Sets the default saturation of the parser.
        /// </summary>
        /// <param name="saturation"></param>
        public void SetDefaultSaturation(int saturation)
        {
            m_viewer.Saturation = saturation;
        }
        /// <summary>
        /// Initializes the files list
        /// </summary>
        void InitializeFilesList()
        {
            // Image list
            ImageList l = new ImageList();
            l.Images.Add(Common.AppRessources.RessourceSystemBitmap("ok.png"));
            l.Images.Add(Common.AppRessources.RessourceSystemBitmap("warning.png"));
            l.Images.Add(Common.AppRessources.RessourceSystemBitmap("none.png"));
            l.Images.Add(Common.AppRessources.RessourceSystemBitmap("blueball.png"));
            this.m_listbox.SmallImageList = l;
            this.m_listbox.SelectedIndexChanged += new EventHandler(OnFileSelectionChanged);
        }
        /// <summary>
        /// Called when the selected file changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnFileSelectionChanged(object sender, EventArgs e)
        {
            if (SelectedFilename == null)
            {
                m_viewer.Image = null;
                return;
            }
            // Now we open is the file is valid.
            string fullpath = Common.Globals.Project.ContentDirectory + "\\" + m_directory + "\\" + SelectedFilename;
            if (ValidityPredicate(fullpath) == Common.Project.ValidityState.OK)
            {
                m_viewer.Image = Common.Tools.Graphics.SystemBitmapCache.CachedBitmap(fullpath);
            }
            else
            {
                m_viewer.Image = null;
            }
        }
        /// <summary>
        /// Loads the files in the folder.
        /// </summary>
        protected void UpdateFilesList()
        {
            m_listbox.Clear();
            m_listbox.Items.Add(new ListViewItem("<" + Common.Lang.I["Global_None"] + ">", 3));
            // We will seek the graphics files in the content, not directly in the file system.
            Common.Project.ContentWork work = ContentWork; // new Common.ContentWork(Common.Globals.Project.CurrentProjectDirectory + "\\" + Common.Globals.Project.CsProjectFilename);
            List<string> elems = work.GetElements(m_directory);
            foreach (string file in elems)
            {
                string displayName = file.Replace(m_directory + "\\", "");
                string fullpath = Common.Globals.Project.ContentDirectory + "\\" + file;

                // Choose the good image
                int image = 0;
                switch (ValidityPredicate(fullpath))
                {
                    case Common.Project.ValidityState.OK:
                        image = 0;
                        break;
                    case Common.Project.ValidityState.Invalid:
                        image = 1;
                        break;
                    case Common.Project.ValidityState.DoesNotExist:
                        image = 2;
                        break;
                }
                // Adds the item
                m_listbox.Items.Add(new ListViewItem(displayName, image));
            }
        }
        /// <summary>
        /// Initializes the lang.
        /// </summary>
        void InitializeLang()
        {
            this.m_okButton.Text = Common.Lang.I["Global_OK"];
            this.m_filesLabel.Text = Lang.I["GraphicsParser_FilesLabel"];
            this.m_previewLabel.Text = Lang.I["GraphicsParser_PreviewLabel"];
            this.Text = Lang.I["GraphicsParser_Caption"] + " - " + m_directory;
        }
        /// <summary>
        /// Memorizes the data into the output for convenience.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Output.Options = m_options;
            Output.Filename = SelectedFilename;
        }
        #endregion
    }
    /// <summary>
    /// Object containing the output data produced by the GraphicsParser.
    /// </summary>
    public class GraphicsParserOutput
    {
        public string Filename { get; set; }
        public GraphicsParserOptions Options { get; set; }
        /// <summary>
        /// Creates a new instance of GraphicsParserOutputs and sets the values of
        /// Filename and Options to their default values (i.e. String.Empty and null).
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="opts"></param>
        public GraphicsParserOutput()
        {
            Filename = "";
            Options = null;
        }
        /// <summary>
        /// Creates a new instance of GraphicsParserOutputs and sets the values of
        /// Filename and Options to the given values.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="opts"></param>
        public GraphicsParserOutput(string filename, GraphicsParserOptions opts)
        {
            Filename = filename;
            Options = opts;
        }
    }
}