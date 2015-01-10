using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using TWrite = System.Collections.Generic.List<UeFGame.GameComponents.Tileset>;


namespace MapImporter
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class UeFTilesetWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            // Ecriture binaire.
            XmlSerializer ser = new XmlSerializer(typeof(TWrite));
            ser.Serialize(output.BaseStream, value);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "UeFGame.ContentReaders.TilesetDataReader, Shared";
        }
    }
}
