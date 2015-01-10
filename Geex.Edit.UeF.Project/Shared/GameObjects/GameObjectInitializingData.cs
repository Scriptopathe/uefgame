using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace UeFGame.GameObjects
{
    public enum StructSpeed
    {
        VerySlow,
        Slow,
        Medium,
        Fast,
        VeryFast,
    }
    /// <summary>
    /// Basic data needed to initialize a GameObject.
    /// </summary>
    [Serializable()]
    public class GameObjectInitializingData
    {
        #region Variables
        /// <summary>
        /// The event's name
        /// </summary>
        public string Name;
        /// <summary>
        /// The ID to be used to find the event's compiled behaviors, stored as static methods.
        /// To find the behavior :
        /// In Namespace : UeF
        /// In Type : Behaviors[MapName]
        /// In Method : OnEvtSmth[BehaviorID]
        /// </summary>
        public int BehaviorID;
        /// <summary>
        /// Start position x in simulation units.
        /// </summary>
        public float SimStartX;
        /// <summary>
        /// Start position y in simulation units
        /// </summary>
        public float SimStartY;
        /// <summary>
        /// Z layer depth.
        /// Floating value between 0.0 (back) and 1.0 (front).
        /// </summary>
        public float Z;
        /// <summary>
        /// The full name of the type of the considered GameObject.
        /// </summary>
        public string Type;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the GameObjectInitializingData
        /// </summary>
        public GameObjectInitializingData()
        {
            Name = "";
            BehaviorID = -1;
            SimStartX = 0;
            SimStartY = 0;
            Z = 0;
        }
        /// <summary>
        /// Copies the data inside this object to another data object.
        /// </summary>
        /// <param name="data"></param>
        public void CopyTo(GameObjectInitializingData data)
        {
            data.Name = Name;
            data.BehaviorID = BehaviorID;
            data.SimStartX = SimStartX;
            data.SimStartY = SimStartY;
            data.Z = Z;
        }
        #endregion
    }
}
