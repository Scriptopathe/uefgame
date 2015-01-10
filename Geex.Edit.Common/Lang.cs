using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.Common
{
    /// <summary>
    /// Class used to get language information.
    /// </summary>
    public partial class Lang
    {
        /// <summary>Lang singleton instance</summary>
        public static Lang I;

        // Lang variables initialisation.
        #region LANG VARIABLES
        Dictionary<string, string> m_langDictionary = new Dictionary<string,string>();
        /// <summary>
        /// Returns the lang element with the given key.
        /// </summary>
        public string this[string v]
        {
            get 
            {
                if (m_langDictionary.ContainsKey(v))
                    return m_langDictionary[v];
                else
                    return v;
            }
            set { m_langDictionary[v] = value; }
        }
        #endregion
        // Contructor
        #region Constructor
        /// <summary>
        /// Contructor
        /// </summary>
        public Lang()
        {
            LoadDefault();
            I = this;
        }
        #endregion
        /// <summary>
        /// Loads the default values for all the variables
        /// </summary>
        public void LoadDefault()
        {
            #region Plugin choice form
            this["ModuleChoiceForm_RememberMyChoice"] = "Remember my choice";
            this["ModuleChoiceForm_Caption"] = "Choose a module";
            this["ModuleChoiceForm_LabelText"] = "Please select the module you want to use to edit this project.";
            #endregion

            #region Global
            this["Global_OK"] = "Ok";
            this["Global_Cancel"] = "Cancel";
            this["Global_Apply"] = "Apply";
            this["Global_None"] = "None";
            #endregion

            #region MapTreeView
            #region ContextMenu
            this["MapTreeView_ContextMenu_NewMap"] = "New map";
            this["MapTreeView_ContextMenu_DeleteMap"] = "Delete map";
            this["MapTreeView_ContextMenu_CopyMap"] = "Copy map";
            this["MapTreeView_ContextMenu_CutMap"] = "Cut map";
            this["MapTreeView_ContextMenu_PasteMap"] = "Paste";
            this["MapTreeView_ContextMenu_MapProperties"] = "Properties...";
            this["MapTreeView_ProjectRootName"] = "Project";
            #endregion
            #endregion

            #region MainFormMenus (MainFormManager)

            this["MainForm_Menu_File"] = "File";
            this["MainForm_MenuFile_Open"] = "Open...";
            this["MainForm_MenuFile_RecentProject"] = "Recent projects";
            #endregion

            #region Plugin load
            this["ProjectLoad_FileLoadFail_Text"] = "Failed to load file '#', it may be used by another program or may not exist";
            this["ProjectLoad_FileLoadFail_Caption"] = "Error";
            this["ProjectLoad_UnsupportedProjectType_Text"] = "Unsupported project type or version #. Check you have installed the needed ProjectPlugin.";
            this["ProjectLoad_UnsupportedProjectType_Caption"] = "Error : unsuported";
            #endregion

            #region Advanced Picturebox Panel
            this["AdvancedPictureBoxPanel_SatGroupbox"] = "Saturation";
            #endregion

            #region GraphicsParser
            this["GraphicsParser_FilesLabel"] = "Files";
            this["GraphicsParser_PreviewLabel"] = "Preview";
            this["GraphicsParser_Caption"] = "Choose a file";
            #endregion

            #region RessourcesManagerForm
            // Graphics
            this["RessourcesManagerForm_FilesLabel"] = "Files";
            this["RessourcesManagerForm_CategoriesLabel"] = "Categories";
            this["RessourcesManagerForm_DeleteButton"] = "Delete";
            this["RessourcesManagerForm_ImportButton"] = "Import...";
            this["RessourcesManagerForm_GraphicsTab"] = "Graphics";
            // Audio
            this["RessourcesManagerForm_AudioTab"] = "Audio";
            this["RessourcesManagerForm_MusicPlay"] = "Play";
            this["RessourcesManagerForm_MusicStop"] = "Stop";
            #endregion

        }
    }
}