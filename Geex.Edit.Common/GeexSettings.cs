using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Geex.Edit.Common
{
    /// <summary>
    /// Handles the options the user can specify for the User interface of geex edit.
    /// </summary>
    public class GeexSettings
    {
        #region Constants
        /// <summary>
        /// Returns an array of string containing the name of all the supported bitmap extensions
        /// (i.e : bmp, png).
        /// </summary>
        public static readonly string[] SupportedBitmapExtensions = new string[] { "bmp", "png" };
        public static readonly string[] SupportedAudioExtensions = new string[] { "wav", "mp3" };
        const int RecentProjectsLimit = 10;
        #endregion

        #region Variables
        /// <summary>
        /// Path to the last opened project
        /// </summary>
        [XmlElement("RecentProjects")]
        List<string> m_recentProjects;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the RecentProjects list (readonly)
        /// </summary>
        public List<string> RecentProjects
        {
            get { return m_recentProjects; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a filename to the recent project list.
        /// </summary>
        /// <param name="filename"></param>
        public void AddRecentProject(string filename)
        {
            // Check for doublons.
            foreach (string fn in RecentProjects)
            {
                if (fn == filename)
                    return;
            }
            // Checks limit :
            if (RecentProjects.Count >= RecentProjectsLimit)
                m_recentProjects.RemoveAt(0);
            // Adds the file
            m_recentProjects.Add(filename);
        }
        #endregion

        #region Core Methods
        /// <summary>
        /// Constructor, does nothing.
        /// </summary>
        public GeexSettings()
        {
            m_recentProjects = new List<string>();
        }
        /// <summary>
        /// Loads the user options from file Options.geexopt
        /// </summary>
        public static GeexSettings Load()
        {
            if(System.IO.File.Exists(AppRessources.GeexOptions))
                return Tools.Serializer.Deserialize<GeexSettings>(AppRessources.GeexOptions, true);
            return new GeexSettings();
        }
        /// <summary>
        /// Saves the user options to Options.xml
        /// </summary>
        public void Save()
        {
            Tools.Serializer.Serialize<GeexSettings>(this, AppRessources.GeexOptions);
        }
        #endregion
    }
}
