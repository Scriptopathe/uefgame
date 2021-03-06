﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
namespace UeFGame.GameObjects.MoveManagers
{
    /// <summary>
    /// Interface for all the move managers.
    /// </summary>
    public interface IMoveManager
    {
        /// <summary>
        /// Updates the manager.
        /// </summary>
        void Update();
        /// <summary>
        /// Makes the object move to the right
        /// </summary>
        void MoveRight();
        /// <summary>
        /// Makes the object move to the left.
        /// </summary>
        void MoveLeft();
        /// <summary>
        /// Resets the move manager.
        /// </summary>
        void Reset();
    }
}
