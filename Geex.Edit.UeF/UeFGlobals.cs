using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapDataWrapper = Geex.Edit.UeF.Project.MapDataWrapper;
using MapViewC = Geex.Edit.UeF.MapView.MapView;
using MapTreeView = Geex.Edit.UeF.MainForm.MapTreeView;
using ProjectC = Geex.Edit.UeF.Project.UeFGeexProject;
using Controler = Geex.Edit.UeF.MapView.Controler;
using Globals = Geex.Edit.Common.Globals;
namespace Geex.Edit.UeF
{
    public class UeFGlobals
    {
        static MapDataWrapper m_dataWrapper;
        static MapTreeView m_mapTreeView;
        static MapViewC m_mapView;
        static ProjectC m_project;
        static Controler m_controler;
        static TilePicker m_tilePicker;
        /// <summary>
        /// Initialize the globals : it avoids casting the object each time we want
        /// a reference to them.
        /// </summary>
        public static void Init()
        {
            m_mapTreeView = (MapTreeView)Globals.MapTreeView;
            m_mapView = (MapViewC)Globals.MapView;
            m_project = (ProjectC)Globals.Project;
            m_dataWrapper = (MapDataWrapper)m_mapView.MapDataWrapper;
            m_controler = (Controler)m_mapView.Controler;
            if (m_project != null && m_project.ContentWork == null)
            {
                m_project.ContentWork = new Common.Project.ContentWork(m_project.CurrentProjectDirectory + "\\" + m_project.CsProjectFilename);
                Common.Globals.ContentWork = ContentWork;
            }
            UeFGame.Ressources.FileRessourceProvider.ContentDir = m_project.ContentDirectory + "\\";
        }
        /// <summary>
        /// Returns the tile picker instance.
        /// </summary>
        public static TilePicker TilePicker
        {
            get { return m_tilePicker; }
            set { m_tilePicker = value; }
        }
        /// <summary>
        /// Gets the content work.
        /// </summary>
        public static Common.Project.ContentWork ContentWork
        {
            get
            {
                return Project.ContentWork;
            }
        }
        /// <summary>
        /// Shortcut to the MapDataWrapper object.
        /// </summary>
        public static MapDataWrapper MapDataWrapper
        {
            get
            {
                if (m_dataWrapper != null)
                    return m_dataWrapper;
                else
                {
                    m_dataWrapper = (MapDataWrapper)((MapViewC)Globals.MapView).MapDataWrapper;
                    return m_dataWrapper;
                }
            }
        }
        /// <summary>
        /// Shortcut to the MapTreeView object
        /// </summary>
        public static MapTreeView MapTreeView
        {
            get
            {
                if (m_mapTreeView != null)
                    return m_mapTreeView;
                else
                {
                    m_mapTreeView = (MapTreeView)Globals.MapTreeView;
                    return m_mapTreeView;
                }
            }
        }
        /// <summary>
        /// Shortcut to the MapView object
        /// </summary>
        public static MapViewC MapView
        {
            get
            {
                if (m_mapView != null)
                    return m_mapView;
                else
                {
                    m_mapView = (MapViewC)Globals.MapView;
                    return m_mapView;
                }
            }
        }
        /// <summary>
        /// Shortcut to the Controler object
        /// </summary>
        public static Controler Controler
        {
            get
            {
                if (m_controler != null)
                    return m_controler;
                else
                {
                    m_controler = (Controler)((MapViewC)Globals.MapView).Controler;
                    return m_controler;
                }
            }
        }
        /// <summary>
        /// Shortcut to the Project object
        /// Must be initialized first.
        /// </summary>
        public static ProjectC Project
        {
            get
            {
                if (m_project != null)
                    return m_project;
                else
                {
                    m_project = (ProjectC)Globals.Project;
                    return m_project;
                }
            }
        }
        /// <summary>
        /// Shortcut to the UIHandler object
        /// </summary>
        public static MainForm.UIHandler UIHandler
        {
            get
            {
                return (MainForm.UIHandler)(Common.Globals.MainFormUIManager);
            }
        }
    }
}
