using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System.Xml.Serialization;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Data used to initialize a map
    /// </summary>
    [XmlInclude(typeof(GameObjects.UniqueBodyGameObjectInitializingData))]
    public class MapInitializingData
    {
        /// <summary>
        /// Size in Simulation Units
        /// </summary>
        public Vector2 SimSize = new Vector2(ConvertUnits.ToSimUnits(800));
        /// <summary>
        /// List of the Game object's initializing data.
        /// </summary>
        public List<GameObjects.GameObjectInitializingData> GameObjects = new List<GameObjects.GameObjectInitializingData>();
    }
}
