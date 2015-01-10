using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Masque de collision d'un tileset (pixel par pixel).
    /// Il est utilisé pour la génération de polygones.
    /// </summary>
    public class TilesetMask
    {
        [XmlIgnore()]
        public bool[,] Raw
        {
            get;
            set;
        }
        /// <summary>
        /// Version sérialisable des données brutes.
        /// </summary>
        [XmlElement("Raw")]
        public byte[] Serializable
        {
            get
            {
                System.IO.MemoryStream stream = new MemoryStream();
                System.IO.BinaryWriter writer = new BinaryWriter(stream);
                writer.Write((int)Raw.GetLength(0));
                writer.Write((int)Raw.GetLength(1));
                for (int x = 0; x < Raw.GetLength(0); x++)
                {
                    for (int y = 0; y < Raw.GetLength(1); y++)
                    {
                        writer.Write(Raw[x, y]);
                    }
                }
                writer.Flush();
                return stream.ToArray();
            }
            set
            {
                System.IO.MemoryStream stream = new MemoryStream(value, false);
                System.IO.BinaryReader reader = new BinaryReader(stream);
                bool[,] raw = new bool[reader.ReadInt32(), reader.ReadInt32()];
                for (int x = 0; x < raw.GetLength(0); x++)
                {
                    for (int y = 0; y < raw.GetLength(1); y++)
                    {
                        raw[x, y] = reader.ReadBoolean();
                    }
                }
                Raw = raw;
            }
        }
        /// <summary>
        /// Transforme le masque actuel en un bitmap monochrome.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Bitmap ToMonochromaticBitmap()
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Raw.GetLength(0), Raw.GetLength(1));
            for (int x = 0; x < Raw.GetLength(0); x++)
            {
                for (int y = 0; y < Raw.GetLength(1); y++)
                {
                    bmp.SetPixel(x, y, Raw[x, y] ? System.Drawing.Color.Black : System.Drawing.Color.White);
                }
            }
            return bmp;
        }
        public TilesetMask()
        {

        }
    }
}
