using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
namespace UeFGame.GameObjects
{
    public class BaseModule : Module
    {

        #region Variables
        /// <summary>
        /// The event's name
        /// </summary>
        [Editor.PropertyEdition("Identification")]
        public string Name = "";
        /// <summary>
        /// The ID to be used to find the event's compiled behaviors, stored as static methods.
        /// To find the behavior :
        /// In Namespace : UeF
        /// In Type : Behaviors[MapName]
        /// In Method : OnEvtSmth[BehaviorID]
        /// </summary>
        [Editor.PropertyEdition("Identification")]
        public int BehaviorID;
        /// <summary>
        /// Start position x in simulation units.
        /// </summary>
        [Editor.PropertyEdition("Positionnement")]
        public float SimStartX;
        /// <summary>
        /// Start position y in simulation units
        /// </summary>
        [Editor.PropertyEdition("Positionnement")]
        public float SimStartY;
        /// <summary>
        /// Z layer depth.
        /// Floating value between 0.0 (back) and 1.0 (front).
        /// </summary>
        [Editor.PropertyEdition("Affichage")]
        public float Z;
        /// <summary>
        /// Scripts.
        ///     - The static method Evt_{EVTID}_Initialize() is called and can 
        ///       subscribe to the game object's events.
        /// </summary>
        [Editor.NotEditableProperty()]
        public string Script
        {
            get;
            set;
        }

        public virtual Vector2 SimPosition
        {
            get { return new Vector2(SimStartX, SimStartY); }
            set { SimStartX = value.X; SimStartY = value.Y; }
        }
        #endregion

        public BaseModule() { }

        #region Overrides
        public override object DeserializeString(string str)
        {
            return Module.DeserializeString<BaseModule>(str);
        }

        public override string SerializeString()
        {
            return base.SerializeString(typeof(BaseModule));
        }
        public override Module DeepCopy()
        {
            BaseModule mod = new BaseModule();
            mod.Name = Name;
            mod.BehaviorID = BehaviorID;
            mod.SimPosition = SimPosition;
            mod.Z = Z;
            return mod;
        }
        #endregion
    }
}