using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
namespace UeFGame.GameObjects.Shoots
{
    /// <summary>
    /// Classe de base de toutes les tirs.
    /// Chaque classe de particule ne peut être héritée, et doit implémenter IPoolable.
    /// </summary>
    public abstract class Shoot : PhysicalObject
    {
        /// <summary>
        /// Deactivates the shoot.
        /// </summary>
        public virtual void Deactivate()
        {
            m_body.Enabled = false;
        }
    }
}
