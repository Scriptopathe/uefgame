using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using Pen = System.Drawing.Pen;
using Brush = System.Drawing.Brush;
using System.Threading;
using MapImporter;
namespace UeFGame.Tools
{
    public enum OptimizationMode
    {
        /// <summary>
        /// Optimisation précise mais ne fonctionnant que dans les cas suivant :
        ///     - Les pentes ont des angles uniformes
        ///     - Les pentes ne sont pas directement suivies d'autres pentes d'angle différent.
        ///     - Pas d'arrondis.
        /// </summary>
        PreciseConstrained,
        UnpreciseUnconstrained,
        Ultimate,
    }
    public class PPoint
    {
        public int X;
        public int Y;
        public bool IsEssential = false;
        public PPoint() { }
        public PPoint(int x, int y) { X = x; Y = y; }
    }
    /// <summary>
    /// Génère un polygone à partir de données de masque.
    /// 
    /// TODO : gérer plusieurs algorithmes
    /// </summary>
    public class PolygonGenerator
    {
        const int CORES = 4;
        const int SOLID_LAYER = 1;
        #region Méthodes finales
        #region Generate Polygons From Map
 
        /// <summary>
        /// Génère la liste de vertices à partir de la map.
        /// v2
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<Vertices> GeneratePolygonsFromMap(GameComponents.MapInitializingData map, List<Rectangle> regions, 
            OptimizationMode mode = OptimizationMode.Ultimate)
        {
            int tilesetId = map.TilesetId;
            GameComponents.Tileset tileset = FileRessourceProvider.LoadTileset(tilesetId);
            // System.Drawing.Bitmap texture = Ressources.FileRessourceLoader
            GameComponents.TilesetMask mask = null;
            try
            {
                mask = FileRessourceProvider.LoadTilesetMask(tilesetId);
            }
            catch (Exception e) { }
            if (mask == null)
            {
                mask = GenerateMaskFromTexture(FileRessourceProvider.LoadTilesetTexture(tileset.TextureName));
                FileRessourceProvider.SaveTilesetMask(tilesetId, mask);
            }
            // -----------
            // Multithread
            object fullPointsLock = false;
            List<Vertices> fullPoints = new List<Vertices>();
            // Procédé de calcul qui sera répété sur plusieurs threads.
            ParameterizedThreadStart process = new ParameterizedThreadStart(delegate(object o)
            {
                List<Rectangle> regionsThisCore = (List<Rectangle>)o;
                foreach (Rectangle region in regionsThisCore)
                {
                    bool[,] mapMask = GenerateMapMask(map, mask, region);
                    List<List<PPoint>> points = GenerateFromMask(ref mapMask, region, mode);
                    lock (fullPointsLock)
                    {
                        fullPoints.AddRange(ConvertToSimUnits(points));
                    }
                }
            });

            // Distribution des régions à calculer dans les coeurs
            List<List<Rectangle>> regionsPerCore = new List<List<Rectangle>>();
            for (int core = 0; core < CORES; core++)
            {
                regionsPerCore.Add(new List<Rectangle>());
            }
            for(int i = 0; i < regions.Count; i++)
            {
                regionsPerCore[i % CORES].Add(regions[i]);
            }

            // Création des threads.
            List<Thread> threads = new List<Thread>();
            for (int core = 0; core < CORES; core++)
            {
                Thread thread = new Thread(process);
                thread.Start(regionsPerCore[core]);
                threads.Add(thread);
            }

            // On patiente pour que tous les cores aient fini.
            bool allEnded = false;
            while (!allEnded)
            {
                allEnded = true;
                for (int core = 0; core < CORES; core++)
                {
                    if (threads[core].ThreadState != ThreadState.Stopped)
                    {
                        allEnded = false;
                        break;
                    }
                }
                Thread.Sleep(5);
            }

            map.Polygons = fullPoints;
            DEBUG_DrawMap(map);
            return fullPoints;
        }
        /// <summary>
        /// Génère la liste de vertices à partir de la map.
        /// v1
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<Vertices> GeneratePolygonsFromMap(GameComponents.MapInitializingData map, OptimizationMode mode = OptimizationMode.UnpreciseUnconstrained)
        {
            int tilesetId = map.TilesetId;
            GameComponents.Tileset tileset = FileRessourceProvider.LoadTileset(tilesetId);
            // System.Drawing.Bitmap texture = Ressources.FileRessourceLoader
            
            GameComponents.TilesetMask mask = FileRessourceProvider.LoadTilesetMask(tilesetId);
            if (mask == null)
            {
                mask = GenerateMaskFromTexture(FileRessourceProvider.LoadTilesetTexture(tileset.TextureName));
                FileRessourceProvider.SaveTilesetMask(tilesetId, mask);
            }

            bool[,] mapMask = GenerateMapMask(map, mask);
            List<List<PPoint>> points = GenerateFromMask(ref mapMask, mode);

            return ConvertToSimUnits(points);
        }
        #endregion
        /// <summary>
        /// Génère une liste de polygones (échelle : 1 unité = 1 pixel) à partir
        /// d'un masque. (false = rien, true = qqch).
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static List<List<PPoint>> GenerateFromMask(ref bool[,] mask, Rectangle clipRect, OptimizationMode mode)
        {
            //return Optimize(SortOutlinePoints(GetOutline(ref mask, clipRect)));
            return Optimize(SortOutlinePoints(GetOutline(ref mask, clipRect)), ref mask, clipRect, mode);
        }
        /// <summary>
        /// Génère une liste de polygones (échelle : 1 unité = 1 pixel) à partir
        /// d'un masque. (false = rien, true = qqch).
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static List<List<PPoint>> GenerateFromMask(ref bool[,] mask, OptimizationMode mode)
        {
            return Optimize(SortOutlinePoints(GetOutline(ref mask)), ref mask, new Rectangle(0, 0, mask.GetLength(0), mask.GetLength(1)), mode);
        }
        /// <summary>
        /// Alias de GenerateFromMask.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static List<List<PPoint>> GenerateFromMask(bool[,] mask)
        {
            throw new Exception();
            //return Optimize(SortOutlinePoints(GetOutline(ref mask)));
        }
        #endregion
        /// <summary>
        /// Convertit la liste de liste de points en liste de vertices et en unités de simulation. (pas des pixels).
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<Vertices> ConvertToSimUnits(List<List<PPoint>> points)
        {
            List<Vertices> vertices = new List<Vertices>();
            foreach (List<PPoint> pts in points)
            {
                Vertices vert = new Vertices();
                foreach (PPoint pt in pts)
                {
                    //vert.Add(new Vector2(pt.X, pt.Y));
                    vert.Add(new Vector2(UeFGame.ConvertUnits.ToSimUnits(pt.X), UeFGame.ConvertUnits.ToSimUnits(pt.Y)));
                }
                // Décomposition Bayazit en polygones convexess

                try
                {
                    if (vert.Count >= 3)
                        //vertices.Add(vert);
                        vertices.AddRange(FarseerPhysics.Common.Decomposition.BayazitDecomposer.ConvexPartition(vert));
                }
                catch (Exception e)
                {
                    //vertices.AddRange(vert);
                }
            }
            return vertices;
        }

