using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace UeFGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class UeFClass : Microsoft.Xna.Framework.Game
    {
        /* --------------------------------------------------------------------------------
        * Variables
        * -------------------------------------------------------------------------------*/
        #region Variables
        GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;
        /// <summary>
        /// The current Scene.
        /// </summary>
        public Scenes.IScene Scene { get; set; }
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static UeFClass Instance;
        /// <summary>
        /// Returns the content instance of the singleton.
        /// </summary>
        public static ContentManager GetContent
        {
            get
            {
                return Instance.Content;
            }
        }
        #endregion

        public UeFClass()
        {
            Instance = this;
            LoadConfig();
            SetupGraphicsParameters();
            SetupContentPaths();
            Globals.Content = Content;
        }

        /// <summary>
        /// Sets up the graphical parameters : screen resolution etc...
        /// </summary>
        void SetupGraphicsParameters()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_graphics.PreferredBackBufferWidth = Globals.Cfg.Resolution.Width;
            m_graphics.PreferredBackBufferHeight = Globals.Cfg.Resolution.Height;
            Globals.ScreenWidth = m_graphics.PreferredBackBufferWidth;
            Globals.ScreenHeight = m_graphics.PreferredBackBufferHeight;
        }
        /// <summary>
        /// Loads the configuration from config file.
        /// </summary>
        void LoadConfig()
        {
            try
            {

                Globals.Cfg = Config.LoadFromFile(AppDomain.CurrentDomain.BaseDirectory + "Content\\config.xml");
            }
            catch
            {
                Globals.Cfg = new Config();
            }

        }

        /// <summary>
        /// Indicates the content repertory path to ressources managers.
        /// </summary>
        void SetupContentPaths()
        {

            Content.RootDirectory = "Content";
            Ressources.FileRessourceProvider.ContentDir = "Content\\";
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Scene = new Scenes.SceneGame();
            UeFGame.Input.ModuleInit();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            DrawingRoutines.InitTextures();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            Scene.Update(gameTime);
            UeFGame.Input.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            Scene.Draw(gameTime, m_spriteBatch);
            base.Draw(gameTime);
        }
    }
}
