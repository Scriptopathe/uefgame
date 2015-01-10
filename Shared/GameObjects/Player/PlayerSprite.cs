using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameObjects.Player
{
    public enum PlayerAnimationState
    {
        Running,
        Idle,
        Jumping,
        Falling,
        Shooting,
    }

    public enum PlayerDirection
    {
        Left,
        Right
    }
    /// <summary>
    /// Représente une séquence d'animation pour une position donnée et pour les jambes et corps de Géo.
    /// </summary>
    public class CharAnimationSequence
    {
        /// <summary>
        /// Rectangles sources des poses de dessin pour les jambes de Géo.
        /// </summary>
        public List<Rectangle> LegsSrcRects;
        /// <summary>
        /// Rectangles sources des poses de dessin pour le buste de Géo.
        /// </summary>
        public List<Rectangle> BustSrcRects;
        public int FramesCount = 2;
        public CharAnimationSequence() { }
        public CharAnimationSequence(List<Rectangle> legsSrcRects, List<Rectangle> bustSrcRect)
        {
            LegsSrcRects = legsSrcRects;
            BustSrcRects = bustSrcRect;
        }
    }
    /// <summary>
    /// Représente une séquence d'animation pour une position donnée et pour les jambes et corps de Géo.
    /// </summary>
    public class SmokeAnimationSequence
    {
        /// <summary>
        /// Rectangles sources des poses de dessin pour les jambes de Géo.
        /// </summary>
        public List<Rectangle> SrcRects;
        /// <summary>
        /// Rectangles sources des poses de dessin pour le buste de Géo.
        /// /!\ Cela doit être callé sur les offsets à placer par rapport aux frames de l'animation de Géo.
        /// </summary>
        public List<Vector2> Offsets;
        public int FramesCount = 2;
        public SmokeAnimationSequence() { }
        public SmokeAnimationSequence(List<Rectangle> srcRects, List<Vector2> offsets)
        {
            SrcRects = srcRects;
            Offsets = offsets;
        }
    }
    /// <summary>
    /// Gère l'animation du héros.
    /// </summary>
    public class PlayerSprite
    {
        /* -------------------------------------------------------------------------------------------------------------
         * Variables
         * -----------------------------------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Représente la liste de séquences d'animation indexées par l'état d'animation du joueur.
        /// </summary>
        Dictionary<PlayerAnimationState, CharAnimationSequence> m_geoAnimationSequences;
        /// <summary>
        /// Représente la liste de séquences d'animation de fumée indexées par l'état d'animation du joueur.
        /// </summary>
        Dictionary<PlayerAnimationState, SmokeAnimationSequence> m_smokeAnimationSequences;
        /// <summary>
        /// Texture source utilisée pour le dessin de l'animation.
        /// </summary>
        Texture2D m_animationTexture;
        /// <summary>
        /// Etat d'animation de la frame précédente.
        /// </summary>
        PlayerAnimationState m_oldState = PlayerAnimationState.Running;
        /// <summary>
        /// Frame actuelle de la séquence actuelle d'animation.
        /// </summary>
        int m_animationFrame;
        /// <summary>
        /// Frame actuelle de l'animation de la fumée.
        /// </summary>
        int m_smokeAnimationFrame;
        /// <summary>
        /// Direction à la frame précédente.
        /// </summary>
        PlayerDirection m_oldDir;
        #endregion
        /* -------------------------------------------------------------------------------------------------------------
         * Properties
         * -----------------------------------------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Représente la liste de séquences d'animation indexées par l'état d'animation du joueur.
        /// </summary>
        public Dictionary<PlayerAnimationState, CharAnimationSequence> GeoAnimationSequences
        {
            get { return m_geoAnimationSequences; }
            set { m_geoAnimationSequences = value; }
        }
        /// <summary>
        /// Représente la liste de séquences d'animation indexées par l'état d'animation de la fumée.
        /// </summary>
        public Dictionary<PlayerAnimationState, SmokeAnimationSequence> SmokeAnimationSequences
        {
            get { return m_smokeAnimationSequences; }
            set { m_smokeAnimationSequences = value; }
        }
        #endregion
        /* -------------------------------------------------------------------------------------------------------------
         * Setup
         * -----------------------------------------------------------------------------------------------------------*/
        #region Setup
        /// <summary>
        /// Crée les séquences d'animation de la fumée.
        /// </summary>
        void SetupSmokeAnimation()
        {
            SmokeAnimationSequences = new Dictionary<PlayerAnimationState, SmokeAnimationSequence>();

            // Running
            SmokeAnimationSequence sequence = new SmokeAnimationSequence();
            SmokeAnimationSequences.Add(PlayerAnimationState.Running, sequence);
            sequence.Offsets = new List<Vector2>() {
                new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -4), new Vector2(0, -7), new Vector2(0, -6), new Vector2(0, -4),
                new Vector2(0, -3), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -5), new Vector2(0, -4), new Vector2(0, -2)
            };
            sequence.SrcRects = new List<Rectangle>()
            {
                At(0, 2), At(1, 2), At(2, 2), At(3, 2)
            };

            // Idle
            sequence = new SmokeAnimationSequence();
            sequence.Offsets = new List<Vector2>() {
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)
            };
            sequence.SrcRects = new List<Rectangle>()
            {
                At(0, 2), At(1, 2), At(2, 2), At(3, 2)
            };
            sequence.FramesCount = 4;
            SmokeAnimationSequences.Add(PlayerAnimationState.Idle, sequence);
        }
        /// <summary>
        /// Crée les séquences d'animation de géo.
        /// </summary>
        void SetupGeoAnimation()
        {
            GeoAnimationSequences = new Dictionary<PlayerAnimationState, CharAnimationSequence>();

            // Running
            CharAnimationSequence sequence = new CharAnimationSequence();
            GeoAnimationSequences.Add(PlayerAnimationState.Running, sequence);
            sequence.BustSrcRects = new List<Rectangle>();
            for (int x = 0; x < 12; x++)
            {
                sequence.BustSrcRects.Add(At(x, 0));
            }
            sequence.LegsSrcRects = new List<Rectangle>();
            for (int x = 0; x < 12; x++)
            {
                sequence.LegsSrcRects.Add(At(x, 1));
            }
            // Idle
            sequence = new CharAnimationSequence();
            GeoAnimationSequences.Add(PlayerAnimationState.Idle, sequence);
            sequence.BustSrcRects = new List<Rectangle>();
            for (int x = 0; x < 4; x++)
            {
                sequence.BustSrcRects.Add(At(x, 3));
            }
            sequence.LegsSrcRects = new List<Rectangle>();
            for (int x = 0; x < 4; x++)
            {
                sequence.LegsSrcRects.Add(At(x, 4));
            }
            
        }
        #endregion
        /* -------------------------------------------------------------------------------------------------------------
         * Methods
         * -----------------------------------------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de PlayerAnimationManager avec la texture spécifiée comme source des frames.
        /// </summary>
        /// <param name="animationTexture"></param>
        public PlayerSprite(Texture2D animationTexture)
        {
            m_animationTexture = animationTexture;
            SetupSmokeAnimation();
            SetupGeoAnimation();
        }
        /* -------------------------------------------------------------------------------------------------------------
        * Draw
        * -----------------------------------------------------------------------------------------------------------*/
        #region Draw
        /// <summary>
        /// Dessine l'animation du joueur.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="position"></param>
        /// <param name="dstRect"></param>
        public void DrawAnimation(SpriteBatch batch, Rectangle dstRect, PlayerAnimationState state, PlayerDirection direction,
            float rotation, Vector2 origin, float layerDepth)
        {
            UpdateAnimationFrames(state, direction);
            CharAnimationSequence seq = GeoAnimationSequences[state];
            int frames = seq.FramesCount;
            SpriteEffects effect = direction == PlayerDirection.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            // Dessine les jambes
            batch.Draw(m_animationTexture, dstRect, seq.LegsSrcRects[m_animationFrame/frames], Color.White, rotation, origin, effect, layerDepth);
            // Dessine le corps
            batch.Draw(m_animationTexture, dstRect, seq.BustSrcRects[m_animationFrame/frames], Color.White, rotation, origin, effect, layerDepth+0.01f);
            // Dessine la fumée
            int sign = direction == PlayerDirection.Right ? 1 : -1;
            dstRect.X += sign * (int)SmokeAnimationSequences[state].Offsets[m_animationFrame/frames].X;
            dstRect.Y += (int)SmokeAnimationSequences[state].Offsets[m_animationFrame/frames].Y;

            SmokeAnimationSequence smokeSeq = SmokeAnimationSequences[state];
            batch.Draw(m_animationTexture, dstRect, smokeSeq.SrcRects[m_smokeAnimationFrame/smokeSeq.FramesCount], Color.White, rotation, origin, effect, layerDepth);
        }

        /// <summary>
        /// Permet le dessin de l'animation en relation avec plusieurs paramètres.
        /// </summary>
        void Draw(SpriteBatch batch,
                    PlayerAnimationState state,
                    PlayerDirection direction,
                    Vector2 realPosition, float rotation,
                    Vector2 scrollPx, Vector2 centerOffset,
                    Vector2 shapeSize, float layerDepth)
        {
            if (m_animationTexture == null)
                return;

            Rectangle destRect = new Rectangle();

            // Ajustements faits en fonction de la direction.
            if(direction == PlayerDirection.Right)
                destRect.X = (int)disp(realPosition.X) - (int)scrollPx.X+12;
            else
                destRect.X = (int)disp(realPosition.X) - (int)scrollPx.X;

            // Ajustement de la hauteur.
            destRect.Y = (int)disp(realPosition.Y) - (int)scrollPx.Y-29;

            // Largeur et hauteur constantes.
            destRect.Width = 96;
            destRect.Height = 96;
            // Origine : milieu de la texture pour les rotations.
            var origin = new Vector2(96/2, 96/2);
            DrawAnimation(batch, destRect, state, direction, rotation, origin, layerDepth);
        }
        /// <summary>
        /// Draws the sprite of the given GameEvent.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        public void Draw(SpriteBatch batch, Player evt, Vector2 scrollPx, float layerDepth)
        {
            Draw(batch, evt.AnimationState, evt.Direction, evt.Body.Position, evt.Body.Rotation, scrollPx, evt.MPhysicalObject.CenterOffset, evt.MPhysicalObject.ShapeSizeSim, layerDepth);
        }

        #endregion
        /* -------------------------------------------------------------------------------------------------------------
         * Animation
         * -----------------------------------------------------------------------------------------------------------*/
        #region Animation
        /// <summary>
        /// Mets à jour le numéro de la frame de l'animation.
        /// </summary>
        void UpdateAnimationFrames(PlayerAnimationState state, PlayerDirection dir)
        {
            // Si changement d'état, on revient au début de l'animation.
            if (m_oldState != state || dir != m_oldDir)
                m_animationFrame = 0;
            else
            {
                m_animationFrame++;
                m_animationFrame %= GeoAnimationSequences[state].LegsSrcRects.Count * GeoAnimationSequences[state].FramesCount;
            }

            // Animation de fumée continue.
            m_smokeAnimationFrame++;
            m_smokeAnimationFrame %= SmokeAnimationSequences[state].SrcRects.Count * SmokeAnimationSequences[state].FramesCount;

            m_oldDir = dir;
            m_oldState = state;
        }
        #endregion
        #endregion


        #region Utils
        /// <summary>
        /// Returns a texture relative coordinate from a body relative coordinate.
        /// Ex :     m_size.X   -> m_texture.Width
        ///    :     m_size.X/2 -> m_texture.Width/2
        /// </summary>  
        /// <param name="bodyRelative">body relative value to convert</param>
        /// <param name="horiz">true if the given data is width related</param>
        /// <returns></returns>
        protected float texRelative(float bodyRelative, Vector2 shapeSize, bool horiz)
        {
            if (horiz)
                return bodyRelative * 96 / disp(shapeSize.X);
            else
                return bodyRelative * 96 / disp(shapeSize.Y);
        }
        /// <summary>
        /// Retourne le rectangle tile à la position en tiles spécifiée.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        Rectangle At(int x, int y)
        {
            return new Rectangle(x * 96, y * 96, 96, 96);
        }
        protected float sim(double v) { return ConvertUnits.ToSimUnits(v); }
        protected Vector2 sim(Vector2 v) { return ConvertUnits.ToSimUnits(v); }
        protected float disp(float v) { return ConvertUnits.ToDisplayUnits(v); }
        protected Vector2 disp(Vector2 v) { return ConvertUnits.ToDisplayUnits(v); }
        #endregion
    }
}
