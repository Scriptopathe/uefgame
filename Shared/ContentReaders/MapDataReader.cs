using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;
namespace UeFGame.ContentReaders
{
    public class MapDataReader : ContentTypeReader<GameComponents.MapInitializingData>
    {
        protected override GameComponents.MapInitializingData Read(ContentReader input, GameComponents.MapInitializingData existingInstance)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameComponents.MapInitializingData));
            return (GameComponents.MapInitializingData)serializer.Deserialize(input.BaseStream);
            
        } 

    }
}
