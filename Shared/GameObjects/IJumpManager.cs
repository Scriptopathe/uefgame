using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
namespace UeFGame.GameObjects
{
    /// <summary>
    /// Interface for all the jump managers.
    /// </summary>
    public interface IJumpManager
    {
        /// <summary>
        /// Updates the manager.
        /// </summary>
        void Update();
        /// <summary>
        /// Requests for a jump
        /// </summary>
        void RequestJump();
        /// <summary>
        /// Resets the jump manager.
        /// </summary>
        void Reset();
        /// <summary>
        /// Sets the offset used by the subclasses of JumpManager.
        /// </summary>
        void SetOffsetPx(Microsoft.Xna.Framework.Vector2 offset);
    }
    
}
