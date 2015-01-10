using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

// TODO: replace this with the type you want to import.
using TImport = UeFGame.GameComponents.MapInitializingData;

namespace MapImporter
{
    public class PolygonBytes
    {
        [System.Xml.Serialization.XmlElement("Bytes")]
        public byte[] Bytes;
        public PolygonBytes(byte[] b) { Bytes = b; }
        public PolygonBytes() { }
    }
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".uefmap", DisplayName = "UeF Map Importer - UeF Pipeline", DefaultProcessor = "UeFMapProcessor")]
    public class UeFMapImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            FileRessourceProvider.ContentDir = "..\\UsineEnFolieContent\\";
            GameDatabase.Load();

            // Map générée sans ses vertices.
            TImport map =  UeFGame.Tools.Serializer.Deserialize<TImport>(filename);
            DateTime start = DateTime.Now;

            // si la map est pas dans la liste on charge l'ancien et on renvoie ! 
            UeFGame.GameComponents.ModifiedMapList list = UeFGame.GameComponents.ModifiedMapList.Load(FileRessourceProvider.ContentDir + "\\DesignTimeAssets\\Data\\ModifiedMaps.xml");
            string str = filename.Substring(filename.Count() - 11, 4);
            int id = int.Parse(str);
            if (!list.GetMaps().Contains(id) && map.Polygons.Count != 0)
            {
                if (System.IO.File.Exists(filename + ".polygons"))
                {
                    System.IO.File.AppendAllText("generation_log.txt",
    start.Hour.ToString().PadLeft(2, '0') + ":" + start.Minute.ToString().PadLeft(2, '0') + ":" + start.Second.ToString().PadLeft(2, '0') + "  --  " +
    "Retourned Polygons " + System.IO.Path.GetFileName(filename) + ".\r\n");
                    PolygonBytes polygonBytes = ((PolygonBytes)UeFGame.Tools.Serializer.Deserialize<PolygonBytes>(filename + ".polygons", false));
                    map.PolygonsSerializable = polygonBytes.Bytes;
                    return map;
                }
            }

            // Génération des polygones
            // Test rectangles
            map.Polygons = new List<FarseerPhysics.Common.Vertices>();
            
            List<Rectangle> clipRects = new List<Rectangle>();
            int rectsX = (int)map.SizeInTiles.X / 20;
            int rectsY = (int)map.SizeInTiles.Y / 20;
            int sx = (int)(map.SizeInTiles.X / rectsX);
            int sy = (int)(map.SizeInTiles.Y / rectsY);
            for (int x = 0; x < rectsX; x++)
            {
                for (int y = 0; y < rectsY; y++)
                {
                    clipRects.Add(new Rectangle(x * sx, y * sy, sx, sy));
                }
            }
            try
            {
                var vertices = UeFGame.Tools.PolygonGenerator.GeneratePolygonsFromMap(map, clipRects);
                map.Polygons = vertices;

                // Timing
                DateTime stop = DateTime.Now;
                double ms = (stop - start).TotalMilliseconds;

                System.IO.File.AppendAllText("generation_log.txt",
                    start.Hour.ToString().PadLeft(2, '0') + ":" + start.Minute.ToString().PadLeft(2, '0') + ":" + start.Second.ToString().PadLeft(2, '0') + "  --  " +
                    "Generated Map " + System.IO.Path.GetFileName(filename) + " in " + ms.ToString() + "ms.\r\n");
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText("generation_log.txt",
    start.Hour.ToString().PadLeft(2, '0') + ":" + start.Minute.ToString().PadLeft(2, '0') + ":" + start.Second.ToString().PadLeft(2, '0') + "  --  " +
    "[ERROR] Generating Map " + System.IO.Path.GetFileName(filename) + 
    "\r\n-------------------\r\n" +
    e.Message + e.StackTrace + 
    "\r\n-------------------\r\n");
            }
            
            // Serializable polygons. 
            UeFGame.Tools.Serializer.Serialize<PolygonBytes>(new PolygonBytes(map.PolygonsSerializable), filename+".polygons");
            return map;
        }

        void compiler_OnError(System.CodeDom.Compiler.CompilerErrorCollection errors)
        {
            throw new NotImplementedException();
        }
    }
}
