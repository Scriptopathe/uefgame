using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidityState = Geex.Edit.Common.Project.ValidityState;
namespace Geex.Edit.UeF.Project
{
    /// <summary>
    /// Provides quick access to Game ressources.
    /// </summary>
    public class GameRessources
    {
        /// <summary>
        /// Gets the validity of a graphics file.
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static ValidityState GetGraphicsFileValidity(string fullpath)
        {
            if (System.IO.File.Exists(fullpath))
                if (Common.GeexSettings.SupportedBitmapExtensions.Contains(System.IO.Path.GetExtension(fullpath).Replace(".", "")))
                    return ValidityState.OK;
                else
                    return ValidityState.Invalid;
            else
                return ValidityState.DoesNotExist;
        }
        /// <summary>
        /// Gets the tileset filename given its id.
        /// /!\ It is only the filename without the containing folder.
        /// TODO
        /// </summary>
        /// <param name="tilesetId"></param>
        /// <returns></returns>
        public string GetTilesetFilename(int tilesetId)
        {
            return "";
        }
    }
}
