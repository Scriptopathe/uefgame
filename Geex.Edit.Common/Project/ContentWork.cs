using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Construction;
using IEventSource = Microsoft.Build.Framework.IEventSource;

namespace Geex.Edit.Common.Project
{
    #region Logger
    /// <summary>
    /// Logger used to show the errors in the Geex.Edit.Common logger.
    /// </summary>
    class Logger : Microsoft.Build.Framework.ILogger
    {
        Tools.ErrorReporter m_reporter;
        public string Parameters { 
            get; 
            set; 
        }
        public Microsoft.Build.Framework.LoggerVerbosity Verbosity {
            get { return Microsoft.Build.Framework.LoggerVerbosity.Detailed; }
            set { }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public Logger(Tools.ErrorReporter reporter)
        {
            m_reporter = reporter;
        }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.ErrorRaised += new Microsoft.Build.Framework.BuildErrorEventHandler(eventSource_ErrorRaised);   
        }

        void eventSource_ErrorRaised(object sender, Microsoft.Build.Framework.BuildErrorEventArgs e)
        {
            m_reporter.Add(e.Message + " AT " + e.ProjectFile + " line :" + e.LineNumber);
        }

        public void Shutdown()
        {

        }
    }
    #endregion
    /// <summary>
    /// Instance used to work with content.
    ///  -- faire de cette classe une classe générique permettant la gestion des ressources en jeu.
    ///  -- créer un serveur de ressources (classe de base), permettant de récupérer toutes les ressources
    ///     du jeu en faisant des requêtes.
    ///  -- Objectifs :
    ///     - récupération unifiée des LISTES de ressources, 
    ///     - récupération unifiée des OBJETS ressources depuis les assets non compilés.
    ///     -
    /// </summary>
    public class ContentWork
    {
        /* -------------------------------------------------------------------
         * Variables
         * -----------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Reference to the content project.
        /// </summary>
        ProjectRootElement m_contentProject;
        /// <summary>
        /// Filename of the content project.
        /// </summary>
        string m_contentProjFilename;
        /// <summary>
        /// Filename of the "host" csproj
        /// </summary>
        string m_csProjFilename;
        #endregion
        /* -------------------------------------------------------------------
         * Methods
         * -----------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Starts the work operation.
        /// All the filenames of the GAME ressources MUST be retrieved through this class.
        /// </summary>
        public ContentWork(string csProjFilename)
        {
            ProjectRootElement csProj;
            // Opens the cs project
            csProj = ProjectRootElement.Open(csProjFilename);

            // Here, we look for a content project reference.
            string contentProjFilename = null;
            foreach (ProjectItemGroupElement group in csProj.ItemGroups)
            {
                foreach (ProjectItemElement item in group.Items)
                {
                    if (item.ItemType == "ProjectReference")
                        if (item.Include.Contains(".contentproj"))
                            contentProjFilename = System.IO.Path.GetDirectoryName(csProjFilename) + "\\" + item.Include;
                }
            }
            // Throw an exception if no content project reference was found.
            if (contentProjFilename == null)
                throw new Exception("No content file found");

            // Opens the content project
            m_contentProject = ProjectRootElement.Open(contentProjFilename);

            // Stores the filename
            m_contentProjFilename = contentProjFilename;
            m_csProjFilename = csProjFilename;

            // NB : the csProj is not saved as we won't use it for later use 
            // (we will use a different class to build the project).
        }
        /// <summary>
        /// Disposes the project.
        /// </summary>
        public void Dispose()
        {
            m_contentProject = null;
        }
        /// <summary>
        /// Saves the project file
        /// </summary>
        public void Save()
        {
            m_contentProject.Save();
        }
        /// <summary>
        /// Builds the content
        /// Returns true if the operation was successful.
        /// </summary>
        public bool Build()
        {
            // Compile the cs project.
            Microsoft.Build.Evaluation.Project csproj = new Microsoft.Build.Evaluation.Project(m_csProjFilename);
            // For the error reports
            Tools.ErrorReporter report = new Tools.ErrorReporter(Common.Globals.MainForm);
            Microsoft.Build.Framework.ILogger log = new Logger(report);
            // Builds the cs proj (and the linked content project)
            bool buildSuccess = csproj.Build(log);
            // Shows the error report.
            report.ShowErrors();
            return buildSuccess;
        }
        /// <summary>
        /// Adds a new element in the project.
        /// </summary>
        public void AddElement(string includePath, string importer, string processor, bool compile, bool copyToOutputDirectory)
        {
            string type = compile ? "Compile" : "None";
            Dictionary<string, string> item = new Dictionary<string, string>()
            {
                {"Name", System.IO.Path.GetFileNameWithoutExtension(includePath)},
                {"Importer", importer},
                {"Processor", processor}
            };
            // Adds the copy to output directory property.
            if (copyToOutputDirectory)
                item.Add("CopyToOutputDirectory", "PreserveNewest");

            // Vérifie que l'élément n'existe pas.
            bool exists = false;
            foreach (ProjectItemElement pitem in m_contentProject.Items)
            {
                if(pitem.Include == includePath)
                {
                    exists = true;
                    break;
                }
            }
            // On n'ajoute pas l'item s'il n'existe pas afin d'éviter les doublons.
            if(!exists)
                m_contentProject.AddItem(type, includePath, item);
        }
        /// <summary>
        /// Adds a new element in the project.
        /// </summary>
        public void AddElement(string includePath, string importer, string processor)
        {
            AddElement(includePath, importer, processor, true, false);
        }
        /// <summary>
        /// Deletes the element with the given include path.
        /// </summary>
        /// <param name="includePath"></param>
        public void DeleteElement(string includePath)
        {
            // Object which will be deleted after the iteration.
            ProjectItemElement toDelete = null;
            foreach (ProjectItemElement item in m_contentProject.Items)
            {
                if (item.Include == includePath)
                {
                    toDelete = item;
                    break;
                }
            }
            if (toDelete != null)
                toDelete.Parent.RemoveChild(toDelete);//m_contentProject.RemoveChild(toDelete); //.Items.Remove(toDelete);
            else
                throw new InvalidOperationException("The given element does not exist and can't be deleted");
        }
        /// <summary>
        /// Adds a new element in the project.
        /// Tries to determine which importer/processor should be used.
        /// </summary>
        /// <param name="includePath"></param>
        public void AddElement(string includePath)
        {
            string extension = System.IO.Path.GetExtension(includePath);
            string processor = "", importer = "";
            switch (extension)
            {
                case "png":
                case "jpg":
                case "jpeg":
                case "bmp":
                    importer = "TextureImporter";
                    processor = "TextureProcessor";
                    break;
                case "mp3":
                    importer = "Mp3Importer";
                    processor = "SongProcessor";
                    break;
                case "wav":
                    importer = "WavImporter";
                    processor = "SoundEffectProcessor";
                    break;
                case "spritefont":
                    importer = "FontDescriptionImporter";
                    processor = "FontDescriptionProcessor";
                    break;       
            }
            AddElement(includePath, importer, processor);
        }
        /// <summary>
        /// Gets the include filenames of content matching the given pattern.
        /// The include path is relative to the content root directory.
        /// </summary>
        /// <param name="searchPattern">A string to find in each element. May be a directory name.</param>
        /// <returns></returns>
        public List<string> GetElements(string searchPattern)
        {
            List<string> items = new List<string>();
            foreach (ProjectItemElement item in m_contentProject.Items)
            {
                if (item.ItemType == "Compile" || item.ItemType == "None")
                {
                    if (item.Include.Contains(searchPattern))
                    {
                        items.Add(item.Include);
                    }
                }
            }
            return items;
        }
        /// <summary>
        /// Gets the first include filename of content matching the given pattern.
        /// The include path is relative to the content root directory.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public string GetFirst(string searchPattern)
        {
            foreach (ProjectItemElement item in m_contentProject.Items)
            {
                if (item.ItemType == "Compile" || item.ItemType == "None")
                    if (item.Include.Contains(searchPattern))
                        return item.Include;
            }
            return null;
        }
        /// <summary>
        /// Gets the filenames (full, i.e. not starting from the content root)
        /// of content matching the given pattern.
        /// Returns them only if the elements exists.
        /// </summary>
        /// <param name="searchPattern">A string to find in each element. May be a directory name.</param>
        /// <returns></returns>
        public List<string> GetExistingElementsFilenames(string searchPattern)
        {
            List<string> items = new List<string>();
            foreach (ProjectItemElement item in m_contentProject.Items)
            {
                if (item.ItemType == "Compile" || item.ItemType == "None")
                {
                    if (item.Include.Contains(searchPattern))
                    {
                        string filename = System.IO.Path.GetDirectoryName(m_contentProjFilename) + "\\" +
                            item.Include;

                        if(System.IO.File.Exists(filename))
                            items.Add(filename);
                    }
                }
            }
            return items;
        }
        #endregion
    }
}
