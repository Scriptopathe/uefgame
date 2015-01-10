using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using UeFGame.GameComponents;
namespace UeFGame.Scenes
{
    public class SceneGame : IScene
    {
        /* --------------------------------------------------------------------------------
        * Variables
        * -------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The map of this scene
        /// </summary>
        GameMap m_map;

        #endregion
        /* --------------------------------------------------------------------------------
        * Properties
        * -------------------------------------------------------------------------------*/
        #region Properties
        public bool Bloom
        {
            get;
            set;
        }
        #endregion
        /* --------------------------------------------------------------------------------
        * Basics / Scene Implementation
        * -------------------------------------------------------------------------------*/
        #region Basics / Scene Implementation
        /// <summary>
        /// Initializes the Scene.
        /// </summary>
        public SceneGame()
        {
            UeFGame.GameDatabase.Load();
            m_map = new GameMap();
            InitializeGraphics();
            Bloom = true;
        }
        /// <summary>
        /// Initialize some graphics components.
        /// </summary>
        void InitializeGraphics()
        {
            m_mainRenderTarget = new RenderTarget2D(UeFClass.Instance.GraphicsDevice, Globals.ScreenWidth, Globals.ScreenHeight);
            m_tempRenderTarget = new RenderTarget2D(UeFClass.Instance.GraphicsDevice, Globals.ScreenWidth, Globals.ScreenHeight);
            m_mapRenderTarget = new RenderTarget2D(UeFClass.Instance.GraphicsDevice, Globals.ScreenWidth, Globals.ScreenHeight);
            m_bloomExtract = Globals.Content.Load<Effect>("Shaders\\BloomExtract");
            m_blurX = Globals.Content.Load<Effect>("Shaders\\GaussianBlurX");
            m_blurY = Globals.Content.Load<Effect>("Shaders\\GaussianBlurY");
            m_combine = Globals.Content.Load<Effect>("Shaders\\Combine");
            m_test = Globals.Content.Load<Effect>("Shaders\\test");
            m_test2 = Globals.Content.Load<Effect>("Shaders\\test2");
        }
        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            m_map.Update(gameTime);
        }

        #region Shader Params
        /// <summary>
        /// Main render target.
        /// </summary>
        RenderTarget2D m_mainRenderTarget;
        RenderTarget2D m_mapRenderTarget;
        RenderTarget2D m_tempRenderTarget;
        /// <summary>
        /// Pixel shader.
        /// </summary>
        Effect m_bloomExtract;
        Effect m_blurX;
        Effect m_blurY;
        Effect m_combine;
        Effect m_test;
        Effect m_test2;
        #endregion
        /// <summary>
        /// Draws the scene.
        /// The sprite batch Begin() and End() functions must be called in this method.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(GameTime time, SpriteBatch batch)
        {
            GraphicsDevice gfx = UeFClass.Instance.GraphicsDevice;
            if (Bloom)
            {
                // Renders the map
                gfx.SetRenderTarget(m_mapRenderTarget);
                m_map.Draw(time, batch);

                // Extract bloom
                gfx.SetRenderTarget(m_tempRenderTarget);
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                m_bloomExtract.CurrentTechnique.Passes[0].Apply();
                batch.Draw(m_mapRenderTarget, new Vector2(0, 0), Color.White);
                batch.End();
                // Flou
                gfx.SetRenderTarget(m_mainRenderTarget);
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                m_blurX.CurrentTechnique.Passes[0].Apply();
                batch.Draw(m_tempRenderTarget, new Vector2(0, 0), Color.White);
                batch.End();
                // Flou
                gfx.SetRenderTarget(m_tempRenderTarget);
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                m_blurY.CurrentTechnique.Passes[0].Apply();
                batch.Draw(m_mainRenderTarget, new Vector2(0, 0), Color.White);
                batch.End();

                // Final
                gfx.SetRenderTarget(null);
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                gfx.Textures[11] = m_mapRenderTarget;
                UeFClass.Instance.GraphicsDevice.SamplerStates[11] = SamplerState.LinearClamp;

                m_combine.CurrentTechnique.Passes[0].Apply();
                batch.Draw(m_tempRenderTarget, new Vector2(0, 0), Color.White);
                batch.End();

            }
            else
            {
                // Renders the map
                gfx.SetRenderTarget(null);
                m_map.Draw(time, batch);
            }
        }
        /// <summary>
        /// Disposes the scene
        /// </summary>
        public void Dispose()
        {
            m_map.Dispose();
        }
        #endregion
    }
}
