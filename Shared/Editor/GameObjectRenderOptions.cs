using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.Editor
{
    /// <summary>
    /// Render options of the game objects in the editor.
    /// </summary>
    public class GameObjectRenderOptions
    {
        public bool IsSelected = false;
        public bool DrawOnTop
        {
            get { return IsSelected; }
        }
        public float Zoom;
    }
}
