using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace Geex.Edit.Common.Tools
{
    /// <summary>
    /// Geometry Helper
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// Returns a rectangle created using 2 points
        /// </summary>
        public static Rectangle GetRect(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X) + 1, Math.Abs(p1.Y - p2.Y) + 1);
        }
    }
}
