using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System.Threading;
namespace UeFGame.Tools
{
    /// <summary>
    /// Génère un polygone à partir de données de masque.
    /// </summary>
    public class PolygonGenerator
    {
        #region Méthodes finales
        /// <summary>
        /// Génère la liste de vertices à partir de la map.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<Vertices> GeneratePolygonsFromMap(GameComponents.MapInitializingData map)
        {
            int tilesetId = map.TilesetId;
            GameComponents.Tileset tileset = Ressources.FileRessourceProvider.LoadTileset(tilesetId);
            // System.Drawing.Bitmap texture = Ressources.FileRessourceLoader
            
            GameComponents.TilesetMask mask = Ressources.FileRessourceProvider.LoadTilesetMask(tilesetId);
            if (mask == null)
            {
                mask = GenerateMaskFromTexture(Ressources.FileRessourceProvider.LoadTilesetTexture(tileset.TextureName));
                Ressources.FileRessourceProvider.SaveTilesetMask(tilesetId, mask);
            }

            bool[,] mapMask = GenerateMapMask(map, mask);
            List<List<Point>> points = GenerateFromMask(ref mapMask);

            return ConvertToSimUnits(points);
        }
        /// <summary>
        /// Génère une liste de polygones (échelle : 1 unité = 1 pixel) à partir
        /// d'un masque. (false = rien, true = qqch).
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static List<List<Point>> GenerateFromMask(ref bool[,] mask)
        {
            // Timing
            DateTime start;
            start = DateTime.Now; var v1 = GetOutline(ref mask); 
            Console.WriteLine("-- Mask Generation : GetOutline (" + mask.GetLength(0).ToString() + "x" + mask.GetLength(1).ToString() + ") : " +
                (DateTime.Now - start).TotalMilliseconds.ToString() + "ms.");
            int count = v1.Count;

            start = DateTime.Now; var v2 = SortOutlinePoints(v1);
            Console.WriteLine("-- Mask Generation : SortOutlinePoints (" + count.ToString() + " pts) : " +
    (DateTime.Now - start).TotalMilliseconds.ToString() + "ms.");
            count = v2.Count;

            start = DateTime.Now; var ret = Optimize(v2);
            Console.WriteLine("-- Mask Generation : Optimize (" + count.ToString() + ") : " +
    (DateTime.Now - start).TotalMilliseconds.ToString() + "ms.");
            return ret;
            
            // return Optimize(SortOutlinePoints(GetOutline(ref mask)));
        }
        /// <summary>
        /// Alias de GenerateFromMask.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static List<List<Point>> GenerateFromMask(bool[,] mask)
        {
            // Timing
            DateTime start;
            start = DateTime.Now; var v1 = GetOutline(ref mask);
            Console.WriteLine("-- Mask Generation : GetOutline (" + mask.GetLength(0).ToString() + "x" + mask.GetLength(1).ToString() + ") : " +
                (DateTime.Now - start).TotalMilliseconds.ToString() + "ms.");
            int count = v1.Count;

            start = DateTime.Now; var v2 = SortOutlinePoints(v1);
            Console.WriteLine("-- Mask Generation : SortOutlinePoints (" + count.ToString() + " pts) : " +
    (DateTime.Now - start).TotalMilliseconds.ToString() + "ms.");
            count = v2.Count;

            start = DateTime.Now; var ret = Optimize(v2);
            Console.WriteLine("-- Mask Generation : Optimize (" + count.ToString() + ") : " +
    (DateTime.Now - start).TotalMilliseconds.ToString() + "ms.");
            return ret;

            // return Optimize(SortOutlinePoints(GetOutline(mask)));
        }
        #endregion
        /// <summary>
        /// Convertit la liste de liste de points en liste de vertices et en unités de simulation. (pas des pixels).
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<Vertices> ConvertToSimUnits(List<List<Point>> points)
        {
            List<Vertices> vertices = new List<Vertices>();
            foreach (List<Point> pts in points)
            {
                Vertices vert = new Vertices();
                foreach (Point pt in pts)
                {
                    vert.Add(new Vector2(UeFGame.ConvertUnits.ToSimUnits(pt.X), UeFGame.ConvertUnits.ToSimUnits(pt.Y)));
                }
                // Décomposition Bayazit en polygones convexess
                vertices.Add(vert);
                // vertices.AddRange(FarseerPhysics.Common.Decomposition.BayazitDecomposer.ConvexPartition(vert));
            }
            return vertices;
        }
        /// <summary>
        /// Génère un masque de la taille de la map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool[,] GenerateMapMask(GameComponents.MapInitializingData map, GameComponents.TilesetMask mask)
        {
            int[][,] ids = map.TileIds;
            bool[,] mapMask = new bool[ids[0].GetLength(0) * GameConstants.Tilesize, ids[1].GetLength(1) * GameConstants.Tilesize];

            // Largeur du tileset.
            int width = GameComponents.Tileset.TilesetWidthInTiles;

            for (int z = 0; z < ids.Count(); z++)
            {
                for (int x = 0; x < ids[z].GetLength(0); x++)
                {
                    for (int y = 0; y < ids[z].GetLength(1); y++)
                    {
                        int id = ids[z][x, y];

                        if (id != 0)
                        {
                            // Position de départ du masque à blitter
                            int sx = (id % width) * GameConstants.Tilesize;
                            int sy = (id / width) * GameConstants.Tilesize;

                            // Position où coller le masque
                            int dx = x * GameConstants.Tilesize;
                            int dy = y * GameConstants.Tilesize;

                            // Opération de collage
                            for (int px = 0; px < GameConstants.Tilesize; px++)
                            {
                                for (int py = 0; py < GameConstants.Tilesize; py++)
                                {
                                    mapMask[dx + px, dy + py] = mask.Raw[sx + px, sy + py];
                                }
                            }
                        }
                    }
                }
            }

            return mapMask;
        }

