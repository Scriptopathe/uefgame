using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
namespace UeFGame.GameObjects
{
    public abstract class Module
    {
        /// <summary>
        /// Serializes this instance of the module.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string SerializeString(Type type)
        {
            System.Xml.Serialization.XmlSerializer ser = new XmlSerializer(type);
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, this);
            string str = writer.ToString();
            writer.Close();
            writer.Dispose();
            return str;
        }
        /// <summary>
        /// Deserializes a module from a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T DeserializeString<T>(string str)
        {
            System.Xml.Serialization.XmlSerializer ser = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(str);
            T obj = (T)ser.Deserialize(reader);
            reader.Close();
            return obj;
        }
        /// <summary>
        /// Deserializes a module from a string.
        /// </summary>
        /// <returns></returns>
        public abstract object DeserializeString(string str);
        /// <summary>
        /// Serializes this instance of Module into a string.
        /// </summary>
        /// <returns></returns>
        public abstract string SerializeString();
        /// <summary>
        /// Creates and returns a deep copy of this module.
        /// </summary>
        /// <returns></returns>
        public abstract Module DeepCopy();
    }
}
