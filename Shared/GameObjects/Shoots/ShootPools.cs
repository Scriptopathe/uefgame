using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameObjects.Shoots
{
    /// <summary>
    /// Classe servant à la gestion des pools de tirs.
    /// </summary>
    public static class ShootPools
    {
        /// <summary>
        /// Initialisation des tirs.
        /// </summary>
        public static void Initialize()
        {
            GatlingShoot.Pool = new GameObjectPool<GatlingShoot, GatlingShootInit>(10000);
        }
        /// <summary>
        /// Mise à jour des tirs.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        /// <param name="scroll"></param>
        public static void Update(GameTime time)
        {
            foreach (GatlingShoot part in GatlingShoot.Pool.GetActive())
            {
                part.Update(time);
            }
            GatlingShoot.Pool.Update();
        }
        /// <summary>
        /// Dessin des tirs.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        /// <param name="scroll"></param>
        public static void Draw(GameTime time, SpriteBatch batch, Vector2 scroll)
        {
            foreach (GatlingShoot part in GatlingShoot.Pool.GetActive())
            {
                part.Draw(time, batch, scroll);
            }
        }
        /// <summary>
        /// Libération des ressources allouées.
        /// </summary>
        public static void Dispose()
        {
            GatlingShoot.Pool.Dispose();
        }
    }
}