        #region  Génération de masque de tileset
        /// <summary>
        /// Génère une masque à partir d'une texture.
        /// Interprétation : 
        ///     - true : quelquechose
        ///     - false : rien
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static GameComponents.TilesetMask GenerateMaskFromTexture(System.Drawing.Bitmap texture)
        {
            bool[,] mask = new bool[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    mask[x, y] = texture.GetPixel(x, y).A > 5;
                }
            }
            GameComponents.TilesetMask tilesetmask = new GameComponents.TilesetMask();
            tilesetmask.Raw = mask;
            return tilesetmask;
        }

        public static GameComponents.TilesetMask GenerateMaskFromTexture2(System.Drawing.Bitmap texture)
        {
            bool[,] mask = new bool[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    mask[x, y] = texture.GetPixel(x, y).A > 5;
                }
            }
            GameComponents.TilesetMask tilesetmask = new GameComponents.TilesetMask();
            tilesetmask.Raw = mask;
            return tilesetmask;
        }
        #endregion



        #region GetOutline
        /// <summary>
        /// Récupère le contour d'un masque sous forme d'une liste de points.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static List<Point> GetOutline(ref bool[,] mask)
        {
            List<Point> outline = new List<Point>();
            for (int x = 0; x < mask.GetLength(0); x++)
            {
                for (int y = 0; y < mask.GetLength(1); y++)
                {
                    if (GetDiff(ref mask, x, y) != 0)
                        outline.Add(new Point(x, y));
                }
            }
            return outline;
        }
        /// <summary>
        /// Retourne 0 si ce pixel est identique à ceux qui l'entourent, sinon, 
        /// le nombre de pixel voisins différents est retourné.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static int GetDiff(ref bool[,] mask, int x, int y)
        {
            bool thisVal = mask[x, y];
            int diff = 0;

            // Si dans les bords droits, on gère différemment.
            if ((x == mask.GetLength(0) - 1) || (y == mask.GetLength(1) - 1))
            {
                for (int sx = 0; sx <= 1; sx++)
                {
                    for (int sy = 0; sy <= 1; sy++)
                    {
                        diff += thisVal != GetMaskValueAt(x + sx, y + sy, ref mask) ? 1 : 0;
                    }
                }
                return diff;
            }
            else if (x == 0)
            {
                for (int sx = -1; sx <= 1; sx++)
                {
                    for (int sy = 0; sy <= 1; sy++)
                    {
                        diff += thisVal != GetMaskValueAt(x + sx, y + sy, ref mask) ? 1 : 0;
                    }
                }
                return diff;
            }
            else if (y == 0)
            {
                for (int sx = 0; sx <= 1; sx++)
                {
                    for (int sy = -1; sy <= 1; sy++)
                    {
                        diff += thisVal != GetMaskValueAt(x + sx, y + sy, ref mask) ? 1 : 0;
                    }
                }
                return diff;
            }
            else
            {
                // On calcule le nombre de différences avec les voisins.
                for (int sx = 0; sx <= 1; sx++)
                {
                    for (int sy = 0; sy <= 1; sy++)
                    {
                        diff += thisVal != mask[x + sx, y + sy] ? 1 : 0;
                    }
                }
                return diff;
            }
        }
        /// <summary>
        /// Retourne la valeur du masque à l'endroit indiqué.
        /// Si elle existe pas, la remplace par du vide (true).
        /// </summary>
        static bool GetMaskValueAt(int x, int y, ref bool[,] mask)
        {
            if (x < 0 || y < 0 || x >= mask.GetLength(0) || y >= mask.GetLength(1))
                return false; // rien sur les bords
            return mask[x, y];
        }
        #endregion

        #region Sort / Optimize
        /// <summary>
        /// Distance maximale entre deux points d'un contour. Au dela de cette valeur, les deux points
        /// seront considérés comme faisant partie de deux contours différents.
        /// </summary>
        public const float MaxDistanceBetweenOutlinePts = 8.0f;

        /// <summary>
        /// Mets dans l'ordre les points du contour.
        /// Lorsque des points sont trop éloignés, une autre liste est générée, afin
        /// de permettre la création de plusieurs polygones.
        /// v2
        /// </summary>
        static List<List<Point>> SortOutlinePoints(List<Point> outlinePts)
        {
            List<List<Point>> list = new List<List<Point>>();
            List<Point> currentList = new List<Point>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            Point nearest;
            Point last = currentList.Last();
            int minDst;
            int maxIterations, minIterations, meanIterations, meanTotal;
            maxIterations = int.MinValue;
            minIterations = int.MaxValue;
            meanIterations = 0;
            meanTotal = outlinePts.Count;

            int i = 0;
            int nearestId = 0;
            int startId = 0;
            int iterations = 0;
            while (outlinePts.Count != 0)
            {
                // Initialisation
                minDst = int.MaxValue;
                nearest = Point.Zero;
                startId = Math.Max(0, nearestId-1);
                iterations = 0;
                
                // On commence par les points les plus probables :
                for (i = startId; i < outlinePts.Count; i++)
                {
                    iterations++;
                    Point pt = outlinePts[i];
                    // Pseudo-distance ne pouvant servir que pour du tri.
                    // 10x plus rapide qu'une vraie distance.
                    int dst = Math.Abs(pt.X - last.X) + Math.Abs(pt.Y - last.Y);
                    if (dst < minDst)
                    {
                        minDst = dst;
                        nearest = pt;
                        nearestId = i;
                        // Optimisation à l'aide d'une heuristique
                        if (minDst == 1)
                            break;
                    }
                }
                if (minDst != 1)
                {
                    for (i = startId-1; i >= 0; i--)
                    {
                        iterations++;
                        Point pt = outlinePts[i];
                        // Pseudo-distance ne pouvant servir que pour du tri.
                        // 10x plus rapide qu'une vraie distance.
                        int dst = Math.Abs(pt.X - last.X) + Math.Abs(pt.Y - last.Y);
                        if (dst < minDst)
                        {
                            minDst = dst;
                            nearest = pt;
                            nearestId = i;
                            // Optimisation à l'aide d'une heuristique
                            if (minDst == 1)
                                break;
                        }
                    }
                }

                if (iterations < minIterations)
                {
                    minIterations = iterations;
                }
                else if (iterations > maxIterations)
                {
                    maxIterations = iterations;
                }
                meanIterations += iterations;

                // Gestion multi-polygones.
                if (minDst >= MaxDistanceBetweenOutlinePts)
                {
                    list.Add(currentList);
                    currentList = new List<Point>();
                }

                // Ajout du nouveau point.
                currentList.Add(nearest);
                last = nearest;
                
                // On supprime le point de la liste.
                outlinePts.Remove(nearest);
            }

            if(currentList.Count != 0)
                list.Add(currentList);


            // Print stats :
            Console.WriteLine("---- Min Iterations : " + minIterations.ToString());
            Console.WriteLine("---- Max Iterations : " + maxIterations.ToString());
            Console.WriteLine("---- Mean Iterations : " + (meanIterations/meanTotal).ToString());
            return list;
        }
        /// <summary>
        /// Mets dans l'ordre les points du contour.
        /// Lorsque des points sont trop éloignés, une autre liste est générée, afin
        /// de permettre la création de plusieurs polygones.
        /// v1
        /// </summary>
        static List<List<Point>> SortOutlinePointsv1(List<Point> outlinePts)
        {
            List<List<Point>> list = new List<List<Point>>();
            List<Point> currentList = new List<Point>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            Point nearest;
            Point last = currentList.Last();
            int minDst;
            int iterations;
            long maxIterations, minIterations, meanIterations, meanTotal;
            maxIterations = int.MinValue;
            minIterations = int.MaxValue;
            meanIterations = 0;
            meanTotal = outlinePts.Count;

            while (outlinePts.Count != 0)
            {
                // Initialisation
                minDst = int.MaxValue;
                nearest = Point.Zero;
                iterations = 0;
                foreach (Point pt in outlinePts)
                {
                    iterations++;

                    // Pseudo-distance ne pouvant servir que pour du tri.
                    // 10x plus rapide qu'une vraie distance.
                    int dst = Math.Abs(pt.X - last.X) + Math.Abs(pt.Y - last.Y);
                    if (dst < minDst)
                    {
                        minDst = dst;
                        nearest = pt;

                        // Optimisation à l'aide d'une heuristique
                        if (minDst == 1)
                            break;
                    }

                }
                // Statistiques 
                if (iterations < minIterations)
                {
                    minIterations = iterations;
                }
                else if (iterations > maxIterations)
                {
                    maxIterations = iterations;
                }
                meanIterations += iterations;
                // ---

                // Gestion multi-polygones.
                if (minDst >= MaxDistanceBetweenOutlinePts)
                {
                    list.Add(currentList);
                    currentList = new List<Point>();
                }

                // Ajout du nouveau point.
                currentList.Add(nearest);
                last = nearest;

                // On supprime le point de la liste.
                outlinePts.Remove(nearest);
            }

            if (currentList.Count != 0)
                list.Add(currentList);

            // Print stats :
            Console.WriteLine("---- Min Iterations : " + minIterations.ToString());
            Console.WriteLine("---- Max Iterations : " + maxIterations.ToString());
            Console.WriteLine("---- Mean Iterations : " + (meanIterations / meanTotal).ToString());
            return list;
        }
        /// <summary>
        /// Mets dans l'ordre les points du contour.
        /// Lorsque des points sont trop éloignés, une autre liste est générée, afin
        /// de permettre la création de plusieurs polygones.
        /// </summary>
        static List<List<Point>> SortOutlinePointsBak(List<Point> outlinePts)
        {
            List<List<Point>> list = new List<List<Point>>();
            List<Point> currentList = new List<Point>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            Point nearest;
            Point last = currentList.Last();
            int minDst;
            while (outlinePts.Count != 0)
            {
                // Initialisation
                minDst = int.MaxValue;
                nearest = Point.Zero;

                foreach (Point pt in outlinePts)
                {
                    // Pseudo-distance ne pouvant servir que pour du tri.
                    // 10x plus rapide qu'une vraie distance.
                    int dst = Math.Abs(pt.X - last.X) + Math.Abs(pt.Y - last.Y);
                    if (dst < minDst)
                    {
                        minDst = dst;
                        nearest = pt;

                        // Optimisation à l'aide d'une heuristique
                        if (minDst == 1)
                            break;
                    }
                }

                // Gestion multi-polygones.
                if (minDst >= MaxDistanceBetweenOutlinePts)
                {
                    list.Add(currentList);
                    currentList = new List<Point>();
                }

                // Ajout du nouveau point.
                currentList.Add(nearest);
                last = nearest;

                // On supprime le point de la liste.
                outlinePts.Remove(nearest);
            }

            if (currentList.Count != 0)
                list.Add(currentList);

            return list;
        }
        /// <summary>
        /// Supprime des points non nécessaires en effectuant des approximations du contour.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        static List<List<Point>> Optimize(List<List<Point>> pts)
        {
            List<List<Point>> optimised = new List<List<Point>>();
            foreach(List<Point> list in pts)
            {
                var optimisedLst = Optimize(list);
                if(optimisedLst.Count <= 2)
                    continue;
                optimised.Add(optimisedLst);
            }
            return optimised;
        }
        /// <summary>
        /// Supprime des points non nécessaires en effectuant des approximations du contour.
        /// </summary>
        static List<Point> Optimize(List<Point> pts)
        {
            List<Point> newList = new List<Point>();
            Point startPoint = pts.First();
            newList.Add(startPoint);
            Vector3 lastEqn = new Vector3(-1, -1, -1);
            int linelenght = 1;
            for (int i = 1; i < pts.Count; i++)
            {
                linelenght++;
                // Si on vient de casser l'équation 
                if (lastEqn.X == 0 && lastEqn.Y == 0 && lastEqn.Z == 0)
                {
                    lastEqn = GetLineEquation(startPoint, pts[i]);
                    continue;
                }
                bool pointOnLine = EquationCheck(lastEqn, pts[i], linelenght);
                // Ajustage de la droite
                if (linelenght <= 8)
                {
                    if(linelenght == 8)
                        lastEqn = GetLineEquation(startPoint, pts[i]);
                    continue;
                }
                // Sinon, on vérifie que le point actuel vérifie l'équation, si c'est pas le cas on l'ajoute  :
                if (!pointOnLine)
                {
                    newList.Add(pts[i-1]);
                    startPoint = pts[i];
                    lastEqn = Vector3.Zero;
                    linelenght = 0;
                }
            }


            return newList;
        }
        /// <summary>
        /// Vérifie si le point donné est sur la droite donnée.
        /// </summary>
        /// <returns></returns>
        static bool EquationCheck(Vector3 line, Point pt, int serieLenght)
        {
            float abs = Math.Abs(line.X * pt.X + line.Y * pt.Y + line.Z);
            // Si ligne droite, tolérance 0.
            if (line.X == 0 || line.Y == 0)
            {
                return abs <= 0.8;
            }
            
            return abs <= (serieLenght > 16 ? (serieLenght > 32 ? 4 : 8) : 16);
        }

        /// <summary>
        /// Retourne l'équation de droite formée par les deux points passés en argument.
        /// aX + bY + c = 0
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Vector3 GetLineEquation(Point pt1, Point pt2)
        {
            // Cas particulier : 
            if (pt1.X == pt2.X)
            {
                // x = pt1.x
                int c = pt2.X;
                return new Vector3(1, 0, -c);
            }
            
            // Coef de y = ax + b
            float y = 1.0f;
            float ax = (float)(pt1.Y - pt2.Y) / (float)(pt1.X - pt2.X);
            float b = pt1.Y - ax * pt1.X;
            return new Vector3(-ax, y, -b);
        }
        #endregion

        #region SortOutlinePoints Multithread
        /// <summary>
        /// Mets dans l'ordre les points du contour.
        /// Lorsque des points sont trop éloignés, une autre liste est générée, afin
        /// de permettre la création de plusieurs polygones.
        /// (vieux et lent)
        /// </summary>
        static List<List<Point>> SortOutlinePoints1(List<Point> outlinePts)
        {
            List<List<Point>> list = new List<List<Point>>();
            List<Point> currentList = new List<Point>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            while (outlinePts.Count != 0)
            {
                float minDst = float.MaxValue;
                Point nearest = Point.Zero;
                Point last = currentList.Last();
                foreach (Point pt in outlinePts)
                {
                    // Pseudo-distance ne pouvant servir que pour du tri.
                    // 10x plus rapide qu'une vraie distance.
                    float dst = MathHelper.GetDistance(pt, last);
                    if (dst < minDst)
                    {
                        minDst = dst;
                        nearest = pt;
                    }
                }

                // Gestion multi-polygones.
                if (minDst >= MaxDistanceBetweenOutlinePts)
                {
                    list.Add(currentList);
                    currentList = new List<Point>();
                }

                // Ajout du nouveau point.
                currentList.Add(nearest);

                // On supprime le point de la liste.
                outlinePts.Remove(nearest);
            }

            if (currentList.Count != 0)
                list.Add(currentList);

            return list;
        }
        /// <summary>
        /// Mets dans l'ordre les points du contour.
        /// Lorsque des points sont trop éloignés, une autre liste est générée, afin
        /// de permettre la création de plusieurs polygones.
        /// </summary>
        static List<List<Point>> SortOutlinePointsParallel(List<Point> outlinePts)
        {
            List<List<Point>> list = new List<List<Point>>();
            List<Point> currentList = new List<Point>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            
            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            Point nearest;
            Point last = currentList.Last();
            int minDst;
            while (outlinePts.Count != 0)
            {
                // Initialisation
                minDst = int.MaxValue;
                nearest = Point.Zero;

                foreach (Point pt in outlinePts)
                {
                    // Pseudo-distance ne pouvant servir que pour du tri.
                    // 10x plus rapide qu'une vraie distance.
                    int dst = Math.Abs(pt.X - last.X) + Math.Abs(pt.Y - last.Y);
                    if (dst < minDst)
                    {
                        minDst = dst;
                        nearest = pt;

                        // Optimisation à l'aide d'une heuristique
                        if (minDst == 1)
                            break;
                    }
                }

                // Gestion multi-polygones.
                if (minDst >= MaxDistanceBetweenOutlinePts)
                {
                    list.Add(currentList);
                    currentList = new List<Point>();
                }

                // Ajout du nouveau point.
                currentList.Add(nearest);
                last = nearest;

                // On supprime le point de la liste.
                outlinePts.Remove(nearest);
            }

            if (currentList.Count != 0)
                list.Add(currentList);

            return list;
        }

        #endregion
    }
}
