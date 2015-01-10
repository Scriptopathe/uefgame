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
    /// Form that enables the user to import or delete ressources of the content project, 
    /// directly from Geex.Edit.
    /// It will overwrite the contentproj file each time a new file is imported, after having placed
    /// it in the good folder.
    /// 
    /// The implementations must override the following methods :
    ///     - GetGraphicsCategoriesList()
    ///     - GetAudioCategoriesList()
    /// </summary>
    public abstract partial class RessourcesManagerForm : Form
    {
        #region Delegates
        delegate void FileSelectedDelegate(string fullFilename);
        delegate bool FileValidDelegate(string fullFilename);
        #endregion
        /* ---------------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Content work class used to load and modify content.
        /// </summary>
        Common.Project.ContentWork ContentWork
        {
            get { return Common.Globals.ContentWork; }
        }
        /// <summary>
        /// Gets the current directory name selected in the Graphics part.
        /// (ex : "Graphics\\Pictures")
        /// </summary>
        string m_graphicsCurrentSelectionDir;
        /// <summary>
        /// Gets the current directory name selected in the audio part.
        /// </summary>
        string m_audioCurrentSelectionDir;
        /// <summary>
        /// Audio service used to play music/sound files.
        /// </summary>
        Common.Tools.Audio.AudioPlayer m_audioService;
        #endregion
        /* ---------------------------------------------------------------------
         * Properties
         * --------------------------------------------------------------------*/
        #region Properties


        #endregion
        /* ---------------------------------------------------------------------
         * Methods
         * --------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor : displays the ressources manager form.
        /// </summary>
        public RessourcesManagerForm()
        {
            // m_contentWork = new Common.ContentWork(Common.Globals.Project.CurrentProjectDirectory + "\\" + Common.Globals.Project.CsProjectFilename);
            m_audioService = new Common.Tools.Audio.AudioPlayer();
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
            InitializeComponent();
            SetupGraphicsCategoriesList();
            SetupGraphicsFilenameList();
            SetupAudioCategoriesList();
            SetupAudioFilenameList();
            SetupButtonsEvents();

            InitLang();
        }
        /// <summary>
        /// This method is called when the form is closing.
        /// It ends the audio service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            m_audioService.StopSong();
            m_audioService.End();
        }

        #region Lang
        /// <summary>
        /// Changes the text of all the items to match the current language.
        /// </summary>
        void InitLang()
        {
            // Graphics
            m_graphicsFilesLabel.Text = Lang.I["RessourcesManagerForm_FilesLabel"];
            m_graphicsCatLabel.Text = Lang.I["RessourcesManagerForm_CategoriesLabel"];
            m_graphicsDeleteButton.Text = Lang.I["RessourcesManagerForm_DeleteButton"];
            m_graphicsImportButton.Text = Lang.I["RessourcesManagerForm_ImportButton"];
            m_graphicsTab.Text = Lang.I["RessourcesManagerForm_GraphicsTab"];
            // Audio
            m_audioFilesLabel.Text = Lang.I["RessourcesManagerForm_FilesLabel"];
            m_audioCatLabel.Text = Lang.I["RessourcesManagerForm_CategoriesLabel"];
            m_audioDeleteButton.Text = Lang.I["RessourcesManagerForm_DeleteButton"];
            m_audioImportButton.Text = Lang.I["RessourcesManagerForm_ImportButton"];
            m_audioTab.Text = Lang.I["RessourcesManagerForm_AudioTab"];
            m_musicPlayButton.Text = Lang.I["RessourcesManagerForm_MusicPlay"];
            m_musicStopButton.Text = Lang.I["RessourcesManagerForm_MusicStop"];
            // Both
            m_okButton.Text = Common.Lang.I["Global_OK"];
        }
        #endregion

        #region Common
        /// <summary>
        /// Sets ups the events attached to the buttons.
        /// </summary>
        void SetupButtonsEvents()
        {
            m_graphicsDeleteButton.Click += new EventHandler(OnDeleteGraphicsSelection);
            m_graphicsImportButton.Click += new EventHandler(OnImportGraphics);
            m_audioDeleteButton.Click += new EventHandler(OnDeleteAudioSelection);
            m_audioImportButton.Click += new EventHandler(OnImportAudio);
            // Disable the buttons because no item is selected.
            m_graphicsImportButton.Enabled = false;
            m_graphicsDeleteButton.Enabled = false;
            m_audioImportButton.Enabled = false;
            m_audioDeleteButton.Enabled = false;

            // Audio buttons
            m_musicPlayButton.Click += new EventHandler(OnAudioFileDoubleClick);
            m_musicStopButton.Click += delegate(object o, EventArgs e) { m_audioService.StopSong(); };

        }

        /// <summary>
        /// Called each time the contentWork is modified.
        /// </summary>
        void OnContentWorkModified()
        {
            // TODO : check if removing of that line is needed.
            ContentWork.Save();
        }
        /// <summary>
        /// Common method for setting up the filename list.
        /// Takes as argument :
        /// </summary>
        /// <param name="listview">The list view to setup</param>
        /// <param name="categories">The categories (ie subfolders of content to include)</param>
        /// <param name="onItemSelectionChanged">The method to call when the item selection has changed</param>
        void SetupCategoriesList(ListView listview, string[] categories,
            ListViewItemSelectionChangedEventHandler onItemSelectionChanged)
        {
            // List view setup
            listview.View = View.SmallIcon;
            listview.MultiSelect = false;
            listview.ItemSelectionChanged += onItemSelectionChanged;
            // Image list
            ImageList list = new ImageList();
            list.Images.Add(Common.AppRessources.RessourceSystemBitmap("folder.png"));
            list.ImageSize = new Size(16, 16);
            listview.SmallImageList = list;
            // Adds categories.
            // The categories correspond to the folders they refer to.
            foreach (string cat in categories)
            {
                listview.Items.Add(new ListViewItem(cat, 0));
            }
        }
        /// <summary>
        /// Sets up a filename list, which will display the filenames of the files in the
        /// current category / directory.
        /// </summary>
        /// <param name="listview">the listview to set up.</param>
        /// <param name="onFileSelected">the method to call when a file is selected.</param>
        void SetupFilenameList(ListView listview, ListViewItemSelectionChangedEventHandler onFileSelected)
        {
            listview.View = View.SmallIcon;
            listview.MultiSelect = true;
            listview.ItemSelectionChanged += onFileSelected;
            // Sets up the image list
            ImageList list = new ImageList();
            list.Images.Add("ok", Common.AppRessources.RessourceSystemBitmap("ok.png"));
            list.Images.Add("none", Common.AppRessources.RessourceSystemBitmap("none.png"));
            list.Images.Add("invalid", Common.AppRessources.RessourceSystemBitmap("warning.png"));
            listview.SmallImageList = list;
        }
        /// <summary>
        /// Called when a file is selected, it is a "generic" implementation of it.
        /// </summary>
        /// <param name="view">The listview concerned by the event</param>
        /// <param name="currentSelectionDir">the directory (from the content root) selected, ie, the one which contains the selected file.</param>
        /// <param name="filename">The filename (not full) of the file clicked (must be in the currentSelectionDir).</param>
        /// <param name="onFileSelected">Method called when the file is selected</param>
        /// <param name="deleteButton">Delete button linked with the list, it will be automatically enabled / disabled</param>
        void OnFileSelected(ListView view, string currentSelectionDir, 
            string filename, FileSelectedDelegate onFileSelected,
            Button deleteButton)
        {
            if (view.SelectedIndices.Count > 0)
            {
                // Enables the "Delete" button
                deleteButton.Enabled = true;
                // Full filename of the selected file. 
                string filenameFull = Common.Globals.Project.ContentDirectory + "\\" + currentSelectionDir + "\\" + filename;
                if (System.IO.File.Exists(filenameFull))
                {
                    onFileSelected(filenameFull);
                }
            }
            else
            {
                // Disables the "Delete" button (no selection to delete)
                deleteButton.Enabled = false;
            }
        }
        /// <summary>
        /// Adds an item in the given filename listview.
        /// Filename may be the full path
        /// </summary>
        /// <param name="isValidFile">Indicates whether or not the file is valid</param>
        void FilenamesListAddItem(ListView list, string fullFilename, bool isValidFile)
        {
            string imageState;
            if (System.IO.File.Exists(fullFilename))
                if (isValidFile)
                    imageState = "ok";
                else
                    imageState = "invalid";
            else
                imageState = "none";
            list.Items.Add(
                new ListViewItem(System.IO.Path.GetFileName(fullFilename), imageState));
        }
        /// <summary>
        /// Called when a directory is Selected.
        /// Update the Graphics filename list items : loads the content and display it.
        /// </summary>
        /// <param name="filenamesList">The filename list to refresh</param>
        /// <param name="e">The event args of the event from the Categories Listbox, which determines
        /// which directory has been chosen.</param>
        /// <param name="deleteButton">The delete button linked to that list</param>
        /// <param name="importButton">The import button linked to that list</param>
        void OnRefreshFilenameListItems(ListView filenamesList,
            Button importButton, Button deleteButton,
            ListViewItemSelectionChangedEventArgs e,
            FileValidDelegate isValidDelegate)
        {
            // Enables the Import button because we have a folder selected.
            importButton.Enabled = e.IsSelected;

            // Here we assume that the directory is 
            // ContentDirectory + label of the directory in the categories list.
            // Path of the directory containing the file.
            string dir = Common.Globals.Project.ContentDirectory + "\\" + e.Item.Text;

            // Clears the list
            filenamesList.Clear();

            if (e.IsSelected)
            {
                // Now add the elements to the list
                string includeDir = e.Item.Text;
                string[] elems = ContentWork.GetElements(includeDir).ToArray();
                foreach (string path in ContentWork.GetElements(includeDir))
                {
                    string filename = System.IO.Path.GetFileName(path);
                    FilenamesListAddItem(filenamesList, 
                       dir + "\\" + filename,
                       isValidDelegate(filename));
                }
            }
            else
            {
                deleteButton.Enabled = false;
            }
        }
        /// <summary>
        /// Import new files into the current folder.
        /// </summary>
        /// <param name="dialogFilter">The dialogbox extension filter</param>
        /// <param name="currentSelectionDir">The directory into which import the files.
        /// (starting from the content root)</param>
        /// <param name="view">The view where to add the items once imported.</param>
        void OnImport(ListView view, string dialogFilter, string currentSelectionDir)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Multiselect = true;
            dlg.Filter = dialogFilter;

            // Now changes the default path
            FileDialogCustomPlace place = new FileDialogCustomPlace(
                System.IO.Path.GetFullPath(Common.Globals.Project.ContentDirectory));
            dlg.CustomPlaces.Add(place);

            // Shows the dialog
            if ((dlg.ShowDialog(this) & System.Windows.Forms.DialogResult.OK) != 0)
            {
                // Now import all the ressources in the folder.
                string dirName = Common.Globals.Project.ContentDirectory + "\\" + currentSelectionDir;
                foreach (string srcFile in dlg.FileNames)
                {
                    // Full path of the new filename
                    string newFileName = dirName + "\\" + System.IO.Path.GetFileName(srcFile);
                    // Copies the new file on the disk at its new location in the project.
                    if (newFileName != srcFile)
                        System.IO.File.Copy(srcFile, newFileName);

                    // Adds the file in the project
                    string includePath = currentSelectionDir + "\\" + System.IO.Path.GetFileName(srcFile);
                    ContentWork.AddElement(includePath);
                    // Note : the imported file is considered as valid
                    FilenamesListAddItem(view, newFileName, true);
                }

                OnContentWorkModified();
            }
        }
        /// <summary>
        /// Deletes the files selected in the given filename list.
        /// Removes them from the project.
        /// </summary>
        /// <param name="filenameList">The filename list where to delete the files</param>
        /// <param name="currentSelectionDir">The current directory in which are located the
        /// selected files</param>
        void OnDeleteSelection(ListView filenameList, string currentSelectionDir)
        {
            foreach (ListViewItem item in filenameList.SelectedItems)
            {
                string itemInclude = currentSelectionDir + "\\" + item.Text;
                ContentWork.DeleteElement(itemInclude);
                filenameList.Items.Remove(item);
                OnContentWorkModified();
            }
        }
        #endregion

        #region Graphics
        /// <summary>
        /// Import new graphics into the current folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnImportGraphics(object sender, EventArgs e)
        {
            // Gets the filter needed for extensions
            string exts = "";
            foreach (string ext in Common.GeexSettings.SupportedBitmapExtensions)
            {
                exts += "*." + ext + ";";
            }
            exts.Remove(exts.Count() - 1); // remove last coma ";"
            exts = "Supported Bitmap files (" + exts + ")|" + exts;
            OnImport(m_graphicsFilenamesListBox, exts, m_graphicsCurrentSelectionDir);
        }
        /// <summary>
        /// Deletes the files selected in the graphics part.
        /// Removes them from the project.
        /// </summary>
        void OnDeleteGraphicsSelection(object sender, EventArgs e)
        {
            OnDeleteSelection(m_graphicsFilenamesListBox, m_graphicsCurrentSelectionDir);
        }
        /// <summary>
        /// Gets the graphics categories corresponding to folders in the Graphics directory.
        /// </summary>
        /// <returns></returns>
        protected abstract string[] GetGraphicsCategoriesList();
        /// <summary>
        /// Sets up the graphics categories list. May only be called in the constructor.
        /// </summary>
        void SetupGraphicsCategoriesList()
        {
            SetupCategoriesList(m_graphicsCategoriesListBox, GetGraphicsCategoriesList(),
                new ListViewItemSelectionChangedEventHandler(OnRefreshGraphicsFilenameListItems));
        }
        /// <summary>
        /// Sets up the Graphics filename list.
        /// It doesn't change the items in it, just subscribe to events and 
        /// change some properties.
        /// </summary>
        void SetupGraphicsFilenameList()
        {
            SetupFilenameList(m_graphicsFilenamesListBox, 
                new ListViewItemSelectionChangedEventHandler(OnGraphicsFileSelected));
        }
        /// <summary>
        /// This method is called when a file is selected.
        /// Used to display the preview.
        /// </summary>
        void OnGraphicsFileSelected(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // When a file is selected, opens it in the preview
            FileSelectedDelegate onFileSelected = delegate(string filename)
            {
                try
                {
                    System.Drawing.Bitmap bmp = new Bitmap(filename);
                    m_graphicsPreview.Image = bmp;
                }
                catch (Exception)
                {

                }
            };
            OnFileSelected(m_graphicsFilenamesListBox, m_graphicsCurrentSelectionDir, e.Item.Text,
                onFileSelected, m_graphicsDeleteButton);
        }
        /// <summary>
        /// Called when a directory is Selected.
        /// Update the Graphics filename list items : loads the content and display it.
        /// </summary>
        void OnRefreshGraphicsFilenameListItems(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            m_graphicsCurrentSelectionDir = e.Item.Text;
            OnRefreshFilenameListItems(m_graphicsFilenamesListBox,
                m_graphicsImportButton, m_graphicsDeleteButton, e,
                delegate(string filename) // always valid
                {
                    return true;
                });
        }
        #endregion

        #region Audio
        /// <summary>
        /// Import new graphics into the current folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnImportAudio(object sender, EventArgs e)
        {
            // Gets the filter needed for extensions
            string exts = "";
            foreach (string ext in Common.GeexSettings.SupportedAudioExtensions)
            {
                exts += "*." + ext + ";";
            }
            exts.Remove(exts.Count() - 1); // remove last coma ";"
            exts = "Supported Audio files (" + exts + ")|" + exts;
            OnImport(m_audioFilenames, exts, m_audioCurrentSelectionDir);
        }
        /// <summary>
        /// Deletes the files selected in the graphics part.
        /// Removes them from the project.
        /// </summary>
        void OnDeleteAudioSelection(object sender, EventArgs e)
        {
            OnDeleteSelection(m_audioFilenames, m_audioCurrentSelectionDir);
        }
        /// <summary>
        /// Gets the graphics categories corresponding to folders in the Graphics directory.
        /// </summary>
        /// <returns></returns>
        protected abstract string[] GetAudioCategoriesList();
        /// <summary>
        /// Sets up the graphics categories list. May only be called in the constructor.
        /// </summary>
        void SetupAudioCategoriesList()
        {
            SetupCategoriesList(m_audioCategoriesListBox, GetAudioCategoriesList(),
                new ListViewItemSelectionChangedEventHandler(OnRefreshAudioFilenameListItems));
        }
        /// <summary>
        /// Sets up the Graphics filename list.
        /// It doesn't change the items in it, just subscribe to events and 
        /// change some properties.
        /// </summary>
        void SetupAudioFilenameList()
        {
            SetupFilenameList(m_audioFilenames,
                new ListViewItemSelectionChangedEventHandler(OnAudioFileSelected));
            m_audioFilenames.DoubleClick += new EventHandler(OnAudioFileDoubleClick);
        }
        /// <summary>
        /// This method is called when a file is double clicked.
        /// Previews the audio?
        /// </summary>
        void OnAudioFileDoubleClick(object sender, EventArgs e)
        {
            if (m_audioFilenames.SelectedIndices.Count > 0)
            {
                // filename
                string fn = m_audioFilenames.SelectedItems[0].Text;
                string path = Common.Globals.Project.ContentDirectory + "\\" + m_audioCurrentSelectionDir + "\\" + fn;
                if(IsValidAudioFile(path) && System.IO.File.Exists(path))
                    m_audioService.PlaySong(path);
            }
        }
        /// <summary>
        /// This method is called when a file is selected.
        /// Used to display the preview.
        /// </summary>
        void OnAudioFileSelected(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // When a file is selected... does nothing for the moment.
            m_musicPlayButton.Enabled = false;
            FileSelectedDelegate onFileSelected = delegate(string filename)
            {
                m_musicPlayButton.Enabled = IsValidAudioFile(filename);
            };
            OnFileSelected(m_audioFilenames, m_audioCurrentSelectionDir, e.Item.Text,
                onFileSelected, m_audioDeleteButton);
        }
        /// <summary>
        /// Called when a directory is Selected.
        /// Update the Graphics filename list items : loads the content and display it.
        /// </summary>
        void OnRefreshAudioFilenameListItems(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            m_audioCurrentSelectionDir = e.Item.Text;
            m_musicPlayButton.Enabled = false;
            OnRefreshFilenameListItems(m_audioFilenames,
                m_audioImportButton, m_audioDeleteButton, e,
                new FileValidDelegate(IsValidAudioFile));
        }
        /// <summary>
        /// Returns true if the given file is a valid audio file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool IsValidAudioFile(string filename)
        {
            string ext = System.IO.Path.GetExtension(filename).Replace(".", "");
            return (Common.GeexSettings.SupportedAudioExtensions.Contains(ext));
        }
        #endregion

        #endregion
        /* ---------------------------------------------------------------------
         * Designer
         * --------------------------------------------------------------------*/
        #region Designer
        private void RessourcesManagerForm_Load(object sender, EventArgs e)
        {

        }
        public void Center()
        {
            this.CenterToParent();
        }
        #endregion
    }
}
