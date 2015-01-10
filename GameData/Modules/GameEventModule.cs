using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
namespace UeFGame.GameObjects
{
    /// <summary>
    /// Class used to initialize a GameEvent.
    /// </summary>
    public class GameEventModule : Module
    {
        #region Drawing
        /// <summary>
        /// Name of the texture which will be used to draw.
        /// </summary>
        [Editor.NotEditableProperty()]
        [XmlElement("TextureName")]
        public virtual string TextureName
        {
            get;
            set;
        }
        /// <summary>
        /// Filename only of the texture (in RunTimeAssets\\Graphics\\Events\\).
        /// </summary>
        [XmlIgnore()]
        [Editor.PropertyEdition("Drawing")]
        public virtual string EventTextureName
        {
            get { return TextureName.Replace("RunTimeAssets\\Graphics\\GameObjects\\Events\\", ""); }
            set { TextureName = "RunTimeAssets\\Graphics\\GameObjects\\Events\\" + value; }
        }
        /// <summary>
        /// The Drawing color of the texture.
        /// </summary>
        [Editor.PropertyEdition("Drawing")]
        [XmlElement("Tone")]
        public virtual Color Tone
        {
            get;
            set;
        }
        /// <summary>
        /// True if the events needs a sneak body.
        /// </summary>
        public bool HasSneakBody
        {
            get;
            set;
        }
        #endregion

        #region Scripting
        /// <summary>
        /// True if the event register collisions.
        /// </summary>
        [Editor.PropertyEdition("Collisions")]
        public bool RegisterCollisions { get; set; }

        #endregion

        #region Constructor / overrides
        public GameEventModule()
            : base()
        {
            TextureName = "";
            Tone = Color.White;
        }

        public override object DeserializeString(string str)
        {
            return Module.DeserializeString<GameEventModule>(str);
        }


        public override string SerializeString()
        {
            return base.SerializeString(typeof(GameEventModule));
        }

        public override Module DeepCopy()
        {
            GameEventModule module = new GameEventModule();
            module.EventTextureName = EventTextureName;
            module.HasSneakBody = HasSneakBody;
            module.TextureName = TextureName;
            module.Tone = Tone;
            module.RegisterCollisions = RegisterCollisions;
            return module;
        }
        #endregion
    }
}
