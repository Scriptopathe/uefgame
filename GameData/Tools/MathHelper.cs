using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace UeFGame.Tools
{
    /// <summary>
    /// Contient des fonctions mathématiques diverses.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Retourne la distance entre deux points.
        /// </summary>
        /// <returns></returns>
        public static float GetDistance(Point pt1, Point pt2)
        {
            return (float)Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2.0) + Math.Pow(pt1.Y - pt2.Y, 2.0));
        }

        /// <summary>
        /// Retourne la distance au carré entre deux points. (plus rapide)
        /// </summary>
        /// <returns></returns>
        public static float GetSquareDistance(Point pt1, Point pt2)
        {
            return (float)(Math.Pow(pt1.X - pt2.X, 2.0) + Math.Pow(pt1.Y - pt2.Y, 2.0));
        }
        /// <summary>
        /// Retourne une pseudo-distance entre deux points. Plus rapide, mais la valeur obtenue
        /// n'est pas une vraie estimation de la distance ni même une approximation.
        /// Ne peut être utilisé que pour du tri.
        /// </summary>
        /// <returns></returns>
        public static int GetFastDistance(Point pt1, Point pt2)
        {
            return (Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y));
        }
    }
}
