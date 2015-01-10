using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameObjects.Particles
{
    /// <summary>
    /// Classe servant à la gestion des pools de particules.
    /// </summary>
    public static class ParticlePools
    {
        /// <summary>
        /// Initialisation des particules.
        /// </summary>
        public static void Initialize()
        {
            ImageParticle.Pool = new GameObjectPool<ImageParticle, ImageParticleInit>(20000);
        }
        /// <summary>
        /// Mise à jour des particules.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        /// <param name="scroll"></param>
        public static void Update(GameTime time, SpriteBatch batch, Vector2 scroll)
        {
            ImageParticle.Pool.Update();
            foreach (ImageParticle part in ImageParticle.Pool.GetActive())
            {
                part.Draw(time, batch, scroll);
            }
        }
        /// <summary>
        /// Libération des ressources allouées.
        /// </summary>
        public static void Dispose()
        {
            ImageParticle.Pool.Dispose();
        }
    }
}
