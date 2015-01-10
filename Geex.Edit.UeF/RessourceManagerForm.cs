using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.UeF
{
    /// <summary>
    /// Form that enables the user to manages assets / ressources.
    /// </summary>
    class RessourcesManagerForm : Geex.Edit.Common.Tools.Controls.RessourcesManagerForm
    {
        protected override string[] GetAudioCategoriesList()
        {
            return new string[] { "Audio\\BGM" };
        }
        protected override string[] GetGraphicsCategoriesList()
        {
            return new string[] { "RunTimeAssets\\Graphics\\Tilesets", "RunTimeAssets\\Graphics\\GameObjects" };
        }
    }
}
