using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace UeFGame.GameObjects
{
    /// <summary>
    /// Object containing information about the Game Map's objects creation and settings.
    /// </summary>
    public class GameObjectInit
    {
        [XmlAttribute("GameObjectType")]
        public string Type = "";

        [XmlElement("ModuleSet")]
        public ModuleSet ModuleSet;

        public GameObjectInit()
        {
            ModuleSet = new ModuleSet();
        }

        /// <summary>
        /// Creates and returns a Deep Copy of this instance of GameObjectInit.
        /// </summary>
        /// <returns></returns>
        public GameObjectInit DeepCopy()
        {
            GameObjectInit init = new GameObjectInit();
            init.Type = Type;
            init.ModuleSet = ModuleSet.DeepCopy();
            return init;
        }
    }

}
