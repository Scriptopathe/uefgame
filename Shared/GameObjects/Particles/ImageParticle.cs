using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameObjects.Particles
{
    public class ImageParticleInit
    {
        /// <summary>
        /// Position x en pixels de la particule relative au début de la map.
        /// </summary>
        public int PxX;
        /// <summary>
        /// Position y en pixels de la particule relative au début de la map.
        /// </summary>
        public int PxY;
        /// <summary>
        /// Nom de la texture.
        /// </summary>
        public string TextureName;
        /// <summary>
        /// Angle de dessin.
        /// </summary>
        public float Angle = 0.0f;
        /// <summary>
        /// Alpha de la texture à la première frame..
        /// </summary>
        public byte BaseAlpha = 255;
        /// <summary>
        /// Durée de vie de la particule en millisecondes.
        /// </summary>
        public int Lifetime = 1000;
        /// <summary>
        /// Durée du fadeout de la particule en millisecondes.
        /// Si -1, aucun fadeout.
        /// Sinon, le fadeout intervient juste avant la fin de la durée de vie.
        /// </summary>
        public int FadeOutTime = -1;
        /// <summary>
        /// Vélocité de la particule.
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// Retourne la vitesse angulaire de l'image en rad/s.
        /// </summary>
        public float AngleVelocity;
        /// <summary>
        /// Profondeur.
        /// </summary>
        public float LayerDepth = 0.10f;
    }
    /// <summary>
    /// Particle affichant simplement une image.
    /// </summary>
    public sealed class ImageParticle : Particle, IPoolable<ImageParticleInit>
    {
        #region Pool
        /// <summary>
        /// Pool d'instance de ImageParticle.
        /// /!\ Ne pas initialiser paraisseusement.
        /// </summary>
        public static GameObjectPool<ImageParticle, ImageParticleInit> Pool;
        #endregion

        #region Variables
        ImageParticleInit m_initData;
        Texture2D m_texture;
        Vector2 m_position;
        int m_ellapsedMilliseconds;
        byte m_alpha;
        float m_angle;
        #endregion

        #region Particle
        /// <summary>
        /// Dessine la particule.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        /// <param name="scroll"></param>
        public override void Draw(GameTime time, SpriteBatch batch, Vector2 scroll)
        {
            UpdateLogic(time);

            int displayPosX = (int)m_position.X - (int)scroll.X;
            int displayPosY = (int)m_position.Y - (int)scroll.Y;

            batch.Draw(m_texture,
                new Rectangle(displayPosX-m_texture.Width/2, displayPosY-m_texture.Height/2, m_texture.Width, m_texture.Height),
                null,
                new Color(255, 255, 255, m_alpha),
                m_angle,
                new Vector2(m_texture.Width / 2, m_texture.Height / 2),
                SpriteEffects.None,
                m_initData.LayerDepth);

        }
        /// <summary>
        /// Logique de la particule.
        /// </summary>
        void UpdateLogic(GameTime time)
        {
            // Logique de la particule.
            m_position += m_initData.Velocity;
            m_ellapsedMilliseconds += (int)time.ElapsedGameTime.TotalMilliseconds;
            // Calcul de l'alpha.
            if (m_ellapsedMilliseconds >= m_initData.Lifetime - m_initData.FadeOutTime)
            {
                // On commence le fadeout
                // On fait une interpolation linéaire de l'alpha.
                float startFadeout = m_initData.Lifetime - m_initData.FadeOutTime;
                float endFadeout = m_initData.Lifetime;
                float max = m_initData.BaseAlpha;
                float min = 0;

                float coef = (max - min) / (startFadeout - endFadeout);
                int newAlpha = (int)max + (int)(coef * (m_ellapsedMilliseconds - startFadeout));
                if (newAlpha < 0)
                    newAlpha = 0;
                m_alpha = (byte)newAlpha;
            }
            
            m_angle += m_initData.AngleVelocity * m_ellapsedMilliseconds / 1000;

            if (m_ellapsedMilliseconds > m_initData.Lifetime)
            {
                ImageParticle.Pool.Deactivate(this);
            }
        }
        #endregion

        #region IPoolable
        /// <summary>
        /// Vaut vraie si la particule est encore active dans la pool.
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }
        /// <summary>
        /// Désactive l'instance pour la remettre dans la pool.
        /// </summary>
        public void Deactivate()
        {

        }
        /// <summary>
        /// Initialise la particule.
        /// </summary>
        /// <param name="?"></param>
        public void Initialize(ImageParticleInit init)
        {
            m_initData = init;
            m_alpha = m_initData.BaseAlpha;
            m_texture = Globals.Content.Load<Texture2D>(m_initData.TextureName);
            m_angle = m_initData.Angle;
            m_position.X = init.PxX;
            m_position.Y = init.PxY;
            m_ellapsedMilliseconds = 0;
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose l'instance définitivement.
        /// </summary>
        public void Dispose()
        {

        }
        /// <summary>
        /// Dispose l'instance définitivement.
        /// </summary>
        public void Dispose(bool disposing)
        {
            Dispose();
        }
        #endregion
    }
}
