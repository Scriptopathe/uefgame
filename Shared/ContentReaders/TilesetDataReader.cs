using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;
namespace UeFGame.ContentReaders
{
    public class TilesetDataReader : ContentTypeReader<List<GameComponents.Tileset>>
    {
        protected override List<GameComponents.Tileset> Read(ContentReader input, List<GameComponents.Tileset> existingInstance)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<GameComponents.Tileset>));
            return (List<GameComponents.Tileset>)serializer.Deserialize(input.BaseStream);
        } 

    }
}
