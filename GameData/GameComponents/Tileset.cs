using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System.Xml.Serialization;
using System.IO;
namespace UeFGame.GameComponents
{

    /// <summary>
    /// Represents a tileset.
    /// </summary>
    [XmlRoot("Tileset")]
    public class Tileset
    {
        #region Enum & structs
        /// <summary>
        /// Represents a neightboor.
        /// </summary>
        enum Neightboor
        {
            Top,
            Right, 
            Bottom,
            Left
        }
        /// <summary>
        /// Represents an edge.
        /// </summary>
        public struct Edge
        {
            public Vector2 P1;
            public Vector2 P2;
            public Edge(Vector2 p1, Vector2 p2)
            {
                P1 = p1;
                P2 = p2;
            }
        }
        #endregion

        #region Constants
        /// <summary>
        /// Max tileset Width and Height
        /// </summary>
        public const int MaxTilesetSize = 2024 / GameConstants.Tilesize;
        public const int TilesetWidthInTiles = 8;
        #endregion

        #region Properties
        /// <summary>
        /// Name of this tileset.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Name of the Texture of this Tileset (without extension).
        /// </summary>
        [XmlAttribute("TextureName")]
        public string TextureName { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a copy of the given tileset.
        /// TODO : finish
        /// </summary>
        /// <param name="tileset"></param>
        public Tileset(Tileset tileset)
        {
            this.Name = tileset.Name;
            this.TextureName = tileset.TextureName;
            
        }
        /// <summary>
        /// Creates a new tileset
        /// </summary>
        public Tileset()
        {
            Name = "";
            TextureName = "default";
        }






        #region Map Bodies computation
        /*/// <summary>
        /// Gets a TileMask given its id.
        /// </summary>
        TileMaskOld GetTileMaskById(int id)
        {
            return Mask[id / MaxTilesetSize, id % MaxTilesetSize];
        }
        /// <summary>
        /// Get the tilemask at the given position in the ids.
        /// </summary>
        TileMaskOld GetTileMaskAt(int[,] ids, int x, int y)
        {
            int id = ids[x, y];
            return GetTileMaskById(id);
        }
        /// <summary>
        /// Computes a list of bodies using the tileset mask.
        /// ids : 2-dimmensionnal array of ids corresponding to the Tileset mask.
        /// </summary>
        /// <returns></returns>
        public List<Body> Compute(int[,] ids)
        {
            bool[,] isComputed = new bool[ids.GetLength(0), ids.GetLength(1)];
            List<Body> bodies = new List<Body>();
            for (int chunkX = 0; chunkX <= ids.GetLength(0) / ChunkSize; chunkX++)
            {
                for (int chunkY = 0; chunkY <= ids.GetLength(0) / ChunkSize; chunkY++)
                {
                    ComputeChunk(ids, isComputed, chunkX, chunkY, bodies);
                }
            }
            return bodies;
        }
        /// <summary>
        /// Computes a chunck.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <returns></returns>
        void ComputeChunk(int[,] ids, bool[,] isComputed, int chunkX, int chunkY, List<Body> bodies)
        {
            int startX = chunkX * ChunkSize;
            int startY = chunkY * ChunkSize;
            int maxX = Math.Min((chunkX + 1) * ChunkSize, ids.GetLength(0));
            int maxY = Math.Min((chunkY + 1) * ChunkSize, ids.GetLength(1));
            // While there are some uncomputed tiles in the list :
            Point toCompute = FindFirstUncomputed(isComputed, ids, startX, startY, maxX, maxY);
            while (toCompute.X != -1)
            {
                TileMaskOld tile = GetTileMaskAt(ids, toCompute.X, toCompute.Y);
                bodies.Add(ComputeBody(ids, isComputed, tile.MaterialId, toCompute.X, toCompute.Y, startX, startY, maxX, maxY));
                toCompute = FindFirstUncomputed(isComputed, ids, startX, startY, maxX, maxY);
            }
        }
        /// <summary>
        /// Computes a body starting from the given (x, y) Tile coordinates.
        /// </summary>
        Body ComputeBody(int[,] ids, bool[,] isComputed, int materialId, int x, int y, int minX, int minY, int maxX, int maxY)
        {
            Vertices vertices = new Vertices();
            List<Edge> edges = new List<Edge>();
            TileMaterial material = TileMaterial.GetMaterialById(materialId);

            // Computes the edges
            ComputeTileEdgesAndNeightboors(ids, isComputed, materialId, x, y, edges);

            // Debug code
            if (Scenes.SceneDebug.Instance != null)
            {
                Scenes.SceneDebug.Instance.Edges.AddRange(edges);
                Scenes.SceneDebug.Instance.MyVertices.Add(EdgesToVertices(edges));
            }

            // Creates the body
            if (Scenes.SceneDebug.Instance == null)
            {
                Body body = new Body(Globals.World);
                body.BodyType = BodyType.Static;
                var verticesList = FarseerPhysics.Common.Decomposition.BayazitDecomposer.ConvexPartition(vertices);
                FixtureFactory.AttachCompoundPolygon(verticesList, material.Density, body);
                return body;
            }
            else
                return null;
        }
        /// <summary>
        /// Transform a list of edges into a list of vertices.
        /// </summary>
        Vertices EdgesToVertices(List<Edge> edges)
        {
            Vertices vertices = new Vertices();
            vertices.Add(edges[0].P1);
            Edge lastEdge = edges[0];
            bool found;
            while (edges.Count != 0)
            {
                Edge lastEdgeRef = lastEdge;
                found = false;
                foreach(Edge edge in edges)
                {
                    if (lastEdge.P2 == edge.P1)
                    {
                        vertices.Add(lastEdge.P2);
                        lastEdge = edge;
                        found = true;
                        break;
                    }
                }
                // Aucune vertice n'a été trouvée.
                if (!found)
                {
                    lastEdge = new Edge(lastEdge.P2, GetNearest(lastEdge.P2, edges));
                }
                if (edges.Contains(lastEdgeRef))
                {
                    edges.Remove(lastEdgeRef);
                }

            }
            return vertices;
        }
        Vector2 GetNearest(Vector2 vertice, List<Edge> others)
        {
            float dist = 100000000.0f;
            Vector2 nearest = new Vector2(-1, -1);
            foreach (Edge edge in others)
            {
                if (nearest.X == -1)
                {
                    dist = GetDist(vertice, edge.P1);
                    nearest = edge.P1;
                    continue;
                }
                float distTemp = GetDist(vertice, edge.P1);
                if (distTemp < dist)
                {
                    dist = distTemp;
                    nearest = edge.P1;
                }

            }
            return nearest;
        }
        /// <summary>
        /// Distance entre 2 points
        /// </summary>
        /// <returns></returns>
        float GetDist(Vector2 p1, Vector2 p2)
        {
            float d1 = (p1.X - p2.X);
            float d2 = (p2.Y - p1.Y);
            return (float)Math.Sqrt(d1 * d1 + d2 * d2);
        }
        /// <summary>
        /// Compute this tile's edges and his neightboors edges, and put them in the given List of edges.
        /// </summary>
        void ComputeTileEdgesAndNeightboors(int[,] ids, bool[,] isComputed, int materialId, int x, int y, List<Edge> edges)
        {
            // Performs some pre-initializations : destroys the "closed" vertices.
            isComputed[x, y] = true;
            TileMaskOld mask = GetTileMaskAt(ids, x, y);

            // TODO : faire au cas par cas à la place. et merde. Zutre.
            // éléments à prendre en compte : 
            //
            int oldVerticeId = 0;
            Vector2 offset = new Vector2(ConvertUnits.ToSimUnits(x * GameConstants.Tilesize),
                                        ConvertUnits.ToSimUnits(y * GameConstants.Tilesize));
            for (int i = 1; i < mask.GetVerticesCount()+1; i++)
            {
                int verticeId = i % mask.GetVerticesCount();
                if (!(IsClosedVertice(ids, materialId, verticeId, x, y) || IsClosedVertice(ids, materialId, oldVerticeId, x, y)))
                {
                    Vector2 v1 = mask.GetVerticeById(oldVerticeId) + offset;
                    Vector2 v2 = mask.GetVerticeById(verticeId) + offset;
                    edges.Add(new Edge(v1, v2));
                    
                }
                oldVerticeId = verticeId;
            }
            // Compute neightboors
            if ((y != 0) && HasLinkWith(ids, materialId, x, y, Neightboor.Top) && (!isComputed[x, y-1]))
            {
                ComputeTileEdgesAndNeightboors(ids, isComputed, materialId, x, y - 1, edges);
            }
            if ((y < ids.GetLength(1) - 1) && HasLinkWith(ids, materialId, x, y, Neightboor.Bottom) && (!isComputed[x, y + 1]))
            {
                ComputeTileEdgesAndNeightboors(ids, isComputed, materialId, x, y + 1, edges);
            }
            if ((x != 0) && HasLinkWith(ids, materialId, x, y, Neightboor.Left) && (!isComputed[x-1, y]))
            {
                ComputeTileEdgesAndNeightboors(ids, isComputed, materialId, x - 1, y, edges);
            }
            if ((x < ids.GetLength(0) - 1) && HasLinkWith(ids, materialId, x, y, Neightboor.Right) && (!isComputed[x + 1, y]))
            {
                ComputeTileEdgesAndNeightboors(ids, isComputed, materialId, x + 1, y, edges);
            }
        }

        /// <summary>
        /// Returns true if the given (x, y) Tile has a link with one of this neightboors
        /// (top, right, etc...).
        /// The material is used to know if the given neightboor can be included in the same body.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="materialId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="neightboor"></param>
        /// <returns>Returns true if the given tile has a link with the given neightboor</returns>
        bool HasLinkWith(int[,] ids, int materialId, int x, int y, Neightboor neightboor)
        {
            TileMaskOld mask1 = GetTileMaskAt(ids, x, y);
            TileMaskOld mask2;
            bool res;
            switch (neightboor)
            {
                case Neightboor.Top:
                    mask2 = GetTileMaskAt(ids, x, y - 1);
                    if (mask1.MaterialId != mask2.MaterialId)
                        return false;
                    res = EdgeIntersect(mask1.TopEdgeSize, mask2.BottomEdgeSize);
                    return res;
                case Neightboor.Bottom:
                    mask2 = GetTileMaskAt(ids, x, y + 1);
                    if (mask1.MaterialId != mask2.MaterialId)
                        return false;
                    res = EdgeIntersect(mask1.BottomEdgeSize, mask2.TopEdgeSize);
                    return res;
                case Neightboor.Left:
                    mask2 = GetTileMaskAt(ids, x - 1, y);
                    if (mask1.MaterialId != mask2.MaterialId)
                        return false;
                    res = EdgeIntersect(mask1.LeftEdgeSize, mask2.RightEdgeSize);
                    return res;
                case Neightboor.Right:
                    mask2 = GetTileMaskAt(ids, x + 1, y);
                    if (mask1.MaterialId != mask2.MaterialId)
                        return false;
                    res = EdgeIntersect(mask1.RightEdgeSize, mask2.LeftEdgeSize);
                    return res;
                default:
                    throw new Exception("invalid neightboor");
            }
    
        }
        /// <summary>
        /// Returns true if the two 1-dimentionnal vectors intersect.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        bool EdgeIntersect(Vector2 v1, Vector2 v2)
        {
            float fromV1 = Math.Min(v1.X, v1.Y);
            float toV1 = Math.Max(v1.X, v1.Y);
            float fromV2 = Math.Min(v2.X, v2.Y);
            float toV2 = Math.Max(v2.X, v2.Y);

            return ((fromV1 <= fromV2 && toV1 >= fromV2) || (fromV1 <= toV2 && toV1 >= toV2));
        }
        /// <summary>
        /// Returns true if the given vertice is "closed" (= useless).
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="materialId"></param>
        /// <param name="x">x position of the Tile in the ids</param>
        /// <param name="y">y position of the Tile in the ids</param>
        /// <param name="corner">0 : topleft, 1 topright, 2 lowerright, 3 lowerleft</param>
        /// <returns></returns>
        bool IsClosedVertice(int[,] ids, int materialId, int verticeId, int x, int y)
        {
            TileMaskOld mask = GetTileMaskAt(ids, x, y);
            Vector2 thisVertice = mask.GetVerticeById(verticeId);

            // Check for existancy of the tiles on the sides.
            if (x == 0)
            {
                if (mask.IsLowerLeft(verticeId) || mask.IsUpperLeft(verticeId))
                    return false;
            }
            else if (x == ids.GetLength(0) - 1)
            {
                if (mask.IsLowerRight(verticeId) || mask.IsUpperRight(verticeId))
                    return false;
            }
            if (y == 0)
            {
                if (mask.IsUpperRight(verticeId) || mask.IsUpperLeft(verticeId))
                    return false;
            }
            else if (y == ids.GetLength(1) - 1)
            {
                if (mask.IsLowerRight(verticeId) || mask.IsLowerLeft(verticeId))
                    return false;
            }

            if (mask.IsUpperLeft(verticeId)) // top left
            {
                if(!(mask.MaterialId == materialId && mask.UpperLeftCorner))
                    return false;
                mask = GetTileMaskAt(ids, x - 1, y);
                if (mask.MaterialId == materialId && mask.UpperRightCorner)
                {
                    mask = GetTileMaskAt(ids, x - 1, y - 1);
                    if (mask.MaterialId == materialId && mask.LowerRightCorner)
                    {
                        mask = GetTileMaskAt(ids, x, y - 1);
                        if (mask.MaterialId == materialId && mask.LowerLeftCorner)
                            return true;
                    }
                }
            }
            else if (mask.IsUpperRight(verticeId)) // top right
            {
                if (!(mask.MaterialId == materialId && mask.UpperRightCorner))
                    return false;
                mask = GetTileMaskAt(ids, x + 1, y);
                if (mask.MaterialId == materialId && mask.UpperLeftCorner)
                {
                    mask = GetTileMaskAt(ids, x + 1, y - 1);
                    if (mask.MaterialId == materialId && mask.LowerLeftCorner)
                    {
                        mask = GetTileMaskAt(ids, x, y - 1);
                        if (mask.MaterialId == materialId && mask.LowerRightCorner)
                            return true;
                    }
                }
            }
            else if (mask.IsLowerLeft(verticeId)) // lower left
            {
                if (!(mask.MaterialId == materialId && mask.LowerLeftCorner))
                    return false;
                mask = GetTileMaskAt(ids, x - 1, y);
                if (mask.MaterialId == materialId && mask.LowerRightCorner)
                {
                    mask = GetTileMaskAt(ids, x - 1, y + 1);
                    if (mask.MaterialId == materialId && mask.UpperRightCorner)
                    {
                        mask = GetTileMaskAt(ids, x, y + 1);
                        if (mask.MaterialId == materialId && mask.UpperLeftCorner)
                            return true;
                    }
                }
            }
            else if (mask.IsLowerRight(verticeId)) // lower right
            {
                if (!(mask.MaterialId == materialId && mask.LowerRightCorner))
                    return false;
                mask = GetTileMaskAt(ids, x + 1, y);
                if (mask.MaterialId == materialId && mask.LowerLeftCorner)
                {

                    mask = GetTileMaskAt(ids, x + 1, y + 1);
                    if (mask.MaterialId == materialId && mask.UpperLeftCorner)
                    {
                        mask = GetTileMaskAt(ids, x, y + 1);
                        if (mask.MaterialId == materialId && mask.UpperRightCorner)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Find first uncomputed (and computable i.e. not Void) tile.
        /// If the result is Point(-1, -1), then all the tiles are computed.
        /// </summary>
        Point FindFirstUncomputed(bool[,] isComputed, int[,] ids, int sx, int sy, int mx, int my)
        {
            for (int x = sx; x < mx; x++)
            {
                for (int y = sy; y < my; y++)
                {
                    if (!isComputed[x, y])
                    {
                        // We want a non void tile.
                        int material = GetTileMaskAt(ids, x, y).MaterialId;
                        if (material != TileMaterial.Void.Id)
                        {
                            return new Point(x, y);
                        }
                    }
                }
            }
            return new Point(-1, -1);
        }
         * */
        #endregion
        #endregion
    }
}
