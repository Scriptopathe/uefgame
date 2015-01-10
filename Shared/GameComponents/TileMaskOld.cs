using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using System.Xml.Serialization;
using System.IO;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Mask data for a tile.
    /// </summary>
    public class TileMaskOld
    {
        #region Variables
        /// <summary>
        /// Byte containing bool data about the vertices of the corners.
        /// Each bit contains a bool, in that order :
        ///     UpperRightCorner
        ///     UpperLeftCorner
        ///     LowerRightCorner
        ///     LowerLeftCorner
        /// </summary>
        [XmlAttribute("UR")]
        bool m_upperRightCorner = true;
        [XmlAttribute("LR")]
        bool m_lowerRightCorner = true;
        [XmlAttribute("UL")]
        bool m_upperLeftCorner = true;
        [XmlAttribute("LL")]
        bool m_lowerLeftCorner = true;
        [XmlAttribute("U")]
        byte m_topVertice = 0;
        [XmlAttribute("B")]
        byte m_bottomVertice = 0;
        [XmlAttribute("L")]
        byte m_leftVertice = 0;
        [XmlAttribute("R")]
        byte m_rightVertice = 0;
        /// <summary>
        /// Material of the tile.
        /// </summary>
        [XmlAttribute("MaterialId")]
        public int MaterialId
        {
            get;
            set;
        }
        #endregion

        #region Save / Load
        /// <summary>
        /// Saves this mask to a byte array.
        /// </summary>
        /// <returns></returns>
        public void WriteMask(BinaryWriter writer)
        {
            byte UL = m_upperLeftCorner ? (byte)1 : (byte)0;
            byte UR = (byte)((m_upperRightCorner ? 1 : 0) << 1);
            byte LL = (byte)((m_lowerLeftCorner ? 1 : 0) << 2);
            byte LR = (byte)((m_lowerRightCorner ? 1 : 0) << 3);
            byte corners = (byte)(UL | UR | LL | LR);
            writer.Write(corners);
            writer.Write(m_leftVertice);
            writer.Write(m_topVertice);
            writer.Write(m_bottomVertice);
            writer.Write(m_rightVertice);
        }
        /// <summary>
        /// Reads a mask and returns the corresponding tileset.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static TileMaskOld ReadMask(BinaryReader reader)
        {
            TileMaskOld mask = new TileMaskOld();
            byte corners = reader.ReadByte();
            mask.m_upperLeftCorner = (corners & (byte)1) == corners ? true : false;
            mask.m_upperRightCorner = (corners & (byte)(1 << 1)) == corners ? true : false;
            mask.m_lowerLeftCorner = (corners & (byte)(1 << 2)) == corners ? true : false;
            mask.m_lowerRightCorner = (corners & (byte)(1 << 3)) == corners ? true : false;
            mask.m_leftVertice = reader.ReadByte();
            mask.m_topVertice = reader.ReadByte();
            mask.m_bottomVertice = reader.ReadByte();
            mask.m_rightVertice = reader.ReadByte();
            return mask;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Public constructor.
        /// </summary>
        public TileMaskOld()
        {
            MaterialId = 0;
            _idMapping = new List<Vector2>();
            _vertexIdMapping = new List<int>();
            UpperLeftCorner = UpperRightCorner = LowerLeftCorner = LowerRightCorner = true;
            GenerateIdMapping();
            
        }
        #endregion

        #region public corners
        public bool UpperRightCorner
        {
            get { return m_upperRightCorner; }
            set { m_upperRightCorner = value; GenerateIdMapping(); }
        }
        public bool LowerRightCorner
        {
            get { return m_lowerRightCorner; }
            set { m_lowerRightCorner = value; GenerateIdMapping(); }
        }
        public bool UpperLeftCorner
        {
            get { return m_upperLeftCorner; }
            set { m_upperLeftCorner = value; GenerateIdMapping(); }
        }
        public bool LowerLeftCorner
        {
            get { return m_lowerLeftCorner; }
            set { m_lowerLeftCorner = value; GenerateIdMapping(); }
        }
       
        #endregion

        #region Vertice by id
        [NonSerialized()]
        List<Vector2> _idMapping;
        [NonSerialized()]
        List<int> _vertexIdMapping;
        /// <summary>
        /// Generates the id mapping for this mask.
        /// Contains the different vertices in order, and in sim units.
        /// </summary>
        void GenerateIdMapping()
        {
            _idMapping.Clear();
            _vertexIdMapping.Clear();
            if (UpperLeftCorner)
            {
                _idMapping.Add(new Vector2(0, 0));
                _vertexIdMapping.Add(0);
            }
            if (TopVertice != 0)
            {
                _idMapping.Add(new Vector2(sim(GameConstants.Tilesize * TopVertice / 100.0), 0));
                _vertexIdMapping.Add(1);
            }
            if (UpperRightCorner)
            {
                _idMapping.Add(new Vector2(sim(GameConstants.Tilesize), 0));
                _vertexIdMapping.Add(2);
            }
            if (RightVertice != 0)
            {
                _idMapping.Add(new Vector2(sim(GameConstants.Tilesize), sim(GameConstants.Tilesize * RightVertice / 100.0)));
                _vertexIdMapping.Add(3);
            }
            if (LowerRightCorner)
            {
                _idMapping.Add(new Vector2(sim(GameConstants.Tilesize), sim(GameConstants.Tilesize)));
                _vertexIdMapping.Add(4);
            }
            if (BottomVertice != 0)
            {
                _idMapping.Add(new Vector2(sim(GameConstants.Tilesize * BottomVertice / 100.0), sim(GameConstants.Tilesize)));
                _vertexIdMapping.Add(5);
            }
            if (LowerLeftCorner)
            {
                _idMapping.Add(new Vector2(0, sim(GameConstants.Tilesize)));
                _vertexIdMapping.Add(6);
            }
            if (LeftVertice != 0)
            {
                _idMapping.Add(new Vector2(0, sim(GameConstants.Tilesize * LeftVertice / 100.0)));
                _vertexIdMapping.Add(7);
            }

        }

        #region Vertice check
        /// <summary>
        /// Returns true if a the given vertice id corresponds to the upper left vertice.
        /// </summary>
        /// <param name="verticeId"></param>
        /// <returns></returns>
        public bool IsUpperLeft(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 0;
        }
        public bool IsTop(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 1;
        }
        public bool IsUpperRight(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 2;
        }
        public bool IsRight(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 3;
        }
        public bool IsLowerRight(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 4;
        }
        public bool IsBottom(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 5;
        }
        public bool IsLowerLeft(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 6;
        }
        public bool IsLeft(int verticeId)
        {
            return _vertexIdMapping[verticeId] == 7;
        }
        #endregion

        /// <summary>
        /// Tilesize in sim units.
        /// </summary>
        public float TileSizeSim
        {
            get { return sim(GameConstants.Tilesize); }
        }
        /// <summary>
        /// Gets the vertice given their id.
        /// Id starts to the top left vertice, and goes to the to right etc...
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector2 GetVerticeById(int id)
        {
            return _idMapping[id];
        }
        /// <summary>
        /// Gets the next id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetNextId(int id)
        {
            id++;
            id %= _idMapping.Count;
            return id;
        }
        /// <summary>
        /// Gets the previous id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetPreviousId(int id)
        {
            id--;
            if (id == -1)
                id = _idMapping.Count - 1;
            return id;
        }
        /// <summary>
        /// Vertices count
        /// </summary>
        /// <returns></returns>
        public int GetVerticesCount()
        {
            return _idMapping.Count;
        }
        #endregion

        #region Public vertice properties
        /// <summary>
        /// Value from 1 to 99 indicating the position of the top vertice of the tile
        /// in percent, starting from the left. 
        /// </summary>
        public byte TopVertice
        {
            get
            {
                if (UpperLeftCorner && UpperRightCorner)
                    return 0;
                else
                    return m_topVertice;
            }
            set { m_topVertice = value; GenerateIdMapping(); }
        }
        /// <summary>
        /// Start and end of the top edge in sim units.
        /// </summary>
        public Vector2 TopEdgeSize
        {
            get
            {
                if (UpperLeftCorner && UpperRightCorner)
                    return new Vector2(0, TileSizeSim);
                else if (UpperLeftCorner && TopVertice != 0)
                    return new Vector2(0, ConvertUnits.ToSimUnits(GameConstants.Tilesize * TopVertice / 100.0));
                else if (UpperRightCorner && TopVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * TopVertice / 100.0), TileSizeSim);
                else if (TopVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * TopVertice / 100.0));
                else
                    return new Vector2(-1, -1);
            }
        }
        /// <summary>
        /// Value from 0 to 100 indicating the position of the bottom vertice of the tile
        /// in percent, starting from the left.
        /// </summary>
        public byte BottomVertice
        {
            get 
            {
                if (LowerLeftCorner && LowerRightCorner)
                    return 0;
                else
                    return m_bottomVertice;
            }
            set { m_bottomVertice = value; GenerateIdMapping(); }
        }
        /// <summary>
        /// Start and end of the bottom edge in sim units.
        /// </summary>
        public Vector2 BottomEdgeSize
        {
            get
            {
                if (LowerLeftCorner && LowerRightCorner)
                    return new Vector2(0, ConvertUnits.ToSimUnits(GameConstants.Tilesize));
                else if (LowerLeftCorner && BottomVertice != 0)
                    return new Vector2(0, ConvertUnits.ToSimUnits(GameConstants.Tilesize * BottomVertice / 100.0));
                else if (LowerRightCorner && BottomVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * BottomVertice / 100.0), ConvertUnits.ToSimUnits(GameConstants.Tilesize));
                else if (BottomVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * BottomVertice / 100.0));
                else
                    return new Vector2(-1, -1);
            }
        }
        /// <summary>
        /// Value from 0 to 100 indicating the position of the top vertice of the tile
        /// in percent, starting from the top.
        /// </summary>
        public byte LeftVertice
        {
            get 
            {
                if (UpperLeftCorner && LowerLeftCorner)
                    return 0;
                else
                    return m_leftVertice; 
            }
            set { m_leftVertice = value; GenerateIdMapping(); }
        }
        /// <summary>
        /// Start and end of the left edge in sim units.
        /// </summary>
        public Vector2 LeftEdgeSize
        {
            get
            {
                if (UpperLeftCorner && LowerLeftCorner)
                    return new Vector2(0, ConvertUnits.ToSimUnits(GameConstants.Tilesize));
                else if (UpperLeftCorner && LeftVertice != 0)
                    return new Vector2(0, ConvertUnits.ToSimUnits(GameConstants.Tilesize * LeftVertice / 100.0));
                else if (LowerLeftCorner && LeftVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * LeftVertice / 100.0), ConvertUnits.ToSimUnits(GameConstants.Tilesize));
                else if (LeftVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * LeftVertice / 100.0));
                else
                    return new Vector2(-1, -1);
            }
        }
        /// <summary>
        /// Value from 0 to 100 indicating the position of the right vertice of the tile
        /// in percent, starting from the top.
        /// </summary>
        public byte RightVertice
        {
            get 
            {
                if (UpperRightCorner && LowerRightCorner)
                    return 0;
                else
                    return m_rightVertice; 
            }
            set { m_rightVertice = value; GenerateIdMapping(); }
        }
        /// <summary>
        /// Start and end of the Right edge in sim units.
        /// </summary>
        public Vector2 RightEdgeSize
        {
            get
            {
                if (UpperRightCorner && LowerRightCorner)
                    return new Vector2(0, ConvertUnits.ToSimUnits(GameConstants.Tilesize));
                else if (UpperRightCorner && RightVertice != 0)
                    return new Vector2(0, ConvertUnits.ToSimUnits(GameConstants.Tilesize * RightVertice / 100.0));
                else if (LowerRightCorner && RightVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * RightVertice / 100.0), ConvertUnits.ToSimUnits(GameConstants.Tilesize));
                else if (RightVertice != 0)
                    return new Vector2(ConvertUnits.ToSimUnits(GameConstants.Tilesize * RightVertice / 100.0));
                else
                    return new Vector2(-1, -1);
            }
        }
        #endregion

        #region Utils

        float sim(double n)
        {
            return ConvertUnits.ToSimUnits(n);
        }

        #endregion
    }

    
}
