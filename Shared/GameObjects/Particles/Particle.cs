using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameObjects.Particles
{
    /// <summary>
    /// Classe de base de toutes les particules.
    /// Chaque classe de particule ne peut être héritée, et doit implémenter IPoolable.
    /// </summary>
    public abstract class Particle : IDisposable
    {
        public abstract void Draw(GameTime time, SpriteBatch batch, Vector2 scroll);
        public void Dispose() { }
    }
}
