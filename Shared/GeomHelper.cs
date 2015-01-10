using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace UeFGame
{
    /// <summary>
    /// Helper class for geometry.
    /// </summary>
    public class GeomHelper
    {
        /// <summary>
        /// Rotates a point from the given center of rotation.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="pt"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 RotatePoint(Vector2 center, Vector2 pt, float angle)
        {
            if (pt.X == center.X && pt.Y == center.Y)
                return new Vector2(pt.X, pt.Y);
            // Angle par rapport à l'axe (Ox)
            float alpha = (float)Math.Atan((pt.Y - center.Y) / (pt.X - center.X));
            if (pt.X - center.X < 0)
                alpha += (float)Math.PI;
            // Distance between center and pt
            float dst = (float)Math.Sqrt((center.X - pt.X) * (center.X - pt.X) + (center.Y - pt.Y) * (center.Y - pt.Y));
            alpha += angle;
            // Coords of the new point.
            return new Vector2(center.X + dst*(float)Math.Cos(alpha),
                               center.Y + dst*(float)Math.Sin(alpha));
        }
        /// <summary>
        /// Returns the value of the the distance between 2 points.
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        public static float Distance(Vector2 pt1, Vector2 pt2)
        {
            return (float)Math.Sqrt((pt1.X - pt2.X) * (pt1.X - pt2.X) + (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y));
        }
    }
}
