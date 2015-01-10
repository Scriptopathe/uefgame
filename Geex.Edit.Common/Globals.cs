using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Geex.Edit.Common
{
    /// <summary>
    /// Class containing global instances.
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Random instance
        /// </summary>
        public static Random Rand = new Random();
        /// <summary>
        /// Manages the event inside the Xna context.
        /// </summary>
        public static Geex.Edit.Common.MapView.IMapView MapView;
        /// <summary>
        /// Manages the controls on the main window.
        /// </summary>
        public static Geex.Edit.Common.MainForm.IUIManager MainFormUIManager;
        /// <summary>
        /// Reference to the project class.
        /// </summary>
        public static Geex.Edit.Common.Project.GeexProject Project;
        /// <summary>
        /// Reference to the map tree view.
        /// </summary>
        public static Geex.Edit.Common.Tools.Controls.MapTreeView MapTreeView;
        /// <summary>
        /// Reference to the content work.
        /// </summary>
        public static Geex.Edit.Common.Project.ContentWork ContentWork
        {
            get { return Project.ContentWork; }
            set { Project.ContentWork = value; }
        }
        /// <summary>
        /// Main form
        /// </summary>
        public static MainForm.MainFormClass MainForm;
        /// <summary>
        /// Settings of the Geex application.
        /// </summary>
        public static GeexSettings Settings;
    }
}
