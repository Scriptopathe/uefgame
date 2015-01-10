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
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using UeFGame.GameObjects;
using UeFGame.GameComponents;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Class representing the GameMap.
    /// It contains all the physics and logic of the map.
    /// </summary>
    public class GameMap
    {
        Vector2 MapDefaultGravity = Vector2.UnitY*15;
        /* --------------------------------------------------------------------------------
        * Variables
        * -------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The Object Pool of the map
        /// </summary>
        GameObjects.GameObjectPool m_objectPool;
        /// <summary>
        /// The physics world of the map
        /// </summary>
        World m_world;
        /// <summary>
        /// Size of the map in simulation units.
        /// </summary>
        Vector2 m_sizeSim;
        /// <summary>
        /// Scrolling of the map in pixels
        /// </summary>
        Vector2 m_scrollPx;
        /// <summary>
        /// Object kept in reference for scrolling.
        /// This object will always be at the center of the screen.
        /// </summary>
        GameObjects.GameObject m_scrollRef;
        /// <summary>
        /// Reference to the player.
        /// </summary>
        GameObjects.Player m_player;
        #endregion
        /* --------------------------------------------------------------------------------
        * Properties
        * -------------------------------------------------------------------------------*/
        #region Properties
        

        #endregion
        /* --------------------------------------------------------------------------------
        * Basics
        * -------------------------------------------------------------------------------*/
        #region Basics
        /// <summary>
        /// Constructor of the game map
        /// </summary>
        public GameMap()
        {
            m_world = new World(MapDefaultGravity);
            m_objectPool = new GameObjects.GameObjectPool(m_world);
            m_player = new Player(m_world);
            Globals.World = m_world;

            // TEST
            MapInitializingData data = new MapInitializingData();
            data.SimSize = new Vector2(sim(10000), sim(10000));
            LoadMap(data);
        }

        /* --------------------------------------------------------------------------------
        * Update
        * -------------------------------------------------------------------------------*/
        #region Update
        /// <summary>
        /// Updates the game map.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            m_world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            // Updates the GameObjects
            UpdateObjects(gameTime);
            // Updates the player
            m_player.Update(gameTime);
            // Updates the scrolling
            UpdateScrolling();
        }
        /// <summary>
        /// Update the game objects
        /// </summary>
        void UpdateObjects(GameTime gameTime)
        {
            List<GameObjects.GameObject> objs = m_objectPool.GetActive();
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i].IsDisposed)
                {
                    objs.RemoveAt(i);
                    i--;
                }
                else
                {
                    objs[i].Update(gameTime);
                }
            }
        }
        /// <summary>
        /// Updates the scrolling
        /// </summary>
        void UpdateScrolling()
        {
            if (m_scrollRef != null)
            {
                m_scrollPx.X = (int)Math.Max(0, Math.Min(disp(m_scrollRef.SimPosition.X) - Globals.ScreenWidth / 2, disp(m_sizeSim.X) - Globals.ScreenWidth));
                m_scrollPx.Y = (int)Math.Max(0, Math.Min(disp(m_scrollRef.SimPosition.Y) - Globals.ScreenHeight / 2, disp(m_sizeSim.Y) - Globals.ScreenHeight));
            }
        }
        #endregion

        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(SpriteBatch batch)
        {
            // Draws the game objects
            List<GameObjects.GameObject> objs = m_objectPool.GetActive();
            foreach (GameObjects.GameObject obj in objs)
            {
                obj.Draw(batch, m_scrollPx);
            }
            // Draws the player
            m_player.Draw(batch, m_scrollPx);
        }
        /// <summary>
        /// Disposes the map it can't be used after that.
        /// </summary>
        public void Dispose()
        {
            m_objectPool.Clear();
            m_player.Dispose();
        }

        #region Loading
        /// <summary>
        /// Resets the map before loading
        /// </summary>
        void ResetMap()
        {
            m_objectPool.Clear();
            m_world.Clear();
            m_world.ClearForces();
        }
        /// <summary>
        /// Loads a map.
        /// </summary>
        public void LoadMap(MapInitializingData data)
        {
            m_sizeSim = data.SimSize;
            m_scrollRef = m_player;
            // Test : to remove
            test_initGround();
        }
        /// <summary>
        /// TEST
        /// </summary>
        void test_initGround()
        {
            int l = 20;
            UniqueBodyGameObjectInitializingData init = new UniqueBodyGameObjectInitializingData();
            init.CollisionCategories = Category.Object | Category.Ground;
            init.ShapeType = ShapeTypes.Rectangle;
            init.SizePx = new Vector2(disp(m_sizeSim.X), l);
            init.TextureName = "Editor\\squareTex";
            init.FixtureOffset = new Vector2();
            init.Z = 1.0f;
            init.BodyType = BodyType.Static;
            init.Tone = Color.Green;
            init.UpperRightStartX = 0;
            init.UpperRightStartY = 0;
            init.BodyFriction = Constants.GroundFriction;
            m_objectPool.GenericGetFromPool<UniqueBodyGameObjectInitializingData, UniqueBodyGameObject>(init);
            init.UpperRightStartY = m_sizeSim.Y - sim(l);
            
            m_objectPool.GenericGetFromPool<UniqueBodyGameObjectInitializingData, UniqueBodyGameObject>(init);
            // Vertical
            init.SizePx = new Vector2(l, disp(m_sizeSim.Y));
            init.UpperRightStartX = 0;
            init.UpperRightStartY = 0;
            m_objectPool.GenericGetFromPool<UniqueBodyGameObjectInitializingData, UniqueBodyGameObject>(init);
            //
            init.UpperRightStartX = m_sizeSim.X - sim(l);
            m_objectPool.GenericGetFromPool<UniqueBodyGameObjectInitializingData, UniqueBodyGameObject>(init);


        }
        void test_initThings()
        {
            UniqueBodyGameObjectInitializingData init = new UniqueBodyGameObjectInitializingData();
            init.CollisionCategories = Category.Object | Category.Ground;
            init.ShapeType = ShapeTypes.Rectangle;
            init.TextureName = "Editor\\squareTex";
            init.FixtureOffset = new Vector2();
            init.Z = 1.0f;
            init.BodyType = BodyType.Static;
            init.Tone = Color.Green;
            init.UpperRightStartX = 0;
            init.UpperRightStartY = 0;
            init.BodyFriction = Constants.GroundFriction;
            init.Rotation = 0.4f;
            init.SizePx = new Vector2(400, 20);
            init.UpperRightStartX = m_sizeSim.X - sim(800);
            init.UpperRightStartY = m_sizeSim.Y - sim(200);
            m_objectPool.GenericGetFromPool<UniqueBodyGameObjectInitializingData, UniqueBodyGameObject>(init);
            for (int i = 0; i < Globals.Rand.Next(60) + 400; i++)
            {
                if (Globals.Rand.Next(10) == 0)
                    init.BodyType = BodyType.Dynamic;
                else
                    init.BodyType = BodyType.Static;
                init.Rotation = Globals.Rand.Next(314) / 100.0f;
                init.SizePx = new Vector2(Globals.Rand.Next(400) + 1, Globals.Rand.Next(400) + 1);
                init.UpperRightStartX = sim(Globals.Rand.Next((int)disp(m_sizeSim.X)) + 1);
                init.UpperRightStartY = sim(Globals.Rand.Next((int)disp(m_sizeSim.Y)) + 1);
                m_objectPool.GenericGetFromPool<UniqueBodyGameObjectInitializingData, UniqueBodyGameObject>(init);
            }

            // Test initialization
            init = new UniqueBodyGameObjectInitializingData();
            init.SimStartX = sim(500);
            init.SimStartY = sim(200);
            init.ShapeType = ShapeTypes.Rectangle;
            init.SizePx = new Vector2(50, 50);
            init.TextureName = "Editor\\squareTex";
            init.Tone = Color.Gray;
            init.Z = 1.0f;
            init.FixtureOffset = new Vector2();
            init.BodyType = BodyType.Dynamic;
            init.BodyFriction = Constants.EventFriction;
            UniqueBodyGameObject obj = m_objectPool.GenericGetFromPool<UniqueBodyGameObjectInitializingData,
                BasicEvent>(init);
        }
        /* ---------------------------------------------------------------
         * Utils
         * -------------------------------------------------------------*/
        #region Utils
        protected float sim(double v) { return ConvertUnits.ToSimUnits(v); }
        protected Vector2 sim(Vector2 v) { return ConvertUnits.ToSimUnits(v); }
        protected float disp(float v) { return ConvertUnits.ToDisplayUnits(v); }
        protected Vector2 disp(Vector2 v) { return ConvertUnits.ToDisplayUnits(v); }
        #endregion
        #endregion
        #endregion
    }
}