        #region Generate Map Mask
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

                        if (id != -1)
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
                                    mapMask[dx + px, dy + py] =  mapMask[dx + px, dy + py] == false ? mask.Raw[sx + px, sy + py] : true;
                                }
                            }
                        }
                    }
                }
            }
            return mapMask;
        }

        /// <summary>
        /// Génère un masque du rectangle donné de la map.
        /// Rect : en cases.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool[,] GenerateMapMask(GameComponents.MapInitializingData map, GameComponents.TilesetMask mask, Rectangle rect)
        {
            int[][,] ids = map.TileIds;
            bool[,] mapMask = new bool[(rect.Width)*GameConstants.Tilesize, (rect.Height)*GameConstants.Tilesize];

            // Largeur du tileset.
            int width = GameComponents.Tileset.TilesetWidthInTiles;

            for (int z = 0; z < ids.Count(); z++)
            {
                for (int x = rect.X; x < rect.Right; x++)
                {
                    for (int y = rect.Y; y < rect.Bottom; y++)
                    {
                        int id = ids[z][x, y];

                        if (id != -1)
                        {
                            // Position de départ du masque à blitter
                            int sx = (id % width) * GameConstants.Tilesize;
                            int sy = (id / width) * GameConstants.Tilesize;

                            // Position où coller le masque
                            int dx = (x-rect.X) * GameConstants.Tilesize;
                            int dy = (y-rect.Y) * GameConstants.Tilesize;

                            // Opération de collage
                            for (int px = 0; px < GameConstants.Tilesize; px++)
                            {
                                for (int py = 0; py < GameConstants.Tilesize; py++)
                                {
                                    mapMask[dx + px, dy + py] = mapMask[dx + px, dy + py] == false ? mask.Raw[sx + px, sy + py] : true;
                                }
                            }
                        }
                    }
                }
            }
            // Largeur du tileset.
            /*int width = GameComponents.Tileset.TilesetWidthInTiles;
            int startX = rect.X / 32;
            int startY = rect.Y / 32;
            for (int z = 0; z < ids.Count(); z++)
            {
                for (int x = startX; x < (rect.Right/32); x++)
                {
                    for (int y = startY; y < (rect.Bottom / 32); y++)
                    {
                        int id = ids[z][x, y];

                        if (id != 0)
                        {
                            // Position de départ du masque à blitter
                            int sx = (id % width) * GameConstants.Tilesize;
                            int sy = (id / width) * GameConstants.Tilesize;

                            // Position où coller le masque
                            int dx = Math.Max((x-startX) * GameConstants.Tilesize, rect.X);
                            int dy = Math.Max((y-startY) * GameConstants.Tilesize, rect.Y);

                            // Opération de collage
                            for (int px = dx; px < Math.Min(dx + GameConstants.Tilesize, rect.Right); px++)
                            {
                                for (int py = dy; py < Math.Min(dy + GameConstants.Tilesize, rect.Bottom); py++)
                                {
                                    mapMask[px-rect.X, py-rect.Y] = mask.Raw[sx + px - dx, sy + py - dy];
                                }
                            }
                        }
                    }
                }
            }
            */
            return mapMask;
        }
        #endregion

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
        public static List<PPoint> GetOutline(ref bool[,] mask, Rectangle clipRect)
        {
            List<PPoint> outline = new List<PPoint>();
            for (int x = 0; x < mask.GetLength(0); x++)
            {
                for (int y = 0; y < mask.GetLength(1); y++)
                {
                    if (GetDiff(ref mask, x, y) != 0)
                        outline.Add(new PPoint(x+clipRect.X*GameConstants.Tilesize, y+clipRect.Y*GameConstants.Tilesize));
                }
            }
            return outline;
        }
        /// <summary>
        /// Récupère le contour d'un masque sous forme d'une liste de points.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static List<PPoint> GetOutline(ref bool[,] mask)
        {
            List<PPoint> outline = new List<PPoint>();
            for (int x = 0; x < mask.GetLength(0); x++)
            {
                for (int y = 0; y < mask.GetLength(1); y++)
                {
                    if (GetDiff(ref mask, x, y) != 0)
                        outline.Add(new PPoint(x, y));
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
            bool thisVal = GetMaskValueAt(x, y, ref mask);
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
                        diff += thisVal != GetMaskValueAt(x + sx, y + sy, ref mask) ? 1 : 0;
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

        #region Sort
        /// <summary>
        /// Distance maximale entre deux points d'un contour. Au dela de cette valeur, les deux points
        /// seront considérés comme faisant partie de deux contours différents.
        /// </summary>
        public const float MaxDistanceBetweenOutlinePts = 8.0f;
        /// <summary>
        /// Mets dans l'ordre les points du contour.
        /// Lorsque des points sont trop éloignés, une autre liste est générée, afin
        /// de permettre la création de plusieurs polygones.
        /// v2 + stats
        /// </summary>
        static List<List<PPoint>> SortOutlinePoints(List<PPoint> outlinePts)
        {
            if (outlinePts.Count == 0)
                return new List<List<PPoint>>() { outlinePts };

            List<List<PPoint>> list = new List<List<PPoint>>();
            List<PPoint> currentList = new List<PPoint>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            PPoint nearest;
            PPoint last = currentList.Last();
            int minDst;

            int i = 0;
            int nearestId = 0;
            int startId = 0;
            while (outlinePts.Count != 0)
            {
                // Initialisation
                minDst = int.MaxValue;
                nearest = new PPoint(0, 0);
                startId = Math.Max(0, nearestId - 1);

                // On commence par les points les plus probables :
                for (i = startId; i < outlinePts.Count; i++)
                {
                    PPoint pt = outlinePts[i];
                    // Pseudo-distance ne pouvant servir que pour du tri.
                    // 10x plus rapide qu'une vraie distance.
                    //int dst = Math.Abs(pt.X - last.X) + Math.Abs(pt.Y - last.Y);
                    int dst = Math.Abs(pt.X - last.X) + Math.Abs(last.Y - pt.Y);
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
                    for (i = startId - 1; i >= 0; i--)
                    {
                        PPoint pt = outlinePts[i];
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

                // Gestion multi-polygones.
                if (minDst >= MaxDistanceBetweenOutlinePts)
                {
                    list.Add(currentList);
                    currentList = new List<PPoint>();
                }

                // Ajout du nouveau PPoint.
                currentList.Add(nearest);
                last = nearest;

                // On supprime le PPoint de la liste.
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
        /// v2 + stats
        /// </summary>
        static List<List<PPoint>> SortOutlinePointsV2Stats(List<PPoint> outlinePts)
        {
            List<List<PPoint>> list = new List<List<PPoint>>();
            List<PPoint> currentList = new List<PPoint>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            PPoint nearest;
            PPoint last = currentList.Last();
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
                nearest = new PPoint(0, 0);
                startId = Math.Max(0, nearestId-1);
                iterations = 0;
                
                // On commence par les points les plus probables :
                for (i = startId; i < outlinePts.Count; i++)
                {
                    iterations++;
                    PPoint pt = outlinePts[i];
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
                        PPoint pt = outlinePts[i];
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
                    currentList = new List<PPoint>();
                }

                // Ajout du nouveau PPoint.
                currentList.Add(nearest);
                last = nearest;
                
                // On supprime le PPoint de la liste.
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
        /// v1 + stats
        /// </summary>
        static List<List<PPoint>> SortOutlinePointsv1(List<PPoint> outlinePts)
        {
            List<List<PPoint>> list = new List<List<PPoint>>();
            List<PPoint> currentList = new List<PPoint>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            PPoint nearest;
            PPoint last = currentList.Last();
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
                nearest = new PPoint(0, 0);
                iterations = 0;
                foreach (PPoint pt in outlinePts)
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
                    currentList = new List<PPoint>();
                }

                // Ajout du nouveau PPoint.
                currentList.Add(nearest);
                last = nearest;

                // On supprime le PPoint de la liste.
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
        /// v1
        /// </summary>
        static List<List<PPoint>> SortOutlinePointsBak(List<PPoint> outlinePts)
        {
            List<List<PPoint>> list = new List<List<PPoint>>();
            List<PPoint> currentList = new List<PPoint>();
            currentList.Add(outlinePts.First());
            outlinePts.Remove(outlinePts.First());

            // Attention, très long !
            // Mets les points dans currentList, de proche en proche.
            // Si deux points sont à trop grande distance, on crée une nouvelle liste
            // qui est un contour de polygone différent.
            PPoint nearest;
            PPoint last = currentList.Last();
            int minDst;
            while (outlinePts.Count != 0)
            {
                // Initialisation
                minDst = int.MaxValue;
                nearest = new PPoint(0, 0);

                foreach (PPoint pt in outlinePts)
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
                    currentList = new List<PPoint>();
                }

                // Ajout du nouveau PPoint.
                currentList.Add(nearest);
                last = nearest;

                // On supprime le PPoint de la liste.
                outlinePts.Remove(nearest);
            }

            if (currentList.Count != 0)
                list.Add(currentList);

            return list;
        }
        #endregion

        #region Optimize
        /// <summary>
        /// Supprime des points non nécessaires en effectuant des approximations du contour.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        static List<List<PPoint>> Optimize(List<List<PPoint>> pts, ref bool[,] mask, Rectangle clipRect, OptimizationMode mode)
        {
            List<List<PPoint>> optimised = new List<List<PPoint>>();
            foreach(List<PPoint> list in pts)
            {
                List<PPoint> optimisedLst;

                // Optimisation faite selon le mode choisi.
                switch (mode)
                {

                    case OptimizationMode.PreciseConstrained:
                        optimisedLst = OptimizePreciseConstrained(list, ref mask, clipRect);
                        break;
                    case OptimizationMode.UnpreciseUnconstrained:
                        optimisedLst = OptimizeUnpreciseUnconstrained(list);
                        break;
                    case OptimizationMode.Ultimate:
                        optimisedLst = OptimizeUltimate(list, ref mask, clipRect);
                        break;
                    default:
                        optimisedLst = new List<PPoint>();
                        break;
                }
                
                if(optimisedLst.Count > 2)
                    optimised.Add(optimisedLst);
            }
            return optimised;
        }
        const int maxTol = 64;
        const int TileSize = 32;
        #region Optimize Ultimate

        /// <summary>
        /// Méthode d'optimisation combinée + lente mais + précise.
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="mask"></param>
        /// <param name="clipRect"></param>
        /// <returns></returns>
        static List<PPoint> OptimizeUltimate(List<PPoint> pts, ref bool[,] mask, Rectangle clipRect)
        {
            // Marks key points as essential.
            var essential = GetEssential(pts, ref mask, clipRect);
            foreach (PPoint pt in essential) { pt.IsEssential = true; }

            var newPts = PerformAngleSelection(OptimizeUnpreciseUnconstrained(pts));
            return newPts;
        }

        /// <summary>
        /// Fait une dernière sélection en éléminant les points entre deux segments de même "angle".
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        static List<PPoint> PerformAngleSelection(List<PPoint> pts)
        {
            if (pts.Count <= 2)
                return pts;

            List<PPoint> newList = new List<PPoint>();
            newList.Add(pts.First());
            float prevLineAngle = PsAngle(pts[0], pts[1]);
            for (int i = 1; i < pts.Count - 1; i++)
            {
                float newLineAngle = PsAngle(pts[i], pts[i+1]);
                float diff = Math.Abs(newLineAngle - prevLineAngle);
                if (diff >= 0.15f)
                    newList.Add(pts[i]);
                prevLineAngle = newLineAngle;
            }
            newList.Add(pts.Last());
            return newList;
        }
        /// <summary>
        /// Retourne "l'angle" entre les point donnés.
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        static float PsAngle(PPoint pt1, PPoint pt2)
        {
            return (float)Math.Atan((pt1.Y - pt2.Y) / (Math.Max(pt1.X - pt2.X, 0.00001f)));
        }
        /// <summary>
        /// Permet l'obtention de "points clef" à ne pas oublier.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        static List<PPoint> GetEssential(List<PPoint> pts, ref bool[,] mask, Rectangle clipRect)
        {
            if (pts.Count == 0)
                return pts;

            // Première étape : lignes droites
            List<PPoint> newList = new List<PPoint>();
            newList.Add(pts.First());
            int currentDiff = GetDiff(ref mask, pts[0].X - clipRect.X * TileSize, pts[0].Y - clipRect.Y * TileSize);
            for (int i = 1; i < pts.Count - 1; i++)
            {
                int diff = (GetDiff(ref mask, pts[i].X - clipRect.X * TileSize, pts[i].Y - clipRect.Y * TileSize));

                int d = Math.Abs(currentDiff - diff);
                if (diff != currentDiff || currentDiff == 1 && (pts[i].X >= clipRect.Right * TileSize || pts[i].Y >= clipRect.Bottom * TileSize))
                {
                    newList.Add(pts[i]);
                    currentDiff = diff;
                }

            }
            newList.Add(pts.Last());

            List<PPoint> final = new List<PPoint>();
            final.Add(newList.First());
            for (int i = 1; i < newList.Count - 1; i++)
            {
                int dstPrev = Math.Abs(newList[i].X - newList[i - 1].X) + Math.Abs(newList[i].Y - newList[i - 1].Y);
                int dstSuiv = Math.Abs(newList[i].X - newList[i + 1].X) + Math.Abs(newList[i].Y - newList[i + 1].Y);
                if (dstPrev >= 8 || dstSuiv >= 8)
                    final.Add(newList[i]);
            }
            final.Add(newList.Last());
            return final;
        }
        #endregion
        /// <summary>
        /// Méthode d'optimisation précise mais ne fonctionnant que dans les conditions suivantes :
        ///     - Les pentes ont un angle uniforme et ne sont pas suivies d'autres pentes.
        ///     - Les arrondis etc ne sont pas autorisés.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        static List<PPoint> OptimizePreciseConstrained(List<PPoint> pts, ref bool[,] mask, Rectangle clipRect)
        {
            return GetEssential(pts, ref mask, clipRect);
            /*if (pts.Count == 0)
                return pts;
            
            // Première étape : lignes droites
            List<PPoint> newList = new List<PPoint>();
            newList.Add(pts.First());
            int currentDiff = GetDiff(ref mask, pts[0].X - clipRect.X * TileSize, pts[0].Y - clipRect.Y * TileSize);
            for (int i = 1; i < pts.Count - 1; i++)
            {
                int diff = (GetDiff(ref mask, pts[i].X - clipRect.X*TileSize, pts[i].Y - clipRect.Y*TileSize));

                int d = Math.Abs(currentDiff - diff);
                if (diff != currentDiff || currentDiff == 1 && (pts[i].X >= clipRect.Right*TileSize || pts[i].Y >= clipRect.Bottom*TileSize))
                {
                    newList.Add(pts[i]);
                    currentDiff = diff;
                }

            }
            newList.Add(pts.Last());
            return newList;//OptimizePreciseConstrainedPass2(newList);*/
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        static List<PPoint> OptimizePreciseConstrainedPass2(List<PPoint> pts)
        {
            List<PPoint> newList = new List<PPoint>();
            PPoint lastPt = pts.First();
            //bool isInterpolating = false;
            Vector3 lastEqn = new Vector3(0, 0, 0);
            //bool isStartingInterpolation = false;
            PPoint firstInterpolationPt = pts.First();
            newList.Add(pts.First());
            for(int i = 1; i < pts.Count - 1; i++)
            {
                PPoint pt = pts[i];

                int dst = Math.Abs(pt.X - lastPt.X) + Math.Abs(pt.Y - lastPt.Y);
                int dst2 = Math.Abs(pt.X - pts[i + 1].X) + Math.Abs(pt.Y - pts[i + 1].Y);
                if (dst >= 8 || dst2 >= 8)
                    newList.Add(pt);
                lastPt = pt;
                /*bool add = false;
                int dst = Math.Abs(pt.X - lastPt.X) + Math.Abs(pt.Y - lastPt.Y);

                bool oldInterpolating = isInterpolating;
                // Détermine si une interpolation est nécessaire.
                if (dst <= 4)
                    isInterpolating = true;
                else
                    isInterpolating = false;

                if (!isInterpolating)
                {
                    add = true;
                    isStartingInterpolation = false;
                }
                else
                {
                    // Première phase de l'interpolation
                    isStartingInterpolation = isInterpolating && !oldInterpolating;

                    if (isStartingInterpolation)
                    {
                        // On vérifie que les 32 points soient des points "serrés" avant d'interpoler jusqu'au 32e.
                        int j; // à la sortie de la boucle, représente l'avancement possible
                        for (j = i+1; j < Math.Min(i + 32, pts.Count); j++)
                        {
                            int dst2 = Math.Abs(pts[j].X - pts[j-1].X) + Math.Abs(pts[j].Y - pts[j-1].Y);
                            if (dst >= 4)
                                break;
                        }

                        // Si on a de la place pour une droite alignant 32 points, on y go
                        int oldI = i;
                        i = Math.Min(j-1, pts.Count - 1);
                        lastEqn = GetLineEquation(pts[oldI], pts[i]);
                        firstInterpolationPt = pts[oldI];
                        add = false;
                        newList.Add(pts[j-1]);
                    }
                    else
                    {
                        // L'interpolation a déja démarré
                        if (EquationCheck(lastEqn, pts[i], 32))
                            lastEqn = GetLineEquation(firstInterpolationPt, pts[i]);
                        else
                        {
                            // On arrête l'interpolation
                            add = true;
                            isStartingInterpolation = false;
                            isInterpolating = false;
                            firstInterpolationPt = pts[i];
                        }

                    }
                }

                lastPt = pt;
                if(add || i == pts.Count - 1)
                    newList.Add(pt);*/
            }
            newList.Add(pts.Last());
            return newList;
        }
        /// <summary>
        /// Supprime des points non nécessaires en effectuant des approximations du contour.
        /// </summary>
        static List<PPoint> OptimizeUnpreciseUnconstrained(List<PPoint> pts)
        {
            if (pts.Count == 0)
                return pts;

            List<PPoint> newList = new List<PPoint>();
            PPoint startPoint = pts.First();
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
                    if (linelenght == 8)
                        lastEqn = GetLineEquation(startPoint, pts[i]);
                    continue;
                }
                // Sinon, on vérifie que le PPoint actuel vérifie l'équation, si c'est pas le cas on l'ajoute  :
                if (!pointOnLine)
                {
                    newList.Add(pts[i - 1]);
                    startPoint = pts[i];
                    lastEqn = Vector3.Zero;
                    linelenght = 0;
                }
                else if (pts[i - 1].IsEssential)
                    newList.Add(pts[i - 1]);
            }


            return newList;
        }

        /// <summary>
        /// Vérifie si le PPoint donné est sur la droite donnée.
        /// </summary>
        /// <returns></returns>
        static bool EquationCheck(Vector3 line, PPoint pt, int serieLenght)
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
        public static Vector3 GetLineEquation(PPoint pt1, PPoint pt2)
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


        #region DEBUG
        /// <summary>
        /// Génère la liste de vertices à partir de la map.
        /// v2 [DEBUG]
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<Vertices> DEBUG_GeneratePolygonsFromMap(GameComponents.MapInitializingData map, List<Rectangle> regions, OptimizationMode mode)
        {
            int tilesetId = map.TilesetId;
            GameComponents.Tileset tileset = FileRessourceProvider.LoadTileset(tilesetId);
            // System.Drawing.Bitmap texture = Ressources.FileRessourceLoader
            GameComponents.TilesetMask mask = null;
            try
            {
                mask = FileRessourceProvider.LoadTilesetMask(tilesetId);
            }
            catch (Exception e) { }
            if (mask == null)
            {
                mask = GenerateMaskFromTexture(FileRessourceProvider.LoadTilesetTexture(tileset.TextureName));
                FileRessourceProvider.SaveTilesetMask(tilesetId, mask);
                /* DEBUG 
 */
            }
            // -----------
            // Multithread
            object fullPointsLock = false;
            List<Vertices> fullPoints = new List<Vertices>();
            // Procédé de calcul qui sera répété sur plusieurs threads.
            ParameterizedThreadStart process = new ParameterizedThreadStart(delegate(object o)
            {
                List<Rectangle> regionsThisCore = (List<Rectangle>)o;
                foreach (Rectangle region in regionsThisCore)
                {
                    bool[,] mapMask = GenerateMapMask(map, mask, region);
                    List<List<PPoint>> points = GenerateFromMask(ref mapMask, region, mode);
                    lock (fullPointsLock)
                    {
                        fullPoints.AddRange(ConvertToSimUnits(points));
                    }
                }
            });

            // Distribution des régions à calculer dans les coeurs
            List<List<Rectangle>> regionsPerCore = new List<List<Rectangle>>();
            for (int core = 0; core < CORES; core++)
            {
                regionsPerCore.Add(new List<Rectangle>());
            }
            for (int i = 0; i < regions.Count; i++)
            {
                regionsPerCore[i % CORES].Add(regions[i]);
            }

            // Création des threads.
            List<Thread> threads = new List<Thread>();
            for (int core = 0; core < CORES; core++)
            {
                Thread thread = new Thread(process);
                thread.Start(regionsPerCore[core]);
                threads.Add(thread);
            }

            // On patiente pour que tous les cores aient fini.
            bool allEnded = false;
            while (!allEnded)
            {
                allEnded = true;
                for (int core = 0; core < CORES; core++)
                {
                    if (threads[core].ThreadState != ThreadState.Stopped)
                    {
                        allEnded = false;
                        break;
                    }
                }
                Thread.Sleep(5);
            }
            // ----------
            /*
            foreach (Rectangle region in regions)
            {
                bool[,] mapMask = GenerateMapMask(map, mask, region);
                List<List<PPoint>> points = GenerateFromMask(ref mapMask, region);
                fullPoints.AddRange(ConvertToSimUnits(points));
            }*/
            /* DEBUG */
            map.Polygons = fullPoints;
            DEBUG_DrawMap(map);
            bool[,] maskk = GenerateMapMask(map, mask);
            System.IO.File.AppendAllText("generation_log.txt", "-- Polygons : " + fullPoints.Count.ToString() + "\r\n");
            DEBUG_DrawMask(maskk, "MASQUEMAP");
            // */
            return fullPoints;
        }

        static void DEBUG_DrawMask(bool[,] mask, string name)
        {

            // Snapshot du masque du tileset
            Bitmap bmp = new Bitmap(mask.GetLength(0), mask.GetLength(1));
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    bmp.SetPixel(x, y, mask[x, y] ? System.Drawing.Color.Black : System.Drawing.Color.White);
                }
            }
            bmp.Save(FileRessourceProvider.ContentDir + "RunTimeAssets\\Data\\Maps\\Snapshot" + name + "-TilesetMask.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        static void DEBUG_DrawMap(GameComponents.MapInitializingData Map)
        {
            Bitmap bmp = new Bitmap((int)(Map.SizeInTiles.X * 32), (int)(Map.SizeInTiles.Y * 32));
            Bitmap texture = FileRessourceProvider.LoadTilesetTexture(FileRessourceProvider.LoadTileset(Map.TilesetId).TextureName);
            Graphics g = Graphics.FromImage(bmp);
            int z = 0;
            for (int x = 0; x < Map.TileIds[z].GetLength(0); x++)
            {
                for (int y = 0; y < Map.TileIds[z].GetLength(1); y++)
                {
                    int id = Map.TileIds[z][x, y];
                    if (id != -1)
                    {
                        PPoint srcPos = new PPoint((id % GameComponents.Tileset.TilesetWidthInTiles) * 32, (id / GameComponents.Tileset.TilesetWidthInTiles) * 32);
                        PPoint dstPos = new PPoint(x * 32, y * 32);
                        g.DrawImage(texture, new System.Drawing.Rectangle(dstPos.X, dstPos.Y, 32, 32),
                            new System.Drawing.Rectangle(srcPos.X, srcPos.Y, 32, 32), System.Drawing.GraphicsUnit.Pixel);
                    }

                }
            }

            // Dessin des polygones
            int Zoom = 100;
            PPoint center = new PPoint(0, 0);
            Pen p, p2;
            foreach (Vertices polygon in Map.Polygons)
            {
                //p = new Pen(System.Drawing.Color.FromArgb(i * 20 % 255, Math.Max(0, (255 - i * 20) % 255), 0), 3);
                p = new Pen(System.Drawing.Color.FromArgb(255, 0, 0), 1);
                p2 = new Pen(System.Drawing.Color.FromArgb(0, 255, 255), 1);
                Microsoft.Xna.Framework.Vector2 old = polygon.First();
                // Dessine lignes puis les points
                foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                {
                    g.DrawLine(p2, new System.Drawing.Point((int)(center.X + vect.X * Zoom), (int)(center.Y + vect.Y * Zoom)),
                                    new System.Drawing.Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));
                    old = vect;
                }
                g.DrawLine(p2, new System.Drawing.Point((int)(center.X + polygon.First().X * Zoom), (int)(center.Y + polygon.First().Y * Zoom)),
                    new System.Drawing.Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));

                foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                {
                    g.DrawEllipse(p, new System.Drawing.Rectangle((int)(center.X + vect.X * Zoom) - 1, (int)(center.Y + vect.Y * Zoom) - 1, 2, 2));
                }
            }
            g.DrawString("Polygons : " + Map.Polygons.Count.ToString(), System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Black, new System.Drawing.PointF(20, 20));
            g.Flush();
            bmp.Save(FileRessourceProvider.ContentDir + "RunTimeAssets\\Data\\Maps\\Snapshot" + Map.Polygons.Count.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }
        #endregion
    }
}
