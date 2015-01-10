using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System.Xml.Serialization;
using System.IO;
using UeFGame;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Data used to initialize a map
    /// </summary>
    /*[XmlInclude(typeof(GameObjects.UniqueBodyGameObjectInitializingData))]
    [XmlInclude(typeof(GameObjects.BodyInitializingData))]
    [XmlInclude(typeof(GameObjects.ShapeInitializingData))]
    [XmlInclude(typeof(GameObjects.SpecializedGameObjectInitializingData))]
    [XmlInclude(typeof(GameObjects.ImageInitializingData))]
    [XmlInclude(typeof(GameObjects.TestInit))]*/
    [XmlInclude(typeof(GameObjects.BaseModule))]
    [XmlInclude(typeof(GameObjects.PhysicalObjectModule))]
    [XmlInclude(typeof(GameObjects.GameEventModule))]
    [XmlInclude(typeof(GameObjects.PlatformEventModule))]
    public class MapInitializingData
    {
        public MapInitializingData()
        {
            TileIds = null;
            SizeInTiles = new Vector2(20, 20);
            Version = 0;
            GameObjects = new List<GameObjects.GameObjectInit>();
        }
        #region Properties
        /// <summary>
        /// Version number of the map.
        /// </summary>
        [XmlAttribute("Version")]
        public int Version
        {
            get;
            set;
        }

        /// <summary>
        /// Size in tiles.
        /// </summary>
        [XmlElement("Size")]
        public Vector2 SizeInTiles
        {
            get { return new Vector2(TileIds[0].GetLength(0), TileIds[0].GetLength(1)); }
            set
            {
                int[][,] newTileIds = new int[GameConstants.Layers][,];
                int oldW;
                int oldH;
                if (TileIds != null)
                {
                    oldW = (int)SizeInTiles.X;
                    oldH = (int)SizeInTiles.Y;
                }
                else
                {
                    oldW = oldH = 0;
                }
                // Resize the tile array.
                for (int z = 0; z < newTileIds.GetLength(0); z++)
                {
                    newTileIds[z] = new int[(int)value.X, (int)value.Y];
                    for (int x = 0; x < newTileIds[0].GetLength(0); x++)
                    {
                        for (int y = 0; y < newTileIds[0].GetLength(1); y++)
                        {

                                if (x < oldW && y < oldH)
                                    newTileIds[z][x, y] = TileIds[z][x, y];
                                else
                                    newTileIds[z][x, y] = -1;
                        }
                    }
                }
                TileIds = newTileIds;

            }
        }
        /// <summary>
        /// List of the Game object's initializing data.
        /// </summary>
        [XmlElement("GameObjects")]
        public List<GameObjects.GameObjectInit> GameObjects;
        /// <summary>
        /// Name of the Tileset object associated to this map.
        /// </summary>
        [XmlElement("TilesetId")]
        public int TilesetId = 0;
        /// <summary>
        /// Three-dimentional array containing the ids of the tiles (which have a graphics and mask
        /// meaning).
        /// (z, x, y)
        /// </summary>
        [XmlIgnore()]
        public int[][,] TileIds;
        /// <summary>
        /// List of computed static map polygons generated from tile ids and mask.
        /// They are computed in the Pipeline process by the Importer.
        /// </summary>
        [XmlIgnore()]
        public List<Vertices> Polygons
        {
            get;
            set;
        }

        #region Serialization
        /// <summary>
        /// Serializable version of the TileIds ùmember.
        /// </summary>
        [XmlElement("Tiles")]
        public byte[] TileIdsSerializable
        {
            get 
            {
                System.IO.MemoryStream stream = new MemoryStream();
                System.IO.BinaryWriter writer = new BinaryWriter(stream);
                writer.Write((int)TileIds.Count()); // z
                writer.Write((int)TileIds[0].GetLength(0)); // x
                writer.Write((int)TileIds[0].GetLength(1)); // y
                for (int z = 0; z < TileIds.Count(); z++)
                {
                    for (int x = 0; x < TileIds[0].GetLength(0); x++)
                    {
                        for (int y = 0; y < TileIds[0].GetLength(1); y++)
                        {
                            writer.Write((short)TileIds[z][x, y]);
                        }
                    }
                }
                writer.Flush();
                return stream.ToArray();
            }
            set 
            {
                System.IO.MemoryStream stream = new MemoryStream(value, false);
                System.IO.BinaryReader reader = new BinaryReader(stream);
                int zMax = reader.ReadInt32();
                int xMax = reader.ReadInt32();
                int yMax = reader.ReadInt32();
                int[][,] newIds = new int[zMax][,];
                for (int z = 0; z < zMax; z++)
                {
                    newIds[z] = new int[xMax, yMax];
                    for (int x = 0; x < xMax; x++)
                    {
                        for (int y = 0; y < yMax; y++)
                        {
                           newIds[z][x, y] = reader.ReadInt16();
                        }
                    }
                }
                TileIds = newIds;
            }
        }
        /// <summary>
        /// Serializable polygons.
        /// </summary>
        [XmlElement("Polygons")]
        public byte[] PolygonsSerializable
        {
            get
            {
                System.IO.MemoryStream stream = new MemoryStream();
                System.IO.BinaryWriter writer = new BinaryWriter(stream);
                if (Polygons == null)
                    writer.Write((int)-1);
                else
                {
                    writer.Write((int)Polygons.Count);
                    foreach (Vertices vertices in Polygons)
                    {
                        writer.Write((int)vertices.Count);
                        foreach (Vector2 vect in vertices)
                        {
                            writer.Write((float)vect.X);
                            writer.Write((float)vect.Y);
                        }
                    }
                }
                writer.Flush();
                return stream.ToArray();
            }
            set
            {
                System.IO.MemoryStream stream = new MemoryStream(value, false);
                System.IO.BinaryReader reader = new BinaryReader(stream);
                int polygonCount = reader.ReadInt32();
                List<Vertices> polygons;
                if (polygonCount == -1)
                {
                    polygons = new List<Vertices>();
                }
                else
                {
                    polygons = new List<Vertices>();
                    for (int i = 0; i < polygonCount; i++)
                    {
                        int polygonSize = reader.ReadInt32();
                        Vertices vertices = new Vertices();
                        for (int j = 0; j < polygonSize; j++)
                        {
                            vertices.Add(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
                        }
                        polygons.Add(vertices);
                    }
                    
                }
                Polygons = polygons;
            }
        }
        #endregion

        #endregion
        #region Utils
        /// <summary>
        /// Gets the size of the map in simulation units.
        /// </summary>
        public Vector2 SimSize
        {
            get
            {
                float x = ConvertUnits.ToSimUnits(TileIds[0].GetLength(0) * GameConstants.Tilesize);
                float y = ConvertUnits.ToSimUnits(TileIds[1].GetLength(1) * GameConstants.Tilesize);
                return new Vector2(x, y);
            }
        }
        #endregion
    }
}
