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
    public class PlatformEventModule : Module
    {
        #region Platform
        
        /// <summary>
        /// Defines a unit of trajectory.
        /// This unit controls 
        ///     - end point of the trajectory
        ///     - speed and interpolation mode of the trajectory
        ///     - waiting time.
        ///     - trigger.
        /// </summary>
        public class TrajectoryUnit
        {
            #region Enums
            public enum TriggerMode
            {
                PlayerOn,
                Always
            }
            /// <summary>
            /// Represents a mode of trajectory.
            /// </summary>
            public enum TrajectoryMode
            {
                Linear,
                Wait,
            }

            /// <summary>
            /// Represents a mode of coordinates : either relative or absolute.
            /// </summary>
            public enum CoordinateMode
            {
                Relative,
                Absolute
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the coordinate mode : either relative or absolute.
            /// </summary>
            public CoordinateMode Coordinate
            {
                get;
                set;
            }
            /// <summary>
            /// Gets or sets the trigger mode of the unit.
            /// The trigger mode is the conditions that must be achieved in order
            /// to start the action of the platform.
            /// </summary>
            public TriggerMode Trigger
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the trajectory mode of this unit.
            /// This might be Linear or any other interpolation kind, or
            /// waiting.
            /// </summary>
            public TrajectoryMode Trajectory
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the speed of the trajectory unit.
            /// The speed is given in Pixels per second.
            /// 
            /// If the Trajectory is TrajectoryMode.Waiting, the speed defines
            /// the amount of waiting time in seconds. 
            /// </summary>
            public float Speed
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the End point of the trajectory unit.
            /// 
            /// The EndPoint might be either relative to the current position or
            /// absolute depending on the value of CoordinateMode.
            /// </summary>
            public Vector2 EndPoint
            {
                get;
                set;
            }
            #endregion

            #region Constructor / Methods

            /// <summary>
            /// Creates a new instance of TrajectoryUnit.
            /// </summary>
            public TrajectoryUnit()
            {
                EndPoint = Vector2.Zero;
                Speed = 40.0f;
                Trajectory = TrajectoryMode.Linear;
                Coordinate = CoordinateMode.Relative;
                Trigger = TriggerMode.Always;
            }

            /// <summary>
            /// Creates and returns a deep copy of this instance.
            /// </summary>
            /// <returns></returns>
            public TrajectoryUnit DeepCopy()
            {
                TrajectoryUnit cp = new TrajectoryUnit();
                cp.Trigger = Trigger;
                cp.Coordinate = Coordinate;
                cp.EndPoint = EndPoint;
                cp.Speed = Speed;
                cp.Trajectory = Trajectory;
                return cp;
            }
            #endregion

        }
        /// <summary>
        /// Gets or sets the trajectory units.
        /// The trajectory units are the trajectories achieved by this instance.
        /// </summary>
        public List<TrajectoryUnit> TrajectoryUnits
        {
            get;
            set;
        }
        #endregion

        #region Scripting
        
        #endregion

        #region Constructor / overrides
        /// <summary>
        /// Creates a new instance of Platform Event Module.
        /// </summary>
        public PlatformEventModule()
            : base()
        {
            TrajectoryUnits = new List<TrajectoryUnit>();
        }

        public override object DeserializeString(string str)
        {
            return Module.DeserializeString<PlatformEventModule>(str);
        }


        public override string SerializeString()
        {
            Type type = typeof(PlatformEventModule);
            return base.SerializeString(type);
        }

        /// <summary>
        /// Creates a deep copy of this module.
        /// </summary>
        /// <returns></returns>
        public override Module DeepCopy()
        {
            PlatformEventModule module = new PlatformEventModule();
            foreach (TrajectoryUnit unit in TrajectoryUnits)
            {
                module.TrajectoryUnits.Add(unit.DeepCopy());
            }
            return module;
        }
        #endregion
    }
}
