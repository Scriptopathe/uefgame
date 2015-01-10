using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geex.Edit;
using System.Xml.Serialization;
using System.IO;
using Map = UeFGame.GameComponents.MapInitializingData;
using TSerializeMap = UeFGame.GameComponents.MapInitializingData;
namespace Geex.Edit.UeF.Project
{
    /// <summary>
    /// Work class for the Map class
    /// TODO : complete the methods' body.
    /// </summary>
    [XmlInclude(typeof(Microsoft.Xna.Framework.Color))]
    [XmlInclude(typeof(Microsoft.Xna.Framework.Vector2))]
    public class UeFMapWork
    {
        /// <summary>
        /// Copies the map meta data from the given map.
        /// If the width and height have changed, the map is NOT resized.
        /// Corresponds to the data that can be edited to the "Properties" form.
        /// </summary>
        /// <param name="src"></param>
        public static void CopyMetadataFrom(Map dst, Map src)
        {
            
        }

        /// <summary>
        /// Returns a copy of the map object.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Map CopyFromMap(Map map)
        {
            Map newMap = new Map();
            CopyMetadataFrom(newMap, map);
            return newMap;
        }

        /// <summary>
        /// Creates a new map
        /// </summary>
        public static Map CreateNewMap()
        {
            Map map = new Map();
            map.SizeInTiles = new Microsoft.Xna.Framework.Vector2(2000, 200);
            return map;
        }

        /// <summary>
        /// Resize the map data.
        /// </summary>
        public static void ResizeMap(Map map, int w, int h)
        {
            map.SizeInTiles = new Microsoft.Xna.Framework.Vector2(w, h);
        }
        
        /// <summary>
        /// Serializes the map
        /// </summary>
        /// <param name="filename"></param>
        public static void Save(Map map, string filename)
        {
            if (map == null)
                return;
            XmlSerializer serializer = new XmlSerializer(typeof(Map));
            FileStream Stream = File.Open(filename, FileMode.Create);
            serializer.Serialize(Stream, map);
            Stream.Close();

            // Retrieves the map id
            string str = filename.Substring(filename.Count() - 11, 4);
            int id = int.Parse(str);
            UeFGeexProject proj = (UeFGeexProject)(Geex.Edit.Common.Globals.Project);
            MapImporter.UeFMapCompiler compiler = new MapImporter.UeFMapCompiler();

            string oldStr = "\\UsineEnFolieContent\\";
#if DEBUG
            string newStr = "\\UsineEnFolie\\bin\\x86\\Debug\\Content\\";
#else
            string newStr = "\\UsineEnFolie\\bin\\x86\\Release\\Content\\";
#endif
            string dllFilename = filename.Replace(oldStr, newStr) + ".dll";
            compiler.CompileAll(map, id, dllFilename);
            string contentFilename = dllFilename.Replace(proj.ContentDirectory + "\\", "");
            Geex.Edit.Common.Globals.ContentWork.AddElement(contentFilename, "", "", false, true);
        }
        /// <summary>
        /// Loads a map from a serialized file
        /// </summary>
        /// <param name="filename"></param>
        public static Map Load(string filename)
        {
            XmlSerializer Serializer = new XmlSerializer(typeof(Map));
            FileStream Stream = File.Open(filename, FileMode.Open);
            Map Object = (Map)Serializer.Deserialize(Stream);
            Stream.Close();

            if (Object == null)
                Object = new Map();
            return Object;
           
            /*XmlSerializer Serializer = new XmlSerializer(typeof(Project.CustomSerializableObject));
            FileStream Stream = File.Open(filename, FileMode.Open);
            Map Object = (Map)(((Project.CustomSerializableObject)Serializer.Deserialize(Stream)).GetObject());
            return Object; //*/
        }
    }
}
