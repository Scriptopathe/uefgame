using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame
{
    public static class DrawingDepths
    {
        /// <summary>
        /// Depths of the tile layers in the map.
        /// 0 : back
        /// 1 : front
        /// </summary>
        public static float[] MapLayerDepths = new float[] { 0.10f, 0.15f, 0.50f };
        public static float PlayerDepth = 0.21f;
        public static float DefaultEventDepth = 0.20f;
        public static float PanoramaDepth = 0.05f;
    }
}
