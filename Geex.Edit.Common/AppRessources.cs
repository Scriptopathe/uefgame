using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Geex.Edit.Common
{
    /// <summary>
    /// Internal paths of the application.
    /// Provides access to ressources files used by Geex.Edit.
    /// </summary>
    public class AppRessources
    {
        /// <summary>
        /// Path to Geex options
        /// </summary>
        public static string GeexOptions = AppDomain.CurrentDomain.BaseDirectory + "Geex.Edit.Options.geexopt";
        /// <summary>
        /// Opens and returns the System.Drawing.Bitmap in the ressources/ folder of the editor.
        /// WARNING : These bitmaps are not cached ; use only for loading GUI Bitmaps for buttons,
        /// etc...
        /// </summary>
        public static System.Drawing.Bitmap RessourceSystemBitmap(string basename)
        {
            return new System.Drawing.Bitmap(AppDomain.CurrentDomain.BaseDirectory + "ressources\\" + basename);
        }
        /// <summary>
        /// Returns the ressource base directory, NOT followed by "\\".
        /// </summary>
        /// <returns></returns>
        public static string RessourceDir()
        {
            return (AppDomain.CurrentDomain.BaseDirectory + "ressources");
        }
        /// <summary>
        /// Returns the application base directory, followed by "\\"
        /// </summary>
        /// <returns></returns>
        public static string BaseDir()
        {
            return (AppDomain.CurrentDomain.BaseDirectory);
        }
        /// <summary>
        /// Returns the path to the lang file.
        /// </summary>
        /// <returns></returns>
        public static string Lang(string Language)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "Lang-" + Language + ".xml";
        }
    }
}